# Cellular Automata: Real-Time Statistics Implementation Guide

**Date**: 2025-01-16  
**Feature**: CA Real-Time Statistics Display  
**Related**: `CELLULAR_AUTOMATA_IMPLEMENTATION_CHECKLIST.md` - Phase 11

---

## Overview

This document provides detailed implementation guidance for adding real-time statistics tracking and display to the Cellular Automata rendering system. Statistics will appear on the status bar (bottom of MainPage) and update with each render.

---

## 1. Statistics Data Model

### CAStatistics.cs

Create a new model class to hold all CA statistics:

**File**: `ManpWinUI/Models/CellularAutomata/CAStatistics.cs`

```csharp
namespace ManpWinUI.Models.CellularAutomata;

/// <summary>
/// Real-time statistics for Cellular Automata simulation.
/// Tracks generation progress, cell counts, and cell lifetimes.
/// </summary>
public class CAStatistics
{
    /// <summary>
    /// Current generation number (starts at 0).
    /// Acts as a "clock" counting each iteration.
    /// </summary>
    public int Generation { get; set; }

    /// <summary>
    /// Number of currently alive (active) cells.
    /// </summary>
    public int AliveCells { get; set; }

    /// <summary>
    /// Total number of cells in the grid (gridWidth × 1).
    /// Note: For 1D CA, this is just the width of one row.
    /// </summary>
    public int TotalCells { get; set; }

    /// <summary>
    /// Proportion of alive cells to total cells (0.0 to 1.0).
    /// </summary>
    public double AliveProportionality => TotalCells > 0 
        ? (double)AliveCells / TotalCells 
        : 0.0;

    /// <summary>
    /// Average number of generations cells have been continuously alive.
    /// </summary>
    public double AverageLifetime { get; set; }

    /// <summary>
    /// Maximum number of generations any single cell has been continuously alive.
    /// </summary>
    public int LongestLifetime { get; set; }

    /// <summary>
    /// Formatted string for display on status bar.
    /// Example: "Gen: 42 | Alive: 157/400 (39.3%) | Avg Life: 8.3 | Max Life: 25"
    /// </summary>
    public string FormattedDisplay =>
        $"Gen: {Generation} | Alive: {AliveCells}/{TotalCells} ({AliveProportionality:P1}) | " +
        $"Avg Life: {AverageLifetime:F1} | Max Life: {LongestLifetime}";

    /// <summary>
    /// Creates a default instance with all values set to zero.
    /// </summary>
    public CAStatistics()
    {
        Generation = 0;
        AliveCells = 0;
        TotalCells = 0;
        AverageLifetime = 0.0;
        LongestLifetime = 0;
    }
}
```

---

## 2. Cell Lifetime Tracking

### Implementation Strategy

To track cell lifetimes, we need to monitor:
1. **When cells are born** (transition from dead to alive)
2. **How long cells stay alive** (consecutive generations)
3. **When cells die** (transition from alive to dead)

### Data Structure

Add to `CellularAutomatonRenderService.cs`:

```csharp
/// <summary>
/// Tracks how many consecutive generations each cell has been alive.
/// Index = cell position, Value = generations alive (0 = dead)
/// </summary>
private int[] _cellLifetimes;

/// <summary>
/// Previous generation state for detecting births/deaths.
/// </summary>
private bool[] _previousState;
```

### Initialization

```csharp
// In RenderElementaryCA() method, before generation loop:
_cellLifetimes = new int[gridWidth];
_previousState = new bool[gridWidth];
Array.Copy(initialState, _previousState, gridWidth);

// Initialize lifetimes for initial pattern
for (int i = 0; i < gridWidth; i++)
{
    _cellLifetimes[i] = initialState[i] ? 1 : 0;
}
```

### Tracking Method

