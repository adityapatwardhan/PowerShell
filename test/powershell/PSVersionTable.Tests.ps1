Describe "PSVersionTable" {
    It "Should have version table entries" {
        $PSVersionTable.Count | Should Be 8
    }

    It "Should have the right version table entries" {
        $PSVersionTable.ContainsKey("PSVersion")                 | Should Be True
        $PSVersionTable.ContainsKey("PSEdition")                 | Should Be True
        $PSVersionTable.ContainsKey("WSManStackVersion")         | Should Be True
        $PSVersionTable.ContainsKey("SerializationVersion")      | Should Be True
        $PSVersionTable.ContainsKey("CLRVersion")                | Should Be True
        $PSVersionTable.ContainsKey("BuildVersion")              | Should Be True
        $PSVersionTable.ContainsKey("PSCompatibleVersions")      | Should Be True
        $PSVersionTable.ContainsKey("PSRemotingProtocolVersion") | Should Be True

    }

    It "Should have the correct edition" -Skip:(!$IsCore) {
        $expected =
        if ($IsLinux -Or $IsOSX) {
            "Linux"
        } elseif ($IsWindows) {
            "Core"
        } else {
            throw [PlatformNotSupportedException]
        }

        $PSVersionTable["PSEdition"] | Should Be $expected
    }
}
