# Bifurcation Diagram Implementation Plan

## Overview
This document describes the complete implementation plan for adding true bifurcation diagram rendering to ManpLab. Traditional bifurcation diagrams (like those in Fractint) plot multiple attractor values per parameter column, creating the characteristic "branching tree" visualization. The current parameter-space visualizations average attractor behavior into a single color per pixel.

## Background

### Current State
- **Parameter Space Visualizations**: Show averaged attractor behavior across 2D parameter space using single-value-per-pixel rendering
- **Examples**: Logistic Parameter Space, Lambda Parameter Space, Henon Parameter Space
- **Limitation**: Cannot show bifurcation structure (period doubling, chaos bands, etc.)

### Traditional Bifurcation Diagrams
- **X-axis**: Parameter value (e.g., r from 2.8 to 4.0 for logistic map)
- **Y-axis**: Attractor values (e.g., population from 0 to 1)
- **Rendering**: Each column plots multiple points showing the attractor after transient behavior
- **Reference**: Fractint's `Bif.cpp` implementation (lines 99-187)

## Architecture Overview

### New Components Required

1. **New FractalCategory Type**: `BifurcationDiagram`
2. **New Calculator Signature**: `BifurcationCalculator` - returns multiple y-values for a parameter
3. **New Native Rendering Loop**: Column-based rendering with multi-point plotting
4. **New FractalSpec Field**: `bifurcationCalculator` member
5. **C# Dispatcher Updates**: Handle new category in `FractalRenderService`
6. **New Fractal Entries**: Logistic/Lambda/Henon Bifurcation (distinct from Parameter Space versions)

---

## Implementation Steps

### Phase 1: Native Infrastructure (C++)

#### Step 1.1: Extend FractalCategory Enum
**File**: `ManpCore.Native\FractalRegistry.h`
**Location**: Around line 128

```cpp
enum class FractalCategory
{
    EscapeTime2D,           // Standard 2D escape-time fractals
    Sequence2D,             // Hailstone, bifurcation, etc.
    AttractorBased3D,       // Lorenz, Rössler, etc.
    HistogramBased,         // Orbit accumulation rendering
    BifurcationDiagram,     // NEW: Traditional bifurcation diagrams
    Special                 // Perturbation, Buddhabrot, etc.
};
```

**Rationale**: Separates bifurcation diagrams from other rendering modes for proper dispatch.

---

#### Step 1.2: Add BifurcationCalculator Signature
**File**: `ManpCore.Native\FractalRegistry.h`
**Location**: After `OrbitIterator` definition (around line 154)

```cpp
// Bifurcation diagram calculator signature
// Given a parameter value, returns vector of attractor points to plot
// Used for traditional bifurcation diagrams (one column = one parameter)
typedef std::function<std::vector<double>(
    double parameter,           // Parameter value (e.g., r for logistic map)
    int transient,              // Number of iterations to skip (settle to attractor)
    int samples,                // Number of points to collect after transient
    const ParamMap& params      // Additional parameters
)> BifurcationCalculator;
```

**Rationale**: This signature matches the Fractint bifurcation model where each parameter produces multiple plottable values.

---

#### Step 1.3: Add BifurcationCalculator Field to FractalSpec
**File**: `ManpCore.Native\FractalRegistry.h`
**Location**: In `FractalSpec` struct (around line 168)

```cpp
struct FractalSpec
{
    std::string name;
    std::string displayName;
    std::string category;
    FractalCategory type;

    FractalCalculator calculator;       // For escape-time fractals
    OrbitIterator orbitIterator;        // For histogram-based fractals
    BifurcationCalculator bifurcationCalculator;  // NEW: For bifurcation diagrams

    // ... rest of fields
};
```

---

#### Step 1.4: Add Bifurcation Rendering Loop
**File**: `ManpCore.Native\FractalEngineWrapper.cpp`
**Location**: In `Calculate()` method, after histogram rendering block (around line 622)

