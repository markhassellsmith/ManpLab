# Session Persistence Policy

## Design Decision: Fractal Quality Parameters vs. UI Preferences

**Date**: 2025
**Status**: ✅ Implemented

## Overview

The application maintains two distinct types of settings with different persistence behaviors:

### 1. UI Preferences (PERSIST between sessions)
User interface settings and preferences that should remain consistent across all fractals:
- ✅ **Color palette choice** (via `IAppSettingsService.GetDefaultPalette()`)
- ✅ **Panel widths** (Browser panel, Properties panel)
- ✅ **Panel visibility** (Which panels are shown/hidden)
- ✅ **Selected fractal** (Last fractal selected in browser)
- ✅ **Properties tab selection** (Which tab was active: Parameters, Colors, Render, etc.)
- ✅ **Window layout** (Size, position)
- ✅ **Application theme** (Light, Dark, System)
- ✅ **Rendering options** (Show axes, smooth coloring, antialiasing level, deep zoom)

### 2. Fractal Quality Parameters (DO NOT persist between sessions)
Algorithmic rendering parameters specific to each fractal type:
- ❌ **`max_iterations`** - Computational depth
- ❌ **`bailout`** / **`escape_radius`** - Escape threshold
- ❌ **`auto_scale_iterations`** - Adaptive iteration scaling
- ❌ **Exponent** (for Multibrot variants)
- ❌ **Julia constants** (complex c values)
- ❌ Other fractal-specific algorithm parameters

## Rationale

### UI Preferences Should Persist
Users expect their workspace layout and visual preferences to remain stable. If a user prefers the "Fire" palette, they want it applied to all fractals until they explicitly change it. Similarly, panel sizes and window position should feel consistent.

### Fractal Quality Should NOT Persist
1. **Predictable Behavior**: Users always start with known, tested default values per fractal
2. **Clean Testing**: Eliminates cross-session contamination when comparing builds or debugging
3. **Application Control**: The application determines optimal initial quality settings per fractal type
4. **Explicit State Management**: Bookmarks provide explicit, user-controlled persistence for complete resume scenarios

## Architecture

### UI Preferences Persistence (Active)

**Service**: `AppSettingsService` implementing `IAppSettingsService`
**Storage**: `Windows.Storage.ApplicationData.Current.LocalSettings`
**Keys**: Single-value keys like `"DefaultPalette"`, `"BrowserPanelWidth"`, `"Theme"`, etc.

Examples:
```csharp
// Color palette persists across all fractals
_settingsService.SetDefaultPalette("Fire");
var palette = _settingsService.GetDefaultPalette(); // Returns "Fire" in next session

// Panel width persists
_settingsService.SetBrowserPanelWidth(300.0);
var width = _settingsService.GetBrowserPanelWidth(); // Returns 300.0 in next session
```

**Implementation locations**:
- `ColorEditorViewModel.SelectedPalette` setter calls `_settingsService.SetDefaultPalette()`
- `MainViewModel.UI.cs` loads panel widths, visibility, and selected fractal
- `App.xaml.cs` handles window size/position persistence

### Fractal Parameter Persistence (Disabled)

**Previous behavior** (now removed):
- `FractalParameterSet.LoadFromSettings()` / `SaveToSettings()` existed
- Used keys like `"FractalParams_{FractalType}"` (e.g., `"FractalParams_Multibrot-10"`)
- Automatically restored/saved on fractal selection and parameter changes

**Current behavior**:
- ❌ `LoadFromSettings()` no longer called in `MainViewModel.Parameters.InitializeParametersForFractal()`
- ❌ `SaveToSettings()` no longer called in `OnParameterValueChanged()`
- ✅ Parameters always load from template defaults via `FractalParameterService`

**Files modified**:
- `ManpWinUI/ViewModels/MainViewModel.Parameters.cs` (lines 83-86, 108-110)

### Bookmark Persistence (Active)

**File**: `ManpWinUI/Models/FractalBookmark.cs`
**Storage**: Managed by `BookmarkService` (JSON file serialization)

Bookmarks capture **complete state** for exact reproduction:
- Viewport (`CenterX`, `CenterY`, `Zoom`)
- Fractal identification (`FractalType`, `IterationMode`)
- UI preferences (`ColorPalette`, `MaxIterations`)
- Julia parameters (if applicable)
- **NEW**: Complete flexible parameter snapshot via `Parameters` dictionary
  - Includes all quality settings: `bailout`, `escape_radius`, `exponent`, `auto_scale_iterations`, etc.
  - Exported from `FractalParameterSet.ExportForSave()`

**Save flow**:
1. User clicks "Save Bookmark"
2. `MainViewModel.Bookmarks.SaveCurrentAsBookmarkAsync()` collects current state
3. **Captures full parameter snapshot**: `CurrentParameters.ExportForSave()`
4. Creates `FractalBookmark` with all data
5. `BookmarkService` persists to JSON file

**Load flow**:
1. User selects bookmarked item
2. `MainViewModel.Bookmarks.LoadBookmarkAsync()` restores all properties
3. **Restores full parameter snapshot**: `CurrentParameters.ImportValues(bookmark.Parameters)`
   - If `Parameters` is null (legacy bookmark), falls back to template defaults gracefully
   - Debug log indicates whether snapshot was restored or legacy format detected
4. Triggers auto-render with restored state

This ensures bookmarks reproduce the **exact same visual result** regardless of session defaults.

