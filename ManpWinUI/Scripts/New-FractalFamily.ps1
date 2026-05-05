# New-FractalFamily.ps1
# Automated tool to scaffold a new fractal family

param(
    [Parameter(Mandatory=$true)]
    [string]$FamilyName,

    [Parameter(Mandatory=$false)]
    [string]$Category = "Other Fractals",

    [Parameter(Mandatory=$false)]
    [switch]$Interactive
)

$ErrorActionPreference = "Stop"

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "ManpLab Fractal Family Generator" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Validate family name
if ($FamilyName -notmatch '^[A-Za-z][A-Za-z0-9]*$') {
    Write-Error "Family name must start with a letter and contain only letters and numbers."
    exit 1
}

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$solutionRoot = (Get-Item $scriptDir).Parent.Parent.FullName
$nativeProject = Join-Path $solutionRoot "ManpCore.Native"
$templateFile = Join-Path $nativeProject "FractalTemplate.cpp.template"
$outputFile = Join-Path $nativeProject "${FamilyName}Family.cpp"
$registryFile = Join-Path $nativeProject "FractalRegistry.cpp"
$vcxprojFile = Join-Path $nativeProject "ManpCore.Native.vcxproj"

# Check if files exist
if (-not (Test-Path $templateFile)) {
    Write-Error "Template file not found: $templateFile"
    exit 1
}

if (Test-Path $outputFile) {
    $overwrite = Read-Host "File $outputFile already exists. Overwrite? (y/n)"
    if ($overwrite -ne 'y') {
        Write-Host "Aborted." -ForegroundColor Yellow
        exit 0
    }
}

# Interactive mode
if ($Interactive) {
    Write-Host "Interactive mode - answer questions to customize your fractal:" -ForegroundColor Yellow
    $Category = Read-Host "Category name (e.g., 'Classic Fractals', 'Newton Fractals')"

    $supportsJulia = Read-Host "Supports Julia mode? (y/n)"
    $juliaSupport = ($supportsJulia -eq 'y')

    $hasSymmetry = Read-Host "Has x-axis or y-axis symmetry? (y/n)"
    $symmetry = ($hasSymmetry -eq 'y')

    $centerX = Read-Host "Default center X coordinate (default: 0.0)"
    if ([string]::IsNullOrWhiteSpace($centerX)) { $centerX = "0.0" }

    $centerY = Read-Host "Default center Y coordinate (default: 0.0)"
    if ([string]::IsNullOrWhiteSpace($centerY)) { $centerY = "0.0" }

    $zoom = Read-Host "Default zoom level (default: 1.0)"
    if ([string]::IsNullOrWhiteSpace($zoom)) { $zoom = "1.0" }
}

Write-Host "`nGenerating files..." -ForegroundColor Cyan

# Step 1: Generate .cpp file from template
Write-Host "  1. Creating ${FamilyName}Family.cpp..." -ForegroundColor Yellow

$content = Get-Content $templateFile -Raw
$content = $content -replace '\{\{FAMILY_NAME\}\}', $FamilyName
$content = $content -replace '\{\{CATEGORY\}\}', $Category

if ($Interactive) {
    $content = $content -replace 'spec\.supportsJulia = true;', "spec.supportsJulia = $($juliaSupport.ToString().ToLower());"
    $content = $content -replace 'spec\.hasSymmetry = false;', "spec.hasSymmetry = $($symmetry.ToString().ToLower());"
    $content = $content -replace 'spec\.defaultCenterX = 0\.0;', "spec.defaultCenterX = $centerX;"
    $content = $content -replace 'spec\.defaultCenterY = 0\.0;', "spec.defaultCenterY = $centerY;"
    $content = $content -replace 'spec\.defaultZoom = 1\.0;', "spec.defaultZoom = $zoom;"
}

$content | Out-File -FilePath $outputFile -Encoding UTF8
Write-Host "     ✓ Created: $outputFile" -ForegroundColor Green

# Step 2: Update FractalRegistry.cpp
Write-Host "  2. Updating FractalRegistry.cpp..." -ForegroundColor Yellow

$registryContent = Get-Content $registryFile -Raw

# Add forward declaration
$declarationPattern = '(// Forward declarations[^\n]*\n)'
$declaration = "void Register${FamilyName}Family();`n"
if ($registryContent -notmatch [regex]::Escape($declaration)) {
    $registryContent = $registryContent -replace $declarationPattern, "`$1$declaration"
    Write-Host "     ✓ Added forward declaration" -ForegroundColor Green
} else {
    Write-Host "     ℹ Forward declaration already exists" -ForegroundColor Gray
}

