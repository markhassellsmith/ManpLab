# Fractal Rendering Debug Checklist

## Current Status
**Fix Applied:** Event syntax correction + Thread marshaling + Parameter validation
**Build:** ✅ Successful
**Ready for Testing:** ✅ Yes

---

## Test Execution Monitor

### Test 1: Initial Launch
- [ ] App starts without crash
- [ ] Browser loads and shows categories
- [ ] Default fractal (Mandelbrot) is selected

**If crashes here:** 🚩 RED FLAG - Architecture problem, not event issue

---

### Test 2: Select Registry Fractal (e.g., Tetrate, Lambda)
- [ ] Selection changes in browser
- [ ] `SelectedFractalType` updates in debug output
- [ ] No exception before render starts

**If crashes here:** 🚩 RED FLAG - Selection/binding still broken

---

### Test 3: Render Start
Watch debug output for this sequence:
```
FRACTAL RENDER SERVICE - STARTING RENDER
Creating FractalParameters object...
Native Calculate: Parameters validated - 800x600, 256 iterations
Native Calculate: Creating result for 800x600
Native Calculate: Pixel array allocated successfully
Native Calculate: Setting up parameters...
```

- [ ] All these messages appear
- [ ] No exception during parameter setup

**If crashes here with "parameter is incorrect":** 🚩 RED FLAG - Validation didn't catch the issue

---

### Test 4: Render Progress
Watch for:
```
Native Calculate: Line 0 of 600
Native Calculate: Line 10 of 600
Native Calculate: Line 20 of 600
...
```

- [ ] Progress lines appear
- [ ] No "Failed to enqueue progress update" warnings
- [ ] Progress bar updates in UI (if visible)

**If crashes during progress:** Check if:
- Exception is in WinRT marshaling → Thread safety issue (need Option A)
- Exception is in native code → Memory/calculation bug
- Random crashes at different lines → 🚩 RED FLAG - Memory corruption

---

### Test 5: Render Complete
Watch for:
```
Native Calculate: Line 590 of 600
Complete
Calculate() completed successfully
Mandelbrot render complete: 800x600 in XXXms
```

- [ ] Final progress message appears
- [ ] Image displays in UI
- [ ] No exception after completion

**Success Criteria:** See a colored fractal image!

---

## Rabbit Hole Warning Signs

### 🟢 GREEN (Good Progress)
1. Each fix moves the crash **later** in the execution
2. Error messages become **more specific**
3. More debug output appears before crash
4. Stack traces point to **different locations**

### 🟡 YELLOW (Watch Carefully)
1. Same error in same place after 1 fix
2. New exception but related to same subsystem
3. Intermittent crashes (sometimes works, sometimes fails)

### 🔴 RED (Stop and Reassess)
1. **Same error in same place after 2+ fixes**
2. Crashes move randomly (sometimes line 100, sometimes line 300)
3. Access violations / heap corruption messages
4. Native debugger shows corrupted pointers
5. Crash happens in Windows SDK code we don't control

---

## Next Steps Based on Results

### If Test 3 Crashes (Parameter Validation)
→ The issue is earlier in the pipeline (parameter creation in C#)
→ Check `FractalRenderService.cs` parameter setup

### If Test 4 Crashes (Progress Events)
→ Thread marshaling needs to move to C++/CLI (Option A)
→ Or disable progress events entirely as workaround

### If Test 5 Crashes (Completion)
→ Final progress event or pixel data handoff issue
→ Check `WriteableBitmap` creation in ViewModel

### If Random/Intermittent Crashes
→ 🚩 **RABBIT HOLE ALERT**
→ Likely native memory issue or race condition
→ Need to step back and review architecture

### If Specific Exception in Windows SDK
→ 🚩 **RABBIT HOLE ALERT**
→ We're triggering an OS/framework bug
→ Need workaround, not more fixes

---

## Emergency Exit Strategy

If after this test we see RED flags, we should:

1. **Disable progress events entirely** (comment out handler in service)
2. **Test with just Mandelbrot** (skip registry fractals temporarily)
3. **Add native memory diagnostics** (check for leaks/corruption)
4. **Consider alternate architecture:**
   - Pure managed port of calculation
   - Or COM-style interface instead of C++/CLI
   - Or separate process for native code

---

## Success Definition

**Minimum:** One fractal (any type) renders completely with a visible image
**Good:** Registry fractals (Tetrate, Lambda) render successfully
**Excellent:** Multiple fractals render, progress updates work smoothly

---

## Debugging Commands

If crash occurs, capture:
```
!analyze -v          (in native debugger)
!dumpstack           (for full managed/native stack)
!gcroot <address>    (if suspect GC issue)
```

In VS Output window, look for:
- Last debug message before crash
- Any "Failed to enqueue" warnings
- Stack trace with line numbers

---

**Remember:** Quality over speed. If we're not making clear progress after this test, we stop and reassess rather than trying more random fixes.
