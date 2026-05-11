#include "FractalEngineWrapper.h"

// Prevent IServiceProvider namespace collision between COM and .NET
// Must be defined before any Windows headers are included
#define _OLE2_H_  
#define __SERVPROV_H__

#include "MandelbrotCalculator.h"
#include "FractalRegistry.h"
#include "NativePerformanceBaseline.h"
#include "BigDoubleMarshaller.h"
#include "Complex.h"  // ManpWIN64 Complex class for POC
#include "../ManpWIN64/BigDouble.h"
#include "../ManpWIN64/BigComplex.h"
#include "../ManpWIN64/PertEngine.h"  // Perturbation theory engine
#include <string>

using namespace System;
using namespace System::Diagnostics;
using namespace System::Runtime::InteropServices;
using namespace ManpCore::Native;

// External global for MPFR precision (defined in ManpWIN64/BigDouble.cpp)
extern int decimals;

// External perturbation theory functions and globals (defined in ManpWIN64/PertSetup.cpp)
extern int ReferenceZoomPoint(BigComplex& centre, int maxIteration, int user_data(HWND hwnd), char* StatusBarInfo, int *pPertProgress, double bailout, int ArithType, int power, ::BigDouble BigWidth, int &SlopeDegree);
extern void PertSetupArithType(int &ArithType, int subtype, long MaxIteration, int precision, BYTE BigNumFlag);
extern bool CheckValidRef(BigComplex ReferenceCoordinate, ::BigDouble BigWidth, int maxIteration, double bailout, StoreReferenceData &RefData, int power, int ArithType);
extern std::vector<ExpComplex> ExpXSubN;
extern std::vector<Complex> XSubN;
extern int ArithType;
extern int MaxRefIteration;
extern BLAS Bla;
extern int SlopeDegree;
extern bool EnableApproximation;

// Helper function to convert managed string to std::string without msclr/marshal
static std::string ManagedToStdString(String^ str)
{
    if (String::IsNullOrEmpty(str))
        return std::string();

    array<unsigned char>^ bytes = System::Text::Encoding::UTF8->GetBytes(str);
    pin_ptr<unsigned char> pinnedBytes = &bytes[0];
    return std::string(reinterpret_cast<char*>(pinnedBytes), bytes->Length);
}

// Helper function to convert std::string to managed String
static String^ StdStringToManaged(const std::string& str)
{
    if (str.empty())
        return String::Empty;

    array<unsigned char>^ bytes = gcnew array<unsigned char>((int)str.size());
    Marshal::Copy(IntPtr((void*)str.data()), bytes, 0, (int)str.size());
    return System::Text::Encoding::UTF8->GetString(bytes);
}

//=============================================================================
// BigDouble Implementation (Managed wrapper for MPFR)
//=============================================================================

// Constructor from double
ManpCore::Native::BigDouble::BigDouble(double value)
{
    m_nativeBigDouble = new ::Native::MPFRBigDouble(value);
    m_precision = 16;
}

// Constructor with precision
ManpCore::Native::BigDouble::BigDouble(double value, int precision)
{
    m_nativeBigDouble = new ::Native::MPFRBigDouble(value, precision);
    m_precision = precision;
}

// Copy constructor
ManpCore::Native::BigDouble::BigDouble(ManpCore::Native::BigDouble^ other)
{
    if (other == nullptr)
        throw gcnew ArgumentNullException("other");

    auto nativeOther = static_cast<::Native::MPFRBigDouble*>(other->m_nativeBigDouble);
    m_nativeBigDouble = new ::Native::MPFRBigDouble(*nativeOther);
    m_precision = other->m_precision;
}

// Destructor
ManpCore::Native::BigDouble::~BigDouble()
{
    this->!BigDouble();
}

// Finalizer
ManpCore::Native::BigDouble::!BigDouble()
{
    if (m_nativeBigDouble != nullptr)
    {
        delete static_cast<::Native::MPFRBigDouble*>(m_nativeBigDouble);
        m_nativeBigDouble = nullptr;
    }
}

// Convert to double
double ManpCore::Native::BigDouble::ToDouble()
{
    auto native = static_cast<::Native::MPFRBigDouble*>(m_nativeBigDouble);
    return native->ToDouble();
}

// Convert to string
String^ ManpCore::Native::BigDouble::ToString()
{
    auto native = static_cast<::Native::MPFRBigDouble*>(m_nativeBigDouble);
    std::string str = native->ToString();
    return gcnew String(str.c_str());
}

// Parse from string
ManpCore::Native::BigDouble^ ManpCore::Native::BigDouble::Parse(String^ str)
{
    if (String::IsNullOrEmpty(str))
        throw gcnew ArgumentNullException("str");

    std::string nativeStr = ManagedToStdString(str);

    auto native = ::Native::MPFRBigDouble::FromString(nativeStr);
    return gcnew ManpCore::Native::BigDouble(native.ToDouble(), 50);
}

// Arithmetic operators
ManpCore::Native::BigDouble^ ManpCore::Native::BigDouble::operator+(ManpCore::Native::BigDouble^ a, ManpCore::Native::BigDouble^ b)
{
    if (a == nullptr || b == nullptr)
        throw gcnew ArgumentNullException();

    auto nativeA = static_cast<::Native::MPFRBigDouble*>(a->m_nativeBigDouble);
    auto nativeB = static_cast<::Native::MPFRBigDouble*>(b->m_nativeBigDouble);

    auto result = (*nativeA) + (*nativeB);
    return gcnew ManpCore::Native::BigDouble(result.ToDouble(), Math::Max(a->m_precision, b->m_precision));
}

ManpCore::Native::BigDouble^ ManpCore::Native::BigDouble::operator-(ManpCore::Native::BigDouble^ a, ManpCore::Native::BigDouble^ b)
{
    if (a == nullptr || b == nullptr)
        throw gcnew ArgumentNullException();

    auto nativeA = static_cast<::Native::MPFRBigDouble*>(a->m_nativeBigDouble);
    auto nativeB = static_cast<::Native::MPFRBigDouble*>(b->m_nativeBigDouble);

    auto result = (*nativeA) - (*nativeB);
    return gcnew ManpCore::Native::BigDouble(result.ToDouble(), Math::Max(a->m_precision, b->m_precision));
}

