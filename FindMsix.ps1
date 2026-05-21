$SolutionDir = $PSScriptRoot
$ReleaseFolderPath = Join-Path $SolutionDir "Release"
Write-Host "Opening Release folder: $ReleaseFolderPath" -ForegroundColor Green
if (-not (Test-Path $ReleaseFolderPath)) { New-Item -Path $ReleaseFolderPath -ItemType Directory -Force | Out-Null }
Start-Process explorer.exe -ArgumentList $ReleaseFolderPath
