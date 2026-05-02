# WinUI Fractal Integration - Architecture Fix Tasks

**Date:** 2024
**Status:** Critical - Blocks full fractal registry integration
**Branch:** development

## Executive Summary

The WinRT runtime exception when clicking fractals in the Browser is a **symptom** of fundamental architectural deficiencies:

1. **No flexible parameter system** - Hard-coded parameters for 4 fractal types, can't support 50+ registry fractals
2. **View-first instantiation pattern** - Creates binding/threading violations
3. **Tight coupling** - MainPage orchestrates 3+ ViewModels synchronously
4. **No abstraction for fractal metadata** - Direct P/Invoke calls during UI operations
5. **Threading model confusion** - C++/CLI objects cross thread boundaries unsafely

## Critical Priority Tasks (Do First)

### Task 1: Create Flexible Parameter System ⚠️ CRITICAL
**Priority:** P0 - Blocks all other work
**Estimated Effort:** 2-3 days
**Concern:** Current system cannot support dynamic fractal parameters

#### Problem
```csharp
// Current: Hard-coded properties in MainViewModel
public partial double CenterX { get; set; } = -0.5;
public partial double CenterY { get; set; } = 0.0;
public partial double Zoom { get; set; } = 1.0;
```

Cannot support:
- Fractals with 4+ parameters (Lambda: λ real, λ imaginary, seed, exponent)
- Fractals with different parameter types (Barnsley: integer modes, complex coefficients)
- Dynamic parameter discovery from native registry

#### Solution Architecture
Create a flexible parameter descriptor system:

**Files to Create:**
1. `ManpWinUI\Models\Parameters\IFractalParameter.cs`
2. `ManpWinUI\Models\Parameters\FractalParameterDescriptor.cs`
3. `ManpWinUI\Models\Parameters\FractalParameterSet.cs`
4. `ManpWinUI\Models\Parameters\ParameterType.cs` (enum)
5. `ManpWinUI\Services\IFractalParameterService.cs`
6. `ManpWinUI\Services\FractalParameterService.cs`

**Implementation Plan:**

```csharp
// ManpWinUI\Models\Parameters\ParameterType.cs
public enum ParameterType
{
    Double,          // Floating-point value
    Integer,         // Whole number
    Complex,         // Real + Imaginary pair
    Boolean,         // True/false toggle
    Choice,          // Dropdown selection
    ColorPalette,    // Color scheme selector
    Point2D          // X,Y coordinate pair
}

// ManpWinUI\Models\Parameters\FractalParameterDescriptor.cs
public class FractalParameterDescriptor
{
    public string Name { get; set; }              // "Lambda Real"
    public string Key { get; set; }               // "lambda_real"
    public ParameterType Type { get; set; }       // ParameterType.Double
    public object DefaultValue { get; set; }      // -0.5
    public object? MinValue { get; set; }         // -2.0
    public object? MaxValue { get; set; }         // 2.0
    public string? Description { get; set; }      // "Real component of λ"
    public bool IsEditable { get; set; } = true;  // Can user change?
    public string? ValidationRule { get; set; }   // Regex or expression
    public string Category { get; set; }          // "View", "Algorithm", "Color"
}

// ManpWinUI\Models\Parameters\FractalParameterSet.cs
public class FractalParameterSet : ObservableObject
{
    public string FractalType { get; set; }
    public ObservableCollection<FractalParameterDescriptor> Parameters { get; set; }

    public object? GetValue(string key) { /* ... */ }
    public void SetValue(string key, object value) { /* ... */ }
    public bool Validate() { /* ... */ }
    public Dictionary<string, object> ToRenderParameters() { /* ... */ }
}

// ManpWinUI\Services\IFractalParameterService.cs
public interface IFractalParameterService
{
    /// <summary>
    /// Get parameter descriptors for a fractal type from native registry
    /// </summary>
    Task<FractalParameterSet> GetParametersAsync(string fractalType);

    /// <summary>
    /// Update a parameter value and trigger validation
    /// </summary>
    void UpdateParameter(FractalParameterSet paramSet, string key, object value);

    /// <summary>
    /// Load saved parameter overrides from settings
    /// </summary>
    Task<Dictionary<string, object>> LoadParameterOverridesAsync(string fractalType);

    /// <summary>
    /// Save parameter values for persistence
    /// </summary>
    Task SaveParameterOverridesAsync(string fractalType, Dictionary<string, object> values);
}
```

