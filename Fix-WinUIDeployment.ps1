# Fix WinUI 3 MSIX Deployment Issues
# Run this script as Administrator if deployment fails

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "ManpLab Deployment Fix Script" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as admin
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "⚠️  WARNING: Not running as Administrator" -ForegroundColor Yellow
    Write-Host "Some operations may fail. Consider running as Admin." -ForegroundColor Yellow
    Write-Host ""
}

# Step 1: Clean old registrations
Write-Host "Step 1: Cleaning old package registrations..." -ForegroundColor Green
try {
    $packages = Get-AppxPackage | Where-Object { $_.Name -like "*ManpWinUI*" -or $_.Name -like "*ManpLab*" }

    if ($packages) {
        Write-Host "Found existing packages:" -ForegroundColor Yellow
        $packages | ForEach-Object { Write-Host "  - $($_.Name) ($($_.Version))" }

        Write-Host "Removing old packages..." -ForegroundColor Yellow
        $packages | Remove-AppxPackage -ErrorAction SilentlyContinue
        Write-Host "✅ Old packages removed" -ForegroundColor Green
    } else {
        Write-Host "No old packages found" -ForegroundColor Gray
    }
} catch {
    Write-Host "⚠️  Could not remove old packages: $($_.Exception.Message)" -ForegroundColor Yellow
}

Write-Host ""

# Step 2: Clean build artifacts
Write-Host "Step 2: Cleaning build artifacts..." -ForegroundColor Green
$cleanPaths = @(
    "ManpWinUI\bin",
    "ManpWinUI\obj",
    "ManpWinUI\AppPackages",
    "ManpCore.Native\x64",
    "ManpCore.Services\bin",
    "ManpCore.Services\obj",
    "x64"
)

foreach ($path in $cleanPaths) {
    if (Test-Path $path) {
        Write-Host "Removing $path..." -ForegroundColor Gray
        Remove-Item -Path $path -Recurse -Force -ErrorAction SilentlyContinue
    }
}

Write-Host "✅ Build artifacts cleaned" -ForegroundColor Green
Write-Host ""

# Step 3: Verify solution configuration
Write-Host "Step 3: Checking solution configuration..." -ForegroundColor Green

$slnContent = Get-Content "ManpLab.sln" -Raw

if ($slnContent -match "Debug\|x64\.Deploy\.0 = Debug\|x64") {
    Write-Host "✅ Deploy configuration found for Debug|x64" -ForegroundColor Green
} else {
    Write-Host "⚠️  Deploy configuration may be missing" -ForegroundColor Yellow
    Write-Host "   → Check Configuration Manager in Visual Studio" -ForegroundColor Yellow
}

Write-Host ""

# Step 4: Instructions
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "Next Steps in Visual Studio:" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Open Visual Studio" -ForegroundColor White
Write-Host "2. Select configuration: Debug | x64" -ForegroundColor White
Write-Host "3. Right-click ManpWinUI project → Rebuild" -ForegroundColor White
Write-Host "4. Press F5 or click the green Start button" -ForegroundColor White
Write-Host ""
Write-Host "If still getting deployment errors:" -ForegroundColor Yellow
Write-Host "  → Build → Configuration Manager" -ForegroundColor Yellow
Write-Host "  → Check 'Deploy' checkbox for ManpWinUI (Debug|x64)" -ForegroundColor Yellow
Write-Host ""
Write-Host "If errors persist:" -ForegroundColor Yellow
Write-Host "  1. Close Visual Studio" -ForegroundColor Yellow
Write-Host "  2. Run this script AS ADMINISTRATOR" -ForegroundColor Yellow
Write-Host "  3. Restart Visual Studio" -ForegroundColor Yellow
Write-Host ""

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "Script complete!" -ForegroundColor Green
Write-Host "==================================" -ForegroundColor Cyan
