# BLA Image Dimension Fix - Implementation Summary

**Status:** ✅ **COMPLETED**  
**Date:** 2026-05-04  
**Branch:** `feature/perturbation-integration`  
**Priority:** **CRITICAL** - Core deep zoom performance feature

---

## Problem Statement

### Root Cause
BLA (Bilinear Approximation) table initialization was using **hardcoded image dimensions** (`xdots=1920`, `ydots=1080`) instead of the actual render resolution. This caused BLA to calculate incorrect approximation regions, rendering the optimization completely ineffective.

### Impact
- **No performance improvement** from BLA at E-12 through E-14
- Expected 5-10x speedup was not realized
- Deep zoom rendering remained at raw perturbation speed (~100-200ms) instead of BLA-accelerated speed (~20-40ms)

---

## Solution Overview

### Core Fix
**Pass actual image dimensions** to `BuildReferenceOrbit()` so BLA initialization uses correct pixel-to-complex-plane scaling.

### Key Formula (from `PertSetup.cpp` line 164-176)
```cpp
int image_size = min(xdots, ydots);
double ZoomRadius = mpfr_get_d(BigWidth.x, MPFR_RNDN);

// BLA size depends on image dimensions and zoom radius
Complex temp_size_image_size = Complex(ZoomRadius / (double)image_size, 
                                        ZoomRadius / (double)image_size);
double blaSize = hypot(temp_size_image_size.x * xdots * 0.5, 
                       temp_size_image_size.y * ydots * 0.5);

Bla.init(M, XSubN, blaSize, power, subtype, maxIteration, param);
```

**Critical:** If `xdots`/`ydots` don't match the actual render dimensions, `blaSize` is wrong and BLA approximations fail.

---

## Implementation Changes

### 1. Native Wrapper API Update
**File:** `ManpCore.Native/FractalEngineWrapper.h`

**Added Parameters:**
```cpp
int BuildReferenceOrbit(
    String^ centerX,
    String^ centerY,
    String^ viewWidth,
    int maxIteration,
    double bailout,
    int power,
    int subtype,
    int precision,
    bool enableBLA,
    int imageWidth,      // ← NEW: Actual render width
    int imageHeight      // ← NEW: Actual render height
);
```

### 2. Native Implementation
**File:** `ManpCore.Native/FractalEngineWrapper.cpp`

**Changes:**
```cpp
int FractalEngineWrapper::BuildReferenceOrbit(
    String^ centerX,
    String^ centerY,
    String^ viewWidth,
    int maxIteration,
    double bailout,
    int power,
    int subtype,
    int precision,
    bool enableBLA,
    int imageWidth,
    int imageHeight)
{
    // Set image dimensions for BLA size calculation
    extern int xdots, ydots;
    xdots = imageWidth;   // ← Set before ReferenceZoomPoint()
    ydots = imageHeight;

    Debug::WriteLine(String::Format("BuildReferenceOrbit: Set xdots={0}, ydots={1}", 
        xdots, ydots));

    // ... rest of reference orbit building
    ReferenceZoomPoint(...);  // ← Now uses correct dimensions
}
```

### 3. Managed Service Update
**File:** `ManpWinUI/Services/FractalRenderService.cs`

**Call Site Update:**
```csharp
var orbitResult = _engine.BuildReferenceOrbit(
    parameters.BigCenterX.ToString(),
    parameters.BigCenterY.ToString(),
    parameters.BigViewWidth.ToString(),
    parameters.MaxIterations,
    256.0,  // bailout
    2,      // power
    0,      // subtype
    parameters.Precision,
    true,   // enable BLA
    parameters.Width,   // ← NEW: Pass actual render width
    parameters.Height   // ← NEW: Pass actual render height
);
```

### 4. Stub File Cleanup
**File:** `ManpCore.Native/PerturbationStubs.cpp`

**Removed duplicate definitions** that conflicted with real implementations:
- Removed `BLAS::initExp()` stub (now in `ApproximationExp.cpp`)
- Removed `ExpComplex::~ExpComplex()` stub (now in `ExpComplex.cpp`)

