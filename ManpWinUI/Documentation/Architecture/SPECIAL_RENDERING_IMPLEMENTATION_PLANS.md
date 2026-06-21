# Implementation Plans for Special Rendering Fractals

**Document Status:** Planning Phase  
**Last Updated:** 2025-01-XX  
**Branch:** development  

---

## Executive Summary

This document contains comprehensive implementation plans for five fractal types that require specialized rendering techniques beyond the standard per-pixel escape-time calculator model. Each plan includes technical architecture, step-by-step implementation instructions, testing strategies, and risk assessments.

### Priority Order
1. **[P0] Buddhabrot** — 2-4 hours (LOW-HANGING FRUIT)
2. **[P1] Bifurcation Diagrams** — 4-6 hours
3. **[P2] Cellular Automata** — 8-12 hours
4. **[P3] L-Systems** — 12-20 hours
5. **[P4] Flame Fractals** — 20-40+ hours (FUTURE)

---

## Architecture Context

The ManpLab codebase has an established pattern for special rendering:

### Layer Architecture
- **Native C++ layer** (`ManpCore.Native`): Fractal registration, category detection, algorithm implementation
- **C++/CLI wrapper** (`FractalEngineWrapper`): Bridges native and managed code, routes by `FractalCategory`
- **C# services** (`ManpWinUI\Services`): High-level rendering orchestration, UI integration

### Existing Patterns
- **`HistogramBased`** — Strange attractors, IFS (orbit accumulation)
- **`Hailstone2D`** — Trajectory rendering with custom service
- **`EscapeTime2D`** — Standard Mandelbrot/Julia per-pixel calculator

### Key Files
- `ManpCore.Native\FractalRegistry.h` — Category enum definitions
- `ManpCore.Native\FractalEngineWrapper.cpp` — Rendering router (lines 614-700)
- `ManpWinUI\Services\FractalRenderService.cs` — High-level orchestration
- `ManpWinUI\Services\HailstoneRenderService.cs` — Custom renderer example

---

## Plan 1: Buddhabrot (Path Accumulation Histogram)

### Priority: **P0 - HIGHEST** ⭐
**Estimated Time:** 2-4 hours  
**Complexity:** Low (algorithm exists, pattern proven)  
**Visual Impact:** VERY HIGH (iconic nebula imagery)

### Technical Brief

Buddhabrot requires Monte Carlo sampling of millions of starting points, tracking escape paths, and accumulating visit counts into a histogram. The original `ManpWIN64\BuddhaBrot.cpp` contains the complete working algorithm.

**Current Problem:** Placeholder calculator in `SpecialExoticFamily.cpp` (lines 150-207) produces standard Mandelbrot-style imagery instead of true Buddhabrot with glowing filaments.

**Key Differences from Histogram Attractors:**
| Aspect | Strange Attractors | Buddhabrot |
|--------|-------------------|------------|
| Sampling | One continuous orbit | Millions of random points |
| Path tracking | All iterations | Only escaping paths |
| Histogram | Single channel | Three channels (RGB) |
| Visual result | Phase space plot | Nebula-like glow |

**Reference Code:**
- `ManpWIN64\BuddhaBrot.cpp` lines 120-272 (complete algorithm)
- `FractalEngineWrapper::RenderHistogramFractal()` lines 250-400 (histogram pattern)

**Architecture Decision:** Create dedicated `RenderBuddhabrotFractal()` function rather than forcing into histogram pattern because:
1. Different sampling model (random points vs continuous orbit)
2. Three-channel RGB accumulation vs single histogram
3. Escape-only filtering vs all-iteration tracking

### Implementation Steps

#### Step 1: Update Native Registration
**File:** `ManpCore.Native\SpecialExoticFamily.cpp` line 156

```cpp
// BEFORE
spec.type = FractalCategory::Special;

// AFTER
spec.type = FractalCategory::BuddhabrotBased;
// NOTE: Requires path accumulation rendering, not per-pixel calculator
```

Keep placeholder calculator for registry compliance.

---

#### Step 2: Add Category Enum
**File:** `ManpCore.Native\FractalRegistry.h` line 133

```cpp
enum class FractalCategory
{
    EscapeTime2D,
    Sequence2D,
    AttractorBased3D,
    HistogramBased,
    BuddhabrotBased,  // ← ADD THIS: Monte Carlo path accumulation (Buddhabrot, Anti-Buddhabrot)
    Special
};
```

**Mirror in C#:** `ManpWinUI\Services\FractalRenderResult.cs` enum

