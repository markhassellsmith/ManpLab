# Open-AuditDocumentation.ps1
# Opens the fractal audit documentation in your default markdown editor

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "╔════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║  Opening Fractal Audit Documentation                  ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Define paths
$startGuide = "Documentation\FractalReviewAudit\START_HERE.md"
$checklist = "Documentation\FractalReviewAudit\Checklists\TIER1_CRITICAL_FRACTALS.md"
$cheatSheet = "Documentation\FractalReviewAudit\Guides\CHEAT_SHEET.md"

# Check files exist
$files = @($startGuide, $checklist, $cheatSheet)
$allExist = $true

foreach ($file in $files) {
    if (-not (Test-Path $file)) {
        Write-Host "❌ Missing: $file" -ForegroundColor Red
        $allExist = $false
    }
}

if (-not $allExist) {
    Write-Host ""
    Write-Host "Files are missing. Are you in the solution root?" -ForegroundColor Yellow
    Write-Host "Current directory: $(Get-Location)" -ForegroundColor Gray
    exit 1
}

# Open the files
Write-Host "Opening files..." -ForegroundColor Cyan
Write-Host ""

try {
    # Try to open in VS Code first (if available)
    $vsCodePath = Get-Command code -ErrorAction SilentlyContinue

    if ($vsCodePath) {
        Write-Host "📝 Opening in VS Code..." -ForegroundColor Green
        & code $startGuide
        Start-Sleep -Milliseconds 500
        & code $checklist
        Start-Sleep -Milliseconds 500
        & code $cheatSheet
    }
    else {
        # Fall back to default application
        Write-Host "📝 Opening in default editor..." -ForegroundColor Green
        Start-Process $startGuide
        Start-Sleep -Milliseconds 500
        Start-Process $checklist
        Start-Sleep -Milliseconds 500
        Start-Process $cheatSheet
    }

    Write-Host ""
    Write-Host "✓ Opened:" -ForegroundColor Green
    Write-Host "  • START_HERE.md (Beginner's guide)" -ForegroundColor White
    Write-Host "  • TIER1_CRITICAL_FRACTALS.md (Checklist)" -ForegroundColor White
    Write-Host "  • CHEAT_SHEET.md (Quick reference)" -ForegroundColor White
    Write-Host ""
    Write-Host "🚀 Ready to start auditing!" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "  1. Read START_HERE.md" -ForegroundColor Gray
    Write-Host "  2. Open ManpLab application" -ForegroundColor Gray
    Write-Host "  3. Start auditing fractals!" -ForegroundColor Gray
}
catch {
    Write-Host "❌ Error opening files: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Try opening manually:" -ForegroundColor Yellow
    Write-Host "  1. $startGuide" -ForegroundColor Gray
    Write-Host "  2. $checklist" -ForegroundColor Gray
    Write-Host "  3. $cheatSheet" -ForegroundColor Gray
}

Write-Host ""
