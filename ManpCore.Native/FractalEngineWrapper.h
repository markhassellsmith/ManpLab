#pragma once

using namespace System;
using namespace System::Runtime::InteropServices;

namespace ManpCore {
namespace Native {

    /// <summary>
    /// Color palette types for fractal rendering
    /// </summary>
    public enum class ColorPalette
    {
        Grayscale = 0,
        Classic = 1,
        Fire = 2,
        Ocean = 3,
        Rainbow = 4,
        Psychedelic = 5
    };

    /// <summary>
    /// High-precision floating point number for deep zoom rendering
    /// Wraps native SimpleBigDouble (Phase 2) or MPFR BigDouble (later phases)
    /// </summary>
    public ref class BigDouble
    {
    private:
        void* m_nativeBigDouble;  // Pointer to native SimpleBigDouble
        int m_precision;

    public:
        /// <summary>
        /// Create BigDouble from regular double
        /// </summary>
        BigDouble(double value);

        /// <summary>
        /// Create BigDouble with specified precision (decimal digits)
        /// </summary>
        BigDouble(double value, int precision);

        /// <summary>
        /// Copy constructor
        /// </summary>
        BigDouble(BigDouble^ other);

        /// <summary>
        /// Destructor
        /// </summary>
        ~BigDouble();

        /// <summary>
        /// Finalizer (cleanup native resources)
        /// </summary>
        !BigDouble();

        /// <summary>
        /// Convert to double (loses precision for very large/small numbers)
        /// </summary>
        double ToDouble();

        /// <summary>
        /// Get string representation with full precision
        /// </summary>
        String^ ToString() override;

        /// <summary>
        /// Parse BigDouble from string
        /// </summary>
        static BigDouble^ Parse(String^ str);

        /// <summary>
        /// Get/set precision in decimal digits
        /// </summary>
        property int Precision
        {
            int get() { return m_precision; }
            void set(int value) { m_precision = value; }
        }

        // Arithmetic operators
        static BigDouble^ operator+(BigDouble^ a, BigDouble^ b);
        static BigDouble^ operator-(BigDouble^ a, BigDouble^ b);
        static BigDouble^ operator*(BigDouble^ a, BigDouble^ b);
        static BigDouble^ operator/(BigDouble^ a, BigDouble^ b);
        static bool operator<(BigDouble^ a, BigDouble^ b);
        static bool operator>(BigDouble^ a, BigDouble^ b);
    };

    /// <summary>
    /// Managed wrapper for fractal calculation parameters
    /// </summary>
    public ref class FractalParameters
    {
    public:
        // Core fractal parameters
        property String^ FractalType;
        property int MaxIterations;
        property int Width;
        property int Height;

        // Complex plane coordinates
        property double CenterX;
        property double CenterY;
        property double ViewWidth;
        property double ViewHeight;

        // High-precision coordinates for deep zoom (optional - use when zoom > 10^15)
        property BigDouble^ BigCenterX;
        property BigDouble^ BigCenterY;
        property BigDouble^ BigViewWidth;
        property BigDouble^ BigViewHeight;

        // Julia set parameters (if applicable)
        property bool IsJuliaSet;
        property double JuliaCX;
        property double JuliaCY;

        // Color palette selection
        property ColorPalette Palette;

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
        }
    };

    /// <summary>
    /// Result of fractal calculation
    /// </summary>
    public ref class FractalResult
    {
    public:
        property array<Byte>^ PixelData;
        property int Width;
        property int Height;
        property TimeSpan RenderTime;
        property long long IterationCount;
    };

    /// <summary>
    /// Progress reporting for long-running calculations
    /// </summary>
    public ref class ProgressEventArgs : public EventArgs
    {
    public:
        property double Percentage;
        property int CurrentLine;
        property int TotalLines;
        property String^ StatusMessage;
    };

    /// <summary>
    /// C++/CLI wrapper for the native C++ fractal engine
    /// Provides managed interface for C# WinUI application
    /// </summary>
    public ref class FractalEngineWrapper
    {
    private:
        // Pointer to native C++ fractal engine (will be implemented)
        void* m_nativeEngine;
        bool m_cancelled;

    public:
        /// <summary>
        /// Progress event for UI updates
        /// </summary>
        event EventHandler<ProgressEventArgs^>^ ProgressChanged;

        FractalEngineWrapper();
        ~FractalEngineWrapper();
        !FractalEngineWrapper();

        /// <summary>
        /// Calculate fractal with given parameters
        /// </summary>
        /// <param name="parameters">Fractal calculation parameters</param>
        /// <returns>Result containing pixel data and metadata</returns>
        FractalResult^ Calculate(FractalParameters^ parameters);

        /// <summary>
        /// Cancel ongoing calculation
        /// </summary>
        void Cancel();

        /// <summary>
        /// Get list of available fractal types from native engine
        /// </summary>
        array<String^>^ GetAvailableFractalTypes();

        /// <summary>
        /// Run native C++ baseline benchmark (no C++/CLI overhead)
        /// Used to measure wrapper overhead
        /// </summary>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        /// <param name="maxIterations">Maximum iterations</param>
        /// <param name="runs">Number of benchmark runs</param>
        /// <returns>Average time in milliseconds</returns>
        double RunNativeBaselineBenchmark(int width, int height, int maxIterations, int runs);

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
