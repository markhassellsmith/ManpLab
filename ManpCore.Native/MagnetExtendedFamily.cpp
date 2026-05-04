#include "FractalRegistry.h"
#include <cmath>

namespace Native {

//=============================================================================
// Magnet Extended Family
// Extended Magnet fractals with Julia modes and power variations
// Based on rational functions from theoretical physics (Ising model)
//=============================================================================

void RegisterMagnetExtendedFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Magnet 1 Julia Mode
    //=========================================================================
    spec.name = "Magnet1J";
    spec.displayName = "Magnet I Julia";
    spec.category = "Magnet Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Magnet I in Julia mode with fixed parameter c = 1 + 0i (interesting structure)";
    spec.formula = "z = ((z² + c - 1) / (2z + c - 2))²";
    spec.formulaLatex = R"(z_{n+1} = \left(\frac{z_n^2 + c - 1}{2z_n + c - 2}\right)^2)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;  // Julia mode: z starts at pixel location
        ComplexD constant = ComplexD(1.0, 0.0);  // Classic Magnet Julia parameter
        const double bailout = 1000.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // Calculate z²
            ComplexD z2 = z * z;

            // Numerator: z² + c - 1
            ComplexD numerator = z2 + constant - ComplexD(1.0, 0.0);

            // Denominator: 2z + c - 2
            ComplexD denominator = z * 2.0 + constant - ComplexD(2.0, 0.0);

            // Check for division by zero
            double denom_mag2 = denominator.real * denominator.real + 
                               denominator.imag * denominator.imag;
            if (denom_mag2 < 1e-10) break;

            // Divide: (z² + c - 1) / (2z + c - 2)
            ComplexD ratio = numerator / denominator;

            // Square the result
            z = ratio * ratio;

            // Bailout test
            double magnitude2 = z.real * z.real + z.imag * z.imag;
            if (magnitude2 > bailout)
            {
                double log_zn = std::log(magnitude2) / 2.0;
                double nu = std::log(log_zn / std::log(2.0)) / std::log(2.0);
                return i + 1.0 - nu;
            }
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 1000.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Magnet 2 Julia Mode
    //=========================================================================
    spec.name = "Magnet2J";
    spec.displayName = "Magnet II Julia";
    spec.category = "Magnet Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Magnet II in Julia mode with cubic rational function";
    spec.formula = "z = ((z³ + 3(c-1)z + (c-1)(c-2)) / (3z² + 3(c-2)z + (c-1)(c-2) + 1))²";
    spec.formulaLatex = R"(z_{n+1} = \left(\frac{z_n^3 + 3(c-1)z_n + (c-1)(c-2)}{3z_n^2 + 3(c-2)z_n + (c-1)(c-2) + 1}\right)^2)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        ComplexD constant = ComplexD(1.5, 0.0);  // Good Julia parameter for Magnet 2
        const double bailout = 1000.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // Pre-calculate (c - 1) and (c - 2)
            ComplexD c_minus_1 = constant - ComplexD(1.0, 0.0);
            ComplexD c_minus_2 = constant - ComplexD(2.0, 0.0);
            ComplexD c1c2 = c_minus_1 * c_minus_2;

            // Calculate z² and z³
            ComplexD z2 = z * z;
            ComplexD z3 = z2 * z;

            // Numerator: z³ + 3(c-1)z + (c-1)(c-2)
            ComplexD numerator = z3 + (c_minus_1 * z * 3.0) + c1c2;

            // Denominator: 3z² + 3(c-2)z + (c-1)(c-2) + 1
            ComplexD denominator = (z2 * 3.0) + (c_minus_2 * z * 3.0) + c1c2 + ComplexD(1.0, 0.0);

            // Check for division by zero
            double denom_mag2 = denominator.real * denominator.real + 
                               denominator.imag * denominator.imag;
            if (denom_mag2 < 1e-10) break;

            // Divide and square
            ComplexD ratio = numerator / denominator;
            z = ratio * ratio;

