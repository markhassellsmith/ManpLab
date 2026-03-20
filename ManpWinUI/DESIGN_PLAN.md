# WinUI Interface Modernization Plan

## Overview
This document outlines the plan to modernize the ManpLab application by replatforming the user interface from C++ to WinUI 3. The goal is to preserve what's good and efficient about the existing project while redesigning clunky aspects and building a modern, maintainable UI.

## Modernization Strategy
- **Keep**: Efficient core functionality and proven algorithms
- **Replatform**: User interface from C++ to WinUI 3 / .NET 10
- **Redesign**: Clunky workflows, outdated patterns, and pain points

## Goals
- [ ] Analyze existing C++ interface functionality
- [ ] Identify what to keep vs. what to redesign
- [ ] Design modern WinUI architecture
- [ ] Implement core UI components with best practices
- [ ] Improve user workflows and experience
- [ ] Test and validate functionality

---

## Analysis of ManpWIN64 C++ Project

### Project Structure
**Main Application:** ManpWIN64
- C++ Win32 desktop application with traditional Windows GUI
- 150+ source files implementing fractal mathematics and visualization
- Visual Studio project using v145 platform toolset

**Dependent Libraries (All C++):**
1. **parser** - Custom formula parser/compiler for user-defined fractals
2. **qdlib** - Quad-double precision arithmetic library (~64 decimal digits)
3. **zlib** - Compression library for PNG file support
4. **MPEG** - Video encoding for animation export
5. **External dependencies:**
   - MPFR/MPIR - Arbitrary precision arithmetic (100+ decimal places)
   - PNG library - Image I/O
   - HTML Help - Documentation system

### Core Architecture & Components to **KEEP**

#### ✅ Mathematical Engine (Core Business Logic)
**These are PROVEN, OPTIMIZED algorithms - DO NOT REWRITE**

1. **High-Precision Arithmetic**
   - `BigDouble.cpp/h`, `BigComplex.cpp/h` - MPFR arbitrary precision wrapper
   - `DDComplex.cpp/h`, `DDMatrix.cpp/h` - Double-double precision (~32 digits)
   - `floatexp.cpp/h`, `ExpComplex.cpp/h` - Extended range floating-point
   - `Complex.cpp/h` - Standard double complex numbers
   - **Action:** Wrap as .NET libraries or use P/Invoke

2. **Perturbation Theory Engine** 
   - `PertEngine.cpp/h` - Deep zoom using perturbation algorithm
   - `Approximation.cpp/h` - BLA (Bilinear Approximation) for iteration skipping
   - Reference orbit calculation and delta iteration
   - **Why Keep:** State-of-the-art algorithm enabling zoom depths of 10^100+
   - **Action:** Keep in C++, expose via C++/CLI or COM

3. **Formula Parser** (parser project)
   - `parser.cpp`, `ParserCtx.cpp`, `ParserFn.cpp`
   - Compiles user-defined formulas to bytecode
   - Supports 240+ built-in fractal types
   - **Why Keep:** Complex compiler - hard to replicate
   - **Action:** Keep as native DLL, called from .NET

4. **Fractal Calculation Kernels**
   - `Fractalp.cpp` - 240+ fractal type implementations
   - `BigPixel.cpp` - High-precision pixel calculation
   - `Pixel.cpp` - Standard precision routines
   - Specialized files: `BigMandelDerivatives.cpp`, `BuddhaB rot.cpp`, etc.
   - **Why Keep:** Heavily optimized, mathematically correct
   - **Action:** Keep in C++, minimal interface changes

5. **Coloring Algorithms**
   - `Colour.cpp/h`, `ColourMethod.cpp` - Multiple coloring schemes
   - `Filter.cpp/h` - Tierazon-compatible filters
   - Smooth iteration coloring, potential fields, slope shading
   - **Why Keep:** Complex algorithms, well-tested
   - **Action:** Keep, but modernize palette management for WinUI

#### ✅ Libraries to Keep As-Is
- **qdlib** - Standard library, don't modify
- **zlib** - Standard library, don't modify  
- **MPEG encoder** - Keep for video export

### Components to **REDESIGN** (Clunky/Outdated)

#### ❌ User Interface Layer
**Replace entirely with WinUI 3**

1. **Windows GUI Code** - Traditional Win32/MFC style
   - `Manpwin.cpp` - WndProc message loop
   - `User.cpp` - Dialog-based user input
   - `menu.cpp`, `resource.h` - Old-style menus
   - **Issues:** Non-intuitive, modal dialogs, inflexible layout
   - **Replace with:** WinUI XAML views with MVVM

2. **Drawing/Display**
   - `Dib.cpp/h` - Device Independent Bitmap (Windows GDI)
   - Direct Win32 GDI calls
   - **Issues:** Inefficient, no GPU acceleration
   - **Replace with:** WinUI WriteableBitmap or DirectX interop

3. **File I/O Dialogs**
   - Old Win32 common dialogs
   - **Replace with:** Modern WinUI file pickers

4. **Configuration**  
   - `Config.cpp` - INI-style configuration
   - **Replace with:** JSON/XML with .NET serialization

#### ⚠️ Modernize But Keep Logic

1. **Plot.cpp/h** - Pixel plotting interface
   - **Keep:** Plotting logic and color mapping
   - **Modernize:** Interface to work with WinUI bitmaps

2. **Animation System**
   - `anim.cpp/h`, `Bt.cpp` - Animation engine
   - **Keep:** Animation calculation
   - **Modernize:** Use modern timing and rendering

3. **Zoom.cpp** - Zoom rectangle selection
   - **Keep:** Math for coordinate transformations
   - **Replace:** UI interaction with WinUI gestures/mouse handling

### File Formats to Support
- **.MAP files** - Parameter/fractal state (keep format)
- **.PNG** - Image export (keep)
- **.MPG** - Animation export (keep)
- **.PAR** - Fractint compatibility (keep)
- **.KFR** - Kalles Fraktaler deep zoom (keep)
- **.FRM** - Formula files (keep)

---

## Design Considerations

### Architecture
**Hybrid C++/C# Architecture**
- **C++ Core:** Math engine, parsers, algorithms (existing code)
- **C# WinUI:** User interface, application logic, data binding
- **Interop Layer:** C++/CLI wrapper or native P/Invoke
- **Benefits:** Keep proven algorithms, modern UI, maintain performance

**Recommended Structure:**
```
ManpWinUI (C# WinUI)
├── Views/ (XAML)
├── ViewModels/ (MVVM)
├── Models/ (data structures)
├── Services/ (business logic)
└── Interop/ (C++ wrapper)

ManpCore.Native (C++/CLI wrapper)
└── Wraps existing C++ libraries

ManpWIN64 (existing C++)
├── Math engine
├── Parser
└── Algorithms
```

### UI Components
**Modern WinUI Controls to Use:**
- `NavigationView` - Main app navigation
- `MenuBar` - Top-level menus  
- `CommandBar` - Toolbar with modern icons
- `SplitView` - Side panels for parameters
- `ListView/GridView` - Fractal type selection
- `NumberBox` - Parameter entry with validation
- `Slider` - Real-time parameter adjustment
- `TeachingTip` - Contextual help
- `ContentDialog` - Modal dialogs (replace Win32 dialogs)
- `Image/Canvas` - Fractal display with touch/pen support

**Replace Clunky Patterns:**
- ❌ Modal parameter dialogs → ✅ Side panel with live update
- ❌ Menu-driven workflow → ✅ Direct manipulation + toolbar
- ❌ Clipboard-only sharing → ✅ Modern share contract
- ❌ INI files → ✅ JSON settings with UI

### Data Binding & MVVM
**ViewModels:**
- `MainViewModel` - Overall app state
- `FractalParametersViewModel` - Current fractal settings  
- `RenderViewModel` - Rendering progress/controls
- `PaletteViewModel` - Color palette editor
- `AnimationViewModel` - Animation sequencing

**Observable Properties:**
- Fractal parameters (bind to NumberBox/Slider)
- Rendering progress (bind to ProgressBar)
- Zoom level, coordinates (bind to display)
- Palette colors (bind to color pickers)

**Commands:**
- `RenderCommand` - Start fractal calculation
- `ZoomInCommand`, `ZoomOutCommand`
- `ExportCommand` - Save image/video
- `LoadPresetCommand` - Load .MAP/.PAR files

### Integration Points
**C++ to C# Bridge:**
1. **C++/CLI Wrapper Approach** (Recommended)
   - Create ManpCore.Native project (C++/CLI)
   - Expose managed types to WinUI
   - Direct memory access for performance

2. **P/Invoke Approach** (Alternative)
   - Export C functions from C++ DLL
   - Use `DllImport` in C#
   - More portable but requires marshalling

**Key Interfaces to Expose:**
```csharp
// Fractal calculation
interface IFractalEngine {
    void Calculate(FractalParameters params);
    byte[] GetImageData();
    void Cancel();
}

// Parser
interface IFormulaParser {
    bool ParseFormula(string formula);
    FractalType[] GetAvailableTypes();
}

// Precision arithmetic
interface IArithmeticProvider {
    void SetPrecision(int decimals);
    // Complex number operations
}
```

**Performance Considerations:**
- Use `Memory<byte>` for large buffer transfers
- `Span<T>` for zero-copy operations  
- Async/await for long calculations
- `IProgress<T>` for progress reporting
- CancellationToken for stop functionality


---

## Implementation Tasks

### Phase 1: Planning & Analysis ✅ IN PROGRESS
- [x] Document existing C++ interface features  
- [x] Identify what's good/efficient (keep) vs. clunky (redesign)
- [x] Analyze project dependencies and libraries
- [x] List core algorithms to preserve in C++
- [x] Create detailed C++/C# interop strategy
- [x] Map C++ functionality to WinUI equivalents
- [x] Define modern project structure
- [x] Document file format specifications
- [x] Identify pain points and improvement opportunities

