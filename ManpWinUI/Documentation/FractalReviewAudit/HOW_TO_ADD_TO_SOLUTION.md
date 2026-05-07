# Adding Documentation to Solution Explorer

## Automatic Method (Recommended)

### In Visual Studio:
1. Right-click the solution in Solution Explorer
2. Add → New Solution Folder → Name it "Documentation"
3. Right-click "Documentation" folder
4. Add → New Solution Folder → Name it "FractalReviewAudit"
5. Right-click "FractalReviewAudit" folder
6. Add → Existing Item → Browse to `Documentation\FractalReviewAudit\`
7. Select all `.md` files and add them
8. Repeat for subdirectories (Guides, Checklists)

### Result in Solution Explorer:
```
Solution 'ManpLab'
├── ManpCore.Native
├── ManpCore.Services
├── ManpWinUI
└── Documentation (Solution Folder)
    └── FractalReviewAudit (Solution Folder)
        ├── README.md
        ├── START_HERE.md
        ├── YOU_ARE_HERE.md
        ├── AUDIT_SUMMARY.md
        ├── Guides (Solution Folder)
        │   ├── WORKFLOW_VISUAL.md
        │   ├── CHEAT_SHEET.md
        │   └── QUICK_START.md
        └── Checklists (Solution Folder)
            └── TIER1_CRITICAL_FRACTALS.md
```

---

## Manual Method (If Automatic Doesn't Work)

Edit `ManpLab.sln` directly and add before `Global` section:

```
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Documentation", "Documentation", "{GUID1}"
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "FractalReviewAudit", "FractalReviewAudit", "{GUID2}"
    ProjectSection(SolutionItems) = preProject
        Documentation\FractalReviewAudit\README.md = Documentation\FractalReviewAudit\README.md
        Documentation\FractalReviewAudit\START_HERE.md = Documentation\FractalReviewAudit\START_HERE.md
        Documentation\FractalReviewAudit\YOU_ARE_HERE.md = Documentation\FractalReviewAudit\YOU_ARE_HERE.md
        Documentation\FractalReviewAudit\AUDIT_SUMMARY.md = Documentation\FractalReviewAudit\AUDIT_SUMMARY.md
    EndProjectSection
EndProject
```

Then in the `GlobalSection(NestedProjects)`:
```
{GUID2} = {GUID1}
```

---

## Quick Add via PowerShell

Run this script to add files to solution:

```powershell
# This would need to be run from Visual Studio's Package Manager Console
# or use dotnet sln command

dotnet sln ManpLab.sln add Documentation\FractalReviewAudit\README.md
```

---

## Verification

After adding, you should see in Solution Explorer:
- ✅ Documentation folder (solution folder icon)
- ✅ FractalReviewAudit subfolder
- ✅ All markdown files visible
- ✅ Can double-click to open in editor

---

## Files to Add

### Root Level
- README.md
- START_HERE.md
- YOU_ARE_HERE.md
- AUDIT_SUMMARY.md

### Guides Subfolder
- WORKFLOW_VISUAL.md
- CHEAT_SHEET.md
- QUICK_START.md

### Checklists Subfolder
- TIER1_CRITICAL_FRACTALS.md

---

**Easiest method:** Use Visual Studio's Add → Existing Item UI!
