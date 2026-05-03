# Parameter Naming Conventions

**Purpose:** Maintain consistency with Paul DeLeeuw's ManpWIN64 codebase for easier code maintenance and cross-referencing.

**Last Updated:** 2026-05-02

---

## 🎯 **Principle**

**Use Paul DeLeeuw's parameter names wherever applicable** to maintain:
- ✅ Code consistency across ManpWIN64 and ManpLab
- ✅ Easier maintenance when porting algorithms
- ✅ Clear traceability between C++ native and C# UI layers
- ✅ Familiar terminology for users of the original ManpWIN64

---

## 📋 **Standard Parameter Names (from ManpWIN64/fractalp.cpp)**

### **View Parameters**
| Paul's Name | Description | Type | Typical Range |
|-------------|-------------|------|---------------|
| (implicit: center/zoom calculated from hor/vert/width) | View window coordinates | double | -10.0 to 10.0 |

**Note:** ManpWIN64 uses `hor`, `vert`, `width` (line 246) for view definition. We use `centerX`, `centerY`, `zoom` for consistency with modern fractal explorers.

### **Algorithm Parameters**
| Paul's Name | Description | Type | Typical Range |
|-------------|-------------|------|---------------|
| `realz0` | "Real Perturbation of Z(0)" | double | 0.0 |
| `imagz0` | "Imaginary Perturbation of Z(0)" | double | 0.0 |
| `realparm` | "Real Part of Parameter" | double | varies |
| `imagparm` | "Imaginary Part of Parameter" | double | varies |

### **Newton/Basin Parameters**
| Paul's Name | Description | Type | Typical Range |
|-------------|-------------|------|---------------|
| `newtdegree` | "Polynomial Degree (>= 2) 5 is a diamond" | int | 3 |
| *(subtype)* | "Subtype: 0=Normal, 1=Stripes, 2=Basin" | int | 0-2 |

### **Special Parameters**
| Paul's Name | Description | Type | Typical Range |
|-------------|-------------|------|---------------|
| `lambda` | "Lambda" | double | varies |
| `alpha` | "Alpha" | double | varies |
| `beta` | "Beta" | double | varies |
| `gamma2` | "Gamma" | double | varies |
| `omega` | "Omega" | double | varies |
| `symdegree` | "+Degree of symmetry" | int | varies |

### **Bifurcation Parameters**
| Paul's Name | Description | Type | Typical Range |
|-------------|-------------|------|---------------|
| `filt` | "+Filter Cycles" | int | varies |
| `seed` | "Seed Population" | double | varies |

### **Orbit Fractal Parameters**
| Paul's Name | Description | Type | Typical Range |
|-------------|-------------|------|---------------|
| `A` / `a` | "a" parameter | double | varies |
| `B` / `b` | "b" parameter | double | varies |
| `D` / `d` | "d" parameter | double | varies |
| `H` / `h` | "h" parameter | double | varies |
| `P` / `p` | "p" parameter | double | varies |

### **4D/Quaternion Parameters**
| Paul's Name | Description | Type | Typical Range |
|-------------|-------------|------|---------------|
| `C` / `c` | "c" parameter | double | varies |
| `C1` / `c1` | "c1" parameter | double | varies |
| `CI` / `ci` | "ci" parameter | double | varies |
| `CJ` / `cj` | "cj" parameter | double | varies |
| `CK` / `ck` | "ck" parameter | double | varies |
| `ZJ` / `zj` | "zj" parameter | double | varies |
| `ZK` / `zk` | "zk" parameter | double | varies |

### **Attractor Parameters**
| Paul's Name | Description | Type | Typical Range |
|-------------|-------------|------|---------------|
| `frothattractor` | "+3 or 6 attractor system" | int | 3 or 6 |
| `frothshade` | "+Enter non-zero value for alternate color shading" | int | 0 or 1 |

### **Gingerbreadman Parameters**
| Paul's Name | Description | Type | Typical Range |
|-------------|-------------|------|---------------|
| `initx` | "Initial x" | double | varies |
| `inity` | "Initial y" | double | varies |

### **Popcorn Parameters**
| Paul's Name | Description | Type | Typical Range |
|-------------|-------------|------|---------------|
| `step` | "Step size" | double | varies |

### **Symmetry/Function Parameters**
| Paul's Name | Description | Type | Typical Range |
|-------------|-------------|------|---------------|
| `shiftval` | "Function Shift Value" | double | varies |

