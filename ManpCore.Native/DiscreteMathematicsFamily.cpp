#include "FractalRegistry.h"
#include "InitialConditionsService.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

    void RegisterDiscreteMathematicsFamily()
    {
        // ───────────────────────────────────────────────────────────────────────────────
        // Combinatorial Mandelbrot: z^4 - z + c (polynomial blend)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "CombinatorialMandelbrot";
            spec.displayName = "Combinatorial Mandelbrot z⁴ - z + c";
            spec.category = "Discrete Mathematics";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "A polynomial fractal combining quartic and linear terms: z(n+1) = z(n)^4 - z(n) + c. This blend of high-degree and low-degree terms creates intricate combinatorial-like structure with multiple escape basins and fine recursive detail, bridging discrete recurrence and dynamical systems.";
            spec.formula = "z(n+1) = z(n)^4 - z(n) + c";
            spec.formulaLatex = R"(z_{n+1} = z_n^4 - z_n + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Deeply layered escape bands with a complex central bulb structure. The interplay of fourth-power and linear decay creates intricate filaments and self-similar islands at multiple scales, evoking combinatorial branching.";
            spec.discoveredBy = "Polynomial iteration (dynamical systems, exploration)";
            spec.computationalNotes = "The quartic term dominates at large |z|, while the linear term creates competing attractors and escape pathways. The balance yields rich detail across the parameter space.";

            auto ic = InitialConditionsService::Get("CombinatorialMandelbrot");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom != 0 ? ic.zoom : 0.6;
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
                        double mag2 = x * x + y * y;

                        if (mag2 > 256.0)
                        {
                            return static_cast<double>(i) + 1.0 - std::log2(std::log(mag2) / 2.0);
                        }

                        // z^2
                        double z2r = x * x - y * y;
                        double z2i = 2.0 * x * y;

                        // z^4 = (z^2)^2
                        double z4r = z2r * z2r - z2i * z2i;
                        double z4i = 2.0 * z2r * z2i;

                        // z^4 - z + c
                        z.real = z4r - x + constant.real;
                        z.imag = z4i - y + constant.imag;
                    }

                    return static_cast<double>(maxIter);
                };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Inverse Combinatorial: z^(-2) + c (reciprocal mapping)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "InverseCombinatorial";
            spec.displayName = "Inverse Combinatorial 1/z² + c";
            spec.category = "Discrete Mathematics";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "A reciprocal fractal: z(n+1) = 1/z(n)^2 + c. Reciprocal dynamics create inversive geometry with singularities and pole structures. Points near zero map to infinity; escape is determined by the competing pull of reciprocal inversion and parameter c.";
            spec.formula = "z(n+1) = 1/z(n)² + c";
            spec.formulaLatex = R"(z_{n+1} = \frac{1}{z_n^2} + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Highly symmetric inversive structure with pole-like singularities creating radial escape patterns. Inversion creates mirror-like self-similarity and elaborate ring structures as points cycle around the origin.";
            spec.discoveredBy = "Inversive geometry and reciprocal iteration";
            spec.computationalNotes = "Division by zero is avoided by checking for small |z| and damping the reciprocal magnitude. Points with |z| < 0.01 are treated as near-singular; their reciprocal is clamped to prevent runaway behavior.";

            auto ic = InitialConditionsService::Get("InverseCombinatorial");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom != 0 ? ic.zoom : 0.5;
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
                        double mag2 = x * x + y * y;

                        if (mag2 > 256.0)
                        {
                            return static_cast<double>(i) + 1.0 - std::log2(std::log(mag2) / 2.0);
                        }

                        if (mag2 < 0.0001)
                        {
                            // Near singularity: damp to avoid runaway
                            z.real = 0.1 + constant.real;
                            z.imag = 0.1 + constant.imag;
                        }
                        else
                        {
                            // z^2
                            double z2r = x * x - y * y;
                            double z2i = 2.0 * x * y;
                            double z2mag2 = z2r * z2r + z2i * z2i;

                            // 1/z^2
                            double invr = z2r / z2mag2;
                            double invi = -z2i / z2mag2;

                            z.real = invr + constant.real;
                            z.imag = invi + constant.imag;
                        }
                    }

                    return static_cast<double>(maxIter);
                };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Chebyshev Polynomial: T_3(z) = 4z^3 - 3z + c
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "ChebyshevPolynomial";
            spec.displayName = "Chebyshev Polynomial T₃(z) = 4z³ - 3z + c";
            spec.category = "Discrete Mathematics";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "The Chebyshev polynomial of the first kind T_3: z(n+1) = 4z(n)^3 - 3z(n) + c. Chebyshev polynomials are orthogonal polynomials fundamental to approximation theory and numerical analysis. Their equioscillating property creates distinctive escape band structures.";
            spec.formula = "z(n+1) = 4z(n)³ - 3z(n) + c";
            spec.formulaLatex = R"(z_{n+1} = 4z_n^3 - 3z_n + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Three-lobed main body with deep filamentary escape bands reflecting the three roots of the Chebyshev polynomial. Equioscillating structure creates uniform escape band widths and highly organized recursive detail.";
            spec.discoveredBy = "Chebyshev polynomials (approximation theory, Chebyshev)";
            spec.computationalNotes = "T_3(z) = 4z³ - 3z is derived from the trigonometric identity cos(3θ) = 4cos³(θ) - 3cos(θ). The iterative form exhibits three competing attractors, creating a three-way bifurcation landscape.";

            auto ic = InitialConditionsService::Get("ChebyshevPolynomial");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom != 0 ? ic.zoom : 0.7;
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
                        double mag2 = x * x + y * y;

                        if (mag2 > 256.0)
                        {
                            return static_cast<double>(i) + 1.0 - std::log2(std::log(mag2) / 2.0);
                        }

                        // z^2
                        double z2r = x * x - y * y;
                        double z2i = 2.0 * x * y;

                        // z^3 = z * z^2
                        double z3r = x * z2r - y * z2i;
                        double z3i = x * z2i + y * z2r;

                        // 4z^3 - 3z + c
                        z.real = 4.0 * z3r - 3.0 * x + constant.real;
                        z.imag = 4.0 * z3i - 3.0 * y + constant.imag;
                    }

                    return static_cast<double>(maxIter);
                };

            FractalRegistry::Register(spec);
        }
    }
}
