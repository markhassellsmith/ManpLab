# Cellular Automata: Theme-Aware Backgrounds and Grid Implementation

**Date**: 2025-01-16  
**Feature**: CA Theme Integration and Grid Overlay  
**Related**: `CELLULAR_AUTOMATA_IMPLEMENTATION_CHECKLIST.md`

---

## Overview

This document provides detailed implementation guidance for adding theme-aware backgrounds and grid overlays to the Cellular Automata rendering system.

---

## 1. Theme Detection

### Option A: Pass Theme from MainPage (Recommended)

**MainPage.cs** already has access to the current theme via WinUI 3's theme system:

```csharp
// In MainPage.cs - when triggering CA render
var currentTheme = this.ActualTheme; // ElementTheme.Light or ElementTheme.Dark

// Pass to render service
var result = await _renderService.RenderCellularAutomatonAsync(
    /* parameters */,
    currentTheme: currentTheme
);
```

**FractalRenderService.cs** - Add theme parameter:

```csharp
public async Task<FractalRenderResult> RenderCellularAutomatonAsync(
    /* existing parameters */,
    ElementTheme currentTheme = ElementTheme.Default)
{
    // Pass theme to CA renderer
    var caRenderer = new CellularAutomatonRenderService();
    return await caRenderer.RenderElementaryCA(
        /* parameters */,
        currentTheme: currentTheme
    );
}
```

### Option B: Query Theme from Render Service

If passing theme is not feasible, query from Application:

```csharp
// In CellularAutomatonRenderService.cs
private (byte r, byte g, byte b) GetThemeBackgroundColor()
{
    // This must run on UI thread or have dispatcher access
    ElementTheme theme = ElementTheme.Default;

    // Query application theme (requires UI thread context)
    App.Current.DispatcherQueue.TryEnqueue(() =>
    {
        var rootFrame = App.Current.Window?.Content as Frame;
        theme = rootFrame?.ActualTheme ?? ElementTheme.Light;
    });

    return theme == ElementTheme.Dark 
        ? ((byte)0, (byte)0, (byte)0)       // Black for dark theme
        : ((byte)255, (byte)255, (byte)255); // White for light theme
}
```

**⚠️ Note**: Option B requires careful handling of thread context. Option A is simpler and recommended.

---

## 2. Background Fill Implementation

**CellularAutomatonRenderService.cs** - New method:

```csharp
/// <summary>
/// Fill pixel buffer with theme-appropriate background color.
/// </summary>
/// <param name="pixels">BGRA pixel buffer to fill</param>
/// <param name="width">Image width in pixels</param>
/// <param name="height">Image height in pixels</param>
/// <param name="isDarkTheme">True for black background, false for white</param>
private void FillBackground(byte[] pixels, int width, int height, bool isDarkTheme)
{
    byte bgValue = isDarkTheme ? (byte)0 : (byte)255;

    // Fill entire buffer with background color (BGRA format)
    for (int i = 0; i < pixels.Length; i += 4)
    {
        pixels[i + 0] = bgValue; // B
        pixels[i + 1] = bgValue; // G
        pixels[i + 2] = bgValue; // R
        pixels[i + 3] = 255;     // A (fully opaque)
    }
}
```

**Usage in RenderElementaryCA():**

```csharp
public byte[] RenderElementaryCA(
    /* parameters */,
    ElementTheme currentTheme = ElementTheme.Default)
{
    byte[] pixels = new byte[width * height * 4];

    // Step 1: Fill background
    bool isDarkTheme = (currentTheme == ElementTheme.Dark);
    FillBackground(pixels, width, height, isDarkTheme);

    // Step 2: Draw CA cells
    for (int gen = 0; gen < generations; gen++)
    {
        for (int cell = 0; cell < gridWidth; cell++)
        {
            if (state[cell])
            {
                DrawCell(pixels, width, height, cell, gen, cellSize, color);
            }
        }
        state = ApplyRule(state, rule, wrapEdges);
    }

    // Step 3: Draw grid (if enabled)
    if (showGrid)
    {
        DrawGrid(pixels, width, height, cellSize, gridWidth, gridHeight);
    }

    return pixels;
}
```