### Phase 2: C++ Core Preparation
- [ ] Extract pure computation code from UI code
- [ ] Create clean C API or C++/CLI wrapper project
- [ ] Define data exchange structures
- [ ] Implement marshalling for complex types
- [ ] Test interop with simple fractal calculation
- [ ] Profile performance of C++/C# boundary
- [ ] Document API for C# developers

### Phase 3: WinUI Foundation
- [ ] Set up MVVM framework (CommunityToolkit.Mvvm)
- [ ] Create base view models and navigation
- [ ] Implement dependency injection (Microsoft.Extensions.DI)
- [ ] Set up async/await patterns for calculations
- [ ] Create basic main window layout
- [ ] Implement settings/configuration system (JSON)
- [ ] Set up logging (Serilog or NLog)

### Phase 4: Core UI Development
- [ ] Main navigation (NavigationView with fractal categories)
- [ ] Fractal display canvas with zoom/pan
- [ ] Parameter entry panel (adaptive for fractal type)
- [ ] Real-time Julia set preview
- [ ] Color palette editor with visual feedback
- [ ] Rendering progress indicator
- [ ] Coordinate display and precision selector
- [ ] Formula editor with syntax highlighting

### Phase 5: Advanced Features
- [ ] Animation sequencer and timeline
- [ ] 3D visualization integration
- [ ] Zoom box/rectangle selection (touch-friendly)
- [ ] Orbit diagram display
- [ ] Batch rendering queue
- [ ] Deep zoom with perturbation UI
- [ ] BLA approximation controls
- [ ] Slope/derivative shading options

### Phase 6: File Operations
- [ ] Modern file pickers (load/save)
- [ ] .MAP file import/export
- [ ] .PAR (Fractint) compatibility
- [ ] .KFR (Kalles) import
- [ ] PNG export with metadata
- [ ] MP4 video export (replace MPEG)
- [ ] Drag-and-drop support
- [ ] Recent files management

### Phase 7: Polish & Features
- [ ] Keyboard shortcuts (accelerators)
- [ ] Context menus (right-click)
- [ ] Undo/redo system
- [ ] Fractal presets/gallery
- [ ] Share contract integration
- [ ] Touch/pen optimization
- [ ] High DPI support
- [ ] Dark/light theme support
- [ ] Accessibility (screen readers, keyboard navigation)
- [ ] Localization preparation

### Phase 8: Testing & Optimization
- [ ] Unit tests for C# ViewModels
- [ ] Integration tests for C++/C# interop
- [ ] UI automation tests
- [ ] Performance profiling
- [ ] Memory leak detection
- [ ] Stress testing (deep zoom, long animations)
- [ ] Multi-threading validation
- [ ] Cross-version file format testing

### Phase 9: Documentation & Deployment
- [ ] User documentation
- [ ] Developer documentation for interop
- [ ] Formula language reference
- [ ] Tutorial for new users
- [ ] API documentation (XML comments)
- [ ] Build/deployment scripts
- [ ] Installer package (MSIX)
- [ ] Update mechanism

---

## Technology Stack

### WinUI / .NET
- **.NET 10** - Latest runtime
- **WinUI 3** - Modern Windows UI framework
- **CommunityToolkit.Mvvm** - MVVM helpers
- **CommunityToolkit.WinUI** - Additional controls
- **Microsoft.Extensions.DependencyInjection** - DI container
- **System.Text.Json** - Settings serialization
- **Serilog / NLog** - Logging framework

### C++ Interop Options
**Option 1: C++/CLI (Recommended)**
- Direct CLR integration
- Easier type marshalling
- Mixed-mode debugging
- Slightly larger binary

**Option 2: Native P/Invoke**
- Standard .NET interop
- Requires careful marshalling
- More portable
- Smaller binary

### Existing C++ Libraries (Keep)
- **MPFR/MPIR** - Arbitrary precision (via existing ManpWIN64)
- **QD Library** - Quad-double precision
- **zlib** - PNG compression  
- **Parser** - Formula compilation
- **Custom Math Libraries** - BigDouble, BigComplex, etc.

### New Libraries to Add
- **FFmpeg / Windows Media Foundation** - Modern video encoding (replace MPEG)
- **SkiaSharp** (optional) - Advanced 2D graphics
- **Windows Community Toolkit** - WinUI extensions

---

## Notes & Decisions

### Key Architectural Decisions

