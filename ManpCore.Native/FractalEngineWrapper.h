#pragma once

using namespace System;
using namespace System::Runtime::InteropServices;

namespace ManpCore {
namespace Native {

    /// <summary>
    /// Fractal rendering category determines appropriate visualization behavior.
    /// </summary>
    public enum class FractalCategory
    {
        /// <summary>Standard escape-time fractals (Mandelbrot, Julia, Burning Ship, etc.) - escape percentage is meaningful</summary>
        EscapeTime2D = 0,

        /// <summary>Sequence-based fractals (Hailstone, bifurcation) - use step/trajectory counts</summary>
        Sequence2D = 1,

        /// <summary>Legacy 3D attractor rendering (deprecated - use HistogramBased instead)</summary>
        AttractorBased3D = 2,

        /// <summary>Histogram/orbit accumulation rendering (attractors, flame fractals) - escape percentage not meaningful</summary>
        HistogramBased = 3,

        /// <summary>Special renderers (Buddhabrot, perturbation) - custom metrics</summary>
        Special = 4
    };

    /// <summary>
    /// Color palette types for fractal rendering.
    /// Each palette uses mathematical color generation for smooth, infinite-resolution gradients.
    /// </summary>
    /// <remarks>
    /// Palettes are procedurally generated and support any iteration count without banding.
    /// For custom palettes, see the palette import system in Phase 4+.
    /// </remarks>
    public enum class ColorPalette
    {
        /// <summary>Black to white linear gradient (classic mathematical visualization)</summary>
        Grayscale = 0,

        /// <summary>Blue to cyan to white gradient (traditional fractal coloring)</summary>
        Classic = 1,

        /// <summary>Black to red to yellow to white (hot color scheme)</summary>
        Fire = 2,

        /// <summary>Deep blue to cyan to white (cool ocean colors)</summary>
        Ocean = 3,

        /// <summary>Inverted complementary colors - negative spectrum effect (cyan to magenta)</summary>
        Afterimage = 4,

        /// <summary>Vibrant multi-color psychedelic pattern with wave functions</summary>
        Psychedelic = 5,

        /// <summary>Pure HSV color wheel at 100% saturation - smooth progression through all hues</summary>
        Spectrum = 6,

        /// <summary>Electric neon colors with rapid cycling for maximum detail revelation</summary>
        Neon = 7
    };

