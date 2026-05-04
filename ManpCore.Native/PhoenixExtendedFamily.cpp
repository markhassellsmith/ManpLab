#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Phoenix Extended Family
// Phoenix fractals with memory of previous iteration
// General formula: z_new = f(z) + c + p * z_prev
// where z_prev is the previous iteration's z value
//=============================================================================

// Helper function for complex power
static ComplexD ComplexPower(ComplexD z, int n)
{
    if (n == 2) {
        return ComplexD(z.real*z.real - z.imag*z.imag, 2.0*z.real*z.imag);
    }

    // Use polar form for higher powers: z^n = r^n * (cos(nθ) + i*sin(nθ))
    double r = std::sqrt(z.real*z.real + z.imag*z.imag);
    if (r < 1e-10) return ComplexD(0.0, 0.0);

    double theta = std::atan2(z.imag, z.real);
    double r_n = std::pow(r, n);
    double n_theta = n * theta;

    return ComplexD(r_n * std::cos(n_theta), r_n * std::sin(n_theta));
}

void RegisterPhoenixExtendedFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Phoenix Mandelbrot Mode
    //=========================================================================
    spec.name = "PhoenixM";
    spec.displayName = "Phoenix Mandelbrot";
    spec.category = "Phoenix Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Phoenix fractal in Mandelbrot mode with memory of previous iteration";
    spec.formula = "z = z² + c + p*z_prev";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c + p \cdot z_{n-1})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = ComplexD(0.0, 0.0);
        ComplexD z_prev(0.0, 0.0);
        ComplexD constant = c;
        double p = 0.5;  // Phoenix parameter (can be adjusted)

        for (int i = 0; i < maxIter; ++i)
        {
            ComplexD z_squared = z * z;
            ComplexD feedback = z_prev * p;
            ComplexD z_new = z_squared + constant + feedback;

            double magSq = z_new.real * z_new.real + z_new.imag * z_new.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);

            z_prev = z;
            z = z_new;
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.6;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Phoenix Julia Mode
    //=========================================================================
    spec.name = "PhoenixJ";
    spec.displayName = "Phoenix Julia";
    spec.category = "Phoenix Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Phoenix fractal in Julia mode with beautiful parameter: c = 0.56667 - 0.5i";
    spec.formula = "z = z² + c + p*z_prev (Julia mode)";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c + p \cdot z_{n-1}, \text{ } z_0 = \text{pixel})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;  // In Julia mode, z starts at pixel
        ComplexD z_prev(0.0, 0.0);
        ComplexD constant = ComplexD(0.56667, -0.5);  // Classic Phoenix Julia parameter
        double p = 0.5;

        for (int i = 0; i < maxIter; ++i)
        {
            ComplexD z_squared = z * z;
            ComplexD feedback = z_prev * p;
            ComplexD z_new = z_squared + constant + feedback;

            double magSq = z_new.real * z_new.real + z_new.imag * z_new.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);

            z_prev = z;
            z = z_new;
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
    // Phoenix Power 3
    //=========================================================================
    spec.name = "PhoenixPower3";
    spec.displayName = "Phoenix Cubic";
    spec.category = "Phoenix Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Phoenix fractal with cubic power: z³ + c + p*z_prev";
    spec.formula = "z = z³ + c + p*z_prev";
    spec.formulaLatex = R"(z_{n+1} = z_n^3 + c + p \cdot z_{n-1})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD z_prev(0.0, 0.0);
        ComplexD constant = isJulia ? ComplexD(0.3, -0.4) : c;
        double p = 0.5;

        for (int i = 0; i < maxIter; ++i)
        {
            ComplexD z_cubed = ComplexPower(z, 3);
            ComplexD feedback = z_prev * p;
            ComplexD z_new = z_cubed + constant + feedback;

            double magSq = z_new.real * z_new.real + z_new.imag * z_new.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);

            z_prev = z;
            z = z_new;
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.8;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Phoenix Power 4
    //=========================================================================
    spec.name = "PhoenixPower4";
    spec.displayName = "Phoenix Quartic";
    spec.category = "Phoenix Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Phoenix fractal with quartic power: z⁴ + c + p*z_prev";
    spec.formula = "z = z⁴ + c + p*z_prev";
    spec.formulaLatex = R"(z_{n+1} = z_n^4 + c + p \cdot z_{n-1})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD z_prev(0.0, 0.0);
        ComplexD constant = isJulia ? ComplexD(0.2, -0.3) : c;
        double p = 0.5;

        for (int i = 0; i < maxIter; ++i)
        {
            ComplexD z_quartic = ComplexPower(z, 4);
            ComplexD feedback = z_prev * p;
            ComplexD z_new = z_quartic + constant + feedback;

            double magSq = z_new.real * z_new.real + z_new.imag * z_new.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);

            z_prev = z;
            z = z_new;
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
    // Phoenix with Hyperbolic Cosine
    //=========================================================================
    spec.name = "PhoenixCosh";
    spec.displayName = "Phoenix Cosh";
    spec.category = "Phoenix Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Phoenix fractal with hyperbolic cosine: cosh(z) + c + p*z_prev";
    spec.formula = "z = cosh(z) + c + p*z_prev";
    spec.formulaLatex = R"(z_{n+1} = \cosh(z_n) + c + p \cdot z_{n-1})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD z_prev(0.0, 0.0);
        ComplexD constant = isJulia ? ComplexD(0.4, 0.3) : c;
        double p = 0.5;

        for (int i = 0; i < maxIter; ++i)
        {
            // Complex cosh: cosh(a+bi) = cosh(a)cos(b) + i*sinh(a)sin(b)
            double real_cosh = std::cosh(z.real) * std::cos(z.imag);
            double imag_cosh = std::sinh(z.real) * std::sin(z.imag);
            ComplexD cosh_z(real_cosh, imag_cosh);

            ComplexD feedback = z_prev * p;
            ComplexD z_new = cosh_z + constant + feedback;

            double magSq = z_new.real * z_new.real + z_new.imag * z_new.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);

            z_prev = z;
            z = z_new;
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
    // Phoenix with Sine
    //=========================================================================
    spec.name = "PhoenixSin";
    spec.displayName = "Phoenix Sine";
    spec.category = "Phoenix Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Phoenix fractal with sine function: sin(z) + c + p*z_prev";
    spec.formula = "z = sin(z) + c + p*z_prev";
    spec.formulaLatex = R"(z_{n+1} = \sin(z_n) + c + p \cdot z_{n-1})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD z_prev(0.0, 0.0);
        ComplexD constant = isJulia ? ComplexD(0.5, -0.3) : c;
        double p = 0.5;

        for (int i = 0; i < maxIter; ++i)
        {
            // Complex sine: sin(a+bi) = sin(a)cosh(b) + i*cos(a)sinh(b)
            double real_sin = std::sin(z.real) * std::cosh(z.imag);
            double imag_sin = std::cos(z.real) * std::sinh(z.imag);
            ComplexD sin_z(real_sin, imag_sin);

            ComplexD feedback = z_prev * p;
            ComplexD z_new = sin_z + constant + feedback;

            double magSq = z_new.real * z_new.real + z_new.imag * z_new.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);

            z_prev = z;
            z = z_new;
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
    // Phoenix Complex Parameter (stronger feedback)
    //=========================================================================
    spec.name = "PhoenixComplex";
    spec.displayName = "Phoenix Complex Feedback";
    spec.category = "Phoenix Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Phoenix fractal with complex parameter variation: z² + c + (0.5+0.2i)*z_prev";
    spec.formula = "z = z² + c + (0.5+0.2i)*z_prev";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c + (0.5+0.2i) \cdot z_{n-1})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD z_prev(0.0, 0.0);
        ComplexD constant = isJulia ? ComplexD(0.45, -0.42) : c;
        ComplexD p(0.5, 0.2);  // Complex feedback parameter

        for (int i = 0; i < maxIter; ++i)
        {
            ComplexD z_squared = z * z;
            ComplexD feedback = z_prev * p;
            ComplexD z_new = z_squared + constant + feedback;

            double magSq = z_new.real * z_new.real + z_new.imag * z_new.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);

            z_prev = z;
            z = z_new;
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.7;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Phoenix Lambda Hybrid
    //=========================================================================
    spec.name = "PhoenixLambda";
    spec.displayName = "Phoenix Lambda";
    spec.category = "Phoenix Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Hybrid Phoenix-Lambda fractal: c*z*(1-z) + p*z_prev";
    spec.formula = "z = c*z*(1-z) + p*z_prev";
    spec.formulaLatex = R"(z_{n+1} = c \cdot z_n(1-z_n) + p \cdot z_{n-1})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.5, 0.5);  // Different starting point for Lambda
        ComplexD z_prev(0.0, 0.0);
        ComplexD constant = isJulia ? ComplexD(1.2, -0.6) : c;
        double p = 0.3;  // Weaker feedback for Lambda variant

        for (int i = 0; i < maxIter; ++i)
        {
            // Lambda formula: c*z*(1-z)
            ComplexD one_minus_z = ComplexD(1.0, 0.0) - z;
            ComplexD lambda_term = constant * z * one_minus_z;

            ComplexD feedback = z_prev * p;
            ComplexD z_new = lambda_term + feedback;

            double magSq = z_new.real * z_new.real + z_new.imag * z_new.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);

            z_prev = z;
            z = z_new;
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
