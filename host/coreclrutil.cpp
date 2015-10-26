#include "coreclrutil.h"
#include <dirent.h>
#include <dlfcn.h>
#include <assert.h>
#include <string.h>
#include <sys/stat.h>
#include <unistd.h>
#include <cstdlib>
#include <iostream>
#include <set>
#include <unicode/unistr.h>

// The name of the CoreCLR native runtime DLL
#if defined(__APPLE__)
constexpr char coreClrDll[] = "libcoreclr.dylib";
#else
constexpr char coreClrDll[] = "libcoreclr.so";
#endif

void* coreclrLib;

// Prototype of the coreclr_initialize function from the libcoreclr.so
typedef int (*InitializeCoreCLRFunction)(
    const char* exePath,
    const char* appDomainFriendlyName,
    int propertyCount,
    const char** propertyKeys,
    const char** propertyValues,
    void** hostHandle,
    unsigned int* domainId);

// Prototype of the coreclr_shutdown function from the libcoreclr.so
typedef int (*ShutdownCoreCLRFunction)(
    void* hostHandle,
    unsigned int domainId);

InitializeCoreCLRFunction initializeCoreCLR;
ShutdownCoreCLRFunction shutdownCoreCLR;
ExecuteAssemblyFunction executeAssembly;
CreateDelegateFunction createDelegate;

//
// Below is from unixcoreruncommon/coreruncommon.cpp
//

bool GetAbsolutePath(const char* path, std::string& absolutePath)
{
    char realPath[PATH_MAX];
    if (realpath(path, realPath) != nullptr && realPath[0] != '\0')
    {
        absolutePath.assign(realPath);
        // realpath should return canonicalized path without the trailing slash
        assert(absolutePath.back() != '/');
        return true;
    }

    std::cerr << "failed to get absolute path for " << path << std::endl;
    return false;
}

// TODO use dirname
bool GetDirectory(const char* absolutePath, std::string& directory)
{
    directory.assign(absolutePath);
    size_t lastSlash = directory.rfind('/');
    if (lastSlash != std::string::npos)
    {
        directory.erase(lastSlash);
        return true;
    }

    std::cerr << "failed to get directory for " << absolutePath << std::endl;
    return false;
}

//
// Get the absolute path given the environment variable.
//
// Return true in case of a success, false otherwise.
//
bool GetEnvAbsolutePath(const char* env_var, std::string& absolutePath)
{
    const char* filesPathLocal = std::getenv(env_var);;
    if (filesPathLocal == nullptr)
    {
        std::cerr << "failed because $" << env_var << " was empty" << std::endl;
        return false;
    }

    if (!GetAbsolutePath(filesPathLocal, absolutePath))
    {
        std::cerr << "failed to get absolute path for " << env_var << std::endl;
        return false;
    }

    return true;
}

// Add all *.dll, *.ni.dll, *.exe, and *.ni.exe files from the specified directory to the tpaList string.
void AddFilesFromDirectoryToTpaList(const char* directory, std::string& tpaList)
{
    const char * const tpaExtensions[] = {
        ".ni.dll",      // Probe for .ni.dll first so that it's preferred if ni and il coexist in the same dir
        ".dll",
        ".ni.exe",
        ".exe",
    };

    DIR* dir = opendir(directory);
    if (dir == nullptr)
    {
        return;
    }

    std::set<std::string> addedAssemblies;

    // Walk the directory for each extension separately so that we first get files with .ni.dll extension,
    // then files with .dll extension, etc.
    for (unsigned int extIndex = 0; extIndex < sizeof(tpaExtensions) / sizeof(tpaExtensions[0]); extIndex++)
    {
        const char* ext = tpaExtensions[extIndex];
        int extLength = strlen(ext);

        struct dirent* entry;

        // For all entries in the directory
        while ((entry = readdir(dir)) != nullptr)
        {
            // We are interested in files only
            switch (entry->d_type)
            {
            case DT_REG:
                break;

                // Handle symlinks and file systems that do not support d_type
            case DT_LNK:
            case DT_UNKNOWN:
            {
                std::string fullFilename;

                fullFilename.append(directory);
                fullFilename.append("/");
                fullFilename.append(entry->d_name);

                struct stat sb;
                if (stat(fullFilename.c_str(), &sb) == -1)
                {
                    continue;
                }

                if (!S_ISREG(sb.st_mode))
                {
                    continue;
                }
            }
            break;

            default:
                continue;
            }

            std::string filename(entry->d_name);

            // Check if the extension matches the one we are looking for
            int extPos = filename.length() - extLength;
            if ((extPos <= 0) || (filename.compare(extPos, extLength, ext) != 0))
            {
                continue;
            }

            std::string filenameWithoutExt(filename.substr(0, extPos));

            // Make sure if we have an assembly with multiple extensions present,
            // we insert only one version of it.
            if (addedAssemblies.find(filenameWithoutExt) == addedAssemblies.end())
            {
                addedAssemblies.insert(filenameWithoutExt);

                tpaList.append(directory);
                tpaList.append("/");
                tpaList.append(filename);
                tpaList.append(":");
            }
        }

        // Rewind the directory stream to be able to iterate over it for the next extension
        rewinddir(dir);
    }

    closedir(dir);
}

