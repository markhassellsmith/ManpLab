# References & Resources

This document contains links to documentation, research papers, and libraries relevant to the ManpLab WinUI modernization project.

---

## Documentation

### Microsoft Documentation
- [WinUI 3 Documentation](https://docs.microsoft.com/windows/apps/winui/)
- [MVVM Toolkit Documentation](https://learn.microsoft.com/windows/communitytoolkit/mvvm/)
- [C++/CLI Documentation](https://docs.microsoft.com/cpp/dotnet/)
- [.NET MAUI Documentation](https://learn.microsoft.com/dotnet/maui/)
- [MAUI vs WinUI Comparison](https://learn.microsoft.com/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/overall-migration-strategy)
- [Building Cross-Platform Apps](https://learn.microsoft.com/dotnet/architecture/maui/cross-platform-development)

### Project Repository
- [ManpWIN GitHub](https://github.com/markhassellsmith/ManpLab)

---

## Fractal Mathematics

### Fundamental Theory
- **Mandelbrot Set theory** - Classic fractal mathematics
- **Julia Sets** - Related fractals with parameter variations
- **Complex Dynamics** - Mathematical foundation

### Advanced Algorithms
- **Perturbation theory** (Kalles Fraktaler)
  - Enables deep zoom beyond 10^100 magnification
  - Reference orbit calculation
  - Delta iteration techniques
- **BLA (Bilinear Approximation)**
  - Iteration skipping for performance
  - Series approximation algorithms
- **Derivatives and Slopes**
  - Distance estimation
  - Normal map calculation for 3D effects

### Research Papers & Articles
- Perturbation theory for deep zooms
- Bilinear approximation optimization
- Series approximation methods

---

## Libraries & Dependencies

### Arithmetic Precision Libraries
- **MPFR** - Multiple Precision Floating-Point Reliable library
  - Website: https://www.mpfr.org/
  - Provides arbitrary precision arithmetic (100+ decimal places)
  - Used for deep zoom calculations

- **MPIR** - Multiple Precision Integers and Rationals
  - Windows-optimized fork of GMP
  - Backend for MPFR

- **QD Library** - Quad-Double precision arithmetic
  - Website: https://www.davidhbailey.com/dhbsoftware/
  - Provides ~64 decimal digits of precision (~32 digits for double-double)
  - Faster than MPFR for moderate precision needs

### Fractal-Specific Tools
- **Fractint** - Historic fractal generation software
  - Website: https://www.fractint.org/
  - File format compatibility (.PAR files)
  - Formula language reference

- **Kalles Fraktaler** - Deep zoom fractal software
  - .KFR file format support
  - Perturbation algorithm reference implementation

### Graphics & Media Libraries
- **zlib** - Compression library
  - PNG file support
  - Standard library, widely used

- **libpng** - PNG image I/O
  - Image export functionality

- **FFmpeg** - Video encoding (planned replacement for MPEG)
  - Modern codec support (H.264, H.265)
  - MP4 container format

- **Windows Media Foundation** - Alternative to FFmpeg
  - Native Windows video encoding
  - Modern codec support

- **SkiaSharp** (optional) - 2D graphics library
  - Cross-platform graphics rendering
  - Alternative to GDI/DirectX for certain operations

### .NET & WinUI Libraries
- **CommunityToolkit.Mvvm** - MVVM framework helpers
- **CommunityToolkit.WinUI** - Additional WinUI controls
- **Microsoft.Extensions.DependencyInjection** - Dependency injection
- **System.Text.Json** - JSON serialization for settings
- **Serilog** or **NLog** - Logging frameworks

---

## Community & Forums

### Fractal Community
- Fractal Forums
- Ultra Fractal community
- Mandelbrot Set Explorer communities

### Development Communities
- WinUI GitHub Discussions
- .NET Community Toolkit
- Stack Overflow (tags: winui3, xaml, fractals)

---

## Related Projects

### Similar Fractal Software
- **Ultra Fractal** - Commercial fractal software
- **Fractal eXtreme** - Deep zoom specialist
- **Mandelbulber** - 3D fractal renderer
- **XaoS** - Real-time fractal zoomer

### Open Source Alternatives
- **Gnofract 4D** - Python-based fractal generator
- **FractView** - Android fractal app
- **Various JavaScript fractal explorers** - Web-based implementations

---

## Learning Resources

### Tutorials & Guides
- WinUI 3 getting started guides
- MVVM pattern tutorials
- C++/CLI interop best practices
- Fractal mathematics introductions

### Books
- "The Beauty of Fractals" - Classic mathematical text
- "Fractal Geometry" by Kenneth Falconer
- "Programming WinUI 3" - Modern UI development

---

## Tools & Utilities

### Development Tools
- **Visual Studio 2026** - Primary IDE
- **Git** - Version control
- **GitHub** - Repository hosting
- **Markdown editors** - Documentation

### Testing & Profiling
- **Visual Studio Profiler** - Performance analysis
- **BenchmarkDotNet** - .NET benchmarking
- **Visual Studio Test Explorer** - Unit testing

### Design Tools
- **Figma** or **Adobe XD** - UI mockups
- **IcoFX** or **Paint.NET** - Icon creation

---

**Last Updated:** January 2025  
**Maintained By:** ManpLab Development Team
