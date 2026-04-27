# docker-entrypoint.ps1
# Substitutes environment variables into Web.config before IIS starts.

$webConfig = "C:\inetpub\wwwroot\Web.config"

if (Test-Path $webConfig) {
    $content = Get-Content $webConfig -Raw

    # Replace SESSION_DB_HOST placeholder with environment variable if set
    if ($env:SESSION_DB_HOST) {
        $content = $content -replace 'SESSION_DB_HOST', $env:SESSION_DB_HOST
        Write-Host "Replaced SESSION_DB_HOST in Web.config"
    }

    # Replace connection string if provided via environment variable
    if ($env:ConnectionString) {
        $content = $content -replace '(?<=name="CatalogDBContext"\s+connectionString=")[^"]*', [regex]::Escape($env:ConnectionString)
        Write-Host "Replaced CatalogDBContext connection string in Web.config"
    }

    Set-Content $webConfig $content
}

# Start IIS and wait
Write-Host "Starting IIS..."
Start-Service W3SVC
Write-Host "IIS started. Monitoring..."

while ($true) {
    Start-Sleep -Seconds 3600
}
