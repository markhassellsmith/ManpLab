# Code Modularization Guidelines & Analysis

**Goal:** Keep files small, focused, and maintainable (typically < 500 lines / < 20KB)

---

## ✅ Current State - Good Modularization Examples

### MainPage Partial Classes (Already Well Modularized)

The `MainPage` class demonstrates **excellent modularization** through partial classes:

```
MainPage.cs (core initialization)
├── MainPage.KeyboardHandling.cs    (14.4 KB, 360 lines) ✓
├── MainPage.MouseInteraction.cs    (21.1 KB, 456 lines) ⚠️
├── MainPage.EventHandlers.cs       (15.1 KB, 371 lines) ✓
├── MainPage.HailstoneLabels.cs     (20.5 KB, 447 lines) ⚠️
├── MainPage.HailstoneInfo.cs
└── MainPage.CoordinateAxes.cs
```

**Strengths:**
- Clear separation of concerns (keyboard, mouse, rendering)
- Each file has single responsibility
- Easy to navigate and maintain
- Follows WinUI best practices

**Recommendations:**
- ✓ Keep this pattern for all page classes
- ⚠️ Consider splitting `MouseInteraction` into click/drag/zoom handlers
- ⚠️ Consider splitting `HailstoneLabels` into placement/rendering concerns

---

## ⚠️ Files Needing Modularization

### 1. HailstoneRenderServiceWin2D.cs
**Current:** 700 lines, 29 KB

**Analysis:**
```csharp
// Current structure (one large file):
public class HailstoneRenderServiceWin2D
{
    // Rendering pipeline
    public Task<HailstoneRenderResult> RenderSequenceAsync(...)

    // Grid drawing
    private void DrawGridAndAxes(...)
    private void DrawGridAndAxesWithTransform(...)
    private void DrawAxisTickMarks(...)

    // Sequence drawing
    private void DrawSequencePath(...)
    private void DrawSequencePathWithTransform(...)

    // Point drawing
    private void DrawPoints(...)
    private void DrawPointsWithTransform(...)

    // Text rendering
    private void DrawPointLabels(...)
    private void DrawInfoText(...)

    // Transform calculations
    private void SetupCoordinateTransform(...)
    private (double, double, double, double) CalculateTransform(...)
    private (float, float) WorldToScreen(...)

    // Helper utilities
    private int CalculateTickSpacing(...)
    private static int CalculateNextX(...)
    private static int CalculateNextY(...)
}
```

**Recommended Refactoring:**

Split into focused classes using composition:

```
Services/Hailstone/
├── HailstoneRenderServiceWin2D.cs       (main coordinator, ~150 lines)
├── HailstoneGridRenderer.cs             (grid/axes rendering, ~150 lines)
├── HailstoneTrajectoryRenderer.cs       (path/points, ~150 lines)
├── HailstoneCoordinateTransform.cs      (transforms, ~100 lines)
└── HailstoneRenderHelpers.cs            (utilities, ~100 lines)
```

**Benefits:**
- Each class < 200 lines
- Single responsibility per file
- Easier to test independently
- Better code navigation
- Reusable components

---

### 2. HailstoneRenderService.cs
**Current:** 731 lines, 28.1 KB

**Same modularization approach as Win2D version**

This is the legacy pixel-based renderer. Apply the same pattern:

```
Services/Hailstone/
├── HailstoneRenderService.cs            (main coordinator)
├── HailstonePixelRenderer.cs            (pixel-based drawing)
├── HailstoneColorCalculator.cs          (color mapping)
└── (shared components from above)
```

**Consolidation Opportunity:**
Both render services share:
- Transform calculations
- Grid drawing logic (concepts, not implementation)
- Helper utilities

Consider creating:
```
Services/Hailstone/Common/
├── IHailstoneRenderer.cs                (interface)
├── HailstoneTransformCalculator.cs      (shared transforms)
└── HailstoneRenderConfig.cs             (configuration)
```

---

### 3. MainViewModel.cs
**Current:** 759 lines, 27.9 KB

**Analysis:**
This is a **monolithic ViewModel** handling multiple concerns:

```csharp
public class MainViewModel : ObservableObject
{
    // Mandelbrot rendering
    // Julia set rendering  
    // Hailstone rendering
    // UI state management
    // Color palette management
    // Mouse interaction state
    // Keyboard shortcuts
    // File operations (future)
    // Animation (future)
}
```

