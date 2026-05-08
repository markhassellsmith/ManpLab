# Simple Audit Workflow - Visual Guide

## The Ultra-Simple Process

```
┌─────────────────────────────────────────────────────────┐
│  STEP 1: Open These Two Things                         │
│  ────────────────────────────────────────────────       │
│  1. docs\audits\TIER1_CRITICAL_FRACTALS.md (editor)    │
│  2. ManpLab application (running)                       │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────┐
│  STEP 2: Pick a Fractal from the List                  │
│  ────────────────────────────────────────────────       │
│  Start with #1 (Mandelbrot)                            │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────┐
│  STEP 3: Launch It in ManpLab                          │
│  ────────────────────────────────────────────────       │
│  Find it in the fractal browser → Click → View it      │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────┐
│  STEP 4: Answer 3 Quick Questions                      │
│  ────────────────────────────────────────────────       │
│  ✓ Does it render without crashing?                    │
│  ✓ How fast? (note the render time)                    │
│  ✓ Does it look correct?                               │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────┐
│  STEP 5: Mark Checkboxes in the File                   │
│  ────────────────────────────────────────────────       │
│  Change [ ] to [x] for things you checked              │
│  Add notes about render time and any issues            │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────┐
│  REPEAT: Do 4 More Fractals (5 total)                  │
│  ────────────────────────────────────────────────       │
│  Same process for Julia, Burning Ship, Newton, Phoenix │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────┐
│  STEP 6: Save Your Work                                │
│  ────────────────────────────────────────────────       │
│  Save file → git commit → git push                     │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────┐
│  DONE! Take a Break! 🎉                                │
│  ────────────────────────────────────────────────       │
│  You audited 5 fractals! Come back later for 5 more.   │
└─────────────────────────────────────────────────────────┘
```

---

## What Your Screen Should Look Like

```
┌─────────────────────────────┬──────────────────────────────┐
│                             │                              │
│   EDITOR/VS CODE            │      MANPLAB RUNNING         │
│                             │                              │
│   docs\audits\              │   ┌────────────────────┐     │
│   TIER1_CRITICAL_           │   │  Fractal Browser   │     │
│   FRACTALS.md               │   ├────────────────────┤     │
│                             │   │ □ Mandelbrot       │     │
│   #### 1. Mandelbrot        │   │ □ Julia            │     │
│   Audit Checklist:          │   │ □ Burning Ship     │ ←── │
│   - [x] Formula correct     │   │ □ Newton           │     │
│   - [x] Default view        │   │ □ Phoenix          │  Click these!
│   - [x] Performance good    │   └────────────────────┘     │
│                             │                              │
│   Test Notes:               │   [Fractal Display Area]     │
│   - Render time: 0.3s       │   ┌────────────────────┐     │
│   - Issues: None            │   │  ░░▒▒▓▓████▓▓▒▒░░  │     │
│   - Overall: ☑ Pass         │   │  ░▒▓███████████▓▒░  │  ←── Watch it
│                             │   │  ▒▓████████████▓▒  │     render!
│                             │   │  ░▒▓███████████▓▒░  │     │
│                             │   │  ░░▒▒▓▓████▓▓▒▒░░  │     │
│                             │   └────────────────────┘     │
│                             │   Render time: 0.32s         │
│                             │                              │
└─────────────────────────────┴──────────────────────────────┘
```

**Two windows open side-by-side is ideal!**

---

## The 3 Key Questions (Per Fractal)

### 1. Does it work? ✅
```
Launch fractal → Does it render?
  ├─ YES: Great! Continue.
  └─ NO:  Note "crashes" in Issues section.
```

### 2. How fast? ⏱️
```
Look at render time
  ├─ < 1 second:   ✅ Good
  ├─ 1-3 seconds:  ⚠️ Acceptable
  └─ > 3 seconds:  ❌ Note as slow
```

