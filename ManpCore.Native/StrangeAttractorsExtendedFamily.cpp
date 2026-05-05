#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// Strange Attractors Extended Family
// Additional chaotic dynamical systems and strange attractors
//=============================================================================

void RegisterStrangeAttractorsExtendedFamily()
{
    FractalSpec spec;

    //=========================================================================
    // Clifford Attractor
    //=========================================================================
    spec.name = "Clifford";
    spec.displayName = "Clifford Attractor";
    spec.category = "Strange Attractors";
    spec.type = FractalCategory::AttractorBased3D;
    spec.description = "Clifford attractor: iterative 2D map creating swirling patterns";
    spec.formula = "x' = sin(a·y) + c·cos(a·x); y' = sin(b·x) + d·cos(b·y)";
    spec.formulaLatex = R"(x_{n+1} = \sin(ay_n) + c\cos(ax_n), \; y_{n+1} = \sin(bx_n) + d\cos(by_n))";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Map pixel to initial position
        double x = c.real * 0.1;
        double y = c.imag * 0.1;

        // Clifford parameters
        double a = -1.4;
        double b = 1.6;
        double cc = 1.0;
        double d = 0.7;

        double minDist = 1000.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double x_new = std::sin(a * y) + cc * std::cos(a * x);
            double y_new = std::sin(b * x) + d * std::cos(b * y);

            x = x_new;
            y = y_new;

            // Distance from pixel
            double dx = x - c.real * 0.1;
            double dy = y - c.imag * 0.1;
            double dist = std::sqrt(dx * dx + dy * dy);
            if (dist < minDist)
                minDist = dist;
        }

        return maxIter * (1.0 - std::min(1.0, minDist * 2.0));
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.3;
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // De Jong (Peter de Jong) Attractor
    //=========================================================================
    spec.name = "DeJong";
    spec.displayName = "De Jong Attractor";
    spec.category = "Strange Attractors";
    spec.type = FractalCategory::AttractorBased3D;
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

        double minDist = 1000.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double x_new = std::sin(a * y) - std::cos(b * x);
            double y_new = std::sin(cc * x) - std::cos(d * y);

            x = x_new;
            y = y_new;

            double dx = x - c.real;
            double dy = y - c.imag;
            double dist = std::sqrt(dx * dx + dy * dy);
            if (dist < minDist)
                minDist = dist;
        }

        return maxIter * (1.0 - std::min(1.0, minDist * 0.5));
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.5;
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Tinkerbell Attractor
    //=========================================================================
    spec.name = "Tinkerbell";
    spec.displayName = "Tinkerbell Attractor";
    spec.category = "Strange Attractors";
    spec.type = FractalCategory::AttractorBased3D;
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

        double minDist = 1000.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double x_new = x * x - y * y + a * x + b * y;
            double y_new = 2.0 * x * y + cc * x + d * y;

            x = x_new;
            y = y_new;

            double dx = x - c.real;
            double dy = y - c.imag;
            double dist = std::sqrt(dx * dx + dy * dy);
            if (dist < minDist)
                minDist = dist;
        }

        return maxIter * (1.0 - std::min(1.0, minDist * 0.3));
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.8;
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Duffing Attractor
    //=========================================================================
    spec.name = "Duffing";
    spec.displayName = "Duffing Attractor";
    spec.category = "Strange Attractors";
    spec.type = FractalCategory::AttractorBased3D;
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

        double minDist = 1000.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double dx = y;
            double dy = -delta * y - alpha * x - beta * x * x * x + gamma * std::cos(omega * t);

            x += dx * dt;
            y += dy * dt;
            t += dt;

            double dist_x = x - c.real * 0.5;
            double dist_y = y - c.imag * 0.5;
            double dist = std::sqrt(dist_x * dist_x + dist_y * dist_y);
            if (dist < minDist)
                minDist = dist;
        }

        return maxIter * (1.0 - std::min(1.0, minDist));
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.5;
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Svensson Attractor
    //=========================================================================
    spec.name = "Svensson";
    spec.displayName = "Svensson Attractor";
    spec.category = "Strange Attractors";
    spec.type = FractalCategory::AttractorBased3D;
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

        double minDist = 1000.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double x_new = d * std::sin(a * x) - std::sin(b * y);
            double y_new = cc * std::cos(a * x) + std::cos(b * y);

            x = x_new;
            y = y_new;

            double dx = x - c.real * 0.2;
            double dy = y - c.imag * 0.2;
            double dist = std::sqrt(dx * dx + dy * dy);
            if (dist < minDist)
                minDist = dist;
        }

        return maxIter * (1.0 - std::min(1.0, minDist * 0.3));
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.4;
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);

    //=========================================================================
    // Bedhead Attractor
    //=========================================================================
    spec.name = "Bedhead";
    spec.displayName = "Bedhead Attractor";
    spec.category = "Strange Attractors";
    spec.type = FractalCategory::AttractorBased3D;
    spec.description = "Bedhead (Ivan Emathajuet Khatsanov): chaotic point cloud";
    spec.formula = "x' = sin(x·y/b)·y + cos(a·x - y); y' = x + sin(y)/b";
    spec.formulaLatex = R"(x_{n+1} = \sin(x_ny_n/b) \cdot y_n + \cos(ax_n - y_n), \; y_{n+1} = x_n + \sin(y_n)/b)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        double x = 1.0;
        double y = 1.0;

        // Bedhead parameters
        double a = 0.65343;
        double b = 0.7345348;

        double minDist = 1000.0;

        for (int i = 0; i < maxIter; ++i)
        {
            double x_new = std::sin(x * y / b) * y + std::cos(a * x - y);
            double y_new = x + std::sin(y) / b;

            x = x_new;
            y = y_new;

            double dx = x - c.real;
            double dy = y - c.imag;
            double dist = std::sqrt(dx * dx + dy * dy);
            if (dist < minDist)
                minDist = dist;
        }

        return maxIter * (1.0 - std::min(1.0, minDist * 0.5));
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.5;
    spec.defaultBailout = 100.0;
    spec.hasSymmetry = false;

    FractalRegistry::Register(spec);
}

} // namespace Native
