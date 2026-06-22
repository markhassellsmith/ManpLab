# CA Implementation Quick Reference Card

**Quick lookup for theme, grid, statistics, and animation controls**

---

## 🎨 Colors (Copy-Paste Ready)

### Background Colors
```csharp
// Light Theme
byte bgR = 255, bgG = 255, bgB = 255; // White

// Dark Theme  
byte bgR = 0, bgG = 0, bgB = 0; // Black
```

### Grid Color
```csharp
// Light gray (visible on both themes)
const byte GRID_R = 192;
const byte GRID_G = 192;
const byte GRID_B = 192;
```

---

## 📏 Parameter Specifications

| Parameter | Type | Min | Max | Default |
|-----------|------|-----|-----|---------|
| gridWidth | int | 10 | 200 | 100 |
| gridHeight | int | 10 | 200 | 100 |
| showGrid | bool | - | - | true |
| cellSize | int | 1 | 5 | 2 |

---

## 🔧 Code Snippets

### Theme Detection (MainPage.cs)
```csharp
var theme = this.ActualTheme; // ElementTheme.Light or .Dark
```

### Background Fill (RenderService)
```csharp
void FillBackground(byte[] pixels, int width, int height, bool isDark)
{
    byte bg = isDark ? (byte)0 : (byte)255;
    for (int i = 0; i < pixels.Length; i += 4)
    {
        pixels[i] = pixels[i+1] = pixels[i+2] = bg;
        pixels[i+3] = 255;
    }
}
```

### Draw Vertical Grid Line
```csharp
void DrawVerticalLine(byte[] pixels, int width, int height, int x, int y0, int y1)
{
    const byte GRAY = 192;
    for (int y = Math.Max(0, y0); y <= Math.Min(height-1, y1); y++)
    {
        int idx = (y * width + x) * 4;
        pixels[idx] = pixels[idx+1] = pixels[idx+2] = GRAY;
        pixels[idx+3] = 255;
    }
}
```

### Draw Horizontal Grid Line
```csharp
void DrawHorizontalLine(byte[] pixels, int width, int height, int x0, int x1, int y)
{
    const byte GRAY = 192;
    for (int x = Math.Max(0, x0); x <= Math.Min(width-1, x1); x++)
    {
        int idx = (y * width + x) * 4;
        pixels[idx] = pixels[idx+1] = pixels[idx+2] = GRAY;
        pixels[idx+3] = 255;
    }
}
```

### Grid Dimension Constraints
```csharp
int maxGridWidth = canvasWidth / cellSize;
int maxGridHeight = canvasHeight / cellSize;
gridWidth = Math.Clamp(gridWidth, 10, maxGridWidth);
gridHeight = Math.Clamp(gridHeight, 10, maxGridHeight);
```

---

## 📊 Statistics Model (Copy-Paste Ready)

### CAStatistics.cs
```csharp
public class CAStatistics
{
    public int Generation { get; set; }
    public int AliveCells { get; set; }
    public int TotalCells { get; set; }
    public double AliveProportionality => TotalCells > 0 ? (double)AliveCells / TotalCells : 0.0;
    public double AverageLifetime { get; set; }
    public int LongestLifetime { get; set; }

    public string FormattedDisplay =>
        $"Gen: {Generation} | Alive: {AliveCells}/{TotalCells} ({AliveProportionality:P1}) | " +
        $"Avg Life: {AverageLifetime:F1} | Max Life: {LongestLifetime}";
}
```

### Cell Lifetime Tracking
```csharp
private int[] _cellLifetimes;  // Consecutive generations alive
private bool[] _previousState; // Previous generation for comparison

void TrackCellLifetimes(bool[] currentState, int gridWidth)
{
    for (int i = 0; i < gridWidth; i++)
    {
        if (currentState[i])
            _cellLifetimes[i] = _previousState[i] ? _cellLifetimes[i] + 1 : 1;
        else
            _cellLifetimes[i] = 0;
    }
    Array.Copy(currentState, _previousState, gridWidth);
}
```

