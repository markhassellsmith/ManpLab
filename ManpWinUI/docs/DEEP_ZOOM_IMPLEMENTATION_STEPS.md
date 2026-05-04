# Deep Zoom Implementation - Step-by-Step Plan

**Status**: Phase A - Architecture Analysis  
**Branch**: `feature/perturbation-integration`  
**Last Updated**: January 2025

---

## Architecture Analysis Summary

### Core Components Identified

#### 1. Reference Orbit System
**Location**: `ManpWIN64/PertSetup.cpp` - `ReferenceZoomPoint()`
- **Purpose**: Build the high-precision reference orbit once per zoom level
- **Key Variables**:
  - `XSubN` (std::vector<Complex>) - Double precision reference orbit
  - `ExpXSubN` (std::vector<ExpComplex>) - Extended precision reference orbit
  - `MaxRefIteration` - Last iteration before escape (critical for BLA)
- **Process**:
  1. Allocate orbit storage based on `ArithType`
  2. Iterate from z=0 using BigDouble precision
  3. Store each iteration in XSubN or ExpXSubN
  4. Find escape iteration (MaxRefIteration)
  5. Build BLA table if enabled

#### 2. Perturbation Engine
**Location**: `ManpWIN64/PertEngine.h` - `CPerturbation` class
- **Purpose**: Calculate individual pixels using reference orbit + delta
- **Key Methods**:
  - `initialiseCalculateFrame()` - Setup per-frame parameters
  - `calculateOneFrame()` - Main perturbation loop
  - `AttachSharedTables()` - Link to reference orbit
  - `iterateFractalWithPerturbationBLA()` / `iterateFractalWithPerturbationBLAExp()`
- **Threading**: Each thread gets its own CPerturbation instance

#### 3. ArithType System
**Location**: `ManpWIN64/PertSetup.cpp` - `PertSetupArithType()`
```cpp
#define DOUBLE           0  // Normal double precision
#define FLOATEXP         1  // Extended precision (floatexp)
#define DBL_UNSUPPORTED  2  // Use double but no BLA
#define EXP_UNSUPPORTED  3  // Use floatexp but no BLA
```
- **Selection Logic**:
  - `precision <= 300` → DOUBLE or DBL_UNSUPPORTED
  - `precision > 300` → FLOATEXP or EXP_UNSUPPORTED
  - Only Mandelbrot (subtype 0) and Power (subtype 1) support full BLA

#### 4. BLA (Bilinear Approximation)
**Location**: `ManpWIN64/Approximation.h` - `BLAS` class
- **Purpose**: Skip iterations using polynomial approximation
- **Key Methods**:
  - `init()` / `initExp()` - Build approximation tables
  - Uses reference orbit to create series approximation
- **Performance**: 10-100x speedup for deep zooms

#### 5. Reference Validation & Caching
**Location**: `ManpWIN64/Perturbation.cpp` - `CheckValidRef()`
- **Purpose**: Reuse reference orbit when possible
- **Checks**:
  - Coordinates match
  - Zoom width matches
  - Bailout matches
  - Power/degree matches
  - Orbit size is sufficient

---

## Integration Strategy

### Phase 1: Native Wrapper Extension (2-3 days)

#### Step 1.1: Expose Reference Orbit Building
**File**: `ManpCore.Native/FractalEngineWrapper.h/cpp`

**Add new methods**:
```cpp
// Build reference orbit for perturbation
int BuildReferenceOrbit(
    String^ centerX,        // BigDouble as string
    String^ centerY,
    String^ viewWidth,
    int maxIteration,
    double bailout,
    int power,
    int subtype,
    int precision,
    bool enableBLA
);

// Check if reference orbit is valid for these parameters
bool IsReferenceOrbitValid(
    String^ centerX,
    String^ centerY,
    String^ viewWidth,
    int maxIteration,
    double bailout,
    int power
);

// Calculate with perturbation using existing reference orbit
FractalResult^ CalculateWithPerturbation(
    FractalParameters^ parameters
);
```

**Implementation Notes**:
- Call `PertSetupArithType()` to determine arithmetic type
- Call `ReferenceZoomPoint()` to build orbit
- Store `RefData` (StoreReferenceData struct) in wrapper
- Marshal progress callbacks to managed code

#### Step 1.2: Extend FractalResult
**File**: `ManpCore.Native/FractalEngineWrapper.h`

