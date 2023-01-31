#
#  Copyright (c) Microsoft Corporation.
#  Licensed under the Apache License, Version 2.0 (the "License");
#  you may not use this file except in compliance with the License.
#  You may obtain a copy of the License at
#  https://www.apache.org/licenses/LICENSE-2.0
#
#  Unless required by applicable law or agreed to in writing, software
#  distributed under the License is distributed on an "AS IS" BASIS,
#  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
#  See the License for the specific language governing permissions and
#  limitations under the License.
#
# ------------------ PackageManagement Test  -----------------------------------


Describe "PackageManagement Acceptance Test" -Tags "Feature" {

    BeforeAll{
        $packageName = "jQuery"
        $gallery = "https://www.powershellgallery.com/api/v2"
        $source = 'OneGetTestSource'
        $localSourceName = [guid]::NewGuid().ToString("N")
        $nupkgPath = Join-Path $PSScriptRoot assets
        Register-PackageSource -Name $localSourceName -provider NuGet -Location $nupkgPath -Force -Trusted

        $packageSource = Get-PackageSource -Location $gallery -ErrorAction SilentlyContinue
        if ($packageSource) {
            $source = $packageSource.Name
            Set-PackageSource -Name $source -Trusted
        } else {
            Register-PackageSource -Name $source -Location $gallery -ProviderName 'PowerShellGet' -Trusted -ErrorAction SilentlyContinue
        }

        $SavedProgressPreference = $ProgressPreference
        $ProgressPreference = "SilentlyContinue"
    }

    AfterAll {
        Unregister-PackageSource -Name $localSourceName -Force -ErrorAction Ignore
        $ProgressPreference = $SavedProgressPreference
    }

    It "Get-PackageProvider" {
        $gpp = Get-PackageProvider
        $gpp.Name | Should -Contain 'NuGet'
        $gpp.Name | Should -Contain 'PowerShellGet'
    }

    It "Find-PackageProvider PowerShellGet" {
        $fpp = (Find-PackageProvider -Name "PowerShellGet" -Force).name
        $fpp | Should -Contain "PowerShellGet"
    }

    It "Install-PackageProvider, Expect succeed" {
        $name = "NanoServerPackage"
        $ipp = (Install-PackageProvider -Name $name -Force -Source $source -Scope CurrentUser).name
        $ipp | Should -Contain $name
    }

    It "Find-package"  {
        $f = Find-Package -ProviderName NuGet -Name $packageName -Source $localSourceName
        $f.Name | Should -Contain "$packageName"
	}

    It "Install-package"  {
        $i = Install-Package -ProviderName NuGet -Name $packageName -Force -Source $localSourceName -Scope CurrentUser
        $i.Name | Should -Contain "$packageName"
	}

    # this test relies on the previous test to install jquery
    It "Get-package"  {
        $g = Get-Package -ProviderName NuGet -Name $packageName
        $g.Name | Should -Contain "$packageName"
	}

    It "save-package"  {
        $s = Save-Package -ProviderName NuGet -Name $packageName -Path $TestDrive -Force -Source $localSourceName
        $s.Name | Should -Contain "$packageName"
	}

    It "uninstall-package"  {
        $u = Uninstall-Package -ProviderName NuGet -Name $packageName
        $u.Name | Should -Contain "$packageName"
	}
}
