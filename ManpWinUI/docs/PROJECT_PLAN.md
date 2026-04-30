# ManpLab Production Plan

**Vision**: Expose all 240+ C++ fractals through a modern WinUI 3 interface  
**Timeline**: 12 weeks to production release  
**Architecture**: Keep C++ for computation, WinUI for interface

---

## Quick Overview

```
WinUI 3 Interface (C# XAML)
    ↓
ManpCore.Native (C++/CLI Bridge) ← EXPAND THIS
    ↓
ManpWIN64 (C++ Engine - 240+ fractals) ← ALREADY EXISTS
```

**Strategy**: Build a professional UI that exposes the existing C++ fractal engine through an expanded C++/CLI wrapper.

---

## Phase 1: Native Bridge (Weeks 1-3)

### Week 1: Fractal Registry
**Create system to expose all 240+ fractals**

Files to create:
- `ManpCore.Native/FractalRegistry.h`
- `ManpCore.Native/FractalRegistry.cpp`

Features:
- List all fractals with metadata (name, category, description)
- 10 categories: Classic, Variants, Attractors, IFS, etc.
- Query by category or search

### Week 2: Fractal Switching
**Update wrapper to render any fractal type**

Files to update:
- `ManpCore.Native/FractalEngineWrapper.h/.cpp`
- `ManpCore.Native/FractalParameters` (add CustomParams dictionary)

Features:
- Accept any fractal ID from registry
- Route to correct C++ fractal function
- Handle per-fractal custom parameters

### Week 3: Batch Rendering ✅ COMPLETE
**Support animations and batch operations**

Files created:
- `ManpCore.Native/BatchRenderer.h` ✅
- `ManpCore.Native/BatchRenderer.cpp` ✅

Features:
- ✅ Queue multiple render jobs
- ✅ Animation frame interpolation (5 modes: Linear, EaseIn, EaseOut, EaseInOut, Exponential)
- ✅ Progress events for UI
- ✅ Cancellation support
- ✅ Per-job status tracking (Pending, Running, Completed, Failed, Cancelled)
- ✅ Keyframe-based animation system

Documentation:
- ✅ `docs/BatchRenderer-Guide.md` with examples

---

## Phase 2: UI Redesign (Weeks 4-8)

### New Layout Structure

```
┌──────────────────────────────────────────────────────┐
│ Menu Bar                                             │
├────────────┬────────────────────────┬────────────────┤
│  BROWSER   │       CANVAS           │    PROPERTIES  │
│            │                        │                │
│  [Search]  │   (Render View)       │  [Tabs:]       │
│            │                        │  • Parameters  │
│  Categories│                        │  • Colors      │
│  ├─Classic │                        │  • Info        │
│  ├─Variants│                        │  • Presets     │
│  ├─IFS     │                        │  • History     │
│  └─...     │                        │                │
└────────────┴────────────────────────┴────────────────┘
```

### Week 4: Layout Foundation ✅ COMPLETE
**Create 3-panel resizable layout with splitters**

Status: ✅ Complete
- ✅ 3-panel layout (Browser | Canvas | Properties)
- ✅ GridSplitter implementation with drag-to-resize
- ✅ Panel persistence (save/restore widths and visibility)
- ✅ Panel collapse/expand buttons (VS Code-style)
- ✅ Keyboard shortcuts: Ctrl+B (Browser), Ctrl+P (Properties)
- ✅ `IAppSettingsService` for persistent settings storage
- ✅ `DoubleToGridLengthConverter` for width bindings
- ✅ Visual feedback (cursor changes on hover)
- ✅ Resizable with min/max width constraints

Documentation:
- ✅ Updated keyboard shortcuts help (F1)
- ✅ `MainPage.PanelManagement.cs` for splitter handlers

### Week 5: Fractal Browser ✅ COMPLETE
**TreeView with all fractals from registry, search, selection persistence**

Status: ✅ Complete
- ✅ C++/CLI wrapper (`FractalRegistryWrapper.h/.cpp`) for native registry access
- ✅ Registry initialization at app startup
- ✅ Load categories and fractals from `FractalRegistry` into browser
- ✅ Display fractal DisplayName and Description tooltips
- ✅ Click-to-select-and-render workflow
- ✅ Real-time search/filtering by name, display name, and description
- ✅ Selection persistence across app restarts via `IAppSettingsService`
- ✅ All 14 registered fractals accessible from browser UI

Documentation:
- ✅ `Phase2-Week5-Progress.md` with complete implementation details

Branch: Merged to `development`

### Week 6: Parameter Editor ✅ COMPLETE
**Dynamic form that changes based on selected fractal**

Status: ✅ Complete (All 6 Tasks)

