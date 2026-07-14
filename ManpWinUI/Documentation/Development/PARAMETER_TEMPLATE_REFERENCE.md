# Parameter Template Reference
## Quick Guide for Adding New Fractals

**Purpose:** This document helps you quickly identify which parameter template to use when adding a new fractal to ManpLab.

**Last Updated:** July 12, 2026

---

## How to Use This Guide

1. **Identify your fractal type** in the categories below
2. **Find the matching template** name
3. **Use the template** in `FractalParameterService.GetParametersAsync()`
4. **Customize** only the fractal-specific parameters

---

## Template Hierarchy

### Level 1: Base Templates (Universal)

Used by almost all fractals. These are **automatically included** in most templates.

| Template Method | When to Use | Parameters Included |
|----------------|-------------|---------------------|
| `StandardView2D()` | All 2D fractals | center_x, center_y, zoom |
| `StandardAlgorithm()` | Escape-time fractals | max_iterations, auto_scale_iterations, bailout, escape_radius |
| `JuliaMode()` | Fractals with Julia variants | julia_mode, julia_c_real, julia_c_imag |

**Convenience builders that combine these:**
- `CreateStandardEscapeTime()` → View + Algorithm
- `CreateWithJulia()` → View + Algorithm + JuliaMode

---

## Level 2: Family-Specific Templates

### 1. Mandelbrot/Multibrot Family

**Fractal Types:** Mandelbrot variants, Multibrot powers, Mandelbar, Celtic, Buffalo, etc.

**Template:** `CreateMultibrot()`

**Example:**
```csharp
case "Multibrot-3":
    return StandardParameterTemplates.CreateMultibrot(
        "Multibrot-3",
        defaultExponent: 3,
        defaultCenterX: -0.5,
        defaultCenterY: 0.0,
        defaultZoom: 1.0
    );
```

**Adds:** `exponent` (integer, 2-20)

**Fractals in this family (43):**
- Mandelbrot, Mandelbar, Celtic, Buffalo, Perpendicular, etc.
- Multibrot-2 through Multibrot-10
- All power variants with integer exponents

---

### 2. Julia Set Presets

**Fractal Types:** Julia sets with specific constant values (Classic, Dragon, Douady Rabbit, etc.)

**Template:** `CreateJuliaPreset()`

**Example:**
```csharp
case "Julia (Dragon)":
    return StandardParameterTemplates.CreateJuliaPreset(
        "Julia (Dragon)",
        juliaReal: -0.8,
        juliaImag: 0.156,
        centerX: 0.0,
        centerY: 0.0,
        zoom: 1.0
    );
```

**Sets:** `julia_mode = true`, `julia_c_real`, `julia_c_imag` to preset values

**Common Julia Constants:**
- Classic: (-0.7, 0.27015)
- Dragon: (-0.8, 0.156)
- Douady Rabbit: (-0.123, 0.745)
- San Marco: (-0.75, 0.0)
- Siegel Disk: (-0.390541, -0.586788)

**Fractals in this family (23):** All Julia preset variants

---

### 3. Burning Ship Family

**Fractal Types:** Burning Ship and variants with absolute value operations

**Template:** `CreateWithJulia()` + `IntegerExponent()`

**Example:**
```csharp
case "BurningShip3":
    var set = StandardParameterTemplates.CreateWithJulia(
        "BurningShip3",
        defaultCenterX: -0.5,
        defaultCenterY: -0.5,
        defaultZoom: 1.5
    );
    set.AddParameter(StandardParameterTemplates.IntegerExponent(3, min: 2, max: 10));
    return set;
```

**Adds:** `exponent` (integer, 2-10)

**Fractals in this family (14):**
- Burning Ship (classic)
- BurningShip3, BurningShip4, BurningShip5
- Bird of Prey, Buffalo, Celtic, Diagonal, Partial, Perpendicular, Reverse, Shark, Vertical

---

### 4. Bifurcation Diagrams

**Fractal Types:** Parameter space diagrams, orbit diagrams, Lyapunov plots

**Template:** `BifurcationParameters()`

