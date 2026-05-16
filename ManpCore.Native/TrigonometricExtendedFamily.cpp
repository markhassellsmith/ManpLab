#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Trigonometric Extended Family
// Additional trigonometric function-based fractals
//=============================================================================

void RegisterTrigonometricExtendedFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Tangent Mandelbrot
    //=========================================================================
    spec.name = "TanMandel";
    spec.displayName = "Tangent Mandelbrot";
    spec.category = "Trigonometric";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with tangent function: z = tan(z) + c";
    spec.formula = "z = tan(z) + c";
    spec.formulaLatex = R"(z_{n+1} = \tan(z_n) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            // tan(z) = sin(z)/cos(z)
            double sin_real = std::sin(z.real) * std::cosh(z.imag);
            double sin_imag = std::cos(z.real) * std::sinh(z.imag);

            double cos_real = std::cos(z.real) * std::cosh(z.imag);
            double cos_imag = -std::sin(z.real) * std::sinh(z.imag);

            double cos_magSq = cos_real * cos_real + cos_imag * cos_imag;
            if (cos_magSq < 1e-10) break;

            double tan_real = (sin_real * cos_real + sin_imag * cos_imag) / cos_magSq;
            double tan_imag = (sin_imag * cos_real - sin_real * cos_imag) / cos_magSq;

            z.real = tan_real + c.real;
            z.imag = tan_imag + c.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Cotangent Fractal
    //=========================================================================
    spec.name = "CotMandel";
    spec.displayName = "Cotangent Mandelbrot";
    spec.category = "Trigonometric";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with cotangent function: z = cot(z) + c";
    spec.formula = "z = cot(z) + c";
    spec.formulaLatex = R"(z_{n+1} = \cot(z_n) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.1, 0.1);
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            // cot(z) = cos(z)/sin(z)
            double sin_real = std::sin(z.real) * std::cosh(z.imag);
            double sin_imag = std::cos(z.real) * std::sinh(z.imag);

            double cos_real = std::cos(z.real) * std::cosh(z.imag);
            double cos_imag = -std::sin(z.real) * std::sinh(z.imag);

            double sin_magSq = sin_real * sin_real + sin_imag * sin_imag;
            if (sin_magSq < 1e-10) break;

            double cot_real = (cos_real * sin_real + cos_imag * sin_imag) / sin_magSq;
            double cot_imag = (cos_imag * sin_real - cos_real * sin_imag) / sin_magSq;

            z.real = cot_real + c.real;
            z.imag = cot_imag + c.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Secant Fractal
    //=========================================================================
    spec.name = "SecMandel";
    spec.displayName = "Secant Mandelbrot";
    spec.category = "Trigonometric";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with secant function: z = sec(z) + c";
    spec.formula = "z = sec(z) + c";
    spec.formulaLatex = R"(z_{n+1} = \sec(z_n) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            // sec(z) = 1/cos(z)
            double cos_real = std::cos(z.real) * std::cosh(z.imag);
            double cos_imag = -std::sin(z.real) * std::sinh(z.imag);

            double cos_magSq = cos_real * cos_real + cos_imag * cos_imag;
            if (cos_magSq < 1e-10) break;

            double sec_real = cos_real / cos_magSq;
            double sec_imag = -cos_imag / cos_magSq;

            z.real = sec_real + c.real;
            z.imag = sec_imag + c.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Cosecant Fractal
    //=========================================================================
    spec.name = "CscMandel";
    spec.displayName = "Cosecant Mandelbrot";
    spec.category = "Trigonometric";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with cosecant function: z = csc(z) + c";
    spec.formula = "z = csc(z) + c";
    spec.formulaLatex = R"(z_{n+1} = \csc(z_n) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.1, 0.1);
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            // csc(z) = 1/sin(z)
            double sin_real = std::sin(z.real) * std::cosh(z.imag);
            double sin_imag = std::cos(z.real) * std::sinh(z.imag);

            double sin_magSq = sin_real * sin_real + sin_imag * sin_imag;
            if (sin_magSq < 1e-10) break;

            double csc_real = sin_real / sin_magSq;
            double csc_imag = -sin_imag / sin_magSq;

            z.real = csc_real + c.real;
            z.imag = csc_imag + c.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // ArcSin Fractal
    //=========================================================================
    spec.name = "ArcSinMandel";
    spec.displayName = "ArcSin Mandelbrot";
    spec.category = "Trigonometric";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with arcsine function: z = asin(z) + c";
    spec.formula = "z = asin(z) + c";
    spec.formulaLatex = R"(z_{n+1} = \arcsin(z_n) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            // asin(z) = -i*ln(iz + sqrt(1-z^2))
            // Approximate for fractal purposes
            double asin_real = std::asin(std::tanh(z.real)) * std::cos(z.imag);
            double asin_imag = std::log(std::abs(z.imag + std::sqrt(1.0 + z.imag * z.imag)));

            z.real = asin_real + c.real;
            z.imag = asin_imag + c.imag;
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
    // ArcCos Fractal
    //=========================================================================
    spec.name = "ArcCosMandel";
    spec.displayName = "ArcCos Mandelbrot";
    spec.category = "Trigonometric";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with arccosine function: z = acos(z) + c";
    spec.formula = "z = acos(z) + c";
    spec.formulaLatex = R"(z_{n+1} = \arccos(z_n) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.5, 0.0);
        const double bailout = 256.0;
        const double pi = 3.14159265358979323846;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            // acos(z) = pi/2 - asin(z)
            double asin_real = std::asin(std::tanh(z.real)) * std::cos(z.imag);
            double asin_imag = std::log(std::abs(z.imag + std::sqrt(1.0 + z.imag * z.imag)));

            z.real = (pi / 2.0 - asin_real) + c.real;
            z.imag = -asin_imag + c.imag;
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
    // ArcTan Fractal
    //=========================================================================
    spec.name = "ArcTanMandel";
    spec.displayName = "ArcTan Mandelbrot";
    spec.category = "Trigonometric";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with arctangent function: z = atan(z) + c";
    spec.formula = "z = atan(z) + c";
    spec.formulaLatex = R"(z_{n+1} = \arctan(z_n) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            // atan(z) for complex z
            double atan_real = 0.5 * (std::atan2(z.real, 1.0 - z.imag) + std::atan2(-z.real, 1.0 + z.imag));
            double atan_imag = 0.25 * std::log((1.0 + z.imag) * (1.0 + z.imag) + z.real * z.real) -
                              0.25 * std::log((1.0 - z.imag) * (1.0 - z.imag) + z.real * z.real);

            z.real = atan_real + c.real;
            z.imag = atan_imag + c.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Hyperbolic Tangent
    //=========================================================================
    spec.name = "TanhMandel";
    spec.displayName = "Tanh Mandelbrot (Linear)";
    spec.category = "Trigonometric";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with hyperbolic tangent: z = tanh(z) + c";
    spec.formula = "z = tanh(z) + c";
    spec.formulaLatex = R"(z_{n+1} = \tanh(z_n) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            // tanh(z) = sinh(z)/cosh(z)
            double sinh_real = std::sinh(z.real) * std::cos(z.imag);
            double sinh_imag = std::cosh(z.real) * std::sin(z.imag);

            double cosh_real = std::cosh(z.real) * std::cos(z.imag);
            double cosh_imag = std::sinh(z.real) * std::sin(z.imag);

            double cosh_magSq = cosh_real * cosh_real + cosh_imag * cosh_imag;
            if (cosh_magSq < 1e-10) break;

            double tanh_real = (sinh_real * cosh_real + sinh_imag * cosh_imag) / cosh_magSq;
            double tanh_imag = (sinh_imag * cosh_real - sinh_real * cosh_imag) / cosh_magSq;

            z.real = tanh_real + c.real;
            z.imag = tanh_imag + c.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);
}

} // namespace Native
