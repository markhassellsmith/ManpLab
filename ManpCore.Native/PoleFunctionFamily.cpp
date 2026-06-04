#include "FractalRegistry.h"
#include "InitialConditionsService.h"
#include "MandelbrotCalculator.h"
#include <cmath>

#ifndef M_PI
#define M_PI 3.14159265358979323846
#endif

namespace Native {
    //=============================================================================
    // Pole-Based Special Functions
    // Functions with poles (singularities) that create unbounded behavior
    // suitable for escape-time fractal visualization
    //=============================================================================

    // Helper: Lanczos approximation for Gamma function (reused)
    inline double gamma_approx_pole(double z)
    {
        if (z < 0.5)
        {
            return M_PI / (std::sin(M_PI * z) * gamma_approx_pole(1.0 - z));
        }

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

    void RegisterPoleFunctionFamily()
    {
        FractalSpec spec;
        InitialConditions ic;

        //=========================================================================
        // Digamma Function ψ(z) = Γ'(z)/Γ(z)
        //=========================================================================
        spec.name = "DigammaFractal";
        spec.displayName = "Digamma Function ψ(z)";
        spec.category = "Special Function Fractals";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Uses digamma function ψ(z) = Γ'(z)/Γ(z) in iteration: z = ψ(z) + c. The logarithmic derivative of the Gamma function with poles at negative integers. Creates intricate branching patterns around singularities.";
        spec.formula = "z = ψ(z) + c";
        spec.formulaLatex = R"(z_{n+1} = \psi(z_n) + c)";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0.5, 0.5);
            const double bailout = 100.0;

            for (int i = 0; i < maxIter; ++i)
            {
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

                // Check for poles at negative integers
                if (z.real < 0.0 && std::abs(z.imag) < 0.1 && 
                    std::abs(z.real - std::round(z.real)) < 0.01)
                {
                    return static_cast<double>(i);
                }

                // Digamma approximation: ψ(z) ≈ ln(z) - 1/(2z) - 1/(12z²) + ...
                // For |z| large enough, use asymptotic expansion
                double mag = std::sqrt(magSq);

                if (mag < 0.1)
                {
                    // Near poles, approximate with -1/z
                    if (magSq < 1e-10)
                        return static_cast<double>(i);

                    z.real = -z.real / magSq + c.real;
                    z.imag = z.imag / magSq + c.imag;
                }
                else
                {
                    // Asymptotic: ψ(z) ≈ ln(z) - 1/(2z)
                    double ln_mag = std::log(mag);
                    double arg = std::atan2(z.imag, z.real);

                    // ln(z) = ln|z| + i·arg(z)
                    double ln_real = ln_mag;
                    double ln_imag = arg;

                    // -1/(2z) correction
                    double inv_2z_real = -z.real / (2.0 * magSq);
                    double inv_2z_imag = z.imag / (2.0 * magSq);

                    z.real = ln_real + inv_2z_real + c.real;
                    z.imag = ln_imag + inv_2z_imag + c.imag;
                }

                double newMagSq = z.real * z.real + z.imag * z.imag;
                if (newMagSq < 1e-10)
                    return static_cast<double>(i);
            }

            return static_cast<double>(maxIter);
        };

