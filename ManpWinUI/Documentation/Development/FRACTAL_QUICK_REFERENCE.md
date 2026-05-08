# ManpLab Fractal Development - Quick Reference Card

## 🚀 Quick Start (3 Minutes)

```powershell
# Generate scaffold
.\ManpWinUI\Scripts\New-FractalFamily.ps1 -FamilyName "MyFractal" -Category "My Category"

# Edit implementation
code ManpCore.Native\MyFractalFamily.cpp

# Build and test
dotnet build
```

---

## 📝 Minimal Fractal Template

```cpp
#include "FractalRegistry.h"

void RegisterMyFractalFamily() {
    FractalSpec spec;
    spec.name = "MyFractalID";              // Unique ID
    spec.displayName = "My Fractal Name";   // UI display
    spec.category = "My Category";          // Browser group
    spec.description = "Short description"; // Tooltip

    // Iteration formula
    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, 
                         ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; iter++) {
            z = z * z + constant;  // YOUR FORMULA HERE

            if (z.magnitudeSquared() > 65536.0) {
                double log_zn = log(z.magnitudeSquared()) / 2.0;
                double nu = log(log_zn / log(2.0)) / log(2.0);
                return iter + 1 - nu;  // Smooth coloring
            }
        }
        return maxIter;
    };

    spec.supportsJulia = true;
    spec.defaultZoom = 1.0;
    FractalRegistry::Register(spec);
}
```

---

## 🔧 Integration Checklist

- [ ] Create `YourFractalFamily.cpp` in `ManpCore.Native/`
- [ ] Add `void RegisterYourFractalFamily();` to `FractalRegistry.cpp`
- [ ] Call `RegisterYourFractalFamily();` in `InitializeBuiltins()`
- [ ] Add `<ClCompile Include="YourFractalFamily.cpp" />` to `.vcxproj`
- [ ] Build solution: `dotnet build`

---

## 📊 FractalSpec Fields

| Field | Type | Required | Example |
|-------|------|----------|---------|
| `name` | `string` | ✅ | `"BurningShip"` |
| `displayName` | `string` | ✅ | `"Burning Ship"` |
| `category` | `string` | ✅ | `"Mandelbrot Variants"` |
| `description` | `string` | ✅ | `"Uses abs() operations"` |
| `calculator` | `lambda` | ✅ | See template |
| `supportsJulia` | `bool` | ⚠️ | `true` / `false` |
| `defaultCenterX` | `double` | ⚠️ | `-0.5` |
| `defaultCenterY` | `double` | ⚠️ | `0.0` |
| `defaultZoom` | `double` | ⚠️ | `1.0` |
| `hasSymmetry` | `bool` | ⚠️ | `true` (optimization) |

**Legend**: ✅ Required, ⚠️ Recommended

---

## 🧮 ComplexD Operations

```cpp
ComplexD a(1.0, 2.0);                    // Create
ComplexD sum = a + b;                    // Add
ComplexD product = a * b;                // Multiply
double mag = a.magnitude();              // |a|
double magSq = a.magnitudeSquared();     // |a|² (faster!)
ComplexD squared = a * a;                // a²
ComplexD power = a.pow(3.5);             // a^3.5
ComplexD abs_a = ComplexD(std::abs(a.real), std::abs(a.imag));
```

---

## 🎨 Common Iteration Patterns

### Standard (Mandelbrot)
```cpp
z = z * z + constant;
```

### Cubic
```cpp
z = z * z * z + constant;
```

### Absolute Value (Burning Ship)
```cpp
z = ComplexD(std::abs(z.real), std::abs(z.imag));
z = z * z + constant;
```

### Trigonometric
```cpp
z = ComplexD(sin(z.real) * cosh(z.imag), 
             cos(z.real) * sinh(z.imag)) + constant;
```

### Newton Method
```cpp
ComplexD f = z * z * z - ComplexD(1.0, 0.0);  // f(z)
ComplexD fp = ComplexD(3.0, 0.0) * z * z;     // f'(z)
z = z - f / fp;
```

---

## 🎯 Julia Mode Support

```cpp
// Start point and constant swap between modes
ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
ComplexD constant = isJulia ? juliaC : c;

// Then use z and constant in iteration
for (int iter = 0; iter < maxIter; iter++) {
    z = z * z + constant;
    // ...
}
```

---

## 🔢 Custom Parameters

