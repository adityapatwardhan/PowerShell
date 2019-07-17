

function Receive-BuildPackage {
    param(
        [Parameter(Mandatory)] [string] $Build,
        [Parameter(Mandatory)] [string] $ProductArtifactsPath,
        [Parameter(Mandatory)] [string] $Token
    )

    Write-Verbose -Verbose "Start: Receive-BuildPackage"

    $base64Token = [Convert]::ToBase64String([System.Text.ASCIIEncoding]::ASCII.GetBytes(("{0}:{1}" -f [string]::Empty, $token)))
    $authValue = "Basic", $base64Token -join " "
    $header = @{ "Authorization" = $authValue }

    $productArtifactUrl = "https://dev.azure.com/mscodehub/powershellcore/_apis/build/builds/$Build/artifacts?artifactName=finalResults"

    $productArtifactDownloadUrl = (Invoke-RestMethod $productArtifactUrl -Headers $header -PreserveAuthorizationOnRedirect).Resource.downloadUrl
    Write-Verbose -Verbose "productArtifactDownloadUrl : $productArtifactDownloadUrl"

    $productDownload = Start-ThreadJob -ScriptBlock {
        $wc = [System.Net.Webclient]::new()
        $wc.Headers.Add([System.Net.HttpRequestHeader]::Authorization, $using:authValue)
        $wc.DownloadFile($using:productArtifactDownloadUrl, $using:ProductArtifactsPath)
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
