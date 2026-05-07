# 🚀 START HERE - Your First Fractal Audit

**Welcome!** This guide will walk you through auditing your first 5 fractals in about 20 minutes.

---

## Step 1: Open the Tier 1 File (30 seconds)

📂 Open this file in Visual Studio or your text editor:
```
docs\audits\TIER1_CRITICAL_FRACTALS.md
```

Keep this file open in one window/tab. You'll be editing it as you go.

---

## Step 2: Launch ManpLab (30 seconds)

1. Run ManpLab application
2. Navigate to the fractal browser
3. Keep ManpLab open alongside your editor

**You now have two windows:**
- Window 1: TIER1_CRITICAL_FRACTALS.md (for taking notes)
- Window 2: ManpLab (for viewing fractals)

---

## Step 3: Audit Your First Fractal - Mandelbrot (3-4 minutes)

### Find it in the file
Scroll to **"1. Mandelbrot"** in TIER1_CRITICAL_FRACTALS.md

### Launch it in ManpLab
1. In ManpLab, find "Mandelbrot Set" in the browser
2. Click to view it
3. Watch it render

### Answer these 3 questions:

#### Question 1: Does it render? ✅
- If you see a black shape with colorful borders → **YES**
- If it crashes or shows error → **NO** (note the problem)

#### Question 2: How fast? ⏱️
- Look at the render time (bottom of window or status bar)
- Write the time next to "Render time: ___s" in the file

#### Question 3: Does it look right? 👀
- Does it look like the classic Mandelbrot set?
- Big round body on the left
- Circular bulb on the right
- If it looks weird, note it

### Mark your checklist
In TIER1_CRITICAL_FRACTALS.md, find the Mandelbrot section:

```markdown
**Audit Checklist:**
- [ ] Formula correct (z² + c)
- [ ] Default center shows main body and bulbs
- [ ] Performance good (< 1s)
- [ ] Julia mode works
- [ ] Zooming reveals infinite detail
- [ ] Description accurate
```

Change `[ ]` to `[x]` for items you checked:

```markdown
**Audit Checklist:**
- [x] Formula correct (z² + c)         ← Put x if it looks correct
- [x] Default center shows main body and bulbs
- [x] Performance good (< 1s)           ← Put x if < 1 second
- [ ] Julia mode works                  ← Skip for now if you want
- [ ] Zooming reveals infinite detail   ← Skip for now if you want
- [x] Description accurate
```

### Add your notes
```markdown
**Test Notes:**
- Render time: 0.3s                     ← Write the actual time
- Issues found: None                    ← Or note problems
- Overall: ☑ Pass ☐ Fail ☐ Needs Work  ← Mark one with ☑
```

**🎉 Congrats! You just audited your first fractal!**

---

## Step 4: Do Four More (15 minutes)

Repeat Step 3 for these fractals:
- **2. Julia** (in TIER1_CRITICAL_FRACTALS.md)
- **3. Burning Ship**
- **4. Newton (z³-1)**
- **5. Phoenix**

Each one takes 3-4 minutes. Same process:
1. Find fractal in ManpLab
2. Watch it render
3. Note the time
4. Check if it looks right
5. Mark checkboxes in the file
6. Add notes

**Tips:**
- Don't overthink it!
- If it looks good and renders fast → check the boxes
- If something seems wrong → note it
- You can skip detailed checks (like Julia mode) for now

---

## Step 5: Save Your Work (1 minute)

### In VS Code or your editor:
- Save the TIER1_CRITICAL_FRACTALS.md file

### In Terminal:
```powershell
git add docs\audits\TIER1_CRITICAL_FRACTALS.md
git commit -m "Audited first 5 Tier 1 fractals: Mandelbrot, Julia, Burning Ship, Newton, Phoenix"
git push
```

**Done!** You've audited 5 fractals! 🎉

---

## Step 6: Take a Break! (Optional but recommended)

You did great! Come back later or tomorrow to audit 5 more.

**Progress so far:**
- ✅ Completed: 5 / 30 Tier 1 fractals (17%)
- Remaining: 25 fractals
- Sessions needed: 5 more sessions like this

---

## What Each Checkbox Means (Quick Reference)

When auditing, here's what to check:

| Checkbox | What to Check | How Long |
|----------|---------------|----------|
| **Formula correct** | Does the math look right? (Quick glance at description) | 10 sec |
| **Default view** | Does the starting view show interesting features? | 5 sec |
| **Performance good** | Renders in < 1 second? | 5 sec |
| **Julia mode works** | (Optional) Try switching to Julia mode | 30 sec |
| **Zooming works** | (Optional) Try zooming in | 30 sec |
| **Description accurate** | Does the name/description make sense? | 10 sec |

**Minimum to pass:** Formula looks ok + Renders + Performance acceptable

---

## Common Questions

### Q: What if I don't know the correct formula?
**A:** Just check if it looks visually correct. If the Mandelbrot looks like a Mandelbrot, you're good!

### Q: What if it's slow?
**A:** Note the render time. If it's > 3 seconds, mark it as an issue.

### Q: What if it crashes?
**A:** Mark it as FAIL and note "Crashes on launch" in the Issues section.

### Q: Do I need to check Julia mode?
**A:** Not required for your first pass! Check the basic stuff first.

### Q: Can I skip fractals?
**A:** Yes, but try to do them in order. The Tier 1 fractals are the most important.

### Q: How many should I do per session?
**A:** 5-10 fractals. About 20-40 minutes total.

---

## Next Session

When you come back:
1. Open `docs\audits\TIER1_CRITICAL_FRACTALS.md`
2. Find where you left off (look for unchecked fractals)
3. Audit 5 more fractals
4. Save and commit

**After 6 sessions, you'll have completed all 30 Tier 1 fractals!** 🏆

---

## Visual Guide: What Good Fractals Look Like

### Mandelbrot Set
- **Expect:** Black main body (cardioid) on left, circular bulb on right, colorful bands
- **Red flag:** Completely blank, all one color, crashes

### Julia Set
- **Expect:** Disconnected islands or connected swirls (depends on parameters)
- **Red flag:** Solid color, nothing visible

### Burning Ship
- **Expect:** Shape that looks like a ship with sails
- **Red flag:** Looks identical to normal Mandelbrot

### Newton Fractals
- **Expect:** Usually 3 or 4 colored regions with fractal boundaries
- **Red flag:** Solid color, no boundaries

### Phoenix
- **Expect:** Organic, flowing shapes, not like standard Mandelbrot
- **Red flag:** Crashes or blank

---

## Quick Commands Reference

### Run validation script:
```powershell
.\Scripts\Test-FractalBasics.ps1
```

### Commit your progress:
```powershell
git add docs\audits\TIER1_CRITICAL_FRACTALS.md
git commit -m "Audited fractals [list names]"
git push
```

### Update progress in summary (optional):
Edit `docs\FRACTAL_AUDIT_SUMMARY.md` and update the "Tier 1 Complete" counter.

---

## Remember

✅ **Simple is better** - Don't overthink it  
✅ **5 at a time** - Small, focused sessions  
✅ **Save often** - Commit after each session  
✅ **Take breaks** - This isn't a race!  

**You're doing great! Let's audit some fractals! 🚀**

---

**Need help?** Review:
- [Audit Summary](FRACTAL_AUDIT_SUMMARY.md) - Overall strategy
- [Quick Start](QUICK_START_AUDIT.md) - More detailed workflow
- [Tier 1 File](audits/TIER1_CRITICAL_FRACTALS.md) - The actual checklist