#### 1. Hybrid C++/C# Architecture ✅
**Decision:** Keep C++ math engine, build new WinUI interface in C#  
**Rationale:**
- C++ code is proven, optimized, and mathematically complex
- Rewriting 150+ source files of mathematical code is high-risk
- WinUI provides modern UI without sacrificing performance
- Clear separation between computation (C++) and presentation (C#)

#### 2. C++/CLI Wrapper Layer (Recommended)
**Decision:** Use C++/CLI for interop  
**Alternatives Considered:** Pure P/Invoke, COM, .NET Native AOT
**Rationale:**
- Easier marshalling of complex types (Complex, BigDouble)
- Direct memory access for image buffers
- Better debugging experience
- Can gradually refactor if needed

#### 3. Keep Existing File Formats
**Decision:** Maintain backward compatibility with .MAP, .PAR, .KFR formats  
**Rationale:**
- Users have existing fractal libraries
- Fractint compatibility is a feature
- File parsing is already implemented

#### 4. Modernize Video Export
**Decision:** Replace MPEG-2 encoder with Windows Media Foundation or FFmpeg  
**Rationale:**
- MPEG-2 is outdated (1995 standard)
- Modern codecs: H.264, H.265, VP9 offer better compression
- MP4 container is more widely supported
- Can keep MPEG as legacy export option

### Pain Points Identified in ManpWIN64

#### UI/UX Issues (Priority: HIGH)
1. **Modal Dialog Hell** - Too many nested modal dialogs
   - **Fix:** Side panels with live parameter updates
2. **Non-intuitive Menus** - Deep menu hierarchies
   - **Fix:** Command bar, ribbon, or toolbar with categories
3. **Parameter Entry** - Text-only input with no validation feedback
   - **Fix:** NumberBox controls with range validation + sliders
4. **No Undo** - Can't revert parameter changes easily
   - **Fix:** Command pattern with undo/redo stack
5. **Clipboard-Only Sharing** - No modern share integration
   - **Fix:** Windows Share contract

#### Technical Debt (Priority: MEDIUM)
1. **Global Variables** - Heavy use of global state
   - **Impact:** Hard to test, multithreading issues
   - **Fix:** Encapsulate in service classes with DI
2. **Mixed Responsibilities** - UI code mixed with calculation
   - **Fix:** Strict MVVM separation
3. **INI-style Config** - Difficult to extend
   - **Fix:** JSON with strong typing

#### Performance Opportunities (Priority: LOW)

**1. Display Rendering - Automatic GPU Acceleration ✅**
- **Old (ManpWIN64):** GDI software rendering
- **New (WinUI 3):** Automatic GPU acceleration via DirectX/Composition API
- **Benefit:** 5-10x faster display, smoother zoom/pan, free with WinUI 3
- **Action Required:** None - happens automatically
- **Recommendation:** ✅ Use WinUI 3 (already planned)

**2. Fractal Calculation - Selective GPU Acceleration ⚠️**
- **Note:** Calculation is the bottleneck, not display rendering
- **Complexity:** High - requires DirectCompute/HLSL shader development
- **See Decision Matrix below for per-fractal-type analysis**

**3. Formula Parser - Parallel Compilation**
- **Potential:** Compile multiple formulas in parallel for batch operations
- **Benefit:** Faster startup when loading many custom fractals
- **Priority:** Very low - rarely a bottleneck
- **Recommendation:** ❌ Not worth implementing

---

### GPU Acceleration Decision Matrix

**Context:** This matrix helps decide if/when to add GPU calculation support for specific fractal types. Display rendering already uses GPU automatically via WinUI 3.

#### Matrix: GPU Calculation Benefit by Fractal Type

| Fractal Type | Typical CPU Time (1920x1080, 1K iter) | GPU Potential | Implementation Complexity | Effort vs Gain | Recommend GPU? |
|--------------|---------------------------------------|---------------|---------------------------|----------------|----------------|
| **Standard Mandelbrot** (z² + c) | 2.5s | **30-50x faster** | Medium | ⭐⭐⭐⭐ High gain | ✅ **Consider** |
| **Standard Julia** (z² + c) | 2.3s | **35-55x faster** | Medium | ⭐⭐⭐⭐ High gain | ✅ **Consider** |
| **Burning Ship** | 2.8s | **25-40x faster** | Medium | ⭐⭐⭐ Good gain | ⚠️ Maybe |
| **Polynomial** (z³, z⁴, z⁵ + c) | 3.5s | **20-35x faster** | Medium-High | ⭐⭐⭐ Good gain | ⚠️ Maybe |
| **Newton Fractals** (basic) | 4.2s | **10-15x faster** | High | ⭐⭐ Marginal | ❌ Not worth |
| **Deep Zoom (Perturbation)** | 45s+ | **Not supported** | Impossible | N/A | ❌ Cannot do |
| **High-Precision (MPFR)** | 15s+ | **Not supported** | Impossible | N/A | ❌ Cannot do |
| **Formula Parser** (user formulas) | 8s+ | **Not supported** | Impossible | N/A | ❌ Cannot do |
| **BLA Approximation** | 25s+ | **Not supported** | Impossible | N/A | ❌ Cannot do |
| **Derivatives/Slopes** | 6s+ | **5-8x faster** | Very High | ⭐ Poor gain | ❌ Not worth |

#### Technical Limitations (Why Some Fractals Can't Use GPU)

| Limitation | Affected Fractals | Reason |
|------------|-------------------|--------|
| **Arbitrary Precision** | MPFR, BigDouble, QD | GPU only supports float/double (32/64-bit) |
| **Dynamic Code Execution** | Formula parser | GPU shaders must be compiled ahead of time |
| **Complex Algorithms** | Perturbation, BLA | Require reference orbit, iteration skipping logic |
| **Conditional Logic** | Many advanced types | GPU is inefficient with branches |
| **High Iteration Counts** | Deep zoom (100K+ iter) | GPU timeout limits (2-5 seconds max) |

#### Performance Improvement Estimate by Scenario

**Simple Fractals (Standard Mandelbrot/Julia):**
```
Scenario: 1920x1080, 1000 iterations, standard precision

CPU (C++):          2.5 seconds
GPU (DirectCompute): 0.08 seconds (31x faster) ✅

User Experience: Instant rendering instead of 2-3 second wait
Worth implementing? YES - if users frequently render simple fractals
```

**Deep Zoom with Perturbation:**
```
Scenario: 1920x1080, deep zoom at 10^50 magnification

CPU (C++ Perturbation): 45 seconds
GPU:                    Not possible (algorithm too complex)

User Experience: No improvement possible
Worth implementing? NO - this is the primary use case for power users
```

**High Iteration Count:**
```
Scenario: 1920x1080, 100,000 iterations (deep zoom)

CPU (C++):          180 seconds
GPU:                Timeout (Windows kills GPU tasks after 2-5 seconds)

User Experience: GPU would crash, CPU works fine
Worth implementing? NO - GPU can't handle it
```

#### Implementation Strategy (If Pursuing GPU Acceleration)

**Phase 1: CPU-Only (Recommended for Initial Release)**
- Use WinUI 3's automatic display GPU (free benefit)
- Keep all calculation in C++ (proven, reliable)
- Ship v1.0 without GPU calculation
- **Effort:** 0 hours
- **Benefit:** Stable, works for all fractal types

**Phase 2: Evaluate User Demand (After v1.0 Release)**
- Monitor user feedback and usage patterns
- Identify if users primarily use simple fractals
- Measure actual CPU performance on target hardware
- **Effort:** 2-4 hours (data collection)
- **Decision Point:** Only proceed if >50% of renders are standard Mandelbrot/Julia

**Phase 3: Implement Selective GPU (Optional, Post-v1.0)**
- Add DirectCompute shader for Standard Mandelbrot ONLY
- Automatic fallback to CPU for all other types
- Add UI toggle: "GPU Acceleration (simple fractals only)"
- **Effort:** 40-60 hours
- **Benefit:** 30x faster for simple fractals, 0 benefit for advanced features

```csharp
// Example implementation approach
public class FractalEngineFactory
{
    public IFractalEngine CreateEngine(FractalParameters params, bool allowGpu = true)
    {
        // Only use GPU for very specific cases
        if (allowGpu && IsGpuBeneficial(params))
        {
            return new GpuMandelbrotEngine(); // Handles ONLY standard Mandelbrot
        }
        else
        {
            return new CpuFractalEngine(); // Handles ALL 240 fractal types
        }
    }

    private bool IsGpuBeneficial(FractalParameters params)
    {
        return params.FractalType == "StandardMandelbrot"
            && params.Precision == PrecisionType.Double  // Not BigDouble
            && params.MaxIterations < 10000              // Not deep zoom
            && !params.UsePerturbation                   // Not using perturbation
            && !params.UseFormulaParser                  // Not custom formula
            && !params.UseSlopes;                        // Not using derivatives
    }
}
```

#### Decision Criteria

**✅ Consider GPU Calculation If:**
- [ ] >50% of user renders are Standard Mandelbrot/Julia
- [ ] Users complain about 2-3 second render times
- [ ] Target hardware has discrete GPU (not integrated)
- [ ] Have 40-60 hours available for implementation
- [ ] Willing to maintain GPU shader code long-term

**❌ Skip GPU Calculation If:**
- [ ] Users primarily use deep zoom (perturbation/BLA)
- [ ] Users frequently use high-precision (MPFR)
- [ ] Users rely on formula parser (custom fractals)
- [ ] CPU render times are acceptable (<5 seconds)
- [ ] Development time better spent on UI/features

#### Recommendation for ManpLab WinUI Modernization

**Phase 1-6 (Initial Release):** ❌ **Do NOT implement GPU calculation**

**Rationale:**
1. ManpLab's strength is **advanced features** (perturbation, BLA, high-precision)
2. These features **cannot benefit** from GPU acceleration
3. WinUI 3 already provides **automatic display GPU** (free benefit)
4. **Development effort** better spent on:
   - MVVM architecture
   - Modern UI (NavigationView, parameter panels)
   - File format compatibility
   - Animation timeline
   - Testing and polish

**Phase 7+ (Post-Release Optimization):** ⚠️ **Evaluate based on usage data**

**Decision point after v1.0 release:**
- Collect telemetry: % of renders that are simple Mandelbrot/Julia
- If >50% are simple fractals → consider adding GPU for v1.1
- If <50% are simple fractals → skip GPU, focus elsewhere

**Final Verdict:** 
🎯 **Use WinUI 3's automatic display GPU (free), skip custom calculation GPU acceleration for initial release.**

---

### Improvements Over Original

#### User Experience
- ✅ **Touch/Pen Support** - WinUI native touch gestures
- ✅ **Responsive Layout** - Adaptive UI for different screen sizes
- ✅ **Modern File Dialogs** - Thumbnail previews, recent files
- ✅ **Inline Help** - TeachingTips and tooltips
- ✅ **Real-time Preview** - Live parameter updates (optional)
- ✅ **Drag-and-Drop** - Load files by dropping

#### Features
- ✅ **Better Animation Timeline** - Visual sequencer vs. text-based
- ✅ **Preset Gallery** - Thumbnail browser for fractals
- ✅ **Modern Video Export** - H.264/H.265 support
- ✅ **Cloud Integration** - OneDrive, Google Drive support (future)

#### Developer Experience
- ✅ **MVVM Testability** - Unit test view models
- ✅ **Async/Await** - Cleaner async code vs. Win32 threads
- ✅ **Dependency Injection** - Easier to mock and test
- ✅ **Modern Debugging** - Better tooling in VS 2026

### Migration Strategy

#### Phase-Based Approach & Time Estimates

**Phase 1-3:** Foundation (4-6 weeks)
- Set up projects and interop
- Basic rendering working

**Phase 4-5:** Feature Parity (8-12 weeks)
- Match core ManpWIN64 features
- All fractal types working

**Phase 6-7:** Enhancement (6-8 weeks)
- New UI improvements
- Modern file formats

**Phase 8-9:** Polish (4-6 weeks)
- Testing, docs, deployment

#### Development Time Estimates

**Scenario 1: Part-Time Solo (No AI)**
- **Timeline:** 6-9 months calendar time
- **Effort:** 10-20 hours/week
- **Total Manhours:** 240-720 hours
- **Bottlenecks:** Learning WinUI, C++ interop complexity, testing

**Scenario 2: Part-Time with AI Assistance** ⭐ *Recommended*
- **Timeline:** 3-5 months calendar time
- **Effort:** 10-20 hours/week  
- **Total Manhours:** 120-400 hours
- **AI Benefits:**
  - XAML generation for UI layouts
  - Boilerplate MVVM code (ViewModels, Commands)
  - C++/CLI wrapper code generation
  - Unit test scaffolding
  - Documentation generation
- **Reduction:** ~40-50% time savings

**Scenario 3: Full-Time Solo (No AI)**
- **Timeline:** 2-3 months calendar time
- **Effort:** 40 hours/week
- **Total Manhours:** 320-480 hours

**Scenario 4: Full-Time with AI Assistance**
- **Timeline:** 1-2 months calendar time
- **Effort:** 40 hours/week
- **Total Manhours:** 160-320 hours
- **Notes:** Fastest path to MVP

**What AI Helps Most With:**
- ✅ XAML UI layouts and styling
- ✅ MVVM boilerplate (INotifyPropertyChanged, RelayCommand)
- ✅ C# interop marshalling code
- ✅ Unit test creation
- ✅ Documentation and comments
- ❌ C++ algorithm understanding (still need deep review)
- ❌ Architecture decisions (human judgment required)
- ❌ UX design decisions

#### Parallel Development
- Can keep ManpWIN64 functional during development
- Test new UI against existing calculation engine
- Users can use both versions during transition

### Risk Mitigation

#### Technical Risks
**Risk:** Performance loss due to interop overhead  
**Mitigation:** Profile early, keep large data processing in C++, use `Span<T>` and `Memory<T>`

**Risk:** Complex marshalling of MPFR types  
**Mitigation:** Keep high-precision types in C++, only pass results to C#

**Risk:** Threading issues across C++/C# boundary  
**Mitigation:** Clear ownership model, use .NET thread pool for UI

#### Project Risks  
**Risk:** Scope creep - trying to improve too much  
**Mitigation:** Start with feature parity, then enhance

**Risk:** Breaking existing workflows  
**Mitigation:** User testing, beta program, maintain old version

### Open Questions
- [ ] GPU acceleration worth the complexity?
- [ ] Support for Linux/Mac via Avalonia or MAUI?
- [ ] Web-based version using WebAssembly?
- [ ] Mobile version feasibility?
- [ ] Cloud rendering for deep zooms?

---

## Git Versioning & Commit Strategy

### Branch Strategy

#### Main Branches
```
main (or master)
├── Production-ready code (C++ ManpWIN64)
└── Stable releases only (tagged: v1.0.0, v1.1.0, etc.)

development ⭐ INTEGRATION/TESTING BRANCH
├── All completed work merges here FIRST
├── C++ bugfixes go here
├── Test integration before releasing to main
└── Base for ALL feature branches

feature/add-winui-interface (your current branch)
├── Long-running feature branch for WinUI development
├── Based on development
├── Regularly merges FROM development (to get C++ fixes)
└── Merges TO development when phases complete
```

**🎯 Key Workflow:**
```
1. C++ bugfix → commit to development
2. development → merge into feature/add-winui-interface (you get the fix)
3. Work on WinUI → commit to feature/add-winui-interface
4. Phase complete → merge feature/add-winui-interface → development (integration test)
5. All phases done → merge development → main (production release)
```

#### Feature Branch Naming Convention
```
feature/add-winui-interface          # Main WinUI branch (YOUR BRANCH)
├── feature/phase1-planning          # Optional sub-branches per phase
├── feature/phase2-cpp-interop
├── feature/phase3-mvvm-foundation
├── feature/phase4-core-ui
├── feature/phase5-advanced-features
├── feature/phase6-file-operations
├── feature/phase7-polish
├── feature/phase8-testing
└── feature/phase9-deployment

hotfix/*                             # Critical C++ fixes during development
docs/*                               # Documentation updates
```

#### 🚀 Setting Up `development` Branch (If Not Created Yet)

**First-Time Setup:**
```powershell
# 1. Make sure main is up to date
git checkout main
git pull origin main

# 2. Create development from main
git checkout -b development
git push origin development

# 3. Rebase your feature branch on development
git checkout feature/add-winui-interface
git rebase development

# 4. Set development as default base for future branches
git config branch.feature/add-winui-interface.merge refs/heads/development
```

**If `development` Already Exists:**
```powershell
# Just rebase your feature branch
git checkout development
git pull origin development
git checkout feature/add-winui-interface
git rebase development
```

### Commit Strategy by Phase

#### Phase 1: Planning & Analysis (10-15 commits)
**Branch:** `feature/add-winui-interface` (already created) or `feature/phase1-planning`

```
Commit examples:
✅ docs: add DESIGN_PLAN.md with project analysis
✅ docs: document C++ components to keep vs redesign
✅ docs: add interop strategy and architecture decisions
✅ docs: create file format specification
✅ docs: complete Phase 1 checklist

Commit pattern: docs: <what was documented>
```

**Merge point:** After Phase 1 complete → merge to `development` with tag `v0.1.0-planning`

```powershell
# After Phase 1 complete
git checkout development
git merge feature/add-winui-interface
git tag v0.1.0-planning
git push origin development --tags

# Continue working on feature branch
git checkout feature/add-winui-interface
```

---

#### Phase 2: C++ Core Preparation (20-30 commits)
**Branch:** `feature/phase2-cpp-interop`

```
Commit examples:
feat(cpp): extract calculation engine interface
feat(cpp): create ManpCore.Native C++/CLI project
feat(interop): define FractalParameters data structure
feat(interop): implement Complex type marshalling
feat(interop): add BigDouble wrapper with tests
test(interop): benchmark C++/C# boundary performance
docs(api): document C++ interop API

Commit pattern: 
- feat(cpp): <C++ changes>
- feat(interop): <interop layer changes>
- test(interop): <tests>
```

**Merge point:** After Phase 2 complete → merge to `feature/add-winui-interface` with tag `v0.2.0-interop`

```powershell
# If using sub-branch for Phase 2
git checkout feature/add-winui-interface
git merge feature/phase2-cpp-interop
git tag v0.2.0-interop
git push origin feature/add-winui-interface --tags
```

---

#### Phase 3: WinUI Foundation (15-25 commits)
**Branch:** `feature/phase3-mvvm-foundation`

```
Commit examples:
feat(winui): add CommunityToolkit.Mvvm package
feat(mvvm): create base ViewModelBase class
feat(di): configure dependency injection container
feat(ui): create MainWindow basic layout
feat(config): implement JSON settings service
feat(logging): add Serilog with file output
test(mvvm): add ViewModel unit tests

Commit pattern:
- feat(winui): <WinUI project setup>
- feat(mvvm): <MVVM infrastructure>
- feat(di): <dependency injection>
- feat(ui): <basic UI>
```

**Merge point:** After Phase 3 complete → merge to `feature/add-winui-interface` with tag `v0.3.0-foundation`

```powershell
git checkout feature/add-winui-interface
git merge feature/phase3-mvvm-foundation
git tag v0.3.0-foundation
git push origin feature/add-winui-interface --tags

# Then merge to development for integration testing
git checkout development
git merge feature/add-winui-interface
git push origin development
```

---

#### Phase 4: Core UI Development (30-50 commits)
**Branch:** `feature/phase4-core-ui`

```
Commit examples:
feat(ui): add NavigationView with fractal categories
feat(ui): implement fractal display canvas
feat(ui): create parameter panel with NumberBox controls
feat(viewmodel): add FractalParametersViewModel
feat(ui): add real-time Julia preview
feat(ui): implement color palette editor
feat(ui): add rendering progress indicator
style(ui): apply Fluent Design theme
test(ui): add UI automation tests for navigation

Commit pattern:
- feat(ui): <UI component>
- feat(viewmodel): <ViewModel>
- style(ui): <styling>
```

**Merge point:** After Phase 4 complete → merge to `feature/add-winui-interface` with tag `v0.4.0-core-ui`

**Milestone:** 🎉 **First usable prototype** - can render basic fractals

```powershell
# Phase 4 complete - major milestone!
git checkout feature/add-winui-interface
git merge feature/phase4-core-ui
git tag v0.4.0-core-ui
git push origin feature/add-winui-interface --tags

# Merge to development for integration testing
git checkout development
git merge feature/add-winui-interface
git push origin development
```

---

#### Phase 5: Advanced Features (25-40 commits)
**Branch:** `feature/phase5-advanced-features`

```
Commit examples:
feat(animation): add animation timeline control
feat(ui): implement zoom box selection with touch
feat(3d): integrate 3D visualization
feat(perturbation): add deep zoom UI controls
feat(bla): add BLA approximation settings panel
feat(slope): implement slope shading options

Commit pattern:
- feat(animation): <animation features>
- feat(perturbation): <deep zoom>
- feat(<feature>): <specific feature>
```

**Merge point:** After Phase 5 complete → merge to `feature/add-winui-interface` with tag `v0.5.0-advanced`

---

#### Phase 6: File Operations (15-25 commits)
**Branch:** `feature/phase6-file-operations`

```
Commit examples:
feat(file): add WinUI file picker integration
feat(file): implement .MAP file import/export
feat(file): add .PAR (Fractint) compatibility
feat(file): implement .KFR (Kalles) reader
feat(export): add PNG export with metadata
feat(export): replace MPEG with H.264 encoder
feat(ui): add drag-and-drop file support
feat(ui): implement recent files menu

Commit pattern:
- feat(file): <file I/O>
- feat(export): <export functionality>
```

**Merge point:** After Phase 6 complete → merge to `feature/add-winui-interface` with tag `v0.6.0-files`

**Milestone:** 🎉 **Feature complete** - all major features implemented

```powershell
# Feature complete!
git checkout feature/add-winui-interface
git merge feature/phase6-file-operations
git tag v0.6.0-files
git push origin feature/add-winui-interface --tags

# Merge to development
git checkout development
git merge feature/add-winui-interface
git push origin development
```

---

#### Phase 7: Polish & Features (20-35 commits)
**Branch:** `feature/phase7-polish`

```
Commit examples:
feat(ux): add keyboard shortcuts (Ctrl+R, Ctrl+Z, etc)
feat(ui): implement context menus
feat(undo): add undo/redo system
feat(presets): create fractal gallery
feat(share): integrate Windows Share contract
perf(ui): optimize touch/pen input
feat(theme): add dark/light theme toggle
a11y(ui): add screen reader support
i18n: prepare for localization

Commit pattern:
- feat(ux): <UX improvements>
- perf(ui): <performance>
- a11y(ui): <accessibility>
- i18n: <internationalization>
```

**Merge point:** After Phase 7 complete → merge to `feature/add-winui-interface` with tag `v0.7.0-polish`

---

#### Phase 8: Testing & Optimization (20-30 commits)
**Branch:** `feature/phase8-testing`

```
Commit examples:
test(viewmodel): add comprehensive ViewModel tests
test(interop): add C++/C# integration tests
test(ui): expand UI automation coverage
perf: profile and optimize rendering pipeline
fix(memory): resolve memory leak in bitmap handling
test(stress): add deep zoom stress tests
test(thread): validate multi-threading safety
test(file): add cross-version file format tests

Commit pattern:
- test(<component>): <tests added>
- perf: <optimization>
- fix(<issue>): <bug fix>
```

**Merge point:** After Phase 8 complete → merge to `feature/add-winui-interface` with tag `v0.8.0-tested`

**Milestone:** 🎉 **Release Candidate** - ready for beta testing

```powershell
# Release candidate!
git checkout feature/add-winui-interface
git merge feature/phase8-testing
git tag v0.8.0-tested
git push origin feature/add-winui-interface --tags

# Final merge to development for pre-release testing
git checkout development
git merge feature/add-winui-interface
git push origin development
```

---

#### Phase 9: Documentation & Deployment (10-20 commits)
**Branch:** `feature/phase9-deployment`

```
Commit examples:
docs: add user manual
docs: create developer interop guide
docs: write formula language reference
docs: add getting started tutorial
docs(api): complete XML documentation
build: add MSIX packaging configuration
build: configure release pipeline
ci: add GitHub Actions workflow
release: prepare v1.0.0

Commit pattern:
- docs: <documentation>
- build: <build/deployment>
- ci: <CI/CD>
- release: <release prep>
```

**Merge point:** After Phase 9 complete → merge to `development` then to `main` with tag `v1.0.0`

**Milestone:** 🎉 **Version 1.0 Release** - Production ready!

---

### Semantic Versioning Scheme

**During Development:**
- `v0.1.0-planning` - Phase 1 complete
- `v0.2.0-interop` - Phase 2 complete
- `v0.3.0-foundation` - Phase 3 complete
- `v0.4.0-core-ui` - Phase 4 complete (first prototype)
- `v0.5.0-advanced` - Phase 5 complete
- `v0.6.0-files` - Phase 6 complete (feature complete)
- `v0.7.0-polish` - Phase 7 complete
- `v0.8.0-tested` - Phase 8 complete (RC)
- `v0.9.0-beta1`, `v0.9.1-beta2`, etc. - Beta releases
- `v1.0.0` - First production release

**After Release:**
- `v1.0.x` - Patch releases (bug fixes)
- `v1.1.x` - Minor releases (new features, backward compatible)
- `v2.0.x` - Major releases (breaking changes)

---

### Commit Message Format (Conventional Commits)

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation
- `style`: Formatting, missing semicolons, etc.
- `refactor`: Code restructuring
- `perf`: Performance improvement
- `test`: Adding tests
- `build`: Build system changes
- `ci`: CI/CD changes
- `chore`: Maintenance tasks
- `revert`: Revert a previous commit

**Scopes:**
- `cpp`: C++ code changes
- `interop`: C++/CLI wrapper
- `winui`: WinUI project
- `ui`: User interface
- `mvvm`: ViewModels/MVVM
- `di`: Dependency injection
- `file`: File I/O
- `export`: Export functionality
- `animation`: Animation system
- `perturbation`: Deep zoom
- `api`: API changes

**Examples:**
```
feat(interop): add BigComplex marshalling support

Implements two-way marshalling between C++ BigComplex
and managed Complex types with proper memory management.

Closes #42

---

fix(ui): resolve rendering flicker on zoom

The fractal display was flickering during zoom operations
due to incorrect bitmap buffer handling.

Fixes #108

---

perf(cpp): optimize perturbation iteration loop

Reduced memory allocations in hot path by 40%.
Benchmark shows 15% performance improvement.

---

docs: complete Phase 1 planning documentation

Added comprehensive analysis of C++ components,
architecture decisions, and implementation roadmap.
```

---

### Merge Strategy

**Option 1: Squash Merge (Recommended for small phases)**
- Clean linear history
- One commit per phase in main branch
- Good for: Phases 1-3, 6, 9

**Option 2: Merge Commit (Recommended for large phases)**
- Preserve detailed commit history
- Shows all work done
- Good for: Phases 4, 5, 7, 8

**Option 3: Rebase and Merge**
- Linear history with detailed commits
- Requires careful rebasing
- Good for: Individual features within phases

---

### Handling Dual Codebase (ManpWIN64 + ManpWinUI)

#### Directory Structure in Git
```
ManpLab/
├── ManpWIN64/              # Legacy C++ (keep maintained)
├── parser/                 # Shared C++ library
├── qdlib/                  # Shared C++ library
├── zlib/                   # Shared C++ library
├── MPEG/                   # Shared C++ library
├── ManpCore.Native/        # NEW: C++/CLI wrapper (Phase 2)
├── ManpWinUI/              # NEW: WinUI app (Phase 3+)
├── .github/workflows/      # CI/CD
├── docs/                   # Documentation
└── README.md
```

#### Commit Guidelines for Shared C++ Code
```bash
# If you modify shared C++ libraries:
git commit -m "feat(cpp/parser): add new formula function

This change affects both ManpWIN64 and ManpWinUI.
Tested in both codebases.

Related: #feature/add-winui-interface"
```

#### Syncing ManpWIN64 Improvements
```bash
# Apply C++ fixes to both codebases
git checkout development
git pull origin development

# Make fix to ManpWIN64
git commit -m "fix(cpp): resolve parser memory leak"
git push origin development

# Ensure it's available for WinUI branch
git checkout feature/add-winui-interface
git merge development  # Pull C++ fix into your feature branch
git push origin feature/add-winui-interface
```

**Why This Matters:**
- C++ fixes go to `development` (affects both old and new code)
- You merge `development` into your feature branch to get those fixes
- Keeps your WinUI work up-to-date with C++ improvements

---

### Recommended Daily Workflow

```bash
# Start of work session
git checkout feature/add-winui-interface
git pull origin feature/add-winui-interface

# Get any new C++ fixes from development
git checkout development
git pull origin development
git checkout feature/add-winui-interface
git merge development  # Integrate C++ fixes

# Create sub-branch for specific work (optional)
git checkout -b feature/phase4-parameter-panel

# Make changes, commit frequently
git add .
git commit -m "feat(ui): add parameter panel NumberBox controls"

# More work...
git commit -m "feat(viewmodel): bind parameters to FractalParametersViewModel"

# End of work session - push to remote
git push origin feature/phase4-parameter-panel

# When sub-feature complete, merge back
git checkout feature/add-winui-interface
git merge feature/phase4-parameter-panel
git push origin feature/add-winui-interface
```

**Daily Sync Checklist:**
- ✅ Pull latest feature/add-winui-interface
- ✅ Check development for new C++ fixes
- ✅ Merge development if needed
- ✅ Work on feature
- ✅ Commit frequently (every 15-30 min)
- ✅ Push at end of session

---

### Milestone Checklist

**Before Each Merge to `development`:**
- [ ] All phase tasks checked off in DESIGN_PLAN.md
- [ ] Code builds without errors
- [ ] Tests pass (if tests exist for that phase)
- [ ] Documentation updated
- [ ] README.md reflects current state
- [ ] Tag created with version number
- [ ] PR created with phase summary

**Before v1.0.0 Release:**
- [ ] All 9 phases complete
- [ ] Feature parity with ManpWIN64 achieved
- [ ] User documentation complete
- [ ] MSIX package created and tested
- [ ] Beta testing complete
- [ ] Known issues documented
- [ ] Migration guide written (C++ → WinUI)

---

## Working Safely with AI - Corruption Prevention

### Why This Matters
- **Large files** (like this DESIGN_PLAN.md) can overwhelm AI context windows
- **AI engine crashes** can corrupt in-progress edits
- **Lost work** is frustrating and wastes time
- **Git history** is your safety net

---

### Golden Rules for AI-Assisted Development

#### 1. **Commit Early, Commit Often** ⭐ MOST IMPORTANT
```bash
# After EVERY successful AI edit session (even small ones):
git add .
git commit -m "docs: update DESIGN_PLAN.md with [what changed]"
git push origin feature/add-winui-interface

# Frequency: Every 15-30 minutes of work
# Benefit: Maximum 30 minutes of work lost if crash occurs
```

**Why:** If AI crashes mid-edit, you can always `git reset --hard` to last commit.

---

#### 2. **Work in Small Chunks**
**Bad (risky):**
- "Update all 9 phase sections at once"
- "Rewrite the entire architecture section"
- "Make 50 changes across 10 files"

**Good (safe):**
- "Add Phase 2 commit examples to Git section"
- "Update MAUI section with file I/O patterns"
- "Create 3 new service interfaces in one commit"

**Rule of Thumb:**
- **Single file edits:** Safe for files < 2000 lines
- **Multi-file edits:** Max 3-5 files at a time
- **Large refactors:** Break into multiple sessions

---

#### 3. **File Size Awareness**

**Current File Sizes:**
```
DESIGN_PLAN.md: ~1,400 lines (approaching risk threshold)
```

**Risk Levels:**
- ✅ **< 500 lines:** Very safe
- ⚠️ **500-1,500 lines:** Safe with precautions
- 🚨 **1,500-3,000 lines:** Higher risk, split if possible
- ❌ **> 3,000 lines:** High corruption risk

**Action for DESIGN_PLAN.md:**
If this file exceeds 2,000 lines, consider splitting:
```
DESIGN_PLAN.md (overview + links)
├── docs/architecture.md
├── docs/implementation-phases.md
├── docs/git-strategy.md
├── docs/maui-compatibility.md
└── docs/ai-safety.md
```

---

#### 4. **Session Management Strategy**

**Start of Session:**
```bash
# 1. Pull latest changes
git checkout feature/add-winui-interface
git pull origin feature/add-winui-interface

# 2. Sync with development (get C++ fixes)
git checkout development
git pull origin development
git checkout feature/add-winui-interface
git merge development

# 3. Create session branch (optional but recommended)
git checkout -b session/2025-01-15-phase2-work

# 4. Verify clean state
git status
```

**During Session:**
```bash
# After each successful AI interaction:
git add -A
git commit -m "wip: [brief description]"

# Every 30 minutes:
git push origin session/2025-01-15-phase2-work
```

**End of Session:**
```bash
# 1. Review changes
git log --oneline -5

# 2. Squash WIP commits (optional)
git rebase -i HEAD~5  # Combine "wip:" commits

# 3. Merge to main branch
git checkout feature/add-winui-interface
git merge session/2025-01-15-phase2-work

# 4. Push
git push origin feature/add-winui-interface

# 5. Delete session branch
git branch -d session/2025-01-15-phase2-work
```

---

#### 5. **Backup Before Major Edits**

**Before large refactors:**
```bash
# Create a backup branch
git branch backup/before-phase3-refactor

# Or tag current state
git tag backup-2025-01-15

# If something goes wrong:
git reset --hard backup/before-phase3-refactor
# or
git reset --hard backup-2025-01-15
```

**Before editing critical files:**
```bash
# Copy file outside git
cp ManpWinUI/DESIGN_PLAN.md ../DESIGN_PLAN.md.backup

# If AI corrupts it:
cp ../DESIGN_PLAN.md.backup ManpWinUI/DESIGN_PLAN.md
```

---

#### 6. **AI Crash Recovery Procedures**

**If AI crashes during file edit:**

**Option A: Revert to last commit (safest)**
```bash
# Check what changed
git status
git diff ManpWinUI/DESIGN_PLAN.md

# If file is corrupted or incomplete:
git checkout HEAD -- ManpWinUI/DESIGN_PLAN.md

# Verify
git status
```

**Option B: Manual recovery (if some work is salvageable)**
```bash
# 1. Copy corrupted file
cp ManpWinUI/DESIGN_PLAN.md ~/corrupted-backup.md

# 2. Restore clean version
git checkout HEAD -- ManpWinUI/DESIGN_PLAN.md

# 3. Manually copy salvageable parts from corrupted-backup.md
# Use diff tool to compare
```

**Option C: Use Git reflog (if you committed before crash)**
```bash
# Find commit before crash
git reflog

# Example output:
# a1b2c3d HEAD@{0}: commit: wip: added phase 2 details
# e4f5g6h HEAD@{1}: commit: docs: update git strategy

# Reset to good commit
git reset --hard e4f5g6h
```

---

#### 7. **Preventing Context Window Overload**

**Signs AI is struggling:**
- Responses getting slower
- Partial/incomplete edits
- Repeated "I apologize" messages
- Missing context from earlier in conversation

**Solutions:**
1. **Start fresh conversation** every 10-15 exchanges
2. **Reference specific line numbers** instead of large blocks
3. **Break tasks smaller:** "Add section X" vs "Rewrite entire file"
4. **Use multiple tools:**
   - AI for generation
   - Manual editing for simple changes
   - Git for version control

---

#### 8. **Multi-File Edit Safety**

**When making changes across multiple files:**

```bash
# 1. List files to change
echo "Files to edit:
- ManpWinUI/App.xaml.cs
- ManpWinUI/MainWindow.xaml
- ManpWinUI/ViewModels/MainViewModel.cs"

# 2. Edit ONE file at a time with AI
# 3. Commit after EACH file
git add ManpWinUI/App.xaml.cs
git commit -m "feat(winui): configure DI container in App.xaml.cs"

git add ManpWinUI/MainWindow.xaml
git commit -m "feat(ui): add basic layout to MainWindow"

git add ManpWinUI/ViewModels/MainViewModel.cs
git commit -m "feat(mvvm): create MainViewModel with commands"

# 4. If AI crashes, only lose current file (not all 3)
```

---

#### 9. **AI-Assisted Editing Best Practices**

**Good prompts (specific, bounded):**
- ✅ "Add a new section after line 450 titled 'C++ Interop Strategy'"
- ✅ "Update Phase 2 checklist to include 3 new tasks"
- ✅ "Replace the file I/O example at lines 200-220 with async version"

**Bad prompts (vague, unbounded):**
- ❌ "Improve the entire document"
- ❌ "Add examples to all sections"
- ❌ "Reorganize everything for better flow"

**When AI suggests large changes:**
```
User: "I need to add MAUI compatibility info"
AI: "I can add a comprehensive MAUI section with 15 subsections..."

STOP! Ask AI to:
1. Create outline first (small edit)
2. Commit outline
3. Fill in sections one-by-one (small edits)
4. Commit after each section
```

---

#### 10. **File Corruption Detection**

**After AI edits, always verify:**

```bash
# 1. Check file is valid markdown
code ManpWinUI/DESIGN_PLAN.md  # Visual Studio Code has MD preview

# 2. Check file size didn't drastically change
git diff --stat

# Example good output:
# ManpWinUI/DESIGN_PLAN.md | 45 +++++++++++++++++++++++++++++++++++

# Example bad output (possible corruption):
# ManpWinUI/DESIGN_PLAN.md | 2000 ++++++++++++++++-----------------

# 3. Scan for common corruption signs
grep "undefined" ManpWinUI/DESIGN_PLAN.md
grep "null" ManpWinUI/DESIGN_PLAN.md
grep "```" ManpWinUI/DESIGN_PLAN.md | wc -l  # Should be even number
```

**Corruption Indicators:**
- File size suddenly doubled or halved
- Odd number of code fences (```)
- Duplicate sections
- Truncated/incomplete sentences at end
- Missing headings (broken structure)

