#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Newton Fractal Family
// Newton-Raphson method for finding roots of complex polynomials
//=============================================================================

// Newton's method for z³ - 1 = 0
static double CalculateNewton(ComplexD c, int maxIter, bool isJulia, ComplexD juliaC)
{
    ComplexD z = c;  // Start at pixel position
    const double tolerance = 0.0001;
    const double bailout = 100.0;

    int iteration = 0;

    // Three roots of z³ - 1 = 0
    const ComplexD root1(1.0, 0.0);                           // 1
    const ComplexD root2(-0.5, 0.866025403784439);             // e^(2πi/3)
    const ComplexD root3(-0.5, -0.866025403784439);            // e^(4πi/3)

    while (iteration < maxIter)
    {
        // Check if we've converged to a root
        double dist1 = sqrt((z.x - root1.x) * (z.x - root1.x) + (z.y - root1.y) * (z.y - root1.y));
        double dist2 = sqrt((z.x - root2.x) * (z.x - root2.x) + (z.y - root2.y) * (z.y - root2.y));
        double dist3 = sqrt((z.x - root3.x) * (z.x - root3.x) + (z.y - root3.y) * (z.y - root3.y));

        if (dist1 < tolerance || dist2 < tolerance || dist3 < tolerance)
        {
            // Converged - return smooth iteration based on distance to root
            double minDist = fmin(dist1, fmin(dist2, dist3));
            return iteration + 1.0 - log(minDist / tolerance) / log(2.0);
        }

        // Newton iteration: z_new = z - f(z)/f'(z)
        // For f(z) = z³ - 1:
        // f'(z) = 3z²
        // z_new = z - (z³ - 1) / (3z²) = (2z³ + 1) / (3z²)

        // Calculate z²
        double z2_x = z.x * z.x - z.y * z.y;
        double z2_y = 2.0 * z.x * z.y;

        // Calculate z³
        double z3_x = z.x * z2_x - z.y * z2_y;
        double z3_y = z.x * z2_y + z.y * z2_x;

        // Calculate 3z²
        double denom_x = 3.0 * z2_x;
        double denom_y = 3.0 * z2_y;

        // Avoid division by zero
        double denom_mag2 = denom_x * denom_x + denom_y * denom_y;
        if (denom_mag2 < 1e-10)
            break;

        // Calculate (2z³ + 1) / (3z²)
        double numer_x = 2.0 * z3_x + 1.0;
        double numer_y = 2.0 * z3_y;

        // Complex division: (a + bi) / (c + di) = ((ac + bd) + (bc - ad)i) / (c² + d²)
        z.x = (numer_x * denom_x + numer_y * denom_y) / denom_mag2;
        z.y = (numer_y * denom_x - numer_x * denom_y) / denom_mag2;

        // Bailout test
        double magnitude = sqrt(z.x * z.x + z.y * z.y);
        if (magnitude > bailout)
            return (double)maxIter;

        iteration++;
    }

    return (double)maxIter;
}

// Nova fractal: Newton + Mandelbrot hybrid
// z_new = z - (z³ - 1)/(3z²) + c
static double CalculateNova(ComplexD c, int maxIter, bool isJulia, ComplexD juliaC)
{
    ComplexD z = ComplexD(1.0, 0.0);  // Start at (1,0) for Nova
    ComplexD constant = isJulia ? juliaC : c;
    const double bailout = 256.0;

    int iteration = 0;

    while (iteration < maxIter)
    {
        // Calculate z²
        double z2_x = z.x * z.x - z.y * z.y;
        double z2_y = 2.0 * z.x * z.y;

        // Calculate z³
        double z3_x = z.x * z2_x - z.y * z2_y;
        double z3_y = z.x * z2_y + z.y * z2_x;

        // Calculate 3z²
        double denom_x = 3.0 * z2_x;
        double denom_y = 3.0 * z2_y;

        double denom_mag2 = denom_x * denom_x + denom_y * denom_y;
        if (denom_mag2 < 1e-10)
            break;

        // Calculate (z³ - 1)
        double numer_x = z3_x - 1.0;
        double numer_y = z3_y;

        // Complex division: (z³ - 1) / (3z²)
        double div_x = (numer_x * denom_x + numer_y * denom_y) / denom_mag2;
        double div_y = (numer_y * denom_x - numer_x * denom_y) / denom_mag2;

        // Newton step + Mandelbrot: z_new = z - (z³ - 1)/(3z²) + c
        z.x = z.x - div_x + constant.x;
        z.y = z.y - div_y + constant.y;

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

void RegisterNewtonFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Newton Fractal (z³ - 1 = 0)
    //=========================================================================
    spec.name = "Newton";
    spec.displayName = "Newton (z³-1)";
    spec.category = "Newton's Method";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Newton's method for finding roots of z³ - 1 = 0. Colors show convergence basins.";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        return CalculateNewton(c, maxIter, isJulia, juliaC);
    };

    spec.supportsJulia = false;  // Newton fractals don't use Julia mode
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.5;
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = true;  // 3-fold rotational symmetry
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // Nova Fractal (Newton + Mandelbrot hybrid)
    //=========================================================================
    spec.name = "Nova";
    spec.displayName = "Nova";
    spec.category = "Newton's Method";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Hybrid of Newton's method and Mandelbrot: z = z - (z³-1)/(3z²) + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        return CalculateNova(c, maxIter, isJulia, juliaC);
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