ManpCore::Native::BigDouble^ ManpCore::Native::BigDouble::operator*(ManpCore::Native::BigDouble^ a, ManpCore::Native::BigDouble^ b)
{
    if (a == nullptr || b == nullptr)
        throw gcnew ArgumentNullException();

    auto nativeA = static_cast<::Native::MPFRBigDouble*>(a->m_nativeBigDouble);
    auto nativeB = static_cast<::Native::MPFRBigDouble*>(b->m_nativeBigDouble);

    auto result = (*nativeA) * (*nativeB);
    return gcnew ManpCore::Native::BigDouble(result.ToDouble(), Math::Max(a->m_precision, b->m_precision));
}

ManpCore::Native::BigDouble^ ManpCore::Native::BigDouble::operator/(ManpCore::Native::BigDouble^ a, ManpCore::Native::BigDouble^ b)
{
    if (a == nullptr || b == nullptr)
        throw gcnew ArgumentNullException();

    auto nativeA = static_cast<::Native::MPFRBigDouble*>(a->m_nativeBigDouble);
    auto nativeB = static_cast<::Native::MPFRBigDouble*>(b->m_nativeBigDouble);

    auto result = (*nativeA) / (*nativeB);
    return gcnew ManpCore::Native::BigDouble(result.ToDouble(), Math::Max(a->m_precision, b->m_precision));
}

bool ManpCore::Native::BigDouble::operator<(ManpCore::Native::BigDouble^ a, ManpCore::Native::BigDouble^ b)
{
    if (a == nullptr || b == nullptr)
        throw gcnew ArgumentNullException();

    auto nativeA = static_cast<::Native::MPFRBigDouble*>(a->m_nativeBigDouble);
    auto nativeB = static_cast<::Native::MPFRBigDouble*>(b->m_nativeBigDouble);

    return (*nativeA) < (*nativeB);
}

bool ManpCore::Native::BigDouble::operator>(ManpCore::Native::BigDouble^ a, ManpCore::Native::BigDouble^ b)
{
    if (a == nullptr || b == nullptr)
        throw gcnew ArgumentNullException();

    auto nativeA = static_cast<::Native::MPFRBigDouble*>(a->m_nativeBigDouble);
    auto nativeB = static_cast<::Native::MPFRBigDouble*>(b->m_nativeBigDouble);

    return (*nativeA) > (*nativeB);
}

//=============================================================================
// FractalEngineWrapper Implementation
//=============================================================================

// Constructor
FractalEngineWrapper::FractalEngineWrapper()
{
    m_nativeEngine = nullptr;
    m_cancelled = false;
    m_progressChangedDelegate = nullptr;

    // Initialize perturbation state
    m_refData = new StoreReferenceData();
    m_referenceOrbitValid = false;
    m_cachedArithType = DOUBLE;

    // TODO Phase 2: Initialize native C++ fractal engine
    // m_nativeEngine = CreateNativeFractalEngine();
}

// Destructor (managed)
FractalEngineWrapper::~FractalEngineWrapper()
{
    this->!FractalEngineWrapper();
}

// Finalizer (unmanaged cleanup)
FractalEngineWrapper::!FractalEngineWrapper()
{
    if (m_nativeEngine != nullptr)
    {
        // TODO Phase 2: Destroy native C++ engine
        // DestroyNativeFractalEngine(m_nativeEngine);
        m_nativeEngine = nullptr;
    }

    // Clean up perturbation state
    if (m_refData != nullptr)
    {
        delete static_cast<StoreReferenceData*>(m_refData);
        m_refData = nullptr;
    }
}

//=============================================================================
// Histogram-Based Rendering (Phase 2)
// For strange attractors and other orbit accumulation fractals
//=============================================================================

