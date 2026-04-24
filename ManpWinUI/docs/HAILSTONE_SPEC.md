# Hailstone Sequence Visualization - WinUI Implementation Specification

**Document Version:** 1.0  
**Target Phase:** Phase 5 - Advanced Features  
**Status:** Planning  
**Last Updated:** 2025-01-XX

---

## 1. Overview

### 1.1 Purpose
Integrate the 2D Hailstone Sequence visualization from ManpWIN64 into the ManpWinUI application, enabling exploration of discrete dynamical systems on the integer lattice with modern graphics export capabilities.

### 1.2 Key Differences from Standard Fractals
- **Discrete vs Continuous:** Operates on integer lattice (ℤ × ℤ), not complex plane (ℂ)
- **Sequential vs Pixel-based:** Visualizes a path/trajectory, not escape-time coloring
- **Rendering Approach:** Line/curve drawing, not pixel array coloring
- **Data Structure:** Ordered sequence of points, not 2D pixel grid

---

## 2. Mathematical Foundation

### 2.1 Transformation Rules

**Current 2D Hailstone (Default)**
Given point (x, y), next point (x', y') determined by parity:

| x parity | y parity | x' formula | y' formula |
|----------|----------|------------|------------|
| even     | even     | x/2        | y/2        |
| even     | odd      | x/2 + 1    | 3y - 1     |
| odd      | even     | 3x - 1     | y/2 - 1    |
| odd      | odd      | 3x + 1     | 3y - 3     |

**Additional Presets** (5 total)
1. Current 2D (above)
2. Simple Collatz (classic conjecture on both dimensions)
3. Symmetric Variant (balanced growth/shrinkage)
4. Coordinate Swap (swaps x/y in odd cases)
5. Bounded Growth (2x instead of 3x expansion)

### 2.2 Sequence Properties
- **Cycle Detection:** Some starting points enter repeating loops
- **Divergence:** Some sequences grow unbounded
- **Convergence:** Some reach stable fixed points
- **Step Count:** Variable (controlled by Max Iterations parameter)

---

## 3. User Interface Requirements

### 3.1 Parameter Panel Additions

**New Section: "Hailstone Parameters"** (visible when Fractal Type = "Hailstone")

```
┌─ Hailstone Parameters ────────────────────┐
│                                            │
│ Starting Point                             │
│   X: [______-10______] (NumberBox)         │
│   Y: [______6_______] (NumberBox)          │
│                                            │
│ Max Iterations: [____150____] (NumberBox)  │
│                                            │
│ Transformation Preset: (ComboBox)          │
│   • Current 2D Hailstone                   │
│   • Simple Collatz                         │
│   • Symmetric Variant                      │
│   • Coordinate Swap                        │
│   • Bounded Growth                         │
│                                            │
│ [Show Grid]     [Show Axes]  (CheckBoxes)  │
│ [Show Cycle]    [Label Points]             │
│                                            │
│ [Render Sequence] (Button)                 │
└────────────────────────────────────────────┘
```

**Key Differences from Standard Parameters:**
- No Center X/Y or Zoom (fixed viewport)
- No color palette (use single color + highlighting)
- No Julia mode
- Integer-only inputs for Start X/Y

### 3.2 Fractal Type Addition

Update ComboBox in MainPage.xaml:
```xml
<ComboBox SelectedItem="{x:Bind ViewModel.SelectedFractalType, Mode=TwoWay}">
    <x:String>Mandelbrot</x:String>
    <x:String>BurningShip</x:String>
    <x:String>Tricorn</x:String>
    <x:String>Phoenix</x:String>
    <x:String>Hailstone</x:String>  <!-- NEW -->
</ComboBox>
```

---

## 4. Rendering Requirements

### 4.1 Visual Design

**Components to Render:**
1. **Background:** White or light gray canvas
2. **Grid:** Optional, integer lattice lines (light gray)
3. **Coordinate Axes:** X and Y axes through origin (black)
4. **Sequence Path:**
   - Line segments connecting consecutive points (blue, 2px width)
   - Points marked as circles (4-6px diameter)
   - Starting point (green circle, 8px diameter)
   - Ending point (red circle, 8px diameter if diverged, orange if cycle)
5. **Cycle Highlighting:** If cycle detected, highlight repeating section (yellow background or dashed line)
6. **Labels:** Optional point coordinates (small text)

**Color Scheme:**
```
- Path line: #2196F3 (Material Blue)
- Points: #1976D2 (Darker Blue)
- Start point: #4CAF50 (Green)
- End point (diverged): #F44336 (Red)
- End point (cycle): #FF9800 (Orange)
- Cycle highlight: #FFEB3B40 (Yellow, 25% opacity)
- Grid: #E0E0E0 (Light Gray)
- Axes: #000000 (Black)
```

