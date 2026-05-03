# Task 8: Native Parameter Metadata Integration

**Priority:** HIGH  
**Estimated Time:** 3-5 hours  
**Dependencies:** Tasks 1-7 complete ✅  
**Blockers:** None (all critical issues resolved)  
**Naming Convention:** See `docs/PARAMETER_NAMING_CONVENTIONS.md` for Paul DeLeeuw's parameter names

---

## 🎯 **Goal**

Replace hardcoded C# parameter templates with dynamic parameter metadata from the native C++ fractal registry.

**Current Problem:**
- `StandardParameterTemplates.cs` has 24 hardcoded templates
- Native `FractalSpec` has empty `parameters` vector
- Fractal-specific defaults (like Lambda bailout/view) are duplicated or missing

**Target Solution:**
- Native C++ fractals define their own parameters with metadata
- C# layer fetches parameter definitions via P/Invoke
- Single source of truth: C++ fractal registry

---

## 📋 **Implementation Steps**

### **Step 1: Define Parameter Metadata Structure in C++**

**File:** `ManpCore.Native/FractalRegistry.h`

**Add parameter descriptor struct:**

```cpp
namespace Native {

// Parameter type enum (mirrors C# ParameterType)
enum class ParamType {
    Integer,
    Decimal,
    Boolean,
    Complex,
    Choice
};

// Parameter category enum (mirrors C# ParameterCategory)
enum class ParamCategory {
    General,
    Calculation,
    View,
    Color,
    Advanced
};

// Parameter descriptor (native definition)
struct ParameterDescriptor {
    std::string name;           // "maxIterations"
    std::string displayName;    // "Max Iterations"
    std::string description;    // "Maximum number of iterations"
    ParamType type;             // Integer, Decimal, etc.
    ParamCategory category;     // Calculation, View, etc.

    // Default value (stored as string for flexibility)
    std::string defaultValue;   // "256"

    // Range constraints (for numeric types)
    double minValue;            // 1
    double maxValue;            // 100000
    double step;                // 1

    // Choice options (for Choice type)
    std::vector<std::string> choices;

    // Visibility
    bool isAdvanced;            // false = visible by default
    bool isReadOnly;            // false = user can edit
};

// FractalSpec already has:
// std::vector<ParameterDescriptor> parameters;

} // namespace Native
```

---

### **Step 2: Populate Parameters for Lambda (Example)**

**File:** `ManpCore.Native/ClassicEscapeTimeFamily.cpp`

**Lambda section (after line 51):**

```cpp
    // Lambda parameters
    // Using Paul DeLeeuw's parameter names from ManpWIN64/fractalp.cpp line 273
    spec.parameters = {
        // Calculation parameters (Paul's standard names)
        {
            "realz0", "Real Perturbation of Z(0)",
            "Initial value for real part of z (usually 0.0)",
            ParamType::Decimal, ParamCategory::Calculation,
            "0.0",          // Paul's default from fractalp.cpp line 273
            -10.0, 10.0, 0.1,
            {},
            false, false    // not advanced, not readonly
        },
        {
            "imagz0", "Imaginary Perturbation of Z(0)",
            "Initial value for imaginary part of z (usually 0.0)",
            ParamType::Decimal, ParamCategory::Calculation,
            "0.0",          // Paul's default from fractalp.cpp line 273
            -10.0, 10.0, 0.1,
            {},
            false, false
        },
        {
            "maxIterations", "Max Iterations",
            "Maximum number of iterations before considering pixel inside the set",
            ParamType::Integer, ParamCategory::Calculation,
            "256",          // Standard default
            1, 100000, 1,
            {},
            false, false
        },
        {
            "bailout", "Bailout Radius",
            "Escape radius for Lambda fractal (typically 4.0)",
            ParamType::Decimal, ParamCategory::Calculation,
            "4.0",          // Lambda-specific bailout (STDBAILOUT from line 136)
            2.0, 1000.0, 0.1,
            {},
            true, false     // advanced parameter
        },

        // View parameters (modern convention - no direct Paul equivalent)
        {
            "centerX", "Center X",
            "Real axis center coordinate",
            ParamType::Decimal, ParamCategory::View,
            "1.0",          // Lambda-specific center (derived from hor=-4.2, width=6.0)
            -10.0, 10.0, 0.1,
            {},
            false, false
        },
        {
            "centerY", "Center Y",
            "Imaginary axis center coordinate",
            ParamType::Decimal, ParamCategory::View,
            "0.0",          // Lambda-specific center (derived from vert=-3.0)
            -10.0, 10.0, 0.1,
            {},
            false, false
        },
        {
            "zoom", "Zoom Level",
            "Magnification factor (higher = more zoomed in)",
            ParamType::Decimal, ParamCategory::View,
            "0.375",        // Lambda-specific zoom (derived from width=6.0)
            0.01, 1000000.0, 0.1,
            {},
            false, false
        }
    };
```