    /// <summary>
    /// High-precision floating point number for deep zoom rendering.
    /// Supports up to 30 decimal digits of precision for extreme magnification levels (zoom > 10^15).
    /// </summary>
    /// <remarks>
    /// <para>Phase 2 Implementation: Uses SimpleBigDouble (double-precision wrapper with string serialization).</para>
    /// <para>Future Implementation: Will use MPFR library for true arbitrary-precision arithmetic.</para>
    /// <para>Performance: ~2-10x slower than double for arithmetic operations, but essential for deep zoom beyond double precision limits (~10^-15).</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create high-precision coordinate for deep zoom
    /// var centerX = new BigDouble(-0.7463, 30);  // 30 decimal digits
    /// var viewWidth = new BigDouble(1e-16, 30);  // Zoom level 10^16
    /// 
    /// // Use in fractal parameters
    /// var params = new FractalParameters 
    /// {
    ///     BigCenterX = centerX,
    ///     BigViewWidth = viewWidth,
    ///     MaxIterations = 5000  // Deep zoom requires more iterations
    /// };
    /// </code>
    /// </example>
    public ref class BigDouble
    {
    private:
        void* m_nativeBigDouble;  // Pointer to native SimpleBigDouble
        int m_precision;

    public:
        /// <summary>
        /// Create BigDouble from regular double with default 16-digit precision.
        /// </summary>
        /// <param name="value">Initial value (limited to double precision ~15-16 digits)</param>
        BigDouble(double value);

        /// <summary>
        /// Create BigDouble with specified precision (decimal digits).
        /// </summary>
        /// <param name="value">Initial value (limited to double precision ~15-16 digits)</param>
        /// <param name="precision">Number of decimal digits to maintain (16-30 recommended for deep zoom)</param>
        /// <remarks>
        /// Higher precision increases memory usage and computation time.
        /// Typical usage: 20-25 digits for zoom levels 10^10 to 10^20.
        /// </remarks>
        BigDouble(double value, int precision);

        /// <summary>
        /// Create a copy of an existing BigDouble with same precision.
        /// </summary>
        /// <param name="other">BigDouble to copy</param>
        /// <exception cref="ArgumentNullException">Thrown if other is null</exception>
        BigDouble(BigDouble^ other);

        /// <summary>
        /// Destructor - releases managed and native resources.
        /// </summary>
        ~BigDouble();

        /// <summary>
        /// Finalizer - releases native SimpleBigDouble memory.
        /// </summary>
        !BigDouble();

        /// <summary>
        /// Convert to double (loses precision for very large/small numbers or high-precision values).
        /// </summary>
        /// <returns>Double approximation of the value (limited to ~15-16 significant digits)</returns>
        /// <remarks>
        /// Use only for display or when precision loss is acceptable.
        /// For deep zoom calculations, keep values as BigDouble.
        /// </remarks>
        double ToDouble();

        /// <summary>
        /// Get string representation with full precision in scientific notation.
        /// </summary>
        /// <returns>String in format "1.234567890123456789e+10" with all significant digits</returns>
        /// <remarks>
        /// Use for serialization, debugging, or displaying exact values.
        /// Can be parsed back with BigDouble::Parse().
        /// </remarks>
        String^ ToString() override;

        /// <summary>
        /// Parse BigDouble from string representation (scientific or decimal notation).
        /// </summary>
        /// <param name="str">String in format "1.234e+10" or "123.456"</param>
        /// <returns>New BigDouble with value parsed from string</returns>
        /// <exception cref="ArgumentNullException">Thrown if str is null or empty</exception>
        /// <exception cref="FormatException">Thrown if str cannot be parsed as a number</exception>
        static BigDouble^ Parse(String^ str);

        /// <summary>
        /// Get or set precision in decimal digits.
        /// </summary>
        /// <value>Number of decimal digits maintained (16-30 typical range)</value>
        /// <remarks>
        /// Changing precision after construction may cause rounding.
        /// Setting higher precision does not add digits, only maintains more in future operations.
        /// </remarks>
        property int Precision
        {
            int get() { return m_precision; }
            void set(int value) { m_precision = value; }
        }

        /// <summary>Add two BigDouble values</summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>Result with precision of max(a.Precision, b.Precision)</returns>
        static BigDouble^ operator+(BigDouble^ a, BigDouble^ b);

        /// <summary>Subtract two BigDouble values</summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>Result with precision of max(a.Precision, b.Precision)</returns>
        static BigDouble^ operator-(BigDouble^ a, BigDouble^ b);

        /// <summary>Multiply two BigDouble values</summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>Result with precision of max(a.Precision, b.Precision)</returns>
        static BigDouble^ operator*(BigDouble^ a, BigDouble^ b);

        /// <summary>Divide two BigDouble values</summary>
        /// <param name="a">Numerator</param>
        /// <param name="b">Denominator</param>
        /// <returns>Result with precision of max(a.Precision, b.Precision)</returns>
        /// <exception cref="DivideByZeroException">Thrown if b is zero</exception>
        static BigDouble^ operator/(BigDouble^ a, BigDouble^ b);

        /// <summary>Compare if first BigDouble is less than second</summary>
        static bool operator<(BigDouble^ a, BigDouble^ b);

        /// <summary>Compare if first BigDouble is greater than second</summary>
        static bool operator>(BigDouble^ a, BigDouble^ b);
    };

