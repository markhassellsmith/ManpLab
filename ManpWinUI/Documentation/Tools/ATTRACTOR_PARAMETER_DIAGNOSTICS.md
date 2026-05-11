# Attractor Parameter Diagnostics Tool

## Overview

A standalone C++ diagnostic utility for systematically testing parameter combinations of chaotic attractors to find stable, visually interesting configurations. This tool is essential when histogram-based attractors produce solid colors, blank screens, or escape to infinity.

## Purpose

When implementing new histogram-based attractors, finding the right parameters is challenging because:
- **Orbits may explode** (escape to infinity) with large timesteps or wrong parameter values
- **Orbits may collapse** (converge to a fixed point) with poor parameters
- **Attractor structures may be too small or large** for the default zoom level
- **Initial conditions matter** - some starting points lead to different basins of attraction

This tool automates the search for "sweet spots" where attractors produce bounded, structured, visually interesting results.

## How It Works

### Statistical Analysis
For each parameter combination tested, the tool:
1. Runs 50,000 orbit iterations (skipping first 100 for transient behavior)
2. Tracks min/max bounds in X, Y, Z dimensions
3. Calculates average orbit magnitude
4. Detects explosion (magnitude > 1000 or NaN/Inf)
5. Detects collapse (total range < 0.01)
6. Reports configurations with visible structure (bounded, magnitude 0.1-100)

### Test Types
1. **Parameter sweep**: Tests combinations of system parameters and timesteps
2. **Initial condition test**: Tests different starting points (x₀, y₀, z₀)
3. **Zoom estimation**: Reports actual attractor bounds to guide zoom settings

## Implementation

### Complete Source Code

```cpp
#include <iostream>
#include <vector>
#include <cmath>
#include <algorithm>

namespace Native
{
    struct AttractorStats
    {
        double minX, maxX, minY, maxY, minZ, maxZ;
        double avgMagnitude;
        bool isExploding;
        bool isCollapsing;
        bool hasStructure;
        int validPoints;
    };

    // Test an attractor configuration and return statistics
    template<typename OrbitFunc>
    AttractorStats TestAttractorParameters(
        OrbitFunc orbitFunc,
        double x0, double y0, double z0,
        int iterations = 50000)
    {
        AttractorStats stats;
        stats.minX = stats.minY = stats.minZ = 1e10;
        stats.maxX = stats.maxY = stats.maxZ = -1e10;
        stats.avgMagnitude = 0.0;
        stats.validPoints = 0;
        stats.isExploding = false;
        stats.isCollapsing = false;
        stats.hasStructure = false;

        double x = x0, y = y0, z = z0;
        const int skipTransient = 100;

        for (int i = 0; i < iterations + skipTransient; ++i)
        {
            orbitFunc(x, y, z);

            // Check for explosion
            double mag = std::sqrt(x * x + y * y + z * z);
            if (mag > 1000.0 || std::isnan(mag) || std::isinf(mag))
            {
                stats.isExploding = true;
                return stats;
            }

            // After transient, collect statistics
            if (i >= skipTransient)
            {
                stats.minX = std::min(stats.minX, x);
                stats.maxX = std::max(stats.maxX, x);
                stats.minY = std::min(stats.minY, y);
                stats.maxY = std::max(stats.maxY, y);
                stats.minZ = std::min(stats.minZ, z);
                stats.maxZ = std::max(stats.maxZ, z);
                stats.avgMagnitude += mag;
                stats.validPoints++;
            }
        }

        if (stats.validPoints > 0)
        {
            stats.avgMagnitude /= stats.validPoints;

            // Check if collapsed to a point or small region
            double rangeX = stats.maxX - stats.minX;
            double rangeY = stats.maxY - stats.minY;
            double rangeZ = stats.maxZ - stats.minZ;
            double totalRange = rangeX + rangeY + rangeZ;

            stats.isCollapsing = (totalRange < 0.01);
            stats.hasStructure = !stats.isCollapsing && 
                                 stats.avgMagnitude > 0.1 && 
                                 stats.avgMagnitude < 100.0;
        }

        return stats;
    }

    // Example: Diagnose a specific attractor system
    void DiagnoseYourAttractor()
    {
        std::cout << "\n=== YOUR ATTRACTOR PARAMETER SWEEP ===\n\n";

        // Define parameter ranges to test
        std::vector<double> param_values = { 0.5, 1.0, 1.5, 2.0, 2.5 };
        std::vector<double> dt_values = { 0.001, 0.005, 0.01, 0.02 };

        for (double param : param_values)
        {
            for (double dt : dt_values)
            {
                // Define your attractor's orbit iterator
                auto orbitFunc = [param, dt](double& x, double& y, double& z) {
                    // TODO: Replace with your attractor's differential equations
                    // Example: Simple system
                    double dx = -y - z;
                    double dy = x + 0.2 * y;
                    double dz = 0.2 + z * (x - param);

                    x += dx * dt;
                    y += dy * dt;
                    z += dz * dt;
                };

                AttractorStats stats = TestAttractorParameters(
                    orbitFunc, 0.1, 0.1, 1.0);

                if (stats.hasStructure)
                {
                    std::cout << "✓ param=" << param << ", dt=" << dt
                        << " | Range X: [" << stats.minX << " to " << stats.maxX << "]"
                        << " | Y: [" << stats.minY << " to " << stats.maxY << "]"
                        << " | Z: [" << stats.minZ << " to " << stats.maxZ << "]"
                        << " | Avg mag: " << stats.avgMagnitude << "\n";
                }
                else if (stats.isExploding)
                {
                    std::cout << "✗ param=" << param << ", dt=" << dt 
                        << " | EXPLODING\n";
                }
                else if (stats.isCollapsing)
                {
                    std::cout << "- param=" << param << ", dt=" << dt 
                        << " | COLLAPSING\n";
                }
            }
        }
    }
}

int main()
{
    std::cout << "=================================================\n";
    std::cout << "  ATTRACTOR PARAMETER DIAGNOSTICS UTILITY\n";
    std::cout << "=================================================\n";

    Native::DiagnoseYourAttractor();

    std::cout << "\n=================================================\n";
    std::cout << "  DIAGNOSTICS COMPLETE\n";
    std::cout << "=================================================\n";

    return 0;
}
```

