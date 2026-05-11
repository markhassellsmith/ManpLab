#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

//=============================================================================
// 3D Attractors Family
// Strange attractors and chaotic systems from ManpWIN64
// Includes: Lorenz, Rossler, Henon, Chua, Ikeda, etc.
//=============================================================================

void RegisterAttractors3DFamily()
{
    FractalSpec spec;

    //=========================================================================
    // LORENZ (64) - Lorenz attractor (2D projection)
    //=========================================================================
    spec.name = "Lorenz";
    spec.displayName = "Lorenz Attractor";
    spec.category = "Attractors";
    spec.type = FractalCategory::HistogramBased;
    spec.description = "Classic Lorenz strange attractor (2D projection)";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Lorenz system: dx/dt = σ(y-x), dy/dt = x(ρ-z)-y, dz/dt = xy-βz
        double sigma = 10.0;
        double rho = 28.0;
        double beta = 8.0/3.0;
        double dt = 0.01;

        double x = c.real * 0.1;
        double y = c.imag * 0.1;
        double z = 1.0;

        for (int iter = 0; iter < maxIter; ++iter) {
            double dx = sigma * (y - x);
            double dy = x * (rho - z) - y;
            double dz = x * y - beta * z;

            x += dx * dt;
            y += dy * dt;
            z += dz * dt;

            if (fabs(x) > 100.0 || fabs(y) > 100.0 || fabs(z) > 100.0)
                return static_cast<double>(iter);
        }

        // Return value based on final position
        return sqrt(x*x + y*y) * 10.0;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 10.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // ROSSLER (86) - Rössler attractor
    //=========================================================================
    spec.name = "Rossler";
    spec.displayName = "Rössler Attractor";
    spec.category = "Attractors";
    spec.type = FractalCategory::HistogramBased;
    spec.description = "Rössler strange attractor";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Rössler: dx/dt = -y-z, dy/dt = x+ay, dz/dt = b+z(x-c)
        double a = 0.2;
        double b = 0.2;
        double c_param = 5.7;
        double dt = 0.1;

        double x = c.real * 0.1;
        double y = c.imag * 0.1;
        double z = 0.0;

        for (int iter = 0; iter < maxIter; ++iter) {
            double dx = -y - z;
            double dy = x + a * y;
            double dz = b + z * (x - c_param);

            x += dx * dt;
            y += dy * dt;
            z += dz * dt;

            if (fabs(x) > 100.0 || fabs(y) > 100.0 || fabs(z) > 100.0)
                return static_cast<double>(iter);
        }

        return sqrt(x*x + y*y + z*z) * 10.0;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 5.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // HENON (88) - Hénon map
    //=========================================================================
    spec.name = "Henon";
    spec.displayName = "Hénon Map";
    spec.category = "Attractors";
    spec.type = FractalCategory::HistogramBased;
    spec.description = "Hénon discrete-time chaotic map";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Hénon map: x' = 1 - ax² + y, y' = bx
        double a = 1.4;
        double b = 0.3;

        double x = c.real * 0.1;
        double y = c.imag * 0.1;

        for (int iter = 0; iter < maxIter; ++iter) {
            double x_new = 1.0 - a * x * x + y;
            double y_new = b * x;

            x = x_new;
            y = y_new;

            if (fabs(x) > 10.0 || fabs(y) > 10.0)
                return static_cast<double>(iter);
        }

        return sqrt(x*x + y*y) * 50.0;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // PICKOVER (89) - Pickover attractor
    //=========================================================================
    spec.name = "Pickover";
    spec.displayName = "Pickover Attractor";
    spec.category = "Attractors";
    spec.type = FractalCategory::HistogramBased;
    spec.description = "Clifford Pickover's biomorphic attractor";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Pickover: x' = sin(a*y) - z*cos(b*x), y' = z*sin(c*x) - cos(d*y), z' = sin(x)
        double a = 2.24;
        double b = 0.43;
        double c_param = -0.65;
        double d = -2.43;

        double x = c.real * 0.1;
        double y = c.imag * 0.1;
        double z = 0.0;

        for (int iter = 0; iter < maxIter; ++iter) {
            double x_new = sin(a * y) - z * cos(b * x);
            double y_new = z * sin(c_param * x) - cos(d * y);
            double z_new = sin(x);

            x = x_new;
            y = y_new;
            z = z_new;

            if (fabs(x) > 100.0 || fabs(y) > 100.0)
                return static_cast<double>(iter);
        }

        return sqrt(x*x + y*y) * 20.0;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 5.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // GINGERBREAD (90) - Gingerbread man attractor
    //=========================================================================
    spec.name = "Gingerbread";
    spec.displayName = "Gingerbread Man";
    spec.category = "Attractors";
    spec.type = FractalCategory::HistogramBased;
    spec.description = "Gingerbread man chaotic attractor";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Gingerbread: x' = 1 - y + |x|, y' = x
        double x = c.real * 0.1;
        double y = c.imag * 0.1;

        for (int iter = 0; iter < maxIter; ++iter) {
            double x_new = 1.0 - y + fabs(x);
            double y_new = x;

            x = x_new;
            y = y_new;

            if (fabs(x) > 100.0 || fabs(y) > 100.0)
                return static_cast<double>(iter);
        }

        return sqrt(x*x + y*y) * 10.0;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 10.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // CHUA (222) - Chua's circuit attractor
    //=========================================================================
    spec.name = "Chua";
    spec.displayName = "Chua's Circuit";
    spec.category = "Attractors";
    spec.type = FractalCategory::HistogramBased;
    spec.description = "Chua's circuit strange attractor";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Chua's circuit system
        double alpha = 15.6;
        double beta = 28.0;
        double a = -1.143;
        double b = -0.714;
        double dt = 0.01;

        double x = c.real * 0.1;
        double y = c.imag * 0.1;
        double z = 0.0;

        for (int iter = 0; iter < maxIter; ++iter) {
            double f = b * x + 0.5 * (a - b) * (fabs(x + 1.0) - fabs(x - 1.0));
            double dx = alpha * (y - x - f);
            double dy = x - y + z;
            double dz = -beta * y;

            x += dx * dt;
            y += dy * dt;
            z += dz * dt;

            if (fabs(x) > 100.0 || fabs(y) > 100.0 || fabs(z) > 100.0)
                return static_cast<double>(iter);
        }

        return sqrt(x*x + y*y) * 10.0;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 5.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // IKEDA (213) - Ikeda map
    //=========================================================================
    spec.name = "Ikeda";
    spec.displayName = "Ikeda Map";
    spec.category = "Attractors";
    spec.type = FractalCategory::HistogramBased;
    spec.description = "Ikeda nonlinear dynamical system";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Ikeda map from laser physics
        double u = 0.918;
        double x = c.real * 0.5;
        double y = c.imag * 0.5;

        for (int iter = 0; iter < maxIter; ++iter) {
            double t = 0.4 - 6.0 / (1.0 + x*x + y*y);
            double x_new = 1.0 + u * (x * cos(t) - y * sin(t));
            double y_new = u * (x * sin(t) + y * cos(t));

            x = x_new;
            y = y_new;

            if (fabs(x) > 100.0 || fabs(y) > 100.0)
                return static_cast<double>(iter);
        }

        return sqrt(x*x + y*y) * 20.0;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 2.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // HOPALONG (120) - Martin-Hopalong attractor
    //=========================================================================
    spec.name = "Hopalong";
    spec.displayName = "Hopalong Attractor";
    spec.category = "Attractors";
    spec.type = FractalCategory::HistogramBased;
    spec.description = "Barry Martin's Hopalong attractor";

    spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
        // Hopalong: x' = y - sign(x)*sqrt(|b*x-c|), y' = a - x
        double a = 0.4;
        double b = 1.0;
        double c_param = 0.0;

        double x = c.real * 0.1;
        double y = c.imag * 0.1;

        for (int iter = 0; iter < maxIter; ++iter) {
            double sign_x = (x >= 0) ? 1.0 : -1.0;
            double x_new = y - sign_x * sqrt(fabs(b * x - c_param));
            double y_new = a - x;

            x = x_new;
            y = y_new;

            if (fabs(x) > 100.0 || fabs(y) > 100.0)
                return static_cast<double>(iter);
        }

        return sqrt(x*x + y*y) * 10.0;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 10.0;
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);
}

} // namespace Native
