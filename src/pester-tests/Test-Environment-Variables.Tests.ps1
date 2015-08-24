﻿Describe "Test-Environment-Variables" {
    It "Should have environment variables" {
        Get-Item ENV: | Should Not BeNullOrEmpty
    }

    It "Should be able to access the members of the environment variable" {
        $expected = /bin/bash -c "cd ~ && pwd"

        (Get-Item ENV:HOME).Value     | Should Be $expected
    }

    It "Should be able to set the environment variables" {
        { $ENV:TESTENVIRONMENTVARIABLE = "this is a test environment variable" } | Should Not Throw

        $ENV:TESTENVIRONMENTVARIABLE | Should Not BeNullOrEmpty
        $ENV:TESTENVIRONMENTVARIABLE | Should Be "this is a test environment variable"

    }

    It "Should contain /bin in the PATH" {
        $ENV:PATH | Should Match "/bin"
    }

    It "Should have the correct HOSTNAME" {
        $expected = /bin/bash -c hostname

        $ENV:HOSTNAME | Should Be $expected
    }

    It "Should have a nonempty PATH" {
        $ENV:PATH | Should Not BeNullOrEmpty
    }
}
