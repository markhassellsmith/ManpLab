# Developer Infrastructure for Adding Fractals

This document describes the complete infrastructure for easily adding new fractals to ManpLab.

---

## Overview

ManpLab provides a comprehensive system for adding new fractals with minimal boilerplate. The infrastructure includes:

1. **C++ Template** - Copy-paste fractal implementation template
2. **Automated Generator** - PowerShell script to scaffold new families
3. **Comprehensive Guide** - Step-by-step documentation
4. **JSON Metadata System** - Rich educational content
5. **Build Integration** - Automatic project configuration

---

## Quick Start: Adding a New Fractal (3 Minutes)

### Option A: Using the Generator (Easiest)

```powershell
# Navigate to the solution directory
cd ManpLab

# Run the generator
.\ManpWinUI\Scripts\New-FractalFamily.ps1 -FamilyName "Sierpinski" -Category "IFS Fractals"

# Optional: Interactive mode asks questions
.\ManpWinUI\Scripts\New-FractalFamily.ps1 -FamilyName "Sierpinski" -Interactive
```

The generator will:
- ✅ Create `ManpCore.Native/SierpinskiFamily.cpp` from template
- ✅ Update `FractalRegistry.cpp` with registration calls
- ✅ Add to `ManpCore.Native.vcxproj` build configuration
- ✅ Create `SierpinskiFamily_metadata.json` for documentation

### Option B: Manual Method

1. Copy `ManpCore.Native/FractalTemplate.cpp.template`
2. Rename to `YourFractalFamily.cpp`
3. Search/replace `{{FAMILY_NAME}}` and `{{CATEGORY}}`
4. Implement the iteration formula
5. Add to build system manually

---

## File Structure

```
ManpLab/
├── ManpCore.Native/
│   ├── FractalTemplate.cpp.template      # Copy-paste template
│   ├── ADDING_FRACTALS.md                # Detailed developer guide
│   ├── FractalRegistry.h                 # Schema definitions
│   ├── FractalRegistry.cpp               # Central registration
│   ├── MandelbrotFamily.cpp              # Reference implementation
│   ├── BurningShipFamily.cpp             # Reference implementation
│   └── [YourFractal]Family.cpp           # Your implementation
│
├── ManpWinUI/
│   ├── Scripts/
│   │   └── New-FractalFamily.ps1         # Automated generator
│   ├── Assets/
│   │   └── FractalKnowledge/
│   │       ├── schema.json               # JSON Schema
│   │       ├── fractals_sample.json      # Examples
│   │       └── [YourFractal]_metadata.json  # Your documentation
│   └── docs/
│       └── FRACTAL_DEVELOPER_INFRASTRUCTURE.md  # This file
```

---

## Infrastructure Components

### 1. C++ Template (`FractalTemplate.cpp.template`)

**Purpose**: Provides a complete, copy-paste-ready skeleton for new fractal implementations.

**Features**:
- All required `FractalSpec` fields with comments
- Example iteration formula
- Julia mode support skeleton
- Custom parameter examples
- Smooth coloring implementation
- Extended metadata placeholders

**Usage**:
```bash
cp ManpCore.Native/FractalTemplate.cpp.template ManpCore.Native/MyFractalFamily.cpp
# Edit and replace {{FAMILY_NAME}} and {{CATEGORY}}
```

---

### 2. Generator Script (`New-FractalFamily.ps1`)

**Purpose**: Automate the entire scaffolding process.

**Features**:
- Creates .cpp file from template
- Updates FractalRegistry.cpp automatically
- Adds to .vcxproj build configuration
- Generates JSON metadata template
- Interactive mode for customization

**Usage**:
```powershell
# Basic usage
.\ManpWinUI\Scripts\New-FractalFamily.ps1 -FamilyName "Newton" -Category "Newton Fractals"

# Interactive mode (asks questions)
.\ManpWinUI\Scripts\New-FractalFamily.ps1 -FamilyName "Newton" -Interactive

# Help
Get-Help .\ManpWinUI\Scripts\New-FractalFamily.ps1 -Detailed
```

