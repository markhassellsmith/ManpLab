# Bookmark Format Migration: Legacy to Enhanced Parameters

## Overview

The bookmark format has been enhanced to capture complete parameter snapshots. This change is **fully backward compatible** with existing bookmarks.

## What Changed

### Old Format (Legacy)
```json
{
  "id": "abc123",
  "name": "Interesting Spiral",
  "fractalType": "Multibrot-10",
  "centerX": -0.172,
  "centerY": 0.933,
  "zoom": 5.876,
  "maxIterations": 2048,
  "colorPalette": "Fire"
}
```

### New Format (Enhanced)
```json
{
  "id": "abc123",
  "name": "Interesting Spiral",
  "fractalType": "Multibrot-10",
  "centerX": -0.172,
  "centerY": 0.933,
  "zoom": 5.876,
  "maxIterations": 2048,
  "colorPalette": "Fire",
  "parameters": {
    "max_iterations": 2048,
    "bailout": 512.0,
    "escape_radius": 2.0,
    "exponent": 10,
    "auto_scale_iterations": true
  }
}
```

## Backward Compatibility

### Loading Legacy Bookmarks ✅

**What happens:**
1. User loads an old bookmark (without `"parameters"` field)
2. `bookmark.Parameters` is `null`
3. Load code checks: `if (bookmark.Parameters != null && CurrentParameters != null)`
4. Since `Parameters` is `null`, the condition is false
5. Debug log shows: `"No parameter snapshot in bookmark (legacy bookmark format)"`
6. Bookmark still loads successfully using:
   - Viewport (`CenterX`, `CenterY`, `Zoom`)
   - `FractalType` (which triggers template defaults)
   - `MaxIterations` (set via ViewModel property)
   - `ColorPalette`
   - Julia constants (if present)

**Result**: Legacy bookmarks work, but use current template defaults for quality parameters not explicitly stored.

### Upgrading Legacy Bookmarks 🔄

**Automatic upgrade on re-save:**
1. User loads a legacy bookmark
2. User makes any viewport change (pan, zoom, parameter adjustment)
3. User saves bookmark with same name (overwrite)
4. New save captures complete parameter snapshot
5. Bookmark file is now in enhanced format

**No explicit migration required** - bookmarks upgrade naturally as users interact with them.

## Migration Behavior Details

### Field: `parameters` (Dictionary<string, object>?)

- **Type**: Optional (nullable)
- **Serialization**: JSON property name `"parameters"`
- **When null**: Bookmark falls back to template defaults for quality parameters
- **When present**: Complete parameter snapshot is restored

### Loading Priority

When a bookmark is loaded:

1. **Viewport** - Always restored from bookmark
2. **Fractal type** - Always restored, triggers parameter template load
3. **Color palette** - Always restored from bookmark
4. **Quality parameters** - Restored from `parameters` snapshot if present, otherwise use template defaults

This means:
- ✅ Legacy bookmarks: Viewport + palette are correct, quality uses current template defaults
- ✅ Enhanced bookmarks: Exact reproduction of saved state

## Testing Legacy Bookmarks

### Expected Behavior

**Scenario 1: Load legacy bookmark with no viewport changes**
```
Load "Old Bookmark" (no parameters field)
→ FractalType="Multibrot-10" loads template: max_iterations=512, bailout=256
→ Bookmark viewport applied: center=(-0.172, 0.933), zoom=5.876
→ Result: Correct viewport, current template quality settings
```

**Scenario 2: Load legacy bookmark, adjust quality, re-save**
```
Load "Old Bookmark" (no parameters field)
→ Uses template defaults
→ User increases max_iterations to 2048
→ User saves bookmark (overwrite)
→ New save includes: "parameters": { "max_iterations": 2048, ... }
→ Next load: Exact state reproduction
```

**Scenario 3: Load enhanced bookmark**
```
Load "New Bookmark" (has parameters field)
→ Template loads first: max_iterations=512
→ Parameter snapshot imported: max_iterations=2048, bailout=512
→ Result: Exact saved state restored
```

## Debug Output

### Legacy Bookmark Load
```
[MainViewModel.Bookmarks] Loaded bookmark: Old Bookmark
[MainViewModel.Bookmarks] No parameter snapshot in bookmark (legacy bookmark format)
[MainViewModel.Parameters] Using application defaults for 'Multibrot-10' (session persistence disabled)
```

### Enhanced Bookmark Load
```
[MainViewModel.Bookmarks] Loaded bookmark: New Bookmark
[MainViewModel.Bookmarks] Restored 8 parameters from bookmark
[MainViewModel.Parameters] Using application defaults for 'Multibrot-10' (session persistence disabled)
```

### Enhanced Bookmark Save
```
[MainViewModel.Bookmarks] Captured 8 parameters for bookmark
```

## Compatibility Matrix

| Scenario | Viewport | Palette | Quality Parameters | Result |
|----------|----------|---------|-------------------|--------|
| Legacy bookmark, legacy app | ✅ Correct | ✅ Correct | ✅ Old defaults | Works |
| Legacy bookmark, new app | ✅ Correct | ✅ Correct | ⚠️ New template defaults | Works (best-effort) |
| Enhanced bookmark, new app | ✅ Correct | ✅ Correct | ✅ Exact saved state | Perfect reproduction |
| Enhanced bookmark, old app* | ✅ Correct | ✅ Correct | ⚠️ Ignores `parameters` | Degrades gracefully |

\* Hypothetically, if user downgrades to older app version without `parameters` support

## Benefits

### For Users
- ✅ Existing bookmarks continue to work without manual migration
- ✅ Bookmarks gradually upgrade as they're used and re-saved
- ✅ No data loss or compatibility warnings
- ✅ Enhanced precision for new bookmarks going forward

### For Developers
- ✅ Clean migration path (additive, not breaking)
- ✅ Explicit null-checking makes behavior clear
- ✅ Debug logging helps diagnose format issues
- ✅ No database migration scripts needed

## Implementation Files

- `ManpWinUI/Models/FractalBookmark.cs` - Added nullable `Parameters` property
- `ManpWinUI/ViewModels/MainViewModel.Bookmarks.cs` - Enhanced save/load with null-checks
- `ManpWinUI/Models/Parameters/FractalParameterSet.cs` - `ExportForSave()` / `ImportValues()` methods

## Future Enhancements

If needed, could add:
- Explicit "Upgrade Bookmark" UI command
- Batch migration tool for all bookmarks
- Format version field for explicit compatibility checks
- Warning UI when loading legacy bookmarks

Currently, the silent graceful degradation + automatic upgrade on re-save approach provides the best user experience.
