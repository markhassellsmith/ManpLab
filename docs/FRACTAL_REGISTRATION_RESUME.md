# Fractal Registration Resume Point

**Date:** 2025-01-XX  
**Branch:** `feature/fractal-type-expansion`  
**Status:** Ready for final registration step

---

## ✅ Completed Work

### All Precision Implementations Done
Successfully implemented 22 new fractal types across **all four precision layers**:

#### 1. Enum Definitions (`ManpWIN64\Fractype.h`) ✅
Added the following fractal type constants:
```cpp
MULTIBROT3 = 313,
MULTIBROT4 = 314,
MULTIBROT5 = 315,
MULTIBROT6 = 316,
EXPFRACTAL2 = 317,
LOGFRACTAL = 318,
EXPLOGFRACTAL = 319,
SINFRACTAL2 = 320,
COSFRACTAL = 321,
TANFRACTAL = 322,
SINHFRACTAL = 323,
COSHFRACTAL = 324,
TANHFRACTAL = 325,
RATIONALFRACTAL1 = 326,
RATIONALFRACTAL2 = 327,
CELTIC = 328,
MANDELBARCELTIC = 329,
PERPCELTIC = 330,
CUBICFLYINGSQUIRREL = 331,
MULTIBROT3JULIA = 332,
MULTIBROT4JULIA = 333,
MULTIBROT5JULIA = 334
```

#### 2. BigNum/MPFR Precision (`ManpWIN64\BigFunctions.cpp`) ✅
- Implemented `BigInitFunctions()` cases for all 22 fractals
- Implemented `BigRunFunctions()` iteration logic for all 22 fractals
- Uses `BigComplex` and `BigDouble` types with MPFR library

#### 3. Standard Double Precision (`ManpWIN64\FractintFunctions.cpp`) ✅
- Implemented `InitFractintFunctions()` cases for all 22 fractals
- Implemented `RunFractintFunctions()` iteration logic for all 22 fractals
- Uses standard `Complex` type (double precision)

#### 4. Double-Double Precision (`ManpWIN64\DDFractintFunctions.cpp`) ✅
- Implemented `DDInitFractintFunctions()` cases for all 22 fractals
- Implemented `DDRunFractintFunctions()` iteration logic for all 22 fractals
- Uses `DDComplex` type (double-double precision)

#### 5. Quad-Double Precision (`ManpWIN64\QDFractintFunctions.cpp`) ✅
- Implemented `QDInitFractintFunctions()` cases for all 22 fractals
- Implemented `QDRunFractintFunctions()` iteration logic for all 22 fractals
- Uses `QDComplex` type (quad-double precision)

#### 6. Build Status ✅
- **Last Build:** Successful
- **Date:** 2025-01-XX at 10:30 PM
- **Duration:** 6.207 seconds
- **Status:** 1 succeeded, 0 failed, 6 up-to-date, 0 skipped

---

## 🔄 Next Step: Fractal Registration

### What Needs to Be Done
Register all 22 new fractals in `ManpWIN64\fractalp.cpp` to make them selectable in the UI.

### File to Modify
**`ManpWIN64\fractalp.cpp`** - The fractal specification table

### Registration Structure
Each fractal needs an entry in the `fractalspecific[]` array with the following structure:

```cpp
{
   "Fractal Display Name",
   "Parameter 1 description", "Parameter 2 description", ES, ES, ES, ES, ES, ES, ES, ES,
   param1_default, param2_default, 0.0, 0.0, 0.0, 0.0, 0, 0, 0, 0,
   corner_x, corner_y, size, flags, julia_type, mandel_type, num_params, trig_index,
   NULL, NULL, function_flags, symmetry_flag, bailout,
   NULL, NULL, "SelectFracParams", SelectFracParams, bailout_type
},
```

### Fractals to Register (22 Total)

#### Polynomial/Multibrot Family (7 fractals)
1. **MULTIBROT3** - z³ + c Mandelbrot variant
   - Julia companion: MULTIBROT3JULIA
   - Default params: realz0=0.0, imagz0=0.0
   - Suggested range: -3.5, -2.0, 4.0

2. **MULTIBROT4** - z⁴ + c Mandelbrot variant
   - Julia companion: MULTIBROT4JULIA
   - Default params: realz0=0.0, imagz0=0.0
   - Suggested range: -3.5, -2.0, 4.0

3. **MULTIBROT5** - z⁵ + c Mandelbrot variant
   - Julia companion: MULTIBROT5JULIA
   - Default params: realz0=0.0, imagz0=0.0
   - Suggested range: -3.5, -2.0, 4.0

4. **MULTIBROT6** - z⁶ + c Mandelbrot variant
   - Default params: realz0=0.0, imagz0=0.0
   - Suggested range: -3.5, -2.0, 4.0

#### Exponential/Logarithmic Family (3 fractals)
5. **EXPFRACTAL2** - exp(z) + c
   - Formula: z(n+1) = exp(z(n)) + c
   - Default params: realz0=0.0, imagz0=0.0
   - Suggested range: -4.0, -3.0, 6.0

