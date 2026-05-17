#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Extended Julia Sets Family
// Additional Julia set variations and presets beyond the core ones
//=============================================================================

void RegisterExtendedJuliaFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Julia - Dendrite
    //=========================================================================
    spec.name = "JuliaDendrite";
    spec.displayName = "Julia - Dendrite";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set with dendrite-like branching structures at c ≈ i";
    spec.formula = "z = z² + c, where c ≈ (0, 1)";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \quad c \approx i)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Force Julia mode with dendrite constant
        return MandelbrotCalculator::CalculateSmoothIterations(c, maxIter, true, ComplexD(0.0, 1.0));
    };

    spec.supportsJulia = false;  // Fixed Julia constant
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Julia - Siegel Disk
    //=========================================================================
    spec.name = "JuliaSiegelDisk";
    spec.displayName = "Julia - Siegel Disk";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set with Siegel disk at c ≈ -0.390541 - 0.586788i";
    spec.formula = "z = z² + c, where c is golden ratio point";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \quad c = e^{2\pi i \phi})";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Siegel disk constant (golden ratio point)
        return MandelbrotCalculator::CalculateSmoothIterations(c, maxIter, true, ComplexD(-0.390541, -0.586788));
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Julia - Dragon
    //=========================================================================
    spec.name = "JuliaDragon";
    spec.displayName = "Julia - Dragon";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set with dragon-like shape at c ≈ -0.8 + 0.156i";
    spec.formula = "z = z² + c, where c creates dragon shape";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \quad c \approx -0.8 + 0.156i)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        return MandelbrotCalculator::CalculateSmoothIterations(c, maxIter, true, ComplexD(-0.8, 0.156));
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Julia - Spiral
    //=========================================================================
    spec.name = "JuliaSpiral";
    spec.displayName = "Julia - Spiral";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set with spiral arms at c ≈ -0.75 + 0.11i";
    spec.formula = "z = z² + c, where c creates spiral patterns";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \quad c \approx -0.75 + 0.11i)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        return MandelbrotCalculator::CalculateSmoothIterations(c, maxIter, true, ComplexD(-0.75, 0.11));
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Julia - Custom (User-defined c)
    //=========================================================================
    spec.name = "JuliaCustom";
    spec.displayName = "Julia - Custom";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set with user-defined c parameter";
    spec.formula = "z = z² + c, where c is user-specified";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Use juliaC if provided, otherwise default to interesting point
        ComplexD actualC = (juliaC.real != 0.0 || juliaC.imag != 0.0) ? juliaC : ComplexD(-0.7, 0.27);
        return MandelbrotCalculator::CalculateSmoothIterations(c, maxIter, true, actualC);
    };

    spec.supportsJulia = true;  // Allows user customization
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Lambda Julia
    //=========================================================================
    spec.name = "LambdaJulia";
    spec.displayName = "Julia - Lambda (Alt)";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set for lambda iteration z = c*z*(1-z)";
    spec.formula = "z = c * z * (1 - z)";
    spec.formulaLatex = R"(z_{n+1} = c \cdot z_n \cdot (1 - z_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Pure Julia set with lambda = 2.8 + 0.9i (fractal tendrils and loops)
        ComplexD z = c;
        ComplexD lambda = ComplexD(2.8, 0.9);

        for (int i = 0; i < maxIter; ++i)
        {
            // z = λ * z * (1 - z)
            ComplexD one_minus_z(1.0 - z.real, -z.imag);
            z = lambda * z * one_minus_z;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(2.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;  // Pure Julia set, not toggleable
    spec.defaultCenterX = 0.5;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 3.0;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Multibrot Julia (Power 3)
    //=========================================================================
    spec.name = "Multibrot3Julia";
    spec.displayName = "Julia - Multibrot 3";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set for z³ + c";
    spec.formula = "z = z³ + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^3 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z = c;
        ComplexD constant = isJulia ? juliaC : ComplexD(-0.5, 0.5);

        for (int i = 0; i < maxIter; ++i)
        {
            // z³ = (a+bi)³ = a³ - 3ab² + i(3a²b - b³)
            double a = z.real;
            double b = z.imag;
            z = ComplexD(a*a*a - 3.0*a*b*b, 3.0*a*a*b - b*b*b) + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(3.0);
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
    // Multibrot Julia (Power 4)
    //=========================================================================
    spec.name = "Multibrot4Julia";
    spec.displayName = "Julia - Multibrot 4";
    spec.category = "Julia Sets";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Julia set for z⁴ + c";
    spec.formula = "z = z⁴ + c";
    spec.formulaLatex = R"(z_{n+1} = z_n^4 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Pure Julia set with c = 0.484 + 0.467i (intricate quatrefoil/4-fold structure)
        ComplexD z = c;
        ComplexD constant = ComplexD(0.484, 0.467);

        for (int i = 0; i < maxIter; ++i)
        {
            // z⁴ = (z²)²
            ComplexD z2(z.real*z.real - z.imag*z.imag, 2.0*z.real*z.imag);
            z = ComplexD(z2.real*z2.real - z2.imag*z2.imag, 2.0*z2.real*z2.imag) + constant;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > 256.0)
                return i + 1.0 - std::log(std::log(std::sqrt(magSq))) / std::log(4.0);
        }
        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;  // Pure Julia set, not toggleable
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.2;
    spec.defaultBailout = 256.0;

    FractalRegistry::Register(spec);

    // TODO: Add more Julia variants:
    // - Phoenix Julia sets
    // - Magnet Julia sets
    // - Other famous Julia parameter values
}

} // namespace Native