---

## 3. Grid Overlay Implementation

### Grid Drawing Method

**CellularAutomatonRenderService.cs** - New method:

```csharp
/// <summary>
/// Draw a light gray grid overlay on the pixel buffer.
/// </summary>
/// <param name="pixels">BGRA pixel buffer</param>
/// <param name="width">Image width in pixels</param>
/// <param name="height">Image height in pixels</param>
/// <param name="cellSize">Size of each cell in pixels</param>
/// <param name="gridWidth">Number of cells horizontally</param>
/// <param name="gridHeight">Number of cells vertically (generations)</param>
private void DrawGrid(byte[] pixels, int width, int height, 
    int cellSize, int gridWidth, int gridHeight)
{
    // Light gray color (visible on both light and dark backgrounds)
    const byte GRID_R = 192;
    const byte GRID_G = 192;
    const byte GRID_B = 192;

    // Calculate actual grid dimensions in pixels
    int gridPixelWidth = gridWidth * cellSize;
    int gridPixelHeight = gridHeight * cellSize;

    // Draw vertical lines (cell boundaries)
    for (int x = 0; x <= gridWidth; x++)
    {
        int pixelX = x * cellSize;
        if (pixelX >= width) break;

        DrawVerticalLine(pixels, width, height, pixelX, 0, gridPixelHeight, 
            GRID_R, GRID_G, GRID_B);
    }

    // Draw horizontal lines (generation boundaries)
    for (int y = 0; y <= gridHeight; y++)
    {
        int pixelY = y * cellSize;
        if (pixelY >= height) break;

        DrawHorizontalLine(pixels, width, height, 0, gridPixelWidth, pixelY,
            GRID_R, GRID_G, GRID_B);
    }
}

/// <summary>
/// Draw a vertical line in the pixel buffer.
/// </summary>
private void DrawVerticalLine(byte[] pixels, int width, int height, 
    int x, int y0, int y1, byte r, byte g, byte b)
{
    if (x < 0 || x >= width) return;

    int startY = Math.Max(0, y0);
    int endY = Math.Min(height - 1, y1);

    for (int y = startY; y <= endY; y++)
    {
        int index = (y * width + x) * 4;
        pixels[index + 0] = b; // B
        pixels[index + 1] = g; // G
        pixels[index + 2] = r; // R
        pixels[index + 3] = 255; // A
    }
}

/// <summary>
/// Draw a horizontal line in the pixel buffer.
/// </summary>
private void DrawHorizontalLine(byte[] pixels, int width, int height,
    int x0, int x1, int y, byte r, byte g, byte b)
{
    if (y < 0 || y >= height) return;

    int startX = Math.Max(0, x0);
    int endX = Math.Min(width - 1, x1);

    for (int x = startX; x <= endX; x++)
    {
        int index = (y * width + x) * 4;
        pixels[index + 0] = b; // B
        pixels[index + 1] = g; // G
        pixels[index + 2] = r; // R
        pixels[index + 3] = 255; // A
    }
}
```

---

## 4. Parameter Integration

### FractalParameterService.cs - Updated CA Template

Add new grid parameters to the CA parameter template:

```csharp
private ParameterSet CreateCATemplate(string ruleName, int ruleNumber)
{
    var paramSet = new ParameterSet
    {
        FractalName = ruleName,
        Category = "CellularAutomaton"
    };

    // Existing parameters (generations, cellSize, etc.)
    // ...

    // NEW: Grid dimension parameters
    paramSet.Parameters["gridWidth"] = new ParameterDefinition
    {
        Name = "Grid Width",
        Type = ParameterType.Integer,
        DefaultValue = 100,
        MinValue = 10,
        MaxValue = 200,
        Category = "Grid",
        Description = "Number of cells per row"
    };

    paramSet.Parameters["gridHeight"] = new ParameterDefinition
    {
        Name = "Grid Height",
        Type = ParameterType.Integer,
        DefaultValue = 100,
        MinValue = 10,
        MaxValue = 200,
        Category = "Grid",
        Description = "Number of generations (rows)"
    };

    paramSet.Parameters["showGrid"] = new ParameterDefinition
    {
        Name = "Show Grid",
        Type = ParameterType.Boolean,
        DefaultValue = true,
        Category = "Grid",
        Description = "Display grid overlay"
    };

    return paramSet;
}
```

