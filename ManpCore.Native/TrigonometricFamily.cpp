#include "FractalRegistry.h"
#include <cmath>

namespace Native {

//=============================================================================
// Trigonometric Fractals Family
// Variants that use sine, cosine, tangent, and hyperbolic trig functions
//=============================================================================

void RegisterTrigonometricFamily()
{
    FractalSpec spec;

    //=========================================================================
    // MandelTrig (Mandelbrot with Sine/Cosine)
    //=========================================================================
    spec.name = "MandelTrig";
    spec.displayName = "Mandelbrot Trig";
    spec.category = "Trigonometric Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot set with trigonometric functions added";
    spec.formula = "z = z² + sin(z) + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + \sin(z_n) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            // Complex sine: sin(a+bi) = sin(a)cosh(b) + i*cos(a)sinh(b)
            double real_sin = std::sin(z.real) * std::cosh(z.imag);
            double imag_sin = std::cos(z.real) * std::sinh(z.imag);
            ComplexD sin_z(real_sin, imag_sin);

            // z = z² + sin(z) + c
            z = (z * z) + sin_z + constant;

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
    // Sine (Mandelbrot Sine)
    //=========================================================================
    spec.name = "Sine";
    spec.displayName = "Sine Fractal";
    spec.category = "Trigonometric Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Fractal using sine function iteration";
    spec.formula = "z = sin(z) + c";
    spec.formulaLatex = R"(z_{n+1} = \sin(z_n) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            // Complex sine
            double real_sin = std::sin(z.real) * std::cosh(z.imag);
            double imag_sin = std::cos(z.real) * std::sinh(z.imag);
            z = ComplexD(real_sin, imag_sin) + constant;

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
    // LMandelSine
    //=========================================================================
    spec.name = "LMandelSine";
    spec.displayName = "Lambda Mandel Sine";
    spec.category = "Trigonometric Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Lambda-style fractal with sine function";
    spec.formula = "z = c * sin(z)";
    spec.formulaLatex = R"(z_{n+1} = c \cdot \sin(z_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.5, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            double real_sin = std::sin(z.real) * std::cosh(z.imag);
            double imag_sin = std::cos(z.real) * std::sinh(z.imag);
            ComplexD sin_z(real_sin, imag_sin);

            z = constant * sin_z;

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
    // LLambdaSine
    //=========================================================================
    spec.name = "LLambdaSine";
    spec.displayName = "Lambda Lambda Sine";
    spec.category = "Trigonometric Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Lambda variation with sine function";
    spec.formula = "z = c * z * sin(z)";
    spec.formulaLatex = R"(z_{n+1} = c \cdot z_n \cdot \sin(z_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(1.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            double real_sin = std::sin(z.real) * std::cosh(z.imag);
            double imag_sin = std::cos(z.real) * std::sinh(z.imag);
            ComplexD sin_z(real_sin, imag_sin);

            z = constant * z * sin_z;

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
    // LMandelCos (Mandel Cosine)
    //=========================================================================
    spec.name = "LMandelCos";
    spec.displayName = "Lambda Mandel Cosine";
    spec.category = "Trigonometric Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Lambda-style fractal with cosine function";
    spec.formula = "z = c * cos(z)";
    spec.formulaLatex = R"(z_{n+1} = c \cdot \cos(z_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.5, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            // Complex cosine: cos(a+bi) = cos(a)cosh(b) - i*sin(a)sinh(b)
            double real_cos = std::cos(z.real) * std::cosh(z.imag);
            double imag_cos = -std::sin(z.real) * std::sinh(z.imag);
            ComplexD cos_z(real_cos, imag_cos);

            z = constant * cos_z;

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
    // LLambdaCos
    //=========================================================================
    spec.name = "LLambdaCos";
    spec.displayName = "Lambda Lambda Cosine";
    spec.category = "Trigonometric Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Lambda variation with cosine function";
    spec.formula = "z = c * z * cos(z)";
    spec.formulaLatex = R"(z_{n+1} = c \cdot z_n \cdot \cos(z_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(1.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            double real_cos = std::cos(z.real) * std::cosh(z.imag);
            double imag_cos = -std::sin(z.real) * std::sinh(z.imag);
            ComplexD cos_z(real_cos, imag_cos);

            z = constant * z * cos_z;

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
    // LMandelSinh (Hyperbolic Sine)
    //=========================================================================
    spec.name = "LMandelSinh";
    spec.displayName = "Lambda Mandel Sinh";
    spec.category = "Trigonometric Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Lambda-style fractal with hyperbolic sine";
    spec.formula = "z = c * sinh(z)";
    spec.formulaLatex = R"(z_{n+1} = c \cdot \sinh(z_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.5, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            // Complex sinh: sinh(a+bi) = sinh(a)cos(b) + i*cosh(a)sin(b)
            double real_sinh = std::sinh(z.real) * std::cos(z.imag);
            double imag_sinh = std::cosh(z.real) * std::sin(z.imag);
            ComplexD sinh_z(real_sinh, imag_sinh);

            z = constant * sinh_z;

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
    // LLambdaSinh
    //=========================================================================
    spec.name = "LLambdaSinh";
    spec.displayName = "Lambda Lambda Sinh";
    spec.category = "Trigonometric Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Lambda variation with hyperbolic sine";
    spec.formula = "z = c * z * sinh(z)";
    spec.formulaLatex = R"(z_{n+1} = c \cdot z_n \cdot \sinh(z_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(1.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            double real_sinh = std::sinh(z.real) * std::cos(z.imag);
            double imag_sinh = std::cosh(z.real) * std::sin(z.imag);
            ComplexD sinh_z(real_sinh, imag_sinh);

            z = constant * z * sinh_z;

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
    // LMandelCosh (Hyperbolic Cosine)
    //=========================================================================
    spec.name = "LMandelCosh";
    spec.displayName = "Lambda Mandel Cosh";
    spec.category = "Trigonometric Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Lambda-style fractal with hyperbolic cosine";
    spec.formula = "z = c * cosh(z)";
    spec.formulaLatex = R"(z_{n+1} = c \cdot \cosh(z_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.5, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            // Complex cosh: cosh(a+bi) = cosh(a)cos(b) + i*sinh(a)sin(b)
            double real_cosh = std::cosh(z.real) * std::cos(z.imag);
            double imag_cosh = std::sinh(z.real) * std::sin(z.imag);
            ComplexD cosh_z(real_cosh, imag_cosh);

            z = constant * cosh_z;

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
    // LLambdaCosh
    //=========================================================================
    spec.name = "LLambdaCosh";
    spec.displayName = "Lambda Lambda Cosh";
    spec.category = "Trigonometric Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Lambda variation with hyperbolic cosine";
    spec.formula = "z = c * z * cosh(z)";
    spec.formulaLatex = R"(z_{n+1} = c \cdot z_n \cdot \cosh(z_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(1.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            double real_cosh = std::cosh(z.real) * std::cos(z.imag);
            double imag_cosh = std::sinh(z.real) * std::sin(z.imag);
            ComplexD cosh_z(real_cosh, imag_cosh);

            z = constant * z * cosh_z;

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
    // SinZ (Simple Sine)
    //=========================================================================
    spec.name = "SinZ";
    spec.displayName = "Sin(z) + c";
    spec.category = "Trigonometric Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Simple sine iteration";
    spec.formula = "z = sin(z) + c";
    spec.formulaLatex = R"(z_{n+1} = \sin(z_n) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            double real_sin = std::sin(z.real) * std::cosh(z.imag);
            double imag_sin = std::cos(z.real) * std::sinh(z.imag);
            z = ComplexD(real_sin, imag_sin) + constant;

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
    // CosZ (Simple Cosine)
    //=========================================================================
    spec.name = "CosZ";
    spec.displayName = "Cos(z) + c";
    spec.category = "Trigonometric Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Simple cosine iteration";
    spec.formula = "z = cos(z) + c";
    spec.formulaLatex = R"(z_{n+1} = \cos(z_n) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            double real_cos = std::cos(z.real) * std::cosh(z.imag);
            double imag_cos = -std::sin(z.real) * std::sinh(z.imag);
            z = ComplexD(real_cos, imag_cos) + constant;

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

    // TODO: Add more trigonometric variants:
    // - TanZ, SinhZ, CoshZ, TanhZ
    // - Combinations like z²*sin(z), z²*cos(z)
    // - Exponential-trig hybrids
}

} // namespace Native