```cpp
// ═════════════════════════════════════════════════════════════════════════
// Bifurcation Diagram Rendering
// ═════════════════════════════════════════════════════════════════════════
if (spec.type == ::Native::FractalCategory::BifurcationDiagram)
{
    Debug::WriteLine("Native Calculate: Using bifurcation diagram rendering");

    if (!spec.bifurcationCalculator)
    {
        throw gcnew InvalidOperationException(
            String::Format("Fractal '{0}' has type BifurcationDiagram but no bifurcationCalculator defined",
                          gcnew String(fractalType.c_str())));
    }

    stopwatch->Start();

    // Get vertical range from custom parameters or use defaults
    double minY = 0.0;
    double maxY = 1.0;
    auto it = customParams.find("minY");
    if (it != customParams.end()) minY = it->second;
    it = customParams.find("maxY");
    if (it != customParams.end()) maxY = it->second;

    // Transient and sample counts
    int transient = 200;
    int samples = 100;
    it = customParams.find("transient");
    if (it != customParams.end()) transient = (int)it->second;
    it = customParams.find("samples");
    if (it != customParams.end()) samples = (int)it->second;

    Debug::WriteLine(String::Format(
        "Bifurcation rendering: transient={0}, samples={1}, yRange=[{2}, {3}]",
        transient, samples, minY, maxY));

    // Calculate parameter range based on viewport
    // X-axis represents parameter value
    double paramMin = nativeParams.centerX - (nativeParams.viewWidth / 2.0);
    double paramMax = nativeParams.centerX + (nativeParams.viewWidth / 2.0);
    double paramStep = nativeParams.viewWidth / width;

    Debug::WriteLine(String::Format(
        "Parameter range: [{0}, {1}], step={2}",
        paramMin, paramMax, paramStep));

    // Initialize image to black
    for (int i = 0; i < width * height; i++)
        imageData[i] = 0xFF000000;  // Black with full alpha

    // Render each column (parameter value)
    for (int col = 0; col < width; col++)
    {
        // Report progress
        if (col % 10 == 0)
        {
            if (m_progressChangedDelegate != nullptr)
            {
                auto progressArgs = gcnew ProgressEventArgs();
                progressArgs->Percentage = (col * 100.0) / width;
                progressArgs->CurrentLine = col;
                progressArgs->TotalLines = width;
                progressArgs->StatusMessage = String::Format(
                    "Calculating parameter {0} of {1}", col, width);
                ProgressChanged(this, progressArgs);
            }
        }

        if (m_cancelled)
            throw gcnew OperationCanceledException("Calculation cancelled by user");

        // Calculate parameter for this column
        double parameter = paramMin + (col * paramStep);

        // Get attractor points for this parameter
        std::vector<double> points;
        try
        {
            points = spec.bifurcationCalculator(parameter, transient, samples, customParams);
        }
        catch (const std::exception& ex)
        {
            Debug::WriteLine(String::Format(
                "Bifurcation calculator failed for parameter {0}: {1}",
                parameter, gcnew String(ex.what())));
            continue;
        }

        // Plot each point in the column
        for (double yValue : points)
        {
            // Skip out-of-range values
            if (yValue < minY || yValue > maxY)
                continue;

            // Map y-value to pixel row (flip y-axis: top = maxY, bottom = minY)
            int row = (int)((maxY - yValue) / (maxY - minY) * height);

            // Clamp to valid range
            if (row < 0) row = 0;
            if (row >= height) row = height - 1;

            // Plot pixel (white for now; could use density-based coloring later)
            int pixelIndex = row * width + col;
            imageData[pixelIndex] = 0xFFFFFFFF;  // White pixel
        }
    }

    stopwatch->Stop();

    result->ImageData = imageBuffer;
    result->Width = width;
    result->Height = height;
    result->RenderTime = stopwatch->Elapsed;
    result->IterationCount = 0;  // Not applicable
    result->EscapedPixelCount = 0;  // Not applicable
    result->Category = FractalCategory::BifurcationDiagram;

    Debug::WriteLine(String::Format(
        "Bifurcation diagram rendering complete in {0}ms",
        stopwatch->ElapsedMilliseconds));

    return result;
}
```

