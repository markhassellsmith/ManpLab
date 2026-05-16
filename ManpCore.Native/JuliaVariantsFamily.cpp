#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Julia Variants Family
// Specialized Julia set variations with interesting parameters
//=============================================================================

void RegisterJuliaVariantsFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Julia Set - Classic
    //=========================================================================
    spec.name = "JuliaClassic";
    spec.displayName = "Julia - Classic";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Classic Julia set with standard Mandelbrot iteration";
    spec.formula = "z = z² + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        ComplexD param(-0.4, 0.6);  // Default Julia parameter
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            ComplexD temp = z;
            z.real = temp.real * temp.real - temp.imag * temp.imag + param.real;
            z.imag = 2.0 * temp.real * temp.imag + param.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Julia Cubic
    //=========================================================================
    spec.name = "JuliaCubic";
    spec.displayName = "Julia - Cubic";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set with cubic iteration";
    spec.formula = "z = z³ + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^3 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        ComplexD param(-0.2, 0.75);
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            // z^3
            ComplexD z2;
            z2.real = z.real * z.real - z.imag * z.imag;
            z2.imag = 2.0 * z.real * z.imag;

            ComplexD temp = z;
            z.real = z2.real * temp.real - z2.imag * temp.imag + param.real;
            z.imag = z2.real * temp.imag + z2.imag * temp.real + param.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Julia Burning Ship
    //=========================================================================
    spec.name = "JuliaBurningShip";
    spec.displayName = "Julia - Burning Ship";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set with Burning Ship formula";
    spec.formula = "z = (|Re(z)| + i|Im(z)|)² + c";
    spec.formulaLatex = R"(z_{n+1} = (|Re(z_n)| + i|Im(z_n)|)^2 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        ComplexD param(-1.75, -0.05);
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            double abs_real = std::abs(z.real);
            double abs_imag = std::abs(z.imag);

            z.real = abs_real * abs_real - abs_imag * abs_imag + param.real;
            z.imag = 2.0 * abs_real * abs_imag + param.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.2;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Julia Phoenix
    //=========================================================================
    spec.name = "JuliaPhoenix";
    spec.displayName = "Julia - Phoenix";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set with Phoenix formula: uses previous iteration";
    spec.formula = "z = z² + c + p·z_prev";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c + p \cdot z_{n-1})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        ComplexD z_prev(0.0, 0.0);
        ComplexD param(0.56667, -0.5);
        ComplexD phoenix_p(-0.5, 0.0);
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            ComplexD temp = z;
            ComplexD z2;
            z2.real = z.real * z.real - z.imag * z.imag;
            z2.imag = 2.0 * z.real * z.imag;

            z.real = z2.real + param.real + phoenix_p.real * z_prev.real;
            z.imag = z2.imag + param.imag + phoenix_p.imag * z_prev.imag;

            z_prev = temp;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Julia Lambda
    //=========================================================================
    spec.name = "JuliaLambda";
    spec.displayName = "Julia - Lambda";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set with Lambda formula";
    spec.formula = "z = λ·z·(1-z)";
    spec.formulaLatex = R"(z_{n+1} = \lambda \cdot z_n \cdot (1 - z_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        ComplexD lambda(0.5, 0.5);
        const double bailout = 100.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0;

            ComplexD one_minus_z;
            one_minus_z.real = 1.0 - z.real;
            one_minus_z.imag = -z.imag;

            // lambda * z
            ComplexD lambda_z;
            lambda_z.real = lambda.real * z.real - lambda.imag * z.imag;
            lambda_z.imag = lambda.real * z.imag + lambda.imag * z.real;

            // (lambda * z) * (1 - z)
            z.real = lambda_z.real * one_minus_z.real - lambda_z.imag * one_minus_z.imag;
            z.imag = lambda_z.real * one_minus_z.imag + lambda_z.imag * one_minus_z.real;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.5;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Julia Sine
    //=========================================================================
    spec.name = "JuliaSine";
    spec.displayName = "Julia - Sine";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set with sine function";
    spec.formula = "z = sin(z) + c";
    spec.formulaLatex = R"(z_{n+1} = \sin(z_n) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        ComplexD param(1.0, 0.1);
        const double bailout = 100.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0;

            // sin(z) = sin(a)cosh(b) + i*cos(a)sinh(b)
            double sin_real = std::sin(z.real) * std::cosh(z.imag);
            double sin_imag = std::cos(z.real) * std::sinh(z.imag);

            z.real = sin_real + param.real;
            z.imag = sin_imag + param.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Julia Exponential
    //=========================================================================
    spec.name = "JuliaExp";
    spec.displayName = "Julia - Exponential";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set with exponential function";
    spec.formula = "z = e^z + c";
    spec.formulaLatex = R"(z_{n+1} = e^{z_n} + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        ComplexD param(0.0, -0.65);
        const double bailout = 100.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0;

            // e^z = e^a * (cos(b) + i*sin(b))
            double exp_a = std::exp(z.real);
            double cos_b = std::cos(z.imag);
            double sin_b = std::sin(z.imag);

            z.real = exp_a * cos_b + param.real;
            z.imag = exp_a * sin_b + param.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.4;
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Julia Magnet
    //=========================================================================
    spec.name = "JuliaMagnet";
    spec.displayName = "Julia - Magnet";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set with Magnet 1 formula";
    spec.formula = "z = ((z² + c - 1)/(2z + c - 2))²";
    spec.formulaLatex = R"(z_{n+1} = \left(\frac{z_n^2 + c - 1}{2z_n + c - 2}\right)^2)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        ComplexD param(1.0, 0.0);
        const double bailout = 100.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0;

            // Numerator: z² + c - 1
            ComplexD z2;
            z2.real = z.real * z.real - z.imag * z.imag;
            z2.imag = 2.0 * z.real * z.imag;

            ComplexD numer;
            numer.real = z2.real + param.real - 1.0;
            numer.imag = z2.imag + param.imag;

            // Denominator: 2z + c - 2
            ComplexD denom;
            denom.real = 2.0 * z.real + param.real - 2.0;
            denom.imag = 2.0 * z.imag + param.imag;

            double denom_magSq = denom.real * denom.real + denom.imag * denom.imag;
            if (denom_magSq < 1e-10) break;

            // Division
            ComplexD ratio;
            ratio.real = (numer.real * denom.real + numer.imag * denom.imag) / denom_magSq;
            ratio.imag = (numer.imag * denom.real - numer.real * denom.imag) / denom_magSq;

            // Square the result
            z.real = ratio.real * ratio.real - ratio.imag * ratio.imag;
            z.imag = 2.0 * ratio.real * ratio.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 3.0;
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);
}

} // namespace Native