```csharp
/// <summary>
/// Update cell lifetime tracking after applying CA rule.
/// </summary>
/// <param name="currentState">Current generation state (alive/dead)</param>
/// <param name="gridWidth">Number of cells in the row</param>
private void TrackCellLifetimes(bool[] currentState, int gridWidth)
{
    for (int i = 0; i < gridWidth; i++)
    {
        if (currentState[i])
        {
            // Cell is alive
            if (_previousState[i])
            {
                // Cell was alive last generation -> increment lifetime
                _cellLifetimes[i]++;
            }
            else
            {
                // Cell just born -> reset lifetime to 1
                _cellLifetimes[i] = 1;
            }
        }
        else
        {
            // Cell is dead -> reset lifetime to 0
            _cellLifetimes[i] = 0;
        }
    }

    // Copy current state to previous for next iteration
    Array.Copy(currentState, _previousState, gridWidth);
}
```

---

## 3. Statistics Computation

### Compute Statistics Method

```csharp
/// <summary>
/// Compute current CA statistics based on cell states and lifetimes.
/// </summary>
/// <param name="generation">Current generation number</param>
/// <param name="currentState">Current alive/dead state of cells</param>
/// <param name="gridWidth">Total number of cells</param>
/// <returns>Populated CAStatistics object</returns>
private CAStatistics ComputeStatistics(int generation, bool[] currentState, int gridWidth)
{
    var stats = new CAStatistics
    {
        Generation = generation,
        TotalCells = gridWidth
    };

    // Count alive cells
    int aliveCount = 0;
    for (int i = 0; i < gridWidth; i++)
    {
        if (currentState[i])
            aliveCount++;
    }
    stats.AliveCells = aliveCount;

    // Compute average and max lifetime (only for alive cells)
    if (aliveCount > 0)
    {
        int totalLifetime = 0;
        int maxLifetime = 0;

        for (int i = 0; i < gridWidth; i++)
        {
            if (currentState[i])
            {
                int lifetime = _cellLifetimes[i];
                totalLifetime += lifetime;
                if (lifetime > maxLifetime)
                    maxLifetime = lifetime;
            }
        }

        stats.AverageLifetime = (double)totalLifetime / aliveCount;
        stats.LongestLifetime = maxLifetime;
    }
    else
    {
        // No alive cells
        stats.AverageLifetime = 0.0;
        stats.LongestLifetime = 0;
    }

    return stats;
}
```

---

## 4. Integration into Rendering Loop

### Updated RenderElementaryCA Method

```csharp
public (byte[] pixels, CAStatistics statistics) RenderElementaryCA(
    /* existing parameters */,
    int gridWidth,
    int gridHeight,
    /* other parameters */)
{
    byte[] pixels = new byte[width * height * 4];

    // 1. Fill background
    FillBackground(pixels, width, height, isDarkTheme);

    // 2. Initialize tracking
    _cellLifetimes = new int[gridWidth];
    _previousState = new bool[gridWidth];
    bool[] state = InitializePattern(initialPattern, gridWidth);
    Array.Copy(state, _previousState, gridWidth);

    for (int i = 0; i < gridWidth; i++)
        _cellLifetimes[i] = state[i] ? 1 : 0;

    // 3. Simulate generations
    CAStatistics finalStats = null;
    for (int gen = 0; gen < generations; gen++)
    {
        // Draw current generation
        for (int cell = 0; cell < gridWidth; cell++)
        {
            if (state[cell])
            {
                var color = GenerationToSpectralColor(gen, generations, /* color params */);
                DrawCell(pixels, width, height, cell, gen, cellSize, color);
            }
        }

        // Apply rule to get next generation
        state = ApplyRule(state, rule, wrapEdges);

        // Track lifetimes
        TrackCellLifetimes(state, gridWidth);

        // Compute statistics (final generation)
        if (gen == generations - 1)
        {
            finalStats = ComputeStatistics(gen + 1, state, gridWidth);
        }
    }

    // 4. Draw grid overlay
    if (showGrid)
        DrawGrid(pixels, width, height, cellSize, gridWidth, gridHeight);

    return (pixels, finalStats ?? new CAStatistics());
}
```