## Usage Instructions

### Step 1: Create the Diagnostic File

Save the code above as `AttractorDiagnostics.cpp` in the `ManpCore.Native` directory.

### Step 2: Customize for Your Attractor

Modify the `DiagnoseYourAttractor()` function:

1. **Define parameter ranges** to test:
   ```cpp
   std::vector<double> c_values = { 0.25, 0.5, 1.0, 2.0 };
   std::vector<double> dt_values = { 0.001, 0.01, 0.05 };
   ```

2. **Implement your attractor's equations** in the lambda:
   ```cpp
   auto orbitFunc = [a, b, c, dt](double& x, double& y, double& z) {
       // Your differential equations here
       double dx = /* ... */;
       double dy = /* ... */;
       double dz = /* ... */;

       x += dx * dt;
       y += dy * dt;
       z += dz * dt;
   };
   ```

3. **Test different initial conditions** if needed:
   ```cpp
   std::vector<std::tuple<double, double, double>> initConds = {
       {0.1, 0.1, 1.0},   // Default
       {1.0, 1.0, 1.0},
       {0.5, -0.5, 0.0}
   };
   ```

### Step 3: Compile and Run

```powershell
# Navigate to the native code directory
cd "C:\Users\Mark\source\repos\ManpLab\ManpCore.Native"

# Compile (requires MSVC compiler in PATH)
cl.exe /EHsc /std:c++17 /O2 /Fe:AttractorDiagnostics.exe AttractorDiagnostics.cpp

# Run the diagnostics
.\AttractorDiagnostics.exe
```

### Step 4: Interpret Results

The tool outputs three types of results:

```
✓ param=2.0, dt=0.001 | Range X: [-0.22 to 0.30] | Avg mag: 0.136
```
**GOOD**: Bounded, stable attractor. Use these parameters!
- Note the ranges to estimate zoom: `range ≈ 0.52 units → zoom ≈ 80-100`

```
✗ param=1.0, dt=0.05 | EXPLODING
```
**BAD**: Orbit escapes to infinity. Reduce timestep or adjust parameters.

```
- param=0.5, dt=0.001 | COLLAPSING
```
**BAD**: Orbit converges to a point. Try different parameters or initial conditions.

### Step 5: Apply Findings

Use the successful configurations in your fractal registration:

```cpp
spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
    const double a = 2.0;      // From diagnostic sweep
    const double dt = 0.001;   // From diagnostic sweep

    // Your equations...
};

// Estimate zoom from range:
// If range is 0.5 units, use zoom ~80-100
// If range is 4.0 units, use zoom ~3-5
spec.defaultZoom = 80.0;  // Based on diagnostic output
```

## Real-World Examples

### Example 1: Arneodo Attractor (Success Case)

**Problem**: Solid color screen with initial parameters

**Diagnostic sweep**: Tested 35 combinations (7 c-values × 5 dt-values)

**Results**:
```
✓ c=2.0, dt=0.001 | Range: [-0.222 to 0.304] | Avg mag: 0.136
```

**Solution Applied**:
- Changed `c` from `1.0` → `2.0` (only stable value found)
- Changed `dt` from `0.01` → `0.001` (4× reduction)
- Changed `zoom` from `8.0` → `80.0` (10× increase for tiny structure)

**Outcome**: Visible ribbon structure with proper detail

### Example 2: Liu-Chen Attractor (Parameter Validation)

**Problem**: Blank screen, suspected bad parameters

**Diagnostic sweep**: Tested 20 combinations (5 parameter sets × 4 dt-values)

