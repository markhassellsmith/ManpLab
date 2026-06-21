#include "FractalRegistry.h"
#include "InitialConditionsService.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

    void RegisterChemicalEngineeringFamily()
    {
        FractalSpec spec;
        InitialConditions ic;  // Declare ONCE at function scope

        // ───────────────────────────────────────────────────────────────────────────────
        // Cahn-Hilliard Map (Phase Separation / Spinodal Decomposition)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            spec.name = "CahnHilliard";
            spec.displayName = "Cahn-Hilliard Map (Phase Separation)";
            spec.category = "Chemical Engineering";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Models the spinodal decomposition of a single-phase mixture into two distinct phases, mapping purely thermodynamic drivers into complex fractals.";
            spec.formula = "z(n+1) = z(n)^3 - z(n) + c";
            spec.formulaLatex = R"(z_{n+1} = z_n^3 - z_n + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Subtracted cubic terms push the Mandelbrot structures apart, resembling micro-emulsions, detached droplets, and interconnected constituent bridges typical in two-liquid phase separation.";
            spec.discoveredBy = "Chemical Thermodynamics (Cahn-Hilliard 1958 equation transposed)";
            spec.computationalNotes = "Classic double-well free energy mapped into complex plane iterations.";

            ic = InitialConditionsService::Get("CahnHilliard");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom != 0 ? ic.zoom : 0.8;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
                {
                    ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                    ComplexD constant = isJulia ? juliaC : c;

                    for (int i = 0; i < maxIter; ++i)
                    {
                        double x2 = z.real * z.real;
                        double y2 = z.imag * z.imag;

                        if (x2 + y2 > 256.0)
                        {
                            return static_cast<double>(i) + 1.0 - std::log2(std::log(x2 + y2) / 2.0);
                        }

                        // z^3 = (x^3 - 3xy^2) + i(3x^2y - y^3)
                        double newReal = z.real * (x2 - 3.0 * y2) - z.real + constant.real;
                        double newImag = z.imag * (3.0 * x2 - y2) - z.imag + constant.imag;

                        z.real = newReal;
                        z.imag = newImag;
                    }

                    return static_cast<double>(maxIter);
                };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Gray-Scott Autocatalysis Map (Reaction-Diffusion)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            spec.name = "GrayScott";
            spec.displayName = "Gray-Scott Autocatalysis (Reaction-Diffusion)";
            spec.category = "Chemical Engineering";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Models non-linear Belousov-Zhabotinsky oscillating chemical kinetics. The map represents interactions where species U depletes and V grows at a rate proportional to U*V^2.";
            spec.formula = "x(n+1) = x(n) - x(n)*y(n)^2 + c_x\n y(n+1) = y(n) + x(n)*y(n)^2 + c_y";
            spec.formulaLatex = R"(\begin{cases} x_{n+1} = x_n - x_n y_n^2 + c_x \\ y_{n+1} = y_n + x_n y_n^2 + c_y \end{cases})";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Math breaks conformal symmetry, outputting chaotic, non-analytic fluid-like dynamics. Cross-coupling creates vortex-like sinks and distinct chemical 'plumes'.";
            spec.discoveredBy = "Gray-Scott (1983) oscillating system interpretation";
            spec.computationalNotes = "Directly mimics the kinetic rate term U + 2V -> 3V mapped as a discrete escape-time system.";

            ic = InitialConditionsService::Get("GrayScott");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom != 0 ? ic.zoom : 0.8;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
                {
                    ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                    ComplexD constant = isJulia ? juliaC : c;

                    for (int i = 0; i < maxIter; ++i)
                    {
                        double x2 = z.real * z.real;
                        double y2 = z.imag * z.imag;
                        double mag2 = x2 + y2;

                        if (mag2 > 256.0)
                        {
                            // Smoothed iteration
                            return static_cast<double>(i) + 1.0 - std::log2(std::log(mag2) / 2.0);
                        }

                        // Gray-Scott discrete kinetic terms: x loses xy^2, y gains xy^2
                        double xy2 = z.real * y2;
                        double newReal = z.real - xy2 + constant.real;
                        double newImag = z.imag + xy2 + constant.imag;

                        z.real = newReal;
                        z.imag = newImag;
                    }

                    return static_cast<double>(maxIter);
                };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Langmuir-Hinshelwood Isotherm (Surface Catalysis)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            spec.name = "LangmuirIsotherm";
            spec.displayName = "Langmuir-Hinshelwood Isotherm";
            spec.category = "Chemical Engineering";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Models catalytic reaction rates where surface coverage saturates competing with desorption. Singular at ±i, resulting in distinct disconnected geometric islands.";
            spec.formula = "z(n+1) = z(n)^2 / (1 + z(n)^2) + c";
            spec.formulaLatex = R"(z_{n+1} = \frac{z_n^2}{1 + z_n^2} + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Rational map behavior creates teardrops and fragmented 'islands', imitating saturation boundaries in active site coverage.";
            spec.discoveredBy = "Langmuir (1918) & Hinshelwood (1926) Reaction Kinetics";
            spec.computationalNotes = "Complex rational division handles ±i singularities dynamically.";

            ic = InitialConditionsService::Get("LangmuirIsotherm");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom != 0 ? ic.zoom : 1.0;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
                {
                    ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                    ComplexD constant = isJulia ? juliaC : c;

                    for (int i = 0; i < maxIter; ++i)
                    {
                        double x2 = z.real * z.real;
                        double y2 = z.imag * z.imag;
                        double mag2 = x2 + y2;

                        if (mag2 > 256.0) return static_cast<double>(i);

                        // Calculate z^2
                        double z2r = x2 - y2;
                        double z2i = 2.0 * z.real * z.imag;

                        // Calculate denominator 1 + z^2
                        double denomr = 1.0 + z2r;
                        double denomi = z2i;
                        double denom_mag2 = denomr * denomr + denomi * denomi;

                        // Prevent catastrophic div by zero if close to pole
                        if (denom_mag2 < 1e-12) denom_mag2 = 1e-12;

                        // (z^2) / (1+z^2) = (z2r + i z2i) / (denomr + i denomi)
                        double newReal = (z2r * denomr + z2i * denomi) / denom_mag2;
                        double newImag = (z2i * denomr - z2r * denomi) / denom_mag2;

                        z.real = newReal + constant.real;
                        z.imag = newImag + constant.imag;
                    }

                    return static_cast<double>(maxIter);
                };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Arrhenius Kinetics Map (Thermal Activation)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            spec.name = "ArrheniusKinetics";
            spec.displayName = "Arrhenius Kinetics Map (Thermal Activation)";
            spec.category = "Chemical Engineering";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Maps the temperature-dependent reaction exponential progression with an activation energy barrier. Features a dominant essential singularity.";
            spec.formula = "z(n+1) = exp(-1 / z(n)^2) + c";
            spec.formulaLatex = R"(z_{n+1} = \exp\left(\frac{-1}{z_n^2}\right) + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Displays massive essential singularity fragmentation (known as 'Fatou dust' or exponential petal structures) representing runaway kinetics.";
            spec.discoveredBy = "Svante Arrhenius (1889) Equation";
            spec.computationalNotes = "Uses w = -1/z^2, then e^w. Extremely chaotic near the origin due to the essential singularity.";

            ic = InitialConditionsService::Get("ArrheniusKinetics");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom != 0 ? ic.zoom : 1.0;
            spec.defaultBailout = 100.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
                {
                    ComplexD z = isJulia ? c : ComplexD(0.001, 0.0); // Offset to avoid div 0 at origin
                    ComplexD constant = isJulia ? juliaC : c;

                    for (int i = 0; i < maxIter; ++i)
                    {
                        double mag2 = z.real * z.real + z.imag * z.imag;

                        if (mag2 > 100.0) return static_cast<double>(i);
                        if (mag2 < 1e-12) mag2 = 1e-12; // Prevent div by zero at origin

                        // Calculate z^2
                        double z2r = z.real * z.real - z.imag * z.imag;
                        double z2i = 2.0 * z.real * z.imag;

                        // Calculate -1 / z^2 (activation energy barrier mapping)
                        double z2mag2 = z2r * z2r + z2i * z2i;
                        if (z2mag2 < 1e-12) z2mag2 = 1e-12;

                        double wr = -z2r / z2mag2;
                        double wi = z2i / z2mag2; // conjugate divided by mag2

                        // Calculate exp(w)
                        double exp_wr = std::exp(wr);
                        double newReal = exp_wr * std::cos(wi) + constant.real;
                        double newImag = exp_wr * std::sin(wi) + constant.imag;

                        z.real = newReal;
                        z.imag = newImag;
                    }

                    return static_cast<double>(maxIter);
                };

            FractalRegistry::Register(spec);
        }
    }
}