### 3. Does it look right? 👀
```
Visual check
  ├─ Looks like expected fractal: ✅ Good
  ├─ Weird but renders:           ⚠️ Note it
  └─ Blank or all one color:      ❌ Problem
```

**Time per fractal: ~4 minutes**

---

## Checkbox Quick Reference

When you see this in TIER1_CRITICAL_FRACTALS.md:

```markdown
**Audit Checklist:**
- [ ] Formula correct
- [ ] Default view OK
- [ ] Performance good
- [ ] Julia mode works
- [ ] Zooming works
- [ ] Description accurate
```

Change to this as you test:

```markdown
**Audit Checklist:**
- [x] Formula correct         ← Put x when checked
- [x] Default view OK
- [x] Performance good
- [ ] Julia mode works        ← Can skip for now
- [ ] Zooming works           ← Can skip for now
- [x] Description accurate
```

**Minimum:** Check the first 3 and last 1 (formula, view, performance, description).

---

## Example: Your First Fractal (Mandelbrot)

### In ManpLab:
1. Open fractal browser
2. Find "Mandelbrot Set"
3. Click it
4. Watch it render
5. Note the time (e.g., "0.32s")

### In Your Editor:
```markdown
#### 1. Mandelbrot

**Audit Checklist:**
- [x] Formula correct (z² + c)
- [x] Default center shows main body and bulbs
- [x] Performance good (< 1s)
- [ ] Julia mode works
- [ ] Zooming reveals infinite detail
- [x] Description accurate

**Test Notes:**
- Render time: 0.32s
- Issues found: None
- Overall: ☑ Pass ☐ Fail ☐ Needs Work
```

**That's it!** On to the next fractal!

---

## Progress Tracker

Use this to track your sessions:

### Session 1 (Today):
```
Audited: Mandelbrot, Julia, Burning Ship, Newton, Phoenix
Status: [ ] Done
```

### Session 2:
```
Audited: _______________________________________
Status: [ ] Done
```

### Session 3:
```
Audited: _______________________________________
Status: [ ] Done
```

### Session 4:
```
Audited: _______________________________________
Status: [ ] Done
```

### Session 5:
```
Audited: _______________________________________
Status: [ ] Done
```

### Session 6:
```
Audited: _______________________________________
Status: [ ] Done
```

**After 6 sessions = 30 Tier 1 fractals complete!** 🏆

---

## Git Commands (Copy-Paste Ready)

After each session:

```powershell
# Save your work
git add docs\audits\TIER1_CRITICAL_FRACTALS.md
git commit -m "Audited 5 Tier 1 fractals"
git push
```

---

## Quick Tips

✅ **Start simple** - Just check if it renders and looks OK  
✅ **5 at a time** - Don't burn out!  
✅ **Use a timer** - Set 20 minutes and see how many you finish  
✅ **Take notes** - Even brief notes help later  
✅ **Don't perfect** - Good enough is good enough!  

---

## What Success Looks Like

After your first session, you should have:

1. ✅ 5 fractals with checkboxes marked
2. ✅ Render times noted
3. ✅ Any issues documented
4. ✅ File saved and committed to git
5. ✅ Feeling accomplished! 🎉

**Total time: ~20-25 minutes**

---

## Stuck? Try This

### Can't find a fractal in ManpLab?
- Check the category in the browser
- Try searching by name
- If missing, note "Not found in browser" and move on

### Don't know if it looks "right"?
- Google image search: "[fractal name] fractal"
- Compare to Wikipedia images
- If it looks like a fractal (not blank), probably OK

### Render time not showing?
- Check bottom status bar
- Check window title
- Estimate: "fast" (< 1s) or "slow" (> 3s)

### Crashes?
- Note "Crashes on launch"
- Mark as FAIL
- Move to next fractal

---

**Ready? Open these two files and let's go!**

1. `docs\audits\TIER1_CRITICAL_FRACTALS.md`
2. ManpLab application

**You got this! 🚀**
