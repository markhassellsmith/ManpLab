# Add-AuditDocsToSolution.ps1
# Adds Documentation/FractalReviewAudit folders to ManpLab.sln

$slnPath = "ManpLab.sln"

if (-not (Test-Path $slnPath)) {
    Write-Error "Solution file not found: $slnPath"
    exit 1
}

Write-Host "Reading solution file..." -ForegroundColor Cyan
$slnContent = Get-Content $slnPath -Raw

# GUIDs for the new solution folders
$docGuid = "{1A5039BC-5A27-4FAB-B143-E48B2AD6AF66}"
$auditGuid = "{AC2E2147-F1BC-429B-A4F3-E1DBA64414DA}"
$guidesGuid = "{76A903F2-20C0-423C-8EEA-1C03406EBE72}"
$checklistsGuid = "{F1BCD571-6763-4D8A-93E7-3958FE8FD9B9}"

# Define the new project entries to add
$newProjects = @"
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Documentation", "Documentation", "$docGuid"
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "FractalReviewAudit", "FractalReviewAudit", "$auditGuid"
    ProjectSection(SolutionItems) = preProject
        Documentation\FractalReviewAudit\README.md = Documentation\FractalReviewAudit\README.md
        Documentation\FractalReviewAudit\START_HERE.md = Documentation\FractalReviewAudit\START_HERE.md
        Documentation\FractalReviewAudit\YOU_ARE_HERE.md = Documentation\FractalReviewAudit\YOU_ARE_HERE.md
        Documentation\FractalReviewAudit\AUDIT_SUMMARY.md = Documentation\FractalReviewAudit\AUDIT_SUMMARY.md
        Documentation\FractalReviewAudit\HOW_TO_ADD_TO_SOLUTION.md = Documentation\FractalReviewAudit\HOW_TO_ADD_TO_SOLUTION.md
    EndProjectSection
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Guides", "Guides", "$guidesGuid"
    ProjectSection(SolutionItems) = preProject
        Documentation\FractalReviewAudit\Guides\WORKFLOW_VISUAL.md = Documentation\FractalReviewAudit\Guides\WORKFLOW_VISUAL.md
        Documentation\FractalReviewAudit\Guides\CHEAT_SHEET.md = Documentation\FractalReviewAudit\Guides\CHEAT_SHEET.md
        Documentation\FractalReviewAudit\Guides\QUICK_START.md = Documentation\FractalReviewAudit\Guides\QUICK_START.md
    EndProjectSection
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Checklists", "Checklists", "$checklistsGuid"
    ProjectSection(SolutionItems) = preProject
        Documentation\FractalReviewAudit\Checklists\TIER1_CRITICAL_FRACTALS.md = Documentation\FractalReviewAudit\Checklists\TIER1_CRITICAL_FRACTALS.md
    EndProjectSection
EndProject
"@

# Find the insertion point (before "Global")
$insertBeforeGlobal = "Global"
$insertionIndex = $slnContent.IndexOf($insertBeforeGlobal)

if ($insertionIndex -lt 0) {
    Write-Error "Could not find 'Global' section in solution file"
    exit 1
}

# Insert the new projects
$before = $slnContent.Substring(0, $insertionIndex)
$after = $slnContent.Substring($insertionIndex)
$newContent = $before + $newProjects + $after

# Add nested projects section if it doesn't exist
if ($newContent -notmatch "GlobalSection\(NestedProjects\)") {
    # Find ExtensibilityGlobals section
    $extIndex = $newContent.IndexOf("GlobalSection(ExtensibilityGlobals)")
    if ($extIndex -gt 0) {
        $nestedSection = @"
    GlobalSection(NestedProjects) = preSolution
        $auditGuid = $docGuid
        $guidesGuid = $auditGuid
        $checklistsGuid = $auditGuid
    EndGlobalSection

"@
        $before = $newContent.Substring(0, $extIndex)
        $after = $newContent.Substring($extIndex)
        $newContent = $before + $nestedSection + $after
    }
}
else {
    # Nested section exists, add our entries
    $pattern = "(GlobalSection\(NestedProjects\) = preSolution[\r\n]+)"
    $replacement = "`$1`t`t$auditGuid = $docGuid`r`n`t`t$guidesGuid = $auditGuid`r`n`t`t$checklistsGuid = $auditGuid`r`n"
    $newContent = $newContent -replace $pattern, $replacement
}

# Backup original
$backupPath = "$slnPath.backup"
Copy-Item $slnPath $backupPath -Force
Write-Host "Created backup: $backupPath" -ForegroundColor Yellow

# Write updated solution file
Set-Content -Path $slnPath -Value $newContent -Encoding UTF8 -NoNewline

Write-Host ""
Write-Host "✓ Solution file updated!" -ForegroundColor Green
Write-Host ""
Write-Host "Added to Solution Explorer:" -ForegroundColor Cyan
Write-Host "  └─ Documentation/" -ForegroundColor White
Write-Host "     └─ FractalReviewAudit/" -ForegroundColor White
Write-Host "        ├─ README.md" -ForegroundColor Gray
Write-Host "        ├─ START_HERE.md" -ForegroundColor Gray
Write-Host "        ├─ YOU_ARE_HERE.md" -ForegroundColor Gray
Write-Host "        ├─ AUDIT_SUMMARY.md" -ForegroundColor Gray
Write-Host "        ├─ Guides/" -ForegroundColor White
Write-Host "        │  ├─ WORKFLOW_VISUAL.md" -ForegroundColor Gray
Write-Host "        │  ├─ CHEAT_SHEET.md" -ForegroundColor Gray
Write-Host "        │  └─ QUICK_START.md" -ForegroundColor Gray
Write-Host "        └─ Checklists/" -ForegroundColor White
Write-Host "           └─ TIER1_CRITICAL_FRACTALS.md" -ForegroundColor Gray
Write-Host ""
Write-Host "Close and reopen Visual Studio to see the changes!" -ForegroundColor Yellow
Write-Host ""
Write-Host "If something went wrong, restore from: $backupPath" -ForegroundColor Gray