**Task 1: Foundation Setup** ✅
- ✅ Created `ParameterEditorView.xaml` and code-behind
- ✅ Created `ParameterEditorViewModel.cs` with `ParameterItem` class
- ✅ Integrated into Properties Panel (Parameters tab)
- ✅ Basic ItemsControl displaying parameter name/value pairs
- ✅ Placeholder parameters for demonstration

**Task 2: Connect to Selected Fractal** ✅
- ✅ Added `LoadParametersForFractal()` method in ViewModel
- ✅ Wire up to FractalBrowserViewModel.FractalSelected event
- ✅ Load parameters based on selected fractal type from registry
- ✅ Display fractal metadata (DisplayName, Category, defaults)
- ✅ Show Julia support and Multibrot power detection
- ✅ Clear parameters when no fractal selected

**Task 3: Dynamic Parameter Metadata** ✅
- ✅ Add parameter type information (double, complex, enum, bool)
- ✅ Add min/max/default values
- ✅ Add parameter descriptions/tooltips

**Task 4: Type-Specific Controls** ✅
- ✅ NumberBox for numeric parameters (with min/max)
- ✅ Complex number input for Julia C values
- ✅ CheckBox for boolean parameters
- ✅ DataTemplateSelector for dynamic control generation
- ✅ Typed property accessors (ValueAsDouble, ValueAsBoolean, ComplexReal/Imaginary)

**Task 5: Parameter Validation & Updates** ✅
- ✅ Parameter change notifications (ValueChanged events)
- ✅ Wire parameter changes to trigger automatic re-renders
- ✅ Bidirectional sync (parameter editor ↔ MainViewModel)
- ✅ "Reset to Defaults" button
- ✅ Default value storage for each parameter
- ✅ Real-time validation with NumberBox min/max

**Task 6: Parameter Persistence** ✅
- ✅ Save parameter values per fractal type (JSON to LocalSettings)
- ✅ Restore last-used parameters on fractal selection
- ✅ Automatic save on change, restore on load
- ✅ Per-fractal isolation with fallback to defaults

Documentation:
- ✅ `Phase2-Week6-Progress.md` with all 6 tasks complete

Branch: `feature/phase2-week7-color-render-panels`

### Week 8: Presets & History
Save locations, navigation undo/redo

### Week 8.5: File Export 🚨 CRITICAL - MISSING IMPLEMENTATION
**Implement image save functionality (toolbar buttons exist but not functional)**

Status: ⏳ Not Started - Discovered during Week 7

Features:
- PNG/JPEG export with file picker (FileSavePicker)
- Export options dialog (resolution, quality, metadata)
- SVG export for Hailstone (via native bridge)
- Metadata embedding (coordinates, parameters, fractal type)
- Event handlers for Save Image toolbar buttons

Priority: **HIGH** - Core user-facing feature
Estimated: 7-8 hours (~1 week task)

See: `docs/FILE_EXPORT_TODO.md` for detailed implementation plan

---

## Phase 3: Advanced Features (Weeks 9-10)

### Week 9: Core Features
- Render cancellation (ESC key) ✅ Already implemented!
- Deep zoom toggle (arbitrary precision)
- Enhanced status bar

### Week 10: Animation System
Palette selection, render mode (escape-time, distance, orbit trap)

Status: 🚧 Task 2 Complete ✅

**Task 1: Foundation Setup** ✅
- ✅ Created `ColorEditorView.xaml` and `ColorEditorViewModel.cs`
- ✅ Created `RenderSettingsView.xaml` and `RenderSettingsViewModel.cs`
- ✅ Integrated into Properties Panel (Colors and Render tabs)
- ✅ 6 built-in color palettes aligned with native engine
- ✅ 4 render modes (EscapeTime, SmoothColoring, DistanceEstimation, OrbitTrap)
- ✅ Quality settings (antialiasing, smooth coloring, deep zoom)
- ✅ Resolution controls with presets (HD, Full HD, 4K)
- ✅ Value converters for XAML bindings
- ✅ Reset to defaults functionality
- ✅ Build successful

**Task 2: Palette System** ✅
- ✅ Wired ColorEditorViewModel and RenderSettingsViewModel to MainPage
- ✅ Subscribed to palette change events
- ✅ Aligned UI palettes with ManpCore.Native (Grayscale, Classic, Fire, Ocean, Rainbow, Psychedelic)
- ✅ Automatic re-render when palette changes
- ✅ Data flow: UI → ViewModel → MainViewModel → RenderService → Native Engine
- ✅ Build successful, ready for testing

**Task 3: Advanced Features** ⏳ Next
- Render mode switching (pass to native engine)
- Color cycle speed and offset effects
- Palette persistence per fractal
- Smooth coloring and antialiasing integration

Documentation:
- ✅ `Phase2-Week7-Progress.md` started
- ✅ `Phase2-Week7-Task2-Complete.md` with full implementation details

Branch: `feature/phase2-week7-color-render-panels`

### Week 8: Presets & History
Save locations, navigation undo/redo