/// <summary>
/// Render a histogram-based fractal using orbit accumulation.
/// Iterates a dynamical system millions of times and accumulates visit counts per pixel.
/// </summary>
/// <param name="result">Output buffer to write pixel data</param>
/// <param name="spec">Fractal specification with orbitIterator</param>
/// <param name="params">Render parameters (center, zoom, etc.)</param>
/// <param name="width">Image width in pixels</param>
/// <param name="height">Image height in pixels</param>
/// <param name="palette">Color palette for density mapping</param>
/// <param name="colorOffset">Color rotation offset</param>
static void RenderHistogramFractal(
    FractalResult^ result,
    const ::Native::FractalSpec* spec,
    const ::Native::MandelbrotParams& params,
    int width,
    int height,
    ::Native::PaletteType palette,
    int colorOffset)
{
    Debug::WriteLine("RenderHistogramFractal: Starting histogram-based rendering");
    Debug::WriteLine(String::Format("  Fractal: {0}", gcnew String(spec->name.c_str())));
    Debug::WriteLine(String::Format("  Canvas: {0}x{1}", width, height));

    // Validate orbitIterator exists
    if (!spec->orbitIterator)
    {
        throw gcnew InvalidOperationException(
            String::Format("Fractal '{0}' is marked as HistogramBased but has no orbitIterator defined. "
                          "Please add spec.orbitIterator in the fractal registration code.",
                          gcnew String(spec->name.c_str())));
    }

    Debug::WriteLine("  OrbitIterator validated");

    // Step 1: Allocate histogram buffer (visit counter per pixel)
    std::vector<int> histogram(width * height, 0);
    Debug::WriteLine(String::Format("  Allocated histogram buffer: {0} pixels", width * height));

    // Step 2: Initialize starting point
    double x = 0.1;
    double y = 0.1;
    double z = 1.0;

    // Step 3: Iterate system and accumulate histogram
    const int orbitCount = 5000000;  // 5 million iterations
    int skipIterations = 100;  // Skip transient behavior

    Debug::WriteLine(String::Format("  Orbit count: {0:N0}", orbitCount));
    Debug::WriteLine(String::Format("  Skip transient: {0}", skipIterations));

    // Track bounds for coordinate mapping
    double minX = 1e10, maxX = -1e10;
    double minY = 1e10, maxY = -1e10;

    // Empty parameter map for now
    ::Native::ParamMap customParams;

    // Phase 1: Iterate to find actual attractor bounds
    for (int iter = 0; iter < orbitCount; ++iter)
    {
        // Call the fractal's orbit iterator to update (x, y, z)
        spec->orbitIterator(x, y, z, customParams);

        // Skip initial transient iterations
        if (iter < skipIterations)
            continue;

        // Track bounds
        if (x < minX) minX = x;
        if (x > maxX) maxX = x;
        if (y < minY) minY = y;
        if (y > maxY) maxY = y;
    }

    Debug::WriteLine(String::Format("  Orbit bounds: X=[{0:F2}, {1:F2}], Y=[{2:F2}, {3:F2}]", minX, maxX, minY, maxY));

    // Phase 2: Reset and accumulate histogram using discovered bounds
    x = 0.1;
    y = 0.1;
    z = 1.0;

    // Use attractor's natural bounds for viewport (ignore user zoom/pan for now)
    double rangeX = maxX - minX;
    double rangeY = maxY - minY;
    double margin = 0.1;  // 10% margin
    double viewLeft = minX - rangeX * margin;
    double viewRight = maxX + rangeX * margin;
    double viewBottom = minY - rangeY * margin;
    double viewTop = maxY + rangeY * margin;
    double viewWidth = viewRight - viewLeft;
    double viewHeight = viewTop - viewBottom;

    Debug::WriteLine(String::Format("  Viewport: X=[{0:F2}, {1:F2}], Y=[{2:F2}, {3:F2}]", viewLeft, viewRight, viewBottom, viewTop));

    for (int iter = 0; iter < orbitCount; ++iter)
    {
        // Call the fractal's orbit iterator to update (x, y, z)
        spec->orbitIterator(x, y, z, customParams);

        // Skip initial transient iterations
        if (iter < skipIterations)
            continue;

        // Map attractor coordinates to pixel coordinates using auto-fit viewport
        double worldX = x;
        double worldY = y;

        // Map to pixel coordinates
        int px = (int)((worldX - viewLeft) / viewWidth * width);
        int py = (int)((viewTop - worldY) / viewHeight * height);

        // Bounds check
        if (px >= 0 && px < width && py >= 0 && py < height)
        {
            histogram[py * width + px]++;
        }

        // Progress reporting every 500k iterations
        if (iter % 500000 == 0 && iter > 0)
        {
            Debug::WriteLine(String::Format("  Progress: {0:N0} / {1:N0} iterations ({2:F1}%)", 
                iter, orbitCount, (iter * 100.0 / orbitCount)));
        }
    }

    // Step 4: Find max histogram value for normalization
    int maxVisits = 0;
    for (int i = 0; i < width * height; ++i)
    {
        if (histogram[i] > maxVisits)
            maxVisits = histogram[i];
    }

    Debug::WriteLine(String::Format("  Max visits per pixel: {0}", maxVisits));

    // Step 5: Convert histogram to colors
    Debug::WriteLine("  Converting histogram to colors...");

    for (int y = 0; y < height; ++y)
    {
        for (int x = 0; x < width; ++x)
        {
            int index = y * width + x;
            int visits = histogram[index];

            // Normalize to [0, 1] with logarithmic scaling for better visibility
            double density = 0.0;
            if (visits > 0 && maxVisits > 0)
            {
                // Log scaling: density = log(visits + 1) / log(maxVisits + 1)
                density = std::log(visits + 1.0) / std::log(maxVisits + 1.0);
            }

            // Map density to iteration count for color palette
            // Scale to maxIterations range for palette compatibility
            double iterationValue = density * params.maxIterations;

            // Convert to color using existing palette system
            ::Native::ColorRGB color = ::Native::MandelbrotCalculator::IterationToColor(
                iterationValue,
                params.maxIterations,
                palette,
                colorOffset
            );

            // Write BGRA pixel
            int pixelIndex = index * 4;
            result->PixelData[pixelIndex + 0] = color.b;  // Blue
            result->PixelData[pixelIndex + 1] = color.g;  // Green
            result->PixelData[pixelIndex + 2] = color.r;  // Red
            result->PixelData[pixelIndex + 3] = 255;      // Alpha
        }
    }

    Debug::WriteLine("RenderHistogramFractal: Rendering complete");
}

