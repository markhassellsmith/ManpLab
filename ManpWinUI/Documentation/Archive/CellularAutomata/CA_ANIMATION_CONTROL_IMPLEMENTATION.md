# CA Animation Control Implementation Guide

**Feature**: Dynamic Toolbar Button + Start/Stop/Pause Animation Controls  
**Phase**: 9-10 of CA Implementation Checklist  
**Last Updated**: 2025-01-16

---

## Overview

This guide details the implementation of animation controls for Cellular Automata, including:
- **Dynamic toolbar button** that swaps between "Render" (fractals) and "Start/Stop" (CA)
- **Animation loop** that displays CA generations incrementally
- **Keyboard shortcuts** for Start/Stop/Pause
- **Simulation state management** to coordinate UI and rendering

---

## Architecture

### Key Components

1. **MainPage.xaml** - Toolbar with two AppBarButtons (visibility toggled by mode)
2. **MainViewModel.Commands.cs** - Commands for Start/Stop/Pause CA simulation
3. **MainPage.KeyboardHandling.cs** - Keyboard shortcuts for CA controls
4. **CellularAutomatonRenderService.cs** - Generation-by-generation rendering
5. **MainPage.cs** - Mode switching logic and button visibility control

---

## Part 1: Dynamic Toolbar Button (Phase 9)

### Current State

```xaml
<!-- MainPage.xaml - Current Render button -->
<AppBarButton
    Icon="Play"
    Label="Render"
    IsEnabled="True"
    Command="{x:Bind ViewModel.RenderCommand}">
    <ToolTipService.ToolTip>
        <ToolTip Content="Render fractal (Ctrl+R)" />
    </ToolTipService.ToolTip>
</AppBarButton>
```

### Target State

```xaml
<!-- MainPage.xaml - Dynamic buttons with visibility binding -->
<CommandBar Grid.Row="0"
            DefaultLabelPosition="Right"
            Background="{ThemeResource AppBarBackground}">

    <!-- FRACTAL MODE: Render Button -->
    <AppBarButton
        x:Name="RenderButton"
        Icon="Play"
        Label="Render"
        IsEnabled="True"
        Visibility="{x:Bind ViewModel.IsFractalMode, Mode=OneWay, Converter={StaticResource BoolToVisibilityConverter}}"
        Command="{x:Bind ViewModel.RenderCommand}">
        <ToolTipService.ToolTip>
            <ToolTip Content="Render fractal (Ctrl+R)" />
        </ToolTipService.ToolTip>
    </AppBarButton>

    <!-- CA MODE: Start Button -->
    <AppBarButton
        x:Name="StartCAButton"
        Icon="Play"
        Label="Start"
        IsEnabled="{x:Bind ViewModel.CanStartCASimulation, Mode=OneWay}"
        Visibility="{x:Bind ViewModel.IsCAMode, Mode=OneWay, Converter={StaticResource BoolToVisibilityConverter}}"
        Command="{x:Bind ViewModel.StartCASimulationCommand}">
        <ToolTipService.ToolTip>
            <ToolTip Content="Start CA simulation (Ctrl+R or Space)" />
        </ToolTipService.ToolTip>
    </AppBarButton>

    <!-- CA MODE: Stop Button -->
    <AppBarButton
        x:Name="StopCAButton"
        Icon="Stop"
        Label="Stop"
        IsEnabled="{x:Bind ViewModel.IsCASimulationRunning, Mode=OneWay}"
        Visibility="{x:Bind ViewModel.IsCAMode, Mode=OneWay, Converter={StaticResource BoolToVisibilityConverter}}"
        Command="{x:Bind ViewModel.StopCASimulationCommand}">
        <ToolTipService.ToolTip>
            <ToolTip Content="Stop CA simulation (Esc)" />
        </ToolTipService.ToolTip>
    </AppBarButton>

    <!-- CA MODE: Pause/Resume Button (Optional) -->
    <AppBarButton
        x:Name="PauseCAButton"
        Icon="Pause"
        Label="Pause"
        IsEnabled="{x:Bind ViewModel.IsCASimulationRunning, Mode=OneWay}"
        Visibility="{x:Bind ViewModel.IsCAMode, Mode=OneWay, Converter={StaticResource BoolToVisibilityConverter}}"
        Command="{x:Bind ViewModel.PauseCASimulationCommand}">
        <ToolTipService.ToolTip>
            <ToolTip Content="Pause/Resume simulation (P)" />
        </ToolTipService.ToolTip>
    </AppBarButton>

    <AppBarSeparator />

    <!-- Rest of toolbar unchanged -->
    <AppBarButton Icon="Refresh" Label="Reset View" ... />
    ...
</CommandBar>
```