**Recommended Refactoring:**

Split into feature-specific ViewModels:

```
ViewModels/
├── MainViewModel.cs                     (app state, navigation, ~200 lines)
├── MandelbrotViewModel.cs               (fractal rendering, ~200 lines)
├── HailstoneViewModel.cs                (sequence rendering, ~200 lines)
├── PaletteViewModel.cs                  (color management, ~150 lines)
└── RenderProgressViewModel.cs           (progress tracking, ~100 lines)
```

**Integration Pattern:**
```csharp
public class MainViewModel : ObservableObject
{
    // Child ViewModels (composition)
    public MandelbrotViewModel Mandelbrot { get; }
    public HailstoneViewModel Hailstone { get; }
    public PaletteViewModel Palette { get; }
    public RenderProgressViewModel Progress { get; }

    // App-level state only
    public string CurrentMode { get; set; }
    public bool IsBusy { get; set; }

    // Coordination between ViewModels
    public void SwitchToMandelbrot() { ... }
    public void SwitchToHailstone() { ... }
}
```

**Benefits:**
- Each ViewModel < 250 lines
- Feature isolation
- Easier testing (test MandelbrotViewModel separately)
- Better MVVM pattern adherence
- Supports future features without bloating main VM

---

## 📏 Modularization Guidelines

### File Size Targets

| Size Range | Status | Action |
|------------|--------|--------|
| < 300 lines / 15 KB | ✅ Ideal | No action needed |
| 300-500 lines / 15-20 KB | ⚠️ Monitor | Consider splitting when adding features |
| 500-700 lines / 20-30 KB | 🔶 Review | Split into logical components |
| > 700 lines / 30 KB | ❌ Refactor | Immediate modularization priority |

### When to Split

Split a file when:
1. **Multiple responsibilities** - File handles more than one concern
2. **Hard to navigate** - Scrolling > 2 screens to find code
3. **Testing difficulty** - Can't test one feature without others
4. **Merge conflicts** - Multiple developers editing same file
5. **Unclear naming** - Can't describe file purpose in one sentence

### When NOT to Split

Keep files together when:
1. **Tight coupling** - Methods depend heavily on each other
2. **Small total size** - < 300 lines even with multiple concerns
3. **Clear cohesion** - All code serves one clear purpose
4. **Partial classes** - Already using partial for organization

---

## 🏗️ Recommended Patterns

### 1. Partial Classes (for UI code-behind)

```csharp
// Use for large page/control classes
MainPage.cs                 // Core initialization
MainPage.Keyboard.cs        // Keyboard handling
MainPage.Mouse.cs           // Mouse handling
MainPage.Rendering.cs       // Rendering logic
```

**Best for:** UI code-behind, ViewModels with many commands

---

### 2. Composition (for services)

```csharp
// Main service coordinates sub-services
public class HailstoneRenderService
{
    private readonly HailstoneGridRenderer _gridRenderer;
    private readonly HailstoneTrajectoryRenderer _trajectoryRenderer;
    private readonly HailstoneTransformCalculator _transform;

    public Task<Result> RenderAsync(...)
    {
        _gridRenderer.Draw(...);
        _trajectoryRenderer.Draw(...);
    }
}
```

**Best for:** Services, complex business logic, rendering pipelines

---

### 3. Inheritance (for related implementations)

```csharp
// Base class with common logic
public abstract class HailstoneRendererBase
{
    protected abstract void DrawLine(...);
    protected abstract void DrawCircle(...);

    // Common algorithms use abstract methods
    public void DrawGrid(...) { /* uses DrawLine */ }
}

// Specific implementations
public class HailstoneRendererWin2D : HailstoneRendererBase { }
public class HailstoneRendererPixel : HailstoneRendererBase { }
```

**Best for:** Multiple implementations of same concept

---

### 4. Strategy Pattern (for interchangeable algorithms)

```csharp
// Interface
public interface IColorMapper
{
    Color MapIterationToColor(int iteration, int maxIterations);
}

// Implementations
public class SpectrumColorMapper : IColorMapper { }
public class GrayscaleColorMapper : IColorMapper { }
public class PaletteColorMapper : IColorMapper { }

// Usage
public class FractalRenderer
{
    private IColorMapper _colorMapper;

    public void SetColorMapper(IColorMapper mapper)
    {
        _colorMapper = mapper;
    }
}
```

