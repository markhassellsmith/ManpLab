# Fractal Review Audit Documentation

**Branch:** `qualitycheck/fractal-review-audit`  
**Purpose:** Quality assurance for all 300 fractals in ManpLab  
**Status:** Ready to begin

---

## 🚀 Quick Start

**New to auditing? Start here:**

1. **[START_HERE.md](START_HERE.md)** - Complete beginner's guide (read this first!)
2. **[Checklists/TIER1_CRITICAL_FRACTALS.md](Checklists/TIER1_CRITICAL_FRACTALS.md)** - The actual checklist to edit
3. Launch ManpLab and audit your first 5 fractals!

**Time required:** 30 minutes for your first session

---

## 📁 Folder Structure

```
Documentation/FractalReviewAudit/
│
├── START_HERE.md                      ← Read this first!
├── YOU_ARE_HERE.md                    ← Quick navigation
├── AUDIT_SUMMARY.md                   ← Strategy & progress tracking
├── README.md                          ← This file
│
├── Guides/
│   ├── WORKFLOW_VISUAL.md            ← Visual process guide
│   ├── CHEAT_SHEET.md                ← One-page reference
│   └── QUICK_START.md                ← Detailed workflow
│
└── Checklists/
    └── TIER1_CRITICAL_FRACTALS.md    ← The actual audit checklist
```

---

## 📚 Document Guide

### Entry Points (Choose One)

| Document | Best For | Time |
|----------|----------|------|
| **[START_HERE.md](START_HERE.md)** | Complete beginners | 10 min read + 20 min first audit |
| **[YOU_ARE_HERE.md](YOU_ARE_HERE.md)** | Quick navigation | 1 min |
| **[Guides/WORKFLOW_VISUAL.md](Guides/WORKFLOW_VISUAL.md)** | Visual learners | 5 min |
| **[Guides/CHEAT_SHEET.md](Guides/CHEAT_SHEET.md)** | Quick reference | Printable |

### Working Documents

| Document | Purpose |
|----------|---------|
| **[Checklists/TIER1_CRITICAL_FRACTALS.md](Checklists/TIER1_CRITICAL_FRACTALS.md)** | The actual checklist - edit this! |
| **[AUDIT_SUMMARY.md](AUDIT_SUMMARY.md)** | Overall strategy and progress |

### Supporting Materials

