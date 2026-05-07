# Fractal Audit Scripts

This directory contains PowerShell utilities to help with the fractal quality audit process.

## Scripts

### Test-FractalBasics.ps1
**Purpose:** Automated validation of basic fractal metadata integrity

**What it checks:**
- ✅ Source files exist (all *Family.cpp files)
- ✅ Header files exist (Fractype.h)
- ✅ JSON metadata files are valid
- ✅ Audit documentation structure
- ✅ Fractal definitions in Fractype.h
- ✅ No duplicate fractal IDs
- ✅ Project structure is intact

**Usage:**
```powershell
# Basic run - shows only failures
.\Scripts\Test-FractalBasics.ps1

# Verbose mode - shows all tests
.\Scripts\Test-FractalBasics.ps1 -Verbose
```

**When to use:**
- Before starting an audit session
- After making changes to source files
- To verify workspace integrity
- As part of build validation

---

### Split-AuditFile.ps1
**Purpose:** Split the massive audit checklist into category-based files

**What it does:**
- Reads `docs\FRACTAL_QUALITY_AUDIT_CHECKLIST.md`
- Extracts each category section
- Creates separate audit files in `docs\audits\`
- Each file contains 10-40 fractals (manageable size)

**Usage:**
```powershell
# Dry run - preview what would be created
.\Scripts\Split-AuditFile.ps1 -DryRun

# Actually split the file
.\Scripts\Split-AuditFile.ps1
```

**When to use:**
- When you need category-specific audit files
- If working on a specific fractal family
- For parallel audit work across team members

**Output:**
Creates files like:
- `docs\audits\AUDIT_Classic_Fractals.md`
- `docs\audits\AUDIT_Julia_Sets.md`
- `docs\audits\AUDIT_Newton_Fractals.md`
- etc.

---

## Audit Workflow

### Step 1: Validate Workspace
```powershell
.\Scripts\Test-FractalBasics.ps1
```
Ensures your workspace is ready for auditing.

### Step 2: Review Strategy
Open `docs\FRACTAL_AUDIT_SUMMARY.md` to understand the tiered approach.

### Step 3: Start with Tier 1
Open `docs\audits\TIER1_CRITICAL_FRACTALS.md` and audit the 30 most important fractals.

### Step 4: (Optional) Split by Category
```powershell
.\Scripts\Split-AuditFile.ps1
```
If you prefer working with category-specific files.

### Step 5: Audit Session
1. Pick a file (Tier 1, Tier 2, or a specific category)
2. Audit 5-10 fractals (30-60 minutes)
3. Mark checkboxes and add notes
4. Commit your progress

### Step 6: Track Progress
Update `docs\FRACTAL_AUDIT_SUMMARY.md` with completion counts.

---

## Tips

### For Efficient Auditing:
- 🎯 **Focus on one tier/category at a time**
- ⏱️ **Limit sessions to 30-60 minutes**
- 📝 **Document issues immediately**
- 💾 **Commit progress frequently**
- 🔄 **Run Test-FractalBasics.ps1 regularly**

### For Quality Checks:
- ✅ Launch the fractal in ManpLab
- ✅ Check visual correctness
- ✅ Test performance (note render time)
- ✅ Verify formula in source code
- ✅ Test Julia mode (if applicable)
- ✅ Try zooming in/out

### For Mathematical Verification:
- 📚 Check Wikipedia for well-known fractals
- 🔬 Consult Wolfram MathWorld
- 💬 Review fractal forums
- 📄 Look for original papers
- 👀 Read source code comments

---

## Future Script Ideas

### Test-FractalLaunch.ps1 (Not yet implemented)
Could programmatically launch fractals and verify they render.

### Generate-ProgressReport.ps1 (Not yet implemented)
Could parse audit files and generate progress statistics.

### Compare-FractalFormulas.ps1 (Not yet implemented)
Could extract formulas from source and compare to documentation.

---

## Contributing

When adding new scripts:
1. Follow the existing naming convention
2. Add parameter validation
3. Include help documentation
4. Update this README
5. Test in both success and failure scenarios

---

## Related Documentation

- **[Audit Summary](../docs/FRACTAL_AUDIT_SUMMARY.md)** - Overview and strategy
- **[Tier 1 Fractals](../docs/audits/TIER1_CRITICAL_FRACTALS.md)** - Critical fractals to audit first
- **[Original Massive Checklist](../docs/FRACTAL_QUALITY_AUDIT_CHECKLIST.md)** - Archived reference

---

## Notes

These scripts are designed to make the fractal audit process manageable and efficient. The original 5,223-line, 178KB audit file was too large to be practical. This tiered, category-based approach with automation support allows focused, productive audit sessions.

**Remember:** Audit 5-10 fractals at a time, not 300 at once! 😊