### 4.2 Viewport Management

**Auto-scaling Algorithm:**
1. Calculate bounding box of all sequence points: (minX, minY, maxX, maxY)
2. Add 10% padding on all sides
3. Scale to fit canvas dimensions (maintain aspect ratio)
4. Center the sequence in viewport

**Zoom/Pan Behavior:**
- Mouse wheel: Zoom in/out (fixed center)
- Right-click drag: Pan viewport
- Double-click: Reset to auto-scale
- No zoom-to-rectangle (not applicable for discrete sequences)

### 4.3 Canvas Implementation Options

**Option A: WriteableBitmap (Current approach)**
- ✅ Consistent with existing fractal rendering
- ✅ Export to PNG/JPEG works immediately
- ❌ Rasterized output (pixelated when zoomed)
- ❌ No vector export (SVG)

**Option B: Win2D CanvasControl (Recommended)**
- ✅ High-quality vector rendering
- ✅ Hardware-accelerated
- ✅ Can export to SVG via vector commands
- ✅ Smooth lines and anti-aliasing
- ❌ Requires additional NuGet package (Microsoft.Graphics.Win2D)
- ❌ Different rendering pipeline

**Option C: Hybrid Approach**
- Render to WriteableBitmap for preview
- Provide "Export to SVG" option that regenerates as vector

---

## 5. Data Export Formats

### 5.1 Bitmap Export (PNG/JPEG)

**Requirements:**
- Reuse existing `ImageExportService`
- High-resolution rendering option (2x, 4x scale)
- Transparent background option for PNG

### 5.2 SVG Export (Vector Graphics)

**Target Format:**
```xml
<svg xmlns="http://www.w3.org/2000/svg" 
     width="800" height="600" 
     viewBox="-50 -50 100 100">

  <!-- Grid (optional) -->
  <g id="grid" stroke="#e0e0e0" stroke-width="0.1">
    <line x1="-50" y1="0" x2="50" y2="0"/>
    <line x1="0" y1="-50" x2="0" y2="50"/>
    <!-- More grid lines -->
  </g>

  <!-- Sequence path -->
  <polyline points="-10,6 -5,3 -2,1 ..." 
            fill="none" 
            stroke="#2196F3" 
            stroke-width="0.5"/>

  <!-- Points -->
  <circle cx="-10" cy="6" r="1" fill="#4CAF50"/> <!-- Start -->
  <circle cx="-5" cy="3" r="0.4" fill="#1976D2"/>
  <!-- More points -->
  <circle cx="0" cy="0" r="1" fill="#F44336"/> <!-- End -->

  <!-- Labels (optional) -->
  <text x="-10" y="7" font-size="2">(-10, 6)</text>
</svg>
```

**SVG Export Service:**
- Create `SvgExportService` class
- Method: `ExportHailstoneToSvg(List<HailstonePoint> sequence, HailstoneRenderOptions options, string filePath)`
- Use System.Xml.Linq for XML generation

### 5.3 CSV Export (Data)

**Format:**
```csv
Step,X,Y,IsCycle
0,-10,6,false
1,-5,3,false
2,-2,1,false
...
47,1,0,true
48,1,0,true
```

**Use Cases:**
- Mathematical analysis in Excel/Python
- Sharing sequences without images
- Reproducible research

---

## 6. Technical Integration

### 6.1 ViewModel Extensions

**MainViewModel.cs additions:**
```csharp
// Hailstone-specific properties
[ObservableProperty]
public partial int HailstoneStartX { get; set; } = -10;

[ObservableProperty]
public partial int HailstoneStartY { get; set; } = 6;

[ObservableProperty]
public partial int HailstoneMaxIterations { get; set; } = 150;

[ObservableProperty]
public partial string SelectedHailstonePreset { get; set; } = "Current 2D";

[ObservableProperty]
public partial bool ShowHailstoneGrid { get; set; } = true;

[ObservableProperty]
public partial bool ShowHailstoneLabels { get; set; } = false;

// Computed property
public bool IsHailstoneMode => SelectedFractalType == "Hailstone";

// Command
[RelayCommand]
private async Task RenderHailstoneAsync() { /* ... */ }
```

### 6.2 ManpCore.Native Integration

**New Files Required:**
1. `ManpCore.Native\HailstoneCalculator.h`
2. `ManpCore.Native\HailstoneCalculator.cpp`
3. `ManpCore.Native\HailstoneFamily.cpp` (registry)

