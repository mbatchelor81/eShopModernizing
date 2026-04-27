# docker-entrypoint.ps1
# Substitutes environment variables into Web.config before IIS starts.
# Uses XML parsing instead of regex to avoid special character issues.

$webConfigPath = "C:\inetpub\wwwroot\Web.config"

if (Test-Path $webConfigPath) {
    [xml]$xml = Get-Content $webConfigPath

    # Replace SESSION_DB_HOST placeholder in sessionState connection string
    if ($env:SESSION_DB_HOST) {
        $sessionState = $xml.SelectSingleNode("//system.web/sessionState")
        if ($sessionState -and $sessionState.sqlConnectionString) {
            $sessionState.sqlConnectionString = $sessionState.sqlConnectionString -replace 'SESSION_DB_HOST', $env:SESSION_DB_HOST
            Write-Host "Replaced SESSION_DB_HOST in Web.config"
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

    $xml.Save($webConfigPath)
}

# Start IIS and wait
Write-Host "Starting IIS..."
Start-Service W3SVC
Write-Host "IIS started. Monitoring..."

while ($true) {
    Start-Sleep -Seconds 3600
}
