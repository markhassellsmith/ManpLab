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
        // Symmetric Icon - 2D discrete map with histogram rendering
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "SymmetricIcon";
            spec.displayName = "Symmetric Icon";
            spec.category = "Chaotic Maps";
            spec.type = FractalCategory::HistogramBased;
            spec.description = "Creates symmetric icon-like patterns using: x(n+1) = a + by(n) + c*sin(x), y(n+1) = d + ex(n) + f*sin(y).";
            spec.formula = "x' = a + by + c*sin(x), y' = d + ex + f*sin(y)";
            spec.formulaLatex = R"(x_{n+1} = a + by_n + c\sin(x_n), \quad y_{n+1} = d + ex_n + f\sin(y_n))";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Symmetric patterns, icon-like forms, crystalline structures";
            spec.discoveredBy = "Mathematical icon research";
            spec.discoveryYear = 1990;
            spec.computationalNotes = "Parameter space exploration creates various symmetric forms";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 5.0;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                double x = 0.1;
                double y = 0.1;

                double a = 1.0;
                double b = -0.8;
                double cp = 1.0;
                double d = 1.0;
                double e = -0.8;
                double f = 1.0;

                for (int i = 0; i < maxIter; ++i)
                {
                    double xNew = a + b * y + cp * std::sin(x);
                    double yNew = d + e * x + f * std::sin(y);

                    x = xNew;
                    y = yNew;

                    if (std::abs(x) > 100.0 || std::abs(y) > 100.0)
                        return static_cast<double>(i);
                }

                return std::sqrt(x * x + y * y) * 10.0;
            };

            spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
                // Symmetric Icon: x' = a + by + c*sin(x), y' = d + ex + f*sin(y)
                double a = 1.0;
                double b = -0.8;
                double c = 1.0;
                double d = 1.0;
                double e = -0.8;
                double f = 1.0;

                double x_new = a + b * y + c * std::sin(x);
                double y_new = d + e * x + f * std::sin(y);

                x = x_new;
                y = y_new;
                // z unused for 2D map
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Gingerbread Man - 2D chaotic map with histogram rendering
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "Gingerbread";
            spec.displayName = "Gingerbread Man";
            spec.category = "Chaotic Maps";
            spec.type = FractalCategory::HistogramBased;
            spec.description = "Discrete chaotic map: x(n+1) = 1 - y + |x|, y(n+1) = x. Creates patterns resembling a gingerbread man figure.";
            spec.formula = "x' = 1 - y + |x|, y' = x";
            spec.formulaLatex = R"(x_{n+1} = 1 - y_n + |x_n|, \quad y_{n+1} = x_n)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Gingerbread man shape, discrete points, chaotic distribution";
            spec.discoveredBy = "Robert L. Devaney";
            spec.discoveryYear = 1984;
            spec.computationalNotes = "2D discrete-time iterative map with chaotic behavior";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 10.0;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
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

            spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
                // Gingerbread man: x' = 1 - y + |x|, y' = x
                double x_new = 1.0 - y + fabs(x);
                double y_new = x;

                x = x_new;
                y = y_new;
                // z unused for 2D map
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Popcorn - 2D chaotic map with trigonometric terms and histogram rendering
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "Popcorn";
            spec.displayName = "Popcorn";
            spec.category = "Chaotic Maps";
            spec.type = FractalCategory::HistogramBased;
            spec.description = "Popcorn chaotic map with trigonometric terms: x' = x - h*sin(y + tan(3y)), y' = y - h*sin(x + tan(3x))";
            spec.formula = "x' = x - h*sin(y + tan(3y)), y' = y - h*sin(x + tan(3x))";
            spec.formulaLatex = R"(x_{n+1} = x_n - h\sin(y_n + \tan(3y_n)), \quad y_{n+1} = y_n - h\sin(x_n + \tan(3x_n)))";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Popcorn-like scattered patterns, trigonometric chaos";
            spec.computationalNotes = "2D discrete map with trigonometric nonlinearity, h=0.05";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 5.0;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
                // Popcorn: x' = x - h*sin(y + tan(3*y)), y' = y - h*sin(x + tan(3*x))
                double x = c.real;
                double y = c.imag;
                double h = 0.05;

                for (int iter = 0; iter < maxIter; ++iter) {
                    double x_new = x - h * sin(y + tan(3.0 * y));
                    double y_new = y - h * sin(x + tan(3.0 * x));
                    x = x_new;
                    y = y_new;

                    if (fabs(x) > 100.0 || fabs(y) > 100.0)
                        return static_cast<double>(iter);
                }
                return static_cast<double>(maxIter);
            };

            spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
                // Popcorn attractor: x' = x - h*sin(y + tan(3*y)), y' = y - h*sin(x + tan(3*x))
                double h = 0.05;

                double x_new = x - h * sin(y + tan(3.0 * y));
                double y_new = y - h * sin(x + tan(3.0 * x));

                x = x_new;
                y = y_new;
                // z unused for 2D map
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Polynomial Attractor (Sprott) - 2D discrete map with histogram rendering
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "SprottAttractor";
            spec.displayName = "Sprott Polynomial Attractor";
            spec.category = "Chaotic Maps";
            spec.type = FractalCategory::HistogramBased;
            spec.description = "Julien Sprott's polynomial attractor: x(n+1) = a₁ + a₂x + a₃x² + a₄xy + a₅y + a₆y². Creates diverse chaotic forms.";
            spec.formula = "x' = polynomial(x,y), y' = polynomial(x,y)";
            spec.formulaLatex = R"(x_{n+1} = \sum a_i p_i(x_n, y_n))";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Varies widely with parameters, can show loops, spirals, or complex attractors";
            spec.discoveredBy = "Julien Clinton Sprott";
            spec.discoveryYear = 1993;
            spec.computationalNotes = "Polynomial coefficients from parameter space exploration";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 5.0;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                double x = 0.1;
                double y = 0.1;

                // Example coefficients for one attractor
                double a1 = -0.2, a2 = 0.5, a3 = -0.3, a4 = 0.8, a5 = 1.0, a6 = -0.6;
                double b1 = 0.3, b2 = -0.7, b3 = 0.4, b4 = -0.5, b5 = 0.9, b6 = 0.2;

                for (int i = 0; i < maxIter; ++i)
                {
                    double xNew = a1 + a2 * x + a3 * x * x + a4 * x * y + a5 * y + a6 * y * y;
                    double yNew = b1 + b2 * x + b3 * x * x + b4 * x * y + b5 * y + b6 * y * y;

                    x = xNew;
                    y = yNew;

                    if (std::abs(x) > 100.0 || std::abs(y) > 100.0)
                        return static_cast<double>(i);
                }

                return std::sqrt(x * x + y * y) * 10.0;
            };

            spec.orbitIterator = [](double& x, double& y, double& z, const ParamMap& params) {
                // Sprott polynomial attractor
                double a1 = -0.2, a2 = 0.5, a3 = -0.3, a4 = 0.8, a5 = 1.0, a6 = -0.6;
                double b1 = 0.3, b2 = -0.7, b3 = 0.4, b4 = -0.5, b5 = 0.9, b6 = 0.2;

                double x_new = a1 + a2 * x + a3 * x * x + a4 * x * y + a5 * y + a6 * y * y;
                double y_new = b1 + b2 * x + b3 * x * x + b4 * x * y + b5 * y + b6 * y * y;

                x = x_new;
                y = y_new;
                // z unused for 2D map
            };

            FractalRegistry::Register(spec);
        }
    }
}