**Parameters**:
- `-FamilyName` (Required): Name of the fractal family (e.g., "Newton", "Sierpinski")
- `-Category` (Optional): Browser category (default: "Other Fractals")
- `-Interactive` (Switch): Enable interactive mode with prompts

**What It Does**:

1. **Creates .cpp file** with your family name
   ```
   ManpCore.Native/NewtonFamily.cpp
   ```

2. **Updates FractalRegistry.cpp**:
   ```cpp
   // Adds forward declaration
   void RegisterNewtonFamily();

   // Adds registration call in InitializeBuiltins()
   RegisterNewtonFamily();
   ```

3. **Updates .vcxproj**:
   ```xml
   <ClCompile Include="NewtonFamily.cpp" />
   ```

4. **Creates metadata template**:
   ```
   ManpWinUI/Assets/FractalKnowledge/Newton_metadata.json
   ```

---

### 3. Developer Guide (`ADDING_FRACTALS.md`)

**Purpose**: Comprehensive documentation for fractal developers.

**Contents**:
- Quick start guide
- Architecture overview
- FractalSpec field reference
- Calculator function patterns
- Custom parameter system
- Complex number helpers
- Testing procedures
- Common patterns and examples
- Troubleshooting guide

**Sections**:
1. **Quick Start (5 Minutes)** - Get up and running fast
2. **Detailed Guide** - In-depth architecture explanation
3. **FractalSpec Fields Reference** - Complete field documentation
4. **Calculator Function Guide** - Iteration formula patterns
5. **Custom Parameters** - How to add user-configurable parameters
6. **Algorithm Patterns** - Common fractal types with code
7. **Testing** - How to verify your implementation
8. **Reference Implementations** - Study existing fractals

---

### 4. JSON Schema (`FractalKnowledge/schema.json`)

**Purpose**: Formal schema definition for fractal metadata validation.

**Features**:
- JSON Schema draft-07 compliant
- Documents all metadata fields
- Type validation
- Required vs. optional fields

**Usage**:
```bash
# Validate your metadata
npm install -g ajv-cli
ajv validate -s schema.json -d YourFractal_metadata.json
```

---

### 5. Sample Metadata (`fractals_sample.json`)

**Purpose**: High-quality examples showing target documentation level.

**Includes**:
- Mandelbrot Set - Complete documentation
- Burning Ship - Variant with abs() operations
- Julia - San Marco - Pre-set Julia constant

**Fields Demonstrated**:
- Rich mathematical formulas (plain text + LaTeX)
- Historical attribution (discoverer, year)
- Visual characteristics descriptions
- Suggested viewpoints with coordinates
- Related fractals links
- Academic references

---

## Development Workflow

### End-to-End Example: Adding "Newton's Method"

#### Step 1: Generate Scaffold (30 seconds)

```powershell
cd ManpLab
.\ManpWinUI\Scripts\New-FractalFamily.ps1 -FamilyName "Newton" -Category "Newton Fractals" -Interactive
```

**Interactive prompts**:
```
Category name: Newton Fractals
Supports Julia mode? (y/n): n
Has symmetry? (y/n): y
Default center X: 0.0
Default center Y: 0.0
Default zoom: 2.0
```

#### Step 2: Implement Formula (5 minutes)

Edit `ManpCore.Native/NewtonFamily.cpp`:

```cpp
spec.calculator = [](ComplexD c, int maxIter, bool isJulia, 
                     ComplexD juliaC, const ParamMap& params) -> double {
    ComplexD z = c;
    ComplexD one(1.0, 0.0);

    for (int iter = 0; iter < maxIter; iter++) {
        // f(z) = z³ - 1
        ComplexD f = z * z * z - one;

        // f'(z) = 3z²
        ComplexD fp = ComplexD(3.0, 0.0) * z * z;

        // Newton's method: z = z - f(z)/f'(z)
        z = z - f / fp;

        // Check convergence to root
        if ((z - one).magnitude() < 0.001)
            return iter;
    }
    return maxIter;
};
```

#### Step 3: Add Metadata (5 minutes)

