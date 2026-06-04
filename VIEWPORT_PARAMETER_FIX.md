# Viewport Parameter Persistence Fix

## Problem
When selecting `LogisticParameterSpace` (or any fractal), the viewport would not respect the InitialConditions service defaults. Instead, previously saved viewport parameters (`center_x`, `center_y`, `zoom`) from LocalSettings would override the intended "forest view" starting position.

Additionally, the CSV file had encoding issues due to PowerShell's `Export-Csv` adding quotes around all values.

## Root Cause Analysis

### Issue 1: CSV Encoding
- PowerShell's `Export-Csv` was adding quotes: `"LogisticParameterSpace","2.0","0.0","0.697"`
- The C++ parser expected unquoted values: `LogisticParameterSpace,2.0,0.0,0.697`
- This could cause parsing failures and `SEHException` in the native layer

### Issue 2: Viewport Parameter Persistence
The parameter system was treating viewport parameters (`center_x`, `center_y`, `zoom`) as regular user parameters:

1. User navigates around a fractal → viewport changes
2. Parameter editor saves ALL non-readonly parameters (including viewport)
3. Next time fractal is selected:
   - `MainPage.cs` loads metadata and sets viewport from `InitialConditionsService`
   - `FractalParameterService` loads saved overrides and **re-imports** old viewport values
   - Result: User sees their old zoom/pan position, NOT the "forest view"

## Solution

### Fix 1: CSV Encoding ✅
- CSV is now written using `[System.IO.File]::WriteAllText()` with UTF-8 BOM
- Values are unquoted: `LogisticParameterSpace,2.0,0.0,0.697`
- Compatible with both C++ parser and Excel

### Fix 2: Viewport Parameter Exclusion ✅

Added `IsViewportParameter` flag to the parameter system:

**Files Changed:**
1. `ManpWinUI/Models/Parameters/FractalParameterDescriptor.cs`
   - Added `public bool IsViewportParameter { get; init; } = false;`
   - Updated `WithOverrides()` to preserve the flag

2. `ManpWinUI/Models/Parameters/StandardParameterTemplates.Core.cs`
   - Marked `center_x`, `center_y`, `zoom` with `IsViewportParameter = true`
   - Added comments: "Don't persist - controlled by InitialConditions/Bookmarks"

3. `ManpWinUI/ViewModels/Properties/ParameterEditorViewModel.Core.cs`
   - Added `IsViewportParameter` property to `ParameterItem` class

4. `ManpWinUI/ViewModels/Properties/ParameterEditorViewModel.Flexible.cs`
   - Populate `IsViewportParameter` when mapping descriptors to UI items

5. `ManpWinUI/ViewModels/Properties/ParameterEditorViewModel.Persistence.cs`
   - Modified save logic: `if (!parameter.IsReadOnly && !parameter.IsViewportParameter)`
   - Viewport parameters now excluded from persistence

## Behavior After Fix

### Viewport Control Flow
```
┌─────────────────────────────────────────────────────────────┐
│  User Selects Fractal                                       │
└────────────────┬────────────────────────────────────────────┘
                 │
                 v
┌─────────────────────────────────────────────────────────────┐
│  MainPage.cs: Load metadata from MetadataService            │
│  ViewModel.CenterX = metadata.DefaultCenterX                │
│  ViewModel.CenterY = metadata.DefaultCenterY                │
│  ViewModel.Zoom = metadata.DefaultZoom                      │
│  (Values come from InitialConditionsService CSV)            │
└────────────────┬────────────────────────────────────────────┘
                 │
                 v
┌─────────────────────────────────────────────────────────────┐
│  FractalParameterService: Load saved parameter overrides    │
│  NOW: Viewport params (center_x, center_y, zoom) SKIPPED    │
│  Only fractal-specific params (iterations, bailout, etc.)   │
└────────────────┬────────────────────────────────────────────┘
                 │
                 v
┌─────────────────────────────────────────────────────────────┐
│  Result: Viewport shows "forest view" from CSV!             │
│  LogisticParameterSpace: (2.0, 0.0, 0.697)                  │
└─────────────────────────────────────────────────────────────┘
```

### Saved Viewport States
Viewport positions are now managed by **two separate systems**:

1. **InitialConditionsService** (CSV file)
   - Global "forest view" defaults
   - One per fractal
   - Edited via code/CSV only
   - Example: `LogisticParameterSpace,2.0,0.0,0.697`

2. **BookmarkService**
   - User-saved interesting views
   - Multiple bookmarks per fractal
   - Full UI for create/load/delete
   - Stores: name, description, viewport, parameters, coloring

**Parameter system no longer stores viewport!**