// Calculate fractal
FractalResult^ FractalEngineWrapper::Calculate(FractalParameters^ parameters)
{
    if (parameters == nullptr)
        throw gcnew ArgumentNullException("parameters");

    // Validate parameters
    if (parameters->Width <= 0 || parameters->Width > 8192)
        throw gcnew ArgumentOutOfRangeException("Width", parameters->Width, "Width must be between 1 and 8192");
    if (parameters->Height <= 0 || parameters->Height > 8192)
        throw gcnew ArgumentOutOfRangeException("Height", parameters->Height, "Height must be between 1 and 8192");
    if (parameters->MaxIterations <= 0 || parameters->MaxIterations > 100000)
        throw gcnew ArgumentOutOfRangeException("MaxIterations", parameters->MaxIterations, "MaxIterations must be between 1 and 100000");

    Debug::WriteLine(String::Format("Native Calculate: Parameters validated - {0}x{1}, {2} iterations", 
        parameters->Width, parameters->Height, parameters->MaxIterations));

    m_cancelled = false;
    auto stopwatch = Stopwatch::StartNew();

    try
    {
        int width = parameters->Width;
        int height = parameters->Height;
        int pixelCount = width * height * 4; // BGRA (Blue, Green, Red, Alpha)

        Debug::WriteLine(String::Format("Native Calculate: Creating result for {0}x{1} ({2} bytes)", width, height, pixelCount));

        auto result = gcnew FractalResult();
        result->Width = width;
        result->Height = height;

        Debug::WriteLine("Native Calculate: Allocating pixel data array...");
        try
        {
            result->PixelData = gcnew array<Byte>(pixelCount);
            Debug::WriteLine(String::Format("Native Calculate: Pixel array allocated successfully ({0} bytes)", pixelCount));
        }
        catch (Exception^ ex)
        {
            Debug::WriteLine(String::Format("ERROR: Failed to allocate pixel array: {0}", ex->Message));
            throw gcnew ArgumentException(String::Format("Failed to allocate pixel buffer for {0}x{1} image ({2} bytes): {3}", 
                width, height, pixelCount, ex->Message));
        }

        Debug::WriteLine("Native Calculate: Setting up parameters...");

        // Check if deep zoom parameters are provided
        // ╔═══════════════════════════════════════════════════════════════════════════╗
        // ║ TODO: TEMPORARY DEEP ZOOM IMPLEMENTATION - REQUIRES REPLACEMENT           ║
        // ║                                                                            ║
        // ║ This code uses simple BigDouble coordinate conversion (25 decimal places) ║
        // ║ which works but has severe performance limitations beyond 10^20 zoom.     ║
        // ║                                                                            ║
        // ║ REPLACEMENT PLAN (Phase 3.5 - 12-17 days):                                ║
        // ║   1. Integrate perturbation theory from ManpWIN64/Perturbation.cpp        ║
        // ║   2. Implement reference orbit calculation (ReferenceZoomPoint)            ║
        // ║   3. Add perturbation pixel calculation (delta-based optimization)         ║
        // ║   4. Integrate BLA (Bilinear Approximation) for 50-90% iteration skip     ║
        // ║   5. Add reference orbit caching for pan operations                        ║
        // ║                                                                            ║
        // ║ See: ManpWinUI/docs/DEEP_ZOOM_INTEGRATION_PLAN.md for detailed roadmap    ║
        // ║                                                                            ║
        // ║ Expected outcome: 10-100x faster at extreme zooms, support up to 10^100+  ║
        // ╚═══════════════════════════════════════════════════════════════════════════╝
        bool useDeepZoom = (parameters->BigCenterX != nullptr && 
                            parameters->BigCenterY != nullptr &&
                            parameters->BigViewWidth != nullptr &&
                            parameters->BigViewHeight != nullptr);

        if (useDeepZoom)
        {
            Debug::WriteLine("Native Calculate: Deep Zoom Mode - Using MPFR BigDouble precision");

            // Set MPFR precision based on the BigDouble precision
            int requiredPrecision = parameters->Precision;
            decimals = requiredPrecision;
            Debug::WriteLine(String::Format("Native Calculate: Set MPFR decimals to {0}", decimals));

            // ╔═══════════════════════════════════════════════════════════════════╗
            // ║ TEMPORARY LIMITATION:                                              ║
            // ║ The following code converts BigDouble → double, losing precision   ║
            // ║ benefits. This works for moderate zooms but fails at extreme zoom. ║
            // ║                                                                     ║
            // ║ PROPER IMPLEMENTATION (to be added in Phase 3.5):                  ║
            // ║   1. Build high-precision reference orbit at center point          ║
            // ║      (using BigDouble/MPFR, expensive but done once)               ║
            // ║   2. For each pixel:                                               ║
            // ║      a. Calculate delta from reference center (double precision OK)║
            // ║      b. Use perturbation formula: ΔZ_n ≈ 2·Z_n·ΔZ_(n-1) + ΔC      ║
            // ║      c. Z_n comes from cached reference orbit (fast lookup)        ║
            // ║   3. Result: 10-100x faster, supports 10^100+ zoom                 ║
            // ╚═══════════════════════════════════════════════════════════════════╝

            // TODO: Implement deep zoom rendering using BigDouble arithmetic
            // For now, fall back to double precision with extracted values
            Debug::WriteLine("WARNING: Deep zoom BigDouble rendering path not yet implemented - falling back to double");
        }

        // Setup native Mandelbrot parameters
        ::Native::MandelbrotParams nativeParams;
        nativeParams.width = width;
        nativeParams.height = height;
        nativeParams.maxIterations = parameters->MaxIterations;
        nativeParams.centerX = useDeepZoom ? 
            parameters->BigCenterX->ToDouble() :
            parameters->CenterX;
        nativeParams.centerY = useDeepZoom ?
            parameters->BigCenterY->ToDouble() :
            parameters->CenterY;
        nativeParams.viewWidth = useDeepZoom ?
            parameters->BigViewWidth->ToDouble() :
            parameters->ViewWidth;
        nativeParams.viewHeight = useDeepZoom ?
            parameters->BigViewHeight->ToDouble() :
            parameters->ViewHeight;
        nativeParams.isJulia = parameters->IsJuliaSet;
        nativeParams.juliaCX = parameters->JuliaCX;
        nativeParams.juliaCY = parameters->JuliaCY;

        Debug::WriteLine(String::Format("Native Calculate: Parameters set - {0}x{1}, maxIter={2}", width, height, parameters->MaxIterations));

        // Convert managed palette enum to native palette enum
        ::Native::PaletteType nativePalette = static_cast<::Native::PaletteType>((int)parameters->Palette);
        Debug::WriteLine(String::Format("Native Calculate: Palette={0}", (int)nativePalette));

        // Extract color offset for palette rotation
        int colorOffset = parameters->ColorOffset;
        Debug::WriteLine(String::Format("Native Calculate: ColorOffset={0}", colorOffset));

        // Convert fractal type string and get calculator from registry
        std::string fractalType = ManagedToStdString(parameters->FractalType);
        Debug::WriteLine(String::Format("Native Calculate: Fractal type: {0}", gcnew String(fractalType.c_str())));

        // Initialize registry if not already done
        static bool registryInitialized = false;
        if (!registryInitialized)
        {
            Debug::WriteLine("Native Calculate: Initializing fractal registry...");
            ::Native::FractalRegistry::InitializeBuiltins();
            registryInitialized = true;
            Debug::WriteLine("Native Calculate: Registry initialized");
        }

        Debug::WriteLine("Native Calculate: Verifying fractal registration...");
        Debug::WriteLine(String::Format("Native Calculate: Looking up fractal: '{0}'", gcnew String(fractalType.c_str())));

        // Check if fractal is registered
        bool isRegistered = ::Native::FractalRegistry::IsRegistered(fractalType);
        Debug::WriteLine(String::Format("Native Calculate: Is '{0}' registered? {1}", gcnew String(fractalType.c_str()), isRegistered));

        // Fallback to Mandelbrot if type not found
        if (!isRegistered)
        {
            Debug::WriteLine(String::Format("Native Calculate: Fractal '{0}' not found, falling back to Mandelbrot", gcnew String(fractalType.c_str())));
            fractalType = "Mandelbrot";
        }

        Debug::WriteLine(String::Format("Native Calculate: Using fractal: '{0}'", gcnew String(fractalType.c_str())));

        // Get fractal specification and check rendering category
        const ::Native::FractalSpec* spec = ::Native::FractalRegistry::GetSpec(fractalType);
        if (spec == nullptr)
        {
            throw gcnew InvalidOperationException(String::Format("Fractal specification not found: {0}", gcnew String(fractalType.c_str())));
        }

        Debug::WriteLine(String::Format("Native Calculate: Fractal category: {0}", (int)spec->type));

        // Check if this fractal requires histogram-based rendering
        if (spec->type == ::Native::FractalCategory::HistogramBased)
        {
            Debug::WriteLine("Native Calculate: Histogram-based fractal detected");
            Debug::WriteLine("  Calling RenderHistogramFractal for orbit accumulation rendering");

            // Convert managed palette enum to native
            ::Native::PaletteType nativePalette = static_cast<::Native::PaletteType>((int)parameters->Palette);
            int colorOffset = parameters->ColorOffset;

            // Call histogram renderer
            RenderHistogramFractal(result, spec, nativeParams, width, height, nativePalette, colorOffset);

            // Set statistics and timing
            stopwatch->Stop();
            result->RenderTime = stopwatch->Elapsed;
            result->IterationCount = 0;  // Not applicable for histogram rendering
            result->EscapedPixelCount = 0;

            Debug::WriteLine(String::Format("Native Calculate: Histogram rendering complete in {0}ms", stopwatch->ElapsedMilliseconds));
            return result;
        }

        // Prepare parameter map for extensibility (currently empty, but ready for custom params)
        ::Native::ParamMap customParams;

        long long totalIterations = 0;
        int escapedPixels = 0;

        Debug::WriteLine("Native Calculate: Using per-pixel escape-time rendering");
        Debug::WriteLine("Native Calculate: Starting pixel loop...");

        // Calculate fractal using native C++ code
        for (int y = 0; y < height; y++)
        {
            // Report progress every 10 lines
            if (y % 10 == 0)
            {
                // Only raise progress event if there are subscribers
                if (m_progressChangedDelegate != nullptr)
                {
                    auto progressArgs = gcnew ProgressEventArgs();
                    progressArgs->Percentage = (y * 100.0) / height;
                    progressArgs->CurrentLine = y;
                    progressArgs->TotalLines = height;
                    progressArgs->StatusMessage = String::Format("Calculating line {0} of {1}", y, height);
                    ProgressChanged(this, progressArgs);  // Raise the event
                }
                Debug::WriteLine(String::Format("Native Calculate: Line {0} of {1}", y, height));
            }

            if (m_cancelled)
                throw gcnew OperationCanceledException("Calculation cancelled by user");

            for (int x = 0; x < width; x++)
            {
                // Map pixel to complex plane
                ::Native::ComplexD c = ::Native::MandelbrotCalculator::PixelToComplex(x, y, nativeParams);

                // DIAGNOSTIC: Log first pixel only
                if (x == 0 && y == 0)
                {
                    Debug::WriteLine(String::Format("First pixel: c=({0}, {1})", c.real, c.imag));
                    Debug::WriteLine(String::Format("About to call FractalRegistry::Calculate with fractalType='{0}'", gcnew String(fractalType.c_str())));
                }

                // Calculate using registry - entirely in native code, no std::function boundary crossing
                double iteration;
                try
                {
                    iteration = ::Native::FractalRegistry::Calculate(
                        fractalType,
                        c, 
                        nativeParams.maxIterations,
                        nativeParams.isJulia,
                        ::Native::ComplexD(nativeParams.juliaCX, nativeParams.juliaCY),
                        customParams
                    );

                    // DIAGNOSTIC: Confirm first pixel calculated
                    if (x == 0 && y == 0)
                    {
                        Debug::WriteLine(String::Format("First pixel calculated: iteration={0}", iteration));
                    }
                }
                catch (const std::exception& ex)
                {
                    Debug::WriteLine(String::Format("ERROR in Calculate at pixel ({0},{1}): {2}", x, y, gcnew String(ex.what())));
                    throw;
                }
                catch (...)
                {
                    Debug::WriteLine(String::Format("ERROR in Calculate at pixel ({0},{1}): Unknown exception", x, y));
                    throw;
                }

                totalIterations += (long long)iteration;

                // Track if pixel escaped (for diagnostics)
                if (iteration < nativeParams.maxIterations)
                {
                    escapedPixels++;
                }

                // DIAGNOSTIC: Log before color conversion (first pixel only)
                if (x == 0 && y == 0)
                {
                    Debug::WriteLine(String::Format("About to convert to color: iteration={0}, maxIter={1}, palette={2}", iteration, nativeParams.maxIterations, (int)nativePalette));
                }

                // Convert iteration to color using selected palette with color offset
                ::Native::ColorRGB color = ::Native::MandelbrotCalculator::IterationToColor(
                    iteration, 
                    nativeParams.maxIterations, 
                    nativePalette,
                    colorOffset
                );

                // DIAGNOSTIC: Log after color conversion (first pixel only)
                if (x == 0 && y == 0)
                {
                    Debug::WriteLine(String::Format("Color converted: R={0}, G={1}, B={2}", color.r, color.g, color.b));
                    int testIndex = (y * width + x) * 4;
                    Debug::WriteLine(String::Format("About to write to pixel array at index {0} (array length={1})", testIndex, result->PixelData->Length));
                }

                // Write BGRA pixel (WinUI WriteableBitmap format)
                int index = (y * width + x) * 4;
                result->PixelData[index + 0] = color.b;  // Blue
                result->PixelData[index + 1] = color.g;  // Green
                result->PixelData[index + 2] = color.r;  // Red
                result->PixelData[index + 3] = 255;      // Alpha (full opacity)

                // DIAGNOSTIC: Log after write (first pixel only)
                if (x == 0 && y == 0)
                {
                    Debug::WriteLine("First pixel written successfully");
                }
            }
        }

        stopwatch->Stop();
        result->RenderTime = stopwatch->Elapsed;
        result->IterationCount = totalIterations;
        result->EscapedPixelCount = escapedPixels;

        // Final progress update (only if there are subscribers)
        if (m_progressChangedDelegate != nullptr)
        {
            auto finalProgress = gcnew ProgressEventArgs();
            finalProgress->Percentage = 100.0;
            finalProgress->CurrentLine = height;
            finalProgress->TotalLines = height;
            finalProgress->StatusMessage = "Complete";
            ProgressChanged(this, finalProgress);
        }

        return result;
    }
    catch (Exception^)
    {
        stopwatch->Stop();
        throw;
    }
}

