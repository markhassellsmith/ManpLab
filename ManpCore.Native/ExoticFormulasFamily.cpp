#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Exotic Formulas Family
// Unusual and creative fractal formulas
//=============================================================================

void RegisterExoticFormulasFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Celtic Mandelbrot
    //=========================================================================
    spec.name = "CelticMandel";
    spec.displayName = "Celtic Mandelbrot";
    spec.category = "Exotic";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with absolute value of real component";
    spec.formula = "z = (|Re(z²)| + i·Im(z²)) + c";
    spec.formulaLatex = R"(z_{n+1} = (|Re(z_n^2)| + i \cdot Im(z_n^2)) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD param = isJulia ? juliaC : c;
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            double zr = z.real * z.real - z.imag * z.imag;
            double zi = 2.0 * z.real * z.imag;

            z.real = std::abs(zr) + param.real;
            z.imag = zi + param.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = -0.5;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.333333;  // Viewport tuning: X scale 3.0, Y scale 1.69
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Buffalo Fractal
    //=========================================================================
    spec.name = "Buffalo";
    spec.displayName = "Buffalo Fractal";
    spec.category = "Exotic";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Hybrid of Mandelbrot and Celtic with both components absolute";
    spec.formula = "z = (|Re(z²)| - |Im(z²)|) + i·Im(z²) + c";
    spec.formulaLatex = R"(z_{n+1} = (|Re(z_n^2)| - |Im(z_n^2)|) + i \cdot Im(z_n^2) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD param = isJulia ? juliaC : c;
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            double zr = z.real * z.real - z.imag * z.imag;
            double zi = 2.0 * z.real * z.imag;

            z.real = std::abs(zr) - std::abs(z.imag * z.imag) + param.real;
            z.imag = zi + param.imag;
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
    // Perpendicular Mandelbrot
    //=========================================================================
    spec.name = "PerpendicularMandel";
    spec.displayName = "Perpendicular Mandelbrot";
    spec.category = "Exotic";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with perpendicular imaginary component";
    spec.formula = "z = (Re(z²) + i·|Im(z²)|) + c";
    spec.formulaLatex = R"(z_{n+1} = (Re(z_n^2) + i \cdot |Im(z_n^2)|) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD param = isJulia ? juliaC : c;
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            double zr = z.real * z.real - z.imag * z.imag;
            double zi = 2.0 * z.real * z.imag;

            z.real = zr + param.real;
            z.imag = std::abs(zi) + param.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = -0.4;
    spec.defaultCenterY = -0.3;
    spec.defaultZoom = 1.219512;  // Viewport tuning: X scale 3.28, Y scale 1.84
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Heart Mandelbrot
    //=========================================================================
    spec.name = "HeartMandel";
    spec.displayName = "Heart Mandelbrot";
    spec.category = "Exotic";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot variant with heart-shaped main bulb";
    spec.formula = "z = (|z²| - Re(z²)) + i·Im(z²) + c";
    spec.formulaLatex = R"(z_{n+1} = (|z_n^2| - Re(z_n^2)) + i \cdot Im(z_n^2) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD param = isJulia ? juliaC : c;
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            double zr = z.real * z.real - z.imag * z.imag;
            double zi = 2.0 * z.real * z.imag;
            double mag = std::sqrt(zr * zr + zi * zi);

            z.real = mag - zr + param.real;
            z.imag = zi + param.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = -0.6;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.961538;  // Viewport tuning: X scale 4.16, Y scale 2.34
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Quasi-Perpendicular (Quasi-Celtic)
    //=========================================================================
    spec.name = "QuasiPerpendicular";
    spec.displayName = "Quasi-Perpendicular";
    spec.category = "Exotic";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Hybrid of Celtic and perpendicular operations";
    spec.formula = "z = (|Re(z²)| + i·|Im(z²)|) + c";
    spec.formulaLatex = R"(z_{n+1} = (|Re(z_n^2)| + i \cdot |Im(z_n^2)|) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD param = isJulia ? juliaC : c;
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            double zr = z.real * z.real - z.imag * z.imag;
            double zi = 2.0 * z.real * z.imag;

            z.real = std::abs(zr) + param.real;
            z.imag = std::abs(zi) + param.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = -0.8;
    spec.defaultCenterY = -0.5;
    spec.defaultZoom = 0.966184;  // Viewport tuning: X scale 4.14, Y scale 2.33
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Shark Fin Fractal
    //=========================================================================
    spec.name = "SharkFin";
    spec.displayName = "Shark Fin";
    spec.category = "Exotic";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Asymmetric fractal with distinctive shark fin shape";
    spec.formula = "z = z² + c, with real(z) = |real(z)|";
    spec.formulaLatex = R"(z_{n+1} = (|Re(z_n)|^2 - Im(z_n)^2) + i \cdot 2|Re(z_n)| \cdot Im(z_n) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD param = isJulia ? juliaC : c;
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            double abs_real = std::abs(z.real);
            ComplexD temp = z;
            z.real = abs_real * abs_real - temp.imag * temp.imag + param.real;
            z.imag = 2.0 * abs_real * temp.imag + param.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = -0.5;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.532567;  // Viewport tuning: X scale 2.61, Y scale 1.47
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Zubieta Fractal
    //=========================================================================
    spec.name = "Zubieta";
    spec.displayName = "Zubieta";
    spec.category = "Exotic";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Complex polynomial variation discovered by Zubieta";
    spec.formula = "z = z² + c/z";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + \frac{c}{z_n})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.1, 0.1);  // Avoid division by zero
        ComplexD param = isJulia ? juliaC : c;
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            if (magSq < 1e-10)  // Avoid division by zero
                return static_cast<double>(maxIter);

            // z^2
            ComplexD z_squared;
            z_squared.real = z.real * z.real - z.imag * z.imag;
            z_squared.imag = 2.0 * z.real * z.imag;

            // c/z = c * conj(z) / |z|^2
            ComplexD c_over_z;
            c_over_z.real = (param.real * z.real + param.imag * z.imag) / magSq;
            c_over_z.imag = (param.imag * z.real - param.real * z.imag) / magSq;

            z.real = z_squared.real + c_over_z.real;
            z.imag = z_squared.imag + c_over_z.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 7.8125;  // Viewport tuning: X scale 0.512, Y scale 0.288
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Wavy Fractal
    //=========================================================================
    spec.name = "Wavy";
    spec.displayName = "Wavy";
    spec.category = "Exotic";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Fractal with wave-like distortion";
    spec.formula = "z = z² + c + sin(z)";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c + \sin(z_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD param = isJulia ? juliaC : c;
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            // z^2
            ComplexD z_squared;
            z_squared.real = z.real * z.real - z.imag * z.imag;
            z_squared.imag = 2.0 * z.real * z.imag;

            // sin(z) = sin(a+bi) = sin(a)cosh(b) + i·cos(a)sinh(b)
            double sin_real = std::sin(z.real) * std::cosh(z.imag);
            double sin_imag = std::cos(z.real) * std::sinh(z.imag);

            z.real = z_squared.real + param.real + sin_real * 0.1;
            z.imag = z_squared.imag + param.imag + sin_imag * 0.1;
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
