# Testing Guide: 14 Registered Fractals

## Overview
This guide explains how to test the 14 fractals you just registered in the FractalRegistry system.

## What Was Changed
- **Backend (C++)**: Added 10 new fractals across 3 new families + 3 Julia presets
- **UI (XAML)**: Updated `MainPage.xaml` ComboBox to expose all 14 fractals

## The 14 Fractals to Test

### 1. Classic Fractals (4 fractals)
| Fractal | Registry Name | What to Expect |
|---------|--------------|----------------|
| **Mandelbrot** | `Mandelbrot` | Classic M-set, seahorse valley, spirals |
| **Burning Ship** | `BurningShip` | Ship-like structure, vertical symmetry |
| **Tricorn** | `Tricorn` | Heart-shaped with cusp, conjugate symmetry |
| **Phoenix** | `Phoenix` | Complex 3-parameter fractal |

### 2. Multibrot Family (3 fractals)
Higher powers of z → z^n + c

| Fractal | Registry Name | Power | What to Expect |
|---------|--------------|-------|----------------|
| **Multibrot³** | `Multibrot3` | z³+c | 3-fold rotational symmetry |
| **Multibrot⁴** | `Multibrot4` | z⁴+c | 4-fold symmetry, cross pattern |
| **Multibrot⁵** | `Multibrot5` | z⁵+c | 5-fold symmetry, starfish shape |

**Test Parameters:**
- Center: (0, 0)
- Zoom: 1.0x
- Iterations: 256-512
- Look for: Symmetry axes matching the power (3/4/5 axes)

### 3. Newton Method Fractals (2 fractals)
Root-finding algorithms that color by convergence basin

| Fractal | Registry Name | Formula | What to Expect |
|---------|--------------|---------|----------------|
| **Newton z³-1** | `Newton` | Roots of z³-1=0 | 3 colored basins (120° apart) |
| **Nova** | `Nova` | Newton + Mandelbrot hybrid | Fractal convergence boundaries |

**Test Parameters:**
- Center: (0, 0)
- Zoom: 1.0x - 4.0x
- Iterations: 50-100 (lower than M-set)
- Look for: Distinct colored regions for different roots

### 4. Magnet Fractals (2 fractals)
Rational functions from theoretical physics

| Fractal | Registry Name | Type | What to Expect |
|---------|--------------|------|----------------|
| **Magnet I** | `Magnet1` | Degree 4 rational | Smooth connected regions |
| **Magnet II** | `Magnet2` | Degree 6 rational | More complex boundaries |

**Test Parameters:**
- Center: (0, 0)
- Zoom: 1.0x - 2.0x
- Iterations: 256-512
- Look for: Smooth gradient transitions, fewer sharp edges than Mandelbrot

### 5. Julia Presets (3 fractals)
Pre-configured Julia sets with famous constants

| Fractal | Registry Name | Constant (c) | What to Expect |
|---------|--------------|--------------|----------------|
| **San Marco** | `JuliaSanMarco` | -0.75 + 0i | Dragon-like fractal (famous shape) |
| **Douady Rabbit** | `JuliaDouadyRabbit` | -0.123 + 0.745i | Rabbit silhouette |
| **Siegel Disk** | `JuliaSiegelDisk` | -0.391 - 0.587i | Circular disk structure |

**Test Parameters:**
- Center: (0, 0)
- Zoom: 1.0x initially, then explore
- Iterations: 512
- **Note:** These are Julia sets, so the "Julia Mode" toggle should be irrelevant (they force Julia mode internally)

---

## How to Test

### Step 1: Build and Run
```bash
# Already done - build was successful!
# Just press F5 in Visual Studio to launch ManpWinUI
```

### Step 2: Select a Fractal
1. Look at the **right side panel** under "Parameters"
2. Find the **"Fractal Type"** dropdown (ComboBox)
3. Select one of the 14 fractals

### Step 3: Render It
1. Click **"Render"** button in toolbar (or Ctrl+R)
2. Watch the progress bar
3. See your fractal appear!

### Step 4: Explore
- **Zoom In**: Click "+" button or press `+` key
- **Zoom Out**: Click "-" button or press `-` key
- **Pan**: Use arrow keys or right-click and drag
- **Reset**: Click "Reset View" or press `Space`

### Step 5: Try Different Palettes
Change the **"Color Palette"** dropdown to see different color schemes:
- Grayscale (for structure analysis)
- Classic (traditional Mandelbrot colors)
- Fire (red-orange-yellow gradient)
- Ocean (blue-green gradient)
- Rainbow (full spectrum)
- Psychedelic (high contrast)

---

## Test Checklist

### Quick Smoke Test (5 minutes)
- [ ] Select **Mandelbrot** → Render → See classic M-set
- [ ] Select **Multibrot3** → Render → See 3-fold symmetry
- [ ] Select **Newton** → Render → See 3 colored basins
- [ ] Select **JuliaSanMarco** → Render → See dragon fractal