**Interface:**
```cpp
// Result structure
public ref class HailstoneResult
{
public:
    array<int>^ X;           // X coordinates
    array<int>^ Y;           // Y coordinates
    int StepCount;           // Number of points
    bool HasCycle;           // Cycle detected?
    int CycleStart;          // Index where cycle begins (-1 if no cycle)
    int CycleLength;         // Length of cycle
};

// Calculator method
HailstoneResult^ CalculateHailstoneSequence(
    int startX, 
    int startY, 
    int maxIterations, 
    HailstonePreset preset
);
```

### 6.3 Service Layer

**New Service: `IHailstoneRenderService`**
```csharp
public interface IHailstoneRenderService
{
    Task<HailstoneRenderResult> RenderSequenceAsync(
        int startX,
        int startY,
        int maxIterations,
        HailstonePreset preset,
        HailstoneRenderOptions options,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default
    );
}
```

**HailstoneRenderResult:**
```csharp
public class HailstoneRenderResult
{
    public List<HailstonePoint> Sequence { get; set; }
    public WriteableBitmap? Image { get; set; }  // For bitmap rendering
    public bool HasCycle { get; set; }
    public int CycleStartIndex { get; set; }
    public int CycleLength { get; set; }
    public TimeSpan RenderTime { get; set; }
}

public class HailstonePoint
{
    public int Step { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsInCycle { get; set; }
}
```

---

## 7. Implementation Phases

### Phase 1: Core Integration (2-3 hours)
- [ ] Add Hailstone to fractal type ComboBox
- [ ] Create HailstoneCalculator in ManpCore.Native
- [ ] Implement basic sequence calculation
- [ ] Add Hailstone parameters to MainViewModel
- [ ] Show/hide Hailstone parameter panel based on fractal type

### Phase 2: Basic Rendering (3-4 hours)
- [ ] Implement WriteableBitmap rendering
- [ ] Auto-scaling viewport algorithm
- [ ] Draw line segments and points
- [ ] Highlight start/end points
- [ ] Basic color scheme

### Phase 3: Advanced Rendering (2-3 hours)
- [ ] Grid overlay
- [ ] Coordinate axes
- [ ] Cycle detection and highlighting
- [ ] Point labels (optional)
- [ ] Zoom/pan controls

### Phase 4: Export Features (3-4 hours)
- [ ] High-res bitmap export (reuse ImageExportService)
- [ ] Implement SvgExportService
- [ ] SVG export with all visual elements
- [ ] CSV data export
- [ ] Add export menu options

### Phase 5: Polish (1-2 hours)
- [ ] Preset bookmarks for interesting sequences
- [ ] Tooltips and help text
- [ ] Error handling (overflow, divergence)
- [ ] Performance optimization
- [ ] Unit tests

**Total Estimated Time:** 11-16 hours

---

## 8. Open Questions & Decisions Needed

### 8.1 Rendering Technology
**Decision:** WriteableBitmap vs Win2D vs Hybrid?
- **Recommendation:** Start with WriteableBitmap (Phase 1-3), add Win2D/SVG later (Phase 4)
- **Rationale:** Faster initial implementation, consistent with current codebase

### 8.2 Viewport Controls
**Decision:** Should Hailstone support same mouse controls as standard fractals?
- Zoom-to-rectangle: **No** (not meaningful for discrete sequences)
- Pan: **Yes** (right-click drag)
- Mouse wheel zoom: **Yes** (with fixed center)
- Arrow key pan: **Yes**

### 8.3 Performance Limits
**Decision:** Maximum iterations before warning user?
- **Recommendation:** 10,000 iterations (reasonable for most sequences)
- Show progress bar for long calculations (>1000 iterations)

### 8.4 Cycle Detection Algorithm
**Decision:** Use hash set or array-based detection?
- **Recommendation:** Hash set of (x, y) pairs (O(1) lookup)
- Stop immediately when cycle detected

---

## 9. Testing Plan

### 9.1 Known Interesting Sequences

**Test Cases:**
```
1. Classic Example: (-10, 6) → Known cycle
2. Origin: (0, 0) → Fixed point
3. Large Initial: (100, 100) → Divergence test
4. Negative: (-20, -30) → Sign handling
5. Asymmetric: (50, -25) → Mixed behavior
```

### 9.2 Edge Cases
- [ ] Integer overflow (x or y > INT_MAX)
- [ ] Very long sequences (>10,000 steps)
- [ ] Immediate cycle (starting point is in cycle)
- [ ] Single-point cycle (fixed point)

### 9.3 Rendering Validation
- [ ] Very small sequences (2-3 points)
- [ ] Very large bounding boxes
- [ ] Aspect ratio extremes (tall vs wide)
- [ ] Export quality at multiple resolutions

