# Complete Clean and Rebuild for Deep Zoom Fix
# This ensures the DI changes are properly applied

Write-Host "=== DEEP ZOOM FIX - COMPLETE REBUILD ===" -ForegroundColor Cyan
Write-Host ""

# Step 1: Kill any running instances
Write-Host "Step 1: Stopping any running ManpWinUI processes..." -ForegroundColor Yellow
Stop-Process -Name "ManpWinUI" -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 1

# Step 2: Clean bin and obj folders
Write-Host "Step 2: Cleaning build artifacts..." -ForegroundColor Yellow
if (Test-Path "ManpWinUI\bin") {
    Remove-Item -Path "ManpWinUI\bin" -Recurse -Force
    Write-Host "  Deleted ManpWinUI\bin" -ForegroundColor Green
}
if (Test-Path "ManpWinUI\obj") {
    Remove-Item -Path "ManpWinUI\obj" -Recurse -Force
    Write-Host "  Deleted ManpWinUI\obj" -ForegroundColor Green
}

# Step 3: Instructions for Visual Studio
Write-Host ""
Write-Host "Step 3: Manual steps in Visual Studio:" -ForegroundColor Yellow
Write-Host "  1. Close Visual Studio completely (File → Exit)" -ForegroundColor White
Write-Host "  2. Reopen Visual Studio" -ForegroundColor White
Write-Host "  3. Open ManpLab.sln" -ForegroundColor White
Write-Host "  4. Build → Rebuild Solution (Ctrl+Shift+B)" -ForegroundColor White
Write-Host "  5. Debug → Start Debugging (F5)" -ForegroundColor White
Write-Host ""

# Step 4: Testing instructions
Write-Host "Step 4: After app starts, test Deep Zoom:" -ForegroundColor Yellow
Write-Host "  1. View → Output (Ctrl+Alt+O)" -ForegroundColor White
Write-Host "  2. Select 'Debug' from dropdown at top" -ForegroundColor White
Write-Host "  3. Properties Panel → Render tab" -ForegroundColor White
Write-Host "  4. Check 'Enable Deep Zoom (Arbitrary Precision)'" -ForegroundColor White
Write-Host "  5. Click Render (or press F5)" -ForegroundColor White
Write-Host ""

Write-Host "Expected Output in Debug window:" -ForegroundColor Green
Write-Host "[RenderCommand] Using PARAMETER SYSTEM for render" -ForegroundColor Gray
Write-Host "[RenderCommand] Deep Zoom Setting: True (from RenderSettingsViewModel: True)" -ForegroundColor Gray
Write-Host "[FractalRenderService] useDeepZoom parameter received: True" -ForegroundColor Gray
Write-Host "[DeepZoom] Enabled with 25 digit precision" -ForegroundColor Gray
Write-Host ""

Write-Host "If you still don't see messages, set a breakpoint at:" -ForegroundColor Yellow
Write-Host "  ManpWinUI\ViewModels\MainViewModel.Commands.cs line 127" -ForegroundColor White
Write-Host "  and inspect _renderSettingsViewModel.UseDeepZoom when it hits" -ForegroundColor White
Write-Host ""

Write-Host "=== READY TO PROCEED ===" -ForegroundColor Cyan