    /// <summary>
    /// Parameters for fractal calculation including coordinates, iterations, and rendering options.
    /// </summary>
    /// <remarks>
    /// <para>Coordinates can be specified as regular doubles (for zoom levels up to 10^15) 
    /// or BigDouble (for extreme deep zoom beyond 10^15).</para>
    /// <para>If BigDouble coordinates are provided, they take precedence over regular double coordinates.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Standard Mandelbrot with double precision
    /// var standardParams = new FractalParameters 
    /// {
    ///     FractalType = "Mandelbrot",
    ///     Width = 1920,
    ///     Height = 1080,
    ///     CenterX = -0.5,
    ///     CenterY = 0.0,
    ///     ViewWidth = 3.0,
    ///     MaxIterations = 1000,
    ///     Palette = ColorPalette.Classic
    /// };
    /// 
    /// // Deep zoom with BigDouble precision
    /// var deepZoomParams = new FractalParameters 
    /// {
    ///     FractalType = "Mandelbrot",
    ///     Width = 1920,
    ///     Height = 1080,
    ///     BigCenterX = new BigDouble(-0.7463, 25),
    ///     BigCenterY = new BigDouble(0.1102, 25),
    ///     BigViewWidth = new BigDouble(1e-16, 25),
    ///     MaxIterations = 5000,
    ///     Palette = ColorPalette.Rainbow
    /// };
    /// 
    /// // Julia set
    /// var juliaParams = new FractalParameters 
    /// {
    ///     FractalType = "Julia",
    ///     IsJuliaSet = true,
    ///     JuliaCX = -0.7,
    ///     JuliaCY = 0.27015,
    ///     MaxIterations = 256
    /// };
    /// </code>
    /// </example>
    public ref class FractalParameters
    {
    public:
        /// <summary>
        /// Fractal formula type name (e.g., "Mandelbrot", "Julia", "BurningShip").
        /// </summary>
        /// <value>Case-insensitive fractal type name from available types</value>
        /// <remarks>
        /// Phase 2: Supports "Mandelbrot" and "Julia" only.
        /// Future: Will support 240+ fractal types from ManpWIN64 engine.
        /// Use FractalEngineWrapper::GetAvailableFractalTypes() to query supported types.
        /// </remarks>
        property String^ FractalType;

        /// <summary>
        /// Maximum number of iterations before considering a point in the set.
        /// </summary>
        /// <value>Positive integer, typically 256-10000 depending on zoom level</value>
        /// <remarks>
        /// <para>Higher iterations = more detail but slower rendering.</para>
        /// <para>Recommended values:</para>
        /// <list type="bullet">
        /// <item>Quick preview: 100-256</item>
        /// <item>Standard quality: 500-1000</item>
        /// <item>High quality: 2000-5000</item>
        /// <item>Deep zoom (10^10): 5000-10000</item>
        /// <item>Extreme zoom (10^100): 50000+</item>
        /// </list>
        /// </remarks>
        property int MaxIterations;

        /// <summary>Image width in pixels</summary>
        /// <value>Positive integer, typically 800-3840</value>
        property int Width;

        /// <summary>Image height in pixels</summary>
        /// <value>Positive integer, typically 600-2160</value>
        property int Height;

        // Complex plane coordinates

        /// <summary>
        /// X-coordinate of view center in complex plane (real part).
        /// </summary>
        /// <value>Double precision value, typically -2.0 to 1.0 for Mandelbrot set</value>
        /// <remarks>
        /// Ignored if BigCenterX is set (high-precision mode).
        /// Default: -0.5 (center of Mandelbrot set)
        /// </remarks>
        property double CenterX;

        /// <summary>
        /// Y-coordinate of view center in complex plane (imaginary part).
        /// </summary>
        /// <value>Double precision value, typically -1.5 to 1.5 for Mandelbrot set</value>
        /// <remarks>
        /// Ignored if BigCenterY is set (high-precision mode).
        /// Default: 0.0 (real axis)
        /// </remarks>
        property double CenterY;

        /// <summary>
        /// Width of visible area in complex plane coordinates.
        /// </summary>
        /// <value>Positive double, typically 0.001 to 4.0</value>
        /// <remarks>
        /// <para>Smaller value = higher magnification/zoom.</para>
        /// <para>ViewWidth = 3.0 shows full Mandelbrot set.</para>
        /// <para>ViewWidth = 1e-10 is deep zoom.</para>
        /// <para>Ignored if BigViewWidth is set (high-precision mode).</para>
        /// </remarks>
        property double ViewWidth;

        /// <summary>
        /// Height of visible area in complex plane coordinates.
        /// </summary>
        /// <value>Positive double, should maintain aspect ratio with ViewWidth</value>
        /// <remarks>
        /// Typically: ViewHeight = ViewWidth * (Height / Width) to maintain aspect ratio.
        /// Ignored if BigViewHeight is set (high-precision mode).
        /// </remarks>
        property double ViewHeight;

        // High-precision coordinates for deep zoom (optional - use when zoom > 10^15)

        /// <summary>
        /// High-precision X-coordinate of view center (overrides CenterX when set).
        /// </summary>
        /// <value>BigDouble with 20-30 decimal digits, or null for standard double precision</value>
        /// <remarks>
        /// Use when zoom level exceeds double precision limits (~10^-15).
        /// If null, CenterX is used instead.
        /// </remarks>
        property BigDouble^ BigCenterX;

        /// <summary>
        /// High-precision Y-coordinate of view center (overrides CenterY when set).
        /// </summary>
        /// <value>BigDouble with 20-30 decimal digits, or null for standard double precision</value>
        property BigDouble^ BigCenterY;

        /// <summary>
        /// High-precision view width (overrides ViewWidth when set).
        /// </summary>
        /// <value>BigDouble with 20-30 decimal digits, or null for standard double precision</value>
        property BigDouble^ BigViewWidth;

        /// <summary>
        /// High-precision view height (overrides ViewHeight when set).
        /// </summary>
        /// <value>BigDouble with 20-30 decimal digits, or null for standard double precision</value>
        property BigDouble^ BigViewHeight;

        /// <summary>
        /// Number of decimal digits of precision to use for BigDouble calculations.
        /// </summary>
        /// <value>Integer, typically 20-30 for deep zoom</value>
        /// <remarks>
        /// Only used when BigDouble coordinates are provided.
        /// Higher precision increases accuracy but decreases performance.
        /// </remarks>
        property int Precision;

        // Julia set parameters (if applicable)

        /// <summary>
        /// If true, renders Julia set instead of Mandelbrot set.
        /// </summary>
        /// <value>True for Julia set, false for Mandelbrot set (default)</value>
        /// <remarks>
        /// When true, JuliaCX and JuliaCY specify the constant 'c' parameter.
        /// Each (c_x, c_y) pair produces a different Julia set shape.
        /// </remarks>
        property bool IsJuliaSet;

        /// <summary>
        /// Julia set constant - real part (c_x).
        /// </summary>
        /// <value>Double value, typically -1.0 to 1.0</value>
        /// <remarks>
        /// Classic values: (-0.7, 0.27015), (-0.8, 0.156), (0.285, 0.01)
        /// Only used when IsJuliaSet = true.
        /// </remarks>
        property double JuliaCX;

        /// <summary>
        /// Julia set constant - imaginary part (c_y).
        /// </summary>
        /// <value>Double value, typically -1.0 to 1.0</value>
        property double JuliaCY;

        // Color palette selection

        /// <summary>
        /// Color palette for rendering escaped points.
        /// </summary>
        /// <value>ColorPalette enum value (Grayscale, Classic, Fire, Ocean, Afterimage, Psychedelic, Spectrum)</value>
        /// <remarks>
        /// Points in the set (reached MaxIterations) are always rendered black.
        /// Palette colors escaped points based on iteration count with smooth gradients.
        /// </remarks>
        property ColorPalette Palette;

        /// <summary>
        /// Color offset in degrees (0-360) for rotating the color palette.
        /// </summary>
        /// <value>Integer from 0 to 360, rotates the color spectrum</value>
        /// <remarks>
        /// Shifts the entire color palette by the specified number of degrees.
        /// For example, offset of 180° inverts colors (red becomes cyan, etc.).
        /// Useful for exploring different color variations of the same fractal structure.
        /// </remarks>
        property int ColorOffset;

        /// <summary>
        /// Render mode for fractal coloring algorithm.
        /// </summary>
        /// <value>0=EscapeTime, 1=SmoothColoring, 2=DistanceEstimation, 3=OrbitTrap</value>
        /// <remarks>
        /// - EscapeTime: Standard iteration count coloring
        /// - SmoothColoring: Uses the smooth coloring algorithm as primary method
        /// - DistanceEstimation: Highlights fractal boundaries/edges
        /// - OrbitTrap: Colors based on orbit proximity to geometric shapes
        /// </remarks>
        property int RenderMode;

        /// <summary>
        /// Enable fractional iteration counts for smoother color gradients.
        /// </summary>
        /// <value>True to use fractional iterations (eliminates banding), false for integer-only</value>
        /// <remarks>
        /// When true, uses log(log|z|) formula to compute fractional escape iterations.
        /// Works with any render mode to eliminate color banding artifacts.
        /// When false, uses integer iteration counts (classic banding effect).
        /// </remarks>
        property bool UseSmoothColoring;

        /// <summary>
        /// Creates FractalParameters with default Mandelbrot set values.
        /// </summary>
        /// <remarks>
        /// Defaults: Mandelbrot set, 800x600, 256 iterations, full set view, Classic palette.
        /// </remarks>
        FractalParameters()
        {
            // Default Mandelbrot set parameters
            FractalType = "Mandelbrot";
            MaxIterations = 256;
            Width = 800;
            Height = 600;
            CenterX = -0.5;
            CenterY = 0.0;
            ViewWidth = 3.0;
            ViewHeight = 2.25;
            IsJuliaSet = false;
            JuliaCX = 0.0;
            JuliaCY = 0.0;
            Palette = ColorPalette::Classic;  // Default to classic fractal colors
            ColorOffset = 0;  // No offset by default
            RenderMode = 0;  // Default to EscapeTime
            UseSmoothColoring = true;  // Default to smooth (anti-banding ON)
            Precision = 50;  // Default MPFR precision for deep zoom
        }
    };

