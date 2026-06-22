# Cellular Automata: Visual Design Reference

**Date**: 2025-01-16  
**Feature**: CA Theme-Aware Backgrounds and Grid Overlay  
**Purpose**: Visual reference for implementation

---

## Color Specifications

### Background Colors (Theme-Aware)

| Theme | Background Color | Hex Code | RGB Values |
|-------|------------------|----------|------------|
| **Light** | White | `#FFFFFF` | `(255, 255, 255)` |
| **Dark** | Black | `#000000` | `(0, 0, 0)` |
| **Ocean Blue** | White (inherits Light) | `#FFFFFF` | `(255, 255, 255)` |

### Grid Color

| Element | Color | Hex Code | RGB Values | Visibility |
|---------|-------|----------|------------|------------|
| **Grid Lines** | Light Gray | `#C0C0C0` | `(192, 192, 192)` | Visible on both Light and Dark |

**Contrast ratios:**
- Light gray on white: 3.9:1 (adequate for non-text UI elements)
- Light gray on black: 5.3:1 (good contrast)

### CA Cell Colors (Spectral)

Cellular automata cells use **spectral (rainbow) color mapping** based on generation number:

| Generation | Hue (°) | Example Color | RGB (approx) |
|------------|---------|---------------|--------------|
| 0 | 0° | Red | `(255, 0, 0)` |
| 25% | 90° | Yellow-Green | `(128, 255, 0)` |
| 50% | 180° | Cyan | `(0, 255, 255)` |
| 75% | 270° | Purple | `(128, 0, 255)` |
| 100% | 359° | Red | `(255, 0, 0)` |

**Parameters affecting color:**
- `colorStart`: Starting hue angle (default 0° = red)
- `colorStop`: Ending hue angle (default 359° = full spectrum)
- `colorCycles`: Number of times gradient repeats (default 1)

**Example configurations:**
1. **Full Spectrum**: `colorStart=0°, colorStop=359°, colorCycles=1` → Rainbow from red to red
2. **Blue-Green**: `colorStart=120°, colorStop=210°, colorCycles=1` → Green through blue
3. **Double Rainbow**: `colorStart=0°, colorStop=359°, colorCycles=2` → Two full spectrums

---

## Layout and Dimensions

### Grid Cell Structure

```
┌─────┬─────┬─────┬─────┐
│  ■  │     │  ■  │     │  ← Generation 0 (top row)
├─────┼─────┼─────┼─────┤
│ ■ ■ │ ■   │ ■ ■ │ ■   │  ← Generation 1
├─────┼─────┼─────┼─────┤
│■   ■│■ ■  │■   ■│■ ■  │  ← Generation 2
├─────┼─────┼─────┼─────┤
│  ■  │  ■  │  ■  │  ■  │  ← Generation 3
└─────┴─────┴─────┴─────┘
   ↑      ↑      ↑      ↑
 Cell 0  Cell 1 Cell 2 Cell 3
```

**Dimensions:**
- **Cell Size**: 1-5 pixels per cell (user-adjustable)
- **Grid Width**: 10-200 cells (horizontal, user-adjustable)
- **Grid Height**: 10-200 generations (vertical, user-adjustable)

**Example at `cellSize=2` pixels:**
- 100-cell grid = 200 pixels wide
- 100-generation grid = 200 pixels tall
- Total canvas = 200×200 pixels

**Example at `cellSize=5` pixels:**
- 100-cell grid = 500 pixels wide
- 100-generation grid = 500 pixels tall
- Total canvas = 500×500 pixels

### Aspect Ratio Guidelines

| Aspect Ratio | Grid Dimensions | Use Case |
|--------------|-----------------|----------|
| **1:1** | 100×100 | Balanced, square display |
| **2:1** | 200×100 | Wide patterns, horizontal emphasis |
| **1:2** | 100×200 | Tall patterns, vertical growth |
| **4:1** | 200×50 | Ultra-wide, banner-style |
| **3:2** | 150×100 | Landscape orientation |

**Constraints:**
- Minimum: 10×10 cells
- Maximum: 200×200 cells (or canvas size ÷ cellSize, whichever is smaller)

---

## Visual Examples (Conceptual ASCII Art)

### Example 1: Rule 90 (Sierpinski Triangle) - Light Theme

