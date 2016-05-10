﻿Describe "Write-Host DRT Unit Tests" -Tags DRT{ 
    $testData = @(
        @{ Name = 'NoNewline';Command = "Write-Host a,b -Separator ',' -ForegroundColor Yellow -BackgroundColor DarkBlue -NoNewline"; returnValue = "a,b" }
        @{ Name = 'Separator';Command = "Write-Host a,b,c -Separator '+'"; returnValue = "a+b+c" }
    )
   
    It "write-Host works with '<Name>' switch" -TestCases $testData { 
        param($Command, $returnValue) 

        $tempFile = [io.path]::getTempFIleName() 
        $script = Join-Path $TestDrive -ChildPath writeHost.ps1           
        
        $Command > $script

        if($IsLinux)
        {
            powershell $script > $tempFile
        }
        else
        {
            powershell.exe $script > $tempFile
        }
        $content = Get-Content $tempFile
        $content | Should Be $returnValue
    }
}