**Key Design Decisions**:
- **X-axis = parameter**: Each column represents a different parameter value
- **Y-axis = attractor value**: Vertical position shows the dynamical system's output
- **White pixels**: Simple visualization (density-based coloring could be added later)
- **Fixed y-range**: Defaults to [0, 1] but can be customized via `customParams`
- **Transient/samples**: Configurable via `customParams` or use sensible defaults

---

### Phase 2: C# Rendering Service Integration

#### Step 2.1: Update FractalCategory Enum (Managed)
**File**: `ManpCore.Native\FractalRegistryWrapper.h`
**Location**: Managed enum definition (around line 25)

```cpp
public enum class FractalCategory
{
    EscapeTime2D = 0,
    Sequence2D = 1,
    AttractorBased3D = 2,
    HistogramBased = 3,
    BifurcationDiagram = 4,  // NEW
    Special = 5
};
```

---

#### Step 2.2: Add Category Dispatcher
**File**: `ManpWinUI\Services\FractalRenderService.cs`
**Location**: In `RenderAsync()` method, decision tree around line 270

```csharp
// Determine rendering strategy based on fractal category
var descriptor = await _descriptorService.GetDescriptorAsync(fractalName);
if (descriptor == null)
    throw new InvalidOperationException($"Fractal '{fractalName}' not found in registry");

// Route to appropriate renderer
switch (descriptor.Category)
{
    case FractalCategory.HistogramBased:
        System.Diagnostics.Debug.WriteLine("Using histogram-based rendering");
        // Existing histogram logic...
        break;

    case FractalCategory.BifurcationDiagram:  // NEW
        System.Diagnostics.Debug.WriteLine("Using bifurcation diagram rendering");
        // Native engine handles bifurcation rendering internally
        // Just pass through parameters with bifurcation-specific settings
        break;

    case FractalCategory.EscapeTime2D:
    case FractalCategory.Sequence2D:
    default:
        System.Diagnostics.Debug.WriteLine("Using standard per-pixel rendering");
        // Existing escape-time/sequence logic...
        break;
}
```

**Note**: The bifurcation rendering is self-contained in the native engine, so C# just needs to recognize the category and let the native code handle it.

---

### Phase 3: Register Bifurcation Diagram Fractals

#### Step 3.1: Add Logistic Bifurcation Diagram
**File**: `ManpCore.Native\BifurcationFamily.cpp`
**Location**: Add new entry after existing Logistic Parameter Space

