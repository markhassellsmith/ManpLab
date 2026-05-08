# Split-AuditFile.ps1
# Utility to split the massive FRACTAL_QUALITY_AUDIT_CHECKLIST.md into category-based files

param(
    [string]$InputFile = "docs\FRACTAL_QUALITY_AUDIT_CHECKLIST.md",
    [string]$OutputDir = "docs\audits",
    [switch]$DryRun
)

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$solutionRoot = $scriptDir
$inputPath = Join-Path $solutionRoot $InputFile
$outputPath = Join-Path $solutionRoot $OutputDir

Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host "Audit File Splitter" -ForegroundColor Cyan
Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host ""

if (-not (Test-Path $inputPath)) {
    Write-Error "Input file not found: $inputPath"
    exit 1
}

Write-Host "Input:  $inputPath" -ForegroundColor Gray
Write-Host "Output: $outputPath" -ForegroundColor Gray
Write-Host ""

# Create output directory
if (-not $DryRun) {
    New-Item -ItemType Directory -Path $outputPath -Force | Out-Null
}

# Read the massive file
Write-Host "Reading input file..." -ForegroundColor Cyan
$content = Get-Content $inputPath -Raw

# Extract categories and their fractals
$categoryPattern = '###\s+([^\r\n]+)\s+\((\d+)\s+fractals?\)'
$categoryMatches = [regex]::Matches($content, $categoryPattern)

Write-Host "Found $($categoryMatches.Count) categories" -ForegroundColor Green
Write-Host ""

$header = @"
# Fractal Audit: {CATEGORY}

## Category Information
**Fractal Count:** {COUNT}  
**Source:** Split from FRACTAL_QUALITY_AUDIT_CHECKLIST.md  
**Parent Document:** [Audit Summary](../FRACTAL_AUDIT_SUMMARY.md)

---

## Quick Audit Instructions

For each fractal:
1. Launch in ManpLab
2. Check visual quality
3. Test performance
4. Verify formula correctness
5. Mark checkboxes below

**Standard Test:** 1920×1080, 256 iterations

---

"@

foreach ($categoryMatch in $categoryMatches) {
    $categoryName = $categoryMatch.Groups[1].Value.Trim()
    $fractalCount = $categoryMatch.Groups[2].Value

    # Sanitize filename
    $fileName = "AUDIT_$($categoryName -replace '[^a-zA-Z0-9_]', '_').md"
    $filePath = Join-Path $outputPath $fileName

    Write-Host "Category: $categoryName ($fractalCount fractals)" -ForegroundColor White
    Write-Host "  → $fileName" -ForegroundColor Gray

    if (-not $DryRun) {
        # Extract this category's section
        $categoryStartPos = $categoryMatch.Index

        # Find next category or end of file
        $nextCategoryPos = $content.Length
        $categoryIndex = [array]::IndexOf($categoryMatches, $categoryMatch)
        if ($categoryIndex -lt $categoryMatches.Count - 1) {
            $nextCategoryPos = $categoryMatches[$categoryIndex + 1].Index
        }

        $categoryContent = $content.Substring($categoryStartPos, $nextCategoryPos - $categoryStartPos).Trim()

        # Build output file
        $outputContent = $header -replace '{CATEGORY}', $categoryName -replace '{COUNT}', $fractalCount
        $outputContent += $categoryContent

        # Add footer
        $outputContent += @"


---

## Progress Summary

- **Total in Category:** $fractalCount
- **Audited:** ___
- **Pass:** ___
- **Fail:** ___
- **Needs Work:** ___

---

## Notes

Add category-wide observations here.

---

**Navigation:**
- [Back to Audit Summary](../FRACTAL_AUDIT_SUMMARY.md)
- [Tier 1 Critical](TIER1_CRITICAL_FRACTALS.md)

"@

        # Write file
        Set-Content -Path $filePath -Value $outputContent -Encoding UTF8
    }
}

Write-Host ""
Write-Host "=" * 80 -ForegroundColor Cyan

if ($DryRun) {
    Write-Host "DRY RUN - No files written" -ForegroundColor Yellow
    Write-Host "Remove -DryRun flag to actually create files" -ForegroundColor Gray
}
else {
    Write-Host "✓ Split complete!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Created $($categoryMatches.Count) category audit files in:" -ForegroundColor White
    Write-Host "  $outputPath" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "  1. Review the split files" -ForegroundColor Gray
    Write-Host "  2. Start with Tier 1: docs\audits\TIER1_CRITICAL_FRACTALS.md" -ForegroundColor Gray
    Write-Host "  3. Update progress in: docs\FRACTAL_AUDIT_SUMMARY.md" -ForegroundColor Gray
}

Write-Host ""
