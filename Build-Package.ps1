# ManpLab Package Script
# Packages an already-built Release into portable ZIP
# PREREQUISITE: Build solution in Visual Studio (Release/x64) first!

param(
    [string]$Version = "1.1.1",
    [string]$Platform = "x64"
)

Write-Host "`n=== ManpLab Package Builder (Portable ZIP) ===" -ForegroundColor Cyan
Write-Host "Version: $Version | Platform: $Platform`n" -ForegroundColor Yellow

# Check we're in the right place
if (-not (Test-Path "ManpLab.sln")) {
    Write-Host "ERROR: Run from solution root" -ForegroundColor Red
    exit 1
}

# Check if already built
$buildOutput = "ManpWinUI\bin\$Platform\Release\net10.0-windows10.0.22621.0"
if (-not (Test-Path "$buildOutput\ManpWinUI.exe")) {
    Write-Host "ERROR: Solution not built! Please:" -ForegroundColor Red
    Write-Host "  1. Open ManpLab.sln in Visual Studio" -ForegroundColor Yellow
    Write-Host "  2. Set Configuration to Release, Platform to x64" -ForegroundColor Yellow
    Write-Host "  3. Build > Rebuild Solution" -ForegroundColor Yellow
    Write-Host "  4. Then run this script again" -ForegroundColor Yellow
    exit 1
}

Write-Host "Step 1: Found existing build ✓" -ForegroundColor Green

# Setup paths
$outputRoot = "Release-Output"
$zipOutputDir = "$outputRoot\Portable-ZIP"
$portableDir = "$zipOutputDir\ManpLab-Portable-$Version-$Platform"

# Clean and create directories
if (Test-Path $outputRoot) {
    Remove-Item $outputRoot -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $portableDir | Out-Null

Write-Host "Step 2: Copying files to package..." -ForegroundColor Yellow

# Copy main executable and dependencies
Copy-Item -Path "$buildOutput\*" -Destination $portableDir -Recurse -Force

# Copy native C++ DLLs from their build locations
$nativeDlls = @(
    "ManpCore.Native\$Platform\Release\ManpCore.Native.dll",
    "x64\Release\ManpWIN64.dll",
    "x64\Release\parser.dll",
    "x64\Release\qdlib.dll",
    "external\*"  # FFmpeg, MPFR, GMP
)

foreach ($dll in $nativeDlls) {
    if (Test-Path $dll) {
        Copy-Item -Path $dll -Destination $portableDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}

Write-Host "Step 3: Cleaning up debug files..." -ForegroundColor Yellow
Get-ChildItem $portableDir -Filter "*.pdb" -Recurse | Remove-Item -Force -ErrorAction SilentlyContinue
Get-ChildItem $portableDir -Filter "*.xml" -Recurse | Remove-Item -Force -ErrorAction SilentlyContinue

Write-Host "Step 4: Creating README..." -ForegroundColor Yellow
@"
ManpLab Portable v$Version

QUICK START:
1. Extract this folder anywhere
2. Run ManpWinUI.exe
3. Explore 300 fractals!

FEATURES:
- No installation needed
- No admin rights required  
- All dependencies included
- Settings persist in %LocalAppData%\ManpLab

REQUIREMENTS:
- Windows 10 (build 1809+) or Windows 11
- x64 processor
- 4 GB RAM (8 GB recommended)

DOCUMENTATION:
https://github.com/markhassellsmith/ManpLab

Built: $(Get-Date -Format 'yyyy-MM-dd HH:mm')
Platform: $Platform | Configuration: Release
"@ | Out-File -FilePath "$portableDir\README.txt" -Encoding UTF8

if (Test-Path "LICENSE") {
    Copy-Item "LICENSE" -Destination $portableDir
}

Write-Host "Step 5: Creating ZIP archive..." -ForegroundColor Yellow
$zipPath = "$zipOutputDir\ManpLab-Portable-$Version-$Platform.zip"
Compress-Archive -Path $portableDir -DestinationPath $zipPath -Force

$zipSize = [math]::Round((Get-Item $zipPath).Length / 1MB, 2)

Write-Host "`n=== PACKAGE COMPLETE ===" -ForegroundColor Green
Write-Host "Package: $zipPath" -ForegroundColor Cyan
Write-Host "Size: $zipSize MB" -ForegroundColor Cyan
Write-Host "`nNext: Test the package, then create GitHub release" -ForegroundColor Yellow