---

#### Step 3: Implement Native Renderer
**File:** `ManpCore.Native\FractalEngineWrapper.cpp` after line 450

Add new function:

```cpp
/// <summary>
/// Render Buddhabrot using Monte Carlo path accumulation.
/// Samples millions of starting points, tracks escape orbits, accumulates RGB histogram.
/// </summary>
static void RenderBuddhabrotFractal(
    FractalResult^ result,
    const ::Native::MandelbrotParams& params,
    int width,
    int height,
    ::Native::PaletteType palette,
    int colorOffset)
{
    Debug::WriteLine("RenderBuddhabrotFractal: Starting Monte Carlo path accumulation");

    // Extract parameters
    double brightness = 1.0;
    int threshold = 1000;
    // TODO: Extract from params.customParams map

    // Allocate three histograms (RGB channels)
    std::vector<long> redCount(width * height, 0);
    std::vector<long> greenCount(width * height, 0);
    std::vector<long> blueCount(width * height, 0);

    // Sampling grid (10x denser than output resolution)
    const int SOURCE_COLUMNS = width * 10;
    const int SOURCE_ROWS = height * 10;

    double x_jump = params.viewWidth / SOURCE_COLUMNS;
    double y_jump = params.viewHeight / SOURCE_ROWS;

    // Main sampling loop
    double y = params.centerY + params.viewHeight / 2.0;
    for (int source_row = SOURCE_ROWS - 1; source_row >= 0; source_row--, y -= y_jump)
    {
        double x = params.centerX - params.viewWidth / 2.0;
        for (int source_column = 0; source_column < SOURCE_COLUMNS; source_column++, x += x_jump)
        {
            // Optimization: Skip main Mandelbrot body (reduces wasted iterations)
            if ((x > -1.2 && x <= -1.1 && y > -0.1 && y < 0.1) ||
                (x > -1.1 && x <= -0.9 && y > -0.2 && y < 0.2) ||
                // ... (full optimization mask from BuddhaBrot.cpp lines 180-191)
                ) continue;

            // Test if point escapes
            double zr = 0.0, zi = 0.0;
            bool escapes = false;
            for (int n = 0; n <= threshold; n++)
            {
                double zr2 = zr * zr;
                double zi2 = zi * zi;
                if (zr2 + zi2 > 4.0) {
                    escapes = true;
                    break;
                }
                double temp = zr2 - zi2 + x;
                zi = 2.0 * zr * zi + y;
                zr = temp;
            }

            // If escapes, track path and accumulate histogram
            if (escapes)
            {
                DrawPath(x, y, params, width, height, threshold, 
                         redCount, greenCount, blueCount);
            }
        }
    }

    // Convert histogram to pixel colors
    ConvertHistogramToPixels(result, width, height, 
                            redCount, greenCount, blueCount, 
                            brightness, palette, colorOffset);
}

/// <summary>
/// Track Mandelbrot orbit path and accumulate histogram.
/// </summary>
static void DrawPath(
    double cx, double cy,
    const ::Native::MandelbrotParams& params,
    int width, int height, int threshold,
    std::vector<long>& redCount,
    std::vector<long>& greenCount,
    std::vector<long>& blueCount)
{
    double zr = 0.0, zi = 0.0;

    for (int n = 0; n <= threshold; n++)
    {
        double zr2 = zr * zr;
        double zi2 = zi * zi;

        if (zr2 + zi2 > 4.0) return;

        // Map orbit point to pixel coordinates
        int px = (int)((zr - (params.centerX - params.viewWidth / 2.0)) / params.viewWidth * width);
        int py = (int)((zi - (params.centerY - params.viewHeight / 2.0)) / params.viewHeight * height);

        if (px >= 0 && px < width && py >= 0 && py < height)
        {
            int index = py * width + px;

            // RGB channel thresholds (iteration depth ranges)
            if (n <= 100) {
                blueCount[index]++;   // Fast escapers (bright core)
            } else if (n <= 1000) {
                greenCount[index]++;  // Medium escapers (tendrils)
            } else {
                redCount[index]++;    // Slow escapers (outer glow)
            }
        }

        double temp = zr2 - zi2 + cx;
        zi = 2.0 * zr * zi + cy;
        zr = temp;
    }
}

/// <summary>
/// Convert accumulated histogram to RGB pixels with log compression.
/// </summary>
static void ConvertHistogramToPixels(
    FractalResult^ result,
    int width, int height,
    const std::vector<long>& redCount,
    const std::vector<long>& greenCount,
    const std::vector<long>& blueCount,
    double brightness,
    ::Native::PaletteType palette,
    int colorOffset)
{
    const double RED_MULTIPLIER = 0.09 * brightness;
    const double GREEN_MULTIPLIER = 0.11 * brightness;
    const double BLUE_MULTIPLIER = 0.18 * brightness;

    for (int i = 0; i < width * height; i++)
    {
        // Log compression to handle wide dynamic range
        double r = std::log(1.0 + redCount[i] * RED_MULTIPLIER);
        double g = std::log(1.0 + greenCount[i] * GREEN_MULTIPLIER);
        double b = std::log(1.0 + blueCount[i] * BLUE_MULTIPLIER);

        // Clamp to [0, 255]
        byte red = (byte)std::min(255.0, r * 50.0);
        byte green = (byte)std::min(255.0, g * 50.0);
        byte blue = (byte)std::min(255.0, b * 50.0);

        // Write BGRA format
        int pixelIndex = i * 4;
        result->PixelData[pixelIndex + 0] = blue;
        result->PixelData[pixelIndex + 1] = green;
        result->PixelData[pixelIndex + 2] = red;
        result->PixelData[pixelIndex + 3] = 255;
    }
}
```

