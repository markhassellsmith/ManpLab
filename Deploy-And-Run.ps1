# Deploy-And-Run.ps1
# Quick deployment script to run before debugging in Visual Studio
# Usage: Run this script, then press F5 in Visual Studio

Write-Host "╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║  ManpLab - Deploy MSIX Package                            ║" -ForegroundColor Cyan
Write-Host "╚═══════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

$manifestPath = "ManpWinUI\bin\x64\Debug\net10.0-windows10.0.22621.0\win-x64\AppX\AppxManifest.xml"

if (-not (Test-Path $manifestPath)) {
    Write-Host "❌ AppxManifest.xml not found!" -ForegroundColor Red
    Write-Host "   Path: $manifestPath" -ForegroundColor Yellow
    Write-Host "   Please build the ManpWinUI project first (Ctrl+Shift+B)" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "📦 Deploying MSIX package..." -ForegroundColor Yellow

try {
    Add-AppxPackage -Register $manifestPath -ForceApplicationShutdown -ForceUpdateFromAnyVersion -ErrorAction Stop
    Write-Host "✅ Package deployed successfully!" -ForegroundColor Green
    Write-Host ""

    $pkg = Get-AppxPackage -Name "6335fa26-c01e-4e88-b714-effc8096fc01"
    if ($pkg) {
        Write-Host "📊 Package Info:" -ForegroundColor Cyan
        Write-Host "   Name: $($pkg.Name)" -ForegroundColor White
        Write-Host "   Version: $($pkg.Version)" -ForegroundColor White
        Write-Host "   Status: $($pkg.Status)" -ForegroundColor Green
        Write-Host ""
    }

    Write-Host "🚀 Ready to debug!" -ForegroundColor Green
    Write-Host "   Switch to Visual Studio and press F5" -ForegroundColor White
    Write-Host ""

} catch {
    Write-Host "❌ Deployment failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Read-Host "Press Enter to close"
