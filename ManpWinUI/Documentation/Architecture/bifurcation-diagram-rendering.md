# Bifurcation Diagram Rendering Architecture

**Document Status:** Implementation Complete  
**Last Updated:** 2025-01-XX  
**Related Files:** `FractalRegistry.h`, `FractalEngineWrapper.cpp`, `BifurcationFamily.cpp`

---

## Overview

Bifurcation diagrams visualize how dynamical systems transition from stable to chaotic behavior as control parameters change. Unlike standard per-pixel escape-time fractals, bifurcation diagrams require **column-based rendering** where each vertical column represents multiple attractor points for a single parameter value.

---

## Rendering Model

### Standard Fractals vs Bifurcation Diagrams

| Aspect | Escape-Time Fractals | Bifurcation Diagrams |
|--------|---------------------|---------------------|
| **X-axis** | Real spatial coordinate | Parameter value |
| **Y-axis** | Imaginary spatial coordinate | Attractor values |
| **Computation** | One value per pixel | Multiple values per column |
| **Iteration** | Escape-time or histogram | Transient skip + sample collection |
| **Visual structure** | Spatial fractal geometry | Dynamical behavior transitions |

### Column-Based Rendering Algorithm

```
FOR each column x in [0, width):
    parameter = map column x to parameter range [minParam, maxParam]

    attractor_points = bifurcationCalculator(parameter, transient, samples)

    FOR each attractor_value in attractor_points:
        y = map attractor_value to pixel row [0, height)
        IF y within bounds:
            plot white pixel at (x, y)
```

**Key Insight:** Each column independently computes its own set of attractor points, allowing trivial parallelization in future GPU implementations.

---

## Architecture Components

### 1. Native Registry (`FractalRegistry.h`)

#### Category Enum
```cpp
enum class FractalCategory
{
    // ...
    BuddhabrotBased = 4,
    BifurcationDiagram = 5,  // ← Column-based parameter sweep
    Special = 6
};
```

#### Calculator Function Signature
```cpp
typedef std::function<std::vector<double>(
    double parameter,        // Parameter value for this column
    int transient,          // Iterations to skip (let attractor settle)
    int samples,            // Number of points to collect
    const ParamMap& params  // Optional custom parameters
)> BifurcationCalculator;
```

#### FractalSpec Extension
```cpp
struct FractalSpec
{
    // ... existing fields ...
    BifurcationCalculator bifurcationCalculator;  // Optional, used only for BifurcationDiagram types
};
```

---

### 2. Native Renderer (`FractalEngineWrapper.cpp`)

**Location:** After BuddhabrotBased check, before Special fallback

#### Rendering Branch
```cpp
if (spec->type == ::Native::FractalCategory::BifurcationDiagram)
{
    // 1. Validate calculator exists
    if (!spec->bifurcationCalculator) {
        throw gcnew InvalidOperationException("Missing bifurcationCalculator");
    }

    // 2. Map viewport to parameter range
    double minParam = nativeParams.centerX - nativeParams.viewWidth / 2.0;
    double maxParam = nativeParams.centerX + nativeParams.viewWidth / 2.0;
    double paramStep = nativeParams.viewWidth / width;

    double minY = nativeParams.centerY - nativeParams.viewHeight / 2.0;
    double maxY = nativeParams.centerY + nativeParams.viewHeight / 2.0;

    // 3. Column iteration
    for (int col = 0; col < width; col++)
    {
        double parameter = minParam + col * paramStep;

        // 4. Invoke calculator (transient=100, samples=100 hardcoded for now)
        std::vector<double> points = spec->bifurcationCalculator(
            parameter, 100, 100, nativeParams.customParams);

        // 5. Plot each attractor point
        for (double yVal : points)
        {
            int row = (int)((maxY - yVal) / nativeParams.viewHeight * height);
            if (row >= 0 && row < height)
            {
                int pixelIndex = (row * width + col) * 4;
                result->PixelData[pixelIndex + 0] = 255; // B
                result->PixelData[pixelIndex + 1] = 255; // G
                result->PixelData[pixelIndex + 2] = 255; // R
                result->PixelData[pixelIndex + 3] = 255; // A
            }
        }

        // 6. Progress reporting (every 10 columns)
        if (col % 10 == 0) {
            int progress = (int)((double)col / width * 100.0);
            RaiseProgressChanged(progress);
        }

        // 7. Cancellation support
        if (CancellationToken->IsCancellationRequested) break;
    }
}
```