---

## 5. MainViewModel Integration

### Add Statistics Property

**File**: `ManpWinUI/ViewModels/MainViewModel.cs`

```csharp
using ManpWinUI.Models.CellularAutomata;

public partial class MainViewModel : ObservableObject
{
    // Existing properties...

    /// <summary>
    /// Current Cellular Automata statistics (if CA is active).
    /// </summary>
    [ObservableProperty]
    private CAStatistics? _caStatistics;

    /// <summary>
    /// Visibility for CA statistics display (visible only when CA is active).
    /// </summary>
    public Visibility CAStatisticsVisibility => 
        CAStatistics != null ? Visibility.Visible : Visibility.Collapsed;

    // Trigger property changed when CAStatistics updates
    partial void OnCAStatisticsChanged(CAStatistics? value)
    {
        OnPropertyChanged(nameof(CAStatisticsVisibility));
    }
}
```

---

## 6. FractalRenderService Integration

### Propagate Statistics from CA Renderer

**File**: `ManpWinUI/Services/FractalRenderService.cs`

```csharp
private async Task<FractalRenderResult> RenderCellularAutomatonAsync(
    Dictionary<string, object> extendedParameters,
    int width,
    int height,
    ElementTheme currentTheme)
{
    // Extract parameters...
    int gridWidth = GetIntParameter(extendedParameters, "gridWidth", 100);
    // ... other parameters ...

    var caRenderer = new CellularAutomatonRenderService();

    return await Task.Run(() =>
    {
        // Call renderer (now returns tuple with statistics)
        var (pixels, statistics) = caRenderer.RenderElementaryCA(
            /* all parameters */,
            gridWidth: gridWidth,
            gridHeight: gridHeight,
            /* ... */
        );

        _logger.LogInformation(
            "CA Statistics: Gen {Gen}, Alive {Alive}/{Total} ({Prop:P1}), Avg Life {AvgLife:F1}, Max Life {MaxLife}",
            statistics.Generation, statistics.AliveCells, statistics.TotalCells,
            statistics.AliveProportionality, statistics.AverageLifetime, statistics.LongestLifetime
        );

        return new FractalRenderResult 
        { 
            PixelData = pixels,
            CAStatistics = statistics  // Add statistics to result
        };
    });
}
```

### Update FractalRenderResult

```csharp
public class FractalRenderResult
{
    public byte[]? PixelData { get; set; }
    public string? ErrorMessage { get; set; }
    public CAStatistics? CAStatistics { get; set; }  // NEW
}
```

---

## 7. MainPage Status Bar UI

### Update MainPage.xaml

Add statistics display to status bar (Grid.Row="2"):

