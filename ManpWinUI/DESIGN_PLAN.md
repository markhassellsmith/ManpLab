# WinUI Interface Modernization Plan

## Overview
This document is the master index for the ManpLab WinUI modernization project. The comprehensive planning documentation has been organized into modular files for easier maintenance and navigation.

**Project Goal:** Replatform the ManpLab fractal application from C++ Win32 to modern WinUI 3 while preserving the proven mathematical engine.

## Modernization Strategy
- **Keep**: Efficient core functionality and proven algorithms (C++ math engine)
- **Replatform**: User interface from C++ to WinUI 3 / .NET 10
- **Redesign**: Clunky workflows, outdated patterns, and pain points
- **Architecture**: Hybrid C++/C# with C++/CLI interop layer

---

## 📚 Documentation Index

### Planning & Analysis
- **[01 - Project Analysis](docs/01-project-analysis.md)** - Detailed analysis of ManpWIN64 C++ codebase
  - Components to keep vs. redesign
  - File format specifications
  - Migration strategy and code reuse estimates

- **[02 - Architecture & Design](docs/02-architecture.md)** - System architecture and design decisions
  - Hybrid C++/C# architecture
  - UI components and MVVM patterns
  - C++/CLI interop strategy
  - Data binding, commands, and services
  - Performance considerations

### Implementation
- **[03 - Implementation Phases](docs/03-implementation-phases.md)** - 9-phase development plan
  - Detailed task checklists for each phase
  - Timeline estimates (3-5 months with AI assistance)
  - Milestones and success criteria
  - Phase dependencies

- **[04 - Technology Stack](docs/04-technology-stack.md)** - Technologies, libraries, and tools
  - .NET 10 + WinUI 3 packages
  - C++ interop options (C++/CLI vs P/Invoke)
  - Build configuration and dependencies
  - Package installation commands by phase

### Development Process
- **[05 - Git Strategy](docs/05-git-strategy.md)** - Branching, commits, and version control
  - Branch hierarchy (main → development → feature)
  - Commit conventions per phase (with examples)
  - Semantic versioning scheme
  - Daily workflow and merge strategies

- **[06 - AI Safety](docs/06-ai-safety.md)** - Working safely with AI assistance
  - Commit frequency guidelines (every 15-30 min)
  - File corruption prevention
  - Recovery procedures
  - Session management strategy

### Cross-Platform
- **[07 - MAUI Compatibility](docs/07-maui-compatibility.md)** - Future cross-platform expansion
  - Platform-agnostic architecture patterns
  - Service abstractions (IFileService, IBitmapService, etc.)
  - WinUI vs MAUI control mapping
  - Migration path to mobile/macOS (2-4 weeks if architecture is ready)

### Resources
- **[08 - References](docs/08-references.md)** - Documentation links and external resources
  - Microsoft documentation (WinUI, MVVM, C++/CLI, MAUI)
  - Fractal mathematics resources
  - Library references (MPFR, QD, zlib, FFmpeg)

---

## Current Status

### ✅ Phase 1: Planning & Analysis - COMPLETE
**Git Tags:**
- `v0.1.0-planning` - Planning phase complete (merged to development)
- `v0.1.1-docs-split` - Documentation split into modular files (current)

**Completed Tasks:**
- [x] Document existing C++ interface features
- [x] Identify what to keep vs. redesign
- [x] Analyze project dependencies and libraries
- [x] List core algorithms to preserve in C++
- [x] Create detailed C++/C# interop strategy
- [x] Map C++ functionality to WinUI equivalents
- [x] Define modern project structure
- [x] Document file format specifications
- [x] Identify pain points and improvement opportunities
- [x] Split documentation into modular files for safety

**Branch:** `feature/add-winui-interface` (on `development`)

### ⏳ Next: Phase 2 - C++ Core Preparation
**Focus:** Create C++/CLI wrapper layer
- Extract computation code from UI dependencies
- Define managed/unmanaged type marshalling
- Test interop performance
- Document C++ API for C# consumers

See [Implementation Phases](docs/03-implementation-phases.md) for full roadmap.

---

## Quick Reference

### Key Architectural Decisions

✅ **Hybrid C++/C# Architecture**
- Keep C++ math engine (150+ source files, 80,000+ lines, proven algorithms)
- Build new C# WinUI 3 interface with MVVM
- Connect via C++/CLI wrapper layer for performance