**Native Layer Changes Required:**
Extend `FractalSpec` in `ManpCore.Native\FractalRegistry.h`:

```cpp
struct ParameterSpec {
    std::string key;           // "lambda_real"
    std::string displayName;   // "Lambda Real"
    ParameterType type;        // PARAM_DOUBLE
    double defaultValue;
    double minValue;
    double maxValue;
    std::string description;
    bool editable;
};

struct FractalSpec {
    // ... existing fields ...
    std::vector<ParameterSpec> parameters;  // ADD THIS
};
```

**Migration Strategy:**
1. Create new parameter system alongside existing properties
2. Map standard properties (CenterX, Zoom) to parameters for backwards compatibility
3. Gradually migrate UI bindings to use parameter system
4. Remove hard-coded properties in final cleanup phase

**Benefits:**
- ✅ Support all 50+ fractals without code changes
- ✅ UI auto-generates parameter editors from metadata
- ✅ Easy to add new fractal types
- ✅ Parameter validation built-in
- ✅ Settings persistence unified

---

### Task 2: Fix FractalBrowserViewModel DI Pattern
**Priority:** P0 - Causes WinRT exceptions
**Estimated Effort:** 4 hours
**Files to Modify:**
- `ManpWinUI\Views\Browser\FractalBrowserView.xaml.cs`
- `ManpWinUI\Views\MainPage.cs`
- `ManpWinUI\App.xaml.cs` (DI registration)

#### Current Problem
```csharp
// FractalBrowserView.xaml.cs - Creates its own ViewModel ❌
public FractalBrowserView()
{
    ViewModel = new FractalBrowserViewModel(settingsService);
    DataContext = ViewModel;
}
```

#### Solution
**Step 1:** Register in DI container (`App.xaml.cs`):
```csharp
services.AddSingleton<FractalBrowserViewModel>();
```

**Step 2:** Inject into MainPage:
```csharp
public MainPage(
    MainViewModel viewModel,
    FractalBrowserViewModel browserViewModel,  // ADD THIS
    // ... other dependencies
)
{
    ViewModel = viewModel;
    BrowserViewModel = browserViewModel;

    // Subscribe to events
    BrowserViewModel.FractalSelected += OnFractalSelected;
}
```

**Step 3:** Update FractalBrowserView to accept ViewModel:
```csharp
public sealed partial class FractalBrowserView : UserControl
{
    public FractalBrowserViewModel ViewModel { get; set; }

    public FractalBrowserView()
    {
        InitializeComponent();
    }
}
```

**Step 4:** Set in MainPage.xaml.cs:
```csharp
BrowserView.ViewModel = BrowserViewModel;
BrowserView.DataContext = BrowserViewModel;
```

**Step 5:** Fix XAML binding (FractalBrowserView.xaml):
```xml
<!-- OLD: ❌ -->
Command="{Binding ElementName=BrowserViewRoot, Path=ViewModel.SelectFractalCommand}"

<!-- NEW: ✅ -->
Command="{Binding SelectFractalCommand}"
```

---

### Task 3: Create Fractal Metadata Service Layer
**Priority:** P1 - Enables proper separation of concerns
**Estimated Effort:** 1 day

#### Problem
Direct C++/CLI calls in UI layer:
```csharp
var fractalInfo = ManpCore.Native.FractalRegistryWrapper.GetFractalInfo(e.Fractal.Name);
```

This causes:
- Threading violations (C++/CLI on UI thread)
- No caching (repeated P/Invoke overhead)
- Tight coupling to native layer

#### Solution
**File:** `ManpWinUI\Services\FractalMetadataService.cs`

