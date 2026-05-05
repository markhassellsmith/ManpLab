#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Polynomial Variants Family
// Various polynomial-based fractal formulas
//=============================================================================

// Helper: Complex power function
static ComplexD ComplexPower(ComplexD z, double power)
{
    if (z.real == 0.0 && z.imag == 0.0)
        return ComplexD(0.0, 0.0);

    double r = std::sqrt(z.real * z.real + z.imag * z.imag);
    double theta = std::atan2(z.imag, z.real);
    double newR = std::pow(r, power);
    double newTheta = theta * power;

    return ComplexD(newR * std::cos(newTheta), newR * std::sin(newTheta));
}

void RegisterPolynomialVariantsFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Cubic Mandelbrot
    //=========================================================================
    spec.name = "CubicMandel";
    spec.displayName = "Cubic Mandelbrot";
    spec.category = "Polynomial";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with cubic iteration";
    spec.formula = "z = z³ + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^3 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            // z^3 = z * z * z
            ComplexD z2;
            z2.real = z.real * z.real - z.imag * z.imag;
            z2.imag = 2.0 * z.real * z.imag;

            ComplexD temp = z;
            z.real = z2.real * temp.real - z2.imag * temp.imag + c.real;
            z.imag = z2.real * temp.imag + z2.imag * temp.real + c.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.2;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Quartic Mandelbrot
    //=========================================================================
    spec.name = "QuarticMandel";
    spec.displayName = "Quartic Mandelbrot";
    spec.category = "Polynomial";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with quartic iteration";
    spec.formula = "z = z⁴ + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^4 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            // z^4 = (z^2)^2
            ComplexD z2;
            z2.real = z.real * z.real - z.imag * z.imag;
            z2.imag = 2.0 * z.real * z.imag;

            z.real = z2.real * z2.real - z2.imag * z2.imag + c.real;
            z.imag = 2.0 * z2.real * z2.imag + c.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.3;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Quintic Mandelbrot
    //=========================================================================
    spec.name = "QuinticMandel";
    spec.displayName = "Quintic Mandelbrot";
    spec.category = "Polynomial";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with quintic iteration";
    spec.formula = "z = z⁵ + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^5 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            ComplexD z5 = ComplexPower(z, 5.0);
            z.real = z5.real + c.real;
            z.imag = z5.imag + c.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.4;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Sextic Mandelbrot
    //=========================================================================
    spec.name = "SexticMandel";
    spec.displayName = "Sextic Mandelbrot";
    spec.category = "Polynomial";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot with sextic (6th power) iteration";
    spec.formula = "z = z⁶ + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^6 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            // z^6 = (z^2)^3
            ComplexD z2;
            z2.real = z.real * z.real - z.imag * z.imag;
            z2.imag = 2.0 * z.real * z.imag;

            ComplexD z4;
            z4.real = z2.real * z2.real - z2.imag * z2.imag;
            z4.imag = 2.0 * z2.real * z2.imag;

            z.real = z4.real * z2.real - z4.imag * z2.imag + c.real;
            z.imag = z4.real * z2.imag + z4.imag * z2.real + c.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Rational (z^2+c)/(z^2+1)
    //=========================================================================
    spec.name = "RationalR1";
    spec.displayName = "Rational R1";
    spec.category = "Polynomial";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Rational map: (z²+c)/(z²+1)";
    spec.formula = "z = (z²+c)/(z²+1)";
    spec.formulaLatex = R"(z_{n+1} = \frac{z_n^2 + c}{z_n^2 + 1})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.1, 0.1);
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            // Numerator: z^2 + c
            ComplexD num;
            num.real = z.real * z.real - z.imag * z.imag + c.real;
            num.imag = 2.0 * z.real * z.imag + c.imag;

            // Denominator: z^2 + 1
            ComplexD denom;
            denom.real = z.real * z.real - z.imag * z.imag + 1.0;
            denom.imag = 2.0 * z.real * z.imag;

            // Division: num/denom
            double denomMagSq = denom.real * denom.real + denom.imag * denom.imag;
            if (denomMagSq < 1e-10)
                return static_cast<double>(maxIter);

            z.real = (num.real * denom.real + num.imag * denom.imag) / denomMagSq;
            z.imag = (num.imag * denom.real - num.real * denom.imag) / denomMagSq;
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
    // Polynomial z^3 - z + c
    //=========================================================================
    spec.name = "PolyZ3MinusZ";
    spec.displayName = "Polynomial z³-z+c";
    spec.category = "Polynomial";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Polynomial fractal: z³ - z + c";
    spec.formula = "z = z³ - z + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^3 - z_n + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
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

            ComplexD z3;
            z3.real = z2.real * z.real - z2.imag * z.imag;
            z3.imag = z2.real * z.imag + z2.imag * z.real;

            // z^3 - z + c
            ComplexD temp = z;
            z.real = z3.real - temp.real + c.real;
            z.imag = z3.imag - temp.imag + c.imag;
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
    // Polynomial z^4 + z^3 + c
    //=========================================================================
    spec.name = "PolyZ4PlusZ3";
    spec.displayName = "Polynomial z⁴+z³+c";
    spec.category = "Polynomial";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Polynomial fractal: z⁴ + z³ + c";
    spec.formula = "z = z⁴ + z³ + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^4 + z_n^3 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

            // z^2
            ComplexD z2;
            z2.real = z.real * z.real - z.imag * z.imag;
            z2.imag = 2.0 * z.real * z.imag;

            // z^3
            ComplexD z3;
            z3.real = z2.real * z.real - z2.imag * z.imag;
            z3.imag = z2.real * z.imag + z2.imag * z.real;

            // z^4
            ComplexD z4;
            z4.real = z2.real * z2.real - z2.imag * z2.imag;
            z4.imag = 2.0 * z2.real * z2.imag;

            z.real = z4.real + z3.real + c.real;
            z.imag = z4.imag + z3.imag + c.imag;
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
    // Biomorph
    //=========================================================================
    spec.name = "Biomorph";
    spec.displayName = "Biomorph";
    spec.category = "Polynomial";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Biomorph fractal with organism-like shapes";
    spec.formula = "z = z² + c, special bailout condition";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \; |Re(z)| > B \text{ or } |Im(z)| > B)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 10.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // Biomorph bailout condition
            if (std::abs(z.real) > bailout || std::abs(z.imag) > bailout)
                return i + 1.0;

            ComplexD temp = z;
            z.real = temp.real * temp.real - temp.imag * temp.imag + c.real;
            z.imag = 2.0 * temp.real * temp.imag + c.imag;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.5;
    spec.defaultBailout = 10.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);
}

} // namespace Native