```cpp
// Define parameter
spec.parameters = {
    ParameterSpec(
        "exponent",                // Internal name
        "Exponent",                // Display name
        "Power of z",              // Description
        ParameterType::Float,
        ParameterCategory::Calculation,
        "2.0",                     // Default value (as string)
        1.0, 10.0,                // Min, max
        0.1                       // Step size
    )
};

// Use in calculator
double exp = params.count("exponent") > 0 
    ? params.at("exponent") 
    : 2.0;

z = z.pow(exp) + constant;
```

---

## 📚 Parameter Types

| Type | Usage | Example |
|------|-------|---------|
| `Float` | Numeric | Exponent, angle |
| `Integer` | Whole numbers | Iteration count |
| `Boolean` | On/Off | Use symmetry |
| `Choice` | Dropdown | Algorithm variant |
| `Complex` | Real + Imag | Julia constant |

---

## 🧪 Testing

```bash
# Build
dotnet build

# Run application
# Open browser panel
# Find your category
# Click your fractal
# Should render immediately
```

### Verification Checklist
- [ ] Appears in correct category
- [ ] Renders without errors
- [ ] Julia mode works (if enabled)
- [ ] Zoom in/out smooth
- [ ] Parameters functional (if defined)
- [ ] Metadata displays in info panel

---

## 🐛 Common Issues

| Problem | Solution |
|---------|----------|
| Not in browser | Check `InitializeBuiltins()` call |
| All black | Check bailout condition |
| Build error | Add forward declaration |
| Linker error | Add to `.vcxproj` |
| Crash on render | Check division by zero |

---

## 📁 File Locations

```
ManpCore.Native/
├── FractalTemplate.cpp.template    ← Copy this
├── ADDING_FRACTALS.md              ← Detailed guide
├── FractalRegistry.cpp             ← Add registration here
└── YourFractalFamily.cpp           ← Your implementation

ManpWinUI/
├── Scripts/
│   └── New-FractalFamily.ps1       ← Generator script
└── Assets/FractalKnowledge/
    └── YourFractal_metadata.json   ← Rich metadata
```

---

## 🔗 Resources

- **Full Guide**: `ManpCore.Native/ADDING_FRACTALS.md`
- **Examples**: `MandelbrotFamily.cpp`, `NewtonFamily.cpp`
- **Generator**: `.\Scripts\New-FractalFamily.ps1 -Help`
- **Infrastructure**: `docs/FRACTAL_DEVELOPER_INFRASTRUCTURE.md`

---

## 💡 Pro Tips

1. **Start simple** - Copy Mandelbrot formula first
2. **Test early** - Build after every change
3. **Use symmetry** - Set `hasSymmetry = true` for optimization
4. **Smooth coloring** - Use log formula for gradients
5. **Add metadata** - Rich documentation makes fractals discoverable
6. **Study examples** - Reference existing families

---

## 🎓 Example: Adding Newton's Method (5 Minutes)

```powershell
# 1. Generate
.\Scripts\New-FractalFamily.ps1 -FamilyName "Newton" -Category "Newton Fractals"

# 2. Edit ManpCore.Native\NewtonFamily.cpp
```

```cpp
spec.calculator = [](ComplexD c, int maxIter, bool isJulia, 
                     ComplexD juliaC, const ParamMap& params) -> double {
    ComplexD z = c;
    ComplexD one(1.0, 0.0);

    for (int iter = 0; iter < maxIter; iter++) {
        ComplexD f = z * z * z - one;          // f(z) = z³ - 1
        ComplexD fp = ComplexD(3.0, 0.0) * z * z;  // f'(z) = 3z²
        z = z - f / fp;                        // Newton's method

        if ((z - one).magnitude() < 0.001)
            return iter;
    }
    return maxIter;
};
```

```powershell
# 3. Build and test
dotnet build
# Done! Newton fractal now in browser under "Newton Fractals"
```

---

## 🚀 Generator Commands

```powershell
# Basic usage
.\Scripts\New-FractalFamily.ps1 -FamilyName "MyFractal" -Category "My Category"

# Interactive mode (asks questions)
.\Scripts\New-FractalFamily.ps1 -FamilyName "MyFractal" -Interactive

# Get help
Get-Help .\Scripts\New-FractalFamily.ps1 -Detailed
```

---

**Print this page for quick reference while coding! 🖨️**

**Version**: 1.0 | **Updated**: 2026-05-04 | **ManpLab Fractal Development Team**