            // Bailout test
            double magnitude2 = z.real * z.real + z.imag * z.imag;
            if (magnitude2 > bailout)
            {
                double log_zn = std::log(magnitude2) / 2.0;
                double nu = std::log(log_zn / std::log(2.0)) / std::log(2.0);
                return i + 1.0 - nu;
            }
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.5;
    spec.defaultBailout = 1000.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Magnet 1 Power 3 (Cubic variant)
    //=========================================================================
    spec.name = "Magnet1Power3";
    spec.displayName = "Magnet I Cubic";
    spec.category = "Magnet Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Magnet I with cubic power: ((z² + c - 1) / (2z + c - 2))³";
    spec.formula = "z = ((z² + c - 1) / (2z + c - 2))³";
    spec.formulaLatex = R"(z_{n+1} = \left(\frac{z_n^2 + c - 1}{2z_n + c - 2}\right)^3)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? ComplexD(1.2, 0.0) : c;
        const double bailout = 1000.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // Calculate z²
            ComplexD z2 = z * z;

            // Numerator: z² + c - 1
            ComplexD numerator = z2 + constant - ComplexD(1.0, 0.0);

            // Denominator: 2z + c - 2
            ComplexD denominator = z * 2.0 + constant - ComplexD(2.0, 0.0);

            // Check for division by zero
            double denom_mag2 = denominator.real * denominator.real + 
                               denominator.imag * denominator.imag;
            if (denom_mag2 < 1e-10) break;

            // Divide
            ComplexD ratio = numerator / denominator;

            // Cube the result: ratio³ = ratio * ratio²
            ComplexD ratio2 = ratio * ratio;
            z = ratio * ratio2;

            // Bailout test
            double magnitude2 = z.real * z.real + z.imag * z.imag;
            if (magnitude2 > bailout)
            {
                double log_zn = std::log(magnitude2) / 2.0;
                double nu = std::log(log_zn / std::log(2.0)) / std::log(2.0);
                return i + 1.0 - nu;
            }
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.5;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.7;
    spec.defaultBailout = 1000.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Magnet 2 Power 3 (Cubic variant)
    //=========================================================================
    spec.name = "Magnet2Power3";
    spec.displayName = "Magnet II Cubic";
    spec.category = "Magnet Fractals";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Magnet II with cubic power: (rational function)³";
    spec.formula = "z = ((z³ + 3(c-1)z + (c-1)(c-2)) / (3z² + 3(c-2)z + (c-1)(c-2) + 1))³";
    spec.formulaLatex = R"(z_{n+1} = \left(\frac{z_n^3 + 3(c-1)z_n + (c-1)(c-2)}{3z_n^2 + 3(c-2)z_n + (c-1)(c-2) + 1}\right)^3)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? ComplexD(1.8, 0.0) : c;
        const double bailout = 1000.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // Pre-calculate (c - 1) and (c - 2)
            ComplexD c_minus_1 = constant - ComplexD(1.0, 0.0);
            ComplexD c_minus_2 = constant - ComplexD(2.0, 0.0);
            ComplexD c1c2 = c_minus_1 * c_minus_2;

            // Calculate z² and z³
            ComplexD z2 = z * z;
            ComplexD z3 = z2 * z;

            // Numerator: z³ + 3(c-1)z + (c-1)(c-2)
            ComplexD numerator = z3 + (c_minus_1 * z * 3.0) + c1c2;

            // Denominator: 3z² + 3(c-2)z + (c-1)(c-2) + 1
            ComplexD denominator = (z2 * 3.0) + (c_minus_2 * z * 3.0) + c1c2 + ComplexD(1.0, 0.0);

            // Check for division by zero
            double denom_mag2 = denominator.real * denominator.real + 
                               denominator.imag * denominator.imag;
            if (denom_mag2 < 1e-10) break;

            // Divide
            ComplexD ratio = numerator / denominator;

            // Cube the result
            ComplexD ratio2 = ratio * ratio;
            z = ratio * ratio2;

            // Bailout test
            double magnitude2 = z.real * z.real + z.imag * z.imag;
            if (magnitude2 > bailout)
            {
                double log_zn = std::log(magnitude2) / 2.0;
                double nu = std::log(log_zn / std::log(2.0)) / std::log(2.0);
                return i + 1.0 - nu;
            }
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 1.5;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.6;
    spec.defaultBailout = 1000.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);
}

} // namespace Native