**Example:**
```csharp
case "LogisticParameterSpace":
    var set = new FractalParameterSet("LogisticParameterSpace");
    set.AddParameters(StandardParameterTemplates.BifurcationParameters(
        defaultMinY: 0.0,
        defaultMaxY: 1.0,
        defaultTransient: 100,
        defaultSamples: 200
    ).ToArray());
    return set;
```

**Adds:** `minY`, `maxY`, `transient`, `samples`

**Fractals in this family (6):**
- Logistic Parameter Space
- Henon Parameter Space
- Lambda Parameter Space
- Mandelbrot Parameter Space
- May-Lyapunov Reference
- Orbit Diagram

---

### 5. Phoenix Fractals

**Fractal Types:** Phoenix set and variants

**Template:** `CreateWithJulia()` + `PhoenixParameters()`

**Example:**
```csharp
case "Phoenix":
    var set = StandardParameterTemplates.CreateWithJulia("Phoenix");
    set.AddParameters(StandardParameterTemplates.PhoenixParameters().ToArray());
    return set;
```

**Adds:** `phoenix_p`, `phoenix_q` (both double, -2.0 to 2.0)

**Fractals in this family (8):** Phoenix variants with different P/Q values

---

### 6. Lambda Fractals

**Fractal Types:** Lambda set and variants

**Template:** `CreateWithJulia()` + `LambdaParameters()`

**Example:**
```csharp
case "Lambda":
    var set = StandardParameterTemplates.CreateWithJulia("Lambda");
    set.AddParameters(StandardParameterTemplates.LambdaParameters().ToArray());
    return set;
```

**Adds:** `lambda_real`, `lambda_imag` (both double, -2.0 to 2.0)

**Fractals in this family (8):** Lambda variants

---

### 7. Newton/Nova Method

**Fractal Types:** Root-finding fractals with polynomial equations

**Template:** `CreateNewtonPolynomial()`

**Example:**
```csharp
case "Newton-3":
    return StandardParameterTemplates.CreateNewtonPolynomial(
        "Newton-3",
        degree: 3,  // z³ + az² + bz + c = 0
        defaultCenterX: 0.0,
        defaultCenterY: 0.0,
        defaultZoom: 2.0
    );
```

**Adds:** `poly_coeff_a`, `poly_coeff_b`, `poly_coeff_c`, ... (one per degree-1)

**Fractals in this family (8):**
- Newton-3, Newton-4, Newton-5
- Nova-3, Nova-4
- Halley-3, Halley-4

---

### 8. Magnet Fractals

**Fractal Types:** Magnet set (Type 1 and Type 2)

**Template:** `CreateWithJulia()` + `MagnetParameters()`

**Example:**
```csharp
case "Magnet1M":
    var set = StandardParameterTemplates.CreateWithJulia("Magnet1M");
    set.AddParameters(StandardParameterTemplates.MagnetParameters().ToArray());
    return set;
```

**Adds:** `magnet_order` (integer, 1 or 2)

**Fractals in this family (4):**
- Magnet1M, Magnet1J
- Magnet2M, Magnet2J

---

### 9. Barnsley Fractals

**Fractal Types:** Barnsley set (variants 1, 2, 3)

**Template:** `CreateWithJulia()` + `BarnsleyParameters()`

**Example:**
```csharp
case "BarnsleyJ1":
    var set = StandardParameterTemplates.CreateWithJulia("BarnsleyJ1");
    set.AddParameters(StandardParameterTemplates.BarnsleyParameters().ToArray());
    return set;
```

**Adds:** `barnsley_variant` (integer, 1-3)

**Fractals in this family (6):**
- BarnsleyJ1, BarnsleyJ2, BarnsleyJ3
- BarnsleyM1, BarnsleyM2, BarnsleyM3

---

### 10. Attractor Systems

**Fractal Types:** Strange attractors, chaotic systems (Lorenz, Rössler, etc.)

**Template:** `AttractorParameters()` (varies by attractor)

