#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Lambda Extended Family
// Extended Lambda fractal variations with different powers and functions
// Formula base: z = λ * z * (1 - z) with variations
//=============================================================================

void RegisterLambdaExtendedFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Lambda Power 3 (Lambda³)
    //=========================================================================
    spec.name = "LambdaPower3";
    spec.displayName = "Lambda Power 3";
    spec.category = "Lambda Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Lambda fractal with cubic power: z = λ * z³ * (1 - z)";
    spec.formula = "z = λ * z³ * (1 - z)";
    spec.formulaLatex = R"(z_{n+1} = \lambda \cdot z_n^3 \cdot (1 - z_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        ComplexD lambda = isJulia ? juliaC : c;
        const double bailout = 4.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // z³
            ComplexD z2 = z * z;
            ComplexD z3 = z2 * z;

            // λ * z³ * (1 - z)
            z = lambda * z3 * (ComplexD(1.0, 0.0) - z);

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
    spec.defaultBailout = 4.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Lambda Power 4 (Lambda⁴)
    //=========================================================================
    spec.name = "LambdaPower4";
    spec.displayName = "Lambda Power 4";
    spec.category = "Lambda Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Lambda fractal with quartic power: z = λ * z⁴ * (1 - z)";
    spec.formula = "z = λ * z⁴ * (1 - z)";
    spec.formulaLatex = R"(z_{n+1} = \lambda \cdot z_n^4 \cdot (1 - z_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        ComplexD lambda = isJulia ? juliaC : c;
        const double bailout = 4.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // z⁴
            ComplexD z2 = z * z;
            ComplexD z4 = z2 * z2;

            // λ * z⁴ * (1 - z)
            z = lambda * z4 * (ComplexD(1.0, 0.0) - z);

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
    spec.defaultBailout = 4.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Lambda Tanh (Hyperbolic Tangent)
    //=========================================================================
    spec.name = "LambdaTanh";
    spec.displayName = "Lambda Tanh";
    spec.category = "Lambda Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Lambda fractal with tanh function: z = λ * tanh(z)";
    spec.formula = "z = λ * tanh(z)";
    spec.formulaLatex = R"(z_{n+1} = \lambda \cdot \tanh(z_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        ComplexD lambda = isJulia ? juliaC : c;
        const double bailout = 100.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // Complex tanh(z) = tanh(a+bi) = (sinh(2a) + i*sin(2b)) / (cosh(2a) + cos(2b))
            double sinh_2a = std::sinh(2.0 * z.real);
            double sin_2b = std::sin(2.0 * z.imag);
            double cosh_2a = std::cosh(2.0 * z.real);
            double cos_2b = std::cos(2.0 * z.imag);

            double denom = cosh_2a + cos_2b;
            if (std::abs(denom) < 1e-10) break;

            ComplexD tanh_z(sinh_2a / denom, sin_2b / denom);
            z = lambda * tanh_z;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Lambda Tan (Tangent)
    //=========================================================================
    spec.name = "LambdaTan";
    spec.displayName = "Lambda Tan";
    spec.category = "Lambda Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Lambda fractal with tangent function: z = λ * tan(z)";
    spec.formula = "z = λ * tan(z)";
    spec.formulaLatex = R"(z_{n+1} = \lambda \cdot \tan(z_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        ComplexD lambda = isJulia ? juliaC : c;
        const double bailout = 100.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // Complex tan(z) = sin(z)/cos(z)
            // sin(z) = sin(a)cosh(b) + i*cos(a)sinh(b)
            double sin_z_real = std::sin(z.real) * std::cosh(z.imag);
            double sin_z_imag = std::cos(z.real) * std::sinh(z.imag);
            ComplexD sin_z(sin_z_real, sin_z_imag);

            // cos(z) = cos(a)cosh(b) - i*sin(a)sinh(b)
            double cos_z_real = std::cos(z.real) * std::cosh(z.imag);
            double cos_z_imag = -std::sin(z.real) * std::sinh(z.imag);
            ComplexD cos_z(cos_z_real, cos_z_imag);

            double cos_mag2 = cos_z.real * cos_z.real + cos_z.imag * cos_z.imag;
            if (cos_mag2 < 1e-10) break;

            ComplexD tan_z = sin_z / cos_z;
            z = lambda * tan_z;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Lambda Squared (λ * z² * (1 - z²))
    //=========================================================================
    spec.name = "LambdaSquared";
    spec.displayName = "Lambda Squared";
    spec.category = "Lambda Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Lambda fractal with squared variation: z = λ * z² * (1 - z²)";
    spec.formula = "z = λ * z² * (1 - z²)";
    spec.formulaLatex = R"(z_{n+1} = \lambda \cdot z_n^2 \cdot (1 - z_n^2))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        ComplexD lambda = isJulia ? juliaC : c;
        const double bailout = 4.0;

        for (int i = 0; i < maxIter; ++i)
        {
            ComplexD z2 = z * z;

            // λ * z² * (1 - z²)
            z = lambda * z2 * (ComplexD(1.0, 0.0) - z2);

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
    spec.defaultBailout = 4.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Lambda Flip (λ * (1 - z) * z - inverse order)
    //=========================================================================
    spec.name = "LambdaFlip";
    spec.displayName = "Lambda Flip";
    spec.category = "Lambda Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Lambda fractal with flipped formula: z = λ * (1 - z) * z";
    spec.formula = "z = λ * (1 - z) * z";
    spec.formulaLatex = R"(z_{n+1} = \lambda \cdot (1 - z_n) \cdot z_n)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.5, 0.0);  // Different starting point for variation
        ComplexD lambda = isJulia ? juliaC : c;
        const double bailout = 4.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // λ * (1 - z) * z - mathematically same but numerically different order
            z = lambda * (ComplexD(1.0, 0.0) - z) * z;

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
    spec.defaultBailout = 4.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Lambda Modified (λ * z * (1 - z) + z)
    //=========================================================================
    spec.name = "LambdaModified";
    spec.displayName = "Lambda Modified";
    spec.category = "Lambda Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Modified Lambda fractal: z = λ * z * (1 - z) + z";
    spec.formula = "z = λ * z * (1 - z) + z";
    spec.formulaLatex = R"(z_{n+1} = \lambda \cdot z_n \cdot (1 - z_n) + z_n)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        ComplexD lambda = isJulia ? juliaC : c;
        const double bailout = 4.0;

        for (int i = 0; i < maxIter; ++i)
        {
            ComplexD z_old = z;

            // λ * z * (1 - z) + z
            z = lambda * z * (ComplexD(1.0, 0.0) - z) + z_old;

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
    spec.defaultBailout = 4.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Lambda Phoenix (λ * z * (1 - z) + p * z_prev)
    //=========================================================================
    spec.name = "LambdaPhoenix";
    spec.displayName = "Lambda Phoenix";
    spec.category = "Lambda Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Lambda fractal with Phoenix-style term: z = λ * z * (1 - z) + p * z_prev";
    spec.formula = "z = λ * z * (1 - z) + p * z_prev";
    spec.formulaLatex = R"(z_{n+1} = \lambda \cdot z_n \cdot (1 - z_n) + p \cdot z_{n-1})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        ComplexD z_prev(0.0, 0.0);
        ComplexD lambda = isJulia ? juliaC : c;
        ComplexD p(0.5, 0.0);  // Phoenix parameter
        const double bailout = 4.0;

        for (int i = 0; i < maxIter; ++i)
        {
            ComplexD z_new = lambda * z * (ComplexD(1.0, 0.0) - z) + p * z_prev;
            z_prev = z;
            z = z_new;

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
    spec.defaultBailout = 4.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);
}

} // namespace Native