// Cancel calculation
void FractalEngineWrapper::Cancel()
{
    m_cancelled = true;
}

// Get available fractal types
array<String^>^ FractalEngineWrapper::GetAvailableFractalTypes()
{
    // Query native FractalRegistry for all registered types
    ::Native::FractalRegistry::InitializeBuiltins();  // Ensure registry is initialized
    std::vector<std::string> nativeTypes = ::Native::FractalRegistry::GetRegisteredNames();

    // Convert to managed array
    array<String^>^ types = gcnew array<String^>(static_cast<int>(nativeTypes.size()));
    for (size_t i = 0; i < nativeTypes.size(); i++)
    {
        types[static_cast<int>(i)] = StdStringToManaged(nativeTypes[i]);
    }

    return types;
}

// Get fractal categories
array<String^>^ FractalEngineWrapper::GetFractalCategories()
{
    // Query native FractalRegistry for all unique categories
    ::Native::FractalRegistry::InitializeBuiltins();  // Ensure registry is initialized
    std::vector<std::string> nativeCategories = ::Native::FractalRegistry::GetCategories();

    // Convert to managed array
    array<String^>^ categories = gcnew array<String^>(static_cast<int>(nativeCategories.size()));
    for (size_t i = 0; i < nativeCategories.size(); i++)
    {
        categories[static_cast<int>(i)] = StdStringToManaged(nativeCategories[i]);
    }

    return categories;
}