## Testing Instructions

### Before Testing: Clear Old Saved Viewport Data
Since the parameter system previously saved viewport values, you need to clear LocalSettings:

**Option A: Delete LocalSettings (Recommended)**
```powershell
# Close the app first, then:
$localAppData = [Environment]::GetFolderPath('LocalApplicationData')
$settingsPath = Join-Path $localAppData "Packages\*ManpLab*\LocalState"
Remove-Item $settingsPath -Recurse -Force -ErrorAction SilentlyContinue
```

**Option B: Use Settings UI**
- Launch app → Settings → "Clear All Settings"

### Test Case 1: LogisticParameterSpace Initial View
1. Launch app
2. Select "Logistic Parameter Space" from browser
3. **Expected:** Viewport centers at (2.0, 0.0) with zoom 0.697
4. Should show bifurcation region, not blank/colored bands

### Test Case 2: Parameter Persistence Exclusion
1. Select any fractal
2. Pan/zoom to a different location
3. Change a non-viewport parameter (e.g., max iterations)
4. Close app
5. Relaunch and select the same fractal
6. **Expected:** Viewport resets to InitialConditions "forest view"
7. **Expected:** Non-viewport parameter changes ARE preserved

### Test Case 3: Bookmark System Still Works
1. Select a fractal and navigate to an interesting view
2. Create a bookmark
3. Pan somewhere else
4. Load the bookmark
5. **Expected:** Viewport restores to bookmarked position

## Files Modified

### Native Layer
- `ManpCore.Native/Resources/InitialConditions.csv` - Fixed encoding, corrected LogisticParameterSpace values
- `ManpCore.Native/BifurcationFamily.cpp` - **NEW: Replaced averaging with period detection** for LogisticParameterSpace and MayLyapunovRef

### C# Parameter System
- `ManpWinUI/Models/Parameters/FractalParameterDescriptor.cs` - Added `IsViewportParameter` property
- `ManpWinUI/Models/Parameters/StandardParameterTemplates.Core.cs` - Marked viewport params as non-persistent
- `ManpWinUI/ViewModels/Properties/ParameterEditorViewModel.Core.cs` - Added flag to ParameterItem
- `ManpWinUI/ViewModels/Properties/ParameterEditorViewModel.Flexible.cs` - Populate flag during mapping
- `ManpWinUI/ViewModels/Properties/ParameterEditorViewModel.Persistence.cs` - Exclude viewport params from save

## Related Issues
- SEHException: Likely caused by malformed CSV (now fixed)
- Colored vertical bands: ~~Could be caused by wrong viewport (0,0,1) instead of bifurcation region (2.0,0.0,0.697)~~ **Fixed with period detection algorithm**

## Bifurcation Visualization Enhancement

### Problem
LogisticParameterSpace and MayLyapunovRef were using **averaging algorithms** that produced smooth color bands, hiding bifurcation structure. Initial attempt with **period detection** produced vertical stripes because the logistic map is fundamentally a 1D system (only r parameter matters, Y-axis irrelevant).

### Solution: Orbit Variance
Replaced averaging with **orbit variance** algorithm:
- Measures the **standard deviation** of attractor values
- Mathematically truthful and visually smooth
- No vertical striping (continuous gradient)

### How It Works
1. Run transient iterations to reach the attractor
2. Sample 100 orbit points
3. Calculate variance: how spread out are the points?
4. Map to color: low variance (dark) → high variance (bright)

### Mathematical Interpretation
- **Period-1**: All points identical → variance ≈ 0 → **dark**
- **Period-2**: Two alternating points → small variance → **slightly brighter**
- **Period-4, 8, 16...**: More points, more spread → **increasing brightness**
- **Chaos**: Points scattered across [0,1] → high variance → **bright**

### Expected Visual Result
**Optimal Viewport: CenterX=3.4, CenterY=0.5, Zoom=2.5 (shows r ≈ [2.2, 4.6])**

Smooth color gradient showing:
- **r < 3.0**: Dark region (period-1, zero variance)
- **r ≈ 3.0-3.5**: Gradual brightening (period-doubling, increasing variance)
- **r > 3.57**: Bright region (chaos, high variance)
- **r ≈ 3.83**: Darker band within chaos (period-3 window, lower variance)

This creates a **smooth, truthful visualization** of the transition from order to chaos!

This matches the mathematical reality: the logistic map undergoes a **period-doubling cascade** leading to chaos, which is now visible in the visualization.

## Future Considerations
- Consider adding UI indicator showing when viewport differs from default
- Could add "Reset Viewport" button to restore InitialConditions default
- May want to add CSV validation tool to catch encoding issues early
