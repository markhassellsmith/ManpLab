#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Complex Functions Family
// Fractals based on various complex function combinations
//=============================================================================

void RegisterComplexFunctionsFamily()
{
    FractalSpec spec;

    //=========================================================================
    // SqrTrig (z² + trig(z) + c)
    //=========================================================================
    spec.name = "SqrTrig";
    spec.displayName = "Square + Trig";
    spec.category = "Complex Functions";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Combination of squaring and trigonometric function: z² + sin(z) + c";
    spec.formula = "z = z² + sin(z) + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + \sin(z_n) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        const double bailout = 100.0;

        for (int i = 0; i < maxIter; ++i)
        {
            ComplexD z2 = z * z;

            // sin(z) = sin(a)cosh(b) + i*cos(a)sinh(b)
            double sin_real = std::sin(z.real) * std::cosh(z.imag);
            double sin_imag = std::cos(z.real) * std::sinh(z.imag);
            ComplexD sin_z(sin_real, sin_imag);

            z = z2 + sin_z + constant;

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
    // TrigSqr (sin(z)² + c)
    //=========================================================================
    spec.name = "TrigSqr";
    spec.displayName = "Trig Squared";
    spec.category = "Complex Functions";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Squared trigonometric function: sin(z)² + c";
    spec.formula = "z = sin(z)² + c";
    spec.formulaLatex = R"(z_{n+1} = \sin^2(z_n) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        const double bailout = 100.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // sin(z)
            double sin_real = std::sin(z.real) * std::cosh(z.imag);
            double sin_imag = std::cos(z.real) * std::sinh(z.imag);
            ComplexD sin_z(sin_real, sin_imag);

            // sin(z)²
            z = sin_z * sin_z + constant;

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
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // TrigPlusTrig (sin(z) + cos(z) + c)
    //=========================================================================
    spec.name = "TrigPlusTrig";
    spec.displayName = "Trig + Trig";
    spec.category = "Complex Functions";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Combination of sine and cosine: sin(z) + cos(z) + c";
    spec.formula = "z = sin(z) + cos(z) + c";
    spec.formulaLatex = R"(z_{n+1} = \sin(z_n) + \cos(z_n) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        const double bailout = 100.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // sin(z)
            double sin_real = std::sin(z.real) * std::cosh(z.imag);
            double sin_imag = std::cos(z.real) * std::sinh(z.imag);
            ComplexD sin_z(sin_real, sin_imag);

            // cos(z)
            double cos_real = std::cos(z.real) * std::cosh(z.imag);
            double cos_imag = -std::sin(z.real) * std::sinh(z.imag);
            ComplexD cos_z(cos_real, cos_imag);

            z = sin_z + cos_z + constant;

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
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // TrigXTrig (sin(z) * cos(z) + c)
    //=========================================================================
    spec.name = "TrigXTrig";
    spec.displayName = "Trig × Trig";
    spec.category = "Complex Functions";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Product of sine and cosine: sin(z) * cos(z) + c";
    spec.formula = "z = sin(z) * cos(z) + c";
    spec.formulaLatex = R"(z_{n+1} = \sin(z_n) \cdot \cos(z_n) + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        const double bailout = 100.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // sin(z)
            double sin_real = std::sin(z.real) * std::cosh(z.imag);
            double sin_imag = std::cos(z.real) * std::sinh(z.imag);
            ComplexD sin_z(sin_real, sin_imag);

            // cos(z)
            double cos_real = std::cos(z.real) * std::cosh(z.imag);
            double cos_imag = -std::sin(z.real) * std::sinh(z.imag);
            ComplexD cos_z(cos_real, cos_imag);

            z = sin_z * cos_z + constant;

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
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Sqr1OverTrig (1/sin(z)² + c)
    //=========================================================================
    spec.name = "Sqr1OverTrig";
    spec.displayName = "1/sin(z)²";
    spec.category = "Complex Functions";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Reciprocal squared sine: 1/sin(z)² + c";
    spec.formula = "z = 1/sin(z)² + c";
    spec.formulaLatex = R"(z_{n+1} = \frac{1}{\sin^2(z_n)} + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        const double bailout = 100.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // sin(z)
            double sin_real = std::sin(z.real) * std::cosh(z.imag);
            double sin_imag = std::cos(z.real) * std::sinh(z.imag);
            ComplexD sin_z(sin_real, sin_imag);

            // sin(z)²
            ComplexD sin_z_sqr = sin_z * sin_z;

            // Check for division by zero
            double mag2 = sin_z_sqr.real * sin_z_sqr.real + sin_z_sqr.imag * sin_z_sqr.imag;
            if (mag2 < 1e-10) break;

            // 1/sin(z)²
            ComplexD one_over_sin_sqr = ComplexD(1.0, 0.0) / sin_z_sqr;
            z = one_over_sin_sqr + constant;

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
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // ZxTrigPlusZ (z * sin(z) + z + c)
    //=========================================================================
    spec.name = "ZxTrigPlusZ";
    spec.displayName = "z·sin(z) + z";
    spec.category = "Complex Functions";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Product with trig plus linear: z * sin(z) + z + c";
    spec.formula = "z = z * sin(z) + z + c";
    spec.formulaLatex = R"(z_{n+1} = z_n \cdot \sin(z_n) + z_n + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        const double bailout = 100.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // sin(z)
            double sin_real = std::sin(z.real) * std::cosh(z.imag);
            double sin_imag = std::cos(z.real) * std::sinh(z.imag);
            ComplexD sin_z(sin_real, sin_imag);

            ComplexD z_old = z;
            z = z * sin_z + z_old + constant;

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
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Tetration (z^z + c)
    //=========================================================================
    spec.name = "Tetration";
    spec.displayName = "Tetration (z^z)";
    spec.category = "Complex Functions";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Tetration function: z^z + c";
    spec.formula = "z = z^z + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^{z_n} + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.5, 0.0);  // Start away from zero
        ComplexD constant = isJulia ? juliaC : c;
        const double bailout = 100.0;

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

            // e^(z*ln(z))
            double exp_a = std::exp(a);
            double z_to_z_real = exp_a * std::cos(b);
            double z_to_z_imag = exp_a * std::sin(b);

            z = ComplexD(z_to_z_real, z_to_z_imag) + constant;

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
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // CosTan (cos(z)/tan(z) + c)
    //=========================================================================
    spec.name = "CosTan";
    spec.displayName = "cos(z)/tan(z)";
    spec.category = "Complex Functions";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Ratio of cosine and tangent: cos(z)/tan(z) + c";
    spec.formula = "z = cos(z)/tan(z) + c";
    spec.formulaLatex = R"(z_{n+1} = \frac{\cos(z_n)}{\tan(z_n)} + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
        ComplexD constant = isJulia ? juliaC : c;
        const double bailout = 100.0;

        for (int i = 0; i < maxIter; ++i)
        {
            // cos(z)
            double cos_real = std::cos(z.real) * std::cosh(z.imag);
            double cos_imag = -std::sin(z.real) * std::sinh(z.imag);
            ComplexD cos_z(cos_real, cos_imag);

            // tan(z) = sin(z)/cos(z)
            double sin_real = std::sin(z.real) * std::cosh(z.imag);
            double sin_imag = std::cos(z.real) * std::sinh(z.imag);
            ComplexD sin_z(sin_real, sin_imag);

            double cos_mag2 = cos_z.real * cos_z.real + cos_z.imag * cos_z.imag;
            if (cos_mag2 < 1e-10) break;

            ComplexD tan_z = sin_z / cos_z;

            double tan_mag2 = tan_z.real * tan_z.real + tan_z.imag * tan_z.imag;
            if (tan_mag2 < 1e-10) break;

            z = cos_z / tan_z + constant;

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
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);
}

} // namespace Native