// Get fractal types by category
array<String^>^ FractalEngineWrapper::GetFractalTypesByCategory(String^ category)
{
    // Query native FractalRegistry for fractals in the specified category
    ::Native::FractalRegistry::InitializeBuiltins();  // Ensure registry is initialized
    std::string nativeCategory = ManagedToStdString(category);
    std::vector<std::string> nativeTypes = ::Native::FractalRegistry::GetFractalsByCategory(nativeCategory);

    // Convert to managed array
    array<String^>^ types = gcnew array<String^>(static_cast<int>(nativeTypes.size()));
    for (size_t i = 0; i < nativeTypes.size(); i++)
    {
        types[static_cast<int>(i)] = StdStringToManaged(nativeTypes[i]);
    }

    return types;
}

// Get total fractal type count
int FractalEngineWrapper::GetFractalTypeCount()
{
    ::Native::FractalRegistry::InitializeBuiltins();  // Ensure registry is initialized
    return static_cast<int>(::Native::FractalRegistry::GetCount());
}

// Run native baseline benchmark
double FractalEngineWrapper::RunNativeBaselineBenchmark(int width, int height, int maxIterations, int runs)
{
    auto result = ::Native::NativePerformanceBaseline::RunMandelbrotBenchmark(
        width, height, maxIterations, runs);

    return result.averageTimeMs;
}

// Test ManpWIN64 integration (POC)
// Temporarily disabled due to Complex type visibility issues in C++/CLI mixed mode
double FractalEngineWrapper::TestManpWIN64Integration(double real, double imaginary)
{
    // TODO: Fix Complex type visibility in managed/unmanaged boundary
    // For now, return a simple calculation to keep the interface working
    return sqrt(real * real + imaginary * imaginary);

    /* Original implementation - requires fixing Complex visibility:
    ::Complex c(real, imaginary);
    return c.CFabs();
    */
}

//=============================================================================
// Perturbation Theory Implementation
//=============================================================================

// Dummy user_data callback for reference orbit building (no GUI interaction in this context)
static int DummyUserData(HWND hwnd)
{
    return 0; // Continue processing
}

int FractalEngineWrapper::BuildReferenceOrbit(
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
    int imageHeight)
{
    if (centerX == nullptr || centerY == nullptr || viewWidth == nullptr)
        throw gcnew ArgumentNullException("Center coordinates and view width are required");

    Debug::WriteLine(String::Format("BuildReferenceOrbit: Starting with precision={0}, maxIter={1}, image={2}x{3}", 
        precision, maxIteration, imageWidth, imageHeight));

    try
    {
        // Set image dimensions for BLA size calculation
        extern int xdots, ydots;
        xdots = imageWidth;
        ydots = imageHeight;
        Debug::WriteLine(String::Format("BuildReferenceOrbit: Set xdots={0}, ydots={1}", xdots, ydots));

        // Convert managed strings to BigDouble
        std::string centerXStr = ManagedToStdString(centerX);
        std::string centerYStr = ManagedToStdString(centerY);
        std::string viewWidthStr = ManagedToStdString(viewWidth);

        // Set global decimals for MPFR precision
        decimals = precision;

        // Create BigComplex for center coordinate (using native ::BigDouble with MPFR string parsing)
        BigComplex centre;
        centre.x = ::BigDouble(0.0);  // Initialize with default constructor
        centre.y = ::BigDouble(0.0);

        // Parse strings directly into MPFR values
        mpfr_set_str(centre.x.x, centerXStr.c_str(), 10, MPFR_RNDN);
        mpfr_set_str(centre.y.x, centerYStr.c_str(), 10, MPFR_RNDN);

        // Create BigDouble for view width (using native ::BigDouble)
        ::BigDouble bigWidth(0.0);
        mpfr_set_str(bigWidth.x, viewWidthStr.c_str(), 10, MPFR_RNDN);

        // Determine arithmetic type (DOUBLE, FLOATEXP, etc.)
        BYTE bigNumFlag = 1; // We're using BigDouble, so this is true
        PertSetupArithType(::ArithType, subtype, maxIteration, precision, bigNumFlag);
        m_cachedArithType = ::ArithType;

        Debug::WriteLine(String::Format("BuildReferenceOrbit: ArithType={0}, SlopeDegree={1}", ::ArithType, ::SlopeDegree));

        // Build reference orbit
        char statusBarInfo[256] = "";
        int pertProgress = 0;

        int result = ReferenceZoomPoint(
            centre,
            maxIteration,
            DummyUserData,
            statusBarInfo,
            &pertProgress,
            bailout,
            ::ArithType,
            power,
            bigWidth,
            ::SlopeDegree
        );

        if (result < 0)
        {
            Debug::WriteLine("BuildReferenceOrbit: Cancelled or failed");
            m_referenceOrbitValid = false;
            return result;
        }

        // Cache reference orbit metadata
        auto refData = static_cast<StoreReferenceData*>(m_refData);
        refData->valid = true;
        refData->BigWidth = bigWidth;
        refData->ReferenceCoordinate = centre;
        refData->rqlim = bailout;
        refData->degree = (WORD)power;
        m_referenceOrbitValid = true;

        Debug::WriteLine(String::Format("BuildReferenceOrbit: Success! MaxRefIteration={0}, orbit size={1}", 
            ::MaxRefIteration,
            (::ArithType == DOUBLE || ::ArithType == DBL_UNSUPPORTED) ? XSubN.size() : ExpXSubN.size()));

        return 0;
    }
    catch (const std::exception& ex)
    {
        Debug::WriteLine(String::Format("BuildReferenceOrbit: Exception: {0}", gcnew String(ex.what())));
        m_referenceOrbitValid = false;
        throw gcnew InvalidOperationException("Failed to build reference orbit: " + gcnew String(ex.what()));
    }
}

