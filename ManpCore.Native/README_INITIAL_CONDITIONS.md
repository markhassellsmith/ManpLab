# Initial Conditions Service - Implementation Summary

## Overview

This restructuring extracts fractal initial view conditions (centerX, centerY, zoom) from hardcoded values in fractal registration code into a centralized service with persistent storage.

## What Changed

### New Files Created

1. **InitialConditionsService.h** - Service interface
   - `Get()` - Retrieve initial conditions by fractal name
   - `Set()` - Store/update initial conditions and persist to file
   - `Has()` - Check if conditions exist
   - Utility methods: `GetCount()`, `GetFractalNames()`, `Reload()`

2. **InitialConditionsService.cpp** - Implementation
   - Lazy initialization from data file
   - Automatic resource file copying (Resources → FractalData)
   - Pipe-delimited text format for easy editing
   - Thread-safe singleton pattern

3. **Resources\InitialConditions.txt** - Source data file
   - Contains all initial conditions for OrbitalFractalsFamily (8 fractals)
   - Pipe-delimited format: `FractalName|CenterX|CenterY|Zoom`
   - Comments explain zoom-viewport relationship

4. **Documentation\INITIAL_CONDITIONS_SERVICE.md** - Complete API documentation

5. **AI_AGENT_FRACTAL_TEMPLATE.h** - Quick reference for AI agents

6. **Scripts\ExtractInitialConditions.ps1** - Utility to extract conditions from existing code

### Modified Files

1. **OrbitalFractalsFamily.cpp**
   - Added: `#include "InitialConditionsService.h"`
   - Changed: All 8 fractals now use `InitialConditionsService::Get()`
   - Removed: Hardcoded centerX, centerY, zoom values
   - Removed: Inline comments about viewport tuning (now in data file)

2. **ManpCore.Native.vcxproj**
   - Added InitialConditionsService.h to ClInclude items
   - Added InitialConditionsService.cpp to ClCompile items
   - Added Resources\InitialConditions.txt as None item

## Benefits

### For Development

1. **Centralized Management**: All initial conditions in one place
2. **Easy Editing**: Simple text format, no recompilation needed
3. **Documentation**: Zoom-viewport relationship explained in data file
4. **Version Control**: Initial conditions tracked separately from code

### For AI Agents

1. **Simple API**: Single call to store conditions: `Set(name, x, y, zoom)`
2. **Automatic Persistence**: Changes immediately saved to file
3. **Clear Template**: AI_AGENT_FRACTAL_TEMPLATE.h provides complete guidance
4. **No Code Duplication**: Reuse same service across all fractals

### For Users

1. **Customizable**: Can edit initial conditions without recompiling
2. **Portable**: Conditions file can be shared/backed up
3. **Consistent**: Same mechanism for all fractals

## File Structure

```
ManpCore.Native/
├── InitialConditionsService.h       [Service interface]
├── InitialConditionsService.cpp     [Implementation]
├── AI_AGENT_FRACTAL_TEMPLATE.h      [AI agent guide]
├── OrbitalFractalsFamily.cpp        [Updated to use service]
├── Resources/
│   └── InitialConditions.txt        [Source data - version controlled]
├── Documentation/
│   └── INITIAL_CONDITIONS_SERVICE.md [API documentation]
└── Scripts/
    └── ExtractInitialConditions.ps1 [Migration utility]

Runtime:
{ExecutableDirectory}/
└── FractalData/
    └── InitialConditions.txt        [Writable runtime copy]
```

## Data Format

### File Format

```
# Comment lines start with #
FractalName|CenterX|CenterY|Zoom

# Examples:
OrbitTrapCross|-0.24|0.0|1.333333
OrbitTrapCircle|-0.38|-0.02|1.333333
AverageDistance|0.0|0.0|0.666667
```

### Zoom-Viewport Relationship

The constant relationship: **viewport_width = 4.0 / zoom**

| Zoom      | Viewport Width | Description      |
|-----------|----------------|------------------|
| 4.0       | 1.0           | Very zoomed in   |
| 2.0       | 2.0           | Zoomed in        |
| 1.333333  | 3.0           | Moderate         |
| 1.0       | 4.0           | Default          |
| 0.666667  | 6.0           | Zoomed out       |
| 0.5       | 8.0           | Very zoomed out  |

## Usage Examples

### Retrieving Initial Conditions (Fractal Registration)

```cpp
#include "InitialConditionsService.h"

void RegisterMyFractalsFamily()
{
    FractalSpec spec;
    spec.name = "MyFractal";
    // ... other setup ...

    // Get initial conditions from service
    auto initialConditions = InitialConditionsService::Get("MyFractal");
    spec.defaultCenterX = initialConditions.centerX;
    spec.defaultCenterY = initialConditions.centerY;
    spec.defaultZoom = initialConditions.zoom;

    FractalRegistry::Register(spec);
}
```

### Storing Initial Conditions (AI Agent)

