# 🧪 Phase 3 Manual Testing Guide

**Goal:** Verify end-to-end fractal rendering works correctly

---

## 🚀 Launch Application

1. **Start Debugging**
   - Press **F5** in Visual Studio
   - Or: Menu → Debug → Start Debugging

2. **Expected: Application Window Opens**
   ```
   ┌─────────────────────────────────────────────┐
   │ ManpWinUI (Window Title)                    │
   ├─────────────────────────────────────────────┤
   │ [▶ Render] [↻ Reset] [🔍+] [🔍-]          │  ← CommandBar
   ├─────────────────────────────────────────────┤
   │                                             │
   │         ManpLab - Fractal Explorer          │  ← Welcome
   │       Phase 3: WinUI MVVM Architecture      │     overlay
   │   Click 'Render' to generate your first...  │     (visible)
   │                                             │
   │         Rendering Parameters                │
   │         Center X: [-0.5____]                │  ← Parameters
   │         Center Y: [0.0_____]                │     panel
   │         Zoom Level: [1.0___]                │
   │         Max Iterations: [256___]            │
   │         Color Palette: [Classic ▼]          │
   │                                             │
   ├─────────────────────────────────────────────┤
   │ Ready                  Last render: ...     │  ← Status bar
   └─────────────────────────────────────────────┘
   ```

---

## ✅ Test 1: First Render

**Action:** Click **"Render"** button

**Expected Behavior:**

1. **Welcome overlay disappears immediately**

2. **Progress overlay appears:**
   ```
   ┌─────────────────────────────────────────┐
   │                                         │
   │              ⟳ (spinning)               │  ← ProgressRing
   │         Rendering fractal...            │
   │         ▓▓▓▓▓▓▓▓▓▓░░░░░░░░░           │  ← ProgressBar
   │                                         │     (fills 0-100%)
   └─────────────────────────────────────────┘
   ```

3. **Progress bar animates** (0% → 100%)
   - Should take ~100-500ms depending on CPU

4. **Progress overlay disappears**

5. **Mandelbrot fractal appears:**
   ```
   ┌─────────────────────────────────────────┐
   │                                         │
   │                                         │
   │        ╭──────╮  ╭╮                    │  ← Iconic
   │       ╭┴──────┴──┴┴╮                   │     Mandelbrot
   │      ╭┴────────────┴╮                  │     shape with
   │      ╰┬────────────┬╯                  │     colors!
   │       ╰──────╯  ╰╯                     │
   │                                         │
   │                                         │
   └─────────────────────────────────────────┘
   ```

6. **Status bar updates:**
   - Shows: "Rendered in XXX ms"

**Visual Verification:**
- [ ] Fractal has circular main body (left side)
- [ ] Smaller circular bulb (right side)
- [ ] Intricate colored boundary details
- [ ] Classic palette colors (blue, cyan, gold gradient)
- [ ] Black interior (converged regions)
- [ ] Image fills canvas area proportionally

---

## ✅ Test 2: Zoom In

**Action:** Click **"Zoom In"** button (or manually set Zoom to 2.0)

**Expected:**
- Zoom parameter updates to 2.0
- Status: "Zoom: 2.00x"
- Fractal is NOT automatically re-rendered (intentional)

**Action:** Click **"Render"** again

**Expected:**
- Fractal re-renders at 2x zoom
- Image shows closer detail of center region
- Main body appears larger
- More detail visible in boundary

**Visual Check:**
- [ ] Fractal looks more "zoomed in"
- [ ] Less of the set visible (cropped view)
- [ ] Details are sharper/larger

---

## ✅ Test 3: Zoom Out

**Action:** Click **"Zoom Out"** button twice (Zoom = 0.5)

**Action:** Click **"Render"**

**Expected:**
- Fractal shows wider view
- Entire Mandelbrot set visible
- Lots of black space around set
- Set appears smaller/more distant

**Visual Check:**
- [ ] Full Mandelbrot set visible
- [ ] Surrounded by black (divergent region)
- [ ] Set centered in view

---

## ✅ Test 4: Pan View

**Action:** Change parameters:
- Center X: -0.7
- Center Y: 0.0
- Zoom: 2.0

**Action:** Click **"Render"**

**Expected:**
- View shifts to left side of Mandelbrot
- Shows left edge detail
- Different region than default center

**Visual Check:**
- [ ] Different portion of set visible
- [ ] Still recognizable Mandelbrot features
- [ ] Boundary details present

---

## ✅ Test 5: Interesting Location

**Action:** Enter "elephant valley" coordinates:
- Center X: 0.34
- Center Y: 0.05
- Zoom: 200
- Max Iterations: 512

**Action:** Click **"Render"**

**Expected:**
- Highly zoomed detail region
- Complex intricate patterns
- More render time (higher iterations)
- Smaller features visible

**Visual Check:**
- [ ] Extremely detailed close-up
- [ ] Complex boundary structures
- [ ] Multiple colors in gradient
- [ ] Recognizable fractal self-similarity

---

## ✅ Test 6: Color Palettes

**Action:** For each palette, select and render:

1. **Grayscale**
   - [ ] Black to white only
   - [ ] No colors
   - [ ] Smooth gradient

2. **Classic** (default)
   - [ ] Blue → Cyan → Gold
   - [ ] Standard Mandelbrot look

3. **Fire**
   - [ ] Red → Orange → Yellow
   - [ ] Hot color theme

4. **Ocean**
   - [ ] Blue → Cyan → Green
   - [ ] Cool color theme

5. **Rainbow**
   - [ ] Full spectrum
   - [ ] HSV color wheel

