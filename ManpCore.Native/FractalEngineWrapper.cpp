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
        // For IFS fractals, pass iteration counter through z for deterministic randomness
        if (spec->category == "Iterated Function Systems")
        {
            z = static_cast<double>(iter);
        }

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

    // Phase 2: Reset and accumulate histogram using USER zoom/pan parameters
    x = 0.1;
    y = 0.1;
    z = 1.0;

    // Use user's zoom/pan parameters from the UI
    // params.centerX, params.centerY = user's pan position
    // params.viewWidth, params.viewHeight = zoom level (smaller = more zoomed in)
    double viewLeft = params.centerX - params.viewWidth / 2.0;
    double viewRight = params.centerX + params.viewWidth / 2.0;
    double viewBottom = params.centerY - params.viewHeight / 2.0;
    double viewTop = params.centerY + params.viewHeight / 2.0;
    double viewWidth = viewRight - viewLeft;
    double viewHeight = viewTop - viewBottom;

    Debug::WriteLine(String::Format("  User Zoom/Pan: Center=({0:F2}, {1:F2}), View={2:F2}x{3:F2}", 
        params.centerX, params.centerY, params.viewWidth, params.viewHeight));
    Debug::WriteLine(String::Format("  Viewport: X=[{0:F2}, {1:F2}], Y=[{2:F2}, {3:F2}]", viewLeft, viewRight, viewBottom, viewTop));

    for (int iter = 0; iter < orbitCount; ++iter)
    {
        // For IFS fractals, pass iteration counter through z for deterministic randomness
        if (spec->category == "Iterated Function Systems")
        {
            z = static_cast<double>(iter);
        }

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

        // Add 3x3 splat filter. Attractor points are infinitesimally thin and sparsely 
        // distributed; this filter smears out the visits, bridging gaps and creating a beautiful continuous glow.
        if (px >= 1 && px < width - 1 && py >= 1 && py < height - 1)
        {
            int baseIdx = py * width + px;
            histogram[baseIdx] += 4;         // Center
            histogram[baseIdx - 1] += 2;     // Left
            histogram[baseIdx + 1] += 2;     // Right
            histogram[baseIdx - width] += 2; // Top
            histogram[baseIdx + width] += 2; // Bottom

            // Corners
            histogram[baseIdx - width - 1] += 1;
            histogram[baseIdx - width + 1] += 1;
            histogram[baseIdx + width - 1] += 1;
            histogram[baseIdx + width + 1] += 1;
        }
        else if (px >= 0 && px < width && py >= 0 && py < height)
        {
            // Boundary fallback
            histogram[py * width + px] += 4;
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

//=============================================================================
// Buddhabrot Rendering (Monte Carlo Path Accumulation)
//=============================================================================

/// <summary>
/// Test if a point escapes and return the escape iteration count.
/// Used for Nebulabrot to filter points by escape-time range before tracking paths.
/// </summary>
/// <param name="cx">Real part of complex constant c</param>
/// <param name="cy">Imaginary part of complex constant c</param>
/// <param name="maxThreshold">Maximum iterations to test</param>
/// <returns>Escape iteration (0 to maxThreshold), or maxThreshold+1 if point never escapes</returns>
static int TestEscapeIteration(double cx, double cy, int maxThreshold)
{
    double zr = 0.0, zi = 0.0;

    for (int n = 0; n <= maxThreshold; n++)
    {
        double zr2 = zr * zr;
        double zi2 = zi * zi;

        if (zr2 + zi2 > 4.0) {
            return n;  // Escaped at iteration n
        }

        double temp = zr2 - zi2 + cx;
        zi = 2.0 * zr * zi + cy;
        zr = temp;
    }

    return maxThreshold + 1;  // Never escaped
}

/// <summary>
/// Track Mandelbrot orbit path and accumulate into SINGLE channel histogram.
/// Used for Nebulabrot RGB separation: each channel tracks different escape-time ranges.
/// </summary>
/// <param name="channelCount">Single channel histogram to accumulate into</param>
static void DrawPathForChannel(
    double cx, double cy,
    const ::Native::MandelbrotParams& params,
    int width, int height,
    int escapeIteration,  // Known escape iteration from prior test
    std::vector<long>& channelCount)
{
    double zr = 0.0, zi = 0.0;

    // Track path up to escape iteration
    for (int n = 0; n <= escapeIteration; n++)
    {
        // Map orbit point to pixel coordinates
        int px = (int)((zr - (params.centerX - params.viewWidth / 2.0)) / params.viewWidth * width);
        int py = (int)((zi - (params.centerY - params.viewHeight / 2.0)) / params.viewHeight * height);

        if (px >= 0 && px < width && py >= 0 && py < height)
        {
            int index = py * width + px;
            channelCount[index]++;
        }

        // Advance orbit
        double zr2 = zr * zr;
        double zi2 = zi * zi;
        double temp = zr2 - zi2 + cx;
        zi = 2.0 * zr * zi + cy;
        zr = temp;
    }
}

/// <summary>
/// Track Mandelbrot orbit path and accumulate histogram.
/// Matches original BuddhaBrot.cpp algorithm: accumulate ALL orbit points into ALL channels.
/// Color separation comes from different multipliers (0.09 red, 0.11 green, 0.18 blue).
/// </summary>
static void DrawPath(
    double cx, double cy,
    const ::Native::MandelbrotParams& params,
    int width, int height, int threshold,
    std::vector<long>& redCount,
    std::vector<long>& greenCount,
    std::vector<long>& blueCount)
{
    double zr = 0.0, zi = 0.0;

    for (int n = 0; n <= threshold; n++)
    {
        double zr2 = zr * zr;
        double zi2 = zi * zi;

        if (zr2 + zi2 > 4.0) return;

        // Map orbit point to pixel coordinates
        int px = (int)((zr - (params.centerX - params.viewWidth / 2.0)) / params.viewWidth * width);
        int py = (int)((zi - (params.centerY - params.viewHeight / 2.0)) / params.viewHeight * height);

        if (px >= 0 && px < width && py >= 0 && py < height)
        {
            int index = py * width + px;

            // Accumulate into ALL three channels (original algorithm)
            // Color separation comes from different brightness multipliers
            redCount[index]++;
            greenCount[index]++;
            blueCount[index]++;
        }

        double temp = zr2 - zi2 + cx;
        zi = 2.0 * zr * zi + cy;
        zr = temp;
    }
}

/// <summary>
/// Map density value [0, 1] to heat spectrum color.
/// Black → Blue → Cyan → Green → Yellow → Orange → Red → White
/// </summary>
static void MapDensityToColor(double density, unsigned char& r, unsigned char& g, unsigned char& b)
{
    if (density <= 0.0) {
        r = g = b = 0;
        return;
    }

    // Clamp to [0, 1]
    density = (std::min)(1.0, density);

    // Inverted scale: low density = dark, high density = bright/hot
    if (density < 0.25) {
        // Black → Dark Blue
        double t = density / 0.25;
        r = 0;
        g = 0;
        b = static_cast<unsigned char>(t * 128);
    }
    else if (density < 0.4) {
        // Dark Blue → Cyan
        double t = (density - 0.25) / 0.15;
        r = 0;
        g = static_cast<unsigned char>(t * 255);
        b = static_cast<unsigned char>(128 + t * 127);
    }
    else if (density < 0.55) {
        // Cyan → Green
        double t = (density - 0.4) / 0.15;
        r = 0;
        g = 255;
        b = static_cast<unsigned char>((1.0 - t) * 255);
    }
    else if (density < 0.7) {
        // Green → Yellow
        double t = (density - 0.55) / 0.15;
        r = static_cast<unsigned char>(t * 255);
        g = 255;
        b = 0;
    }
    else if (density < 0.85) {
        // Yellow → Orange
        double t = (density - 0.7) / 0.15;
        r = 255;
        g = static_cast<unsigned char>((1.0 - t * 0.4) * 255);
        b = 0;
    }
    else if (density < 0.95) {
        // Orange → Red
        double t = (density - 0.85) / 0.1;
        r = 255;
        g = static_cast<unsigned char>((1.0 - t) * 153);
        b = 0;
    }
    else {
        // Red → White (hot spots)
        double t = (density - 0.95) / 0.05;
        r = 255;
        g = static_cast<unsigned char>(t * 255);
        b = static_cast<unsigned char>(t * 255);
    }
}

/// <summary>
/// Convert accumulated histogram to RGB pixels using color spectrum mapping.
/// </summary>
/// <summary>
/// Convert accumulated histogram to RGB pixels.
/// Supports two modes:
/// - Buddhabrot: All channels normalized together for subtle color variation
/// - Nebulabrot: Each channel normalized independently for dramatic RGB separation
/// </summary>
static void ConvertHistogramToPixels(
    FractalResult^ result,
    int width, int height,
    const std::vector<long>& redCount,
    const std::vector<long>& greenCount,
    const std::vector<long>& blueCount,
    double brightness,
    ::Native::PaletteType palette,
    int colorOffset,
    bool isNebulabrot)
{
    if (isNebulabrot)
    {
        //=====================================================================
        // NEBULABROT: Independent channel normalization for dramatic colors
        //=====================================================================
        Debug::WriteLine("  Converting histogram: NEBULABROT mode (independent RGB normalization)");

        // Find max value in EACH channel independently
        long maxRed = 0, maxGreen = 0, maxBlue = 0;
        for (int i = 0; i < width * height; i++)
        {
            if (redCount[i] > maxRed) maxRed = redCount[i];
            if (greenCount[i] > maxGreen) maxGreen = greenCount[i];
            if (blueCount[i] > maxBlue) maxBlue = blueCount[i];
        }

        // Prevent division by zero
        if (maxRed == 0) maxRed = 1;
        if (maxGreen == 0) maxGreen = 1;
        if (maxBlue == 0) maxBlue = 1;

        Debug::WriteLine(String::Format("  Max densities: R={0}, G={1}, B={2}", maxRed, maxGreen, maxBlue));

        // Logarithmic normalization per channel
        double logMaxRed = std::log(1.0 + (double)maxRed);
        double logMaxGreen = std::log(1.0 + (double)maxGreen);
        double logMaxBlue = std::log(1.0 + (double)maxBlue);

        for (int i = 0; i < width * height; i++)
        {
            // Normalize each channel independently (key to dramatic separation!)
            double redDensity = (redCount[i] > 0) ? std::log(1.0 + (double)redCount[i]) / logMaxRed : 0.0;
            double greenDensity = (greenCount[i] > 0) ? std::log(1.0 + (double)greenCount[i]) / logMaxGreen : 0.0;
            double blueDensity = (blueCount[i] > 0) ? std::log(1.0 + (double)blueCount[i]) / logMaxBlue : 0.0;

            // Apply brightness and gamma correction with channel-specific boosts
            // Boost red and green more aggressively to balance against dominant blue
            double gamma = 0.4;  // Lower gamma = more contrast
            double redBoost = 1.5;    // Amplify red channel
            double greenBoost = 1.3;  // Amplify green channel
            double blueBoost = 1.0;   // Keep blue as baseline

            double rVal = 255.0 * std::pow(redDensity * brightness * redBoost, gamma);
            double gVal = 255.0 * std::pow(greenDensity * brightness * greenBoost, gamma);
            double bVal = 255.0 * std::pow(blueDensity * brightness * blueBoost, gamma);

            unsigned char r = static_cast<unsigned char>(rVal > 255.0 ? 255.0 : rVal);
            unsigned char g = static_cast<unsigned char>(gVal > 255.0 ? 255.0 : gVal);
            unsigned char b = static_cast<unsigned char>(bVal > 255.0 ? 255.0 : bVal);

            // Write BGRA format
            int pixelIndex = i * 4;
            result->PixelData[pixelIndex + 0] = b;
            result->PixelData[pixelIndex + 1] = g;
            result->PixelData[pixelIndex + 2] = r;
            result->PixelData[pixelIndex + 3] = 255;
        }
    }
    else
    {
        //=====================================================================
        // BUDDHABROT: Shared normalization for subtle color variation
        //=====================================================================
        Debug::WriteLine("  Converting histogram: BUDDHABROT mode (shared normalization)");

        // Find max histogram value for normalization
        long maxCount = 0;
        for (int i = 0; i < width * height; i++)
        {
            long totalCount = redCount[i] + greenCount[i] + blueCount[i];
            if (totalCount > maxCount) maxCount = totalCount;
        }

        if (maxCount == 0) maxCount = 1; // Prevent division by zero

        Debug::WriteLine(String::Format("  Max density: {0}", maxCount));

        // Use logarithmic normalization for wide dynamic range
        double logMax = std::log(1.0 + (double)maxCount);

        for (int i = 0; i < width * height; i++)
        {
            // Sum all channels for total density
            long totalCount = redCount[i] + greenCount[i] + blueCount[i];

            if (totalCount == 0)
            {
                // Black background
                int pixelIndex = i * 4;
                result->PixelData[pixelIndex + 0] = 0;
                result->PixelData[pixelIndex + 1] = 0;
                result->PixelData[pixelIndex + 2] = 0;
                result->PixelData[pixelIndex + 3] = 255;
                continue;
            }

            // Logarithmic compression to handle wide dynamic range
            double logDensity = std::log(1.0 + (double)totalCount);
            double density = logDensity / logMax;

            // Apply brightness boost
            density = (std::min)(1.0, density * brightness * 2.0);

            // Map density to color spectrum
            unsigned char r, g, b;
            MapDensityToColor(density, r, g, b);

            // Write BGRA format
            int pixelIndex = i * 4;
            result->PixelData[pixelIndex + 0] = b;
            result->PixelData[pixelIndex + 1] = g;
            result->PixelData[pixelIndex + 2] = r;
            result->PixelData[pixelIndex + 3] = 255;
        }
    }
}

/// <summary>
/// Render Buddhabrot using Monte Carlo path accumulation.
/// Samples millions of starting points, tracks escape orbits, accumulates RGB histogram.
/// </summary>
static void RenderBuddhabrotFractal(
    FractalResult^ result,
    const ::Native::MandelbrotParams& params,
    int width,
    int height,
    ::Native::PaletteType palette,
    int colorOffset,
    FractalEngineWrapper^ engine,
    bool isNebulabrot)
{
    if (isNebulabrot)
    {
        Debug::WriteLine("RenderBuddhabrotFractal: Starting NEBULABROT mode (3-pass RGB separation)");
        Debug::WriteLine("  WARNING: Nebulabrot is 3× more expensive than standard Buddhabrot");
    }
    else
    {
        Debug::WriteLine("RenderBuddhabrotFractal: Starting BUDDHABROT mode (single-pass subtle colors)");
    }

    // Use maxIterations from params as threshold
    // TODO: Extract custom brightness parameter when parameter system is integrated
    double brightness = 1.0;
    int threshold = params.maxIterations;

    Debug::WriteLine(String::Format("  Brightness: {0}, Threshold: {1}", brightness, threshold));

    // Allocate three histograms (RGB channels)
    std::vector<long> redCount(width * height, 0);
    std::vector<long> greenCount(width * height, 0);
    std::vector<long> blueCount(width * height, 0);

    // Sampling grid (10x denser than output resolution)
    const int SOURCE_COLUMNS = width * 10;
    const int SOURCE_ROWS = height * 10;

    double x_jump = params.viewWidth / SOURCE_COLUMNS;
    double y_jump = params.viewHeight / SOURCE_ROWS;

    Debug::WriteLine(String::Format("  Sampling grid: {0}x{1} ({2} total points)", 
                                    SOURCE_COLUMNS, SOURCE_ROWS, 
                                    (long long)SOURCE_COLUMNS * SOURCE_ROWS));

    //=========================================================================
    // MODE SELECTION: Buddhabrot (single-pass) vs Nebulabrot (three-pass)
    //=========================================================================

    if (isNebulabrot)
    {
        //=====================================================================
        // NEBULABROT MODE: Three separate passes with RGB threshold separation
        //=====================================================================

        // Define iteration ranges ADAPTIVELY based on maxIterations
        // Blue = fast escapers (first 20% of iteration range)
        // Green = medium escapers (middle 30-60% of iteration range)  
        // Red = slow escapers (last 40% of iteration range)

        const int BLUE_MIN = (int)(threshold * 0.05);   // Start at 5% (avoid very fast escapers)
        const int BLUE_MAX = (int)(threshold * 0.25);   // End at 25%
        const int GREEN_MIN = (int)(threshold * 0.30);  // Start at 30%
        const int GREEN_MAX = (int)(threshold * 0.65);  // End at 65%
        const int RED_MIN = (int)(threshold * 0.70);    // Start at 70%
        const int RED_MAX = threshold;                   // End at 100%

        Debug::WriteLine(String::Format("  Adaptive threshold ranges (maxIter={0}):", threshold));
        Debug::WriteLine(String::Format("    Blue channel: {0}-{1} iterations ({2}% - {3}%)", 
                                        BLUE_MIN, BLUE_MAX, 5, 25));
        Debug::WriteLine(String::Format("    Green channel: {0}-{1} iterations ({2}% - {3}%)", 
                                        GREEN_MIN, GREEN_MAX, 30, 65));
        Debug::WriteLine(String::Format("    Red channel: {0}-{1} iterations ({2}% - {3}%)", 
                                        RED_MIN, RED_MAX, 70, 100));

        int lastReportedPercent = -1;
        const int TOTAL_PASSES = 3;

        // PASS 1: BLUE CHANNEL (fast escapers)
        Debug::WriteLine("  === PASS 1/3: BLUE CHANNEL (fast escapers) ===");
        double y = params.centerY + params.viewHeight / 2.0;

        for (int source_row = SOURCE_ROWS - 1; source_row >= 0; source_row--, y -= y_jump)
        {
            // Progress: Pass 1 contributes 0-33%
            int currentPercent = (int)(((SOURCE_ROWS - source_row) * 33.0) / SOURCE_ROWS);
            if (currentPercent != lastReportedPercent && currentPercent % 1 == 0)
            {
                lastReportedPercent = currentPercent;
                if (engine != nullptr)
                {
                    auto args = gcnew ProgressEventArgs();
                    args->Percentage = currentPercent;
                    args->CurrentLine = SOURCE_ROWS - source_row;
                    args->TotalLines = SOURCE_ROWS;
                    args->StatusMessage = String::Format("Nebulabrot Pass 1/3 (BLUE): row {0}/{1} ({2}%)", 
                                                         SOURCE_ROWS - source_row, SOURCE_ROWS, currentPercent);
                    engine->ProgressChanged(engine, args);
                }
            }

            double x = params.centerX - params.viewWidth / 2.0;
            for (int source_column = 0; source_column < SOURCE_COLUMNS; source_column++, x += x_jump)
            {
                // Skip Mandelbrot body optimization
                if ((x > -1.2 && x <= -1.1 && y > -0.1 && y < 0.1) ||
                    (x > -1.1 && x <= -0.9 && y > -0.2 && y < 0.2) ||
                    (x > -0.9 && x <= -0.8 && y > -0.1 && y < 0.1) ||
                    (x > -0.69 && x <= -0.61 && y > -0.277 && y < -0.193) ||
                    (x > -0.55 && x <= -0.5 && y > 0.47 && y < 0.51) ||
                    (x > -0.55 && x <= -0.5 && y > -0.51 && y < -0.47) ||
                    (x > 0.27 && x <= 0.31 && y > 0.004 && y < 0.026) ||
                    (x > 0.27 && x <= 0.31 && y > -0.026 && y < -0.004))
                    continue;

                // Test escape iteration
                int escapeIter = TestEscapeIteration(x, y, BLUE_MAX);

                // Filter: Only track if escaped in blue range
                if (escapeIter >= BLUE_MIN && escapeIter <= BLUE_MAX)
                {
                    DrawPathForChannel(x, y, params, width, height, escapeIter, blueCount);
                }
            }
        }

        // PASS 2: GREEN CHANNEL (medium escapers)
        Debug::WriteLine("  === PASS 2/3: GREEN CHANNEL (medium escapers) ===");
        y = params.centerY + params.viewHeight / 2.0;

        for (int source_row = SOURCE_ROWS - 1; source_row >= 0; source_row--, y -= y_jump)
        {
            // Progress: Pass 2 contributes 33-66%
            int currentPercent = 33 + (int)(((SOURCE_ROWS - source_row) * 33.0) / SOURCE_ROWS);
            if (currentPercent != lastReportedPercent && currentPercent % 1 == 0)
            {
                lastReportedPercent = currentPercent;
                if (engine != nullptr)
                {
                    auto args = gcnew ProgressEventArgs();
                    args->Percentage = currentPercent;
                    args->CurrentLine = SOURCE_ROWS - source_row;
                    args->TotalLines = SOURCE_ROWS;
                    args->StatusMessage = String::Format("Nebulabrot Pass 2/3 (GREEN): row {0}/{1} ({2}%)", 
                                                         SOURCE_ROWS - source_row, SOURCE_ROWS, currentPercent);
                    engine->ProgressChanged(engine, args);
                }
            }

            double x = params.centerX - params.viewWidth / 2.0;
            for (int source_column = 0; source_column < SOURCE_COLUMNS; source_column++, x += x_jump)
            {
                // Skip Mandelbrot body optimization
                if ((x > -1.2 && x <= -1.1 && y > -0.1 && y < 0.1) ||
                    (x > -1.1 && x <= -0.9 && y > -0.2 && y < 0.2) ||
                    (x > -0.9 && x <= -0.8 && y > -0.1 && y < 0.1) ||
                    (x > -0.69 && x <= -0.61 && y > -0.277 && y < -0.193) ||
                    (x > -0.55 && x <= -0.5 && y > 0.47 && y < 0.51) ||
                    (x > -0.55 && x <= -0.5 && y > -0.51 && y < -0.47) ||
                    (x > 0.27 && x <= 0.31 && y > 0.004 && y < 0.026) ||
                    (x > 0.27 && x <= 0.31 && y > -0.026 && y < -0.004))
                    continue;

                // Test escape iteration
                int escapeIter = TestEscapeIteration(x, y, GREEN_MAX);

                // Filter: Only track if escaped in green range
                if (escapeIter >= GREEN_MIN && escapeIter <= GREEN_MAX)
                {
                    DrawPathForChannel(x, y, params, width, height, escapeIter, greenCount);
                }
            }
        }

        // PASS 3: RED CHANNEL (slow escapers)
        Debug::WriteLine("  === PASS 3/3: RED CHANNEL (slow escapers) ===");
        y = params.centerY + params.viewHeight / 2.0;

        for (int source_row = SOURCE_ROWS - 1; source_row >= 0; source_row--, y -= y_jump)
        {
            // Progress: Pass 3 contributes 66-100%
            int currentPercent = 66 + (int)(((SOURCE_ROWS - source_row) * 34.0) / SOURCE_ROWS);
            if (currentPercent != lastReportedPercent && currentPercent % 1 == 0)
            {
                lastReportedPercent = currentPercent;
                if (engine != nullptr)
                {
                    auto args = gcnew ProgressEventArgs();
                    args->Percentage = currentPercent;
                    args->CurrentLine = SOURCE_ROWS - source_row;
                    args->TotalLines = SOURCE_ROWS;
                    args->StatusMessage = String::Format("Nebulabrot Pass 3/3 (RED): row {0}/{1} ({2}%)", 
                                                         SOURCE_ROWS - source_row, SOURCE_ROWS, currentPercent);
                    engine->ProgressChanged(engine, args);
                }
            }

            double x = params.centerX - params.viewWidth / 2.0;
            for (int source_column = 0; source_column < SOURCE_COLUMNS; source_column++, x += x_jump)
            {
                // Skip Mandelbrot body optimization
                if ((x > -1.2 && x <= -1.1 && y > -0.1 && y < 0.1) ||
                    (x > -1.1 && x <= -0.9 && y > -0.2 && y < 0.2) ||
                    (x > -0.9 && x <= -0.8 && y > -0.1 && y < 0.1) ||
                    (x > -0.69 && x <= -0.61 && y > -0.277 && y < -0.193) ||
                    (x > -0.55 && x <= -0.5 && y > 0.47 && y < 0.51) ||
                    (x > -0.55 && x <= -0.5 && y > -0.51 && y < -0.47) ||
                    (x > 0.27 && x <= 0.31 && y > 0.004 && y < 0.026) ||
                    (x > 0.27 && x <= 0.31 && y > -0.026 && y < -0.004))
                    continue;

                // Test escape iteration
                int escapeIter = TestEscapeIteration(x, y, RED_MAX);

                // Filter: Only track if escaped in red range
                if (escapeIter >= RED_MIN && escapeIter <= RED_MAX)
                {
                    DrawPathForChannel(x, y, params, width, height, escapeIter, redCount);
                }
            }
        }

        Debug::WriteLine("  Nebulabrot three-pass sampling complete");
    }
    else
    {
        //=====================================================================
        // BUDDHABROT MODE: Single-pass with subtle color separation
        //=====================================================================

        int lastReportedPercent = -1;
        double y = params.centerY + params.viewHeight / 2.0;

        for (int source_row = SOURCE_ROWS - 1; source_row >= 0; source_row--, y -= y_jump)
    {
        // Report progress every 1%
        int currentPercent = (int)(((SOURCE_ROWS - source_row) * 100.0) / SOURCE_ROWS);
        if (currentPercent != lastReportedPercent && currentPercent % 1 == 0)
        {
            lastReportedPercent = currentPercent;

            // Fire progress event for UI updates
            if (engine != nullptr)
            {
                auto args = gcnew ProgressEventArgs();
                args->Percentage = currentPercent;
                args->CurrentLine = SOURCE_ROWS - source_row;
                args->TotalLines = SOURCE_ROWS;
                args->StatusMessage = String::Format("Sampling row {0} of {1} ({2}%)", 
                                                     SOURCE_ROWS - source_row, 
                                                     SOURCE_ROWS, 
                                                     currentPercent);
                engine->ProgressChanged(engine, args);
            }
        }

        double x = params.centerX - params.viewWidth / 2.0;
        for (int source_column = 0; source_column < SOURCE_COLUMNS; source_column++, x += x_jump)
        {
            // Optimization: Skip main Mandelbrot body (reduces wasted iterations)
            if ((x > -1.2 && x <= -1.1 && y > -0.1 && y < 0.1) ||
                (x > -1.1 && x <= -0.9 && y > -0.2 && y < 0.2) ||
                (x > -0.9 && x <= -0.8 && y > -0.1 && y < 0.1) ||
                (x > -0.69 && x <= -0.61 && y > -0.277 && y < -0.193) ||
                (x > -0.55 && x <= -0.5 && y > 0.47 && y < 0.51) ||
                (x > -0.55 && x <= -0.5 && y > -0.51 && y < -0.47) ||
                (x > 0.27 && x <= 0.31 && y > 0.004 && y < 0.026) ||
                (x > 0.27 && x <= 0.31 && y > -0.026 && y < -0.004))
                continue;

            // Test if point escapes
            double zr = 0.0, zi = 0.0;
            bool escapes = false;
            for (int n = 0; n <= threshold; n++)
            {
                double zr2 = zr * zr;
                double zi2 = zi * zi;
                if (zr2 + zi2 > 4.0) {
                    escapes = true;
                    break;
                }
                double temp = zr2 - zi2 + x;
                zi = 2.0 * zr * zi + y;
                zr = temp;
            }

            // If escapes, track path and accumulate histogram
            if (escapes)
            {
                DrawPath(x, y, params, width, height, threshold,
                    redCount, greenCount, blueCount);
            }
        }
    }

        Debug::WriteLine("  Buddhabrot single-pass sampling complete");
    }

    Debug::WriteLine("RenderBuddhabrotFractal: Converting histogram to pixels");

    // Convert histogram to pixel colors (mode-specific normalization)
    ConvertHistogramToPixels(result, width, height,
        redCount, greenCount, blueCount,
        brightness, palette, colorOffset, isNebulabrot);

    Debug::WriteLine("RenderBuddhabrotFractal: Rendering complete");
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
            result->EscapedPixelCount = 0;  // Not meaningful for histogram rendering
            result->Category = FractalCategory::HistogramBased;

            Debug::WriteLine(String::Format("Native Calculate: Histogram rendering complete in {0}ms", stopwatch->ElapsedMilliseconds));
            return result;
        }

        // Check if this fractal requires Buddhabrot rendering
        if (spec->type == ::Native::FractalCategory::BuddhabrotBased)
        {
            // Detect Nebulabrot variant by name
            bool isNebulabrot = (fractalType == "Nebulabrot");

            if (isNebulabrot)
            {
                Debug::WriteLine("Native Calculate: NEBULABROT-based fractal detected");
                Debug::WriteLine("  Calling RenderBuddhabrotFractal in NEBULABROT mode (3-pass RGB separation)");
                Debug::WriteLine("  WARNING: Nebulabrot requires 3× the computation time of standard Buddhabrot");
            }
            else
            {
                Debug::WriteLine("Native Calculate: BUDDHABROT-based fractal detected");
                Debug::WriteLine("  Calling RenderBuddhabrotFractal in BUDDHABROT mode (single-pass subtle colors)");
            }

            // Convert managed palette enum to native
            ::Native::PaletteType nativePalette = static_cast<::Native::PaletteType>((int)parameters->Palette);

            // Call Buddhabrot renderer with mode flag
            RenderBuddhabrotFractal(result, nativeParams, width, height, nativePalette, parameters->ColorOffset, this, isNebulabrot);

            // Set statistics and timing
            stopwatch->Stop();
            result->RenderTime = stopwatch->Elapsed;
            result->IterationCount = 0;  // Not applicable for Buddhabrot/Nebulabrot
            result->EscapedPixelCount = 0;  // Not meaningful for Buddhabrot/Nebulabrot
            result->Category = FractalCategory::BuddhabrotBased;

            Debug::WriteLine(String::Format("Native Calculate: {0} rendering complete in {1}ms", 
                                            isNebulabrot ? "Nebulabrot" : "Buddhabrot",
                                            stopwatch->ElapsedMilliseconds));
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

                    // Apply render mode transformations
                    int renderMode = parameters->RenderMode;
                    bool useSmooth = parameters->UseSmoothColoring;

                    // DIAGNOSTIC: Log render mode on first pixel
                    if (x == 0 && y == 0)
                    {
                        Debug::WriteLine(String::Format("RenderMode={0}, UseSmoothColoring={1}", renderMode, useSmooth));
                    }

                    // Render mode routing:
                    // 0 = EscapeTime (integer iterations)
                    // 1 = SmoothColoring (fractional iterations - already computed by registry)
                    // 2 = DistanceEstimation
                    // 3 = OrbitTrap

                    if (renderMode == 2)  // Distance Estimation
                    {
                        iteration = ::Native::MandelbrotCalculator::CalculateDistanceEstimation(
                            c,
                            nativeParams.maxIterations,
                            nativeParams.isJulia,
                            ::Native::ComplexD(nativeParams.juliaCX, nativeParams.juliaCY)
                        );
                    }
                    else if (renderMode == 3)  // Orbit Trap
                    {
                        iteration = ::Native::MandelbrotCalculator::CalculateOrbitTrap(
                            c,
                            nativeParams.maxIterations,
                            nativeParams.isJulia,
                            ::Native::ComplexD(nativeParams.juliaCX, nativeParams.juliaCY)
                        );
                    }
                    else if (renderMode == 0 && !useSmooth)  // EscapeTime with anti-banding disabled
                    {
                        // Truncate to integer for classic banding effect
                        iteration = floor(iteration);
                    }
                    // else renderMode == 1 (SmoothColoring) or useSmooth == true: use fractional iteration as-is

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
        result->Category = static_cast<FractalCategory>((int)spec->type);

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