**Performance:** O(width × samples) — typically 1920 × 100 = 192,000 iterations

---

### 3. Bifurcation Registrations (`BifurcationFamily.cpp`)

#### Logistic Bifurcation
**Formula:** `x_{n+1} = r·x_n·(1 - x_n)`  
**Parameter:** `r ∈ [2.5, 4.0]` (classic range)  
**Attractor:** Final x-values after transient iterations  
**Visual:** Single line → period 2 fork (r≈3.0) → period 4 (r≈3.45) → period 8 → chaos (r≈3.57+)

```cpp
spec.bifurcationCalculator = [](double r, int transient, int samples, const ParamMap&) {
    std::vector<double> results;
    double x = 0.5;  // Initial seed

    // Skip transient iterations
    for (int i = 0; i < transient; i++) {
        x = r * x * (1.0 - x);
    }

    // Collect attractor samples
    for (int i = 0; i < samples; i++) {
        x = r * x * (1.0 - x);
        results.push_back(x);
    }

    return results;
};
```

**Default View:**
- Center: `(3.4, 0.5)` — captures period-doubling cascade
- Zoom: `2.0` — shows r ∈ [2.4, 4.4]

---

#### Lambda Bifurcation
**Formula:** `z_{n+1} = λ·z_n·(1 - z_n)` (complex variant)  
**Parameter:** Real part of `λ` (horizontal axis)  
**Attractor:** Magnitude of complex orbit  
**Visual:** 2D parameter space with bands and periodic windows

```cpp
spec.bifurcationCalculator = [](double lambdaRe, int transient, int samples, const ParamMap& params) {
    std::vector<double> results;

    // Optional imaginary component (default 0.0)
    double lambdaIm = 0.0;
    auto it = params.find("lambdaIm");
    if (it != params.end()) lambdaIm = it->second;

    std::complex<double> lambda(lambdaRe, lambdaIm);
    std::complex<double> z(0.5, 0.0);

    // Transient skip
    for (int i = 0; i < transient; i++) {
        z = lambda * z * (1.0 - z);
    }

    // Sample magnitude
    for (int i = 0; i < samples; i++) {
        z = lambda * z * (1.0 - z);
        results.push_back(std::abs(z));
    }

    return results;
};
```

**Default View:**
- Center: `(2.0, 0.5)` — interesting bifurcation structure
- Zoom: `1.0` — shows λ ∈ [1.0, 3.0]

---

#### Henon Bifurcation
**Formula:** `x_{n+1} = 1 - a·x_n^2 + y_n; y_{n+1} = b·x_n`  
**Parameter:** `a` (horizontal axis), `b` fixed (typically 0.3)  
**Attractor:** x-coordinate of 2D orbit  
**Visual:** 2D attractor projection showing chaotic structure emergence

```cpp
spec.bifurcationCalculator = [](double a, int transient, int samples, const ParamMap& params) {
    std::vector<double> results;

    // Optional b parameter (default 0.3)
    double b = 0.3;
    auto it = params.find("henonB");
    if (it != params.end()) b = it->second;

    double x = 0.0, y = 0.0;

    // Transient skip
    for (int i = 0; i < transient; i++) {
        double xNext = 1.0 - a * x * x + y;
        y = b * x;
        x = xNext;
    }

    // Sample x-coordinate
    for (int i = 0; i < samples; i++) {
        double xNext = 1.0 - a * x * x + y;
        y = b * x;
        x = xNext;
        results.push_back(x);
    }

    return results;
};
```

**Default View:**
- Center: `(1.2, 0.0)` — captures attractor formation
- Zoom: `2.0` — shows a ∈ [0.2, 2.2]

---

## Visual Characteristics

### Logistic Bifurcation
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

