#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"

namespace Native {

//=============================================================================
// Tricorn (Mandelbar) Family
// Formula: z = conj(z)² + c
//=============================================================================

void RegisterTricornFamily()
{
    FractalSpec spec;

    spec.name = "Tricorn";
    spec.displayName = "Tricorn (Mandelbar)";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Tricorn (Mandelbar) fractal. Conjugates z before squaring: z = conj(z)² + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        return MandelbrotCalculator::CalculateTricorn(c, maxIter, isJulia, juliaC);
    };

    spec.supportsJulia = true;

    spec.defaultCenterX = -0.25;
    spec.defaultCenterY = 0.04;
    spec.defaultZoom = 0.574713;  // Viewport tuning: X scale 6.96
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;  // Y-axis symmetry

    spec.parameters = {};

    FractalRegistry::Register(spec);
}

} // namespace Native
