$ErrorActionPreference = "Stop"

Write-Host "Preparing eShopModernizing Windsurf worktree"

if ($env:ROOT_WORKSPACE_PATH -and (Test-Path $env:ROOT_WORKSPACE_PATH)) {
    foreach ($file in @(".env", ".env.local", ".env.development")) {
        $source = Join-Path $env:ROOT_WORKSPACE_PATH $file
        if ((Test-Path $source) -and -not (Test-Path $file)) {
            Copy-Item $source $file
            Write-Host "Copied $file from root workspace"
        }
    }
}

if (Get-Command git -ErrorAction SilentlyContinue) {
    git worktree list
}

if (Get-Command nuget -ErrorAction SilentlyContinue) {
    foreach ($solution in @(
        "eShopModernizedMVCSolution/eShopModernizedMVC.sln",
        "eShopModernizedWebFormsSolution/eShopModernizedWebForms.sln",
        "eShopModernizedNTier/eShopModernizedNTier.sln"
    )) {
        if (Test-Path $solution) {
            Write-Host "Restoring $solution"
            nuget restore $solution
        }
    }
} else {
    Write-Host "nuget not found; skipping package restore"
}

Write-Host "Worktree ready for local Cascade agent work"