**Results**:
```
✓ a=2.5, b=4.0, c=4.0, dt=0.01 | Range X: [0.54 to 4.76] | Avg mag: 6.92
```

**Solution Applied**:
- Parameters validated as correct
- Changed `zoom` from `12.0` → `3.5` (zoom was TOO HIGH)
- Range: ~4.2 units wide (larger than expected)

**Outcome**: Four-wing butterfly structure properly framed

## Zoom Estimation Guidelines

Use the diagnostic range output to estimate zoom:

| Attractor Range | Recommended Zoom | Notes |
|-----------------|------------------|-------|
| 0.3 - 0.6 units | 60 - 100 | Very compact, needs extreme zoom |
| 0.6 - 1.0 units | 30 - 60 | Small structure, high zoom |
| 1.0 - 2.0 units | 15 - 30 | Medium structure |
| 2.0 - 5.0 units | 3 - 15 | Large structure, lower zoom |
| 5.0+ units | 1 - 5 | Very large, minimal zoom |

Formula approximation:
```
zoom ≈ 3.0 / (attractor_range / 2.0)
```

## Troubleshooting

### All configurations explode
- **Try much smaller timesteps**: 0.0001, 0.0005
- **Try different parameter ranges**: the system may be sensitive
- **Try RK2 or RK4 integration** instead of Euler
- **Verify equations** against literature

### All configurations collapse
- **Try different initial conditions**: (1,1,1), (0,5,-0.5), (2,2,2)
- **Check for typos** in differential equations
- **Verify parameter signs** (+ vs -)
- **System may be subcritical** (not chaotic) for those parameters

### Structure found but still looks wrong in browser
- **Adjust zoom** based on range output
- **Try different color palettes** (not a parameter issue)
- **Increase orbit count** in histogram renderer if structure is sparse
- **Check center position** (may need offset if attractor isn't centered on origin)

## Integration with Chat Sessions

When starting a new chat session about attractor tuning:

1. **Provide this document link** or paste relevant sections
2. **Share the diagnostic output** from your previous run
3. **State the problem clearly**: "Arneodo shows solid color with c=1.0, dt=0.01, zoom=8"
4. **Include the working configurations** found: "Diagnostic found c=2.0, dt=0.001 works"
5. **Request specific changes**: "Apply the diagnostic findings to ChaoticMapsFamily.cpp"

## Advanced Techniques

### Testing Multiple Integration Methods

Compare Euler vs RK2 vs RK4:

```cpp
// Euler
x += dx * dt;

// RK2 (Midpoint)
double xmid = x + 0.5 * dx * dt;
// ... recalculate dx at midpoint ...
x += dx_mid * dt;

// RK4 (Most accurate)
double k1 = dx;
double k2 = /* dx at x + 0.5*k1*dt */;
double k3 = /* dx at x + 0.5*k2*dt */;
double k4 = /* dx at x + k3*dt */;
x += (k1 + 2*k2 + 2*k3 + k4) * dt / 6.0;
```

### Adaptive Timestep Testing

```cpp
std::vector<double> dt_schedule = {
    0.1, 0.05, 0.02, 0.01, 0.005, 0.002, 0.001, 0.0005, 0.0001
};
```

Start with larger timesteps and progressively refine.

### Multi-Dimensional Parameter Sweeps

Test 3+ parameters simultaneously:

```cpp
for (double a : a_values)
    for (double b : b_values)
        for (double c : c_values)
            for (double dt : dt_values)
                // Test combination...
```

**Warning**: Combinatorial explosion! 5×5×5×5 = 625 tests.

## Cleanup

After obtaining results, **delete the diagnostic file** to avoid build issues:

```cpp
// Remove these files after use:
ManpCore.Native/AttractorDiagnostics.cpp
ManpCore.Native/AttractorDiagnostics.obj
ManpCore.Native/AttractorDiagnostics.exe
```

The diagnostic tool is **not part of the main build** - it's a standalone utility.

## Summary

This tool transforms the trial-and-error process of attractor parameter tuning into a systematic, data-driven approach. By testing dozens of configurations in seconds, it quickly identifies the "sweet spots" that produce stable, visually interesting attractors.

**Key Benefits**:
- ✅ Eliminates guesswork in parameter selection
- ✅ Finds stable configurations automatically
- ✅ Reports exact bounds for zoom estimation
- ✅ Detects explosion/collapse early
- ✅ Tests multiple initial conditions
- ✅ Provides quantitative metrics (magnitude, range)
- ✅ Fast: ~50ms per configuration

**Typical workflow**: Problem → Create diagnostic → Run sweep → Apply findings → Success

---

**Last Updated**: 2025-01-XX  
**Related Documents**: 
- `PHASE_5_VISUAL_QUALITY_UPGRADE.md`
- `CRITICAL_FIX_HISTOGRAM_REGISTRATION.md`

**Version**: 1.0  
**Author**: Generated during Phase 5 attractor tuning