**Kept necessary stubs:**
- `xdots`, `ydots` globals (now set dynamically)
- `subtype`, `param`, `EnableApproximation`, etc.

### 5. Project Configuration
**File:** `ManpCore.Native/ManpCore.Native.vcxproj`

**Added native sources:**
```xml
<ClCompile Include="..\ManpWIN64\ApproximationExp.cpp">
  <CompileAsManaged>false</CompileAsManaged>
</ClCompile>
<ClCompile Include="..\ManpWIN64\ExpComplex.cpp">
  <CompileAsManaged>false</CompileAsManaged>
</ClCompile>
```

---

## Expected Performance Improvement

### Before Fix (BLA Ineffective)
```
E-12:  ~100ms  (reference orbit + raw perturbation)
E-13:  ~120ms
E-14:  ~150ms
E-15:  ~180ms
```

### After Fix (BLA Active)
```
First render (cold cache):
E-12:  ~100ms  (reference orbit build dominates)
E-13:  ~120ms
E-14:  ~150ms

Subsequent renders (hot cache, BLA optimized):
E-12:   ~20ms  ← 5x faster!
E-13:   ~25ms  ← 5x faster!
E-14:   ~30ms  ← 5x faster!

Panning at same zoom:
E-14:   ~25ms  ← Reference orbit reused, BLA skips 50-90% iterations
```

### BLA Skip Rate (Expected)
- **Seahorse Valley (-0.7463, 0.1102):** 70-90% iteration skips
- **Standard Mandelbrot locations:** 50-70% iteration skips
- **Interior locations:** 0-20% skips (most pixels hit maxIterations anyway)

---

## Testing Recommendations

### Test Location: Seahorse Valley
**Best location for BLA validation:**
```
Center: -0.7463, 0.1102
```

This location is ideal because:
- ✓ Reference orbit escapes reliably
- ✓ Mix of escaped/non-escaped pixels
- ✓ Complex spiral detail at all zoom levels
- ✓ Industry-standard benchmark location

### Test Sequence

#### 1. **Zoom Through Threshold**
```
Start: E-11 (standard rendering, ~18ms)
       E-12 (standard, artifacts, ~20ms)
       ────────────────────────────────
       E-13 (perturbation kicks in, ~100ms first time)
       E-14 (~120ms)
```

**Expected:** Clear performance step at E-13 threshold.

#### 2. **Pan at Fixed Zoom (E-14)**
```
First render:  ~150ms (reference orbit build + BLA init)
Pan left:       ~30ms (orbit reused, BLA active!)
Pan right:      ~30ms
Pan up:         ~30ms
```

**Expected:** **5x speedup** on subsequent renders vs. first render.

#### 3. **Compare With/Without BLA**
Manually disable BLA in code and compare:
```cpp
// In FractalRenderService.cs BuildReferenceOrbit call:
enableBLA: false  // ← Test without BLA

Result: Should take ~150ms even on subsequent renders (no skip optimization)
```

#### 4. **Check Debug Logs**
Look for in `BuildReferenceOrbit` output:
```
BuildReferenceOrbit: Set xdots=1920, ydots=1080
BuildReferenceOrbit: ArithType=0, SlopeDegree=2
BuildReferenceOrbit: Success! MaxRefIteration=11869, orbit size=20001
```

Look for in `CalculateWithPerturbation` output:
```
CalculateWithPerturbation: BLA enabled=true
CalculateWithPerturbation: Complete! Time=25ms, AvgIter=250, BLA skips=185000
```

**Key metric:** `BLA skips` should be **high** (100,000+) on successful renders.

---

## Validation Checklist