```xaml
<!-- Status Bar (Grid.Row="2") -->
<Grid Grid.Row="2" 
      Background="{ThemeResource SystemControlBackgroundChromeMediumBrush}"
      Padding="12,4">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="*" />      <!-- CA Stats (NEW) -->
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="Auto" />
    </Grid.ColumnDefinitions>

    <!-- Left: Visualization name -->
    <TextBlock Grid.Column="0"
               VerticalAlignment="Center"
               Margin="0,0,16,0"
               FontWeight="SemiBold"
               Text="{x:Bind ViewModel.CurrentVisualizationName, Mode=OneWay}"
               Visibility="{x:Bind ViewModel.CurrentVisualizationName, Mode=OneWay, Converter={StaticResource EmptyStringToCollapsedConverter}}" />

    <!-- Left-Center: Status message -->
    <TextBlock Grid.Column="1"
               VerticalAlignment="Center"
               Margin="0,0,16,0"
               Text="{x:Bind ViewModel.StatusMessage, Mode=OneWay}" />

    <!-- Center: CA Statistics (NEW) -->
    <TextBlock Grid.Column="2"
               VerticalAlignment="Center"
               Margin="0,0,16,0"
               FontFamily="Consolas"
               FontSize="12"
               Foreground="{ThemeResource SystemAccentColor}"
               Text="{x:Bind ViewModel.CAStatistics.FormattedDisplay, Mode=OneWay}"
               Visibility="{x:Bind ViewModel.CAStatisticsVisibility, Mode=OneWay}"
               ToolTipService.ToolTip="Cellular Automata Statistics: Generation counter, alive cells, proportionality, and cell lifetimes" />

    <!-- Right: Render time -->
    <TextBlock Grid.Column="3"
               VerticalAlignment="Center"
               Opacity="0.7"
               Margin="0,0,8,0"
               FontSize="12">
        <Run Text="⏱️ " />
        <Run Text="{x:Bind ViewModel.LastRenderTime, Mode=OneWay}" />
    </TextBlock>

    <!-- Copy Button -->
    <Button Grid.Column="4"
            Command="{x:Bind ViewModel.CopyStatusInfoCommand}"
            Background="Transparent"
            BorderThickness="0"
            Padding="6,4"
            VerticalAlignment="Center"
            ToolTipService.ToolTip="Copy status information to clipboard">
        <FontIcon Glyph="&#xE8C8;" FontSize="14" />
    </Button>
</Grid>
```

---

## 8. Update Statistics on Render

### MainPage.cs Event Handler

```csharp
private async void OnFractalSelected(object sender, FractalSelectedEventArgs e)
{
    // ... existing code ...

    // Render fractal
    var result = await ViewModel.RenderFractalAsync();

    // Update CA statistics if available
    if (result.CAStatistics != null)
    {
        ViewModel.CAStatistics = result.CAStatistics;
    }
    else
    {
        // Clear statistics when switching to non-CA fractal
        ViewModel.CAStatistics = null;
    }

    // ... existing code ...
}
```

---

## 9. Testing Checklist

### Core Statistics Accuracy

- [ ] Generation counter starts at 0 and increments correctly
- [ ] Alive cell count matches visual inspection
- [ ] Total cells = gridWidth
- [ ] Proportionality = AliveCells / TotalCells (accurate percentage)

### Lifetime Tracking

- [ ] Single cell at center: lifetime = generation number
- [ ] Rule 90 (deterministic): predictable lifetime patterns
- [ ] Rule 30 (chaotic): varying lifetime patterns
- [ ] Cells that die: lifetime resets to 0
- [ ] Cells that are born: lifetime starts at 1

### Edge Cases

- [ ] All cells dead: AliveCells=0, AverageLifetime=0, LongestLifetime=0
- [ ] All cells alive: AliveCells=TotalCells, Proportionality=100%
- [ ] Generation 0: All statistics initialize correctly
- [ ] Large generation count (500): No performance degradation

### UI Display

- [ ] Statistics appear on status bar when CA is rendered
- [ ] Statistics disappear when switching to non-CA fractal
- [ ] Formatted display is readable and concise
- [ ] Tooltip explains what each metric means
- [ ] Monospace font (Consolas) ensures alignment

### Integration

- [ ] MainViewModel.CAStatistics updates on each render
- [ ] FractalRenderResult includes CAStatistics
- [ ] Statistics reset when parameters change
- [ ] Statistics persist across re-renders with same parameters

---

## 10. Performance Considerations

### Memory Overhead

- **Lifetime array**: `int[gridWidth]` = ~400-800 bytes (100-200 cells)
- **Previous state**: `bool[gridWidth]` = ~100-200 bytes
- **Total**: <1 KB per render

### Computation Overhead

- **Lifetime tracking**: O(gridWidth) per generation = O(gridWidth × generations)
- **Statistics computation**: O(gridWidth) once at end
- **Expected impact**: <1ms for typical parameters (100 cells, 100 generations)

### Optimization Tips

