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
        // Clifford Attractor
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "CliffordAttractor";
            spec.displayName = "Clifford Attractor";
            spec.category = "Chaotic Maps";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Clifford's strange attractor: x(n+1) = sin(ay) + c*cos(ax), y(n+1) = sin(bx) + d*cos(by). Creates swirling patterns with parameters derived from c.";
            spec.formula = "x' = sin(ay) + c*cos(ax), y' = sin(bx) + d*cos(by)";
            spec.formulaLatex = R"(x_{n+1} = \sin(ay_n) + c\cos(ax_n), \quad y_{n+1} = \sin(bx_n) + d\cos(by_n))";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Swirling patterns, dense trajectories, butterfly-like structures";
            spec.discoveredBy = "Clifford Pickover";
            spec.discoveryYear = 1988;
            spec.computationalNotes = "Parameters: a=-1.4, b=1.6, c=1.0, d=0.7 (classic)";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.3;
            spec.defaultBailout = 1000.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                double x = c.real;
                double y = c.imag;

                // Parameters from position or default
                double a = -1.4 + c.real * 0.5;
                double b = 1.6 + c.imag * 0.5;
                double cp = 1.0;
                double d = 0.7;

                for (int i = 0; i < maxIter; ++i)
                {
                    double xNew = std::sin(a * y) + cp * std::cos(a * x);
                    double yNew = std::sin(b * x) + d * std::cos(b * y);

                    x = xNew;
                    y = yNew;

                    double mag2 = x * x + y * y;

                    if (mag2 > 1000.0)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // De Jong Attractor
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "DeJongAttractor";
            spec.displayName = "De Jong Attractor";
            spec.category = "Chaotic Maps";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Peter de Jong's attractor: x(n+1) = sin(a*y) - cos(b*x), y(n+1) = sin(c*x) - cos(d*y). Creates symmetric swirling patterns.";
            spec.formula = "x' = sin(ay) - cos(bx), y' = sin(cx) - cos(dy)";
            spec.formulaLatex = R"(x_{n+1} = \sin(ay_n) - \cos(bx_n), \quad y_{n+1} = \sin(cx_n) - \cos(dy_n))";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Symmetric swirls, curved filaments, dense orbital patterns";
            spec.discoveredBy = "Peter de Jong";
            spec.discoveryYear = 1987;
            spec.computationalNotes = "Classic: a=1.641, b=1.902, c=0.316, d=1.525";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.4;
            spec.defaultBailout = 1000.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                double x = c.real;
                double y = c.imag;

                double a = 1.641 + c.real * 0.3;
                double b = 1.902 + c.imag * 0.3;
                double cp = 0.316;
                double d = 1.525;

                for (int i = 0; i < maxIter; ++i)
                {
                    double xNew = std::sin(a * y) - std::cos(b * x);
                    double yNew = std::sin(cp * x) - std::cos(d * y);

                    x = xNew;
                    y = yNew;

                    double mag2 = x * x + y * y;

                    if (mag2 > 1000.0)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Tinkerbell Map
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "TinkerbellMap";
            spec.displayName = "Tinkerbell Map";
            spec.category = "Chaotic Maps";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Chaotic map creating fairy-like patterns: x(n+1) = x² - y² + ax + by, y(n+1) = 2xy + cx + dy.";
            spec.formula = "x' = x² - y² + ax + by, y' = 2xy + cx + dy";
            spec.formulaLatex = R"(x_{n+1} = x_n^2 - y_n^2 + ax_n + by_n, \quad y_{n+1} = 2x_ny_n + cx_n + dy_n)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Delicate wings, fairy-like appearance, chaotic trajectories";
            spec.discoveredBy = "Ian Stewart (named)";
            spec.discoveryYear = 1992;
            spec.computationalNotes = "Classic: a=0.9, b=-0.6, c=2.0, d=0.5";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.15;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                double x = c.real;
                double y = c.imag;

                double a = 0.9;
                double b = -0.6;
                double cp = 2.0;
                double d = 0.5;

                for (int i = 0; i < maxIter; ++i)
                {
                    double xNew = x * x - y * y + a * x + b * y;
                    double yNew = 2.0 * x * y + cp * x + d * y;

                    x = xNew;
                    y = yNew;

                    double mag2 = x * x + y * y;

                    if (mag2 > 100.0)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Bedhead Attractor
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "BedheadAttractor";
            spec.displayName = "Bedhead Attractor";
            spec.category = "Chaotic Maps";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Ivan Emke's attractor creating tangled hair-like patterns: x(n+1) = sin(xy/b)*y + cos(ax - y), y(n+1) = x + sin(y)/b.";
            spec.formula = "x' = sin(xy/b)*y + cos(ax - y), y' = x + sin(y)/b";
            spec.formulaLatex = R"(x_{n+1} = \sin(x_ny_n/b)y_n + \cos(ax_n - y_n), \quad y_{n+1} = x_n + \sin(y_n)/b)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Tangled, hair-like filaments, chaotic braiding";
            spec.discoveredBy = "Ivan Emke";
            spec.discoveryYear = 1999;
            spec.computationalNotes = "Classic parameters: a=0.653, b=0.4";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                double x = c.real;
                double y = c.imag;

                double a = 0.653 + c.real * 0.1;
                double b = 0.4;

                for (int i = 0; i < maxIter; ++i)
                {
                    double xNew = std::sin(x * y / b) * y + std::cos(a * x - y);
                    double yNew = x + std::sin(y) / b;

                    x = xNew;
                    y = yNew;

                    double mag2 = x * x + y * y;

                    if (mag2 > 100.0)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Svensson Attractor
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "SvenssonAttractor";
            spec.displayName = "Svensson Attractor";
            spec.category = "Chaotic Maps";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Johnny Svensson's attractor: x(n+1) = d*sin(ax) - sin(by), y(n+1) = c*cos(ax) + cos(by). Creates circular patterns.";
            spec.formula = "x' = d*sin(ax) - sin(by), y' = c*cos(ax) + cos(by)";
            spec.formulaLatex = R"(x_{n+1} = d\sin(ax_n) - \sin(by_n), \quad y_{n+1} = c\cos(ax_n) + \cos(by_n))";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Circular loops, orbital patterns, symmetric structures";
            spec.discoveredBy = "Johnny Svensson";
            spec.discoveryYear = 1994;
            spec.computationalNotes = "Classic: a=1.4, b=1.56, c=1.4, d=-6.56";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.1;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                double x = c.real;
                double y = c.imag;

                double a = 1.4;
                double b = 1.56;
                double cp = 1.4;
                double d = -6.56;

                for (int i = 0; i < maxIter; ++i)
                {
                    double xNew = d * std::sin(a * x) - std::sin(b * y);
                    double yNew = cp * std::cos(a * x) + std::cos(b * y);

                    x = xNew;
                    y = yNew;

                    double mag2 = x * x + y * y;

                    if (mag2 > 100.0)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Symmetric Icon
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "SymmetricIcon";
            spec.displayName = "Symmetric Icon";
            spec.category = "Chaotic Maps";
            spec.type = FractalCategory::EscapeTime2D;
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
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                double x = c.real;
                double y = c.imag;

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

                    double mag2 = x * x + y * y;

                    if (mag2 > 100.0)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Gingerbreadman Map
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "GingerbreadmanMap";
            spec.displayName = "Gingerbreadman Map";
            spec.category = "Chaotic Maps";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Discrete chaotic map: x(n+1) = 1 - y + |x|, y(n+1) = x. Creates patterns resembling a gingerbread man figure.";
            spec.formula = "x' = 1 - y + |x|, y' = x";
            spec.formulaLatex = R"(x_{n+1} = 1 - y_n + |x_n|, \quad y_{n+1} = x_n)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Gingerbread man shape, discrete points, chaotic distribution";
            spec.discoveredBy = "Robert L. Devaney";
            spec.discoveryYear = 1984;
            spec.computationalNotes = "Simple piecewise linear map with chaotic behavior";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.2;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                double x = c.real;
                double y = c.imag;

                for (int i = 0; i < maxIter; ++i)
                {
                    double xNew = 1.0 - y + std::abs(x);
                    double yNew = x;

                    x = xNew;
                    y = yNew;

                    double mag2 = x * x + y * y;

                    if (mag2 > 100.0)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Polynomial Attractor (Sprott)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "SprottAttractor";
            spec.displayName = "Sprott Polynomial Attractor";
            spec.category = "Chaotic Maps";
            spec.type = FractalCategory::EscapeTime2D;
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
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                double x = c.real;
                double y = c.imag;

                // Example coefficients for one attractor
                double a1 = -0.2, a2 = 0.5, a3 = -0.3, a4 = 0.8, a5 = 1.0, a6 = -0.6;
                double b1 = 0.3, b2 = -0.7, b3 = 0.4, b4 = -0.5, b5 = 0.9, b6 = 0.2;

                for (int i = 0; i < maxIter; ++i)
                {
                    double xNew = a1 + a2 * x + a3 * x * x + a4 * x * y + a5 * y + a6 * y * y;
                    double yNew = b1 + b2 * x + b3 * x * x + b4 * x * y + b5 * y + b6 * y * y;

                    x = xNew;
                    y = yNew;

                    double mag2 = x * x + y * y;

                    if (mag2 > 100.0)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }
    }
}
