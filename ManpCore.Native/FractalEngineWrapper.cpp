#include "FractalEngineWrapper.h"
#include <msclr/marshal_cppstd.h>

using namespace System;
using namespace System::Diagnostics;
using namespace ManpCore::Native;

// Constructor
FractalEngineWrapper::FractalEngineWrapper()
{
    m_nativeEngine = nullptr;
    m_cancelled = false;
    
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
        // TODO Phase 2: Call native C++ calculation
        // For now, return dummy result for testing
        
        int width = parameters->Width;
        int height = parameters->Height;
        int pixelCount = width * height * 4; // RGBA

        auto result = gcnew FractalResult();
        result->Width = width;
        result->Height = height;
        result->PixelData = gcnew array<Byte>(pixelCount);
        
        // Fill with test pattern (gradient)
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
                int index = (y * width + x) * 4;
                
                // Simple gradient test pattern
                result->PixelData[index + 0] = (Byte)(x * 255 / width);      // R
                result->PixelData[index + 1] = (Byte)(y * 255 / height);     // G
                result->PixelData[index + 2] = 128;                          // B
                result->PixelData[index + 3] = 255;                          // A
            }
        }

        stopwatch->Stop();
        result->RenderTime = stopwatch->Elapsed;
        result->IterationCount = (long long)width * height * parameters->MaxIterations;

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
