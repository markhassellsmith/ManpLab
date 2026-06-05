#include "FractalRegistry.h"
#include "InitialConditionsService.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

    void RegisterChemicalEngineeringFamily()
    {
        // ───────────────────────────────────────────────────────────────────────────────
        // Cahn-Hilliard Map (Phase Separation / Spinodal Decomposition)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
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

            auto ic = InitialConditionsService::Get("CahnHilliard");
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
            FractalSpec spec;
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

            auto ic = InitialConditionsService::Get("GrayScott");
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
    }
}