### ViewModel Properties (MainViewModel.cs)

```csharp
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isFractalMode = true;

    [ObservableProperty]
    private bool _isCAMode = false;

    [ObservableProperty]
    private bool _isCASimulationRunning = false;

    [ObservableProperty]
    private CASimulationState _caSimulationState = CASimulationState.Stopped;

    public bool CanStartCASimulation => !IsCASimulationRunning;

    private CancellationTokenSource? _caSimulationCancellation;
}

public enum CASimulationState
{
    Stopped,
    Running,
    Paused
}
```

### Mode Switching Logic (MainPage.cs)

```csharp
// MainPage.cs

private void ShowFractalBrowser()
{
    // Stop any running CA simulation
    ViewModel.StopCASimulationCommand.Execute(null);

    // Show/hide panels
    FractalBrowserPanel.Visibility = Visibility.Visible;
    CABrowserPanel.Visibility = Visibility.Collapsed;

    // Update mode flags
    ViewModel.IsFractalMode = true;
    ViewModel.IsCAMode = false;

    // Buttons automatically toggle via visibility bindings
}

private void ShowCABrowser()
{
    // Show/hide panels
    FractalBrowserPanel.Visibility = Visibility.Collapsed;
    CABrowserPanel.Visibility = Visibility.Visible;

    // Update mode flags
    ViewModel.IsFractalMode = false;
    ViewModel.IsCAMode = true;

    // Buttons automatically toggle via visibility bindings
}
```

---

## Part 2: Animation Control Commands (Phase 10)

### Command Implementation (MainViewModel.Commands.cs)

```csharp
// MainViewModel.Commands.cs

[RelayCommand(CanExecute = nameof(CanStartCASimulation))]
private async Task StartCASimulation()
{
    if (CurrentFractal?.Category != FractalCategory.CellularAutomaton)
        return;

    IsCASimulationRunning = true;
    CaSimulationState = CASimulationState.Running;

    // Cancel any existing simulation
    _caSimulationCancellation?.Cancel();
    _caSimulationCancellation = new CancellationTokenSource();

    try
    {
        await RunCASimulationAsync(_caSimulationCancellation.Token);
    }
    catch (OperationCanceledException)
    {
        // Expected when stopped by user
    }
    finally
    {
        IsCASimulationRunning = false;
        CaSimulationState = CASimulationState.Stopped;
    }
}

[RelayCommand]
private void StopCASimulation()
{
    _caSimulationCancellation?.Cancel();
    _caSimulationCancellation = null;
}

[RelayCommand]
private void PauseCASimulation()
{
    if (CaSimulationState == CASimulationState.Running)
    {
        CaSimulationState = CASimulationState.Paused;
        PauseCAButton.Icon = new SymbolIcon(Symbol.Play);
        PauseCAButton.Label = "Resume";
    }
    else if (CaSimulationState == CASimulationState.Paused)
    {
        CaSimulationState = CASimulationState.Running;
        PauseCAButton.Icon = new SymbolIcon(Symbol.Pause);
        PauseCAButton.Label = "Pause";
    }
}
```

### Animation Loop

```csharp
// MainViewModel.Commands.cs

private async Task RunCASimulationAsync(CancellationToken cancellation)
{
    if (CurrentParameters == null)
        return;

    // Extract parameters
    int maxGenerations = CurrentParameters.GetIntValue("generations", 100);
    int frameDelayMs = 1000 / CurrentParameters.GetIntValue("frameRate", 30); // Default 30 fps

    // Get initial state from render service
    var caRenderer = _serviceProvider.GetRequiredService<CellularAutomatonRenderService>();
    var initialState = caRenderer.InitializeState(CurrentParameters);

    // Animate generation by generation
    for (int generation = 0; generation < maxGenerations && !cancellation.IsCancellationRequested; generation++)
    {
        // Wait if paused
        while (CaSimulationState == CASimulationState.Paused && !cancellation.IsCancellationRequested)
        {
            await Task.Delay(100, cancellation);
        }

        if (cancellation.IsCancellationRequested)
            break;

        // Render current generation
        var renderResult = await caRenderer.RenderGenerationAsync(
            generation,
            CurrentParameters,
            (int)CurrentWidth,
            (int)CurrentHeight,
            cancellation
        );

        if (renderResult.Success)
        {
            // Update canvas
            await UpdateCanvasAsync(renderResult);

            // Update statistics
            CAStatistics = renderResult.Statistics;
        }

        // Frame rate control
        await Task.Delay(frameDelayMs, cancellation);
    }
}
```

