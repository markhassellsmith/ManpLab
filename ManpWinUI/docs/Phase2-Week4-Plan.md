# Phase 2 Week 4: UI Layout Foundation

## Overview
Transform the current 2-panel layout (Canvas + Properties) into a professional 3-panel layout (Browser | Canvas | Properties) with resizable splitters.

## Current State Analysis

### Existing Layout (MainPage.xaml)
```
┌────────────────────────────────────────────────────┐
│ Toolbar (CommandBar)                              │
├────────────────────┬──────────────────────────────┤
│                    │                              │
│   CANVAS           │    PROPERTIES PANEL          │
│   (Fractal View)   │    (Fixed 350px width)       │
│                    │                              │
│                    │    - Color Palette           │
│                    │    - Fractal Type (dropdown) │
│                    │    - Julia Mode              │
│                    │    - Parameters              │
│                    │    - Bookmarks               │
└────────────────────┴──────────────────────────────┘
```

### Target Layout (Phase 2)
```
┌───────────────────────────────────────────────────────────┐
│ Toolbar (CommandBar) + MenuBar                           │
├───────────┬─────────────────────────┬─────────────────────┤
│           │                         │                     │
│  BROWSER  │       CANVAS            │    PROPERTIES       │
│  250px    │       (flexible)        │    300px            │
│           │                         │                     │
│  Search   │   Fractal Render        │  [Tabbed Panels]    │
│  ─────    │                         │  • Parameters       │
│           │                         │  • Colors           │
│  ▼ Classic│                         │  • Info             │
│    • Mand.│                         │  • Bookmarks        │
│    • Burn.│                         │                     │
│  ▼ Newton │                         │                     │
│    • Newt.│                         │                     │
│    • Nova │                         │                     │
│  ▼ Multi  │                         │                     │
│    • Mult3│                         │                     │
└───────────┴─────────────────────────┴─────────────────────┘
```

## Week 4 Goals

### 1. Three-Column Grid Layout ✅
- Replace current 2-column with 3-column Grid
- Add GridSplitter controls for resizing
- Define min/max widths for panels

### 2. Left Panel: Fractal Browser (Stub)
**Week 4 Deliverable:** Basic structure
- TreeView placeholder
- Search TextBox (non-functional)
- "Coming in Week 5" message

**Week 5 Goal:** Populate from FractalRegistry

### 3. Middle Panel: Canvas (Refactor)
**Week 4 Deliverable:** Extract to UserControl
- Move fractal rendering UI to `Views/Canvas/FractalCanvasView.xaml`
- Keep all overlays (axes, selection, labels)
- Maintain all pointer events

### 4. Right Panel: Properties (Enhance)
**Week 4 Deliverable:** Tabbed interface
- Convert to TabView with 4 tabs:
  - **Parameters** (existing content)
  - **Colors** (palette selection, move from Parameters)
  - **Info** (fractal description, metadata - placeholder)
  - **Bookmarks** (move from SplitView pane)

### 5. Menu Bar
**Week 4 Deliverable:** Basic menu structure
- File (Save, Export, Exit)
- View (Toggle panels)
- Tools (Render, Reset, Zoom)
- Help (About, Documentation)

## Implementation Steps

### Step 1: Create New Files

#### New Files to Create:
```
ManpWinUI/Views/Browser/
    FractalBrowserView.xaml
    FractalBrowserView.xaml.cs

ManpWinUI/Views/Canvas/
    FractalCanvasView.xaml
    FractalCanvasView.xaml.cs

ManpWinUI/Views/Properties/
    PropertiesPanelView.xaml
    PropertiesPanelView.xaml.cs

ManpWinUI/ViewModels/Browser/
    FractalBrowserViewModel.cs

ManpWinUI/ViewModels/Properties/
    PropertiesPanelViewModel.cs
```

### Step 2: Refactor MainPage.xaml

**Current Structure:**
- SplitView (Bookmarks overlay)
  - Grid (2 columns)
    - Canvas
    - Properties Panel

**New Structure:**
- Grid (3 columns with splitters)
  - FractalBrowserView
  - GridSplitter
  - FractalCanvasView
  - GridSplitter
  - PropertiesPanelView