---

#### 11. **Emergency Rollback Procedures**

**Nuclear option (revert all uncommitted changes):**
```bash
# See what would be lost
git status
git diff

# Point of no return
git reset --hard HEAD
git clean -fd  # Remove untracked files

# Verify
git status  # Should say "nothing to commit, working tree clean"
```

**Surgical option (revert specific file):**
```bash
# Revert one file to last commit
git checkout HEAD -- ManpWinUI/DESIGN_PLAN.md

# Revert to specific commit
git checkout a1b2c3d -- ManpWinUI/DESIGN_PLAN.md
```

**Time travel (go back to earlier state):**
```bash
# View commit history
git log --oneline --graph --all

# Create new branch from earlier commit
git checkout -b recovery/phase1-state a1b2c3d

# Compare current vs old
git diff feature/winui-modernization recovery/phase1-state

# If old state is better:
git checkout feature/add-winui-interface
git reset --hard recovery/phase1-state
git push --force origin feature/add-winui-interface  # ⚠️ Use with caution
```

---

#### 12. **Recommended Workflow for This Project**

**Daily Work Cycle:**

```bash
# MORNING: Start fresh
git checkout development
git pull origin development
git checkout feature/add-winui-interface
git pull origin feature/add-winui-interface
git merge development  # Get latest C++ fixes

git checkout -b session/2025-01-15-am

# WORK SESSION 1 (30-45 minutes)
# - Make 3-5 small commits
# - Push every 30 min

git push origin session/2025-01-15-am

# BREAK (15 minutes)

# WORK SESSION 2 (30-45 minutes)
# - Continue on session branch
# - Make 3-5 more commits

git push origin session/2025-01-15-am

# AFTERNOON: Merge to main branch
git checkout feature/add-winui-interface
git merge --squash session/2025-01-15-am
git commit -m "feat(phase2): complete C++ interop design"
git push origin feature/add-winui-interface
git branch -d session/2025-01-15-am
```

