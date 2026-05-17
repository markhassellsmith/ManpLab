#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Hybrid Fractal Family
// Fractals that combine multiple iteration formulas or alternate between them
// These create unique patterns by mixing different mathematical approaches
//=============================================================================

void RegisterHybridFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Mandelbrot-Burning Ship Hybrid (alternating iterations)
    //=========================================================================
    spec.name = "MandelBurningHybrid";
    spec.displayName = "Mandelbrot-Burning Ship Hybrid";
    spec.category = "Hybrid Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Alternates between Mandelbrot (z² + c) and Burning Ship (|z|² + c) iterations";
    spec.formula = "Even iterations: z² + c, Odd iterations: (|Re(z)| + i|Im(z)|)² + c";
    spec.formulaLatex = R"(z_{n+1} = \begin{cases} z_n^2 + c & n \text{ even} \\ (|Re(z_n)| + i|Im(z_n)|)^2 + c & n \text{ odd} \end{cases})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            if (i % 2 == 0)
            {
                // Mandelbrot: z = z² + c
                ComplexD z2 = z * z;
                z = z2 + constant;
            }
            else
            {
                // Burning Ship: z = (|x| + i|y|)² + c
                double abs_x = std::abs(z.real);
                double abs_y = std::abs(z.imag);
                double real_part = abs_x * abs_x - abs_y * abs_y;
                double imag_part = 2.0 * abs_x * abs_y;
                z = ComplexD(real_part, imag_part) + constant;
            }

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
    // Mandelbrot-Lambda Mix (z² + λ*z*(1-z))
    //=========================================================================
    spec.name = "MandelLambdaMix";
    spec.displayName = "Mandelbrot-Lambda Mix";
    spec.category = "Hybrid Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Combines Mandelbrot and Lambda formulas: z = z² + c + λ*z*(1-z)";
    spec.formula = "z = z² + c + λ*z*(1-z), where λ = 0.5";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c + \lambda z_n(1-z_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        ComplexD lambda(0.5, 0.0);  // Mix parameter
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // z = z² + c + λ*z*(1-z)
            ComplexD z2 = z * z;
            ComplexD lambda_term = lambda * z * (ComplexD(1.0, 0.0) - z);
            z = z2 + constant + lambda_term;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = -0.75;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.480942;  // Viewport tuning: X scale 8.32, Y scale 4.68
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Tricorn-Phoenix Hybrid (alternating)
    //=========================================================================
    spec.name = "TricornPhoenixHybrid";
    spec.displayName = "Tricorn-Phoenix Hybrid";
    spec.category = "Hybrid Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Alternates between Tricorn (conjugate) and Phoenix iterations";
    spec.formula = "Alternates: z̄² + c and z² + c + p*z_prev";
    spec.formulaLatex = R"(z_{n+1} = \begin{cases} \bar{z_n}^2 + c & n \text{ even} \\ z_n^2 + c + p \cdot z_{n-1} & n \text{ odd} \end{cases})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD z_prev(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        ComplexD p(0.5, 0.0);  // Phoenix parameter
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            ComplexD z_new;

            if (i % 2 == 0)
            {
                // Tricorn: conjugate before squaring
                ComplexD z_conj(z.real, -z.imag);
                z_new = z_conj * z_conj + constant;
            }
            else
            {
                // Phoenix: z² + c + p*z_prev
                z_new = z * z + constant + p * z_prev;
            }

            z_prev = z;
            z = z_new;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = -0.58;
    spec.defaultCenterY = -0.11;
    spec.defaultZoom = 2.206981;  // Viewport tuning: X scale 1.81, Y scale 1.02
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Newton-Mandelbrot Blend
    //=========================================================================
    spec.name = "NewtonMandelBlend";
    spec.displayName = "Newton-Mandelbrot Blend";
    spec.category = "Hybrid Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Blends Newton method (z³-1) with Mandelbrot iteration";
    spec.formula = "z = α*(z - (z³-1)/(3z²)) + (1-α)*(z² + c), α=0.5";
    spec.formulaLatex = R"(z_{n+1} = \alpha \left(z_n - \frac{z_n^3-1}{3z_n^2}\right) + (1-\alpha)(z_n^2 + c))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        ComplexD constant = isJulia ? juliaC : c;
        const double alpha = 0.5;  // Blend factor
        const double bailout = 100.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // Newton step for z³ - 1
            ComplexD z2 = z * z;
            ComplexD z3 = z2 * z;
            ComplexD numerator = z3 - ComplexD(1.0, 0.0);
            ComplexD denominator = z2 * 3.0;

            double denom_mag2 = denominator.real * denominator.real + 
                               denominator.imag * denominator.imag;

            ComplexD newton_step = z;
            if (denom_mag2 > 1e-10)
            {
                ComplexD ratio = numerator / denominator;
                newton_step = z - ratio;
            }

            // Mandelbrot step
            ComplexD mandel_step = z2 + constant;

            // Blend
            z = newton_step * alpha + mandel_step * (1.0 - alpha);

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
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Sine-Mandelbrot Hybrid (z = sin(z²) + c)
    //=========================================================================
    spec.name = "SineMandelHybrid";
    spec.displayName = "Sine-Mandelbrot Hybrid";
    spec.category = "Hybrid Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Combines sine and squaring: z = sin(z²) + c";
    spec.formula = "z = sin(z²) + c";
    spec.formulaLatex = R"(z_{n+1} = \sin(z_n^2) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        const double bailout = 100.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // z² first
            ComplexD z2 = z * z;

            // sin(z²) = sin(a+bi) = sin(a)cosh(b) + i*cos(a)sinh(b)
            double sin_real = std::sin(z2.real) * std::cosh(z2.imag);
            double sin_imag = std::cos(z2.real) * std::sinh(z2.imag);
            ComplexD sin_z2(sin_real, sin_imag);

            z = sin_z2 + constant;

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
    // Exponential-Mandelbrot Blend (z = α*e^z + (1-α)*z² + c)
    //=========================================================================
    spec.name = "ExpMandelBlend";
    spec.displayName = "Exponential-Mandelbrot Blend";
    spec.category = "Hybrid Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Blends exponential and Mandelbrot: z = α*e^z + (1-α)*z² + c";
    spec.formula = "z = α*e^z + (1-α)*z² + c, α=0.3";
    spec.formulaLatex = R"(z_{n+1} = \alpha e^{z_n} + (1-\alpha)z_n^2 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        const double alpha = 0.3;  // Blend factor (less exponential for stability)
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // Exponential part: e^z = e^(a+bi) = e^a * (cos(b) + i*sin(b))
            double exp_real = std::exp(z.real);
            double exp_z_real = exp_real * std::cos(z.imag);
            double exp_z_imag = exp_real * std::sin(z.imag);
            ComplexD exp_z(exp_z_real, exp_z_imag);

            // Mandelbrot part: z²
            ComplexD z2 = z * z;

            // Blend
            z = exp_z * alpha + z2 * (1.0 - alpha) + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
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
    // Multi-Power Cycle (z² → z³ → z⁴ → repeat)
    //=========================================================================
    spec.name = "MultiPowerCycle";
    spec.displayName = "Multi-Power Cycle";
    spec.category = "Hybrid Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Cycles through powers: z², z³, z⁴, then repeats";
    spec.formula = "z = z^n + c, where n cycles through 2, 3, 4";
    spec.formulaLatex = R"(z_{n+1} = z_n^{p_n} + c, \; p_n \in \{2,3,4\} \text{ (cycling)})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            int power = 2 + (i % 3);  // Cycles: 2, 3, 4, 2, 3, 4, ...

            ComplexD z_power;
            if (power == 2)
            {
                z_power = z * z;
            }
            else if (power == 3)
            {
                ComplexD z2 = z * z;
                z_power = z2 * z;
            }
            else  // power == 4
            {
                ComplexD z2 = z * z;
                z_power = z2 * z2;
            }

            z = z_power + constant;

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
    // Magnet-Mandelbrot Hybrid
    //=========================================================================
    spec.name = "MagnetMandelHybrid";
    spec.displayName = "Magnet-Mandelbrot Hybrid";
    spec.category = "Hybrid Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Alternates between Magnet I and Mandelbrot iterations";
    spec.formula = "Alternates: ((z²+c-1)/(2z+c-2))² and z²+c";
    spec.formulaLatex = R"(z_{n+1} = \begin{cases} \left(\frac{z_n^2+c-1}{2z_n+c-2}\right)^2 & n \text{ even} \\ z_n^2 + c & n \text{ odd} \end{cases})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            if (i % 2 == 0)
            {
                // Magnet I: ((z² + c - 1) / (2z + c - 2))²
                ComplexD z2 = z * z;
                ComplexD numerator = z2 + constant - ComplexD(1.0, 0.0);
                ComplexD denominator = z * 2.0 + constant - ComplexD(2.0, 0.0);

                double denom_mag2 = denominator.real * denominator.real + 
                                   denominator.imag * denominator.imag;
                if (denom_mag2 < 1e-10)
                    break;

                ComplexD ratio = numerator / denominator;
                z = ratio * ratio;
            }
            else
            {
                // Mandelbrot: z² + c
                z = z * z + constant;
            }

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
    // Collatz-Style Hybrid (z² if even iteration, z³ if odd)
    //=========================================================================
    spec.name = "CollatzHybrid";
    spec.displayName = "Collatz-Style Hybrid";
    spec.category = "Hybrid Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Inspired by Collatz: z² on even iterations, z³ on odd iterations";
    spec.formula = "Even: z² + c, Odd: z³ + c";
    spec.formulaLatex = R"(z_{n+1} = \begin{cases} z_n^2 + c & n \text{ even} \\ z_n^3 + c & n \text{ odd} \end{cases})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            if (i % 2 == 0)
            {
                // z² + c
                z = z * z + constant;
            }
            else
            {
                // z³ + c
                ComplexD z2 = z * z;
                z = z2 * z + constant;
            }

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
    // Celtic-Burning Hybrid
    //=========================================================================
    spec.name = "CelticBurningHybrid";
    spec.displayName = "Celtic-Burning Ship Hybrid";
    spec.category = "Hybrid Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Combines Celtic (abs real part) with Burning Ship (abs both)";
    spec.formula = "z = (|Re(z²)| + i*Im(z²)) + abs(c) on even, full abs on odd";
    spec.formulaLatex = R"(z_{n+1} = \begin{cases} |Re(z_n^2)| + i \cdot Im(z_n^2) + c & n \text{ even} \\ (|Re(z_n)| + i|Im(z_n)|)^2 + c & n \text{ odd} \end{cases})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        const double bailout = 256.0;

        for (int i = 0; i < maxIter; ++i)
        {
            if (i % 2 == 0)
            {
                // Celtic: abs real part after squaring
                ComplexD z2 = z * z;
                z = ComplexD(std::abs(z2.real), z2.imag) + constant;
            }
            else
            {
                // Burning Ship: abs before squaring
                double abs_x = std::abs(z.real);
                double abs_y = std::abs(z.imag);
                double real_part = abs_x * abs_x - abs_y * abs_y;
                double imag_part = 2.0 * abs_x * abs_y;
                z = ComplexD(real_part, imag_part) + constant;
            }

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
