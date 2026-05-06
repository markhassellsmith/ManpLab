# Animation Integration with MainViewModel

## Overview
The animation system is now integrated with MainViewModel, allowing you to animate your current fractal view with all your custom settings, colors, and parameters.

## What's Integrated

### 1. **Current Fractal Settings**
All animations now use your current fractal configuration:
- **Fractal Type**: Mandelbrot, Julia, Phoenix, Lambda, etc.
- **Custom Parameters**: All fractal-specific parameters from the parameter system
- **Julia Mode**: If you're viewing a Julia set, animations will use your Julia constants
- **Iteration Settings**: Your current max iterations setting

### 2. **Color & Render Settings**
Your color preferences are preserved:
- **Color Palette**: Current selected palette
- **Color Cycling**: Speed and offset settings
- **Smooth Coloring**: Enabled/disabled state

### 3. **View Parameters**
Starting positions can be synced from your current view:
- **Center Coordinates**: Where you're currently looking
- **Zoom Level**: Your current magnification
- **All Extended Parameters**: Power, exponents, and fractal-specific settings

## How to Use

### Quick Start
1. **Navigate to your desired fractal view** in the main canvas
   - Set up your fractal type, zoom level, colors, etc.
   - Make sure you're happy with how it looks

2. **Open the Animation panel** (right-side properties panel)
   - Click the "Animation" tab

3. **Click "↻ Sync Start Position from Current View"**
   - This captures your current view settings as the animation starting point
   - For Zoom animations: captures current zoom and center
   - For Pan animations: captures current center position

4. **Configure animation end point**
   - **Zoom**: Adjust "End Zoom" or use speed presets
   - **Pan**: Set end center coordinates
   - **Parameter**: Specify which parameter to animate and its range

5. **Click "Render Animation"**
   - Animation will use all your current settings
   - Output will be saved to your specified path

### Animation Types

#### **Zoom Animation**
- Starts at your current view
- Zooms in/out to the specified end zoom level
- Maintains all other settings (colors, parameters, etc.)
- **Tip**: Use "Slow" or "Very Slow" preset for smooth animations

#### **Pan Animation**
- Moves camera from current position to end position
- Keeps zoom constant
- Great for exploring different areas of the fractal

#### **Parameter Animation**
- Sweeps a parameter (like power or exponent) between two values
- Creates morph effects
- All other settings remain constant

## Technical Details

### Dependency Injection
```csharp
// AnimationViewModel now receives MainViewModel as optional dependency
public AnimationViewModel(
    AnimationService animationService,
    ILogger<AnimationViewModel> logger,
    MainViewModel? mainViewModel = null)
```

### Parameter Capture
The system automatically captures:
- Standard properties (center, zoom, iterations)
- Julia mode settings
- Color settings
- Extended parameters from the flexible parameter system

```csharp
private RenderParameters BuildRenderParametersFromMainView()
{
    // Gets all settings from MainViewModel
    // Falls back to defaults if MainViewModel not available
}
```

### Frame-by-Frame Rendering
Each animation frame is rendered with:
- Interpolated view parameters (zoom, pan, etc.)
- **Same fractal type** throughout
- **Same color settings** (unless using ColorCycle animation)
- **Same custom parameters** (unless using Parameter animation)

## Examples

### Example 1: Deep Zoom into Mini-Mandelbrot
1. Navigate to an interesting mini-brot location
2. Set zoom to current interesting zoom level (e.g., 1000x)
3. Sync from current view
4. Select "Extreme" speed preset → calculates 1,000,000x zoom
5. Render → Creates dramatic zoom animation

### Example 2: Julia Set Tour
1. Switch to Julia mode with interesting c value
2. Find a nice starting view
3. Sync from current view
4. Choose Pan animation
5. Set end coordinates to another interesting area
6. Render → Pan across the Julia set

### Example 3: Power Morph
1. Set up a fractal with "power" parameter (e.g., generalized Mandelbrot)
2. Sync current view
3. Choose Parameter animation
4. Parameter name: "power"
5. Start: 2.0 (classic)
6. End: 5.0 (dramatic)
7. Render → Watch the fractal morph

## UI Features

### Current View Info Panel
```
┌─────────────────────────────────────┐
│ Current Fractal View                │
│ Your animation will use the current │
│ fractal type, colors, and all       │
│ custom parameters from main view.   │
│                                      │
│ [↻ Sync Start Position from Current │
│    View]                            │
└─────────────────────────────────────┘
```

### Zoom Settings with Live Feedback
- **Zoom Speed Preset**: Quick selection (Very Slow → Extreme)
- **Start/End Zoom**: Manual fine-tuning
- **Total Magnification**: Live display of zoom ratio
  - Shows as "1.0K×", "10.0M×", etc.
  - Updates in real-time

### Interdependent Controls
- Changing start zoom → recalculates end zoom (when preset active)
- Changing end zoom → detects matching preset or switches to Custom
- Visual feedback shows current magnification

## Benefits

### 1. **Consistency**
- No need to manually copy settings
- Guaranteed that animation matches your current view
- All custom parameters automatically included

### 2. **Flexibility**
- Can still override any setting after sync
- Mix current settings with custom animation endpoints
- Full control over animation parameters

### 3. **Discoverability**
- Clear indication that current settings will be used
- One-click sync button for convenience
- Visual feedback confirms what's captured

### 4. **Power User Features**
- Access to full parameter system
- Extended parameters automatically captured
- Julia mode fully supported

## Troubleshooting

### "Animation doesn't look like my view"
- **Solution**: Click "Sync Start Position from Current View" again
- Settings are captured at sync time, not render time

### "Colors are different"
- **Check**: Color palette, cycle speed, and smooth coloring settings
- These are captured from MainViewModel automatically

### "Custom parameters not working"
- **Verify**: Parameter system is active (`UseParameterSystem = true`)
- Check that your fractal type has registered parameters
- Extended parameters are automatically captured if present

### "Animation starts from wrong position"
- **For Zoom**: Sync sets start zoom and center
- **For Pan**: Sync sets start center (end center must be set manually)
- **For Parameter**: Sync captures parameter value as reference only

## Future Enhancements (Potential)

- **Bookmark-to-Bookmark Animation**: Animate between saved bookmarks
- **History Replay**: Animate through navigation history
- **Multi-Stage Animations**: Chain multiple animation segments
- **Color Morph**: Animate between different palettes
- **Resolution Presets**: Quick selection of output resolutions (720p, 1080p, 4K)
- **Real-Time Preview**: Show animation preview before rendering