### Statistics Computation
```csharp
CAStatistics ComputeStatistics(int generation, bool[] state, int gridWidth)
{
    var stats = new CAStatistics 
    { 
        Generation = generation,
        TotalCells = gridWidth,
        AliveCells = state.Count(c => c)
    };

    if (stats.AliveCells > 0)
    {
        var aliveLifetimes = state.Select((alive, i) => alive ? _cellLifetimes[i] : 0)
                                  .Where(lt => lt > 0);
        stats.AverageLifetime = aliveLifetimes.Average();
        stats.LongestLifetime = aliveLifetimes.Max();
    }

    return stats;
}
```

---

## 📋 Rendering Order (Updated)

1. **Fill background** (theme color)
2. **Initialize tracking** (_cellLifetimes, _previousState)
3. **For each generation:**
   - Draw CA cells (spectral colors)
   - Apply rule to get next generation
   - **Track cell lifetimes** (NEW)
   - **Compute statistics** (NEW, last generation only)
4. **Draw grid** (if showGrid == true)
5. **Return pixels + statistics** (NEW)

---

## 📈 Status Bar Display (XAML)

```xaml
<!-- CA Statistics Display (Grid.Column="2") -->
<TextBlock Grid.Column="2"
           VerticalAlignment="Center"
           Margin="0,0,16,0"
           FontFamily="Consolas"
           FontSize="12"
           Foreground="{ThemeResource SystemAccentColor}"
           Text="{x:Bind ViewModel.CAStatistics.FormattedDisplay, Mode=OneWay}"
           Visibility="{x:Bind ViewModel.CAStatisticsVisibility, Mode=OneWay}"
           ToolTipService.ToolTip="CA Statistics: Generation counter, alive cells, proportionality, and cell lifetimes" />
```

---

## ✅ Testing Checklist (Updated)

### Theme
- [ ] Light theme → white background
- [ ] Dark theme → black background
- [ ] Switch themes → updates correctly

### Grid
- [ ] showGrid = true → grid visible
- [ ] showGrid = false → grid hidden
- [ ] Grid is light gray on both themes
- [ ] Grid spacing = cellSize pixels

### Dimensions
- [ ] gridWidth slider works (10-200)
- [ ] gridHeight slider works (10-200)
- [ ] Constraints respect canvas size
- [ ] cellSize change updates grid

### Statistics (NEW)
- [ ] Generation counter increments (0, 1, 2, ...)
- [ ] Alive cell count is accurate
- [ ] Proportionality calculates correctly
- [ ] Average lifetime tracks properly
- [ ] Longest lifetime identifies max
- [ ] Statistics reset on fractal switch
- [ ] Statistics disappear for non-CA fractals

---

## 🎮 Animation Control (NEW)

### Keyboard Shortcuts

| Key | Mode | Action |
|-----|------|--------|
| **Ctrl+R** | Fractal | Render fractal |
| **Ctrl+R** or **Space** | CA | Start/Stop simulation |
| **P** | CA | Pause/Resume |
| **Esc** | CA | Stop immediately |

### Command Implementation
```csharp
// MainViewModel.Commands.cs
[RelayCommand(CanExecute = nameof(CanStartCASimulation))]
private async Task StartCASimulation()
{
    IsCASimulationRunning = true;
    _caSimulationCancellation = new CancellationTokenSource();

    try
    {
        await RunCASimulationAsync(_caSimulationCancellation.Token);
    }
    catch (OperationCanceledException) { }
    finally
    {
        IsCASimulationRunning = false;
    }
}

[RelayCommand]
private void StopCASimulation()
{
    _caSimulationCancellation?.Cancel();
}
```

### Animation Loop Pattern
```csharp
private async Task RunCASimulationAsync(CancellationToken ct)
{
    int frameDelayMs = 1000 / frameRate; // Default 30 fps

    for (int gen = 0; gen < maxGenerations && !ct.IsCancellationRequested; gen++)
    {
        // Render one generation
        var result = await RenderGenerationAsync(gen, ct);

        // Update UI
        await UpdateCanvasAsync(result);
        CAStatistics = result.Statistics;

        // Frame rate control
        await Task.Delay(frameDelayMs, ct);
    }
}
```