### Step 3: Data Flow Architecture

```
MainViewModel (orchestrator)
    ↓
    ├── FractalBrowserViewModel (fractal selection)
    │   └── Event: FractalSelected
    │       → MainViewModel.LoadFractal()
    │
    ├── FractalCanvasViewModel (rendering)
    │   └── Properties: FractalImage, Zoom, Center
    │
    └── PropertiesPanelViewModel (parameters)
        └── Properties: Iterations, Palette, Julia params
```

### Step 4: Update MainViewModel

**Add properties:**
```csharp
// Panel visibility
bool IsBrowserVisible { get; set; } = true;
bool IsPropertiesVisible { get; set; } = true;

// Child ViewModels
FractalBrowserViewModel BrowserViewModel { get; }
PropertiesPanelViewModel PropertiesViewModel { get; }
```

**Add commands:**
```csharp
RelayCommand ToggleBrowserCommand
RelayCommand TogglePropertiesCommand
```

## UI Component Details

### FractalBrowserView (Week 4 - Stub)

**XAML Structure:**
```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/> <!-- Search -->
        <RowDefinition Height="*"/>    <!-- TreeView -->
    </Grid.RowDefinitions>

    <TextBox PlaceholderText="Search fractals..." Grid.Row="0"/>

    <TreeView Grid.Row="1">
        <!-- Week 5: Populate from FractalRegistry -->
        <TreeViewItem Header="📁 Classic Fractals (4)" IsExpanded="True">
            <TreeViewItem Header="Mandelbrot"/>
            <TreeViewItem Header="BurningShip"/>
            <TreeViewItem Header="Tricorn"/>
            <TreeViewItem Header="Phoenix"/>
        </TreeViewItem>

        <TreeViewItem Header="📁 Multibrot Family (3)">
            <TreeViewItem Header="Multibrot³"/>
            <TreeViewItem Header="Multibrot⁴"/>
            <TreeViewItem Header="Multibrot⁵"/>
        </TreeViewItem>

        <!-- ... more categories ... -->
    </TreeView>
</Grid>
```

### FractalCanvasView (Week 4 - Extract)

**Move from MainPage.xaml:**
- Viewbox + Image
- Selection Rectangle Canvas
- Coordinate Axes Canvas
- Hailstone Labels Canvas
- Progress Overlay
- Welcome Overlay

**Keep pointer event handlers in code-behind**

### PropertiesPanelView (Week 4 - Tabbed)

**XAML Structure:**
```xml
<TabView>
    <TabViewItem Header="Parameters" IconSource="Settings">
        <!-- Current parameter controls -->
    </TabViewItem>

    <TabViewItem Header="Colors" IconSource="Color">
        <!-- Palette selection -->
    </TabViewItem>

    <TabViewItem Header="Info" IconSource="Info">
        <!-- Fractal description (Week 5) -->
    </TabViewItem>

    <TabViewItem Header="Bookmarks" IconSource="Favorite">
        <!-- Move bookmark list here -->
    </TabViewItem>
</TabView>
```

## File Changes Summary

### Files to Create (8 files):
1. `Views/Browser/FractalBrowserView.xaml`
2. `Views/Browser/FractalBrowserView.xaml.cs`
3. `Views/Canvas/FractalCanvasView.xaml`
4. `Views/Canvas/FractalCanvasView.xaml.cs`
5. `Views/Properties/PropertiesPanelView.xaml`
6. `Views/Properties/PropertiesPanelView.xaml.cs`
7. `ViewModels/Browser/FractalBrowserViewModel.cs`
8. `ViewModels/Properties/PropertiesPanelViewModel.cs`

### Files to Modify (2 files):
1. `Views/MainPage.xaml` - Complete restructure to 3-panel layout
2. `ViewModels/MainViewModel.cs` - Add child ViewModels, toggle commands

### Files to Reference (No changes):
- `MainViewModel.StandardFractals.cs` - Keep fractal parameters
- `MainViewModel.Commands.cs` - Keep render commands
- `MainViewModel.UI.cs` - Keep palette/status

