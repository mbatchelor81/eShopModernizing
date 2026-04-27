# docker-entrypoint.ps1
# Substitutes environment variables into Web.config before IIS starts.
# Uses XML parsing instead of regex to avoid special character issues.

$webConfigPath = "C:\inetpub\wwwroot\Web.config"

if (Test-Path $webConfigPath) {
    [xml]$xml = Get-Content $webConfigPath

    # Replace session state connection string if provided
    if ($env:SessionStateConnStr) {
        $sessionState = $xml.SelectSingleNode("//system.web/sessionState")
        if ($sessionState) {
            $sessionState.SetAttribute("sqlConnectionString", $env:SessionStateConnStr)
            Write-Host "Replaced sessionState connection string in Web.config"
        }
    }

    # Replace CatalogDBContext connection string if provided
    if ($env:ConnectionString) {
        $node = $xml.SelectSingleNode("//connectionStrings/add[@name='CatalogDBContext']")
        if ($node) {
            $node.SetAttribute("connectionString", $env:ConnectionString)
            Write-Host "Replaced CatalogDBContext connection string in Web.config"
        }
    }

    # Substitute appSettings from environment variables.
    # ConfigurationManager.AppSettings does not read env vars in IIS containers,
    # so we write them directly into Web.config's <appSettings> section.
    $appSettingsKeys = @(
        "UseMockData",
        "UseCustomizationData",
        "UseAzureStorage",
        "StorageConnectionString",
        "UseAzureActiveDirectory",
        "AzureActiveDirectoryClientId",
        "AzureActiveDirectoryTenant",
        "PostLogoutRedirectUri"
    )

    foreach ($key in $appSettingsKeys) {
        $value = [System.Environment]::GetEnvironmentVariable($key)
        if ($null -ne $value) {
            $node = $xml.SelectSingleNode("//appSettings/add[@key='$key']")
            if ($node) {
                $node.SetAttribute("value", $value)
            } else {
                $appSettings = $xml.SelectSingleNode("//appSettings")
                if ($appSettings) {
                    $newNode = $xml.CreateElement("add")
                    $newNode.SetAttribute("key", $key)
                    $newNode.SetAttribute("value", $value)
                    $appSettings.AppendChild($newNode) | Out-Null
                }
            }
            Write-Host "Set appSetting '$key' in Web.config"
        }
    }

    $xml.Save($webConfigPath)
}

# Start IIS and wait
Write-Host "Starting IIS..."
Start-Service W3SVC
Write-Host "IIS started. Monitoring..."

while ($true) {
    Start-Sleep -Seconds 3600
}