//
// Below is our custom start/stop interface
//
int startCoreCLR(
    const char* appDomainFriendlyName,
    void** hostHandle,
    unsigned int* domainId)
{
    // get the CoreCLR root path
    std::string clrAbsolutePath;
    if(!GetEnvAbsolutePath("CORE_ROOT", clrAbsolutePath))
    {
        return -1;
    }

    // get the CoreCLR shared library path
    std::string coreClrDllPath(clrAbsolutePath);
    coreClrDllPath.append("/");
    coreClrDllPath.append(coreClrDll);

    // open the shared library
    coreclrLib = dlopen(coreClrDllPath.c_str(), RTLD_NOW|RTLD_LOCAL);
    if (coreclrLib == nullptr)
    {
        char* error = dlerror();
        std::cerr << "dlopen failed to open the CoreCLR library: " << error << std::endl;
        return -1;
    }

    // query and verify the function pointers
    initializeCoreCLR = (InitializeCoreCLRFunction)dlsym(coreclrLib,"coreclr_initialize");
    shutdownCoreCLR = (ShutdownCoreCLRFunction)dlsym(coreclrLib,"coreclr_shutdown");
    executeAssembly = (ExecuteAssemblyFunction)dlsym(coreclrLib,"coreclr_execute_assembly");
    createDelegate = (CreateDelegateFunction)dlsym(coreclrLib,"coreclr_create_delegate");

    if (initializeCoreCLR == nullptr)
    {
        std::cerr << "function coreclr_initialize not found in CoreCLR library" << std::endl;
        return -1;
    }
    if (executeAssembly == nullptr)
    {
        std::cerr << "function coreclr_execute_assembly not found in CoreCLR library" << std::endl;
        return -1;
    }
    if (shutdownCoreCLR == nullptr)
    {
        std::cerr << "function coreclr_shutdown not found in CoreCLR library" << std::endl;
        return -1;
    }
    if (createDelegate == nullptr)
    {
        std::cerr << "function coreclr_create_delegate not found in CoreCLR library" << std::endl;
        return -1;
    }

    // generate the Trusted Platform Assemblies list
    std::string tpaList;

    // add assemblies in the CoreCLR root path
    AddFilesFromDirectoryToTpaList(clrAbsolutePath.c_str(), tpaList);

    // get path to AssemblyLoadContext.dll
    std::string psAbsolutePath;
    if(!GetEnvAbsolutePath("PWRSH_ROOT", psAbsolutePath))
    {
        return -1;
    }
    std::string alcAbsolutePath(psAbsolutePath);
    alcAbsolutePath.append("/");
    alcAbsolutePath.append("Microsoft.PowerShell.CoreCLR.AssemblyLoadContext.dll");

    // add AssemblyLoadContext
    tpaList.append(alcAbsolutePath);

    // generate the assembly search paths
    std::string appPath(psAbsolutePath);
    // TODO: add LD_LIBRARY_PATH?

    std::string nativeDllSearchDirs(appPath);
    nativeDllSearchDirs.append(":");
    nativeDllSearchDirs.append(clrAbsolutePath);

    // create list of properties to initialize CoreCLR
    const char* propertyKeys[] = {
        "TRUSTED_PLATFORM_ASSEMBLIES",
        "APP_PATHS",
        "APP_NI_PATHS",
        "NATIVE_DLL_SEARCH_DIRECTORIES",
        "AppDomainCompatSwitch"
    };

    const char* propertyValues[] = {
        tpaList.c_str(),
        appPath.c_str(),
        appPath.c_str(),
        nativeDllSearchDirs.c_str(),
        "UseLatestBehaviorWhenTFMNotSpecified"
    };

    // get path to current executable
    char exePath[PATH_MAX] = { 0 };
    readlink("/proc/self/exe", exePath, PATH_MAX);

    // initialize CoreCLR
    int status = initializeCoreCLR(
        exePath,
        appDomainFriendlyName,
        sizeof(propertyKeys)/sizeof(propertyKeys[0]),
        propertyKeys,
        propertyValues,
        hostHandle,
        domainId);

    if (!SUCCEEDED(status))
    {
        std::cerr << "coreclr_initialize failed - status: " << std::hex << status << std::endl;
        return -1;
    }

    // initialize PowerShell's custom assembly load context
    typedef void (*LoaderRunHelperFp)(const char16_t* appPath);
    LoaderRunHelperFp loaderDelegate = nullptr;
    status = createDelegate(
        *hostHandle,
        *domainId,
        "Microsoft.PowerShell.CoreCLR.AssemblyLoadContext, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
        "System.Management.Automation.PowerShellAssemblyLoadContextInitializer",
        "SetPowerShellAssemblyLoadContext",
        (void**)&loaderDelegate);

    if (!SUCCEEDED(status))
    {
        std::cerr << "could not create delegate for SetPowerShellAssemblyLoadContext - status: " << std::hex << status << std::endl;
        return -1;
    }

    icu::UnicodeString psUnicodeAbsolutePath = icu::UnicodeString::fromUTF8(psAbsolutePath.c_str());

    loaderDelegate((const char16_t*)psUnicodeAbsolutePath.getTerminatedBuffer());

    return status;
}

int stopCoreCLR(void* hostHandle, unsigned int domainId)
{
    // shutdown CoreCLR
    int status = shutdownCoreCLR(hostHandle, domainId);
    if (!SUCCEEDED(status))
    {
        std::cerr << "coreclr_shutdown failed - status: " << std::hex << status << std::endl;
    }

    // close the dynamic library
    if (0 != dlclose(coreclrLib))
    {
        std::cerr << "failed to close CoreCLR library" << std::endl;
    }

    return status;
}