**Best for:** Pluggable behavior, runtime switching

---

## 📋 Immediate Action Plan

### Priority 1: MainViewModel Refactoring
**Impact:** High - Central to all UI operations  
**Effort:** Medium - 2-3 days  
**Risk:** Medium - Core component, needs testing

**Steps:**
1. Create `ViewModels/Features/` directory
2. Extract `MandelbrotViewModel` (fractal-specific logic)
3. Extract `HailstoneViewModel` (sequence-specific logic)
4. Extract `PaletteViewModel` (color management)
5. Update `MainViewModel` to use composition
6. Update XAML bindings (e.g., `{Binding Mandelbrot.CenterX}`)
7. Test all features still work

**Success Criteria:**
- MainViewModel < 250 lines
- Each feature ViewModel < 250 lines
- No functionality regression
- All tests pass

---

### Priority 2: Hailstone Rendering Services
**Impact:** Medium - Isolated to Hailstone feature  
**Effort:** Low - 1-2 days  
**Risk:** Low - Well-defined boundaries

**Steps:**
1. Create `Services/Hailstone/` directory
2. Extract grid rendering to `HailstoneGridRenderer`
3. Extract trajectory rendering to `HailstoneTrajectoryRenderer`
4. Extract transform logic to `HailstoneCoordinateTransform`
5. Update service to use composition
6. Apply same pattern to both Win2D and pixel renderers

**Success Criteria:**
- No file > 200 lines
- Clear single responsibility per class
- Reusable components
- Hailstone tests pass

---

### Priority 3: Future-Proofing
**Impact:** High - Prevents technical debt  
**Effort:** Ongoing  
**Risk:** Low - Proactive measure

**Guidelines:**
1. **New features start in separate files**
   - Animation → `AnimationViewModel.cs`, `AnimationService.cs`
   - Presets → `PresetGalleryViewModel.cs`, `PresetService.cs`

2. **No file exceeds 500 lines**
   - Automated check in CI/CD pipeline
   - Pre-commit hook warning

3. **Regular refactoring sprints**
   - Monthly review of file sizes
   - Proactive splitting before files grow too large

4. **Documentation as you go**
   - Each new class gets XML doc comments
   - README in each directory explaining structure

---

## 🎓 Educational Benefits

Maintaining small files teaches:
- **Single Responsibility Principle** - Each file one job
- **Composition over Inheritance** - Flexible design
- **Dependency Injection** - Loose coupling
- **Interface-Based Design** - Abstraction
- **Testability** - Isolated unit tests

---

## 📊 Metrics to Track

### Current State
| Metric | Current | Target | Status |
|--------|---------|--------|--------|
| Largest source file | 759 lines | < 500 lines | 🔴 |
| Files > 500 lines | 3 | 0 | 🔴 |
| Files > 300 lines | 6 | < 5 | 🟡 |
| Avg file size | ~200 lines | ~150 lines | 🟢 |

### Success Metrics (Post-Refactoring)
- ✅ No source file > 500 lines
- ✅ < 5 files between 300-500 lines
- ✅ Average file size ~150 lines
- ✅ All classes have single responsibility
- ✅ Clear directory structure

---

## 🔧 Tools to Help

### Visual Studio Extensions
- **Code Metrics** - Built-in (Analyze > Calculate Code Metrics)
- **CodeMaid** - Auto-organize files
- **Roslynator** - Refactoring suggestions

### PowerShell Scripts

```powershell
# Find large files
Get-ChildItem -Recurse -Include *.cs | 
  Where-Object { $_.FullName -notmatch '\\obj\\' } |
  Select-Object Name, @{N='Lines';E={(Get-Content $_).Count}} |
  Where-Object { $_.Lines -gt 300 } |
  Sort-Object Lines -Descending

# Count classes per file
Get-ChildItem -Recurse -Include *.cs |
  ForEach-Object {
    $classes = (Get-Content $_) | Select-String 'class ' | Measure-Object
    [PSCustomObject]@{
      File = $_.Name
      Classes = $classes.Count
    }
  } | Where-Object { $_.Classes -gt 1 }
```

---

**Last Updated:** 2026-04-26  
**Status:** Guidelines established, refactoring planned  
**Next Review:** After Priority 1 & 2 completion
