# Task 6: Parameter Persistence - Testing Guide

**Goal**: Verify that parameter values are saved and restored across app sessions.

---

## Test 1: Basic Persistence (Mandelbrot)

**Steps**:
1. Launch app
2. Select **Mandelbrot Set** from fractal browser
3. Note default values:
   - Max Iterations: 256
   - Zoom: 1.0
   - Center: (-0.5, 0.0)
4. Change parameters:
   - Set Max Iterations to **500**
   - Set Zoom to **2.5**
   - Set Center X to **-0.75**
5. **Close app completely**
6. **Relaunch app**
7. Select **Mandelbrot Set** again

**Expected Result**:
- ✅ Max Iterations should be **500** (not 256)
- ✅ Zoom should be **2.5** (not 1.0)
- ✅ Center X should be **-0.75** (not -0.5)
- ✅ Debug output should show: `[FractalParameterSet] Restored X parameters for 'Mandelbrot' from LocalSettings`

---

## Test 2: Per-Fractal Isolation (Lambda vs Mandelbrot)

**Steps**:
1. Select **Mandelbrot Set**
2. Set Max Iterations to **500**
3. Select **Lambda**
4. Set Max Iterations to **300**
5. Switch back to **Mandelbrot Set**

**Expected Result**:
- ✅ Mandelbrot should still show **500** iterations
- ✅ Lambda should show **300** iterations when selected again
- ✅ Each fractal remembers its own separate parameter set

---

## Test 3: Native Parameters (Lambda)

**Steps**:
1. Select **Lambda**
2. Note the 4 native parameters:
   - `realz0` (Real Perturbation of Z(0))
   - `imagz0` (Imaginary Perturbation of Z(0))
   - `maxIterations`
   - `bailout`
3. Change `realz0` to **0.5**
4. Change `bailout` to **10.0**
5. **Close app**
6. **Relaunch app**
7. Select **Lambda**

**Expected Result**:
- ✅ `realz0` should be **0.5**
- ✅ `bailout` should be **10.0**
- ✅ Debug output shows native parameters were restored

---

## Test 4: Reset to Defaults

**Steps**:
1. Select **Mandelbrot Set**
2. Set Max Iterations to **500** (custom value)
3. Call `ResetParametersToDefaults()` (via code or future UI button)

**Expected Result**:
- ✅ Max Iterations resets to **256** (registry default)
- ✅ Saved LocalSettings entry is cleared
- ✅ Next app launch will use defaults again (not custom value)

---

## Test 5: Multiple Sessions

**Steps**:
1. Launch app → Select Mandelbrot → Set Zoom to **1.5**
2. Close app
3. Launch app → Select Lambda → Set Max Iterations to **400**
4. Close app
5. Launch app → Select Mandelbrot

**Expected Result**:
- ✅ Mandelbrot shows Zoom **1.5** (from step 1)
- ✅ Lambda settings are preserved separately

---

## Debug Output to Watch For

### Save Operation:
```
[MainViewModel.Parameters] Parameter 'zoom' changed: 1.0 → 2.5
[FractalParameterSet] Saved 4 parameters for 'Mandelbrot' to LocalSettings
```

### Load Operation (Success):
```
[MainViewModel.Parameters] Initializing parameters for 'Mandelbrot'
[FractalParameterSet] Restored 4 parameters for 'Mandelbrot' from LocalSettings
[MainViewModel.Parameters] Restored saved parameter values for 'Mandelbrot'
```

### Load Operation (No Saved Data):
```
[FractalParameterSet] No saved parameters found for 'Lambda'
[MainViewModel.Parameters] No saved parameters found, using defaults for 'Lambda'
```

### Reset Operation:
```
[MainViewModel.Parameters] Resetting parameters to defaults for 'Mandelbrot'
[FractalParameterSet] Cleared saved parameters for 'Mandelbrot'
[MainViewModel.Parameters] Parameters reset to defaults
```

---

## LocalSettings Inspection (Advanced)

To verify storage format, check Windows LocalSettings:

**Location**: `%LocalAppData%\Packages\<PackageFamilyName>\Settings\settings.dat`

**Expected Keys**:
- `FractalParams_Mandelbrot`
- `FractalParams_Lambda`
- etc.

**Example Value** (JSON):
```json
{
  "maxIterations": 500,
  "center_x": -0.75,
  "center_y": 0.0,
  "zoom": 2.5
}
```

---

## Known Limitations

1. **View parameters (center_x, center_y, zoom)** are saved per fractal, not shared
   - Each fractal remembers where you left it
   - This is intentional for Task 6 behavior

2. **Parameters only save when changed via parameter system**
   - Direct property sets (legacy code paths) may not trigger save
   - This is expected during migration period

3. **No versioning or migration**
   - If parameter schema changes, old saved values may be ignored
   - Future enhancement: add version field to JSON

---

## Success Criteria

✅ Parameters persist across app sessions  
✅ Per-fractal isolation works correctly  
✅ Native parameters (Lambda, etc.) persist  
✅ Auto-save happens on parameter change  
✅ Auto-restore happens on fractal selection  
✅ Reset clears saved values and restores defaults  

**Task 6 Status**: ✅ Complete
