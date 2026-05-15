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
            spec.formula = "dx/dt = a(y-x), dy/dt = bx - xz, dz/dt = -cz + xy";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Four-wing structure with complex orbit patterns";
            spec.discoveredBy = "Liu & Chen";
            spec.discoveryYear = 2004;
            spec.computationalNotes = "Multi-wing attractor with a=10, b=40, c=2.5, requires small dt=0.001";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.025974;  // Viewport tuning: X scale 154, Y scale 86.8
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = nullptr;  // Not used for histogram rendering

            spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
                // Liu-Chen: dx/dt = a(y-x), dy/dt = bx - xz, dz/dt = -cz + xy
                // Corrected parameters for proper four-wing butterfly structure
                const double a = 10.0;
                const double b = 40.0;
                const double c = 2.5;
                const double dt = 0.001;  // Smaller timestep for stability

                // RK2 (midpoint method)
                double dx1 = a * (y - x);
                double dy1 = b * x - x * z;
                double dz1 = -c * z + x * y;

                double xmid = x + 0.5 * dx1 * dt;
                double ymid = y + 0.5 * dy1 * dt;
                double zmid = z + 0.5 * dz1 * dt;

                double dx2 = a * (ymid - xmid);
                double dy2 = b * xmid - xmid * zmid;
                double dz2 = -c * zmid + xmid * ymid;

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
            spec.defaultZoom = 0.268393;  // Viewport of 11.177639 by 6.287422
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

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 2.758621;  // Viewport tuning: X scale 1.45, Y scale 0.813
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
            spec.defaultZoom = 0.15625;  // Viewport of 19.2 by 10.8
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
