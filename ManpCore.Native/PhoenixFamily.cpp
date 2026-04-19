#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"

namespace Native {

//=============================================================================
// Phoenix Family
// Formula: z = z² + Re(c) + Im(c) * previous_z
//=============================================================================

void RegisterPhoenixFamily()
{
    FractalSpec spec;

    spec.name = "Phoenix";
    spec.displayName = "Phoenix Fractal";
    spec.category = "Exotic Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Phoenix fractal with memory of previous iteration: z = z² + Re(c) + Im(c) * p, where p is previous z";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        return MandelbrotCalculator::CalculatePhoenix(c, maxIter, isJulia, juliaC);
    };

    spec.supportsJulia = true;

    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.6;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    spec.parameters = {};

    FractalRegistry::Register(spec);
}

} // namespace Native
