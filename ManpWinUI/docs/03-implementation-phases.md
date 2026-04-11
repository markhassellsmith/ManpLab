# Implementation Phases

This document outlines the detailed implementation plan for the ManpLab WinUI modernization project, broken into 9 progressive phases from planning through deployment.

---

## Phase 1: Planning & Analysis ✅ COMPLETE

**Status:** ✅ Tagged v0.1.0-planning

**Tasks:**
- [x] Document existing C++ interface features  
- [x] Identify what's good/efficient (keep) vs. clunky (redesign)
- [x] Analyze project dependencies and libraries
- [x] List core algorithms to preserve in C++
- [x] Create detailed C++/C# interop strategy
- [x] Map C++ functionality to WinUI equivalents
- [x] Define modern project structure
- [x] Document file format specifications
- [x] Identify pain points and improvement opportunities

**Deliverables:**
- DESIGN_PLAN.md with comprehensive planning documentation
- Git workflow established (development + feature branches)
- Technology stack decisions documented
- Architecture blueprint ready

**Duration:** 4-6 weeks part-time (COMPLETED)

---

## Phase 2: C++ Core Preparation

**Status:** ⏳ Not Started

**Goals:**
- Extract pure computation code from UI dependencies
- Create clean C API or C++/CLI wrapper
- Test interop performance

**Tasks:**
- [ ] Extract pure computation code from UI code
- [ ] Create clean C API or C++/CLI wrapper project
- [ ] Define data exchange structures
- [ ] Implement marshalling for complex types
- [ ] Test interop with simple fractal calculation
- [ ] Profile performance of C++/C# boundary
- [ ] Document API for C# developers

**Key Deliverables:**
- `ManpCore.Native` C++/CLI project created
- Fractal engine wrapper interfaces defined
- Complex type marshalling working (Complex, BigDouble)
- API documentation for C# consumers
- Performance benchmarks (verify <5% overhead)

**Dependencies:**
- Phase 1 planning complete
- Understanding of ManpWIN64 architecture

**Risks:**
- Complex type marshalling may be difficult
- Performance overhead on C++/C# boundary
- Threading model compatibility

**Estimated Duration:** 3-4 weeks part-time

**Git Milestone:** `v0.2.0-interop`

**Android Portability Validation:**
- [ ] C++/CLI wrapper designed knowing it's Windows-only (cloud rendering planned for Android)
- [ ] Core C++ math engine has no Windows dependencies (could compile on Linux if needed)
- [ ] API designed to work with future REST/gRPC cloud rendering service
- [ ] Data structures serializable for network transfer (parameter JSON, image bytes)

---

## Phase 3: WinUI Foundation

**Status:** ⏳ Not Started

**Goals:**
- Set up MVVM framework
- Create basic app structure
- Configure DI and services

**Tasks:**
- [ ] Set up MVVM framework (CommunityToolkit.Mvvm)
- [ ] Create base view models and navigation
- [ ] Implement dependency injection (Microsoft.Extensions.DI)
- [ ] Set up async/await patterns for calculations
- [ ] Create basic main window layout
- [ ] Implement settings/configuration system (JSON)
- [ ] Set up logging (Serilog or NLog)

**Key Deliverables:**
- `ManpWinUI` project with WinUI 3 configured
- `ViewModelBase` class with INotifyPropertyChanged
- Dependency injection container configured
- `App.xaml.cs` with service registration
- `MainWindow.xaml` basic layout
- Settings service (load/save JSON)
- Logging to file and debug output

**NuGet Packages:**
```powershell
dotnet add package CommunityToolkit.Mvvm
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package System.Text.Json
dotnet add package Serilog
dotnet add package Serilog.Sinks.File
```

**Dependencies:**
- Phase 2 interop layer ready
- C++ wrapper callable from C#

**Estimated Duration:** 2-3 weeks part-time

**Git Milestone:** `v0.3.0-foundation`

**✅ Android Portability Validation (MANDATORY):**
- [ ] All ViewModels have zero `Microsoft.UI.Xaml.*` references
- [ ] `CommunityToolkit.Mvvm` used exclusively (cross-platform compatible)
- [ ] Platform services defined as interfaces (`IFileService`, `IBitmapService`, `INavigationService`, `IDispatcherService`)
- [ ] Dependency injection configured for service abstraction (can swap implementations for Android)
- [ ] Settings service uses standard `System.Text.Json` (MAUI compatible)
- [ ] Logging uses Serilog interfaces (can redirect to Android logcat later)
- [ ] ViewModels could compile in separate project without WinUI references

