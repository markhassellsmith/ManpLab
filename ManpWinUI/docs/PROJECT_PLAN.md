# ManpLab Production Plan

**Vision**: Expose all 240+ C++ fractals through a modern WinUI 3 interface  
**Original Timeline**: 12 weeks to production release  
**Updated Delivery Target**: May 31, 2026 (29 days from May 2)  
**Architecture**: Keep C++ for computation, WinUI for interface

---

# 🚀 FINAL SPRINT ROADMAP (May 2 - May 31, 2026)

## 🎯 Strategy: Early Integration, Iterative Expansion

**Current Status (May 2):** Architecture complete, Mandelbrot/Julia working, 240+ fractals in native library awaiting UI integration

**Goal:** Get all fractals rendering through the UI by early May, then scale up systematically.

---

## **Week 1: May 2-8 (7 days) - FOUNDATION & PROOF OF CONCEPT**

### **Days 1-2 (May 2-3): Browser-Registry Integration**
**Priority: CRITICAL**
- Connect FractalBrowserViewModel to FractalRegistryWrapper
- Load fractal list from native library into Browser UI
- Display fractal names, categories, descriptions in the browser panel
- **Deliverable:** See all 240+ fractals listed in the Browser

### **Days 3-4 (May 4-5): First 5 Fractals Working**
**Priority: CRITICAL**
- Pick 5 diverse fractals (different parameter sets)
- Map parameters from FractalRegistry to MainViewModel
- Wire up "Select Fractal" from Browser → Render
- **Deliverable:** 5 fractals fully rendering from Browser selection

### **Day 5 (May 6): Testing & Parameter Framework**
- Test the 5 fractals thoroughly
- Identify common parameter patterns
- Create parameter mapping abstraction layer
- **Deliverable:** Reusable pattern for adding more fractals

### **Days 6-7 (May 7-8): Scale to 20 Fractals**
- Apply the pattern to add 15 more fractals
- Test rendering, zoom, Julia modes
- Fix bugs discovered during expansion
- **Deliverable:** 20 fractals working end-to-end

---

## **Week 2: May 9-15 (7 days) - RAPID EXPANSION**

### **Days 8-10 (May 9-11): Add 60 Fractals (Total: 80)**
- Focus on speed: 20 fractals/day
- Group by category for efficiency
- Automated testing where possible
- Log any fractals that need special handling

### **Days 11-13 (May 12-14): Add 80 Fractals (Total: 160)**
- Continue systematic integration
- Track problem fractals separately
- Build a "known issues" list
- **Deliverable:** 2/3 of fractals integrated

### **Day 14 (May 15): Mid-Project Testing & Fixes**
- Comprehensive testing of 160 fractals
- Fix critical bugs
- Performance profiling
- Update roadmap based on findings

---

## **Week 3: May 16-22 (7 days) - COMPLETION & STABILITY**

### **Days 15-17 (May 16-18): Complete Remaining 80+ Fractals**
- Final push to 240+ fractals
- Handle edge cases and special fractals
- Document any limitations
- **Deliverable:** All fractals integrated

### **Days 18-19 (May 19-20): Bug Bash & Stabilization**
- Fix crashes, rendering glitches
- Optimize slow fractals
- Memory leak testing
- Error handling improvements

### **Days 20-21 (May 21-22): User Testing**
- Walk through all major workflows
- Test bookmarks, export, navigation
- Verify keyboard shortcuts
- Check theme switching

---

## **Week 4: May 23-29 (7 days) - POLISH & PACKAGING**

### **Days 22-23 (May 23-24): Documentation**
- Update README.md
- Create user guide (basic)
- In-app help text (if time)
- Developer notes for future work

### **Days 24-25 (May 25-26): Packaging & Distribution**
- MSIX package refinement
- Test installation on clean machine
- Create release notes
- Version numbering (v1.0)

### **Days 26-27 (May 27-28): Final Polish**
- UI tweaks based on testing
- Performance optimization
- Fix remaining minor bugs
- Code cleanup

### **Day 28 (May 29): Release Candidate**
- Final build
- Complete testing pass
- Sign off on release

---

## **Buffer Days: May 30-31 (2 days) - SAFETY NET**
- Handle unexpected issues
- Last-minute fixes
- Deployment preparation
- **Final delivery: May 31**

---

## 🚨 Risk Mitigation

