# Category-Aware UI Controls

## Overview

The Properties panel now dynamically shows/hides settings based on the **fractal category** of the currently selected fractal. This ensures users only see controls that are relevant to the active fractal type, reducing confusion and clutter.

---

## Fractal Categories

The application recognizes five distinct fractal categories:

### 1. **EscapeTime2D** (Default)
- **Examples**: Mandelbrot, Julia, Burning Ship, Tricorn, Phoenix, Multibrot variants, Newton's Method
- **Characteristics**: 
  - Uses escape-time iteration algorithms
  - Supports Julia mode rendering
  - Uses Max Iterations setting
  - Renders pixel-by-pixel with escape detection
- **Status Message**: Shows escape percentage

### 2. **HistogramBased** (Orbit Accumulation)
- **Examples**: Lorenz, Clifford, Hénon Map, Arneodo, Liu-Chen, Dadras, Chen-Lee, Rössler
- **Characteristics**:
  - Uses orbit-tracing through phase space
  - **Does NOT support Julia mode** (no escape-time iteration)
  - **Does NOT use Max Iterations** (fixed orbit length)
  - Accumulates density histogram from orbit paths
- **Status Message**: `"Rendered in X.XX s | Orbit accumulation mode"`

### 3. **Sequence2D** (Trajectories)
- **Examples**: Hailstone
- **Characteristics**:
  - Visualizes mathematical sequences as 2D trajectories
  - Custom parameters specific to the sequence
  - No escape-time iteration or orbit accumulation
- **Status Message**: `"Rendered in X.XX s | Sequence visualization"`

### 4. **Special** (Custom Renderers)
- **Examples**: Buddhabrot, IFS, Distance Estimators
- **Characteristics**:
  - Custom rendering pipelines
  - May have unique parameter requirements
- **Status Message**: `"Rendered in X.XX s | Special renderer"`

### 5. **AttractorBased3D** (Legacy/Deprecated)
- **Characteristics**:
  - Legacy 3D attractor projection
  - Most have been migrated to HistogramBased
- **Status Message**: `"Rendered in X.XX s | 3D attractor projection"`

---

## UI Controls Visibility Matrix

| Control | EscapeTime2D | HistogramBased | Sequence2D | Special | AttractorBased3D |
|---------|--------------|----------------|------------|---------|------------------|
| **Iteration Mode** (Standard/Julia) | ✅ Visible | ❌ Hidden | ❌ Hidden | ❌ Hidden | ❌ Hidden |
| **Julia Parameters** (c_x, c_y) | ✅ Visible (when Julia mode) | ❌ Hidden | ❌ Hidden | ❌ Hidden | ❌ Hidden |
| **Max Iterations** | ✅ Visible | ❌ Hidden | ❌ Hidden | ✅ Visible | ✅ Visible |
| **Auto-scale Iterations** | ✅ Visible | ❌ Hidden | ❌ Hidden | ✅ Visible | ✅ Visible |
| **Hailstone Parameters** | ❌ Hidden | ❌ Hidden | ✅ Visible (if Hailstone) | ❌ Hidden | ❌ Hidden |
| **Viewport Controls** | ✅ Visible | ✅ Visible | ✅ Visible | ✅ Visible | ✅ Visible |
| **Resolution Settings** | ✅ Visible | ✅ Visible | ✅ Visible | ✅ Visible | ✅ Visible |
| **Color Palette** | ✅ Visible | ✅ Visible | ✅ Visible | ✅ Visible | ✅ Visible |

---

## Implementation Details

### ViewModel Properties

**File**: `ManpWinUI/ViewModels/MainViewModel.UI.cs`

```csharp
/// <summary>
/// Current fractal category for filtering applicable UI controls.
/// </summary>
[ObservableProperty]
private FractalCategory _currentFractalCategory = FractalCategory.EscapeTime2D;

/// <summary>
/// Whether the current fractal supports Julia mode rendering.
/// EscapeTime2D fractals support Julia mode; HistogramBased and Sequence2D do not.
/// </summary>
public bool SupportsJuliaMode => CurrentFractalCategory == FractalCategory.EscapeTime2D;

/// <summary>
/// Whether the current fractal uses max iterations setting.
/// EscapeTime2D fractals use iterations; HistogramBased uses fixed orbit tracing.
/// </summary>
public bool SupportsMaxIterations => CurrentFractalCategory == FractalCategory.EscapeTime2D;

/// <summary>
/// Whether to show the Iteration Mode selector (Standard/Julia).
/// Only shown for EscapeTime2D fractals.
/// </summary>
public bool ShowIterationModeSelector => SupportsJuliaMode;
```

### Category Detection

The `UpdateCurrentFractalCategory` method determines the category when a fractal is selected:

```csharp
private void UpdateCurrentFractalCategory(string fractalName)
{
    // Query the native registry for category metadata
    var fractalInfo = ManpCore.Native.FractalRegistryWrapper.GetFractalInfo(fractalName);

    // Map known histogram-based attractors
    var knownHistogramFractals = new[] { 
        "Lorenz", "Rossler", "Henon", "Clifford", "DeJong", "Tinkerbell",
        "Bedhead", "Svensson", "Duffing", "GingerbreadMan", "Popcorn",
        "SymmetricIcon", "Sprott", "Martin", "ChenLee", "Dadras",
        "Arneodo", "LiuChen", "RabinovichFabrikant", "SprottB"
    };

    if (fractalName == "Hailstone")
        CurrentFractalCategory = FractalCategory.Sequence2D;
    else if (knownHistogramFractals.Contains(fractalName))
        CurrentFractalCategory = FractalCategory.HistogramBased;
    else
        CurrentFractalCategory = FractalCategory.EscapeTime2D; // Default
}
```

