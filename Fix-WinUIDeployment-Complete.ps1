# Complete Fix for WinUI 3 Deployment Issues
# Run this script AS ADMINISTRATOR

Write-Host "==============================================================" -ForegroundColor Cyan
Write-Host "ManpLab - Complete Deployment Fix Script" -ForegroundColor Cyan
Write-Host "==============================================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as admin
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "❌ ERROR: This script MUST be run as Administrator" -ForegroundColor Red
    Write-Host ""
    Write-Host "To run as Administrator:" -ForegroundColor Yellow
    Write-Host "1. Right-click PowerShell in Start Menu" -ForegroundColor Yellow
    Write-Host "2. Select 'Run as Administrator'" -ForegroundColor Yellow
    Write-Host "3. Navigate to: C:\Users\Mark\source\repos\ManpLab" -ForegroundColor Yellow
    Write-Host "4. Run: .\Fix-WinUIDeployment-Complete.ps1" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "✅ Running as Administrator" -ForegroundColor Green
Write-Host ""

# Step 1: Find and remove ALL old package registrations
Write-Host "Step 1: Removing old MSIX package registrations..." -ForegroundColor Green
Write-Host "----------------------------------------" -ForegroundColor Gray

$packagesToRemove = Get-AppxPackage | Where-Object { 
    $_.Name -like "*ManpWinUI*" -or 
    $_.Name -like "*ManpLab*" -or 
    $_.Name -eq "6335fa26-c01e-4e88-b714-effc8096fc01" -or
    $_.Publisher -like "*User Name*"
}

if ($packagesToRemove) {
    Write-Host "Found packages to remove:" -ForegroundColor Yellow
    $packagesToRemove | ForEach-Object { 
        Write-Host "  - $($_.Name) ($($_.Version))" -ForegroundColor Yellow
    }
    Write-Host ""

    foreach ($package in $packagesToRemove) {
        try {
            Write-Host "Removing: $($package.Name)..." -ForegroundColor Gray
            Remove-AppxPackage -Package $package.PackageFullName -ErrorAction Stop
            Write-Host "  ✅ Removed successfully" -ForegroundColor Green
        } catch {
            Write-Host "  ⚠️  Warning: $($_.Exception.Message)" -ForegroundColor Yellow
        }
    }
    Write-Host ""
} else {
    Write-Host "No old packages found" -ForegroundColor Gray
    Write-Host ""
}

# Step 2: Clean ALL build artifacts thoroughly
Write-Host "Step 2: Cleaning build artifacts..." -ForegroundColor Green
Write-Host "----------------------------------------" -ForegroundColor Gray

$cleanPaths = @(
    "ManpWinUI\bin",
    "ManpWinUI\obj",
    "ManpWinUI\AppPackages",
    "ManpCore.Native\x64",
    "ManpCore.Native\Debug",
    "ManpCore.Native\Release",
    "ManpCore.Services\bin",
    "ManpCore.Services\obj",
    "x64",
    ".vs"
)

$totalRemoved = 0
foreach ($path in $cleanPaths) {
    if (Test-Path $path) {
        try {
            Write-Host "Removing: $path" -ForegroundColor Gray
            Remove-Item -Path $path -Recurse -Force -ErrorAction Stop
            $totalRemoved++
        } catch {
            Write-Host "  ⚠️  Could not remove $path (may be in use)" -ForegroundColor Yellow
        }
    }
}

Write-Host "✅ Cleaned $totalRemoved directories" -ForegroundColor Green
Write-Host ""

# Step 3: Clear Visual Studio cache
Write-Host "Step 3: Clearing Visual Studio caches..." -ForegroundColor Green
Write-Host "----------------------------------------" -ForegroundColor Gray

$vsCachePaths = @(
    "$env:LOCALAPPDATA\Microsoft\VisualStudio\18.0_*\ComponentModelCache",
    "$env:LOCALAPPDATA\Temp\Microsoft\VisualStudio\18.0"
)

foreach ($cachePath in $vsCachePaths) {
    $expandedPaths = Get-Item -Path $cachePath -ErrorAction SilentlyContinue
    foreach ($path in $expandedPaths) {
        if (Test-Path $path) {
            try {
                Write-Host "Clearing: $($path.Name)" -ForegroundColor Gray
                Remove-Item -Path $path -Recurse -Force -ErrorAction SilentlyContinue
            } catch {
                # Ignore errors for cache cleanup
            }
        }
    }
}

Write-Host "✅ Visual Studio caches cleared" -ForegroundColor Green
Write-Host ""

