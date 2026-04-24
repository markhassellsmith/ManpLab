# ManpWinUI Keyboard Shortcuts

Modern WinUI 3 fractal explorer keyboard reference for the ManpWinUI application.

**✅ ALL SHORTCUTS FULLY IMPLEMENTED!** Press **F1** in the app to see this help dialog.

---

## 🎯 Rendering & View

| Shortcut | Action | Description |
|----------|--------|-------------|
| **F1** | Show help | Display keyboard shortcuts dialog |
| **F5** | Render fractal | Recalculate and render the current fractal |
| **Ctrl+R** | Render fractal | Alternative render shortcut |
| **Space** | Reset view | Return to default Mandelbrot view |
| **Home** | Reset view | Alternative reset shortcut |
| **Esc** | Cancel render | Stop current rendering (placeholder) |

---

## 🔍 Zoom & Navigation

| Shortcut | Action | Description |
|----------|--------|-------------|
| **+** or **=** | Zoom in | Zoom in 2× at center point |
| **-** or **_** | Zoom out | Zoom out 2× at center point |
| **Arrow Keys** | Pan view | Move view by 10% (smooth panning) |
| **Shift+Arrows** | Pan faster | Move view by 25% (quick navigation) |
| **Mouse Click** | Zoom to point | Left-click to zoom in at cursor position |
| **Mouse Drag** | Select zoom area | Click and drag to define zoom rectangle |
| **Right-Click Drag** | Pan view | Click and drag to pan the fractal |

---

## ⚙️ Parameters & Settings

| Shortcut | Action | Description |
|----------|--------|-------------|
| **Page Up** | Increase iterations | Add 100 to max iterations |
| **Page Down** | Decrease iterations | Subtract 100 from max iterations |
| **Shift+Page Up** | Increase iterations (fast) | Add 500 to max iterations |
| **Shift+Page Down** | Decrease iterations (fast) | Subtract 500 from max iterations |
| **I** | Toggle auto-scale | Enable/disable automatic iteration scaling |

---

## 📐 Resolution Presets

| Shortcut | Resolution | Pixels |
|----------|-----------|--------|
| **1** | HD | 1280×720 |
| **2** | Full HD | 1920×1080 |
| **3** | 2K | 2560×1440 |
| **4** | 4K | 3840×2160 |

---

## 🎨 Display & Modes

| Shortcut | Action | Description |
|----------|--------|-------------|
| **A** | Toggle axes | Show/hide coordinate axes overlay |
| **T** | Toggle mode | Switch between Standard and Julia set modes |

---

## 💾 File Operations

| Shortcut | Action | Description |
|----------|--------|-------------|
| **Ctrl+S** | Save image | Export current fractal (coming soon!) |

---

## ❓ Help

| Shortcut | Action | Description |
|----------|--------|-------------|
| **F1** | Show help | Display this keyboard shortcuts reference |

---

## 💡 Quick Start Guide

### First Time User

1. **Launch ManpWinUI** → The default Mandelbrot set renders automatically
2. **Press F1** → View all available keyboard shortcuts
3. **Try these basics:**
   - **Arrow keys** to pan around
   - **+ / -** to zoom in/out
   - **1-4** to try different resolutions
   - **Space** to reset if you get lost

### Exploring Fractals

**Navigate:**
```
Arrow Keys      → Pan smoothly (10% steps)
Shift+Arrows    → Pan quickly (25% steps)
Click point     → Zoom into that location
Drag rectangle  → Zoom to selection
+ / -           → Zoom in/out at center
```

**Adjust Detail:**
```
Page Up/Down         → Fine-tune iterations (±100)
Shift+Page Up/Down   → Quickly adjust (±500)
I                    → Auto-scale iterations on/off
```

**Change View:**
```
1, 2, 3, 4    → Quick resolution presets
T             → Switch to Julia set mode
A             → Toggle coordinate axes
Space/Home    → Reset to default view
```

---

## 🎯 Pro Tips

**Performance:**
- Use lower resolutions (1280×720) for fast exploration
- Switch to higher resolutions (2K, 4K) for final renders
- Auto-scale iterations (I) helps maintain detail at all zoom levels
- Press F5 or Ctrl+R to re-render after changing settings

**Navigation:**
- Use Shift+Arrows for quick panning across large areas
- Click-and-drag creates a precise zoom rectangle
- Right-click-and-drag to pan without zooming
- Press Space/Home anytime to return to the default view

**Detail Control:**
- Start with lower iterations for faster feedback
- Increase iterations (Page Up) when you find interesting areas
- Use Shift+Page Up/Down for large iteration jumps
- Auto-scale (I) automatically adjusts iterations based on zoom level

**Exploration Workflow:**
1. Pan around with arrow keys to find interesting regions
2. Click to zoom into detail
3. Adjust iterations (Page Up/Down) to reveal fine structure
4. Use resolution presets (1-4) to balance speed vs. quality
5. Press A to show coordinate axes for reference
6. Press Space to reset and start over

---

## 🔬 Technical Notes

**Implementation:**
- All keyboard shortcuts are implemented in `MainPage.KeyboardHandling.cs`
- Shortcuts respect text input focus (won't interfere with typing in fields)
- Modifier key detection supports Ctrl, Shift, and Alt combinations
- Commands are executed through the MVVM architecture via ViewModel commands

**Customization:**
- Keyboard shortcuts are hard-coded in the partial class
- Tooltips on toolbar buttons show relevant shortcuts
- In-app help dialog (F1) provides comprehensive reference

---

## 📝 Changelog

### Phase 3 - WinUI Implementation (January 2026)
- ✅ Comprehensive keyboard shortcut system
- ✅ F1 help dialog with full reference
- ✅ Zoom and pan controls with modifier keys
- ✅ Resolution presets (1-4 keys)
- ✅ Parameter adjustment shortcuts
- ✅ Mode toggles (Julia, axes, auto-scale)
- 🔄 Save functionality (planned)
- 🔄 Render cancellation (planned)

---

## 🆚 Comparison with ManpWIN64

This is the modern WinUI 3 reimplementation. For the original C++ application (ManpWIN64), see the repository root `KEYBOARD_SHORTCUTS.md`.

**Key Differences:**
- ManpWinUI: Modern WinUI 3 interface, MVVM architecture, .NET 10
- ManpWIN64: Classic Win32 API, C++17, extensive fractal library (240+ types)

**Shortcuts Philosophy:**
- ManpWinUI: Simplified, beginner-friendly shortcuts for core features
- ManpWIN64: Extensive shortcuts for advanced features and color controls

---

**WinUI Application** | **Built with .NET 10** | **Last Updated:** January 2026
