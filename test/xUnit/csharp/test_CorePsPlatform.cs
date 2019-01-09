// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using Xunit;
using Xunit.SkippableFact;

namespace PSTests.Parallel
{
    public static class PlatformTests
    {
        [Fact]
        public static void TestIsCoreCLR()
        {
            Assert.True(Platform.IsCoreCLR);
        }

        [SkippableFact]
        public static void TestGetUserName()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Unix);

            var startInfo = new ProcessStartInfo
            {
                FileName = @"/usr/bin/env",
                Arguments = "whoami",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            using (Process process = Process.Start(startInfo))
            {
                // Get output of call to whoami without trailing newline
                string username = process.StandardOutput.ReadToEnd().Trim();
                process.WaitForExit();

                // The process should return an exit code of 0 on success
                Assert.Equal(0, process.ExitCode);
                // It should be the same as what our platform code returns
                Assert.Equal(username, Platform.Unix.UserName());
            }
        }

        [SkippableFact]
        public static void TestGetMachineName()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Unix);

            var startInfo = new ProcessStartInfo
            {
                FileName = @"/usr/bin/env",
                Arguments = "hostname",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            using (Process process = Process.Start(startInfo))
            {
                 // Get output of call to hostname without trailing newline
                string hostname = process.StandardOutput.ReadToEnd().Trim();
                process.WaitForExit();

                // The process should return an exit code of 0 on success
                Assert.Equal(0, process.ExitCode);
                // It should be the same as what our platform code returns
                Assert.Equal(hostname, Environment.MachineName);
            }
        }

        [SkippableFact]
        public static void TestGetFQDN()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Unix);

            var startInfo = new ProcessStartInfo
            {
                FileName = @"/usr/bin/env",
                Arguments = "hostname --fqdn",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            using (Process process = Process.Start(startInfo))
            {
                 // Get output of call to hostname without trailing newline
                string hostname = process.StandardOutput.ReadToEnd().Trim();
                process.WaitForExit();

                // The process should return an exit code of 0 on success
                Assert.Equal(0, process.ExitCode);
                // It should be the same as what our platform code returns
                Assert.Equal(hostname, Platform.NonWindowsGetHostName());
            }
        }

        [SkippableFact]
        public static void TestIsExecutable()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Unix);
            Assert.True(Platform.NonWindowsIsExecutable("/bin/ls"));
        }

        [SkippableFact]
        public static void TestIsNotExecutable()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Unix);
            Assert.False(Platform.NonWindowsIsExecutable("/etc/hosts"));
        }

        [SkippableFact]
        public static void TestDirectoryIsNotExecutable()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Unix);
            Assert.False(Platform.NonWindowsIsExecutable("/etc"));
        }

        [SkippableFact]
        public static void TestFileIsNotHardLink()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Unix);

            string path = @"/tmp/nothardlink";
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.Create(path);

            FileSystemInfo fd = new FileInfo(path);

            // Since this is the only reference to the file, it is not considered a
            // hardlink by our API (though all files are hardlinks on Linux)
            Assert.False(Platform.NonWindowsIsHardLink(fd));

            File.Delete(path);
        }

        [SkippableFact]
        public static void TestFileIsHardLink()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Unix);

            string path = @"/tmp/originallink";
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.Create(path);

            string link = "/tmp/newlink";

            if (File.Exists(link))
            {
                File.Delete(link);
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = @"/usr/bin/env",
                Arguments = "ln " + path + " " + link,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
                Assert.Equal(0, process.ExitCode);
            }

            // Since there are now two references to the file, both are considered
            // hardlinks by our API (though all files are hardlinks on Linux)
            FileSystemInfo fd = new FileInfo(path);
            Assert.True(Platform.NonWindowsIsHardLink(fd));

            fd = new FileInfo(link);
            Assert.True(Platform.NonWindowsIsHardLink(fd));

            File.Delete(path);
            File.Delete(link);
        }

        [SkippableFact]
        public static void TestDirectoryIsNotHardLink()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Unix);

            string path = @"/tmp";

            FileSystemInfo fd = new FileInfo(path);

            Assert.False(Platform.NonWindowsIsHardLink(fd));
        }

        [SkippableFact]
        public static void TestNonExistentIsHardLink()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Unix);

            // A file that should *never* exist on a test machine:
            string path = @"/tmp/ThisFileShouldNotExistOnTestMachines";

            // If the file exists, then there's a larger issue that needs to be looked at
            Assert.False(File.Exists(path));

            // Convert `path` string to FileSystemInfo data type. And now, it should return true
            FileSystemInfo fd = new FileInfo(path);
            Assert.False(Platform.NonWindowsIsHardLink(fd));
        }

        [SkippableFact]
        public static void TestFileIsSymLink()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Unix);

            string path = @"/tmp/originallink";
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.Create(path);

            string link = "/tmp/newlink";

            if (File.Exists(link))
            {
                File.Delete(link);
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = @"/usr/bin/env",
                Arguments = "ln -s " + path + " " + link,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
                Assert.Equal(0, process.ExitCode);
            }

            FileSystemInfo fd = new FileInfo(path);
            Assert.False(Platform.NonWindowsIsSymLink(fd));

            fd = new FileInfo(link);
            Assert.True(Platform.NonWindowsIsSymLink(fd));

            File.Delete(path);
            File.Delete(link);
        }
    }
}