### **IFS Parameters**
| Paul's Name | Description | Type | Typical Range |
|-------------|-------------|------|---------------|
| `color_method` | "+Coloring method (0,1)" | int | 0 or 1 |

---

## 🔑 **Key Naming Principles**

### **1. Use Paul's Names for Fractal-Specific Parameters**

When a parameter appears in `ManpWIN64/fractalp.cpp`, **use that exact name**:

```cpp
// ✅ Good: Matches Paul's code
parameters.Add("realparm", "Real Part of Parameter", ...);
parameters.Add("imagparm", "Imaginary Part of Parameter", ...);

// ❌ Bad: Doesn't match
parameters.Add("real_parameter", "Real Part", ...);
parameters.Add("imag_parameter", "Imaginary Part", ...);
```

### **2. Use Paul's Display Names for UI**

The `ParamName[10]` strings in Paul's code are user-facing descriptions:

```cpp
// From ManpWIN64/fractalp.cpp line 28-29:
static char realz0[] = "Real Perturbation of Z(0)";
static char imagz0[] = "Imaginary Perturbation of Z(0)";
```

**In our C# code:**

```csharp
// ✅ Good: Matches Paul's display text
new FractalParameterDescriptor
{
    Name = "realz0",  // Internal key
    DisplayName = "Real Perturbation of Z(0)",  // Paul's display text
    ...
}
```

### **3. Preserve Case Sensitivity**

Paul uses lowercase for most parameter names:
- `realz0`, `imagz0`, `realparm`, `imagparm` (lowercase)
- But: `A`, `B`, `C`, `D`, `H`, `P` (uppercase for orbit fractals)

**Match Paul's case exactly:**

```csharp
// ✅ Good
parameters.Add("realz0", ...);    // lowercase
parameters.Add("A", ...);         // uppercase

// ❌ Bad
parameters.Add("RealZ0", ...);    // wrong case
parameters.Add("a", ...);         // wrong case
```

### **4. Handle Special Characters**

Some parameter names have special prefixes:
- `+` prefix = Advanced parameter (e.g., `"+Filter Cycles"`)
- `*` prefix = Internal fractal name (e.g., `"*lambda"`)

**In our parameter system:**

```csharp
// ✅ Strip + prefix, mark as advanced
new FractalParameterDescriptor
{
    Name = "filt",
    DisplayName = "Filter Cycles",  // Strip the '+'
    IsAdvanced = true,              // Mark as advanced
    ...
}

// ✅ Strip * prefix from fractal names
fractalSpec.Name = "Lambda";       // Strip the '*'
```

---

## 📊 **Current Status**

### **Our C# Parameter System**

**File:** `ManpWinUI/Models/Parameters/StandardParameterTemplates.Core.cs`

**Current naming:**

```csharp
// View parameters (lines 32-73)
Key = "center_x"    // ⚠️ Our convention (no direct Paul equivalent)
Key = "center_y"    // ⚠️ Our convention
Key = "zoom"        // ⚠️ Our convention

// Algorithm parameters (lines 90+)
Key = "max_iterations"  // ⚠️ Our convention
Key = "bailout"         // ⚠️ Our convention
```

**Status:** 🟡 **Partially aligned**
- View parameters: Our modern convention (no direct ManpWIN64 equivalent)
- Algorithm parameters: Need to verify alignment with Paul's code

---

## 🎯 **Task 8 Integration Plan**

When implementing Task 8 (Native Parameter Metadata), we will:

### **Step 1: Map Paul's Parameters to Native Specs**

For each fractal in `ManpCore.Native/ClassicEscapeTimeFamily.cpp`:

```cpp
// Lambda fractal example
spec.parameters = {
    // Use Paul's names from fractalp.cpp line 273-278
    {
        "realz0",           // Paul's name (line 273)
        "Real Perturbation of Z(0)",  // Paul's display text (line 28)
        "Initial value for real part of z",
        ParamType::Decimal,
        ParamCategory::Calculation,
        "0.0",              // Paul's default (line 273)
        ...
    },
    {
        "imagz0",           // Paul's name (line 273)
        "Imaginary Perturbation of Z(0)",  // Paul's display text (line 29)
        "Initial value for imaginary part of z",
        ParamType::Decimal,
        ParamCategory::Calculation,
        "0.0",              // Paul's default (line 273)
        ...
    }
};
```

