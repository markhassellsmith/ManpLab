# Tier 1: Critical Fractals Audit

## Reference Resources

### Primary Formula & Visual References

1. **[Paul Bourke's Fractal Collection](http://paulbourke.net/fractals/)**
   - Comprehensive formulas with mathematical definitions
   - Visual examples for most classic fractals
   - Detailed parameter specifications and implementation notes

2. **[Wikipedia Fractal Category](https://en.wikipedia.org/wiki/List_of_fractals_by_Hausdorff_dimension)**
   - Authoritative mathematical formulas
   - Historical context and discoverers
   - Reference images and algorithm descriptions
   - Individual pages for major fractals (Mandelbrot, Julia, Newton, etc.)

3. **[Tier 1 Source Code Reference](../Guides/TIER1_SOURCE_REFERENCE.md)** *(Local)*
   - Maps each Tier 1 fractal to its C++ implementation file
   - Lists all 40 fractal family files in ManpCore.Native
   - Guide to finding and reading formula implementations

**Quick lookup tip:** For specific fractals, search "[fractal name] Paul Bourke" or "[fractal name] Wikipedia" for fastest access to formulas and visuals.

---

## Overview
**Priority:** HIGH  
**Count:** 30 fractals  
**Target:** Week 1  
**Purpose:** Audit the most important, well-known, and frequently-used fractals

These are the "showcase" fractals that users will encounter first and that represent the core mathematical beauty of fractal geometry.

---

## Quick Audit Workflow

1. **Launch ManpLab** and navigate to the fractal
2. **Visual Check** - Does it look correct and interesting?
3. **Performance** - Note render time
4. **Zoom Test** - Try zooming in to verify detail
5. **Julia Mode** - (If applicable) Test Julia set
6. **Mark Complete** - Check boxes below

**Standard Test:** 1920×1080, 256 iterations

---

## Critical Fractals to Audit

### Classic Mandelbrot Family

#### 1. Mandelbrot
- **Display Name:** Mandelbrot Set
- **Category:** Classic Fractals
- **Formula:** z → z² + c
- **Source:** `ManpCore.Native\ClassicFractalsFamily.cpp`

**Audit Checklist:**
- [x] Formula correct (z² + c)
- [x] Default center shows main body and bulbs
- [x] Performance good (~ 3.5s)
- [x] Julia mode works
- [x] Zooming reveals infinite detail
- [x] Description accurate

**Test Notes:**
- Render time: 3.5s
- Issues found: ___
- Overall: x Pass ☐ Fail ☐ Needs Work

---

#### 2. Julia
- **Display Name:** Julia Set
- **Category:** Julia Sets
- **Formula:** z → z² + c (with fixed c parameter)
- **Source:** `ManpCore.Native\JuliaSetsFamily.cpp`

**Audit Checklist:**
- [x] Formula correct
- [x] Default c value produces interesting Julia set
- [x] Performance good
- [x] Multiple c values tested and work
- [x] Description accurate

**Test Notes:**
- Default c value: ___
- Render time: 2.9s
- Issues found: ___
- Overall: x Pass ☐ Fail ☐ Needs Work

---

#### 3. Burning Ship
- **Display Name:** Burning Ship
- **Category:** Mandelbrot Variants
- **Formula:** z → (|Re(z)| + i|Im(z)|)² + c
- **Source:** `ManpCore.Native\MandelbrotVariantsFamily.cpp`

**Audit Checklist:**
- [ ] Formula uses abs() correctly
- [ ] Shows characteristic "ship" shape
- [x] Performance acceptable
- [x] Julia mode works
- [x] Description accurate

**Test Notes:**
- Render time:27s
- Ship shape visible: ☐ Yes ☐ No
- Issues found: ___
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

### Newton Fractals

#### 4. Newton (z³-1)
- **Display Name:** Newton (z³-1)
- **Category:** Newton Fractals
- **Formula:** Newton's method for z³-1=0
- **Source:** `ManpCore.Native\NewtonFractalsFamily.cpp`

**Audit Checklist:**
- [ ] Shows three basins of attraction
- [ ] Fractal boundaries between basins
- [ ] Performance good
- [ ] Colors map to roots correctly
- [ ] Description accurate

**Test Notes:**
- Three basins visible: ☐ Yes ☐ No
- Render time: ___s
- Issues found: ___
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

#### 5. Newton (z^3-1)
- **Display Name:** Newton (z^3-1)
- **Category:** Newton Fractals
- **Formula:** Newton's method for z⁴-1=0
- **Source:** `ManpCore.Native\NewtonFractalsFamily.cpp`

**Audit Checklist:**
- [x] Shows three basins of attraction
- [x] Fractal boundaries
- [x] Performance good
- [x] Description accurate

**Test Notes:**
- Three basins visible: x Yes ☐ No
- Render time: 3.78s
- Overall: x Pass ☐ Fail ☐ Needs Work

---

### Phoenix & Magnet

#### 6. Phoenix
- **Display Name:** Phoenix
- **Category:** Phoenix Fractals
- **Formula:** z → z² + Re(c) + Im(c)×p
- **Source:** `ManpCore.Native\PhoenixFractalsFamily.cpp`

**Audit Checklist:**
- [ ] Formula correct
- [ ] Shows phoenix characteristic features
- [ ] Julia mode works
- [ ] Performance acceptable
- [ ] Description accurate

**Test Notes:**
- Render time: ___s
- Issues found: ___
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

#### 7. Magnet 1
- **Display Name:** Magnet Type 1
- **Category:** Magnet Fractals
- **Formula:** ((z²+c-1)/(2z+c-2))²
- **Source:** `ManpCore.Native\MagnetFractalsFamily.cpp`

**Audit Checklist:**
- [ ] Formula matches definition
- [ ] Characteristic magnet features
- [ ] No division-by-zero artifacts
- [ ] Performance acceptable
- [ ] Description accurate

**Test Notes:**
- Render time: ___s
- Issues found: ___
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

#### 8. Magnet 2
- **Display Name:** Magnet Type 2
- **Category:** Magnet Fractals
- **Formula:** ((z³+3(c-1)z+c-1)/(3z²+3(c-2)z+c-1))²
- **Source:** `ManpCore.Native\MagnetFractalsFamily.cpp`

**Audit Checklist:**
- [ ] Formula correct
- [ ] More complex than Magnet 1
- [ ] No artifacts
- [ ] Performance acceptable

**Test Notes:**
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

### Lambda Fractals

#### 9. Lambda (Mandelbrot)
- **Display Name:** Lambda (Mandelbrot)
- **Category:** Lambda Fractals
- **Formula:** z → c×z×(1-z)
- **Source:** `ManpCore.Native\LambdaFractalsFamily.cpp`

**Audit Checklist:**
- [ ] Formula correct
- [ ] Shows lambda characteristic shape
- [ ] Julia mode works
- [ ] Performance good

**Test Notes:**
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

### Barnsley Fractals

#### 10. Barnsley M1
- **Display Name:** Barnsley M1
- **Category:** Barnsley Fractals
- **Formula:** z → (z-1)×c if Re(z)>0, else (z+1)×c
- **Source:** `ManpCore.Native\BarnsleyFractalsFamily.cpp`

**Audit Checklist:**
- [ ] Conditional formula correct
- [ ] Characteristic split appearance
- [ ] Julia mode works
- [ ] Performance acceptable

**Test Notes:**
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

### 3D Attractors

#### 11. Lorenz
- **Display Name:** Lorenz Attractor
- **Category:** Attractors
- **Formula:** Lorenz system (2D projection)
- **Source:** `ManpCore.Native\Attractors3DFamily.cpp`

**Audit Checklist:**
- [ ] Shows butterfly shape
- [ ] Chaotic behavior visible
- [ ] 2D projection clear
- [ ] Performance good

**Test Notes:**
- Butterfly visible: ☐ Yes ☐ No
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

#### 12. Rössler
- **Display Name:** Rössler Attractor
- **Category:** Attractors
- **Formula:** Rössler system
- **Source:** `ManpCore.Native\Attractors3DFamily.cpp`

**Audit Checklist:**
- [ ] Characteristic spiral shape
- [ ] Chaotic attractor visible
- [ ] Performance acceptable

**Test Notes:**
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

### Trigonometric

#### 13. MandelSin
- **Display Name:** Mandel Sin
- **Category:** Trigonometric
- **Formula:** z → sin(z) + c
- **Source:** `ManpCore.Native\TrigonometricFamily.cpp`

**Audit Checklist:**
- [ ] Formula correct
- [ ] Periodic structure visible
- [ ] Julia mode works
- [ ] Performance acceptable

**Test Notes:**
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

#### 14. MandelCos
- **Display Name:** Mandel Cos
- **Category:** Trigonometric
- **Formula:** z → cos(z) + c
- **Source:** `ManpCore.Native\TrigonometricFamily.cpp`

**Audit Checklist:**
- [ ] Formula correct
- [ ] Periodic structure
- [ ] Julia mode works
- [ ] Performance acceptable

**Test Notes:**
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

### Higher Powers

#### 15. Mandelbrot^3
- **Display Name:** Mandelbrot (Power 3)
- **Category:** Power Fractals
- **Formula:** z → z³ + c
- **Source:** `ManpCore.Native\PowerFractalsFamily.cpp`

**Audit Checklist:**
- [ ] Formula uses z³
- [ ] Threefold symmetry visible
- [ ] Julia mode works
- [ ] Performance good

**Test Notes:**
- Symmetry: ☐ 3-fold ☐ Other
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

#### 16. Mandelbrot^4
- **Display Name:** Mandelbrot (Power 4)
- **Category:** Power Fractals
- **Formula:** z → z⁴ + c
- **Source:** `ManpCore.Native\PowerFractalsFamily.cpp`

**Audit Checklist:**
- [ ] Formula uses z⁴
- [ ] Fourfold symmetry
- [ ] Julia mode works
- [ ] Performance acceptable

**Test Notes:**
- Symmetry: ☐ 4-fold ☐ Other
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

### Spider Fractals

#### 17. Spider
- **Display Name:** Spider
- **Category:** Spider Fractals
- **Formula:** z → z² + c, c → c/2 + z
- **Source:** `ManpCore.Native\SpiderFractalsFamily.cpp`

**Audit Checklist:**
- [ ] Two-part iteration correct
- [ ] Characteristic spider features
- [ ] Julia mode works
- [ ] Performance acceptable

**Test Notes:**
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

### Popcorn

#### 18. Popcorn
- **Display Name:** Popcorn
- **Category:** Popcorn Fractals
- **Formula:** Non-escape time fractal with chaotic mapping
- **Source:** `ManpCore.Native\PopcornFractalsFamily.cpp`

**Audit Checklist:**
- [ ] Formula correct
- [ ] Characteristic popcorn texture
- [ ] Performance acceptable
- [ ] No artifacts

**Test Notes:**
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

### Orbital Fractals (Sample)

#### 19. Mandelbar
- **Display Name:** Mandelbar (Tricorn)
- **Category:** Orbital Fractals
- **Formula:** z → conj(z)² + c
- **Source:** `ManpCore.Native\OrbitalFractalsFamily.cpp`

**Audit Checklist:**
- [ ] Uses conjugate correctly
- [ ] Characteristic tricorn shape
- [ ] Julia mode works
- [ ] Performance good

**Test Notes:**
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

#### 20. Perpendicular Mandelbrot
- **Display Name:** Perpendicular Mandelbrot
- **Category:** Orbital Fractals
- **Formula:** z → (Re(z)² - Im(z)²) + i(2|Re(z)×Im(z)|) + c
- **Source:** `ManpCore.Native\OrbitalFractalsFamily.cpp`

**Audit Checklist:**
- [ ] Perpendicular calculation correct
- [ ] Interesting variation on Mandelbrot
- [ ] Julia mode works
- [ ] Performance acceptable

**Test Notes:**
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

### Historical Fractals

#### 21. Manowar
- **Display Name:** Manowar
- **Category:** Historical Fractals
- **Formula:** z → z² + z_prev + c
- **Source:** `ManpCore.Native\HistoricalFractalsFamily.cpp`

**Audit Checklist:**
- [ ] Uses previous z value
- [ ] Characteristic features
- [ ] Julia mode works
- [ ] Performance acceptable

**Test Notes:**
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

### Complex Dynamics

#### 22. Multibrot (z^2.5)
- **Display Name:** Multibrot (Power 2.5)
- **Category:** Complex Dynamics
- **Formula:** z → z^2.5 + c
- **Source:** `ManpCore.Native\ComplexDynamicsFamily.cpp`

**Audit Checklist:**
- [ ] Non-integer power calculated correctly
- [ ] Interesting asymmetric shape
- [ ] Julia mode works
- [ ] Performance acceptable

**Test Notes:**
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

### Convergent

#### 23. Newton-Raphson General
- **Display Name:** Newton-Raphson
- **Category:** Convergent
- **Formula:** z → z - f(z)/f'(z)
- **Source:** `ManpCore.Native\ConvergentFamily.cpp`

**Audit Checklist:**
- [ ] Newton iteration correct
- [ ] Shows basins of attraction
- [ ] Performance acceptable
- [ ] Configurable polynomial

**Test Notes:**
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

### Experimental (Sample High-Value)

#### 24. Buffalo
- **Display Name:** Buffalo
- **Category:** Experimental
- **Formula:** z → |z² + c| (component-wise abs)
- **Source:** `ManpCore.Native\ExperimentalFamily.cpp`

**Audit Checklist:**
- [ ] Component-wise absolute value correct
- [ ] Interesting fractal structure
- [ ] Performance acceptable

**Test Notes:**
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

### Quartic

#### 25. Quartic Mandelbrot
- **Display Name:** Quartic Mandelbrot
- **Category:** Quartic Fractals
- **Formula:** z → z⁴ + c
- **Source:** `ManpCore.Native\QuarticFractalsFamily.cpp`

**Audit Checklist:**
- [ ] z⁴ formula correct
- [ ] Fourfold rotational symmetry
- [ ] Julia mode works
- [ ] Performance acceptable

**Test Notes:**
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

### Cubic

#### 26. Cubic Mandelbrot
- **Display Name:** Cubic Mandelbrot
- **Category:** Cubic Fractals
- **Formula:** z → z³ + c
- **Source:** `ManpCore.Native\CubicFractalsFamily.cpp`

**Audit Checklist:**
- [ ] z³ formula correct
- [ ] Threefold symmetry
- [ ] Julia mode works
- [ ] Performance good

**Test Notes:**
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

### Additional Critical (Top User Favorites)

#### 27. Tetration
- **Display Name:** Tetration
- **Category:** Power Fractals
- **Formula:** z → c^z
- **Source:** Check appropriate family file

**Audit Checklist:**
- [ ] Exponentiation correct
- [ ] Interesting dynamics
- [ ] Performance acceptable
- [ ] No numerical overflow issues

**Test Notes:**
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

#### 28. Feather
- **Display Name:** Feather
- **Category:** Experimental
- **Formula:** Custom feather-like transformation
- **Source:** Check experimental family

**Audit Checklist:**
- [ ] Formula implemented correctly
- [ ] Feather-like appearance
- [ ] Performance acceptable

**Test Notes:**
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

#### 29. Celtic Mandelbrot
- **Display Name:** Celtic Mandelbrot
- **Category:** Mandelbrot Variants
- **Formula:** z → (|Re(z²)| + i×Im(z²)) + c
- **Source:** `ManpCore.Native\MandelbrotVariantsFamily.cpp`

**Audit Checklist:**
- [ ] Real part absolute value correct
- [ ] Celtic knot-like features
- [ ] Julia mode works
- [ ] Performance acceptable

**Test Notes:**
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

#### 30. Heart Mandelbrot
- **Display Name:** Heart Mandelbrot
- **Category:** Experimental
- **Formula:** Custom heart-shaped variation
- **Source:** Check experimental family

**Audit Checklist:**
- [ ] Heart shape visible
- [ ] Formula correct
- [ ] Julia mode works
- [ ] Performance acceptable

**Test Notes:**
- Heart shape: ☐ Yes ☐ No
- Render time: ___s
- Overall: ☐ Pass ☐ Fail ☐ Needs Work

---

## Summary

### Completion Status
- **Audited:** 0 / 30
- **Passed:** 0
- **Failed:** 0
- **Needs Work:** 0

### Common Issues Found
(Fill in as you audit)

---

### Next Steps
After completing Tier 1, proceed to:
- **Tier 2:** Standard fractals (see `TIER2_STANDARD_FRACTALS.md`)
- **Category audits:** Deep dive into specific categories

---

## Notes
- Focus on mathematical correctness first
- Performance issues can be addressed later
- Document any crashes or serious bugs immediately
- Compare visual output to reference images when available