```cpp
// Store initial conditions for a new fractal
InitialConditionsService::Set("NewFractal", 
    -0.5,   // centerX - interesting region
    0.3,    // centerY
    1.5);   // zoom - viewport width will be 2.67

// The change is automatically persisted to:
// FractalData\InitialConditions.txt
```

### Checking and Updating

```cpp
// Check if exists
if (!InitialConditionsService::Has("MyFractal"))
{
    // Set default conditions
    InitialConditionsService::Set("MyFractal", 0.0, 0.0, 1.0);
}

// Update existing
InitialConditionsService::Set("MyFractal", -0.75, 0.1, 2.0);
```

## Migration Guide

To migrate additional fractal families:

### Step 1: Extract Current Values

Run the extraction script:
```powershell
cd ManpCore.Native
.\Scripts\ExtractInitialConditions.ps1 > extracted.txt
```

### Step 2: Add to Data File

Copy entries from extracted.txt to `Resources\InitialConditions.txt`

### Step 3: Update Family File

```cpp
// Before:
spec.defaultCenterX = -0.24;
spec.defaultCenterY = 0.0;
spec.defaultZoom = 1.333333;  // Viewport tuning: X scale 3.0

// After:
#include "InitialConditionsService.h"  // At top of file

auto initialConditions = InitialConditionsService::Get("FractalName");
spec.defaultCenterX = initialConditions.centerX;
spec.defaultCenterY = initialConditions.centerY;
spec.defaultZoom = initialConditions.zoom;
```

### Step 4: Build and Test

```bash
# Build project
msbuild ManpLab.sln

# Run and verify fractal appears with correct initial view
```

## Implementation Status

### Completed
- ✅ InitialConditionsService implementation
- ✅ OrbitalFractalsFamily migration (8 fractals)
- ✅ Data file with initial values
- ✅ Documentation
- ✅ AI agent template
- ✅ Build verification

### Remaining (Future Work)
- ⏳ Migrate other fractal families (~60+ fractals)
- ⏳ Add UI for editing conditions
- ⏳ Version tracking and migration
- ⏳ JSON format with schema validation
- ⏳ Bulk import/export utilities

## Testing

### Build Test
```bash
cd ManpLab
msbuild ManpLab.sln
# Status: ✅ PASSED
```

### Runtime Test Checklist
- [ ] Application starts without errors
- [ ] OrbitalFractalsFamily fractals appear in fractal list
- [ ] Initial views match expected coordinates
- [ ] Zoom levels are correct (verify viewport width on status bar)
- [ ] Data file is created at: FractalData\InitialConditions.txt
- [ ] Can manually edit data file and reload works

## Notes for AI Agents

When adding new fractals:

1. **Always store initial conditions first**:
   ```cpp
   InitialConditionsService::Set("YourFractalName", centerX, centerY, zoom);
   ```

2. **Use Get() in fractal registration**:
   ```cpp
   auto ic = InitialConditionsService::Get("YourFractalName");
   spec.defaultCenterX = ic.centerX;
   spec.defaultCenterY = ic.centerY;
   spec.defaultZoom = ic.zoom;
   ```

3. **Calculate zoom from desired viewport width**:
   ```cpp
   double desiredViewportWidth = 3.0;
   double zoom = 4.0 / desiredViewportWidth;  // = 1.333333
   ```

4. **Reference the template**: See `AI_AGENT_FRACTAL_TEMPLATE.h` for complete example

## Troubleshooting

### Data file not found
- Check: Does `Resources\InitialConditions.txt` exist?
- Service will create empty file at `FractalData\InitialConditions.txt` on first Set()

### Wrong initial view
- Verify fractal name matches exactly (case-sensitive)
- Check data file has entry: `FractalName|X|Y|Zoom`
- Try: `InitialConditionsService::Reload()`

### Zoom seems inverted
- Remember: zoom = 4.0 / viewport_width
- Higher zoom = more zoomed in = smaller viewport
- Lower zoom = more zoomed out = larger viewport

## Related Documentation

- [FractalRegistry.h](FractalRegistry.h) - Fractal registration system
- [INITIAL_CONDITIONS_SERVICE.md](Documentation/INITIAL_CONDITIONS_SERVICE.md) - API reference
- [AI_AGENT_FRACTAL_TEMPLATE.h](AI_AGENT_FRACTAL_TEMPLATE.h) - Quick start guide

## Author Notes

This restructuring maintains the consistent zoom-viewport relationship (4.0 constant) discovered in the original code, while making it easier to manage and document. The service is designed to be simple, reliable, and easy for AI agents to use when adding new fractals to the application.

**Key Design Decisions:**
- Simple text format over JSON (no dependencies, easy to edit)
- Lazy initialization (load on first use)
- Automatic resource copying (seamless development to deployment)
- Explicit Get/Set API (clear intent, no magic)
- Runtime persistence (changes immediately saved)

Last Updated: 2026-05-04
Implementation: ManpLab v0.1 (Development Branch)
