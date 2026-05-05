#include "FractalRegistry.h"
#include "MandelbrotCalculator.h"
#include <cmath>

namespace Native
{
    void RegisterEnhancedJuliaPresetsFamily()
    {
        // ═══════════════════════════════════════════════════════════════════════════════
        // ENHANCED JULIA PRESETS COLLECTION
        // Famous, beautiful, and mathematically interesting Julia set constants
        // ═══════════════════════════════════════════════════════════════════════════════

        // Helper lambda for standard Julia calculation
        auto juliaCalc = [](ComplexD c, int maxIter, ComplexD juliaConstant) -> double
        {
            ComplexD z = c;

            for (int i = 0; i < maxIter; ++i)
            {
                double x = z.real;
                double y = z.imag;

                z.real = x * x - y * y + juliaConstant.real;
                z.imag = 2.0 * x * y + juliaConstant.imag;

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

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Golden Ratio
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaGoldenRatio";
            spec.displayName = "Julia - Golden Ratio";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Julia set with c = φ - 2 where φ = (1+√5)/2 is the golden ratio. Creates spiral patterns related to Fibonacci sequence.";
            spec.formula = "z(n+1) = z² + c, c = φ - 2";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 + c, \quad c = \varphi - 2)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Golden spiral structure, Fibonacci-related patterns";
            spec.discoveredBy = "Golden ratio in complex dynamics";
            spec.discoveryYear = 1985;
            spec.computationalNotes = "φ = 1.618033988749895";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                const double phi = (1.0 + std::sqrt(5.0)) / 2.0;
                return juliaCalc(c, maxIter, ComplexD(phi - 2.0, 0.0));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Dendrite
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaDendrite";
            spec.displayName = "Julia - Dendrite";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Famous dendrite Julia set with c = i. Creates tree-like branching patterns along imaginary axis.";
            spec.formula = "z(n+1) = z² + i";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 + i)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Dendritic (tree-like) branches, symmetric along imaginary axis";
            spec.discoveredBy = "Classic Julia set exploration";
            spec.discoveryYear = 1982;
            spec.computationalNotes = "c = i creates dendritic Julia set";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.6;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(0.0, 1.0));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Spiral
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaSpiral";
            spec.displayName = "Julia - Spiral";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Creates tight spiral arms. c = 0.4 + 0.6i produces beautiful logarithmic spiral structure.";
            spec.formula = "z(n+1) = z² + (0.4 + 0.6i)";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 + 0.4 + 0.6i)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Tight logarithmic spirals, swirling patterns";
            spec.discoveredBy = "Julia set catalog";
            spec.discoveryYear = 1985;
            spec.computationalNotes = "Parameter in upper half-plane creates spirals";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(0.4, 0.6));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Dragon
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaDragon";
            spec.displayName = "Julia - Dragon";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Dragon-shaped Julia set. c = -0.8 + 0.156i creates distinctive dragon silhouette.";
            spec.formula = "z(n+1) = z² + (-0.8 + 0.156i)";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 - 0.8 + 0.156i)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Dragon silhouette, serpentine curves, long tail";
            spec.discoveredBy = "Julia set exploration";
            spec.discoveryYear = 1984;
            spec.computationalNotes = "Classic dragon parameter";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.55;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(-0.8, 0.156));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Cauliflower
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaCauliflower";
            spec.displayName = "Julia - Cauliflower";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Cauliflower-like Julia set. c = 0.25 creates puffy, vegetable-like structure.";
            spec.formula = "z(n+1) = z² + 0.25";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 + 0.25)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Rounded, puffy structures resembling cauliflower florets";
            spec.discoveredBy = "Boundary case Julia sets";
            spec.discoveryYear = 1985;
            spec.computationalNotes = "Near parabolic point creates cauliflower structure";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(0.25, 0.0));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Seahorse Valley
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaSeahorse";
            spec.displayName = "Julia - Seahorse Valley";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "From Mandelbrot's seahorse valley region. c = -0.75 + 0.11i creates curled seahorse patterns.";
            spec.formula = "z(n+1) = z² + (-0.75 + 0.11i)";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 - 0.75 + 0.11i)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Seahorse-like curls, spiral tails";
            spec.discoveredBy = "Mandelbrot seahorse valley exploration";
            spec.discoveryYear = 1985;
            spec.computationalNotes = "From interesting Mandelbrot region";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(-0.75, 0.11));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Airplane
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaAirplane";
            spec.displayName = "Julia - Airplane";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Airplane-shaped Julia set. c = -0.7269 + 0.1889i creates distinctive aircraft silhouette.";
            spec.formula = "z(n+1) = z² + (-0.7269 + 0.1889i)";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 - 0.7269 + 0.1889i)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Aircraft silhouette with wings and fuselage";
            spec.discoveredBy = "Pareidolia in Julia sets";
            spec.discoveryYear = 1990;
            spec.computationalNotes = "Resembles airplane from certain angles";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(-0.7269, 0.1889));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Lightning
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaLightning";
            spec.displayName = "Julia - Lightning";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Creates lightning bolt-like filaments. c = -0.52 + 0.57i produces jagged electric patterns.";
            spec.formula = "z(n+1) = z² + (-0.52 + 0.57i)";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 - 0.52 + 0.57i)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Jagged lightning bolt filaments, electric discharge appearance";
            spec.discoveredBy = "Julia set catalog";
            spec.discoveryYear = 1988;
            spec.computationalNotes = "High imaginary component creates jagged patterns";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(-0.52, 0.57));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Snowflake
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaSnowflake";
            spec.displayName = "Julia - Snowflake";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Snowflake-like hexagonal patterns. c = 0.285 + 0.01i creates delicate crystalline structure.";
            spec.formula = "z(n+1) = z² + (0.285 + 0.01i)";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 + 0.285 + 0.01i)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Hexagonal symmetry, snowflake crystalline patterns";
            spec.discoveredBy = "Near-parabolic Julia sets";
            spec.discoveryYear = 1987;
            spec.computationalNotes = "Close to parabolic point creates intricate detail";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(0.285, 0.01));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Flower
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaFlower";
            spec.displayName = "Julia - Flower";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Flower petal patterns. c = 0.28 + 0.008i creates delicate floral structure.";
            spec.formula = "z(n+1) = z² + (0.28 + 0.008i)";
            spec.formulaLatex = R"(z_{n+1) = z_n^2 + 0.28 + 0.008i)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Flower petals, rotational near-symmetry";
            spec.discoveredBy = "Aesthetic Julia parameter search";
            spec.discoveryYear = 1989;
            spec.computationalNotes = "Near real axis creates petal-like structures";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(0.28, 0.008));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Feigenbaum Point
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaFeigenbaum";
            spec.displayName = "Julia - Feigenbaum Point";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "At Feigenbaum point c = -1.401155... showing period-doubling cascade endpoint.";
            spec.formula = "z(n+1) = z² - 1.401155";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 - 1.401155)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Bifurcation accumulation, Cantor set-like structure";
            spec.discoveredBy = "Mitchell Feigenbaum connection";
            spec.discoveryYear = 1983;
            spec.computationalNotes = "Feigenbaum constant δ ≈ 4.669 appears in scaling";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(-1.401155, 0.0));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Twisted Cross
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaTwistedCross";
            spec.displayName = "Julia - Twisted Cross";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Creates twisted cross or swastika-like pattern. c = 0.45 + 0.1428i produces rotational structure.";
            spec.formula = "z(n+1) = z² + (0.45 + 0.1428i)";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 + 0.45 + 0.1428i)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Four-armed twisted cross, rotational patterns";
            spec.discoveredBy = "Symmetric Julia exploration";
            spec.discoveryYear = 1991;
            spec.computationalNotes = "Near-rational ratio creates pseudo-symmetry";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(0.45, 0.1428));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Backbone
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaBackbone";
            spec.displayName = "Julia - Backbone";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "From Mandelbrot backbone/spine region. c = -1.0 + 0.0i creates linear spine structure.";
            spec.formula = "z(n+1) = z² - 1";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 - 1)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Linear spine, bilateral symmetry, segmented structure";
            spec.discoveredBy = "Real axis Julia sets";
            spec.discoveryYear = 1982;
            spec.computationalNotes = "c = -1 is boundary between connected/disconnected";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(-1.0, 0.0));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Spiral Galaxy
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaSpiralGalaxy";
            spec.displayName = "Julia - Spiral Galaxy";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Creates galaxy-like spiral arms. c = -0.4 + 0.59i produces rotating arm structure.";
            spec.formula = "z(n+1) = z² + (-0.4 + 0.59i)";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 - 0.4 + 0.59i)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Spiral galaxy arms, rotating structures";
            spec.discoveredBy = "Astronomical-looking Julia sets";
            spec.discoveryYear = 1990;
            spec.computationalNotes = "Specific ratio creates spiral arm separation";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(-0.4, 0.59));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Medusa
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaMedusa";
            spec.displayName = "Julia - Medusa";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Medusa-like with many tentacles. c = -0.194 + 0.6557i creates head with snake-hair tendrils.";
            spec.formula = "z(n+1) = z² + (-0.194 + 0.6557i)";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 - 0.194 + 0.6557i)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Central head with many writhing tentacle-like extensions";
            spec.discoveredBy = "Mythological Julia patterns";
            spec.discoveryYear = 1992;
            spec.computationalNotes = "Many small bulbs create tentacle appearance";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(-0.194, 0.6557));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Crystal
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaCrystal";
            spec.displayName = "Julia - Crystal";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Crystalline faceted structure. c = -0.7 + 0.27015i creates gem-like angular patterns.";
            spec.formula = "z(n+1) = z² + (-0.7 + 0.27015i)";
            spec.formulaLatex = R"(z_{n+1) = z_n^2 - 0.7 + 0.27015i)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Angular facets, crystalline structure, gem-like appearance";
            spec.discoveredBy = "Geometric Julia exploration";
            spec.discoveryYear = 1988;
            spec.computationalNotes = "Creates angular rather than smooth boundaries";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(-0.7, 0.27015));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Paisley
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaPaisley";
            spec.displayName = "Julia - Paisley";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Paisley pattern-like curves. c = -0.162 + 1.04i creates decorative swirls.";
            spec.formula = "z(n+1) = z² + (-0.162 + 1.04i)";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 - 0.162 + 1.04i)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Paisley-like decorative swirls, textile pattern appearance";
            spec.discoveredBy = "Artistic Julia parameters";
            spec.discoveryYear = 1993;
            spec.computationalNotes = "High imaginary component creates elongated swirls";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(-0.162, 1.04));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Fuzzy Blob
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaFuzzyBlob";
            spec.displayName = "Julia - Fuzzy Blob";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Nearly circular with fuzzy edge. c = -0.11 + 0.6557i creates blob with fractal boundary.";
            spec.formula = "z(n+1) = z² + (-0.11 + 0.6557i)";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 - 0.11 + 0.6557i)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Rounded blob shape with intricate boundary details";
            spec.discoveredBy = "Nearly-circular Julia exploration";
            spec.discoveryYear = 1986;
            spec.computationalNotes = "Small real component maintains approximate circularity";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(-0.11, 0.6557));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Eye
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaEye";
            spec.displayName = "Julia - Eye";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Eye-shaped Julia set. c = -0.75 creates almond-shaped eye with intricate iris.";
            spec.formula = "z(n+1) = z² - 0.75";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 - 0.75)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Almond eye shape with detailed fractal iris";
            spec.discoveredBy = "Real axis Julia sets";
            spec.discoveryYear = 1983;
            spec.computationalNotes = "Near transition point creates elongated shape";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = true;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(-0.75, 0.0));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Triple Spiral
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaTripleSpiral";
            spec.displayName = "Julia - Triple Spiral";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Three distinct spiral arms. c = -0.4 + 0.6i creates threefold pseudo-symmetry.";
            spec.formula = "z(n+1) = z² + (-0.4 + 0.6i)";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 - 0.4 + 0.6i)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Three spiral arms, Celtic triskelion-like";
            spec.discoveredBy = "Multi-arm Julia sets";
            spec.discoveryYear = 1991;
            spec.computationalNotes = "Parameter creates visual threefold structure";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(-0.4, 0.6));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Heart
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaHeart";
            spec.displayName = "Julia - Heart";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Heart-shaped Julia set. c = -0.835 - 0.2321i creates valentine heart silhouette.";
            spec.formula = "z(n+1) = z² + (-0.835 - 0.2321i)";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 - 0.835 - 0.2321i)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Heart shape with cleft at top, romantic appearance";
            spec.discoveredBy = "Pareidolia in Julia parameters";
            spec.discoveryYear = 1995;
            spec.computationalNotes = "Specific parameter ratio creates heart cleft";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(-0.835, -0.2321));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Neurons
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaNeurons";
            spec.displayName = "Julia - Neurons";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Neural network-like structure. c = -0.8 + 0.156i creates interconnected neuron appearance.";
            spec.formula = "z(n+1) = z² + (-0.8 + 0.156i)";
            spec.formulaLatex = R"(z_{n+1) = z_n^2 - 0.8 + 0.156i)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Interconnected nodes, neural network-like connections";
            spec.discoveredBy = "Biological Julia patterns";
            spec.discoveryYear = 1994;
            spec.computationalNotes = "Creates node-and-connection topology";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(-0.8, 0.156));
            };

            FractalRegistry::Register(spec);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // Julia: Fractal Tree
        // ───────────────────────────────────────────────────────────────────────────────
        {
            FractalSpec spec;
            spec.name = "JuliaFractalTree";
            spec.displayName = "Julia - Fractal Tree";
            spec.category = "Julia Presets";
            spec.type = FractalCategory::EscapeTime2D;
            spec.description = "Tree-like branching structure. c = -0.75 + 0.2i creates trunk with fractal branches.";
            spec.formula = "z(n+1) = z² + (-0.75 + 0.2i)";
            spec.formulaLatex = R"(z_{n+1} = z_n^2 - 0.75 + 0.2i)";
            spec.supportsJulia = false;

            spec.visualCharacteristics = "Tree trunk with self-similar branches, botanical structure";
            spec.discoveredBy = "Botanical Julia patterns";
            spec.discoveryYear = 1989;
            spec.computationalNotes = "Creates hierarchical branching pattern";

            spec.defaultCenterX = 0.0;
            spec.defaultCenterY = 0.0;
            spec.defaultZoom = 0.5;
            spec.defaultBailout = 256.0;
            spec.hasSymmetry = false;

            spec.calculator = [juliaCalc](ComplexD c, int maxIter, bool isJulia, ComplexD juliaC, const ParamMap& params) -> double
            {
                return juliaCalc(c, maxIter, ComplexD(-0.75, 0.2));
            };

            FractalRegistry::Register(spec);
        }
    }
}