6. **LOGFRACTAL** - log(z) + c
   - Formula: z(n+1) = log(z(n)) + c
   - Default params: realz0=0.0, imagz0=0.0
   - Suggested range: -4.0, -3.0, 6.0

7. **EXPLOGFRACTAL** - exp(log(z)) + c
   - Formula: z(n+1) = exp(log(z(n))) + c (essentially z + c but interesting)
   - Default params: realz0=0.0, imagz0=0.0
   - Suggested range: -4.0, -3.0, 6.0

#### Transcendental Function Family (6 fractals)
8. **SINFRACTAL2** - sin(z) + c
   - Formula: z(n+1) = sin(z(n)) + c
   - Default params: realz0=0.0, imagz0=0.0
   - Suggested range: -8.0, -6.0, 12.0

9. **COSFRACTAL** - cos(z) + c
   - Formula: z(n+1) = cos(z(n)) + c
   - Default params: realz0=0.0, imagz0=0.0
   - Suggested range: -8.0, -6.0, 12.0

10. **TANFRACTAL** - tan(z) + c
    - Formula: z(n+1) = tan(z(n)) + c
    - Default params: realz0=0.0, imagz0=0.0
    - Suggested range: -8.0, -6.0, 12.0

11. **SINHFRACTAL** - sinh(z) + c
    - Formula: z(n+1) = sinh(z(n)) + c
    - Default params: realz0=0.0, imagz0=0.0
    - Suggested range: -8.0, -6.0, 12.0

12. **COSHFRACTAL** - cosh(z) + c
    - Formula: z(n+1) = cosh(z(n)) + c
    - Default params: realz0=0.0, imagz0=0.0
    - Suggested range: -8.0, -6.0, 12.0

13. **TANHFRACTAL** - tanh(z) + c
    - Formula: z(n+1) = tanh(z(n)) + c
    - Default params: realz0=0.0, imagz0=0.0
    - Suggested range: -8.0, -6.0, 12.0

#### Rational Function Family (2 fractals)
14. **RATIONALFRACTAL1** - (z² + c) / (z² - c)
    - Formula: z(n+1) = (z(n)² + c) / (z(n)² - c)
    - Default params: realz0=0.0, imagz0=0.0
    - Suggested range: -3.5, -2.0, 4.0
    - **Note:** Division by zero guard implemented (threshold 1e-10)

15. **RATIONALFRACTAL2** - (z³ + c) / (z + c)
    - Formula: z(n+1) = (z(n)³ + c) / (z(n) + c)
    - Default params: realz0=0.0, imagz0=0.0
    - Suggested range: -3.5, -2.0, 4.0
    - **Note:** Division by zero guard implemented (threshold 1e-10)

#### Celtic/Variant Family (4 fractals)
16. **CELTIC** - Celtic Mandelbrot
    - Formula: z(n+1) = (abs(real(z²)) + i*imag(z²)) + c
    - Default params: realz0=0.0, imagz0=0.0
    - Suggested range: -3.5, -2.0, 4.0

17. **MANDELBARCELTIC** - Mandelbar Celtic
    - Formula: z(n+1) = (abs(real(z²)) - i*imag(z²)) + c
    - Default params: realz0=0.0, imagz0=0.0
    - Suggested range: -3.5, -2.0, 4.0

18. **PERPCELTIC** - Perpendicular Celtic
    - Formula: z(n+1) = (real(z²) + i*abs(imag(z²))) + c
    - Default params: realz0=0.0, imagz0=0.0
    - Suggested range: -3.5, -2.0, 4.0

19. **CUBICFLYINGSQUIRREL** - Cubic Flying Squirrel
    - Formula: z(n+1) = z³ + (c-1)*z - c
    - Default params: realz0=0.0, imagz0=0.0
    - Suggested range: -3.5, -2.0, 4.0

---

## 📋 Registration Template

Use this template for each fractal entry in `fractalp.cpp`:

```cpp
{
   "Multibrot z^3",  // Display name
   realz0, imagz0, ES, ES, ES, ES, ES, ES, ES, ES,  // Parameter descriptions
   0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0, 0, 0, 0,  // Default parameter values
   -3.5, -2.0, 4.0,  // corner_x, corner_y, size (viewport defaults)
   0,  // flags (0 = escaping, CONVERGING for Newton types)
   MULTIBROT3JULIA,  // Julia companion type (or NOFRACTAL if none)
   NOFRACTAL,  // Mandelbrot companion (usually NOFRACTAL for Mandelbrot types)
   2,  // number of parameters used
   0,  // trig_index (0 if no trig functions)
   NULL, NULL,  // reserved function pointers
   FUNCTIONINPIXEL + USEDOUBLEDOUBLE,  // function_flags
   XAXIS_NOPARM,  // symmetry flag
   4.0,  // bailout value
   NULL, NULL,  // more function pointers
   "SelectFracParams", SelectFracParams,  // parameter selection function
   STDBAILOUT  // bailout type
},
```

---

## 🔧 Implementation Steps

