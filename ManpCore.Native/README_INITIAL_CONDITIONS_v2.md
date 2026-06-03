# InitialConditions Service - READ-ONLY Design

## Overview

The `InitialConditionsService` provides **read-only access** to default viewport positions for all fractals in ManpLab. This is **immutable application data**, not user preferences.

## Design Philosophy

### "See the Forest, Not the Tree"

The initial viewport positions are **deliberately chosen** to show the **global structure** of each fractal:

- **Top-down view**: Shows the entire fractal shape from a distance
- **Structural overview**: Where large-scale patterns and symmetries are visible
- **Before details**: Circular or amorphous regions with emerging self-similar structure
- **Context first**: Users see the "whole shape" before exploring interesting details

**Not** "most interesting views" - those are for users to discover through exploration!

### What This Service IS:
- ✅ **Immutable fractal metadata** - defines the "forest view" for each fractal
- ✅ **Read-only resource** - loaded from `Resources\InitialConditions.txt`
- ✅ **Application data** - shipped with the app, same for all users
- ✅ **Global context views** - carefully chosen to show overall structure

### What This Service is NOT:
- ❌ **User preferences** - does NOT store where the user last viewed a fractal
- ❌ **Writable data store** - does NOT modify or save changes
- ❌ **Bookmark system** - does NOT track user's favorite positions
- ❌ **"Interesting zoom" catalog** - shows structure, not cherry-picked details
- ❌ **Session state** - does NOT remember viewport changes between runs

## File Location

### Development:
```
ManpCore.Native\Resources\InitialConditions.txt
```

### Deployment:
```
[AppFolder]\Resources\InitialConditions.txt
```

The file is:
- Deployed with the application (via ManpWinUI.csproj Content item)
- Read-only (never modified by the app)
- Same for all users
- Version-controlled in Git

## Data Format

```
# InitialConditions.txt
# Format: FractalName|CenterX|CenterY|Zoom

Mandelbrot|-0.5|0.0|0.8
Julia|0.0|0.0|1.2
BurningShip|0.0|-0.5|1.5
```

Each line defines:
- **FractalName**: Internal name (matches FractalRegistry)
- **CenterX**: X-coordinate of viewport center
- **CenterY**: Y-coordinate of viewport center
- **Zoom**: Initial zoom level (smaller = more zoomed out)

### Zoom Level Philosophy

The zoom values are chosen to show **global structure**:

- **Large zoom values** (e.g., 0.5-2.0): Show the entire fractal's extent
- **Purpose**: Display the "forest" - overall shape and symmetry
- **What users see**: 
  - Circular or amorphous boundary regions
  - Large-scale structural patterns
  - Emerging self-similar features
  - The fractal's "whole shape" before details

**Not optimized for:**
- Close-up details
- "Most beautiful" zoom locations
- Interesting micro-structures

Users discover those through exploration!

## API Usage

```cpp
#include "InitialConditionsService.h"

// Get default viewport for a fractal
InitialConditions ic = InitialConditionsService::Get("Mandelbrot");
double centerX = ic.centerX;  // -0.5
double centerY = ic.centerY;  //  0.0
double zoom = ic.zoom;        //  0.8

// Check if fractal has initial conditions defined
bool exists = InitialConditionsService::Has("Mandelbrot");  // true

// Get count of fractals with initial conditions
size_t count = InitialConditionsService::GetCount();  // 294

// Get all fractal names
std::vector<std::string> names = InitialConditionsService::GetFractalNames();

// Reload from file (e.g., after external edit during development)
InitialConditionsService::Reload();
```

## How Fractals Use This

When registering a fractal in a family file:

```cpp
#include "InitialConditionsService.h"

void RegisterMyFractal()
{
    FractalSpec spec;
    spec.name = "MyFractal";
    spec.displayName = "My Fractal";

    // Load default viewport from InitialConditions service
    InitialConditions ic = InitialConditionsService::Get("MyFractal");
    spec.defaultCenterX = ic.centerX;
    spec.defaultCenterY = ic.centerY;
    spec.defaultZoom = ic.zoom;

    FractalRegistry::Register(spec);
}
```

## User Preferences (Separate Concern)

If you need to save/restore user's viewport position, pan/zoom state, or bookmarks, use a **separate service**:

### Example Architecture:
```
InitialConditionsService  ← Read-only app data (THIS service)
ViewportStateService      ← Runtime viewport position (NOT implemented yet)
BookmarkService           ← User's saved positions (NOT implemented yet)
SettingsService           ← User preferences (EXISTS - AppSettingsService)
```

### Viewport Lifecycle:
1. **App Launch**: Load from `InitialConditionsService.Get(fractalName)`
2. **User Interaction**: Update viewport (pan/zoom/etc.) in UI
3. **Save Bookmark**: Store in `BookmarkService` (user data) - FUTURE FEATURE
4. **Next Session**: Restore from bookmark if user wants - FUTURE FEATURE

## Updating Initial Conditions

To change the default viewport for a fractal:

1. **Edit the file**: Modify `ManpCore.Native\Resources\InitialConditions.txt`
2. **Rebuild**: The file is copied to output directory
3. **Test**: Launch app and select the fractal
4. **Commit**: If good, commit the change to Git

**During development**, you can also call `Reload()` to refresh without restarting the app.

## Migration from Old Design (v1)

### Before (❌ Wrong):
- File copied to user's `AppData\Local\ManpLab\FractalData\`
- `Set()` method allowed modifications
- File was treated as writable user data
- Changes were saved back to file
- Confused application data with user preferences

### After (✅ Correct):
- File stays in `Resources\` folder
- No `Set()` method - read-only API
- File is treated as immutable application data
- Never modified by the application
- Clear separation of concerns

## Benefits of Read-Only Design

1. **Predictable**: All users see the same carefully-chosen initial views
2. **Version-controlled**: Changes tracked in Git
3. **No corruption**: File can't be accidentally modified by app bugs
4. **Clear separation**: App data vs. user data
5. **Simpler code**: No save logic, no file locking, no corruption handling
6. **Safer**: Can't be tampered with by the running application

## Current Status

- **294 fractals** have initial conditions defined
- **All 40 fractal families** use this service
- **File deployment** configured in ManpWinUI.csproj
- **Read-only** since commit 8b032f1 (refactoring completed)
