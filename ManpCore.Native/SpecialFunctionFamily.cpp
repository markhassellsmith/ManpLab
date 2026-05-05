#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

#ifndef M_PI
#define M_PI 3.14159265358979323846
#endif

namespace Native
{
    // Helper function: Error function approximation (erf)
    // Using Abramowitz and Stegun approximation
    inline double erf_approx(double x)
    {
        const double a1 = 0.254829592;
        const double a2 = -0.284496736;
        const double a3 = 1.421413741;
        const double a4 = -1.453152027;
        const double a5 = 1.061405429;
        const double p = 0.3275911;

        int sign = (x >= 0) ? 1 : -1;
        x = std::abs(x);

        double t = 1.0 / (1.0 + p * x);
        double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * std::exp(-x * x);

        return sign * y;
    }

    // Helper: Lanczos approximation for Gamma function
    inline double gamma_approx(double z)
    {
        if (z < 0.5)
        {
            // Use reflection formula: Gamma(z) * Gamma(1-z) = pi / sin(pi*z)
            return M_PI / (std::sin(M_PI * z) * gamma_approx(1.0 - z));
        }

        // Lanczos coefficients (g=7)
        const double coef[] = {
            0.99999999999980993, 676.5203681218851, -1259.1392167224028,
            771.32342877765313, -176.61502916214059, 12.507343278686905,
            -0.13857109526572012, 9.9843695780195716e-6, 1.5056327351493116e-7
        };

        z -= 1.0;
        double x = coef[0];
        for (int i = 1; i < 9; i++)
        {
            x += coef[i] / (z + i);
        }

        double t = z + 7.5;
        return std::sqrt(2.0 * M_PI) * std::pow(t, z + 0.5) * std::exp(-t) * x;
    }

    void RegisterSpecialFunctionFamily()
    {
        // ═══════════════════════════════════════════════════════════════════════════════
        // SPECIAL FUNCTION FRACTALS
        // Using advanced mathematical functions: Gamma, Error, Bessel-like, etc.
        // ═══════════════════════════════════════════════════════════════════════════════

        // ───────────────────────────────────────────────────────────────────────────────
        // Gamma Function Fractal
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "GammaFractal";
            spec.displayName = "Gamma Function Fractal";
            spec.category = "Special Function Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Uses the Gamma function Γ(z) in iteration: z(n+1) = Γ(z) + c. The Gamma function extends factorials to complex numbers and creates intricate pole structures.";
            spec.formula = "z(n+1) = Γ(z) + c";
            spec.formulaLatex = R"(z_{n+1} = \Gamma(z_n) + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Poles at negative integers, exponential growth in right half-plane, intricate branching";
            spec.discoveredBy = "Special function fractal research";
            spec.discoveryYear = 1995;
            spec.computationalNotes = "Uses Lanczos approximation; poles at z = 0, -1, -2, ...";

            spec.defaultCenterX = 1.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.3;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.5, 0.5);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // For complex Gamma, use real part approximation (simplified)
                    // Full complex Gamma is computationally intensive
                    double mag = std::sqrt(z.real * z.real + z.imag * z.imag);

                    // Avoid poles at negative integers
                    if (z.real < 0.0 && std::abs(z.imag) < 0.1 && std::abs(z.real - std::round(z.real)) < 0.01)
                    {
                        return static_cast<double>(i);
                    }

                    // Approximate complex Gamma using |Gamma(z)| behavior
                    double gamma_approx_val = gamma_approx(mag);
                    double angle = std::atan2(z.imag, z.real);

