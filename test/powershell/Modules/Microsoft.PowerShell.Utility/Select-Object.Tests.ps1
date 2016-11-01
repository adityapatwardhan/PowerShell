. (Join-Path -Path $PSScriptRoot -ChildPath Test-Mocks.ps1)

Describe "Select-Object" -Tags "CI" {
    BeforeEach {
	$dirObject  = GetFileMock
	$TestLength = 3
    }

    It "Handle piped input without error" {
	{ $dirObject | Select-Object } | Should Not Throw
    }

    It "Should treat input as a single object with the inputObject parameter" {
	$result   = $(Select-Object -inputObject $dirObject -last $TestLength).Length
	$expected = $dirObject.Length

	$result | Should Be $expected
    }

    It "Should be able to use the alias" {
	{ $dirObject | select } | Should Not Throw
    }

    It "Should have same result when using alias" {
	$result   = $dirObject | select
	$expected = $dirObject | Select-Object

	$result | Should Be $expected
    }

    It "Should return correct object with First parameter" {
	$result = $dirObject | Select-Object -First $TestLength

	$result.Length | Should Be $TestLength

	for ($i=0; $i -lt $TestLength; $i++)
	{
	    $result[$i].Name | Should Be $dirObject[$i].Name
	}
    }

    It "Should return correct object with Last parameter" {
	$result = $dirObject | Select-Object -Last $TestLength

	$result.Length | Should Be $TestLength

	for ($i=0; $i -lt $TestLength; $i++)
	{
	    $result[$i].Name | Should Be $dirObject[$dirObject.Length - $TestLength + $i].Name
	}
    }

    It "Should work correctly with Unique parameter" {
	$result   = ("a","b","c","a","a","a" | Select-Object -Unique).Length
	$expected = 3

	$result | Should Be $expected
    }

    It "Should return correct object with Skip parameter" {
	$result = $dirObject | Select-Object -Skip $TestLength

	$result.Length       | Should Be ($dirObject.Length - $TestLength)

	for ($i=0; $i -lt $TestLength; $i++)
	{
	    $result[$i].Name | Should Be $dirObject[$TestLength + $i].Name
	}
    }

    It "Should return an object with selected columns" {
	$result = $dirObject | Select-Object -Property Name, Size

	$result.Length  | Should Be $dirObject.Length
	$result[0].Name | Should Be $dirObject[0].Name
	$result[0].Size | Should Be $dirObject[0].Size
	$result[0].Mode | Should BeNullOrEmpty
    }

    It "Should send output to pipe properly" {
	{$dirObject | Select-Object -Unique | pipelineConsume} | Should Not Throw
    }

    It "Should select array indices with Index parameter" {
	$firstIndex  = 2
	$secondIndex = 4
	$result      = $dirObject | Select-Object -Index $firstIndex, $secondIndex

	$result[0].Name | Should Be $dirObject[$firstIndex].Name
	$result[1].Name | Should Be $dirObject[$secondIndex].Name
    }

    # Note that these two tests will modify original values of $dirObject

    It "Should not wait when used without -Wait option" {
	$orig1  = $dirObject[0].Size
	$orig2  = $dirObject[$TestLength].Size
	$result = $dirObject | addOneToSizeProperty | Select-Object -First $TestLength

	$result[0].Size              | Should Be ($orig1 + 1)
	$dirObject[0].Size           | Should Be ($orig1 + 1)
	$dirObject[$TestLength].Size | Should Be $orig2
    }

    It "Should wait when used with -Wait option" {
	$orig1  = $dirObject[0].Size
	$orig2  = $dirObject[$TestLength].Size
	$result = $dirObject | addOneToSizeProperty | Select-Object -First $TestLength -Wait

	$result[0].Size              | Should Be ($orig1 + 1)
	$dirObject[0].Size           | Should Be ($orig1 + 1)
	$dirObject[$TestLength].Size | Should Be ($orig2 + 1)
    }
}