## Testing Plan

### Week 4 Acceptance Tests:

1. **Layout Structure**
   - [ ] Three vertical panels visible
   - [ ] GridSplitters allow resizing
   - [ ] Panels remember size after resize
   - [ ] Min width constraints work (Browser: 200px, Properties: 250px)

2. **Browser Panel (Stub)**
   - [ ] Search box present (non-functional)
   - [ ] TreeView shows 5 categories with 14 fractals
   - [ ] Clicking fractal does nothing yet (Week 5)

3. **Canvas Panel**
   - [ ] All fractal rendering works as before
   - [ ] Zoom/pan/reset still functional
   - [ ] Overlays (axes, selection) still work
   - [ ] Mouse events still handled

4. **Properties Panel**
   - [ ] 4 tabs visible: Parameters, Colors, Info, Bookmarks
   - [ ] Parameters tab has all controls from old side panel
   - [ ] Colors tab has palette dropdown
   - [ ] Info tab shows placeholder text
   - [ ] Bookmarks tab has full bookmark functionality

5. **Integration**
   - [ ] Render button still works
   - [ ] All existing features still functional
   - [ ] No crashes or exceptions
   - [ ] Clean build with zero warnings

## Code Quality Standards

### XAML Naming Conventions:
- UserControls: `{Feature}View.xaml` (e.g., `FractalBrowserView`)
- ViewModels: `{Feature}ViewModel.cs`
- Bindings: Use `x:Bind` with Mode=OneWay/TwoWay
- Resources: Use ThemeResource for colors

### C# Code Standards:
- Properties: Use `[ObservableProperty]` from CommunityToolkit.Mvvm
- Commands: Use `[RelayCommand]` attribute
- Async: All render methods use `async`/`await`
- Events: Use weak event pattern for cross-VM communication

### Architecture Principles:
- **Separation of Concerns**: Each UserControl has its own ViewModel
- **Single Responsibility**: ViewModels focus on one feature area
- **MVVM Pattern**: No code-behind logic except UI event routing
- **Testability**: ViewModels are unit-testable (no UI dependencies)

## Risk Mitigation

### Potential Issues:

1. **Breaking Existing Functionality**
   - **Risk:** Moving canvas might break pointer events
   - **Mitigation:** Test all mouse interactions after extraction
   - **Rollback:** Keep old MainPage.xaml as MainPage.old.xaml

2. **Performance Impact**
   - **Risk:** Three ViewModels might slow down rendering
   - **Mitigation:** Profile render times before/after
   - **Target:** < 5% performance degradation

3. **State Management**
   - **Risk:** Shared state between VMs might get out of sync
   - **Mitigation:** MainViewModel is source of truth, child VMs read-only

## Success Criteria

Week 4 is complete when:

✅ Three-panel layout implemented with resizable splitters  
✅ Browser panel shows stub TreeView with 14 fractals  
✅ Canvas extracted to UserControl, all features working  
✅ Properties panel converted to 4-tab TabView  
✅ All existing functionality preserved (render, zoom, bookmarks)  
✅ Clean build with zero warnings  
✅ All Phase 1 fractals still render correctly  
✅ Code follows MVVM architecture  
✅ Documentation updated (this plan + completion report)  

## Timeline

- **Day 1-2:** Create new UserControl files, stub ViewModels
- **Day 3-4:** Refactor MainPage.xaml to 3-panel layout
- **Day 5:** Move canvas to FractalCanvasView
- **Day 6:** Convert properties to tabbed PropertiesPanelView
- **Day 7:** Testing, bug fixes, documentation

**Estimated Effort:** 20-25 hours

## Next Week Preview

**Phase 2 Week 5: Fractal Browser Implementation**
- Populate TreeView from FractalRegistry
- Implement search/filter functionality
- Add fractal thumbnails (64x64 preview)
- Wire up selection → render pipeline
- Add favorites/recent fractals

---

*Created: Phase 2 Week 4 Start*  
*Branch: feature/phase2-week4-ui-foundation*  
*Goal: Professional 3-panel layout foundation*