| Document | Purpose |
|----------|---------|
| **[Guides/QUICK_START.md](Guides/QUICK_START.md)** | Detailed workflow guide |
| **../../Scripts/** | Automation scripts (validation, etc.) |

---

## 🎯 The Audit Process

### Goal
Verify that 300 fractals in ManpLab are:
- ✅ Mathematically correct
- ✅ Visually appealing
- ✅ Performant
- ✅ Properly documented

### Approach
**Tiered Priority System:**

1. **Tier 1: Critical Fractals** (30 fractals) - START HERE
   - Most important fractals
   - Well-known and frequently used
   - Target: Week 1

2. **Tier 2: Standard Fractals** (100 fractals)
   - Interesting variations
   - Notable mathematical types
   - Target: Week 2-3

3. **Tier 3: Extended Collection** (170 fractals)
   - Specialty fractals
   - Experiments and rare types
   - Audit as time permits

### Session Plan
- **Duration:** 20-30 minutes per session
- **Quantity:** 5-10 fractals per session
- **Frequency:** Daily or as available
- **Total Time:** ~6 hours for Tier 1 (30 fractals)

---

## ✅ What to Check

For each fractal, verify:

1. **Launch Test** - Does it render without crashing?
2. **Performance** - Render time < 3 seconds?
3. **Visual Quality** - Looks mathematically correct?
4. **Formula Accuracy** - Implementation matches definition?
5. **Julia Mode** - (If applicable) Works correctly?
6. **Description** - Display name and info accurate?

**Minimum criteria:**
- Launches without error
- Renders in reasonable time
- Visual output looks correct

---

## 📊 Progress Tracking

### Current Status
- **Tier 1:** 0 / 30 (0%)
- **Tier 2:** 0 / 100 (0%)
- **Tier 3:** 0 / 170 (0%)
- **Total:** 0 / 300 (0%)

**Update this in:** [AUDIT_SUMMARY.md](AUDIT_SUMMARY.md)

---

## 🔧 Tools Available

### Scripts Location
See `../../Scripts/` folder:
- `Test-FractalBasics.ps1` - Validates workspace integrity
- `Split-AuditFile.ps1` - Creates category-based audit files
- `README.md` - Script documentation

### Running Scripts
```powershell
# From solution root
.\Scripts\Test-FractalBasics.ps1

# Split audit by category (optional)
.\Scripts\Split-AuditFile.ps1
```

---

## 🎓 Workflow Overview

```
1. Read START_HERE.md
   ↓
2. Open Checklists/TIER1_CRITICAL_FRACTALS.md
   ↓
3. Launch ManpLab
   ↓
4. For each fractal:
   - Find it in ManpLab browser
   - View and test it
   - Mark checkboxes in checklist
   - Add notes
   ↓
5. Save and commit progress
   ↓
6. Update AUDIT_SUMMARY.md (optional)
   ↓
7. Take a break!
   ↓
8. Repeat for next session
```

---

## 💡 Best Practices

### Do:
✅ Start with Tier 1 (most important)  
✅ Work in short sessions (20-30 min)  
✅ Audit 5-10 fractals at a time  
✅ Commit progress frequently  
✅ Take breaks between sessions  
✅ Use the cheat sheet for reference  

### Don't:
❌ Try to audit all 300 at once  
❌ Spend hours on one fractal  
❌ Skip documentation  
❌ Forget to save your work  
❌ Burn yourself out  

---

## 📞 Getting Help

### Documentation Unclear?
1. Check [START_HERE.md](START_HERE.md) for step-by-step
2. Review [Guides/WORKFLOW_VISUAL.md](Guides/WORKFLOW_VISUAL.md) for pictures
3. Use [Guides/CHEAT_SHEET.md](Guides/CHEAT_SHEET.md) for quick reference

### Technical Issues?
1. Run `..\..\Scripts\Test-FractalBasics.ps1`
2. Check build logs in Visual Studio
3. Verify ManpLab launches correctly

### Fractal Issues?
1. Google "[fractal name] fractal" for reference images
2. Check Wikipedia or Wolfram MathWorld
3. Look at source code in `ManpCore.Native\*Family.cpp`
4. Note issues in checklist and move on

---

## 🏆 Success Metrics

You're on track if you:
- ✅ Read at least START_HERE.md
- ✅ Audited at least 5 fractals
- ✅ Marked checkboxes in the checklist
- ✅ Saved and committed your work
- ✅ Know how to continue next time

---

## 📝 Contributing

When you complete a session:

```powershell
# Save your work
git add Documentation/FractalReviewAudit/Checklists/TIER1_CRITICAL_FRACTALS.md
git commit -m "Audited fractals: [list names]"
git push origin qualitycheck/fractal-review-audit
```

---

## 🎯 Next Action

**Right now:**

1. Open **[START_HERE.md](START_HERE.md)**
2. Read it (10 minutes)
3. Open **[Checklists/TIER1_CRITICAL_FRACTALS.md](Checklists/TIER1_CRITICAL_FRACTALS.md)**
4. Launch ManpLab
5. Audit your first 5 fractals (20 minutes)
6. Commit your work!

**Total time: 30 minutes**

---

## 📖 Related Documentation

- **Architecture:** `ManpWinUI\docs\ARCHITECTURE_NATIVE_ENGINE.md`
- **Fractal Development:** `ManpCore.Native\ADDING_FRACTALS.md`
- **Knowledge Base:** `ManpWinUI\docs\FRACTAL_KNOWLEDGE_BASE_PLAN.md`
- **Registry Progress:** `ManpWinUI\docs\FRACTAL_REGISTRY_PROGRESS.md`

---

**Let's audit some fractals! Start with [START_HERE.md](START_HERE.md)** 🚀