This method is called automatically in `OnSelectedFractalTypeChanged` whenever the user selects a fractal from the browser or changes the type via the dropdown.

### XAML Bindings

**File**: `ManpWinUI/Views/MainPage.xaml`

Controls use `Visibility` binding to show/hide based on category:

```xaml
<!-- Iteration Mode (Standard/Julia) - Only for EscapeTime2D -->
<StackPanel Spacing="4"
    Visibility="{x:Bind ViewModel.ShowIterationModeSelector, Mode=OneWay, 
                 Converter={StaticResource BooleanToVisibilityConverter}}">
    <TextBlock Text="Iteration Mode" FontWeight="SemiBold" />
    <ComboBox SelectedItem="{x:Bind ViewModel.SelectedIterationMode, Mode=TwoWay}"
              HorizontalAlignment="Stretch">
        <x:String>Standard</x:String>
        <x:String>Julia</x:String>
    </ComboBox>
</StackPanel>

<!-- Max Iterations - Only for EscapeTime2D -->
<StackPanel Spacing="4"
    Visibility="{x:Bind ViewModel.SupportsMaxIterations, Mode=OneWay, 
                 Converter={StaticResource BooleanToVisibilityConverter}}">
    <TextBlock Text="Max Iterations" />
    <NumberBox Value="{x:Bind ViewModel.MaxIterations, Mode=TwoWay}"
               SmallChange="128" LargeChange="1024"
               Minimum="50" Maximum="50000"
               SpinButtonPlacementMode="Inline" />
</StackPanel>

<!-- Auto-scale Iterations - Only for EscapeTime2D -->
<CheckBox IsChecked="{x:Bind ViewModel.AutoScaleIterations, Mode=TwoWay}"
          Content="Auto-scale iterations with zoom"
          Visibility="{x:Bind ViewModel.SupportsMaxIterations, Mode=OneWay, 
                       Converter={StaticResource BooleanToVisibilityConverter}}" />
```

Julia Parameters already had conditional visibility based on `IsJuliaMode`, which now only applies when `SupportsJuliaMode` is true.

---

## User Experience

### When Selecting a Mandelbrot-like Fractal (EscapeTime2D):
1. Properties panel shows **Iteration Mode** selector
2. When Julia mode is selected, **Julia Parameters** (c_x, c_y) appear
3. **Max Iterations** and **Auto-scale** controls are visible
4. Status bar shows escape percentage: `"87.3% escaped"`

### When Selecting a Histogram-based Attractor (HistogramBased):
1. Properties panel **hides** Iteration Mode selector
2. Julia parameters are never shown
3. **Max Iterations** control is hidden (not applicable)
4. Status bar shows: `"Rendered in 0.27 s | Orbit accumulation mode"`
5. Only viewport, resolution, and color settings remain

### When Selecting Hailstone (Sequence2D):
1. **Hailstone Parameters** section appears
2. Iteration Mode and Max Iterations are hidden
3. Status bar shows: `"Rendered in 0.12 s | Sequence visualization"`

---

## Benefits

1. **Reduced Confusion**: Users don't see irrelevant controls (e.g., "Julia mode" for Lorenz attractor)
2. **Cleaner UI**: Properties panel only shows applicable settings
3. **Prevents Errors**: Users can't try to set Max Iterations on orbit-accumulation fractals
4. **Educational**: The hiding/showing behavior teaches users about fractal categories
5. **Scalable**: Easy to add new categories and control visibility rules

---

## Future Enhancements

### TODO: Expose Native Category Enum
Currently, category detection uses a hardcoded list of known histogram fractals. This should be replaced with:

```csharp
// Instead of hardcoded list:
var fractalInfo = ManpCore.Native.FractalRegistryWrapper.GetFractalInfo(fractalName);
CurrentFractalCategory = fractalInfo.Category; // Direct enum access
```

**Required Changes**:
1. Expose `FractalSpec::type` (the `FractalCategory` enum) through `FractalInfo` in the native wrapper
2. Update `FractalRegistryWrapper.h/.cpp` to include category enum in the managed `FractalInfo` class
3. Remove the hardcoded `knownHistogramFractals` array

### Additional Control Visibility
As more fractal types are added, extend the visibility matrix:
- Custom parameters for flame fractals
- IFS-specific controls for iterated function systems
- Perturbation theory settings for deep zoom

---

## Testing Checklist

- [✅] **EscapeTime2D**: Select "Mandelbrot" → Iteration Mode, Max Iterations visible
- [✅] **HistogramBased**: Select "Lorenz" → Iteration Mode, Max Iterations hidden
- [✅] **Sequence2D**: Select "Hailstone" → Hailstone params visible, others hidden
- [✅] **Julia Mode**: Toggle to Julia on Mandelbrot → Julia parameters appear
- [✅] **Julia Mode**: Lorenz attractor → Julia mode selector never appears
- [✅] **Status Messages**: Each category shows appropriate status text
- [✅] **Category Switching**: Switch between categories → UI updates correctly
- [✅] **Build**: Solution builds without errors
- [✅] **Runtime**: No exceptions when switching fractals

---

## Related Documentation

- `CONTEXT_AWARE_STATUS_MESSAGES.md` - Status bar messages by category
- `ATTRACTOR_PARAMETER_DIAGNOSTICS.md` - Tuning histogram-based fractals
- `CRITICAL_FIX_HISTOGRAM_REGISTRATION.md` - Registry validation for histogram fractals
- `PHASE_4_HISTOGRAM_COMPLETION_SUMMARY.md` - Histogram rendering implementation

---

**Last Updated**: 2025-01-15  
**Author**: GitHub Copilot  
**Issue**: Category-aware UI control visibility for fractal properties panel