**Add properties**:
```cpp
public ref class FractalResult
{
    // ... existing properties ...

    // Perturbation-specific info
    property bool UsedPerturbation;
    property int ArithType;           // DOUBLE, FLOATEXP, etc.
    property int MaxRefIteration;     // Reference escape iteration
    property bool BLAEnabled;
    property double ReferenceOrbitBuildTime; // milliseconds
};
```

#### Step 1.3: CPerturbation Instance Management
**File**: `ManpCore.Native/FractalEngineWrapper.cpp`

**Strategy**:
- Create thread-local CPerturbation instances
- Call `AttachSharedTables()` to link to global XSubN/ExpXSubN
- Reuse instances across renders when possible
- Clean up on wrapper destruction

---

### Phase 2: Managed Service Integration (2-3 days)

#### Step 2.1: Extend FractalRenderService
**File**: `ManpWinUI/Services/FractalRenderService.cs`

**Add reference orbit cache**:
```csharp
private class ReferenceOrbitCache
{
    public BigRational CenterX { get; set; }
    public BigRational CenterY { get; set; }
    public BigRational ViewWidth { get; set; }
    public int MaxIteration { get; set; }
    public double Bailout { get; set; }
    public int Power { get; set; }
    public int Subtype { get; set; }
    public bool IsValid { get; set; }
}

private ReferenceOrbitCache _referenceOrbitCache = new();
```

**Add new render path**:
```csharp
private async Task<FractalResult> RenderWithPerturbationAsync(
    RenderSettings settings,
    IProgress<RenderProgress> progress,
    CancellationToken cancellationToken)
{
    // 1. Check if reference orbit needs rebuilding
    bool needsNewReference = !IsReferenceOrbitValid(settings);

    // 2. Build reference orbit if needed (one-time cost)
    if (needsNewReference)
    {
        await Task.Run(() =>
        {
            _nativeEngine.BuildReferenceOrbit(
                centerX: settings.CenterX.ToString(),
                centerY: settings.CenterY.ToString(),
                viewWidth: settings.ViewWidth.ToString(),
                maxIteration: settings.MaxIterations,
                bailout: settings.Bailout,
                power: settings.Power,
                subtype: GetSubtype(settings.FractalType),
                precision: CalculatePrecision(settings.ViewWidth),
                enableBLA: settings.EnableBLA
            );
        }, cancellationToken);

        UpdateReferenceOrbitCache(settings);
    }

    // 3. Calculate pixels using perturbation
    return await Task.Run(() =>
    {
        return _nativeEngine.CalculateWithPerturbation(parameters);
    }, cancellationToken);
}
```

#### Step 2.2: Deep Zoom Detection Logic
**File**: `ManpWinUI/Services/FractalRenderService.cs`

**Add threshold detection**:
```csharp
private bool ShouldUsePerturbation(RenderSettings settings)
{
    // Perturbation becomes beneficial when:
    // 1. Zoom level exceeds double precision limits (~10^15)
    // 2. Fractal type supports perturbation (Mandelbrot, Power, etc.)

    var precision = CalculatePrecision(settings.ViewWidth);
    const int PERTURBATION_THRESHOLD = 60; // ~10^15 zoom

    return precision >= PERTURBATION_THRESHOLD &&
           IsPerturbationSupported(settings.FractalType);
}

private bool IsPerturbationSupported(string fractalType)
{
    // From PertSetupArithType: only subtype 0 (Mandelbrot) and 1 (Power)
    // support full BLA in both DOUBLE and FLOATEXP modes
    return fractalType switch
    {
        "Mandelbrot" => true,
        "Power" => true,
        "BurningShip" => true,  // DBL_UNSUPPORTED mode
        "Celtic" => true,
        "Buffalo" => true,
        _ => false
    };
}
```

#### Step 2.3: Progress Reporting
**File**: `ManpWinUI/Services/FractalRenderService.cs`

**Update progress model**:
```csharp
public class RenderProgress
{
    // ... existing properties ...

    // Perturbation-specific progress
    public bool BuildingReferenceOrbit { get; set; }
    public int ReferenceOrbitProgress { get; set; } // 0-100
    public int ArithType { get; set; }
    public string ArithTypeName => ArithType switch
    {
        0 => "Double",
        1 => "Extended",
        2 => "Double (No BLA)",
        3 => "Extended (No BLA)",
        _ => "Unknown"
    };
}
```

---

### Phase 3: UI Enhancements (1-2 days)

