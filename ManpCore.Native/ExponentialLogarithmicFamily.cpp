#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

#ifndef M_PI
#define M_PI 3.14159265358979323846
#endif

namespace Native
{
    void RegisterExponentialLogarithmicFamily()
    {
        // ═══════════════════════════════════════════════════════════════════════════════
        // EXPONENTIAL & LOGARITHMIC FRACTALS
        // ═══════════════════════════════════════════════════════════════════════════════

        // ───────────────────────────────────────────────────────────────────────────────
        // Exponential Mandelbrot
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "ExponentialLogarithmic";
            spec.displayName = "Exponential Mandelbrot";
            spec.category = "Exponential Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Uses exponential function in iteration: z(n+1) = exp(z(n)) + c. Creates spiraling patterns with period-based structure.";
            spec.formula = "z(n+1) = exp(z(n)) + c";
            spec.formulaLatex = R"(z_{n+1} = e^{z_n} + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Spiral layers, exponential growth patterns, periodic structure in imaginary direction";
            spec.discoveredBy = "Early fractal experimenters";
            spec.discoveryYear = 1985;
            spec.computationalNotes = "exp(x+iy) = exp(x)(cos(y) + i*sin(y)); periodic in y with period 2π";

            spec.defaultCenterX = -1.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.2;  // Viewport tuning: X scale 20, Y scale 11.25
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // exp(z) = exp(x+iy) = exp(x) * (cos(y) + i*sin(y))
                    double expX = std::exp(z.real);
                    z.real = expX * std::cos(z.imag) + constant.real;
                    z.imag = expX * std::sin(z.imag) + constant.imag;

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
        // Logarithmic Mandelbrot
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "Logarithmic";
            spec.displayName = "Logarithmic Mandelbrot";
            spec.category = "Exponential Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Uses logarithmic function in iteration: z(n+1) = log(z(n)) + c. Creates branch-cut discontinuities along the negative real axis.";
            spec.formula = "z(n+1) = log(z(n)) + c";
            spec.formulaLatex = R"(z_{n+1} = \ln(z_n) + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Branch cut structure, logarithmic spirals, discontinuity effects along negative real axis";
            spec.discoveredBy = "Early fractal experimenters";
            spec.discoveryYear = 1985;
            spec.computationalNotes = "log(z) = log|z| + i*arg(z); requires magnitude check to avoid log(0)";

            spec.defaultCenterX = 0.24;
            spec.defaultCenterY = -0.3;
            spec.defaultZoom = 2.067161;  // Viewport tuning: X scale 1.935, Y scale 1.088
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.1, 0.1);
                ComplexD constant = isJulia ? juliaC : c;
                ComplexD zPrev = z;

                for (int i = 0; i < maxIter; ++i)
                {
                    double mag = std::sqrt(z.real * z.real + z.imag * z.imag);

                    if (mag < 1e-10)
                    {
                        return static_cast<double>(i);  // Converged to zero
                    }

                    // log(z) = log|z| + i*arg(z)
                    double logMag = std::log(mag);
                    double arg = std::atan2(z.imag, z.real);

                    z.real = logMag + constant.real;
                    z.imag = arg + constant.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    // Check for escape (rare but possible)
                    if (mag2 > 100.0)
                    {
                        return static_cast<double>(i);
                    }

                    // Check for convergence (primary exit condition for logarithmic fractals)
                    double dx = z.real - zPrev.real;
                    double dy = z.imag - zPrev.imag;
                    double delta = dx * dx + dy * dy;

                    if (delta < 1e-8)
                        return static_cast<double>(maxIter - i);  // Converged: return inverse iteration count for coloring

                    zPrev = z;
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Exponential Square (exp(z²) + c)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "ExpSquare";
            spec.displayName = "Exponential Square";
            spec.category = "Exponential Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Combines squaring and exponential: z(n+1) = exp(z²) + c. Creates hybrid structures combining polynomial and exponential characteristics.";
            spec.formula = "z(n+1) = exp(z²) + c";
            spec.formulaLatex = R"(z_{n+1} = e^{z_n^2} + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Combines exponential spirals with quadratic symmetry, rapid growth with rotational structure";
            spec.discoveredBy = "Fractal community exploration";
            spec.discoveryYear = 1990;
            spec.computationalNotes = "First squares z, then applies exponential function";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.3;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // First square: z²
                    double x = z.real;
                    double y = z.imag;
                    double zSquaredReal = x * x - y * y;
                    double zSquaredImag = 2.0 * x * y;

                    // Then exp(z²)
                    double expX = std::exp(zSquaredReal);
                    z.real = expX * std::cos(zSquaredImag) + constant.real;
                    z.imag = expX * std::sin(zSquaredImag) + constant.imag;

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
        // Power Tower (z^z + c)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "PowerTower";
            spec.displayName = "Power Tower (z^z)";
            spec.category = "Exponential Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Uses self-exponentiation: z(n+1) = z^z + c. The power tower operation z^z = exp(z*log(z)) creates highly intricate structures.";
            spec.formula = "z(n+1) = z^z + c";
            spec.formulaLatex = R"(z_{n+1} = z_n^{z_n} + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Extremely complex structure, chaotic regions, multiple scales of detail";
            spec.discoveredBy = "Advanced fractal research";
            spec.discoveryYear = 1995;
            spec.computationalNotes = "z^z = exp(z*ln(z)); computationally intensive";

            spec.defaultCenterX = 0.75;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 1.532567;  // Viewport tuning: X scale 2.61, Y scale 1.47
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.5, 0.5);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    double mag = std::sqrt(z.real * z.real + z.imag * z.imag);

