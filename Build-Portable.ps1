# Quick Portable ZIP Build Script for ManpLab
# Simplified version for creating portable distribution only

param(
    [string]$Version = "1.1.1",
    [string]$Platform = "x64"
)

Write-Host "`n=== ManpLab Portable ZIP Builder ===" -ForegroundColor Cyan
Write-Host "Version: $Version | Platform: $Platform`n" -ForegroundColor Yellow

# Check we're in the right place
if (-not (Test-Path "ManpLab.sln")) {
    Write-Host "ERROR: Run from solution root" -ForegroundColor Red
    exit 1
}

# Setup paths
$outputRoot = "Release-Output"
$publishDir = "ManpWinUI\bin\$Platform\Release\net10.0-windows10.0.22621.0\publish"
$portableDir = "$outputRoot\ManpLab-Portable-$Version-$Platform"

# Clean previous output
if (Test-Path $outputRoot) {
    Remove-Item $outputRoot -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $outputRoot | Out-Null

Write-Host "Step 1: Building solution..." -ForegroundColor Yellow
dotnet build ManpLab.sln -c Release -p:Platform=$Platform --nologo

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Step 3: Publishing self-contained package..." -ForegroundColor Yellow
& $msbuild ManpWinUI\ManpWinUI.csproj `
    /t:Publish `
    /p:Configuration=Release `
    /p:Platform=$Platform `
    /p:RuntimeIdentifier=win-$Platform `
    /p:SelfContained=true `
    /p:PublishDir="$publishDir\" `
    /p:PublishSingleFile=false `
    /p:WindowsPackageType=None `
    /v:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "Publish failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Step 3: Creating portable package..." -ForegroundColor Yellow
New-Item -ItemType Directory -Force -Path $portableDir | Out-Null
Copy-Item -Path "$publishDir\*" -Destination $portableDir -Recurse -Force

# Clean up debug files
Get-ChildItem $portableDir -Filter "*.pdb" -Recurse | Remove-Item -Force -ErrorAction SilentlyContinue
Get-ChildItem $portableDir -Filter "*.xml" -Recurse | Remove-Item -Force -ErrorAction SilentlyContinue

# Create README
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

# Copy LICENSE
if (Test-Path "LICENSE") {
    Copy-Item "LICENSE" -Destination $portableDir
}

Write-Host "Step 5: Creating ZIP archive..." -ForegroundColor Yellow
$zipPath = "$outputRoot\ManpLab-Portable-$Version-$Platform.zip"
Compress-Archive -Path $portableDir -DestinationPath $zipPath -Force

$zipSize = [math]::Round((Get-Item $zipPath).Length / 1MB, 2)

Write-Host "`n=== BUILD COMPLETE ===" -ForegroundColor Green
Write-Host "Package: $zipPath" -ForegroundColor Cyan
Write-Host "Size: $zipSize MB" -ForegroundColor Cyan
Write-Host "`nNext: Test the package, then create GitHub release" -ForegroundColor Yellow