#### Step 3.1: Settings ViewModel
**File**: `ManpWinUI/ViewModels/RenderSettingsViewModel.cs`

**Add properties**:
```csharp
private bool _enableBLA = true;
public bool EnableBLA
{
    get => _enableBLA;
    set => SetProperty(ref _enableBLA, value);
}

private bool _autoPerturbation = true;
public bool AutoPerturbation
{
    get => _autoPerturbation;
    set => SetProperty(ref _autoPerturbation, value);
}

private int _perturbationThreshold = 60;
public int PerturbationThreshold
{
    get => _perturbationThreshold;
    set => SetProperty(ref _perturbationThreshold, value);
}
```

#### Step 3.2: Status Display
**File**: `ManpWinUI/Views/MainPage.xaml`

**Add status indicators**:
```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <TextBlock Text="Precision:" />
    <TextBlock Text="{x:Bind ViewModel.CurrentPrecision, Mode=OneWay}" />

    <TextBlock Text="Mode:" Margin="16,0,0,0" />
    <TextBlock Text="{x:Bind ViewModel.ArithTypeName, Mode=OneWay}" />

    <ProgressRing IsActive="{x:Bind ViewModel.BuildingReferenceOrbit, Mode=OneWay}"
                  Width="16" Height="16" Margin="8,0,0,0" />
    <TextBlock Text="Building Reference..."
               Visibility="{x:Bind ViewModel.BuildingReferenceOrbit, Mode=OneWay}" />
</StackPanel>
```

---

### Phase 4: Testing & Validation (2-3 days)

#### Test Case 1: Moderate Zoom (10^20)
**Location**: Mandelbrot set mini-brot antenna
- Compare temporary BigDouble vs perturbation performance
- Verify identical pixel output
- Measure speedup (expect 2-5x)

#### Test Case 2: Deep Zoom (10^50)
**Location**: Deep Mandelbrot mini-brot
- Verify FLOATEXP mode activates
- Check BLA acceleration
- Measure speedup (expect 10-50x)

#### Test Case 3: Extreme Zoom (10^100+)
**Location**: Known deep zoom coordinates
- Verify extended precision handles zoom
- Check memory usage
- Verify no glitches

#### Test Case 4: Reference Orbit Caching
**Scenario**: Pan at same zoom level
- First render: builds reference orbit
- Subsequent renders: reuse orbit
- Verify only reference build happens once

#### Test Case 5: Fractal Type Coverage
**Test each**:
- Mandelbrot (subtype 0) - DOUBLE/FLOATEXP with BLA
- Power (subtype 1) - DOUBLE/FLOATEXP with BLA
- Burning Ship (subtype 2) - DBL_UNSUPPORTED/EXP_UNSUPPORTED
- Celtic, Buffalo, etc. - Verify correct ArithType selection

---

### Phase 5: Cleanup & Documentation (1 day)

#### Step 5.1: Remove Temporary Code
**Files to clean**:
- `ManpCore.Native/FractalEngineWrapper.cpp` - Remove BigDouble fallback
- `ManpCore.Native/BigDoubleSupport.cpp` - Can be removed entirely
- `ManpWinUI/Services/FractalRenderService.cs` - Remove temporary deep zoom block

#### Step 5.2: Update Documentation
**Files to update**:
- `ARCHITECTURE_NATIVE_ENGINE.md` - Add perturbation section
- `PROGRESS_SUMMARY.md` - Mark Phase 3.5 complete
- `PROJECT_PLAN.md` - Update timeline
- `KNOWN_ISSUES.md` - Close temporary deep zoom issue

#### Step 5.3: Performance Documentation
**Create**: `DEEP_ZOOM_PERFORMANCE.md`
- Benchmark results at various zoom levels
- Memory usage analysis
- Thread scaling data
- BLA effectiveness metrics

---

## Implementation Timeline

| Phase | Tasks | Duration | Dependencies |
|-------|-------|----------|--------------|
| **Phase 1** | Native wrapper extension | 2-3 days | Architecture analysis complete |
| **Phase 2** | Managed service integration | 2-3 days | Phase 1 complete |
| **Phase 3** | UI enhancements | 1-2 days | Phase 2 complete |
| **Phase 4** | Testing & validation | 2-3 days | Phase 3 complete |
| **Phase 5** | Cleanup & documentation | 1 day | Phase 4 complete |
| **Total** | | **8-12 days** | |

---

## Key Dependencies Mapped