**Key Points:**
- ✅ Uses Paul's names (`realz0`, `imagz0`) from `ManpWIN64/fractalp.cpp` line 273
- ✅ Uses Paul's display text from `fractalp.cpp` lines 28-29
- ✅ Uses Paul's default values (both `0.0`)
- ✅ Uses Paul's bailout (`STDBAILOUT` = 4.0 from line 136)
- ✅ View parameters derived from Paul's `hor/vert/width` definition (line 276)

---

### **Step 3: Expose Parameters Through Interop**

**File:** `ManpCore.Native/FractalRegistryWrapper.h`

**Add method to retrieve parameters:**

```cpp
// Get parameter descriptors for a fractal type
[DllExport]
int GetFractalParameters(
    const char* fractalType,
    ParameterDescriptor* outParams,
    int maxParams);
```

**File:** `ManpCore.Native/FractalRegistryWrapper.cpp`

**Implementation:**

```cpp
int GetFractalParameters(
    const char* fractalType,
    ParameterDescriptor* outParams,
    int maxParams)
{
    try {
        auto spec = FractalRegistry::GetSpec(fractalType);
        if (!spec.has_value()) {
            return 0;  // Fractal not found
        }

        int count = min((int)spec->parameters.size(), maxParams);
        for (int i = 0; i < count; i++) {
            outParams[i] = spec->parameters[i];
        }
        return count;
    }
    catch (...) {
        return -1;  // Error
    }
}
```

---

### **Step 4: Add C# Interop Layer**

**File:** `ManpCore.Services/NativeBridge/FractalRegistryInterop.cs` (new file)

```csharp
using System.Runtime.InteropServices;

namespace ManpCore.Services.NativeBridge;

[StructLayout(LayoutKind.Sequential)]
public struct NativeParameterDescriptor
{
    [MarshalAs(UnmanagedType.LPStr)]
    public string Name;

    [MarshalAs(UnmanagedType.LPStr)]
    public string DisplayName;

    [MarshalAs(UnmanagedType.LPStr)]
    public string Description;

    public int Type;        // ParamType enum
    public int Category;    // ParamCategory enum

    [MarshalAs(UnmanagedType.LPStr)]
    public string DefaultValue;

    public double MinValue;
    public double MaxValue;
    public double Step;

    [MarshalAs(UnmanagedType.Bool)]
    public bool IsAdvanced;

    [MarshalAs(UnmanagedType.Bool)]
    public bool IsReadOnly;
}

public static class FractalRegistryInterop
{
    [DllImport("ManpCore.Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int GetFractalParameters(
        [MarshalAs(UnmanagedType.LPStr)] string fractalType,
        [Out] NativeParameterDescriptor[] outParams,
        int maxParams);

    public static List<FractalParameterDescriptor> GetParametersForFractal(string fractalType)
    {
        const int maxParams = 50;
        var nativeParams = new NativeParameterDescriptor[maxParams];

        int count = GetFractalParameters(fractalType, nativeParams, maxParams);
        if (count <= 0) {
            return new List<FractalParameterDescriptor>();
        }

        var result = new List<FractalParameterDescriptor>(count);
        for (int i = 0; i < count; i++) {
            result.Add(ConvertFromNative(nativeParams[i]));
        }
        return result;
    }

    private static FractalParameterDescriptor ConvertFromNative(NativeParameterDescriptor native)
    {
        return new FractalParameterDescriptor
        {
            Name = native.Name,
            DisplayName = native.DisplayName,
            Description = native.Description,
            Type = (ParameterType)native.Type,
            Category = (ParameterCategory)native.Category,
            DefaultValue = ParseDefaultValue(native.DefaultValue, (ParameterType)native.Type),
            MinValue = native.MinValue,
            MaxValue = native.MaxValue,
            Step = native.Step,
            IsAdvanced = native.IsAdvanced,
            IsReadOnly = native.IsReadOnly
        };
    }

    private static object ParseDefaultValue(string value, ParameterType type)
    {
        return type switch
        {
            ParameterType.Integer => int.TryParse(value, out var i) ? i : 0,
            ParameterType.Decimal => double.TryParse(value, out var d) ? d : 0.0,
            ParameterType.Boolean => bool.TryParse(value, out var b) && b,
            ParameterType.Complex => ParseComplexValue(value),
            _ => value
        };
    }

    private static ComplexNumber ParseComplexValue(string value)
    {
        // Parse "0.5+0.3i" format
        // Simple implementation - can be enhanced
        return new ComplexNumber(0, 0);
    }
}
```

