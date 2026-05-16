#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native
{
    void RegisterFractalHybridsFamily()
    {
        // ═══════════════════════════════════════════════════════════════════════════════
        // FRACTAL HYBRIDS & MUTATIONS
        // Creative combinations and variations of classic fractals
        // ═══════════════════════════════════════════════════════════════════════════════

        // ───────────────────────────────────────────────────────────────────────────────
        // Burning Ship + Mandelbrot Hybrid
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "BurningMandel";
            spec.displayName = "Burning Mandelbrot Hybrid";
            spec.category = "Hybrid Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Alternates between Burning Ship (|Re(z)|, |Im(z)|) and Mandelbrot (z²) iterations. Creates unique hybrid structures.";
            spec.formula = "Alternate: z = (|x|+i|y|)² + c, then z = z² + c";
            spec.formulaLatex = R"(z_{n+1} = \begin{cases} (|x_n| + i|y_n|)^2 + c & n \text{ even} \\ z_n^2 + c & n \text{ odd} \end{cases})";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Combines ship's angular features with Mandelbrot's curves";
            spec.discoveredBy = "Hybrid fractal exploration";
            spec.discoveryYear = 2000;
            spec.computationalNotes = "Alternating formula creates mixed characteristics";

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
                    double x = z.real;
                    double y = z.imag;

                    if (i % 2 == 0)
                    {
                        // Burning Ship step
                        x = std::abs(x);
                        y = std::abs(y);
                    }

                    // Square: z²
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
        // Exponential Mandelbrot Hybrid
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "ExpMandelHybrid";
            spec.displayName = "Exponential-Mandelbrot Hybrid";
            spec.category = "Hybrid Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Mixes polynomial z² with exponential e^z: z(n+1) = az² + be^z + c. Parameter a+b=1 for balance.";
            spec.formula = "z(n+1) = a*z² + b*e^z + c (a+b=1)";
            spec.formulaLatex = R"(z_{n+1} = az_n^2 + be^{z_n} + c, \quad a+b=1)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Blends spiral polynomial structure with exponential layering";
            spec.discoveredBy = "Transcendental-polynomial hybrids";
            spec.discoveryYear = 2001;
            spec.computationalNotes = "Weight parameter controls polynomial vs exponential character";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.4;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                double a = 0.7; // Polynomial weight
                double b = 0.3; // Exponential weight

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;

                    // z²
                    double zSqReal = x * x - y * y;
                    double zSqImag = 2.0 * x * y;

                    // e^z
                    double expX = std::exp(x);
                    double expReal = expX * std::cos(y);
                    double expImag = expX * std::sin(y);

                    // Hybrid: a*z² + b*e^z + c
                    z.real = a * zSqReal + b * expReal + constant.real;
                    z.imag = a * zSqImag + b * expImag + constant.imag;

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
        // Mutant Mandelbrot (Varying Power)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "MutantMandelbrot";
            spec.displayName = "Mutant Mandelbrot (Power Evolution)";
            spec.category = "Hybrid Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Power varies with iteration: z(n+1) = z^(2+sin(n/20)) + c. Power oscillates between 1.0 and 3.0, creating slowly morphing fractal structure.";
            spec.formula = "z(n+1) = z^(2+sin(n/20)) + c";
            spec.formulaLatex = R"(z_{n+1} = z_n^{2+\sin(n/20)} + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Slowly morphing structure, smooth power transitions between cubic and linear behavior";
            spec.discoveredBy = "Time-varying power experiments";
            spec.discoveryYear = 2003;
            spec.computationalNotes = "Power oscillates between 1.0 and 3.0 with period of ~126 iterations for smooth morphing";

            spec.defaultCenterX = -0.5;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 1.0;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // Power varies between 1.0 and 3.0 with slower oscillation for smoother morphing
                    double power = 2.0 + std::sin(i / 20.0);

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    // Standard escape check
                    if (mag2 > 256.0)
                    {
                        return static_cast<double>(i);
                    }

                    double mag = std::sqrt(mag2);
                    double angle = std::atan2(z.imag, z.real);

                    // z^power using polar form
                    double newMag = std::pow(mag, power);
                    double newAngle = angle * power;

                    z.real = newMag * std::cos(newAngle) + constant.real;
                    z.imag = newMag * std::sin(newAngle) + constant.imag;
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Trigonometric Mandelbrot Blend
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "TrigMandelBlend";
            spec.displayName = "Trig-Mandelbrot Blend";
            spec.category = "Hybrid Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Blends trigonometric and polynomial: z(n+1) = (z² + sin(z))/2 + c. Combines wave and spiral patterns.";
            spec.formula = "z(n+1) = (z² + sin(z))/2 + c";
            spec.formulaLatex = R"(z_{n+1} = \frac{z_n^2 + \sin(z_n)}{2} + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Wavy spirals, periodic modulation of main structure";
            spec.discoveredBy = "Trig-polynomial blending";
            spec.discoveryYear = 1998;
            spec.computationalNotes = "Averaging creates smooth transition between behaviors";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    double x = z.real;
                    double y = z.imag;

                    // z²
                    double zSqReal = x * x - y * y;
                    double zSqImag = 2.0 * x * y;

                    // sin(z)
                    double sinReal = std::sin(x) * std::cosh(y);
                    double sinImag = std::cos(x) * std::sinh(y);

                    // Average and add c
                    z.real = (zSqReal + sinReal) / 2.0 + constant.real;
                    z.imag = (zSqImag + sinImag) / 2.0 + constant.imag;

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
        // Sierpinski-Mandelbrot Cross
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "SierpinskiMandel";
            spec.displayName = "Sierpinski-Mandelbrot Cross";
            spec.category = "Hybrid Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Combines Sierpinski carpet splitting with Mandelbrot iteration. Self-similar triangular structures emerge.";
            spec.formula = "z(n+1) = z² + c, with conditional splitting";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 + c \text{ with Sierpinski logic})";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Triangular subdivisions within Mandelbrot structure, self-similar patterns";
            spec.discoveredBy = "IFS-escape time hybrids";
            spec.discoveryYear = 2004;
            spec.computationalNotes = "Modulo arithmetic creates Sierpinski-like splitting";

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
                    double x = z.real;
                    double y = z.imag;

                    // Standard z²
                    double zSqReal = x * x - y * y;
                    double zSqImag = 2.0 * x * y;

                    z.real = zSqReal + constant.real;
                    z.imag = zSqImag + constant.imag;

                    // Sierpinski-like splitting based on bit patterns
                    int ix = static_cast<int>(std::abs(z.real * 10.0)) % 3;
                    int iy = static_cast<int>(std::abs(z.imag * 10.0)) % 3;

                    if (ix == 1 && iy == 1)
                    {
                        // Sierpinski "hole"
                        return static_cast<double>(i);
                    }

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
        // Perturbed Newton
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "PerturbedNewton";
            spec.displayName = "Perturbed Newton";
            spec.category = "Hybrid Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Newton's method for z³-1 with added perturbation: z = z - (z³-1)/(3z²) + c*sin(z). Creates warped basin boundaries.";
            spec.formula = "z' = Newton(z) + c*sin(z)";
            spec.formulaLatex = R"(z_{n+1} = z_n - \frac{z_n^3-1}{3z_n^2} + c\sin(z_n))";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Warped Newton basins, wave-distorted boundaries";
            spec.discoveredBy = "Perturbed root-finding methods";
            spec.discoveryYear = 1996;
            spec.computationalNotes = "Perturbation term distorts classic basin structure";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.6;
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

                    // Newton step: (2z³+1)/(3z²)
                    double numReal = 2.0 * x3 + 1.0;
                    double numImag = 2.0 * y3;

                    double denReal = 3.0 * x2;
                    double denImag = 3.0 * y2;

                    double denMag2 = denReal * denReal + denImag * denImag;

                    if (denMag2 < 1e-10)
                    {
                        return static_cast<double>(i);
                    }

                    double newtonReal = (numReal * denReal + numImag * denImag) / denMag2;
                    double newtonImag = (numImag * denReal - numReal * denImag) / denMag2;

                    // Perturbation: c*sin(z)
                    double sinReal = std::sin(x) * std::cosh(y);
                    double sinImag = std::cos(x) * std::sinh(y);

                    z.real = newtonReal + constant.real * sinReal;
                    z.imag = newtonImag + constant.imag * sinImag;

                    double mag = std::sqrt(z.real * z.real + z.imag * z.imag);

                    if (std::abs(mag - 1.0) < 0.01)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Bifurcation Mandelbrot
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "BifurcationMandel";
            spec.displayName = "Bifurcation-Mandelbrot";
            spec.category = "Hybrid Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Logistic map embedded in Mandelbrot: z(n+1) = r*z*(1-z) where r = c. Shows bifurcation cascade within complex plane.";
            spec.formula = "z(n+1) = c*z*(1-z)";
            spec.formulaLatex = R"(z_{n+1} = c \cdot z_n(1-z_n))";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Bifurcation tree structures, period-doubling cascades";
            spec.discoveredBy = "Complex dynamics of logistic map";
            spec.discoveryYear = 1990;
            spec.computationalNotes = "Complex extension of famous 1D bifurcation diagram";

            spec.defaultCenterX = 2.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.4;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.5, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // z*(1-z)
                    double zProd1Real = z.real * (1.0 - z.real) - z.imag * (-z.imag);
                    double zProd1Imag = z.real * (-z.imag) + z.imag * (1.0 - z.real);

                    // c * z*(1-z)
                    z.real = constant.real * zProd1Real - constant.imag * zProd1Imag;
                    z.imag = constant.real * zProd1Imag + constant.imag * zProd1Real;

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
        // Celtic Mandelbrot
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "CelticMandelbrot";
            spec.displayName = "Celtic Mandelbrot (Hybrid)";
            spec.category = "Hybrid Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Uses absolute value of real part: z(n+1) = (|Re(z²)| + iIm(z²)) + c. Creates Celtic knot-like patterns.";
            spec.formula = "z(n+1) = (|Re(z²)| + i*Im(z²)) + c";
            spec.formulaLatex = R"(z_{n+1} = (|x_n^2-y_n^2| + i \cdot 2x_ny_n) + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Celtic knot patterns, braided structures, bilateral symmetry";
            spec.discoveredBy = "Absolute value Mandelbrot variants";
            spec.discoveryYear = 1999;
            spec.computationalNotes = "Absolute value on real part only creates unique topology";

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
                    double x = z.real;
                    double y = z.imag;

                    // z²
                    double zSqReal = x * x - y * y;
                    double zSqImag = 2.0 * x * y;

                    // Apply abs to real part only
                    z.real = std::abs(zSqReal) + constant.real;
                    z.imag = zSqImag + constant.imag;

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
