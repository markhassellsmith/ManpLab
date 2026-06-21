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

        //=========================================================================
        // Logistic Bifurcation Diagram
        //=========================================================================
        spec.name = "LogisticBifurcation";
        spec.displayName = "Logistic Bifurcation";
        spec.category = "Bifurcation";
        spec.type = FractalCategory::BifurcationDiagram;
        spec.description = "Classic bifurcation diagram for the logistic map: x = r·x·(1-x). Shows period-doubling cascade and chaos onset as parameter r increases. X-axis = r (parameter), Y-axis = x (attractor values).";
        spec.formula = "x = r·x·(1-x)";
        spec.formulaLatex = R"(x_{n+1} = r \cdot x_n \cdot (1 - x_n))";

        // Logistic map bifurcation calculator
        spec.bifurcationCalculator = [](double parameter, int transient, int samples, const ParamMap& params) -> std::vector<double> {
            const double r = parameter;
            double x = 0.5;  // Initial value
            std::vector<double> attractorPoints;

            // Bailout for divergent cases
            const double maxValue = 1000.0;

            // Run transient iterations to settle to attractor
            for (int i = 0; i < transient; ++i)
            {
                x = r * x * (1.0 - x);

                // Check for divergence or invalid values
                if (std::isnan(x) || std::isinf(x) || std::abs(x) > maxValue)
                    return attractorPoints;  // Return empty vector
            }

            // Collect attractor points
            for (int i = 0; i < samples; ++i)
            {
                x = r * x * (1.0 - x);

                // Check for invalid values
                if (std::isnan(x) || std::isinf(x) || std::abs(x) > maxValue)
                    break;

                attractorPoints.push_back(x);
            }

            return attractorPoints;
        };

        spec.calculator = nullptr;  // Not used for bifurcation diagrams
        spec.orbitIterator = nullptr;  // Not used for bifurcation diagrams
        spec.supportsJulia = false;

        // Default view settings: classic logistic bifurcation range
        // X-axis (parameter r): [2.8, 4.0] shows period-doubling and chaos
        // Y-axis (population x): [0, 1] is the natural range
        ic = InitialConditionsService::Get("LogisticBifurcation");
        if (ic.centerX == 0.0 && ic.centerY == 0.0)
        {
            // Fallback defaults if not in initial conditions database
            spec.defaultCenterX = 3.4;      // Center of interesting range
            spec.defaultCenterY = 0.5;      // Center of y-range (not used, but set for completeness)
            spec.defaultZoom = 2.0;         // Viewport width of ~1.2 covering [2.8, 4.0]
        }
        else
        {
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom;
        }
        spec.defaultBailout = 0.0;  // Not applicable
        spec.hasSymmetry = false;

        // Custom parameters for bifurcation rendering
        spec.parameters = {
            ParameterSpec("minY", "Min Y Value", "Minimum vertical (attractor) value to display",
                          ParameterType::Float, ParameterCategory::View, "0.0", 0.0, 2.0, 0.01),
            ParameterSpec("maxY", "Max Y Value", "Maximum vertical (attractor) value to display",
                          ParameterType::Float, ParameterCategory::View, "1.0", 0.0, 2.0, 0.01),
            ParameterSpec("transient", "Transient Iterations", "Number of iterations to skip before sampling (settle to attractor)",
                          ParameterType::Integer, ParameterCategory::Calculation, "200", 0.0, 1000.0, 10.0),
            ParameterSpec("samples", "Sample Count", "Number of attractor points to collect per parameter value",
                          ParameterType::Integer, ParameterCategory::Calculation, "100", 10.0, 500.0, 10.0)
        };

        FractalRegistry::Register(spec);

        //=========================================================================
        // Lambda Bifurcation Diagram
        //=========================================================================
        spec.name = "LambdaBifurcation";
        spec.displayName = "Lambda Bifurcation";
        spec.category = "Bifurcation";
        spec.type = FractalCategory::BifurcationDiagram;
        spec.description = "Bifurcation diagram for the complex lambda map: z = λ·z·(1-z). Shows bifurcation structure in complex parameter space. X-axis = Re(λ), Y-axis = |z| (attractor magnitude).";
        spec.formula = "z = λ·z·(1-z)";
        spec.formulaLatex = R"(z_{n+1} = \lambda \cdot z_n \cdot (1 - z_n))";

        // Lambda map bifurcation calculator (1D slice through complex parameter space)
        spec.bifurcationCalculator = [](double parameter, int transient, int samples, const ParamMap& params) -> std::vector<double> {
            // Parameter is real part of lambda; imaginary part can be set via custom params
            double lambdaIm = 0.0;
            auto it = params.find("lambdaIm");
            if (it != params.end()) lambdaIm = it->second;

            ComplexD lambda(parameter, lambdaIm);
            ComplexD z(0.5, 0.0);  // Initial value
            std::vector<double> attractorPoints;

            const double bailout = 100.0;

            // Run transient iterations
            for (int i = 0; i < transient; ++i)
            {
                z = lambda * z * (ComplexD(1.0, 0.0) - z);
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    return attractorPoints;  // Diverged, return empty
            }

            // Collect attractor points (magnitude)
            for (int i = 0; i < samples; ++i)
            {
                z = lambda * z * (ComplexD(1.0, 0.0) - z);
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    break;

                double mag = std::sqrt(magSq);
                attractorPoints.push_back(mag);
            }

            return attractorPoints;
        };

        spec.calculator = nullptr;
        spec.orbitIterator = nullptr;
        spec.supportsJulia = false;

        // Default view: interesting lambda range
        ic = InitialConditionsService::Get("LambdaBifurcation");
        if (ic.centerX == 0.0 && ic.centerY == 0.0)
        {
            spec.defaultCenterX = 2.0;
            spec.defaultCenterY = 0.5;
            spec.defaultZoom = 1.0;
        }
        else
        {
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom;
        }
        spec.defaultBailout = 0.0;
        spec.hasSymmetry = false;

        // Custom parameters for Lambda bifurcation rendering
        spec.parameters = {
            ParameterSpec("lambdaIm", "Lambda Imaginary", "Imaginary part of lambda parameter (real part is X-axis)",
                          ParameterType::Float, ParameterCategory::Calculation, "0.0", -5.0, 5.0, 0.01),
            ParameterSpec("minY", "Min Y Value", "Minimum vertical (attractor magnitude) value to display",
                          ParameterType::Float, ParameterCategory::View, "0.0", 0.0, 10.0, 0.1),
            ParameterSpec("maxY", "Max Y Value", "Maximum vertical (attractor magnitude) value to display",
                          ParameterType::Float, ParameterCategory::View, "3.0", 0.0, 10.0, 0.1),
            ParameterSpec("transient", "Transient Iterations", "Number of iterations to skip before sampling (settle to attractor)",
                          ParameterType::Integer, ParameterCategory::Calculation, "200", 0.0, 1000.0, 10.0),
            ParameterSpec("samples", "Sample Count", "Number of attractor points to collect per parameter value",
                          ParameterType::Integer, ParameterCategory::Calculation, "100", 10.0, 500.0, 10.0)
        };

        FractalRegistry::Register(spec);

        //=========================================================================
        // Henon Bifurcation Diagram
        //=========================================================================
        spec.name = "HenonBifurcation";
        spec.displayName = "Henon Bifurcation";
        spec.category = "Bifurcation";
        spec.type = FractalCategory::BifurcationDiagram;
        spec.description = "Bifurcation diagram for the Hénon map: xₙ₊₁ = 1 - a·xₙ² + yₙ; yₙ₊₁ = b·xₙ. Shows 2D chaotic attractor structure. X-axis = a (parameter), Y-axis = x (attractor x-coordinate).";
        spec.formula = "xₙ₊₁ = 1 - a·xₙ² + yₙ; yₙ₊₁ = b·xₙ";
        spec.formulaLatex = R"(x_{n+1} = 1 - a \cdot x_n^2 + y_n, \; y_{n+1} = b \cdot x_n)";

        // Henon map bifurcation calculator
        spec.bifurcationCalculator = [](double parameter, int transient, int samples, const ParamMap& params) -> std::vector<double> {
            const double a = parameter;

            // b parameter (fixed or from custom params)
            double b = 0.3;  // Classic Henon value
            auto it = params.find("henonB");
            if (it != params.end()) b = it->second;

            double x = 0.1;
            double y = 0.1;
            std::vector<double> attractorPoints;

            const double maxValue = 100.0;

            // Run transient iterations
            for (int i = 0; i < transient; ++i)
            {
                double xNew = 1.0 - a * x * x + y;
                double yNew = b * x;
                x = xNew;
                y = yNew;

                if (std::isnan(x) || std::isinf(x) || std::abs(x) > maxValue ||
                    std::isnan(y) || std::isinf(y) || std::abs(y) > maxValue)
                    return attractorPoints;  // Diverged
            }

            // Collect attractor points (x-coordinate)
            for (int i = 0; i < samples; ++i)
            {
                double xNew = 1.0 - a * x * x + y;
                double yNew = b * x;
                x = xNew;
                y = yNew;

                if (std::isnan(x) || std::isinf(x) || std::abs(x) > maxValue ||
                    std::isnan(y) || std::isinf(y) || std::abs(y) > maxValue)
                    break;

                attractorPoints.push_back(x);
            }

            return attractorPoints;
        };

        spec.calculator = nullptr;
        spec.orbitIterator = nullptr;
        spec.supportsJulia = false;

        // Default view: classic Henon parameter range
        ic = InitialConditionsService::Get("HenonBifurcation");
        if (ic.centerX == 0.0 && ic.centerY == 0.0)
        {
            spec.defaultCenterX = 1.2;      // Near classic Henon value a=1.4
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 2.0;         // Show range around [0.7, 1.7]
        }
        else
        {
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom;
        }
        spec.defaultBailout = 0.0;
        spec.hasSymmetry = false;

        // Custom parameters for Henon bifurcation rendering
        spec.parameters = {
            ParameterSpec("henonB", "Henon B Parameter", "Second parameter b in Hénon map (classic value = 0.3)",
                          ParameterType::Float, ParameterCategory::Calculation, "0.3", -2.0, 2.0, 0.01),
            ParameterSpec("minY", "Min Y Value", "Minimum vertical (attractor x-coordinate) value to display",
                          ParameterType::Float, ParameterCategory::View, "-2.0", -10.0, 10.0, 0.1),
            ParameterSpec("maxY", "Max Y Value", "Maximum vertical (attractor x-coordinate) value to display",
                          ParameterType::Float, ParameterCategory::View, "2.0", -10.0, 10.0, 0.1),
            ParameterSpec("transient", "Transient Iterations", "Number of iterations to skip before sampling (settle to attractor)",
                          ParameterType::Integer, ParameterCategory::Calculation, "200", 0.0, 1000.0, 10.0),
            ParameterSpec("samples", "Sample Count", "Number of attractor points to collect per parameter value",
                          ParameterType::Integer, ParameterCategory::Calculation, "100", 10.0, 500.0, 10.0)
        };

        FractalRegistry::Register(spec);

        // =========================================================================
        // REMOVED: MayLyapunovRef - 1D system creates boring vertical stripes
        // =========================================================================
    }
} // namespace Native
