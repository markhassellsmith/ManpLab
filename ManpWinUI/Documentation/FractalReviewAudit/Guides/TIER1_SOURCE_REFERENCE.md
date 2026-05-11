# Tier 1 Fractals - Source Code Reference

## Quick Reference Guide

This document maps each Tier 1 critical fractal to its C++ implementation file in `ManpCore.Native\`.

**Use this when:** You need to verify the actual formula implementation or debug rendering issues.

---

## Source File Mappings

### Classic Mandelbrot Family

| # | Fractal Name | Implementation File |
|---|--------------|---------------------|
| 1 | Mandelbrot | `MandelbrotFamily.cpp` |
| 2 | Julia | `MandelbrotFamily.cpp` (Julia Sets section) |
| 3 | Burning Ship | `BurningShipFamily.cpp` |

### Newton Fractals

| # | Fractal Name | Implementation File |
|---|--------------|---------------------|
| 4 | Newton (z³-1) | `NewtonFamily.cpp` |
| 5 | Newton (z⁴-1) | `NewtonFamily.cpp` |

### Phoenix & Magnet

| # | Fractal Name | Implementation File |
|---|--------------|---------------------|
| 6 | Phoenix | `PhoenixFamily.cpp` |
| 7 | Magnet 1 | `MagnetFamily.cpp` |
| 8 | Magnet 2 | `MagnetFamily.cpp` |

### Lambda Fractals

| # | Fractal Name | Implementation File |
|---|--------------|---------------------|
| 9 | Lambda (Mandelbrot) | `ClassicEscapeTimeFamily.cpp` |

### Barnsley Fractals

| # | Fractal Name | Implementation File |
|---|--------------|---------------------|
| 10 | Barnsley M1 | `BarnsleyFamily.cpp` |

### 3D Attractors

| # | Fractal Name | Implementation File |
|---|--------------|---------------------|
| 11 | Lorenz | `Attractors3DFamily.cpp` |
| 12 | Rössler | `Attractors3DFamily.cpp` |

### Trigonometric

| # | Fractal Name | Implementation File |
|---|--------------|---------------------|
| 13 | MandelSin | `TrigonometricFamily.cpp` |
| 14 | MandelCos | `TrigonometricFamily.cpp` |

### Higher Powers

| # | Fractal Name | Implementation File |
|---|--------------|---------------------|
| 15 | Mandelbrot^3 | `MultibrotFamily.cpp` |
| 16 | Mandelbrot^4 | `MultibrotFamily.cpp` |

### Spider Fractals

| # | Fractal Name | Implementation File |
|---|--------------|---------------------|
| 17 | Spider | Look in: `MandelVariantsFamily.cpp` or dedicated Spider file |

### Popcorn

| # | Fractal Name | Implementation File |
|---|--------------|---------------------|
| 18 | Popcorn | Look in: `ChaoticMapsFamily.cpp` or dedicated Popcorn file |

### Orbital Fractals

| # | Fractal Name | Implementation File |
|---|--------------|---------------------|
| 19 | Mandelbar (Tricorn) | `TricornFamily.cpp` or `OrbitalFractalsFamily.cpp` |
| 20 | Perpendicular Mandelbrot | `OrbitalFractalsFamily.cpp` or `OrbitalModificationsFamily.cpp` |

### Historical Fractals

| # | Fractal Name | Implementation File |
|---|--------------|---------------------|
| 21 | Manowar | `HistoricalFractalsFamily.cpp` |

### Complex Dynamics

| # | Fractal Name | Implementation File |
|---|--------------|---------------------|
| 22 | Multibrot (z^2.5) | `MultibrotFamily.cpp` or `PowerVariantsFamily.cpp` |

### Convergent

| # | Fractal Name | Implementation File |
|---|--------------|---------------------|
| 23 | Newton-Raphson General | `NewtonFamily.cpp` or `NewtonExtendedFamily.cpp` |

### Experimental

| # | Fractal Name | Implementation File |
|---|--------------|---------------------|
| 24 | Buffalo | `ExoticFormulasFamily.cpp` or `MandelVariantsFamily.cpp` |

### Quartic & Cubic

| # | Fractal Name | Implementation File |
|---|--------------|---------------------|
| 25 | Quartic Mandelbrot | `MultibrotFamily.cpp` (power 4) |
| 26 | Cubic Mandelbrot | `MultibrotFamily.cpp` (power 3) |

### Additional Critical

| # | Fractal Name | Implementation File |
|---|--------------|---------------------|
| 27 | Tetration | `ExponentialFamily.cpp` or `ExponentialLogarithmicFamily.cpp` |
| 28 | Feather | `ExoticFormulasFamily.cpp` or `SpecialExoticFamily.cpp` |
| 29 | Celtic Mandelbrot | `MandelVariantsFamily.cpp` |
| 30 | Heart Mandelbrot | `SpecialExoticFamily.cpp` or `ExoticFormulasFamily.cpp` |

---

## All Available Family Files

Complete list of C++ fractal family implementation files in `ManpCore.Native\`:

- `Attractors3DFamily.cpp`
- `BarnsleyFamily.cpp`
- `BifurcationFamily.cpp`
- `BurningShipFamily.cpp`
- `ChaoticMapsFamily.cpp`
- `ClassicEscapeTimeFamily.cpp`
- `ComplexFunctionsFamily.cpp`
- `DistanceEstimatorFamily.cpp`
- `EnhancedJuliaPresetsFamily.cpp`
- `ExoticFormulasFamily.cpp`
- `ExponentialFamily.cpp`
- `ExponentialLogarithmicFamily.cpp`
- `ExtendedJuliaFamily.cpp`
- `FractalHybridsFamily.cpp`
- `HistoricalFractalsFamily.cpp`
- `HybridFamily.cpp`
- `IFSFamily.cpp`
- `JuliaVariantsFamily.cpp`
- `LambdaExtendedFamily.cpp`
- `MagnetExtendedFamily.cpp`
- `MagnetFamily.cpp`
- `MandelbrotFamily.cpp`
- `MandelVariantsFamily.cpp`
- `MultibrotFamily.cpp`
- `NewtonExtendedFamily.cpp`
- `NewtonFamily.cpp`
- `OrbitalFractalsFamily.cpp`
- `OrbitalModificationsFamily.cpp`
- `PhoenixExtendedFamily.cpp`
- `PhoenixFamily.cpp`
- `PolynomialFamily.cpp`
- `PolynomialVariantsFamily.cpp`
- `PowerVariantsFamily.cpp`
- `RationalFunctionFamily.cpp`
- `SpecialExoticFamily.cpp`
- `SpecialFunctionFamily.cpp`
- `StrangeAttractorsExtendedFamily.cpp`
- `TricornFamily.cpp`
- `TrigonometricExtendedFamily.cpp`
- `TrigonometricFamily.cpp`

---

## How to Access C++ Implementation Files (Safe Methods)

### Method 1: File Explorer → Open Directly (RECOMMENDED - Safest)
1. **Open File Explorer** to your workspace:
   ```
   C:\Users\Mark\source\repos\ManpLab\ManpCore.Native\
   ```
2. **Locate the file** using the table above (e.g., `MandelbrotFamily.cpp`)
3. **Double-click** the `.cpp` file - it opens in Visual Studio
4. **Safe:** File opens for viewing only, no changes to your solution/project

**Why this is best:** You're just reading files. No solution modifications. No project changes.

### Method 2: Visual Studio → Open File
1. In Visual Studio, press **Ctrl+O** (File → Open → File)
2. Navigate to: `C:\Users\Mark\source\repos\ManpLab\ManpCore.Native\`
3. Select the `.cpp` file you need (e.g., `NewtonFamily.cpp`)
4. Click **Open**

**Safe:** Opens file without adding it to Solution Explorer or modifying projects.

### Method 3: Quick Open (Ctrl+T)
1. Press **Ctrl+T** in Visual Studio
2. Type the filename: `MandelbrotFamily.cpp`
3. Select it from the list
4. File opens temporarily

### Method 4: Search Within Files (For Finding Code)
**Note:** This searches *content inside files*, not filenames.

1. Press **Ctrl+Shift+F** (Find in Files)
2. Type what to search for: `displayName = "Mandelbrot"`
3. Set "Look in" to: `ManpCore.Native` folder
4. **Results show where that text appears** in files
5. Double-click result to open that file at that line

**Use this when:** You need to find *which file* contains a specific fractal or formula.

---

## Formula Structure in Code

Each fractal is registered using this pattern:

```cpp
spec.name = "FractalName";
spec.displayName = "Display Name";
spec.category = "Category";
spec.description = "Description text";

spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
    // FORMULA IMPLEMENTATION HERE
    // The actual iteration loop with:
    // - z initialization
    // - Iteration formula (e.g., z = z² + c)
    // - Bailout condition
    // - Smooth iteration count return
};

FractalRegistry::Register(spec);
```

The **formula implementation** is inside the `spec.calculator` lambda function.

---

## Quick Tips

- **Most fractals** are in files named after their family (e.g., Phoenix → PhoenixFamily.cpp)
- **Mandelbrot variants** may be in `MandelbrotFamily.cpp` or `MandelVariantsFamily.cpp`
- **Power variations** (z³, z⁴) are typically in `MultibrotFamily.cpp`
- **Experimental fractals** are scattered across `ExoticFormulasFamily.cpp`, `SpecialExoticFamily.cpp`
- **Extended versions** have their own files (e.g., `NewtonExtendedFamily.cpp`)

---

## Related Documentation

- **Main Audit Checklist:** `TIER1_CRITICAL_FRACTALS.md`
- **Quick Start Guide:** `Guides\QUICK_START.md`
- **Cheat Sheet:** `Guides\CHEAT_SHEET.md`

---

*Last Updated: 2025-01-20*
