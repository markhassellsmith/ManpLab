# Win2D Testing Guide - Quick Start

**Status:** ✅ Win2D is ACTIVE - Ready to Test!  
**Date:** December 2024

---

## What Changed

You are now using **Win2D GPU-accelerated rendering** instead of the old pixel buffer approach.

**Key improvements:**
- ✅ Smooth anti-aliased lines (no more jagged "stair-stepping")
- ✅ GPU acceleration via DirectX
- ✅ Transform-based coordinate system (matches GDI+ architecture)
- ✅ Mouse interactions disabled (prevents buggy zoom/pan)

---

## Quick Test Procedure

### 1. Launch the Application
```
Press F5 in Visual Studio (or click Start)
```

### 2. Switch to Hailstone Mode
- In the UI, select "Hailstone" from the fractal type dropdown

### 3. Test Default Sequence
**Parameters:**
- Start X: `-10`
- Start Y: `6`
- Max Iterations: `150`

**Click "Render" (or press F5)**

**Expected Results:**
- ✅ Smooth trajectory lines (not jagged)
- ✅ Magenta cycle segments (if cycle detected)
- ✅ Grid lines visible and subtle
- ✅ Point markers at each step
- ✅ Labels showing coordinates

### 4. Test Diagonal Lines (Steep Angle)
**Parameters:**
- Start X: `2`
- Start Y: `15`

**Click "Render"**

**Look for:**
- ✅ Diagonal lines are smooth with anti-aliasing
- ✅ No "broken segments" or pixel gaps
- ✅ Consistent line thickness

### 5. Test Shallow Angles
**Parameters:**
- Start X: `-5`
- Start Y: `1`

**Click "Render"**

**Look for:**
- ✅ Nearly horizontal lines are smooth
- ✅ No stair-stepping artifacts

### 6. Test Mouse Interactions (Should be Disabled)
**Try these - they should all do NOTHING:**
- ❌ Left-click and drag (should not create zoom box)
- ❌ Right-click and drag (should not pan)
- ❌ Mouse wheel scroll (should not zoom)

**Expected:** Mouse does nothing in Hailstone mode

### 7. Test Keyboard Shortcuts (Should Work)
**These should still work:**
- ✅ `A` key - Toggle axes on/off
- ✅ `F5` - Re-render
- ✅ `Space` - Reset view
- ✅ `1`/`2`/`3`/`4` - Change resolution

---

## Visual Comparison Checklist

### Compare to NumericalVisualizations (GDI+)

If you have the old GDI+ version running:

1. Render the same sequence in both apps:
   - Start: (-10, 6)

2. Compare line quality:
   - [ ] Lines are equally smooth
   - [ ] No visible quality degradation
   - [ ] Anti-aliasing is consistent

3. Compare colors:
   - [ ] Cycle segments are magenta
   - [ ] Regular segments use gradient (blue → cyan)
   - [ ] Grid lines are subtle gray

---

## Performance Check

### Render Times
**Watch the status bar for render times:**

Typical times (1200×900 pixels):
- Simple sequences: 50-150ms
- Complex sequences (100+ points): 150-300ms

**If render times are >500ms, that's unexpected - report it!**

---

## Known Issues / What's Normal

### ✅ Expected (Normal):
1. Labels are still on Canvas overlay (not Win2D text)
2. Mouse does nothing in Hailstone mode
3. Render progress bar shows briefly

### ❌ Unexpected (Report These):
1. Jagged or pixelated lines
2. Application crashes
3. Missing grid lines or axes
4. Extremely slow rendering (>1 second)
5. Distorted or incorrect trajectories

---

## Reporting Issues

If you find problems, note:
1. **What happened** (screenshot if possible)
2. **Parameters used** (start X/Y, iterations)
3. **Expected vs actual** result
4. **Render time** (from status bar)

---

## Success Criteria

This feature is **ready to merge** if:

1. ✅ Lines are smooth with no artifacts
2. ✅ Visual quality matches GDI+ reference
3. ✅ No crashes or errors
4. ✅ Performance is acceptable (<500ms typical)
5. ✅ Mouse interactions are properly disabled
6. ✅ Keyboard shortcuts work

---

## After Testing

### If All Tests Pass:
1. Commit the changes
2. Merge to `development` branch
3. (Optional) Clean up dead code in separate commit

### If Issues Found:
1. Document the issue
2. Check Debug output in Visual Studio
3. Revert to legacy renderer if critical:
   ```csharp
   // MainViewModel.cs line 33
   private readonly HailstoneRenderService _hailstoneRenderService = new();
   ```

---

## Debug Output

**If issues occur, check the Output window in Visual Studio:**

Look for:
```
=== Win2D Hailstone Render ===
Sequence points: [count]
Toggles: Axes=[true/false], Points=[true/false], Labels=[true/false]
Using [CUSTOM/FIXED/AUTO-SCALE] viewport: X=[min, max], Y=[min, max]
Transform: scaleX=[value], scaleY=[value], offsetX=[value], offsetY=[value]
```

---

## Quick Rollback (If Needed)

If Win2D has problems:

**File:** `ManpWinUI\ViewModels\MainViewModel.cs`  
**Line 33:**

```csharp
// Change this:
private readonly HailstoneRenderServiceWin2D _hailstoneRenderService = new();

// Back to this:
private readonly HailstoneRenderService _hailstoneRenderService = new();
```

Rebuild and you're back to the legacy renderer.

---

## Questions?

- **Are lines supposed to be this smooth?** → YES! That's the Win2D anti-aliasing working
- **Why doesn't the mouse work?** → DISABLED by design (was buggy)
- **Can I zoom/pan?** → Not with mouse; keyboard arrows work for fractals (not Hailstone)
- **Labels look the same?** → Yes, still using Canvas overlay (works fine)

---

**Ready to test? Launch the app and try it out!** 🚀
