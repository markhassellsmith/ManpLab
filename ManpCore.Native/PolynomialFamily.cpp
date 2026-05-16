#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native
{
    void RegisterPolynomialFamily()
    {
        // ═══════════════════════════════════════════════════════════════════════════════
        // MULTIBROT POWERS - Pure polynomial generalizations z^n + c
        // ═══════════════════════════════════════════════════════════════════════════════

        // ───────────────────────────────────────────────────────────────────────────────
        // Multibrot-3 (Cubic Mandelbrot)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "Multibrot-3";
            spec.displayName = "Multibrot-3 (Cubic)";
            spec.category = "Multibrot Powers";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Third-order polynomial fractal using z³+c iteration. Features three-fold rotational symmetry and produces distinctive triple-spiral arms.";
            spec.formula = "z(n+1) = z(n)³ + c";
            spec.formulaLatex = R"(z_{n+1} = z_n^3 + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Triangular main body with three major spiral arms, three-fold rotational symmetry";
            spec.discoveredBy = "Derived from Mandelbrot's work";
            spec.discoveryYear = 1980;
            spec.computationalNotes = "Slightly slower than quadratic due to cubic multiplication";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.8;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // z³ = x³ - 3xy² + i(3x²y - y³)
                    double x = z.real;
                    double y = z.imag;
                    double x2 = x * x;
                    double y2 = y * y;

                    z.real = x * x2 - 3.0 * x * y2 + constant.real;
                    z.imag = 3.0 * x2 * y - y * y2 + constant.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0)
                    {
                        double log_zn = std::log(mag2) / 2.0;
                        double nu = std::log(log_zn / std::log(2.0)) / std::log(2.0);
                        return i + 1.0 - nu;
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Multibrot-4 (Quartic Mandelbrot)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "Multibrot-4";
            spec.displayName = "Multibrot-4 (Quartic)";
            spec.category = "Multibrot Powers";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Fourth-order polynomial fractal using z⁴+c iteration. Exhibits four-fold rotational symmetry with square-like main body.";
            spec.formula = "z(n+1) = z(n)⁴ + c";
            spec.formulaLatex = R"(z_{n+1} = z_n^4 + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Square main body, four-fold rotational symmetry, four spiral arms in cross pattern";
            spec.discoveredBy = "Derived from Mandelbrot's work";
            spec.discoveryYear = 1980;
            spec.computationalNotes = "Computed efficiently as (z²)²";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 1.0;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // z⁴ = (z²)²
                    double x = z.real;
                    double y = z.imag;

                    double x2 = x * x - y * y;
                    double y2 = 2.0 * x * y;

                    z.real = x2 * x2 - y2 * y2 + constant.real;
                    z.imag = 2.0 * x2 * y2 + constant.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0)
                    {
                        double log_zn = std::log(mag2) / 2.0;
                        double nu = std::log(log_zn / std::log(2.0)) / std::log(2.0);
                        return i + 1.0 - nu;
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Multibrot-5 (Quintic Mandelbrot)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "Multibrot-5";
            spec.displayName = "Multibrot-5 (Quintic)";
            spec.category = "Multibrot Powers";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Fifth-order polynomial fractal using z⁵+c iteration. Shows five-fold rotational symmetry with pentagonal structure.";
            spec.formula = "z(n+1) = z(n)⁵ + c";
            spec.formulaLatex = R"(z_{n+1} = z_n^5 + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Pentagonal main body, five-fold rotational symmetry, five evenly-spaced spiral arms";
            spec.discoveredBy = "Derived from Mandelbrot's work";
            spec.discoveryYear = 1980;
            spec.computationalNotes = "Computed as z⁴ * z";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 1.2;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // z⁵ = z⁴ * z
                    double x = z.real;
                    double y = z.imag;

                    double x2 = x * x - y * y;
                    double y2 = 2.0 * x * y;

                    double x4 = x2 * x2 - y2 * y2;
                    double y4 = 2.0 * x2 * y2;

                    z.real = x4 * x - y4 * y + constant.real;
                    z.imag = x4 * y + y4 * x + constant.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0)
                    {
                        double log_zn = std::log(mag2) / 2.0;
                        double nu = std::log(log_zn / std::log(2.0)) / std::log(2.0);
                        return i + 1.0 - nu;
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Multibrot-6 (Sextic Mandelbrot)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "Multibrot-6";
            spec.displayName = "Multibrot-6 (Sextic)";
            spec.category = "Multibrot Powers";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Sixth-order polynomial fractal using z⁶+c iteration. Features six-fold rotational symmetry with hexagonal main structure.";
            spec.formula = "z(n+1) = z(n)⁶ + c";
            spec.formulaLatex = R"(z_{n+1} = z_n^6 + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Hexagonal main body, six-fold rotational symmetry, snowflake-like patterns";
            spec.discoveredBy = "Derived from Mandelbrot's work";
            spec.discoveryYear = 1980;
            spec.computationalNotes = "Computed efficiently as (z³)²";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 1.3;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // z⁶ = (z³)²
                    double x = z.real;
                    double y = z.imag;
                    double x2 = x * x;
                    double y2 = y * y;

                    double x3 = x * x2 - 3.0 * x * y2;
                    double y3 = 3.0 * x2 * y - y * y2;

                    z.real = x3 * x3 - y3 * y3 + constant.real;
                    z.imag = 2.0 * x3 * y3 + constant.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0)
                    {
                        double log_zn = std::log(mag2) / 2.0;
                        double nu = std::log(log_zn / std::log(2.0)) / std::log(2.0);
                        return i + 1.0 - nu;
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Multibrot-8 (Octic Mandelbrot)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "Multibrot-8";
            spec.displayName = "Multibrot-8 (Octic)";
            spec.category = "Multibrot Powers";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Eighth-order polynomial fractal using z⁸+c iteration. Exhibits eight-fold rotational symmetry with octagonal structure.";
            spec.formula = "z(n+1) = z(n)⁸ + c";
            spec.formulaLatex = R"(z_{n+1} = z_n^8 + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Octagonal main body, eight-fold rotational symmetry, highly symmetrical star pattern";
            spec.discoveredBy = "Derived from Mandelbrot's work";
            spec.discoveryYear = 1980;
            spec.computationalNotes = "Efficiently computed as ((z²)²)²";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 1.4;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // z⁸ = ((z²)²)²
                    double x = z.real;
                    double y = z.imag;

                    double x2 = x * x - y * y;
                    double y2 = 2.0 * x * y;

                    double x4 = x2 * x2 - y2 * y2;
                    double y4 = 2.0 * x2 * y2;

                    z.real = x4 * x4 - y4 * y4 + constant.real;
                    z.imag = 2.0 * x4 * y4 + constant.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0)
                    {
                        double log_zn = std::log(mag2) / 2.0;
                        double nu = std::log(log_zn / std::log(2.0)) / std::log(2.0);
                        return i + 1.0 - nu;
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Multibrot-10 (Decic Mandelbrot)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "Multibrot-10";
            spec.displayName = "Multibrot-10 (Decic)";
            spec.category = "Multibrot Powers";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Tenth-order polynomial fractal using z¹⁰+c iteration. Shows ten-fold rotational symmetry with decagonal structure.";
            spec.formula = "z(n+1) = z(n)¹⁰ + c";
            spec.formulaLatex = R"(z_{n+1} = z_n^{10} + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Decagonal main body, ten-fold rotational symmetry, nearly circular appearance";
            spec.discoveredBy = "Derived from Mandelbrot's work";
            spec.discoveryYear = 1980;
            spec.computationalNotes = "Computed as (z⁵)² for efficiency";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 1.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // z¹⁰ = (z⁵)²
                    double x = z.real;
                    double y = z.imag;

                    double x2 = x * x - y * y;
                    double y2 = 2.0 * x * y;

                    double x4 = x2 * x2 - y2 * y2;
                    double y4 = 2.0 * x2 * y2;

                    double x5 = x4 * x - y4 * y;
                    double y5 = x4 * y + y4 * x;

                    z.real = x5 * x5 - y5 * y5 + constant.real;
                    z.imag = 2.0 * x5 * y5 + constant.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0)
                    {
                        double log_zn = std::log(mag2) / 2.0;
                        double nu = std::log(log_zn / std::log(2.0)) / std::log(2.0);
                        return i + 1.0 - nu;
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Tricorn (Anti-Mandelbrot / Mandelbar)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "Tricorn-Poly";
            spec.displayName = "Tricorn (Polynomial)";
            spec.category = "Multibrot Powers";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Uses conjugate iteration z̄²+c instead of z²+c. Features distinctive three-pointed structure with reflection symmetry.";
            spec.formula = "z(n+1) = conjugate(z(n))² + c";
            spec.formulaLatex = R"(z_{n+1} = \overline{z_n}^2 + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Three prominent spikes, reflection symmetry only, lacks rotational symmetry";
            spec.discoveredBy = "W. D. Crowe, R. Hasson, P. J. Rippon, P. E. D. Strain-Clark";
            spec.discoveryYear = 1989;
            spec.computationalNotes = "Uses complex conjugate before squaring";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.6;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // Conjugate: z̄ = x - iy, then square
                    double x = z.real;
                    double y = -z.imag;

                    z.real = x * x - y * y + constant.real;
                    z.imag = 2.0 * x * y + constant.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0)
                    {
                        double log_zn = std::log(mag2) / 2.0;
                        double nu = std::log(log_zn / std::log(2.0)) / std::log(2.0);
                        return i + 1.0 - nu;
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Buffalo (Modified Tricorn)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "BuffaloPolynomial";
            spec.displayName = "Buffalo (Polynomial)";
            spec.category = "Multibrot Powers";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Variant using |Re(z)| + i*|Im(z)| before squaring. Creates distinctive buffalo-head shape with bilateral symmetry.";
            spec.formula = "z(n+1) = (|Re(z)| + i*|Im(z)|)² + c";
            spec.formulaLatex = R"(z_{n+1} = (|x_n| + i|y_n|)^2 + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Buffalo-head shaped main body, bilateral symmetry in both axes, sharp angular features";
            spec.discoveredBy = "Fractal community discovery";
            spec.discoveryYear = 2000;
            spec.computationalNotes = "Applies absolute value to both components before squaring";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.7;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // Apply absolute value to both components
                    double x = std::abs(z.real);
                    double y = std::abs(z.imag);

                    z.real = x * x - y * y + constant.real;
                    z.imag = 2.0 * x * y + constant.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 256.0)
                    {
                        double log_zn = std::log(mag2) / 2.0;
                        double nu = std::log(log_zn / std::log(2.0)) / std::log(2.0);
                        return i + 1.0 - nu;
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }
    }
}