### Parameter Extraction in FractalRenderService.cs

```csharp
private async Task<FractalRenderResult> RenderCellularAutomatonAsync(
    Dictionary<string, object> extendedParameters,
    int width,
    int height,
    ElementTheme currentTheme)
{
    // Extract grid parameters
    int gridWidth = GetIntParameter(extendedParameters, "gridWidth", 100);
    int gridHeight = GetIntParameter(extendedParameters, "gridHeight", 100);
    bool showGrid = GetBoolParameter(extendedParameters, "showGrid", true);
    int cellSize = GetIntParameter(extendedParameters, "cellSize", 2);

    // Apply aspect ratio constraints
    int maxGridWidth = width / cellSize;
    int maxGridHeight = height / cellSize;

    gridWidth = Math.Clamp(gridWidth, 10, maxGridWidth);
    gridHeight = Math.Clamp(gridHeight, 10, maxGridHeight);

    _logger.LogInformation(
        "CA Grid: {Width}x{Height} cells, cellSize={CellSize}px, grid={ShowGrid}",
        gridWidth, gridHeight, cellSize, showGrid);

    // Call renderer
    var caRenderer = new CellularAutomatonRenderService();
    return await Task.Run(() =>
    {
        byte[] pixels = caRenderer.RenderElementaryCA(
            /* other parameters */,
            cellSize: cellSize,
            gridWidth: gridWidth,
            gridHeight: gridHeight,
            showGrid: showGrid,
            currentTheme: currentTheme
        );

        return new FractalRenderResult { PixelData = pixels };
    });
}
```

---

## 5. UI Integration

### Dynamic Parameter Max Values

The grid dimension sliders should adapt their max values based on canvas size:

**ParameterEditorViewModel.cs** - Add dynamic validation:

```csharp
public void UpdateGridConstraints(int canvasWidth, int canvasHeight, int cellSize)
{
    if (CurrentParameters?.Parameters == null) return;

    int maxGridWidth = canvasWidth / cellSize;
    int maxGridHeight = canvasHeight / cellSize;

    if (CurrentParameters.Parameters.TryGetValue("gridWidth", out var widthParam))
    {
        widthParam.MaxValue = maxGridWidth;
    }

    if (CurrentParameters.Parameters.TryGetValue("gridHeight", out var heightParam))
    {
        heightParam.MaxValue = maxGridHeight;
    }

    // Notify UI to refresh sliders
    OnPropertyChanged(nameof(CurrentParameters));
}
```

**MainPage.cs** - Call on canvas size change:

```csharp
private void OnCanvasSizeChanged(object sender, SizeChangedEventArgs e)
{
    if (ViewModel.CurrentCategory == FractalCategory.CellularAutomaton)
    {
        int cellSize = ParameterEditorViewModel.GetIntParameter("cellSize", 2);
        ParameterEditorViewModel.UpdateGridConstraints(
            (int)e.NewSize.Width,
            (int)e.NewSize.Height,
            cellSize
        );
    }
}
```

---

## 6. Testing Checklist

### Theme Testing

- [ ] Launch app in Light theme → CA renders with white background
- [ ] Switch to Dark theme → CA re-renders with black background
- [ ] Switch to Ocean Blue (Light-based) → White background
- [ ] Spectral colors remain vibrant on both backgrounds

### Grid Testing