---

#### Step 4: Add Routing Logic
**File:** `FractalEngineWrapper.cpp` line 634 (after histogram check)

```cpp
// Check if this fractal requires Buddhabrot rendering
if (spec->type == ::Native::FractalCategory::BuddhabrotBased)
{
    Debug::WriteLine("Native Calculate: Buddhabrot-based fractal detected");
    Debug::WriteLine("  Calling RenderBuddhabrotFractal for path accumulation");

    ::Native::PaletteType nativePalette = static_cast<::Native::PaletteType>((int)parameters->Palette);

    RenderBuddhabrotFractal(result, nativeParams, width, height, nativePalette, parameters->ColorOffset);

    stopwatch->Stop();
    result->RenderTime = stopwatch->Elapsed;
    result->IterationCount = 0;  // Not applicable
    result->Category = FractalCategory::BuddhabrotBased;

    Debug::WriteLine(String::Format("Native Calculate: Buddhabrot rendering complete in {0}ms", 
                                    stopwatch->ElapsedMilliseconds));
    return result;
}
```

---

#### Step 5: Add Parameter Definitions
**File:** `SpecialExoticFamily.cpp` line 203 (before `FractalRegistry::Register(spec)`)

```cpp
// BEFORE
spec.parameters = {};

// AFTER
spec.parameters = {
    ParameterSpec("brightness", "Brightness Multiplier", 0.1, 5.0, 1.0, 
                  "Controls overall image brightness and saturation"),
    ParameterSpec("threshold", "Path Iterations", 100, 50000, 1000, 
                  "Maximum iterations to track per escape path (higher = more red channel)")
};
```

---

#### Step 6: Update C# Category Mirror
**File:** `ManpWinUI\Services\FractalRenderResult.cs`

```csharp
public enum FractalCategory
{
    EscapeTime2D,
    Sequence2D,
    AttractorBased3D,
    HistogramBased,
    BuddhabrotBased,  // ← ADD THIS
    Special
}
```

---

#### Step 7: Test with Known Coordinates
**Manual Verification:**

Navigate to Buddhabrot at default coordinates:
- **Center:** `(-0.33, 0.03)`
- **Zoom:** `1.066667`
- **Iterations:** `1000`

**Expected Visual Output:**
- ✅ **Dark void** in center (cardioid + circular bulb silhouette)
- ✅ **Bright cyan-white core** around boundary (blue channel dominant)
- ✅ **Yellow-green tendrils** spiraling outward (green channel)
- ✅ **Red outer glow** fading to black (red channel)
- ✅ **Asymmetric, organic structure** (NOT symmetric like Mandelbrot)

**Performance Target:** 5-15 seconds at 1280×720 with 5M samples

---

#### Step 8: Document Implementation
**File:** New file `ManpWinUI\Documentation\Architecture\buddhabrot-rendering.md`

Contents:
- Algorithm overview (Monte Carlo sampling → escape filtering → path accumulation)
- RGB channel explanation (blue=fast, green=medium, red=slow escapers)
- Parameter guide (brightness, threshold)
- Performance characteristics (O(samples × threshold))
- Visual reference description (nebula appearance)
- Comparison to standard Mandelbrot rendering

---

### Testing Strategy

