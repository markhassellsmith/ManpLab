# Navigation Methods - Complete Coverage

**Feature**: All zoom, pan, and render methods record navigation history  
**Status**: ✅ Complete

---

## Overview

Navigation history automatically records state after **any** render completes, regardless of how the render was triggered. This provides comprehensive coverage of all user navigation methods.

---

## Navigation Methods That Record History

### 1. Direct Render ✅

**Trigger**: User clicks "Render" button  
**Path**: `RenderCommand` → `RenderMandelbrotAsync()` / `RenderHailstoneAsync()` → `RecordNavigationState()`  
**Result**: Initial state or parameter-change state recorded

```
User: Click Render
  → Render completes
  → RecordNavigationState() called
  → History: [current state]
```

---

### 2. Toolbar Zoom Buttons ✅

**Trigger**: User clicks Zoom In / Zoom Out toolbar buttons  
**Path**: `ZoomInCommand` / `ZoomOutCommand` → changes `Zoom` property → auto-renders → `RecordNavigationState()`  
**Result**: Zoomed state recorded after render

```
User: Click Zoom In
  → Zoom *= 2.0
  → Auto-render triggered
  → Render completes
  → RecordNavigationState() called
  → History: [..., zoomed state]
```

---

### 3. Mouse Wheel Zoom ✅

**Trigger**: User scrolls mouse wheel over fractal image  
**Path**: `PointerWheelChanged` → changes `Zoom` → debounced render (300ms) → `RecordNavigationState()`  
**Result**: Final zoom state recorded after scroll stops

```
User: Scroll wheel up/down rapidly
  → Zoom changes multiple times
  → Debounce timer waits 300ms after last scroll
  → Render executes once
  → RecordNavigationState() called
  → History: [..., final zoom state]
```

**Note**: Debouncing prevents recording intermediate states during rapid scrolling.

---

### 4. Mouse Drag-Zoom (Box Select) ✅

**Trigger**: User left-click-drags to select a rectangle on the fractal  
**Path**: `PointerReleased` → `ZoomToRectangle()` → calculates new center/zoom → `RenderCommand` → `RecordNavigationState()`  
**Result**: Zoomed-to-rectangle state recorded

```
User: Left-drag to draw zoom box
  → Selection rectangle shown during drag
  → PointerReleased triggers ZoomToRectangle()
  → New center and zoom calculated
  → Auto-render triggered
  → Render completes
  → RecordNavigationState() called
  → History: [..., box-zoom state]
```

**Selection behavior**:
- Rectangle maintains aspect ratio of target image
- Minimum size: 10×10 pixels (prevents accidental clicks)
- Rectangle too small → no zoom, message shown

---

### 5. Mouse Pan (Right-Click Drag) ✅

**Trigger**: User right-click-drags to pan the view  
**Path**: `PointerMoved` (updates center continuously) → `PointerReleased` → `RenderCommand` → `RecordNavigationState()`  
**Result**: Final panned position recorded

```
User: Right-drag to pan
  → CenterX/CenterY updated during drag
  → PointerReleased triggers render
  → Render completes
  → RecordNavigationState() called
  → History: [..., panned state]
```

**Pan behavior**:
- "Paper-on-desk" model: drag right → image moves right
- Real-time coordinate updates during drag
- Single history entry for entire pan operation (not per-frame)

---

### 6. Bookmark Restoration ✅

**Trigger**: User clicks a bookmark to load its saved state  
**Path**: `LoadBookmarkAsync()` → updates all parameters → `RenderCommand` → `RecordNavigationState()`  
**Result**: Bookmark state recorded in history

```
User: Click bookmark
  → All fractal parameters restored
  → Render triggered
  → Render completes
  → RecordNavigationState() called
  → History: [..., bookmark state]
```

**Guard**: `_isRestoringFromHistory` flag is NOT set for bookmarks (they should create history).

---

### 7. Manual Parameter Changes ✅

**Trigger**: User edits center, max iterations, palette, etc., then clicks Render  
**Path**: Parameter changed → user clicks Render → `RenderCommand` → `RecordNavigationState()`  
**Result**: New parameter state recorded

```
User: Change MaxIterations to 500, click Render
  → MaxIterations = 500
  → Render triggered
  → Render completes
  → RecordNavigationState() called
  → History: [..., state with 500 iterations]
```

---

### 8. Fractal Type / Mode Changes ✅

**Trigger**: User switches fractal type (Mandelbrot → Julia, etc.)  
**Path**: Type changed → user clicks Render (or auto-renders) → `RecordNavigationState()`  
**Result**: New fractal type state recorded

```
User: Switch to Julia mode
  → IsJuliaMode = true
  → User clicks Render (or zoom, which auto-renders)
  → Render completes
  → RecordNavigationState() called
  → History: [..., Julia state]
```