---

## Phase 4: Core UI Development

**Status:** ⏳ Not Started

**Goals:**
- Build essential UI components
- Implement fractal display and interaction
- Create parameter entry system

**Tasks:**
- [ ] Main navigation (NavigationView with fractal categories)
- [ ] Fractal display canvas with zoom/pan
- [ ] Parameter entry panel (adaptive for fractal type)
- [ ] Real-time Julia set preview
- [ ] Color palette editor with visual feedback
- [ ] Rendering progress indicator
- [ ] Coordinate display and precision selector
- [ ] Formula editor with syntax highlighting

**Key Deliverables:**
- `MainViewModel` with navigation
- `FractalDisplayView` with WriteableBitmap rendering
- `ParametersPanelView` with NumberBox/Slider controls
- `FractalParametersViewModel` with validation
- `PaletteEditorView` with color picker
- `RenderProgressView` with ProgressBar and cancel
- Julia preview working (real-time parameter changes)

**UI Components:**
- NavigationView (main navigation)
- Image/WriteableBitmap (fractal display)
- NumberBox (parameter entry)
- Slider (real-time adjustment)
- ProgressRing (rendering status)
- ColorPicker (palette editor)
- MenuBar (top-level commands)

**Dependencies:**
- Phase 3 foundation complete
- C++ fractal engine callable

**Estimated Duration:** 4-5 weeks part-time

**Git Milestone:** `v0.4.0-core-ui`

**✅ Android Portability Validation (MANDATORY):**
- [ ] `FractalParametersViewModel` has no WinUI dependencies (only MVVM toolkit)
- [ ] `WriteableBitmap` abstracted behind `IBitmapService.CreateBitmap(byte[])`
- [ ] `DispatcherQueue` calls use `IDispatcherService` interface, not direct WinUI API
- [ ] Fractal calculation logic in separate service (not in ViewModel)
- [ ] Color palette using standard .NET `System.Drawing.Color` or custom struct (not WinUI `Microsoft.UI.Color`)
- [ ] Navigation using `INavigationService`, not direct `Frame.Navigate()`
- [ ] All business logic could run in .NET Standard 2.1 / .NET 10 class library

**🎉 Milestone:** First usable prototype - can render basic fractals!

---

## Phase 5: Advanced Features

**Status:** ⏳ Not Started

**Goals:**
- Add advanced fractal features
- Implement deep zoom support
- 3D visualization integration

**Tasks:**
- [ ] Animation sequencer and timeline
- [ ] 3D visualization integration
- [ ] Zoom box/rectangle selection (touch-friendly)
- [ ] Orbit diagram display
- [ ] Batch rendering queue
- [ ] Deep zoom with perturbation UI
- [ ] BLA approximation controls
- [ ] Slope/derivative shading options

**Key Deliverables:**
- `AnimationViewModel` with timeline controls
- `AnimationSequencerView` with keyframe editor
- Zoom rectangle selection with touch/pen support
- Perturbation parameters UI (reference orbit, BLA)
- Orbit diagram visualization
- Batch rendering queue with progress
- Slope/derivative visualization toggles

**Dependencies:**
- Phase 4 core UI working
- C++ perturbation engine exposed
- BLA algorithms accessible

**Estimated Duration:** 4-6 weeks part-time

**Git Milestone:** `v0.5.0-advanced`

**✅ Android Portability Validation (MANDATORY):**
- [ ] Perturbation calculation logic platform-agnostic
- [ ] BLA algorithm implementation has no WinUI dependencies
- [ ] Animation keyframe system uses standard .NET types (could serialize to JSON)
- [ ] Video encoding abstracted behind `IVideoService` interface (different impl for Android)
- [ ] Color gradient calculations use platform-agnostic math
- [ ] All advanced algorithms testable without UI (unit tests with mock services)

---

## Phase 6: File Operations

**Status:** ⏳ Not Started

**Goals:**
- Implement file I/O
- Support legacy file formats
- Modern export functionality

**Tasks:**
- [ ] Modern file pickers (load/save)
- [ ] .MAP file import/export
- [ ] .PAR (Fractint) compatibility
- [ ] .KFR (Kalles) import
- [ ] PNG export with metadata
- [ ] MP4 video export (replace MPEG)
- [ ] Drag-and-drop support
- [ ] Recent files management

