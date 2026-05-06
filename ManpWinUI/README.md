# ManpWinUI - Modern Fractal Explorer

![Platform](https://img.shields.io/badge/platform-Windows%2011-blue)
![.NET](https://img.shields.io/badge/.NET-10-blue.svg)
![WinUI](https://img.shields.io/badge/WinUI-3-purple.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)

A comprehensive modern fractal rendering application built with WinUI 3 and .NET 10, featuring 300 fractal types, animation rendering, deep zoom, and an intuitive MVVM architecture.

---

## ✨ Features

### 🌌 300 Fractal Types
- **Classic Sets** - Mandelbrot, Julia, Burning Ship, Newton, Phoenix
- **Power Variants** - Multibrot (powers 3-10), Tricorn, Buffalo
- **Strange Attractors** - Lorenz, Rössler, Hénon, Clifford, Pickover
- **IFS Fractals** - Barnsley fern, Sierpinski, Dragon curve
- **Exotic Formulas** - Celtic, Heart, Zubieta, Perpendicular
- **Orbital Modifications** - Circular trap, Cross trap, Stalks, Smoothed
- **23 Famous Julia Presets** - Golden, Dendrite, Spiral, Dragon, and more

### 🎬 Animation System
- **MP4 Export** - Create high-quality fractal animations with FFmpeg
- **Parameter Interpolation** - Smooth transitions between fractal states
- **Zoom Animations** - Automated deep-zoom sequences
- **Color Cycling** - Animated palette rotations
- **Batch Rendering** - Queue multiple animations

### 🎨 Rendering & Visualization
- **Real-time Rendering** - Multi-threaded native computation engine
- **Deep Zoom** - Arbitrary precision arithmetic for extreme magnification
- **Color Palettes** - Multiple built-in schemes with smooth gradients
- **High Resolution** - Export up to 4K (3840×2160) and beyond
- **Julia Mode Toggle** - Instant switching between Mandelbrot and Julia sets

### 🖱️ Interactive Exploration
- **Click to Zoom** - Left-click any point to zoom in
- **Rectangle Zoom** - Click and drag to define zoom area
- **Pan Navigation** - Right-click drag or arrow keys to pan
- **Mouse Wheel** - Zoom in/out with precise control
- **Coordinate Display** - Real-time fractal coordinates and zoom level

### 📚 Fractal Browser
- **Searchable Library** - Browse all 300 fractals by name or category
- **Metadata Display** - Formulas (TeX and plaintext), descriptions, parameters
- **User Bookmarks** - Save favorite fractals with custom names and notes
- **Navigation History** - Back/forward through exploration history
- **User Notes** - Annotate fractals with personal observations

### 🎨 Theme System
- **Light Theme** - Clean, bright interface
- **Dark Theme** - Reduced eye strain for extended sessions
- **Ocean Blue** - Custom maritime-inspired theme
- **System Theme** - Follows Windows settings
- **State Preservation** - Theme changes don't reset your work

### ⌨️ Keyboard Shortcuts
- **F1** - Show help with all shortcuts
- **F5 / Ctrl+R** - Render fractal
- **Arrow Keys** - Pan view (Shift for faster)
- **+/-** - Zoom in/out
- **1-4** - Quick resolution presets (HD, FHD, 2K, 4K)
- **Space** - Reset to default view
- **Ctrl+B** - Toggle browser panel
- **Ctrl+P** - Toggle properties panel
- **Ctrl+S** - Save image

👉 **[Full Keyboard Reference](KEYBOARD_SHORTCUTS.md)**

---

## 🚀 Quick Start

### Running the Application

**Requirements:**
- Windows 11 version 22H2 or later
- .NET 10 Runtime (or SDK for development)

**From Release:**
```bash
# Download and extract the latest release
cd ManpWinUI
./ManpWinUI.exe
```

**From Source:**
```bash
cd ManpWinUI
dotnet run
```

### First Steps

1. **Launch** → Application starts with default Mandelbrot view
2. **Press F1** → View all keyboard shortcuts
3. **Click** on any interesting area to zoom in
4. **Arrow keys** to pan around
5. **+/-** to zoom in/out
6. **Space** to reset when lost

---

## 🏗️ Architecture

### Technology Stack
- **.NET 10** - Latest .NET framework
- **WinUI 3** - Modern Windows UI framework
- **MVVM Pattern** - Clean separation of concerns
- **Dependency Injection** - Microsoft.Extensions.DependencyInjection
- **Async/Await** - Responsive UI with background computation

### Project Structure

```
ManpWinUI/
├── App.xaml                    # Application entry point
├── App.xaml.cs                 # DI container setup
├── ViewModels/
│   └── MainViewModel.cs        # Core application logic & state
├── Views/
│   ├── MainPage.xaml           # UI layout & bindings
│   ├── MainPage.cs             # Page initialization
│   ├── MainPage.KeyboardHandling.cs   # Keyboard shortcuts
│   ├── MainPage.MouseHandling.cs      # Mouse interaction
│   └── MainPage.CoordinateAxes.cs     # Axes rendering
├── Services/
│   ├── MandelbrotService.cs    # Fractal computation engine
│   └── ColorScheme.cs          # Color palette management
├── Helpers/
│   ├── BoolNegationConverter.cs
│   ├── BoolToVisibilityConverter.cs
│   ├── InverseBoolToVisibilityConverter.cs
│   └── RelayCommand.cs         # Command implementation
└── KEYBOARD_SHORTCUTS.md       # Complete shortcut reference
```

### Key Design Patterns

**MVVM (Model-View-ViewModel):**
- `MainViewModel` - Application state, commands, business logic
- `MainPage.xaml` - Declarative UI with data bindings
- `MandelbrotService` - Model/computation layer

**Partial Classes:**
- `MainPage.cs` - Core initialization
- `MainPage.KeyboardHandling.cs` - All keyboard shortcuts
- `MainPage.MouseHandling.cs` - Mouse interaction & zoom
- `MainPage.CoordinateAxes.cs` - Overlay rendering

**Commands:**
- `RelayCommand` - ICommand implementation
- ViewModel exposes commands: `RenderMandelbrotCommand`, `ZoomInCommand`, etc.
- UI binds to commands for click handlers
- Keyboard shortcuts invoke same commands

---

## 🛠️ Building from Source

### Prerequisites
- **Visual Studio 2022** (v17.5+) or **Visual Studio 2026**
- **.NET 10 SDK** (included with VS 2026)
- **Windows App SDK** (WinUI 3)

### Build Steps

**Visual Studio:**
1. Open `ManpLab.sln` (or just `ManpWinUI\ManpWinUI.csproj`)
2. Set `ManpWinUI` as startup project
3. Press **F5** to build and run

**Command Line:**
```bash
cd ManpWinUI
dotnet restore
dotnet build
dotnet run
```

**Release Build:**
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

---

## 📚 Documentation

- **[Keyboard Shortcuts](KEYBOARD_SHORTCUTS.md)** - Complete keyboard reference
- **[Architecture Overview](../ARCHITECTURE_FRACTAL_MODULAR_DESIGN.md)** - Design decisions
- **[Coordinate Axes Feature](../docs/COORDINATE_AXES_FEATURE.md)** - Axes implementation details

---

## 🎯 Feature Roadmap

### ✅ Implemented (Phase 3)
- [x] Basic Mandelbrot rendering
- [x] Interactive zoom & pan
- [x] Multiple color palettes
- [x] Julia set mode
- [x] Coordinate axes overlay
- [x] Auto-scale iterations
- [x] Comprehensive keyboard shortcuts
- [x] Resolution presets
- [x] Real-time status updates

### 🔄 In Progress
- [ ] Save/export images (Ctrl+S)
- [ ] Render cancellation (Esc)
- [ ] Parameter presets
- [ ] Color editor

### 🎯 Planned
- [ ] More fractal types (Newton, Burning Ship, etc.)
- [ ] Animation recording
- [ ] Orbit trap visualization
- [ ] Deep zoom with perturbation theory
- [ ] Distance estimation rendering
- [ ] Custom color gradient editor
- [ ] Location bookmarks
- [ ] Undo/redo navigation

---

## 🤝 Contributing

This is the modern WinUI reimplementation of ManpWIN. Contributions welcome!

**Development Focus:**
- Clean, maintainable .NET code
- MVVM best practices
- Comprehensive keyboard shortcuts
- Responsive, modern UI
- Educational documentation

**Guidelines:**
- Follow existing MVVM patterns
- Add keyboard shortcuts to `MainPage.KeyboardHandling.cs`
- Update `KEYBOARD_SHORTCUTS.md` for new features
- Test on Windows 11
- Keep UI responsive (use async/await)

---

## 🔬 Comparison: ManpWinUI vs ManpWIN64

| Feature | ManpWinUI (WinUI 3) | ManpWIN64 (C++) |
|---------|---------------------|-----------------|
| **Platform** | Windows 11, .NET 10 | Windows 10/11, C++17 |
| **UI Framework** | WinUI 3, XAML | Win32 API |
| **Fractals** | 2 types | 240+ types |
| **Deep Zoom** | Standard precision | Arbitrary precision (MPFR) |
| **Architecture** | MVVM, DI | Procedural |
| **Target** | Modern, beginner-friendly | Advanced, research-level |
| **Code** | Clean, documented | Extensive, complex |

**Use ManpWinUI when:**
- Learning fractal basics
- Want modern Windows 11 UI
- Prefer .NET/C# development
- Need clean, understandable code

**Use ManpWIN64 when:**
- Need 240+ fractal types
- Require deep zoom (10^100+ magnification)
- Researching perturbation theory
- Working on performance optimization

---

## 📖 Learning Resources

**Fractals:**
- [Mandelbrot Set Wikipedia](https://en.wikipedia.org/wiki/Mandelbrot_set)
- [Julia Set Wikipedia](https://en.wikipedia.org/wiki/Julia_set)
- [FractalForums.org](https://fractalforums.org/)

**WinUI 3:**
- [WinUI 3 Documentation](https://learn.microsoft.com/windows/apps/winui/)
- [.NET MAUI & WinUI](https://learn.microsoft.com/windows/apps/windows-app-sdk/)

**MVVM:**
- [MVVM Pattern](https://learn.microsoft.com/windows/communitytoolkit/mvvm/introduction)
- [Data Binding in WinUI](https://learn.microsoft.com/windows/apps/winui/winui3/data-binding-overview)

---

## 📄 License

Copyright Mark Hassell Smith (2026)

Built on the educational work of Paul de Leeuw's ManpWIN

For educational and research use. See root LICENSE for details.

---

## 🐛 Issues & Feedback

**Bug Reports:** [GitHub Issues](https://github.com/markhassellsmith/ManpLab/issues)

**Feature Requests:** Use GitHub Discussions or Issues

**Questions:** See the [main repository](https://github.com/markhassellsmith/ManpLab)

---

**Modern WinUI 3 Fractal Explorer** | **Built with .NET 10** | **January 2026**

*Simple, elegant, and educational fractal exploration for Windows 11*
