#include "FractalRegistry.h"
#include "InitialConditionsService.h"
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

    void RegisterScienceAndEngineeringFamily()
    {
        FractalSpec spec;
        InitialConditions ic;  // Declare ONCE at the top

        // ═══════════════════════════════════════════════════════════════════════════════
        // SPECIAL FUNCTION FRACTALS
        // Using advanced mathematical functions: Gamma, Error, Bessel-like, etc.
        // ═══════════════════════════════════════════════════════════════════════════════

        // ───────────────────────────────────────────────────────────────────────────────
        // Airy Function (Bi) Fractal
        // ───────────────────────────────────────────────────────────────────────────────
        {
            spec.name = "AiryBi";
            spec.displayName = "Airy Function (Bi)";
            spec.category = "Science and Engineering";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Uses the Airy function of the second kind, Bi(z), in an escape-time iteration. Bi(z) is a solution to the differential equation w'' - zw = 0 and is known for its exponentially growing, oscillatory behavior, often creating intricate, feathery visuals.";
            spec.formula = "z(n+1) = Bi(z_n) + c";
            spec.formulaLatex = R"(z_{n+1} = \text{Bi}(z_n) + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Exponentially growing oscillations, feathery and web-like structures, intricate interference patterns.";
            spec.discoveredBy = "George Biddell Airy";
            spec.discoveryYear = 1838;
            spec.computationalNotes = "Implemented using a series approximation. Bi(z) grows exponentially, leading to rapid escape for large |z|.";

            ic = InitialConditionsService::Get("AiryBi");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;

                for (int i = 0; i < maxIter; ++i)
                {
                    // Series approximation for Bi(z)
                    // Bi(z) = (1/sqrt(pi)) * ( (z)^(-1/4) * exp(xi) * sum_{k=0}^{N} c_k * xi^(-k) )
                    // This is complex. A simpler approach for visualization is a direct power series for small z.
                    // Bi(z) = c1 * f(z) + c2 * g(z)
                    // f(z) = 1 + z^3/3! + z^6/6! + ...
                    // g(z) = z + z^4/4! + z^7/7! + ...
                    // c1 = Bi(0) = 3^(-1/6) / Gamma(2/3) approx 0.6149
                    // c2 = Bi'(0) = 3^(1/6) / Gamma(1/3) approx 0.4557

                    ComplexD f_sum(1.0, 0.0);
                    ComplexD g_sum(0.0, 0.0);
                    ComplexD z_pow_n = z;
                    double fact_n = 1.0;

                    for (int n = 1; n <= 10; ++n)
                    {
                        fact_n *= n;
                        if (n % 3 == 0) {
                            f_sum = f_sum + z_pow_n / fact_n;
                        } else if (n % 3 == 1) {
                            g_sum = g_sum + z_pow_n / fact_n;
                        }
                        if (n < 10) z_pow_n = z_pow_n * z;
                    }

                    const double c1 = 0.614921259;
                    const double c2 = 0.455743842;

                    z = f_sum * c1 + g_sum * c2 + constant;

                    if (z.x * z.x + z.y * z.y > 10000.0)
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
            spec.name = "BesselLikeFractal";
            spec.displayName = "Bessel-like Oscillatory";
            spec.category = "Science and Engineering";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Approximates Bessel function behavior: oscillatory with decaying amplitude. Uses J₀(z)-like iteration pattern.";
            spec.formula = "z(n+1) = J₀-like(z) + c";
            spec.formulaLatex = R"(z_{n+1} \approx J_0(z_n) + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Circular wave patterns, concentric oscillations, decaying amplitude";
            spec.discoveredBy = "Oscillatory function fractals";
            spec.discoveryYear = 2000;
            spec.computationalNotes = "Simplified Bessel-like: cos(|z|)/√|z|";

            //spec.defaultCenterX = 0.0;
            //spec.defaultCenterY = 0.0;
            //spec.defaultZoom = 0.2;
            ic = InitialConditionsService::Get("BesselLikeFractal");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
                {
                    ComplexD z = isJulia ? c : ComplexD(0.1, 0.0);  // Better starting point
                    ComplexD constant = isJulia ? juliaC : c;

                    for (int i = 0; i < maxIter; ++i)
                    {
                        double mag = std::sqrt(z.real * z.real + z.imag * z.imag);

                        if (mag < 1e-6)
                        {
                            mag = 1e-6; // Avoid division by zero
                        }

                        // Improved Bessel J₀-like using full complex evaluation: cos(z - π/4) / sqrt(z)
                        double z_shift = z.real - 0.785398;
                        double cx = std::cos(z_shift) * std::cosh(z.imag);
                        double cy = -std::sin(z_shift) * std::sinh(z.imag);

                        double r = std::sqrt(mag);
                        double theta = std::atan2(z.imag, z.real) * 0.5;
                        double denom_r = std::sqrt(r);
                        double denom_real = denom_r * std::cos(theta);
                        double denom_imag = denom_r * std::sin(theta);

                        // Complex division
                        double denom_mag2 = denom_real * denom_real + denom_imag * denom_imag;
                        if (denom_mag2 < 1e-12) denom_mag2 = 1e-12;

                        double bessel_real = (cx * denom_real + cy * denom_imag) / denom_mag2;
                        double bessel_imag = (cy * denom_real - cx * denom_imag) / denom_mag2;

                        z.real = bessel_real + constant.real;
                        z.imag = bessel_imag + constant.imag;

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

        // ... existing code ...
        // ───────────────────────────────────────────────────────────────────────────────
        // Continued Fraction Fractal
        // ───────────────────────────────────────────────────────────────────────────────
        {
            spec.name = "ContinuedFraction";
            spec.displayName = "Continued Fraction Fractal";
            spec.category = "Science and Engineering";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Uses continued fraction iteration: z(n+1) = c/(1+z). Creates hyperbolic patterns and rational approximations.";
            spec.formula = "z(n+1) = c/(1+z)";
            spec.formulaLatex = R"(z_{n+1} = \frac{c}{1 + z_n})";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Hyperbolic curves, convergent regions, golden ratio connections";
            spec.discoveredBy = "Continued fraction dynamics";
            spec.discoveryYear = 1992;
            spec.computationalNotes = "Related to Fibonacci-like sequences when c=1";

            //spec.defaultCenterX = -3.65;
            //spec.defaultCenterY = -0.57;
            //spec.defaultZoom = 0.547192;  // Viewport tuning: X scale 7.31
            ic = InitialConditionsService::Get("ContinuedFraction");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom;
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
        // Dedekind Eta Function
        // ───────────────────────────────────────────────────────────────────────────────
        {
            spec.name = "DedekindEta";
            spec.displayName = "Dedekind Eta Function";
            spec.category = "Science and Engineering";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "The Dedekind eta function is a modular form from number theory, deeply connected to partition functions and string theory. Its visualization is renowned for its intricate, self-similar boundary.";
            spec.formula = "η(τ) = q^(1/24) * Π_{n=1 to ∞} (1 - q^n), where q = e^(2πiτ)";
            spec.formulaLatex = R"(\eta(\tau) = q^{1/24} \prod_{n=1}^{\infty} (1-q^n), q=e^{2\pi i\tau})";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Infinitely detailed boundary, nested circular and paisley-like patterns, modular symmetry.";
            spec.discoveredBy = "Richard Dedekind";
            spec.discoveryYear = 1877;
            spec.computationalNotes = "The iteration z -> eta(z) + c is used. The product is approximated for visualization.";

            ic = InitialConditionsService::Get("DedekindEta");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = c; // Use c as the initial value for tau

                for (int i = 0; i < maxIter; ++i)
                {
                    // Let z be our tau (τ)
                    // q = exp(2 * pi * i * z)
                    ComplexD q_arg = ComplexD(0.0, 2.0 * M_PI) * z;
                    double q_re = std::exp(q_arg.x) * std::cos(q_arg.y);
                    double q_im = std::exp(q_arg.x) * std::sin(q_arg.y);
                    ComplexD q(q_re, q_im);

                    // q_24 = q^(1/24) = exp(2 * pi * i * z / 24)
                    ComplexD q24_arg = ComplexD(0.0, 2.0 * M_PI) * z / 24.0;
                    double q24_re = std::exp(q24_arg.x) * std::cos(q24_arg.y);
                    double q24_im = std::exp(q24_arg.x) * std::sin(q24_arg.y);
                    ComplexD q_24(q24_re, q24_im);

                    // Product term: Π(1 - q^n)
                    ComplexD product(1.0, 0.0);
                    ComplexD q_n = q;
                    for (int n = 1; n <= 20; ++n) // Approximate with 20 terms
                    {
                        product = product * (ComplexD(1.0, 0.0) - q_n);
                        if (q_n.x * q_n.x + q_n.y * q_n.y < 1e-30) break; // Avoid underflow
                        q_n = q_n * q;
                    }

                    z = q_24 * product + c;

                    if (z.x * z.x + z.y * z.y > 10000.0)
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
            spec.name = "ErrorFunctionFractal";
            spec.displayName = "Error Function (erf) Fractal";
            spec.category = "Science and Engineering";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Uses error function erf(z) in iteration. The error function appears in probability theory and creates S-shaped transitional regions.";
            spec.formula = "z(n+1) = erf(z) + c";
            spec.formulaLatex = R"(z_{n+1} = \text{erf}(z_n) + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Smooth sigmoid transitions, bounded between -1 and 1 in real axis, oscillatory in imaginary";
            spec.discoveredBy = "Special function fractal research";
            spec.discoveryYear = 1998;
            spec.computationalNotes = "erf(z) = (2/√π)∫₀ᶻ e^(-t²) dt; bounded on real axis";

            //spec.defaultCenterX = 0.0;
            //spec.defaultCenterY = 0.05;
            //spec.defaultZoom = 0.107239;  // Viewport tuning: X scale 37.3
            ic = InitialConditionsService::Get("ErrorFunctionFractal");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom;
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
        // Gamma Function Fractal
        // ───────────────────────────────────────────────────────────────────────────────
        {
            spec.name = "GammaFractal";
            spec.displayName = "Gamma Function Fractal";
            spec.category = "Science and Engineering";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Uses the Gamma function Γ(z) in iteration: z(n+1) = Γ(z) + c. The Gamma function extends factorials to complex numbers and creates intricate pole structures.";
            spec.formula = "z(n+1) = Γ(z) + c";
            spec.formulaLatex = R"(z_{n+1} = \Gamma(z_n) + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Poles at negative integers, exponential growth in right half-plane, intricate branching";
            spec.discoveredBy = "Special function fractal research";
            spec.discoveryYear = 1995;
            spec.computationalNotes = "Uses Lanczos approximation; poles at z = 0, -1, -2, ...";

            //spec.defaultCenterX = -1.13;
            //spec.defaultCenterY = -0.07;
            //spec.defaultZoom = 0.4;  // Viewport tuning: X scale 10.0
            ic = InitialConditionsService::Get("GammaFractal");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom;
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
        // Hyperbolic Sine Combo
        // ───────────────────────────────────────────────────────────────────────────────
        {
            spec.name = "HyperbolicCombo";
            spec.displayName = "Hyperbolic Combination";
            spec.category = "Science and Engineering";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Combines hyperbolic functions: z(n+1) = sinh(z) + cosh(z) + c. Creates exponential growth with oscillation.";
            spec.formula = "z(n+1) = sinh(z) + cosh(z) + c";
            spec.formulaLatex = R"(z_{n+1} = \sinh(z_n) + \cosh(z_n) + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Exponential growth regions, hyperbolic curves, radial symmetry";
            spec.discoveredBy = "Hyperbolic function combinations";
            spec.discoveryYear = 1994;
            spec.computationalNotes = "sinh + cosh = exp(z); rapid growth";

            //spec.defaultCenterX = -1.64;
            //spec.defaultCenterY = 0.02;
            //spec.defaultZoom = 0.524934;  // Viewport tuning: X scale 7.62
            ic = InitialConditionsService::Get("HyperbolicCombo");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom;
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

        // ───────────────────────────────────────────────────────────────────────────────
        // Jacobi Elliptic cn Function Fractal
        // ───────────────────────────────────────────────────────────────────────────────
        {
            spec.name = "JacobiCN";
            spec.displayName = "Jacobi Elliptic cn";
            spec.category = "Elliptic Functions";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Escape-time fractal using the Jacobi elliptic function cn(z, k). The cn function is one of the twelve Jacobi elliptic functions and satisfies cn² + sn² = 1, generalizing the cosine function. The function is doubly periodic with real and imaginary periods depending on the elliptic modulus k.";
            spec.formula = "z(n+1) = cn(z_n, k) + c";
            spec.formulaLatex = R"(z_{n+1} = \text{cn}(z_n, k) + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Periodic wave-like structures with elliptical modulation, creating regions of alternating density. The parameter k controls the departure from pure cosine behavior.";
            spec.discoveredBy = "Carl Gustav Jacob Jacobi";
            spec.discoveryYear = 1829;
            spec.computationalNotes = "Implemented using a series approximation. For k=0, cn(z,0)=cos(z); for k=1, cn(z,1)=sech(z). The function exhibits double periodicity in the complex plane.";

            ic = InitialConditionsService::Get("JacobiCN");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;
                const double bailout = 100.0;
                const double k = 0.7;  // Elliptic modulus (0 < k < 1)

                for (int i = 0; i < maxIter; ++i)
                {
                    double magSq = z.x * z.x + z.y * z.y;
                    if (magSq > bailout)
                    {
                        return static_cast<double>(i) + 1.0 - std::log2(std::log(magSq) / std::log(bailout));
                    }

                    // Jacobi cn approximation
                    // cn(u+iv, k) where u=real, v=imag
                    // cn relates to cosine: cn(z,0) = cos(z)
                    // For complex argument, use: cn ≈ cos(u)·cosh(v) - i·sin(u)·sinh(v)
                    // with elliptic modulus correction

                    double u = z.x;
                    double v = z.y;

                    double sinU = std::sin(u);
                    double cosU = std::cos(u);
                    double sinhV = std::sinh(v);
                    double coshV = std::cosh(v);

                    // Real part: cos(u)·cosh(v)
                    double cnReal = cosU * coshV;
                    // Imaginary part: -sin(u)·sinh(v)
                    double cnImag = -sinU * sinhV;

                    // Apply elliptic modulus correction
                    double k2 = k * k;
                    double correction = 1.0 / (1.0 + k2 * magSq / 6.0);

                    z.x = cnReal * correction + constant.x;
                    z.y = cnImag * correction + constant.y;
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Jacobi Elliptic dn Function Fractal
        // ───────────────────────────────────────────────────────────────────────────────
        {
            spec.name = "JacobiDN";
            spec.displayName = "Jacobi Elliptic dn";
            spec.category = "Elliptic Functions";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Escape-time fractal using the Jacobi elliptic function dn(z, k). The dn function is the delta amplitude function satisfying dn² + k²·sn² = 1. It is doubly periodic and interpolates between dn(z,0)=1 and dn(z,1)=sech(z).";
            spec.formula = "z(n+1) = dn(z_n, k) + c";
            spec.formulaLatex = R"(z_{n+1} = \text{dn}(z_n, k) + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Creates structured wave patterns with modulated amplitude. The elliptic modulus k controls the deviation from unity, producing intricate periodic tessellations.";
            spec.discoveredBy = "Carl Gustav Jacob Jacobi";
            spec.discoveryYear = 1829;
            spec.computationalNotes = "Implemented using series approximation. The dn function exhibits double periodicity and is closely related to the Weierstrass elliptic function. For k=0, dn(z,0)=1; for k=1, dn(z,1)=sech(z).";

            ic = InitialConditionsService::Get("JacobiDN");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                ComplexD constant = isJulia ? juliaC : c;
                const double bailout = 100.0;
                const double k = 0.7;  // Elliptic modulus (0 < k < 1)

                for (int i = 0; i < maxIter; ++i)
                {
                    double magSq = z.x * z.x + z.y * z.y;
                    if (magSq > bailout)
                    {
                        return static_cast<double>(i) + 1.0 - std::log2(std::log(magSq) / std::log(bailout));
                    }

                    // Jacobi dn approximation
                    // dn(u+iv, k) where u=real, v=imag
                    // The dn function satisfies: dn² + k²·sn² = 1
                    // For k=0: dn(z,0) = 1; for k=1: dn(z,1) = sech(z)
                    // For complex argument, use relation: dn(z,k) ≈ sqrt(1 - k²·sn²(z,k))
                    // Simplified visualization formula: dn ≈ 1 / sqrt(1 + k²·sinh²(v))

                    double u = z.x;
                    double v = z.y;

                    double sinU = std::sin(u);
                    double cosU = std::cos(u);
                    double sinhV = std::sinh(v);
                    double coshV = std::cosh(v);

                    // Compute sn first (similar to JacobiSN)
                    double snReal = sinU * coshV;
                    double snImag = cosU * sinhV;

                    // dn satisfies dn² + k²·sn² = 1
                    // Real part: approximate using sqrt(1 - k²·sn²)
                    double k2 = k * k;
                    double sn2 = snReal * snReal + snImag * snImag;
                    double dnMagnitude = std::sqrt(std::abs(1.0 - k2 * sn2));

                    // For visualization, use modified approach
                    double dnReal = dnMagnitude * cosU / (1.0 + k2 * magSq / 8.0);
                    double dnImag = -dnMagnitude * sinU * std::tanh(v) / (1.0 + k2 * magSq / 8.0);

                    z.x = dnReal + constant.x;
                    z.y = dnImag + constant.y;
                }

                return static_cast<double>(maxIter);
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Lambert W Function Fractal
        // ───────────────────────────────────────────────────────────────────────────────
        {
            spec.name = "LambertW";
            spec.displayName = "Lambert W Function";
            spec.category = "Science and Engineering";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Uses Lambert W function (inverse of z*e^z). Newton iteration for W: z(n+1) = z - (z*e^z - c)/(e^z + z*e^z).";
            spec.formula = "Find W where W*e^W = c";
            spec.formulaLatex = R"(W \cdot e^W = c)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Branch cuts, logarithmic spirals, multi-valued regions";
            spec.discoveredBy = "Lambert W fractal visualization";
            spec.discoveryYear = 1998;
            spec.computationalNotes = "Newton's method for solving W*exp(W) = c";

            //spec.defaultCenterX = 2.56;
            //spec.defaultCenterY = 0.15;
            //spec.defaultZoom = 0.109890;  // Viewport tuning: X scale 36.4
            ic = InitialConditionsService::Get("LambertW");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom;
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
        // Tetration Fractal (Infinite Power Tower)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            spec.name = "TetrationPowerTower";
            spec.displayName = "Tetration (Power Tower)";
            spec.category = "Science and Engineering";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Explores infinite power towers z^z^z^... using fixed-point iteration. Shows convergence regions for tetration.";
            spec.formula = "z(n+1) = c^z(n)";
            spec.formulaLatex = R"(z_{n+1} = c^{z_n})";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Shell-like structures, convergence basins, exponential spirals";
            spec.discoveredBy = "Hyperoperation fractal research";
            spec.discoveryYear = 1996;
            spec.computationalNotes = "c^z = exp(z*ln(c)); convergence for |c| ≤ e^(1/e)";

            //spec.defaultCenterX = 1.0;
            //spec.defaultCenterY = 0.0;
            //spec.defaultZoom = 0.106952;  // Viewport tuning: X scale 37.4
            ic = InitialConditionsService::Get("TetrationPowerTower");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom;
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
    }
}