**Key Deliverables:**
- `IFileService` interface with WinUI implementation
- .MAP file parser (parameters → FractalParameters)
- .PAR file reader (Fractint compatibility)
- .KFR file reader (Kalles Fraktaler deep zoom)
- PNG export with embedded .MAP metadata
- H.264/H.265 video encoder (Windows Media Foundation)
- Drag-and-drop handler for .MAP/.PAR/.KFR files
- Recent files list in File menu

**File Format Support:**
- .MAP - ManpLab parameter files (read/write)
- .PAR - Fractint compatibility (read)
- .KFR - Kalles Fraktaler deep zoom (read)
- .PNG - Image export with metadata
- .MP4 - Animation export (H.264)
- .FRM - Formula files (read via parser)

**Dependencies:**
- Phase 4 core rendering working
- File format specifications understood

**Estimated Duration:** 3-4 weeks part-time

**Git Milestone:** `v0.6.0-files`

**✅ Android Portability Validation (MANDATORY):**
- [ ] `IFileService` interface used exclusively (no direct `Windows.Storage.*`)
- [ ] File parsers (.MAP, .PAR, .KFR) use `Stream` and `byte[]` (MAUI compatible)
- [ ] PNG export uses standard image libraries (not WinUI-specific bitmap APIs)
- [ ] Video encoding behind `IVideoService` (can swap for Android FFmpeg wrapper)
- [ ] Drag-and-drop in UI layer only (ViewModels accept file paths via interface)
- [ ] Recent files logic platform-agnostic (uses `IFileService` for path persistence)
- [ ] All file I/O testable with mock `IFileService` implementation

**🎉 Milestone:** Feature complete - all major functionality implemented!

---

## Phase 7: Polish & Features

**Status:** ⏳ Not Started

**Goals:**
- Improve user experience
- Add modern Windows features
- Accessibility and theming

**Tasks:**
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

**Key Deliverables:**
- Keyboard accelerators (Ctrl+R render, Ctrl+Z undo, etc.)
- Context menus for fractal display and parameters
- Command pattern with undo/redo stack
- Preset gallery with thumbnails
- Windows Share integration
- Touch/pen gesture optimization
- Per-monitor DPI awareness
- Dark/light theme switching
- Screen reader support (AutomationPeer)
- Localization-ready strings (resx files)

**UX Improvements:**
- Keyboard navigation for all controls
- TeachingTips for new users
- Tooltips with keyboard shortcuts
- Gesture support (pinch-to-zoom, two-finger pan)
- High contrast theme support

**Dependencies:**
- Phase 6 file operations complete
- Core features stable

**Estimated Duration:** 3-4 weeks part-time

**Git Milestone:** `v0.7.0-polish`

**✅ Android Portability Validation (MANDATORY):**
- [ ] Undo/redo system uses platform-agnostic command pattern
- [ ] Preset gallery data models have no WinUI dependencies
- [ ] Share functionality abstracted behind `IShareService` (different impl for Android)
- [ ] Touch/pen gestures in View layer only (ViewModels receive results via commands)
- [ ] Theme switching uses abstract `IThemeService` (can map to Android Material themes)
- [ ] Localization uses standard .NET resources (MAUI compatible)
- [ ] Accessibility patterns don't break MAUI compatibility

---

## Phase 8: Testing & Optimization

**Status:** ⏳ Not Started

**Goals:**
- Comprehensive testing
- Performance optimization
- Bug fixes and stability

**Tasks:**
- [ ] Unit tests for C# ViewModels
- [ ] Integration tests for C++/C# interop
- [ ] UI automation tests
- [ ] Performance profiling
- [ ] Memory leak detection
- [ ] Stress testing (deep zoom, long animations)
- [ ] Multi-threading validation
- [ ] Cross-version file format testing

**Key Deliverables:**
- Unit test suite (xUnit) with >80% coverage
- Integration tests for C++ interop
- UI automation tests (WinAppDriver)
- Performance benchmarks documented
- Memory leak fixes
- Stress test results (100K+ iterations, 1-hour animations)
- Thread safety validation
- File format compatibility matrix

**Testing Tools:**
- xUnit - Unit testing framework
- Moq - Mocking framework
- FluentAssertions - Assertion library
- BenchmarkDotNet - Performance benchmarking
- dotMemory - Memory profiling
- WinAppDriver - UI automation

**Test Categories:**
1. **Unit Tests:**
   - ViewModels (commands, property changes, validation)
   - Services (settings, file I/O)
   - Data models (FractalParameters, palette)

2. **Integration Tests:**
   - C++ fractal engine calls
   - File format parsing
   - Bitmap rendering pipeline

