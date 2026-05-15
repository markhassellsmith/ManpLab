#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Magnet Fractal Family
// Based on rational functions from theoretical physics
//=============================================================================

// Magnet I: ((z² + c - 1) / (2z + c - 2))²
static double CalculateMagnet1(ComplexD c, int maxIter, bool isJulia, ComplexD juliaC)
{
    ComplexD z;
    ComplexD constant;

    if (isJulia)
    {
        z = c;
        constant = juliaC;
    }
    else
    {
        z = ComplexD(0.0, 0.0);
        constant = c;
    }

    int iteration = 0;
    const double bailout = 1000.0;

    while (iteration < maxIter)
    {
        // Calculate z²
        double z2_x = z.x * z.x - z.y * z.y;
        double z2_y = 2.0 * z.x * z.y;

        // Numerator: z² + c - 1
        double numer_x = z2_x + constant.x - 1.0;
        double numer_y = z2_y + constant.y;

        // Denominator: 2z + c - 2
        double denom_x = 2.0 * z.x + constant.x - 2.0;
        double denom_y = 2.0 * z.y + constant.y;

        // Check for division by zero
        double denom_mag2 = denom_x * denom_x + denom_y * denom_y;
        if (denom_mag2 < 1e-10)
            break;

        // Complex division: (a + bi) / (c + di)
        double div_x = (numer_x * denom_x + numer_y * denom_y) / denom_mag2;
        double div_y = (numer_y * denom_x - numer_x * denom_y) / denom_mag2;

        // Square the result: ((z² + c - 1) / (2z + c - 2))²
        z.x = div_x * div_x - div_y * div_y;
        z.y = 2.0 * div_x * div_y;

        // Bailout test
        double magnitude2 = z.x * z.x + z.y * z.y;
        if (magnitude2 > bailout)
        {
            double log_zn = log(magnitude2) / 2.0;
            double nu = log(log_zn / log(2.0)) / log(2.0);
            return iteration + 1.0 - nu;
        }

        iteration++;
    }

    return (double)maxIter;
}

// Magnet II: ((z³ + 3(c-1)z + (c-1)(c-2)) / (3z² + 3(c-2)z + (c-1)(c-2) + 1))²
static double CalculateMagnet2(ComplexD c, int maxIter, bool isJulia, ComplexD juliaC)
{
    ComplexD z;
    ComplexD constant;

    if (isJulia)
    {
        z = c;
        constant = juliaC;
    }
    else
    {
        z = ComplexD(0.0, 0.0);
        constant = c;
    }

    int iteration = 0;
    const double bailout = 1000.0;

    while (iteration < maxIter)
    {
        // Pre-calculate common terms
        // (c - 1)
        double c_minus_1_x = constant.x - 1.0;
        double c_minus_1_y = constant.y;

        // (c - 2)
        double c_minus_2_x = constant.x - 2.0;
        double c_minus_2_y = constant.y;

        // (c - 1)(c - 2) = (c-1.x * c-2.x - c-1.y * c-2.y, c-1.x * c-2.y + c-1.y * c-2.x)
        double c1c2_x = c_minus_1_x * c_minus_2_x - c_minus_1_y * c_minus_2_y;
        double c1c2_y = c_minus_1_x * c_minus_2_y + c_minus_1_y * c_minus_2_x;

        // Calculate z²
        double z2_x = z.x * z.x - z.y * z.y;
        double z2_y = 2.0 * z.x * z.y;

        // Calculate z³
        double z3_x = z.x * z2_x - z.y * z2_y;
        double z3_y = z.x * z2_y + z.y * z2_x;

        // Numerator: z³ + 3(c-1)z + (c-1)(c-2)
        double numer_x = z3_x + 3.0 * (c_minus_1_x * z.x - c_minus_1_y * z.y) + c1c2_x;
        double numer_y = z3_y + 3.0 * (c_minus_1_x * z.y + c_minus_1_y * z.x) + c1c2_y;

        // Denominator: 3z² + 3(c-2)z + (c-1)(c-2) + 1
        double denom_x = 3.0 * z2_x + 3.0 * (c_minus_2_x * z.x - c_minus_2_y * z.y) + c1c2_x + 1.0;
        double denom_y = 3.0 * z2_y + 3.0 * (c_minus_2_x * z.y + c_minus_2_y * z.x) + c1c2_y;

        // Check for division by zero
        double denom_mag2 = denom_x * denom_x + denom_y * denom_y;
        if (denom_mag2 < 1e-10)
            break;

        // Complex division
        double div_x = (numer_x * denom_x + numer_y * denom_y) / denom_mag2;
        double div_y = (numer_y * denom_x - numer_x * denom_y) / denom_mag2;

        // Square the result
        z.x = div_x * div_x - div_y * div_y;
        z.y = 2.0 * div_x * div_y;

        // Bailout test
        double magnitude2 = z.x * z.x + z.y * z.y;
        if (magnitude2 > bailout)
        {
            double log_zn = log(magnitude2) / 2.0;
            double nu = log(log_zn / log(2.0)) / log(2.0);
            return iteration + 1.0 - nu;
        }

        iteration++;
    }

    return (double)maxIter;
}

void RegisterMagnetFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Magnet I Fractal
    //=========================================================================
    spec.name = "Magnet1";
    spec.displayName = "Magnet I";
    spec.category = "Exotic Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Magnet I fractal based on rational function: ((z² + c - 1) / (2z + c - 2))²";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        return CalculateMagnet1(c, maxIter, isJulia, juliaC);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = -0.5;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.532567;  // Viewport tuning: X scale 2.61, Y scale 1.47
    spec.defaultBailout = 1000.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // Magnet II Fractal
    //=========================================================================
    spec.name = "Magnet2";
    spec.displayName = "Magnet II";
    spec.category = "Exotic Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Magnet II fractal with cubic rational function (more complex than Magnet I)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        return CalculateMagnet2(c, maxIter, isJulia, juliaC);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 1.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.5;
    spec.defaultBailout = 1000.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);
}

} // namespace Native