```cpp
//=========================================================================
// Logistic Bifurcation Diagram (Traditional)
//=========================================================================
spec.name = "LogisticBifurcation";
spec.displayName = "Logistic Bifurcation Diagram";
spec.category = "Bifurcation";
spec.type = FractalCategory::BifurcationDiagram;
spec.description = "Traditional bifurcation diagram for the logistic map: xₙ₊₁ = r·xₙ·(1-xₙ). Shows period doubling and chaotic behavior as parameter r varies. X-axis = parameter r, Y-axis = attractor values.";
spec.formula = "x = r·x·(1-x)";
spec.formulaLatex = R"(x_{n+1} = r \cdot x_n \cdot (1 - x_n))";

spec.bifurcationCalculator = [](double parameter, int transient, int samples, const ParamMap& params) -> std::vector<double> {
    std::vector<double> results;

    double r = parameter;
    double x = 0.5;  // Standard initial condition

    // Skip transient iterations to settle on attractor
    for (int i = 0; i < transient; ++i)
    {
        x = r * x * (1.0 - x);

        // Divergence check
        if (x < 0.0 || x > 1.0 || std::isnan(x))
            return results;  // Return empty vector
    }

    // Collect attractor samples
    for (int i = 0; i < samples; ++i)
    {
        x = r * x * (1.0 - x);

        // Validate and store
        if (x >= 0.0 && x <= 1.0 && !std::isnan(x))
            results.push_back(x);
    }

    return results;
};

spec.supportsJulia = false;
spec.defaultCenterX = 3.4;  // Center of interesting range [2.8, 4.0]
spec.defaultCenterY = 0.5;  // Center of y-range [0, 1]
spec.defaultZoom = 1.0;     // Will show approximately [2.9, 3.9] horizontally
spec.defaultBailout = 100.0;
spec.hasSymmetry = false;

// Bifurcation-specific parameters (stored in FractalSpec for reference)
spec.parameters = {
    ParameterSpec("minY", "Min Y Value", "Minimum attractor value to display",
                  ParameterType::Float, ParameterCategory::View, "0.0", 0.0, 1.0, 0.01),
    ParameterSpec("maxY", "Max Y Value", "Maximum attractor value to display",
                  ParameterType::Float, ParameterCategory::View, "1.0", 0.0, 1.0, 0.01),
    ParameterSpec("transient", "Transient Iterations", "Iterations to skip before sampling",
                  ParameterType::Integer, ParameterCategory::Calculation, "200", 0, 1000, 10),
    ParameterSpec("samples", "Sample Count", "Number of attractor points to plot per parameter",
                  ParameterType::Integer, ParameterCategory::Calculation, "100", 10, 500, 10)
};

FractalRegistry::Register(spec);
```

**Key Features**:
- **Parameter range**: X-axis maps to r values (typically 2.8 to 4.0)
- **Y-axis range**: Fixed [0, 1] for logistic map population
- **Transient**: 200 iterations to reach attractor
- **Samples**: 100 points per parameter column
- **Divergence handling**: Returns empty vector if orbit escapes [0, 1]

---

#### Step 3.2: Add Lambda Bifurcation Diagram (Real-valued)
**File**: `ManpCore.Native\BifurcationFamily.cpp`

```cpp
//=========================================================================
// Lambda Bifurcation Diagram (Real slice)
//=========================================================================
spec.name = "LambdaBifurcation";
spec.displayName = "Lambda Bifurcation Diagram (Real)";
spec.category = "Bifurcation";
spec.type = FractalCategory::BifurcationDiagram;
spec.description = "Bifurcation diagram for the real lambda map: x = λ·x·(1-x). X-axis = λ parameter, Y-axis = attractor values. Shows similar period-doubling to logistic map.";
spec.formula = "x = λ·x·(1-x)";
spec.formulaLatex = R"(x_{n+1} = \lambda \cdot x_n \cdot (1 - x_n))";

spec.bifurcationCalculator = [](double parameter, int transient, int samples, const ParamMap& params) -> std::vector<double> {
    std::vector<double> results;

    double lambda = parameter;
    double x = 0.5;

    // Skip transient
    for (int i = 0; i < transient; ++i)
    {
        x = lambda * x * (1.0 - x);

        if (std::abs(x) > 10.0 || std::isnan(x))
            return results;
    }

    // Collect samples
    for (int i = 0; i < samples; ++i)
    {
        x = lambda * x * (1.0 - x);

        if (std::abs(x) < 10.0 && !std::isnan(x))
            results.push_back(x);
    }

    return results;
};

spec.supportsJulia = false;
spec.defaultCenterX = 2.0;
spec.defaultCenterY = 0.5;
spec.defaultZoom = 1.0;
spec.defaultBailout = 100.0;
spec.hasSymmetry = false;

FractalRegistry::Register(spec);
```

**Note**: Lambda bifurcation uses real-valued slice of the complex lambda map. For complex bifurcation, a different approach (2D parameter space) would be needed.