1. **Reuse arrays**: Don't allocate new arrays each generation
2. **Single pass**: Compute statistics during drawing loop (avoid separate pass)
3. **Lazy formatting**: Only format display string when requested (use computed property)

---

## 11. Example Statistics Output

### Rule 90 (Sierpinski Triangle)
```
Gen: 50 | Alive: 26/100 (26.0%) | Avg Life: 8.5 | Max Life: 50
```
- **Interpretation**: Generation 50, 26 cells alive, average cell has lived 8.5 generations, one cell has survived all 50 generations (center seed)

### Rule 30 (Chaotic)
```
Gen: 100 | Alive: 47/100 (47.0%) | Avg Life: 3.2 | Max Life: 15
```
- **Interpretation**: Generation 100, 47 cells alive, cells are short-lived (avg 3.2 gens), longest-lived cell survived 15 generations

### Rule 110 (Complex)
```
Gen: 200 | Alive: 38/100 (38.0%) | Avg Life: 12.7 | Max Life: 68
```
- **Interpretation**: Generation 200, 38 cells alive, moderate lifetimes (12.7 avg), some persistent structures (68 generations)

---

## 12. Troubleshooting

### Statistics Not Appearing

**Problem**: Status bar shows no statistics  
**Check**:
- Is `ViewModel.CAStatistics` being set after render?
- Is `CAStatisticsVisibility` binding correct?
- Is the TextBlock's `Visibility` property bound correctly?

### Incorrect Alive Count

**Problem**: Alive count doesn't match visual  
**Check**:
- Is `TrackCellLifetimes()` being called after each generation?
- Is `_previousState` being copied correctly?
- Are cells being drawn if `state[cell] == true`?

### Lifetime Always Zero

**Problem**: Average/Max lifetime shows 0  
**Check**:
- Is `_cellLifetimes` being initialized with initial pattern?
- Is lifetime incrementing for alive cells?
- Is statistics computation iterating only over alive cells?

### Performance Issues

**Problem**: Render slows down with statistics  
**Solutions**:
- Profile with dotTrace/PerfView
- Ensure arrays are reused, not reallocated
- Consider computing statistics every N generations, not every one

---

## 13. Future Enhancements (Out of Scope)

### Advanced Statistics
- [ ] Birth rate (cells born per generation)
- [ ] Death rate (cells died per generation)
- [ ] Stability metric (how much pattern changes per generation)
- [ ] Entropy calculation (Shannon entropy of cell distribution)

### Historical Tracking
- [ ] Graph of alive cells over time
- [ ] Lifetime histogram (distribution of lifetimes)
- [ ] Animation playback of generation history

### Export
- [ ] CSV export of statistics per generation
- [ ] JSON export for data analysis
- [ ] Copy statistics to clipboard (formatted)

---

## 14. Code Style Checklist

- [ ] XML documentation comments on all public methods
- [ ] Debug logging for key statistics computations
- [ ] Guard clauses for null/invalid parameters
- [ ] Meaningful variable names (`aliveCount`, not `ac`)
- [ ] LINQ usage where appropriate (but avoid for hot paths)
- [ ] Consistent formatting (match project style)

---

## 15. References

### Related Documentation
- `CELLULAR_AUTOMATA_IMPLEMENTATION_CHECKLIST.md` - Master checklist (Phase 11)
- `CA_THEME_AND_GRID_IMPLEMENTATION.md` - Theme and grid implementation
- `CA_VISUAL_DESIGN_REFERENCE.md` - Visual design specs

### Code References
- `ManpWinUI/Services/LSystemRenderService.cs` - Similar rendering service pattern
- `ManpWinUI/ViewModels/MainViewModel.cs` - ViewModel pattern
- `ManpWinUI/Views/MainPage.xaml` - Status bar layout

---

**Last Updated**: 2025-01-16  
**Maintainer**: Development Team  
**Status**: Implementation guide complete