Edit `ManpWinUI/Assets/FractalKnowledge/Newton_metadata.json`:

```json
{
  "name": "NewtonCubic",
  "displayName": "Newton - Cubic Roots",
  "category": "Newton Fractals",
  "description": "Newton's method applied to z³ - 1 = 0",
  "formula": "z_{n+1} = z_n - f(z_n)/f'(z_n), f(z) = z³ - 1",
  "discoveredBy": "Isaac Newton",
  "discoveryYear": 1669,
  "visualCharacteristics": "Three basins of attraction forming 120° rotational symmetry",
  "references": [
    "https://en.wikipedia.org/wiki/Newton_fractal"
  ]
}
```

#### Step 4: Build and Test (1 minute)

```powershell
dotnet build
# Run ManpWinUI
# Navigate to "Newton Fractals" in browser
# Click your fractal
# Should render immediately!
```

**Total Time: ~12 minutes** from idea to working fractal

---

## Advanced Features

### Custom Parameters

Add user-configurable parameters:

```cpp
spec.parameters = {
    ParameterSpec(
        "exponent",                    // Internal name
        "Exponent",                    // Display name
        "Power of polynomial",         // Description
        ParameterType::Float,
        ParameterCategory::Calculation,
        "3.0",                         // Default
        2.0, 10.0,                     // Min, max
        0.5                            // Step
    )
};

// Use in calculator
spec.calculator = [](ComplexD c, int maxIter, bool isJulia, 
                     ComplexD juliaC, const ParamMap& params) -> double {
    double exp = params.count("exponent") > 0 ? params.at("exponent") : 3.0;

    // Use exp in formula...
    ComplexD f = z.pow(exp) - ComplexD(1.0, 0.0);
    ComplexD fp = ComplexD(exp, 0.0) * z.pow(exp - 1.0);
    z = z - f / fp;
};
```

### Julia Set Support

```cpp
spec.supportsJulia = true;

spec.calculator = [](ComplexD c, int maxIter, bool isJulia, 
                     ComplexD juliaC, const ParamMap& params) -> double {
    // In Mandelbrot mode: vary c, start with z=0
    // In Julia mode: fix c=juliaC, vary z=c
    ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
    ComplexD constant = isJulia ? juliaC : c;

    for (int iter = 0; iter < maxIter; iter++) {
        z = z * z + constant;
        // ... rest of calculation
    }
};
```

### Multiple Variants

Add multiple fractals in one family:

```cpp
void RegisterNewtonFamily()
{
    FractalSpec spec;

    // Variant 1: Cubic
    spec.name = "NewtonCubic";
    spec.displayName = "Newton - Cubic Roots";
    // ... implementation
    FractalRegistry::Register(spec);

    // Variant 2: Quartic
    spec.name = "NewtonQuartic";
    spec.displayName = "Newton - Quartic Roots";
    // ... implementation
    FractalRegistry::Register(spec);

    // Variant 3: Sin(z)
    spec.name = "NewtonSin";
    spec.displayName = "Newton - Sin(z)";
    // ... implementation
    FractalRegistry::Register(spec);
}
```

---

## Testing Infrastructure

### Unit Testing (Future)

Location: `ManpCore.Tests/FractalTests/`

```csharp
[TestClass]
public class NewtonFractalTests
{
    [TestMethod]
    public void Newton_ConvergesToRoot()
    {
        var registry = FractalRegistryWrapper.Initialize();
        var fractal = registry.GetFractalInfo("NewtonCubic");

        // Test calculation at a known convergent point
        var result = CalculatePoint(fractal, new ComplexD(0.5, 0.5), 100);
        Assert.IsTrue(result < 100);  // Should converge
    }
}
```

### Manual Testing Checklist

- [ ] Fractal appears in browser under correct category
- [ ] Name and description are correct
- [ ] Clicking renders the fractal
- [ ] Default view shows interesting features
- [ ] Julia mode works (if supported)
- [ ] Custom parameters appear and function (if defined)
- [ ] Zooming works smoothly
- [ ] Deep zoom doesn't crash or produce artifacts
- [ ] Info panel shows metadata correctly

