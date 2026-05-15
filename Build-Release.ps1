# ManpLab Release Build Script
# Builds both MSIX package and Portable ZIP distribution

param(
    [string]$Version = "1.1.1",
    [string]$Platform = "x64",
    [switch]$MsixOnly,
    [switch]$ZipOnly
)

Write-Host "`n=== ManpLab Release Builder (MSIX + Portable ZIP) ===" -ForegroundColor Cyan
Write-Host "Version: $Version | Platform: $Platform`n" -ForegroundColor Yellow

# Check we're in the right place
if (-not (Test-Path "ManpLab.sln")) {
    Write-Host "ERROR: Run from solution root" -ForegroundColor Red
    exit 1
}

# Setup paths
$outputRoot = "Release-Output"
$msixOutputDir = "$outputRoot\MSIX"
$zipOutputDir = "$outputRoot\Portable-ZIP"
$publishDir = "ManpWinUI\bin\$Platform\Release\net10.0-windows10.0.22621.0\publish"

# Clean previous output
if (Test-Path $outputRoot) {
    Remove-Item $outputRoot -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $msixOutputDir | Out-Null
New-Item -ItemType Directory -Force -Path $zipOutputDir | Out-Null

Write-Host "Step 1: Finding MSBuild..." -ForegroundColor Yellow
$msbuild = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" `
    -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe `
    -prerelease | Select-Object -First 1

if (-not $msbuild) {
    Write-Host "ERROR: MSBuild not found. Install Visual Studio 2022." -ForegroundColor Red
    exit 1
}

Write-Host "Step 2: Building solution..." -ForegroundColor Yellow
& $msbuild ManpLab.sln /t:Build /p:Configuration=Release /p:Platform=$Platform /v:minimal /nologo

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Create MSIX Package
if (-not $ZipOnly) {
    Write-Host "`nStep 2: Creating MSIX package..." -ForegroundColor Yellow

    # Find MSBuild (already found above, reuse)
    if (-not $msbuild) {
        Write-Host "WARNING: MSBuild not found, skipping MSIX" -ForegroundColor Yellow
    } else {
        & $msbuild ManpWinUI\ManpWinUI.csproj `
            /t:Publish `
            /p:Configuration=Release `
            /p:Platform=$Platform `
            /p:GenerateAppxPackageOnBuild=true `
            /p:AppxPackageDir="$msixOutputDir\" `
            /p:UapAppxPackageBuildMode=SideloadOnly `
            /v:minimal

        # Find and copy MSIX if in AppPackages
        $appPackagesDir = "ManpWinUI\AppPackages"
        if (Test-Path $appPackagesDir) {
            $msixFiles = Get-ChildItem -Path $appPackagesDir -Filter "*.msix" -Recurse
            if ($msixFiles.Count -gt 0) {
                # Rename MSIX to consistent dash-based naming
                $standardizedMsixName = "ManpLab-$Version-$Platform.msix"
                $destinationPath = Join-Path $msixOutputDir $standardizedMsixName
                Copy-Item $msixFiles[0].FullName -Destination $destinationPath -Force
                Write-Host "  MSIX package created: $standardizedMsixName" -ForegroundColor Green
            }
        }

        # Create installation guide
        @"
ManpLab MSIX Installation Guide

INSTALLATION:
1. Extract the ZIP if downloaded compressed
2. Right-click the .msix file and select Install
3. Security Warning: Click Install anyway (normal for open-source)
4. Launch from Start Menu

UNINSTALL:
Settings > Apps > Installed Apps > ManpLab > Uninstall

NOTES:
- Package is not signed (expected for open-source)
- If installation fails, try Portable ZIP version
- Requires Windows 10 build 17763 or later

Version: $Version
https://github.com/markhassellsmith/ManpLab
"@ | Out-File -FilePath "$msixOutputDir\INSTALLATION_GUIDE.txt" -Encoding UTF8
    }
}

# Create Portable ZIP
if (-not $MsixOnly) {
    Write-Host "`nStep 4: Publishing self-contained package..." -ForegroundColor Yellow

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

    Write-Host "Step 5: Creating portable package..." -ForegroundColor Yellow
    $portableDir = "$zipOutputDir\ManpLab-Portable-$Version-$Platform"
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
    $zipPath = "$zipOutputDir\ManpLab-Portable-$Version-$Platform.zip"
    Compress-Archive -Path $portableDir -DestinationPath $zipPath -Force

    $zipSize = [math]::Round((Get-Item $zipPath).Length / 1MB, 2)
    Write-Host "  Portable ZIP created: $zipSize MB" -ForegroundColor Green
}

# Summary
Write-Host "`n=== BUILD COMPLETE ===" -ForegroundColor Green
Write-Host "Output location: $outputRoot`n" -ForegroundColor Cyan

if (-not $ZipOnly -and (Test-Path "$msixOutputDir\*.msix")) {
    Write-Host "MSIX Package:" -ForegroundColor Yellow
    Get-ChildItem -Path $msixOutputDir -Filter "*.msix" | ForEach-Object {
        $size = [math]::Round($_.Length / 1MB, 2)
        Write-Host "  - $($_.Name) ($size MB)" -ForegroundColor White
    }
}

if (-not $MsixOnly) {
    Write-Host "`nPortable ZIP:" -ForegroundColor Yellow
    Get-ChildItem -Path $zipOutputDir -Filter "*.zip" | ForEach-Object {
        $size = [math]::Round($_.Length / 1MB, 2)
        Write-Host "  - $($_.Name) ($size MB)" -ForegroundColor White
    }
}

Write-Host "`nNext: Test packages, then create GitHub release" -ForegroundColor Cyan

# Run validation check
Write-Host "`n--- Running Release File Validation ---" -ForegroundColor Cyan
if (Test-Path "Build\Validate-Release-Files.ps1") {
    & "Build\Validate-Release-Files.ps1" -OutputDir $outputRoot
}