```csharp
public interface IFractalMetadataService
{
    /// <summary>
    /// Initialize and cache all fractal metadata at startup
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Get cached fractal metadata (thread-safe)
    /// </summary>
    FractalDescriptor? GetFractal(string name);

    /// <summary>
    /// Get all fractals in a category
    /// </summary>
    IReadOnlyList<FractalDescriptor> GetFractalsByCategory(string category);

    /// <summary>
    /// Get all categories
    /// </summary>
    IReadOnlyList<string> GetCategories();

    /// <summary>
    /// Check if fractal exists
    /// </summary>
    bool Exists(string name);
}

public class FractalMetadataService : IFractalMetadataService
{
    private readonly Dictionary<string, FractalDescriptor> _cache = new();
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized = false;

    public async Task InitializeAsync()
    {
        await _initLock.WaitAsync();
        try
        {
            if (_initialized) return;

            // Load all metadata from native registry on background thread
            await Task.Run(() =>
            {
                var fractals = ManpCore.Native.FractalRegistryWrapper.GetAllFractals();
                foreach (var info in fractals)
                {
                    var descriptor = new FractalDescriptor
                    {
                        Name = info.Name,
                        DisplayName = info.DisplayName,
                        Category = info.Category,
                        Description = info.Description,
                        SupportsJulia = info.SupportsJulia,
                        DefaultCenterX = info.DefaultCenterX,
                        DefaultCenterY = info.DefaultCenterY,
                        DefaultZoom = info.DefaultZoom
                        // Add parameters when Task 1 complete
                    };
                    _cache[info.Name] = descriptor;
                }
            });

            _initialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public FractalDescriptor? GetFractal(string name)
    {
        if (!_initialized)
            throw new InvalidOperationException("Service not initialized");

        return _cache.TryGetValue(name, out var descriptor) ? descriptor : null;
    }

    // ... implement other methods
}

// ManpWinUI\Models\FractalDescriptor.cs
public class FractalDescriptor
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Category { get; set; }
    public string? Description { get; set; }
    public bool SupportsJulia { get; set; }
    public double DefaultCenterX { get; set; }
    public double DefaultCenterY { get; set; }
    public double DefaultZoom { get; set; }
    public FractalParameterSet? Parameters { get; set; }  // From Task 1
}
```

**Update App.xaml.cs:**
```csharp
services.AddSingleton<IFractalMetadataService, FractalMetadataService>();

// In OnLaunched after window creation:
var metadataService = services.GetRequiredService<IFractalMetadataService>();
await metadataService.InitializeAsync();
```

**Update OnFractalSelected:**
```csharp
private async void OnFractalSelected(object? sender, FractalSelectedEventArgs e)
{
    var metadata = _metadataService.GetFractal(e.Fractal.Name);
    if (metadata == null)
    {
        ViewModel.StatusMessage = $"Error: Fractal '{e.Fractal.Name}' not found";
        return;
    }

    // Use cached metadata - no P/Invoke!
    ViewModel.SelectedFractalType = metadata.Name;
    ViewModel.CenterX = metadata.DefaultCenterX;
    // ...
}
```

---

## High Priority Tasks (This Sprint)

### Task 4: Implement Fractal Render Strategy Pattern
**Priority:** P1
**Estimated Effort:** 1.5 days

Create strategy pattern for different fractal types:

```csharp
// ManpWinUI\Services\Rendering\IFractalRenderStrategy.cs
public interface IFractalRenderStrategy
{
    string FractalType { get; }
    bool CanHandle(string fractalType);
    Task<FractalRenderResult> RenderAsync(
        FractalParameterSet parameters,
        int width,
        int height,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default);
    ValidationResult ValidateParameters(FractalParameterSet parameters);
}

// Implementations:
// - StandardEscapeTimeStrategy (Mandelbrot, Julia, BurningShip, etc.)
// - HailstoneStrategy (special rendering)
// - NewtonStrategy (root-finding fractals)
// - AttractorStrategy (3D attractors)
```