---

## Best Practices

### 1. Naming Conventions

- **Family Name**: PascalCase, descriptive (e.g., `Newton`, `Sierpinski`, `Phoenix`)
- **Variant Names**: FamilyName + Variant (e.g., `NewtonCubic`, `PhoenixM`)
- **Display Names**: Human-readable (e.g., "Newton - Cubic Roots")
- **Categories**: Plural nouns (e.g., "Newton Fractals", "Julia Sets")

### 2. Code Organization

- One .cpp file per fractal family
- Group related variants together
- Use helper functions for complex operations
- Comment unusual mathematical operations

### 3. Documentation

- **Short description**: 1-2 sentences for tooltips
- **Formula**: Plain text mathematical notation
- **Derivation**: Paragraph explaining the mathematics
- **Viewpoints**: At least 3 interesting coordinates
- **References**: Wikipedia + academic papers if available

### 4. Performance

- Use `magnitudeSquared()` instead of `magnitude()` when possible
- Set `hasSymmetry = true` if fractal has axis symmetry
- Avoid expensive operations (trig, exp) in tight loops
- Cache frequently-used values

---

## Common Patterns

### Escape-Time Fractals (Mandelbrot-like)

```cpp
for (int iter = 0; iter < maxIter; iter++) {
    z = /* iteration formula */;
    if (z.magnitudeSquared() > bailoutSquared)
        return iter + smoothing;
}
return maxIter;
```

### Newton Method Fractals

```cpp
for (int iter = 0; iter < maxIter; iter++) {
    ComplexD f = /* function */;
    ComplexD fp = /* derivative */;
    z = z - f / fp;
    if (/* converged to root */)
        return iter;
}
return maxIter;
```

### IFS / Attractor Fractals

```cpp
// Different algorithm - track visited points
std::vector<ComplexD> path;
ComplexD z = c;
for (int iter = 0; iter < maxIter; iter++) {
    // Apply random IFS transformation
    z = ApplyIFSRule(z, rand() % numRules);
    path.push_back(z);
}
// Color based on path density
```

---

## Troubleshooting

### Generator Issues

**Problem**: Script fails with "Template not found"
**Solution**: Run from solution root or check paths

**Problem**: "File already exists"
**Solution**: Use `-Force` or delete existing file

### Build Issues

**Problem**: Linker error "unresolved external"
**Solution**: Add forward declaration in FractalRegistry.cpp

**Problem**: C2065: undeclared identifier
**Solution**: Add `#include "FractalRegistry.h"`

### Runtime Issues

**Problem**: Fractal doesn't appear in browser
**Solution**: Check InitializeBuiltins() has your Register call

**Problem**: All black rendering
**Solution**: Check bailout condition and return values

---

## Future Enhancements

### Planned Infrastructure Improvements

1. **Visual Formula Editor** - GUI for building iteration formulas
2. **Hot Reload** - Update fractals without rebuilding
3. **Formula Language** - DSL for defining iterations
4. **Template Gallery** - More specialized templates
5. **Automated Testing** - Unit tests for fractal calculations
6. **Performance Profiler** - Identify slow calculations
7. **Documentation Generator** - Auto-generate docs from code

---

## Summary

The fractal infrastructure provides:

✅ **C++ Template** - Copy-paste skeleton  
✅ **PowerShell Generator** - Automated scaffolding  
✅ **Comprehensive Guide** - Step-by-step instructions  
✅ **JSON Schema** - Metadata validation  
✅ **Sample Implementations** - Reference code  

**Result**: Adding a new fractal takes ~10-15 minutes instead of hours.

---

## Support

- **Documentation**: `ManpCore.Native/ADDING_FRACTALS.md`
- **Examples**: Reference `MandelbrotFamily.cpp`, `NewtonFamily.cpp`
- **Generator Help**: `Get-Help .\Scripts\New-FractalFamily.ps1 -Detailed`
- **Issues**: Open GitHub issue with `[Fractal Development]` tag

---

**Happy fractal development! 🌀✨**
