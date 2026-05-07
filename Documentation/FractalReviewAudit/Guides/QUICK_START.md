# Quick Start: Fractal Audit

## 🎯 Goal
Audit the quality of fractals in ManpLab - but in a **manageable way**.

## ⚡ Quick Start (5 minutes)

### 1. Validate Your Workspace
```powershell
.\Scripts\Test-FractalBasics.ps1
```
This checks that everything is set up correctly.

### 2. Read the Strategy
Open and skim: **`docs\FRACTAL_AUDIT_SUMMARY.md`**

Key points:
- 300 total fractals (don't panic!)
- Focus on **Tier 1** first (30 fractals)
- Audit 5-10 at a time
- Sessions should be 30-60 minutes

### 3. Start Auditing
Open: **`docs\audits\TIER1_CRITICAL_FRACTALS.md`**

Pick any fractal and:
1. Launch it in ManpLab
2. Check if it looks correct
3. Note the render time
4. Mark the checkboxes
5. Add any notes

### 4. Save Your Progress
After auditing 5-10 fractals:
```bash
git add docs\audits\TIER1_CRITICAL_FRACTALS.md
git commit -m "Audited [fractal names]"
```

---

## 📋 What to Check for Each Fractal

### Visual Check (30 seconds)
- ✅ Does it render?
- ✅ Does it look mathematically correct?
- ✅ Is there interesting detail?

### Performance Check (5 seconds)
- ✅ Render time < 3 seconds? (at 1920×1080, 256 iterations)

### Formula Check (2 minutes)
- ✅ Open source file (listed in audit)
- ✅ Skim the formula code
- ✅ Does it match the mathematical definition?

### Interactive Check (1 minute)
- ✅ Zoom in - is there detail?
- ✅ Pan around - any crashes?
- ✅ Julia mode (if applicable) - works?

**Total time per fractal: ~4 minutes**

**5 fractals = ~20 minutes** ← Perfect session length!

---

## 🎓 Audit Workflow

```
┌─────────────────────────────────────┐
│  Start: Pick a Tier or Category    │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Audit 5-10 fractals                │
│  (30-60 minutes)                    │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Mark checkboxes & add notes        │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Commit your progress               │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Update summary progress counters   │
│  (optional, every few sessions)     │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Take a break! 😊                   │
└──────────────┬──────────────────────┘
               │
               ▼
          [Repeat]
```

---

## 📊 Progress Tracking

**Current Status:**
- Tier 1: 0 / 30 (0%)
- Tier 2: 0 / 100 (0%)
- Tier 3: 0 / 170 (0%)
- **Total: 0 / 300 (0%)**

Update these in `docs\FRACTAL_AUDIT_SUMMARY.md` every few sessions.

---

## 🔧 Tools Available

### Validation Script
```powershell
.\Scripts\Test-FractalBasics.ps1
```
- Checks workspace integrity
- Validates files exist
- Detects duplicate IDs
- Verifies project structure

### Category Splitter (Optional)
```powershell
.\Scripts\Split-AuditFile.ps1
```
- Splits massive file by category
- Creates manageable 10-40 fractal files
- Use if you prefer category-based work

---

## 💡 Tips

### For Efficiency
- ⏱️ **Time-box sessions:** 30-60 minutes max
- 🎯 **Focus on quantity:** 5-10 fractals per session
- 📝 **Quick notes:** Don't write essays
- 💾 **Commit often:** After each session
- 🔄 **Use automation:** Run Test-FractalBasics.ps1 regularly

### For Quality
- 🔬 **Formula first:** Math correctness is critical
- 👀 **Visual second:** Does it look right?
- ⚡ **Performance third:** Note if slow
- 📚 **Reference materials:** Wikipedia, Wolfram MathWorld

### For Sanity
- 🚫 **Don't try to audit all 300 at once**
- ✅ **Start with Tier 1** (30 fractals)
- 🎯 **Focus on one category** if that helps
- 🧘 **Take breaks** between sessions

---

## 📚 Documentation

- **[Audit Summary](FRACTAL_AUDIT_SUMMARY.md)** - Overview and strategy
- **[Tier 1 Fractals](audits/TIER1_CRITICAL_FRACTALS.md)** - 30 critical fractals
- **[Restructure Summary](RESTRUCTURE_SUMMARY.md)** - What changed and why
- **[Scripts README](../Scripts/README.md)** - Script documentation

---

## ❓ FAQ

**Q: Do I have to audit all 300 fractals?**  
A: No! Start with Tier 1 (30 fractals). That's the most important subset.

**Q: How long will this take?**  
A: Tier 1 = ~2 hours (4 min × 30 fractals). Full audit = ~20 hours total.

**Q: Can I skip fractals?**  
A: Yes! Focus on Tier 1 and Tier 2. Tier 3 is optional.

**Q: What if I find bugs?**  
A: Document in the "Issues/Notes" section. Create GitHub issues for serious problems.

**Q: Can multiple people work on this?**  
A: Yes! Split by category or tier. Just coordinate to avoid conflicts.

---

## 🚀 Let's Go!

**Your first session:**

1. Open: `docs\audits\TIER1_CRITICAL_FRACTALS.md`
2. Pick 5 fractals (e.g., Mandelbrot through Lambda)
3. Audit each one (~4 minutes each)
4. Mark checkboxes and add notes
5. Commit your progress
6. Done! Come back tomorrow for 5 more! 😊

---

**Remember:** Progress > Perfection. Better to audit 5 fractals well than to get overwhelmed by 300! 🎯
