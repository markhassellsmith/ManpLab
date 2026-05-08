# ManpLab Portable Package Creator
# Quick script to create portable ZIP distribution only

param(
    [string]$Configuration = "Release",
    [string]$Platform = "x64",
    [string]$Version = "1.0.0"
)

$ErrorActionPreference = "Stop"
$SolutionDir = Split-Path $PSScriptRoot -Parent
$ArtifactsDir = Join-Path $SolutionDir "Build\Artifacts"

Write-Host "Creating ManpLab Portable v$Version..." -ForegroundColor Cyan

# Clean and create artifacts directory
if (Test-Path $ArtifactsDir) {
    Remove-Item $ArtifactsDir -Recurse -Force
}
New-Item -ItemType Directory -Path $ArtifactsDir -Force | Out-Null

$PortableDir = Join-Path $ArtifactsDir "ManpLab-Portable"

# Publish self-contained
Write-Host "Publishing self-contained application..." -ForegroundColor Yellow
& dotnet publish "$SolutionDir\ManpWinUI\ManpWinUI.csproj" `
    -c $Configuration `
    -p:Platform=$Platform `
    -p:WindowsPackageType=None `
    -p:EnableMsixTooling=false `
    -p:SelfContained=true `
    -p:RuntimeIdentifier=win-x64 `
    -p:PublishSingleFile=false `
    -p:PublishTrimmed=false `
    -o $PortableDir

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Create README
$Readme = @"
ManpLab v$Version - Portable Edition

QUICK START:
1. Run ManpWinUI.exe
2. Explore 300 fractals with deep zoom capabilities!

REQUIREMENTS:
- Windows 10 (1809+) or Windows 11
- 64-bit processor

Full documentation: https://github.com/markhassellsmith/ManpLab
"@

Set-Content -Path (Join-Path $PortableDir "README.txt") -Value $Readme

# Copy license
if (Test-Path "$SolutionDir\LICENSE") {
    Copy-Item "$SolutionDir\LICENSE" -Destination $PortableDir
}

# Create ZIP
$ZipName = "ManpLab-v$Version-Portable.zip"
$ZipPath = Join-Path $ArtifactsDir $ZipName

Write-Host "Creating ZIP archive..." -ForegroundColor Yellow
Compress-Archive -Path "$PortableDir\*" -DestinationPath $ZipPath -CompressionLevel Optimal -Force

$ZipSize = [math]::Round((Get-Item $ZipPath).Length / 1MB, 2)
Write-Host ""
Write-Host "✓ Success! Created: $ZipName ($ZipSize MB)" -ForegroundColor Green
Write-Host "Location: $ArtifactsDir" -ForegroundColor Cyan