---

## Part 3: Keyboard Shortcuts (Phase 10)

### Keyboard Handler (MainPage.KeyboardHandling.cs)

```csharp
// MainPage.KeyboardHandling.cs

private void OnKeyDown(object sender, KeyRoutedEventArgs e)
{
    var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down);
    var shift = Window.Current.CoreWindow.GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down);

    // Ctrl+R: Render (fractal) or Start (CA)
    if (e.Key == VirtualKey.R && ctrl && !shift)
    {
        if (ViewModel.IsFractalMode)
        {
            ViewModel.RenderCommand.Execute(null);
        }
        else if (ViewModel.IsCAMode)
        {
            if (ViewModel.IsCASimulationRunning)
            {
                ViewModel.StopCASimulationCommand.Execute(null);
            }
            else
            {
                ViewModel.StartCASimulationCommand.Execute(null);
            }
        }
        e.Handled = true;
    }

    // Space: Start/Stop CA (CA mode only)
    if (e.Key == VirtualKey.Space && ViewModel.IsCAMode)
    {
        if (ViewModel.IsCASimulationRunning)
        {
            ViewModel.StopCASimulationCommand.Execute(null);
        }
        else
        {
            ViewModel.StartCASimulationCommand.Execute(null);
        }
        e.Handled = true;
    }

    // P: Pause/Resume CA
    if (e.Key == VirtualKey.P && ViewModel.IsCAMode && ViewModel.IsCASimulationRunning)
    {
        ViewModel.PauseCASimulationCommand.Execute(null);
        e.Handled = true;
    }

    // Esc: Stop CA
    if (e.Key == VirtualKey.Escape && ViewModel.IsCAMode && ViewModel.IsCASimulationRunning)
    {
        ViewModel.StopCASimulationCommand.Execute(null);
        e.Handled = true;
    }
}
```

---

## Part 4: Render Service Integration (Phase 3)

### Generation-by-Generation Rendering

Update `CellularAutomatonRenderService.cs` to support incremental rendering:

```csharp
// CellularAutomatonRenderService.cs

public CAInitialState InitializeState(FractalParameterSet parameters)
{
    // Extract parameters
    string ruleName = parameters.GetStringValue("rule");
    string patternName = parameters.GetStringValue("initialPattern");
    int gridWidth = parameters.GetIntValue("gridWidth");

    // Create initial pattern
    bool[] initialState = CAInitializer.CreatePattern(patternName, gridWidth, parameters);

    // Build lookup table
    byte[] lookupTable = CARule.GetLookupTable(ruleName);

    return new CAInitialState
    {
        CurrentState = initialState,
        LookupTable = lookupTable,
        GridWidth = gridWidth,
        CellLifetimes = new int[gridWidth],
        Statistics = new CAStatistics()
    };
}

public async Task<RenderResult> RenderGenerationAsync(
    int generation,
    FractalParameterSet parameters,
    int canvasWidth,
    int canvasHeight,
    CancellationToken cancellation)
{
    // Apply CA rule to advance one generation
    var newState = ApplyRule(
        _currentState.CurrentState,
        _currentState.LookupTable,
        parameters.GetBoolValue("wrapEdges")
    );

    // Track cell lifetimes
    TrackCellLifetimes(_currentState.CellLifetimes, _currentState.CurrentState, newState);

    // Update state
    _currentState.CurrentState = newState;

    // Render to pixel buffer
    byte[] pixels = RenderGeneration(
        generation,
        newState,
        parameters,
        canvasWidth,
        canvasHeight
    );

    // Compute statistics
    var stats = ComputeStatistics(
        generation,
        newState,
        _currentState.CellLifetimes,
        parameters.GetIntValue("gridWidth"),
        parameters.GetIntValue("gridHeight")
    );

    return new RenderResult
    {
        Success = true,
        PixelData = pixels,
        Statistics = stats,
        Width = canvasWidth,
        Height = canvasHeight
    };
}
```