---

### **Step 5: Update FractalParameterService**

**File:** `ManpCore.Services/Parameters/FractalParameterService.cs`

**Change `GetParametersAsync()` method:**

```csharp
public async Task<FractalParameterSet?> GetParametersAsync(string fractalType)
{
    await EnsureInitializedAsync();

    // Priority 1: Try native parameter metadata
    var nativeParams = FractalRegistryInterop.GetParametersForFractal(fractalType);
    if (nativeParams.Count > 0)
    {
        _logger.LogInformation("Loaded {Count} parameters from native registry for {FractalType}",
            nativeParams.Count, fractalType);

        return new FractalParameterSet(fractalType, nativeParams);
    }

    // Priority 2: Fall back to hardcoded templates (legacy support)
    var template = StandardParameterTemplates.GetTemplate(fractalType);
    if (template != null)
    {
        _logger.LogWarning("Native parameters not found for {FractalType}, using fallback template",
            fractalType);
        return template;
    }

    // Priority 3: Generic default
    _logger.LogWarning("No parameters found for {FractalType}, using generic defaults",
        fractalType);
    return CreateDefaultParameterSet(fractalType);
}
```

---

## 🧪 **Testing Plan**

### **Phase 1: Native Parameter Definition**
1. Add `ParameterDescriptor` struct to `FractalRegistry.h`
2. Populate `spec.parameters` for Lambda in `ClassicEscapeTimeFamily.cpp`
3. Build native project → verify compilation

### **Phase 2: Interop Layer**
1. Add `GetFractalParameters()` to `FractalRegistryWrapper`
2. Create `FractalRegistryInterop.cs` with P/Invoke marshaling
3. Build solution → verify native/managed bridge compiles

### **Phase 3: Service Integration**
1. Update `FractalParameterService.GetParametersAsync()`
2. Add logging to show native vs. fallback source
3. Run app → check Output window logs

### **Phase 4: UI Verification**
1. Click Lambda in browser
2. Verify parameter editor loads with correct defaults:
   - Max Iterations: 256
   - Bailout: 4.0 (not 256.0!)
   - Center X: 1.0 (not 0.0!)
   - Zoom: 0.375 (not 0.8!)
3. Render Lambda → should now show visible structure!

---

## 📊 **Success Criteria**

- [ ] Native parameter metadata structure defined in C++
- [ ] Lambda fractal has populated `parameters` vector with correct defaults
- [ ] P/Invoke bridge successfully marshals parameter descriptors
- [ ] `FractalParameterService` loads parameters from native registry
- [ ] Parameter editor displays Lambda with native defaults
- [ ] Lambda renders with visible structure (not black screen)
- [ ] Logs show "Loaded X parameters from native registry for Lambda"

---

## 🎯 **Next Steps After Task 8**

### **Option A: Populate All Fractals** (Recommended)
Add parameters to remaining fractal families:
- Mandelbrot, Julia, Tricorn, Burning Ship
- Multibrot family
- Phoenix, Newton, Magnet families

### **Option B: Systematic Fractal Validation**
Run Phase 1 smoke test from `docs/FRACTAL_VISUAL_VALIDATION.md`:
- Click each fractal, verify correct defaults
- Identify visual issues (black screens, bad views)
- Fix using native parameter definitions

### **Option C: Task 9 - Parameter Persistence**
Implement save/load for user parameter sets:
- Save custom configurations to JSON
- Load saved parameter sets from library
- User workflow: customize → save → recall later

---

## 📝 **Notes**

- Start with Lambda as proof-of-concept (smallest scope)
- Can incrementally add parameters to other fractals
- Fallback templates ensure backward compatibility
- Native definitions become single source of truth over time

---

**Ready to implement Task 8!** Let me know when you want to start. 🚀
