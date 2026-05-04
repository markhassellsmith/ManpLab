# Extract Fractal Types from Legacy Codebase
# This script scans Fractype.h and generates a template JSON file

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$solutionRoot = (Get-Item $scriptDir).Parent.Parent.FullName
$fractypeFile = Join-Path $solutionRoot "ManpWIN64\Fractype.h"
$outputFile = Join-Path $scriptDir "..\Assets\FractalKnowledge\fractals_template.json"

Write-Host "Extracting fractal types from: $fractypeFile" -ForegroundColor Cyan

# Read the header file
if (-not (Test-Path $fractypeFile)) {
    Write-Error "Cannot find Fractype.h at: $fractypeFile"
    exit 1
}

$content = Get-Content $fractypeFile -Raw

# Extract #define patterns (e.g., #define MANDEL 0)
$pattern = '#define\s+([A-Z_0-9]+)\s+(\d+)'
$matches = [regex]::Matches($content, $pattern)

$fractals = @()
$excludeList = @('NOFRACTAL', 'SIN', 'COS', 'SINH', 'COSH', 'EXP', 'LOG', 'SQR', 'TAN')

foreach ($match in $matches) {
    $name = $match.Groups[1].Value
    $id = [int]$match.Groups[2].Value

    # Skip utility constants
    if ($excludeList -contains $name) {
        continue
    }

    # Convert SCREAMING_SNAKE_CASE to PascalCase
    $displayName = (Get-Culture).TextInfo.ToTitleCase($name.ToLower().Replace('_', ' '))

    # Try to categorize based on name
    $category = switch -Wildcard ($name) {
        "MANDEL*" { "Classic Fractals" }
        "JULIA*" { "Julia Sets" }
        "NEWTON*" { "Newton Fractals" }
        "BARNSLEY*" { "Barnsley Fractals" }
        "LAMBDA*" { "Lambda Fractals" }
        "*TRIG*" { "Trigonometric Fractals" }
        "*PHOENIX*" { "Phoenix Fractals" }
        "*MAGNET*" { "Magnet Fractals" }
        "BURNING*" { "Mandelbrot Variants" }
        "*3D*" { "3D Attractors" }
        "LORENZ*" { "3D Attractors" }
        "IFS*" { "Iterated Function Systems" }
        Default { "Other Fractals" }
    }

    $fractal = [PSCustomObject]@{
        name = $name
        legacyId = $id
        displayName = $displayName
        category = $category
        description = "TODO: Add description"
        formula = "TODO: Add formula"
        formulaLatex = ""
        derivation = ""
        visualCharacteristics = ""
        discoveredBy = ""
        discoveryYear = 0
        computationalNotes = ""
        suggestedViewpoints = @()
        relatedFractals = @()
        references = @()
    }

    $fractals += $fractal
}

# Sort by ID
$fractals = $fractals | Sort-Object legacyId

# Create JSON structure
$output = @{
    metadata = @{
        generated = (Get-Date -Format "yyyy-MM-dd HH:mm:ss")
        source = "Extracted from ManpWIN64/Fractype.h"
        totalCount = $fractals.Count
        note = "This is a template. Fill in the TODO fields with actual fractal information."
    }
    fractals = $fractals
}

# Ensure output directory exists
$outputDir = Split-Path -Parent $outputFile
if (-not (Test-Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
}

# Write JSON with pretty formatting
$json = $output | ConvertTo-Json -Depth 10
$json | Out-File -FilePath $outputFile -Encoding UTF8

Write-Host "`nExtracted $($fractals.Count) fractal types" -ForegroundColor Green
Write-Host "Template written to: $outputFile" -ForegroundColor Green
Write-Host "`nCategories found:" -ForegroundColor Yellow
$fractals | Group-Object category | ForEach-Object {
    Write-Host "  $($_.Name): $($_.Count) fractals"
}

Write-Host "`nNext steps:" -ForegroundColor Cyan
Write-Host "  1. Review fractals_template.json"
Write-Host "  2. Fill in descriptions, formulas, and metadata"
Write-Host "  3. Use AI assistance to populate bulk information"
Write-Host "  4. Split into multiple JSON files by category if needed"
