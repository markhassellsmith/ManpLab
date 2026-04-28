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

    //=========================================================================
    // Mandelbrot (Standard)
    //=========================================================================
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

    //=========================================================================
    // Julia - San Marco
    //=========================================================================
    spec.name = "JuliaSanMarco";
    spec.displayName = "Julia - San Marco";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Classic Julia set with c = -0.75 + 0.0i (San Marco dragon)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Force Julia mode with classic San Marco constant
        return MandelbrotCalculator::CalculateSmoothIterations(c, maxIter, true, ComplexD(-0.75, 0.0));
    };

    spec.supportsJulia = false;  // Pre-set Julia
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // Julia - Douady Rabbit
    //=========================================================================
    spec.name = "JuliaDouadyRabbit";
    spec.displayName = "Julia - Douady Rabbit";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Famous Julia set with c = -0.123 + 0.745i (Douady's rabbit)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        return MandelbrotCalculator::CalculateSmoothIterations(c, maxIter, true, ComplexD(-0.123, 0.745));
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // Julia - Siegel Disk
    //=========================================================================
    spec.name = "JuliaSiegelDisk";
    spec.displayName = "Julia - Siegel Disk";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set with c = -0.390541 - 0.586788i (Siegel disk)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        return MandelbrotCalculator::CalculateSmoothIterations(c, maxIter, true, ComplexD(-0.390541, -0.586788));
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);
}

} // namespace Native
