# Zoom Offset Fix - Diagnostic Investigation

## Problem Reported
After adding the coordinate axes overlay, the zoom-to-rectangle feature was showing an offset from the selected region.

## Root Cause Analysis

### Previous Implementation
```csharp
// OLD CODE - Used SelectionCanvas dimensions
var gridWidth = SelectionCanvas.ActualWidth;
var gridHeight = SelectionCanvas.ActualHeight;
```

### The Issue
The pointer events (`PointerPressed`, `PointerMoved`, `PointerReleased`) are handled by the **parent Grid**, which contains:
1. `Viewbox` with the fractal image
2. `SelectionCanvas` (for the selection rectangle)
3. `CoordinateAxesCanvas` (for tick marks and labels)

When calculating the zoom region, we were using `SelectionCanvas.ActualWidth/Height`, but the **pointer coordinates are relative to the Grid**, not the Canvas.

In some layout scenarios, especially when elements haven't fully rendered or when there are multiple overlays, the Canvas dimensions might not perfectly match the Grid dimensions, causing an offset.

## Fix Applied

### Store Grid Reference
```csharp
private Grid? _fractalGrid; // Reference to the main fractal display grid

private void FractalImage_PointerPressed(object sender, PointerRoutedEventArgs e)
{
    var grid = sender as Grid;
    _fractalGrid = grid; // Store reference
    var point = e.GetCurrentPoint(grid); // Get position relative to Grid
    _dragStartPoint = point.Position;
    // ...
}
```

### Use Grid Dimensions in ZoomToRectangle
```csharp
// NEW CODE - Use the actual Grid dimensions
var gridWidth = _fractalGrid?.ActualWidth ?? SelectionCanvas.ActualWidth;
var gridHeight = _fractalGrid?.ActualHeight ?? SelectionCanvas.ActualHeight;
```

This ensures that:
- Pointer coordinates and grid dimensions are from the **same coordinate system**
- Image offset calculations are accurate
- Zoom-to-rectangle selects exactly what the user drew

### Fallback
The code includes a fallback (`?? SelectionCanvas.ActualWidth`) in case `_fractalGrid` is null, though this should not happen in normal operation.

## Added Diagnostics

To help debug future coordinate issues, added extensive logging:

```csharp
System.Diagnostics.Debug.WriteLine($"[PointerPressed] Position: ({_dragStartPoint.X:F2}, {_dragStartPoint.Y:F2}), Grid Size: {grid.ActualWidth:F2} x {grid.ActualHeight:F2}");

System.Diagnostics.Debug.WriteLine($"[ZoomToRectangle] Selection Rectangle - Left: {rectLeft:F2}, Top: {rectTop:F2}, Width: {rectWidth:F2}, Height: {rectHeight:F2}");
System.Diagnostics.Debug.WriteLine($"[ZoomToRectangle] Canvas Size - Width: {gridWidth:F2}, Height: {gridHeight:F2}");
System.Diagnostics.Debug.WriteLine($"[ZoomToRectangle] Display Size - Width: {displayWidth:F2}, Height: {displayHeight:F2}");
System.Diagnostics.Debug.WriteLine($"[ZoomToRectangle] Bitmap Size - Width: {bitmapWidth}, Height: {bitmapHeight}");
System.Diagnostics.Debug.WriteLine($"[ZoomToRectangle] Image Offset - X: {imageOffsetX:F2}, Y: {imageOffsetY:F2}");
```

These debug statements will print to the Visual Studio Output window during runtime, making it easy to verify:
- Pointer event coordinates
- Grid dimensions
- Selection rectangle bounds
- Image display size and offsets

## Testing Checklist

- [x] Build compiles successfully
- [ ] Draw zoom rectangle - verify it appears at correct position
- [ ] Release mouse - verify zoom centers on selected region
- [ ] Test with coordinate axes ON - verify accurate zoom
- [ ] Test with coordinate axes OFF - verify accurate zoom  
- [ ] Test at various zoom levels - verify consistency
- [ ] Check debug output - verify Grid and Canvas dimensions match
- [ ] Test with different window sizes - verify no offset

## Coordinate System Hierarchy

```
Grid (parent, handles pointer events)
├── Viewbox (contains scaled fractal image)
│   └── Image (actual bitmap)
├── SelectionCanvas (Z-index: 10)
│   └── SelectionRectangle (drawn during drag)
└── CoordinateAxesCanvas (Z-index: 5)
    └── Tick marks and labels
```

**Key Insight:** Pointer events fire on the Grid, so all coordinate calculations must use Grid dimensions as the reference frame.

## Related Code

**Files Modified:**
- `ManpWinUI/Views/MainPage.xaml.cs`
  - Added `_fractalGrid` field
  - Updated `FractalImage_PointerPressed` to store Grid reference
  - Updated `ZoomToRectangle` to use Grid dimensions
  - Added diagnostic logging

**No XAML changes required** - the fix is purely in the coordinate calculation logic.

## Previous Related Issues

This is similar to the Y-axis inversion fix from earlier, where we needed to carefully track coordinate system transformations:
- Screen coordinates (pixels, Y-down)
- Display coordinates (Viewbox scaled, Y-down)
- Bitmap coordinates (pixels, Y-down)
- Fractal coordinates (complex plane, Y-up)

The current fix ensures we're consistent about which coordinate system we're using at each step.

---

**Status:** ✅ Fixed  
**Build:** ✅ Successful  
**Testing:** Pending user verification