### Full Test (15 minutes)
Go through all 14 fractals and verify:
- [ ] Mandelbrot (classic seahorse valley)
- [ ] BurningShip (ship structure)
- [ ] Tricorn (heart shape)
- [ ] Phoenix (complex pattern)
- [ ] Multibrot3 (3-fold symmetry)
- [ ] Multibrot4 (4-fold symmetry)
- [ ] Multibrot5 (5-fold symmetry)
- [ ] Newton (3 convergence basins)
- [ ] Nova (fractal boundaries)
- [ ] Magnet1 (smooth gradients)
- [ ] Magnet2 (complex boundaries)
- [ ] JuliaSanMarco (dragon)
- [ ] JuliaDouadyRabbit (rabbit shape)
- [ ] JuliaSiegelDisk (disk structure)

### Deep Test (30+ minutes)
For each fractal:
1. Render at default zoom
2. Zoom in 4x and re-render
3. Zoom in 16x and re-render
4. Try different color palettes
5. Check that zoom/pan controls work correctly
6. Save a bookmark for interesting locations

---

## What You're Testing

### Backend Validation
- ✅ **FractalRegistry** correctly initialized all 14 types
- ✅ **FractalEngineWrapper** dynamic dispatch works
- ✅ Each **calculator function** produces valid pixel data
- ✅ **Smooth coloring** applied correctly

### UI Validation
- ✅ **ComboBox** shows all 14 fractal names
- ✅ **SelectedFractalType** binding passes name to backend
- ✅ **FractalRenderService** sends correct parameters
- ✅ **Image display** renders results correctly

### Integration Validation
- ✅ C++ → C++/CLI → C# → XAML pipeline complete
- ✅ No crashes or exceptions
- ✅ Progress reporting works
- ✅ Memory management correct (no leaks)

---

## Known Limitations (Current UI)

### What Works Now
✅ Select fractal type from dropdown
✅ Render any of 14 fractals
✅ Zoom, pan, reset view
✅ Color palette selection
✅ Save images (PNG/JPEG)
✅ Bookmarks

### What's Missing (Phase 2 Goal)
❌ Fractal browser with thumbnails (planned for Phase 2 Week 5)
❌ Fractal categories/grouping (Classic, Multibrot, Newton, etc.)
❌ Fractal descriptions/help text
❌ Per-fractal default parameters
❌ Search/filter fractals by name

---

## Troubleshooting

### Issue: Fractal appears blank/black
**Cause:** Iteration count too low or bad parameters
**Fix:** Increase Max Iterations to 512 or click "Reset View"

### Issue: Fractal renders very slowly
**Cause:** High zoom levels need more iterations
**Fix:** Enable "Auto Scale Iterations" checkbox

### Issue: Wrong fractal type rendered
**Cause:** Registry name mismatch
**Fix:** Check that XAML `<x:String>` matches C++ registration name exactly

### Issue: Build fails after XAML change
**Cause:** XAML syntax error
**Fix:** Check that all `<x:String>` tags are properly closed

---

## Next Steps After Testing

### If All 14 Fractals Work ✅
Congratulations! Your FractalRegistry system is fully functional. Options:

**Option A: Add More Fractals**
- Continue expanding to 20-30 fractals before Phase 2
- Add more families (Lyapunov, IFS, L-systems, etc.)
- Document fractal formulas and expected visuals

**Option B: Start Phase 2 UI Redesign**
- Implement 3-panel layout (Browser | Canvas | Properties)
- Build fractal browser with thumbnails and search
- Add per-fractal metadata and descriptions
- Follow PROJECT_PLAN.md Week 4-8 schedule

**Option C: Performance Testing**
- Profile render times for different fractal types
- Test deep zoom performance (1e6+ zoom levels)
- Benchmark BatchRenderer with animation generation

### If Issues Found 🐛
1. Note which fractal(s) fail and how
2. Check Output Window logs for exceptions
3. Verify registry names match exactly (case-sensitive!)
4. Test calculator functions in isolation if needed

---

## Success Criteria

✅ All 14 fractals appear in dropdown
✅ All 14 fractals render without crashes
✅ Visual output matches expected structure (symmetry, basins, etc.)
✅ Zoom and pan work for all fractal types
✅ Color palettes apply correctly
✅ Performance is acceptable (< 5 seconds for 800×600 at default zoom)

---

## References
- **C++ Implementation**: See `ManpCore.Native/*Family.cpp` files
- **Registration Code**: `ManpCore.Native/FractalRegistry.cpp` → `InitializeBuiltins()`
- **Calculator API**: `ManpCore.Native/MandelbrotCalculator.h` → `FractalCalculator` function pointer
- **UI Binding**: `ManpWinUI/Views/MainPage.xaml` lines 383-422
- **Render Service**: `ManpWinUI/Services/FractalRenderService.cs` → `RenderMandelbrotAsync()`

---

*Last Updated: After fractal-expansion merge*
*Total Fractals: 14 (Mandelbrot, BurningShip, Tricorn, Phoenix, Multibrot3/4/5, Newton, Nova, Magnet1/2, JuliaSanMarco, JuliaDouadyRabbit, JuliaSiegelDisk)*
