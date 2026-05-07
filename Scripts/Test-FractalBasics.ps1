# Test-FractalBasics.ps1
# Automated validation script for basic fractal metadata integrity

param(
    [switch]$Verbose
)

Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host "Fractal Basic Validation Script" -ForegroundColor Cyan
Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host ""

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$solutionRoot = Split-Path -Parent $scriptDir

# Test counters
$totalTests = 0
$passedTests = 0
$failedTests = 0
$warnings = 0

function Test-Condition {
    param(
        [string]$TestName,
        [bool]$Condition,
        [string]$FailureMessage = "",
        [switch]$IsWarning
    )

    $script:totalTests++

    if ($Condition) {
        $script:passedTests++
        if ($Verbose) {
            Write-Host "[OK] $TestName" -ForegroundColor Green
        }
        return $true
    }
    else {
        if ($IsWarning) {
            $script:warnings++
            Write-Host "[WARN] $TestName" -ForegroundColor Yellow
        }
        else {
            $script:failedTests++
            Write-Host "[FAIL] $TestName" -ForegroundColor Red
        }

        if ($FailureMessage) {
            Write-Host "    $FailureMessage" -ForegroundColor Gray
        }
        return $false
    }
}

# ============================================================================
# Test 1: Check that source files exist
# ============================================================================

Write-Host "Testing source file existence..." -ForegroundColor Cyan

$familyFiles = @(
    "ManpCore.Native\ClassicFractalsFamily.cpp",
    "ManpCore.Native\JuliaSetsFamily.cpp",
    "ManpCore.Native\NewtonFractalsFamily.cpp",
    "ManpCore.Native\BarnsleyFractalsFamily.cpp",
    "ManpCore.Native\PhoenixFractalsFamily.cpp",
    "ManpCore.Native\MagnetFractalsFamily.cpp",
    "ManpCore.Native\LambdaFractalsFamily.cpp",
    "ManpCore.Native\TrigonometricFamily.cpp",
    "ManpCore.Native\Attractors3DFamily.cpp",
    "ManpCore.Native\MandelbrotVariantsFamily.cpp",
    "ManpCore.Native\OrbitalFractalsFamily.cpp",
    "ManpCore.Native\HistoricalFractalsFamily.cpp",
    "ManpCore.Native\PowerFractalsFamily.cpp",
    "ManpCore.Native\SpiderFractalsFamily.cpp",
    "ManpCore.Native\PopcornFractalsFamily.cpp",
    "ManpCore.Native\ComplexDynamicsFamily.cpp",
    "ManpCore.Native\ConvergentFamily.cpp",
    "ManpCore.Native\ExperimentalFamily.cpp",
    "ManpCore.Native\QuarticFractalsFamily.cpp",
    "ManpCore.Native\CubicFractalsFamily.cpp"
)