**Benefits:**
- Each fractal family has dedicated rendering logic
- Easy to add new fractal families
- Validation logic co-located with rendering
- Testable in isolation

---

### Task 5: Refactor MainViewModel Parameter Properties
**Priority:** P1 (after Task 1)
**Estimated Effort:** 1 day

**Goal:** Replace hard-coded properties with flexible parameter system

**Before:**
```csharp
public partial class MainViewModel
{
    [ObservableProperty]
    public partial double CenterX { get; set; } = -0.5;
    // ... 20+ more properties
}
```

**After:**
```csharp
public partial class MainViewModel
{
    [ObservableProperty]
    private FractalParameterSet? currentParameters;

    // Convenience accessors for backwards compatibility
    public double CenterX 
    { 
        get => (double?)CurrentParameters?.GetValue("center_x") ?? 0.0;
        set => CurrentParameters?.SetValue("center_x", value);
    }

    // Eventually remove these and use parameters directly
}
```

---

### Task 6: Create Threading-Safe Render Coordinator
**Priority:** P2
**Estimated Effort:** 1 day

**Problem:** Render operations cross thread boundaries unsafely

**Solution:**
```csharp
// ManpWinUI\Services\RenderCoordinator.cs
public class RenderCoordinator
{
    private readonly DispatcherQueue _dispatcher;
    private readonly IFractalRenderStrategy[] _strategies;
    private CancellationTokenSource? _currentRender;

    public async Task<FractalRenderResult> RenderAsync(
        FractalDescriptor fractal,
        FractalParameterSet parameters,
        int width,
        int height,
        IProgress<double>? progress = null)
    {
        // Cancel any ongoing render
        _currentRender?.Cancel();
        _currentRender = new CancellationTokenSource();

        // Find appropriate strategy
        var strategy = _strategies.FirstOrDefault(s => s.CanHandle(fractal.Name));
        if (strategy == null)
            throw new NotSupportedException($"No render strategy for {fractal.Name}");

        // Validate on background thread
        var validation = await Task.Run(() => strategy.ValidateParameters(parameters));
        if (!validation.IsValid)
            throw new ArgumentException(validation.ErrorMessage);

        // Render on background thread
        var result = await strategy.RenderAsync(
            parameters, 
            width, 
            height, 
            progress, 
            _currentRender.Token);

        // Marshal bitmap to UI thread
        await _dispatcher.EnqueueAsync(() =>
        {
            // Convert to WriteableBitmap here
        });

        return result;
    }
}
```

---

## Medium Priority Tasks (Next Sprint)

### Task 7: Dynamic Parameter UI Generation
**Priority:** P2 (after Task 1, 5)
**Estimated Effort:** 2 days

Create data templates that auto-generate UI controls from parameter descriptors:

