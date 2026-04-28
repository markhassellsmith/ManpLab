#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Multibrot Family
// Generalized Mandelbrot: z = z^n + c for various powers n
//=============================================================================

// Helper function for complex power calculation
static ComplexD ComplexPower(ComplexD z, int power)
{
    if (power == 2)
    {
        // Optimized for power 2
        return ComplexD(z.x * z.x - z.y * z.y, 2.0 * z.x * z.y);
    }

    // General power using polar form: (r^n) * (cos(n*θ) + i*sin(n*θ))
    double r = sqrt(z.x * z.x + z.y * z.y);
    double theta = atan2(z.y, z.x);

    double r_n = pow(r, power);
    double n_theta = power * theta;

    return ComplexD(r_n * cos(n_theta), r_n * sin(n_theta));
}

// Generalized Multibrot calculator
static double CalculateMultibrot(ComplexD c, int maxIter, int power, bool isJulia, ComplexD juliaC)
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
    double bailout = 256.0;

    while (iteration < maxIter)
    {
        double magnitude2 = z.x * z.x + z.y * z.y;

        if (magnitude2 > bailout)
        {
            // Smooth coloring
            double log_zn = log(magnitude2) / 2.0;
            double nu = log(log_zn / log(2.0)) / log(2.0);
            return iteration + 1.0 - nu;
        }

        // z = z^power + c
        ComplexD z_powered = ComplexPower(z, power);
        z.x = z_powered.x + constant.x;
        z.y = z_powered.y + constant.y;

        iteration++;
    }

    return (double)maxIter;
}

void RegisterMultibrotFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Multibrot 3 (Cubic Mandelbrot)
    //=========================================================================
    spec.name = "Multibrot3";
    spec.displayName = "Multibrot³ (Cubic)";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Cubic Mandelbrot set. Iteration formula: z = z³ + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        return CalculateMultibrot(c, maxIter, 3, isJulia, juliaC);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.8;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;  // 3-fold rotational symmetry
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // Multibrot 4 (Quartic Mandelbrot)
    //=========================================================================
    spec.name = "Multibrot4";
    spec.displayName = "Multibrot⁴ (Quartic)";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Quartic Mandelbrot set. Iteration formula: z = z⁴ + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        return CalculateMultibrot(c, maxIter, 4, isJulia, juliaC);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.7;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;  // 4-fold rotational symmetry
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // Multibrot 5 (Quintic Mandelbrot)
    //=========================================================================
    spec.name = "Multibrot5";
    spec.displayName = "Multibrot⁵ (Quintic)";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Quintic Mandelbrot set. Iteration formula: z = z⁵ + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        return CalculateMultibrot(c, maxIter, 5, isJulia, juliaC);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.65;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;  // 5-fold rotational symmetry
    spec.parameters = {};

    FractalRegistry::Register(spec);
}

} // namespace Native