```
Background: White
Grid: Light gray lines
Cells: Spectral colors (rainbow gradient)

┌───┬───┬───┬───┬───┬───┬───┐
│   │   │   │ R │   │   │   │  Gen 0 (Red)
├───┼───┼───┼───┼───┼───┼───┤
│   │   │ O │   │ O │   │   │  Gen 1 (Orange)
├───┼───┼───┼───┼───┼───┼───┤
│   │ Y │   │   │   │ Y │   │  Gen 2 (Yellow)
├───┼───┼───┼───┼───┼───┼───┤
│ G │   │   │   │   │   │ G │  Gen 3 (Green)
└───┴───┴───┴───┴───┴───┴───┘

Legend:
R = Red cell (Gen 0)
O = Orange cell (Gen 1)
Y = Yellow cell (Gen 2)
G = Green cell (Gen 3)
```

**Visual description:**
- White background provides clean, bright canvas
- Light gray grid lines create subtle cell boundaries
- Colored cells pop vibrantly against white
- Sierpinski triangle pattern emerges

### Example 2: Rule 30 (Chaotic) - Dark Theme

```
Background: Black
Grid: Light gray lines
Cells: Spectral colors (glowing effect)

┌───┬───┬───┬───┬───┬───┬───┐
│   │   │   │ R │   │   │   │  Gen 0 (Red - glowing)
├───┼───┼───┼───┼───┼───┼───┤
│   │   │ O │ O │ O │   │   │  Gen 1 (Orange - glowing)
├───┼───┼───┼───┼───┼───┼───┤
│   │ Y │ Y │   │ Y │ Y │   │  Gen 2 (Yellow - glowing)
├───┼───┼───┼───┼───┼───┼───┤
│ G │ G │   │   │   │ G │ G │  Gen 3 (Green - glowing)
└───┴───┴───┴───┴───┴───┴───┘

Legend:
R = Red cell (glowing against black)
O = Orange cell
Y = Yellow cell
G = Green cell
```

**Visual description:**
- Black background creates dramatic contrast
- Light gray grid lines remain visible
- Colored cells appear to glow with neon-like effect
- Chaotic expansion pattern is highly visible

---

## Rendering Layer Order (Top to Bottom)

```
Layer 4: Grid Overlay (light gray, drawn last)
         ↓
Layer 3: CA Cells (spectral colors)
         ↓
Layer 2: Background Fill (white or black)
         ↓
Layer 1: Pixel Buffer (initialized, empty)
```

**Drawing sequence:**
1. **Initialize** pixel buffer with zeros
2. **Fill** background (theme-aware: white or black)
3. **Draw** CA cells generation-by-generation (colored rectangles)
4. **Overlay** grid lines (if `showGrid == true`)

---

## Grid Visibility Scenarios

### Scenario A: Grid ON, Cell Size 2px

```
Grid: Clearly visible
Cells: Well-defined, easy to count
Use case: Detailed analysis, education
```

### Scenario B: Grid OFF, Cell Size 2px

```
Grid: Hidden
Cells: Smooth appearance, artistic
Use case: Visual aesthetics, presentations
```

### Scenario C: Grid ON, Cell Size 1px

```
Grid: Very dense, may overwhelm
Cells: Tiny individual pixels
Use case: Large patterns, overview display
Recommendation: Consider hiding grid at cellSize=1
```

### Scenario D: Grid ON, Cell Size 5px

```
Grid: Bold, prominent boundaries
Cells: Large, blocky appearance
Use case: Accessibility, projection displays
```

---

## Color Accessibility

### Contrast Checks

| Foreground | Background | Contrast Ratio | WCAG Level |
|------------|------------|----------------|------------|
| Light Gray Grid | White | 3.9:1 | AA (Large Text) |
| Light Gray Grid | Black | 5.3:1 | AA (Normal Text) |
| Spectral Colors | White | Varies (5:1 - 21:1) | AAA (Best) |
| Spectral Colors | Black | Varies (5:1 - 21:1) | AAA (Best) |

**Notes:**
- Grid lines are non-text UI elements (lower contrast requirements)
- CA cells have excellent contrast on both backgrounds
- Users can toggle grid off if visibility is an issue

### Colorblind Considerations

Spectral color mapping uses **hue variation** (not saturation/brightness):
- ✅ Red-green colorblind users: Can distinguish by brightness differences
- ✅ Blue-yellow colorblind users: Can see blue/cyan regions clearly
- ✅ Monochrome vision: Gradient appears as brightness variation

**Potential enhancement (future):**
- Add "brightness mode" where generations map to luminosity instead of hue
- Add texture/pattern overlays for additional differentiation

---

## Parameter UI Display

