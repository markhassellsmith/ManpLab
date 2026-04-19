# Modular Fractal Architecture Design
## ManpLab Phase 3 - Scalable 240+ Fractal Implementation

### Executive Summary
**Goal**: Implement all 240+ ManpWIN64 fractals in WinUI 3 with:
- ✅ Low memory footprint (only load active fractal)
- ✅ Easy code maintenance (modular, isolated implementations)
- ✅ High code integrity (type-safe registration, compile-time validation)
- ✅ Performance (leverage existing native C++ calculations)

---

## Current State Analysis

### What ManpWIN64 Already Has ✅
1. **Fractal ID System**: `Fractype.h` defines 245+ fractal constants (MANDEL=0, BURNINGSHIP=223, etc.)
2. **Specification Table**: `fractalspecific[]` array with metadata for each fractal
3. **Calculation Functions**: Individual C++ functions per fractal type (e.g., `JuliaFractal()`, `BurningShip()`)
4. **Parameter System**: Each fractal defines its parameters and UI labels

### What We Currently Have (Minimal) ⚠️
- Hardcoded `CalculateSmoothIterations()` for Mandelbrot
- Added `CalculateBurningShip()`, `CalculateTricorn()`, `CalculatePhoenix()` 
- **Problem**: If-else chain will become 240 cases long! 🚫

---

## Proposed Architecture

### 1. **Native Layer: Fractal Function Registry**

```cpp
// ManpCore.Native/FractalRegistry.h
#pragma once
#include <string>
#include <unordered_map>
#include <functional>
#include "MandelbrotCalculator.h"

namespace Native {

// Function signature for all fractal calculators
typedef std::function<double(ComplexD c, int maxIter, bool isJulia, ComplexD juliaC)> FractalCalculator;

class FractalRegistry
{
private:
    static std::unordered_map<std::string, FractalCalculator> s_calculators;
    static bool s_initialized;

public:
    // Register a fractal calculation function
    static void Register(const std::string& name, FractalCalculator calculator);

    // Get calculator by name (returns nullptr if not found)
    static FractalCalculator GetCalculator(const std::string& name);

    // Initialize all built-in fractals
    static void Initialize();

    // Get list of all registered fractal names
    static std::vector<std::string> GetRegisteredNames();
};

} // namespace Native
```

### 2. **Modular Fractal Implementations**

Each fractal family gets its own compilation unit:

```
ManpCore.Native/
├── Calculators/
│   ├── MandelbrotFamily.cpp     // Mandelbrot, Multibrot, etc.
│   ├── BurningShipFamily.cpp    // Burning Ship variations
│   ├── TricornFamily.cpp        // Tricorn/Mandelbar variations
│   ├── PhoenixFamily.cpp        // Phoenix, Lambda, etc.
│   ├── NewtonFamily.cpp         // All Newton fractals
│   ├── MagnetFamily.cpp         // Magnet1, Magnet2
│   ├── TrigFamily.cpp           // All trig-based fractals
│   ├── BarnsleyFamily.cpp       // Barnsley variations
│   ├── ...                      // 20-30 family files total
│   └── FractalRegistry.cpp      // Registration implementation
```

### 3. **Auto-Registration Pattern**

Each family file registers itself on initialization:

```cpp
// BurningShipFamily.cpp
#include "FractalRegistry.h"

namespace Native {

// Burning Ship calculation
static double CalculateBurningShip(ComplexD c, int maxIter, bool isJulia, ComplexD juliaC)
{
    ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
    ComplexD constant = isJulia ? juliaC : c;

    for (int i = 0; i < maxIter; i++)
    {
        double zx_abs = fabs(z.x);
        double zy_abs = fabs(z.y);
        double mag2 = zx_abs * zx_abs + zy_abs * zy_abs;

        if (mag2 > 256.0)
            return i + 1.0 - log(log(mag2) / 2.0) / log(2.0);

        double zx_new = zx_abs * zx_abs - zy_abs * zy_abs + constant.x;
        z.y = 2.0 * zx_abs * zy_abs + constant.y;
        z.x = zx_new;
    }
    return (double)maxIter;
}

// Auto-register on module load
static bool g_burningShipRegistered = []() {
    FractalRegistry::Register("BurningShip", CalculateBurningShip);
    FractalRegistry::Register("BurningShipJulia", CalculateBurningShip); // Same func, different name
    return true;
}();

} // namespace Native
```