**Benefits:**
- ✅ Work is always backed up to GitHub
- ✅ Session branches isolate experimental work
- ✅ Easy to discard bad session without affecting main branch
- ✅ Clean commit history on main branch

---

#### 13. **AI Conversation Management**

**Signs you need to start fresh conversation:**
- AI is forgetting context from earlier
- Responses reference non-existent files
- AI suggests changes to wrong file
- Token limit warnings

**How to transition:**
```
Current conversation (at limit):
User: "We've been working for 2 hours, let's checkpoint"
AI: [provides summary]

NEW conversation:
User: "Continuing from previous session. We're working on
Phase 2 of the WinUI modernization project. Last we did X.
Next I need to do Y. Here's the current state: [brief context]"
```

**Context Preservation:**
- Keep DESIGN_PLAN.md up to date (it's your context)
- Commit frequently (Git is your memory)
- Take notes in comments or TODO.md
- Reference specific line numbers or section titles

---

#### 14. **Testing AI Edits**

**Before committing AI-generated code:**

```bash
# For C# code:
dotnet build ManpWinUI/ManpWinUI.csproj

# For C++ code:
msbuild ManpWIN64/ManpWIN64.vcxproj /t:Build

# For markdown:
# Open in VS Code markdown preview
code --preview ManpWinUI/DESIGN_PLAN.md
```

**Checklist:**
- [ ] File compiles/builds (if code)
- [ ] Markdown renders correctly (if docs)
- [ ] No obvious syntax errors
- [ ] Indentation looks correct
- [ ] Code follows project style
- [ ] No "TODO" or placeholder comments from AI

---

#### 15. **Recovery Contacts**

**If you get stuck:**

1. **Git reflog:** `git reflog` - see every state your repo has been in
2. **GitHub web interface:** Browse commits, download specific file versions
3. **Local backups:** Check `../backup/` folder (if you created one)
4. **Visual Studio:** "Local History" feature (if enabled)

**Prevention > Recovery:**
- Commit every 15-30 minutes ⭐
- Push to GitHub hourly ⭐
- Keep backups of critical files ⭐

---

### Quick Reference Card

```
┌─────────────────────────────────────────────────┐
│  AI SAFETY CHECKLIST - Keep This Handy!        │
├─────────────────────────────────────────────────┤
│                                                 │
│  Before AI Edit Session:                       │
│  [ ] git status (clean?)                       │
│  [ ] git pull (up to date?)                    │
│  [ ] Create session branch                     │
│                                                 │
│  During AI Session:                            │
│  [ ] Work in small chunks (< 50 lines/edit)    │
│  [ ] Commit after each successful edit         │
│  [ ] Push every 30 minutes                     │
│  [ ] Verify file isn't corrupted               │
│                                                 │
│  After AI Session:                             │
│  [ ] Review git diff                           │
│  [ ] Test builds (if code)                     │
│  [ ] Squash WIP commits                        │
│  [ ] Push to main branch                       │
│  [ ] Delete session branch                     │
│                                                 │
│  If AI Crashes:                                │
│  1. git status                                 │
│  2. git diff (salvageable?)                    │
│  3. git checkout HEAD -- <file> (if corrupted) │
│  4. Start fresh conversation                   │
│                                                 │
└─────────────────────────────────────────────────┘
```

---

### File Split Plan (For Future)

**When DESIGN_PLAN.md reaches 2,000 lines, split into:**

```
docs/
├── README.md (overview, links to all docs)
├── 01-project-analysis.md (Current "Analysis" section)
├── 02-architecture.md (Design Considerations)
├── 03-implementation-phases.md (9 phases)
├── 04-technology-stack.md (Tech stack, libraries)
├── 05-git-strategy.md (Versioning, commits)
├── 06-ai-safety.md (This section)
├── 07-maui-compatibility.md (MAUI section)
└── 08-references.md (Links, resources)
```

**Benefits:**
- ✅ Smaller files = safer AI edits
- ✅ Easier to navigate
- ✅ Parallel work on different sections
- ✅ Faster AI processing

**How to split:**
```bash
# Create docs folder
mkdir docs

# Move sections to separate files
# (Do this manually or with AI, one file at a time)

# Update main DESIGN_PLAN.md with links
echo "See [Architecture](docs/02-architecture.md)" >> DESIGN_PLAN.md

# Commit each new file separately
git add docs/01-project-analysis.md
git commit -m "docs: extract project analysis to separate file"
```

---

**Remember:** Git is your time machine. Commit often, push frequently, work in small chunks. 🚀

---

## MAUI Compatibility Considerations

### Overview
While the immediate target is **Windows via WinUI 3**, designing with **.NET MAUI** (Multi-platform App UI) in mind will enable future expansion to macOS, iOS, Android, and Linux with minimal refactoring.

### Why MAUI-Ready Architecture Matters
- **Code Reuse:** Share 90%+ of business logic, ViewModels, and models
- **Platform Expansion:** Easier path to mobile/tablet versions
- **Future-Proofing:** Microsoft is unifying around MAUI for cross-platform
- **Cloud Rendering:** Mobile clients could leverage desktop/cloud for heavy calculations

---

### Architecture Decisions for MAUI Compatibility

#### ✅ **DO: Write Platform-Agnostic Code**

**1. Separate Business Logic from UI**
```csharp
// ✅ GOOD - Platform agnostic
public class FractalService : IFractalService
{
    public async Task<FractalResult> CalculateAsync(FractalParameters parameters)
    {
        // Pure business logic, no UI dependencies
    }
}

// ❌ BAD - WinUI specific
public class FractalService
{
    private WriteableBitmap _bitmap; // WinUI-specific type
}
```

**2. Use Abstraction Layers for Platform Services**
```csharp
// Define interfaces in shared code
public interface IFileService
{
    Task<Stream> PickFileAsync(string[] extensions);
    Task<bool> SaveFileAsync(string filename, byte[] data);
}

// Implement for WinUI
public class WinUIFileService : IFileService { ... }

// Implement for MAUI later
public class MauiFileService : IFileService { ... }
```

**3. ViewModels Should Be 100% Platform-Independent**
```csharp
// ✅ GOOD - Works in both WinUI and MAUI
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _fractalName;

    [RelayCommand]
    private async Task RenderAsync()
    {
        await _fractalService.CalculateAsync(...);
    }
}

// ❌ BAD - WinUI specific
public class MainViewModel
{
    private DispatcherQueue _dispatcher; // WinUI only
}
```

---

#### ❌ **AVOID: Platform-Specific Dependencies in Shared Code**

**What to Avoid:**
- Direct WinUI control references in ViewModels
- `Microsoft.UI.Xaml.*` namespaces in business logic
- Windows-specific APIs (`Windows.Storage.*` in shared code)
- WinUI-specific threading (`DispatcherQueue`)
- Platform-specific DI container registrations mixed with business services

**Where Platform-Specific Code is OK:**
- Views (`.xaml` files) - these will be platform-specific anyway
- Platform service implementations
- App startup / bootstrapping
- Native interop wrappers

---

### Recommended Project Structure for MAUI-Ready Code

```
ManpLab.sln
├── ManpLab.Core/                    # ✅ Shared .NET Standard 2.1 or .NET 10
│   ├── Models/                      # Data structures (FractalParameters, etc.)
│   ├── ViewModels/                  # Platform-agnostic ViewModels
│   ├── Services/                    # Business logic services
│   │   ├── Interfaces/              # IFractalService, IFileService, etc.
│   │   └── Implementations/         # Platform-agnostic implementations
│   ├── Interop/                     # C++ wrapper interfaces
│   └── Utilities/                   # Helpers, converters, etc.
│
├── ManpLab.WinUI/                   # 🪟 Windows WinUI 3 app
│   ├── Views/                       # WinUI XAML views
│   ├── Services/                    # WinUI-specific implementations
│   │   ├── WinUIFileService.cs
│   │   ├── WinUIBitmapService.cs
│   │   └── WinUINavigationService.cs
│   ├── App.xaml.cs                  # WinUI app startup
│   └── Platforms/Windows/           # Windows-specific code
│
├── ManpLab.Maui/ (future)           # 📱 MAUI multi-platform app
│   ├── Views/                       # MAUI XAML views
│   ├── Services/                    # MAUI-specific implementations
│   │   ├── MauiFileService.cs
│   │   ├── MauiBitmapService.cs
│   │   └── MauiNavigationService.cs
│   ├── Platforms/                   # Platform-specific code
│   │   ├── Android/
│   │   ├── iOS/
│   │   ├── MacCatalyst/
│   │   └── Windows/
│   └── MauiProgram.cs               # MAUI app startup
│
├── ManpCore.Native/                 # C++/CLI wrapper
└── ManpWIN64/                       # Existing C++ engine
```

**Key Points:**
- **ManpLab.Core** - 100% shared between WinUI and MAUI
- **ManpLab.WinUI** - Windows-specific UI and services
- **ManpLab.Maui** (future) - Cross-platform UI, references Core
- Both UI projects reference the **same Core library**

---

### Service Interfaces to Abstract

Define these in `ManpLab.Core/Services/Interfaces/`:

```csharp
// File operations
public interface IFileService
{
    Task<Stream> OpenFileAsync(string[] fileTypes);
    Task<bool> SaveFileAsync(string path, byte[] data);
    Task<string[]> GetRecentFilesAsync();
}

// Bitmap/image handling
public interface IBitmapService
{
    object CreateBitmap(int width, int height);
    void UpdatePixels(object bitmap, byte[] pixels);
    Task<bool> SaveBitmapAsync(object bitmap, string path);
}

// Navigation
public interface INavigationService
{
    Task NavigateToAsync(string viewName, object parameter = null);
    Task GoBackAsync();
    bool CanGoBack { get; }
}

// Dialogs
public interface IDialogService
{
    Task ShowMessageAsync(string title, string message);
    Task<bool> ShowConfirmAsync(string title, string message);
    Task<string> ShowInputAsync(string title, string prompt);
}

// Settings
public interface ISettingsService
{
    T Get<T>(string key, T defaultValue);
    void Set<T>(string key, T value);
    Task SaveAsync();
}

// Platform info
public interface IPlatformService
{
    PlatformType CurrentPlatform { get; }
    bool SupportsNativeCpp { get; }
    bool IsMobile { get; }
    int MaxImageWidth { get; }
    int MaxImageHeight { get; }
}

public enum PlatformType
{
    Windows,
    Android,
    iOS,
    MacCatalyst,
    Linux
}
```

---

### UI Controls - WinUI vs MAUI Mapping

**Controls Available in Both Platforms:**

| WinUI 3 Control | MAUI Control | Notes |
|-----------------|--------------|-------|
| `TextBlock` | `Label` | Minor API differences |
| `TextBox` | `Entry` | MAUI has simpler API |
| `Button` | `Button` | Nearly identical |
| `Image` | `Image` | Similar, source binding differs |
| `ListView` | `ListView`/`CollectionView` | MAUI prefers CollectionView |
| `GridView` | `CollectionView` | Use CollectionView in MAUI |
| `ProgressBar` | `ProgressBar` | Similar |
| `Slider` | `Slider` | Nearly identical |
| `CheckBox` | `CheckBox` | Similar |
| `RadioButton` | `RadioButton` | Similar |
| `ComboBox` | `Picker` | Different API |
| `Grid` | `Grid` | Identical layout |
| `StackPanel` | `StackLayout`/`VerticalStackLayout` | MAUI simplified |

**WinUI-Specific Controls (Will Need Alternatives):**

| WinUI 3 Control | MAUI Alternative | Notes |
|-----------------|------------------|-------|
| `NavigationView` | Custom or `Shell` | MAUI Shell provides navigation |
| `NumberBox` | `Entry` + validation | No built-in NumberBox in MAUI |
| `CommandBar` | `Toolbar` | Different API |
| `TeachingTip` | Custom tooltip | No direct equivalent |
| `SplitView` | `FlyoutPage` or custom | Different pattern |
| `MenuBar` | `MenuBarItem` in Shell | Different approach |
| `TreeView` | Custom control | Not in MAUI standard |

**Recommendations:**
- Prefer controls that exist in both platforms
- Use `ContentView` to wrap complex controls for easy platform substitution
- Create custom controls in Core project using platform-agnostic primitives

---

### Data Binding Differences

**Both platforms support:**
- ✅ `INotifyPropertyChanged`
- ✅ `ICommand`
- ✅ `ObservableCollection<T>`
- ✅ CommunityToolkit.Mvvm (works in both!)

**Syntax Differences:**

```xaml
<!-- WinUI 3 -->
<TextBlock Text="{x:Bind ViewModel.FractalName, Mode=OneWay}" />
<Button Command="{x:Bind ViewModel.RenderCommand}" />

<!-- MAUI -->
<Label Text="{Binding FractalName}" />
<Button Command="{Binding RenderCommand}" />
```

**Recommendation:** Use `{Binding}` syntax in WinUI for MAUI compatibility, even though `{x:Bind}` is faster. Or plan to convert XAML when porting.

---

### Threading and Async Patterns

**Both platforms use:**
- `async`/`await`
- `Task` and `Task<T>`
- `CancellationToken`

**Dispatching to UI Thread:**

```csharp
// ✅ GOOD - Use abstraction
public interface IDispatcherService
{
    Task RunOnUIThreadAsync(Action action);
}

// WinUI implementation
public class WinUIDispatcherService : IDispatcherService
{
    private readonly DispatcherQueue _dispatcher;
    public async Task RunOnUIThreadAsync(Action action)
    {
        await _dispatcher.EnqueueAsync(action);
    }
}

// MAUI implementation
public class MauiDispatcherService : IDispatcherService
{
    public async Task RunOnUIThreadAsync(Action action)
    {
        await MainThread.InvokeOnMainThreadAsync(action);
    }
}

// ❌ BAD - Direct WinUI dependency
public class ViewModel
{
    private DispatcherQueue _dispatcher;
    public void UpdateUI()
    {
        _dispatcher.TryEnqueue(() => { ... });
    }
}
```

---

### File I/O Patterns

**Platform Differences:**
- **WinUI:** `Windows.Storage.*` APIs, `StorageFile`, `FileOpenPicker`
- **MAUI:** `Microsoft.Maui.Storage.FilePicker`, `FileSystem` helpers

**Abstraction Example:**

```csharp
// Shared interface
public interface IFileService
{
    Task<FileData> PickFileAsync(PickOptions options);
    Task<string> PickFolderAsync();
    Task<bool> SaveFileAsync(string filename, Stream data);
}

public class PickOptions
{
    public string[] FileTypes { get; set; }
    public string PickerTitle { get; set; }
}

public class FileData
{
    public string FileName { get; set; }
    public Stream Content { get; set; }
}

// WinUI implementation
public class WinUIFileService : IFileService
{
    public async Task<FileData> PickFileAsync(PickOptions options)
    {
        var picker = new FileOpenPicker();
        foreach (var type in options.FileTypes)
            picker.FileTypeFilter.Add(type);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return null;

        return new FileData
        {
            FileName = file.Name,
            Content = await file.OpenStreamForReadAsync()
        };
    }
}

// MAUI implementation
public class MauiFileService : IFileService
{
    public async Task<FileData> PickFileAsync(PickOptions options)
    {
        var fileTypes = new FilePickerFileType(
            new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, options.FileTypes }
            });

        var result = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = fileTypes,
            PickerTitle = options.PickerTitle
        });

        if (result == null) return null;

        return new FileData
        {
            FileName = result.FileName,
            Content = await result.OpenReadAsync()
        };
    }
}
```

---

### Bitmap/Image Handling

**Challenge:** Fractal rendering produces raw pixel data that needs platform-specific bitmap types.

**Solution:**

```csharp
// Shared interface
public interface IBitmapService
{
    IBitmapHandle CreateBitmap(int width, int height);
    void SetPixels(IBitmapHandle handle, byte[] pixelData);
    Task SaveAsync(IBitmapHandle handle, string path, ImageFormat format);
}

public interface IBitmapHandle : IDisposable
{
    int Width { get; }
    int Height { get; }
    object PlatformBitmap { get; } // For binding to Image.Source
}

// WinUI implementation
public class WinUIBitmapService : IBitmapService
{
    public IBitmapHandle CreateBitmap(int width, int height)
    {
        var bitmap = new WriteableBitmap(width, height);
        return new WinUIBitmapHandle(bitmap);
    }
}

// MAUI implementation (future)
public class MauiBitmapService : IBitmapService
{
    public IBitmapHandle CreateBitmap(int width, int height)
    {
        // Use SKBitmap from SkiaSharp (cross-platform)
        var bitmap = new SKBitmap(width, height);
        return new MauiBitmapHandle(bitmap);
    }
}
```

---

### C++ Interop Considerations

**Platform Support:**
- **Windows (WinUI):** Full C++/CLI support ✅
- **Android (MAUI):** Via JNI or C/C++ NDK ⚠️
- **iOS (MAUI):** Via Objective-C bridging ⚠️
- **macOS (MAUI):** Via Objective-C bridging ⚠️

**Strategies:**

**Option 1: Windows-Only C++ (Recommended for Phase 1)**
```csharp
public interface IFractalEngine
{
    Task<byte[]> CalculateAsync(FractalParameters params);
}

// Windows implementation - uses C++ engine
public class NativeFractalEngine : IFractalEngine { ... }

// Mobile implementation - pure C# fallback or cloud rendering
public class ManagedFractalEngine : IFractalEngine { ... }

// Or cloud-based
public class CloudFractalEngine : IFractalEngine { ... }
```

**Option 2: Cross-Compile C++ (Advanced)**
- Compile C++ engine as native library for each platform
- Use P/Invoke on all platforms
- More complex build process
- Best performance on all platforms

**Option 3: Hybrid Approach**
- Desktop (Windows/Mac): Native C++ engine
- Mobile (iOS/Android): Cloud rendering or simplified C# engine
- Mobile devices typically can't handle deep zoom anyway

**Recommendation for This Project:**
Start with **Option 1**. Mobile devices likely won't need the full power of the C++ engine. Consider:
- Mobile: Limited parameter ranges, cloud rendering for heavy work
- Desktop: Full C++ engine with all features

---

### Navigation Patterns

**WinUI:** `Frame` navigation with `NavigationView`

**MAUI:** `Shell` navigation or `NavigationPage`

**Abstraction:**

```csharp
public interface INavigationService
{
    Task NavigateToAsync<TViewModel>(object parameter = null);
    Task GoBackAsync();
    bool CanGoBack { get; }
}

// Register routes in startup
public static class Routes
{
    public const string MainPage = "main";
    public const string ParametersPage = "parameters";
    public const string PalettePage = "palette";
}
```

---

### Dependency Injection

**Both platforms use Microsoft.Extensions.DependencyInjection:**

```csharp
// Shared service registration
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<FractalParametersViewModel>();

        // Business services
        services.AddSingleton<IFractalService, FractalService>();
        services.AddSingleton<ISettingsService, SettingsService>();

        return services;
    }

    public static IServiceCollection AddPlatformServices(
        this IServiceCollection services)
    {
        // Platform-specific registration done in each app
        return services;
    }
}

// WinUI App.xaml.cs
services.AddCoreServices();
services.AddSingleton<IFileService, WinUIFileService>();
services.AddSingleton<IBitmapService, WinUIBitmapService>();

// MAUI MauiProgram.cs (future)
services.AddCoreServices();
services.AddSingleton<IFileService, MauiFileService>();
services.AddSingleton<IBitmapService, MauiBitmapService>();
```

---

### Testing Benefits of MAUI-Ready Architecture

**Easier Unit Testing:**
```csharp
// ViewModels are platform-independent, easy to test
[Fact]
public async Task RenderCommand_ShouldCallFractalService()
{
    // Arrange
    var mockService = new Mock<IFractalService>();
    var viewModel = new MainViewModel(mockService.Object);

    // Act
    await viewModel.RenderCommand.ExecuteAsync(null);

    // Assert
    mockService.Verify(s => s.CalculateAsync(It.IsAny<FractalParameters>()));
}
```

---

### Migration Path to MAUI

**Phase 1: WinUI Development (Now)**
1. Build WinUI app with Core/WinUI split
2. Keep ViewModels platform-agnostic
3. Abstract platform services

**Phase 2: MAUI Project Setup (Future)**
1. Create ManpLab.Maui project
2. Reference ManpLab.Core
3. Implement MAUI platform services
4. Create MAUI-specific views

**Phase 3: Platform Optimization**
1. Add Android-specific optimizations
2. Add iOS touch optimizations
3. Consider cloud rendering for mobile
4. Mobile-specific UI (simplified controls)

**Estimated Effort:**
- If architecture is MAUI-ready: **2-4 weeks** to get MAUI version working
- If architecture is WinUI-coupled: **2-3 months** of refactoring first

---

### Decision Checklist for Each Feature

When implementing any feature in WinUI, ask:

- [ ] Can this ViewModel be used in MAUI without changes?
- [ ] Are platform-specific APIs abstracted behind interfaces?
- [ ] Does this use controls available in both platforms?
- [ ] Is file I/O going through IFileService?
- [ ] Is bitmap handling going through IBitmapService?
- [ ] Are there no direct WinUI namespace references in Core?
- [ ] Can this be unit tested without a UI?

**If any answer is "No"**: Refactor to use abstraction layer.

---

### Performance Considerations for Mobile

**Mobile Limitations:**
- Less RAM (2-8 GB vs 16-64 GB desktop)
- Slower CPUs
- Battery constraints
- Smaller screens (less pixels = faster rendering)

**Design Decisions:**
1. **Parameter Limits:** Reduce max iterations, resolution on mobile
2. **Cloud Rendering:** Offload heavy calculations to server
3. **Simplified Features:** Mobile might not need all 240 fractal types
4. **Caching:** More aggressive caching on mobile
5. **Progressive Rendering:** Show low-res preview first

```csharp
public class FractalParameters
{
    public int MaxIterations { get; set; }

    public int GetEffectiveMaxIterations(IPlatformService platform)
    {
        if (platform.IsMobile)
            return Math.Min(MaxIterations, 1000);
        return MaxIterations;
    }
}
```

---

### Recommended Reading
- [.NET MAUI Documentation](https://learn.microsoft.com/dotnet/maui/)
- [MAUI vs WinUI Comparison](https://learn.microsoft.com/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/overall-migration-strategy)
- [Building Cross-Platform Apps](https://learn.microsoft.com/dotnet/architecture/maui/cross-platform-development)

---

### Documentation
- [ManpWIN GitHub](https://github.com/markhassellsmith/ManpLab)
- [WinUI 3 Documentation](https://docs.microsoft.com/windows/apps/winui/)
- [MVVM Toolkit Documentation](https://learn.microsoft.com/windows/communitytoolkit/mvvm/)
- [C++/CLI Documentation](https://docs.microsoft.com/cpp/dotnet/)

### Fractal Mathematics
- Mandelbrot Set theory
- Perturbation theory (Kalles Fraktaler)
- BLA (Bilinear Approximation)
- Series approximation algorithms

### Libraries
- MPFR: https://www.mpfr.org/
- QD Library: https://www.davidhbailey.com/dhbsoftware/
- Fractint: https://www.fractint.org/


