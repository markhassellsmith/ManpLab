# Direct Deploy ManpWinUI - Run after cleaning
# This script manually deploys the MSIX package

Write-Host "==============================================================" -ForegroundColor Cyan
Write-Host "ManpWinUI - Direct Deployment Script" -ForegroundColor Cyan
Write-Host "==============================================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as admin (recommended but not required for developer mode)
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "⚠️  Not running as Administrator (this is OK if Developer Mode is enabled)" -ForegroundColor Yellow
    Write-Host ""
}

# Step 1: Build the project
Write-Host "Step 1: Building ManpWinUI..." -ForegroundColor Green
Write-Host "----------------------------------------" -ForegroundColor Gray

$msbuildPath = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" `
    -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe `
    | Select-Object -First 1

if (-not $msbuildPath -or -not (Test-Path $msbuildPath)) {
    Write-Host "❌ ERROR: Could not find MSBuild" -ForegroundColor Red
    Write-Host "   Please build from Visual Studio instead" -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "Found MSBuild: $msbuildPath" -ForegroundColor Gray
Write-Host ""

# Build parameters
$solutionPath = "ManpLab.sln"
$configuration = "Debug"
$platform = "x64"

Write-Host "Building configuration: $configuration | $platform" -ForegroundColor White
Write-Host ""

try {
    & $msbuildPath $solutionPath `
        /t:ManpWinUI:Rebuild `
        /p:Configuration=$configuration `
        /p:Platform=$platform `
        /p:AppxBundle=Never `
        /p:UapAppxPackageBuildMode=SideLoadOnly `
        /verbosity:minimal

    if ($LASTEXITCODE -ne 0) {
        throw "Build failed with exit code $LASTEXITCODE"
    }

    Write-Host ""
    Write-Host "✅ Build successful!" -ForegroundColor Green
} catch {
    Write-Host ""
    Write-Host "❌ Build failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Try building from Visual Studio instead:" -ForegroundColor Yellow
    Write-Host "1. Open ManpLab.sln" -ForegroundColor Yellow
    Write-Host "2. Select Debug | x64" -ForegroundColor Yellow
    Write-Host "3. Build → Rebuild Solution" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}
Write-Host ""

# Step 2: Find the package
Write-Host "Step 2: Locating MSIX package..." -ForegroundColor Green
Write-Host "----------------------------------------" -ForegroundColor Gray

$packageSearchPaths = @(
    "ManpWinUI\bin\x64\Debug\net10.0-windows10.0.22621.0\win-x64\*.msix",
    "ManpWinUI\bin\x64\Debug\*.msix",
    "ManpWinUI\AppPackages\*\*.msix",
    "x64\Debug\*.msix"
)

$packagePath = $null
foreach ($searchPath in $packageSearchPaths) {
    $found = Get-ChildItem -Path $searchPath -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($found) {
        $packagePath = $found.FullName
        break
    }
}

if (-not $packagePath) {
    Write-Host "⚠️  Could not find MSIX package in expected locations" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Search paths checked:" -ForegroundColor Gray
    foreach ($path in $packageSearchPaths) {
        Write-Host "  - $path" -ForegroundColor Gray
    }
    Write-Host ""
    Write-Host "Please deploy from Visual Studio:" -ForegroundColor Yellow
    Write-Host "1. Right-click ManpWinUI project" -ForegroundColor Yellow
    Write-Host "2. Click 'Deploy'" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "Found package: $packagePath" -ForegroundColor Green
Write-Host ""

# Step 3: Register/Deploy the package
Write-Host "Step 3: Deploying package..." -ForegroundColor Green
Write-Host "----------------------------------------" -ForegroundColor Gray

try {
    Add-AppxPackage -Path $packagePath -Register -ForceApplicationShutdown -ErrorAction Stop
    Write-Host ""
    Write-Host "✅ Package deployed successfully!" -ForegroundColor Green
} catch {
    Write-Host ""
    Write-Host "❌ Deployment failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""

    if ($_.Exception.Message -like "*0x80073CF3*") {
        Write-Host "This error means a conflicting package is installed." -ForegroundColor Yellow
        Write-Host "Run Fix-WinUIDeployment-Complete.ps1 as Administrator first." -ForegroundColor Yellow
    } elseif ($_.Exception.Message -like "*developer license*" -or $_.Exception.Message -like "*developer mode*") {
        Write-Host "Developer Mode is not enabled." -ForegroundColor Yellow
        Write-Host "Enable it: Settings → Privacy & Security → For developers" -ForegroundColor Yellow
    }

    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "==============================================================" -ForegroundColor Cyan
Write-Host "✅ DEPLOYMENT COMPLETE!" -ForegroundColor Green
Write-Host "==============================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "You can now:" -ForegroundColor White
Write-Host "• Press F5 in Visual Studio to run the app" -ForegroundColor Gray
Write-Host "• Find 'ManpLab - Fractal Explorer' in Start Menu" -ForegroundColor Gray
Write-Host "• See the app in Settings → Apps → Installed apps" -ForegroundColor Gray
Write-Host ""
Write-Host "==============================================================" -ForegroundColor Cyan
Write-Host ""
Read-Host "Press Enter to exit"
