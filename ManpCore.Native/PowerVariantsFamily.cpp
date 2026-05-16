#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Power Variants Family
// Higher-power Mandelbrot and Julia sets (z^n + c for n > 3)
//=============================================================================

// Helper to compute z^n efficiently
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

void RegisterPowerVariantsFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Multibrot Power 6
    //=========================================================================
    spec.name = "Multibrot6";
    spec.displayName = "Multibrot (Power 6)";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot set with power 6: z⁶ + c";
    spec.formula = "z = z⁶ + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^6 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            z = ComplexPower(z, 6) + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(6.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Multibrot Power 7
    //=========================================================================
    spec.name = "Multibrot7";
    spec.displayName = "Multibrot (Power 7)";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot set with power 7: z⁷ + c";
    spec.formula = "z = z⁷ + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^7 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            z = ComplexPower(z, 7) + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(7.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Multibrot Power 8
    //=========================================================================
    spec.name = "Multibrot8";
    spec.displayName = "Multibrot (Power 8)";
    spec.category = "Mandelbrot Variants";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Mandelbrot set with power 8: z⁸ + c";
    spec.formula = "z = z⁸ + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^8 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            z = ComplexPower(z, 8) + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(8.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Julia Power 5
    //=========================================================================
    spec.name = "Julia5";
    spec.displayName = "Julia - Power 5";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set with power 5: z⁵ + c";
    spec.formula = "z = z⁵ + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^5 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;  // Always Julia
        ComplexD constant = ComplexD(0.4, 0.3);  // Interesting c value

        for (int i = 0; i < maxIter; ++i)
        {
            z = ComplexPower(z, 5) + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(5.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;  // Fixed Julia mode
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Julia Power 6
    //=========================================================================
    spec.name = "Julia6";
    spec.displayName = "Julia - Power 6";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set with power 6: z⁶ + c";
    spec.formula = "z = z⁶ + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^6 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        ComplexD constant = ComplexD(0.3, 0.4);

        for (int i = 0; i < maxIter; ++i)
        {
            z = ComplexPower(z, 6) + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(6.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Burning Ship Power 3
    //=========================================================================
    spec.name = "BurningShip3";
    spec.displayName = "Burning Ship (Power 3)";
    spec.category = "Burning Ship Family";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Burning Ship with power 3";
    spec.formula = "z = (|Re(z)| + i|Im(z)|)³ + c";
    spec.formulaLatex = R"(z_{n+1} = (|\text{Re}(z_n)| + i|\text{Im}(z_n)|)^3 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            // Take absolute values
            ComplexD abs_z(std::abs(z.real), std::abs(z.imag));

            // Cube it
            z = ComplexPower(abs_z, 3) + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(3.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;  // Center at origin
    spec.defaultCenterY = 0.0;  // Center at origin
    spec.defaultZoom = 0.6164;  // Viewport: 4.866875 × 2.737617
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Burning Ship Power 4
    //=========================================================================
    spec.name = "BurningShip4";
    spec.displayName = "Burning Ship (Power 4)";
    spec.category = "Burning Ship Family";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Burning Ship with power 4";
    spec.formula = "z = (|Re(z)| + i|Im(z)|)⁴ + c";
    spec.formulaLatex = R"(z_{n+1} = (|\text{Re}(z_n)| + i|\text{Im}(z_n)|)^4 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            ComplexD abs_z(std::abs(z.real), std::abs(z.imag));
            z = ComplexPower(abs_z, 4) + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(4.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;  // Center at origin
    spec.defaultCenterY = 0.0;  // Center at origin
    spec.defaultZoom = 0.6;     // Viewport: 5.0000 × 2.8125
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Tricorn Power 3
    //=========================================================================
    spec.name = "Tricorn3";
    spec.displayName = "Tricorn (Power 3)";
    spec.category = "Tricorn Family";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Tricorn (Mandelbar) with power 3";
    spec.formula = "z = conj(z)³ + c";
    spec.formulaLatex = R"(z_{n+1} = \overline{z_n}^3 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            // Conjugate then cube
            ComplexD conj_z(z.real, -z.imag);
            z = ComplexPower(conj_z, 3) + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(3.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.2;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Tricorn Power 4
    //=========================================================================
    spec.name = "Tricorn4";
    spec.displayName = "Tricorn (Power 4)";
    spec.category = "Tricorn Family";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Tricorn (Mandelbar) with power 4";
    spec.formula = "z = conj(z)⁴ + c";
    spec.formulaLatex = R"(z_{n+1} = \overline{z_n}^4 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;

        for (int i = 0; i < maxIter; ++i)
        {
            ComplexD conj_z(z.real, -z.imag);
            z = ComplexPower(conj_z, 4) + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(4.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.2;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    // TODO: Add more power variants:
    // - Higher powers (9, 10, etc.)
    // - Fractional powers (z^2.5, z^3.7, etc.)
    // - Negative powers (z^-2, z^-3, etc.)
    // - Mixed formulas (z^n + z^m + c)
}

} // namespace Native
