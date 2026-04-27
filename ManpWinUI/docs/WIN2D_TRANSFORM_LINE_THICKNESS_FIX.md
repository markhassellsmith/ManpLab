# Win2D Transform Line Thickness Fix

## Problem

When using coordinate transforms in Win2D, **line thickness values are transformed along with coordinates**. This caused extremely thick lines in the Hailstone visualization.

### Root Cause

When a transform matrix is applied with scale factors (e.g., 30 pixels/world unit), all drawing parameters are affected:
- **Coordinates** are correctly transformed (intended)
- **Line thickness** is also transformed (unintended!)

Example with scale = 30 pixels/unit:
- Desired: 1.2 pixel thick line
- Actual: 1.2 world units × 30 pixels/unit = **36 pixel thick line**
- Result: Giant blob-like lines instead of thin trajectory paths

## Solution

### Line Thickness Compensation

When drawing with transforms enabled, line thickness must be **inversely scaled** to maintain consistent pixel width:

```csharp
// Calculate line thickness in world units to achieve desired pixel width
float avgScale = (float)((Math.Abs(scaleX) + Math.Abs(scaleY)) / 2.0);
float desiredPixelThickness = 1.2f;
float worldThickness = desiredPixelThickness / avgScale;

// Draw with compensated thickness
renderer.DrawLine(x1, y1, x2, y2, color, worldThickness);
```

### Why This Works

```
World Thickness × Scale = Screen Thickness
(desiredPixels / scale) × scale = desiredPixels ✓
```

## Changes Made

### 1. Grid Lines (`DrawGridAndAxesWithTransform`)

**Before:**
```csharp
renderer.DrawLine((float)x, yMin, (float)x, yMax, color, 1.0f);
```

**After:**
```csharp
float avgScale = (float)((Math.Abs(scaleX) + Math.Abs(scaleY)) / 2.0);
float lineThickness = 1.0f / avgScale;  // 1 pixel width
renderer.DrawLine((float)x, yMin, (float)x, yMax, color, lineThickness);
```

### 2. Trajectory Lines (`DrawSequencePathWithTransform`)

**Before:**
```csharp
// Fixed thickness values - gets scaled incorrectly!
renderer.DrawLine((float)p1.X, (float)p1.Y, (float)p2.X, (float)p2.Y, Colors.Magenta, 2.5f);
renderer.DrawLine((float)p1.X, (float)p1.Y, (float)p2.X, (float)p2.Y, color, 1.2f);
```

**After:**
```csharp
float avgScale = (float)((Math.Abs(scaleX) + Math.Abs(scaleY)) / 2.0);
float normalThickness = 1.2f / avgScale;  // 1.2 pixels
float cycleThickness = 2.5f / avgScale;   // 2.5 pixels

renderer.DrawLine((float)p1.X, (float)p1.Y, (float)p2.X, (float)p2.Y, Colors.Magenta, cycleThickness);
renderer.DrawLine((float)p1.X, (float)p1.Y, (float)p2.X, (float)p2.Y, color, normalThickness);
```

### 3. Point Markers (`DrawPointsWithTransform`)

Point markers already fixed in previous commit - transform is **reset** before drawing circles/rectangles to prevent radius scaling:

```csharp
// Reset transform for fixed pixel-size markers
renderer.ResetTransform();

// Draw with screen coordinates (no scaling applied)
renderer.DrawCircle(screenX, screenY, 3, Colors.Yellow);  // 3 pixel radius
```

## Transform Scaling Summary

| Element | Transform Applied? | Size Compensation |
|---------|-------------------|-------------------|
| Grid lines | ✓ Yes | Thickness ÷ scale |
| Trajectory lines | ✓ Yes | Thickness ÷ scale |
| Point circles | ✗ No (reset) | Fixed pixel size |
| Point rectangles | ✗ No (reset) | Fixed pixel size |
| Text labels | ✗ No (reset) | Fixed font size |

## Testing

After these changes:
- Grid lines should be thin (1 pixel)
- Normal trajectory lines should be thin (1.2 pixels)
- Cycle lines should be thicker (2.5 pixels) but not giant
- Point markers should be small circles (2-3 pixel radius)
- Lines should maintain consistent width regardless of zoom level

## Related Issues

This fix completes the Win2D coordinate transform implementation:
1. **First issue**: Circle radii being scaled → Fixed by resetting transform
2. **Second issue**: Line thickness being scaled → Fixed by inverse scaling
3. **Result**: Smooth, correctly-sized rendering in world coordinates

## References

- Original coordinate transform implementation: commit `345834b`
- Circle radius fix: previous commit (reset transform for point markers)
- Line thickness fix: this commit
