# Visual Studio Not Showing Documentation Folder?

## Quick Fix

### Method 1: Reload Solution (Recommended)
1. In Visual Studio, **close the solution**: File → Close Solution
2. **Reopen** the solution: File → Open → Project/Solution → `ManpLab.sln`
3. Look for **"Documentation"** folder in Solution Explorer
4. Expand it to see **"FractalReviewAudit"**

### Method 2: Restart Visual Studio
1. Close Visual Studio completely
2. Reopen `ManpLab.sln`
3. Check Solution Explorer for "Documentation" folder

### Method 3: Manually Add via UI (If above don't work)
1. Right-click solution in Solution Explorer
2. Add → New Solution Folder → Name it "**Documentation**"
3. Right-click "Documentation" folder
4. Add → New Solution Folder → Name it "**FractalReviewAudit**"
5. Right-click "FractalReviewAudit" folder
6. Add → Existing Item...
7. Browse to `Documentation\FractalReviewAudit\`
8. Select all `.md` files → Add
9. Repeat for subdirectories (Guides, Checklists)

---

## Verification

After reloading, you should see:
```
Solution 'ManpLab'
├── ManpCore.Native
├── ManpCore.Services  
├── ManpWinUI
└── Documentation          ← Should appear here!
    └── FractalReviewAudit
        ├── README.md
        ├── START_HERE.md
        ├── YOU_ARE_HERE.md
        ├── AUDIT_SUMMARY.md
        ├── Guides (folder)
        └── Checklists (folder)
```

---

## Files ARE on Disk

The files exist at:
```
C:\Users\Mark\source\repos\ManpLab\Documentation\FractalReviewAudit\
```

You can browse them in Windows Explorer if Solution Explorer doesn't show them yet.

---

## Alternative: Use Files Directly

**Don't need Solution Explorer?** Just open the files:

### Via File Explorer
1. Open Windows Explorer
2. Navigate to: `C:\Users\Mark\source\repos\ManpLab\Documentation\FractalReviewAudit\`
3. Double-click `START_HERE.md` to open in VS Code or default editor

### Via Visual Studio
1. File → Open → File
2. Browse to: `Documentation\FractalReviewAudit\START_HERE.md`
3. Open it

### Via Command
```powershell
# Open in VS Code
code Documentation\FractalReviewAudit\START_HERE.md

# Or in default markdown editor
start Documentation\FractalReviewAudit\START_HERE.md
```

---

## Why This Happened

Visual Studio loads the solution file once when you open it. Changes to `.sln` made externally (by scripts) require:
- Reloading the solution, OR
- Restarting Visual Studio

The files and solution entries are correct - VS just needs to re-read them!

---

## Troubleshooting

### Still not showing after reload?
Check the solution file has the entries:
```powershell
Select-String -Path "ManpLab.sln" -Pattern "Documentation"
```

Should show multiple matches including "FractalReviewAudit"

### Solution file missing entries?
Run the script again:
```powershell
.\Scripts\Add-AuditDocsToSolution.ps1
```

Then reload Visual Studio.

---

## Quick Start (Without Solution Explorer)

**Want to start auditing right now?**

1. Open in any editor:
   ```
   Documentation\FractalReviewAudit\START_HERE.md
   ```

2. Open the checklist:
   ```
   Documentation\FractalReviewAudit\Checklists\TIER1_CRITICAL_FRACTALS.md
   ```

3. Launch ManpLab and start auditing!

The files work perfectly outside of Solution Explorer! ✨