**Example:**
```csharp
case "Lorenz":
    var set = new FractalParameterSet("Lorenz");
    set.AddParameter(new FractalParameterDescriptor
    {
        Key = "sigma",
        Name = "Sigma (σ)",
        Type = ParameterType.Double,
        Category = ParameterCategory.FractalSpecific,
        DefaultValue = 10.0,
        MinValue = 0.0,
        MaxValue = 50.0,
        StepSize = 0.1,
        Description = "Prandtl number parameter"
    });
    // Add rho, beta, dt parameters...
    return set;
```

**Common parameters:** System-specific (a, b, c, sigma, rho, beta, etc.) + `dt` (time step)

**Fractals in this family (13):**
- Lorenz, Rössler, Thomas, Aizawa, Chen-Lee, Dadras, Halvorsen, Pickover
- Arneodo, Liu-Chen, Rabinovich-Fabrikant, Sprott B

**Note:** Each attractor has unique parameters - requires custom definition

---

### 11. Trigonometric Fractals

**Fractal Types:** Fractals with trigonometric functions

**Template:** `CreateWithJulia()` + `TrigonometricParameters()`

**Example:**
```csharp
case "SinZ":
    var set = StandardParameterTemplates.CreateWithJulia("SinZ");
    set.AddParameters(StandardParameterTemplates.TrigonometricParameters().ToArray());
    return set;
```

**Adds:** 
- `trig_function` (Choice: SIN, COS, TAN, SINH, COSH, TANH)
- `trig_frequency` (double, 0.1-10.0)

**Fractals in this family (19):** Various trigonometric fractals

---

### 12. Exponential Fractals

**Fractal Types:** Fractals with exponential functions

**Template:** `CreateWithJulia()` + `ExponentialParameters()`

**Example:**
```csharp
case "Exponential":
    var set = StandardParameterTemplates.CreateWithJulia("Exponential");
    set.AddParameters(StandardParameterTemplates.ExponentialParameters().ToArray());
    return set;
```

**Adds:**
- `exp_base` (double, 1.1-10.0, default: e=2.71828)
- `exp_coefficient` (double, -5.0 to 5.0)

**Fractals in this family (12):** Exponential variants

---

### 13. Rational Function Fractals

**Fractal Types:** Fractals with polynomial ratios (numerator/denominator)

**Template:** `CreateWithJulia()` + `RationalFunctionParameters()`

**Example:**
```csharp
case "RationalZ3":
    var set = StandardParameterTemplates.CreateWithJulia("RationalZ3");
    set.AddParameters(StandardParameterTemplates.RationalFunctionParameters().ToArray());
    return set;
```

**Adds:**
- `numerator_a`, `numerator_b`, `numerator_c`
- `denominator_a`, `denominator_b`, `denominator_c`

**Fractals in this family (8):** Rational function variants

---

## Level 3: Custom/Unique Fractals

**When to use:** Your fractal doesn't fit any of the above categories.

**Approach:**
1. Start with base template: `CreateStandardEscapeTime()` or `CreateWithJulia()`
2. Add custom parameters manually using `FractalParameterDescriptor`

**Example:**
```csharp
case "MyUniqueFractal":
    var set = StandardParameterTemplates.CreateWithJulia("MyUniqueFractal");

    // Add custom parameter
    set.AddParameter(new FractalParameterDescriptor
    {
        Key = "my_custom_param",
        Name = "Custom Parameter",
        Type = ParameterType.Double,
        Category = ParameterCategory.FractalSpecific,
        DefaultValue = 1.0,
        MinValue = 0.0,
        MaxValue = 10.0,
        StepSize = 0.1,
        FormatString = "F2",
        Description = "Controls custom behavior",
        DisplayOrder = 1
    });

    return set;
```

---

## Parameter Categories

Use these for organizing parameters in the UI:

| Category | Purpose | Examples |
|----------|---------|----------|
| `View` | Viewport positioning | center_x, center_y, zoom |
| `Algorithm` | Rendering algorithm | max_iterations, bailout, auto_scale |
| `Julia` | Julia mode settings | julia_mode, julia_c_real, julia_c_imag |
| `FractalSpecific` | Unique to this fractal | exponent, phoenix_p, lambda_real |
| `Advanced` | Expert-level settings | tolerance, epsilon, relaxation |

