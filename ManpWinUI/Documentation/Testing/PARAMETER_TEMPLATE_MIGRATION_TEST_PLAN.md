# Parameter Template Migration - Testing Plan

**Date:** July 12, 2026  
**Branch:** `feature/complete-flexible-parameters`  
**Migration Status:** Code complete — 335/335 fractals registered  
**Test Status:** Ready for validation

---

## Overview

This document provides a **strategic testing plan** to validate the parameter template migration. All 335 fractals now use the flexible parameter system. Rather than testing all 348 template registrations, we focus on:

1. **New code paths** (Phases 1, 2, 6 — 69 new registrations)
2. **Diverse template types** (Standard, Julia, Multibrot, Special Functions, etc.)
3. **Critical infrastructure** (CSV viewport loading, parameter persistence, UI binding)

**Estimated testing time:** 15-20 minutes for Tier 1 (required), +10 minutes for Tier 2 (optional confidence check)

---

## Test Environment Setup

### Prerequisites

1. Solution is built successfully (no compilation errors)
2. `ManpWinUI` project launches without crashes
3. `FractalRegistry.csv` is up-to-date (335 fractals)
4. Branch: `feature/complete-flexible-parameters`

### Launch Application

```bash
# From solution directory
dotnet run --project ManpWinUI
```

Or press **F5** in Visual Studio to debug.

---

## Tier 1: Critical Path Testing (Required)

Test **one fractal from each new code phase** to validate all new template registrations work correctly.

### Phase 1: Multibrot Family (16 new registrations)

**Test 3 fractals:**

| Fractal | Internal Name | What to Verify |
|---------|---------------|----------------|
| **Multibrot-6 (Sextic)** | `Multibrot-6` | Exponent parameter = 6, viewport loads from CSV |
| **Multibrot-10 (Decic)** | `Multibrot-10` | Exponent parameter = 10, dash notation variant works |
| **Buffalo (Polynomial)** | `BuffaloPolynomial` | Polynomial variant template works |

**Steps:**
1. Select fractal from browser (left panel)
2. Open **Parameters** tab (right panel)
3. Verify parameters appear:
   - `center_x`, `center_y`, `zoom` (viewport from CSV)
   - `exponent` (shows correct value: 6, 10, or 3)
   - `max_iterations`, `bailout_radius` (algorithm parameters)
   - `julia_mode` toggle (if applicable)
4. Change `exponent` → fractal shape changes
5. Adjust `zoom` → fractal re-renders
6. No errors in Output window

---

### Phase 2: Julia Presets (3 new registrations)

**Test 2 fractals:**

| Fractal | Internal Name | What to Verify |
|---------|---------------|----------------|
| **Julia - Dendrite (Preset)** | `JuliaDendritePreset` | Preset suffix variant loads correctly |
| **Julia - Spiral (Preset)** | `JuliaSpiralPreset` | Standard template (no Julia mode toggle for presets) |

**Steps:**
1. Select fractal from browser
2. Open **Parameters** tab
3. Verify parameters appear:
   - `center_x`, `center_y`, `zoom` (viewport from CSV)
   - `max_iterations`, `bailout_radius`
   - **NO** `julia_mode` toggle (presets have hardcoded C values)
4. Adjust `max_iterations` → detail level changes
5. No errors

---

### Phase 6: Remaining Families (50 new registrations)

**Test 8-10 fractals** from diverse categories:

#### Engineering Fractals (2 tests)

| Fractal | Internal Name | What to Verify |
|---------|---------------|----------------|
| **Euler Buckling** | `EulerBuckling` | Engineering fractal loads, standard template |
| **Arrhenius Kinetics** | `ArrheniusKinetics` | Science/engineering template |

#### Special Functions (2 tests)

| Fractal | Internal Name | What to Verify |
|---------|---------------|----------------|
| **Weierstrass P** | `WeierstrassP` | Special function with Julia template |
| **Airy Bi** | `AiryBi` | Airy function fractal |

#### Jacobi Elliptic Functions (1 test)

| Fractal | Internal Name | What to Verify |
|---------|---------------|----------------|
| **Jacobi SN** | `JacobiSN` | Elliptic function fractal |

#### Statistical/Quantum (1 test)