---

## Phase 3: Advanced Features (Weeks 9-10)

### Week 9: Core Features
- Render cancellation (ESC key)
- Deep zoom toggle (arbitrary precision)
- Enhanced status bar

### Week 10: Animation System
- Animation builder dialog
- Frame interpolation
- Batch export

---

## Phase 4: Polish & Release (Weeks 11-12)

### Week 11: Quality
- Performance optimization
- Bug fixes
- Accessibility
- Error handling

### Week 12: Documentation & Release
- Update all docs
- Create installer
- GitHub release
- Announce to fractal community

---

## File Structure (Simplified)

```
ManpLab/
├── ManpCore.Native/              # C++/CLI Bridge (EXPAND)
│   ├── FractalRegistry.*         # NEW - Catalog of 240+ fractals
│   ├── FractalEngineWrapper.*    # UPDATE - Support all types
│   └── BatchRenderer.*           # NEW - Animation system
│
├── ManpCore.Services/            # Pure C# (minimal changes)
│   └── [Existing services]
│
├── ManpWinUI/                    # WinUI Interface (MAJOR REDESIGN)
│   ├── Views/
│   │   ├── MainWindow.xaml       # REDESIGN - 3-panel layout
│   │   ├── Browser/              # NEW - Fractal navigation
│   │   ├── Properties/           # NEW - Parameter editors
│   │   └── Dialogs/              # NEW - Animation builder
│   │
│   ├── ViewModels/               # REFACTOR - Modular structure
│   │   ├── MainViewModel.cs
│   │   ├── Browser/
│   │   └── Properties/
│   │
│   └── Services/
│       └── [Existing, enhanced]
│
├── ManpWIN64/                    # C++ Engine (NO CHANGES)
│   └── [156 files - leave as-is]
│
└── docs/
    ├── PROJECT_PLAN.md           # THIS FILE
    ├── ARCHITECTURE.md           # Create in Week 11
    ├── DEVELOPER_GUIDE.md        # Create in Week 12
    └── HAILSTONE.md              # Existing (cleaned up)
```

---

## Key Decisions

### ✅ DO:
- Keep all fractal computation in C++
- Expand C++/CLI bridge to expose more fractals
- Build modern WinUI interface
- Focus on usability and organization

### ❌ DON'T:
- Port fractal algorithms to C# (performance loss)
- Rewrite C++ engine
- Change ManpWIN64 code

---

## Success Criteria

- [ ] All 240+ fractals accessible from UI (14 fractals registered and browsable ✅)
- [x] Professional multi-panel interface (Week 4 ✅: 3-panel resizable layout)
- [x] Performance matches ManpWIN64 (C++ computation maintained)
- [x] Better organization than legacy WinForms (Week 5 ✅: Browser with search/categories)
- [ ] Complete documentation (In progress - BatchRenderer, Week 4, Week 5 documented)
- [ ] Public release ready (Not yet)

**Phase 1 Status**: ✅ Complete (Weeks 1-3)
- ✅ FractalRegistry system (14 fractals registered)
- ✅ Fractal switching via wrapper
- ✅ BatchRenderer with animation support

**Phase 2 Status**: 🚧 In Progress (Week 7/8)
- ✅ Week 4: Layout Foundation complete
  - ✅ 3-panel resizable layout
  - ✅ Panel persistence and collapse/expand
  - ✅ Keyboard shortcuts (Ctrl+B, Ctrl+P)
- ✅ Week 5: Fractal Browser complete
  - ✅ Registry integration with C++/CLI wrapper
  - ✅ Search/filtering functionality
  - ✅ Click-to-load-and-render workflow
  - ✅ Selection persistence
- ✅ Week 6: Parameter Editor complete
  - ✅ Dynamic parameter loading from registry
  - ✅ Type-specific controls (NumberBox, Complex, CheckBox)
  - ✅ Automatic re-rendering on parameter changes
  - ✅ Bidirectional sync with fractal view
  - ✅ Parameter persistence per fractal type
- 🔵 Week 7: Color & Render Panels (NEXT)
- ⏳ Week 8: Presets & History

**Next Phase**: Phase 2 - UI Redesign (Week 7: Color & Render Panels)

---

## Next Step

**Week 6 starts now**: Create dynamic parameter editor that changes based on selected fractal.

**Goals:**
- Properties panel with tabbed interface (Parameters, Colors, Info, Presets, History)
- Dynamic parameter controls based on fractal type
- Support Julia C values, power/exponent settings, iteration modes
- Real-time parameter validation
- Parameter persistence and reset to defaults

See individual week plans above for detailed tasks.

---

**Last Updated**: January 2025  
**Status**: Phase 2 - Week 6 Complete ✅ | Week 7 Starting 🔵  
**Branch**: `feature/phase2-week6-parameter-editor` (ready for merge)
