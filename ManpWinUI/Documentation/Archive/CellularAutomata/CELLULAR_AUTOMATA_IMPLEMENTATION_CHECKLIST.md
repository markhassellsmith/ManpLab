# Cellular Automata Implementation Checklist

**Feature**: 1D Elementary Cellular Automata with Pattern Browser and Spectral Coloring  
**Branch**: `feature/cellular-automata`  
**Status**: 🚧 In Progress  
**Target .NET**: .NET 10  
**Architecture**: Special-rendering pipeline (managed C#), similar to L-Systems

---

## Overview

This checklist tracks the implementation of Elementary Cellular Automata as a new fractal category. The feature includes:
- 20 named CA rules (Wolfram numbering 0-255) with descriptive names
- 15 tiled initial patterns (checkerboards, stripes, motifs, random)
- Full 360° spectral color mapping with start/stop/cycles controls
- Pattern browser UI (left panel) + configuration panel (right panel)
- Wrap-around (toroidal) edge handling
- Pattern scale and density controls
- **Theme-aware backgrounds** (white for Light theme, black for Dark theme)
- **Adjustable light gray grid overlay** with user-configurable dimensions
- **Aspect ratio constraints** to ensure grid fits within application canvas
- **Real-time statistics display** on status bar (generation counter, alive cells, proportion, cell lifetimes)
- **Dynamic toolbar button** (Render for fractals, Start/Stop for CA) with keyboard shortcuts

---

## Phase 1: Native C++ Layer (Foundation)

**Goal**: Register CA families in native engine and route to managed rendering

- [ ] **1.1** Add `CellularAutomaton = 4` to `FractalCategory` enum in `ManpCore.Native/FractalRegistry.h`
- [ ] **1.2** Add `CellularAutomaton = 4` to managed enum in `ManpCore.Native/FractalEngineWrapper.h`
- [ ] **1.3** Create `ManpCore.Native/CellularAutomatonFamily.cpp` with 20 CA family registrations
- [ ] **1.4** Add forward declaration `void RegisterCellularAutomatonFamily();` in `FractalRegistry.cpp`
- [ ] **1.5** Call `RegisterCellularAutomatonFamily();` in `InitializeFractalRegistry()` in `FractalRegistry.cpp`
- [ ] **1.6** Add routing for `FractalCategory::CellularAutomaton` in `FractalEngineWrapper.cpp::RenderFractal()`
- [ ] **1.7** Add `CellularAutomatonFamily.cpp` to `ManpCore.Native.vcxproj` compile list
- [ ] **1.8** Build native project and verify no errors

**Verification**: Native project builds successfully; no linker errors

---

## Phase 2: Managed C# Model Layer (Data & Logic)

**Goal**: Create data models for rules, presets, and pattern initialization

- [ ] **2.1** Create folder `ManpWinUI/Models/CellularAutomata/`
- [ ] **2.2** Create `CARule.cs` with Wolfram lookup table generation
- [ ] **2.3** Create `CAPresets.cs` with all 20 named rule presets
- [ ] **2.4** Create `CAInitializer.cs` with 15 pattern generation methods
- [ ] **2.5** Test pattern generation in isolation (unit test or console app)
- [ ] **2.6** Verify all patterns produce valid bool arrays

**Verification**: All patterns generate without exceptions; visual spot-check of simple patterns

---

## Phase 3: Rendering Service Layer (Core Logic)

**Goal**: Implement CA simulation with spectral coloring, theme-aware backgrounds, grid overlay, and statistics tracking

- [ ] **3.1** Create `ManpWinUI/Services/CellularAutomatonRenderService.cs`
- [ ] **3.2** Implement `RenderElementaryCA()` method with generation loop
- [ ] **3.3** Implement `ApplyRule()` method with wrap-around support
- [ ] **3.4** Implement `GenerationToSpectralColor()` with 360° hue mapping
- [ ] **3.5** Implement `HsvToRgb()` color conversion
- [ ] **3.6** Implement `DrawCell()` pixel buffer writing
- [ ] **3.7** Implement `GetThemeBackgroundColor()` to detect Light/Dark theme (white/black)
- [ ] **3.8** Implement `FillBackground()` to apply theme-aware background color
- [ ] **3.9** Implement `DrawGrid()` to render light gray grid overlay
- [ ] **3.10** Add grid parameter support (gridWidth, gridHeight, showGrid)
- [ ] **3.11** Implement `CAStatistics` class to track cell lifetimes and compute metrics
- [ ] **3.12** Implement `TrackCellLifetimes()` to monitor when cells are born/die
- [ ] **3.13** Implement `ComputeStatistics()` to calculate alive count, proportion, avg/max lifetime
- [ ] **3.14** Add debug logging for render parameters
- [ ] **3.15** Test service with simple rule (e.g., Rule 90, Single Center)

**Verification**: Rule 90 with Single Center produces Sierpinski triangle pattern on theme-appropriate background with grid and accurate statistics

---

## Phase 4: Parameter System Integration (Right Panel)

**Goal**: Integrate CA parameters into flexible parameter system

- [ ] **4.1** Add `CreateCATemplate()` method to `FractalParameterService.cs`
- [ ] **4.2** Add all 13 CA parameters:
  - `rule` (string, read-only display)
  - `generations` (integer, 50-500)
  - `cellSize` (integer, 1-5)
  - `wrapEdges` (boolean)
  - `initialPattern` (choice, 15 options)
  - `patternScale` (integer, 1-10)
  - `randomDensity` (double, 0.0-1.0)
  - `colorStart/colorStop/colorCycles` (integers)
  - `gridWidth` (integer, 10-200, cells per row)
  - `gridHeight` (integer, 10-200, total generations)
  - `showGrid` (boolean, toggle grid overlay)
- [ ] **4.3** Register all 20 CA families in `GetParameterSet()` switch
- [ ] **4.4** Test parameter loading: Select a CA → Right panel populates
- [ ] **4.5** Verify parameter defaults load correctly
- [ ] **4.6** Verify "(tab out to accept)" hints appear on numeric fields
- [ ] **4.7** Verify grid parameters constrain to maintain reasonable aspect ratios

**Verification**: Selecting any CA rule loads 13 parameters in right panel with correct defaults

---

## Phase 5: Main Render Service Integration (Routing)

**Goal**: Route CA rendering through main fractal render service with theme awareness

- [ ] **5.1** Add `RenderCellularAutomatonAsync()` method to `FractalRenderService.cs`
- [ ] **5.2** Add CA category check in `RenderFractalAsync()` (after L-System check)
- [ ] **5.3** Extract parameters from `ExtendedParameters` dictionary
- [ ] **5.4** Pass current theme (Light/Dark) from MainPage or App to render service
- [ ] **5.5** Call `CellularAutomatonRenderService.RenderElementaryCA()` with theme parameter
- [ ] **5.6** Return `RenderResult` with pixel data
- [ ] **5.7** Add debug logging for parameter extraction and theme detection
- [ ] **5.8** Test end-to-end: Native → Wrapper → Service → Renderer → Display

**Verification**: Selecting a CA rule from fractal browser triggers render; image displays on canvas with correct theme background and grid

---

## Phase 6: Pattern Browser UI (Left Panel)

**Goal**: Create dedicated CA pattern browser with thumbnails

- [ ] **6.1** Create folder `ManpWinUI/Views/CellularAutomata/`
- [ ] **6.2** Create `CAPatternBrowserView.xaml` (copy structure from `FractalBrowserView.xaml`)
- [ ] **6.3** Create `CAPatternBrowserView.xaml.cs` code-behind
- [ ] **6.4** Add `ItemsControl` with rule thumbnails
- [ ] **6.5** Add search/filter `TextBox` at top
- [ ] **6.6** Add selection highlight visual state
- [ ] **6.7** Create `CAPatternBrowserViewModel.cs` in `ManpWinUI/ViewModels/CellularAutomata/`
- [ ] **6.8** Implement `ObservableCollection<CARule> Patterns`
- [ ] **6.9** Implement `SelectedPattern` property with `INotifyPropertyChanged`
- [ ] **6.10** Implement `PatternSelected` event with custom event args
- [ ] **6.11** Implement search/filter logic
- [ ] **6.12** Test browser: Displays all 20 rules, selection works

**Verification**: CA browser displays 20 rules; clicking a rule fires selection event

---

## Phase 7: Thumbnail Generation (Visual Polish)

**Goal**: Generate small preview images for each CA rule

- [ ] **7.1** Add `ThumbnailImage` property to `CARule` class
- [ ] **7.2** Implement `GenerateThumbnailsAsync()` in `CAPatternBrowserViewModel`
- [ ] **7.3** Implement `RenderThumbnailAsync()` (200x50px, default settings)
- [ ] **7.4** Call thumbnail generation on ViewModel initialization
- [ ] **7.5** Bind thumbnails to `ItemsControl` in XAML
- [ ] **7.6** Test: All rules show visual previews

**Verification**: Each rule in browser shows a small rendered preview image

---

## Phase 8: MainPage Integration (Wiring)

**Goal**: Connect CA browser to MainPage event handling

- [ ] **8.1** Add `CAPatternBrowserViewModel` property to `MainPage.cs`
- [ ] **8.2** Inject `CAPatternBrowserViewModel` from DI container in constructor
- [ ] **8.3** Set `CABrowserView.ViewModel` and `DataContext`
- [ ] **8.4** Subscribe to `CABrowserViewModel.PatternSelected` event
- [ ] **8.5** Implement `OnCAPatternSelected()` handler
- [ ] **8.6** Load CA parameters into right panel (`ParameterEditorViewModel.LoadFromParameterSet()`)
- [ ] **8.7** Trigger auto-render on pattern selection
- [ ] **8.8** Test: Select CA pattern → Parameters load → Render executes

**Verification**: Clicking a CA pattern in browser loads parameters and renders automatically

---

## Phase 9: MainPage Layout (UI Structure)

**Goal**: Add CA browser panel, mode toggle, and dynamic toolbar buttons to MainPage

- [ ] **9.1** Add `CABrowserPanel` Grid to `MainPage.xaml` (Grid.Column="0")
- [ ] **9.2** Add `CAPatternBrowserView` inside panel
- [ ] **9.3** Set initial `Visibility="Collapsed"` for CA panel
- [ ] **9.4** Add mode toggle buttons to top menu bar ("Fractals" / "Cellular Automata")
- [ ] **9.5** Add CA Start/Stop button to toolbar (initially collapsed)
- [ ] **9.6** Implement `ShowFractalBrowser()` method (show fractal panel, hide CA panel, show Render button, hide Start/Stop)
- [ ] **9.7** Implement `ShowCABrowser()` method (show CA panel, hide fractal panel, hide Render button, show Start/Stop)
- [ ] **9.8** Wire up toggle button click events
- [ ] **9.9** Test: Toggle between fractal and CA modes → Correct buttons visible
- [ ] **9.10** Verify Render button shows for fractals, Start/Stop shows for CA

**Verification**: Toggling mode shows/hides correct browser panel and toolbar buttons; canvas remains visible

---

## Phase 10: Animation Control (Start/Stop/Pause)

**Goal**: Implement CA animation with Start/Stop/Pause controls and keyboard shortcuts

- [ ] **10.1** Add `CASimulationState` enum to MainViewModel (Stopped, Running, Paused)
- [ ] **10.2** Add `IsCASimulationRunning` property to MainViewModel (for button state)
- [ ] **10.3** Implement `StartCASimulationCommand` in MainViewModel
- [ ] **10.4** Implement `StopCASimulationCommand` in MainViewModel
- [ ] **10.5** Implement `PauseCASimulationCommand` in MainViewModel (optional)
- [ ] **10.6** Create `RunCASimulationAsync()` method with generation-by-generation loop
- [ ] **10.7** Add `CancellationTokenSource` for stopping simulation
- [ ] **10.8** Implement frame rate control (e.g., 10-60 fps adjustable)
- [ ] **10.9** Update canvas incrementally (draw one generation per frame)
- [ ] **10.10** Update statistics display in real-time during simulation
- [ ] **10.11** Add keyboard shortcut: Ctrl+R or Space for Start/Stop toggle
- [ ] **10.12** Add keyboard shortcut: P for Pause/Resume
- [ ] **10.13** Ensure simulation stops when switching fractals or closing app
- [ ] **10.14** Test: Start simulation → Generations animate smoothly
- [ ] **10.15** Test: Stop simulation → Animation stops, final state persists
- [ ] **10.16** Test: Keyboard shortcuts work correctly

**Verification**: CA simulation animates generation-by-generation; Start/Stop buttons and keyboard shortcuts function correctly

---

## Phase 11: DI Registration (Dependency Injection)

**Goal**: Register CA services in DI container

- [ ] **11.1** Register `CAPatternBrowserViewModel` as singleton in `App.xaml.cs`
- [ ] **11.2** Register `CellularAutomatonRenderService` as singleton (if needed by DI)
- [ ] **11.3** Verify DI container resolves all dependencies
- [ ] **11.4** Test: App starts without DI errors

**Verification**: App launches without DI resolution exceptions

---

## Phase 12: Statistics Display Integration (Status Bar)

**Goal**: Display real-time CA statistics on the status bar

- [ ] **11.1** Create `ManpWinUI/Models/CellularAutomata/CAStatistics.cs` data model
- [ ] **11.2** Add statistics properties to model:
  - `Generation` (int) - Current generation number (0, 1, 2, ...)
  - `AliveCells` (int) - Count of currently alive cells
  - `TotalCells` (int) - Total grid capacity (gridWidth × gridHeight)
  - `AliveProportionality` (double) - Ratio of alive/total (0.0-1.0)
  - `AverageLifetime` (double) - Mean generations cells have been alive
  - `LongestLifetime` (int) - Maximum generations any cell has been continuously alive
- [ ] **11.3** Update `CellularAutomatonRenderService` to return `CAStatistics` with pixel data
- [ ] **11.4** Add `CAStatistics` property to `MainViewModel`
- [ ] **11.5** Update `FractalRenderService` to propagate statistics from CA renderer
- [ ] **11.6** Add statistics display to `MainPage.xaml` status bar (Grid.Column="2")
- [ ] **11.7** Create compact statistics format: "Gen: 42 | Alive: 157/400 (39%) | Avg Life: 8.3 | Max Life: 25"
- [ ] **11.8** Add visibility binding to show only when CA is active
- [ ] **11.9** Test: Render CA → Statistics appear and update correctly
- [ ] **11.10** Verify statistics reset when switching fractals

**Verification**: CA statistics display on status bar with accurate real-time metrics

---

## Phase 13: Parameter Synchronization (Data Flow)

**Goal**: Ensure parameter changes flow correctly UI → ParameterSet → Renderer

- [ ] **13.1** Verify `OnParameterChanged()` syncs CA parameters to `CurrentParameters`
- [ ] **13.2** Verify `SyncParameterToSystem()` handles all CA parameter types
- [ ] **13.3** Verify "Random" pattern shows/hides density slider dynamically
- [ ] **13.4** Verify tab-out triggers parameter sync
- [ ] **13.5** Verify render button applies changes
- [ ] **13.6** Test: Change generations → Tab out → Render → See difference

**Verification**: Editing any parameter and clicking Render updates the displayed CA pattern

---

## Phase 14: Testing & Validation

**Goal**: Comprehensive functional testing of all CA features

### Core Functionality
- [ ] **14.1** Test Rule 90 (Sierpinski Triangle) with "Single Center" → See triangular pattern
- [ ] **14.2** Test Rule 30 (Chaotic Growth) with "Single Center" → See random expansion
- [ ] **14.3** Test Rule 110 (Turing Complete) with "Random Medium" → See complex patterns
- [ ] **14.4** Test "Checkerboard 2x2" with Rule 90 → See tiled triangles
- [ ] **14.5** Test "Diagonal Stripes" with any rule → See collision patterns

### Animation Control
- [ ] **14.6** Test Start button → Simulation begins animating
- [ ] **14.7** Test Stop button → Animation stops cleanly
- [ ] **14.8** Test Pause button → Animation freezes at current generation
- [ ] **14.9** Test keyboard shortcuts (Ctrl+R, Space, P) → Commands execute
- [ ] **14.10** Test switching fractals during simulation → CA stops cleanly

### Parameter Controls
- [ ] **14.11** Test wrap edges ON vs OFF → See boundary behavior difference
- [ ] **14.12** Test color range 0°→359° → See full rainbow gradient
- [ ] **14.13** Test color range 240°→60° → See blue→red wrap (circular)
- [ ] **14.14** Test color cycles = 2 → See gradient repeat twice across generations
- [ ] **14.15** Test pattern scale slider → See tile size change
- [ ] **14.16** Test random density slider → See density variation
- [ ] **14.17** Test grid width/height sliders → See grid dimension changes
- [ ] **14.18** Test showGrid toggle → Grid appears/disappears
- [ ] **14.19** Test theme switching (Light/Dark) → Background changes to white/black
- [ ] **14.20** Verify grid remains light gray and visible on both themes

### Statistics Display
- [ ] **14.21** Verify generation counter increments correctly (0, 1, 2, ...)
- [ ] **14.22** Verify alive cell count is accurate
- [ ] **14.23** Verify proportion calculation is correct (alive/total)
- [ ] **14.24** Verify average lifetime tracks correctly across generations
- [ ] **14.25** Verify longest lifetime identifies longest-lived cells
- [ ] **14.26** Test statistics with Rule 30 (chaotic) → Varying statistics
- [ ] **14.27** Test statistics with Rule 90 (deterministic) → Predictable patterns
- [ ] **14.28** Verify statistics reset when switching to different rule
- [ ] **14.29** Verify statistics disappear when switching to non-CA fractal

### Browser & UI
- [ ] **14.30** Test all 20 rules load and render without errors
- [ ] **14.31** Test search/filter in browser → Rules filter correctly by name
- [ ] **14.32** Test mode toggle → Switch between fractals and CA smoothly
- [ ] **14.33** Test thumbnail generation → All rules show correct preview images
- [ ] **14.34** Verify grid aspect ratios stay reasonable when adjusting dimensions
- [ ] **14.35** Verify correct button shows for each mode (Render vs Start/Stop)

**Verification**: All test cases pass; no crashes or visual artifacts; theme-aware backgrounds work correctly; statistics are accurate; animation controls function properly

---

## Phase 15: Polish & Documentation

**Goal**: Add documentation, error handling, and UI polish

- [ ] **15.1** Add XML documentation comments to all new classes
- [ ] **15.2** Add debug logging throughout CA pipeline
- [ ] **15.3** Add error handling for invalid parameters
- [ ] **15.4** Add loading indicator during thumbnail generation
- [ ] **15.5** Add tooltips to all CA parameter controls
- [ ] **15.6** Create `CELLULAR_AUTOMATA_FEATURE.md` user documentation
- [ ] **15.7** Update main README with CA feature description
- [ ] **15.8** Add screenshots of example CA patterns to documentation

**Verification**: All code is documented; user documentation is complete

---

## Phase 16: Build & Commit

**Goal**: Final validation and source control

- [ ] **16.1** Full solution rebuild (Debug)
- [ ] **16.2** Full solution rebuild (Release)
- [ ] **16.3** Fix all compiler warnings
- [ ] **16.4** Run app and smoke test all features
- [ ] **16.5** Git add all new files
- [ ] **16.6** Git commit with message: "Implement 1D Elementary Cellular Automata with pattern browser, spectral coloring, animation controls, and real-time statistics"
- [ ] **16.7** Git push to `origin/feature/cellular-automata`
- [ ] **16.8** Create pull request to merge into `development`

**Verification**: Clean build with no warnings; all features working; code pushed to remote

---

## Summary Statistics

- **Total Tasks**: 129 (up from 113)
- **Estimated Time**: 12-16 hours
- **Files Created**: ~16
- **Files Modified**: ~11
- **Lines of Code**: ~3,500

---

## Key Files Reference

### Files to Create (16 new)

| # | File Path | Purpose |
|---|-----------|---------|
| 1 | `ManpCore.Native/CellularAutomatonFamily.cpp` | Native CA family registration |
| 2 | `ManpWinUI/Models/CellularAutomata/CARule.cs` | Rule model with lookup table |
| 3 | `ManpWinUI/Models/CellularAutomata/CAPresets.cs` | 20 named rule presets |
| 4 | `ManpWinUI/Models/CellularAutomata/CAInitializer.cs` | 15 pattern generators |
| 5 | `ManpWinUI/Models/CellularAutomata/CAStatistics.cs` | Real-time statistics model |
| 6 | `ManpWinUI/Services/CellularAutomatonRenderService.cs` | Core CA simulation & rendering |
| 7 | `ManpWinUI/Views/CellularAutomata/CAPatternBrowserView.xaml` | Browser UI layout |
| 8 | `ManpWinUI/Views/CellularAutomata/CAPatternBrowserView.xaml.cs` | Browser code-behind |
| 9 | `ManpWinUI/ViewModels/CellularAutomata/CAPatternBrowserViewModel.cs` | Browser view model |
| 10 | `ManpWinUI/ViewModels/CellularAutomata/CAPatternSelectedEventArgs.cs` | Event args for selection |
| 11-16 | Documentation files | Feature docs, screenshots |

### Files to Modify (11 existing)

| # | File Path | Changes |
|---|-----------|---------|
| 1 | `ManpCore.Native/FractalRegistry.h` | Add `CellularAutomaton` enum value |
| 2 | `ManpCore.Native/FractalRegistry.cpp` | Call `RegisterCellularAutomatonFamily()` |
| 3 | `ManpCore.Native/FractalEngineWrapper.h` | Add managed enum value |
| 4 | `ManpCore.Native/FractalEngineWrapper.cpp` | Route CA category to managed |
| 5 | `ManpCore.Native/ManpCore.Native.vcxproj` | Add new .cpp to compile list |
| 6 | `ManpWinUI/Services/FractalParameterService.cs` | Add `CreateCATemplate()` |
| 7 | `ManpWinUI/Services/FractalRenderService.cs` | Add `RenderCellularAutomatonAsync()` |
| 8 | `ManpWinUI/Views/MainPage.xaml` | Add CA browser panel, statistics display, and dynamic buttons |
| 9 | `ManpWinUI/Views/MainPage.cs` | Add CA browser integration, statistics binding, and mode switching |
| 10 | `ManpWinUI/ViewModels/MainViewModel.cs` | Add `CAStatistics` property and simulation state |
| 11 | `ManpWinUI/ViewModels/MainViewModel.Commands.cs` | Add CA simulation commands (Start/Stop/Pause) |
| 12 | `ManpWinUI/Views/MainPage.KeyboardHandling.cs` | Add keyboard shortcuts for CA simulation |
| 13 | `ManpWinUI/App.xaml.cs` | Register CA services in DI |

---

## Implementation Strategy

### Recommended Order

1. **Backend First** (Phases 1-3): Get CA logic working without UI
   - Build native registration
   - Create models and rendering service
   - Test with console output or minimal UI

2. **Integration** (Phases 4-5): Connect to existing parameter system
   - Add CA to parameter service
   - Wire into main render service
   - Test with manual fractal selection

3. **UI Last** (Phases 6-10): Add beautiful browsing experience and animation controls
   - Build pattern browser
   - Generate thumbnails
   - Add mode toggle
   - Implement Start/Stop/Pause commands

4. **Polish** (Phases 11-16): Testing, docs, and release
   - Comprehensive testing
   - Documentation
   - Clean commit

### Daily Goals (if working full-time)

- **Day 1**: Phases 1-3 (Backend + Rendering)
- **Day 2**: Phases 4-7 (Parameters + Browser UI)
- **Day 3**: Phases 8-10 (Integration + Mode Toggle + Animation Controls)
- **Day 4**: Phases 11-16 (DI + Statistics + Testing + Polish)

---

## Success Criteria

✅ All 20 CA rules render correctly  
✅ All 15 initial patterns work  
✅ Spectral color mapping produces smooth gradients  
✅ Wrap-around and bounded edges both work  
✅ Pattern scale and density controls function  
✅ Parameter synchronization is reliable  
✅ Browser UI is responsive and intuitive  
✅ Thumbnails load quickly  
✅ Mode toggle works smoothly  
✅ **Theme-aware backgrounds** (white for Light, black for Dark)  
✅ **Light gray grid overlay** is visible and adjustable  
✅ **Grid dimensions** respect aspect ratio constraints  
✅ **Grid toggles** on/off smoothly  
✅ **Real-time statistics** display accurately on status bar  
✅ **Statistics metrics** (generation, alive cells, proportion, lifetimes) are correct  
✅ **Statistics visibility** toggles correctly when switching fractals  
✅ **Dynamic toolbar button** shows Render for fractals, Start/Stop for CA  
✅ **Animation controls** (Start/Stop/Pause) function smoothly  
✅ **Keyboard shortcuts** work for both fractal and CA modes  
✅ **Simulation** animates generation-by-generation at adjustable frame rate  
✅ No crashes or visual artifacts  
✅ Code is documented and clean  
✅ Builds without warnings

---

## Technical Implementation Notes

### Theme-Aware Background

The CA renderer must detect the current application theme and apply the appropriate background color:

- **Light Theme**: White background (`#FFFFFF` or RGB 255,255,255)
- **Dark Theme**: Black background (`#000000` or RGB 0,0,0)

**Implementation approach:**
1. Pass theme information from `MainPage` (which has access to `ActualTheme` or `RequestedTheme`)
2. Alternatively, query `App.Current.Resources` or `Application.Current.RequestedTheme` from render service
3. Apply background fill before drawing CA cells
4. Ensure background is opaque (alpha = 255)

**Code location:** `CellularAutomatonRenderService.cs` → `FillBackground()` method

### Light Gray Grid Overlay

A grid overlay helps users understand the cellular structure and see individual cell boundaries:

- **Grid color**: Light gray (`#C0C0C0` or RGB 192,192,192) - visible on both light and dark backgrounds
- **Grid lines**: 1-pixel thickness
- **Grid spacing**: Determined by `cellSize` parameter (cell dimensions in pixels)
- **Grid dimensions**: User-configurable via `gridWidth` (cells per row) and `gridHeight` (total generations)

**Drawing order:**
1. Fill background (theme-aware)
2. Draw CA cells (colored by generation)
3. Draw grid overlay (if `showGrid == true`)

**Implementation approach:**
- Horizontal lines: Draw at every `cellSize` pixels vertically
- Vertical lines: Draw at every `cellSize` pixels horizontally
- Use anti-aliased lines if supported, or simple 1px Bresenham lines
- Grid should be drawn AFTER cells to ensure visibility

**Code location:** `CellularAutomatonRenderService.cs` → `DrawGrid()` method

### Aspect Ratio Constraints

The grid dimensions must fit within the application's canvas while maintaining reasonable aspect ratios:

**Constraints:**
- Minimum grid size: 10×10 cells
- Maximum grid size: 200×200 cells (configurable based on canvas size)
- Recommended aspect ratios: 1:1 to 4:1 (width:height)
- Cell size range: 1-5 pixels per cell

**Calculation:**
```csharp
int canvasWidth = width;   // From render request
int canvasHeight = height; // From render request
int cellSize = GetParameter("cellSize"); // 1-5 pixels
int maxGridWidth = canvasWidth / cellSize;
int maxGridHeight = canvasHeight / cellSize;

// Clamp user parameters
int gridWidth = Math.Clamp(GetParameter("gridWidth"), 10, maxGridWidth);
int gridHeight = Math.Clamp(GetParameter("gridHeight"), 10, maxGridHeight);
```

**UI feedback:**
- Parameter sliders should update their max values based on current canvas size and cell size
- Display helper text: "Max grid: XXX × YYY cells at current zoom"

---

## Notes & Observations

### Updated Parameter List (11 Total)

| # | Parameter | Type | Range/Options | Default | Description |
|---|-----------|------|---------------|---------|-------------|
| 1 | `rule` | String | Read-only | (varies) | Wolfram rule name (e.g., "Rule 90 - Sierpinski Triangle") |
| 2 | `generations` | Integer | 50-500 | 100 | Number of CA generations to simulate |
| 3 | `cellSize` | Integer | 1-5 | 2 | Cell size in pixels (affects grid spacing) |
| 4 | `wrapEdges` | Boolean | true/false | true | Toroidal wrap (true) vs bounded (false) |
| 5 | `initialPattern` | Choice | 15 options | "Single Center" | Starting configuration |
| 6 | `patternScale` | Integer | 1-10 | 1 | Scale multiplier for tiled patterns |
| 7 | `randomDensity` | Double | 0.0-1.0 | 0.5 | Density for random initial patterns |
| 8 | `colorStart` | Integer | 0-359 | 0 | Hue start angle (degrees) |
| 9 | `colorStop` | Integer | 0-359 | 359 | Hue stop angle (degrees) |
| 10 | `colorCycles` | Integer | 1-5 | 1 | Number of gradient repetitions |
| 11 | `gridWidth` | Integer | 10-200 | 100 | Number of cells per row |
| 12 | `gridHeight` | Integer | 10-200 | 100 | Number of generations (rows) |
| 13 | `showGrid` | Boolean | true/false | true | Toggle grid overlay visibility |

### Theme Integration Notes

- Theme detection happens at render time
- Background color is applied before CA cells
- Grid is drawn after CA cells for visibility
- Light gray (#C0C0C0) chosen for visibility on both themes

### Implementation Resources

See `CA_THEME_AND_GRID_IMPLEMENTATION.md` for:
- Detailed code examples
- Theme detection strategies
- Grid drawing algorithms
- Performance optimization tips
- Testing procedures

*(Use this section to track insights, blockers, or design decisions during implementation)*

- 
- 
-

---

**Last Updated**: 2025-01-16  
**Maintainer**: Development Team  
**Related Docs**: 
- `CA_THEME_AND_GRID_IMPLEMENTATION.md` (Theme detection and grid overlay detailed guide)
- `SPECIAL_RENDERING_IMPLEMENTATION_PLANS.md` (L-System architecture reference)
- `PARAMETER_SYSTEM_ARCHITECTURE.md` (Flexible parameter system)
- `FRACTAL_DEVELOPER_INFRASTRUCTURE.md` (General fractal development guide)
- `WINUI3_THEME_BEST_PRACTICES.md` (WinUI 3 theme system)