Describe "Select-Object DRT basic functionality" -Tags "CI" {
	BeforeAll {
		$employees = [pscustomobject]@{"FirstName"="joseph"; "LastName"="smith"; "YearsInMS"=15},
                            [pscustomobject]@{"FirstName"="paul"; "LastName"="smith"; "YearsInMS"=15},
                            [pscustomobject]@{"FirstName"="mary"; "LastName"="soe"; "YearsInMS"=5},
                            [pscustomobject]@{"FirstName"="edmund"; "LastName"="bush"; "YearsInMS"=9}
	}

	It "Select-Object with empty script block property should throw"{
		try  
		{  
			"bar" | select-object -Prop {} -EA Stop  
			Throw "Execution OK"   
		}  
		catch   
		{  
			$_.CategoryInfo | Should Match "PSArgumentException"    
			$_.FullyQualifiedErrorId | Should be "EmptyScriptBlockAndNoName,Microsoft.PowerShell.Commands.SelectObjectCommand"  
		}
	}
	
	It "Select-Object with string property should work"{
		$result = "bar" | select-object -Prop foo | Measure-Object
		$result.Count | Should Be 1
	}
	
	It "Select-Object with Property First Last Overlap should work"{
		$results = $employees | Select-Object -Property "YearsInMS", "L*" -First 2 -Last 3
		
		$results.Count | Should Be 4
		
		$results[0].LastName | Should Be $employees[0].LastName
		$results[1].LastName | Should Be $employees[1].LastName
		$results[2].LastName | Should Be $employees[2].LastName
		$results[3].LastName | Should Be $employees[3].LastName
		
		$results[0].YearsInMS | Should Be $employees[0].YearsInMS
		$results[1].YearsInMS | Should Be $employees[1].YearsInMS
		$results[2].YearsInMS | Should Be $employees[2].YearsInMS
		$results[3].YearsInMS | Should Be $employees[3].YearsInMS
	}
	
	It "Select-Object with Property First Last should work"{
		$results = $employees | Select-Object -Property "YearsInMS", "L*" -First 2 -Last 1
		
		$results.Count | Should Be 3
		
		$results[0].LastName | Should Be $employees[0].LastName
		$results[1].LastName | Should Be $employees[1].LastName
		$results[2].LastName | Should Be $employees[3].LastName
		
		$results[0].YearsInMS | Should Be $employees[0].YearsInMS
		$results[1].YearsInMS | Should Be $employees[1].YearsInMS
		$results[2].YearsInMS | Should Be $employees[3].YearsInMS
	}
	
	It "Select-Object with Property First should work"{
		$results = $employees | Select-Object -Property "YearsInMS", "L*" -First 2
		
		$results.Count | Should Be 2
		
		$results[0].LastName | Should Be $employees[0].LastName
		$results[1].LastName | Should Be $employees[1].LastName
		
		$results[0].YearsInMS | Should Be $employees[0].YearsInMS
		$results[1].YearsInMS | Should Be $employees[1].YearsInMS
	}
	
	It "Select-Object with Property First Zero should work"{
		$results = $employees | Select-Object -Property "YearsInMS", "L*" -First 0
		
		$results.Count | Should Be 0
	}
	
	It "Select-Object with Property Last Zero should work"{
		$results = $employees | Select-Object -Property "YearsInMS", "L*" -Last 0
		
		$results.Count | Should Be 0
	}
	
	It "Select-Object with Unique should work"{
		$results = $employees | Select-Object -Property "YearsInMS", "L*" -Unique:$true
		
		$results.Count | Should Be 3
		
		$results[0].LastName | Should Be $employees[1].LastName
		$results[1].LastName | Should Be $employees[2].LastName
		$results[2].LastName | Should Be $employees[3].LastName
		
		$results[0].YearsInMS | Should Be $employees[1].YearsInMS
		$results[1].YearsInMS | Should Be $employees[2].YearsInMS
		$results[2].YearsInMS | Should Be $employees[3].YearsInMS
	}
	
	It "Select-Object with Simple should work"{
		$employee1 = [pscustomobject]@{"FirstName"="joesph"; "LastName"="smith"; "YearsInMS"=15}
		$employee2 = [pscustomobject]@{"FirstName"="paul"; "LastName"="smith"; "YearsInMS"=15}
		$employee3 = [pscustomobject]@{"FirstName"="mary"; "LastName"="soe"; "YearsInMS"=15}
		$employees3 = @($employee1,$employee2,$employee3,$employee4)
		$results = $employees3 | Select-Object -Property "FirstName", "YearsInMS"
		
		$results.Count | Should Be 3
		
		$results[0].FirstName | Should Be $employees3[0].FirstName
		$results[1].FirstName | Should Be $employees3[1].FirstName
		$results[2].FirstName | Should Be $employees3[2].FirstName
		
		$results[0].YearsInMS | Should Be $employees3[0].YearsInMS
		$results[1].YearsInMS | Should Be $employees3[1].YearsInMS
		$results[2].YearsInMS | Should Be $employees3[2].YearsInMS
	}
	
	It "Select-Object with no input should work"{
		$results = $null | Select-Object -Property "FirstName", "YearsInMS", "FirstNa*"
		$results.Count | Should Be 0
	}
	
	It "Select-Object with Start-Time In Idle Process should work"{
		$results = Get-Process i* | Select-Object ProcessName
		$results.Count | Should Not Be 0
	}
	
	It "Select-Object with Skip should work"{
		$results = "1","2","3" | Select-Object -Skip 1
		$results.Count | Should Be 2
		$results[0] | Should Be 2
		$results[1] | Should Be 3
	}
	
	It "Select-Object with Index should work"{
		$results = "1","2","3" | Select-Object -Index 2
		$results.Count | Should Be 1
		$results[0] | Should Be "3"
	}
}