**High Risk Items:**
1. **Parameter mapping complexity** - Mitigated by testing 5 fractals first
2. **Rendering failures** - Caught early with incremental testing
3. **Performance issues** - Weekly profiling and optimization

**Scope Control:**
- ❌ **Cut:** Advanced features, icon polish, animations, video export
- ✅ **Keep:** Core fractal rendering, bookmarks, export, browser
- 🟡 **Optional:** Enhanced documentation, additional themes

---

## 📊 Success Metrics by Week

- **Week 1 (May 2-8):** 20 fractals working → **Proof of concept success**
- **Week 2 (May 9-15):** 160 fractals integrated → **On track**
- **Week 3 (May 16-22):** All 240+ fractals + stable → **Feature complete**
- **Week 4 (May 23-29):** Packaged & tested → **Ship ready**
- **May 30-31:** Buffer for unexpected issues → **Launch v1.0**

---

## 🎯 Immediate Next Steps (Starting May 3)

1. Wire up FractalBrowserViewModel to FractalRegistryWrapper
2. Display the full fractal list in the UI
3. Select first 5 test fractals for proof of concept

---

# ORIGINAL 12-WEEK PLAN (Historical Reference)

---

## Repository Structure Notes

**Solution Files:**
- ✅ `ManpLab.sln` - Main solution (use this)
- ❌ `ManpWIN64.sln` - DELETED (legacy, redundant)
- ❌ `ManpWinUI.slnx` - DELETED (XML format, not needed)
- ✅ `MPEG/mpeg.sln` - **KEEP** - Required for animation video export (Week 10)

**MPEG Encoder:** The `MPEG/` folder contains Paul DeLeeuw's MPEG-2 encoder used by `ManpWIN64/MPEGWrite.cpp` for exporting fractal animations to video. This will be used in Week 10 and can be modernized post-release.

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

**Task 3: Advanced Color Features** ✅ COMPLETE
- ✅ Added color cycle speed (0-100) parameter to service interface and implementation
- ✅ Added color offset (0-360°) parameter for palette rotation effects
- ✅ Added smooth coloring toggle to eliminate color banding
- ✅ Wired ColorEditorViewModel settings → MainViewModel → RenderService
- ✅ Mapped RenderMode.SmoothColoring to smooth coloring flag
- ✅ Added logging for all advanced color parameters
- ✅ UI responds to color setting changes with automatic re-render
- ✅ Ready for native engine integration (C++ implementation pending)
- ⏳ Antialiasing and Deep Zoom prepared but require native engine support

Documentation:
- ✅ `Phase2-Week7-Progress.md` started
- ✅ `Phase2-Week7-Task2-Complete.md` with palette system details
- ✅ `Phase2-Week7-Task3-Complete.md` with advanced features documentation

Branch: `feature/phase2-week7-color-render-panels`

### Week 8: Presets & History
**Status**: ⏳ Not Started (Deferred to post-release)

Save locations, navigation undo/redo

### Week 8.5: File Export ✅ COMPLETE
**Status**: ✅ Complete and merged to `development`

Features Delivered:
- ✅ PNG export with metadata (lossless)
- ✅ JPEG export with EXIF metadata (lossy)
- ✅ SVG export for Hailstone sequences (vector)
- ✅ Clipboard copy (compatible with Paint.NET, Paint, GIMP, Photoshop)
- ✅ Unified save dialog with format selection
- ✅ Keyboard shortcuts (Ctrl+S, Ctrl+C)
- ✅ Auto-generated filenames with timestamps
- ✅ Complete metadata embedding (fractal state preservation)

Documentation:
- ✅ `Phase2-Week8.5-Summary.md` (297 lines)
- ✅ `Week8.5-COMPLETION.md` with merge details
- ✅ `FILE_EXPORT_TESTING.md` (302 lines)
- ✅ `ARCHITECTURE_NATIVE_ENGINE.md` (393 lines)

Branch: Merged to `development` (commit 81d8b53)

---

## Phase 3: Advanced Features (Weeks 9-10)

### Week 9: Core Features

**Week 9 Task 1: Deep Zoom Toggle** ✅ COMPLETE
**Status**: ✅ Implemented and tested