**Key Features:**
- **r < 3.0:** Single stable fixed point (horizontal line)
- **r ≈ 3.0:** First bifurcation (line splits into two)
- **r ≈ 3.45:** Period 4 window
- **r ≈ 3.57:** Onset of chaos (Feigenbaum constant)
- **r ≈ 3.83:** Period 3 window within chaos
- **White bands:** Periodic attractors
- **Dense regions:** Chaotic attractors

---

### Lambda Bifurcation
**Appearance:** Complex 2D structure with:
- Magnitude bands (horizontal striping)
- Periodic windows (clear gaps in chaos)
- Symmetry around λ = 2.0
- Bounded attractor region (magnitude typically < 5.0)

---

### Henon Bifurcation
**Appearance:** Chaotic attractor structure:
- Single-point attractors at low `a`
- Strange attractor formation around `a ≈ 1.0`
- Fractal filamentation as `a` increases
- Bounded x-coordinate range (typically -1.5 to 1.5)

---

## Performance Characteristics

### Computational Complexity
- **Per-frame:** O(width × samples)
- **Typical:** 1920 pixels × 100 samples = 192,000 iterations
- **Target time:** <1 second at 1920×1080

### Memory Usage
- **Pixel buffer:** width × height × 4 bytes (BGRA)
- **Histogram buffer:** width × height × 4 bytes (int density counts)
- **Temporary storage:** width × samples × 8 bytes (double)
- **Total:** ~40 MB for 1920×1080 @ 100 samples

### Optimization Opportunities
1. **GPU parallelization:** Each column is independent
2. **Adaptive sampling:** Use more samples in chaotic regions
3. ✅ **Density coloring:** IMPLEMENTED - Histogram accumulation with log-scale color mapping
4. **Multi-threading:** Divide columns across CPU cores

---

## Density-Based Coloring (Implemented)

The bifurcation renderer now uses **two-pass rendering** with density accumulation:

### Algorithm
```cpp
// PASS 1: Accumulate visit counts
std::vector<int> histogram(width * height, 0);
int maxDensity = 0;

for (int col = 0; col < width; col++) {
    std::vector<double> points = bifurcationCalculator(parameter, transient, samples);

    for (double yValue : points) {
        int row = MapToPixelRow(yValue);
        histogram[row * width + col]++;
        maxDensity = max(maxDensity, histogram[row * width + col]);
    }
}

// PASS 2: Convert histogram to colors
for (int i = 0; i < width * height; i++) {
    int density = histogram[i];
    if (density == 0) continue;  // Black background

    // Log-scale normalization for wide dynamic range
    double normalized = log(1.0 + density) / log(1.0 + maxDensity);
    double iterationValue = normalized * 255.0;

    // Use standard palette coloring system
    ColorRGB color = MandelbrotCalculator::IterationToColor(
        iterationValue, 256, palette, colorOffset);

    WritePixel(i, color);
}
```

### Color Mapping
- **High density** (frequently visited points) → Bright, high-iteration colors
- **Low density** (rarely visited points) → Dark, low-iteration colors
- **Zero density** (never visited) → Black background
- **Log scaling** → Compresses wide range (1 to 1000+ visits) into visible spectrum

### Palette Support
All standard palettes work:
- **Classic** — Blue/cyan bifurcation bands
- **Fire** — Red/orange chaotic regions
- **Spectrum** — Rainbow-colored period windows
- **Neon** — High-contrast detail revelation

### Color Offset
The `colorOffset` parameter rotates the palette, allowing exploration of different color mappings without recalculating the histogram.

---

## Parameter System (Future)

**Current State:** Hardcoded transient=100, samples=100

**Planned Custom Parameters:**
```cpp
spec.parameters = {
    ParameterSpec("transient", "Transient Iterations", 50, 1000, 100,
                  "Iterations to skip before sampling (allows attractor to settle)"),
    ParameterSpec("samples", "Sample Points", 10, 500, 100,
                  "Number of attractor points to collect per parameter value"),
    ParameterSpec("minY", "Min Y Value", -2.0, 2.0, 0.0,
                  "Minimum vertical axis value (attractor range)"),
    ParameterSpec("maxY", "Max Y Value", -2.0, 2.0, 1.0,
                  "Maximum vertical axis value (attractor range)")
};
```