#### Unit Tests
- ✅ Verify histogram allocation (no memory leaks)
- ✅ Verify escape detection accuracy
- ✅ Verify RGB channel separation (blue ≠ green ≠ red)
- ✅ Verify path tracking doesn't revisit pixels incorrectly

#### Visual Regression
- Capture reference image at `(-0.33, 0.03)`, zoom `1.066667`
- Compare new renders using SSIM (target: >0.95 similarity)

#### Performance Benchmarks
| Resolution | Samples | Target Time |
|------------|---------|-------------|
| 1280×720 | 5M | <15s |
| 1920×1080 | 5M | <25s |
| 3840×2160 | 10M | <90s |

---

### Risk Assessment

**Risk Level:** ✅ **LOW**

**Mitigations:**
- Algorithm is proven (exists in ManpWIN64)
- Histogram pattern already established in codebase
- Incremental porting reduces integration risk

**Potential Issues:**
1. **Memory usage** — 3× histograms = 3× RAM (1920×1080 ≈ 24MB)
   - *Mitigation:* Use `std::vector` for automatic cleanup
2. **Performance** — 5M samples × 1000 iterations = 5B operations
   - *Mitigation:* Optimization mask skips Mandelbrot body (saves ~30%)
3. **Color tuning** — Multipliers may need adjustment for different palettes
   - *Mitigation:* Expose as parameters, document defaults

---

## Plan 2: True Bifurcation Diagrams (Parameter Space Plots)

### Priority: **P1 - HIGH**
**Estimated Time:** 4-6 hours  
**Complexity:** Medium (new rendering model)  
**Educational Value:** VERY HIGH

### Technical Brief

Bifurcation diagrams visualize how dynamical systems transition from stable to chaotic behavior as parameters change. They plot **parameter values** (horizontal axis) vs **attractor points** (vertical axis). Each vertical column represents multiple final orbit values for a specific parameter.

**Key Challenge:** Standard per-pixel calculators return **one value** per pixel. Bifurcation diagrams require **multiple values per column** (one column = one parameter, many y-values = attractor points).

**Reference Documentation:** `ManpWinUI\Documentation\BIFURCATION_DIAGRAM_IMPLEMENTATION_PLAN.md` explains why previous `Sequence2D` implementations were removed.

**Classic Examples:**
- **Logistic Map:** `x_{n+1} = r·x_n·(1-x_n)`, shows period-doubling cascade
- **May Map:** Population dynamics with exponential term
- **Complex Lambda:** `z_{n+1} = λ·z·(1-z)`, 2D parameter space

### Visual Appearance

**Logistic Bifurcation (r ∈ [2.5, 4.0], x ∈ [0, 1]):**
```
1.0 |           ████████████████████
    |         ██  ██  ██  ██  ██  ██
x   |       ██      ██      ██      
    |     ██          ██          ██
    |   ██              ████████████
0.0 |___________________________________
    2.5    3.0    3.5    4.0
              Parameter r
```

**Characteristics:**
- Smooth single curve → period 2 fork → period 4 → period 8 → chaotic band
- Periodic windows within chaos (e.g., period 3 at r≈3.83)

### Implementation Steps

*(Steps 1-9 would follow similar detailed format as Buddhabrot plan)*

**Key Components:**
1. Add `FractalCategory::BifurcationDiagram` enum
2. Create `BifurcationDiagramsFamily.cpp` with logistic/May/lambda registrations
3. Implement `RenderBifurcationDiagram()` function
4. Add `bifurcationIterator` function pointer to `FractalSpec`
5. Route in `FractalEngineWrapper`
6. Test with known bifurcation points

**Estimated substeps:** 9 steps (detailed in full plan)

---

## Plan 3: Cellular Automata (State Grid Evolution)

### Priority: **P2 - MEDIUM**
**Estimated Time:** 8-12 hours  
**Complexity:** Medium-High (requires interactive UI)  
**Interactive Value:** VERY HIGH

### Technical Brief

Cellular automata (CA) evolve discrete state grids based on local neighbor rules. They require:
- **State storage:** 2D array of cell states
- **Rule engine:** Evaluate neighbor counts, update cells
- **Time evolution:** Generational updates (play/pause/step)
- **Interaction:** Click to toggle cells, edit patterns

**Cannot use per-pixel calculator** because CA are inherently stateful and time-based.