# Add registration call
$registrationPattern = '(void FractalRegistry::InitializeBuiltins\(\)[^{]*\{[^}]*)'
$registration = "    Register${FamilyName}Family();`n"
if ($registryContent -notmatch [regex]::Escape($registration)) {
    $registryContent = $registryContent -replace $registrationPattern, "`$1$registration"
    Write-Host "     ✓ Added registration call" -ForegroundColor Green
} else {
    Write-Host "     ℹ Registration call already exists" -ForegroundColor Gray
}

$registryContent | Out-File -FilePath $registryFile -Encoding UTF8

# Step 3: Update .vcxproj
Write-Host "  3. Updating ManpCore.Native.vcxproj..." -ForegroundColor Yellow

[xml]$vcxproj = Get-Content $vcxprojFile

# Find the ItemGroup with ClCompile elements
$itemGroup = $vcxproj.Project.ItemGroup | Where-Object { $_.ClCompile } | Select-Object -First 1

# Check if already exists
$existingCompile = $itemGroup.ClCompile | Where-Object { $_.Include -eq "${FamilyName}Family.cpp" }

if (-not $existingCompile) {
    $newCompile = $vcxproj.CreateElement("ClCompile", $vcxproj.DocumentElement.NamespaceURI)
    $newCompile.SetAttribute("Include", "${FamilyName}Family.cpp")
    $itemGroup.AppendChild($newCompile) | Out-Null
    $vcxproj.Save($vcxprojFile)
    Write-Host "     ✓ Added to project file" -ForegroundColor Green
} else {
    Write-Host "     ℹ Already in project file" -ForegroundColor Gray
}

# Step 4: Create JSON metadata template
Write-Host "  4. Creating JSON metadata template..." -ForegroundColor Yellow

$jsonDir = Join-Path $solutionRoot "ManpWinUI\Assets\FractalKnowledge"
$jsonFile = Join-Path $jsonDir "${FamilyName}_metadata.json"

$jsonTemplate = @"
{
  "fractals": [
    {
      "name": "${FamilyName}V1",
      "displayName": "${FamilyName} - Variant 1",
      "category": "${Category}",
      "description": "TODO: Add description",
      "formula": "TODO: Add formula (e.g., 'z_{n+1} = z_n^2 + c')",
      "formulaLatex": "",
      "derivation": "TODO: Add mathematical background",
      "visualCharacteristics": "TODO: Describe what it looks like",
      "discoveredBy": "",
      "discoveryYear": 0,
      "computationalNotes": "",
      "suggestedViewpoints": [
        "Full View: 0.0, 0.0, 1.0"
      ],
      "relatedFractals": [],
      "references": []
    }
  ]
}
"@

if (-not (Test-Path $jsonDir)) {
    New-Item -ItemType Directory -Path $jsonDir -Force | Out-Null
}

$jsonTemplate | Out-File -FilePath $jsonFile -Encoding UTF8
Write-Host "     ✓ Created: $jsonFile" -ForegroundColor Green

# Summary
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Fractal Family Generated Successfully!" -ForegroundColor Green
Write-Host "========================================`n" -ForegroundColor Cyan

Write-Host "Created files:" -ForegroundColor Yellow
Write-Host "  • $outputFile" -ForegroundColor White
Write-Host "  • $jsonFile" -ForegroundColor White

Write-Host "`nModified files:" -ForegroundColor Yellow
Write-Host "  • $registryFile" -ForegroundColor White
Write-Host "  • $vcxprojFile" -ForegroundColor White

Write-Host "`nNext steps:" -ForegroundColor Cyan
Write-Host "  1. Open ${FamilyName}Family.cpp and implement your iteration formula" -ForegroundColor White
Write-Host "  2. Fill in the TODO sections with proper metadata" -ForegroundColor White
Write-Host "  3. Update ${FamilyName}_metadata.json with rich documentation" -ForegroundColor White
Write-Host "  4. Build the solution: dotnet build" -ForegroundColor White
Write-Host "  5. Test your fractal in the WinUI browser panel" -ForegroundColor White

Write-Host "`nDocumentation:" -ForegroundColor Cyan
Write-Host "  • See ManpCore.Native/ADDING_FRACTALS.md for detailed guide" -ForegroundColor White
Write-Host "  • Reference MandelbrotFamily.cpp for examples" -ForegroundColor White

Write-Host "`n✨ Happy fractal coding! ✨`n" -ForegroundColor Magenta
