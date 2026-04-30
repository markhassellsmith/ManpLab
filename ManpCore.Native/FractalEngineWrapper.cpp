#include "FractalEngineWrapper.h"
#include "MandelbrotCalculator.h"
#include "FractalRegistry.h"
#include "NativePerformanceBaseline.h"
#include "BigDoubleMarshaller.h"
#include "Complex.h"  // ManpWIN64 Complex class for POC
#include <string>

using namespace System;
using namespace System::Diagnostics;
using namespace System::Runtime::InteropServices;
using namespace ManpCore::Native;

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
// BigDouble Implementation
//=============================================================================

// Constructor from double
BigDouble::BigDouble(double value)
{
    m_nativeBigDouble = new ::Native::SimpleBigDouble(value);
    m_precision = 16;
}

// Constructor with precision
BigDouble::BigDouble(double value, int precision)
{
    m_nativeBigDouble = new ::Native::SimpleBigDouble(value, precision);
    m_precision = precision;
}

// Copy constructor
BigDouble::BigDouble(BigDouble^ other)
{
    if (other == nullptr)
        throw gcnew ArgumentNullException("other");

    auto nativeOther = static_cast<::Native::SimpleBigDouble*>(other->m_nativeBigDouble);
    m_nativeBigDouble = new ::Native::SimpleBigDouble(*nativeOther);
    m_precision = other->m_precision;
}

// Destructor
BigDouble::~BigDouble()
{
    this->!BigDouble();
}

// Finalizer
BigDouble::!BigDouble()
{
    if (m_nativeBigDouble != nullptr)
    {
        delete static_cast<::Native::SimpleBigDouble*>(m_nativeBigDouble);
        m_nativeBigDouble = nullptr;
    }
}

// Convert to double
double BigDouble::ToDouble()
{
    auto native = static_cast<::Native::SimpleBigDouble*>(m_nativeBigDouble);
    return native->ToDouble();
}

// Convert to string
String^ BigDouble::ToString()
{
    auto native = static_cast<::Native::SimpleBigDouble*>(m_nativeBigDouble);
    std::string str = native->ToString();
    return gcnew String(str.c_str());
}

// Parse from string
BigDouble^ BigDouble::Parse(String^ str)
{
    if (String::IsNullOrEmpty(str))
        throw gcnew ArgumentNullException("str");

    std::string nativeStr = ManagedToStdString(str);

    auto native = ::Native::SimpleBigDouble::FromString(nativeStr);
    return gcnew BigDouble(native.value, native.precision);
}

// Arithmetic operators
BigDouble^ BigDouble::operator+(BigDouble^ a, BigDouble^ b)
{
    if (a == nullptr || b == nullptr)
        throw gcnew ArgumentNullException();

    auto nativeA = static_cast<::Native::SimpleBigDouble*>(a->m_nativeBigDouble);
    auto nativeB = static_cast<::Native::SimpleBigDouble*>(b->m_nativeBigDouble);

    auto result = (*nativeA) + (*nativeB);
    return gcnew BigDouble(result.value, result.precision);
}

BigDouble^ BigDouble::operator-(BigDouble^ a, BigDouble^ b)
{
    if (a == nullptr || b == nullptr)
        throw gcnew ArgumentNullException();

    auto nativeA = static_cast<::Native::SimpleBigDouble*>(a->m_nativeBigDouble);
    auto nativeB = static_cast<::Native::SimpleBigDouble*>(b->m_nativeBigDouble);

    auto result = (*nativeA) - (*nativeB);
    return gcnew BigDouble(result.value, result.precision);
}

BigDouble^ BigDouble::operator*(BigDouble^ a, BigDouble^ b)
{
    if (a == nullptr || b == nullptr)
        throw gcnew ArgumentNullException();

    auto nativeA = static_cast<::Native::SimpleBigDouble*>(a->m_nativeBigDouble);
    auto nativeB = static_cast<::Native::SimpleBigDouble*>(b->m_nativeBigDouble);

    auto result = (*nativeA) * (*nativeB);
    return gcnew BigDouble(result.value, result.precision);
}

BigDouble^ BigDouble::operator/(BigDouble^ a, BigDouble^ b)
{
    if (a == nullptr || b == nullptr)
        throw gcnew ArgumentNullException();

    auto nativeA = static_cast<::Native::SimpleBigDouble*>(a->m_nativeBigDouble);
    auto nativeB = static_cast<::Native::SimpleBigDouble*>(b->m_nativeBigDouble);

    auto result = (*nativeA) / (*nativeB);
    return gcnew BigDouble(result.value, result.precision);
}

bool BigDouble::operator<(BigDouble^ a, BigDouble^ b)
{
    if (a == nullptr || b == nullptr)
        throw gcnew ArgumentNullException();

    auto nativeA = static_cast<::Native::SimpleBigDouble*>(a->m_nativeBigDouble);
    auto nativeB = static_cast<::Native::SimpleBigDouble*>(b->m_nativeBigDouble);

    return (*nativeA) < (*nativeB);
}

bool BigDouble::operator>(BigDouble^ a, BigDouble^ b)
{
    if (a == nullptr || b == nullptr)
        throw gcnew ArgumentNullException();

    auto nativeA = static_cast<::Native::SimpleBigDouble*>(a->m_nativeBigDouble);
    auto nativeB = static_cast<::Native::SimpleBigDouble*>(b->m_nativeBigDouble);

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
}

