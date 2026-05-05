# Adding New Fractals to ManpLab

This guide explains how to add new fractal types to ManpLab's registry-based system.

## Quick Start (5 Minutes)

### Step 1: Create Your Fractal Family File

```bash
# Copy the template
cp ManpCore.Native/FractalTemplate.cpp.template ManpCore.Native/MyFractalFamily.cpp

# Open and edit
code ManpCore.Native/MyFractalFamily.cpp
```

### Step 2: Implement Your Fractal

Replace `{{FAMILY_NAME}}`, `{{CATEGORY}}`, and fill in the TODO sections:

```cpp
spec.name = "Sierpinski";
spec.displayName = "Sierpinski Triangle";
spec.category = "IFS Fractals";
spec.description = "Classic IFS fractal forming a triangle";

spec.calculator = [](ComplexD c, int maxIter, ...) -> double {
    // Your iteration formula here
    ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
    for (int iter = 0; iter < maxIter; iter++) {
        z = z * z + constant;  // Your formula
        if (z.magnitude() > 256.0) return iter;
    }
    return maxIter;
};
```

### Step 3: Register in Build System

Add to `ManpCore.Native/ManpCore.Native.vcxproj`:

```xml
<ItemGroup>
  <ClCompile Include="MyFractalFamily.cpp" />
</ItemGroup>
```

### Step 4: Register at Startup

Add to `ManpCore.Native/FractalRegistry.cpp`:

```cpp
void FractalRegistry::InitializeBuiltins()
{
    // ... existing registrations ...
    RegisterMyFractalFamily();  // Add this line
}
```

Declare at the top:

```cpp
void RegisterMyFractalFamily();  // Add this line
```

### Step 5: Add Metadata (Optional but Recommended)

Add to `ManpWinUI/Assets/FractalKnowledge/fractals.json`:

```json
{
  "name": "Sierpinski",
  "displayName": "Sierpinski Triangle",
  "category": "IFS Fractals",
  "description": "Classic IFS fractal...",
  "formula": "IFS with three affine transforms",
  "discoveredBy": "Wacław Sierpiński",
  "discoveryYear": 1915,
  "references": [
    "https://en.wikipedia.org/wiki/Sierpinski_triangle"
  ]
}
```

### Step 6: Build and Test

```bash
# Build the solution
dotnet build

# Your fractal should appear in the browser panel!
```

---

## Detailed Guide

### Understanding the Architecture

```
┌─────────────────────────────────────────────────────┐
│  Your Implementation (C++)                          │
│  • YourFractalFamily.cpp                            │
│  • Implements calculation function                  │
│  • Fills FractalSpec with metadata                  │
└────────────────┬────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────┐
│  FractalRegistry (C++)                              │
│  • Central registry of all fractals                 │
│  • Stores specs and calculators                     │
│  • Provides lookup by name/category                 │
└────────────────┬────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────┐
│  FractalRegistryWrapper (C++/CLI)                   │
│  • Marshals to managed code                         │
│  • Exposes FractalInfo to C#                        │
└────────────────┬────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────┐
│  WinUI Browser (C#)                                 │
│  • Displays fractals in categories                  │
│  • Shows metadata on selection                      │
│  • Triggers rendering                               │
└─────────────────────────────────────────────────────┘
```

### FractalSpec Fields Reference

#### Required Fields

| Field | Type | Description | Example |
|-------|------|-------------|---------|
| `name` | `string` | Unique ID (no spaces) | `"BurningShip"` |
| `displayName` | `string` | UI display name | `"Burning Ship"` |
| `category` | `string` | Browser grouping | `"Mandelbrot Variants"` |
| `type` | `FractalCategory` | Algorithm type | `EscapeTime2D` |
| `calculator` | `FractalCalculator` | Calculation function | Lambda function |

#### View Settings

| Field | Type | Description | Default |
|-------|------|-------------|---------|
| `defaultCenterX` | `double` | Initial center X | `0.0` |
| `defaultCenterY` | `double` | Initial center Y | `0.0` |
| `defaultZoom` | `double` | Initial zoom | `1.0` |
| `defaultBailout` | `double` | Escape radius | `256.0` |

#### Features

| Field | Type | Description |
|-------|------|-------------|
| `supportsJulia` | `bool` | Can render Julia sets? |
| `hasSymmetry` | `bool` | Has x/y axis symmetry? |
| `parameters` | `vector<ParameterSpec>` | Custom parameters |