bool FractalEngineWrapper::IsReferenceOrbitValid(
    String^ centerX,
    String^ centerY,
    String^ viewWidth,
    int maxIteration,
    double bailout,
    int power)
{
    if (!m_referenceOrbitValid || m_refData == nullptr)
        return false;

    try
    {
        // Convert managed strings to native types
        std::string centerXStr = ManagedToStdString(centerX);
        std::string centerYStr = ManagedToStdString(centerY);
        std::string viewWidthStr = ManagedToStdString(viewWidth);

        // Get precision from cached ArithType (estimate from reference data)
        auto refData = static_cast<StoreReferenceData*>(m_refData);
        int precision = (m_cachedArithType == FLOATEXP || m_cachedArithType == EXP_UNSUPPORTED) ? 300 : 53;

        // Set global decimals
        decimals = precision;

        // Create BigComplex and BigDouble for comparison (using native types with MPFR parsing)
        BigComplex centre;
        centre.x = ::BigDouble(0.0);
        centre.y = ::BigDouble(0.0);
        mpfr_set_str(centre.x.x, centerXStr.c_str(), 10, MPFR_RNDN);
        mpfr_set_str(centre.y.x, centerYStr.c_str(), 10, MPFR_RNDN);

        ::BigDouble bigWidth(0.0);
        mpfr_set_str(bigWidth.x, viewWidthStr.c_str(), 10, MPFR_RNDN);

        // Check validity using native function
        bool valid = CheckValidRef(centre, bigWidth, maxIteration, bailout, *refData, power, m_cachedArithType);

        Debug::WriteLine(String::Format("IsReferenceOrbitValid: {0}", valid ? "true" : "false"));
        return valid;
    }
    catch (const std::exception& ex)
    {
        Debug::WriteLine(String::Format("IsReferenceOrbitValid: Exception: {0}", gcnew String(ex.what())));
        return false;
    }
}