---

## 10. Documentation Requirements

### 10.1 User Documentation
- [ ] Add Hailstone section to main README
- [ ] Tutorial: "Exploring Hailstone Sequences"
- [ ] Preset gallery with screenshots
- [ ] Mathematical background explanation

### 10.2 Developer Documentation
- [ ] API documentation for HailstoneCalculator
- [ ] SVG export format specification
- [ ] Performance benchmarks
- [ ] Extension points for custom presets

---

## 11. Future Enhancements (Post-v1.0)

### 11.1 Advanced Visualization
- [ ] 3D trajectory view (step as Z-axis)
- [ ] Heat map of visited regions
- [ ] Animation along sequence path
- [ ] Multiple sequences overlaid

### 11.2 Analysis Tools
- [ ] Cycle length histogram
- [ ] Divergence rate analysis
- [ ] Starting point heatmap (which points cycle/diverge)
- [ ] Statistical properties export

### 11.3 Interactive Exploration
- [ ] Click canvas to set new starting point
- [ ] Brush tool to try multiple starting points
- [ ] "Nearby sequences" comparison
- [ ] Parameter sweeps (animate through start points)

---

## 12. References

### 12.1 Existing Implementation
- **C++ Source:** `ManpWIN64\Hailstone.h`, `ManpWIN64\Hailstone.cpp`
- **Documentation:** `docs\HAILSTONE.md` (user guide)
- **Dialog Code:** `ManpWIN64\ManpDlg.cpp` (dialog handling)

### 12.2 Mathematical Resources
- Collatz Conjecture (3n+1 problem)
- Discrete Dynamical Systems
- Integer Lattice Structures

### 12.3 Graphics References
- Win2D Documentation: https://microsoft.github.io/Win2D/
- SVG Specification: https://www.w3.org/TR/SVG2/
- WinUI CanvasControl: https://learn.microsoft.com/windows/windows-app-sdk/

---

## Appendix A: Sample Code Snippets

### A.1 Hailstone Calculation (C#)
```csharp
public class HailstoneSequence
{
    public static List<HailstonePoint> Calculate(int startX, int startY, int maxIterations)
    {
        var sequence = new List<HailstonePoint>();
        var visited = new HashSet<(int, int)>();

        int x = startX, y = startY;

        for (int step = 0; step < maxIterations; step++)
        {
            // Check for cycle
            if (visited.Contains((x, y)))
            {
                // Mark cycle points
                break;
            }

            visited.Add((x, y));
            sequence.Add(new HailstonePoint { Step = step, X = x, Y = y });

            // Apply transformation (Current 2D preset)
            bool xEven = (x % 2 == 0);
            bool yEven = (y % 2 == 0);

            (x, y) = (xEven, yEven) switch
            {
                (true, true)   => (x / 2, y / 2),
                (true, false)  => (x / 2 + 1, 3 * y - 1),
                (false, true)  => (3 * x - 1, y / 2 - 1),
                (false, false) => (3 * x + 1, 3 * y - 3)
            };
        }

        return sequence;
    }
}
```

### A.2 SVG Export (C#)
```csharp
public class SvgExporter
{
    public void ExportToSvg(List<HailstonePoint> sequence, string filePath)
    {
        var (minX, minY, maxX, maxY) = GetBoundingBox(sequence);

        var svg = new XDocument(
            new XElement("svg",
                new XAttribute("xmlns", "http://www.w3.org/2000/svg"),
                new XAttribute("viewBox", $"{minX} {minY} {maxX - minX} {maxY - minY}"),

                // Path
                new XElement("polyline",
                    new XAttribute("points", string.Join(" ", 
                        sequence.Select(p => $"{p.X},{p.Y}"))),
                    new XAttribute("fill", "none"),
                    new XAttribute("stroke", "#2196F3"),
                    new XAttribute("stroke-width", "0.5")
                ),

                // Points (circles)
                sequence.Select(p => new XElement("circle",
                    new XAttribute("cx", p.X),
                    new XAttribute("cy", p.Y),
                    new XAttribute("r", p.Step == 0 ? 1 : 0.4),
                    new XAttribute("fill", p.Step == 0 ? "#4CAF50" : "#1976D2")
                ))
            )
        );

        svg.Save(filePath);
    }
}
```

---

## Document Status

**Prepared By:** GitHub Copilot  
**Review Status:** Draft  
**Approval Required From:** Mark (Product Owner)  
**Target Implementation:** Phase 5

**Next Steps:**
1. Review and approve this specification
2. Create GitHub issue/milestone for Hailstone feature
3. Break down into development tasks
4. Begin Phase 1 implementation
