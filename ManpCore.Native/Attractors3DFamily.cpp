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

    // Orbit iterator for histogram-based rendering
    spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
        // Lorenz system parameters
        const double sigma = 10.0;
        const double rho = 28.0;
        const double beta = 8.0 / 3.0;
        const double dt = 0.01;

        // Compute derivatives
        double dx = sigma * (y - x);
        double dy = x * (rho - z) - y;
        double dz = x * y - beta * z;

        // Euler integration step
        x += dx * dt;
        y += dy * dt;
        z += dz * dt;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.0375;  // Viewport of approximately 80 by 45
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // THOMAS - Thomas attractor (pretzel-like 3D knot)
    //=========================================================================
    spec.name = "Thomas";
    spec.displayName = "Thomas Attractor";
    spec.category = "Attractors";
    spec.type = FractalCategory::HistogramBased;
    spec.description = "Thomas cyclically symmetric attractor with pretzel structure";

    spec.calculator = nullptr;  // Not used for histogram rendering

    spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
        // Thomas attractor: dx/dt = sin(y) - b·x, dy/dt = sin(z) - b·y, dz/dt = sin(x) - b·z
        const double b = 0.208186;
        const double dt = 0.1;

        double dx = sin(y) - b * x;
        double dy = sin(z) - b * y;
        double dz = sin(x) - b * z;

        x += dx * dt;
        y += dy * dt;
        z += dz * dt;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 2.0;
    spec.defaultCenterY = 2.0;
    spec.defaultZoom = 0.2962963;  // Viewport of approximately 10.125000 by 5.695313
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // DADRAS - Dadras attractor (four-wing butterfly)
    //=========================================================================
    spec.name = "Dadras";
    spec.displayName = "Dadras Attractor";
    spec.category = "Attractors";
    spec.type = FractalCategory::HistogramBased;
    spec.description = "Dadras four-wing chaotic attractor";

    spec.calculator = nullptr;  // Not used for histogram rendering

    spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
        // Dadras: dx/dt = y - ax + b·y·z, dy/dt = c·y - x·z + z, dz/dt = d·x·y - e·z
        const double a = 3.0;
        const double b = 2.7;
        const double c = 1.7;
        const double d = 2.0;
        const double e = 9.0;
        const double dt = 0.005;  // Reduced for stability

        double dx = y - a * x + b * y * z;
        double dy = c * y - x * z + z;
        double dz = d * x * y - e * z;

        x += dx * dt;
        y += dy * dt;
        z += dz * dt;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.0623315;  // Viewport of approximately 48.120300 by 27.067669
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

    spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
        // Pickover attractor: x' = sin(a*y) - z*cos(b*x), y' = z*sin(c*x) - cos(d*y), z' = sin(x)
        double a = 2.24;
        double b = 0.43;
        double c_param = -0.65;
        double d = -2.43;

        double x_new = sin(a * y) - z * cos(b * x);
        double y_new = z * sin(c_param * x) - cos(d * y);
        double z_new = sin(x);

        x = x_new;
        y = y_new;
        z = z_new;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.4194006;  // Viewport of approximately 7.153387 by 4.023780
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // AIZAWA - Aizawa attractor (beautiful butterfly wings)
    //=========================================================================
    spec.name = "Aizawa";
    spec.displayName = "Aizawa Attractor";
    spec.category = "Attractors";
    spec.type = FractalCategory::HistogramBased;
    spec.description = "Aizawa chaotic attractor with butterfly-like wings";

    spec.calculator = nullptr;  // Not used for histogram rendering

    spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
        // Aizawa system
        const double a = 0.95;
        const double b = 0.7;
        const double c = 0.6;
        const double d = 3.5;
        const double e = 0.25;
        const double f = 0.1;
        const double dt = 0.01;

        double dx = (z - b) * x - d * y;
        double dy = d * x + (z - b) * y;
        double dz = c + a * z - (z * z * z) / 3.0 - 
                    (x * x + y * y) * (1.0 + e * z) + f * z * x * x * x;

        x += dx * dt;
        y += dy * dt;
        z += dz * dt;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.75;  // Viewport of approximately 4.00 by 2.25
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // HALVORSEN - Halvorsen attractor (triple-lobed structure)
    //=========================================================================
    spec.name = "Halvorsen";
    spec.displayName = "Halvorsen Attractor";
    spec.category = "Attractors";
    spec.type = FractalCategory::HistogramBased;
    spec.description = "Halvorsen threefold symmetric attractor";

    spec.calculator = nullptr;  // Not used for histogram rendering

    spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
        // Halvorsen: dx/dt = -a·x - 4y - 4z - y², dy/dt = -a·y - 4z - 4x - z², dz/dt = -a·z - 4x - 4y - x²
        const double a = 1.89;
        const double dt = 0.01;

        double dx = -a * x - 4.0 * y - 4.0 * z - y * y;
        double dy = -a * y - 4.0 * z - 4.0 * x - z * z;
        double dz = -a * z - 4.0 * x - 4.0 * y - x * x;

        x += dx * dt;
        y += dy * dt;
        z += dz * dt;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = -2.5;
    spec.defaultCenterY = -5.0;
    spec.defaultZoom = 0.0801291;  // Viewport of approximately 37.437343 by 21.058506
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = true;
    spec.parameters = {};

    FractalRegistry::Register(spec);

    //=========================================================================
    // CHEN-LEE - Chen-Lee attractor (double-scroll)
    //=========================================================================
    spec.name = "ChenLee";
    spec.displayName = "Chen-Lee Attractor";
    spec.category = "Attractors";
    spec.type = FractalCategory::HistogramBased;
    spec.description = "Chen-Lee double-scroll chaotic attractor";

    spec.calculator = nullptr;  // Not used for histogram rendering

    spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
        // Chen-Lee: dx/dt = a·x - y·z, dy/dt = b·y + x·z, dz/dt = c·z + x·y/3
        const double a = 5.0;
        const double b = -10.0;
        const double c = -0.38;
        const double dt = 0.0008;  // Very small timestep to prevent escape

        double dx = a * x - y * z;
        double dy = b * y + x * z;
        double dz = c * z + x * y / 3.0;

        x += dx * dt;
        y += dy * dt;
        z += dz * dt;
    };

    spec.supportsJulia = false;
    spec.defaultCenterX = 0.0;
    spec.defaultCenterY = 0.0;
    spec.defaultZoom = 0.0603081;  // Viewport of approximately 49.745700 by 27.981956
    spec.defaultBailout = 256.0;
    spec.hasSymmetry = false;
    spec.parameters = {};

    FractalRegistry::Register(spec);
}

} // namespace Native