```xml
<!-- ManpWinUI\Views\Properties\ParameterEditorView.xaml -->
<ItemsControl ItemsSource="{x:Bind ViewModel.CurrentParameters.Parameters}">
    <ItemsControl.ItemTemplate>
        <DataTemplate x:DataType="models:FractalParameterDescriptor">
            <!-- Auto-select control based on Type -->
            <Grid>
                <!-- Double/Integer: NumberBox -->
                <NumberBox Visibility="{x:Bind IsNumericType}" ... />

                <!-- Boolean: ToggleSwitch -->
                <ToggleSwitch Visibility="{x:Bind IsBooleanType}" ... />

                <!-- Choice: ComboBox -->
                <ComboBox Visibility="{x:Bind IsChoiceType}" ... />

                <!-- Complex: Two NumberBoxes side-by-side -->
                <StackPanel Visibility="{x:Bind IsComplexType}" ... />
            </Grid>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

---

### Task 8: Parameter Validation Framework
**Priority:** P2
**Estimated Effort:** 1 day

```csharp
// ManpWinUI\Models\Parameters\ParameterValidation.cs
public class ParameterValidator
{
    public ValidationResult Validate(FractalParameterDescriptor param, object value)
    {
        // Type checking
        if (!IsCorrectType(param.Type, value))
            return ValidationResult.Error($"{param.Name} must be {param.Type}");

        // Range checking
        if (param.MinValue != null && Compare(value, param.MinValue) < 0)
            return ValidationResult.Error($"{param.Name} must be >= {param.MinValue}");

        // Custom validation rules
        if (!string.IsNullOrEmpty(param.ValidationRule))
        {
            // Execute validation expression
        }

        return ValidationResult.Success();
    }
}
```

---

### Task 9: Extend Native Registry with Parameter Metadata
**Priority:** P2
**Estimated Effort:** 2 days
**Files to Modify:**
- `ManpCore.Native\FractalRegistry.h`
- `ManpCore.Native\FractalRegistry.cpp`
- `ManpCore.Native\*Family.cpp` (all fractal family implementations)

**Example for LambdaFractal:**
```cpp
// ManpCore.Native\SpecialExoticFamily.cpp
void SpecialExoticFamily::Register()
{
    FractalSpec lambda;
    lambda.name = "Lambda";
    lambda.displayName = "Lambda Fractal";
    lambda.category = "Special & Exotic";
    lambda.description = "Exponential mapping z → λ * z^p * (1 - z)";

    // ADD PARAMETER METADATA:
    lambda.parameters.push_back({
        .key = "lambda_real",
        .displayName = "Lambda Real",
        .type = ParameterType::Double,
        .defaultValue = -0.5,
        .minValue = -2.0,
        .maxValue = 2.0,
        .description = "Real component of λ parameter",
        .editable = true
    });

    lambda.parameters.push_back({
        .key = "lambda_imaginary",
        .displayName = "Lambda Imaginary",
        .type = ParameterType::Double,
        .defaultValue = 0.0,
        .minValue = -2.0,
        .maxValue = 2.0,
        .description = "Imaginary component of λ parameter",
        .editable = true
    });

    lambda.parameters.push_back({
        .key = "exponent",
        .displayName = "Exponent",
        .type = ParameterType::Integer,
        .defaultValue = 2,
        .minValue = 2,
        .maxValue = 10,
        .description = "Power in z^p term",
        .editable = true
    });

    FractalRegistry::Register(lambda);
}
```

---

## Implementation Order

### Week 1: Foundation
- [ ] **Day 1-2:** Task 1 (Flexible Parameter System) - Core models and interfaces
- [ ] **Day 3:** Task 2 (Fix DI Pattern) - Immediate bug fix
- [ ] **Day 4:** Task 3 (Metadata Service) - Caching layer
- [ ] **Day 5:** Integration testing of Tasks 1-3

### Week 2: Rendering Refactor
- [ ] **Day 1-2:** Task 4 (Render Strategy Pattern)
- [ ] **Day 3:** Task 5 (Refactor MainViewModel)
- [ ] **Day 4:** Task 6 (Render Coordinator)
- [ ] **Day 5:** Integration testing

### Week 3: UI & Native Integration
- [ ] **Day 1-2:** Task 7 (Dynamic Parameter UI)
- [ ] **Day 2:** Task 8 (Validation Framework)
- [ ] **Day 3-5:** Task 9 (Native Parameter Metadata)

### Week 4: Testing & Polish
- [ ] End-to-end testing with all 50+ fractals
- [ ] Performance profiling
- [ ] Documentation updates
- [ ] Remove deprecated code

---

## Success Criteria

### Functional Requirements
✅ All fractals in native registry work in WinUI
✅ No WinRT exceptions when selecting fractals
✅ Parameters auto-populate based on fractal type
✅ UI auto-generates parameter controls
✅ Settings persist per-fractal parameters

### Non-Functional Requirements
✅ No hard-coded fractal names in ViewModels
✅ All rendering happens on background threads
✅ UI remains responsive during rendering
✅ Memory leaks eliminated (proper disposal)
✅ Code coverage > 80% for parameter system

### Architecture Goals
✅ Proper separation of concerns (MVVM respected)
✅ Dependency injection throughout
✅ Strategy pattern for rendering
✅ Single source of truth for metadata (native registry)
✅ Flexible for future fractal types (no code changes needed)

---

## Risk Mitigation

### Risk: Breaking Existing Functionality
**Mitigation:** 
- Implement new system alongside old
- Use feature flags to toggle between systems
- Keep hard-coded properties as compatibility shim
- Extensive regression testing before removal

### Risk: Performance Degradation
**Mitigation:**
- Cache metadata at startup (Task 3)
- Profile before/after benchmarks
- Use compiled bindings where possible
- Minimize thread marshaling overhead

### Risk: Scope Creep
**Mitigation:**
- Strict task prioritization (P0, P1, P2)
- Focus on flexible parameters first (core requirement)
- Defer nice-to-have features to future sprints
- Time-box each task

---

## Testing Strategy

### Unit Tests Needed
- `FractalParameterSet` - Get/Set/Validate operations
- `ParameterValidator` - All validation rules
- `FractalMetadataService` - Caching behavior
- `FractalRenderStrategy` - Each strategy independently

### Integration Tests Needed
- Parameter system → Native registry roundtrip
- Browser selection → Parameter loading → Render pipeline
- Settings persistence → Parameter restoration
- Thread safety under concurrent renders

### Manual Testing Checklist
- [ ] Click each fractal in browser - no exceptions
- [ ] Modify parameters - auto re-render works
- [ ] Switch between fractals rapidly - no crashes
- [ ] Close/reopen app - parameters persist
- [ ] Zoom/pan - parameters update correctly
- [ ] Julia mode toggle - parameters adapt

---

## Technical Debt to Address

### Current Debt
1. **Hard-coded fractal types** in MainViewModel switch statements
2. **Direct P/Invoke** in UI event handlers
3. **Manual parameter synchronization** between 3 ViewModels
4. **No validation** of parameter ranges
5. **ElementName bindings** causing threading issues

### New Debt Created (Acceptable)
1. **Backwards compatibility shims** for old properties (remove in cleanup phase)
2. **Dual parameter systems** during migration (temporary)

### Debt Prevention
- Automated tests prevent regression
- Code reviews enforce patterns
- Architecture decision records (ADRs) document choices

---

## Next Steps

### Immediate Actions (Today)
1. Review this document with team
2. Create GitHub issues for P0 tasks
3. Set up feature branch: `feature/flexible-parameters`
4. Begin Task 1 (Parameter System Models)

### This Week
1. Complete Tasks 1, 2, 3
2. Daily standups to track progress
3. Document any blockers immediately

### Decision Points
- **After Task 1:** Review parameter API with team before proceeding
- **After Week 1:** Go/No-go decision on full refactor vs. incremental approach
- **After Week 2:** Performance review - any red flags?

---

## Questions to Answer

1. **Should we support custom user-defined fractals?**
   - Impact: Need scripting layer or formula parser
   - Defer to Phase 2

2. **Should parameters support expressions/formulas?**
   - Example: "zoom * 2" as parameter value
   - Useful for advanced users
   - Add to P3 backlog

3. **Should we version parameter schemas?**
   - For backwards compatibility with saved settings
   - Important if shipping to users
   - Add versioning support in Task 1

4. **Should Julia parameters be first-class?**
   - Currently special-cased in many places
   - Make them regular parameters
   - Simplifies code significantly

---

## Resources

### Documentation to Write
- [ ] Parameter System API Reference
- [ ] How to Add a New Fractal Type
- [ ] Render Strategy Implementation Guide
- [ ] Testing Best Practices

### Code Examples to Create
- [ ] Sample custom render strategy
- [ ] Sample fractal with custom parameters
- [ ] Sample parameter validation rule

### Tools Needed
- Unit test framework (xUnit/NUnit)
- Mocking framework (Moq)
- Performance profiler (PerfView/Visual Studio Profiler)

---

**Document Owner:** Development Team  
**Last Updated:** 2024  
**Status:** Ready for Implementation  
**Tracking:** Link to GitHub Project/Issues