| Fractal | Internal Name | What to Verify |
|---------|---------------|----------------|
| **Bose-Einstein Fractal** | `BoseEinsteinFractal` | Quantum statistics fractal |

#### Tetration (1 test)

| Fractal | Internal Name | What to Verify |
|---------|---------------|----------------|
| **Tetration Classic** | `TetrationClassic` | Tetration family fractal |

#### Exotic (1 test)

| Fractal | Internal Name | What to Verify |
|---------|---------------|----------------|
| **Nebulabrot** | `Nebulabrot` | Exotic rendering technique |

#### Combinatorial (1 test)

| Fractal | Internal Name | What to Verify |
|---------|---------------|----------------|
| **Collatz Fractal** | `CollatzFractal` | Discrete mathematics fractal |

#### Chemical Engineering (1 test)

| Fractal | Internal Name | What to Verify |
|---------|---------------|----------------|
| **Gray-Scott** | `GrayScott` | Reaction-diffusion system |

**Steps (same for all):**
1. Select fractal from browser
2. Open **Parameters** tab
3. Verify parameters populate correctly
4. Adjust any parameter → fractal responds
5. No crashes or exceptions

---

## Tier 2: Confidence Check (Optional)

Test a few **already-registered fractals** to ensure the migration didn't break existing functionality.

### Classic Fractals (already working before migration)

| Fractal | Internal Name | What to Verify |
|---------|---------------|----------------|
| **Mandelbrot** | `Mandelbrot` | The classic, Julia mode works |
| **Burning Ship** | `BurningShip` | Julia mode, viewport correct |
| **Phoenix Fractal** | `Phoenix` | Phoenix P/Q custom parameters |
| **Newton (z³-1)** | `Newton` | Newton template with degree parameter |
| **Logistic Bifurcation** | `LogisticBifurcation` | Bifurcation template with min/max Y |
| **Julia - Golden Ratio** | `JuliaGoldenRatio` | Standard Julia preset (already working) |

**Steps:**
1. Select each fractal
2. Verify Parameters tab still works correctly
3. No regressions from the migration

---

## What to Check Per Fractal

### ✅ Pass Criteria

For each test fractal:

1. **Loads:** Fractal appears in browser and renders when selected
2. **Parameters Tab Populated:**
   - View parameters: `center_x`, `center_y`, `zoom`
   - Algorithm parameters: `max_iterations`, `bailout_radius`, `smooth_coloring`
   - Family-specific parameters (e.g., `exponent`, `julia_mode`, `phoenix_p`, etc.)
3. **Initial Viewport Correct:**
   - Fractal appears centered and scaled appropriately
   - Viewport defaults from CSV are applied
4. **Parameter Changes Work:**
   - Adjust `zoom` → fractal re-renders at new scale
   - Adjust `max_iterations` → detail level changes
   - Adjust family-specific parameter → fractal behavior changes
   - (For Julia-enabled) Toggle `julia_mode` → switches between Mandelbrot/Julia set
5. **No Errors:**
   - No exceptions in Output window (View → Output → Build/Debug panes)
   - No UI crashes or freezes
   - Parameters persist when switching fractals and returning

---

## Common Issues & Troubleshooting

### Issue: "Fractal not found in browser"

**Cause:** Internal name mismatch between CSV and `FractalParameterService.cs`  
**Check:**
- CSV column `Internal Name` matches `RegisterTemplate("...")`
- Registry CSV is up-to-date (335 fractals)

---

### Issue: "Parameters tab is empty"

**Cause:** Template registration missing or incorrect  
**Check:**
- `RegisterTemplate("FractalName", ...)` exists in `FractalParameterService.cs`
- Template factory method returns a valid `FractalParameterSet`
- Debug Output window for parameter loading errors

---

### Issue: "Initial viewport is wrong (e.g., zoomed too far out)"

**Cause:** CSV viewport defaults missing or incorrect  
**Check:**
- `FractalRegistry.csv` has values in columns: `Center X`, `Center Y`, `X Scale Width`, `Y Scale Width`
- `GetNativeViewportDefaults()` successfully reads from native registry
- Fallback (0.0, 0.0, 1.0) is being used if CSV data missing

---

### Issue: "Parameter changes don't trigger re-render"

