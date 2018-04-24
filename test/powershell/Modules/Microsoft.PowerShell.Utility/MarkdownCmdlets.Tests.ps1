# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

Describe 'ConvertFrom-Markdown tests' -Tags 'CI' {

    BeforeAll {
        $esc = [char]0x1b

        function GetExpectedString
        {
            [CmdletBinding()]
            param(
            [ValidateSet(
                "Header1", "Header2", "Header3", "Header4", "Header5", "Header6",
                "Code", "CodeBlock",
                "Link", "Image",
                "Bold", "Italics")]
            [Parameter()]
            [string] $ElementType,

            [Parameter(ParameterSetName = "Header")]
            [string] $Text,

            [Parameter(ParameterSetName = "Code")]
            [string] $CodeFormatString,

            [Parameter(ParameterSetName = "Code")]
            [string] $CodeText
            )

            switch($elementType)
            {
                "Header1" { "$esc[7m$text$esc[0m`n`n" }
                "Header2" { "$esc[4;93m$text$esc[0m`n`n" }
                "Header3" { "$esc[4;94m$text$esc[0m`n`n" }
                "Header4" { "$esc[4;95m$text$esc[0m`n`n" }
                "Header5" { "$esc[4;96m$text$esc[0m`n`n" }
                "Header6" { "$esc[4;97m$text$esc[0m`n`n" }

                "Code" { ($CodeFormatString -f "$esc[48;2;155;155;155;38;2;30;30;30m$CodeText$esc[0m") + "`n`n" }
                "CodeBlock" {
                    $expectedString = @()
                    $CodeText -split "`n" | ForEach-Object { $expectedString += "$esc[48;2;155;155;155;38;2;30;30;30m$_$esc[500@[0m" }
                    $expectedString -join "`n"
                }
            }
        }

        function GetExpectedHTML
        {
            [CmdletBinding()]
            param(
            [ValidateSet(
                "Header1", "Header2", "Header3", "Header4", "Header5", "Header6",
                "Code", "CodeBlock",
                "Link", "Image",
                "Bold", "Italics")]
            [Parameter()]
            [string] $ElementType,

            [Parameter(ParameterSetName = "Header")]
            [string] $Text,

            [Parameter(ParameterSetName = "Code")]
            [string] $CodeFormatString,

            [Parameter(ParameterSetName = "Code")]
            [string] $CodeText
            )

            $id = $Text.Replace(" ","-").ToLowerInvariant()

            switch($elementType)
            {
                "Header1" { "<h1 id=`"$id`">$text</h1>`n" }
                "Header2" { "<h2 id=`"$id`">$text</h2>`n" }
                "Header3" { "<h3 id=`"$id`">$text</h3>`n" }
                "Header4" { "<h4 id=`"$id`">$text</h4>`n" }
                "Header5" { "<h5 id=`"$id`">$text</h5>`n" }
                "Header6" { "<h6 id=`"$id`">$text</h6>`n" }

                "Code" { "<p>" + ($CodeFormatString -f "<code>$CodeText</code>") + "</p>`n" }
                "CodeBlock" { "<pre><code>$CodeText</code></pre>`n" }
            }
        }
    }

    Context 'Basic tests' {
        BeforeAll {

            $codeBlock = @'
```
bool function()
{
}
```
'@

            $codeBlockText = @'
bool function()
{
}
'@

                $TestCases = @(
                    @{ element = 'Header1'; InputMD = '# Header 1'; Text = 'Header 1' }
                    @{ element = 'Header2'; InputMD = '## Header 2'; Text = 'Header 2' }
                    @{ element = 'Header3'; InputMD = '### Header 3'; Text = 'Header 3' }
                    @{ element = 'Header4'; InputMD = '#### Header 4'; Text = 'Header 4' }
                    @{ element = 'Header5'; InputMD = '##### Header 5'; Text = 'Header 5' }
                    @{ element = 'Header6'; InputMD = '###### Header 6'; Text = 'Header 6' }
                    @{ element = 'Code'; InputMD = 'This is a `code` sample'; CodeFormatString = 'This is a {0} sample'; CodeText = 'code'}
                    @{ element = 'CodeBlock'; InputMD = $codeBlock; CodeText = $codeBlockText }
                )
        }


        It 'Can convert header element : <element> to vt100 using pipeline input' -TestCases $TestCases {
            param($element, $inputMD, $text, $codeFormatString, $codeText)

            $output = $inputMD | ConvertFrom-Markdown -AsVT100EncodedString

            if($element -like 'Header?')
            {
                $expectedString = GetExpectedString -ElementType $element -Text $text
            }
            elseif($element -eq 'Code')
            {
                $expectedString = GetExpectedString -ElementType $element -CodeFormatString $codeFormatString -CodeText $codeText
            }
            elseif($element -eq 'CodeBlock')
            {
                $expectedString = GetExpectedString -ElementType $element -CodeText $codeText
            }

            $output.VT100EncodedString | Should BeExactly $expectedString
        }

        It 'Can convert header element : <element> to HTML using pipeline input' -TestCases $TestCases {
            param($element, $inputMD, $text, $codeFormatString, $codeText)

            $output = $inputMD | ConvertFrom-Markdown

            if($element -like 'Header?')
            {
                $expectedString = GetExpectedHTML -ElementType $element -Text $text
            }
            elseif($element -eq 'Code')
            {
                $expectedString = GetExpectedHTML -ElementType $element -CodeFormatString $codeFormatString -CodeText $codeText
            }
            elseif($element -eq 'CodeBlock')
            {
                $expectedString = GetExpectedHTML -ElementType $element -CodeText $codeText
            }

            $output.Html | Should BeExactly $expectedString
        }
    }
}
