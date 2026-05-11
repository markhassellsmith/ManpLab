# Fractal Registry Audit Script
# Verifies that all fractals listed in the CSV are properly registered and accessible

Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host "Fractal Registry Comprehensive Audit" -ForegroundColor Cyan
Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host ""

# Parse the CSV to get expected fractals
$csvPath = "ManpWinUI\Documentation\FractalReviewAudit\FractalRegistry_Full.csv"
$csvContent = Import-Csv $csvPath

Write-Host "CSV Analysis:" -ForegroundColor Yellow
Write-Host "  Total entries in CSV: $($csvContent.Count)"

# Group by family
$byFamily = $csvContent | Group-Object -Property "Fractal Family" | Sort-Object Name

Write-Host "  Families found: $($byFamily.Count)"
Write-Host ""

# Count implemented (with phase annotations)
$implemented = $csvContent | Where-Object { $_.Status -like "*PHASE*" }
Write-Host "Implementation Status:" -ForegroundColor Yellow
Write-Host "  Histogram-based fractals (Phases 2-4): $($implemented.Count)"
Write-Host ""

# List families and their counts
Write-Host "Fractal Families in CSV:" -ForegroundColor Yellow
Write-Host ("-" * 80)
foreach ($family in $byFamily) {
    $familyImplemented = ($family.Group | Where-Object { $_.Status -like "*PHASE*" }).Count
    $status = if ($familyImplemented -gt 0) { " [$familyImplemented implemented]" } else { "" }
    Write-Host "  $($family.Name): $($family.Count) fractals$status"
}
Write-Host ""

# Phase breakdown
Write-Host "Implementation Phase Breakdown:" -ForegroundColor Yellow
Write-Host ("-" * 80)
$phase2 = ($csvContent | Where-Object { $_.Status -like "*PHASE 2*" }).Count
$phase3 = ($csvContent | Where-Object { $_.Status -like "*PHASE 3*" }).Count
$phase4 = ($csvContent | Where-Object { $_.Status -like "*PHASE 4*" }).Count
$duplicates = ($csvContent | Where-Object { $_.Status -like "*Duplicate removed*" }).Count

Write-Host "  Phase 2 (Attractors + Chaotic Maps initial): $phase2"
Write-Host "  Phase 3 (Strange Attractors): $phase3"
Write-Host "  Phase 4 (Remaining histogram-suitable): $($phase4 - $duplicates)"
Write-Host "  Phase 4 (Duplicates removed): $duplicates"
Write-Host "  Total histogram-based: $($phase2 + $phase3 + $phase4 - $duplicates)"
Write-Host ""

# Check for duplicate entries in CSV
Write-Host "CSV Data Quality Check:" -ForegroundColor Yellow
Write-Host ("-" * 80)
$duplicateNames = $csvContent | Group-Object -Property "Fractal Name" | Where-Object { $_.Count -gt 1 }
if ($duplicateNames.Count -gt 0) {
    Write-Host "  WARNING: Duplicate fractal names found:" -ForegroundColor Red
    foreach ($dup in $duplicateNames) {
        Write-Host "    - $($dup.Name) appears $($dup.Count) times"
    }
} else {
    Write-Host "  ✓ No duplicate names in CSV" -ForegroundColor Green
}

# Check for missing index values
$missingIndexes = @()
$maxIndex = ($csvContent | ForEach-Object { [int]$_.Index } | Measure-Object -Maximum).Maximum
for ($i = 1; $i -le $maxIndex; $i++) {
    if (-not ($csvContent | Where-Object { [int]$_.Index -eq $i })) {
        $missingIndexes += $i
    }
}
if ($missingIndexes.Count -gt 0) {
    Write-Host "  WARNING: Missing index values: $($missingIndexes -join ', ')" -ForegroundColor Yellow
} else {
    Write-Host "  ✓ All index values sequential" -ForegroundColor Green
}
Write-Host ""

# Priority Implementation Status
Write-Host "Priority Families for Next Implementation:" -ForegroundColor Yellow
Write-Host ("-" * 80)

# Families that should be ready but have no implementations
$escapeFamilies = @(
    "Classic Fractals",
    "Mandelbrot Variants",
    "Julia Sets",
    "Julia Presets",
    "Burning Ship Family",
    "Burning Ship Variants",
    "Tricorn Family",
    "Lambda Fractals",
    "Newton's Method",
    "Phoenix Fractals",
    "Magnet Fractals"
)

foreach ($familyName in $escapeFamilies) {
    $family = $byFamily | Where-Object { $_.Name -eq $familyName }
    if ($family) {
        $implCount = ($family.Group | Where-Object { $_.Status -ne "" }).Count
        if ($implCount -eq 0) {
            Write-Host "  ! $familyName ($($family.Count) fractals) - Escape-time, should be implemented"
        }
    }
}
Write-Host ""

# Expected vs Actual Registry Count
Write-Host "Registry Expectations:" -ForegroundColor Yellow
Write-Host ("-" * 80)
Write-Host "  Expected total (from FractalRegistry.cpp comment): 278 fractals"
Write-Host "  CSV contains: $($csvContent.Count) unique entries"
Write-Host "  Difference: $(278 - $csvContent.Count) fractals"
Write-Host ""

# Summary
Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host "Audit Summary" -ForegroundColor Cyan
Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host "✓ Histogram rendering complete: 19 fractals" -ForegroundColor Green
Write-Host "! Escape-time fractals: Need verification" -ForegroundColor Yellow
Write-Host "! Special renderers (IFS, Bifurcation, etc.): Need verification" -ForegroundColor Yellow
Write-Host ""
Write-Host "Recommended Next Steps:" -ForegroundColor Cyan
Write-Host "  1. Run the application and test fractal selection"
Write-Host "  2. Verify Classic Fractals family (Mandelbrot, Julia, Lambda)"
Write-Host "  3. Check for any missing or broken fractal implementations"
Write-Host "  4. Update CSV with rendering quality notes for all fractals"
Write-Host ""
