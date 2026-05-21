Write-Host "=== ManpLab System Requirements Check ===" -ForegroundColor Cyan
# OS Version
$os = Get-CimInstance Win32_OperatingSystem
Write-Host "OS: $($os.Caption) Build $($os.BuildNumber)" -ForegroundColor Yellow
if ($os.BuildNumber -lt 17763) { Write-Host " ⚠️ FAIL: Need Windows 10 1809+ or Windows 11" -ForegroundColor Red }
# .NET Runtimes
Write-Host "`n.NET Runtimes:" -ForegroundColor Yellow
dotnet --list-runtimes
# Visual C++ Redistributables
Write-Host "`nVisual C++ Redistributables:" -ForegroundColor Yellow
Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\*" -ErrorAction SilentlyContinue |
    Select-Object PSChildName, Version
# WebView2
Write-Host "`nWebView2:" -ForegroundColor Yellow
Get-ItemProperty "HKLM:\SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}" -ErrorAction SilentlyContinue |
    Select-Object pv
# Memory
Write-Host "`nMemory:" -ForegroundColor Yellow
$mem = Get-CimInstance Win32_ComputerSystem
Write-Host " Total RAM: $([math]::Round($mem.TotalPhysicalMemory/1GB, 2)) GB"
# DirectX
Write-Host "`nDirectX Version:" -ForegroundColor Yellow
(Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\DirectX").Version
Write-Host "`n=== Check Complete ===" -ForegroundColor Cyan
