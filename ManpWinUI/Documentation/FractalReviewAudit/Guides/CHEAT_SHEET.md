# Audit Cheat Sheet - One Page Reference

## The Absolute Basics

**Goal:** Check if 30 critical fractals work correctly  
**Time:** ~20 minutes per session, 6 sessions total  
**File to edit:** `docs\audits\TIER1_CRITICAL_FRACTALS.md`

---

## Quick Start (First Time)

1. Open `docs\audits\TIER1_CRITICAL_FRACTALS.md`
2. Open ManpLab
3. Audit fractals #1-5
4. Save and commit

---

## Per Fractal (4 minutes each)

### 1. Launch (30 sec)
- Find fractal in ManpLab browser
- Click to view it

### 2. Observe (30 sec)
- Does it render?
- Does it look right?
- Note render time

### 3. Check (2 min)
- Mark checkboxes: `[ ]` → `[x]`
- Write render time
- Note any issues

### 4. Next! (repeat)

---

## What to Check

| Item | Check This | Time |
|------|------------|------|
| **Formula correct** | Visual looks right? | 10s |
| **Default view** | Shows interesting features? | 5s |
| **Performance** | < 1 second? | 5s |
| **Description** | Name makes sense? | 5s |

**Optional:** Julia mode, zooming (skip first pass)

---

## Mark Checkboxes

**Before:**
```markdown
- [ ] Formula correct
- [ ] Performance good
```

**After:**
```markdown
- [x] Formula correct
- [x] Performance good
```

---

## Add Notes

```markdown
**Test Notes:**
- Render time: 0.3s          ← Actual time
- Issues found: None         ← Or list problems
- Overall: ☑ Pass            ← Mark one
```

---

## Performance Guide

- ✅ **< 1 second:** Good
- ⚠️ **1-3 seconds:** Acceptable
- ❌ **> 3 seconds:** Note as slow

---

## Visual Checks

### Mandelbrot
- Main body + circular bulb = ✅
- Blank screen = ❌

### Julia
- Islands or swirls = ✅
- Solid color = ❌

### Burning Ship
- Ship shape = ✅
- Looks identical to Mandelbrot = ❌

### Newton
- 3-4 colored regions = ✅
- Solid color = ❌

---

## Git Commands

```powershell
# After each session
git add docs\audits\TIER1_CRITICAL_FRACTALS.md
git commit -m "Audited 5 fractals"
git push
```

---

## Session Plan

- **Session 1:** Fractals 1-5 (Mandelbrot → Phoenix)
- **Session 2:** Fractals 6-10
- **Session 3:** Fractals 11-15
- **Session 4:** Fractals 16-20
- **Session 5:** Fractals 21-25
- **Session 6:** Fractals 26-30
- **DONE!** 🏆

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Can't find fractal | Search in ManpLab or skip it |
| Don't know if correct | Google image search |
| Crashes | Mark as FAIL, note "crashes" |
| Too slow | Note render time, mark issue |
| No render time shown | Estimate: fast/slow |

---

## Files Reference

| File | Purpose |
|------|---------|
| `START_HERE.md` | Detailed first-time guide |
| `WORKFLOW_VISUAL.md` | Pictures and diagrams |
| `TIER1_CRITICAL_FRACTALS.md` | The actual checklist |
| `FRACTAL_AUDIT_SUMMARY.md` | Overall strategy |

---

## Quick Tips

✅ Do 5 fractals per session  
✅ Don't overthink it  
✅ Save/commit after each session  
✅ Take breaks between sessions  
✅ Skip optional items first pass  

---

## Success Checklist

After first session:
- [ ] Opened TIER1_CRITICAL_FRACTALS.md
- [ ] Launched ManpLab
- [ ] Audited 5 fractals
- [ ] Marked checkboxes
- [ ] Added notes
- [ ] Saved file
- [ ] Committed to git
- [ ] Feeling good! 🎉

---

**Print this page and keep it next to you while auditing!**
