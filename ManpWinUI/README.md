# ManpLab - Modern Fractal Explorer - Release 1.0 (Educational Fork)

![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-blue)
![C++](https://img.shields.io/badge/C++-17-blue.svg)
![.NET](https://img.shields.io/badge/.NET-10-blue.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)

**A modern WinUI 3 fractal explorer powered by Paul de Leeuw's production-grade rendering engine**

## ðŸš€ Overview

ManpLab combines a modern, intuitive WinUI 3 interface with Paul de Leeuw's exceptional fractal rendering engine - featuring perturbation theory, BLA acceleration, and arbitrary-precision arithmetic for extreme deep zoom capabilities (magnification > 10^100).

## Application Screenshot

![ManpLab Application Screenshot](Documentation/images/ManpLab-Application-Screenshot.png)

*ManpLab Application - Dark Theme - Screenshot (Above)*

### Key Features

- âœ¨ **Modern WinUI 3 Interface** - Clean, responsive UI with MVVM architecture
- ðŸŽ¨ **325 Fractal Types** - Extended from Paul's 246 originals with 79 new implementations ([See complete list â†’](#appendix-complete-fractal-catalog))
- ðŸ”¬ **Deep Zoom Technology** - Perturbation theory with magnifications exceeding 10^100
- âš¡ **BLA Acceleration** - Series approximation for extreme performance at deep zoom levels
- ðŸ§® **Arbitrary Precision** - MPFR, QD, and DD libraries for numerical accuracy
- ðŸŽ¬ **Animation Rendering** - Create MP4 videos with FFmpeg integration
- ðŸ“š **Fractal Browser** - Metadata, formulas, bookmarks, navigation history
- ðŸŽ¨ **Theme System** - Light, Dark, Ocean Blue, and System themes
- ðŸ–±ï¸ **Interactive Exploration** - Mouse, keyboard, and touch navigation
- ðŸ’¾ **Rich Image Metadata** - PNG/JPEG exports embed complete fractal parameters ([View metadata guide â†’](../docs/ImageMetadataGuide.md))
- âŒ¨ï¸ [Full Keyboard Shortcuts](ManpWinUI/KEYBOARD_SHORTCUTS.md)

ðŸ”— **[Get Started with ManpWinUI â†’](ManpWinUI/README.md)**

### Architecture

```
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚   ManpWinUI (WinUI 3 / .NET 10)        â”‚
â”‚   Modern UI, MVVM, Theme System         â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¬â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
                   â”‚
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â–¼â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚   Native C++ Fractal Engine             â”‚
â”‚   (Paul de Leeuw's Production Engine)   â”‚
â”‚   â€¢ Perturbation Theory                 â”‚
â”‚   â€¢ BLA Acceleration                    â”‚
â”‚   â€¢ Arbitrary Precision (MPFR/QD)       â”‚
â”‚   â€¢ 246 Original Fractal Types          â”‚
â”‚   â€¢ Extended to 325 Types in ManpLab    â”‚
â”‚   â€¢ Multithreaded Rendering             â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
```

This educational fork makes Paul's sophisticated rendering technology accessible through a modern, user-friendly interface designed for students, educators, and researchers.

---


## Screenshots

![Mandelbrot Set rendered in the Spectrum palette](Documentation/images/Mandelbrot-in-Spectrum-palette.png)

*Mandelbrot Set rendered in the Spectrum palette*


![Classic Julia Set rendered in the Fire palette](Documentation/images/JuliaClassic-in-Fire-palette.png)

*Classic Julia Set rendered in the Fire palette*


![Zoomed Tetrate rendered in the Psychedelic palette](Documentation/images/Tetrate-zoomed-in-Psychedelic-palette.png)

*Zoomed Tetrate rendered in the Psychedelic palette*


![2-Dimensional Hailstone Sequence with segments and point labels](Documentation/images/Hailstone-sequence.png)

*2-Dimensional Hailstone Sequence with segments and point labels*


---

## Quick Start

### Pre-built Distributions

[![Latest Release](https://img.shields.io/github/v/release/markhassellsmith/ManpLab)](https://github.com/markhassellsmith/ManpLab/releases/latest)

**[Download Latest Release â†’](https://github.com/markhassellsmith/ManpLab/releases/latest)**

#### Portable ZIP (Recommended) âœ…
- **No installation** - extract and run `ManpWinUI.exe`
- **No security warnings** - runs immediately
- **Self-contained** - includes all dependencies
- **Perfect for**: Educational use, quick testing, no admin rights needed

#### MSIX Package (Alternative)
- **Modern Windows app** - clean install/uninstall via Settings
- **âš ï¸ Shows security warning** - unsigned package (normal for open-source)
- See installation guide included in the download
- **Best for**: Users preferring managed apps with auto-update support

### Build from Source

```bash
git clone https://github.com/markhassellsmith/ManpLab.git
cd ManpLab
# Open ManpLab.sln in Visual Studio 2022 and build (F5)
```

All dependencies are included. The project builds without additional configuration.

**Requirements:**
- Windows 10 or 11 (x64)
- Visual Studio 2022 (Community Edition supported)
- .NET 10 SDK
- Git (for cloning)

---

## Educational Applications

ManpLab serves as a comprehensive platform for studying fractals, numerical methods, and computational mathematics across multiple disciplines, combining modern software engineering with advanced mathematical algorithms.

### Mathematics

**Complex Dynamics & Numerical Analysis:**
- 325 fractal types (246 from Paul de Leeuw, 79 new implementations): Mandelbrot, Julia sets, Newton fractals, exotic variants
- Perturbation theory for studying chaotic systems at extreme scales
- Deep zoom with magnifications exceeding 10^100
- Arbitrary-precision arithmetic (MPFR, QD, DD libraries)
- Numerical stability demonstrations and precision management

**Advanced Algorithms:**
- BLA (Bilinear Approximation) series approximation
- Perturbation algorithm with reference orbits
- Newton-Raphson root finding for fractal boundaries
- Analytical derivative calculations for distance estimation

### Computer Science

**Modern Software Architecture:**
- WinUI 3 application with MVVM pattern
- C++/WinRT native-managed interop
- Large-scale C++ engine (156 source files, 6 CMake subprojects)
- Template metaprogramming with generic numeric types
- Service-oriented architecture with dependency injection

**Performance Engineering:**
- Multithreaded rendering engine (utilizes all CPU cores)
- Cache optimization techniques
- Memory management with smart pointers
- Vectorization-ready code structure
- Progressive rendering with cancellation support

### Physics & Engineering

**Applications Across Disciplines:**
- **Electrical:** Chua's circuit, fractal antennas, chaos-based encryption
- **Mechanical:** Turbulent flow, nonlinear oscillators, fracture mechanics
- **General:** Strange attractors (Lorenz, RÃ¶ssler, HÃ©non), bifurcation analysis, orbit traps, Lyapunov exponents

---

## Technical Features

### Native C++ Rendering Engine (Paul de Leeuw's Implementation)

#### Deep Zoom Technology
- Perturbation theory for efficient extreme magnification
- BLA series expansion to skip hundreds of iterations
- Arbitrary precision (MPFR) up to thousands of decimal places
- FloatExp extended exponent range for ultra-deep zooms
- Automatic precision scaling based on zoom level

#### Rendering Capabilities
- Multiple render modes: escape-time, slope/derivative shading, distance estimation
- Potential field, orbit trap, and biomorph coloring
- 24-bit true color with smooth gradients
- Bump mapping and animated color cycling
- Fractint .map palette support

#### Performance Optimizations
- Multithreaded engine utilizing all CPU cores
- Solid guessing and boundary tracing algorithms
- Progressive rendering with cancellation
- Dynamic task distribution
- Memory-mapped file support for large datasets

#### Formula System
- Custom scripting language for fractal definitions
- Virtual machine bytecode execution
- 100+ built-in mathematical functions
- Fractint formula compatibility

### Modern WinUI 3 Interface

- Responsive, touch-friendly design
- MVVM architecture with data binding
- Theme support (Light, Dark, Ocean Blue, System)
- Real-time parameter updates
- Fractal metadata browser with favorites
- Animation timeline editor
- Keyboard shortcuts for power users

#### Image Export with Embedded Metadata

ManpLab embeds comprehensive metadata into every exported PNG and JPEG image, enabling:
- **Exact reproduction** of any fractal render from saved files
- **Sharing parameters** without manual copying
- **Archival preservation** of exploration history
- **Open-source attribution** with automatic GitHub links

**ðŸ“– [Complete Image Metadata Guide â†’](../docs/ImageMetadataGuide.md)**

Metadata includes fractal family, mathematical parameters (center, zoom, iterations), rendering details (color palette, render time), and a full JSON blob for programmatic access. View metadata using ExifTool, XnView, or standard image properties in Windows/macOS.

---

## Fractal Categories (40)

ManpLab features **325 unique fractals** across multiple specialized categories, from classic mathematical sets to cutting-edge physical simulations. [View complete catalog â†’](#appendix-complete-fractal-catalog)

**Classic Fractals (30+):** Mandelbrot variants, Julia sets, Burning Ship, Newton fractals, Magnet fractals, Barnsley systems

**Mathematical Functions (50+):** Trigonometric, exponential, logarithmic, elliptic functions, special functions (Gamma, Bessel, Lambert W), polynomial variants

**Scientific Systems (35+):** Strange attractors (Lorenz, RÃ¶ssler, HÃ©non, Chua), bifurcation diagrams, Lyapunov fractals, reaction-diffusion systems

**Engineering Applications (10+):** Chemical kinetics, mechanical stress/strain, heat transfer, electrical circuits, control systems

**Geometric & IFS (15+):** Sierpinski, Apollonius, Pascal triangle, Barnsley fern, fractal trees

**Orbit Traps & Advanced Coloring (20+):** Circular, cross, triangle traps, stripe averaging, curvature tracking, angle accumulation

**Artistic & Experimental (25+):** Buddhabrot, Biomorphs, Popcorn, Hopalong, Pickover Stalks, Celtic variations

**Research & Hybrid Fractals (20+):** Perturbation-optimized, bifurcation hybrids, mutant Mandelbrot, rational maps

---

## Project Structure

```
ManpLab/
â”œâ”€â”€ ManpWinUI/              # WinUI 3 application (.NET 10)
â”‚   â”œâ”€â”€ ViewModels/         # MVVM view models
â”‚   â”œâ”€â”€ Views/              # XAML pages and controls
â”‚   â”œâ”€â”€ Services/           # Business logic layer
â”‚   â””â”€â”€ Documentation/      # Comprehensive project docs
â”‚
â”œâ”€â”€ ManpCore.Services/      # Shared .NET services
â”‚   â””â”€â”€ FractalEngineWrapper.cs
â”‚
â”œâ”€â”€ ManpCore.Native/        # C++/WinRT interop layer
â”‚   â””â”€â”€ FractalEngineWrapper.cpp/.h
â”‚
â”œâ”€â”€ ManpWIN64/              # Native C++ rendering engine (156 files)
â”‚   â”œâ”€â”€ Perturbation.cpp    # Perturbation algorithm
â”‚   â”œâ”€â”€ Approximation.cpp   # BLA acceleration
â”‚   â”œâ”€â”€ Slope.cpp           # Derivative shading
â”‚   â”œâ”€â”€ BigComplex.cpp      # Arbitrary-precision complex
â”‚   â”œâ”€â”€ Pixel.cpp           # Standard iteration engine
â”‚   â””â”€â”€ ...
â”‚
â”œâ”€â”€ parser/                 # Formula parser & VM (21 files)
â”œâ”€â”€ qdlib/                  # Quad-double arithmetic
â”œâ”€â”€ pnglib/                 # PNG export
â”œâ”€â”€ ZLib/                   # Compression
â””â”€â”€ external/               # MPFR, GMP, FFmpeg libraries
```

### Key Source Categories (Native Engine)

**Core Rendering:** `Pixel.cpp`, `BigPixel.cpp`, `Perturbation.cpp`, `PertEngine.cpp`

**Precision Types:** `Complex.cpp`, `BigComplex.cpp`, `DDComplex.cpp`, `QDComplex.cpp`, `ExpComplex.cpp`

**Algorithms:** `Approximation.cpp`, `Slope.cpp`, `FwdDiff.cpp`, `MandelDerivatives.cpp`

**Fractals:** `FractintFunctions.cpp`, `TierazonFunctions.cpp`, `Miscfrac.cpp`, `Bif.cpp`

**Color:** `Colour.cpp`, `Colour1.cpp`, `ColourMethod.cpp`, `TrueCol.cpp`

---

## Student Project Ideas

### Beginner (1-2 weeks)
1. Add custom color palettes
2. Implement parameter presets
3. Create keyboard shortcuts
4. Implement simple fractal variants

### Intermediate (4-8 weeks)
5. Histogram-based coloring
6. Progressive rendering preview
7. Parameter animation system
8. Undo/redo navigation
9. New escape-time fractals
10. Distance estimation rendering
11. Statistical analysis tools
12. 3D lighting and shadows

### Advanced (8-16 weeks)
13. GPU acceleration (CUDA/OpenCL)
14. Distributed rendering
15. SIMD optimization (AVX2/AVX-512)
16. Adaptive precision management
17. Automatic differentiation
18. Fractal dimension calculator
19. Plugin architecture
20. Cross-platform port (Linux/Mac)

### Research-Level
21. Novel series approximation methods
22. Machine learning for exploration
23. Perturbation theory for complex formulas
24. Real-time deep zoom interaction

---

## Build Instructions

### Visual Studio (Recommended)

1. Install Visual Studio 2022 with:
   - "Desktop development with C++" workload
   - ".NET desktop development" workload
   - .NET 10 SDK
2. Clone repository: `git clone https://github.com/markhassellsmith/ManpLab.git`
3. Open `ManpLab.sln`
4. Build (F5) - ManpWinUI will be set as startup project

### Command Line

```bash
git clone https://github.com/markhassellsmith/ManpLab.git
cd ManpLab
# Build native engine
cmake -B build -G "Visual Studio 17 2022" -A x64
cmake --build build --config Release
# Build WinUI app
dotnet build ManpWinUI/ManpWinUI.csproj -c Release
```

---

## Troubleshooting

**Build Issues:**
- Ensure C++ and .NET workloads are installed
- Verify .NET 10 SDK is present
- Clean and rebuild if linker errors occur
- Check that all NuGet packages restore successfully

**Runtime Issues:**
- Use Release build for production (Debug is significantly slower)
- Ensure native dependencies (MPFR, GMP) are in output directory
- Check Windows 10/11 is up to date for WinUI 3 support

**Performance:**
- Deep zoom automatically enables BLA and perturbation theory
- Reduce max iterations for initial exploration
- Multithreading is automatic (uses all CPU cores)
- Use "Progressive rendering" for interactive feedback

---

## Technology Stack

**Frontend:** .NET 10, WinUI 3, C#, XAML, MVVM Toolkit

**Native Engine:** C++17, Win32 API, CMake 3.23+

**Mathematical Libraries:** MPFR 4.2.2, GMP 6.3.0, QD Library, DD Arithmetic

**Media:** FFmpeg (animation export), libpng, ZLib

---

## Learning Resources

**Books:**
- Mandelbrot, *The Fractal Geometry of Nature*
- Peitgen et al., *Chaos and Fractals*
- Pickover, *Computers, Pattern, Chaos and Beauty*

**Online:**
- [FractalForums.org](https://fractalforums.org/)
- [Kalles Fraktaler](https://github.com/knighty/kf)
- [Inigo Quilez - Distance Estimation](https://iquilezles.org/articles/distancefractals/)

**Papers:**
- Hart, "Distance Estimation for Fractals"
- Claude Heiland-Allen, perturbation theory articles
- Lorenz (1963), "Deterministic Nonperiodic Flow"

---

## Contributing

Contributions are welcome from students, educators, and researchers.

**Guidelines:**
- Test Debug and Release builds
- Keep dependencies in `external/` directory
- Follow existing code style
- Document significant changes
- Maintain backward compatibility

**Development Workflow:**
1. Fork repository
2. Create feature branch
3. Make changes and test
4. Submit pull request with description

**Priority Areas:**
- GPU acceleration, additional fractals, performance optimizations
- Documentation, tutorials, unit tests
- Novel algorithms, research contributions

---

## Credits

**Paul de Leeuw (Paul the LionHeart)** - Native rendering engine with perturbation theory, BLA acceleration, and 246 original fractal implementations

**Mark Hassell Smith** - Modern WinUI 3 interface, MVVM architecture, 79 new fractals, metadata system, and educational materials

**GitHub Copilot** - Development assistance and documentation support

Special thanks to the fractal community at FractalForums.org for continued inspiration and technical contributions.

---

## License

MIT License - See LICENSE file for details.

This project includes third-party libraries with their own licenses (MPFR, GMP, libpng, ZLib).

---

## Version History

**v1.0 (2026)** - Educational fork release
- Modern WinUI 3 interface with MVVM architecture
- Complete integration of Paul de Leeuw's native engine
- Extended from 246 to 325 fractal types (79 new implementations)
- Comprehensive fractal metadata system
- Animation rendering with FFmpeg
- Theme system and accessibility features
- Comprehensive documentation
- Self-contained dependency management

**Original ManpWIN** - Paul de Leeuw (multiple versions 1990s-2010s)
- Deep zoom with perturbation theory
- BLA acceleration algorithms
- Formula parser and 246 fractal implementations
- Arbitrary-precision arithmetic integration

---

## Appendix: Complete Fractal Catalog

ManpLab includes **325 unique fractals** organized into 40 categories. This comprehensive catalog spans classic mathematical fractals, strange attractors, physical systems, and experimental variations.

**Note:** This catalog reflects the complete collection available in the Fractal Browser as of June 2026.
### Attractors (7)

- Aizawa Attractor
- Chen-Lee Attractor
- Dadras Attractor
- Halvorsen Attractor
- Lorenz Attractor
- Pickover Attractor
- Thomas Attractor

### Barnsley (6)

- Barnsley J1
- Barnsley J2
- Barnsley J3
- Barnsley M1
- Barnsley M2
- Barnsley M3

### Bifurcation (7)

- Henon Bifurcation
- Henon Parameter Space
- Lambda Bifurcation
- Lambda Parameter Space
- Logistic Bifurcation
- Mandelbrot Parameter Space
- Orbit Diagram

### Burning Ship Family (2)

- Burning Ship (Power 3)
- Burning Ship (Power 4)

### Burning Ship Variants (12)

- Bird of Prey
- Buffalo Burning Ship
- Burning Ship Cubic
- Burning Ship Quartic
- Burning Ship Quintic
- Celtic Burning Ship
- Diagonal Burning Ship
- Partial Burning Ship
- Perpendicular Burning Ship
- Reverse Burning Ship
- Shark Burning Ship
- Vertical Burning Ship

### Chaotic Maps (4)

- Arneodo Attractor
- Liu-Chen Attractor
- Rabinovich-Fabrikant Attractor
- Sprott B Attractor

### Chemical Engineering (4)

- Arrhenius Kinetics Map (Thermal Activation)
- Cahn-Hilliard Map (Phase Separation)
- Gray-Scott Autocatalysis (Reaction-Diffusion)
- Langmuir-Hinshelwood Isotherm

### Classic Fractals (5)

- Lambda
- Mandelbrot Set
- Mandel-Lambda
- Tetrate
- Unity

### Classical Polynomials (2)

- Chebyshev Polynomial
- Legendre Polynomial

### Complex Functions (8)

- 1/sin(z)Â²
- cos(z)/tan(z)
- Square + Trig
- Tetration (z^z)
- Trig + Trig
- Trig Ã— Trig
- Trig Squared
- zÂ·sin(z) + z

### Discrete Mathematics (3)

- Chebyshev Polynomial Tâ‚ƒ(z) = 4zÂ³ - 3z + c
- Combinatorial Mandelbrot zâ´ - z + c
- Inverse Combinatorial 1/zÂ² + c

### Distance Estimator (4)

- Burning Ship (Distance Estimator)
- Julia (Distance Estimator)
- Mandelbrot (Distance Estimator)
- Tricorn (Distance Estimator)

### Elliptic Functions (4)

- Jacobi Elliptic sn
- Weierstrass â„˜-function
- Weierstrass Î¶-function
- Weierstrass Ïƒ-function

### Exotic (8)

- Buffalo Fractal
- Celtic Mandelbrot
- Heart Mandelbrot
- Perpendicular Mandelbrot
- Quasi-Perpendicular
- Shark Fin
- Wavy
- Zubieta

### Exotic Fractals (3)

- Magnet I
- Magnet II
- Phoenix Fractal

### Exponential Fractals (12)

- Complex Power
- Exponential Fractal
- Exponential Julia
- Exponential Logarithmic
- Exponential Square
- Lambda Lambda Exponential
- Lambda Mandel Exponential
- Logarithm Fractal
- Logarithmic Mandelbrot
- Mandelbrot Exponential
- Power Tower (z^z)
- z^z + c

### Historical Fractals (8)

- Chip Map
- Collatz Fractal
- Duffing Map
- Martin Map
- Pickover Biomorphs
- Pickover Stalks
- Quaternion Julia (2D slice)
- Sinusoidal Fractal

### Hybrid Fractals (18)

- Bifurcation-Mandelbrot
- Burning Mandelbrot Hybrid
- Celtic Mandelbrot (Hybrid)
- Celtic-Burning Ship Hybrid
- Collatz-Style Hybrid
- Exponential-Mandelbrot Blend
- Exponential-Mandelbrot Hybrid
- Magnet-Mandelbrot Hybrid
- Mandelbrot-Burning Ship Hybrid
- Mandelbrot-Lambda Mix
- Multi-Power Cycle
- Mutant Mandelbrot (Power Evolution)
- Newton-Mandelbrot Blend
- Perturbed Newton
- Sierpinski-Mandelbrot Cross
- Sine-Mandelbrot Hybrid
- Tricorn-Phoenix Hybrid
- Trig-Mandelbrot Blend

### Iterated Function Systems (5)

- Barnsley Fern (IFS)
- Dragon Curve (IFS)
- Pentagon (IFS)
- Sierpinski Triangle (IFS)
- Tree (IFS)

### Julia Presets (23)

- Julia - Airplane
- Julia - Backbone
- Julia - Cauliflower
- Julia - Crystal
- Julia - Dendrite (Preset)
- Julia - Dragon (Preset)
- Julia - Eye
- Julia - Feigenbaum Point
- Julia - Flower
- Julia - Fractal Tree
- Julia - Fuzzy Blob
- Julia - Golden Ratio
- Julia - Heart
- Julia - Lightning
- Julia - Medusa
- Julia - Neurons
- Julia - Paisley
- Julia - Seahorse Valley
- Julia - Snowflake
- Julia - Spiral (Preset)
- Julia - Spiral Galaxy
- Julia - Triple Spiral
- Julia - Twisted Cross

### Julia Sets (21)

- Julia - Burning Ship
- Julia - Classic
- Julia - Cubic
- Julia - Custom
- Julia - Dendrite
- Julia - Douady Rabbit
- Julia - Dragon
- Julia - Exponential
- Julia - Lambda
- Julia - Lambda (Alt)
- Julia - Magnet
- Julia - Multibrot 3
- Julia - Multibrot 4
- Julia - Phoenix
- Julia - Power 5
- Julia - Power 6
- Julia - San Marco
- Julia - Siegel Disk
- Julia - Siegel Disk (Alt)
- Julia - Sine
- Julia - Spiral



### L-Systems (7)

- Dragon Curve
- Fractal Plant
- Hilbert Curve
- Koch Curve
- Koch Snowflake
- Peano Curve
- Sierpinski Triangle

### Lambda Fractals (8)

- Lambda Flip
- Lambda Modified
- Lambda Phoenix
- Lambda Power 3
- Lambda Power 4
- Lambda Squared
- Lambda Tan
- Lambda Tanh

### Magnet Fractals (4)

- Magnet I Cubic
- Magnet I Julia
- Magnet II Cubic
- Magnet II Julia

### Mandelbrot Variants (27)

- Burning Ship
- Celtic Buffalo
- Celtic Heart
- Heart Mandelbrot (Sine)
- Julia Power 4
- Mandelbar
- Mandelbar (Conjugate)
- Mandelbrot Power 4
- Mandelbrot-Lambda
- Manowar
- Marks Julia
- Marks Mandelbrot
- Marks Mandelbrot (Classic)
- Multibrot (Power 6)
- Multibrot (Power 7)
- Multibrot (Power 8)
- Multibrotâ´ (Quartic)
- Multibrotâµ (Quintic)
- MultibrotÂ³ (Cubic)
- Perpendicular Mandelbrot (Abs First)
- Shark Fin Mandelbrot
- Spider
- Spider Variant
- Thorn
- Thorn (Classic)
- Tricorn (Mandelbar)
- Wavy Mandelbrot

### Mechanical Engineering (5)

- Basquin Fatigue Power Law (S-N Curve)
- Euler-Bernoulli Buckling (Beam Deflection)
- Ramberg-Osgood Plastic Deformation (Malleability)
- Stefan-Boltzmann Radiative Cooling (Heat Flow)
- Torsional Twist (Angle of Twist)

### Multibrot Powers (8)

- Buffalo (Polynomial)
- Multibrot-10 (Decic)
- Multibrot-3 (Cubic)
- Multibrot-4 (Quartic)
- Multibrot-5 (Quintic)
- Multibrot-6 (Sextic)
- Multibrot-8 (Octic)
- Tricorn (Polynomial)

### Newton's Method (8)

- Newton (zÂ³-1)
- Newton Basin (zÂ³-1)
- Newton Cosh
- Newton Quartic (zâ´-1)
- Newton Quintic (zâµ-1)
- Newton Sextic (zâ¶-1)
- Newton Sine
- Nova

### Orbit Statistics (4)

- Angle Average
- Average Distance
- Maximum Distance
- Minimum Distance

### Orbit Trap (4)

- Orbit Trap (Circle)
- Orbit Trap (Cross)
- Orbit Trap (Point)
- Orbit Trap (Square)

### Orbital Advanced (10)

- Circular Orbit Trap
- Cross Orbit Trap
- Delta Magnitude Tracking
- Orbit Angle Accumulation
- Orbital Curvature Tracking
- Point-Line Orbit Trap
- Smoothed Orbit (Running Average)
- Stalks (Conditional)
- Stripe Average Coloring
- Triangle Orbit Trap

### Phoenix Fractals (8)

- Phoenix Complex Feedback
- Phoenix Cosh
- Phoenix Cubic
- Phoenix Julia
- Phoenix Lambda
- Phoenix Mandelbrot
- Phoenix Quartic
- Phoenix Sine

### Polynomial Variants (8)

- Biomorph
- Cubic Mandelbrot
- Polynomial zâ´+zÂ³+c
- Polynomial zÂ³-z+c
- Quartic Mandelbrot
- Quintic Mandelbrot
- Rational R1
- Sextic Mandelbrot

### Rational Function Fractals (8)

- Halley's Method zÂ³-1
- MÃ¶bius Fractal
- Newton zâ´-1
- Newton zâµ-1
- Newton zÂ³-1
- Rational (zÂ²+c)/(zÂ²-c)
- Rational zÂ²/(zÂ³+c)
- Rational zÂ³/(zÂ³+c)

### Special (7)

- 2-D Hailstone Trajectory
- Buddhabrot (Classic)
- Hailstone Sequence
- Lyapunov
- Nebulabrot (Dramatic RGB)
- NumFractal
- Tetration (Classic)

### Special Function Fractals (15)

- Bessel-like Oscillatory
- Bose-Einstein Distribution
- Continued Fraction Fractal
- Damped Harmonic Oscillator
- Digamma Function Ïˆ(z)
- Error Function (erf) Fractal
- Fermi-Dirac Distribution
- Gamma Function Fractal
- Hyperbolic Combination
- Lambert W Function
- Planck Distribution
- RLC Circuit Resonance
- Root Locus (Control Systems)
- Tetration (Power Tower)
- Trigamma Function Ïˆ'(z)

### Strange Attractors (6)

- Bedhead Attractor
- Clifford Attractor
- De Jong Attractor
- Duffing Attractor
- Svensson Attractor
- Tinkerbell Attractor

### Tricorn Family (2)

- Tricorn (Power 3)
- Tricorn (Power 4)

### Trigonometric (8)

- Cosecant Mandelbrot
- Cosh Mandelbrot
- Cotangent Mandelbrot
- Mandel Trig
- Secant Mandelbrot
- Sech Mandelbrot
- Sinh Mandelbrot
- Tanh Mandelbrot

### Trigonometric Fractals (12)

- Cos(z) + c
- Lambda Lambda Cosh
- Lambda Lambda Cosine
- Lambda Lambda Sine
- Lambda Lambda Sinh
- Lambda Mandel Cosh
- Lambda Mandel Cosine
- Lambda Mandel Sine
- Lambda Mandel Sinh
- Mandelbrot Trig
- Sin(z) + c
- Sine Fractal
---

**Total Fractals:** 325 (79 new implementations beyond Paul de Leeuw's original 246)

*This catalog represents ManpLab v1.0, spanning mathematical functions, physical systems, strange attractors, and experimental variations.*