### Dynamic Toolbar Button (XAML)
```xaml
<!-- Fractal Mode -->
<AppBarButton 
    Icon="Play" 
    Label="Render"
    Visibility="{x:Bind ViewModel.IsFractalMode, Mode=OneWay, Converter={StaticResource BoolToVisibilityConverter}}"
    Command="{x:Bind ViewModel.RenderCommand}" />

<!-- CA Mode -->
<AppBarButton 
    Icon="Play" 
    Label="Start"
    Visibility="{x:Bind ViewModel.IsCAMode, Mode=OneWay, Converter={StaticResource BoolToVisibilityConverter}}"
    Command="{x:Bind ViewModel.StartCASimulationCommand}" />
<AppBarButton 
    Icon="Stop" 
    Label="Stop"
    Visibility="{x:Bind ViewModel.IsCAMode, Mode=OneWay, Converter={StaticResource BoolToVisibilityConverter}}"
    Command="{x:Bind ViewModel.StopCASimulationCommand}" />
```

### Mode Switching
```csharp
// MainPage.cs
private void ShowFractalBrowser()
{
    ViewModel.StopCASimulationCommand.Execute(null); // Stop CA if running
    ViewModel.IsFractalMode = true;
    ViewModel.IsCAMode = false;
}

private void ShowCABrowser()
{
    ViewModel.IsFractalMode = false;
    ViewModel.IsCAMode = true;
}
```

### Testing Checklist
- [ ] Ctrl+R starts/stops CA simulation
- [ ] Space key toggles CA simulation
- [ ] P key pauses/resumes
- [ ] Esc key stops immediately
- [ ] Toolbar buttons swap on mode change
- [ ] Statistics update during animation
- [ ] Switching fractals stops CA cleanly

---

## 🚀 Implementation Steps

### Step 1: Add to RenderService
```csharp
public byte[] RenderElementaryCA(
    /* params */,
    int gridWidth,
    int gridHeight,
    bool showGrid,
    ElementTheme theme)
{
    byte[] pixels = new byte[width * height * 4];

    // 1. Fill background
    bool isDark = (theme == ElementTheme.Dark);
    FillBackground(pixels, width, height, isDark);

    // 2. Draw CA cells
    // ... existing CA simulation code ...

    // 3. Draw grid overlay
    if (showGrid)
        DrawGrid(pixels, width, height, cellSize, gridWidth, gridHeight);

    return pixels;
}
```

### Step 2: Add Parameters (FractalParameterService.cs)
```csharp
// In CreateCATemplate():
paramSet.Parameters["gridWidth"] = new ParameterDefinition
{
    Name = "Grid Width",
    Type = ParameterType.Integer,
    DefaultValue = 100,
    MinValue = 10,
    MaxValue = 200
};
// Repeat for gridHeight and showGrid
```

### Step 3: Pass Theme (FractalRenderService.cs)
```csharp
// Extract from MainPage
var theme = this.ActualTheme;

// Pass to CA renderer
await _renderService.RenderCellularAutomatonAsync(
    /* params */,
    currentTheme: theme
);
```

---

## 📖 Full Documentation

- **Master Checklist**: `CELLULAR_AUTOMATA_IMPLEMENTATION_CHECKLIST.md`
- **Theme & Grid Guide**: `CA_THEME_AND_GRID_IMPLEMENTATION.md`
- **Visual Reference**: `CA_VISUAL_DESIGN_REFERENCE.md`
- **Statistics Guide**: `CA_STATISTICS_IMPLEMENTATION.md`
- **Animation Control Guide**: `CA_ANIMATION_CONTROL_IMPLEMENTATION.md` (NEW)
- **Update Summaries**: 
  - `CA_UPDATE_SUMMARY_2025-01-16.md` (Theme & Grid)
  - `CA_STATISTICS_UPDATE_SUMMARY.md` (Statistics)
  - `CA_ANIMATION_CONTROL_UPDATE_SUMMARY.md` (Animation Control)

---

**Quick Access**: Pin this card for fast reference during implementation!  
**Last Updated**: 2025-01-16 (Added animation control)
