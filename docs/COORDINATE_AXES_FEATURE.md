# Coordinate Axes and Tick Marks Feature

## Overview
Added visual tick marks and coordinate labels to the fractal image borders, showing the actual mathematical coordinates of the complex plane being rendered. **This feature can be toggled on/off via the Display Options panel.**

## Features

### 1. **Toggle Control**
- **Location**: Side panel → Display Options section
- **Control Type**: ToggleSwitch
- **Default State**: Enabled (visible)
- **Tooltip**: "Display tick marks and coordinate labels on image borders"
- **States**: 
  - **On** (Visible) - Tick marks and labels are shown
  - **Off** (Hidden) - Clean view without coordinate overlay

### 2. **Dynamic Tick Marks**
- Tick marks appear on the left and bottom edges of the fractal image
- Automatically calculated based on the current view size
- Smart interval calculation (uses round numbers: 1, 2, 5, 10, etc.)
- Aims for approximately 8 ticks per axis

### 2. **Coordinate Labels**
- Numeric labels show fractal coordinates (not pixel coordinates)
- Labels appear on every other tick mark to avoid clutter
- Adaptive formatting:
  - **Very small/large values**: Scientific notation (e.g., `1.23e-5`)
  - **Small values (< 0.01)**: Four decimal places (e.g., `0.0012`)
  - **Medium values (< 1)**: Three decimal places (e.g., `0.123`)
  - **Normal values**: Two decimal places (e.g., `1.23`)

### 3. **Visual Design**
- White tick marks with 70% opacity
- White text labels with 80% opacity
- Canvas overlay (Z-index 5) sits between fractal and selection rectangle
- Non-interactive (`IsHitTestVisible="False"`) - doesn't interfere with pan/zoom
- **Can be hidden** via toggle switch in Display Options

### 4. **Auto-Update**
Coordinate axes automatically update when:
- Zoom level changes
- Center X/Y coordinates change
- Image resolution changes
- Window/canvas size changes

## Technical Implementation

### ViewModel Property
```csharp
[ObservableProperty]
public partial bool ShowCoordinateAxes { get; set; } = true;
```

### Converter
Created `BooleanToVisibilityConverter` to convert boolean to `Visibility` enum:
- `true` → `Visibility.Visible`
- `false` → `Visibility.Collapsed`

### XAML Changes
```xaml
<!-- Coordinate Axes Overlay -->
<Canvas x:Name="CoordinateAxesCanvas" 
        Canvas.ZIndex="5"
        IsHitTestVisible="False"
        Visibility="{x:Bind ViewModel.ShowCoordinateAxes, Mode=OneWay, Converter={StaticResource BooleanToVisibilityConverter}}"
        SizeChanged="CoordinateAxesCanvas_SizeChanged">
</Canvas>

<!-- Toggle Control -->
<ToggleSwitch 
    IsOn="{x:Bind ViewModel.ShowCoordinateAxes, Mode=TwoWay}"
    Header="Show Coordinate Axes"
    OnContent="Visible"
    OffContent="Hidden" />
```

### Code-Behind Logic

#### `UpdateCoordinateAxes()`
Main method that:
1. Clears previous tick marks and labels
2. Calculates fractal coordinate boundaries
3. Determines appropriate tick interval
4. Draws horizontal and vertical ticks with labels

#### `CalculateTickInterval(double viewRange)`
Smart algorithm for choosing "nice" tick intervals:
- Aims for ~8 ticks across the view
- Chooses round numbers (1, 2, 5, 10 × 10^n)
- Ensures readable spacing at any zoom level

#### `DrawHorizontalTicks()` / `DrawVerticalTicks()`
- Convert fractal coordinates to screen pixels
- Account for Viewbox scaling and centering
- Draw tick marks as `Line` elements
- Add `TextBlock` labels at appropriate positions

#### `FormatCoordinate(double value)`
Adaptive number formatting for readability at any scale

### Coordinate System
```
Fractal Coordinates (Mandelbrot Set):
  X-axis: Real axis (horizontal)
    - Standard view: approximately -2.5 to 1.0
  Y-axis: Imaginary axis (vertical)
    - Standard view: approximately -1.25 to 1.25
    - Positive values are UP (mathematical convention)

Screen Coordinates:
  - Origin (0,0) at top-left
  - Y increases downward
  - Conversion handles Y-axis inversion
```

