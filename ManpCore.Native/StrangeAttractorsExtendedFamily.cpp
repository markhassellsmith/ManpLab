#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Strange Attractors Extended Family
// 2D discrete-time chaotic maps with histogram-based rendering
// Converted to HistogramBased rendering for proper attractor visualization
//=============================================================================

void RegisterStrangeAttractorsExtendedFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Clifford Attractor - 2D discrete map with histogram rendering
    //=========================================================================
    spec.name = "Clifford";
    spec.displayName = "Clifford Attractor";
    spec.category = "Strange Attractors";
    spec.type = FractalCategory::HistogramBased;
    spec.description = "Clifford attractor: iterative 2D map creating swirling patterns";
    spec.formula = "x' = sin(a·y) + c·cos(a·x); y' = sin(b·x) + d·cos(b·y)";
    spec.formulaLatex = R"(x_{n+1} = \sin(ay_n) + c\cos(ax_n), \; y_{n+1} = \sin(bx_n) + d\cos(by_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        double x = 0.1;
        double y = 0.1;

        // Clifford parameters
        double a = -1.4;
        double b = 1.6;
        double cc = 1.0;
        double d = 0.7;

        for (int i = 0; i < maxIter; ++i)
        {
            double x_new = std::sin(a * y) + cc * std::cos(a * x);
            double y_new = std::sin(b * x) + d * std::cos(b * y);

            x = x_new;
            y = y_new;

            if (std::abs(x) > 100.0 || std::abs(y) > 100.0)
                return static_cast<double>(i);
        }

        return std::sqrt(x * x + y * y) * 10.0;
    };

    spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
        // Clifford attractor: x' = sin(a·y) + c·cos(a·x); y' = sin(b·x) + d·cos(b·y)
        double a = -1.4;
        double b = 1.6;
        double c = 1.0;
        double d = 0.7;

        double x_new = std::sin(a * y) + c * std::cos(a * x);
        double y_new = std::sin(b * x) + d * std::cos(b * y);

        x = x_new;
        y = y_new;
        // z unused for 2D map
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // De Jong (Peter de Jong) Attractor - 2D discrete map with histogram rendering
    //=========================================================================
    spec.name = "DeJong";
    spec.displayName = "De Jong Attractor";
    spec.category = "Strange Attractors";
    spec.type = FractalCategory::HistogramBased;
    spec.description = "Peter de Jong attractor: creates flowing, organic patterns";
    spec.formula = "x' = sin(a·y) - cos(b·x); y' = sin(c·x) - cos(d·y)";
    spec.formulaLatex = R"(x_{n+1} = \sin(ay_n) - \cos(bx_n), \; y_{n+1} = \sin(cx_n) - \cos(dy_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        double x = 0.1;
        double y = 0.1;

        // De Jong parameters
        double a = 1.4;
        double b = -2.3;
        double cc = 2.4;
        double d = -2.1;

        for (int i = 0; i < maxIter; ++i)
        {
            double x_new = std::sin(a * y) - std::cos(b * x);
            double y_new = std::sin(cc * x) - std::cos(d * y);

            x = x_new;
            y = y_new;

            if (std::abs(x) > 100.0 || std::abs(y) > 100.0)
                return static_cast<double>(i);
        }

        return std::sqrt(x * x + y * y) * 10.0;
    };

    spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
        // De Jong attractor: x' = sin(a·y) - cos(b·x); y' = sin(c·x) - cos(d·y)
        double a = 1.4;
        double b = -2.3;
        double c = 2.4;
        double d = -2.1;

        double x_new = std::sin(a * y) - std::cos(b * x);
        double y_new = std::sin(c * x) - std::cos(d * y);

        x = x_new;
        y = y_new;
        // z unused for 2D map
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Tinkerbell Attractor - 2D discrete map with histogram rendering
    //=========================================================================
    spec.name = "Tinkerbell";
    spec.displayName = "Tinkerbell Attractor";
    spec.category = "Strange Attractors";
    spec.type = FractalCategory::HistogramBased;
    spec.description = "Tinkerbell map: discrete-time dynamical system";
    spec.formula = "x' = x² - y² + a·x + b·y; y' = 2xy + c·x + d·y";
    spec.formulaLatex = R"(x_{n+1} = x_n^2 - y_n^2 + ax_n + by_n, \; y_{n+1} = 2x_ny_n + cx_n + dy_n)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        double x = 0.1;
        double y = 0.1;

        // Tinkerbell parameters
        double a = 0.9;
        double b = -0.6013;
        double cc = 2.0;
        double d = 0.5;

        for (int i = 0; i < maxIter; ++i)
        {
            double x_new = x * x - y * y + a * x + b * y;
            double y_new = 2.0 * x * y + cc * x + d * y;

            x = x_new;
            y = y_new;

            if (std::abs(x) > 100.0 || std::abs(y) > 100.0)
                return static_cast<double>(i);
        }

        return std::sqrt(x * x + y * y) * 10.0;
    };

    spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
        // Tinkerbell attractor: x' = x² - y² + a·x + b·y; y' = 2xy + c·x + d·y
        double a = 0.9;
        double b = -0.6013;
        double c = 2.0;
        double d = 0.5;

        double x_new = x * x - y * y + a * x + b * y;
        double y_new = 2.0 * x * y + c * x + d * y;

        x = x_new;
        y = y_new;
        // z unused for 2D map
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 3.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Duffing Attractor - Continuous-time system with Euler integration
    //=========================================================================
    spec.name = "Duffing";
    spec.displayName = "Duffing Attractor";
    spec.category = "Strange Attractors";
    spec.type = FractalCategory::HistogramBased;
    spec.description = "Duffing oscillator: forced nonlinear oscillator";
    spec.formula = "dx/dt = y; dy/dt = -δ·y - α·x - β·x³ + γ·cos(ω·t)";
    spec.formulaLatex = R"(\frac{dx}{dt} = y, \; \frac{dy}{dt} = -\delta y - \alpha x - \beta x^3 + \gamma\cos(\omega t))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        double x = 0.1;
        double y = 0.1;
        double t = 0.0;

        // Duffing parameters
        double alpha = -1.0;
        double beta = 1.0;
        double delta = 0.15;
        double gamma = 0.3;
        double omega = 1.0;
        double dt = 0.05;

        for (int i = 0; i < maxIter; ++i)
        {
            double dx = y;
            double dy = -delta * y - alpha * x - beta * x * x * x + gamma * std::cos(omega * t);

            x += dx * dt;
            y += dy * dt;
            t += dt;

            if (std::abs(x) > 100.0 || std::abs(y) > 100.0)
                return static_cast<double>(i);
        }

        return std::sqrt(x * x + y * y) * 10.0;
    };

    spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
        // Duffing oscillator with Euler integration
        // dx/dt = y; dy/dt = -δ·y - α·x - β·x³ + γ·cos(ω·t)
        // Note: t is tracked via z for this continuous-time system
        double alpha = -1.0;
        double beta = 1.0;
        double delta = 0.15;
        double gamma = 0.3;
        double omega = 1.0;
        double dt = 0.05;

        double dx = y;
        double dy = -delta * y - alpha * x - beta * x * x * x + gamma * std::cos(omega * z);

        x += dx * dt;
        y += dy * dt;
        z += dt;  // time parameter
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Svensson Attractor - 2D discrete map with histogram rendering
    //=========================================================================
    spec.name = "Svensson";
    spec.displayName = "Svensson Attractor";
    spec.category = "Strange Attractors";
    spec.type = FractalCategory::HistogramBased;
    spec.description = "Johnny Svensson attractor: generates intricate patterns";
    spec.formula = "x' = d·sin(a·x) - sin(b·y); y' = c·cos(a·x) + cos(b·y)";
    spec.formulaLatex = R"(x_{n+1} = d\sin(ax_n) - \sin(by_n), \; y_{n+1} = c\cos(ax_n) + \cos(by_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        double x = 0.1;
        double y = 0.1;

        // Svensson parameters
        double a = 1.4;
        double b = 1.56;
        double cc = 1.4;
        double d = -6.56;

        for (int i = 0; i < maxIter; ++i)
        {
            double x_new = d * std::sin(a * x) - std::sin(b * y);
            double y_new = cc * std::cos(a * x) + std::cos(b * y);

            x = x_new;
            y = y_new;

            if (std::abs(x) > 100.0 || std::abs(y) > 100.0)
                return static_cast<double>(i);
        }

        return std::sqrt(x * x + y * y) * 10.0;
    };

    spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
        // Svensson attractor: x' = d·sin(a·x) - sin(b·y); y' = c·cos(a·x) + cos(b·y)
        double a = 1.4;
        double b = 1.56;
        double c = 1.4;
        double d = -6.56;

        double x_new = d * std::sin(a * x) - std::sin(b * y);
        double y_new = c * std::cos(a * x) + std::cos(b * y);

        x = x_new;
        y = y_new;
        // z unused for 2D map
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 1.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Bedhead Attractor - 2D discrete map with histogram rendering
    //=========================================================================
    spec.name = "Bedhead";
    spec.displayName = "Bedhead Attractor";
    spec.category = "Strange Attractors";
    spec.type = FractalCategory::HistogramBased;
    spec.description = "Bedhead (Ivan Emathajuet Khatsanov): chaotic point cloud";
    spec.formula = "x' = sin(x·y/b)·y + cos(a·x - y); y' = x + sin(y)/b";
    spec.formulaLatex = R"(x_{n+1} = \sin(x_ny_n/b) \cdot y_n + \cos(ax_n - y_n), \; y_{n+1} = x_n + \sin(y_n)/b)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        double x = 1.0;
        double y = 1.0;

        // Bedhead parameters
        double a = 0.65343;
        double b = 0.7345348;

        for (int i = 0; i < maxIter; ++i)
        {
            double x_new = std::sin(x * y / b) * y + std::cos(a * x - y);
            double y_new = x + std::sin(y) / b;

            x = x_new;
            y = y_new;

            if (std::abs(x) > 100.0 || std::abs(y) > 100.0)
                return static_cast<double>(i);
        }

        return std::sqrt(x * x + y * y) * 10.0;
    };

    spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
        // Bedhead attractor: x' = sin(x·y/b)·y + cos(a·x - y); y' = x + sin(y)/b
        double a = 0.65343;
        double b = 0.7345348;

        double x_new = std::sin(x * y / b) * y + std::cos(a * x - y);
        double y_new = x + std::sin(y) / b;

        x = x_new;
        y = y_new;
        // z unused for 2D map
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);
}

} // namespace Native
