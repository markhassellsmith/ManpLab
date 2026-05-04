# Next Action: Fix Linker Errors

## Problem
The code compiles but has 16 unresolved external symbols from perturbation functions.

## Solution
Add perturbation source files to the ManpCore.Native project build.

## Steps

### 1. Open ManpCore.Native Project
Right-click on `ManpCore.Native` project in Solution Explorer → Properties

### 2. Add Source Files to Build
**Method A: Via Solution Explorer (Quick)**
1. Right-click `ManpCore.Native` project → Add → Existing Item
2. Navigate to `ManpWIN64` folder
3. Select these files:
   - `PertSetup.cpp`
   - `Perturbation.cpp`
   - `Approximation.cpp`
   - `PertEngine.cpp` (if it exists)
4. Add as Link (not copy) to preserve single source

**Method B: Edit .vcxproj Directly**
Add to `<ItemGroup>` in `ManpCore.Native.vcxproj`:
```xml
<ClCompile Include="..\ManpWIN64\PertSetup.cpp" />
<ClCompile Include="..\ManpWIN64\Perturbation.cpp" />
<ClCompile Include="..\ManpWIN64\Approximation.cpp" />
```

### 3. Verify Include Paths
Ensure ManpCore.Native project includes:
- `$(ProjectDir)..\ManpWIN64`
- `$(ProjectDir)..\ManpWIN64\mpfr`
- `$(ProjectDir)..\ManpWIN64\gmp`

### 4. Build and Test
```powershell
# Clean build
msbuild ManpLab.sln /t:Clean /p:Configuration=Debug /p:Platform=x64

# Rebuild
msbuild ManpLab.sln /t:Rebuild /p:Configuration=Debug /p:Platform=x64
```

Expected output: **Build succeeded**

### 5. Quick Smoke Test
Once build succeeds, test in C#:
```csharp
using var engine = new FractalEngineWrapper();

// Build reference orbit for Mandelbrot at moderate zoom
int result = engine.BuildReferenceOrbit(
    centerX: "-0.7463",
    centerY: "0.1102",
    viewWidth: "1e-10",
    maxIteration: 1000,
    bailout: 4.0,
    power: 2,
    subtype: 0,  // Mandelbrot
    precision: 100,
    enableBLA: true
);

// Should return 0 (success)
Console.WriteLine($"BuildReferenceOrbit result: {result}");

// Check if orbit is valid
bool valid = engine.IsReferenceOrbitValid(
    "-0.7463", "0.1102", "1e-10",
    1000, 4.0, 2
);

// Should be true
Console.WriteLine($"Reference orbit valid: {valid}");
```

## Expected Build Time
- Clean build: ~30-60 seconds (perturbation files are large)
- Incremental: ~5-10 seconds

## Known Dependencies
`PertSetup.cpp` and `Perturbation.cpp` depend on:
- `BigDouble.cpp` ✅ (already in ManpWIN64)
- `BigComplex.cpp` ✅
- `Complex.cpp` ✅
- `Approximation.cpp` ⚠️ (need to add)
- `FloatExp.cpp` ✅
- MPFR library ✅ (linked)

## If Build Still Fails
Check for:
1. Missing include directories
2. MPFR/GMP library paths
3. C++/CLI CLR support enabled
4. Precompiled headers conflict
5. Static vs. dynamic runtime mismatch

## After Success
Proceed to implement `CalculateWithPerturbation()` pixel loop as described in DEEP_ZOOM_IMPLEMENTATION_STEPS.md Phase 1, Step 1.3.