6. **Psychedelic**
   - [ ] High contrast
   - [ ] Bold vivid colors

**For Each:**
- Click palette in ComboBox
- Click "Render"
- Verify colors change
- Fractal shape stays same

---

## ✅ Test 7: Iteration Depth

**Action:** Set Max Iterations to different values:

| Iterations | Expected Render Time | Expected Detail     |
|------------|---------------------|---------------------|
| 50         | ~50ms               | Low detail          |
| 256        | ~100ms              | Medium detail       |
| 512        | ~200ms              | High detail         |
| 1000       | ~400ms              | Very high detail    |
| 2000       | ~800ms              | Extreme detail      |

**Visual Check:**
- [ ] Higher iterations = more color bands
- [ ] Smoother gradient transitions
- [ ] More detail in boundary regions
- [ ] Render time increases proportionally

---

## ✅ Test 8: Reset View

**Action:** After zooming/panning, click **"Reset View"**

**Expected:**
- All parameters restore to defaults:
  - Center X: -0.5
  - Center Y: 0.0
  - Zoom: 1.0
  - Max Iterations: 256
- Status: "View reset to default"
- Fractal does NOT auto-render (intentional)

**Action:** Click **"Render"**

**Expected:**
- Back to original full Mandelbrot view

**Visual Check:**
- [ ] Same as first render
- [ ] Full set visible at default location

---

## ✅ Test 9: Rapid Re-renders

**Action:** Click **"Render"** multiple times quickly

**Expected:**
- Each render completes
- No crashes or freezes
- UI remains responsive during render
- Progress overlay shows for each render

**Check:**
- [ ] No exceptions in Output window
- [ ] App doesn't freeze
- [ ] Multiple renders work sequentially

---

## ✅ Test 10: Window Resize

**Action:** Resize application window

**Expected:**
- Fractal image scales proportionally (Viewbox)
- No distortion
- Maintains aspect ratio
- Fills available space

**Visual Check:**
- [ ] Image grows/shrinks with window
- [ ] No stretching or squashing
- [ ] Letterboxing if aspect doesn't match

---

## 🐛 Common Issues & Troubleshooting

### Issue: No fractal appears after clicking Render

**Check:**
1. Output window for exceptions
2. Debug → Windows → Output
3. Look for C++ interop errors

**Possible Causes:**
- ManpCore.Native.dll not found
- C++/CLI wrapper issue
- Platform architecture mismatch

**Solution:**
- Rebuild entire solution
- Check project references
- Verify x64 platform selected

---

### Issue: Fractal is all black

**Possible Causes:**
- Zoom too high (beyond set boundary)
- Parameters are in divergent region
- Max iterations too low

**Solution:**
- Click "Reset View"
- Use default parameters
- Try Classic palette

---

### Issue: Progress bar doesn't update

**Possible Causes:**
- DispatcherQueue not working
- Progress reporting not wired

**Check:**
- Progress property is updating (breakpoint)
- DispatcherQueue.TryEnqueue called
- UI binding correct (Mode=OneWay)

---

### Issue: App crashes on render

**Check:**
1. Output window for exception details
2. Call stack in debugger
3. Exception message

**Common:**
- Null reference in ConvertPixelDataToBitmap
- Invalid pixel data size
- Memory allocation issue in C++

**Solution:**
- Check pixelData is not null
- Verify width * height * 4 == pixelData.Length
- Check C++ engine returns valid data

---

### Issue: Colors look wrong

**Possible Causes:**
- Palette not applied
- BGRA byte order incorrect

**Check:**
- Palette string matches enum
- ParsePalette() returns correct value
- FractalEngineWrapper uses BGRA format

---

## 📸 Screenshot Checklist

For documentation, capture:
- [ ] Welcome screen (before render)
- [ ] Progress overlay (during render)
- [ ] Default Mandelbrot (Classic palette)
- [ ] Zoomed detail view
- [ ] Each color palette
- [ ] High iteration depth example

---

## ✅ Final Checklist - All Tests Passing

- [ ] Application launches successfully
- [ ] Welcome overlay visible on start
- [ ] First render produces visible fractal
- [ ] Mandelbrot shape is correct
- [ ] Colors display properly
- [ ] Progress bar animates smoothly
- [ ] Zoom In/Out buttons work
- [ ] Reset View restores defaults
- [ ] All 6 palettes work
- [ ] Higher iterations increase detail
- [ ] Pan to different coordinates works
- [ ] Window resize scales fractal
- [ ] No crashes or exceptions
- [ ] Status bar updates correctly
- [ ] Render time reported accurately

---

## 🎉 Success Criteria

**If all checkboxes above are ✅, you have:**

✅ Working end-to-end fractal rendering  
✅ C++ SIMD engine integrated  
✅ WinUI MVVM architecture functional  
✅ Progress reporting working  
✅ All color palettes working  
✅ Zoom/pan controls functional  
✅ Professional UI with Material Design  

**Phase 3 Core Functionality: COMPLETE!** 🎊

---

## 📝 Bug Report Template

If you find issues:

```
**Issue:** [Brief description]

**Steps to Reproduce:**
1. [Step 1]
2. [Step 2]
3. [Step 3]

**Expected Behavior:**
[What should happen]

**Actual Behavior:**
[What actually happened]

**Environment:**
- Visual Studio Version: [2026 Preview]
- .NET Version: [10]
- Platform: [x64]
- Build Config: [Debug/Release]

**Output Window:**
```
[Copy exception/error messages]
```

**Screenshots:**
[If applicable]
```

---

**Ready to test? Press F5!** 🚀
