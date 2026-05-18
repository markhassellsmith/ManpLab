# Zoom Center Drift Fix

## Problem Description

When repeatedly clicking the zoom in/out buttons while viewing a Julia set (or any fractal), the center point visibly drifts to one side. This requires manual panning to recenter the radiant point.

## Root Cause

The issue is caused by **discrete pixel grid aliasing**, not hardware or monitor-specific properties.

### Mathematical Explanation

For an image with width W pixels (indexed 0 to W-1):
- The geometric center is at pixel position `(W-1)/2` (e.g., 399.5 for W=800)
- Since we can only render at integer pixel indices, there's always a **sub-pixel offset**
- The pixel-to-complex-plane mapping formula is:
  ```
  x = centerX - (viewWidth / 2) + (px * viewWidth / width)
  ```
- For the center pixel at index `(W-1)/2`:
  ```
  actualCenterX = centerX - (viewWidth / 2) + ((W-1)/2 * viewWidth / W)
  actualCenterX = centerX + ((W-1)/2 - W/2) * (viewWidth / W)
  actualCenterX = centerX - 0.5 * pixelSize
  ```

This **half-pixel offset** means the actual rendered center has a small but systematic offset from the intended `(centerX, centerY)`. When you zoom repeatedly, this offset compounds and causes visible drift.

## Solution

The fix implements a **center-correction mechanism** that:

1. Calculates the **differential correction** needed between the old and new pixel offsets
2. Applies this correction to `CenterX` and `CenterY` **before** changing the zoom
3. Then applies the zoom factor

This ensures the same point in the complex plane remains at the viewport center across zoom operations.

### Mathematical Derivation

From the `PixelToComplex` formula in `MandelbrotCalculator.h`:
```cpp
x = centerX - (viewWidth / 2) + (px * viewWidth / width)
```

For the center pixel at `px = (width-1)/2`:
```
x_center = centerX - viewWidth / (2 * width)
```

The actual complex coordinate at the viewport center is **always offset** by `-viewWidth/(2*width)` from `centerX`.

When zooming by factor `f`:
- Before: `x_old = centerX_old - viewWidth_old / (2 * width)`
- After: `x_new = centerX_new - viewWidth_new / (2 * width)`
- Where: `viewWidth_new = viewWidth_old / f`

To keep the same coordinate at center (`x_old == x_new`):
```
centerX_new = centerX_old + (viewWidth_new - viewWidth_old) / (2 * width)
centerX_new = centerX_old + viewWidth_old * (1/f - 1) / (2 * width)
```

**Examples:**
- **Zoom in by 2x** (f=2): correction = `viewWidth * (1/2 - 1) / (2*width)` = `-viewWidth / (4*width)` (shift left slightly)
- **Zoom out by 2x** (f=0.5): correction = `viewWidth * (2 - 1) / (2*width)` = `+viewWidth / (2*width)` (shift right slightly)

### Implementation

Added `ApplyZoomCorrection(double zoomMultiplier)` method in `MainViewModel.Navigation.cs`:

```csharp
public void ApplyZoomCorrection(double zoomMultiplier)
{
    // Calculate current view dimensions
    double oldViewWidth = 3.0 / Zoom;
    double oldViewHeight = oldViewWidth * ((double)ImageHeight / ImageWidth);

    // Calculate the correction needed to keep the same complex point at the viewport center
    // After zoom by zoomMultiplier, viewWidth becomes viewWidth/zoomMultiplier
    // The correction is: (viewWidth_new - viewWidth_old) / (2 * imageWidth)
    double correctionX = oldViewWidth * (1.0 / zoomMultiplier - 1.0) / (2.0 * ImageWidth);
    double correctionY = oldViewHeight * (1.0 / zoomMultiplier - 1.0) / (2.0 * ImageHeight);

    // Apply the correction to center
    CenterX += correctionX;
    CenterY += correctionY;

    // Apply zoom to the zoom factor
    Zoom *= zoomMultiplier;
}
```

**Key Insight:** The correction is applied **BEFORE** changing the zoom, and it accounts for the **difference** between the old and new pixel grid offsets.

### Modified Files

1. **ManpWinUI\ViewModels\MainViewModel.Navigation.cs**
   - Added `ApplyZoomCorrection()` method
   - Updated `ZoomInAsync()` to use `ApplyZoomCorrection(2.0)`
   - Updated `ZoomOutAsync()` to use `ApplyZoomCorrection(0.5)`
   - Updated `OnZoomFineTuneChanged()` to use `ApplyZoomCorrection()`
   - Updated `ZoomFineTuneStepAsync()` to use `ApplyZoomCorrection()`

2. **ManpWinUI\Views\MainPage.MouseInteraction.cs**
   - Updated mouse wheel zoom handler to use `ApplyZoomCorrection()`

## Benefits

- **Eliminates center drift** during repeated zoom operations
- **Maintains visual stability** when zooming in/out on features
- **No user intervention** required to recenter the viewport
- **Works for all zoom methods**: buttons, slider, fine-tune buttons, mouse wheel

## Testing

To verify the fix:

1. Load a Julia preset (e.g., Julia Preset 1: Classic Spiral)
2. Position the radiant point at the center of the viewport
3. Click the zoom in button repeatedly (10-20 times)
4. Observe that the radiant point **stays centered** without drift
5. Click zoom out to return to original view
6. Verify the center remains stable

## Technical Notes

- This is a **calculation issue**, not hardware-specific
- The fix applies to all fractals (Mandelbrot, Julia, Burning Ship, etc.)
- The correction is mathematically exact for the discrete pixel grid
- Floating-point precision is sufficient even at extreme zoom levels (deep zoom mode uses arbitrary precision for coordinates, not affected by this issue)
