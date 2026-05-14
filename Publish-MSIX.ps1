# Quick MSIX Publish Script for ManpLab
# Run from solution root directory

param(
    [ValidateSet('x64', 'x86', 'ARM64')]
    [string]$Platform = 'x64',

    [ValidateSet('Release', 'Debug')]
    [string]$Configuration = 'Release',

    [string]$Version = '1.4.0.0'
)

Write-Host "====================================" -ForegroundColor Cyan
Write-Host " ManpLab MSIX Publisher" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Platform:      $Platform" -ForegroundColor Yellow
Write-Host "Version:       $Version" -ForegroundColor Yellow
Write-Host ""

# Ensure AppPackages folder exists
if (-not (Test-Path "AppPackages")) {
    New-Item -Path "AppPackages" -ItemType Directory | Out-Null
}

# Step 1: Restore packages
Write-Host "[1/3] Restoring NuGet packages..." -ForegroundColor Green
dotnet restore ManpWinUI.sln
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Restore failed!" -ForegroundColor Red
    exit 1
}

# Step 2: Build solution (ensures native DLLs are built)
Write-Host "[2/3] Building solution..." -ForegroundColor Green
msbuild ManpWinUI.sln /p:Configuration=$Configuration /p:Platform=$Platform /v:minimal
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}

# Step 3: Create MSIX package
Write-Host "[3/3] Creating MSIX package..." -ForegroundColor Green
$outputPath = "..\AppPackages\ManpWinUI_${Version}_${Platform}.msix"
msbuild ManpWinUI\ManpWinUI.csproj `
    /t:Publish `
    /p:Configuration=$Configuration `
    /p:Platform=$Platform `
    /p:GenerateAppxPackageOnBuild=true `
    /p:AppxPackageOutput=$outputPath `
    /p:UapAppxPackageBuildMode=SideloadOnly `
    /v:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Publish failed!" -ForegroundColor Red
    exit 1
}

# Success
Write-Host ""
Write-Host "✅ MSIX package created successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Output location:" -ForegroundColor Cyan
$msixFile = Get-Item "AppPackages\ManpWinUI_${Version}_${Platform}.msix" -ErrorAction SilentlyContinue
if ($msixFile) {
    Write-Host "  $($msixFile.FullName)" -ForegroundColor Yellow
    Write-Host "  Size: $([math]::Round($msixFile.Length / 1MB, 2)) MB" -ForegroundColor Gray
}
Write-Host ""
Write-Host "To install, run:" -ForegroundColor Cyan
Write-Host "  Add-AppxPackage -Path `"AppPackages\ManpWinUI_${Version}_${Platform}.msix`"" -ForegroundColor White
Write-Host ""
Write-Host "Or double-click the .msix file in File Explorer" -ForegroundColor Cyan
