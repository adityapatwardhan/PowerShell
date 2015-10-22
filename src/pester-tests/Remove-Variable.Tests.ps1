﻿# ensure the machine is in a clean state from the outset.
Remove-Variable -Name var1 -ErrorAction SilentlyContinue -Force

Describe "Remove-Variable" {
    It "Should throw an error when a dollar sign is used in the variable name place" {
        New-Variable -Name var1 -Value 4

        Remove-Variable $var1 -ErrorAction SilentlyContinue | Should Throw
    }

    It "Should not throw error when used without the Name field, and named variable is properly specified and exists" {
        New-Variable -Name var1 -Value 4

        Remove-Variable var1

        $var1 | Should Be #nothing.  it should be Nothing at all.
    }

    It "Should not throw error when used with the Name field, and named variable is specified and exists" {
        New-Variable -Name var1 -Value 2

        Remove-Variable -Name var1

        $var1 | Should Be #nothing.  it should be Nothing at all.
    }

    It "Should throw error when used with Name field, and named variable does not exist" {
        Remove-Variable -Name nonexistantVariable -ErrorAction SilentlyContinue | Should Throw
    }

    It "Should be able to remove a variable using the rv alias" {
        New-Variable var1 -Value 2 

        $var1 | Should Be 2

        rv -Name var1

        $var1 | Should Be #nothing.  it should be Nothing at all.

    }

    It "Should be able to remove a set of variables using wildcard characters" {
        New-Variable tmpvar1 -Value "tempvalue"
        New-Variable tmpvar2 -Value 2
        New-Variable tmpmyvar1 -Value 234

        $tmpvar1   | Should Be "tempvalue"
        $tmpvar2   | Should Be 2
        $tmpmyvar1 | Should Be 234

        Remove-Variable -Name tmp*

        $tmpvar1   | Should Be #nothing.  it should be Nothing at all.
        $tmpvar2   | Should Be #nothing.  it should be Nothing at all.
        $tmpmyvar1 | Should Be #nothing.  it should be Nothing at all.
    }

    It "Should be able to exclude a set of variables to remove using the Exclude switch" {
        New-Variable tmpvar1 -Value "tempvalue"
        New-Variable tmpvar2 -Value 2
        New-Variable tmpmyvar1 -Value 234

        $tmpvar1   | Should Be "tempvalue"
        $tmpvar2   | Should Be 2
        $tmpmyvar1 | Should Be 234

        Remove-Variable -Name tmp* -Exclude *my*

        $tmpvar1   | Should Be #nothing.  it should be Nothing at all.
        $tmpvar2   | Should Be #nothing.  it should be Nothing at all.
        $tmpmyvar1 | Should Be 234
    }

    It "Should be able to include a set of variables to remove using the Include switch" {
        New-Variable tmpvar1 -Value "tempvalue"
        New-Variable tmpvar2 -Value 2
        New-Variable tmpmyvar1 -Value 234
        New-Variable thevar -Value 1

        $tmpvar1   | Should Be "tempvalue"
        $tmpvar2   | Should Be 2
        $tmpmyvar1 | Should Be 234
        $thevar    | Should Be 1

        Remove-Variable -Name tmp* -Include *my*

        $tmpvar1   | Should Be "tempvalue"
        $tmpvar2   | Should Be 2
        $tmpmyvar1 | Should Be #nothing.  it should be Nothing at all.
        $thevar    | should Be 1

        Remove-Variable tmpvar1
        Remove-Variable tmpvar2
        Remove-Variable thevar

    }

    It "Should throw an error when attempting to remove a read-only variable and the Force switch is not used" {
        New-Variable -Name var1 -Value 2 -Option ReadOnly

        Remove-Variable -Name var1 -ErrorAction SilentlyContinue | Should Throw

        $var1 | Should Be 2

        Remove-Variable -Name var1 -Force
    }

    It "Should not throw an error when attempting to remove a read-only variable and the Force switch is used" {
        New-Variable -Name var1 -Value 2 -Option ReadOnly

        Remove-Variable -Name var1 -Force

        $var1 | Should Be # Nothing.  It should be nothing at all.
    }

    Context "Scope Tests" {
        It "Should be able to remove a global variable using the global switch" {
            New-Variable -Name var1 -Value "context" -Scope global

            Remove-Variable -Name var1 -Scope global

            $var1 | Should Be #Nothing.
        }

        It "Should not be able to clear a global variable using the local switch" {
            New-Variable -Name var1 -Value "context" -Scope global

            Remove-Variable -Name var1 -Scope local -ErrorAction SilentlyContinue | Should Throw

            $var1 | Should Be "context"

            Remove-Variable -Name var1 -Scope global
            $var1 | Should Be # Nothing
        }

        It "Should not be able to clear a global variable using the script switch" {
            New-Variable -Name var1 -Value "context" -Scope global

            Remove-Variable -Name var1 -Scope local -ErrorAction SilentlyContinue | Should Throw

            $var1 | Should Be "context"

            Remove-Variable -Name var1 -Scope global
            $var1 | Should Be # Nothing
        }

        It "Should be able to remove an item locally using the local switch" {
            New-Variable -Name var1 -Value "context" -Scope local

            Remove-Variable -Name var1 -Scope local -ErrorAction SilentlyContinue | Should Throw

            $var1 | Should Be # Nothing
        }

        It "Should be able to remove an item locally using the global switch" {
            New-Variable -Name var1 -Value "context" -Scope local

            Remove-Variable -Name var1 -Scope global -ErrorAction SilentlyContinue | Should Throw

            $var1 | Should Be "context"

            Remove-Variable -Name var1 -Scope local
            $var1 | Should Be # Nothing
        }

        It "Should be able to remove a local variable using the script scope switch" {
            New-Variable -Name var1 -Value "context" -Scope local

            Remove-Variable -Name var1 -Scope script -ErrorAction SilentlyContinue | Should Throw

            $var1 | Should Be "context"

            Remove-Variable -Name var1 -Scope local
            $var1 | Should Be # Nothing
        }

        It "Should be able to remove a script variable created using the script switch" {
            New-Variable -Name var1 -Value "context" -Scope script

            Remove-Variable -Name var1 -Scope script | Should Throw

            $var1 | Should Be # Nothing
        }

        It "Should not be able to remove a global script variable that was created using the script scope switch" {
            New-Variable -Name var1 -Value "context" -Scope script

            Remove-Variable -Name var1 -Scope global -ErrorAction SilentlyContinue | Should Throw

            $var1 | Should Be "context"
        }
    }
}