---

## Implementation Checklist

### Phase 9: Dynamic Toolbar Button

- [ ] Add `IsFractalMode` and `IsCAMode` properties to MainViewModel
- [ ] Add CA Start/Stop/Pause buttons to MainPage.xaml with visibility bindings
- [ ] Update `ShowFractalBrowser()` to set `IsFractalMode = true, IsCAMode = false`
- [ ] Update `ShowCABrowser()` to set `IsFractalMode = false, IsCAMode = true`
- [ ] Test mode toggle → Correct buttons appear

### Phase 10: Animation Control

- [ ] Add `CASimulationState` enum and property to MainViewModel
- [ ] Add `IsCASimulationRunning` property to MainViewModel
- [ ] Implement `StartCASimulationCommand` in MainViewModel.Commands.cs
- [ ] Implement `StopCASimulationCommand` in MainViewModel.Commands.cs
- [ ] Implement `PauseCASimulationCommand` in MainViewModel.Commands.cs
- [ ] Implement `RunCASimulationAsync()` with generation loop
- [ ] Add `CancellationTokenSource` for stopping simulation
- [ ] Add frame rate parameter and control (10-60 fps)
- [ ] Update `CellularAutomatonRenderService` with `InitializeState()` and `RenderGenerationAsync()`
- [ ] Add keyboard shortcuts in MainPage.KeyboardHandling.cs:
  - Ctrl+R or Space → Start/Stop toggle
  - P → Pause/Resume
  - Esc → Stop
- [ ] Ensure simulation stops when switching fractals or closing app
- [ ] Test animation → Smooth generation-by-generation display
- [ ] Test keyboard shortcuts → Commands execute correctly

---

## Testing Scenarios

### Dynamic Button Tests

| Test | Expected Result |
|------|----------------|
| Toggle to Fractal mode | "Render" button visible, CA buttons hidden |
| Toggle to CA mode | "Start", "Stop", "Pause" buttons visible, "Render" hidden |
| Switch modes during CA simulation | CA stops automatically |

### Animation Control Tests

| Test | Expected Result |
|------|----------------|
| Click Start | CA begins animating generation by generation |
| Click Stop | Animation stops, final state persists |
| Click Pause | Animation freezes at current generation |
| Click Resume | Animation continues from paused generation |
| Press Ctrl+R in CA mode | Starts or stops simulation |
| Press Space in CA mode | Starts or stops simulation |
| Press P during simulation | Pauses/resumes |
| Press Esc during simulation | Stops immediately |
| Switch to fractal mid-simulation | CA stops cleanly |

### Statistics Tests

| Test | Expected Result |
|------|----------------|
| Start simulation | Generation counter increments (0, 1, 2, ...) |
| Mid-simulation | Alive cells count updates in real time |
| End of simulation | Final statistics remain displayed |

---

## Performance Considerations

1. **Frame Rate Control**: Default 30 fps, adjustable 10-60 fps via parameter
2. **Canvas Update**: Use `WritePixels()` for efficient incremental updates
3. **Cancellation**: Always check `cancellation.IsCancellationRequested` in loops
4. **UI Thread**: Statistics updates should use `DispatcherQueue.TryEnqueue()`
5. **Memory**: Reuse pixel buffers where possible, avoid allocations in hot loop

---

## Success Criteria

✅ Dynamic toolbar button switches correctly between fractal and CA modes  
✅ CA Start button begins generation-by-generation animation  
✅ CA Stop button halts animation cleanly  
✅ CA Pause button freezes/resumes animation  
✅ Keyboard shortcuts (Ctrl+R, Space, P, Esc) execute commands correctly  
✅ Statistics update in real time during animation  
✅ Simulation stops when switching fractals or closing app  
✅ No crashes, memory leaks, or visual artifacts  
✅ Animation frame rate is smooth and adjustable  

---

**Next Steps**: See Phase 11 (DI Registration) and Phase 12 (Statistics Display) in main checklist.
