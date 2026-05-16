#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Special & Exotic Fractals Family
// Unique and unusual fractals from ManpWIN64
// Includes: Hailstone, Buddhabrot, Lyapunov, Cellular, etc.
//=============================================================================

void RegisterSpecialExoticFamily()
{
    FractalSpec spec;

    //=========================================================================
    // HAILSTONE (245) - 2D Hailstone sequence with cycle detection
    //=========================================================================
    spec.name = "Hailstone";
    spec.displayName = "Hailstone Sequence";
    spec.category = "Special";
    spec.type = FractalCategory::Sequence2D;
    spec.description = "2D visualization of Collatz (3n+1) sequence with cycle detection";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Hailstone (Collatz) sequence: n → n/2 (even) or 3n+1 (odd)
        // Visualize using real part as starting value
        long long n = static_cast<long long>(fabs(c.real * 1000.0));
        if (n < 1) n = 1;

        int steps = 0;
        while (n != 1 && steps < maxIter) {
            if (n % 2 == 0) {
                n = n / 2;
            } else {
                n = 3 * n + 1;
            }
            steps++;
            if (n > 1000000000LL) break;  // Prevent overflow
        }

        return static_cast<double>(steps);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 500.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.01;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // HAILSTONE2D - 2D Trajectory Visualization with Axes and Labels
    //=========================================================================
    spec.name = "Hailstone2D";
    spec.displayName = "2-D Hailstone Trajectory";
    spec.category = "Special";
    spec.type = FractalCategory::Sequence2D;
    spec.description = "Interactive 2D visualization of Collatz sequence trajectory with coordinate axes, grid, point labels, and path rendering on black background";

    // This is a marker entry - actual rendering uses HailstoneRenderService
    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // This calculator won't be used - HailstoneRenderService handles the rendering
        // But we need a valid calculator for registry compliance
        return 0.0;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 27.0;   // Classic starting point
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 1000.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // NUMFRACTAL (244) - Fractal dedicated to an 11-year-old discoverer
    //=========================================================================
    spec.name = "NumFractal";
    spec.displayName = "NumFractal";
    spec.category = "Special";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Unique fractal dedicated to an 11-year-old discoverer";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Placeholder implementation - unique iteration formula
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            // z = z³ + c (cubic variant)
            double x2 = z.real * z.real;
            double y2 = z.imag * z.imag;
            double x3 = x2 * z.real - 3.0 * z.real * y2;
            double y3 = 3.0 * x2 * z.imag - y2 * z.imag;

            z = ComplexD(x3 + constant.real, y3 + constant.imag);

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // BUDDHABROT (229) - Buddhabrot rendering technique
    //=========================================================================
    spec.name = "Buddhabrot";
    spec.displayName = "Buddhabrot";
    spec.category = "Special";
    spec.type = FractalCategory::Special;
    spec.description = "Mandelbrot set rendered by tracking escape paths";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // For now, render as standard Mandelbrot (full Buddhabrot needs accumulation buffer)
        return MandelbrotCalculator::CalculateSmoothIterations(c, maxIter, false, ComplexD(0, 0));
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = -0.5;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // LYAPUNOV (123) - Lyapunov fractal based on population dynamics
    //=========================================================================
    spec.name = "Lyapunov";
    spec.displayName = "Lyapunov";
    spec.category = "Special";
    spec.type = FractalCategory::Sequence2D;
    spec.description = "Lyapunov exponent fractal from population dynamics";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Lyapunov: iterate x = r*x*(1-x) with alternating r values
        double a = fabs(c.real);
        double b = fabs(c.imag);
        if (a < 0.1) a = 0.1;
        if (b < 0.1) b = 0.1;
        if (a > 4.0) a = 4.0;
        if (b > 4.0) b = 4.0;

        double x = 0.5;
        double sum = 0.0;

        for (int iter = 0; iter < maxIter; ++iter) {
            double r = (iter % 2 == 0) ? a : b;
            x = r * x * (1.0 - x);
            if (x > 0.0 && x < 1.0) {
                sum += log(fabs(r * (1.0 - 2.0 * x)));
            }
        }

        double lyapunov = sum / maxIter;
        return lyapunov * 50.0 + 128.0;  // Scale for coloring
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 2.5;
    spec.defaultCenterY = 2.5;
    spec.defaultZoom = 0.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // MANDELBAR (231) - Mandelbar (Tricorn without conjugate in z²)
    //=========================================================================
    spec.name = "MandelbarExotic";
    spec.displayName = "Mandelbar";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbar fractal: z = conj(z)² + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            // Mandelbar: conjugate before squaring
            z = ComplexD(z.real, -z.imag);
            z = ComplexD(z.real * z.real - z.imag * z.imag + constant.real,
                        2.0 * z.real * z.imag + constant.imag);

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // THORN (227) - Thorn fractal (classic variant)
    //=========================================================================
    spec.name = "ThornClassic";
    spec.displayName = "Thorn (Classic)";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Thorn fractal: z = z²/c + c";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0, 0);
        ComplexD constant = isJulia ? juliaC : c;
        double denom = constant.real * constant.real + constant.imag * constant.imag;
        if (denom < 1e-10) denom = 1e-10;

        for (int iter = 0; iter < maxIter; ++iter) {
            // z = z²/c + c
            double z_sq_real = z.real * z.real - z.imag * z.imag;
            double z_sq_imag = 2.0 * z.real * z.imag;

            z = ComplexD((z_sq_real * constant.real + z_sq_imag * constant.imag) / denom + constant.real,
                        (z_sq_imag * constant.real - z_sq_real * constant.imag) / denom + constant.imag);

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // TETRATION (236) - Infinite tower: z^z^z^...
    //=========================================================================
    spec.name = "TetrationClassic";
    spec.displayName = "Tetration (Classic)";
    spec.category = "Special";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Infinite power tower: z^z^z^z...";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(1, 0);  // Start with z = 1
        ComplexD constant = isJulia ? juliaC : c;

        for (int iter = 0; iter < maxIter; ++iter) {
            // z = c^z (tetration approximation)
            double r = sqrt(constant.real * constant.real + constant.imag * constant.imag);
            if (r < 1e-10) break;
            double theta = atan2(constant.imag, constant.real);
            double ln_r = log(r);

            double re_exp = z.real * ln_r - z.imag * theta;
            double im_exp = z.real * theta + z.imag * ln_r;
            double exp_re = exp(re_exp);

            z = ComplexD(exp_re * cos(im_exp), exp_re * sin(im_exp));

            double modulus = z.real * z.real + z.imag * z.imag;
            if (modulus > 256.0)
                return iter + 1.0 - log(log(modulus)) / log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);
}

} // namespace Native