#### Educational Metadata

| Field | Type | Description |
|-------|------|-------------|
| `description` | `string` | Short tooltip |
| `formula` | `string` | Plain text formula |
| `formulaLatex` | `string` | LaTeX formula |
| `derivation` | `string` | Mathematical explanation |
| `visualCharacteristics` | `string` | Visual description |
| `discoveredBy` | `string` | Historical attribution |
| `discoveryYear` | `int` | Year discovered |
| `computationalNotes` | `string` | Performance notes |
| `suggestedViewpoints` | `vector<string>` | Interesting coordinates |
| `relatedFractals` | `vector<string>` | Related fractals |
| `references` | `vector<string>` | Links/papers |

---

## Calculator Function Guide

### Function Signature

```cpp
FractalCalculator = [](ComplexD c, int maxIter, bool isJulia, 
                       ComplexD juliaC, const ParamMap& params) -> double
```

### Parameters

- **`c`**: The complex point being calculated
- **`maxIter`**: Maximum iterations before giving up
- **`isJulia`**: True if rendering Julia set mode
- **`juliaC`**: Julia constant (only used if `isJulia == true`)
- **`params`**: Custom parameters (access via `params.at("name")`)

### Return Value

- **Smooth iteration count** (0.0 to maxIter)
- Return `maxIter` if point is in the set (doesn't escape)
- Use smooth coloring formula for gradients

### Algorithm Patterns

#### 1. Standard Escape-Time (Mandelbrot-like)

```cpp
spec.calculator = [](ComplexD c, int maxIter, bool isJulia, 
                     ComplexD juliaC, const ParamMap& params) -> double {
    ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
    ComplexD constant = isJulia ? juliaC : c;

    double bailoutSquared = 256.0 * 256.0;

    for (int iter = 0; iter < maxIter; iter++) {
        // Your iteration formula
        z = z * z + constant;

        // Check escape
        double magSq = z.real * z.real + z.imag * z.imag;
        if (magSq > bailoutSquared) {
            // Smooth coloring
            double log_zn = log(magSq) / 2.0;
            double nu = log(log_zn / log(2.0)) / log(2.0);
            return iter + 1 - nu;
        }
    }
    return maxIter;
};
```

#### 2. Absolute Value Variants (Burning Ship-like)

```cpp
z = z * z + constant;

// Take absolute value of components
z.real = std::abs(z.real);
z.imag = std::abs(z.imag);
```

#### 3. Trigonometric Variants

```cpp
// sin(z) = ComplexD(sin(z.real) * cosh(z.imag), cos(z.real) * sinh(z.imag))
z = ComplexD(
    sin(z.real) * cosh(z.imag),
    cos(z.real) * sinh(z.imag)
) + constant;
```

#### 4. Newton Method (Root-Finding)

```cpp
// f(z) = z^3 - 1
// f'(z) = 3z^2
// z_{n+1} = z_n - f(z)/f'(z)

for (int iter = 0; iter < maxIter; iter++) {
    ComplexD f = z * z * z - ComplexD(1.0, 0.0);   // f(z)
    ComplexD fp = ComplexD(3.0, 0.0) * z * z;      // f'(z)
    z = z - f / fp;

    // Check convergence to roots
    double dist = (z - ComplexD(1.0, 0.0)).magnitude();
    if (dist < 0.001) return iter;
}
```

#### 5. Phoenix Family (Multiple Terms)

```cpp
ComplexD z_old = ComplexD(0.0, 0.0);
ComplexD z = ComplexD(0.0, 0.0);

for (int iter = 0; iter < maxIter; iter++) {
    ComplexD temp = z;
    z = z * z + c + p * z_old;  // Phoenix formula
    z_old = temp;

    if (z.magnitudeSquared() > bailoutSquared)
        return iter;
}
```

---

## Custom Parameters

### Defining Parameters

```cpp
spec.parameters = {
    // Numeric parameter
    ParameterSpec(
        "exponent",                    // Internal name
        "Exponent",                    // Display name
        "Power of the iteration",      // Description
        ParameterType::Float,
        ParameterCategory::Calculation,
        "2.0",                         // Default value (as string)
        1.0, 10.0,                     // Min, max
        0.1                            // Step size
    ),

    // Boolean parameter
    ParameterSpec(
        "useAbsolute",
        "Use Absolute Value",
        "Apply abs() to components",
        ParameterType::Boolean,
        ParameterCategory::Calculation,
        "false"                        // Default
    ),

    // Choice parameter
    ParameterSpec(
        "variant",
        "Variant",
        {"Standard", "Modified", "Experimental"},  // Choices
        "Select algorithm variant"
    )
};
```

### Using Parameters in Calculator

```cpp
spec.calculator = [](ComplexD c, int maxIter, bool isJulia, 
                     ComplexD juliaC, const ParamMap& params) -> double {
    // Read parameters
    double exponent = params.count("exponent") > 0 
        ? params.at("exponent") 
        : 2.0;  // Default

    bool useAbsolute = params.count("useAbsolute") > 0 
        ? (params.at("useAbsolute") > 0.5) 
        : false;

    // Use in iteration
    ComplexD z = c;
    for (int iter = 0; iter < maxIter; iter++) {
        z = z.pow(exponent) + constant;

        if (useAbsolute) {
            z.real = std::abs(z.real);
            z.imag = std::abs(z.imag);
        }

        // ... rest of calculation
    }
};
```

---

## Categories Reference

### Existing Categories

- **"Classic Fractals"** - Mandelbrot, Julia sets
- **"Mandelbrot Variants"** - Burning Ship, Tricorn, Bird of Prey
- **"Julia Sets"** - Pre-set Julia constants
- **"Multibrot Family"** - Mandelbrot with different exponents
- **"Newton Fractals"** - Root-finding algorithms
- **"Phoenix Family"** - Multi-term iterations
- **"Magnet Family"** - Magnet fractals
- **"Barnsley Fractals"** - Barnsley ferns and variations
- **"Trigonometric Fractals"** - Trig function iterations
- **"3D Attractors"** - Lorenz, Rössler, etc.
- **"IFS Fractals"** - Iterated Function Systems
- **"Exotic Fractals"** - Experimental/unique types

### Creating New Categories

Just use a new string! Categories are created dynamically:

```cpp
spec.category = "My New Category";
```

It will automatically appear in the browser panel.

---

## Complex Number Helper

### ComplexD Structure

```cpp
struct ComplexD {
    double real;
    double imag;

    ComplexD(double r, double i) : real(r), imag(i) {}

    // Operators: +, -, *, /
    // Methods: magnitude(), magnitudeSquared(), pow(double)
};
```

### Common Operations

```cpp
// Arithmetic
ComplexD a(1.0, 2.0);
ComplexD b(3.0, 4.0);
ComplexD sum = a + b;
ComplexD product = a * b;

// Magnitude
double mag = z.magnitude();           // sqrt(real² + imag²)
double magSq = z.magnitudeSquared(); // real² + imag²

// Power
ComplexD squared = z * z;
ComplexD cubed = z * z * z;
ComplexD power = z.pow(3.5);  // Fractional exponents

// Absolute value
ComplexD abs_z = ComplexD(std::abs(z.real), std::abs(z.imag));
```

---

## Testing Your Fractal

### 1. Build Verification

```bash
# Build the solution
dotnet build

# Should compile without errors
```

### 2. Runtime Testing

1. Run ManpWinUI
2. Open Fractal Browser
3. Find your category
4. Click your fractal
5. Should render immediately

### 3. Verify Features

- [ ] Appears in browser under correct category
- [ ] Renders correctly
- [ ] Julia mode works (if supported)
- [ ] Custom parameters appear in editor (if defined)
- [ ] Metadata shows in info panel
- [ ] Suggested viewpoints work

---

## Common Patterns and Examples

### Example 1: Simple Polynomial Variant

```cpp
void RegisterMyFractals()
{
    FractalSpec spec;

    spec.name = "Cubic";
    spec.displayName = "Cubic Mandelbrot";
    spec.category = "Multibrot Family";
    spec.description = "z = z³ + c iteration";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, 
                         ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; iter++) {
            z = z * z * z + constant;  // Cubic instead of square

            if (z.magnitudeSquared() > 256.0 * 256.0)
                return iter;
        }
        return maxIter;
    };

    spec.supportsJulia = true;
    spec.defaultZoom = 1.5;  // Needs wider view than Mandelbrot

    FractalRegistry::Register(spec);
}
```

### Example 2: Hybrid Formula

```cpp
// Combines two iteration methods
spec.calculator = [](ComplexD c, int maxIter, bool isJulia, 
                     ComplexD juliaC, const ParamMap& params) -> double {
    ComplexD z = c;
    double blend = params.count("blend") > 0 ? params.at("blend") : 0.5;

    for (int iter = 0; iter < maxIter; iter++) {
        // Method A: z² + c
        ComplexD a = z * z + c;

        // Method B: sin(z) + c
        ComplexD b = ComplexD(sin(z.real) * cosh(z.imag), 
                              cos(z.real) * sinh(z.imag)) + c;

        // Blend
        z = ComplexD(
            a.real * blend + b.real * (1.0 - blend),
            a.imag * blend + b.imag * (1.0 - blend)
        );

        if (z.magnitudeSquared() > 256.0 * 256.0)
            return iter;
    }
    return maxIter;
};

spec.parameters = {
    ParameterSpec("blend", "Blend", "Mix ratio (0=Method A, 1=Method B)",
                  ParameterType::Float, ParameterCategory::Calculation,
                  "0.5", 0.0, 1.0, 0.05)
};
```

---

## Advanced Topics

### Performance Optimization

1. **Symmetry Detection**: Set `spec.hasSymmetry = true` if your fractal has x-axis or y-axis symmetry
2. **Bailout Optimization**: Use squared magnitudes to avoid sqrt()
3. **Early Exit**: Check escape condition as early as possible

### Deep Zoom Support

Your fractal automatically supports:
- Arbitrary precision arithmetic (via ManpLab's BigDouble)
- Perturbation theory (for extreme magnifications)
- Series approximation (BLA algorithm)

No special code needed!

### Julia Set Rendering

If `spec.supportsJulia = true`:
- User can toggle Julia mode in UI
- `isJulia` parameter will be true
- `juliaC` contains the Julia constant (selected by user)
- Use `c` as the starting point instead of constant

---

## Troubleshooting

### Problem: Fractal doesn't appear in browser

**Solutions:**
- Check `FractalRegistry::InitializeBuiltins()` includes your `Register*Family()` call
- Verify .cpp file is in ManpCore.Native.vcxproj
- Rebuild entire solution
- Check for C++ compilation errors

### Problem: Renders as all black or all one color

**Solutions:**
- Check bailout condition is correct
- Verify iteration formula updates z
- Check return value is in range [0, maxIter]
- Test with simple Mandelbrot formula first

### Problem: Julia mode doesn't work

**Solutions:**
- Set `spec.supportsJulia = true`
- Use `c` as starting point when `isJulia == true`
- Use `juliaC` as the constant instead of `c`

### Problem: Custom parameters don't show up

**Solutions:**
- Verify `spec.parameters` vector is populated
- Check parameter names match between spec and calculator
- Ensure ParameterSpec constructor is correct

---

## Reference Implementations

Study these families for patterns:

- **MandelbrotFamily.cpp** - Standard escape-time
- **BurningShipFamily.cpp** - Absolute value variants
- **NewtonFamily.cpp** - Root-finding algorithms
- **PhoenixFamily.cpp** - Multi-term iterations
- **MultibrotFamily.cpp** - Parameterized exponents

---

## Contributing

When adding fractals to the main repository:

1. Follow the template structure
2. Add comprehensive metadata
3. Include suggested viewpoints
4. Add references (Wikipedia, papers)
5. Test thoroughly before submitting PR
6. Update this guide if you discover new patterns!

---

## Quick Reference Card

```cpp
// 1. Create file: MyFractalFamily.cpp
#include "FractalRegistry.h"

void RegisterMyFractalFamily() {
    FractalSpec spec;
    spec.name = "UniqueID";
    spec.displayName = "Display Name";
    spec.category = "Category";
    spec.description = "Short description";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, 
                         ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; iter++) {
            z = z * z + constant;  // Your formula
            if (z.magnitudeSquared() > 65536.0) return iter;
        }
        return maxIter;
    };

    spec.supportsJulia = true;
    spec.defaultZoom = 1.0;
    FractalRegistry::Register(spec);
}

// 2. Add to FractalRegistry.cpp
void RegisterMyFractalFamily();  // Declare
RegisterMyFractalFamily();       // Call in InitializeBuiltins()

// 3. Add to .vcxproj
<ClCompile Include="MyFractalFamily.cpp" />

// 4. Build and run!
```

---

**Happy fractal creating! 🌀**