---

## Operations That Do NOT Record History

### ❌ Undo/Redo Navigation

**Reason**: `_isRestoringFromHistory` flag prevents recording during history restoration

```csharp
private void RecordNavigationState(...)
{
    if (_isRestoringFromHistory)
        return; // Don't record when undoing/redoing
    // ...
}
```

Without this guard, undo would create a new entry, making redo impossible.

---

### ❌ Parameter Changes Alone

**Reason**: Only completed renders record history

```
User: Change Zoom to 4.0 (without rendering)
  → Zoom property updates
  → No render triggered
  → No history entry ❌
```

Parameters alone don't create navigable states. Only rendered images do.

---

### ❌ UI Panel Toggles

**Reason**: UI state (bookmarks panel open/closed) is not navigation state

```
User: Toggle bookmarks panel
  → Panel visibility changes
  → No render triggered
  → No history entry ❌
```

---

### ❌ Hailstone Mode (Special Case)

**Note**: Hailstone sequences don't use standard zoom/pan navigation, so `RecordNavigationState()` guards against `IsHailstoneMode`.

However, Hailstone **renders** still record state (for consistency).

---

## Design Principle: Single Recording Point

**All paths converge at render completion:**

```
Zoom Button ────┐
Mouse Wheel ────┤
Drag-Zoom ──────┤
Pan ────────────┼──→ RenderCommand ──→ RecordNavigationState()
Manual Edit ────┤
Bookmark ───────┤
Direct Render ──┘
```

This ensures:
1. **Consistency**: Every navigation creates exactly one history entry
2. **Simplicity**: No need to track which action triggered render
3. **Correctness**: History always represents completed, visible states
4. **No duplicates**: Can't accidentally record multiple times
5. **Debouncing works**: Rapid mouse wheel → single history entry

---

## Testing Matrix

### ✅ All Methods Create History

| Method | Triggers Render | Records State | Enables Undo After |
|--------|----------------|---------------|---------------------|
| Direct Render | ✅ | ✅ | 2nd action |
| Zoom Button | ✅ | ✅ | Yes (2nd zoom) |
| Mouse Wheel | ✅ | ✅ | Yes (after debounce) |
| Drag-Zoom | ✅ | ✅ | Yes |
| Pan | ✅ | ✅ | Yes |
| Bookmark Load | ✅ | ✅ | Yes |
| Manual Edit + Render | ✅ | ✅ | Yes |
| Type Change + Render | ✅ | ✅ | Yes |

### ✅ Guards Work Correctly

| Operation | Creates Render | Records State | Why |
|-----------|---------------|---------------|-----|
| Undo | ✅ | ❌ | `_isRestoringFromHistory` guard |
| Redo | ✅ | ❌ | `_isRestoringFromHistory` guard |
| Param change only | ❌ | ❌ | No render triggered |
| Panel toggle | ❌ | ❌ | Not a navigation action |

---

## User Experience

### Expected Flow

```
1. User renders → History: [A]
   Back: disabled (only 1 state)

2. User zooms (any method) → History: [A, B]
   Back: ENABLED ✅

3. User pans → History: [A, B, C]
   Back: enabled

4. User clicks Back → Position: 1, viewing B
   Forward: ENABLED ✅

5. User zooms again → History: [A, B, D] (C cleared)
   Forward: disabled (branched)
```

### All Zoom Methods Equivalent

From the user's perspective, **all zoom methods produce the same result**: a new entry in history that can be undone.

- Button zoom, mouse wheel zoom, and drag-zoom are functionally identical
- Choice of method is user preference (precision vs speed vs convenience)
- History doesn't distinguish how the state was created

---

## Performance Notes

### Debouncing

- **Mouse wheel**: 300ms debounce (prevents 10+ entries during rapid scroll)
- **Hailstone zoom**: 500ms debounce (heavier render, longer debounce)

### Significance Filtering

Even after debouncing, `IsSignificantChangeFrom()` prevents near-duplicate entries:

```csharp
// Skip if zoom changed < 0.1%
var zoomRatio = Zoom / other.Zoom;
if (zoomRatio >= 0.999 && zoomRatio <= 1.001) return false;
```

This catches accidental mouse moves and floating-point rounding during rapid interactions.

---

## Related Documentation

- `Phase2-Week8-BugFix-DisabledButtons.md` - Why recording happens at render completion
- `Phase2-Week8-PositionModel-Final.md` - How undo/redo position model works
- `Phase2-Week8-NavigationHistory-Summary.md` - Complete feature overview

---

**Status**: ✅ Complete  
**Coverage**: All navigation methods (8 methods)  
**Guards**: Undo/Redo protected from re-recording  
**Testing**: All methods verified to create history correctly