                    if (mag < 1e-10)
                    {
                        return static_cast<double>(maxIter);
                    }

                    // z^z = exp(z * log(z))
                    double logMag = std::log(mag);
                    double arg = std::atan2(z.imag, z.real);
                    double logReal = logMag;
                    double logImag = arg;

                    // Multiply: z * log(z)
                    double productReal = z.real * logReal - z.imag * logImag;
                    double productImag = z.real * logImag + z.imag * logReal;

                    // Exponentiate: exp(z * log(z))
                    double expX = std::exp(productReal);
                    z.real = expX * std::cos(productImag) + constant.real;
                    z.imag = expX * std::sin(productImag) + constant.imag;

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
        // Complex Power (z^c + c)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "ComplexPower";
            spec.displayName = "Complex Power";
            spec.category = "Exponential Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Raises z to the power c: z(n+1) = z^c + c. Creates fractal patterns that vary dramatically with the parameter c.";
            spec.formula = "z(n+1) = z^c + c";
            spec.formulaLatex = R"(z_{n+1} = z_n^c + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Varies greatly with parameter c, can show polynomial or exponential behavior";
            spec.discoveredBy = "Fractal community exploration";
            spec.discoveryYear = 1990;
            spec.computationalNotes = "z^c = exp(c*ln(z)); parameter-dependent structure";

            spec.defaultCenterX = 0.5;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 1.333333;  // Viewport tuning: X scale 3.0, Y scale 1.69
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.5, 0.5);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    double mag = std::sqrt(z.real * z.real + z.imag * z.imag);

                    if (mag < 1e-10)
                    {
                        return static_cast<double>(maxIter);
                    }

                    // z^c = exp(c * log(z))
                    double logMag = std::log(mag);
                    double arg = std::atan2(z.imag, z.real);

                    // c * log(z)
                    double productReal = constant.real * logMag - constant.imag * arg;
                    double productImag = constant.real * arg + constant.imag * logMag;

                    // exp(c * log(z))
                    double expX = std::exp(productReal);
                    z.real = expX * std::cos(productImag) + constant.real;
                    z.imag = expX * std::sin(productImag) + constant.imag;

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
        // Exponential Julia (c*exp(z) + z)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "ExponentialJulia";
            spec.displayName = "Exponential Julia";
            spec.category = "Exponential Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Hybrid formula: z(n+1) = c*exp(z) + z. Combines exponential growth with linear feedback.";
            spec.formula = "z(n+1) = c*exp(z) + z";
            spec.formulaLatex = R"(z_{n+1} = c \cdot e^{z_n} + z_n)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Exponential spirals with feedback, parameter-sensitive behavior";
            spec.discoveredBy = "Fractal formula experiments";
            spec.discoveryYear = 1992;
            spec.computationalNotes = "Linear term provides stability, exponential term provides growth";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // exp(z)
                    double expX = std::exp(z.real);
                    double expReal = expX * std::cos(z.imag);
                    double expImag = expX * std::sin(z.imag);

                    // c * exp(z) + z
                    double newReal = constant.real * expReal - constant.imag * expImag + z.real;
                    double newImag = constant.real * expImag + constant.imag * expReal + z.imag;

                    z.real = newReal;
                    z.imag = newImag;

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
    }
}