// Calculate fractal
FractalResult^ FractalEngineWrapper::Calculate(FractalParameters^ parameters)
{
    if (parameters == nullptr)
        throw gcnew ArgumentNullException("parameters");

    m_cancelled = false;
    auto stopwatch = Stopwatch::StartNew();

    try
    {
        int width = parameters->Width;
        int height = parameters->Height;
        int pixelCount = width * height * 4; // RGBA

        auto result = gcnew FractalResult();
        result->Width = width;
        result->Height = height;
        result->PixelData = gcnew array<Byte>(pixelCount);

        // Setup native Mandelbrot parameters
        ::Native::MandelbrotParams nativeParams;
        nativeParams.width = width;
        nativeParams.height = height;
        nativeParams.maxIterations = parameters->MaxIterations;
        nativeParams.centerX = parameters->CenterX;
        nativeParams.centerY = parameters->CenterY;
        nativeParams.viewWidth = parameters->ViewWidth;
        nativeParams.viewHeight = parameters->ViewHeight;
        nativeParams.isJulia = parameters->IsJuliaSet;
        nativeParams.juliaCX = parameters->JuliaCX;
        nativeParams.juliaCY = parameters->JuliaCY;

        // Convert managed palette enum to native palette enum
        ::Native::PaletteType nativePalette = static_cast<::Native::PaletteType>((int)parameters->Palette);

        // Extract color offset for palette rotation
        int colorOffset = parameters->ColorOffset;

        // Convert fractal type string and get calculator from registry
        std::string fractalType = ManagedToStdString(parameters->FractalType);

        // Initialize registry if not already done
        static bool registryInitialized = false;
        if (!registryInitialized)
        {
            ::Native::FractalRegistry::InitializeBuiltins();
            registryInitialized = true;
        }

        // Get calculator from registry
        auto calculator = ::Native::FractalRegistry::GetCalculator(fractalType);

        // Fallback to Mandelbrot if type not found
        if (!calculator)
        {
            fractalType = "Mandelbrot";
            calculator = ::Native::FractalRegistry::GetCalculator(fractalType);
        }

        // Prepare parameter map for extensibility (currently empty, but ready for custom params)
        ::Native::ParamMap customParams;

        long long totalIterations = 0;
        int escapedPixels = 0;

        // Calculate Mandelbrot set using native C++ code
        for (int y = 0; y < height; y++)
        {
            // Report progress every 10 lines
            if (y % 10 == 0)
            {
                auto progressArgs = gcnew ProgressEventArgs();
                progressArgs->Percentage = (y * 100.0) / height;
                progressArgs->CurrentLine = y;
                progressArgs->TotalLines = height;
                progressArgs->StatusMessage = String::Format("Calculating line {0} of {1}", y, height);
                OnProgressChanged(progressArgs);
            }

            if (m_cancelled)
                throw gcnew OperationCanceledException("Calculation cancelled by user");

            for (int x = 0; x < width; x++)
            {
                // Map pixel to complex plane
                ::Native::ComplexD c = ::Native::MandelbrotCalculator::PixelToComplex(x, y, nativeParams);

                // Calculate using registry dispatcher - single line replaces entire if-else chain!
                double iteration = calculator(
                    c, 
                    nativeParams.maxIterations,
                    nativeParams.isJulia,
                    ::Native::ComplexD(nativeParams.juliaCX, nativeParams.juliaCY),
                    customParams
                );

                totalIterations += (long long)iteration;

                // Track if pixel escaped (for diagnostics)
                if (iteration < nativeParams.maxIterations)
                {
                    escapedPixels++;
                }

                // Convert iteration to color using selected palette with color offset
                ::Native::ColorRGB color = ::Native::MandelbrotCalculator::IterationToColor(
                    iteration, 
                    nativeParams.maxIterations, 
                    nativePalette,
                    colorOffset
                );

                // Write BGRA pixel (WinUI WriteableBitmap format)
                int index = (y * width + x) * 4;
                result->PixelData[index + 0] = color.b;  // Blue
                result->PixelData[index + 1] = color.g;  // Green
                result->PixelData[index + 2] = color.r;  // Red
                result->PixelData[index + 3] = 255;      // Alpha (full opacity)
            }
        }

        stopwatch->Stop();
        result->RenderTime = stopwatch->Elapsed;
        result->IterationCount = totalIterations;
        result->EscapedPixelCount = escapedPixels;

        // Final progress update
        auto finalProgress = gcnew ProgressEventArgs();
        finalProgress->Percentage = 100.0;
        finalProgress->CurrentLine = height;
        finalProgress->TotalLines = height;
        finalProgress->StatusMessage = "Complete";
        OnProgressChanged(finalProgress);

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
    // TODO Phase 2: Query native engine for available types
    // For now, return test list
    
    array<String^>^ types = gcnew array<String^>(5);
    types[0] = "Mandelbrot";
    types[1] = "Julia";
    types[2] = "Burning Ship";
    types[3] = "Newton";
    types[4] = "Lyapunov";

    return types;
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