        spec.supportsJulia = false;
        ic = InitialConditionsService::Get("DigammaFractal");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 100.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);

        //=========================================================================
        // Trigamma Function ψ'(z) = d²/dz² ln Γ(z)
        //=========================================================================
        spec.name = "TrigammaFractal";
        spec.displayName = "Trigamma Function ψ'(z)";
        spec.category = "Special Function Fractals";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Uses trigamma function ψ'(z) (second derivative of ln Γ) in iteration: z = ψ'(z) + c. Has poles at negative integers with rich textural variation around singularities.";
        spec.formula = "z = ψ'(z) + c";
        spec.formulaLatex = R"(z_{n+1} = \psi'(z_n) + c)";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0.5, 0.5);
            const double bailout = 100.0;

            for (int i = 0; i < maxIter; ++i)
            {
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

                // Poles at negative integers
                if (z.real < 0.0 && std::abs(z.imag) < 0.1 && 
                    std::abs(z.real - std::round(z.real)) < 0.01)
                {
                    return static_cast<double>(i);
                }

                if (magSq < 1e-10)
                    return static_cast<double>(i);

                // Trigamma approximation: ψ'(z) ≈ 1/z + 1/(2z²) + 1/(6z³) - ...
                // For simplicity: ψ'(z) ≈ 1/z + 1/(2z²)

                double mag = std::sqrt(magSq);

                // 1/z
                double inv_z_real = z.real / magSq;
                double inv_z_imag = -z.imag / magSq;

                // 1/(2z²)
                double z2_real = z.real * z.real - z.imag * z.imag;
                double z2_imag = 2.0 * z.real * z.imag;
                double z2_mag = z2_real * z2_real + z2_imag * z2_imag;

                if (z2_mag < 1e-10)
                    return static_cast<double>(i);

                double inv_2z2_real = z2_real / (2.0 * z2_mag);
                double inv_2z2_imag = -z2_imag / (2.0 * z2_mag);

                z.real = inv_z_real + inv_2z2_real + c.real;
                z.imag = inv_z_imag + inv_2z2_imag + c.imag;
            }

            return static_cast<double>(maxIter);
        };

        spec.supportsJulia = false;
        ic = InitialConditionsService::Get("TrigammaFractal");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 100.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);

        //=========================================================================
        // Fermi-Dirac Distribution Fractal
        //=========================================================================
        spec.name = "FermiDiracFractal";
        spec.displayName = "Fermi-Dirac Distribution";
        spec.category = "Special Function Fractals";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Uses Fermi-Dirac distribution f(E) = 1/(exp(E) + 1) from quantum statistics describing fermion occupation probability. Iteration: z = 1/(exp(z) + 1) + c. Has rich pole structure where exp(z) = -1.";
        spec.formula = "z = 1/(exp(z) + 1) + c";
        spec.formulaLatex = R"(z_{n+1} = \frac{1}{e^{z_n} + 1} + c)";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0.5, 0.5);
            const double bailout = 100.0;

            for (int i = 0; i < maxIter; ++i)
            {
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

                // Compute exp(z) = exp(x) * (cos(y) + i*sin(y))
                double exp_real = std::exp(z.real);
                double exp_z_real = exp_real * std::cos(z.imag);
                double exp_z_imag = exp_real * std::sin(z.imag);

                // exp(z) + 1
                double denom_real = exp_z_real + 1.0;
                double denom_imag = exp_z_imag;

                // Check for near-zero denominator (pole location)
                double denom_magSq = denom_real * denom_real + denom_imag * denom_imag;
                if (denom_magSq < 1e-10)
                    return static_cast<double>(i);

                // 1 / (exp(z) + 1) using complex division
                z.real = denom_real / denom_magSq + c.real;
                z.imag = -denom_imag / denom_magSq + c.imag;

                if (std::isnan(z.real) || std::isnan(z.imag))
                    return static_cast<double>(i);
            }

            return static_cast<double>(maxIter);
        };

        spec.supportsJulia = false;
        ic = InitialConditionsService::Get("FermiDiracFractal");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 100.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);

        //=========================================================================
        // Bose-Einstein Distribution Fractal
        //=========================================================================
        spec.name = "BoseEinsteinFractal";
        spec.displayName = "Bose-Einstein Distribution";
        spec.category = "Special Function Fractals";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Uses Bose-Einstein distribution f(E) = 1/(exp(E) - 1) from quantum statistics describing boson occupation probability. Iteration: z = 1/(exp(z) - 1) + c. Bosons (integer spin particles like photons) can share quantum states, creating different pole structure than Fermi-Dirac at z = 2πni.";
        spec.formula = "z = 1/(exp(z) - 1) + c";
        spec.formulaLatex = R"(z_{n+1} = \frac{1}{e^{z_n} - 1} + c)";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0.5, 0.5);
            const double bailout = 100.0;

            for (int i = 0; i < maxIter; ++i)
            {
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

                // Compute exp(z) = exp(x) * (cos(y) + i*sin(y))
                double exp_real = std::exp(z.real);
                double exp_z_real = exp_real * std::cos(z.imag);
                double exp_z_imag = exp_real * std::sin(z.imag);

                // exp(z) - 1
                double denom_real = exp_z_real - 1.0;
                double denom_imag = exp_z_imag;

                // Check for near-zero denominator (pole locations where exp(z) = 1)
                // This occurs at z = 0, 2πi, 4πi, 6πi, ... (along imaginary axis)
                double denom_magSq = denom_real * denom_real + denom_imag * denom_imag;
                if (denom_magSq < 1e-10)
                    return static_cast<double>(i);

                // 1 / (exp(z) - 1) using complex division
                z.real = denom_real / denom_magSq + c.real;
                z.imag = -denom_imag / denom_magSq + c.imag;

                if (std::isnan(z.real) || std::isnan(z.imag))
                    return static_cast<double>(i);
            }

            return static_cast<double>(maxIter);
        };

        spec.supportsJulia = false;
        ic = InitialConditionsService::Get("BoseEinsteinFractal");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 100.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);

        //=========================================================================
        // Planck Distribution Fractal
        //=========================================================================
        spec.name = "PlanckFractal";
        spec.displayName = "Planck Distribution";
        spec.category = "Special Function Fractals";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Uses Planck blackbody radiation law B(ν,T) ∝ ν³/(exp(ν) - 1). Iteration: z = z³/(exp(z) - 1) + c. Combines polynomial growth with exponential singularities for rich fractal structure.";
        spec.formula = "z = z³/(exp(z) - 1) + c";
        spec.formulaLatex = R"(z_{n+1} = \frac{z_n^3}{e^{z_n} - 1} + c)";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0.5, 0.5);
            const double bailout = 100.0;

            for (int i = 0; i < maxIter; ++i)
            {
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

                // Check for very small z (avoid division issues)
                if (magSq < 1e-10)
                    return static_cast<double>(i);

                // Compute z³
                double z_cubed_real = z.real * (z.real * z.real - 3.0 * z.imag * z.imag);
                double z_cubed_imag = z.imag * (3.0 * z.real * z.real - z.imag * z.imag);

                // Compute exp(z) = exp(x) * (cos(y) + i*sin(y))
                double exp_real = std::exp(z.real);
                double exp_z_real = exp_real * std::cos(z.imag);
                double exp_z_imag = exp_real * std::sin(z.imag);

                // exp(z) - 1
                double denom_real = exp_z_real - 1.0;
                double denom_imag = exp_z_imag;

                // Check for near-zero denominator (pole where exp(z) = 1)
                double denom_magSq = denom_real * denom_real + denom_imag * denom_imag;
                if (denom_magSq < 1e-10)
                    return static_cast<double>(i);

                // z³ / (exp(z) - 1) using complex division
                // (a + bi) / (c + di) = ((ac + bd) + i(bc - ad)) / (c² + d²)
                double result_real = (z_cubed_real * denom_real + z_cubed_imag * denom_imag) / denom_magSq;
                double result_imag = (z_cubed_imag * denom_real - z_cubed_real * denom_imag) / denom_magSq;

                z.real = result_real + c.real;
                z.imag = result_imag + c.imag;

                if (std::isnan(z.real) || std::isnan(z.imag))
                    return static_cast<double>(i);
            }

            return static_cast<double>(maxIter);
        };

        spec.supportsJulia = false;
        ic = InitialConditionsService::Get("PlanckFractal");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 100.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);

        //=========================================================================
        // RLC Circuit Transfer Function (Electrical Engineering)
        //=========================================================================
        spec.name = "RLCCircuitFractal";
        spec.displayName = "RLC Circuit Resonance";
        spec.category = "Special Function Fractals";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Uses RLC circuit transfer function H(s) = 1/(s² + 2ζωₙs + ωₙ²) from electrical engineering. Models resonant circuits with damping. Iteration: z = 1/(z² + 0.4z + 1) + c. The complex poles represent resonance frequency and damping ratio.";
        spec.formula = "z = 1/(z² + 0.4z + 1) + c";
        spec.formulaLatex = R"(z_{n+1} = \frac{1}{z_n^2 + 0.4z_n + 1} + c)";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0.5, 0.5);
            const double bailout = 100.0;
            const double zeta = 0.2;  // Damping ratio (underdamped for ζ < 1)
            const double omega_n = 1.0;  // Natural frequency

            for (int i = 0; i < maxIter; ++i)
            {
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

                // Compute z² + 2ζωₙz + ωₙ²
                double z_sq_real = z.real * z.real - z.imag * z.imag;
                double z_sq_imag = 2.0 * z.real * z.imag;

                double damping_coeff = 2.0 * zeta * omega_n;
                double linear_real = damping_coeff * z.real;
                double linear_imag = damping_coeff * z.imag;

                double denom_real = z_sq_real + linear_real + omega_n * omega_n;
                double denom_imag = z_sq_imag + linear_imag;

                // Check for near-zero denominator (resonance poles)
                double denom_magSq = denom_real * denom_real + denom_imag * denom_imag;
                if (denom_magSq < 1e-10)
                    return static_cast<double>(i);

                // 1 / denominator using complex division
                z.real = denom_real / denom_magSq + c.real;
                z.imag = -denom_imag / denom_magSq + c.imag;

                if (std::isnan(z.real) || std::isnan(z.imag))
                    return static_cast<double>(i);
            }

            return static_cast<double>(maxIter);
        };

        spec.supportsJulia = false;
        ic = InitialConditionsService::Get("RLCCircuitFractal");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 100.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);

        //=========================================================================
        // Damped Harmonic Oscillator (Mechanical Engineering)
        //=========================================================================
        spec.name = "DampedOscillatorFractal";
        spec.displayName = "Damped Harmonic Oscillator";
        spec.category = "Special Function Fractals";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Models spring-mass-damper system from mechanical engineering: x(t) = e^(-ζωₙt) × cos(ωₐt). Iteration: z = exp(-0.3z) × cos(z) + c. Shows exponential decay combined with oscillation.";
        spec.formula = "z = exp(-0.3z) × cos(z) + c";
        spec.formulaLatex = R"(z_{n+1} = e^{-0.3z_n} \cos(z_n) + c)";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0.5, 0.5);
            const double bailout = 100.0;
            const double damping = 0.3;

            for (int i = 0; i < maxIter; ++i)
            {
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

                // Compute exp(-damping × z)
                double exp_arg_real = -damping * z.real;
                double exp_arg_imag = -damping * z.imag;
                double exp_mag = std::exp(exp_arg_real);
                double exp_real = exp_mag * std::cos(exp_arg_imag);
                double exp_imag = exp_mag * std::sin(exp_arg_imag);

                // Compute cos(z) = cos(x)cosh(y) - i sin(x)sinh(y)
                double cos_real = std::cos(z.real) * std::cosh(z.imag);
                double cos_imag = -std::sin(z.real) * std::sinh(z.imag);

                // Multiply: exp(-damping×z) × cos(z)
                z.real = exp_real * cos_real - exp_imag * cos_imag + c.real;
                z.imag = exp_real * cos_imag + exp_imag * cos_real + c.imag;

                if (std::isnan(z.real) || std::isnan(z.imag))
                    return static_cast<double>(i);
            }

            return static_cast<double>(maxIter);
        };

        spec.supportsJulia = false;
        ic = InitialConditionsService::Get("DampedOscillatorFractal");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 100.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);

        //=========================================================================
        // Root Locus (Control Systems Engineering)
        //=========================================================================
        spec.name = "RootLocusFractal";
        spec.displayName = "Root Locus (Control Systems)";
        spec.category = "Special Function Fractals";
        spec.type = FractalCategory::EscapeTime2D;
        spec.description = "Based on control systems root locus: 1 + KG(s)H(s) = 0. Shows how system poles move in complex plane with gain K. Iteration: z = z - (z² + 1)/(2z) + c (Newton-like for z² + 1 = 0). Models feedback control dynamics.";
        spec.formula = "z = z - (z² + 1)/(2z) + c";
        spec.formulaLatex = R"(z_{n+1} = z_n - \frac{z_n^2 + 1}{2z_n} + c)";

        spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double {
            ComplexD z(0.5, 0.5);
            const double bailout = 100.0;

            for (int i = 0; i < maxIter; ++i)
            {
                double magSq = z.real * z.real + z.imag * z.imag;
                if (magSq > bailout)
                    return i + 1.0 - std::log2(std::log(magSq) / std::log(bailout));

                // Check for near-zero z (avoid division by zero)
                if (magSq < 1e-10)
                    return static_cast<double>(i);

                // Compute z² + 1
                double z_sq_real = z.real * z.real - z.imag * z.imag + 1.0;
                double z_sq_imag = 2.0 * z.real * z.imag;

                // Compute (z² + 1) / (2z)
                double denom_real = 2.0 * z.real;
                double denom_imag = 2.0 * z.imag;
                double denom_magSq = denom_real * denom_real + denom_imag * denom_imag;

                double quotient_real = (z_sq_real * denom_real + z_sq_imag * denom_imag) / denom_magSq;
                double quotient_imag = (z_sq_imag * denom_real - z_sq_real * denom_imag) / denom_magSq;

                // z = z - quotient + c
                z.real = z.real - quotient_real + c.real;
                z.imag = z.imag - quotient_imag + c.imag;

                if (std::isnan(z.real) || std::isnan(z.imag))
                    return static_cast<double>(i);
            }

            return static_cast<double>(maxIter);
        };

        spec.supportsJulia = false;
        ic = InitialConditionsService::Get("RootLocusFractal");
        spec.defaultCenterX = ic.centerX;
        spec.defaultCenterY = ic.centerY;
        spec.defaultZoom = ic.zoom;
        spec.defaultBailout = 100.0;
        spec.hasSymmetry = false;

        FractalRegistry::Register(spec);
    }
} // namespace Native
