# Deep Zoom Diagnostic Script
# Run this in the Immediate Window (Debug → Windows → Immediate) while app is running

Write-Host "=== DEEP ZOOM DIAGNOSTIC ===" -ForegroundColor Cyan
Write-Host ""

# Instructions to run in Immediate Window:
<#
COPY AND PASTE THESE COMMANDS INTO THE IMMEDIATE WINDOW (Ctrl+Alt+I):

# 1. Check if RenderSettingsViewModel exists in MainViewModel
_renderSettingsViewModel

# 2. Check UseDeepZoom property value
_renderSettingsViewModel.UseDeepZoom

# 3. Check if MainPage has the correct ViewModel
this.RenderSettingsViewModel

# 4. Check if they're the same instance
object.ReferenceEquals(_renderSettingsViewModel, this.RenderSettingsViewModel)

# 5. Force a debug output
System.Diagnostics.Debug.WriteLine("TEST MESSAGE FROM IMMEDIATE WINDOW")

#>

Write-Host "Instructions:" -ForegroundColor Yellow
Write-Host "1. Stop the debugger (Shift+F5)"
Write-Host "2. Close and reopen Visual Studio"
Write-Host "3. Rebuild solution (Ctrl+Shift+B)"
Write-Host "4. Start debugging (F5)"
Write-Host "5. Set breakpoint at MainViewModel.Commands.cs line 127"
Write-Host "6. Check the checkbox and click Render"
Write-Host "7. When breakpoint hits, inspect _renderSettingsViewModel.UseDeepZoom"
Write-Host ""
Write-Host "If UseDeepZoom is still false, the DI fix didn't apply yet."
Write-Host ""
