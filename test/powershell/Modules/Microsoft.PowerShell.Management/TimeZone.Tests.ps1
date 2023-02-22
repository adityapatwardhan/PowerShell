# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
    --------------------------------------
    Script Notes: opportunities for improving this test script
    --------------------------------------
    Localization
        Many of the tests below looking-up timezones by Name do not support localization.
        That is, the current tests use US english versions of StandardName and DaylightName for tests.

        ref: https://msdn.microsoft.com/library/windows/desktop/ms725481.aspx
           [snippet] Both StandardName and DaylightName are localized according to the current user default UI language.
#>

Describe "Get-TimeZone Test cases" -Tags "CI" {
    BeforeAll {
        $timeZonesAvailable = [system.timezoneinfo]::GetSystemTimeZones()
        $skipTest = $timeZonesAvailable.Count -eq 0
    }

    It "Call without ListAvailable switch returns current TimeZoneInfo" -skip:$skipTest {
        $observed = (Get-TimeZone).Id
        $expected = ([System.TimeZoneInfo]::Local).Id
        $observed | Should -Be $expected
    }

    It "Call without ListAvailable switch returns an object of type TimeZoneInfo" -skip:$skipTest {
        $result = Get-TimeZone
        $result | Should -BeOfType TimeZoneInfo
    }

    It "Call WITH ListAvailable switch returns ArrayList of TimeZoneInfo objects where the list is greater than 0 item" -skip:$skipTest {
        $list = Get-TimeZone -ListAvailable
        $list.Count | Should -BeGreaterThan 0

        ,$list | Should -BeOfType Object[]
        $list[0] | Should -BeOfType TimeZoneInfo
    }

    ## The local time zone could be set to UTC or GMT*. In this case, the .NET API returns the region ID
    ## and not 'UTC'. To avoid a string matching error, we compare the BaseUtcOffset instead.
    It "Call with ListAvailable switch returns a list containing TimeZoneInfo.Local" -skip:$skipTest {
        $observedIdList = Get-TimeZone -ListAvailable | Select-Object -ExpandProperty BaseUtcOffset
        $oneExpectedOffset = ([System.TimeZoneInfo]::Local).BaseUtcOffset
        $oneExpectedOffset | Should -BeIn $observedIdList
    }

    ## The local time zone could be set to UTC or GMT*. In this case, the .NET API returns the region ID
    ## and not UTC. To avoid a string matching error, we compare the BaseUtcOffset instead.
    It "Call with ListAvailable switch returns a list containing one returned by Get-TimeZone" -skip:$skipTest {
        $observedIdList = Get-TimeZone -ListAvailable | Select-Object -ExpandProperty BaseUtcOffset
        $oneExpectedOffset = (Get-TimeZone).BaseUtcOffset
        $oneExpectedOffset | Should -BeIn $observedIdList
    }

    It "Call Get-TimeZone using ID param and single item" -skip:$skipTest {
        $selectedTZ = $timeZonesAvailable[0]
        (Get-TimeZone -Id $selectedTZ.Id).Id | Should -Be $selectedTZ.Id
    }

    It "Call Get-TimeZone using ID param and multiple items" -skip:$skipTest {
        wait-debugger
        $selectedTZ = $timeZonesAvailable | Select-Object -First 3 -ExpandProperty Id
        $result = (Get-TimeZone -Id $selectedTZ).Id
        $result | Should -Be $selectedTZ
    }

    It "Call Get-TimeZone using ID param and multiple items, where first and third are invalid ids - expect error" -skip:$skipTest {
        $selectedTZ = $timeZonesAvailable[0].Id
        $null = Get-TimeZone -Id @("Cape Verde Standard",$selectedTZ,"Azores Standard") `
                             -ErrorVariable errVar -ErrorAction SilentlyContinue
        $errVar.Count | Should -Be 2
        $errVar[0].FullyQualifiedErrorID | Should -Be "TimeZoneNotFound,Microsoft.PowerShell.Commands.GetTimeZoneCommand"
    }

    It "Call Get-TimeZone using ID param and multiple items, one is wild card but error action ignore works as expected" -skip:$skipTest {
        $selectedTZ = $timeZonesAvailable | Select-Object -First 3 -ExpandProperty Id
        $inputArray = $selectedTZ + "*"
        $result = Get-TimeZone -Id $inputArray -ErrorAction SilentlyContinue | ForEach-Object Id
        $result | Should -Be $selectedTZ
    }

    It "Call Get-TimeZone using Name param and single item" -skip:$skipTest {
        $timezoneList = Get-TimeZone -ListAvailable
        $timezoneName = $timezoneList[0].StandardName
        $observed = Get-TimeZone -Name $timezoneName
        $observed.StandardName | Should -Be $timezoneName
    }

    It "Call Get-TimeZone using Name param with wild card" -skip:$skipTest {
        $result = (Get-TimeZone -Name "Pacific*").Id
        $expectedIdList = ($timeZonesAvailable | Where-Object { $_.StandardName -match "^Pacific" }).Id
        $result | Should -Be $expectedIdList
    }

    It "Call Get-TimeZone Name parameter from pipeline by value " -skip:$skipTest {
        $result = ("Pacific*" | Get-TimeZone).Id
        $expectedIdList = ($timeZonesAvailable | Where-Object { $_.StandardName -match "^Pacific" }).Id
        $result | Should -Be $expectedIdList
    }

    It "Call Get-TimeZone Id parameter from pipeline by ByPropertyName" -skip:$skipTest {
        $timezoneList = Get-TimeZone -ListAvailable
        $timezone = $timezoneList[0]
        $observed = $timezone | Get-TimeZone
        $observed.StandardName | Should -Be $timezone.StandardName
    }
}

# Set-TimeZone fails due to missing ApiSet dependency on Windows Server 2012 R2.
$osInfo = [System.Environment]::OSVersion.Version
$isSrv2k12R2 = $osInfo.Major -eq 6 -and $osInfo.Minor -eq 3

Describe "Set-Timezone CI test case: call by single Id" -Tags @('CI', 'RequireAdminOnWindows') {
    BeforeAll {
        $skipTest = ! $IsWindows -or $isSrv2k12R2
        if ($skipTest) {
            return
        }

        $originalTimeZoneId = (Get-TimeZone).Id
    }
    AfterAll {
        if ($skipTest) {
            return
        }

        Set-TimeZone -Id $originalTimeZoneId
    }

    It "Call Set-TimeZone by Id" -skip:$skipTest {
        $origTimeZoneID = (Get-TimeZone).Id
        $timezoneList = Get-TimeZone -ListAvailable
        $testTimezone = $null
        foreach ($timezone in $timezoneList) {
            if ($timezone.Id -ne $origTimeZoneID) {
                $testTimezone = $timezone
                break
            }
        }
        Set-TimeZone -Id $testTimezone.Id
        $observed = Get-TimeZone
        $testTimezone.Id | Should -Be $observed.Id
    }
}

Describe "Set-Timezone Feature test cases" -Tags @('Feature', 'RequireAdminOnWindows') {
    BeforeAll {
        if ($skipTest) {
            return
        }

        $originalTimeZoneId = (Get-TimeZone).Id
    }

    AfterAll {
        if ($skipTest) {
            return
        }

        Set-TimeZone -Id $originalTimeZoneId
    }

    It "Call Set-TimeZone with invalid Id" -skip:$skipTest {
        { Set-TimeZone -Id "zzInvalidID" } | Should -Throw -ErrorId "TimeZoneNotFound,Microsoft.PowerShell.Commands.SetTimeZoneCommand"
    }

    It "Call Set-TimeZone by Name" -skip:$skipTest {
        $origTimeZoneName = (Get-TimeZone).StandardName
        $timezoneList = Get-TimeZone -ListAvailable
        $testTimezone = $null
        foreach ($timezone in $timezoneList) {
            if ($timezone.StandardName -ne $origTimeZoneName) {
                $testTimezone = $timezone
                break
            }
        }
        Set-TimeZone -Name $testTimezone.StandardName
        $observed = Get-TimeZone
        $testTimezone.StandardName | Should -Be $observed.StandardName
    }

    It "Call Set-TimeZone with invalid Name" -skip:$skipTest {
        { Set-TimeZone -Name "zzINVALID_Name" } | Should -Throw -ErrorId "TimeZoneNotFound,Microsoft.PowerShell.Commands.SetTimeZoneCommand"
    }

    It "Call Set-TimeZone from pipeline input object of type TimeZoneInfo" -skip:$skipTest {
        $origTimeZoneID = (Get-TimeZone).Id
        $timezoneList = Get-TimeZone -ListAvailable
        $testTimezone = $null
        foreach ($timezone in $timezoneList) {
            if ($timezone.Id -ne $origTimeZoneID) {
                $testTimezone = $timezone
                break
            }
        }

        $testTimezone | Set-TimeZone
        $observed = Get-TimeZone
        $observed.ID | Should -Be $testTimezone.Id
    }

    It "Call Set-TimeZone from pipeline input object of type TimeZoneInfo, verify supports whatif" -skip:$skipTest {
        $origTimeZoneID = (Get-TimeZone).Id
        $timezoneList = Get-TimeZone -ListAvailable
        $testTimezone = $null
        foreach ($timezone in $timezoneList) {
            if ($timezone.Id -ne $origTimeZoneID) {
                $testTimezone = $timezone
                break
            }
        }

        Set-TimeZone -Id $testTimezone.Id -WhatIf > $null
        $observed = Get-TimeZone
        $observed.Id | Should -Be $origTimeZoneID
    }
}