- [ ] Toggle "Show Grid" ON → Grid appears
- [ ] Toggle "Show Grid" OFF → Grid disappears
- [ ] Adjust grid width → Grid columns change
- [ ] Adjust grid height → Grid rows change
- [ ] Adjust cell size → Grid spacing changes proportionally
- [ ] Grid lines are visible on both light and dark backgrounds
- [ ] Grid does not obscure CA cell colors significantly

### Edge Cases

- [ ] Cell size = 1 pixel → Grid still renders (may be very dense)
- [ ] Cell size = 5 pixels → Grid is clearly visible
- [ ] Grid width = 200 cells → Renders without performance issues
- [ ] Grid height = 200 generations → Renders without performance issues
- [ ] Canvas resize → Grid constraints update dynamically

---

## 7. Performance Considerations

### Grid Drawing Optimization

Drawing 200×200 grid lines (400 lines total) at 1920×1080 resolution:
- Estimated pixels drawn: ~800,000 pixels (grid lines only)
- Expected overhead: <5ms on modern hardware

**Optimization strategies:**
1. Draw grid lines only within visible canvas bounds
2. Skip grid drawing if cell size < 2 pixels (grid would be too dense)
3. Use horizontal/vertical line primitives (faster than Bresenham)
4. Consider caching grid overlay as separate layer (if UI framework supports it)

### Memory Impact

No significant memory impact:
- Grid is drawn directly into existing pixel buffer
- No additional allocations required
- Total memory = `width × height × 4 bytes` (same as before)

---

## 8. Visual Examples (Conceptual)

### Light Theme with Grid (Conceptual)
```
Background: White (#FFFFFF)
CA Cells: Spectral colors (rainbow gradient)
Grid: Light gray (#C0C0C0)
Result: Vibrant colors on white with subtle grid
```

### Dark Theme with Grid (Conceptual)
```
Background: Black (#000000)
CA Cells: Spectral colors (rainbow gradient)
Grid: Light gray (#C0C0C0)
Result: Glowing colors on black with visible grid
```

---

## 9. Implementation Order

Follow the checklist phases, but within Phase 3 (Rendering Service):

1. **Step 3.7**: Implement `GetThemeBackgroundColor()`
2. **Step 3.8**: Implement `FillBackground()`
3. **Step 3.9**: Implement `DrawGrid()` (with helper line methods)
4. **Step 3.10**: Add grid parameters to method signature
5. **Test**: Render with theme detection and grid overlay

---

## 10. Code Style Guidelines

### Following Project Conventions

Based on `LSystemRenderService.cs` patterns:

- ✅ Use XML documentation comments for all public methods
- ✅ Add debug logging with `Debug.WriteLine()` for diagnostics
- ✅ Use descriptive variable names (e.g., `isDarkTheme`, not `dark`)
- ✅ Validate parameters with guard clauses
- ✅ Use `const` for magic numbers (e.g., `GRID_COLOR_R`)
- ✅ Format code consistently (match project style)

### Error Handling

```csharp
public void DrawGrid(/* parameters */)
{
    // Validate parameters
    if (pixels == null)
        throw new ArgumentNullException(nameof(pixels));

    if (cellSize <= 0)
        throw new ArgumentOutOfRangeException(nameof(cellSize), 
            "Cell size must be positive");

    // Clamp grid dimensions to safe bounds
    gridWidth = Math.Clamp(gridWidth, 1, width / cellSize);
    gridHeight = Math.Clamp(gridHeight, 1, height / cellSize);

    // Proceed with drawing...
}
```

---

## 11. References

- **Theme System**: `ManpWinUI/Documentation/Features/WINUI3_THEME_BEST_PRACTICES.md`
- **Rendering Architecture**: `ManpWinUI/Services/LSystemRenderService.cs` (reference implementation)
- **Parameter System**: `ManpWinUI/Services/FractalParameterService.cs`
- **Main Integration**: `ManpWinUI/Views/MainPage.cs`

---

**Last Updated**: 2025-01-16  
**Maintainer**: Development Team  
**Status**: Implementation guidance complete