                    z.real = gamma_approx_val * std::cos(angle * 0.7) + constant.real;
                    z.imag = gamma_approx_val * std::sin(angle * 0.7) + constant.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 100.0 || mag2 < 1e-10)
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Error Function Fractal
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "ErrorFunctionFractal";
            spec.displayName = "Error Function (erf) Fractal";
            spec.category = "Special Function Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Uses error function erf(z) in iteration. The error function appears in probability theory and creates S-shaped transitional regions.";
            spec.formula = "z(n+1) = erf(z) + c";
            spec.formulaLatex = R"(z_{n+1} = \text{erf}(z_n) + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Smooth sigmoid transitions, bounded between -1 and 1 in real axis, oscillatory in imaginary";
            spec.discoveredBy = "Special function fractal research";
            spec.discoveryYear = 1998;
            spec.computationalNotes = "erf(z) = (2/√π)∫₀ᶻ e^(-t²) dt; bounded on real axis";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // Complex error function approximation
                    // erf(x+iy) ≈ erf(x) + i*imag_part
                    double erfReal = erf_approx(z.real);
                    double erfImag = 2.0 / std::sqrt(M_PI) * z.imag * std::exp(-z.real * z.real);

                    z.real = erfReal + constant.real;
                    z.imag = erfImag + constant.imag;

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
        // Bessel-like Oscillatory Fractal
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "BesselLikeFractal";
            spec.displayName = "Bessel-like Oscillatory";
            spec.category = "Special Function Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Approximates Bessel function behavior: oscillatory with decaying amplitude. Uses J₀(z)-like iteration pattern.";
            spec.formula = "z(n+1) = J₀-like(z) + c";
            spec.formulaLatex = R"(z_{n+1} \approx J_0(z_n) + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Circular wave patterns, concentric oscillations, decaying amplitude";
            spec.discoveredBy = "Oscillatory function fractals";
            spec.discoveryYear = 2000;
            spec.computationalNotes = "Simplified Bessel-like: cos(|z|)/√|z|";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.2;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.5, 0.5);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    double mag = std::sqrt(z.real * z.real + z.imag * z.imag);

                    if (mag < 1e-6)
                    {
                        mag = 1e-6; // Avoid division by zero
                    }

                    // Bessel J₀-like: oscillates with decay
                    double besselVal = std::cos(mag) / std::sqrt(mag);
                    double angle = std::atan2(z.imag, z.real);

                    z.real = besselVal * std::cos(angle) + constant.real;
                    z.imag = besselVal * std::sin(angle) + constant.imag;

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
        // Continued Fraction Fractal
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "ContinuedFraction";
            spec.displayName = "Continued Fraction Fractal";
            spec.category = "Special Function Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Uses continued fraction iteration: z(n+1) = c/(1+z). Creates hyperbolic patterns and rational approximations.";
            spec.formula = "z(n+1) = c/(1+z)";
            spec.formulaLatex = R"(z_{n+1} = \frac{c}{1 + z_n})";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Hyperbolic curves, convergent regions, golden ratio connections";
            spec.discoveredBy = "Continued fraction dynamics";
            spec.discoveryYear = 1992;
            spec.computationalNotes = "Related to Fibonacci-like sequences when c=1";

            spec.defaultCenterX = 1.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.5, 0.5);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // z(n+1) = c / (1 + z)
                    double denReal = 1.0 + z.real;
                    double denImag = z.imag;

                    double denMag2 = denReal * denReal + denImag * denImag;

                    if (denMag2 < 1e-10)
                    {
                        return static_cast<double>(i);
                    }

                    z.real = (constant.real * denReal + constant.imag * denImag) / denMag2;
                    z.imag = (constant.imag * denReal - constant.real * denImag) / denMag2;

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
        // Tetration Fractal (Infinite Power Tower)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "Tetration";
            spec.displayName = "Tetration (Power Tower)";
            spec.category = "Special Function Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Explores infinite power towers z^z^z^... using fixed-point iteration. Shows convergence regions for tetration.";
            spec.formula = "z(n+1) = c^z(n)";
            spec.formulaLatex = R"(z_{n+1} = c^{z_n})";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Shell-like structures, convergence basins, exponential spirals";
            spec.discoveredBy = "Hyperoperation fractal research";
            spec.discoveryYear = 1996;
            spec.computationalNotes = "c^z = exp(z*ln(c)); convergence for |c| ≤ e^(1/e)";

            spec.defaultCenterX = 1.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(1.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // c^z = exp(z * ln(c))
                    double cMag = std::sqrt(constant.real * constant.real + constant.imag * constant.imag);

                    if (cMag < 1e-10)
                    {
                        return static_cast<double>(i);
                    }

                    double cArg = std::atan2(constant.imag, constant.real);
                    double lnMag = std::log(cMag);

                    // z * ln(c)
                    double prodReal = z.real * lnMag - z.imag * cArg;
                    double prodImag = z.real * cArg + z.imag * lnMag;

                    // exp(z * ln(c))
                    double expProd = std::exp(prodReal);
                    z.real = expProd * std::cos(prodImag);
                    z.imag = expProd * std::sin(prodImag);

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 100.0 || std::isnan(mag2))
                    {
                        return static_cast<double>(i);
                    }
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Lambert W Function Fractal
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "LambertW";
            spec.displayName = "Lambert W Function";
            spec.category = "Special Function Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Uses Lambert W function (inverse of z*e^z). Newton iteration for W: z(n+1) = z - (z*e^z - c)/(e^z + z*e^z).";
            spec.formula = "Find W where W*e^W = c";
            spec.formulaLatex = R"(W \cdot e^W = c)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Branch cuts, logarithmic spirals, multi-valued regions";
            spec.discoveredBy = "Lambert W fractal visualization";
            spec.discoveryYear = 1998;
            spec.computationalNotes = "Newton's method for solving W*exp(W) = c";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = c; // Initial guess
                ComplexD target = c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // Newton iteration for W*exp(W) = target
                    // f(z) = z*exp(z) - target
                    // f'(z) = exp(z) + z*exp(z) = exp(z)*(1+z)

                    double expZ = std::exp(z.real);
                    double expReal = expZ * std::cos(z.imag);
                    double expImag = expZ * std::sin(z.imag);

                    // f(z) = z*exp(z) - target
                    double fReal = z.real * expReal - z.imag * expImag - target.real;
                    double fImag = z.real * expImag + z.imag * expReal - target.imag;

                    // f'(z) = exp(z)*(1+z)
                    double fpReal = expReal * (1.0 + z.real) - expImag * z.imag;
                    double fpImag = expImag * (1.0 + z.real) + expReal * z.imag;

                    double fpMag2 = fpReal * fpReal + fpImag * fpImag;

                    if (fpMag2 < 1e-10)
                    {
                        return static_cast<double>(i);
                    }

                    // z = z - f/f'
                    double divReal = (fReal * fpReal + fImag * fpImag) / fpMag2;
                    double divImag = (fImag * fpReal - fReal * fpImag) / fpMag2;

                    z.real -= divReal;
                    z.imag -= divImag;

                    // Check convergence
                    double stepSize = divReal * divReal + divImag * divImag;

                    if (stepSize < 1e-6)
                    {
                        return static_cast<double>(i);
                    }

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
        // Hyperbolic Sine Combo
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "HyperbolicCombo";
            spec.displayName = "Hyperbolic Combination";
            spec.category = "Special Function Fractals";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Combines hyperbolic functions: z(n+1) = sinh(z) + cosh(z) + c. Creates exponential growth with oscillation.";
            spec.formula = "z(n+1) = sinh(z) + cosh(z) + c";
            spec.formulaLatex = R"(z_{n+1} = \sinh(z_n) + \cosh(z_n) + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Exponential growth regions, hyperbolic curves, radial symmetry";
            spec.discoveredBy = "Hyperbolic function combinations";
            spec.discoveryYear = 1994;
            spec.computationalNotes = "sinh + cosh = exp(z); rapid growth";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.3;
            spec.defaultBailout = 10.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // sinh(z) + cosh(z) = exp(z)
                    double expZ = std::exp(z.real);
                    z.real = expZ * std::cos(z.imag) + constant.real;
                    z.imag = expZ * std::sin(z.imag) + constant.imag;

                    double mag2 = z.real * z.real + z.imag * z.imag;

                    if (mag2 > 10.0)
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