FractalResult^ FractalEngineWrapper::CalculateWithPerturbation(FractalParameters^ parameters)
{
    if (parameters == nullptr)
        throw gcnew ArgumentNullException("parameters");

    if (!m_referenceOrbitValid)
        throw gcnew InvalidOperationException("Reference orbit not built. Call BuildReferenceOrbit() first.");

    Debug::WriteLine("CalculateWithPerturbation: Starting perturbation-based render");

    m_cancelled = false;
    auto stopwatch = Stopwatch::StartNew();

    try
    {
        int width = parameters->Width;
        int height = parameters->Height;
        int maxIterations = parameters->MaxIterations;
        int pixelCount = width * height * 4; // BGRA

        Debug::WriteLine(String::Format("CalculateWithPerturbation: {0}x{1}, maxIter={2}", width, height, maxIterations));

        // Create result
        auto result = gcnew FractalResult();
        result->PixelData = gcnew array<Byte>(pixelCount);
        result->Width = width;
        result->Height = height;

        // Get reference orbit data
        auto refData = static_cast<StoreReferenceData*>(m_refData);

        // Extract reference center coordinates (convert BigDouble to double)
        double centerX = mpfr_get_d(refData->ReferenceCoordinate.x.x, MPFR_RNDN);
        double centerY = mpfr_get_d(refData->ReferenceCoordinate.y.x, MPFR_RNDN);
        double viewWidth = mpfr_get_d(refData->BigWidth.x, MPFR_RNDN);
        double bailout = refData->rqlim;

        Debug::WriteLine(String::Format("CalculateWithPerturbation: Center=({0}, {1}), ViewWidth={2}, Bailout={3}", 
            centerX, centerY, viewWidth, bailout));
        Debug::WriteLine(String::Format("CalculateWithPerturbation: ArithType={0}, Orbit size={1}", 
            m_cachedArithType, 
            (m_cachedArithType == DOUBLE || m_cachedArithType == DBL_UNSUPPORTED) ? XSubN.size() : ExpXSubN.size()));

        // Get palette info
        ::Native::PaletteType nativePalette = static_cast<::Native::PaletteType>((int)parameters->Palette);
        int colorOffset = parameters->ColorOffset;

        long long totalIterations = 0;
        int escapedPixels = 0;
        int blaSkipsUsed = 0;

        // Check if BLA is available (from PertSetup.cpp: Bla global)
        // Bla is a global BLAS instance, so we take its address
        bool blaEnabled = (::Bla.isValid && ::EnableApproximation);

        Debug::WriteLine(String::Format("CalculateWithPerturbation: BLA enabled={0}", blaEnabled));

        // Pixel loop - calculate fractal using perturbation theory
        for (int y = 0; y < height; y++)
        {
            // Report progress every 10 lines
            if (y % 10 == 0)
            {
                if (m_progressChangedDelegate != nullptr)
                {
                    auto progressArgs = gcnew ProgressEventArgs();
                    progressArgs->Percentage = (y * 100.0) / height;
                    progressArgs->CurrentLine = y;
                    progressArgs->TotalLines = height;
                    progressArgs->StatusMessage = String::Format("Perturbation: line {0} of {1}", y, height);
                    ProgressChanged(this, progressArgs);
                }
            }

            if (m_cancelled)
                throw gcnew OperationCanceledException("Calculation cancelled by user");

            for (int x = 0; x < width; x++)
            {
                // Map pixel to complex plane offset from reference center
                // ΔC = (pixel_coordinate - reference_center)
                Complex deltaC;
                deltaC.x = ((x - width / 2.0) / width) * viewWidth;
                deltaC.y = ((y - height / 2.0) / height) * viewWidth;

                // Perturbation iteration: ΔZₙ₊₁ ≈ 2·Zₙ·ΔZₙ + ΔC
                Complex deltaZ = deltaC;  // ΔZ₀ = ΔC
                int iteration = 0;
                int refIteration = 0;  // Track position in reference orbit

                // Choose reference orbit based on ArithType
                if (m_cachedArithType == DOUBLE || m_cachedArithType == DBL_UNSUPPORTED)
                {
                    // Use double-precision reference orbit (XSubN)
                    int orbitSize = (int)XSubN.size();

                    while (iteration < maxIterations && refIteration < orbitSize)
                    {
                        // Try BLA acceleration first
                        if (blaEnabled && ::EnableApproximation)
                        {
                            double deltaNormSq = deltaZ.x * deltaZ.x + deltaZ.y * deltaZ.y;
                            const BLA* blaPtr = ::Bla.lookup(refIteration, deltaNormSq, iteration, maxIterations);

                            if (blaPtr != nullptr && blaPtr->l > 0)
                            {
                                // BLA skip: apply linear transform ΔZ = A·ΔZ + B·ΔC
                                double newDzX = blaPtr->Ax * deltaZ.x - blaPtr->Ay * deltaZ.y 
                                              + blaPtr->Bx * deltaC.x - blaPtr->By * deltaC.y;
                                double newDzY = blaPtr->Ax * deltaZ.y + blaPtr->Ay * deltaZ.x 
                                              + blaPtr->Bx * deltaC.y + blaPtr->By * deltaC.x;
                                deltaZ.x = newDzX;
                                deltaZ.y = newDzY;

                                iteration += blaPtr->l;
                                refIteration += blaPtr->l;
                                blaSkipsUsed++;

                                // Check bounds and escape after skip
                                if (refIteration >= orbitSize)
                                    break;

                                Complex Zn = XSubN[refIteration];
                                double zx = Zn.x + deltaZ.x;
                                double zy = Zn.y + deltaZ.y;
                                double magnitudeSq = zx * zx + zy * zy;

                                if (magnitudeSq > bailout)
                                    break;

                                continue;  // Try another BLA skip
                            }
                        }

                        // Fall back to single-step perturbation
                        Complex Zn = XSubN[refIteration];

                        // Perturbation formula: ΔZ_{n+1} ≈ 2·Z_n·ΔZ_n + ΔC
                        Complex temp;
                        temp.x = 2.0 * (Zn.x * deltaZ.x - Zn.y * deltaZ.y);
                        temp.y = 2.0 * (Zn.x * deltaZ.y + Zn.y * deltaZ.x);
                        deltaZ.x = temp.x + deltaC.x;
                        deltaZ.y = temp.y + deltaC.y;

                        iteration++;
                        refIteration++;

                        // Test for escape: |Z_n + ΔZ_n|² > bailout
                        double zx = Zn.x + deltaZ.x;
                        double zy = Zn.y + deltaZ.y;
                        double magnitudeSq = zx * zx + zy * zy;

                        if (magnitudeSq > bailout)
                            break;
                    }
                }
                else
                {
                    // Use extended-precision reference orbit (ExpXSubN)
                    int orbitSize = (int)ExpXSubN.size();

                    while (iteration < maxIterations && refIteration < orbitSize)
                    {
                        // Try BLA acceleration first (ExpComplex version)
                        if (blaEnabled && ::EnableApproximation)
                        {
                            floatexp deltaNormSq;
                            deltaNormSq.val = deltaZ.x * deltaZ.x + deltaZ.y * deltaZ.y;
                            deltaNormSq.exp = 0;

                            const BLAExp* blaPtr = ::Bla.lookupExp(refIteration, deltaNormSq, iteration, maxIterations);

                            if (blaPtr != nullptr && blaPtr->l > 0)
                            {
                                // Convert floatexp to double for deltaZ application
                                double Ax = blaPtr->Ax.val * pow(2.0, blaPtr->Ax.exp);
                                double Ay = blaPtr->Ay.val * pow(2.0, blaPtr->Ay.exp);
                                double Bx = blaPtr->Bx.val * pow(2.0, blaPtr->Bx.exp);
                                double By = blaPtr->By.val * pow(2.0, blaPtr->By.exp);

                                double newDzX = Ax * deltaZ.x - Ay * deltaZ.y 
                                              + Bx * deltaC.x - By * deltaC.y;
                                double newDzY = Ax * deltaZ.y + Ay * deltaZ.x 
                                              + Bx * deltaC.y + By * deltaC.x;
                                deltaZ.x = newDzX;
                                deltaZ.y = newDzY;

                                iteration += blaPtr->l;
                                refIteration += blaPtr->l;
                                blaSkipsUsed++;

                                // Check bounds and escape after skip
                                if (refIteration >= orbitSize)
                                    break;

                                ExpComplex Zn = ExpXSubN[refIteration];
                                double ZnX = Zn.x.val * pow(2.0, Zn.x.exp);
                                double ZnY = Zn.y.val * pow(2.0, Zn.y.exp);
                                double zx = ZnX + deltaZ.x;
                                double zy = ZnY + deltaZ.y;
                                double magnitudeSq = zx * zx + zy * zy;

                                if (magnitudeSq > bailout)
                                    break;

                                continue;  // Try another BLA skip
                            }
                        }

                        // Fall back to single-step perturbation
                        ExpComplex Zn = ExpXSubN[refIteration];

                        // Convert ExpComplex to double for this calculation
                        double ZnX = Zn.x.val * pow(2.0, Zn.x.exp);
                        double ZnY = Zn.y.val * pow(2.0, Zn.y.exp);

                        // Perturbation formula
                        Complex temp;
                        temp.x = 2.0 * (ZnX * deltaZ.x - ZnY * deltaZ.y);
                        temp.y = 2.0 * (ZnX * deltaZ.y + ZnY * deltaZ.x);
                        deltaZ.x = temp.x + deltaC.x;
                        deltaZ.y = temp.y + deltaC.y;

                        iteration++;
                        refIteration++;

                        // Test for escape
                        double zx = ZnX + deltaZ.x;
                        double zy = ZnY + deltaZ.y;
                        double magnitudeSq = zx * zx + zy * zy;

                        if (magnitudeSq > bailout)
                            break;
                    }
                }

                totalIterations += iteration;
                if (iteration < maxIterations)
                    escapedPixels++;

                // Convert iteration to color
                ::Native::ColorRGB color = ::Native::MandelbrotCalculator::IterationToColor(
                    (double)iteration,
                    maxIterations,
                    nativePalette,
                    colorOffset
                );

                // Write BGRA pixel
                int index = (y * width + x) * 4;
                result->PixelData[index + 0] = color.b;  // Blue
                result->PixelData[index + 1] = color.g;  // Green
                result->PixelData[index + 2] = color.r;  // Red
                result->PixelData[index + 3] = 255;      // Alpha
            }
        }

        stopwatch->Stop();

        // Fill result metadata
        result->UsedPerturbation = true;
        result->ArithType = m_cachedArithType;
        result->MaxRefIteration = ::MaxRefIteration;
        result->BLAEnabled = blaEnabled;
        result->ReferenceOrbitBuildTime = 0.0;  // TODO: Track this in BuildReferenceOrbit
        result->RenderTime = stopwatch->Elapsed;
        result->IterationCount = totalIterations;
        result->EscapedPixelCount = escapedPixels;

        // Final progress update
        if (m_progressChangedDelegate != nullptr)
        {
            auto finalProgress = gcnew ProgressEventArgs();
            finalProgress->Percentage = 100.0;
            finalProgress->CurrentLine = height;
            finalProgress->TotalLines = height;
            finalProgress->StatusMessage = "Complete";
            ProgressChanged(this, finalProgress);
        }

        Debug::WriteLine(String::Format("CalculateWithPerturbation: Complete! Time={0}ms, AvgIter={1}, BLA skips={2}", 
            result->RenderTime.TotalMilliseconds, 
            totalIterations / (width * height),
            blaSkipsUsed));

        return result;
    }
    catch (Exception^)
    {
        stopwatch->Stop();
        throw;
    }
}
