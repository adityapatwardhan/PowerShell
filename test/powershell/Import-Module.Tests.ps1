﻿Describe "Import-Module" {
    $moduleName = "Microsoft.PowerShell.Platform"

    BeforeEach {
        Remove-Module -Name $moduleName -Force
        (Get-Module -Name $moduleName).Name | Should BeNullOrEmpty 
    }

    AfterEach {
        Import-Module -Name $moduleName -Force
        (Get-Module -Name $moduleName).Name | Should Be $moduleName
    }

    It "should be able to add a module with using Name switch" {
        { Import-Module -Name $moduleName } | Should Not Throw        
        (Get-Module -Name $moduleName).Name | Should Be $moduleName
    }

    It "should be able to add a module with using ModuleInfo switch" {
        $a = Get-Module -ListAvailable $moduleName
        { Import-Module -ModuleInfo $a } | Should Not Throw
        (Get-Module -Name $moduleName).Name | Should Be $moduleName
    }
}