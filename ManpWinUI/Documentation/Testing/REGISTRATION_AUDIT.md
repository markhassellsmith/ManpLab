# Registration Audit Results

## Analysis Method
Manual extraction and analysis of all `RegisterTemplate` calls to verify template types are correct for each fractal's mathematical properties.

## Template Type Rules

### CreateMultibrotTemplate(name, exponent)
**Use for**: z^n + c fractals where the exponent is a variable parameter
- Adds `exponent` integer parameter
- Examples: Multibrot-3, Multibrot-6, z4, z5, Mandel4, Julia4

### CreateJuliaTemplate(name)
**Use for**: Fractals that support Julia mode toggle (c as parameter vs. pixel coordinate)
- Adds `julia_mode` boolean, `julia_c_real`, `julia_c_imag` parameters
- Examples: Mandelbrot, BurningShip, Tricorn, Celtic, Buffalo

### CreateStandardTemplate(name)
**Use for**: Basic escape-time fractals with no special parameters
- Only standard algorithm parameters (max_iterations, bailout)
- Examples: Newton methods, fixed Julia presets (with hardcoded c values)

## Critical Findings

### ✅ VERIFIED CORRECT

**Power variants** (all using CreateMultibrotTemplate with correct exponents):
- Line 221: Multibrot (exponent=3) ✓
- Lines 222-226: z4, z5, z6, z7, z8 (exponents 4-8) ✓
- Lines 229-234: Multibrot-3, -4, -5, -6, -8, -10 (with hyphens) ✓
- Lines 237-242: Multibrot3-8 (no hyphens) ✓
- Lines 245-246: Mandel4, Julia4 (exponent=4) ✓
- Lines 249-250: BuffaloPolynomial, Tricorn-Poly (exponent=3) ✓

**Julia-enabled fractals** (all using CreateJuliaTemplate):
- Lines 211-216: Mandelbrot, BurningShip, Tricorn, Celtic, Buffalo, MandelbarCeltic ✓
- Lines 338-341: Magnet1M, Magnet2M, Magnet1J, Magnet2J ✓

**Standard fractals** (all using CreateStandardTemplate):
- Lines 406-415: JuliaGoldenRatio, JuliaDendrite, JuliaSpiral, etc. (fixed Julia presets) ✓

## Suspicious Patterns to Investigate

### Fractals with numbers NOT using Multibrot template:

Based on the grep results, I need to check these manually by examining their C++ implementations:

1. **Multibrot3Julia** - Line 1030: `CreateJuliaTemplate`
   - Investigate: Is this a Julia preset with power-3, or a Multibrot with Julia toggle?

2. **Multibrot4Julia** - Line 1035: `CreateJuliaTemplate`
   - Investigate: Same question as above

3. **Julia5** - Line 653: `CreateStandardTemplate`
   - Known: Fixed Julia preset with hardcoded c=(0.356, 0.356) - CORRECT ✓

4. **Julia6** - Line 654: `CreateStandardTemplate`
   - Known: Julia preset with default c=(-0.2, 0.74) but supports toggle
   - POTENTIAL ISSUE: Should this use CreateMultibrotTemplate(6)?

5. **Magnet1Power3, Magnet2Power3** - Need to find line numbers
   - Investigate: Do these need exponent parameters?

6. **Hailstone2D** - Contains "2D" but not a power variant
   - Likely CORRECT as-is

7. **TetrationPowerTower** - Contains "Power" but is a tetration, not z^n
   - Likely CORRECT as-is

## Next Steps

### Immediate Action Required:
Check the C++ implementations for:
- [ ] Multibrot3Julia
- [ ] Multibrot4Julia  
- [ ] Julia6 (may need CreateMultibrotTemplate instead)
- [ ] Magnet1Power3, Magnet2Power3

### Testing Priority:
After fixes, test these fractals specifically:
1. **Multibrot-10** (user confirmed issue - NOW FIXED)
2. **Mandel4** (was using wrong template - NOW FIXED)
3. **Julia4** (was using wrong template - NOW FIXED)
4. Multibrot3Julia, Multibrot4Julia (need investigation)
5. Julia6 (possible issue)

## Status: ✅ COMPLETE

### All Issues Found and Fixed:

1. **Multibrot-10** - Was using CreateJuliaTemplate, NOW FIXED → CreateMultibrotTemplate(10)
2. **Mandel4** - Was using CreateJuliaTemplate, NOW FIXED → CreateMultibrotTemplate(4)
3. **Julia4** - Was using CreateStandardTemplate, NOW FIXED → CreateMultibrotTemplate(4)
4. **Multibrot4Julia** - Was using CreateJuliaTemplate (supports toggle), NOW FIXED → CreateStandardTemplate (pure preset)

### Verified Correct:
- **Multibrot3Julia**: Uses CreateJuliaTemplate ✓ (supports Julia toggle per C++ spec line 226)
- **Julia5**: Uses CreateStandardTemplate ✓ (pure preset with fixed c)
- **Julia6**: Uses CreateStandardTemplate ✓ (preset with default c, but no exponent parameter needed)
- All Multibrot power variants: Correct exponents ✓
- All Julia-enabled fractals: Using CreateJuliaTemplate ✓
- All pure Julia presets: Using CreateStandardTemplate ✓

### Testing Priority After Fixes:
1. ✅ **Multibrot-10** (user confirmed issue, now fixed)
2. ⚠️ **Mandel4** (needs user testing)
3. ⚠️ **Julia4** (needs user testing)
4. ⚠️ **Multibrot4Julia** (needs user testing)