Describe "Select-Object with Property = '*'" -Tags "CI" {

        # Issue #2420
	It "Select-Object with implicit Property = '*' don't return property named '*'"{
		$results = [pscustomobject]@{Thing="thing1"} | Select-Object -ExcludeProperty thing 
		$results.psobject.Properties.Item("*") | Should Be $null
	}

        # Issue #2420
	It "Select-Object with explicit Property = '*' don't return property named '*'"{
		$results = [pscustomobject]@{Thing="thing1"} | Select-Object -Property * -ExcludeProperty thing 
		$results.psobject.Properties.Item("*") | Should Be $null
	}

        # Issue #2351
	It "Select-Object with implicit Property = '*' exclude single property"{
		$results = [pscustomobject]@{Thing="thing1"} | Select-Object -ExcludeProperty thing
		$results.psobject.Properties.Item("Thing") | Should Be $null
		$results.psobject.Properties.Item("*") | Should Be $null
	}

        # Issue #2351
	It "Select-Object with explicit Property = '*' exclude single property"{
		$results = [pscustomobject]@{Thing="thing1"} | Select-Object -Property * -ExcludeProperty thing
		$results.psobject.Properties.Item("Thing") | Should Be $null
		$results.psobject.Properties.Item("*") | Should Be $null
	}

        # Issue #2351
	It "Select-Object with implicit Property = '*' exclude not single property"{
		$results = [pscustomobject]@{Thing="thing1";Param2="param2"} | Select-Object -ExcludeProperty Param2 
		$results.Param2 | Should Be $null
		$results.Thing | Should Be "thing1"
	}

        # Issue #2351
	It "Select-Object with explicit Property = '*' exclude not single property"{
		$results = [pscustomobject]@{Thing="thing1";Param2="param2"} | Select-Object -Property * -ExcludeProperty Param2 
		$results.Param2 | Should Be $null
		$results.Thing | Should Be "thing1"
	}

    It "Select-Object with ExpandProperty and Property don't skip processing ExcludeProperty" {
		$p = Get-Process -Id $pid | Select-Object -Property Process* -ExcludeProperty ProcessorAffinity -ExpandProperty Modules
		$p[0].psobject.Properties.Item("ProcessorAffinity") | Should Be $null
    }
}