---

#### Step 3.3: Add Henon Bifurcation Diagram
**File**: `ManpCore.Native\BifurcationFamily.cpp`

```cpp
//=========================================================================
// Henon Bifurcation Diagram
//=========================================================================
spec.name = "HenonBifurcation";
spec.displayName = "Hénon Bifurcation Diagram";
spec.category = "Bifurcation";
spec.type = FractalCategory::BifurcationDiagram;
spec.description = "Bifurcation diagram for the Hénon map: xₙ₊₁ = 1 - a·xₙ² + yₙ; yₙ₊₁ = b·xₙ. X-axis = parameter a (with b fixed), Y-axis = x-coordinate attractor values.";
spec.formula = "xₙ₊₁ = 1 - a·xₙ² + yₙ; yₙ₊₁ = b·xₙ";
spec.formulaLatex = R"(x_{n+1} = 1 - a \cdot x_n^2 + y_n, \; y_{n+1} = b \cdot x_n)";

spec.bifurcationCalculator = [](double parameter, int transient, int samples, const ParamMap& params) -> std::vector<double> {
    std::vector<double> results;

    double a = parameter;
    double b = 0.3;  // Fixed second parameter

    // Check if custom b parameter provided
    auto it = params.find("henonB");
    if (it != params.end())
        b = it->second;

    double x = 0.1;
    double y = 0.1;

    // Skip transient
    for (int i = 0; i < transient; ++i)
    {
        double x_new = 1.0 - a * x * x + y;
        y = b * x;
        x = x_new;

        if (std::abs(x) > 10.0 || std::abs(y) > 10.0)
            return results;
    }

    // Collect samples (plot x-coordinate only)
    for (int i = 0; i < samples; ++i)
    {
        double x_new = 1.0 - a * x * x + y;
        y = b * x;
        x = x_new;

        if (std::abs(x) < 10.0 && !std::isnan(x))
            results.push_back(x);
    }

    return results;
};

spec.supportsJulia = false;
spec.defaultCenterX = 1.2;
spec.defaultCenterY = 0.0;
spec.defaultZoom = 1.0;
spec.defaultBailout = 100.0;
spec.hasSymmetry = false;

spec.parameters = {
    ParameterSpec("henonB", "Henon b parameter", "Fixed second parameter for Henon map",
                  ParameterType::Float, ParameterCategory::Calculation, "0.3", -1.0, 1.0, 0.05)
};

FractalRegistry::Register(spec);
```

**Key Design**: Hénon map has two parameters (a, b). For bifurcation diagram, b is fixed and a varies along the x-axis.

---

### Phase 4: Testing and Validation

#### Test Cases

1. **Logistic Bifurcation @ r ∈ [2.8, 4.0]**
   - Should show period-1 region (r < 3)
   - Period-2 fork around r ≈ 3.0
   - Period-4 fork around r ≈ 3.449
   - Chaotic region with islands around r > 3.57

2. **Lambda Bifurcation**
   - Similar structure to logistic
   - Verify real-valued slice produces expected branching

3. **Henon Bifurcation @ a ∈ [0.8, 1.4], b = 0.3**
   - Should show transition from periodic to chaotic behavior

#### Validation Checklist

- [ ] Category enum added to native and managed code
- [ ] BifurcationCalculator signature defined
- [ ] Rendering loop handles multi-point-per-column plotting
- [ ] Y-axis scaling works correctly (flip and clamp)
- [ ] Transient iterations properly skip initial behavior
- [ ] Sample collection produces expected point distribution
- [ ] Progress reporting works during column rendering
- [ ] Cancellation works mid-render
- [ ] All three bifurcation fractals register successfully
- [ ] UI shows bifurcation diagrams in Bifurcation family
- [ ] Visual output matches expected bifurcation structure

---

## Advanced Features (Future Enhancements)

