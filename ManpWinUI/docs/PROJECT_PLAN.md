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

### Week 3: Batch Rendering
**Support animations and batch operations**

Files to create:
- `ManpCore.Native/BatchRenderer.h/.cpp`

Features:
- Queue multiple render jobs
- Animation frame interpolation
- Progress events for UI

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

### Week 4: Layout Foundation
Create 3-panel resizable layout with splitters

### Week 5: Fractal Browser
TreeView with all 240+ fractals, search, favorites

### Week 6: Parameter Editor
Dynamic form that changes based on selected fractal

### Week 7: Color & Render Panels
Palette selection, render mode (escape-time, distance, orbit trap)

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

- [ ] All 240+ fractals accessible from UI
- [ ] Professional multi-panel interface
- [ ] Performance matches ManpWIN64
- [ ] Better organization than legacy WinForms
- [ ] Complete documentation
- [ ] Public release ready

---

## Next Step

**Week 1 starts now**: Create `FractalRegistry` to catalog all fractals.

See individual week plans above for detailed tasks.

---

**Last Updated**: April 27, 2026  
**Status**: Phase 1 - Week 1  
**Branch**: development
