#include "FractalRegistry.h"
#include "InitialConditionsService.h"
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
        InitialConditions ic;  // Declare ONCE at the top

        // =========================================================================
        // REMOVED: LogisticParameterSpace - 1D system creates boring vertical stripes
        // REMOVED: MayLyapunovRef - 1D system creates boring vertical stripes
        // See BIFURCATION_DIAGRAM_IMPLEMENTATION_PLAN.md for true bifurcation diagrams
        // =========================================================================

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
        //spec.defaultCenterX = 1.0;
        //spec.defaultCenterY = 0.0;
        //spec.defaultZoom = 0.536203;  // Viewport of approximately 5.596998 by 3.148312
        ic = InitialConditionsService::Get("LambdaParameterSpace");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
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
        //spec.defaultCenterX = 0.0;
        //spec.defaultCenterY = 0.0;
        //spec.defaultZoom = 1.0;
        ic = InitialConditionsService::Get("MandelParameter");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
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
        //spec.defaultCenterX = 0.75;
        //spec.defaultCenterY = -0.25;
        //spec.defaultZoom = 1.0;  // Viewport of exactly 3.0000 by 1.6875
        ic = InitialConditionsService::Get("HenonParameterSpace");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
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
        //spec.defaultCenterX = 0.0;
        //spec.defaultCenterY = 0.0;
        //spec.defaultZoom = 1.0;
        ic = InitialConditionsService::Get("OrbitDiagram");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 256.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);

        // =========================================================================
        // REMOVED: MayLyapunovRef - 1D system creates boring vertical stripes
        // =========================================================================
    }
} // namespace Native