# Step 4: Verify Developer Mode is enabled
Write-Host "Step 4: Checking Windows Developer Mode..." -ForegroundColor Green
Write-Host "----------------------------------------" -ForegroundColor Gray

$devModeKey = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock"
$devModeEnabled = $false

if (Test-Path $devModeKey) {
    $allowDevelopmentWithoutDevLicense = Get-ItemProperty -Path $devModeKey -Name "AllowDevelopmentWithoutDevLicense" -ErrorAction SilentlyContinue
    if ($allowDevelopmentWithoutDevLicense.AllowDevelopmentWithoutDevLicense -eq 1) {
        $devModeEnabled = $true
    }
}

if ($devModeEnabled) {
    Write-Host "✅ Developer Mode is enabled" -ForegroundColor Green
} else {
    Write-Host "⚠️  Developer Mode is NOT enabled" -ForegroundColor Yellow
    Write-Host "   Please enable it manually:" -ForegroundColor Yellow
    Write-Host "   Settings → Privacy & Security → For developers → Developer Mode" -ForegroundColor Yellow
}
Write-Host ""

# Step 5: Verify solution configuration
Write-Host "Step 5: Verifying solution configuration..." -ForegroundColor Green
Write-Host "----------------------------------------" -ForegroundColor Gray

if (Test-Path "ManpLab.sln") {
    $slnContent = Get-Content "ManpLab.sln" -Raw

    $checks = @(
        @{Pattern = "Debug\|x64\.Deploy\.0 = Debug\|x64"; Name = "Debug|x64 Deploy"},
        @{Pattern = "Release\|x64\.Deploy\.0 = Release\|x64"; Name = "Release|x64 Deploy"}
    )

    foreach ($check in $checks) {
        if ($slnContent -match $check.Pattern) {
            Write-Host "✅ $($check.Name) configuration found" -ForegroundColor Green
        } else {
            Write-Host "⚠️  $($check.Name) configuration may be missing" -ForegroundColor Yellow
        }
    }
} else {
    Write-Host "⚠️  ManpLab.sln not found in current directory" -ForegroundColor Yellow
}
Write-Host ""

# Step 6: Check for file locks
Write-Host "Step 6: Checking for process locks..." -ForegroundColor Green
Write-Host "----------------------------------------" -ForegroundColor Gray

$processesToCheck = @("devenv", "MSBuild", "ManpWinUI")
$foundProcesses = @()

foreach ($processName in $processesToCheck) {
    $processes = Get-Process -Name $processName -ErrorAction SilentlyContinue
    if ($processes) {
        $foundProcesses += $processes
        Write-Host "⚠️  Found running: $processName" -ForegroundColor Yellow
    }
}

if ($foundProcesses.Count -eq 0) {
    Write-Host "✅ No conflicting processes found" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "⚠️  Recommendation: Close Visual Studio before rebuilding" -ForegroundColor Yellow
}
Write-Host ""

# Final Summary
Write-Host "==============================================================" -ForegroundColor Cyan
Write-Host "✅ CLEANUP COMPLETE!" -ForegroundColor Green
Write-Host "==============================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps to deploy ManpWinUI:" -ForegroundColor White
Write-Host ""
Write-Host "1. CLOSE VISUAL STUDIO if it's open" -ForegroundColor Yellow
Write-Host ""
Write-Host "2. RESTART VISUAL STUDIO" -ForegroundColor Yellow
Write-Host ""
Write-Host "3. In Visual Studio:" -ForegroundColor White
Write-Host "   a. Open ManpLab.sln" -ForegroundColor Gray
Write-Host "   b. Select: Debug | x64" -ForegroundColor Gray
Write-Host "   c. Right-click ManpWinUI → Set as Startup Project" -ForegroundColor Gray
Write-Host "   d. Build → Rebuild Solution" -ForegroundColor Gray
Write-Host "   e. Build → Deploy ManpWinUI" -ForegroundColor Gray
Write-Host "   f. Press F5 to run" -ForegroundColor Gray
Write-Host ""
Write-Host "4. If you still get the deployment error:" -ForegroundColor Yellow
Write-Host "   a. Build → Configuration Manager" -ForegroundColor Gray
Write-Host "   b. Find ManpWinUI row" -ForegroundColor Gray
Write-Host "   c. Check the 'Deploy' checkbox" -ForegroundColor Gray
Write-Host "   d. Click Close" -ForegroundColor Gray
Write-Host "   e. Try F5 again" -ForegroundColor Gray
Write-Host ""
Write-Host "==============================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press Enter to exit..."
Read-Host
