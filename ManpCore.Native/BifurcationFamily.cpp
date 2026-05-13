#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Bifurcation Family
// Bifurcation diagrams and parameter space visualizations
//=============================================================================

void RegisterBifurcationFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Logistic Parameter Space
    //=========================================================================
    spec.name = "LogisticParameterSpace";
    spec.displayName = "Logistic Parameter Space";
    spec.category = "Bifurcation";
    spec.type = FractalCategory::Sequence2D;
    spec.description = "Parameter space visualization for the logistic map: xₙ₊₁ = r·xₙ·(1-xₙ). Shows averaged attractor behavior across parameter space. (Note: bifurcation diagrams require specialized rendering)";
    spec.formula = "x = r·x·(1-x)";
    spec.formulaLatex = R"(x_{n+1} = r \cdot x_n \cdot (1 - x_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // For parameter space: c.real = parameter r, c.imag = secondary parameter
        double r = c.real;
        double x = 0.5;  // Standard initial value
        const int transient = 200;  // Skip transient behavior

        // Run transient iterations
        for (int i = 0; i < transient; ++i)
        {
            x = r * x * (1.0 - x);
        }

        // Sample final behavior
        double sum = 0.0;
        int samples = maxIter < 50 ? maxIter : 50;
        for (int i = 0; i < samples; ++i)
        {
            x = r * x * (1.0 - x);
            sum += x;
        }

        // Return average to show attractors
        return sum / samples * maxIter;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 2.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.697;  // Viewport of approximately 4.303873 by 2.420929
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Lambda Parameter Space
    //=========================================================================
    spec.name = "LambdaParameterSpace";
    spec.displayName = "Lambda Parameter Space";
    spec.category = "Bifurcation";
    spec.type = FractalCategory::Sequence2D;
    spec.description = "Parameter space visualization for the complex lambda map: z = λ·z·(1-z). Shows averaged attractor behavior across parameter space. (Note: bifurcation diagrams require specialized rendering)";
    spec.formula = "z = λ·z·(1-z)";
    spec.formulaLatex = R"(z_{n+1} = \lambda \cdot z_n \cdot (1 - z_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD lambda = c;
        ComplexD z(0.5, 0.0);
        const double bailout = 100.0;
        const int transient = 50;

        // Run transient
        for (int i = 0; i < transient; ++i)
        {
            z = lambda * z * (ComplexD(1.0, 0.0) - z);
            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout) return 0.0;
        }

        // Sample final behavior
        double sum = 0.0;
        int samples = maxIter < 50 ? maxIter : 50;
        for (int i = 0; i < samples; ++i)
        {
            z = lambda * z * (ComplexD(1.0, 0.0) - z);
            sum += std::sqrt(z.real * z.real + z.imag * z.imag);

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout) return i + 1.0;
        }

        return sum / samples * maxIter;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 1.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.536203;  // Viewport of approximately 5.596998 by 3.148312
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Mandelbrot Parameter Space
    //=========================================================================
    spec.name = "MandelParameter";
    spec.displayName = "Mandelbrot Parameter Space";
    spec.category = "Bifurcation";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Parameter space visualization showing periodicity and stability for z = z² + c";
    spec.formula = "z = z² + c, showing parameter stability";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;

        // Track period detection
        int lastPeriod = -1;
        ComplexD lastZ = z;

        for (int i = 0; i < maxIter; ++i)
        {
            z = z * z + c;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0;

            // Simple period detection
            double diff = std::sqrt((z.real - lastZ.real) * (z.real - lastZ.real) + 
                                   (z.imag - lastZ.imag) * (z.imag - lastZ.imag));
            if (diff < 0.001 && i > 10)
            {
                return maxIter - (i - lastPeriod);
            }

            if (i % 10 == 0)
                lastZ = z;
        }

        return static_cast<double>(maxIter);
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Henon Map Parameter Space (2D discrete dynamical system)
    //=========================================================================
    spec.name = "HenonParameterSpace";
    spec.displayName = "Henon Parameter Space";
    spec.category = "Bifurcation";
    spec.type = FractalCategory::Sequence2D;
    spec.description = "Parameter space visualization for the Hénon map: xₙ₊₁ = 1 - a·xₙ² + yₙ; yₙ₊₁ = b·xₙ. Shows averaged attractor behavior across parameter space. (Note: bifurcation diagrams require specialized rendering)";
    spec.formula = "xₙ₊₁ = 1 - a·xₙ² + yₙ; yₙ₊₁ = b·xₙ";
    spec.formulaLatex = R"(x_{n+1} = 1 - a \cdot x_n^2 + y_n, \; y_{n+1} = b \cdot x_n)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // c.real = a, c.imag = b
        double a = c.real;
        double b = c.imag;
        double x = 0.1;
        double y = 0.1;
        const int transient = 100;

        // Run transient
        for (int i = 0; i < transient; ++i)
        {
            double x_new = 1.0 - a * x * x + y;
            y = b * x;
            x = x_new;
        }

        // Sample behavior
        double sum = 0.0;
        int samples = maxIter < 50 ? maxIter : 50;
        for (int i = 0; i < samples; ++i)
        {
            double x_new = 1.0 - a * x * x + y;
            y = b * x;
            x = x_new;
            sum += std::sqrt(x * x + y * y);
        }

        return sum / samples * maxIter;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.75;
    spec.defaultCenterY = -0.25;
    spec.defaultZoom = 1.0;  // Viewport of exactly 3.0000 by 1.6875
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Orbit Diagram
    //=========================================================================
    spec.name = "OrbitDiagram";
    spec.displayName = "Orbit Diagram";
    spec.category = "Bifurcation";
    spec.type = FractalCategory::EscapeTime2D;
    spec.description = "Orbit visualization showing the trajectory of points for z = z² + c";
    spec.formula = "z = z² + c, showing orbit paths";
    spec.formulaLatex = R"(z_{n+1} = z_n^2 + c)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        ComplexD z(0.0, 0.0);
        const double bailout = 256.0;

        double orbitLength = 0.0;
        ComplexD lastZ = z;

        for (int i = 0; i < maxIter; ++i)
        {
            z = z * z + c;

            double magSq = z.real * z.real + z.imag * z.imag;
            if (magSq > bailout)
                return i + 1.0;

            // Accumulate orbit length
            orbitLength += std::sqrt((z.real - lastZ.real) * (z.real - lastZ.real) + 
                                    (z.imag - lastZ.imag) * (z.imag - lastZ.imag));
            lastZ = z;
        }

        // Return orbit length as visualization metric
        return orbitLength;
    };

    spec.supportsJulia = true;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // May Lyapunov Reference
    //=========================================================================
    spec.name = "MayLyapunovRef";
    spec.displayName = "May-Lyapunov Reference";
    spec.category = "Bifurcation";
    spec.type = FractalCategory::Sequence2D;
    spec.description = "Lyapunov exponent visualization for the May logistic map: xₙ₊₁ = r·xₙ·(1-xₙ). Maps stability across parameter space via Lyapunov exponent calculation.";
    spec.formula = "xₙ₊₁ = r·xₙ·(1-xₙ), showing stability via Lyapunov exponent";
    spec.formulaLatex = R"(x_{n+1} = r \cdot x_n \cdot (1 - x_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        double r = c.real;
        double x = 0.5;
        const int transient = 200;

        // Run transient
        for (int i = 0; i < transient; ++i)
        {
            x = r * x * (1.0 - x);
            if (x < 0.0 || x > 1.0) return 0.0;
        }

        // Calculate Lyapunov exponent
        double lyapunov = 0.0;
        int samples = maxIter < 100 ? maxIter : 100;
        for (int i = 0; i < samples; ++i)
        {
            x = r * x * (1.0 - x);
            if (x < 1e-10 || x > 1.0) break;

            double derivative = r * (1.0 - 2.0 * x);
            if (std::abs(derivative) > 1e-10)
                lyapunov += std::log(std::abs(derivative));
        }

        lyapunov /= samples;

        // Map to visualization range
        return (lyapunov + 1.0) * maxIter * 0.5;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 2.0;  // Center of interesting parameter range
    spec.defaultCenterY = 0.0;  // Centered vertically
    spec.defaultZoom = 0.3;     // Viewport: 10.000 × 5.625
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);
}

} // namespace Native