**Reference:** `ManpWIN64\ant.cpp` (Langton's Ant implementation)

**Classic Examples:**
- **Conway's Game of Life:** B3/S23 (born with 3 neighbors, survive with 2-3)
- **Langton's Ant:** Autonomous agent with turn rules
- **Brian's Brain:** 3-state system (alive → dying → dead)

### UI Requirements

**Control Panel:**
```
┌────────────────────────────────────┐
│ [▶Play] [❚❚Pause] [⏭Step] [↻Reset] │
│ Speed: [====|=====] 100ms           │
│ Generation: 1,234                   │
│ Rule: [Conway's Life ▼]             │
│ Pattern: [Glider ▼]                 │
└────────────────────────────────────┘
```

**Grid Canvas:**
- Click/drag to toggle cells
- Display generation counter overlay
- Show grid lines (optional)

### Implementation Steps

*(Steps 1-12 would follow detailed format)*

**Key Components:**
1. Add `FractalCategory::CellularAutomaton` enum
2. Create `CellularAutomataService.cs` with state grid
3. Implement Conway's Life rules
4. Implement Langton's Ant rules
5. Create `CellularAutomataPanel.xaml` UI
6. Integrate animation loop
7. Add preset patterns (Glider, LWSS, Acorn)
8. Register CA fractals in native layer

**NOTE:** User will provide additional CA-specific requirements when this phase is reached.

---

## Plan 4: L-Systems (Turtle Graphics)

### Priority: **P3 - LOW**
**Estimated Time:** 12-20 hours  
**Complexity:** High (grammar parsing + vector rendering)  
**Botanical Value:** HIGH

### Technical Brief

L-systems generate fractal curves through:
1. **String rewriting:** Apply grammar rules (e.g., `F → F+F--F+F`)
2. **Turtle interpretation:** Parse string as graphics commands
3. **Vector rendering:** Draw resulting paths

**Reference:** `ManpWIN64\Lsys.cpp` lines 282-726

**Classic Examples:**
- **Koch Snowflake:** `F+F+F` axiom, `F→F+F--F+F`, angle 60°
- **Dragon Curve:** `F` axiom, `F→F+G`, `G→F-G`, angle 90°
- **Fractal Plant:** Branching with stack push/pop

### Turtle Commands

| Symbol | Meaning |
|--------|---------|
| `F`, `G` | Move forward, draw line |
| `+` | Turn left by angle |
| `-` | Turn right by angle |
| `[` | Push state (position, heading) to stack |
| `]` | Pop state from stack |

### Implementation Steps

*(Steps 1-14 would follow detailed format)*

**Key Components:**
1. Add `FractalCategory::LSystem` enum
2. Create `LSystemService.cs`
3. Implement string expansion engine
4. Implement turtle state machine
5. Implement Bresenham line rasterizer
6. Create `LSystemPanel.xaml` UI
7. Register presets (Koch, Dragon, Sierpinski, Plant)
8. Add generation limits (prevent string explosion)

---

## Plan 5: Flame Fractals (IFS with Variations)

### Priority: **P4 - FUTURE**
**Estimated Time:** 20-40+ hours  
**Complexity:** VERY HIGH (advanced mathematics)  
**Artistic Value:** EXTREME HIGH

### Technical Brief

Flame fractals extend IFS with non-linear variation functions and produce stunning artistic imagery. Popular in fractal art community (Electric Sheep screensaver, Apophysis software).

**Requires:**
- 20-50 variation functions (spherical, swirl, horseshoe, polar, julia, etc.)
- Affine transform chains with weighted selection
- Log-density coloring with gamma correction
- Color palette interpolation
- Multi-sample anti-aliasing

**Deferred to future sprint** due to complexity and GPU acceleration considerations.

---

## Implementation Priority Order

### Current Sprint
✅ **Plan 1: Buddhabrot** (2-4 hours) — IMMEDIATE START

### Next Sprint
📋 **Plan 2: Bifurcation Diagrams** (4-6 hours)  
📋 **Plan 3: Cellular Automata** (8-12 hours, awaiting user specifics)

### Future Sprints
📅 **Plan 4: L-Systems** (12-20 hours)  
📅 **Plan 5: Flame Fractals** (20-40 hours)

---

## Cross-Plan Architecture Decisions

### New Category Enum Order
```cpp
enum class FractalCategory
{
    EscapeTime2D,           // Standard per-pixel calculators
    Sequence2D,             // 1D/2D sequences (Hailstone, Lyapunov)
    AttractorBased3D,       // Legacy (deprecated in favor of HistogramBased)
    HistogramBased,         // Orbit accumulation (attractors, IFS)
    BuddhabrotBased,        // Monte Carlo path accumulation (NEW)
    BifurcationDiagram,     // Parameter-vs-attractor plots (NEW)
    CellularAutomaton,      // Grid-based state evolution (NEW)
    LSystem,                // Grammar-based turtle graphics (NEW)
    FlameFractal,           // IFS with variations (NEW, FUTURE)
    Special                 // Catch-all for custom renderers
};
```

### Rendering Router Pattern
```cpp
// In FractalEngineWrapper::Calculate()
if (spec->type == FractalCategory::HistogramBased)
    RenderHistogramFractal(...);
else if (spec->type == FractalCategory::BuddhabrotBased)
    RenderBuddhabrotFractal(...);
else if (spec->type == FractalCategory::BifurcationDiagram)
    RenderBifurcationDiagram(...);
else if (spec->type == FractalCategory::CellularAutomaton)
    return EmptyResultWithMetadata();  // C# layer handles
else if (spec->type == FractalCategory::LSystem)
    return EmptyResultWithMetadata();  // C# layer handles
else
    StandardPerPixelRender(...);  // Default escape-time
```

### Service Layer Pattern
- **Native rendering:** Buddhabrot, Bifurcation (performance-critical)
- **C# rendering:** Cellular Automata, L-Systems (UI integration, state management)

---

## Testing Strategy (All Plans)

### Visual Regression Suite
Create reference images for:
- Buddhabrot at `(-0.33, 0.03)`, zoom `1.066667`
- Logistic bifurcation at `r ∈ [2.5, 4.0]`
- Conway's Life Glider after 100 generations
- Koch Snowflake generation 4
- (Flame fractals TBD)

### Performance Benchmarks
| Fractal Type | Resolution | Target Time |
|--------------|------------|-------------|
| Buddhabrot | 1920×1080 | <25s |
| Bifurcation | 1920×1080 | <1s |
| CA (30fps) | 256×256 | <33ms/frame |
| L-System | 1920×1080 | <5s (gen 15) |

### Unit Test Coverage
- Histogram accumulation accuracy
- Bifurcation iterator correctness
- CA rule compliance (Life, Ant)
- L-system string expansion correctness
- Memory leak detection (all histogram allocations)

---

## Documentation Deliverables

Each implemented plan produces:
1. ✅ Architecture document explaining rendering model
2. ✅ Parameter/control reference
3. ✅ Performance characteristics
4. ✅ Visual examples and expected output
5. ✅ Code comments in implementation

**File Locations:**
- `ManpWinUI\Documentation\Architecture\buddhabrot-rendering.md`
- `ManpWinUI\Documentation\Architecture\bifurcation-diagram-rendering.md`
- `ManpWinUI\Documentation\Architecture\cellular-automata-rendering.md`
- `ManpWinUI\Documentation\Architecture\lsystem-rendering.md`
- `ManpWinUI\Documentation\Architecture\flame-fractal-rendering.md` (future)

---

## Risk Summary

| Plan | Risk Level | Primary Mitigation |
|------|-----------|-------------------|
| Buddhabrot | ✅ LOW | Algorithm exists, pattern proven |
| Bifurcation | ⚠️ MEDIUM | Start with 1D logistic map |
| Cellular Automata | ⚠️ MEDIUM | Prototype Conway first |
| L-Systems | ⚠️ HIGH | Add generation limits early |
| Flame Fractals | 🔴 VERY HIGH | Defer to future, consider GPU |

---

## Open Questions

### For Cellular Automata (Plan 3)
*User will provide specifics when CA phase is reached:*
- Which rule systems to prioritize? (Life, Ant, Brian's Brain, others?)
- Grid size preferences? (Fixed 256×256, or adjustable?)
- Pattern library scope? (Basic presets, or extensive catalog?)
- Export capabilities? (RLE format, GIF animation?)

### For L-Systems (Plan 4)
- Should grammar be user-editable, or preset-only?
- Color mapping for plant structures (leaf vs stem)?
- 3D L-systems in scope (requires depth dimension)?

### For Flame Fractals (Plan 5)
- GPU acceleration mandatory, or acceptable CPU fallback?
- Integration with existing flame parameter databases?
- Real-time preview during transform editing?

---

## Next Steps

1. ✅ **Review this plan document**
2. ✅ **Begin Plan 1: Buddhabrot implementation**
3. Track progress using step completion markers
4. Commit and test after each major milestone
5. Update this document with actual timings and lessons learned

---

**Document End**