---

## Parameter Types

| Type | C# Type | UI Control | Example |
|------|---------|------------|---------|
| `Double` | double | NumberBox | 3.14159 |
| `Integer` | int | NumberBox (no decimals) | 512 |
| `Boolean` | bool | CheckBox | true/false |
| `Choice` | string | ComboBox | "SIN", "COS", "TAN" |
| `Complex` | (double, double) | Two NumberBoxes | (-0.7, 0.27) |

---

## Quick Decision Tree

```
Adding a new fractal?
│
├─ Is it a power variant of Mandelbrot/Multibrot?
│  └─ YES → Use CreateMultibrot()
│
├─ Is it a Julia set with specific constant?
│  └─ YES → Use CreateJuliaPreset()
│
├─ Is it a bifurcation/parameter space diagram?
│  └─ YES → Use BifurcationParameters()
│
├─ Does it use Phoenix-style distortion?
│  └─ YES → CreateWithJulia() + PhoenixParameters()
│
├─ Does it use Lambda parameter?
│  └─ YES → CreateWithJulia() + LambdaParameters()
│
├─ Is it a Newton/root-finding method?
│  └─ YES → CreateNewtonPolynomial(degree)
│
├─ Does it have trigonometric functions?
│  └─ YES → CreateWithJulia() + TrigonometricParameters()
│
├─ Does it use exponential functions?
│  └─ YES → CreateWithJulia() + ExponentialParameters()
│
├─ Is it an attractor system?
│  └─ YES → Create custom with system parameters
│
└─ None of the above?
   └─ Use CreateWithJulia() + custom parameters
```

---

## Common Patterns

### Pattern 1: Standard Escape-Time Fractal with Julia
```csharp
case "MyFractal":
    return StandardParameterTemplates.CreateWithJulia(
        "MyFractal",
        defaultCenterX: 0.0,
        defaultCenterY: 0.0,
        defaultZoom: 1.0
    );
```

### Pattern 2: Adding Single Custom Parameter
```csharp
case "MyFractal":
    var set = StandardParameterTemplates.CreateWithJulia("MyFractal");
    set.AddParameter(StandardParameterTemplates.IntegerExponent(4));
    return set;
```

### Pattern 3: Multiple Custom Parameters
```csharp
case "MyComplexFractal":
    var set = StandardParameterTemplates.CreateWithJulia("MyComplexFractal");
    set.AddParameter(CustomParameter1());
    set.AddParameter(CustomParameter2());
    set.AddParameter(CustomParameter3());
    return set;
```

### Pattern 4: No Julia Support
```csharp
case "MyAttractor":
    var set = StandardParameterTemplates.CreateStandardEscapeTime("MyAttractor");
    // Add attractor-specific parameters
    return set;
```

---

## Files to Modify

When adding a new fractal with parameter template:

1. **`ManpWinUI/Models/Parameters/StandardParameterTemplates.*.cs`**
   - Add new template method if creating a reusable pattern

2. **`ManpWinUI/Services/FractalParameterService.cs`**
   - Add case in `GetParametersAsync()` switch statement

3. **Test your fractal:**
   - Launch app
   - Select fractal from browser
   - Verify parameters appear in Parameters tab
   - Change parameters and verify rendering updates

---

## Need Help?

**If you're unsure which template to use:**
1. Look at similar fractals in the registry
2. Check which parameters your fractal formula uses
3. Start with `CreateWithJulia()` - works for 80% of fractals
4. Add custom parameters as needed

**Template not flexible enough?**
- You can always mix templates or create fully custom parameter sets
- The templates are **helpers**, not constraints

---

**Document Version:** 1.0  
**Last Updated:** July 12, 2026  
**Maintained by:** Mark  
**Related Documents:**
- `PARAMETER_SYSTEM_ARCHITECTURE.md` - Deep dive into how the system works
- `PARAMETER_TEMPLATE_STRATEGY.md` - Migration plan for remaining fractals
- `ADDING_FRACTALS.md` - Complete guide to adding new fractals
