# ManpLab Release Packaging Script
# Creates both portable ZIP and MSIX distributions

param(
    [string]$Configuration = "Release",
    [string]$Platform = "x64",
    [string]$Version = "1.0.0"
)

$ErrorActionPreference = "Stop"
$SolutionDir = Split-Path $PSScriptRoot -Parent
$OutputDir = Join-Path $SolutionDir "Build\Output"
$ArtifactsDir = Join-Path $SolutionDir "Build\Artifacts"

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "ManpLab Release Packaging" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Version: $Version"
Write-Host "Configuration: $Configuration"
Write-Host "Platform: $Platform"
Write-Host ""

# Clean previous artifacts
Write-Host "Cleaning previous artifacts..." -ForegroundColor Yellow
if (Test-Path $ArtifactsDir) {
    Remove-Item $ArtifactsDir -Recurse -Force
}
New-Item -ItemType Directory -Path $ArtifactsDir -Force | Out-Null

# Build the solution
Write-Host "Building solution..." -ForegroundColor Yellow
$BuildOutput = & dotnet build "$SolutionDir\ManpWinUI\ManpWinUI.csproj" `
    -c $Configuration `
    -p:Platform=$Platform `
    -p:WindowsPackageType=None `
    -p:EnableMsixTooling=false `
    2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    Write-Host $BuildOutput
    exit 1
}

Write-Host "Build successful!" -ForegroundColor Green
Write-Host ""

# ============================================
# OPTION 1: Portable ZIP Distribution
# ============================================
Write-Host "Creating portable ZIP distribution..." -ForegroundColor Yellow

$PublishDir = Join-Path $SolutionDir "ManpWinUI\bin\$Platform\$Configuration\net10.0-windows10.0.22621.0\win-$Platform"
$PortableDir = Join-Path $ArtifactsDir "ManpLab-Portable"

# Publish self-contained
Write-Host "Publishing self-contained application..." -ForegroundColor Gray
& dotnet publish "$SolutionDir\ManpWinUI\ManpWinUI.csproj" `
    -c $Configuration `
    -p:Platform=$Platform `
    -p:WindowsPackageType=None `
    -p:EnableMsixTooling=false `
    -p:SelfContained=true `
    -p:PublishSingleFile=false `
    -p:PublishTrimmed=false `
    -o $PortableDir `
    --no-build

if ($LASTEXITCODE -ne 0) {
    Write-Host "Publish failed!" -ForegroundColor Red
    exit 1
}

# Create README for portable distribution
$PortableReadme = @"
# ManpLab v$Version - Portable Distribution

## Quick Start

1. Extract this folder anywhere on your computer
2. Run ManpWinUI.exe
3. No installation required!

## Requirements

- Windows 10 (version 1809 or later) or Windows 11
- 64-bit (x64) processor

## What's Included

This portable distribution includes:
- ManpWinUI application
- All required Windows App SDK runtime components
- Native C++ fractal rendering engine
- All dependencies (MPFR, GMP, FFmpeg)

## Troubleshooting

If you see "The application was unable to start correctly":
1. Make sure you extracted ALL files from the ZIP
2. Check that you're running on 64-bit Windows
3. Ensure Windows is up to date (Windows Update)

## Documentation

Full documentation: https://github.com/markhassellsmith/ManpLab

## License

MIT License - See LICENSE file for details.
"@

Set-Content -Path (Join-Path $PortableDir "README.txt") -Value $PortableReadme

# Copy license and documentation
if (Test-Path "$SolutionDir\LICENSE") {
    Copy-Item "$SolutionDir\LICENSE" -Destination $PortableDir
}

# Create ZIP
$ZipName = "ManpLab-v$Version-Windows-x64-Portable.zip"
$ZipPath = Join-Path $ArtifactsDir $ZipName

Write-Host "Creating ZIP archive: $ZipName..." -ForegroundColor Gray
Compress-Archive -Path "$PortableDir\*" -DestinationPath $ZipPath -CompressionLevel Optimal -Force