    /// <summary>
    /// Result of fractal calculation containing rendered image data and statistics.
    /// </summary>
    /// <remarks>
    /// PixelData is in RGBA format (4 bytes per pixel): Red, Green, Blue, Alpha.
    /// Memory layout: [R0,G0,B0,A0, R1,G1,B1,A1, ...] row by row from top to bottom.
    /// </remarks>
    public ref class FractalResult
    {
    public:
        /// <summary>
        /// RGBA pixel data array (4 bytes per pixel).
        /// </summary>
        /// <value>Byte array of length Width * Height * 4</value>
        /// <remarks>
        /// <para>Format: [R, G, B, A] for each pixel, row-major order.</para>
        /// <para>Alpha channel is always 255 (fully opaque).</para>
        /// <para>Can be directly copied to WriteableBitmap.PixelBuffer for display.</para>
        /// </remarks>
        property array<Byte>^ PixelData;

        /// <summary>Image width in pixels</summary>
        /// <value>Matches FractalParameters.Width</value>
        property int Width;

        /// <summary>Image height in pixels</summary>
        /// <value>Matches FractalParameters.Height</value>
        property int Height;

        /// <summary>
        /// Total time spent rendering the fractal.
        /// </summary>
        /// <value>TimeSpan including calculation time and color mapping</value>
        /// <remarks>
        /// Does not include marshalling overhead or event invocation time.
        /// Pure calculation time measured in native C++ code.
        /// </remarks>
        property TimeSpan RenderTime;

        /// <summary>
        /// Total number of iterations performed across all pixels.
        /// </summary>
        /// <value>Sum of iteration counts for all pixels (can exceed Int32.MaxValue for large images)</value>
        /// <remarks>
        /// Useful for performance analysis and algorithm optimization.
        /// IterationCount / PixelCount = average iterations per pixel.
        /// </remarks>
        property long long IterationCount;

        /// <summary>
        /// Number of pixels that escaped (diverged) rather than stayed in the set.
        /// </summary>
        /// <value>Count of pixels with iteration < MaxIterations</value>
        /// <remarks>
        /// Useful for diagnosing "black screen" issues.
        /// If EscapedPixelCount is very low, you're looking at the interior of the set.
        /// Percentage escaped = (EscapedPixelCount / TotalPixels) * 100
        /// NOTE: Only meaningful for EscapeTime2D fractals. For HistogramBased fractals, this is always 0.
        /// </remarks>
        property int EscapedPixelCount;

        /// <summary>
        /// Fractal rendering category for context-aware status messages.
        /// </summary>
        /// <value>Category indicating the type of fractal and appropriate UI feedback</value>
        /// <remarks>
        /// Used to generate appropriate status bar messages:
        /// - EscapeTime2D: Show escape percentage and boundary guidance
        /// - HistogramBased: Show orbit accumulation info, no escape percentage
        /// - Sequence2D: Show trajectory/sequence metrics
        /// </remarks>
        property FractalCategory Category;

        // ========== Deep Zoom / Perturbation-specific properties ==========

        /// <summary>
        /// Whether perturbation theory was used for this render.
        /// </summary>
        /// <value>true if reference orbit + perturbation was used; false for direct iteration</value>
        /// <remarks>
        /// Perturbation theory is automatically enabled for deep zooms (zoom > 10^15).
        /// Provides 10-100x speedup vs. brute-force high-precision arithmetic.
        /// </remarks>
        property bool UsedPerturbation;

        /// <summary>
        /// Arithmetic precision mode used for this render.
        /// </summary>
        /// <value>
        /// 0 = DOUBLE (normal double precision)
        /// 1 = FLOATEXP (extended precision with exponent)
        /// 2 = DBL_UNSUPPORTED (double precision, no BLA optimization)
        /// 3 = EXP_UNSUPPORTED (extended precision, no BLA optimization)
        /// </value>
        /// <remarks>
        /// FLOATEXP mode automatically activates when precision > 300 bits (~90 decimal digits).
        /// Only Mandelbrot and Power fractals support full BLA optimization (modes 0 and 1).
        /// </remarks>
        property int ArithType;

        /// <summary>
        /// Maximum iteration count where reference orbit escaped.
        /// </summary>
        /// <value>Iteration count where reference orbit magnitude exceeded bailout; 0 if didn't escape</value>
        /// <remarks>
        /// Critical for BLA (Bilinear Approximation) optimization.
        /// BLA table is built up to this iteration count.
        /// If reference orbit never escapes, MaxRefIteration = MaxIterations.
        /// </remarks>
        property int MaxRefIteration;

        /// <summary>
        /// Whether BLA (Bilinear Approximation) acceleration was enabled.
        /// </summary>
        /// <value>true if BLA was used to skip iterations; false otherwise</value>
        /// <remarks>
        /// BLA provides 10-100x speedup by approximating multiple iterations at once.
        /// Only available for Mandelbrot and Power fractals in DOUBLE or FLOATEXP modes.
        /// Automatically disabled for unsupported fractal types.
        /// </remarks>
        property bool BLAEnabled;

        /// <summary>
        /// Time spent building the reference orbit (milliseconds).
        /// </summary>
        /// <value>Elapsed time for reference orbit calculation; 0 if perturbation not used</value>
        /// <remarks>
        /// Reference orbit is built once per zoom level, then reused for all pixels.
        /// Typically 10-30% of total render time for first render at a zoom level.
        /// Subsequent renders at same zoom (e.g., panning) reuse cached orbit (time = 0).
        /// </remarks>
        property double ReferenceOrbitBuildTime;
    };