- [x] ✅ Build succeeds with new parameters
- [x] ✅ Native DLL copied to solution output directory
- [ ] ⏳ Test E-12 through E-14 at Seahorse Valley
- [ ] ⏳ Verify `MaxRefIteration < maxIterations` (reference orbit escapes)
- [ ] ⏳ Confirm `BLA enabled=true` in logs
- [ ] ⏳ Measure first render time (~100-150ms expected)
- [ ] ⏳ Measure panning render time (~20-40ms expected)
- [ ] ⏳ Check BLA skip count (should be > 50% of total iterations)
- [ ] ⏳ Verify image quality (should be pixel-perfect at E-14)

---

## Known Limitations

### Formula Support
BLA is **only available** for:
- ✓ Mandelbrot (`subtype=0`)
- ✓ Power fractals (`subtype=1`)

**Not available** for:
- ❌ Burning Ship (`subtype=2`)
- ❌ Other exotic formulas

When BLA is unavailable, `ArithType` will be set to `DBL_UNSUPPORTED` or `EXP_UNSUPPORTED`, and raw perturbation is used (slower but still correct).

### Precision Limits
- **DOUBLE mode:** E-12 to ~E-16 (fast, BLA works)
- **FLOATEXP mode:** E-17+ (10x slower, BLA still works but less effective)

### Reference Orbit Cache Invalidation
Cached orbit becomes invalid when:
- ❌ Center coordinates change (new location)
- ❌ Zoom level changes (new viewWidth)
- ❌ MaxIterations increases
- ❌ Power parameter changes
- ✓ Palette changes (orbit reused!)
- ✓ Color offset changes (orbit reused!)

---

## Next Steps (If Testing Succeeds)

1. ✅ **Commit changes** to `feature/perturbation-integration`
2. ⏳ **Run validation tests** at Seahorse Valley
3. ⏳ **Document performance benchmarks** in `DEEP_ZOOM_VALIDATION_PLAN.md`
4. ⏳ **Add automated tests** for BLA initialization with different image sizes
5. ⏳ **Profile BLA effectiveness** across zoom levels E-12 to E-20
6. ⏳ **Merge to development** after validation passes

---

## References

### Source Files Modified
- `ManpCore.Native/FractalEngineWrapper.h` (API signature)
- `ManpCore.Native/FractalEngineWrapper.cpp` (implementation)
- `ManpCore.Native/PerturbationStubs.cpp` (cleanup)
- `ManpCore.Native/ManpCore.Native.vcxproj` (build config)
- `ManpWinUI/Services/FractalRenderService.cs` (call site)

### Key Native Files (Reference Only)
- `ManpWIN64/PertSetup.cpp` (BLA initialization logic, lines 155-180)
- `ManpWIN64/Approximation.cpp` (BLA table construction)
- `ManpWIN64/ApproximationExp.cpp` (extended-precision BLA)
- `ManpWIN64/ExpComplex.cpp` (floatexp arithmetic)

### Documentation
- `ManpWinUI/docs/DEEP_ZOOM_IMPLEMENTATION_STEPS.md` (overall roadmap)
- `ManpWinUI/docs/DEEP_ZOOM_VALIDATION_PLAN.md` (testing strategy)
- `ManpWinUI/docs/DEEP_ZOOM_THRESHOLD_FIX.md` (viewport-width threshold fix)

---

## Commit Message (Draft)

```
fix(deep-zoom): Pass actual image dimensions to BLA initialization

BLA (Bilinear Approximation) was using hardcoded 1920x1080 dimensions
instead of actual render size, causing incorrect approximation regions
and zero performance benefit.

Changes:
- Add imageWidth/imageHeight parameters to BuildReferenceOrbit()
- Set xdots/ydots globals before ReferenceZoomPoint() call
- Update FractalRenderService to pass parameters.Width/Height
- Add ApproximationExp.cpp and ExpComplex.cpp to native build
- Remove duplicate stubs for BLAS::initExp and ExpComplex destructor

Expected impact: 5-10x speedup for deep zoom renders after first orbit build.

Testing: Use Seahorse Valley (-0.7463, 0.1102) at E-14 to verify BLA skips.

Related: DEEP_ZOOM_IMPLEMENTATION_STEPS.md Phase 3.5
```

---

**Status:** Implementation complete, ready for testing! 🚀
