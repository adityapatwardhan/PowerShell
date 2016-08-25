﻿#
# Copyright (c) Microsoft Corporation, 2015
# 
# This is a Pester test suite which validate the Json cmdlets.
#

#
# This 'Describe' is for tests that were converted from utscripts (SDXROOT/admin/monad/tests/monad/DRT/utscripts) 
# and C# tests (SDXROOT/admin/monad/tests/monad/DRT/commands/utility/UnitTests) to Pester.
#
Describe "Json Tests" -Tags "Feature" {

    BeforeAll {

        function ValidateSampleObject
        {
            param ($result, [switch]$hasEmbeddedSampleObject )

            Write-Verbose "validating deserialized SampleObject" -Verbose
            $result.SampleInt | Should Be 98765
            $result.SampleString | Should Match "stringVal"
            $result.SampleArray.Count | Should Be 2
            $result.SampleTrue | Should Be $true
            $result.SampleFalse | Should Be $false
            $result.SampleNull | Should Be $null
            $result.SampleFloat | Should Be 9.8765E+43

            if ($hasEmbeddedSampleObject)
            {
                Write-Verbose "validating deserialized Embedded SampleObject" -Verbose
                ValidateSampleObject -result $result.SampleObject
            }
        }

    }

    Context "ConvertTo-Json Bug Fixes" {

        It "ConvertTo-JSON should not have hard coded english error message" {

            # Test follow-up for bug WinBlue: 163372 - ConvertTo-JSON has hard coded english error message.
            $process = Get-Process -Id $PID
            $hash = @{ $process = "def" }
            $expectedFullyQualifiedErrorId = "NonStringKeyInDictionary,Microsoft.PowerShell.Commands.ConvertToJsonCommand"

            try
            {
                ConvertTo-Json -InputObject $hash
                throw "CodeExecuted"
            }
            catch
            {
                $_.FullyQualifiedErrorId | should be $expectedFullyQualifiedErrorId
            }
        }

        It "ConvertTo-Json should handle terms with double quotes" {

            # Test follow-up for bug WinBlue: 11484 - ConvertTo-Json can't handle terms with double quotes.

            $notcompressed = ConvertTo-JSON @{ FirstName = 'Hello " World' }
            $compressed = ConvertTo-Json @{ FirstName = 'Hello " World' } -Compress
            $valueFromNotCompressedResult = ConvertFrom-Json -InputObject $notcompressed
            $valueFromCompressedResult = ConvertFrom-Json -InputObject $compressed

            $valueFromNotCompressedResult.FirstName | Should Match $valueFromCompressedResult.FirstName
        }

        It "Convertto-Json should handle Enum based on Int64" {

            # Test follow-up for bug Win8: 378368 Convertto-Json problems with Enum based on Int64.
            if ( ("JsonEnumTest" -as "Type") -eq $null ) { 
                $enum1 = "TestEnum" + (get-random)
                $enum2 = "TestEnum" + (get-random)
                $enum3 = "TestEnum" + (get-random)

                $jsontype = add-type -pass -TypeDef "
                public enum $enum1 : ulong { One = 1, Two = 2 };
                public enum $enum2 : long  { One = 1, Two = 2 };
                public enum $enum3 : int   { One = 1, Two = 2 };
                public class JsonEnumTest {
                    public $enum1 TestEnum1 = ${enum1}.One;
                    public $enum2 TestEnum2 = ${enum2}.Two;
                    public $enum3 TestEnum3 = ${enum3}.One;
                }" 
            }
            $op = [JsonEnumTest]::New() | convertto-json | convertfrom-json
            $op.TestEnum1 | Should Be "One"
            $op.TestEnum2 | Should Be "Two"
            $op.TestEnum3 | Should Be 1
        }

        It "Test followup for Windows 8 bug 121627" {

            $JsonString = Get-Command Get-help |Select-Object Name, Noun, Verb| ConvertTo-Json
            $actual = ConvertFrom-Json $JsonString

            $actual.Name | Should Be "Get-Help"
            $actual.Noun | Should Be "Help"
            $actual.Verb | Should Be "Get"
        }
    }

    Context "ConvertFrom and ConvertTo on JsonObject Tests" {

        It "Convert dictionary to PSObject" {
            
            $response = ConvertFrom-Json '{"d":{"__type":"SimpleJsonObject","Name":{"First":"Joel","Last":"Wood"},"Greeting":"Hello"}}'
            $response.d.Name.First | Should Match "Joel"
        }

        It "Convert to Json using PSObject" -pending:($IsCoreCLR) {
            
            $response = ConvertFrom-Json '{"d":{"__type":"SimpleJsonObject","Name":{"First":"Joel","Last":"Wood"},"Greeting":"Hello"}}'
            
            $response2 = ConvertTo-Json -InputObject $response -ErrorAction Continue
            $response2 = ConvertTo-Json -InputObject $response -ErrorAction Inquire
            $response2 = ConvertTo-Json -InputObject $response -ErrorAction SilentlyContinue
            $response2 = ConvertTo-Json -InputObject $response -Depth 2 -Compress
            $response2 | Should Be '{"d":{"Name":{"First":"Joel","Last":"Wood"},"Greeting":"Hello"}}'

            $response2 = ConvertTo-Json -InputObject $response -Depth 1 -Compress
            $nameString = [System.Management.Automation.LanguagePrimitives]::ConvertTo($response.d.Name, [string]) 
            $response2 | Should Be "{`"d`":{`"Name`":`"$nameString`",`"Greeting`":`"Hello`"}}"

            $result1 = @"
{
    "d":  {
              "Name":  {
                           "First":  "Joel",
                           "Last":  "Wood"
                       },
              "Greeting":  "Hello"
          }
}
"@
            $response2 = ConvertTo-Json -InputObject $response -Depth 2
            $response2 | Should Match $result1

            $result2 = @"
{
    "d":  {
              "Name":  "$nameString",
              "Greeting":  "Hello"
          }
}
"@
            $response2 = ConvertTo-Json -InputObject $response -Depth 1
            $response2 | Should Match $result2

            $arraylist = new-Object System.Collections.ArrayList
            [void]$arraylist.Add("one")
            [void]$arraylist.Add("two")
            [void]$arraylist.Add("three")
            $response2 = ConvertTo-Json -InputObject $arraylist -Compress
            $response2 | Should Be '["one","two","three"]'

            $result3 = @"
[
    "one",
    "two",
    "three"
]
"@
            $response2 = ConvertTo-Json -InputObject $arraylist
            $response2 | Should Be $result3

            $response2 = $arraylist | ConvertTo-Json 
            $response2 | Should Be $result3
        }
        
        It "Convert to Json using hashtable" -pending:($IsCoreCLR) {
            
            $nameHash = @{First="Joe1";Last="Wood"}
            $dHash = @{Name=$nameHash; Greeting="Hello"}
            $rootHash = @{d=$dHash}
            $response3 = ConvertTo-Json -InputObject $rootHash -Depth 2 -Compress
            $response3 | Should Be '{"d":{"Greeting":"Hello","Name":{"Last":"Wood","First":"Joe1"}}}'

            $response3 = ConvertTo-Json -InputObject $rootHash -Depth 1 -Compress
            $response3 | Should Be '{"d":{"Greeting":"Hello","Name":"System.Collections.Hashtable"}}'

            $result4 = @"
{
    "d":  {
              "Greeting":  "Hello",
              "Name":  {
                           "Last":  "Wood",
                           "First":  "Joe1"
                       }
          }
}
"@
            $response3 = ConvertTo-Json -InputObject $rootHash -Depth 2
            $response3 | Should Be $result4

            $result5 = @"
{
    "d":  {
              "Greeting":  "Hello",
              "Name":  "System.Collections.Hashtable"
          }
}
"@
            $response3 = ConvertTo-Json -InputObject $rootHash -Depth 1
            $response3 | Should Be $result5
        }

        It "Convert from Json allows an empty string" {
            
            $emptyStringResult = ConvertFrom-Json ""
            $emptyStringResult | Should Be $null
        }
    }

    Context "JsonObject Tests" {

        It "AddMember on JsonObject" {
        
            # create a Version object
            $versionObject = New-Object System.Version 2, 3, 4, 14

            # add a NoteProperty member called Note with a text note
            $versionObject | Add-Member -MemberType NoteProperty -Name Note -Value "a version object"

            # add an AliasProperty called Rev as an alias to the Revision property
            $versionObject | Add-Member -MemberType AliasProperty -Name Rev -Value Revision

            # add a ScriptProperty called IsOld which returns whether the version is an older version  
            $versionObject | Add-Member -MemberType ScriptProperty -Name IsOld -Value { ($this.Major -le 3) }

            $jstr = ConvertTo-Json $versionObject
    
            # convert the JSON string to a JSON object
            $json = ConvertFrom-Json $jstr
        
            # Check the basic properties
            $json.Major | Should Be 2
            $json.Minor | Should Be 3
            $json.Build | Should Be 4
            $json.Revision | Should Be 14
            $json.Note | Should Match "a version object"

            # Check the AliasProperty
            $json.Rev | Should Be $json.Revision

            # Check the ScriptProperty
            $json.IsOld | Should Be $true
        }

        It "ConvertFrom-Json with a key value pair" {

            $json = "{name:1}"
            $result = ConvertFrom-Json $json
            $result.name | Should Be 1
        }

        It "ConvertFrom-Json with a simple array" {

            $json = "[1,2,3,4,5,6]"
            $result = ConvertFrom-Json $json
            $result.Count | Should Be 6
            $result.GetType().BaseType.fullname | Should Be "System.Array"
        }

        It "ConvertFrom-Json with a float value" {

            $json = '{"SampleFloat1":1.2345E67, "SampleFloat2":-7.6543E-12}'
            $result = ConvertFrom-Json $json

            $sampleFloat1 = Invoke-Expression 1.2345E67
            $result.SampleFloat1 | Should Be $sampleFloat1

            $sampleFloat2 = Invoke-Expression -7.6543E-12
            $result.SampleFloat2 | Should Be $sampleFloat2
        }

        It "ConvertFrom-Json hash table nested in array" {

            $json = "['one', 'two', {'First':1,'Second':2,'Third':['Five','Six', 'Seven']}, 'four']"
            $result = ConvertFrom-Json $json

            $result.Count | Should Be 4
            $result[0] | Should Be "one"
            $result[1] | Should Be "two"
            $result[3] | Should Be "four"

            $hash = $result[2]
            $hash.First | Should Be 1
            $hash.Second | Should Be 2
            $hash.Third.Count | Should Be 3
            $hash.Third[0] | Should Be "Five"
            $hash.Third[1] | Should Be "Six"
            $hash.Third[2] | Should Be "Seven"
        }

        It "ConvertFrom-Json array nested in hash table" {

            $json = '{"First":["one", "two", "three"], "Second":["four", "five"], "Third": {"blah": 4}}'
            $result = ConvertFrom-Json $json

            $result.First.Count | Should Be 3
            $result.First[0] | Should Be "one"
            $result.First[1] | Should Be "two"
            $result.First[2] | Should Be "three"

            $result.Second.Count | Should Be 2
            $result.Second[0] | Should Be "four"
            $result.Second[1] | Should Be "five"
            
            $result.Third.blah | Should Be "4"
        }

        It "ConvertFrom-Json case insensitive test" {

            $json = '{"sAMPleValUE":12345}'
            $result = ConvertFrom-Json $json

            $result.SampleValue | Should Be 12345
        }
        

        It "ConvertFrom-Json sample values" {

            $json = '{"SampleInt":98765, "SampleString":"stringVal","SampleArray":[2,"two"], "SampleTrue":true, "SampleFalse":false,"SampleNull":null, "SampleFloat":9.8765E43}'
            $result = ConvertFrom-Json $json

            # Validate the result object
            ValidateSampleObject -result $result


            $json = '{"SampleInt":98765, "SampleString":"stringVal","SampleArray":[2,"two"], "SampleTrue":true, ' +
                    '"SampleFalse":false,"SampleNull":null, "SampleFloat":9.8765E43, "SampleObject":'+
                    '{"SampleInt":98765, "SampleString":"stringVal","SampleArray":[2,"two"], '+
                    '"SampleTrue":true, "SampleFalse":false,"SampleNull":null, "SampleFloat":9.8765E43}}'

            # Validate the result object
            $result = ConvertFrom-Json $json
            ValidateSampleObject -result $result -hasEmbeddedSampleObject
        }

        It "ConvertFrom-Json with special characters" {

            $json = '{"SampleValue":"\"\\\b\f\n\r\t\u4321\uD7FF"}'
            $result = ConvertFrom-Json $json
            $result.SampleValue[0] | Should Be '"'
            $result.SampleValue[1] | Should Be '\'
            $result.SampleValue[2] | Should Be 0x8
            $result.SampleValue[3] | Should Be 0xC
            $result.SampleValue[4] | Should Be 0xA
            $result.SampleValue[5] | Should Be 0xD
            $result.SampleValue[6] | Should Be 0x9
            $result.SampleValue[7] | Should Be 0x4321
            $result.SampleValue[8] | Should Be 0xD7FF
        }
    }
}