    /// <summary>
    /// Progress event arguments for long-running fractal calculations.
    /// </summary>
    /// <remarks>
    /// Events are fired every 10 scan lines by default.
    /// For 600-line image = 60 progress events total.
    /// </remarks>
    public ref class ProgressEventArgs : public EventArgs
    {
    public:
        /// <summary>
        /// Completion percentage (0.0 to 100.0).
        /// </summary>
        /// <value>Double from 0.0 (just started) to 100.0 (complete)</value>
        property double Percentage;

        /// <summary>
        /// Current scan line being rendered (0-based).
        /// </summary>
        /// <value>Integer from 0 to TotalLines-1</value>
        property int CurrentLine;

        /// <summary>
        /// Total number of scan lines in the image.
        /// </summary>
        /// <value>Equals FractalParameters.Height</value>
        property int TotalLines;

        /// <summary>
        /// Human-readable status message for UI display.
        /// </summary>
        /// <value>String like "Calculating line 120 of 600"</value>
        /// <remarks>
        /// Optional: Can bind directly to UI StatusBar or ignore if using Percentage for ProgressBar.
        /// </remarks>
        property String^ StatusMessage;
    };

    /// <summary>
    /// C++/CLI wrapper for the native C++ fractal calculation engine.
    /// Provides managed interface for C# applications with minimal performance overhead.
    /// </summary>
    /// <remarks>
    /// <para><b>Thread Safety:</b> Not thread-safe. Create one instance per thread.</para>
    /// <para><b>Disposal:</b> Implements IDisposable pattern. Call Dispose() or use 'using' statement.</para>
    /// <para><b>Performance:</b> Wrapper overhead < 5% for typical rendering (see performance-benchmark-report.md).</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// using var engine = new FractalEngineWrapper();
    /// 
    /// // Subscribe to progress events
    /// engine.ProgressChanged += (sender, e) => 
    /// {
    ///     Console.WriteLine($"Progress: {e.Percentage:F1}%");
    /// };
    /// 
    /// // Set up parameters
    /// var parameters = new FractalParameters 
    /// {
    ///     FractalType = "Mandelbrot",
    ///     Width = 1920,
    ///     Height = 1080,
    ///     MaxIterations = 1000,
    ///     Palette = ColorPalette.Rainbow
    /// };
    /// 
    /// // Calculate fractal
    /// var result = engine.Calculate(parameters);
    /// 
    /// // Use pixel data (e.g., display in WinUI Image control)
    /// Console.WriteLine($"Rendered in {result.RenderTime.TotalMilliseconds}ms");
    /// Console.WriteLine($"Total iterations: {result.IterationCount:N0}");
    /// </code>
    /// </example>
    public ref class FractalEngineWrapper
    {
    private:
        // Pointer to native C++ fractal engine (will be implemented)
        void* m_nativeEngine;
        bool m_cancelled;

        // Event backing delegate
        EventHandler<ProgressEventArgs^>^ m_progressChangedDelegate;

        // Perturbation theory state
        void* m_refData;  // Pointer to StoreReferenceData struct
        bool m_referenceOrbitValid;
        int m_cachedArithType;

    public:
        /// <summary>
        /// Event fired periodically during calculation to report progress.
        /// </summary>
        /// <remarks>
        /// <para>Fired every 10 scan lines (configurable in future phases).</para>
        /// <para>Subscribe to update UI progress bars and status text.</para>
        /// <para>Event handler runs on calculation thread - marshal to UI thread for UI updates.</para>
        /// </remarks>
        event EventHandler<ProgressEventArgs^>^ ProgressChanged
        {
            void add(EventHandler<ProgressEventArgs^>^ handler)
            {
                m_progressChangedDelegate = static_cast<EventHandler<ProgressEventArgs^>^>(
                    System::Delegate::Combine(m_progressChangedDelegate, handler));
            }

            void remove(EventHandler<ProgressEventArgs^>^ handler)
            {
                m_progressChangedDelegate = static_cast<EventHandler<ProgressEventArgs^>^>(
                    System::Delegate::Remove(m_progressChangedDelegate, handler));
            }

            void raise(Object^ sender, ProgressEventArgs^ args)
            {
                EventHandler<ProgressEventArgs^>^ temp = m_progressChangedDelegate;
                if (temp != nullptr)
                {
                    temp(sender, args);
                }
            }
        }

        /// <summary>
        /// Create a new fractal engine wrapper instance.
        /// </summary>
        /// <remarks>
        /// Allocates native resources. Call Dispose() when done or use 'using' statement.
        /// </remarks>
        FractalEngineWrapper();

        /// <summary>
        /// Destructor - releases managed and native resources.
        /// </summary>
        ~FractalEngineWrapper();

        /// <summary>
        /// Finalizer - cleanup native resources if Dispose() not called.
        /// </summary>
        !FractalEngineWrapper();

        /// <summary>
        /// Calculate fractal image with specified parameters (synchronous).
        /// </summary>
        /// <param name="parameters">Fractal configuration including type, coordinates, and rendering options</param>
        /// <returns>FractalResult containing RGBA pixel data and statistics</returns>
        /// <exception cref="ArgumentNullException">Thrown if parameters is null</exception>
        /// <exception cref="ArgumentException">Thrown if parameters contains invalid values</exception>
        /// <exception cref="OperationCanceledException">Thrown if Cancel() is called during calculation</exception>
        /// <remarks>
        /// <para>This method blocks until calculation is complete. Use Task.Run() to run asynchronously:</para>
        /// <code>
        /// var result = await Task.Run(() => engine.Calculate(parameters));
        /// </code>
        /// <para>ProgressChanged events fire on the calling thread.</para>
        /// <para>Call Cancel() from another thread to abort calculation.</para>
        /// </remarks>
        /// <param name="parameters">Fractal calculation parameters</param>
        /// <returns>Result containing pixel data and metadata</returns>
        FractalResult^ Calculate(FractalParameters^ parameters);

        /// <summary>
        /// Cancel ongoing fractal calculation (thread-safe).
        /// </summary>
        /// <remarks>
        /// <para>Can be called from any thread while Calculate() is running.</para>
        /// <para>Calculate() will throw OperationCanceledException shortly after Cancel() is called.</para>
        /// <para>Typical cancellation response time: 1-50ms (checked every scan line).</para>
        /// </remarks>
        void Cancel();

        /// <summary>
        /// Get list of available fractal formula types supported by the engine.
        /// </summary>
        /// <returns>Array of fractal type names from the native FractalRegistry (44+ types)</returns>
        /// <remarks>
        /// <para>Queries the native FractalRegistry for all registered fractal types.</para>
        /// <para>Returns fractal type identifiers (e.g., "Mandelbrot", "BurningShip", "Phoenix").</para>
        /// <para>Use returned names for FractalParameters.FractalType property.</para>
        /// </remarks>
        array<String^>^ GetAvailableFractalTypes();

        /// <summary>
        /// Get list of all fractal category names for organizing fractals in the UI.
        /// </summary>
        /// <returns>Array of unique category names (e.g., "Classic Fractals", "Julia Sets", "Newton Fractals")</returns>
        /// <remarks>
        /// <para>Categories organize fractals into logical groups for UI presentation.</para>
        /// <para>Use with GetFractalTypesByCategory() to build hierarchical UI navigation.</para>
        /// <para>Examples: "Classic Fractals", "Julia Sets", "Burning Ship Variants", "Newton Fractals", "3D Attractors"</para>
        /// </remarks>
        array<String^>^ GetFractalCategories();

        /// <summary>
        /// Get list of fractal type names within a specific category.
        /// </summary>
        /// <param name="category">Category name (from GetFractalCategories())</param>
        /// <returns>Array of fractal type names in the specified category</returns>
        /// <remarks>
        /// <para>Returns all fractals that belong to the specified category.</para>
        /// <para>If category is not found or empty, returns empty array.</para>
        /// <para>Example: GetFractalTypesByCategory("Julia Sets") returns all Julia set variants.</para>
        /// </remarks>
        array<String^>^ GetFractalTypesByCategory(String^ category);

        /// <summary>
        /// Get total count of registered fractal types.
        /// </summary>
        /// <returns>Number of registered fractal types (44+)</returns>
        /// <remarks>
        /// <para>Useful for UI initialization and diagnostics.</para>
        /// <para>Count reflects the total after InitializeBuiltins() is called.</para>
        /// </remarks>
        int GetFractalTypeCount();

        /// <summary>
        /// Run pure native C++ benchmark without C++/CLI wrapper overhead (for performance analysis).
        /// </summary>
        /// <param name="width">Image width in pixels</param>
        /// <param name="height">Image height in pixels</param>
        /// <param name="maxIterations">Maximum iteration count</param>
        /// <param name="runs">Number of benchmark iterations to average</param>
        /// <returns>Average render time in milliseconds (pure native C++ performance)</returns>
        /// <remarks>
        /// <para>Used by performance benchmarking framework to measure wrapper overhead.</para>
        /// <para>Compare this result to Calculate() time to determine marshalling overhead percentage.</para>
        /// <para>Typical overhead: -0.5% to +2% for standard rendering scenarios.</para>
        /// </remarks>
        /// <returns>Average time in milliseconds</returns>
        double RunNativeBaselineBenchmark(int width, int height, int maxIterations, int runs);

        /// <summary>
        /// Proof-of-concept: Test ManpWIN64 engine integration by performing simple complex number arithmetic.
        /// </summary>
        /// <param name="real">Real part of complex number</param>
        /// <param name="imaginary">Imaginary part of complex number</param>
        /// <returns>Magnitude (absolute value) of the complex number</returns>
        /// <remarks>
        /// <para>POC for Phase 2: Validates that C++/CLI wrapper can link to and call ManpWIN64 native code.</para>
        /// <para>Uses ManpWIN64::Complex class to calculate sqrt(real² + imaginary²).</para>
        /// <para>Full fractal type integration will be completed in Phase 3 with UI testing framework.</para>
        /// </remarks>
        double TestManpWIN64Integration(double real, double imaginary);

        // ========== Deep Zoom / Perturbation Theory Methods ==========

        /// <summary>
        /// Build high-precision reference orbit for perturbation theory rendering.
        /// </summary>
        /// <param name="centerX">Complex plane center X coordinate (BigDouble as string)</param>
        /// <param name="centerY">Complex plane center Y coordinate (BigDouble as string)</param>
        /// <param name="viewWidth">View width in complex plane (BigDouble as string)</param>
        /// <param name="maxIteration">Maximum iteration count</param>
        /// <param name="bailout">Escape radius (typically 4.0 for Mandelbrot)</param>
        /// <param name="power">Polynomial power (2 for Mandelbrot, 3 for cubic, etc.)</param>
        /// <param name="subtype">Fractal variant (0=Mandelbrot, 1=Power, 2=BurningShip, etc.)</param>
        /// <param name="precision">MPFR precision in bits (e.g., 166 for ~50 decimal digits)</param>
        /// <param name="enableBLA">Enable BLA (Bilinear Approximation) acceleration</param>
        /// <param name="imageWidth">Image width in pixels (for BLA size calculation)</param>
        /// <param name="imageHeight">Image height in pixels (for BLA size calculation)</param>
        /// <returns>0 on success, negative on error or user cancellation</returns>
        /// <remarks>
        /// <para>This method builds the reference orbit once for a given zoom level/location.</para>
        /// <para>The orbit is cached internally and reused by CalculateWithPerturbation().</para>
        /// <para>Building the reference orbit is single-threaded and can take 10-30% of total render time.</para>
        /// <para>Progress is reported via custom status updates (not ProgressChanged event).</para>
        /// <para>Call this before CalculateWithPerturbation() when deep zoom is needed.</para>
        /// </remarks>
        int BuildReferenceOrbit(
            String^ centerX,
            String^ centerY,
            String^ viewWidth,
            int maxIteration,
            double bailout,
            int power,
            int subtype,
            int precision,
            bool enableBLA,
            int imageWidth,
            int imageHeight
        );

        /// <summary>
        /// Check if the cached reference orbit is still valid for these parameters.
        /// </summary>
        /// <param name="centerX">Complex plane center X coordinate (BigDouble as string)</param>
        /// <param name="centerY">Complex plane center Y coordinate (BigDouble as string)</param>
        /// <param name="viewWidth">View width in complex plane (BigDouble as string)</param>
        /// <param name="maxIteration">Maximum iteration count</param>
        /// <param name="bailout">Escape radius</param>
        /// <param name="power">Polynomial power</param>
        /// <returns>true if cached orbit is valid and can be reused; false if BuildReferenceOrbit() needed</returns>
        /// <remarks>
        /// <para>Checks if center coordinates, zoom, bailout, and power match cached values.</para>
        /// <para>Use this to avoid unnecessarily rebuilding the reference orbit.</para>
        /// <para>Example: panning at the same zoom level can reuse the orbit; changing zoom cannot.</para>
        /// </remarks>
        bool IsReferenceOrbitValid(
            String^ centerX,
            String^ centerY,
            String^ viewWidth,
            int maxIteration,
            double bailout,
            int power
        );

        /// <summary>
        /// Calculate fractal using perturbation theory with existing reference orbit.
        /// </summary>
        /// <param name="parameters">Fractal parameters (must have BigDouble coordinates)</param>
        /// <returns>FractalResult with UsedPerturbation = true and perturbation-specific stats</returns>
        /// <exception cref="InvalidOperationException">Thrown if BuildReferenceOrbit() not called first</exception>
        /// <exception cref="ArgumentNullException">Thrown if parameters is null</exception>
        /// <exception cref="OperationCanceledException">Thrown if Cancel() is called during calculation</exception>
        /// <remarks>
        /// <para>This method uses the reference orbit built by BuildReferenceOrbit().</para>
        /// <para>Multi-threaded: Each thread calculates pixels using reference orbit + delta.</para>
        /// <para>10-100x faster than brute-force BigDouble iteration at deep zoom levels.</para>
        /// <para>Automatically selects DOUBLE or FLOATEXP arithmetic based on precision.</para>
        /// <para>ProgressChanged events fire normally during pixel calculation.</para>
        /// </remarks>
        FractalResult^ CalculateWithPerturbation(FractalParameters^ parameters);

    protected:
        /// <summary>
        /// Report progress to managed event handlers
        /// </summary>
        void OnProgressChanged(ProgressEventArgs^ args)
        {
            ProgressChanged(this, args);
        }
    };

} // namespace Native
} // namespace ManpCore
