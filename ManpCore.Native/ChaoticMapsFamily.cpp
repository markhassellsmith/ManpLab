#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native
{
    void RegisterChaoticMapsFamily()
    {
        // ═══════════════════════════════════════════════════════════════════════════════
        // CHAOTIC MAPS & DYNAMICAL SYSTEMS
        // Famous maps from chaos theory and nonlinear dynamics
        // ═══════════════════════════════════════════════════════════════════════════════

        // ───────────────────────────────────────────────────────────────────────────────
        // Clifford Attractor - Moved to Strange Attractors family (duplicate removed)
        // ───────────────────────────────────────────────────────────────────────────────

        // ───────────────────────────────────────────────────────────────────────────────
        // De Jong Attractor - Moved to Strange Attractors family (duplicate removed)
        // ───────────────────────────────────────────────────────────────────────────────

        // ───────────────────────────────────────────────────────────────────────────────
        // Tinkerbell Map - Moved to Strange Attractors family (duplicate removed)
        // ───────────────────────────────────────────────────────────────────────────────

        // ───────────────────────────────────────────────────────────────────────────────
        // Bedhead Attractor - Moved to Strange Attractors family (duplicate removed)
        // ───────────────────────────────────────────────────────────────────────────────

        // ───────────────────────────────────────────────────────────────────────────────
        // Svensson Attractor - Moved to Strange Attractors family (duplicate removed)
        // ───────────────────────────────────────────────────────────────────────────────

        // ───────────────────────────────────────────────────────────────────────────────
        // Liu-Chen Attractor - Four-wing butterfly structure with histogram rendering
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "LiuChen";
            spec.displayName = "Liu-Chen Attractor";
            spec.category = "Chaotic Maps";
            spec.type = FractalCategory::HistogramBased;
            spec.description = "Liu-Chen: Four-wing butterfly structure with intricate detail";
            spec.formula = "dx/dt = ay + bx + cyz, dy/dt = dy - xz, dz/dt = ez + fxy";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Four-wing structure with complex orbit patterns";
            spec.discoveredBy = "Liu & Chen";
            spec.discoveryYear = 2004;
            spec.computationalNotes = "Multi-wing attractor, extends Lorenz system";

            spec.defaultCenterX = 2.65;
            spec.defaultCenterY = 2.65;
            spec.defaultZoom = 15.0;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = nullptr;  // Not used for histogram rendering

            spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
                // Liu-Chen: dx/dt = a(y-x), dy/dt = bx - xz, dz/dt = xy - cz
                // Diagnostic confirmed: a=2.5, b=4.0, c=4.0, dt=0.01 is stable
                const double a = 2.5;
                const double b = 4.0;
                const double c = 4.0;
                const double dt = 0.01;

                // RK2 (midpoint method)
                double dx1 = a * (y - x);
                double dy1 = b * x - x * z;
                double dz1 = x * y - c * z;

                double xmid = x + 0.5 * dx1 * dt;
                double ymid = y + 0.5 * dy1 * dt;
                double zmid = z + 0.5 * dz1 * dt;

                double dx2 = a * (ymid - xmid);
                double dy2 = b * xmid - xmid * zmid;
                double dz2 = xmid * ymid - c * zmid;

                x += dx2 * dt;
                y += dy2 * dt;
                z += dz2 * dt;
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Rabinovich-Fabrikant Attractor - Complex 3D attractor with histogram rendering
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "RabinovichFabrikant";
            spec.displayName = "Rabinovich-Fabrikant Attractor";
            spec.category = "Chaotic Maps";
            spec.type = FractalCategory::HistogramBased;
            spec.description = "Rabinovich-Fabrikant attractor: Multi-lobed chaotic structure";
            spec.formula = "dx/dt = y(z - 1 + x²) + γx, dy/dt = x(3z + 1 - x²) + γy, dz/dt = -2z(α + xy)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Multi-lobed structure with complex folding";
            spec.discoveredBy = "Mikhail Rabinovich & Anatoly Fabrikant";
            spec.discoveryYear = 1979;
            spec.computationalNotes = "Euler integration with small timestep for stability";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 3.0;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = nullptr;  // Not used for histogram rendering

            spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
                // Rabinovich-Fabrikant: dx/dt = y(z - 1 + x²) + γx, dy/dt = x(3z + 1 - x²) + γy, dz/dt = -2z(α + xy)
                const double alpha = 0.14;
                const double gamma = 0.1;
                const double dt = 0.01;

                double dx = y * (z - 1.0 + x * x) + gamma * x;
                double dy = x * (3.0 * z + 1.0 - x * x) + gamma * y;
                double dz = -2.0 * z * (alpha + x * y);

                x += dx * dt;
                y += dy * dt;
                z += dz * dt;
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Arneodo Attractor - Ribbon-like structure with histogram rendering
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "Arneodo";
            spec.displayName = "Arneodo Attractor";
            spec.category = "Chaotic Maps";
            spec.type = FractalCategory::HistogramBased;
            spec.description = "Arneodo attractor: Twisted ribbon-like structure";
            spec.formula = "dx/dt = y, dy/dt = z, dz/dt = -ax - by - cz + dx³";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Twisted ribbon forming Möbius-like band";
            spec.discoveredBy = "Alain Arneodo";
            spec.discoveryYear = 1981;
            spec.computationalNotes = "Third-order differential equation system";

            spec.defaultCenterX = 0.04;
            spec.defaultCenterY = -0.04;
            spec.defaultZoom = 4.15;  // Optimal for range [-0.22 to 0.30, -0.45 to 0.36, -0.74 to 0.70]
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = nullptr;  // Not used for histogram rendering

            spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
                // Arneodo: dx/dt = y, dy/dt = z, dz/dt = -ax - by - cz + dx³
                // Diagnostic sweep found: c=2.0, dt=0.001 produces bounded attractor
                const double a = 5.5;
                const double b = 3.5;
                const double c = 2.0;  // Critical: only c=2.0 is stable
                const double d = -1.0;
                const double dt = 0.001;  // Must be very small

                // RK2 (midpoint method) for better stability
                double dx1 = y;
                double dy1 = z;
                double dz1 = -a * x - b * y - c * z + d * x * x * x;

                double xmid = x + 0.5 * dx1 * dt;
                double ymid = y + 0.5 * dy1 * dt;
                double zmid = z + 0.5 * dz1 * dt;

                double dx2 = ymid;
                double dy2 = zmid;
                double dz2 = -a * xmid - b * ymid - c * zmid + d * xmid * xmid * xmid;

                x += dx2 * dt;
                y += dy2 * dt;
                z += dz2 * dt;
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Sprott B Attractor - Minimalist elegant loop with histogram rendering
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "SprottB";
            spec.displayName = "Sprott B Attractor";
            spec.category = "Chaotic Maps";
            spec.type = FractalCategory::HistogramBased;
            spec.description = "Sprott B: Minimalist chaotic attractor with elegant simplicity";
            spec.formula = "dx/dt = yz, dy/dt = x - y, dz/dt = 1 - xy";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Elegant twisted loop structure, simple but beautiful";
            spec.discoveredBy = "Julien Clinton Sprott";
            spec.discoveryYear = 1994;
            spec.computationalNotes = "Simplest chaotic attractor with only quadratic nonlinearity";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 2.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = nullptr;  // Not used for histogram rendering

            spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
                // Sprott B: dx/dt = yz, dy/dt = x - y, dz/dt = 1 - xy
                const double dt = 0.05;

                double dx = y * z;
                double dy = x - y;
                double dz = 1.0 - x * y;

                x += dx * dt;
                y += dy * dt;
                z += dz * dt;
            };

            FractalRegistry::Register(spec);
        }
    }
}
