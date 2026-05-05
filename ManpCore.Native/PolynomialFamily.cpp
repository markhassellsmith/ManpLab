#include "pch.h"
#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace ManpCore::Native
{
    void RegisterPolynomialFamily()
    {
        // ═══════════════════════════════════════════════════════════════════════════════
        // MULTIBROT FAMILY - Polynomial generalizations of Mandelbrot set
        // ═══════════════════════════════════════════════════════════════════════════════

        // ───────────────────────────────────────────────────────────────────────────────
        // Multibrot-3 (Cubic Mandelbrot)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "Multibrot-3";
            spec.displayName = "Multibrot-3 (Cubic)";
            spec.category = "Polynomial Fractals";
            spec.description = "Third-order polynomial fractal using z³+c iteration. Features three-fold rotational symmetry and produces distinctive triple-spiral arms. The main body is more triangular than the classic Mandelbrot set.";
            spec.formula = "z(n+1) = z(n)³ + c";
            spec.formulaLatex = R"(z_{n+1} = z_n^3 + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Triangular main body with three major spiral arms, three-fold rotational symmetry, sharper cusps than Mandelbrot set";
            spec.discoveredBy = "Derived from Mandelbrot's work";
            spec.discoveryYear = 1980;
            spec.computationalNotes = "Slightly slower than quadratic due to cubic multiplication";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.8;
            spec.defaultMaxIterations = 256;

            spec.calculator = [](double cx, double cy, int maxIter, IterationMode mode, double juliaReal, double juliaImag) -> IterationResult
            {
                IterationResult result{};
                result.escaped = false;
                result.finalMagnitude = 0.0;

                ComplexD z = (mode == IterationMode::Julia) ? ComplexD(cx, cy) : ComplexD(0.0, 0.0);
                ComplexD c = (mode == IterationMode::Julia) ? ComplexD(juliaReal, juliaImag) : ComplexD(cx, cy);

                for (int i = 0; i < maxIter; ++i)
                {
                    // z³ = (x + iy)³ = x³ + 3x²(iy) + 3x(iy)² + (iy)³
                    //    = x³ - 3xy² + i(3x²y - y³)
                    double x = z.real;
                    double y = z.imag;
                    double x2 = x * x;
                    double y2 = y * y;

                    z.real = x * x2 - 3.0 * x * y2 + c.real;
                    z.imag = 3.0 * x2 * y - y * y2 + c.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 4.0)
                    {
                        result.escaped = true;
                        result.iterations = i;
                        result.finalMagnitude = std::sqrt(mag2);
                        result.finalReal = z.real;
                        result.finalImag = z.imag;
                        return result;
                    }
                }

                result.iterations = maxIter;
                result.finalMagnitude = std::sqrt(z.real * z.real + z.imag * z.imag);
                result.finalReal = z.real;
                result.finalImag = z.imag;
                return result;
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
            spec.category = "Polynomial Fractals";
            spec.description = "Fourth-order polynomial fractal using z⁴+c iteration. Exhibits four-fold rotational symmetry with square-like main body and four major spiral arms.";
            spec.formula = "z(n+1) = z(n)⁴ + c";
            spec.formulaLatex = R"(z_{n+1} = z_n^4 + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Square main body, four-fold rotational symmetry, four spiral arms arranged in cross pattern";
            spec.discoveredBy = "Derived from Mandelbrot's work";
            spec.discoveryYear = 1980;
            spec.computationalNotes = "Requires quartic calculation per iteration";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 1.0;
            spec.defaultMaxIterations = 256;

            spec.calculator = [](double cx, double cy, int maxIter, IterationMode mode, double juliaReal, double juliaImag) -> IterationResult
            {
                IterationResult result{};
                result.escaped = false;
                result.finalMagnitude = 0.0;

                ComplexD z = (mode == IterationMode::Julia) ? ComplexD(cx, cy) : ComplexD(0.0, 0.0);
                ComplexD c = (mode == IterationMode::Julia) ? ComplexD(juliaReal, juliaImag) : ComplexD(cx, cy);

                for (int i = 0; i < maxIter; ++i)
                {
                    // z⁴ = (z²)²
                    double x = z.real;
                    double y = z.imag;

                    // First compute z²
                    double x2 = x * x - y * y;
                    double y2 = 2.0 * x * y;

                    // Then square again for z⁴
                    z.real = x2 * x2 - y2 * y2 + c.real;
                    z.imag = 2.0 * x2 * y2 + c.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 4.0)
                    {
                        result.escaped = true;
                        result.iterations = i;
                        result.finalMagnitude = std::sqrt(mag2);
                        result.finalReal = z.real;
                        result.finalImag = z.imag;
                        return result;
                    }
                }

                result.iterations = maxIter;
                result.finalMagnitude = std::sqrt(z.real * z.real + z.imag * z.imag);
                result.finalReal = z.real;
                result.finalImag = z.imag;
                return result;
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
            spec.category = "Polynomial Fractals";
            spec.description = "Fifth-order polynomial fractal using z⁵+c iteration. Shows five-fold rotational symmetry with pentagonal structure and five major spiral arms.";
            spec.formula = "z(n+1) = z(n)⁵ + c";
            spec.formulaLatex = R"(z_{n+1} = z_n^5 + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Pentagonal main body, five-fold rotational symmetry, five evenly-spaced spiral arms";
            spec.discoveredBy = "Derived from Mandelbrot's work";
            spec.discoveryYear = 1980;
            spec.computationalNotes = "Computationally intensive due to quintic power";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 1.2;
            spec.defaultMaxIterations = 256;

            spec.calculator = [](double cx, double cy, int maxIter, IterationMode mode, double juliaReal, double juliaImag) -> IterationResult
            {
                IterationResult result{};
                result.escaped = false;
                result.finalMagnitude = 0.0;

                ComplexD z = (mode == IterationMode::Julia) ? ComplexD(cx, cy) : ComplexD(0.0, 0.0);
                ComplexD c = (mode == IterationMode::Julia) ? ComplexD(juliaReal, juliaImag) : ComplexD(cx, cy);

                for (int i = 0; i < maxIter; ++i)
                {
                    // z⁵ = z⁴ * z = (z²)² * z
                    double x = z.real;
                    double y = z.imag;

                    // Compute z²
                    double x2 = x * x - y * y;
                    double y2 = 2.0 * x * y;

                    // Compute z⁴ = (z²)²
                    double x4 = x2 * x2 - y2 * y2;
                    double y4 = 2.0 * x2 * y2;

                    // Compute z⁵ = z⁴ * z
                    z.real = x4 * x - y4 * y + c.real;
                    z.imag = x4 * y + y4 * x + c.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 4.0)
                    {
                        result.escaped = true;
                        result.iterations = i;
                        result.finalMagnitude = std::sqrt(mag2);
                        result.finalReal = z.real;
                        result.finalImag = z.imag;
                        return result;
                    }
                }

                result.iterations = maxIter;
                result.finalMagnitude = std::sqrt(z.real * z.real + z.imag * z.imag);
                result.finalReal = z.real;
                result.finalImag = z.imag;
                return result;
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
            spec.category = "Polynomial Fractals";
            spec.description = "Sixth-order polynomial fractal using z⁶+c iteration. Features six-fold rotational symmetry with hexagonal main structure and six spiral arms.";
            spec.formula = "z(n+1) = z(n)⁶ + c";
            spec.formulaLatex = R"(z_{n+1} = z_n^6 + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Hexagonal main body, six-fold rotational symmetry, snowflake-like patterns in detail areas";
            spec.discoveredBy = "Derived from Mandelbrot's work";
            spec.discoveryYear = 1980;
            spec.computationalNotes = "Can be computed efficiently as (z³)²";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 1.3;
            spec.defaultMaxIterations = 256;

            spec.calculator = [](double cx, double cy, int maxIter, IterationMode mode, double juliaReal, double juliaImag) -> IterationResult
            {
                IterationResult result{};
                result.escaped = false;
                result.finalMagnitude = 0.0;

                ComplexD z = (mode == IterationMode::Julia) ? ComplexD(cx, cy) : ComplexD(0.0, 0.0);
                ComplexD c = (mode == IterationMode::Julia) ? ComplexD(juliaReal, juliaImag) : ComplexD(cx, cy);

                for (int i = 0; i < maxIter; ++i)
                {
                    // z⁶ = (z³)²
                    double x = z.real;
                    double y = z.imag;
                    double x2 = x * x;
                    double y2 = y * y;

                    // Compute z³
                    double x3 = x * x2 - 3.0 * x * y2;
                    double y3 = 3.0 * x2 * y - y * y2;

                    // Square z³ to get z⁶
                    z.real = x3 * x3 - y3 * y3 + c.real;
                    z.imag = 2.0 * x3 * y3 + c.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 4.0)
                    {
                        result.escaped = true;
                        result.iterations = i;
                        result.finalMagnitude = std::sqrt(mag2);
                        result.finalReal = z.real;
                        result.finalImag = z.imag;
                        return result;
                    }
                }

                result.iterations = maxIter;
                result.finalMagnitude = std::sqrt(z.real * z.real + z.imag * z.imag);
                result.finalReal = z.real;
                result.finalImag = z.imag;
                return result;
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
            spec.category = "Polynomial Fractals";
            spec.description = "Eighth-order polynomial fractal using z⁸+c iteration. Exhibits eight-fold rotational symmetry with octagonal structure and eight symmetrically arranged spiral arms.";
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
            spec.defaultMaxIterations = 256;

            spec.calculator = [](double cx, double cy, int maxIter, IterationMode mode, double juliaReal, double juliaImag) -> IterationResult
            {
                IterationResult result{};
                result.escaped = false;
                result.finalMagnitude = 0.0;

                ComplexD z = (mode == IterationMode::Julia) ? ComplexD(cx, cy) : ComplexD(0.0, 0.0);
                ComplexD c = (mode == IterationMode::Julia) ? ComplexD(juliaReal, juliaImag) : ComplexD(cx, cy);

                for (int i = 0; i < maxIter; ++i)
                {
                    // z⁸ = ((z²)²)²
                    double x = z.real;
                    double y = z.imag;

                    // Compute z²
                    double x2 = x * x - y * y;
                    double y2 = 2.0 * x * y;

                    // Compute z⁴ = (z²)²
                    double x4 = x2 * x2 - y2 * y2;
                    double y4 = 2.0 * x2 * y2;

                    // Compute z⁸ = (z⁴)²
                    z.real = x4 * x4 - y4 * y4 + c.real;
                    z.imag = 2.0 * x4 * y4 + c.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 4.0)
                    {
                        result.escaped = true;
                        result.iterations = i;
                        result.finalMagnitude = std::sqrt(mag2);
                        result.finalReal = z.real;
                        result.finalImag = z.imag;
                        return result;
                    }
                }

                result.iterations = maxIter;
                result.finalMagnitude = std::sqrt(z.real * z.real + z.imag * z.imag);
                result.finalReal = z.real;
                result.finalImag = z.imag;
                return result;
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
            spec.category = "Polynomial Fractals";
            spec.description = "Tenth-order polynomial fractal using z¹⁰+c iteration. Shows ten-fold rotational symmetry with decagonal structure and ten evenly-spaced spiral arms.";
            spec.formula = "z(n+1) = z(n)¹⁰ + c";
            spec.formulaLatex = R"(z_{n+1} = z_n^{10} + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Decagonal main body, ten-fold rotational symmetry, nearly circular appearance with fine detail";
            spec.discoveredBy = "Derived from Mandelbrot's work";
            spec.discoveryYear = 1980;
            spec.computationalNotes = "Computed as (z⁵)² for efficiency";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 1.5;
            spec.defaultMaxIterations = 256;

            spec.calculator = [](double cx, double cy, int maxIter, IterationMode mode, double juliaReal, double juliaImag) -> IterationResult
            {
                IterationResult result{};
                result.escaped = false;
                result.finalMagnitude = 0.0;

                ComplexD z = (mode == IterationMode::Julia) ? ComplexD(cx, cy) : ComplexD(0.0, 0.0);
                ComplexD c = (mode == IterationMode::Julia) ? ComplexD(juliaReal, juliaImag) : ComplexD(cx, cy);

                for (int i = 0; i < maxIter; ++i)
                {
                    // z¹⁰ = (z⁵)²
                    double x = z.real;
                    double y = z.imag;

                    // Compute z² first
                    double x2 = x * x - y * y;
                    double y2 = 2.0 * x * y;

                    // Compute z⁴
                    double x4 = x2 * x2 - y2 * y2;
                    double y4 = 2.0 * x2 * y2;

                    // Compute z⁵ = z⁴ * z
                    double x5 = x4 * x - y4 * y;
                    double y5 = x4 * y + y4 * x;

                    // Square z⁵ to get z¹⁰
                    z.real = x5 * x5 - y5 * y5 + c.real;
                    z.imag = 2.0 * x5 * y5 + c.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 4.0)
                    {
                        result.escaped = true;
                        result.iterations = i;
                        result.finalMagnitude = std::sqrt(mag2);
                        result.finalReal = z.real;
                        result.finalImag = z.imag;
                        return result;
                    }
                }

                result.iterations = maxIter;
                result.finalMagnitude = std::sqrt(z.real * z.real + z.imag * z.imag);
                result.finalReal = z.real;
                result.finalImag = z.imag;
                return result;
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Tricorn (Anti-Mandelbrot / Mandelbar)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "Tricorn";
            spec.displayName = "Tricorn (Mandelbar)";
            spec.category = "Polynomial Fractals";
            spec.description = "Uses conjugate iteration z̄²+c instead of z²+c. Breaks rotational symmetry but preserves reflection symmetry across the real axis. Features distinctive three-pointed structure.";
            spec.formula = "z(n+1) = conjugate(z(n))² + c";
            spec.formulaLatex = R"(z_{n+1} = \overline{z_n}^2 + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Three prominent spikes, reflection symmetry only, lacks rotational symmetry of Mandelbrot set";
            spec.discoveredBy = "W. D. Crowe, R. Hasson, P. J. Rippon, P. E. D. Strain-Clark";
            spec.discoveryYear = 1989;
            spec.computationalNotes = "Uses complex conjugate before squaring";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.6;
            spec.defaultMaxIterations = 256;

            spec.calculator = [](double cx, double cy, int maxIter, IterationMode mode, double juliaReal, double juliaImag) -> IterationResult
            {
                IterationResult result{};
                result.escaped = false;
                result.finalMagnitude = 0.0;

                ComplexD z = (mode == IterationMode::Julia) ? ComplexD(cx, cy) : ComplexD(0.0, 0.0);
                ComplexD c = (mode == IterationMode::Julia) ? ComplexD(juliaReal, juliaImag) : ComplexD(cx, cy);

                for (int i = 0; i < maxIter; ++i)
                {
                    // Conjugate: z̄ = x - iy, then square
                    double x = z.real;
                    double y = -z.imag;  // Conjugate

                    z.real = x * x - y * y + c.real;
                    z.imag = 2.0 * x * y + c.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 4.0)
                    {
                        result.escaped = true;
                        result.iterations = i;
                        result.finalMagnitude = std::sqrt(mag2);
                        result.finalReal = z.real;
                        result.finalImag = z.imag;
                        return result;
                    }
                }

                result.iterations = maxIter;
                result.finalMagnitude = std::sqrt(z.real * z.real + z.imag * z.imag);
                result.finalReal = z.real;
                result.finalImag = z.imag;
                return result;
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Buffalo (Modified Tricorn)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "Buffalo";
            spec.displayName = "Buffalo Fractal";
            spec.category = "Polynomial Fractals";
            spec.description = "Variant using |Re(z)| + i*|Im(z)| before squaring. Creates distinctive buffalo-head shape with bilateral symmetry across both axes.";
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
            spec.defaultMaxIterations = 256;

            spec.calculator = [](double cx, double cy, int maxIter, IterationMode mode, double juliaReal, double juliaImag) -> IterationResult
            {
                IterationResult result{};
                result.escaped = false;
                result.finalMagnitude = 0.0;

                ComplexD z = (mode == IterationMode::Julia) ? ComplexD(cx, cy) : ComplexD(0.0, 0.0);
                ComplexD c = (mode == IterationMode::Julia) ? ComplexD(juliaReal, juliaImag) : ComplexD(cx, cy);

                for (int i = 0; i < maxIter; ++i)
                {
                    // Apply absolute value to both components
                    double x = std::abs(z.real);
                    double y = std::abs(z.imag);

                    z.real = x * x - y * y + c.real;
                    z.imag = 2.0 * x * y + c.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 4.0)
                    {
                        result.escaped = true;
                        result.iterations = i;
                        result.finalMagnitude = std::sqrt(mag2);
                        result.finalReal = z.real;
                        result.finalImag = z.imag;
                        return result;
                    }
                }

                result.iterations = maxIter;
                result.finalMagnitude = std::sqrt(z.real * z.real + z.imag * z.imag);
                result.finalReal = z.real;
                result.finalImag = z.imag;
                return result;
            };

            FractalRegistry::Register(spec);
        }
    }
}