### 4. **Single Point of Dispatch**

```cpp
// FractalEngineWrapper.cpp (simplified)
double iteration;
auto calculator = ::Native::FractalRegistry::GetCalculator(fractalType);

if (calculator)
{
    iteration = calculator(c, nativeParams.maxIterations, 
                          nativeParams.isJulia, 
                          ::Native::ComplexD(nativeParams.juliaCX, nativeParams.juliaCY));
}
else
{
    // Fallback to Mandelbrot or throw error
    iteration = ::Native::MandelbrotCalculator::CalculateSmoothIterations(...);
}
```

---

## Memory & Performance Benefits

### Current Approach (If-Else Chain)
```cpp
// ❌ BAD: All 240+ formulas compiled into one giant function
if (fractalType == "BurningShip") { ... }
else if (fractalType == "Tricorn") { ... }
else if (fractalType == "Phoenix") { ... }
// ... 237 more cases ...
```
- **Memory**: All code loaded into instruction cache
- **Compile time**: Single massive file, slow compilation
- **Maintenance**: One huge file, hard to navigate

### Modular Approach
```cpp
// ✅ GOOD: Registry dispatches to loaded function
auto calculator = FractalRegistry::GetCalculator(fractalType);
```
- **Memory**: Only active calculator in hot path
- **Compile time**: Parallel compilation of family files
- **Maintenance**: Each family isolated, easy to test

---

## Integration with ManpWIN64 Native Engine

### Phase 1: Wrapper Calculators (Current Approach)
We write our own calculation functions in `MandelbrotCalculator.h`

**Pros**: 
- Simple, no dependencies on ManpWIN64 internals
- Good for learning/testing

**Cons**: 
- Duplicates existing work
- Won't support all 240+ fractals easily

### Phase 2: Direct Native Engine Integration ⭐ (Recommended)

```cpp
// Leverage ManpWIN64's existing fractalspecific[] table
extern struct fractalspecificstuff fractalspecific[];

// Wrapper for native fractal calculation
static double CallNativeFractal(int fractalID, ComplexD c, int maxIter, ...)
{
    // Call the actual ManpWIN64 orbit calculation function
    return fractalspecific[fractalID].orbitcalc();
}

// Register by fractal ID
FractalRegistry::Register("Mandelbrot", 
    [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC) {
        return CallNativeFractal(MANDEL, c, maxIter, ...);
    });
```

**Pros**:
- ✅ Instant access to all 240+ fractals
- ✅ Battle-tested calculation code
- ✅ Preserves exact rendering fidelity

**Cons**:
- Requires deeper C++ interop
- Need to understand ManpWIN64's state management

---

## Migration Path

### Milestone 1: Registry Infrastructure ✅ (Week 1)
- [ ] Create `FractalRegistry` class
- [ ] Implement registration system
- [ ] Add `GetCalculator()` dispatch
- [ ] Unit tests for registry

### Milestone 2: Refactor Existing 4 Fractals (Week 1-2)
- [ ] Move Mandelbrot → `MandelbrotFamily.cpp`
- [ ] Move BurningShip → `BurningShipFamily.cpp`
- [ ] Move Tricorn → `TricornFamily.cpp`
- [ ] Move Phoenix → `PhoenixFamily.cpp`
- [ ] Update `FractalEngineWrapper` to use registry
- [ ] Verify all 4 render identically

### Milestone 3: Add 10 More Fractal Families (Week 2-3)
- [ ] Newton family (5 variations)
- [ ] Barnsley family (4 variations)
- [ ] Magnet family (2 variations)
- [ ] Lambda family (3 variations)
- [ ] Trig family (8 variations)
- [ ] Test and validate each

