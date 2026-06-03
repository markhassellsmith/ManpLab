# InitialConditions Service

## Overview

The `InitialConditionsService` provides centralized management of fractal initial view conditions (centerX, centerY, zoom) with persistent storage. This service separates initial conditions from fractal calculation code, making it easier for AI agents and developers to manage and update view parameters.

## Architecture

### Components

1. **InitialConditionsService** - Static service class providing Get/Set operations
2. **InitialConditions** - Struct containing centerX, centerY, and zoom values
3. **Data File** - Pipe-delimited text file storing all initial conditions

### File Locations

- **Development**: `ManpCore.Native\Resources\InitialConditions.txt` (source of truth)
- **Runtime**: `{ExecutableDirectory}\FractalData\InitialConditions.txt` (writable copy)

The service automatically copies from Resources to FractalData on first run if the runtime file doesn't exist.

## Data Format

The data file uses a simple pipe-delimited format:

```
# Fractal Initial Conditions Data File
# Format: FractalName|CenterX|CenterY|Zoom
#
OrbitTrapCross|-0.24|0.0|1.333333
OrbitTrapCircle|-0.38|-0.02|1.333333
AverageDistance|0.0|0.0|0.666667
```

### Format Rules

- Lines starting with `#` are comments
- Empty lines are ignored
- Format: `FractalName|CenterX|CenterY|Zoom`
- Fractal names must be unique
- Numeric values use standard double precision

### Zoom-Viewport Relationship

The zoom value has an inverse relationship with viewport width:
- **viewport_width = 4.0 / defaultZoom**

Examples:
- `zoom = 1.333333` → viewport width = 3.0 (more zoomed in)
- `zoom = 0.666667` → viewport width = 6.0 (more zoomed out)
- `zoom = 0.995025` → viewport width = 4.02

## API Reference

### Get Initial Conditions

Retrieve initial conditions for a fractal:

```cpp
#include "InitialConditionsService.h"

auto conditions = InitialConditionsService::Get("OrbitTrapCross");
spec.defaultCenterX = conditions.centerX;
spec.defaultCenterY = conditions.centerY;
spec.defaultZoom = conditions.zoom;
```

If the fractal doesn't exist, returns default values: `(0.0, 0.0, 1.0)`

### Set Initial Conditions

Add or update initial conditions:

```cpp
// Method 1: Individual parameters
InitialConditionsService::Set("NewFractal", -0.5, 0.3, 1.5);

// Method 2: Using InitialConditions struct
InitialConditions conditions(-0.5, 0.3, 1.5);
InitialConditionsService::Set("NewFractal", conditions);
```

Changes are automatically persisted to the data file.

### Check If Exists

Check if a fractal has stored initial conditions:

```cpp
if (InitialConditionsService::Has("NewFractal"))
{
    // Fractal has initial conditions
}
```

### Utility Methods

```cpp
// Get count of stored fractals
size_t count = InitialConditionsService::GetCount();

// Get all fractal names
std::vector<std::string> names = InitialConditionsService::GetFractalNames();

// Reload from file (after external edits)
InitialConditionsService::Reload();

// Get data file path
std::string path = InitialConditionsService::GetDataFilePath();
```

## Usage Patterns

### Registering a New Fractal

When registering a fractal in a FractalsFamily file:

```cpp
void RegisterMyFractalsFamily()
{
    FractalSpec spec;

    spec.name = "MyNewFractal";
    spec.displayName = "My New Fractal";
    spec.category = "Custom Fractals";
    spec.type = FractalCategory::EscapeTime2D;

    // ... set up calculator, description, etc. ...

    // Get initial conditions from service
    auto initialConditions = InitialConditionsService::Get("MyNewFractal");
    spec.defaultCenterX = initialConditions.centerX;
    spec.defaultCenterY = initialConditions.centerY;
    spec.defaultZoom = initialConditions.zoom;

    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);
}
```

### AI Agent Integration

For AI agents adding new fractals to the application:

```cpp
// Step 1: Store initial conditions
InitialConditionsService::Set("AIGeneratedFractal", 
    -0.75,  // centerX - interesting region
    0.1,    // centerY
    2.0);   // zoom - viewport width will be 2.0

// Step 2: Register the fractal (use Get in registration code as shown above)
```

### Batch Operations

```cpp
// Get all fractals and their conditions
auto names = InitialConditionsService::GetFractalNames();
for (const auto& name : names)
{
    auto conditions = InitialConditionsService::Get(name);
    std::cout << name << ": center=(" 
              << conditions.centerX << "," << conditions.centerY 
              << "), zoom=" << conditions.zoom << "\n";
}
```

## Implementation Details

### Thread Safety

The service is **not thread-safe** in the current implementation. All calls should be made from the main thread during initialization or fractal registration.

### Performance

- **First Access**: Lazy initialization loads the data file into memory
- **Subsequent Gets**: O(log n) map lookup (very fast)
- **Sets**: Writes entire file to disk (use sparingly during runtime)

### Error Handling

- Missing file: Service creates empty registry, file created on first Set
- Malformed lines: Silently skipped during loading
- Missing fractal: Get returns default values (0.0, 0.0, 1.0)

## Maintenance

### Adding Initial Conditions for Existing Fractals

1. Edit `ManpCore.Native\Resources\InitialConditions.txt`
2. Add line: `FractalName|CenterX|CenterY|Zoom`
3. Update the fractal registration code to use `InitialConditionsService::Get()`

### Updating Initial Conditions

**Option 1: Edit Source File** (Preferred)
1. Edit `ManpCore.Native\Resources\InitialConditions.txt`
2. Delete runtime file to force refresh: `{ExecutableDirectory}\FractalData\InitialConditions.txt`
3. Restart application

**Option 2: Programmatic Update**
```cpp
InitialConditionsService::Set("ExistingFractal", newCenterX, newCenterY, newZoom);
```

### Migrating More Fractals

To migrate additional fractal families to use the service:

1. Add entries to `Resources\InitialConditions.txt`
2. Update the fractal family file to include: `#include "InitialConditionsService.h"`
3. Replace hardcoded values with `InitialConditionsService::Get()` calls
4. Remove inline comments about viewport tuning (kept in data file header)

## Future Enhancements

Potential improvements for future versions:

- JSON format with schema validation
- Version tracking and migration support
- Bulk import/export utilities
- Integration with fractal metadata service
- Undo/redo for condition changes
- Historical tracking of condition updates
- UI for editing conditions without code changes

## See Also

- [FractalRegistry Documentation](../FractalRegistry.h)
- [Fractal Metadata Service](../../ManpWinUI/Documentation/Fractals/FRACTAL_METADATA_SUMMARY.md)
- [Adding New Fractals Guide](../../ManpWinUI/Documentation/Fractals/METADATA_POPULATION_MAINTENANCE.md)