3. **UI Tests:**
   - Navigation flows
   - Parameter entry validation
   - Rendering progress updates

4. **Performance Tests:**
   - Fractal calculation benchmarks
   - Bitmap update performance
   - Interop overhead measurement

**Dependencies:**
- All previous phases complete
- Feature freeze before testing begins

**Estimated Duration:** 3-4 weeks part-time

**Git Milestone:** `v0.8.0-tested`

**✅ Android Portability Validation (MANDATORY):**
- [ ] Run full test suite to verify zero WinUI dependencies in ViewModels/business logic
- [ ] Verify all platform services use interfaces (can be mocked for unit tests)
- [ ] Confirm no `Windows.*` or `Microsoft.UI.*` in shared code (except Views)
- [ ] Test that ViewModels/services could compile in .NET Standard 2.1 library
- [ ] Validate service abstractions are complete (no direct platform API calls)
- [ ] Document any platform-specific code that would need Android equivalents
- [ ] Create "Android Readiness Report" summarizing portability validation results

**🎉 Milestone:** Release Candidate - ready for beta testing!

---

## Phase 9: Documentation & Deployment

**Status:** ⏳ Not Started

**Goals:**
- Complete documentation
- Package for distribution
- Deployment infrastructure

**Tasks:**
- [ ] User documentation
- [ ] Developer documentation for interop
- [ ] Formula language reference
- [ ] Tutorial for new users
- [ ] API documentation (XML comments)
- [ ] Build/deployment scripts
- [ ] Installer package (MSIX)
- [ ] Update mechanism

**Key Deliverables:**
- User manual (PDF/HTML)
- Developer API documentation
- Formula language reference guide
- Getting Started tutorial with screenshots
- XML documentation comments in code
- MSIX package configuration
- MSIX installer tested
- Auto-update mechanism (if applicable)
- GitHub Releases page setup

**Documentation Components:**

1. **User Documentation:**
   - Installation guide
   - Quick start tutorial
   - Feature reference (fractals, coloring, animation)
   - Keyboard shortcuts reference
   - File format guide
   - FAQ and troubleshooting

2. **Developer Documentation:**
   - Architecture overview
   - C++/CLI interop guide
   - Adding new fractal types
   - Custom formula syntax
   - Contributing guidelines

3. **Technical Documentation:**
   - Build instructions
   - Development environment setup
   - Testing procedures
   - Deployment process

**MSIX Packaging:**
- Configure Package.appxmanifest
- Code signing certificate
- Store submission preparation (optional)
- Sideloading instructions

**Dependencies:**
- Phase 8 testing complete
- All features stable and documented

**Estimated Duration:** 2-3 weeks part-time

**Git Milestone:** `v1.0.0`

**✅ Android Portability Validation (MANDATORY):**
- [ ] Document which code is platform-agnostic (ViewModels, services, business logic)
- [ ] Document platform-specific implementations (IFileService, IBitmapService, etc.)
- [ ] Create Android migration guide referencing service abstractions
- [ ] List all WinUI dependencies that need Android equivalents
- [ ] Verify 07-maui-compatibility.md is up-to-date with final architecture
- [ ] Document cloud rendering strategy for Android (REST API, image transfer)
- [ ] Estimate Android migration effort based on actual architecture (target: 6-8 weeks)

**🎉 Milestone:** Version 1.0 Release - Production ready!

**📱 Next Step:** Android Migration (Post-v1.0, 6-8 weeks if constraints followed)

---

## Summary Timeline

### Development Time Estimates

**Scenario 1: Part-Time Solo (No AI)**
- Timeline: 6-9 months calendar time
- Effort: 10-20 hours/week
- Total Hours: 240-720 hours

**Scenario 2: Part-Time with AI Assistance** ⭐ Recommended
- Timeline: 3-5 months calendar time
- Effort: 10-20 hours/week
- Total Hours: 120-400 hours
- AI Benefits: XAML generation, MVVM boilerplate, wrapper code, tests

**Scenario 3: Full-Time Solo (No AI)**
- Timeline: 2-3 months calendar time
- Effort: 40 hours/week
- Total Hours: 320-480 hours

**Scenario 4: Full-Time with AI Assistance**
- Timeline: 1-2 months calendar time
- Effort: 40 hours/week
- Total Hours: 160-320 hours

### Phase Breakdown

