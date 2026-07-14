# Duplicate Registration Fixes

## Issue
During the phased migration, some fractals were registered multiple times with conflicting template types, causing rendering degradation.

## Root Cause
The phased migration approach had different phases registering the same fractals:
- **Phase 1** (lines 220-250): Initial core fractals registered correctly
- **Phase 4** (lines 630+): Later phase re-registered some fractals with wrong template types

The second registration **overwrites** the first, causing incorrect parameters to be used.

## Fixes Applied

### 1. Multibrot-10 (CRITICAL - User Reported)
**Problem**: Registered twice
- Line 234: `CreateMultibrotTemplate("Multibrot-10", 10)` ✅ Correct
- Line 1180: `CreateJuliaTemplate("Multibrot-10")` ❌ Wrong - missing exponent parameter

**Impact**: Multibrot-10 rendered incorrectly without proper exponent parameter
**Fix**: Removed duplicate on line 1180

### 2. Mandel4
**Problem**: Registered twice
- Line 245: `CreateMultibrotTemplate("Mandel4", 4)` ✅ Correct
- Line 636: `CreateJuliaTemplate("Mandel4")` ❌ Wrong - missing exponent parameter

**Impact**: Mandel4 would render incorrectly
**Fix**: Removed duplicate on line 636

### 3. Julia4
**Problem**: Registered twice
- Line 246: `CreateMultibrotTemplate("Julia4", 4)` ✅ Correct
- Line 639: `CreateStandardTemplate("Julia4")` ❌ Wrong - missing exponent parameter

**Impact**: Julia4 would render incorrectly
**Fix**: Removed duplicate on line 639

### 4. Multibrot3, 4, 5, 6, 7, 8 (no-hyphen variants)
**Problem**: Registered twice
- Lines 237-242: `CreateMultibrotTemplate` with correct exponents ✅ Correct
- Lines 654-666: Duplicate registrations ❌ Redundant

**Impact**: Wasteful but functionally identical (both used correct template)
**Fix**: Removed duplicates on lines 654-666

## Template Selection Rules

### When to use each template:

1. **CreateMultibrotTemplate(name, exponent)**
   - For z^n + c fractals where n is a parameter
   - Examples: Multibrot-3, Multibrot-6, Mandel4
   - Adds `exponent` parameter to the set

2. **CreateJuliaTemplate(name)**
   - For fractals that support Julia mode toggle
   - Examples: Standard Mandelbrot, Phoenix, Lambda
   - Adds Julia toggle and julia_c_real/julia_c_imag parameters

3. **CreateStandardTemplate(name)**
   - For basic escape-time fractals
   - Examples: Newton methods, fixed Julia presets (Julia5 with hardcoded c)
   - No special parameters beyond basic algorithm params

## Prevention
To prevent future duplicates:
1. Search for existing registration before adding new one
2. Use the PowerShell duplicate-detection script before committing:
   ```powershell
   $content = Get-Content "ManpWinUI\Services\FractalParameterService.cs" -Raw
   $pattern = 'RegisterTemplate\("([^"]+)"'
   $matches = [regex]::Matches($content, $pattern)
   $grouped = $matches | Group-Object -Property {$_.Groups[1].Value} | Where-Object {$_.Count -gt 1}
   if ($grouped) {
       Write-Host "DUPLICATES FOUND:"
       $grouped | ForEach-Object {
           Write-Host "  $($_.Name): $($_.Count) registrations"
       }
   } else {
       Write-Host "No duplicates found ✓"
   }
   ```

## Testing
After these fixes, test the following fractals specifically:
- ✅ Multibrot-10 (user confirmed degradation)
- ⚠️ Mandel4 (needs testing)
- ⚠️ Julia4 (needs testing)

All other Multibrot variants should be unaffected since the duplicates were identical.