**Cause:** Parameter change event not wired or render pipeline issue  
**Check:**
- Not a template issue — this is a broader render pipeline problem
- Check `MainViewModel.Parameters.cs` → `OnParameterValueChanged` handler

---

## Automated Verification (Optional)

If you want to script a bulk check:

```powershell
# Verify all CSV fractals are registered
$csv = Import-Csv "ManpWinUI\Documentation\Architecture\FractalRegistry.csv"
$content = Get-Content "ManpWinUI\Services\FractalParameterService.cs" -Raw
$matches = [regex]::Matches($content, 'RegisterTemplate\("([^"]+)"')
$registered = $matches | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique

$missing = $csv | Where-Object { 
    $_.'Internal Name' -notin $registered 
} | Select-Object 'Fractal Name', 'Internal Name'

if ($missing.Count -eq 0) {
    Write-Host "✅ All CSV fractals are registered!" -ForegroundColor Green
} else {
    Write-Host "❌ Missing fractals:" -ForegroundColor Red
    $missing | Format-Table
}
```

---

## Test Results Log

Use this table to track your testing progress:

### Phase 1: Multibrot Family

| Fractal | Loads | Params Tab | Viewport | Param Changes | Errors | Status |
|---------|-------|------------|----------|---------------|--------|--------|
| Multibrot-6 | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |
| Multibrot-10 | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |
| BuffaloPolynomial | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |

### Phase 2: Julia Presets

| Fractal | Loads | Params Tab | Viewport | Param Changes | Errors | Status |
|---------|-------|------------|----------|---------------|--------|--------|
| JuliaDendritePreset | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |
| JuliaSpiralPreset | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |

### Phase 6: Remaining Families

| Fractal | Loads | Params Tab | Viewport | Param Changes | Errors | Status |
|---------|-------|------------|----------|---------------|--------|--------|
| EulerBuckling | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |
| ArrheniusKinetics | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |
| WeierstrassP | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |
| AiryBi | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |
| JacobiSN | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |
| BoseEinsteinFractal | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |
| TetrationClassic | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |
| Nebulabrot | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |
| CollatzFractal | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |
| GrayScott | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |

### Tier 2: Confidence Check (Optional)

| Fractal | Loads | Params Tab | Viewport | Param Changes | Errors | Status |
|---------|-------|------------|----------|---------------|--------|--------|
| Mandelbrot | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |
| BurningShip | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |
| Phoenix | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |
| Newton | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |
| LogisticBifurcation | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |
| JuliaGoldenRatio | ☐ | ☐ | ☐ | ☐ | ☐ | ⬜ |

**Legend:**
- ☐ = Not tested
- ✅ = Pass
- ❌ = Fail
- ⬜ = Overall status (pass all → ✅, any fail → ❌)

---

## Completion Checklist

- [ ] **Tier 1 tests complete** (Phase 1, 2, 6 — minimum 13 fractals)
- [ ] All tested fractals **load and render**
- [ ] All tested fractals **populate Parameters tab**
- [ ] All tested fractals **apply CSV viewport defaults**
- [ ] Parameter changes **trigger re-render** correctly
- [ ] No errors or crashes during testing
- [ ] (Optional) **Tier 2 confidence check** passed
- [ ] Test results documented (mark ✅ or ❌ in tables above)

---

## Sign-Off

**Tester:** _________________________  
**Date:** _________________________  
**Result:** ☐ Pass  ☐ Fail (see issues below)  
**Notes:**

---

**Issues Found (if any):**

| Fractal | Issue Description | Severity | Status |
|---------|-------------------|----------|--------|
|         |                   |          |        |

---

## Next Steps After Testing

### If all tests pass ✅

1. Commit changes:
   ```bash
   git add .
   git commit -m "Complete parameter template migration for 335 fractals"
   ```

2. Push to remote:
   ```bash
   git push origin feature/complete-flexible-parameters
   ```

3. (Optional) Merge to `development` or `master` branch when ready

### If issues found ❌

1. Document failures in "Issues Found" table above
2. Investigate root cause (see "Common Issues" section)
3. Fix issue in `FractalParameterService.cs` or related files
4. Rebuild solution
5. Re-test failed fractals
6. Repeat until all tests pass

---

**End of Test Plan**
