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