### Step 1: Locate Insertion Point
Find the end of the existing `fractalspecific[]` array in `ManpWIN64\fractalp.cpp`.
Look for a location after the last existing fractal entry, before the closing brace.

### Step 2: Add Entries
Add all 22 fractal entries using the template above. Suggested order:
1. Multibrot family (MULTIBROT3, MULTIBROT3JULIA, MULTIBROT4, MULTIBROT4JULIA, etc.)
2. Exponential/logarithmic (EXPFRACTAL2, LOGFRACTAL, EXPLOGFRACTAL)
3. Transcendental (SINFRACTAL2, COSFRACTAL, etc.)
4. Rational (RATIONALFRACTAL1, RATIONALFRACTAL2)
5. Celtic variants (CELTIC, MANDELBARCELTIC, etc.)

### Step 3: Parameter Descriptions
Use existing parameter description strings:
- `realz0` = "Real Perturbation of Z(0)"
- `imagz0` = "Imaginary Perturbation of Z(0)"
- `ES` = Empty String (for unused parameters)

### Step 4: Flags Reference
Common flag combinations:
- **FUNCTIONINPIXEL** - Uses function-based calculation
- **USEDOUBLEDOUBLE** - Supports double-double precision
- **FRACTINTINPIXEL** - Uses Fractint-style calculation
- **OTHERFNINPIXEL** - Other function type

Symmetry flags:
- **XAXIS_NOPARM** - X-axis symmetry, no parameter dependency
- **XAXIS** - X-axis symmetry
- **NOSYM** - No symmetry

### Step 5: Build and Test
After adding entries:
1. Build the solution to check for syntax errors
2. Run ManpLab and verify new fractals appear in the selection menu
3. Test rendering at least one example from each family
4. Verify parameter controls work correctly

---

## 📝 Notes and Considerations

### Bailout Values
- Standard fractals: `STDBAILOUT` (4.0)
- Transcendental fractals: `LTRIGBAILOUT` (64.0 or higher for trig functions)
- Newton types: `NOBAILOUT` (convergence based)

### Julia/Mandelbrot Pairs
- For Multibrot fractals, pair Mandelbrot with its Julia variant
- Julia types should reference their Mandelbrot parent
- Use `NOFRACTAL` when no companion exists

### Function Flags
All new fractals should include:
- `FUNCTIONINPIXEL` or `FRACTINTINPIXEL` depending on calculation path
- `USEDOUBLEDOUBLE` to enable high-precision modes

### Viewport Defaults
Coordinate suggestions are starting points. Users can adjust these, but good defaults improve first-time experience:
- Standard Mandelbrot range: -3.5, -2.0, 4.0
- Wider transcendental range: -8.0, -6.0, 12.0
- Exponential range: -4.0, -3.0, 6.0

---

## 🚀 After Registration

Once registration is complete, consider:

### UI/Metadata Integration
1. Check if fractal names appear in selection menus
2. Verify parameter dialogs show correct descriptions
3. Test Julia/Mandelbrot switching for paired types

### Documentation
1. Update user documentation with new fractal descriptions
2. Add example parameter sets for interesting views
3. Create sample images for each fractal family

### Testing
1. Verify all precision modes work (standard, DD, QD, BigNum)
2. Test bailout behavior
3. Check for any rendering artifacts
4. Verify zoom/pan operations
5. Test parameter modifications

---

## 📚 Additional Resources

### Related Files
- **Fractype.h** - Fractal type enumeration
- **BigFunctions.cpp** - BigNum implementations
- **FractintFunctions.cpp** - Standard precision implementations
- **DDFractintFunctions.cpp** - Double-double implementations
- **QDFractintFunctions.cpp** - Quad-double implementations
- **fractalp.cpp** - Registration table ⬅️ **NEXT TO EDIT**

### Git Status
- Branch: `feature/fractal-type-expansion`
- Remote: `origin: https://github.com/markhassellsmith/ManpLab`
- Uncommitted changes: Implementation complete, registration pending

### Recommended Commit Message (After Registration)
```
feat: Add 22 new fractal types across all precision layers

- Added Multibrot family (z^3, z^4, z^5, z^6) with Julia variants
- Added exponential/logarithmic family (exp, log, exp-log)
- Added transcendental family (sin, cos, tan, sinh, cosh, tanh)
- Added rational function family (two variants)
- Added Celtic variant family (Celtic, Mandelbar Celtic, Perpendicular Celtic, Cubic Flying Squirrel)

All fractals implemented across BigNum, standard, DD, and QD precision layers.
Registered in fractalp.cpp for UI integration.

Closes #XXX
```

---

## ✅ Pre-Resume Checklist

Before resuming work:
- [ ] Read this entire resume document
- [ ] Review the registration template above
- [ ] Locate `ManpWIN64\fractalp.cpp` in the workspace
- [ ] Have `ManpWIN64\Fractype.h` open for reference (enum values)
- [ ] Ensure branch `feature/fractal-type-expansion` is active
- [ ] Check that all previous builds succeeded

---

**Ready to resume!** The next coding session should focus solely on adding the 22 fractal entries to `fractalp.cpp`.

Good night! 🌙
