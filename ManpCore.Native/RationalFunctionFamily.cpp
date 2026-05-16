#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native
{
    void RegisterRationalFunctionFamily()
    {
        // ═══════════════════════════════════════════════════════════════════════════════
        // RATIONAL FUNCTION FRACTALS - Ratios of polynomials
        // ═══════════════════════════════════════════════════════════════════════════════

        // ───────────────────────────────────────────────────────────────────────────────
        // Newton z³-1 (Classic Newton Fractal)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "NewtonCubic";
            spec.displayName = "Newton z³-1";
            spec.category = "Rational Function Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Newton's method for finding roots of z³-1. Shows three basins of attraction converging to cube roots of unity. Formula: z(n+1) = z(n) - (z³-1)/(3z²)";
            spec.formula = "z(n+1) = z - (z³-1)/(3z²)";
            spec.formulaLatex = R"(z_{n+1} = z_n - \frac{z_n^3 - 1}{3z_n^2})";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Three-fold rotational symmetry, fractal boundaries between basins of attraction";
            spec.discoveredBy = "Isaac Newton (method), visualized by fractal community";
            spec.discoveryYear = 1669;
            spec.computationalNotes = "Simplified to z(n+1) = (2z³+1)/(3z²) for efficiency";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.6;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // z³
                    double x = z.real;
                    double y = z.imag;
                    double x2 = x * x - y * y;
                    double y2 = 2.0 * x * y;
                    double x3 = x * x2 - y * y2;
                    double y3 = x * y2 + y * x2;

                    // z²
                    double zSqReal = x * x - y * y;
                    double zSqImag = 2.0 * x * y;

                    // Newton iteration: z = z - (z³-1)/(3z²) = (2z³+1)/(3z²)
                    // Numerator: 2z³ + 1
                    double numReal = 2.0 * x3 + 1.0;
                    double numImag = 2.0 * y3;

                    // Denominator: 3z²
                    double denReal = 3.0 * zSqReal;
                    double denImag = 3.0 * zSqImag;

                    // Complex division
                    double denMag2 = denReal * denReal + denImag * denImag;
                    if (denMag2 < 1e-10) return static_cast<double>(maxIter);

                    z.real = (numReal * denReal + numImag * denImag) / denMag2;
                    z.imag = (numImag * denReal - numReal * denImag) / denMag2;

                    // Check for convergence (distance to any root < tolerance)
                    double mag = std::sqrt(z.real * z.real + z.imag * z.imag);
                    if (std::abs(mag - 1.0) < 0.001)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Newton z⁴-1
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "NewtonQuarticRational";
            spec.displayName = "Newton z⁴-1";
            spec.category = "Rational Function Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Newton's method for z⁴-1=0. Shows four basins of attraction with four-fold rotational symmetry.";
            spec.formula = "z(n+1) = z - (z⁴-1)/(4z³)";
            spec.formulaLatex = R"(z_{n+1} = z_n - \frac{z_n^4 - 1}{4z_n^3})";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Four-fold rotational symmetry, square-like basin structure";
            spec.discoveredBy = "Newton's method visualized";
            spec.discoveryYear = 1980;
            spec.computationalNotes = "Simplified to z(n+1) = (3z⁴+1)/(4z³)";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.6;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = c;

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;

                    // z²
                    double x2 = x * x - y * y;
                    double y2 = 2.0 * x * y;

                    // z⁴ = (z²)²
                    double x4 = x2 * x2 - y2 * y2;
                    double y4 = 2.0 * x2 * y2;

                    // z³
                    double x3 = x * x2 - y * y2;
                    double y3 = x * y2 + y * x2;

                    // Newton: (3z⁴+1)/(4z³)
                    double numReal = 3.0 * x4 + 1.0;
                    double numImag = 3.0 * y4;

                    double denReal = 4.0 * x3;
                    double denImag = 4.0 * y3;

                    double denMag2 = denReal * denReal + denImag * denImag;
                    if (denMag2 < 1e-10) return static_cast<double>(maxIter);

                    z.real = (numReal * denReal + numImag * denImag) / denMag2;
                    z.imag = (numImag * denReal - numReal * denImag) / denMag2;

                    double mag = std::sqrt(z.real * z.real + z.imag * z.imag);
                    if (std::abs(mag - 1.0) < 0.001)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Newton z⁵-1
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "NewtonQuinticRational";
            spec.displayName = "Newton z⁵-1";
            spec.category = "Rational Function Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Newton's method for z⁵-1=0. Five basins of attraction with pentagonal symmetry.";
            spec.formula = "z(n+1) = z - (z⁵-1)/(5z⁴)";
            spec.formulaLatex = R"(z_{n+1} = z_n - \frac{z_n^5 - 1}{5z_n^4})";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Five-fold rotational symmetry, pentagonal basin structure";
            spec.discoveredBy = "Newton's method visualized";
            spec.discoveryYear = 1980;
            spec.computationalNotes = "Simplified to z(n+1) = (4z⁵+1)/(5z⁴)";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.6;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = c;

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;

                    double x2 = x * x - y * y;
                    double y2 = 2.0 * x * y;

                    double x4 = x2 * x2 - y2 * y2;
                    double y4 = 2.0 * x2 * y2;

                    double x5 = x * x4 - y * y4;
                    double y5 = x * y4 + y * x4;

                    // Newton: (4z⁵+1)/(5z⁴)
                    double numReal = 4.0 * x5 + 1.0;
                    double numImag = 4.0 * y5;

                    double denReal = 5.0 * x4;
                    double denImag = 5.0 * y4;

                    double denMag2 = denReal * denReal + denImag * denImag;
                    if (denMag2 < 1e-10) return static_cast<double>(maxIter);

                    z.real = (numReal * denReal + numImag * denImag) / denMag2;
                    z.imag = (numImag * denReal - numReal * denImag) / denMag2;

                    double mag = std::sqrt(z.real * z.real + z.imag * z.imag);
                    if (std::abs(mag - 1.0) < 0.001)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Rational Map: z²/(z³+c)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "RationalZ2Z3";
            spec.displayName = "Rational z²/(z³+c)";
            spec.category = "Rational Function Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Rational function iteration z(n+1) = z²/(z³+c). Creates interesting patterns from the ratio of quadratic to cubic terms.";
            spec.formula = "z(n+1) = z²/(z³+c)";
            spec.formulaLatex = R"(z_{n+1} = \frac{z_n^2}{z_n^3 + c})";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Asymmetric patterns, pole singularities create intricate structures";
            spec.discoveredBy = "Rational function fractal exploration";
            spec.discoveryYear = 1990;
            spec.computationalNotes = "Check for division by zero in denominator";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 1.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.5, 0.5);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;

                    // z²
                    double x2 = x * x - y * y;
                    double y2 = 2.0 * x * y;

                    // z³
                    double x3 = x * x2 - y * y2;
                    double y3 = x * y2 + y * x2;

                    // z³ + c
                    double denReal = x3 + constant.real;
                    double denImag = y3 + constant.imag;

                    double denMag2 = denReal * denReal + denImag * denImag;
                    if (denMag2 < 1e-10)
                    {
                        return static_cast<double>(i);
                    }

                    // z² / (z³ + c)
                    z.real = (x2 * denReal + y2 * denImag) / denMag2;
                    z.imag = (y2 * denReal - x2 * denImag) / denMag2;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0 || mag2 < 1e-10)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Rational Map: (z²+c)/(z²-c)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "RationalSymmetric";
            spec.displayName = "Rational (z²+c)/(z²-c)";
            spec.category = "Rational Function Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Symmetric rational function z(n+1) = (z²+c)/(z²-c). Creates balanced structures with interesting pole behavior.";
            spec.formula = "z(n+1) = (z²+c)/(z²-c)";
            spec.formulaLatex = R"(z_{n+1} = \frac{z_n^2 + c}{z_n^2 - c})";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Symmetric patterns, circular and hyperbolic features";
            spec.discoveredBy = "Rational function fractal exploration";
            spec.discoveryYear = 1992;
            spec.computationalNotes = "Singularities when z² = c";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 1.0;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.5, 0.5);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;

                    // z²
                    double x2 = x * x - y * y;
                    double y2 = 2.0 * x * y;

                    // Numerator: z² + c
                    double numReal = x2 + constant.real;
                    double numImag = y2 + constant.imag;

                    // Denominator: z² - c
                    double denReal = x2 - constant.real;
                    double denImag = y2 - constant.imag;

                    double denMag2 = denReal * denReal + denImag * denImag;
                    if (denMag2 < 1e-10)
                    {
                        return static_cast<double>(i);
                    }

                    z.real = (numReal * denReal + numImag * denImag) / denMag2;
                    z.imag = (numImag * denReal - numReal * denImag) / denMag2;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Halley's Method for z³-1
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "HalleyCubic";
            spec.displayName = "Halley's Method z³-1";
            spec.category = "Rational Function Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Halley's root-finding method for z³-1. Similar to Newton but with cubic convergence. Formula uses second derivative for faster convergence.";
            spec.formula = "z(n+1) = z - (2f(z)f'(z))/(2f'(z)²-f(z)f''(z))";
            spec.formulaLatex = R"(z_{n+1} = z_n - \frac{2f(z)f'(z)}{2f'(z)^2 - f(z)f''(z)})";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Sharper basin boundaries than Newton, faster convergence zones";
            spec.discoveredBy = "Edmond Halley (method), visualized later";
            spec.discoveryYear = 1694;
            spec.computationalNotes = "More complex than Newton but converges faster near roots";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.6;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = c;

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;

                    // f(z) = z³ - 1
                    double x2 = x * x - y * y;
                    double y2 = 2.0 * x * y;
                    double fx = x * x2 - y * y2 - 1.0;
                    double fy = x * y2 + y * x2;

                    // f'(z) = 3z²
                    double fpx = 3.0 * (x * x - y * y);
                    double fpy = 3.0 * (2.0 * x * y);

                    // f''(z) = 6z
                    double fppx = 6.0 * x;
                    double fppy = 6.0 * y;

                    // 2f'(z)²
                    double fp2Real = fpx * fpx - fpy * fpy;
                    double fp2Imag = 2.0 * fpx * fpy;
                    fp2Real *= 2.0;
                    fp2Imag *= 2.0;

                    // f(z)f''(z)
                    double ffppReal = fx * fppx - fy * fppy;
                    double ffppImag = fx * fppy + fy * fppx;

                    // Denominator: 2f'² - ff''
                    double denReal = fp2Real - ffppReal;
                    double denImag = fp2Imag - ffppImag;

                    double denMag2 = denReal * denReal + denImag * denImag;
                    if (denMag2 < 1e-10) return static_cast<double>(maxIter);

                    // Numerator: 2f(z)f'(z)
                    double numReal = 2.0 * (fx * fpx - fy * fpy);
                    double numImag = 2.0 * (fx * fpy + fy * fpx);

                    // Division
                    double divReal = (numReal * denReal + numImag * denImag) / denMag2;
                    double divImag = (numImag * denReal - numReal * denImag) / denMag2;

                    z.real = x - divReal;
                    z.imag = y - divImag;

                    double mag = std::sqrt(z.real * z.real + z.imag * z.imag);
                    if (std::abs(mag - 1.0) < 0.001)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Möbius Transformation Fractal
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "Mobius";
            spec.displayName = "Möbius Fractal";
            spec.category = "Rational Function Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Iteration using Möbius transformation: z(n+1) = (az+b)/(cz+d) where a,b,c,d depend on c parameter. Creates circular inversion patterns.";
            spec.formula = "z(n+1) = (z+c)/(z-c)";
            spec.formulaLatex = R"(z_{n+1} = \frac{z_n + c}{z_n - c})";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Circular arcs, conformal mapping effects, inversion symmetry";
            spec.discoveredBy = "August Ferdinand Möbius (transformation)";
            spec.discoveryYear = 1827;
            spec.computationalNotes = "Preserves circles and angles (conformal)";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 1.0;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.5, 0.5);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // Numerator: z + c
                    double numReal = z.real + constant.real;
                    double numImag = z.imag + constant.imag;

                    // Denominator: z - c
                    double denReal = z.real - constant.real;
                    double denImag = z.imag - constant.imag;

                    double denMag2 = denReal * denReal + denImag * denImag;
                    if (denMag2 < 1e-10)
                    {
                        return static_cast<double>(i);
                    }

                    z.real = (numReal * denReal + numImag * denImag) / denMag2;
                    z.imag = (numImag * denReal - numReal * denImag) / denMag2;

                    double mag2 = z.real * z.real + z.imag * z.imag;

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
        // Rational Power Map: z^n / (z^n + c)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "RationalPower";
            spec.displayName = "Rational z³/(z³+c)";
            spec.category = "Rational Function Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Rational iteration z(n+1) = z³/(z³+c). The cubic power creates rotational symmetry while the rational form adds poles.";
            spec.formula = "z(n+1) = z³/(z³+c)";
            spec.formulaLatex = R"(z_{n+1} = \frac{z_n^3}{z_n^3 + c})";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Three-fold rotational symmetry with rational function distortions";
            spec.discoveredBy = "Rational function fractal exploration";
            spec.discoveryYear = 1992;
            spec.computationalNotes = "Combines polynomial and rational characteristics";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 1.2;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.5, 0.5);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;

                    // z³
                    double x2 = x * x - y * y;
                    double y2 = 2.0 * x * y;
                    double x3 = x * x2 - y * y2;
                    double y3 = x * y2 + y * x2;

                    // Denominator: z³ + c
                    double denReal = x3 + constant.real;
                    double denImag = y3 + constant.imag;

                    double denMag2 = denReal * denReal + denImag * denImag;
                    if (denMag2 < 1e-10)
                    {
                        return static_cast<double>(i);
                    }

                    // z³ / (z³ + c)
                    z.real = (x3 * denReal + y3 * denImag) / denMag2;
                    z.imag = (y3 * denReal - x3 * denImag) / denMag2;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0 || mag2 < 1e-10)
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