**Backward compatibility**: Legacy bookmarks (without `parameters` field) continue to work, loading viewport and palette correctly while using current template defaults for quality parameters. They automatically upgrade to the enhanced format when re-saved.

## Usage Scenarios

### Scenario 1: Fresh Session with UI Preferences
1. User launches application
2. **UI preferences restored**: Last selected palette "Fire", panel widths, window position
3. User selects "Multibrot-10" from fractal list
4. **Fractal parameters from template**: `max_iterations=512`, `bailout=256` (application defaults)
5. Fractal renders with "Fire" palette and default quality settings

### Scenario 2: Palette Change Persists
1. User renders "Mandelbrot" with "Classic" palette
2. User switches to "Fire" palette
3. User renders "Multibrot-10" - automatically uses "Fire" palette
4. User closes application
5. **Next session**: All fractals render with "Fire" palette until user changes it

### Scenario 3: Parameter Changes Do NOT Persist
1. User adjusts `max_iterations` to 1024 for deeper exploration
2. Changes apply immediately to current session
3. User closes application
4. **Next session**: Selecting "Multibrot-10" again returns to `max_iterations=512` default

### Scenario 3: Parameter Changes Do NOT Persist
1. User adjusts `max_iterations` to 1024 for deeper exploration
2. Changes apply immediately to current session
3. User closes application
4. **Next session**: Selecting "Multibrot-10" again returns to `max_iterations=512` default
5. **But**: User's palette choice, panel widths, and window layout are preserved

### Scenario 4: Bookmark for Complete Resume State
1. User finds an interesting location with custom settings (`zoom=5.8`, `max_iterations=2048`)
2. User saves bookmark: "Deep Multibrot-10 Detail"
3. User closes application
4. **Next session**: Loading the bookmark restores **all** saved state including custom iterations

## Benefits

### For UI Preferences (Persistent)
- **Consistency**: Workspace layout feels stable across sessions
- **User Control**: "I want fractals rendered in this palette" - one choice applies to all
- **Productivity**: Don't need to resize panels or reposition window every time

### For Fractal Parameters (Ephemeral)
### For Fractal Parameters (Ephemeral)
- **Consistency**: Every user and every session starts from the same known baseline per fractal
- **Debuggability**: No hidden state from previous sessions affecting render quality
- **Clarity**: Bookmarks are the explicit, visible mechanism for saving exploration state
- **Flexibility**: Users can still preserve interesting views via bookmarks

## Technical Details

### UI Preferences Storage (Active)

**Service**: `IAppSettingsService` / `AppSettingsService`  
**Keys used**:
- `"DefaultPalette"` - Color palette name (e.g., "Fire", "Ocean", "Classic")
- `"BrowserPanelWidth"` - Width in pixels
- `"PropertiesPanelWidth"` - Width in pixels
- `"BrowserPanelVisible"` - Boolean
- `"PropertiesPanelVisible"` - Boolean
- `"SelectedFractal"` - Last selected fractal name
- `"PropertiesTabIndex"` - Active tab index
- `"Theme"` - "Light", "Dark", or "System"
- `"ShowAxesByDefault"` - Boolean
- `"UseSmoothColoring"` - Boolean
- `"DefaultAntialiasingLevel"` - String level
- `"UseDeepZoom"` - Boolean
- `"WindowWidth"`, `"WindowHeight"`, `"WindowX"`, `"WindowY"` - Window geometry

**Persistence happens**:
- When user changes palette in `ColorEditorViewModel`
- When user resizes panels (via binding in `MainViewModel.UI.cs`)
- When user selects different fractal in browser
- When window is resized/moved (via `App.xaml.cs`)

### Fractal Parameter Storage (Inactive)

**Storage keys** (no longer used for runtime):
- Format: `"FractalParams_{FractalType}"` (e.g., `"FractalParams_Multibrot-10"`)
- These keys may still exist in storage from previous versions
- The application no longer reads or writes these keys during fractal selection or parameter changes

**Methods exist but are not called**:
- `FractalParameterSet.LoadFromSettings()` - Still exists, not called at runtime
- `FractalParameterSet.SaveToSettings()` - Still exists, not called at runtime
- These could be used in the future for explicit "Save as Default" features if needed

### Template System (Single Source of Truth)
- **Source**: `FractalParameterService.cs` template factories
- **Defaults**: Defined in `StandardParameterTemplates.Core.cs`
- **Loading**: `MainViewModel.Parameters.InitializeParametersForFractal()`

### Bookmark Persistence (Explicit User Intent)
- **Source**: `FractalBookmark.FromCurrentState()`
- **Storage**: Managed by `BookmarkService`
- **Loading**: `MainViewModel.Bookmarks.LoadBookmarkAsync()`

## Migration Notes

Applications upgraded from versions with session persistence will experience a one-time reset to application defaults. Users who previously relied on implicit session persistence should be encouraged to use bookmarks for saving interesting views.

## Related Documentation

- `PARAMETER_TEMPLATE_STRATEGY.md` – Template system architecture
- `PARAMETER_TEMPLATE_REFERENCE.md` – Available template families
- `PARAMETER_TEMPLATE_MIGRATION_TEST_PLAN.md` – Testing guidelines

## Future Considerations

If user feedback indicates a need for "last session state" persistence, consider:
1. Adding a user preference toggle for session persistence
2. Implementing a "Recently Used" bookmark category that auto-saves on exit
3. Maintaining the current policy as default, with opt-in persistence

The current design prioritizes predictability and explicit state management over convenience.