| Phase | Name | Duration (Part-Time) | Milestone Tag |
|-------|------|---------------------|---------------|
| 1 | Planning & Analysis | 4-6 weeks | ✅ v0.1.0-planning |
| 2 | C++ Core Preparation | 3-4 weeks | v0.2.0-interop |
| 3 | WinUI Foundation | 2-3 weeks | v0.3.0-foundation |
| 4 | Core UI Development | 4-5 weeks | v0.4.0-core-ui 🎉 |
| 5 | Advanced Features | 4-6 weeks | v0.5.0-advanced |
| 6 | File Operations | 3-4 weeks | v0.6.0-files 🎉 |
| 7 | Polish & Features | 3-4 weeks | v0.7.0-polish |
| 8 | Testing & Optimization | 3-4 weeks | v0.8.0-tested 🎉 |
| 9 | Documentation & Deployment | 2-3 weeks | v1.0.0 🚀 |

**Total:** 28-39 weeks part-time (3-5 months with AI assistance)

---

## Key Milestones

### v0.1.0-planning ✅ ACHIEVED
- Comprehensive planning complete
- Architecture decisions documented
- Git workflow established

### v0.4.0-core-ui (First Prototype)
- Can render basic fractals
- Parameter entry working
- Display with zoom/pan

### v0.6.0-files (Feature Complete)
- All major features implemented
- File I/O working
- Export functionality ready

### v0.8.0-tested (Release Candidate)
- Comprehensive testing complete
- Performance optimized
- Memory leaks fixed
- Beta testing ready

### v1.0.0 (Production Release) 🚀
- Full documentation
- MSIX installer
- Production-ready
- Feature parity with ManpWIN64

---

## Phase Dependencies

```
Phase 1 (Planning)
    ↓
Phase 2 (C++ Interop) ← Required for all subsequent phases
    ↓
Phase 3 (WinUI Foundation) ← Required for all UI work
    ↓
Phase 4 (Core UI) ← First usable version
    ↓
Phase 5 (Advanced Features) ← Parallel with Phase 6 possible
    ↓
Phase 6 (File Operations) ← Can start during Phase 5
    ↓
Phase 7 (Polish) ← Must follow Phase 6
    ↓
Phase 8 (Testing) ← Requires feature freeze
    ↓
Phase 9 (Deployment) ← Final phase
```

---

## Risk Mitigation by Phase

### Phase 2 Risks
- **Risk:** C++ interop performance overhead
- **Mitigation:** Profile early, use Span<T> and Memory<T>, keep large data in C++

### Phase 4 Risks
- **Risk:** Bitmap rendering performance
- **Mitigation:** Use WriteableBitmap efficiently, consider DirectX interop if needed

### Phase 5 Risks
- **Risk:** Complex animation timeline UI
- **Mitigation:** Start with simple linear timeline, add complexity incrementally

### Phase 6 Risks
- **Risk:** Legacy file format compatibility issues
- **Mitigation:** Test with existing .MAP/.PAR files from ManpWIN64

### Phase 8 Risks
- **Risk:** Memory leaks in C++ interop
- **Mitigation:** Use dotMemory profiler, ensure proper IDisposable implementation

---

## Success Criteria

### Phase 1 ✅
- [x] Complete planning documentation
- [x] Architecture decisions made
- [x] Git workflow established

### Phase 2
- [ ] C++/CLI wrapper compiles
- [ ] Simple fractal calculation works from C#
- [ ] Performance overhead <5%

### Phase 4 (MVP)
- [ ] Can render standard Mandelbrot set
- [ ] Parameter changes update display
- [ ] Zoom and pan working

### Phase 6 (Feature Complete)
- [ ] Can open existing .MAP files
- [ ] Can export PNG images
- [ ] All 240 fractal types working

### Phase 8 (Release Candidate)
- [ ] All tests passing
- [ ] No memory leaks
- [ ] Performance meets targets

### Phase 9 (v1.0 Release)
- [ ] MSIX installer working
- [ ] Documentation complete
- [ ] Ready for public release

---

## Post-Release Roadmap

### v1.1 (3 months after v1.0)
- User feedback integration
- Bug fixes
- Performance improvements
- Optional: GPU acceleration for simple fractals

### v1.2 (6 months after v1.0)
- Additional fractal types
- Enhanced coloring algorithms
- Cloud rendering support (optional)

### v2.0 (12 months after v1.0)
- MAUI expansion (macOS, iOS, Android)
- Collaborative features
- Cloud storage integration

---

For detailed commit strategy and Git workflow per phase, see [Git Strategy](05-git-strategy.md).

For technology stack details, see [Technology Stack](04-technology-stack.md).
