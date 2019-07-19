param(
    [Parameter(Mandatory)] [string] $Destination,
    [Parameter(Mandatory)] [string] $Token
)

function Receive-BuildPackage {
    param(
        [Parameter(Mandatory)] [string] $Path,
        [Parameter(Mandatory)] [string] $Token
    )

    Write-Verbose -Verbose "Start: Receive-BuildPackage"

    $base64Token = [Convert]::ToBase64String([System.Text.ASCIIEncoding]::ASCII.GetBytes(("{0}:{1}" -f [string]::Empty, $token)))
    $authValue = "Basic", $base64Token -join " "
    $header = @{ "Authorization" = $authValue }

    $latestBuildId = (Invoke-RestMethod 'https://dev.azure.com/mscodehub/powershellcore/_apis/build/builds?definitions=696&queryOrder=finishTimeDescending&$top=1' -Headers $header).Value.Id

    $productArtifactUrl = "https://dev.azure.com/mscodehub/powershellcore/_apis/build/builds/$latestBuildId/artifacts?artifactName=finalResults"

    $productArtifactDownloadUrl = (Invoke-RestMethod $productArtifactUrl -Headers $header -PreserveAuthorizationOnRedirect).Resource.downloadUrl
    Write-Verbose -Verbose "productArtifactDownloadUrl : $productArtifactDownloadUrl to $path"

    $productDownload = Start-ThreadJob -ScriptBlock {
        $wc = [System.Net.Webclient]::new()
        $wc.Headers.Add([System.Net.HttpRequestHeader]::Authorization, $using:authValue)
        $wc.DownloadFile($using:productArtifactDownloadUrl, $using:Path)
    }

    Write-Verbose -Verbose "Waiting for downloading build artifacts with timeout of 1200 seconds."
    Wait-Job $productDownload -Timeout 1200

    if ($productDownload.State -ne 'Completed') {
        Write-Log Error "Downloading build artifacts timed out."
        throw "Downloading build artifacts timed out."
    }

    Write-Verbose -Verbose "Downloading build artifacts completed."

    Write-Verbose -Verbose "End: Receive-BuildPackage"
}

function Get-PSExecutablePath {
    param(
        [Parameter(Mandatory)] [string] $ZipPath,
        [Parameter(Mandatory)] [string] $Destination
    )

    Write-Verbose -Verbose "Expanding $ZipPath"

    Expand-Archive -Path $ZipPath -DestinationPath $Destination -Force

    $packageName = if ($IsWindows) {
        "powershell-*-win-x64.zip"
    } elseif ($IsLinux) {
        "powershell-*-linux-x64.tar.gz"
    } else {
        "powershell-*-osx-x64.tar.gz"
    }

    $zipArtifact = Resolve-Path (Join-Path $Destination "finalResults" -AdditionalChildPath $packageName)

    $pwshFolder = Join-Path $Destination "ps"

    $execName = if ($IsWindows) {
        "pwsh.exe"
    } else {
        "pwsh"
    }

    $pwshPath = Join-Path $pwshFolder $execName

    Write-Verbose -Verbose "Expanding $zipArtifact to $pwshFolder"

    if ($IsWindows) {
        Expand-Archive -Path $zipArtifact -DestinationPath "$pwshFolder" -Force
    }
    else {
        tar -xvf $zipArtifact -C $pwshFolder
    }

    if (Test-Path $pwshPath) {
        return $pwshPath
    } else {
        throw "pwsh executable not found at: $pwshPath"
    }
}

$topLevelZipPath = Join-Path $Destination "product.zip"
Receive-BuildPackage -Path $topLevelZipPath -Token $Token
Get-PSExecutablePath -ZipPath $topLevelZipPath -Destination $Destination
