#include "FractalRegistry.h"
#include <cmath>

namespace Native {

//=============================================================================
// Exponential & Logarithmic Fractals Family
// Variants using exp, log, and power functions
//=============================================================================

void RegisterExponentialFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Exponential (z = e^z + c)
    //=========================================================================
    spec.name = "Exponential";
    spec.displayName = "Exponential Fractal";
    spec.category = "Exponential Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Fractal using exponential function";
    spec.formula = "z = e^z + c";
    spec.formulaLatex = R"(z_{n+1} = e^{z_n} + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            // Complex exponential: e^(a+bi) = e^a * (cos(b) + i*sin(b))
            double exp_real = std::exp(z.real);
            double real_exp = exp_real * std::cos(z.imag);
            double imag_exp = exp_real * std::sin(z.imag);
            z = ComplexD(real_exp, imag_exp) + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    //=========================================================================
    // MandelExp (z² + e^z + c)
    //=========================================================================
    spec.name = "MandelExp";
    spec.displayName = "Mandelbrot Exponential";
    spec.category = "Exponential Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot set with exponential term";
    spec.formula = "z = z² + e^z + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + e^{z_n} + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            double exp_real = std::exp(z.real);
            double real_exp = exp_real * std::cos(z.imag);
            double imag_exp = exp_real * std::sin(z.imag);
            ComplexD exp_z(real_exp, imag_exp);

            z = (z * z) + exp_z + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    //=========================================================================
    // LMandelExp (c * e^z)
    //=========================================================================
    spec.name = "LMandelExp";
    spec.displayName = "Lambda Mandel Exponential";
    spec.category = "Exponential Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Lambda-style with exponential";
    spec.formula = "z = c * e^z";
    spec.formulaLatex = R"(z_{n+1} = c \cdot e^{z_n})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.5, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            double exp_real = std::exp(z.real);
            double real_exp = exp_real * std::cos(z.imag);
            double imag_exp = exp_real * std::sin(z.imag);
            ComplexD exp_z(real_exp, imag_exp);

            z = constant * exp_z;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    //=========================================================================
    // LLambdaExp (c * z * e^z)
    //=========================================================================
    spec.name = "LLambdaExp";
    spec.displayName = "Lambda Lambda Exponential";
    spec.category = "Exponential Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Lambda variation with exponential";
    spec.formula = "z = c * z * e^z";
    spec.formulaLatex = R"(z_{n+1} = c \cdot z_n \cdot e^{z_n})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(1.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            double exp_real = std::exp(z.real);
            double real_exp = exp_real * std::cos(z.imag);
            double imag_exp = exp_real * std::sin(z.imag);
            ComplexD exp_z(real_exp, imag_exp);

            z = constant * z * exp_z;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    //=========================================================================
    // ZToTheZ (z^z + c)
    //=========================================================================
    spec.name = "ZToTheZ";
    spec.displayName = "z^z + c";
    spec.category = "Exponential Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Self-power fractal";
    spec.formula = "z = z^z + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^{z_n} + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.5, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            // z^z = e^(z*ln(z))
            double mag = std::sqrt(z.real * z.real + z.imag * z.imag);
            if (mag < 1e-10) break;

            double arg = std::atan2(z.imag, z.real);
            double ln_mag = std::log(mag);

            // z * ln(z)
            double a = z.real * ln_mag - z.imag * arg;
            double b = z.real * arg + z.imag * ln_mag;

            // e^(a+bi)
            double exp_a = std::exp(a);
            z = ComplexD(exp_a * std::cos(b), exp_a * std::sin(b)) + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Logarithm (log(z) + c)
    //=========================================================================
    spec.name = "Logarithm";
    spec.displayName = "Logarithm Fractal";
    spec.category = "Exponential Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Fractal using logarithm function";
    spec.formula = "z = log(z) + c";
    spec.formulaLatex = R"(z_{n+1} = \log(z_n) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(1.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            // Complex logarithm: log(a+bi) = ln(|z|) + i*arg(z)
            double mag = std::sqrt(z.real * z.real + z.imag * z.imag);
            if (mag < 1e-10) break;

            double arg = std::atan2(z.imag, z.real);
            z = ComplexD(std::log(mag), arg) + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    // TODO: Add more exponential/power variants:
    // - z^n * e^z variations
    // - Tetration (z^^z)
    // - Lambert W function
    // - Combinations like log(z²) + c
}

} // namespace Native
