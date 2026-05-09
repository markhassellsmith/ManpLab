# ManpLab Release File Validation Script
# Ensures consistent dash-based naming before GitHub release

param(
    [string]$OutputDir = "Release-Output"
)

Write-Host "`n=== ManpLab Release File Validator ===" -ForegroundColor Cyan
Write-Host "Checking naming conventions and file integrity...`n" -ForegroundColor Yellow

$issues = @()
$warnings = @()

# Check if output directory exists
if (-not (Test-Path $OutputDir)) {
    Write-Host "ERROR: Output directory '$OutputDir' not found!" -ForegroundColor Red
    exit 1
}

# Find all release files
$msixFiles = Get-ChildItem -Path $OutputDir -Filter "*.msix" -Recurse
$zipFiles = Get-ChildItem -Path $OutputDir -Filter "*.zip" -Recurse

Write-Host "Found Files:" -ForegroundColor White
Write-Host "  MSIX: $($msixFiles.Count)" -ForegroundColor Gray
Write-Host "  ZIP:  $($zipFiles.Count)" -ForegroundColor Gray
Write-Host ""

# Validation Rule 1: Check for underscore in filenames (should use dashes)
Write-Host "[1/4] Checking naming convention (dashes vs underscores)..." -ForegroundColor Yellow
foreach ($file in ($msixFiles + $zipFiles)) {
    if ($file.Name -match '_') {
        $issues += "NAMING: File '$($file.Name)' contains underscores. Should use dashes for consistency."
        Write-Host "  ✗ $($file.Name) - Contains underscores" -ForegroundColor Red
    } else {
        Write-Host "  ✓ $($file.Name)" -ForegroundColor Green
    }
}

# Validation Rule 2: Check naming pattern matches expected format
Write-Host "`n[2/4] Checking filename patterns..." -ForegroundColor Yellow
$expectedMsixPattern = "^ManpLab(-v)?[\d\.]+-(x64|x86|arm64)\.msix$"
$expectedZipPattern = "^ManpLab-Portable-[\d\.]+-?(x64|x86|arm64)?\.zip$"

foreach ($file in $msixFiles) {
    if ($file.Name -notmatch $expectedMsixPattern) {
        $warnings += "PATTERN: MSIX filename '$($file.Name)' doesn't match expected pattern: ManpLab-[version]-[arch].msix"
        Write-Host "  ! $($file.Name) - Unexpected pattern" -ForegroundColor Yellow
    } else {
        Write-Host "  ✓ $($file.Name)" -ForegroundColor Green
    }
}

foreach ($file in $zipFiles) {
    if ($file.Name -notmatch $expectedZipPattern) {
        $warnings += "PATTERN: ZIP filename '$($file.Name)' doesn't match expected pattern: ManpLab-Portable-[version]-[arch].zip"
        Write-Host "  ! $($file.Name) - Unexpected pattern" -ForegroundColor Yellow
    } else {
        Write-Host "  ✓ $($file.Name)" -ForegroundColor Green
    }
}

# Validation Rule 3: Check file sizes (basic sanity check)
Write-Host "`n[3/4] Checking file sizes..." -ForegroundColor Yellow
foreach ($file in ($msixFiles + $zipFiles)) {
    $sizeMB = [math]::Round($file.Length / 1MB, 2)
    if ($sizeMB -lt 1) {
        $issues += "SIZE: File '$($file.Name)' is suspiciously small ($sizeMB MB)"
        Write-Host "  ✗ $($file.Name) - Only $sizeMB MB" -ForegroundColor Red
    } elseif ($sizeMB -gt 500) {
        $warnings += "SIZE: File '$($file.Name)' is unusually large ($sizeMB MB)"
        Write-Host "  ! $($file.Name) - $sizeMB MB (seems large)" -ForegroundColor Yellow
    } else {
        Write-Host "  ✓ $($file.Name) - $sizeMB MB" -ForegroundColor Green
    }
}

# Validation Rule 4: Check for documentation files
Write-Host "`n[4/4] Checking for documentation..." -ForegroundColor Yellow
$msixDir = Join-Path $OutputDir "MSIX"
$zipDir = Join-Path $OutputDir "Portable-ZIP"

if (Test-Path $msixDir) {
    $hasInstallGuide = Test-Path (Join-Path $msixDir "INSTALLATION_GUIDE.txt")
    if (-not $hasInstallGuide) {
        $warnings += "DOCS: Missing INSTALLATION_GUIDE.txt in MSIX directory"
        Write-Host "  ! INSTALLATION_GUIDE.txt missing" -ForegroundColor Yellow
    } else {
        Write-Host "  ✓ INSTALLATION_GUIDE.txt present" -ForegroundColor Green
    }
}

# Summary
Write-Host "`n=== VALIDATION RESULTS ===" -ForegroundColor Cyan

if ($issues.Count -eq 0 -and $warnings.Count -eq 0) {
    Write-Host "✓ All checks passed! Files are ready for GitHub release." -ForegroundColor Green
    exit 0
} else {
    if ($issues.Count -gt 0) {
        Write-Host "`nCRITICAL ISSUES ($($issues.Count)):" -ForegroundColor Red
        foreach ($issue in $issues) {
            Write-Host "  • $issue" -ForegroundColor Red
        }
    }

    if ($warnings.Count -gt 0) {
        Write-Host "`nWARNINGS ($($warnings.Count)):" -ForegroundColor Yellow
        foreach ($warning in $warnings) {
            Write-Host "  • $warning" -ForegroundColor Yellow
        }
    }

    if ($issues.Count -gt 0) {
        Write-Host "`nPlease fix critical issues before creating GitHub release." -ForegroundColor Red
        exit 1
    } else {
        Write-Host "`nWarnings found, but you can proceed with release." -ForegroundColor Yellow
        exit 0
    }
}
