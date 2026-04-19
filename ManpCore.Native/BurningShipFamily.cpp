#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"

namespace Native {

//=============================================================================
// Burning Ship Family
// Formula: z = (|Re(z)| + i|Im(z)|)² + c
//=============================================================================

void RegisterBurningShipFamily()
{
    FractalSpec spec;

    spec.name = "BurningShip";
    spec.displayName = "Burning Ship";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Burning Ship fractal. Takes absolute values before squaring: z = (|Re(z)| + i|Im(z)|)² + c";

    // Use existing calculation function
    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        return MandelbrotCalculator::CalculateBurningShip(c, maxIter, isJulia, juliaC);
    };

    spec.supportsJulia = true;

    // Burning Ship has different default view (ship appears in bottom-left quadrant)
    spec.defaultCenterX = -0.5;
    spec.defaultCenterY = -0.5;
    spec.defaultZoom = 0.8;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;  // No symmetry due to absolute values

    spec.parameters = {};

    FractalRegistry::Register(spec);
}

} // namespace Native