### Right Panel Layout (Conceptual)

```
┌─────────────────────────────────┐
│ Rule: Rule 90 - Sierpinski      │
├─────────────────────────────────┤
│ Generations: [====●═══] 100     │
│ Cell Size:   [==●═════] 2 px    │
├─────────────────────────────────┤
│ Initial Pattern: [Single Center▼]│
│ Pattern Scale:   [●═══════] 1   │
│ Random Density:  [════●═══] 0.5 │
│ Wrap Edges:      [✓] Toroidal   │
├─────────────────────────────────┤
│ Color Start:  [●═══════] 0°     │
│ Color Stop:   [═══════●] 359°   │
│ Color Cycles: [●═══════] 1      │
├─────────────────────────────────┤
│ Grid Width:   [═════●══] 100    │
│ Grid Height:  [═════●══] 100    │
│ Show Grid:    [✓] Enabled       │
└─────────────────────────────────┘
```

**Categories:**
1. **Rule & Basic Settings** (rule, generations, cellSize)
2. **Initial Conditions** (pattern, scale, density, wrap)
3. **Color Mapping** (start, stop, cycles)
4. **Grid Overlay** (width, height, show)

---

## Implementation Testing Visual Checklist

### ✅ Theme Detection
- [ ] Light theme → White background renders
- [ ] Dark theme → Black background renders
- [ ] Ocean Blue → White background renders (inherits Light)
- [ ] Switch themes during runtime → Background updates

### ✅ Grid Rendering
- [ ] Grid lines are light gray (#C0C0C0)
- [ ] Grid lines are 1 pixel thick
- [ ] Grid spacing matches cell size parameter
- [ ] Grid draws correctly at all cell sizes (1-5 px)
- [ ] Grid toggle works (shows/hides)

### ✅ Color Vibrancy
- [ ] Spectral colors are vibrant on white background
- [ ] Spectral colors appear glowing on black background
- [ ] Grid does not obscure cell colors significantly
- [ ] Color gradient is smooth across generations

### ✅ Dimension Constraints
- [ ] Grid dimensions clamp to canvas size
- [ ] Aspect ratios remain reasonable (1:1 to 4:1)
- [ ] Parameter sliders disable invalid ranges
- [ ] UI displays max grid size based on canvas

---

## Design Rationale

### Why Light Gray for Grid?

| Color | On White | On Black | Decision |
|-------|----------|----------|----------|
| Black | ✅ High contrast | ❌ Invisible | ❌ Dark-theme fails |
| White | ❌ Invisible | ✅ High contrast | ❌ Light-theme fails |
| Medium Gray | ✅ Visible | ✅ Visible | ✅ **Chosen** |
| Dark Gray | ✅ Good | ⚠️ Low contrast | ⚠️ Less visible on dark |
| Light Gray | ⚠️ Low contrast | ✅ Good | ✅ **Best balance** |

**RGB(192, 192, 192) chosen because:**
- Visible on both white (75% luminosity) and black (25% luminosity)
- Subtle enough not to dominate the visual
- Standard "silver" color, familiar to users

### Why Theme-Aware Backgrounds?

1. **Consistency**: Matches application theme (user expectation)
2. **Accessibility**: Respects user's visual preference (light-sensitive users prefer dark)
3. **Aesthetics**: Different moods (clinical/bright vs dramatic/glowing)
4. **Professional**: Shows attention to detail and polish

### Why Adjustable Grid?

1. **Educational**: Users can count cells and generations precisely
2. **Analysis**: Researchers may need specific grid dimensions
3. **Artistic**: Some users prefer smooth look without grid
4. **Flexibility**: Different use cases (teaching vs presentation vs exploration)

---

## Future Enhancements (Not in Current Scope)

### Potential Grid Improvements
- [ ] Grid line thickness adjustment (1-3 pixels)
- [ ] Grid color customization
- [ ] Grid opacity slider (0-100%)
- [ ] Axis labels (generation numbers, cell indices)
- [ ] Major/minor grid lines (every 10th line bolder)

### Potential Theme Improvements
- [ ] Custom background color picker
- [ ] Gradient backgrounds
- [ ] Image/texture backgrounds
- [ ] Per-fractal-type background preferences

---

**Last Updated**: 2025-01-16  
**Maintainer**: Development Team  
**Related**: `CELLULAR_AUTOMATA_IMPLEMENTATION_CHECKLIST.md`, `CA_THEME_AND_GRID_IMPLEMENTATION.md`