# This Describe is for new Json tests
#
Describe "Validate Json serialization" -Tags "CI" {

    Context "Validate Json serialization ascii values" {

        $testCases = @(
            @{
                TestInput = 0
                ToJson = if ( $IsCoreCLR ) { '"\u0000"' } else { 'null' }
                FromJson = ''
             }
            @{
                TestInput = 1
                ToJson = '"\u0001"'
                FromJson = ''
             }
            @{
                TestInput = 2
                ToJson = '"\u0002"'
                FromJson = ''
             }
            @{
                TestInput = 3
                ToJson = '"\u0003"'
                FromJson = ''
             }
            @{
                TestInput = 4
                ToJson = '"\u0004"'
                FromJson = ''
             }
            @{
                TestInput = 5
                ToJson = '"\u0005"'
                FromJson = ''
             }
            @{
                TestInput = 6
                ToJson = '"\u0006"'
                FromJson = ''
             }
            @{
                TestInput = 7
                ToJson = '"\u0007"'
                FromJson = ''
             }
            @{
                TestInput = 8
                ToJson = '"\b"'
                FromJson = ''
             }
            @{
                TestInput = 9
                ToJson = '"\t"'
                FromJson = '	'
             }
            @{
                TestInput = 10
                ToJson = '"\n"'
                FromJson = "`n"
             }
            @{
                TestInput = 11
                ToJson = '"\u000b"'
                FromJson = ''
             }
            @{
                TestInput = 12
                ToJson = '"\f"'
                FromJson = ''
             }
            @{
                TestInput = 13
                ToJson = '"\r"'
                FromJson = ''
             }
            @{
                TestInput = 14
                ToJson = '"\u000e"'
                FromJson = ''
             }
            @{
                TestInput = 15
                ToJson = '"\u000f"'
                FromJson = ''
             }
            @{
                TestInput = 16
                ToJson = '"\u0010"'
                FromJson = ''
             }
            @{
                TestInput = 17
                ToJson = '"\u0011"'
                FromJson = ''
             }
            @{
                TestInput = 18
                ToJson = '"\u0012"'
                FromJson = ''
             }
            @{
                TestInput = 19
                ToJson = '"\u0013"'
                FromJson = ''
             }
            @{
                TestInput = 20
                ToJson = '"\u0014"'
                FromJson = ''
             }
            @{
                TestInput = 21
                ToJson = '"\u0015"'
                FromJson = ''
             }
            @{
                TestInput = 22
                ToJson = '"\u0016"'
                FromJson = ''
             }
            @{
                TestInput = 23
                ToJson = '"\u0017"'
                FromJson = ''
             }
            @{
                TestInput = 24
                ToJson = '"\u0018"'
                FromJson = ''
             }
            @{
                TestInput = 25
                ToJson = '"\u0019"'
                FromJson = ''
             }
            @{
                TestInput = 26
                ToJson = '"\u001a"'
                FromJson = ''
             }
            @{
                TestInput = 27
                ToJson = '"\u001b"'
                FromJson = ''
             }
            @{
                TestInput = 28
                ToJson = '"\u001c"'
                FromJson = ''
             }
            @{
                TestInput = 29
                ToJson = '"\u001d"'
                FromJson = ''
             }
            @{
                TestInput = 30
                ToJson = '"\u001e"'
                FromJson = ''
             }
            @{
                TestInput = 31
                ToJson = '"\u001f"'
                FromJson = ''
             }
            @{
                TestInput = 32
                ToJson = '" "'
                FromJson = ' '
             }
            @{
                TestInput = 33
                ToJson = '"!"'
                FromJson = '!'
             }
            @{
                TestInput = 34
                ToJson = '"\""'
                FromJson = '"'
             }
            @{
                TestInput = 35
                ToJson = '"#"'
                FromJson = '#'
             }
            @{
                TestInput = 36
                ToJson = '"$"'
                FromJson = '$'
             }
            @{
                TestInput = 37
                ToJson = '"%"'
                FromJson = '%'
             }
            @{
                TestInput = 38
                ToJson = if ( $IsCoreCLR ) { '"&"' } else { '"\u0026"' }
                FromJson = '&'
             }
            @{
                TestInput = 39
                ToJson = if ( $IsCoreCLR ) { '"''"' } else { '"\u0027"' }
                FromJson = "'"
             }
            @{
                TestInput = 40
                ToJson = '"("'
                FromJson = '('
             }
            @{
                TestInput = 41
                ToJson = '")"'
                FromJson = ')'
             }
            @{
                TestInput = 42
                ToJson = '"*"'
                FromJson = '*'
             }
            @{
                TestInput = 43
                ToJson = '"+"'
                FromJson = '+'
             }
            @{
                TestInput = 44
                ToJson = '","'
                FromJson = ','
             }
            @{
                TestInput = 45
                ToJson = '"-"'
                FromJson = '-'
             }
            @{
                TestInput = 46
                ToJson = '"."'
                FromJson = '.'
             }
            @{
                TestInput = 47
                ToJson = '"/"'
                FromJson = '/'
             }
            @{
                TestInput = 48
                ToJson = '"0"'
                FromJson = '0'
             }
            @{
                TestInput = 49
                ToJson = '"1"'
                FromJson = '1'
             }
            @{
                TestInput = 50
                ToJson = '"2"'
                FromJson = '2'
             }
            @{
                TestInput = 51
                ToJson = '"3"'
                FromJson = '3'
             }
            @{
                TestInput = 52
                ToJson = '"4"'
                FromJson = '4'
             }
            @{
                TestInput = 53
                ToJson = '"5"'
                FromJson = '5'
             }
            @{
                TestInput = 54
                ToJson = '"6"'
                FromJson = '6'
             }
            @{
                TestInput = 55
                ToJson = '"7"'
                FromJson = '7'
             }
            @{
                TestInput = 56
                ToJson = '"8"'
                FromJson = '8'
             }
            @{
                TestInput = 57
                ToJson = '"9"'
                FromJson = '9'
             }
            @{
                TestInput = 58
                ToJson = '":"'
                FromJson = ':'
             }
            @{
                TestInput = 59
                ToJson = '";"'
                FromJson = ';'
             }
            @{
                TestInput = 60
                ToJson = if ( $IsCoreCLR ) { '"<"' } else { '"\u003c"' }
                FromJson = '<'
             }
            @{
                TestInput = 61
                ToJson = '"="'
                FromJson = '='
             }
            @{
                TestInput = 62
                ToJson = if ( $IsCoreCLR ) { '">"' } else { '"\u003e"' }
                FromJson = '>'
             }
            @{
                TestInput = 63
                ToJson = '"?"'
                FromJson = '?'
             }
            @{
                TestInput = 64
                ToJson = '"@"'
                FromJson = '@'
             }
            @{
                TestInput = 65
                ToJson = '"A"'
                FromJson = 'A'
             }
            @{
                TestInput = 66
                ToJson = '"B"'
                FromJson = 'B'
             }
            @{
                TestInput = 67
                ToJson = '"C"'
                FromJson = 'C'
             }
            @{
                TestInput = 68
                ToJson = '"D"'
                FromJson = 'D'
             }
            @{
                TestInput = 69
                ToJson = '"E"'
                FromJson = 'E'
             }
            @{
                TestInput = 70
                ToJson = '"F"'
                FromJson = 'F'
             }
            @{
                TestInput = 71
                ToJson = '"G"'
                FromJson = 'G'
             }
            @{
                TestInput = 72
                ToJson = '"H"'
                FromJson = 'H'
             }
            @{
                TestInput = 73
                ToJson = '"I"'
                FromJson = 'I'
             }
            @{
                TestInput = 74
                ToJson = '"J"'
                FromJson = 'J'
             }
            @{
                TestInput = 75
                ToJson = '"K"'
                FromJson = 'K'
             }
            @{
                TestInput = 76
                ToJson = '"L"'
                FromJson = 'L'
             }
            @{
                TestInput = 77
                ToJson = '"M"'
                FromJson = 'M'
             }
            @{
                TestInput = 78
                ToJson = '"N"'
                FromJson = 'N'
             }
            @{
                TestInput = 79
                ToJson = '"O"'
                FromJson = 'O'
             }
            @{
                TestInput = 80
                ToJson = '"P"'
                FromJson = 'P'
             }
            @{
                TestInput = 81
                ToJson = '"Q"'
                FromJson = 'Q'
             }
            @{
                TestInput = 82
                ToJson = '"R"'
                FromJson = 'R'
             }
            @{
                TestInput = 83
                ToJson = '"S"'
                FromJson = 'S'
             }
            @{
                TestInput = 84
                ToJson = '"T"'
                FromJson = 'T'
             }
            @{
                TestInput = 85
                ToJson = '"U"'
                FromJson = 'U'
             }
            @{
                TestInput = 86
                ToJson = '"V"'
                FromJson = 'V'
             }
            @{
                TestInput = 87
                ToJson = '"W"'
                FromJson = 'W'
             }
            @{
                TestInput = 88
                ToJson = '"X"'
                FromJson = 'X'
             }
            @{
                TestInput = 89
                ToJson = '"Y"'
                FromJson = 'Y'
             }
            @{
                TestInput = 90
                ToJson = '"Z"'
                FromJson = 'Z'
             }
            @{
                TestInput = 91
                ToJson = '"["'
                FromJson = '['
             }
            @{
                TestInput = 92
                ToJson = '"\\"'
                FromJson = '\'
             }
            @{
                TestInput = 93
                ToJson = '"]"'
                FromJson = ']'
             }
            @{
                TestInput = 94
                ToJson = '"^"'
                FromJson = '^'
             }
            @{
                TestInput = 95
                ToJson = '"_"'
                FromJson = '_'
             }
            @{
                TestInput = 96
                ToJson = '"`"'
                FromJson = '`'
             }
            @{
                TestInput = 97
                ToJson = '"a"'
                FromJson = 'a'
             }
            @{
                TestInput = 98
                ToJson = '"b"'
                FromJson = 'b'
             }
            @{
                TestInput = 99
                ToJson = '"c"'
                FromJson = 'c'
             }
            @{
                TestInput = 100
                ToJson = '"d"'
                FromJson = 'd'
             }
            @{
                TestInput = 101
                ToJson = '"e"'
                FromJson = 'e'
             }
            @{
                TestInput = 102
                ToJson = '"f"'
                FromJson = 'f'
             }
            @{
                TestInput = 103
                ToJson = '"g"'
                FromJson = 'g'
             }
            @{
                TestInput = 104
                ToJson = '"h"'
                FromJson = 'h'
             }
            @{
                TestInput = 105
                ToJson = '"i"'
                FromJson = 'i'
             }
            @{
                TestInput = 106
                ToJson = '"j"'
                FromJson = 'j'
             }
            @{
                TestInput = 107
                ToJson = '"k"'
                FromJson = 'k'
             }
            @{
                TestInput = 108
                ToJson = '"l"'
                FromJson = 'l'
             }
            @{
                TestInput = 109
                ToJson = '"m"'
                FromJson = 'm'
             }
            @{
                TestInput = 110
                ToJson = '"n"'
                FromJson = 'n'
             }
            @{
                TestInput = 111
                ToJson = '"o"'
                FromJson = 'o'
             }
            @{
                TestInput = 112
                ToJson = '"p"'
                FromJson = 'p'
             }
            @{
                TestInput = 113
                ToJson = '"q"'
                FromJson = 'q'
             }
            @{
                TestInput = 114
                ToJson = '"r"'
                FromJson = 'r'
             }
            @{
                TestInput = 115
                ToJson = '"s"'
                FromJson = 's'
             }
            @{
                TestInput = 116
                ToJson = '"t"'
                FromJson = 't'
             }
            @{
                TestInput = 117
                ToJson = '"u"'
                FromJson = 'u'
             }
            @{
                TestInput = 118
                ToJson = '"v"'
                FromJson = 'v'
             }
            @{
                TestInput = 119
                ToJson = '"w"'
                FromJson = 'w'
             }
            @{
                TestInput = 120
                ToJson = '"x"'
                FromJson = 'x'
             }
            @{
                TestInput = 121
                ToJson = '"y"'
                FromJson = 'y'
             }
            @{
                TestInput = 122
                ToJson = '"z"'
                FromJson = 'z'
             }
            @{
                TestInput = 123
                ToJson = '"{"'
                FromJson = '{'
             }
            @{
                TestInput = 124
                ToJson = '"|"'
                FromJson = '|'
             }
            @{
                TestInput = 125
                ToJson = '"}"'
                FromJson = '}'
             }
            @{
                TestInput = 126
                ToJson = '"~"'
                FromJson = '~'
             }
            @{
                TestInput = 127
                ToJson = '""'
                FromJson = ''
             }
        )

        function ValidateJsonSerializationForAsciiValues
        {
            param ($testCase)

            It "Validate 'ConvertTo-Json ([char]$($testCase.TestInput))', and 'ConvertTo-Json ([char]$($testCase.TestInput)) | ConvertFrom-Json'" {

                $result = @{
                    ToJson = ConvertTo-Json ([char]$testCase.TestInput)
                    FromJson = ConvertTo-Json ([char]$testCase.TestInput) | ConvertFrom-Json
                }

                if ($testCase.FromJson)
                {
                    $result.FromJson | Should Be $testCase.FromJson
                }
                else
                {
                    # There are two char for which the deserialized object must be compare to the serialized one via "Should Match"
                    # These values are [char]0 and [char]13.
                    $result.FromJson | Should Match $testCase.FromJson
                }
                $result.ToJson | Should Be $testCase.ToJson
            }
        }

        foreach ($testCase in $testCases)
        {
            ValidateJsonSerializationForAsciiValues $testCase
        }
    }

    Context "Validate Json serialization for types" {

        $testCases = @(

            ## Decimal types - Decimals are a 128-bit data type
            @{
                TestInput = '[decimal]::MinValue'
                FromJson = [decimal]::MinValue
                ToJson = [decimal]::MinValue
            }
            @{
                TestInput = '[decimal]::MaxValue'
                FromJson = [decimal]::MaxValue
                ToJson = [decimal]::MaxValue
            }

            # An sbyte is a signed 8-bit integer, and it ranges from -128 to 127.
            # A byte is an unsigned 8-bit integer that ranges from 0 to 255
            @{
                TestInput = '[byte]::MinValue'
                FromJson = [byte]::MinValue
                ToJson = [byte]::MinValue
            }
            @{
                TestInput = '[byte]::MaxValue'
                FromJson = [byte]::MaxValue
                ToJson = [byte]::MaxValue
            }
            @{
                TestInput = '[sbyte]::MinValue'
                FromJson = [sbyte]::MinValue
                ToJson = [sbyte]::MinValue
            }
            @{
                TestInput = '[sbyte]::MaxValue'
                FromJson = [sbyte]::MaxValue
                ToJson = [sbyte]::MaxValue
            }
            @{
                TestInput = '[char]::MinValue'
                FromJson = $null
                ToJson = 'null'
            }
            @{
                TestInput = '[char]::MaxValue - 1'
                FromJson = [char]::MaxValue - 1
                ToJson = [char]::MaxValue - 1
            }
            @{
                TestInput = '[string]::Empty'
                FromJson = [string]::Empty
                ToJson = '""'
            }
            @{
                TestInput = '[string]"hello"'
                FromJson = [string]"hello"
                ToJson = '"hello"'
            }
        
            # Int, int32, uint32, uint16, int16

            # 32-bit signed integer
            @{
                TestInput = '[int]::MaxValue'
                FromJson = [int]::MaxValue
                ToJson = [int]::MaxValue
            }
            @{
                TestInput = '[int]::MinValue'
                FromJson = [int]::MinValue
                ToJson = [int]::MinValue
            }
            @{
                TestInput = '[int32]::MaxValue'
                FromJson = [int32]::MaxValue
                ToJson = [int32]::MaxValue
            }
            @{
                TestInput = '[int32]::MinValue'
                FromJson = [int32]::MinValue
                ToJson = [int32]::MinValue
            }

            # 32-bit unsigned integer
            @{
                TestInput = '[uint32]::MaxValue'
                FromJson = [uint32]::MaxValue
                ToJson = [uint32]::MaxValue
            }
            @{
                TestInput = '[uint32]::MinValue'
                FromJson = [uint32]::MinValue
                ToJson = [uint32]::MinValue
            }

            # 16-bit unsigned integer
            @{
                TestInput = '[int16]::MinValue'
                FromJson = [int16]::MinValue
                ToJson = [int16]::MinValue
            }
            @{
                TestInput = '[uint16]::MaxValue'
                FromJson = [uint16]::MaxValue
                ToJson = [uint16]::MaxValue
            }

            # 64-bit unsigned integer
            @{
                TestInput = '[uint64]::MinValue'
                FromJson = [uint64]::MinValue
                ToJson = [uint64]::MinValue
            }
            @{
                TestInput = '[uint64]::MinValue'
                FromJson = [uint64]::MinValue
                ToJson = [uint64]::MinValue
            }

            # 64 bit signed integer
            @{
                TestInput = '[int64]::MaxValue'
                FromJson = [int64]::MaxValue
                ToJson = [int64]::MaxValue
            }
            @{
                TestInput = '[int64]::MinValue'
                FromJson = [int64]::MinValue
                ToJson = [int64]::MinValue
            }
            @{
                TestInput = '[long]::MaxValue'
                FromJson = [long]::MaxValue
                ToJson = [long]::MaxValue
            }
            @{
                TestInput = '[long]::MinValue'
                FromJson = [long]::MinValue
                ToJson = [long]::MinValue
            }

            # Bool
            @{
                TestInput = '[bool](1)'
                FromJson = [bool](1)
                ToJson = $true
            }
            @{
                TestInput = '[bool](0)'
                FromJson = $false
                ToJson = 'False'
            }

            # Decimal
            @{
                TestInput = '[decimal]::MaxValue'
                FromJson = [decimal]::MaxValue
                ToJson = [decimal]::MaxValue
            }
            @{
                TestInput = '[decimal]::MinValue'
                FromJson = [decimal]::MinValue
                ToJson = [decimal]::MinValue
            }

            # Single
            @{
                TestInput = '[single]::MaxValue'
                FromJson = "3.40282347E+38"
                ToJson = "3.40282347E+38"
            }
            @{
                TestInput = '[single]::MinValue'
                FromJson = "-3.40282347E+38"
                ToJson = "-3.40282347E+38"
            }

            # Double
            @{
                TestInput = '[double]::MaxValue'
                FromJson = [double]::MaxValue
                ToJson = [double]::MaxValue
            }
            @{
                TestInput = '[double]::MinValue'
                FromJson = [double]::MinValue
                ToJson = [double]::MinValue
            }
        )

        function ValidateJsonSerialization
        {
            param ($testCase)

            if ( $TestCase.TestInput -eq "[char]::MinValue" ) { $pending = $true } else { $pending = $false }
            It "Validate '$($testCase.TestInput) | ConvertTo-Json' and '$($testCase.TestInput) | ConvertTo-Json | ConvertFrom-Json'" -pending:$pending {

                # The test case input is executed via invoke-expression. Then, we use this value as an input to ConvertTo-Json, 
                # and the result is saved into in the $result.ToJson variable. Lastly, this value is deserialized back using 
                # ConvertFrom-Json, and the value is saved to $result.FromJson for comparison.

                $expression = Invoke-Expression $testCase.TestInput
                $result = @{
                    ToJson = $expression | ConvertTo-Json
                    FromJson = $expression | ConvertTo-Json | ConvertFrom-Json
                }

                $result.ToJson | Should Be $testCase.ToJson
                $result.FromJson | Should Be $testCase.FromJson
            }
        }

        foreach ($testCase in $testCases)
        {
            ValidateJsonSerialization $testCase
        }
    }


    Context "Validate Json Serialization for 'Get-CimClass' and 'Get-Command'" {

        function ValidateProperties
        {
            param (
                $serialized,
                $expected,
                $properties
            )

            # Validate that the two collections are the same size.
            $expected.Count | Should Be $serialized.Count

            for ($index = 0; $index -lt $serialized.Count; $index++)
            {
                $serializedObject = $serialized[$index]
                $expectedObject = $expected[$index]
                foreach ($property in $properties)
                {
                    # Write-Verbose "Validating $property" -Verbose
                    if ($property -eq "Qualifiers")
                    {
                        $serializedObject.$property.Count | Should Be $expectedObject.$property.Count
                    }
                    else
                    {
                        $serializedObject.$property | Should Be $expectedObject.$property
                    }
                }
            }
        }

        It "Validate that CimClass Properties for win32_bios can be serialized using ConvertTo-Json and ConvertFrom-Json" -skip {

            $class = Get-CimClass win32_bios

            $result = @{
                Expected = $class.CimClassProperties | ForEach-Object {$_}
                SerializedViaJson = $class.CimClassProperties | ConvertTo-Json -Depth 10 | ConvertFrom-Json
            }

            $propertiesToValidate = @("Name", "Flags", "Qualifiers", "ReferenceClassName")

            ValidateProperties -serialized $result.SerializedViaJson -expected $result.Expected -properties $propertiesToValidate
        }

        It "Validate 'Get-Command Get-help' output with Json conversion" {

            $result = @{
                Expected = @(Get-Command Get-help)
                SerializedViaJson = @(Get-Command Get-help | ConvertTo-Json | ConvertFrom-Json)
            }

            $propertiesToValidate = @("Name", "Noun", "Verb")
            ValidateProperties -serialized $result.SerializedViaJson -expected $result.Expected -properties $propertiesToValidate
        }

        It "Validate 'Get-Command Get-Help, Get-command, Get-Member' output with Json conversion" {
            
            $result = @{
                Expected = @(Get-Command Get-Help, Get-Command, Get-Member)
                SerializedViaJson = @(Get-Command Get-Help, Get-Command, Get-Member) | ConvertTo-Json | ConvertFrom-Json
            }

            $propertiesToValidate = @("Name", "Source", "HelpFile")
            ValidateProperties -serialized $result.SerializedViaJson -expected $result.Expected -properties $propertiesToValidate
        }
        
        It "ConvertTo-JSON a dictionary of arrays" {
            $a = 1..5
            $b = 6..10

            # remove whitespace (and newline/cr) which reduces the complexity of a
            # cross-plat test
            $actual = ([ordered]@{'a'=$a;'b'=$b} | ConvertTo-Json) -replace "\s"
            $expected = @'
{
    "a":  [
              1,
              2,
              3,
              4,
              5
          ],
    "b":  [
              6,
              7,
              8,
              9,
              10
          ]
}
'@
            $expectedNoWhiteSpace = $expected -replace "\s"
            $actual | Should Be $expectedNoWhiteSpace
        }
    }
}