### 1. Density-Based Coloring
Instead of white pixels, accumulate density and color by frequency:

```cpp
// Replace simple white pixel with density accumulation
std::vector<int> densityMap(width * height, 0);

// In rendering loop:
for (double yValue : points)
{
    int row = /* ... calculate row ... */;
    int pixelIndex = row * width + col;
    densityMap[pixelIndex]++;
}

// After all columns, map density to color
for (int i = 0; i < width * height; i++)
{
    int density = densityMap[i];
    if (density > 0)
    {
        // Map density to color (logarithmic scale)
        double intensity = std::min(1.0, std::log(density + 1) / std::log(maxDensity + 1));
        UINT color = /* map intensity to palette */;
        imageData[i] = color;
    }
}
```

### 2. Lyapunov Exponent Overlay
Overlay Lyapunov exponent as background color to show stability:
- Negative λ (stable) = blue background
- Zero λ (neutral) = gray background  
- Positive λ (chaotic) = red background

### 3. Multiple Initial Conditions
For basins of attraction, iterate with several different initial x values per parameter and color-code by basin.

### 4. Interactive Parameter Selection
Allow click-to-select a parameter value and display the corresponding time series or phase portrait.

---

## Implementation Effort Estimate

| Phase | Task | Estimated Time |
|-------|------|----------------|
| 1.1 | Add FractalCategory enum value | 5 min |
| 1.2 | Define BifurcationCalculator signature | 10 min |
| 1.3 | Add bifurcationCalculator field to FractalSpec | 5 min |
| 1.4 | Implement native bifurcation rendering loop | 1-2 hours |
| 2.1 | Update managed FractalCategory enum | 5 min |
| 2.2 | Add category dispatcher in C# | 15 min |
| 3.1-3.3 | Register three bifurcation fractals | 30 min |
| 4 | Testing and validation | 30 min |
| **Total** | | **2.5-3.5 hours** |

---

## Rollout Strategy

### Phase A: Infrastructure Only (No UI Changes)
1. Add category, signature, and rendering loop
2. Register one test fractal (Logistic Bifurcation)
3. Validate rendering produces expected output
4. **Deliverable**: Working proof-of-concept

### Phase B: Complete Family
1. Add Lambda and Henon bifurcation diagrams
2. Tune default views for best initial appearance
3. Document behavior in fractal descriptions
4. **Deliverable**: Full bifurcation diagram family

### Phase C: Advanced Features (Optional)
1. Implement density-based coloring
2. Add Lyapunov exponent overlay
3. Add custom parameter UI controls
4. **Deliverable**: Enhanced bifurcation visualization suite

---

## References

1. **Fractint Implementation**: `ManpWIN64\Bif.cpp` (lines 99-187)
2. **Histogram Renderer Pattern**: `ManpCore.Native\FractalEngineWrapper.cpp` (lines 570-622)
3. **Michael Barnsley, "Fractals Everywhere"**: Theoretical foundation for bifurcation behavior
4. **Robert Devaney, "An Introduction to Chaotic Dynamical Systems"**: Period doubling and chaos theory

---

## Notes

- **Coordinate System**: Bifurcation diagrams flip the typical fractal coordinate mapping. X-axis is parameter, Y-axis is dynamical variable. This is intentional and matches mathematical convention.
- **Performance**: Bifurcation rendering is O(width × transient × samples) rather than O(width × height × maxIter). For typical settings (width=1920, transient=200, samples=100), this is ~38M operations, comparable to a 1920×1080 escape-time render at 200 iterations.
- **Color Strategy**: Initial implementation uses simple white-on-black. Density-based coloring can be added later for more visually striking diagrams.
- **Compatibility**: Existing parameter-space visualizations remain unchanged. Bifurcation diagrams are a parallel family, not a replacement.

---

**Document Version**: 1.0  
**Date**: 2025-01-29  
**Author**: GitHub Copilot  
**Status**: Ready for Implementation