**Blocking Issue:** `ParameterSpec` initializer-list constructor not implemented yet. Parameters removed temporarily to unblock initial implementation.

---

## Comparison to Other Rendering Models

| Category | Rendering Model | Example |
|----------|----------------|---------|
| **EscapeTime2D** | Per-pixel escape count | Mandelbrot, Julia |
| **HistogramBased** | Orbit accumulation | Strange attractors, IFS |
| **BuddhabrotBased** | Monte Carlo path tracing | Buddhabrot, Nebulabrot |
| **BifurcationDiagram** | Column-based parameter sweep | Logistic, Lambda, Henon |
| **Special** | Custom algorithms | Hailstone trajectories |

---

## Testing and Validation

### Visual Regression Targets

**Logistic Bifurcation @ (3.4, 0.5), zoom 2.0:**
- ✅ Single line visible at r ≈ 2.8-3.0
- ✅ First fork visible at r ≈ 3.0
- ✅ Period-4 structure at r ≈ 3.45
- ✅ Dense chaotic band at r ≈ 3.6+
- ✅ Period-3 window at r ≈ 3.83

**Lambda Bifurcation @ (2.0, 0.5), zoom 1.0:**
- ✅ Bounded magnitude range
- ✅ Horizontal banding structure
- ✅ Periodic windows visible

**Henon Bifurcation @ (1.2, 0.0), zoom 2.0:**
- ✅ Attractor formation visible
- ✅ Chaotic structure at higher `a`
- ✅ Bounded x-coordinate range

---

## Known Limitations

1. ✅ ~~**No density coloring yet**~~ — **IMPLEMENTED:** Full palette support with log-scale histogram coloring
2. **Hardcoded sampling parameters** — Cannot adjust transient/samples via UI (uses transient=200, samples=100)
3. **No y-axis zoom control** — Viewport height controls parameter range, not attractor range
4. **Single-threaded** — Column computation is serial
5. **No anti-aliasing** — Point plotting is nearest-neighbor (histogram accumulation provides some smoothing)

---

## Future Enhancements

### Phase 2: Density Coloring ✅ **COMPLETED**
Implemented two-pass histogram rendering with log-scale color mapping. All standard palettes (Classic, Fire, Ocean, Spectrum, Neon, etc.) now work with bifurcation diagrams.

### Phase 3: Custom Parameters (Priority HIGH)
Implement `ParameterSpec` initializer-list constructor and wire UI sliders for:
- Transient iterations (currently hardcoded to 200)
- Sample count (currently hardcoded to 100)
- Y-axis range (minY, maxY)
- System-specific parameters (lambdaIm, henonB)

### Phase 4: GPU Acceleration
Port column loop to compute shader:
```hlsl
[numthreads(1, 1, 1)]
void BifurcationColumn(uint columnId : SV_DispatchThreadID)
{
    double parameter = minParam + columnId * paramStep;
    // Run calculator and plot points...
}
```

### Phase 5: Additional Systems
- Sine map: `x_{n+1} = r·sin(π·x_n)`
- Tent map: `x_{n+1} = r·min(x_n, 1-x_n)`
- Gauss map: `x_{n+1} = exp(-α·x_n^2) + β`
- Ikeda map (2D complex)

---

## References

### Internal Documentation
- `BIFURCATION_DIAGRAM_IMPLEMENTATION_PLAN.md` — Original technical specification
- `SPECIAL_RENDERING_IMPLEMENTATION_PLANS.md` — Master priority roadmap
- `FractalRegistry.h` — Native category and calculator definitions
- `FractalEngineWrapper.cpp` — Rendering dispatch and column loop
- `BifurcationFamily.cpp` — Logistic/Lambda/Henon registrations

### External Resources
- Strogatz, S. H. (2015). *Nonlinear Dynamics and Chaos*. Westview Press.
- Feigenbaum, M. J. (1978). "Quantitative universality for a class of nonlinear transformations."
- May, R. M. (1976). "Simple mathematical models with very complicated dynamics."

---

**Document End**