Describe "Json Bug fixes"  -Tags "Feature" {

    function RunJsonTest
    {
        param ($testCase)

        It "$($testCase.Name)" {

            # Create a nested object
            $start = 1
            $previous = @{
                Depth = $($testCase.NumberOfElements)
                Next = $null
            }

            ($($testCase.NumberOfElements)-1)..$start | foreach {
                $current = @{
                    Depth = $_
                    Next = $previous
                }
                $previous = $current
            }

            if ($testCase.ShouldThrow)
            {
                try
                {
                    $previous | ConvertTo-Json -Depth $testCase.MaxDepth
                    throw "CodeExecuted"
                }
                catch
                {
                    $_.FullyQualifiedErrorId | Should Be $testCase.FullyQualifiedErrorId
                }
            }
            else
            {
                $theError = $null
                try
                {
                    $previous | ConvertTo-Json -Depth $testCase.MaxDepth | ConvertFrom-Json
                }
                catch
                {
                    $theError = $_
                }
                $theError | Should Be $null
            }
        }
    }

    $testCases = @(
        @{
            Name = "ConvertTo-Json -Depth 101 throws MaximumAllowedDepthReached when the user specifies a depth greater than 100."
            NumberOfElements = 10
            MaxDepth = 101
            FullyQualifiedErrorId = "ReachedMaximumDepthAllowed,Microsoft.PowerShell.Commands.ConvertToJsonCommand"
            ShouldThrow = $true
        }
        @{
            Name = "ConvertTo-Json and ConvertFrom-Json work for any depth less than or equal to 100."
            NumberOfElements = 100
            MaxDepth = 100
            ShouldThrow = $false
        }
        @{
            Name = "ConvertTo-Json and ConvertFrom-Json work for depth 100 with an object larger than 100."
            NumberOfElements = 105
            MaxDepth = 100
            ShouldThrow = $false
        }
    )

    foreach ($testCase in $testCases)
    {
        RunJsonTest $testCase
    }
}