foreach ($file in $familyFiles) {
    $fullPath = Join-Path $solutionRoot $file
    $fileName = Split-Path $file -Leaf
    Test-Condition `
        -TestName "Source file exists: $fileName" `
        -Condition (Test-Path $fullPath) `
        -FailureMessage "File not found: $fullPath"
}

# ============================================================================
# Test 2: Check that header files exist
# ============================================================================

Write-Host ""
Write-Host "Testing header file existence..." -ForegroundColor Cyan

$fractypeHeader = Join-Path $solutionRoot "ManpWIN64\Fractype.h"
Test-Condition `
    -TestName "Fractype.h exists" `
    -Condition (Test-Path $fractypeHeader) `
    -FailureMessage "Critical header missing: $fractypeHeader"

# ============================================================================
# Test 3: Check JSON metadata files
# ============================================================================

Write-Host ""
Write-Host "Testing JSON metadata files..." -ForegroundColor Cyan

$jsonFiles = @(
    "ManpWinUI\Assets\FractalKnowledge\fractals_sample.json",
    "ManpWinUI\Assets\FractalKnowledge\fractals_template.json",
    "ManpWinUI\Assets\FractalKnowledge\fractals_tier1.json",
    "ManpWinUI\Assets\FractalKnowledge\schema.json"
)

foreach ($file in $jsonFiles) {
    $fullPath = Join-Path $solutionRoot $file
    $fileName = Split-Path $file -Leaf

    if (Test-Path $fullPath) {
        Test-Condition `
            -TestName "JSON file exists: $fileName" `
            -Condition $true

        # Try to parse JSON
        try {
            $content = Get-Content $fullPath -Raw | ConvertFrom-Json
            Test-Condition `
                -TestName "JSON is valid: $fileName" `
                -Condition $true
        }
        catch {
            Test-Condition `
                -TestName "JSON is valid: $fileName" `
                -Condition $false `
                -FailureMessage "JSON parse error: $($_.Exception.Message)"
        }
    }
    else {
        Test-Condition `
            -TestName "JSON file exists: $fileName" `
            -Condition $false `
            -FailureMessage "File not found: $fullPath" `
            -IsWarning
    }
}

# ============================================================================
# Test 4: Check audit documentation structure
# ============================================================================

Write-Host ""
Write-Host "Testing audit documentation structure..." -ForegroundColor Cyan

$auditFiles = @(
    "docs\FRACTAL_AUDIT_SUMMARY.md",
    "docs\audits\TIER1_CRITICAL_FRACTALS.md"
)

foreach ($file in $auditFiles) {
    $fullPath = Join-Path $solutionRoot $file
    $fileName = Split-Path $file -Leaf
    Test-Condition `
        -TestName "Audit doc exists: $fileName" `
        -Condition (Test-Path $fullPath) `
        -FailureMessage "Documentation missing: $fullPath" `
        -IsWarning
}

# ============================================================================
# Test 5: Check Fractype.h for fractal definitions (sample)
# ============================================================================

Write-Host ""
Write-Host "Testing Fractype.h content..." -ForegroundColor Cyan

if (Test-Path $fractypeHeader) {
    $fractypeContent = Get-Content $fractypeHeader -Raw

    # Check for critical fractals
    $criticalFractals = @("MANDEL", "JULIA", "BURNING_SHIP", "PHOENIX", "NEWTON")

    foreach ($fractal in $criticalFractals) {
        $pattern = "#define\s+$fractal\s+\d+"
        $found = $fractypeContent -match $pattern
        Test-Condition `
            -TestName "Fractype.h defines: $fractal" `
            -Condition $found `
            -FailureMessage "Expected #define $fractal not found"
    }

    # Count total definitions
    $definePattern = '#define\s+([A-Z_0-9]+)\s+(\d+)'
    $matches = [regex]::Matches($fractypeContent, $definePattern)
    $defineCount = $matches.Count

    Test-Condition `
        -TestName "Fractype.h has fractal definitions (found: $defineCount)" `
        -Condition ($defineCount -gt 50) `
        -FailureMessage "Expected at least 50 fractal definitions, found: $defineCount" `
        -IsWarning
}

# ============================================================================
# Test 6: Check for duplicate fractal IDs (if we can parse Fractype.h)
# ============================================================================

Write-Host ""
Write-Host "Testing for duplicate fractal IDs..." -ForegroundColor Cyan

if (Test-Path $fractypeHeader) {
    $fractypeContent = Get-Content $fractypeHeader -Raw
    $definePattern = '#define\s+([A-Z_0-9]+)\s+(\d+)'
    $matches = [regex]::Matches($fractypeContent, $definePattern)

    $ids = @{}
    $duplicates = @()

    foreach ($match in $matches) {
        $name = $match.Groups[1].Value
        $id = $match.Groups[2].Value

        # Skip non-fractal constants
        if ($name -match '^(NOFRACTAL|SIN|COS|SINH|COSH|EXP|LOG|SQR|TAN|PI)$') {
            continue
        }

        if ($ids.ContainsKey($id)) {
            $duplicates += "$name and $($ids[$id]) both use ID $id"
        }
        else {
            $ids[$id] = $name
        }
    }

    Test-Condition `
        -TestName "No duplicate fractal IDs" `
        -Condition ($duplicates.Count -eq 0) `
        -FailureMessage "Duplicates found: $($duplicates -join ', ')"
}

# ============================================================================
# Test 7: Check project structure
# ============================================================================

Write-Host ""
Write-Host "Testing project structure..." -ForegroundColor Cyan

$projectFiles = @(
    "ManpCore.Services\ManpCore.Services.csproj",
    "ManpWinUI\ManpWinUI.csproj"
)

foreach ($file in $projectFiles) {
    $fullPath = Join-Path $solutionRoot $file
    $fileName = Split-Path $file -Leaf
    Test-Condition `
        -TestName "Project file exists: $fileName" `
        -Condition (Test-Path $fullPath) `
        -FailureMessage "Project file missing: $fullPath"
}

# ============================================================================
# Test 8: Check for common documentation files
# ============================================================================

Write-Host ""
Write-Host "Testing documentation completeness..." -ForegroundColor Cyan

$docFiles = @(
    "README.md",
    "ManpCore.Native\FRACTAL_QUICK_REFERENCE.md",
    "ManpCore.Native\ADDING_FRACTALS.md",
    "ManpWinUI\docs\FRACTAL_KNOWLEDGE_BASE_PLAN.md"
)

foreach ($file in $docFiles) {
    $fullPath = Join-Path $solutionRoot $file
    $fileName = Split-Path $file -Leaf
    Test-Condition `
        -TestName "Documentation exists: $fileName" `
        -Condition (Test-Path $fullPath) `
        -FailureMessage "Doc file missing: $fullPath" `
        -IsWarning
}

# ============================================================================
# Summary
# ============================================================================

Write-Host ""
Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host "Test Summary" -ForegroundColor Cyan
Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host ""

$passRate = if ($totalTests -gt 0) { [math]::Round(($passedTests / $totalTests) * 100, 1) } else { 0 }

Write-Host "Total Tests:   $totalTests" -ForegroundColor White
Write-Host "Passed:        $passedTests " -ForegroundColor Green -NoNewline
Write-Host "($passRate%)" -ForegroundColor Gray
Write-Host "Failed:        $failedTests" -ForegroundColor $(if ($failedTests -gt 0) { "Red" } else { "White" })
Write-Host "Warnings:      $warnings" -ForegroundColor $(if ($warnings -gt 0) { "Yellow" } else { "White" })
Write-Host ""

if ($failedTests -eq 0) {
    Write-Host "[OK] All critical tests passed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "  1. Begin Tier 1 fractal audit: docs\audits\TIER1_CRITICAL_FRACTALS.md" -ForegroundColor Gray
    Write-Host "  2. Review audit summary: docs\FRACTAL_AUDIT_SUMMARY.md" -ForegroundColor Gray
}
else {
    Write-Host "[FAIL] Some tests failed. Please review errors above." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "For verbose output, run with -Verbose flag" -ForegroundColor Gray
Write-Host ""
