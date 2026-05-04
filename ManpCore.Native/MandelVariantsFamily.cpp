#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Mandelbrot Variants Family
// Additional Mandelbrot variations including higher powers and hybrids
//=============================================================================

void RegisterMandelVariantsFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Mandel4 (z⁴ + c)
    //=========================================================================
    spec.name = "Mandel4";
    spec.displayName = "Mandelbrot Power 4";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot set with quartic power: z⁴ + c";
    spec.formula = "z = z⁴ + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^4 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            ComplexD z2 = z * z;
            ComplexD z4 = z2 * z2;
            z = z4 + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.8;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Julia4 (z⁴ + c) - Julia set preset
    //=========================================================================
    spec.name = "Julia4";
    spec.displayName = "Julia Power 4";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set for z⁴ + c with preset constant";
    spec.formula = "z = z⁴ + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^4 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        ComplexD constant(-0.2, 0.6);  // Nice Julia4 parameter
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            ComplexD z2 = z * z;
            ComplexD z4 = z2 * z2;
            z = z4 + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.8;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // MandelLambda (z² + λ*z*(1-z))
    //=========================================================================
    spec.name = "MandelLambda";
    spec.displayName = "Mandelbrot-Lambda";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Hybrid of Mandelbrot and Lambda: z² + c*z*(1-z)";
    spec.formula = "z = z² + c*z*(1-z)";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c \cdot z_n(1-z_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            ComplexD z2 = z * z;
            ComplexD lambda_term = constant * z * (ComplexD(1.0, 0.0) - z);
            z = z2 + lambda_term;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Marks Mandelbrot (variant with different starting point)
    //=========================================================================
    spec.name = "MarksMandel";
    spec.displayName = "Marks Mandelbrot";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot variant with starting point z = c instead of z = 0";
    spec.formula = "z = z² + c, starting with z₀ = c";
    spec.formulaLatex = R"(z_0 = c, \; z_{n+1} = z_n^2 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD constant = isJulia ? juliaC : c;
        ComplexD z = constant;  // Start at c instead of 0
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            z = z * z + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Marks Julia (Julia variant with different starting point)
    //=========================================================================
    spec.name = "MarksJulia";
    spec.displayName = "Marks Julia";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia variant with modified starting conditions";
    spec.formula = "z = z² + c, with special initialization";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        ComplexD constant(-0.74543, 0.11301);  // Classic Julia parameter
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            z = z * z + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Mandelbar (conjugate Mandelbrot)
    //=========================================================================
    spec.name = "Mandelbar";
    spec.displayName = "Mandelbar (Conjugate)";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with conjugate: z̄² + c";
    spec.formula = "z = z̄² + c (conjugate before squaring)";
    spec.formulaLatex = R"(z_{n+1} = \overline{z_n}^2 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // Conjugate before squaring
            ComplexD z_conj(z.real, -z.imag);
            z = z_conj * z_conj + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Thorn (z/c + z² + c)
    //=========================================================================
    spec.name = "Thorn";
    spec.displayName = "Thorn";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Thorn fractal: z/c + z² + c";
    spec.formula = "z = z/c + z² + c";
    spec.formulaLatex = R"(z_{n+1} = \frac{z_n}{c} + z_n^2 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        const double bailout = 256.0;

        // Avoid division by zero
        double c_mag2 = constant.real * constant.real + constant.imag * constant.imag;
        if (c_mag2 < 1e-10)
            return static_cast<double>(maxIter);

        for (int i = 0; i < maxIter; ++i)
        {
            ComplexD z_div_c = z / constant;
            ComplexD z2 = z * z;
            z = z_div_c + z2 + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Spider (z² + c, where c evolves)
    //=========================================================================
    spec.name = "SpiderVariant";
    spec.displayName = "Spider Variant";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Spider fractal where the constant evolves with iteration";
    spec.formula = "z = z² + c; c = c/2 + z";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c_n, \; c_{n+1} = \frac{c_n}{2} + z_n)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD c_evolving = isJulia ? juliaC : c;
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            ComplexD z_old = z;
            z = z * z + c_evolving;
            c_evolving = c_evolving / 2.0 + z_old;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);
}

} // namespace Native
