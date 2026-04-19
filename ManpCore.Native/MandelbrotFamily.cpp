#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"

namespace Native {

//=============================================================================
// Mandelbrot Family
// Standard Mandelbrot set: z = z² + c
//=============================================================================

void RegisterMandelbrotFamily()
{
    FractalSpec spec;

    // Mandelbrot (Standard)
    spec.name = "Mandelbrot";
    spec.displayName = "Mandelbrot Set";
    spec.category = "Classic Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "The classic Mandelbrot set. Iteration formula: z = z² + c";

    // Use existing calculation function from MandelbrotCalculator.h
    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        return MandelbrotCalculator::CalculateSmoothIterations(c, maxIter, isJulia, juliaC);
    };

    // Mandelbrot supports Julia mode
    spec.supportsJulia = true;

    // Default view settings
    spec.defaultCenterX = -0.5;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;  // X-axis symmetry

    // No custom parameters (uses standard Julia c_x, c_y when in Julia mode)
    spec.parameters = {};

    FractalRegistry::Register(spec);
}

} // namespace Native