$ZipSize = [math]::Round((Get-Item $ZipPath).Length / 1MB, 2)
Write-Host "✓ Portable ZIP created: $ZipSize MB" -ForegroundColor Green
Write-Host ""

# ============================================
# OPTION 2: MSIX Package
# ============================================
Write-Host "Creating MSIX package..." -ForegroundColor Yellow

# Build with MSIX enabled
Write-Host "Building MSIX package..." -ForegroundColor Gray
& msbuild "$SolutionDir\ManpWinUI\ManpWinUI.csproj" `
    /p:Configuration=$Configuration `
    /p:Platform=$Platform `
    /p:EnableMsixTooling=true `
    /p:WindowsPackageType=MSIX `
    /p:AppxPackageSigningEnabled=false `
    /p:GenerateAppInstallerFile=false `
    /p:UapAppxPackageBuildMode=SideloadOnly `
    /t:Build `
    /t:_GenerateAppxPackage `
    /v:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "MSIX build failed!" -ForegroundColor Red
    exit 1
}

# Find the generated MSIX
$MsixSource = Get-ChildItem -Path "$SolutionDir\ManpWinUI\AppPackages" -Filter "*.msix" -Recurse | 
    Select-Object -First 1

if ($MsixSource) {
    $MsixName = "ManpLab-v$Version-Windows-x64.msix"
    $MsixPath = Join-Path $ArtifactsDir $MsixName
    Copy-Item $MsixSource.FullName -Destination $MsixPath

    $MsixSize = [math]::Round((Get-Item $MsixPath).Length / 1MB, 2)
    Write-Host "✓ MSIX package created: $MsixSize MB" -ForegroundColor Green

    # Create installation guide
    $MsixReadme = @"
# ManpLab v$Version - MSIX Installation Guide

## About MSIX Distribution

This is a modern Windows app package that provides:
- Clean installation/uninstallation
- Automatic updates (future versions)
- Sandbox security

## Installation Steps

### Option A: Direct Installation (Easiest)

1. Double-click the .msix file
2. Click "Install" when prompted
3. **You will see a security warning** - this is normal for unsigned apps
4. Click "Install anyway" or "More info" → "Install anyway"

### Why the Security Warning?

ManpLab is not commercially code-signed (signing costs \$200-500/year).
This is common for open-source educational software. The warning does NOT 
mean the software is unsafe - it just means Microsoft hasn't verified the publisher.

### Option B: PowerShell Installation

If direct installation doesn't work:

1. Right-click the .msix file
2. Select "Open with" → "PowerShell"
3. Run: ``Add-AppxPackage -Path "ManpLab-v$Version-Windows-x64.msix"``

### Option C: Developer Mode

1. Open Settings → Privacy & Security → For developers
2. Enable "Developer Mode"
3. Double-click the .msix file to install

## Prefer No Security Warnings?

Download the **Portable ZIP** version instead:
- No installation required
- No security warnings
- Extract and run ManpWinUI.exe

## Uninstallation

Settings → Apps → ManpLab → Uninstall

## Documentation

Full documentation: https://github.com/markhassellsmith/ManpLab

## License

MIT License - See LICENSE file for details.
"@

    Set-Content -Path (Join-Path $ArtifactsDir "MSIX-Installation-Guide.txt") -Value $MsixReadme

} else {
    Write-Host "⚠ MSIX package not found in expected location" -ForegroundColor Yellow
    Write-Host "  This might be OK - check ManpWinUI\AppPackages manually" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Packaging Complete!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Output directory: $ArtifactsDir" -ForegroundColor White
Write-Host ""
Write-Host "Distribution files created:" -ForegroundColor White
Get-ChildItem $ArtifactsDir | ForEach-Object {
    $size = if ($_.PSIsContainer) { "DIR" } else { "$([math]::Round($_.Length / 1MB, 2)) MB" }
    Write-Host "  $($_.Name) - $size" -ForegroundColor Cyan
}
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Test the portable ZIP on a clean machine" -ForegroundColor White
Write-Host "2. Test the MSIX installation" -ForegroundColor White
Write-Host "3. Upload both to GitHub Releases" -ForegroundColor White
Write-Host ""