✅ **C++/CLI Interop Layer (Recommended)**
- Easier type marshalling than P/Invoke
- Direct memory access for image buffers
- Mixed-mode debugging capability
- Better for complex types (Complex, BigDouble)

✅ **Backward Compatibility**
- Maintain .MAP, .PAR, .KFR file format support
- Preserve all 240+ fractal types
- Keep existing formula parser unchanged

✅ **MAUI-Ready Architecture**
- Platform-agnostic ViewModels (no WinUI dependencies)
- Service abstractions for file I/O, bitmaps, navigation
- 90%+ code reuse for future mobile/macOS versions

✅ **GPU Acceleration Decision**
- **Display rendering:** Automatic GPU via WinUI 3 (free benefit)
- **Calculation:** CPU-only for v1.0 (advanced features can't use GPU)
- **Post-v1.0:** Consider GPU for simple Mandelbrot/Julia if >50% of user renders

---

## Project Summary

**What's Being Preserved:**
- 80,000+ lines of C++ mathematical code
- 240+ fractal type implementations
- Perturbation theory engine (deep zoom 10^100+)
- BLA (Bilinear Approximation) iteration skipping
- High-precision arithmetic (MPFR, quad-double)
- Formula parser with 240+ built-in types
- Coloring algorithms (smooth iteration, distance estimation, orbit traps)

**What's Being Replaced:**
- 15,000+ lines of C++ Win32 UI code
- Modal dialog-based parameter entry → Side panels with live update
- GDI software rendering → WriteableBitmap with GPU acceleration
- Menu-driven workflow → CommandBar + NavigationView
- INI configuration → JSON with strong typing
- MPEG-2 video export → H.264/H.265 (MP4)

**Development Timeline:**
- **Part-time with AI:** 3-5 months (10-20 hours/week)
- **Full-time with AI:** 1-2 months (40 hours/week)
- **AI Benefits:** 40-50% time savings (XAML generation, MVVM boilerplate, tests)

---

## Pain Points Addressed

| Old (ManpWIN64) | Issue | New (WinUI) |
|-----------------|-------|-------------|
| Modal dialog hell | Blocks workflow | Side panels with live feedback |
| Deep menu hierarchies | Hard to discover | Visual CommandBar + NavigationView |
| Text-only parameter entry | No validation | NumberBox + Slider with range validation |
| No undo | Can't revert mistakes | Command pattern with undo/redo stack |
| Clipboard-only sharing | Limited | Windows Share contract |
| GDI rendering | Slow, no GPU | GPU-accelerated (5-10x faster display) |
| Fixed-size windows | Not flexible | Responsive adaptive layouts |

---

## Open Questions (For Later Phases)

- [ ] GPU calculation worth the complexity for simple fractals? (Evaluate post-v1.0 with usage data)
- [ ] Linux/Mac support via Avalonia or MAUI? (MAUI-ready architecture enables this)
- [ ] Web-based version using WebAssembly? (Research phase)
- [ ] Mobile version feasibility? (Cloud rendering likely needed)
- [ ] Cloud rendering for deep zooms? (Could benefit mobile clients)

---

## Notes

**Why This Documentation Was Split:**
- Original DESIGN_PLAN.md exceeded 2,500 lines (🚨 high corruption risk for AI operations)
- Split into 8 files of 200-900 lines each (✅ safe range)
- Easier navigation and parallel editing
- Better git diff readability
- Enables safer AI-assisted development

**File Size Guidelines:**
- ✅ < 500 lines: Very safe
- ⚠️ 500-1,500 lines: Safe with precautions
- 🚨 1,500-3,000 lines: Split if possible
- ❌ > 3,000 lines: High corruption risk

---

## Getting Started

**For detailed information, see:**
1. Start with [Project Analysis](docs/01-project-analysis.md) to understand the existing codebase
2. Review [Architecture](docs/02-architecture.md) for design decisions
3. Follow [Implementation Phases](docs/03-implementation-phases.md) for the development plan
4. Use [Git Strategy](docs/05-git-strategy.md) for branching and commits
5. Read [AI Safety](docs/06-ai-safety.md) before making AI-assisted changes

**Git Repository:** https://github.com/markhassellsmith/ManpLab

**Current Branch:** `development` (integration/testing)  
**Feature Branch:** `feature/add-winui-interface` (WinUI development)