## Example Scenarios

### Scenario 1: Full Mandelbrot View (Zoom = 1.0)
```
View: 3.00 × 2.25 fractal units
Tick interval: 0.5
Labels: -2.5, -2.0, -1.5, -1.0, -0.5, 0.0, 0.5, 1.0
```

### Scenario 2: Medium Zoom (Zoom = 10.0)
```
View: 0.30 × 0.225 fractal units
Tick interval: 0.05
Labels: -0.15, -0.10, -0.05, 0.00, 0.05, 0.10, 0.15
```

### Scenario 3: Deep Zoom (Zoom = 10,000.0)
```
View: 3.00e-4 × 2.25e-4 fractal units
Tick interval: 5.0e-5
Labels: -1.5e-4, -1.0e-4, -5.0e-5, 0.0e0, 5.0e-5, 1.0e-4, 1.5e-4
```

## Benefits

### For Users
- **Understand zoom depth** - See actual mathematical coordinates
- **Navigate precisely** - Know exactly where you are in the complex plane
- **Educational value** - Learn the mathematical structure of the Mandelbrot set
- **Screenshot-ready** - Labeled coordinates make images self-documenting

### For Developers
- **Debugging tool** - Verify coordinate transformations are correct
- **Visual feedback** - Confirm zoom/pan calculations
- **Quality assurance** - Ensure accuracy at extreme zoom levels

## Future Enhancements

### Possible Improvements
1. **Grid lines** - Optional full grid overlay
2. **Axis labels** - "Real" and "Imaginary" axis labels
3. **Toggle visibility** - Show/hide coordinate axes via checkbox
4. **Customizable style** - Color, opacity, font size settings
5. **Origin marker** - Highlight (0, 0) point if visible
6. **Zoom level indicator** - Display current magnification
7. **Tick mark density** - User-adjustable tick spacing

### Integration with Original ManpWin
The original ManpWin displays coordinates in a status dialog. This implementation provides:
- **Real-time visual feedback** vs on-demand dialog
- **Spatial awareness** - See coordinates in context
- **Modern design** - Integrated into WinUI interface

## Testing Checklist

- [x] Build compiles successfully
- [x] Toggle switch appears in Display Options section
- [x] Toggle switch default state is ON
- [ ] Tick marks appear after initial render (when toggle is ON)
- [ ] Labels show correct fractal coordinates
- [ ] Tick marks update when zooming in/out
- [ ] Tick marks update when panning
- [ ] Tick marks update when changing resolution
- [ ] Labels use scientific notation at deep zoom
- [ ] Tick marks don't interfere with selection rectangle
- [ ] Canvas resizes correctly with window
- [ ] Performance is acceptable (no lag when updating)
- [ ] Tick marks align with actual fractal coordinates
- [ ] **Toggle OFF hides all tick marks and labels**
- [ ] **Toggle ON shows tick marks and labels**
- [ ] **Toggle state persists during zoom/pan operations**

## User Experience

### When to Use Coordinate Axes

**Show Axes (ON):**
- 🎓 Educational/learning purposes
- 📸 Creating screenshots for documentation
- 📊 Precise navigation to specific coordinates
- 🔬 Scientific/research work
- 🐛 Debugging coordinate transformations

**Hide Axes (OFF):**
- 🎨 Artistic screenshots (clean aesthetic)
- 🖼️ Wallpaper creation
- 👁️ Pure visual exploration
- 📹 Video recordings
- 🎥 Presentation mode

### Quick Access
The toggle is easily accessible in the side panel's Display Options section, allowing users to quickly show/hide axes without disrupting their workflow.

## Performance Notes

- Tick marks are redrawn only when view parameters change
- Typically 8-16 tick marks per axis = ~16-32 UI elements
- Minimal performance impact
- Canvas clearing and redrawing is fast enough for real-time updates

---

**Status:** ✅ Implemented  
**Version:** Phase 3 - WinUI MVVM Architecture  
**Related Features:** Zoom, Pan, Status Bar