### **Step 2: Verify Against Paul's Defaults**

Cross-reference default values from `fractalp.cpp`:

```cpp
// Lambda fractal (line 273-278):
t_lambda+1, realz0, imagz0, ES, ES, ..., 0, 0, 0, 0, ...
// Default values: realz0=0, imagz0=0

// Our native spec should match:
"0.0", "0.0"
```

### **Step 3: Maintain Backward Compatibility**

Our C# `StandardParameterTemplates` will remain as fallback:

```csharp
// Priority 1: Try native registry (Paul's names)
var nativeParams = FractalRegistryInterop.GetParametersForFractal(fractalType);

// Priority 2: Fall back to our C# templates (our names)
var template = StandardParameterTemplates.GetTemplate(fractalType);
```

---

## 📝 **Documentation References**

**Paul's Code:**
- `ManpWIN64/fractalp.cpp` lines 28-33: Standard parameter name definitions
- `ManpWIN64/fractalp.cpp` lines 80-128: Special parameter names
- `ManpWIN64/fractalp.cpp` lines 253+: Fractal specs with parameter references
- `ManpWIN64/fract.h` lines 24-31: `CFract` class parameter arrays

**Our Code:**
- `ManpWinUI/Models/Parameters/StandardParameterTemplates.Core.cs`: Current C# templates
- `ManpCore.Native/FractalRegistry.h`: Native parameter structure (Task 8)
- `docs/TASK_8_IMPLEMENTATION_GUIDE.md`: Native metadata integration plan

---

## ✅ **Checklist for New Fractals**

When adding a new fractal:

- [ ] Check if fractal exists in `ManpWIN64/fractalp.cpp`
- [ ] If yes: Use Paul's parameter names exactly (case-sensitive)
- [ ] If yes: Use Paul's display names for UI labels
- [ ] If yes: Use Paul's default values from fractalp.cpp
- [ ] If no: Follow our modern naming convention (`snake_case`)
- [ ] Document any deviations from Paul's naming with rationale
- [ ] Add cross-reference comments linking to Paul's code

---

## 🎯 **Examples**

### **Example 1: Lambda Fractal**

**Paul's definition** (`fractalp.cpp` line 273-278):
```cpp
t_lambda+1, realz0, imagz0, ES, ES, ..., 0, 0, ...
-4.2, -3.0, 6.0, ESCAPING, LAMBDAFP, 3, 2, 0, NULL, NULL, ...
```

**Our native spec** (to be added in Task 8):
```cpp
spec.name = "Lambda";  // Strip '*' from t_lambda
spec.parameters = {
    {"realz0", "Real Perturbation of Z(0)", ..., "0.0"},
    {"imagz0", "Imaginary Perturbation of Z(0)", ..., "0.0"}
};
spec.defaultCenterX = 1.0;   // Derived from hor=-4.2, width=6.0
spec.defaultCenterY = 0.0;   // Derived from vert=-3.0
spec.defaultZoom = 0.375;    // Derived from width=6.0 → viewWidth=8.0
```

### **Example 2: Newton Fractal**

**Paul's definition** (`fractalp.cpp` line 289-293):
```cpp
t_newton, newtdegree, "Subtype: 0=Normal, 1=Stripes, 2=Basin", ES, ...
```

**Our native spec:**
```cpp
spec.parameters = {
    {"newtdegree", "Polynomial Degree (>= 2) 5 is a diamond", ..., "3.0"},
    {"subtype", "Subtype: 0=Normal, 1=Stripes, 2=Basin", ..., "0"}
};
```

---

## 🚀 **Benefits of This Approach**

1. **Easier Maintenance**
   - When Paul updates ManpWIN64, we can easily find and update corresponding parameters
   - Parameter names act as cross-reference keys between codebases

2. **User Familiarity**
   - Users migrating from ManpWIN64 see familiar parameter names
   - Documentation references work across both applications

3. **Algorithm Porting**
   - When porting algorithms, parameter mappings are obvious
   - Reduces chance of parameter mismatch bugs

4. **Code Traceability**
   - Git history shows lineage to Paul's original code
   - Comments can reference exact line numbers in Paul's code

---

**Principle:** When in doubt, check `ManpWIN64/fractalp.cpp` first! 🎯