Features Delivered:
- ✅ UI checkbox toggle (RenderSettingsView)
- ✅ BigDouble conversion with 25-digit precision
- ✅ Service interface updated (useDeepZoom parameter)
- ✅ Wired through rendering pipeline (Mandelbrot & Julia)
- ✅ Native MPFR engine integration
- ✅ Performance acceptable (~2-5x slower for extreme precision)

Files Modified:
- `IFractalRenderService.cs` (3 lines)
- `FractalRenderService.cs` (35 lines)
- `MainViewModel.Commands.cs` (2 lines)

Documentation:
- ✅ `Phase3-Week9-Task1-Complete.md` with full implementation details

Branch: `development`
Status: ✅ Ready for testing and merge

**Week 9 Task 2: Enhanced Status Bar** ⏳ NOT STARTED
- Display current zoom level with scientific notation
- Show "Deep Zoom Active" indicator
- Recommend iteration count based on zoom
- Display render performance metrics

**Week 9 Task 3: Render Cancellation** ✅ COMPLETE
- ✅ ESC key cancellation already implemented in Week 7

### Week 10: Animation System
**Build UI for creating fractal animations and export to video**

Features:
- Animation builder dialog (keyframe editor UI)
- Frame interpolation (leverage existing BatchRenderer with 5 modes)
- Batch export to image sequences
- Video export integration

**🎬 Video Export Architecture:**

Current State:
- ✅ `BatchRenderer` (Week 3) provides frame generation infrastructure
- ✅ Legacy `MPEG/` folder contains working MPEG-2 encoder from Paul DeLeeuw
- ✅ `ManpWIN64/MPEGWrite.cpp` provides integration interface
- ✅ `ManpWIN64/anim.h` defines animation frame structures

**Recommended Approach:**

**Phase 1 (Week 10 MVP):** Use existing MPEG-2 encoder
- Wrap `MPEGWrite.cpp` functionality in C++/CLI bridge
- Export animation frames via `BatchRenderer` → MPEG-2 file
- Pros: Proven, already integrated, unblocks animation feature
- Cons: MPEG-2 is outdated, large file sizes

**Phase 2 (Post-Release Enhancement):** Modernize to MP4/H.264
- Replace MPEG-2 with Windows Media Foundation or FFmpeg wrapper
- Smaller file sizes, better browser/device compatibility
- Keep `mpeg.sln` as reference but can be archived after migration

**Decision Point:** Keep `MPEG/` folder until video modernization is complete.

**Note:** The MPEG project (`mpeg.sln`) should NOT be deleted - it's required for animation video export functionality.

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

**Phase 2 Status**: ✅ Complete (Weeks 4-8.5)
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
- ✅ Week 7: Color & Render Panels complete
  - ✅ Task 1: Foundation setup (Views + ViewModels)
  - ✅ Task 2: Palette system wiring
  - ✅ Task 3: Advanced color features (cycle, offset, smooth coloring)
- ⏳ Week 8: Presets & History (Deferred to post-release)
- ✅ Week 8.5: File Export complete
  - ✅ PNG/JPEG/SVG export with metadata
  - ✅ Clipboard copy with Paint.NET compatibility
  - ✅ Unified save dialog and keyboard shortcuts
  - ✅ Merged to `development` branch

**Phase 3 Status**: 🚧 In Progress (Week 9 Task 1 Complete)
- ✅ Week 9 Task 1: Deep Zoom Toggle complete
  - ✅ BigDouble arbitrary precision (25 digits)
  - ✅ Service interface and pipeline integration
  - ✅ Works with Mandelbrot and Julia sets
- ⏳ Week 9 Task 2: Enhanced Status Bar (NEXT)
- ✅ Week 9 Task 3: Render Cancellation (Already done in Week 7)

**Next Task**: Phase 3 - Week 9 Task 2: Enhanced Status Bar

---

## Next Step

**Phase 3 - Week 9 Task 2 starts now**: Create enhanced status bar with zoom information and performance metrics.

**Goals:**
- Display current zoom level with scientific notation (e.g., "Zoom: 1.23E+15")
- Show "Deep Zoom Active (25 digits)" indicator when enabled
- Recommend iteration count based on zoom level
- Display render performance metrics (time, pixels/sec)
- Update status bar in real-time during renders

See Phase 3 plan above for detailed tasks.

---

**Last Updated**: January 2025  
**Status**: Phase 3 - Week 9 Task 1 Complete ✅ | Task 2 Next 🔵  
**Branch**: `development`
