#include "FractalRegistry.h"
#include "InitialConditionsService.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native {

    void RegisterMechanicalEngineeringFamily()
    {
        // ───────────────────────────────────────────────────────────────────────────────
        // Stefan-Boltzmann Radiative Cooling (Heat Flow)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "StefanBoltzmann";
            spec.displayName = "Stefan-Boltzmann Radiative Cooling (Heat Flow)";
            spec.category = "Mechanical Engineering";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Maps radiative heat transfer, where emitted flux scales with the fourth power of temperature (q = e*sigma*T^4). The quartic relaxation drives a thermal-equilibrium escape map.";
            spec.formula = "z(n+1) = z(n) - k * (z(n)^4 - c)";
            spec.formulaLatex = R"(z_{n+1} = z_n - k\left(z_n^4 - c\right))";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Quartic relaxation produces a four-fold symmetric body with radiating 'thermal bloom' filaments, evoking heat dissipating away from a glowing core.";
            spec.discoveredBy = "Stefan-Boltzmann Law (Stefan 1879, Boltzmann 1884)";
            spec.computationalNotes = "Relaxation form of dT/dt = -k(T^4 - T_env^4). The cooling rate k = 0.25 controls how aggressively each step relaxes toward the radiative equilibrium c.";

            auto ic = InitialConditionsService::Get("StefanBoltzmann");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom != 0 ? ic.zoom : 0.7;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
                {
                    ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                    ComplexD constant = isJulia ? juliaC : c;

                    const double k = 0.25; // radiative cooling rate

                    for (int i = 0; i < maxIter; ++i)
                    {
                        double x2 = z.real * z.real;
                        double y2 = z.imag * z.imag;
                        double mag2 = x2 + y2;

                        if (mag2 > 256.0)
                        {
                            return static_cast<double>(i) + 1.0 - std::log2(std::log(mag2) / 2.0);
                        }

                        // z^2 = (x^2 - y^2) + i(2xy)
                        double a = x2 - y2;
                        double b = 2.0 * z.real * z.imag;

                        // z^4 = (z^2)^2 = (a^2 - b^2) + i(2ab)
                        double z4r = a * a - b * b;
                        double z4i = 2.0 * a * b;

                        // z - k*(z^4 - c)  ->  z - k*z^4 + k*c
                        double newReal = z.real - k * z4r + k * constant.real;
                        double newImag = z.imag - k * z4i + k * constant.imag;

                        z.real = newReal;
                        z.imag = newImag;
                    }

                    return static_cast<double>(maxIter);
                };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Euler-Bernoulli Buckling (Beam Deflection)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "EulerBuckling";
            spec.displayName = "Euler-Bernoulli Buckling (Beam Deflection)";
            spec.category = "Mechanical Engineering";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Models the onset of column buckling, where a slender beam loses stability past the critical load P_cr = pi^2 EI / (KL)^2. Superimposes the first two symmetric buckling eigenmodes (P_n = n^2 pi^2 EI / L^2): a cubic fundamental-mode stiffening and a quintic second-mode stiffening, both resisting a linear restoring load.";
            spec.formula = "z(n+1) = mu * z(n)^5 + z(n)^3 - 3k * z(n) + c";
            spec.formulaLatex = R"(z_{n+1} = \mu\,z_n^5 + z_n^3 - 3k\,z_n + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "Twin buckling lobes from the fundamental cubic mode, now folded by the quintic second eigenmode that introduces extra inflection points and finer side-lobes, echoing the S-shaped higher mode shape a slender column snaps into under increasing load.";
            spec.discoveredBy = "Euler (1744) elastica & Euler-Bernoulli beam theory";
            spec.computationalNotes = "The restoring coefficient k = 1.0 is the critical-load factor and the cubic z^3 is the fundamental-mode geometric stiffening. The added quintic term mu*z^5 (mu = 0.5) is the second symmetric buckling eigenmode; using only odd powers keeps the map odd, so the set retains its point symmetry about the origin.";

            auto ic = InitialConditionsService::Get("EulerBuckling");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom != 0 ? ic.zoom : 0.6;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
                {
                    ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                    ComplexD constant = isJulia ? juliaC : c;

                    const double k = 1.0;   // critical-load restoring factor (fundamental mode)
                    const double mu = 0.5;  // second buckling eigenmode weight (quintic stiffening)

                    for (int i = 0; i < maxIter; ++i)
                    {
                        double x2 = z.real * z.real;
                        double y2 = z.imag * z.imag;
                        double mag2 = x2 + y2;

                        if (mag2 > 256.0)
                        {
                            return static_cast<double>(i) + 1.0 - std::log2(std::log(mag2) / 2.0);
                        }

                        // z^2 = (x^2 - y^2) + i(2xy)
                        double z2r = x2 - y2;
                        double z2i = 2.0 * z.real * z.imag;

                        // z^3 = (x^3 - 3xy^2) + i(3x^2y - y^3)   (fundamental buckling mode)
                        double z3r = z.real * (x2 - 3.0 * y2);
                        double z3i = z.imag * (3.0 * x2 - y2);

                        // z^5 = z^3 * z^2   (second symmetric buckling eigenmode)
                        double z5r = z3r * z2r - z3i * z2i;
                        double z5i = z3r * z2i + z3i * z2r;

                        // mu*z^5 + z^3 - 3k*z + c
                        double newReal = mu * z5r + z3r - 3.0 * k * z.real + constant.real;
                        double newImag = mu * z5i + z3i - 3.0 * k * z.imag + constant.imag;

                        z.real = newReal;
                        z.imag = newImag;
                    }

                    return static_cast<double>(maxIter);
                };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Ramberg-Osgood Plastic Deformation (Malleability)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "RambergOsgood";
            spec.displayName = "Ramberg-Osgood Plastic Deformation (Malleability)";
            spec.category = "Mechanical Engineering";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Models the elastic-plastic strain response of malleable metals, combining a linear elastic term (sigma/E) with a non-linear plastic hardening term K*(sigma)^n past yield.";
            spec.formula = "z(n+1) = z(n)/E + K * z(n)^3 + c";
            spec.formulaLatex = R"(z_{n+1} = \frac{z_n}{E} + K\,z_n^{3} + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "The linear elastic term shears the cubic plastic body, producing asymmetric 'yielding' bulbs that flow to one side like a metal deforming permanently under load.";
            spec.discoveredBy = "Ramberg & Osgood (1943) stress-strain relation";
            spec.computationalNotes = "Elastic modulus E = 2.0 sets the recoverable linear response; hardening coefficient K = 1.0 with exponent n = 3 models plastic strain accumulation.";

            auto ic = InitialConditionsService::Get("RambergOsgood");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom != 0 ? ic.zoom : 0.6;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
                {
                    ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                    ComplexD constant = isJulia ? juliaC : c;

                    const double E = 2.0; // elastic modulus (linear, recoverable strain)
                    const double K = 1.0; // plastic hardening coefficient

                    for (int i = 0; i < maxIter; ++i)
                    {
                        double x2 = z.real * z.real;
                        double y2 = z.imag * z.imag;
                        double mag2 = x2 + y2;

                        if (mag2 > 256.0)
                        {
                            return static_cast<double>(i) + 1.0 - std::log2(std::log(mag2) / 2.0);
                        }

                        // z^3 = (x^3 - 3xy^2) + i(3x^2y - y^3)
                        double z3r = z.real * (x2 - 3.0 * y2);
                        double z3i = z.imag * (3.0 * x2 - y2);

                        // z/E + K*z^3 + c   (elastic + plastic strain)
                        double newReal = z.real / E + K * z3r + constant.real;
                        double newImag = z.imag / E + K * z3i + constant.imag;

                        z.real = newReal;
                        z.imag = newImag;
                    }

                    return static_cast<double>(maxIter);
                };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Basquin Fatigue Power Law (Metal Fatigue / S-N Curve)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "BasquinFatigue";
            spec.displayName = "Basquin Fatigue Power Law (S-N Curve)";
            spec.category = "Mechanical Engineering";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Maps the high-cycle fatigue power law relating stress amplitude to cycles-to-failure (sigma_a = sigma_f' * (2N)^b). The fractional complex power encodes the self-similar accumulation of fatigue damage.";
            spec.formula = "z(n+1) = z(n)^p + c   (fractional power p)";
            spec.formulaLatex = R"(z_{n+1} = z_n^{p} + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "The fractional power introduces a branch cut along the negative real axis, producing swirling striation-like bands reminiscent of fatigue beach marks on a fractured surface.";
            spec.discoveredBy = "Basquin (1910) S-N fatigue power law";
            spec.computationalNotes = "Uses the complex power z^p = r^p * (cos(p*theta) + i sin(p*theta)) with p = 2.5 representing the fatigue power-law exponent. Branch cut handled via atan2.";

            auto ic = InitialConditionsService::Get("BasquinFatigue");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom != 0 ? ic.zoom : 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
                {
                    ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                    ComplexD constant = isJulia ? juliaC : c;

                    const double p = 2.5; // fatigue power-law exponent

                    for (int i = 0; i < maxIter; ++i)
                    {
                        double mag2 = z.real * z.real + z.imag * z.imag;

                        if (mag2 > 256.0) return static_cast<double>(i);

                        // Complex power z^p = r^p * (cos(p*theta) + i sin(p*theta))
                        if (mag2 < 1e-18)
                        {
                            // z near 0: z^p -> 0 for p > 0
                            z.real = constant.real;
                            z.imag = constant.imag;
                            continue;
                        }

                        double r = std::sqrt(mag2);
                        double theta = std::atan2(z.imag, z.real);
                        double rp = std::pow(r, p);
                        double ptheta = p * theta;

                        double newReal = rp * std::cos(ptheta) + constant.real;
                        double newImag = rp * std::sin(ptheta) + constant.imag;

                        z.real = newReal;
                        z.imag = newImag;
                    }

                    return static_cast<double>(maxIter);
                };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Torsional Twist (Contorsion / Angle of Twist)
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "TorsionalTwist";
            spec.displayName = "Torsional Twist (Angle of Twist)";
            spec.category = "Mechanical Engineering";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Models the angle of twist per unit length of a shaft under torque (phi = TL / GJ). Each iteration rotates the complex state by the torsion angle, winding the dynamics into a contorted spiral.";
            spec.formula = "z(n+1) = z(n)^2 * exp(i*theta) + c";
            spec.formulaLatex = R"(z_{n+1} = z_n^2\,e^{i\theta} + c)";
            spec.supportsJulia = true;

            spec.visualCharacteristics = "The per-iteration rotation winds the Mandelbrot body into spiral arms whose curl tightens with the twist rate, evoking a shaft contorting under applied torque.";
            spec.discoveredBy = "Classical torsion theory (Coulomb 1784, Saint-Venant)";
            spec.computationalNotes = "Twist rate theta = 0.5 rad represents T/(GJ). z^2 is rotated by exp(i*theta) = cos(theta) + i sin(theta) before adding c.";

            auto ic = InitialConditionsService::Get("TorsionalTwist");
            spec.defaultCenterX = ic.centerX;
            spec.defaultCenterY = ic.centerY;
            spec.defaultZoom = ic.zoom != 0 ? ic.zoom : 1.0;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
                {
                    ComplexD z = isJulia ? c : ComplexD(0.0, 0.0);
                    ComplexD constant = isJulia ? juliaC : c;

                    const double theta = 0.5; // twist rate T/(GJ)
                    const double cosT = std::cos(theta);
                    const double sinT = std::sin(theta);

                    for (int i = 0; i < maxIter; ++i)
                    {
                        double x2 = z.real * z.real;
                        double y2 = z.imag * z.imag;
                        double mag2 = x2 + y2;

                        if (mag2 > 256.0)
                        {
                            return static_cast<double>(i) + 1.0 - std::log2(std::log(mag2) / 2.0);
                        }

                        // z^2 = (x^2 - y^2) + i(2xy)
                        double z2r = x2 - y2;
                        double z2i = 2.0 * z.real * z.imag;

                        // Rotate z^2 by exp(i*theta), then add c
                        double rotR = z2r * cosT - z2i * sinT;
                        double rotI = z2r * sinT + z2i * cosT;

                        z.real = rotR + constant.real;
                        z.imag = rotI + constant.imag;
                    }

                    return static_cast<double>(maxIter);
                };

            FractalRegistry::Register(spec);
        }
    }
}