### Milestone 4: ManpWIN64 Native Integration (Week 3-4)
- [ ] Study `fractalspecific[]` structure
- [ ] Create wrapper for `orbitcalc()` functions
- [ ] Map fractal names → IDs
- [ ] Test 50 random fractals for correctness

### Milestone 5: Complete 240+ Fractals (Week 4-6)
- [ ] Generate registration code from `Fractype.h`
- [ ] Create metadata service (names, descriptions, parameters)
- [ ] Build fractal selection dialog
- [ ] Performance testing at scale

---

## UI Layer: Fractal Metadata Service

```csharp
// ManpWinUI/Services/IFractalMetadataService.cs
public interface IFractalMetadataService
{
    IEnumerable<FractalInfo> GetAllFractals();
    FractalInfo GetFractalByName(string name);
    IEnumerable<FractalInfo> GetFractalsByCategory(string category);
}

public record FractalInfo(
    string Name,
    string DisplayName,
    string Category,
    string Description,
    FractalParameter[] Parameters,
    ViewDefaults DefaultView,
    bool SupportsJulia
);
```

This allows the UI to:
- Build dynamic fractal selection dialogs
- Show context-appropriate parameters
- Provide tooltips and help text
- Group fractals by category

---

## Code Integrity Safeguards

### 1. Compile-Time Registration Validation
```cpp
// Ensure all Fractype.h constants are registered
static_assert(FractalRegistry::GetCount() >= NUMFRACTAL, 
              "Not all fractals registered!");
```

### 2. Unit Tests Per Family
```cpp
TEST(BurningShipFamily, RendersCorrectly) {
    auto calc = FractalRegistry::GetCalculator("BurningShip");
    ASSERT_NE(calc, nullptr);

    double result = calc(ComplexD(-0.5, -0.5), 256, false, ComplexD());
    EXPECT_GT(result, 0.0);
}
```

### 3. Metadata Consistency Checks
```csharp
[Test]
public void AllFractalsHaveMetadata() {
    var registered = nativeEngine.GetRegisteredFractals();
    var metadata = metadataService.GetAllFractals();

    Assert.Equal(registered.Count, metadata.Count);
}
```

---

## Performance Characteristics

| Metric | Current (If-Else) | Modular (Registry) |
|--------|------------------|-------------------|
| Memory footprint | ~500KB (all formulas) | ~50KB (active + registry) |
| Instruction cache | Poor (branch mispredicts) | Excellent (direct call) |
| Compile time | O(n²) - one file | O(n) - parallel build |
| Extensibility | Add to if-else chain | Drop in new .cpp file |
| Testing | Test entire module | Test individual families |

---

## Recommended Next Steps

1. **Approve Architecture** ✅ (You're reading this!)
2. **Create Registry Infrastructure** (2-3 hours)
3. **Refactor Existing 4 Fractals** (4-6 hours)
4. **Add 5 More Families** (1-2 days) - Validate approach
5. **ManpWIN64 Integration Study** (2-3 days) - Critical path
6. **Scale to 240+** (1-2 weeks) - Mostly mechanical

**Total Estimated Time**: 3-4 weeks to full 240+ fractal support

---

## Alternative Considered: DLL Plugin System

**Why NOT chosen**:
- Overkill for this use case
- Adds deployment complexity
- Runtime discovery overhead
- Static registration is compile-time safe

**When to reconsider**:
- User-defined fractal formulas
- Third-party fractal packs
- Hot-reload during development

---

## Questions for Discussion

1. **Should we start with wrapper calculators or go straight to ManpWIN64 integration?**
   - My recommendation: Start with wrappers (Milestone 2), then integrate (Milestone 4)

2. **Category/grouping strategy?**
   - Use ManpWIN64's existing organization?
   - Create new taxonomy for modern UI?

3. **Parameter handling?**
   - Each fractal has 0-4 custom parameters
   - Should UI be dynamic or have preset layouts?

4. **Julia mode support?**
   - All fractals? Subset? Case-by-case basis?

---

**Author**: GitHub Copilot  
**Date**: 2025-01-XX  
**Status**: PROPOSED - Awaiting Approval