### Native Side (ManpWIN64)
```
PertSetup.cpp
├── ReferenceZoomPoint() - Build reference orbit
├── PertSetupArithType() - Determine precision mode
└── CheckValidRef() - Validate cached orbit

PertEngine.h/CPerturbation
├── initialiseCalculateFrame() - Setup
├── calculateOneFrame() - Main loop
├── AttachSharedTables() - Link to XSubN/ExpXSubN
├── iterateFractalWithPerturbationBLA() - Pixel calculation
└── RefFunctions() - Reference orbit iteration

Approximation.h/BLAS
├── init() - Build BLA table (DOUBLE)
├── initExp() - Build BLA table (FLOATEXP)
└── Various LA_* members - BLA data structures

Global State
├── XSubN - Double precision reference orbit
├── ExpXSubN - Extended precision reference orbit
├── MaxRefIteration - Escape iteration
├── Bla - BLA table instance
├── ArithType - Current precision mode
└── SlopeDegree - Derivative power
```

### Wrapper Side (ManpCore.Native)
```
FractalEngineWrapper.cpp
├── BuildReferenceOrbit() - NEW: Call ReferenceZoomPoint
├── IsReferenceOrbitValid() - NEW: Call CheckValidRef
├── CalculateWithPerturbation() - NEW: Use CPerturbation
└── RefData - NEW: Cache reference orbit metadata
```

### Managed Side (ManpWinUI)
```
FractalRenderService.cs
├── RenderWithPerturbationAsync() - NEW: Perturbation path
├── ShouldUsePerturbation() - NEW: Detection logic
├── IsReferenceOrbitValid() - NEW: Cache validation
└── ReferenceOrbitCache - NEW: Managed cache

RenderSettingsViewModel.cs
├── EnableBLA - NEW: User setting
├── AutoPerturbation - NEW: Auto-enable setting
└── PerturbationThreshold - NEW: Precision threshold
```

---

## Critical Implementation Notes

### 1. Threading Model
- **Native**: Each thread gets its own `CPerturbation` instance
- **Managed**: Service must coordinate reference orbit building (single-threaded) before pixel calculation (multi-threaded)
- **Synchronization**: Reference orbit must complete before any pixel calculation starts

### 2. Memory Management
- **XSubN/ExpXSubN**: Global vectors, managed by native code
- **Bla tables**: Owned by global `Bla` instance
- **CPerturbation instances**: Thread-local, created on demand
- **Cleanup**: Must ensure vectors are cleared when switching fractals

### 3. Precision Calculation
```csharp
private int CalculatePrecision(BigRational viewWidth)
{
    // precision ≈ -log10(viewWidth) * 3.32 (bits per decimal digit)
    // Example: 10^-50 zoom = 50 * 3.32 ≈ 166 bits precision
    var log10Width = BigRational.Log10(viewWidth);
    return (int)Math.Ceiling(Math.Abs(log10Width) * 3.32);
}
```

### 4. Progress Reporting
**Two-phase progress**:
1. **Reference orbit building**: 0-30% of total time (depends on maxIteration)
2. **Pixel calculation**: 30-100% (normal progress reporting)

### 5. Error Handling
**Key error scenarios**:
- Reference orbit build failure (user cancellation, precision overflow)
- Glitch detection (pixel delta exceeds threshold)
- Memory allocation failure (extreme zoom levels)

---

## Next Action Items

1. ✅ **Complete**: Architecture analysis (this document)
2. **Next**: Implement Phase 1, Step 1.1 - Add `BuildReferenceOrbit()` to FractalEngineWrapper
3. **Then**: Implement Phase 1, Step 1.2 - Extend FractalResult
4. **Then**: Implement Phase 1, Step 1.3 - CPerturbation instance management

---

## Success Criteria

- [ ] Can zoom to 10^100+ on Mandelbrot set without artifacts
- [ ] Reference orbit caching works (no rebuild on pan)
- [ ] BLA acceleration provides 10-100x speedup vs. brute force
- [ ] FLOATEXP mode auto-activates for precision > 300
- [ ] All 14 fractals work in appropriate ArithType mode
- [ ] Progress reporting shows reference build + pixel calculation
- [ ] Memory usage stays reasonable (< 2GB for typical deep zoom)
- [ ] Thread scaling works (4 threads = ~3.5x speedup)
- [ ] Temporary BigDouble code fully removed
- [ ] No regression in existing double-precision rendering

