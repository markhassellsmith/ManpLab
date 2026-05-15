using ManpWinUI.Models.Parameters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ManpWinUI.Services;

/// <summary>
/// Implementation of fractal parameter service.
/// Manages parameter definitions, caching, and persistence.
/// </summary>
public class FractalParameterService : IFractalParameterService
{
    // ═══════════════════════════════════════════════════════════════════════════════
    // DEPENDENCIES
    // ═══════════════════════════════════════════════════════════════════════════════

    private readonly IAppSettingsService _settingsService;
    private readonly IFractalMetadataService _metadataService;

    // ═══════════════════════════════════════════════════════════════════════════════
    // STATE
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Cache of parameter templates by fractal type.
    /// Populated during initialization from native registry.
    /// </summary>
    private readonly Dictionary<string, Func<FractalParameterSet>> _parameterTemplates = new();

    /// <summary>
    /// Lock for thread-safe initialization.
    /// </summary>
    private readonly SemaphoreSlim _initLock = new(1, 1);

    /// <summary>
    /// Initialization state flag.
    /// </summary>
    private bool _initialized = false;

    // ═══════════════════════════════════════════════════════════════════════════════
    // CONSTRUCTOR
    // ═══════════════════════════════════════════════════════════════════════════════

    public FractalParameterService(
        IAppSettingsService settingsService,
        IFractalMetadataService metadataService)
    {
        _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
        _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // INITIALIZATION
    // ═══════════════════════════════════════════════════════════════════════════════

    public async Task InitializeAsync()
    {
        await _initLock.WaitAsync();
        try
        {
            if (_initialized)
                return;

            Debug.WriteLine("[FractalParameterService] Initializing parameter templates...");

            // Phase 1: Register hardcoded templates for known fractal families
            // This provides immediate backwards compatibility while we extend native registry
            RegisterBuiltInTemplates();

            // Phase 2: Load parameter metadata from native registry (future enhancement)
            // await LoadFromNativeRegistryAsync();

            // *** ONE-TIME FIX: Clear bad ExpSquare MaxIterations saved value ***
            // Remove this after users have run the app once with this fix
            ClearExpSquareSettingsIfNeeded();

            _initialized = true;
            Debug.WriteLine($"[FractalParameterService] Initialized with {_parameterTemplates.Count} parameter templates");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[FractalParameterService] Initialization failed: {ex.Message}");
            throw;
        }
        finally
        {
            _initLock.Release();
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // HELPER: Get native viewport defaults
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Gets viewport defaults from native FractalDescriptor, with fallbacks to (0, 0, 1.0).
    /// </summary>
    private (double centerX, double centerY, double zoom) GetNativeViewportDefaults(string fractalType)
    {
        var descriptor = _metadataService.GetFractal(fractalType);
        if (descriptor != null)
        {
            return (descriptor.DefaultCenterX, descriptor.DefaultCenterY, descriptor.DefaultZoom);
        }
        return (0.0, 0.0, 1.0); // Fallback if fractal not found
    }

    /// <summary>
    /// Helper: Create a standard escape-time template WITH native viewport defaults.
    /// </summary>
    private FractalParameterSet CreateStandardTemplate(string fractalType)
    {
        var (cx, cy, z) = GetNativeViewportDefaults(fractalType);
        return StandardParameterTemplates.CreateStandardEscapeTime(fractalType, cx, cy, z);
    }

    /// <summary>
    /// Helper: Create a Julia-enabled template WITH native viewport defaults.
    /// </summary>
    private FractalParameterSet CreateJuliaTemplate(string fractalType)
    {
        var (cx, cy, z) = GetNativeViewportDefaults(fractalType);
        return StandardParameterTemplates.CreateWithJulia(fractalType, cx, cy, z);
    }

    /// <summary>
    /// Helper: Create a Multibrot template WITH native viewport defaults.
    /// </summary>
    private FractalParameterSet CreateMultibrotTemplate(string fractalType, int exponent)
    {
        var (cx, cy, z) = GetNativeViewportDefaults(fractalType);
        return StandardParameterTemplates.CreateMultibrot(fractalType, exponent, cx, cy, z);
    }

    /// <summary>
    /// Register built-in parameter templates for known fractal families.
    /// This is the 80/20 solution: covers most fractals with standard templates.
    /// </summary>
    private void RegisterBuiltInTemplates()
    {
        // ═══════════════════════════════════════════════════════════════════════════
        // MANDELBROT FAMILY (Standard + Julia)
        // ═══════════════════════════════════════════════════════════════════════════
        RegisterTemplate("Mandelbrot", () => CreateJuliaTemplate("Mandelbrot"));
        RegisterTemplate("BurningShip", () => CreateJuliaTemplate("BurningShip"));
        RegisterTemplate("Tricorn", () => CreateJuliaTemplate("Tricorn"));
        RegisterTemplate("Celtic", () => CreateJuliaTemplate("Celtic"));
        RegisterTemplate("Buffalo", () => CreateJuliaTemplate("Buffalo"));
        RegisterTemplate("MandelbarCeltic", () => CreateJuliaTemplate("MandelbarCeltic"));

        // ═══════════════════════════════════════════════════════════════════════════
        // MULTIBROT FAMILY (z^n + c with integer exponent)
        // ═══════════════════════════════════════════════════════════════════════════
        RegisterTemplate("Multibrot", () => CreateMultibrotTemplate("Multibrot", 3));
        RegisterTemplate("z4", () => CreateMultibrotTemplate("z4", 4));
        RegisterTemplate("z5", () => CreateMultibrotTemplate("z5", 5));
        RegisterTemplate("z6", () => CreateMultibrotTemplate("z6", 6));
        RegisterTemplate("z7", () => CreateMultibrotTemplate("z7", 7));
        RegisterTemplate("z8", () => CreateMultibrotTemplate("z8", 8));

        // ═══════════════════════════════════════════════════════════════════════════
        // COMPLEX EXPONENT FAMILY
        // ═══════════════════════════════════════════════════════════════════════════
        RegisterTemplate("MarksMandel", () =>
        {
            var paramSet = CreateJuliaTemplate("MarksMandel");
            paramSet.AddParameters(StandardParameterTemplates.ComplexExponent().ToArray());
            return paramSet;
        });

        RegisterTemplate("MarksMandelpwr", () =>
        {
            var paramSet = CreateJuliaTemplate("MarksMandelpwr");
            paramSet.AddParameters(StandardParameterTemplates.ComplexExponent().ToArray());
            return paramSet;
        });

        // ═══════════════════════════════════════════════════════════════════════════
        // PHOENIX FAMILY (z² + c + p*z_prev)
        // ═══════════════════════════════════════════════════════════════════════════
        RegisterTemplate("Phoenix", () =>
        {
            var paramSet = CreateStandardTemplate("Phoenix");

            // Phoenix-specific parameters
            paramSet.AddParameter(new FractalParameterDescriptor
            {
                Key = "phoenix_p",
                Name = "Phoenix P (Real)",
                Type = ParameterType.Double,
                Category = ParameterCategory.FractalSpecific,
                DefaultValue = 0.56667,
                MinValue = -2.0,
                MaxValue = 2.0,
                StepSize = 0.001,
                FormatString = "F5",
                Description = "Real part of the Phoenix parameter p",
                DisplayOrder = 1
            });

            paramSet.AddParameter(new FractalParameterDescriptor
            {
                Key = "phoenix_q",
                Name = "Phoenix P (Imaginary)",
                Type = ParameterType.Double,
                Category = ParameterCategory.FractalSpecific,
                DefaultValue = 0.0,
                MinValue = -2.0,
                MaxValue = 2.0,
                StepSize = 0.001,
                FormatString = "F5",
                Description = "Imaginary part of the Phoenix parameter p",
                DisplayOrder = 2
            });

            return paramSet;
        });

        // ═══════════════════════════════════════════════════════════════════════════
        // NEWTON METHOD FAMILY
        // ═══════════════════════════════════════════════════════════════════════════
        RegisterTemplate("Newton", () =>
        {
            var (cx, cy, z) = GetNativeViewportDefaults("Newton");
            return StandardParameterTemplates.CreateNewton("Newton", cx, cy, z);
        });
        RegisterTemplate("NewtonSinExp", () =>
        {
            var (cx, cy, z) = GetNativeViewportDefaults("NewtonSinExp");
            return StandardParameterTemplates.CreateNewton("NewtonSinExp", cx, cy, z);
        });

        // ═══════════════════════════════════════════════════════════════════════════
        // NOVA FAMILY (Newton + Julia hybrid)
        // ═══════════════════════════════════════════════════════════════════════════
        RegisterTemplate("Nova", () =>
        {
            var (cx, cy, z) = GetNativeViewportDefaults("Nova");
            var paramSet = StandardParameterTemplates.CreateNewton("Nova", cx, cy, z);
            paramSet.AddParameters(StandardParameterTemplates.JuliaMode().ToArray());
            return paramSet;
        });

        // ═══════════════════════════════════════════════════════════════════════════
        // MAGNET FAMILY
        // ═══════════════════════════════════════════════════════════════════════════
        RegisterTemplate("Magnet1M", () => CreateJuliaTemplate("Magnet1M"));
        RegisterTemplate("Magnet2M", () => CreateJuliaTemplate("Magnet2M"));
        RegisterTemplate("Magnet1J", () => CreateJuliaTemplate("Magnet1J"));
        RegisterTemplate("Magnet2J", () => CreateJuliaTemplate("Magnet2J"));

        // ═══════════════════════════════════════════════════════════════════════════
        // LAMBDA FAMILY (λ * z^p * (1-z))
        // ═══════════════════════════════════════════════════════════════════════════
        RegisterTemplate("Lambda", () =>
        {
            var paramSet = CreateStandardTemplate("Lambda");

            paramSet.AddParameter(new FractalParameterDescriptor
            {
                Key = "lambda_real",
                Name = "Lambda (Real)",
                Type = ParameterType.Double,
                Category = ParameterCategory.FractalSpecific,
                DefaultValue = -0.5,
                MinValue = -2.0,
                MaxValue = 2.0,
                StepSize = 0.001,
                FormatString = "F6",
                Description = "Real part of the λ parameter",
                DisplayOrder = 1
            });

            paramSet.AddParameter(new FractalParameterDescriptor
            {
                Key = "lambda_imag",
                Name = "Lambda (Imaginary)",
                Type = ParameterType.Double,
                Category = ParameterCategory.FractalSpecific,
                DefaultValue = 0.0,
                MinValue = -2.0,
                MaxValue = 2.0,
                StepSize = 0.001,
                FormatString = "F6",
                Description = "Imaginary part of the λ parameter",
                DisplayOrder = 2
            });

            paramSet.AddParameter(StandardParameterTemplates.IntegerExponent(2, 2, 10));

            return paramSet;
        });

        // ═══════════════════════════════════════════════════════════════════════════
        // EXPONENTIAL FRACTALS WITH CUSTOM DEFAULTS
        // ═══════════════════════════════════════════════════════════════════════════
        RegisterTemplate("ExpSquare", () =>
        {
            // Create standard template first
            var paramSet = CreateJuliaTemplate("ExpSquare");

            // Set MaxIterations to 1000 (override any saved setting by creating a fresh parameter set)
            // This fractal was accidentally saved with 5000 which is too high for general use
            paramSet.SetValue("max_iterations", 1000);

            return paramSet;
        });

        // ═══════════════════════════════════════════════════════════════════════════
        // JULIA PRESET FAMILY (23 named Julia sets with fixed C values)
        // ═══════════════════════════════════════════════════════════════════════════
        // Julia presets are NOT Julia mode fractals - they have hardcoded C values
        // and only need standard view + algorithm parameters

        RegisterTemplate("JuliaGoldenRatio", () => CreateStandardTemplate("JuliaGoldenRatio"));
        RegisterTemplate("JuliaDendrite", () => CreateStandardTemplate("JuliaDendrite"));
        RegisterTemplate("JuliaSpiral", () => CreateStandardTemplate("JuliaSpiral"));
        RegisterTemplate("JuliaDragon", () => CreateStandardTemplate("JuliaDragon"));
        RegisterTemplate("JuliaCauliflower", () => CreateStandardTemplate("JuliaCauliflower"));
        RegisterTemplate("JuliaSeahorse", () => CreateStandardTemplate("JuliaSeahorse"));
        RegisterTemplate("JuliaAirplane", () => CreateStandardTemplate("JuliaAirplane"));
        RegisterTemplate("JuliaLightning", () => CreateStandardTemplate("JuliaLightning"));
        RegisterTemplate("JuliaSnowflake", () => CreateStandardTemplate("JuliaSnowflake"));
        RegisterTemplate("JuliaFlower", () => CreateStandardTemplate("JuliaFlower"));
        RegisterTemplate("JuliaFeigenbaum", () => CreateStandardTemplate("JuliaFeigenbaum"));
        RegisterTemplate("JuliaTwistedCross", () => CreateStandardTemplate("JuliaTwistedCross"));
        RegisterTemplate("JuliaBackbone", () => CreateStandardTemplate("JuliaBackbone"));
        RegisterTemplate("JuliaSpiralGalaxy", () => CreateStandardTemplate("JuliaSpiralGalaxy"));
        RegisterTemplate("JuliaMedusa", () => CreateStandardTemplate("JuliaMedusa"));
        RegisterTemplate("JuliaCrystal", () => CreateStandardTemplate("JuliaCrystal"));
        RegisterTemplate("JuliaPaisley", () => CreateStandardTemplate("JuliaPaisley"));
        RegisterTemplate("JuliaFuzzyBlob", () => CreateStandardTemplate("JuliaFuzzyBlob"));
        RegisterTemplate("JuliaEye", () => CreateStandardTemplate("JuliaEye"));
        RegisterTemplate("JuliaTripleSpiral", () => CreateStandardTemplate("JuliaTripleSpiral"));
        RegisterTemplate("JuliaHeart", () => CreateStandardTemplate("JuliaHeart"));
        RegisterTemplate("JuliaNeurons", () => CreateStandardTemplate("JuliaNeurons"));
        RegisterTemplate("JuliaFractalTree", () => CreateStandardTemplate("JuliaFractalTree"));

        // ═══════════════════════════════════════════════════════════════════════════
        // BURNING SHIP FAMILY POWER VARIANTS
        // ═══════════════════════════════════════════════════════════════════════════
        // Higher power variations of the Burning Ship formula

        RegisterTemplate("BurningShipCubic", () => CreateJuliaTemplate("BurningShipCubic"));
        RegisterTemplate("BurningShipQuartic", () => CreateJuliaTemplate("BurningShipQuartic"));
        RegisterTemplate("BurningShipQuintic", () => CreateJuliaTemplate("BurningShipQuintic"));
        RegisterTemplate("PerpendicularBurningShip", () => CreateJuliaTemplate("PerpendicularBurningShip"));
        RegisterTemplate("BuffaloBurningShip", () => CreateJuliaTemplate("BuffaloBurningShip"));
        RegisterTemplate("SharkBurningShip", () => CreateJuliaTemplate("SharkBurningShip"));
        RegisterTemplate("CelticBurningShip", () => CreateJuliaTemplate("CelticBurningShip"));
        RegisterTemplate("ReverseBurningShip", () => CreateJuliaTemplate("ReverseBurningShip"));
        RegisterTemplate("VerticalBurningShip", () => CreateJuliaTemplate("VerticalBurningShip"));
        RegisterTemplate("DiagonalBurningShip", () => CreateJuliaTemplate("DiagonalBurningShip"));
        RegisterTemplate("PerpendicularMandelbrot", () => CreateJuliaTemplate("PerpendicularMandelbrot"));
        RegisterTemplate("BirdOfPrey", () => CreateJuliaTemplate("BirdOfPrey"));

        // ═══════════════════════════════════════════════════════════════════════════
        // NEWTON/CONVERGENCE FAMILY EXTENSIONS
        // ═══════════════════════════════════════════════════════════════════════════

        RegisterTemplate("NewtonQuartic", () => CreateStandardTemplate("NewtonQuartic"));
        RegisterTemplate("HalleyCubic", () => CreateStandardTemplate("HalleyCubic"));

        // ═══════════════════════════════════════════════════════════════════════════
        // TRIGONOMETRIC FAMILY (sine, cosine, tangent fractals)
        // ═══════════════════════════════════════════════════════════════════════════

        RegisterTemplate("MandelTrig", () => CreateJuliaTemplate("MandelTrig"));
        RegisterTemplate("Sine", () => CreateJuliaTemplate("Sine"));
        RegisterTemplate("LMandelSine", () => CreateJuliaTemplate("LMandelSine"));
        RegisterTemplate("LLambdaSine", () => CreateJuliaTemplate("LLambdaSine"));
        RegisterTemplate("LMandelCos", () => CreateJuliaTemplate("LMandelCos"));
        RegisterTemplate("LLambdaCos", () => CreateJuliaTemplate("LLambdaCos"));
        RegisterTemplate("LMandelSinh", () => CreateJuliaTemplate("LMandelSinh"));
        RegisterTemplate("LLambdaSinh", () => CreateJuliaTemplate("LLambdaSinh"));
        RegisterTemplate("LMandelCosh", () => CreateJuliaTemplate("LMandelCosh"));
        RegisterTemplate("LLambdaCosh", () => CreateJuliaTemplate("LLambdaCosh"));
        RegisterTemplate("SinZ", () => CreateJuliaTemplate("SinZ"));
        RegisterTemplate("CosZ", () => CreateJuliaTemplate("CosZ"));
        RegisterTemplate("CosTan", () => CreateJuliaTemplate("CosTan"));
        RegisterTemplate("LambdaTan", () => CreateJuliaTemplate("LambdaTan"));
        RegisterTemplate("NewtonSin", () => CreateStandardTemplate("NewtonSin"));
        RegisterTemplate("PhoenixSin", () => CreateJuliaTemplate("PhoenixSin"));
        RegisterTemplate("Sqr1OverTrig", () => CreateJuliaTemplate("Sqr1OverTrig"));
        RegisterTemplate("SqrTrig", () => CreateJuliaTemplate("SqrTrig"));
        RegisterTemplate("TrigPlusTrig", () => CreateJuliaTemplate("TrigPlusTrig"));
        RegisterTemplate("TrigXTrig", () => CreateJuliaTemplate("TrigXTrig"));

        // ═══════════════════════════════════════════════════════════════════════════
        // EXPONENTIAL/LOGARITHMIC FAMILY (exp, log, power functions)
        // ═══════════════════════════════════════════════════════════════════════════
        RegisterTemplate("Exponential", () => CreateJuliaTemplate("Exponential"));
        RegisterTemplate("Logarithm", () => CreateJuliaTemplate("Logarithm"));
        RegisterTemplate("Logarithmic", () => CreateJuliaTemplate("Logarithmic"));
        RegisterTemplate("MandelExp", () => CreateJuliaTemplate("MandelExp"));
        RegisterTemplate("LMandelExp", () => CreateJuliaTemplate("LMandelExp"));
        RegisterTemplate("LLambdaExp", () => CreateJuliaTemplate("LLambdaExp"));
        RegisterTemplate("PowerTower", () => CreateJuliaTemplate("PowerTower"));
        RegisterTemplate("ZToTheZ", () => CreateJuliaTemplate("ZToTheZ"));
        RegisterTemplate("ComplexPower", () => CreateJuliaTemplate("ComplexPower"));
        RegisterTemplate("ExponentialJulia", () => CreateJuliaTemplate("ExponentialJulia"));

        // ═══════════════════════════════════════════════════════════════════════════
        // POLYNOMIAL VARIANTS - PHOENIX EXTENDED FAMILY
        // ═══════════════════════════════════════════════════════════════════════════
        // Phoenix fractals with memory: z_new = f(z) + c + p * z_prev
        RegisterTemplate("PhoenixM", () => CreateStandardTemplate("PhoenixM"));
        RegisterTemplate("PhoenixJ", () => CreateJuliaTemplate("PhoenixJ"));
        RegisterTemplate("PhoenixPower3", () => CreateJuliaTemplate("PhoenixPower3"));
        RegisterTemplate("PhoenixPower4", () => CreateJuliaTemplate("PhoenixPower4"));

        // ═══════════════════════════════════════════════════════════════════════════
        // POLYNOMIAL VARIANTS - LAMBDA EXTENDED FAMILY
        // ═══════════════════════════════════════════════════════════════════════════
        // Lambda variations: z = λ * f(z) with different functions
        RegisterTemplate("LambdaPower3", () => CreateJuliaTemplate("LambdaPower3"));
        RegisterTemplate("LambdaPower4", () => CreateJuliaTemplate("LambdaPower4"));
        RegisterTemplate("LambdaTanh", () => CreateJuliaTemplate("LambdaTanh"));
        RegisterTemplate("LambdaSquared", () => CreateJuliaTemplate("LambdaSquared"));
        RegisterTemplate("LambdaFlip", () => CreateJuliaTemplate("LambdaFlip"));

        // ═══════════════════════════════════════════════════════════════════════════
        // POLYNOMIAL VARIANTS - BARNSLEY FAMILY
        // ═══════════════════════════════════════════════════════════════════════════
        // Michael Barnsley's IFS-inspired escape-time fractals
        RegisterTemplate("BarnsleyM1", () => CreateJuliaTemplate("BarnsleyM1"));
        RegisterTemplate("BarnsleyJ1", () => CreateStandardTemplate("BarnsleyJ1"));
        RegisterTemplate("BarnsleyM2", () => CreateJuliaTemplate("BarnsleyM2"));
        RegisterTemplate("BarnsleyJ2", () => CreateStandardTemplate("BarnsleyJ2"));
        RegisterTemplate("BarnsleyM3", () => CreateJuliaTemplate("BarnsleyM3"));
        RegisterTemplate("BarnsleyJ3", () => CreateStandardTemplate("BarnsleyJ3"));

        // ═══════════════════════════════════════════════════════════════════════════
        // POLYNOMIAL VARIANTS - SPIDER FAMILY
        // ═══════════════════════════════════════════════════════════════════════════
        // Spider fractal where the constant evolves with iteration
        RegisterTemplate("SpiderVariant", () => CreateJuliaTemplate("SpiderVariant"));

        // ═══════════════════════════════════════════════════════════════════════════
        // RATIONAL FUNCTION FAMILY (ratios of polynomials)
        // ═══════════════════════════════════════════════════════════════════════════
        // Newton methods were registered earlier; these are additional rational forms
        RegisterTemplate("NewtonQuintic", () => CreateStandardTemplate("NewtonQuintic"));
        RegisterTemplate("RationalZ2Z3", () => CreateJuliaTemplate("RationalZ2Z3"));
        RegisterTemplate("RationalSymmetric", () => CreateJuliaTemplate("RationalSymmetric"));
        RegisterTemplate("Mobius", () => CreateJuliaTemplate("Mobius"));
        RegisterTemplate("RationalPower", () => CreateJuliaTemplate("RationalPower"));

        // ═══════════════════════════════════════════════════════════════════════════
        // STRANGE ATTRACTORS (histogram-based, minimal parameters)
        // ═══════════════════════════════════════════════════════════════════════════
        // Attractors are histogram-based fractals that plot orbit trajectories
        // They don't use escape-time parameters (iterations, bailout, Julia mode)
        // Native code has hardcoded system parameters (sigma, rho, beta, etc.)
        // So we only provide view parameters here

        RegisterTemplate("Lorenz", () => CreateStandardTemplate("Lorenz"));
        RegisterTemplate("Thomas", () => CreateStandardTemplate("Thomas"));
        RegisterTemplate("Dadras", () => CreateStandardTemplate("Dadras"));
        RegisterTemplate("Pickover", () => CreateStandardTemplate("Pickover"));
        RegisterTemplate("Aizawa", () => CreateStandardTemplate("Aizawa"));
        RegisterTemplate("Halvorsen", () => CreateStandardTemplate("Halvorsen"));
        RegisterTemplate("ChenLee", () => CreateStandardTemplate("ChenLee"));
        RegisterTemplate("Clifford", () => CreateStandardTemplate("Clifford"));
        RegisterTemplate("DeJong", () => CreateStandardTemplate("DeJong"));
        RegisterTemplate("Tinkerbell", () => CreateStandardTemplate("Tinkerbell"));
        RegisterTemplate("Duffing", () => CreateStandardTemplate("Duffing"));
        RegisterTemplate("LiuChen", () => CreateStandardTemplate("LiuChen"));
        RegisterTemplate("RabinovichFabrikant", () => CreateStandardTemplate("RabinovichFabrikant"));
        RegisterTemplate("Arneodo", () => CreateStandardTemplate("Arneodo"));

        // ═══════════════════════════════════════════════════════════════════════════
        // PHASE 3 STEP 3B: NEWTON/CONVERGENCE VARIANTS (3 fractals)
        // Newton's method and related convergence methods for various functions
        // ═══════════════════════════════════════════════════════════════════════════

        // Newton Sextic (z⁶ - 1 = 0) - 6 convergence basins
        RegisterTemplate("NewtonSextic", () => CreateStandardTemplate("NewtonSextic"));

        // Newton Cosh (cosh(z) - 1 = 0) - hyperbolic convergence
        RegisterTemplate("NewtonCosh", () => CreateStandardTemplate("NewtonCosh"));

        // Newton Basin (z³ - 1 with root coloring) - classic Newton basins visualization
        RegisterTemplate("NewtonBasin", () => CreateStandardTemplate("NewtonBasin"));

        // ═══════════════════════════════════════════════════════════════════════════
        // PHASE 3 STEP 3C: VISUAL PRIORITY FRACTALS (8 fractals)
        // Visually distinctive fractals with unique rendering characteristics
        // ═══════════════════════════════════════════════════════════════════════════

        // Special rendering fractals
        RegisterTemplate("Buddhabrot", () => CreateStandardTemplate("Buddhabrot"));
        RegisterTemplate("Lyapunov", () => CreateStandardTemplate("Lyapunov"));
        RegisterTemplate("NumFractal", () => CreateJuliaTemplate("NumFractal"));

        // Historical biomorphs
        RegisterTemplate("Biomorphs", () => CreateJuliaTemplate("Biomorphs"));
        RegisterTemplate("PickoverStalks", () => CreateJuliaTemplate("PickoverStalks"));

        // IFS (Iterated Function Systems)
        RegisterTemplate("BarnsleyFern", () => CreateStandardTemplate("BarnsleyFern"));
        RegisterTemplate("SierpinskiIFS", () => CreateStandardTemplate("SierpinskiIFS"));
        RegisterTemplate("DragonCurveIFS", () => CreateStandardTemplate("DragonCurveIFS"));

        // ═══════════════════════════════════════════════════════════════════════════
        // PHASE 4 PRIORITY 1: JULIA CORE VARIANTS (8 fractals)
        // Core Julia set variations with different formulas and parameters
        // ═══════════════════════════════════════════════════════════════════════════

        // Julia Classic: Standard z² + c Julia set
        RegisterTemplate("JuliaClassic", () => CreateJuliaTemplate("JuliaClassic"));

        // Julia Cubic: z³ + c cubic Julia variation
        RegisterTemplate("JuliaCubic", () => CreateJuliaTemplate("JuliaCubic"));

        // Julia Burning Ship: Julia set with absolute value (Burning Ship formula)
        RegisterTemplate("JuliaBurningShip", () => CreateJuliaTemplate("JuliaBurningShip"));

        // Julia Phoenix: z² + c + p·z_prev (uses previous iteration)
        RegisterTemplate("JuliaPhoenix", () => CreateJuliaTemplate("JuliaPhoenix"));

        // Julia Lambda: λ·z·(1-z) logistic map variant
        RegisterTemplate("JuliaLambda", () => CreateJuliaTemplate("JuliaLambda"));

        // Julia Sine: sin(z) + c trigonometric variant
        RegisterTemplate("JuliaSine", () => CreateJuliaTemplate("JuliaSine"));

        // Julia Exponential: e^z + c exponential variant
        RegisterTemplate("JuliaExp", () => CreateJuliaTemplate("JuliaExp"));

        // Julia Magnet: Magnet 1 formula in Julia form
        RegisterTemplate("JuliaMagnet", () => CreateJuliaTemplate("JuliaMagnet"));

        // ═══════════════════════════════════════════════════════════════════════════
        // PHASE 4 PRIORITY 1: MANDELBROT/MULTIBROT VARIANTS (9 fractals)
        // Core Mandelbrot variations with different powers and modifications
        // ═══════════════════════════════════════════════════════════════════════════

        // Mandel4: z⁴ + c quartic Mandelbrot
        RegisterTemplate("Mandel4", () => CreateJuliaTemplate("Mandel4"));

        // Julia4: z⁴ + c Julia preset
        RegisterTemplate("Julia4", () => CreateStandardTemplate("Julia4"));

        // MandelLambda: z² + c*z*(1-z) hybrid formula
        RegisterTemplate("MandelLambda", () => CreateJuliaTemplate("MandelLambda"));

        // MarksJulia: Julia variant with special initialization
        RegisterTemplate("MarksJulia", () => CreateStandardTemplate("MarksJulia"));

        // Mandelbar: z̄² + c conjugate Mandelbrot
        RegisterTemplate("Mandelbar", () => CreateJuliaTemplate("Mandelbar"));

        // Thorn: z/c + z² + c unique formula
        RegisterTemplate("Thorn", () => CreateJuliaTemplate("Thorn"));

        // Multibrot3, 4, 5: Separate registrations for z³, z⁴, z⁵ (distinct from our z3, z4, z5 aliases)
        RegisterTemplate("Multibrot3", () => CreateMultibrotTemplate("Multibrot3", 3));
        RegisterTemplate("Multibrot4", () => CreateMultibrotTemplate("Multibrot4", 4));
        RegisterTemplate("Multibrot5", () => CreateMultibrotTemplate("Multibrot5", 5));

        // ═══════════════════════════════════════════════════════════════════════════
        // PHASE 4 PRIORITY 1: POWER VARIANTS (9 fractals)
        // Higher-power variations of Mandelbrot, Julia, Burning Ship, and Tricorn
        // ═══════════════════════════════════════════════════════════════════════════

        // Multibrot6, 7, 8: z⁶ + c, z⁷ + c, z⁸ + c (separate from z6/z7/z8 aliases)
        RegisterTemplate("Multibrot6", () => CreateMultibrotTemplate("Multibrot6", 6));
        RegisterTemplate("Multibrot7", () => CreateMultibrotTemplate("Multibrot7", 7));
        RegisterTemplate("Multibrot8", () => CreateMultibrotTemplate("Multibrot8", 8));

        // Julia5, Julia6: z⁵ + c and z⁶ + c Julia preset fractals
        RegisterTemplate("Julia5", () => CreateStandardTemplate("Julia5"));
        RegisterTemplate("Julia6", () => CreateStandardTemplate("Julia6"));

        // BurningShip3, BurningShip4: Burning Ship with cubic and quartic powers
        RegisterTemplate("BurningShip3", () => CreateJuliaTemplate("BurningShip3"));
        RegisterTemplate("BurningShip4", () => CreateJuliaTemplate("BurningShip4"));

        // Tricorn3, Tricorn4: Tricorn (conjugate) with cubic and quartic powers
        RegisterTemplate("Tricorn3", () => CreateJuliaTemplate("Tricorn3"));
        RegisterTemplate("Tricorn4", () => CreateJuliaTemplate("Tricorn4"));

        // ═══════════════════════════════════════════════════════════════════════════
        // SPECIAL: HAILSTONE (custom UI, no parameters)
        // ═══════════════════════════════════════════════════════════════════════════
        RegisterTemplate("Hailstone", () =>
        {
            // Hailstone has no editable parameters - it uses mouse input
            var paramSet = new FractalParameterSet("Hailstone");

            paramSet.AddParameter(new FractalParameterDescriptor
            {
                Key = "info",
                Name = "Information",
                Type = ParameterType.String,
                Category = ParameterCategory.View,
                DefaultValue = "Click the canvas to explore Collatz sequences",
                IsEditable = false,
                Description = "Hailstone fractal is interactive - click points on the canvas",
                DisplayOrder = 1
            });

            return paramSet;
        });

        // ═══════════════════════════════════════════════════════════════════════════
        // PHASE 4 PRIORITY 2: Extended Trigonometric Family (8 fractals)
        // ═══════════════════════════════════════════════════════════════════════════
        // Tangent-based Mandelbrot variants with extended trigonometric functions

        RegisterTemplate("TanMandel", () => CreateStandardTemplate("TanMandel"));
        RegisterTemplate("CotMandel", () => CreateStandardTemplate("CotMandel"));
        RegisterTemplate("SecMandel", () => CreateStandardTemplate("SecMandel"));
        RegisterTemplate("CscMandel", () => CreateStandardTemplate("CscMandel"));
        RegisterTemplate("ArcSinMandel", () => CreateStandardTemplate("ArcSinMandel"));
        RegisterTemplate("ArcCosMandel", () => CreateStandardTemplate("ArcCosMandel"));
        RegisterTemplate("ArcTanMandel", () => CreateStandardTemplate("ArcTanMandel"));
        RegisterTemplate("TanhMandel", () => CreateStandardTemplate("TanhMandel"));

        // ═══════════════════════════════════════════════════════════════════════════
        // PHASE 4 PRIORITY 3: Complex Functions & Special (13 fractals)
        // ═══════════════════════════════════════════════════════════════════════════

        // Complex Functions (6): Hyperbolic variants and special shapes
        RegisterTemplate("SinhMandelbrot", () => CreateJuliaTemplate("SinhMandelbrot"));
        RegisterTemplate("CoshMandelbrot", () => CreateJuliaTemplate("CoshMandelbrot"));
        RegisterTemplate("TanhMandelbrot", () => CreateJuliaTemplate("TanhMandelbrot"));
        RegisterTemplate("HeartMandel", () => CreateJuliaTemplate("HeartMandel"));
        RegisterTemplate("SharkFin", () => CreateJuliaTemplate("SharkFin"));
        RegisterTemplate("Wavy", () => CreateJuliaTemplate("Wavy"));

        // Special Functions (7): Advanced mathematical functions
        RegisterTemplate("GammaFractal", () => CreateJuliaTemplate("GammaFractal"));
        RegisterTemplate("ErrorFunctionFractal", () => CreateJuliaTemplate("ErrorFunctionFractal"));
        RegisterTemplate("BesselLikeFractal", () => CreateJuliaTemplate("BesselLikeFractal"));
        RegisterTemplate("ContinuedFraction", () => CreateJuliaTemplate("ContinuedFraction"));
        RegisterTemplate("Tetration", () => CreateJuliaTemplate("Tetration"));
        RegisterTemplate("LambertW", () => CreateStandardTemplate("LambertW"));
        RegisterTemplate("HyperbolicCombo", () => CreateJuliaTemplate("HyperbolicCombo"));

        // ═══════════════════════════════════════════════════════════════════════════
        // PHASE 4 PRIORITY 4: Exotic Formulas (4 fractals)
        // ═══════════════════════════════════════════════════════════════════════════
        // Unusual and creative fractal formulas with absolute value operations

        RegisterTemplate("CelticMandel", () => CreateJuliaTemplate("CelticMandel"));
        RegisterTemplate("PerpendicularMandel", () => CreateJuliaTemplate("PerpendicularMandel"));
        RegisterTemplate("QuasiPerpendicular", () => CreateJuliaTemplate("QuasiPerpendicular"));
        RegisterTemplate("Zubieta", () => CreateJuliaTemplate("Zubieta"));

        // ═══════════════════════════════════════════════════════════════════════════
        // PHASE 4 PRIORITY 5: Hybrids & Blends (18 fractals)
        // ═══════════════════════════════════════════════════════════════════════════
        // Fractals that combine multiple iteration formulas or alternate between them
        // Source: FractalHybridsFamily.cpp and HybridFamily.cpp

        // Hybrid Fractals Family (8 fractals from FractalHybridsFamily.cpp)
        RegisterTemplate("BurningMandel", () => CreateJuliaTemplate("BurningMandel"));
        RegisterTemplate("ExpMandelHybrid", () => CreateJuliaTemplate("ExpMandelHybrid"));
        RegisterTemplate("MutantMandelbrot", () => CreateJuliaTemplate("MutantMandelbrot"));
        RegisterTemplate("TrigMandelBlend", () => CreateJuliaTemplate("TrigMandelBlend"));
        RegisterTemplate("SierpinskiMandel", () => CreateJuliaTemplate("SierpinskiMandel"));
        RegisterTemplate("PerturbedNewton", () => CreateJuliaTemplate("PerturbedNewton"));
        RegisterTemplate("BifurcationMandel", () => CreateJuliaTemplate("BifurcationMandel"));
        RegisterTemplate("CelticMandelbrot", () => CreateJuliaTemplate("CelticMandelbrot"));

        // Hybrid Family (10 fractals from HybridFamily.cpp)
        RegisterTemplate("MandelBurningHybrid", () => CreateJuliaTemplate("MandelBurningHybrid"));
        RegisterTemplate("MandelLambdaMix", () => CreateJuliaTemplate("MandelLambdaMix"));
        RegisterTemplate("TricornPhoenixHybrid", () => CreateJuliaTemplate("TricornPhoenixHybrid"));
        RegisterTemplate("NewtonMandelBlend", () => CreateJuliaTemplate("NewtonMandelBlend"));
        RegisterTemplate("SineMandelHybrid", () => CreateJuliaTemplate("SineMandelHybrid"));
        RegisterTemplate("ExpMandelBlend", () => CreateJuliaTemplate("ExpMandelBlend"));
        RegisterTemplate("MultiPowerCycle", () => CreateJuliaTemplate("MultiPowerCycle"));
        RegisterTemplate("MagnetMandelHybrid", () => CreateJuliaTemplate("MagnetMandelHybrid"));
        RegisterTemplate("CollatzHybrid", () => CreateJuliaTemplate("CollatzHybrid"));
        RegisterTemplate("CelticBurningHybrid", () => CreateJuliaTemplate("CelticBurningHybrid"));

        // ═══════════════════════════════════════════════════════════════════════════
        // PHASE 4 PRIORITY 6: Orbital & Distance Estimators (12 fractals)
        // ═══════════════════════════════════════════════════════════════════════════
        // Fractals with orbit modifications and distance estimation techniques
        // Source: OrbitalFractalsFamily.cpp and DistanceEstimatorFamily.cpp

        // Orbital Fractals (8 fractals) - Mandelbrot with orbit trapping and modifications
        RegisterTemplate("OrbitTrapCross", () => CreateStandardTemplate("OrbitTrapCross"));
        RegisterTemplate("OrbitTrapCircle", () => CreateStandardTemplate("OrbitTrapCircle"));
        RegisterTemplate("OrbitTrapPoint", () => CreateStandardTemplate("OrbitTrapPoint"));
        RegisterTemplate("OrbitTrapSquare", () => CreateStandardTemplate("OrbitTrapSquare"));
        RegisterTemplate("AverageDistance", () => CreateStandardTemplate("AverageDistance"));
        RegisterTemplate("MinimumDistance", () => CreateStandardTemplate("MinimumDistance"));
        RegisterTemplate("MaximumDistance", () => CreateStandardTemplate("MaximumDistance"));
        RegisterTemplate("AngleAverage", () => CreateStandardTemplate("AngleAverage"));

        // Distance Estimators (4 fractals) - Precise boundary visualization with derivative tracking
        // MandelbrotDEM: center=-0.5, zoom=1.0
        // JuliaDEM: center=0.0, zoom=1.5, supports Julia mode
        // BurningShipDEM: center=-0.5,-0.5, zoom=0.4
        // TricornDEM: center=0.0, zoom=1.0
        RegisterTemplate("MandelbrotDEM", () => CreateStandardTemplate("MandelbrotDEM"));
        RegisterTemplate("JuliaDEM", () => CreateJuliaTemplate("JuliaDEM"));
        RegisterTemplate("BurningShipDEM", () => CreateStandardTemplate("BurningShipDEM"));
        RegisterTemplate("TricornDEM", () => CreateStandardTemplate("TricornDEM"));

        // ═══════════════════════════════════════════════════════════════════════════
        // PHASE 4 PRIORITY 7: IFS (Iterated Function Systems) - Additional (2 fractals)
        // ═══════════════════════════════════════════════════════════════════════════
        // Additional IFS fractals beyond Phase 3 Step 3C (BarnsleyFern, SierpinskiIFS, DragonCurveIFS)
        // Source: IFSFamily.cpp

        // PentagonIFS: Chaos game with 5 vertices (center 0,0; zoom 1.5)
        RegisterTemplate("PentagonIFS", () => CreateStandardTemplate("PentagonIFS"));

        // TreeIFS: Branching tree structure (center 0, 2; zoom 0.3)
        RegisterTemplate("TreeIFS", () => CreateStandardTemplate("TreeIFS"));

        // ═══════════════════════════════════════════════════════════════════════════
        // PHASE 4 PRIORITY 8: CHAOTIC MAPS & BIFURCATION DIAGRAMS (7 fractals)
        // ═══════════════════════════════════════════════════════════════════════════
        // Chaotic dynamical systems and parameter space visualizations
        // Source: ChaoticMapsFamily.cpp (1 fractal), BifurcationFamily.cpp (6 fractals)

        // SprottB: Minimalist chaotic attractor with elegant simplicity (center 0,0; zoom 0.15625)
        // Formula: dx/dt = yz, dy/dt = x - y, dz/dt = 1 - xy
        RegisterTemplate("SprottB", () => CreateStandardTemplate("SprottB"));

        // Bifurcation Diagrams & Parameter Space Visualizations
        // These fractals visualize parameter stability and periodic behavior

        // LogisticParameterSpace: Parameter space for logistic map xₙ₊₁ = r·xₙ·(1-xₙ) (center 2,0; zoom 0.697)
        RegisterTemplate("LogisticParameterSpace", () => CreateStandardTemplate("LogisticParameterSpace"));

        // LambdaParameterSpace: Complex lambda map z = λ·z·(1-z) parameter space (center 1,0; zoom 0.536203)
        RegisterTemplate("LambdaParameterSpace", () => CreateStandardTemplate("LambdaParameterSpace"));

        // MandelParameter: Mandelbrot parameter space showing periodicity for z² + c (center 0,0; zoom 1.0)
        RegisterTemplate("MandelParameter", () => CreateStandardTemplate("MandelParameter"));

        // HenonParameterSpace: Hénon map xₙ₊₁ = 1 - a·xₙ² + yₙ parameter space (center 0.75,-0.25; zoom 1.0)
        RegisterTemplate("HenonParameterSpace", () => CreateStandardTemplate("HenonParameterSpace"));

        // OrbitDiagram: Orbit trajectory visualization for z² + c (center 0,0; zoom 1.0)
        RegisterTemplate("OrbitDiagram", () => CreateStandardTemplate("OrbitDiagram"));

        // MayLyapunovRef: Lyapunov exponent visualization for May logistic map (center 2,0; zoom 0.3)
        RegisterTemplate("MayLyapunovRef", () => CreateStandardTemplate("MayLyapunovRef"));

        // ═══════════════════════════════════════════════════════════════════════════
        // PHASE 4 PRIORITY 9: HISTORICAL & RESEARCH FRACTALS (4 fractals)
        // ═══════════════════════════════════════════════════════════════════════════
        // Classic fractals from early fractal research and lesser-known discoveries
        // Source: HistoricalFractalsFamily.cpp
        // Note: Biomorphs, PickoverStalks already registered in Phase 3
        // Note: CollatzFractal registered as "Hailstone" in Phase 1
        // Note: DuffingMap registered as "Duffing" in Phase 3A attractors

        // MartinMap: Barry Martin's hopalong variant with sqrt operations (center 0,0; zoom 0.5)
        // Histogram-based discrete map creating organic flowing patterns
        // Formula: x' = y - sgn(x)·√|bx - c|, y' = a - x
        RegisterTemplate("MartinMap", () => CreateStandardTemplate("MartinMap"));

        // ChipMap: Pickover's silicon chip-like patterns with modulo operation (center 0,0; zoom 0.3)
        // Julia-enabled escape-time fractal with rectangular circuit board appearance
        // Formula: z(n+1) = (z² + c) mod 2π
        RegisterTemplate("ChipMap", () => CreateJuliaTemplate("ChipMap"));

        // QuaternionJulia2D: 2D projection of 4D quaternion Julia set (center 0,0; zoom 0.6)
        // Julia-enabled escape-time with quaternion multiplication creating 3D-like depth
        // Formula: q(n+1) = q² + c (quaternion), project to (x,y)
        RegisterTemplate("QuaternionJulia2D", () => CreateJuliaTemplate("QuaternionJulia2D"));

        // SinusoidalFractal: Early transcendental fractal with sine iteration (center 0,0; zoom 0.2)
        // Julia-enabled escape-time creating wavy bands from sine periodicity
        // Formula: z(n+1) = c·sin(z)
        RegisterTemplate("SinusoidalFractal", () => CreateJuliaTemplate("SinusoidalFractal"));

        // ═══════════════════════════════════════════════════════════════════════════
        // PHASE 4 PRIORITY 10: POLYNOMIAL VARIANTS FAMILY (8 fractals)
        // ═══════════════════════════════════════════════════════════════════════════
        // Polynomial-based fractal formulas with various powers and rational functions
        // Source: PolynomialVariantsFamily.cpp

        // CubicMandel: Mandelbrot with cubic iteration (center 0,0; zoom 1.2)
        // Standard escape-time fractal, no Julia support, threefold rotational symmetry
        // Formula: z(n+1) = z³ + c
        RegisterTemplate("CubicMandel", () => CreateStandardTemplate("CubicMandel"));

        // QuarticMandel: Mandelbrot with quartic iteration (center 0,0; zoom 1.3)
        // Standard escape-time fractal, no Julia support, fourfold rotational symmetry
        // Formula: z(n+1) = z⁴ + c
        RegisterTemplate("QuarticMandel", () => CreateStandardTemplate("QuarticMandel"));

        // QuinticMandel: Mandelbrot with quintic iteration (center 0,0; zoom 1.4)
        // Standard escape-time fractal, no Julia support, fivefold rotational symmetry
        // Formula: z(n+1) = z⁵ + c
        RegisterTemplate("QuinticMandel", () => CreateStandardTemplate("QuinticMandel"));

        // SexticMandel: Mandelbrot with sextic (6th power) iteration (center 0,0; zoom 1.5)
        // Standard escape-time fractal, no Julia support, sixfold rotational symmetry
        // Formula: z(n+1) = z⁶ + c
        RegisterTemplate("SexticMandel", () => CreateStandardTemplate("SexticMandel"));

        // RationalR1: Rational map (z²+c)/(z²+1) (center 0,0; zoom 1.5)
        // Rational function fractal with poles, asymmetric behavior
        // Formula: z(n+1) = (z²+c)/(z²+1)
        RegisterTemplate("RationalR1", () => CreateStandardTemplate("RationalR1"));

        // PolyZ3MinusZ: Polynomial z³-z+c (center 0,0; zoom 1.5)
        // Mixed-degree polynomial creating hybrid escape dynamics
        // Formula: z(n+1) = z³ - z + c
        RegisterTemplate("PolyZ3MinusZ", () => CreateStandardTemplate("PolyZ3MinusZ"));

        // PolyZ4PlusZ3: Polynomial z⁴+z³+c (center 0,0; zoom 1.5)
        // Mixed-degree polynomial with combined cubic and quartic terms
        // Formula: z(n+1) = z⁴ + z³ + c
        RegisterTemplate("PolyZ4PlusZ3", () => CreateStandardTemplate("PolyZ4PlusZ3"));

        // Biomorph: Organism-like fractal with special bailout (center 0,0; zoom 0.5)
        // Modified Mandelbrot using component-wise bailout: |Re(z)| > B or |Im(z)| > B
        // Formula: z(n+1) = z² + c (bailout = 10.0)
        RegisterTemplate("Biomorph", () => CreateStandardTemplate("Biomorph"));

        // ═══════════════════════════════════════════════════════════════════════════
        // PHASE 4 PRIORITY 11: CLASSIC ESCAPE-TIME & EXTENDED FAMILIES (17 fractals)
        // ═══════════════════════════════════════════════════════════════════════════
        // Remaining fractals from ClassicEscapeTimeFamily, StrangeAttractorsExtended, and ExtendedJulia
        // These are established fractal types that complete core family coverage

        // ─────────────────────────────────────────────────────────────────────────────
        // Classic Escape-Time Family (10 fractals)
        // Source: ClassicEscapeTimeFamily.cpp
        // ─────────────────────────────────────────────────────────────────────────────

        // Manowar: z = z² + z_prev + c (center 0,0; zoom 1.0)
        // Escape-time with memory of previous iteration, Julia-enabled
        // Formula: z(n+1) = z(n)² + z(n-1) + c
        RegisterTemplate("Manowar", () => CreateJuliaTemplate("Manowar"));

        // Sierpinski: z = 2·z·(1-z) (center 0.5,0.5; zoom 1.5)
        // Classic Sierpinski triangle using escape-time iteration
        // Formula: z(n+1) = 2·z(n)·(1-z(n))
        RegisterTemplate("Sierpinski", () => CreateStandardTemplate("Sierpinski"));

        // Unity: z = z² + 1/c (center 0,0; zoom 1.0)
        // Circle inversion fractal, no Julia support
        // Formula: z(n+1) = z² + 1/c
        RegisterTemplate("Unity", () => CreateStandardTemplate("Unity"));

        // Spider: z = z² + c, c = c/2 + z (center 0,0; zoom 1.0)
        // Evolving constant where c changes with each iteration, Julia-enabled
        // Formula: z(n+1) = z² + c; c(n+1) = c(n)/2 + z(n+1)
        RegisterTemplate("Spider", () => CreateJuliaTemplate("Spider"));

        // Tetrate: z = c^z (center 0,0; zoom 1.0)
        // Tetration (infinite power tower), Julia-enabled
        // Formula: z(n+1) = c^z(n)
        RegisterTemplate("Tetrate", () => CreateJuliaTemplate("Tetrate"));

        // HeartMandelbrot: z² + c + sin(z) (center 0,0; zoom 1.5)
        // Heart-shaped variation with sine term, Julia-enabled
        // Formula: z(n+1) = z² + c + sin(z)
        RegisterTemplate("HeartMandelbrot", () => CreateJuliaTemplate("HeartMandelbrot"));

        // SharkFinMandelbrot: z² + c/z (center 0,0; zoom 1.5)
        // Shark fin variation with division, Julia-enabled
        // Formula: z(n+1) = z² + c/z
        RegisterTemplate("SharkFinMandelbrot", () => CreateJuliaTemplate("SharkFinMandelbrot"));

        // PartialBurningShip: re² + i·|im|² + c (center -0.25,0; zoom 0.75)
        // Partial absolute value application, Julia-enabled
        // Formula: z(n+1) = re² + i·|im|² + c
        RegisterTemplate("PartialBurningShip", () => CreateJuliaTemplate("PartialBurningShip"));

        // BirdOfPrey: |re|² + i·im² + c (center 0,0; zoom 1.5)
        // Burning Ship variant with real abs only, Julia-enabled
        // Formula: z(n+1) = |re|² + i·im² + c
        // Note: Already registered earlier, but keeping for completeness

        // CelticHeart: |re| + i·im, then z² + sin(z) + c (center 0,0; zoom 1.5)
        // Celtic absolute value combined with heart formula, Julia-enabled
        // Formula: z(n+1) = (|re| + i·im)² + sin(z) + c
        RegisterTemplate("CelticHeart", () => CreateJuliaTemplate("CelticHeart"));

        // WavyMandelbrot: z² + c + 0.1·sin(z) (center 0,0; zoom 1.5)
        // Wavy variation with scaled sine term, Julia-enabled
        // Formula: z(n+1) = z² + c + 0.1·sin(z)
        RegisterTemplate("WavyMandelbrot", () => CreateJuliaTemplate("WavyMandelbrot"));

        // ─────────────────────────────────────────────────────────────────────────────
        // Strange Attractors Extended Family (2 fractals)
        // Source: StrangeAttractorsExtendedFamily.cpp
        // ─────────────────────────────────────────────────────────────────────────────

        // Svensson: Johnny Svensson attractor (center 0,0; zoom 1.0)
        // 2D discrete map: x' = d·sin(a·x) - sin(b·y); y' = c·cos(a·x) + cos(b·y)
        // Histogram-based rendering with intricate patterns
        RegisterTemplate("Svensson", () => CreateStandardTemplate("Svensson"));

        // Bedhead: Ivan Emathajuet Khatsanov attractor (center 0,0; zoom 1.0)
        // 2D discrete map: x' = sin(x·y/b)·y + cos(a·x - y); y' = x + sin(y)/b
        // Chaotic point cloud with unique structure
        RegisterTemplate("Bedhead", () => CreateStandardTemplate("Bedhead"));

        // ─────────────────────────────────────────────────────────────────────────────
        // Extended Julia Family (5 fractals)
        // Source: ExtendedJuliaFamily.cpp
        // ─────────────────────────────────────────────────────────────────────────────

        // JuliaSiegelDisk: Julia set at golden ratio point (center 0,0; zoom 1.0)
        // Fixed c ≈ -0.390541 - 0.586788i (Siegel disk constant)
        // Formula: z(n+1) = z² + c where c = e^(2πiφ)
        RegisterTemplate("JuliaSiegelDisk", () => CreateStandardTemplate("JuliaSiegelDisk"));

        // JuliaCustom: Julia set with user-defined c (center 0,0; zoom 1.0)
        // Allows user customization of Julia constant, Julia-enabled
        // Formula: z(n+1) = z² + c (c is user-specified)
        RegisterTemplate("JuliaCustom", () => CreateJuliaTemplate("JuliaCustom"));

        // LambdaJulia: Lambda Julia set (center 0,0; zoom 2.0)
        // Julia set for lambda iteration, Julia-enabled
        // Formula: z(n+1) = c·z·(1-z)
        RegisterTemplate("LambdaJulia", () => CreateJuliaTemplate("LambdaJulia"));

        // Multibrot3Julia: z³ + c Julia set (center 0,0; zoom 1.5)
        // Cubic power Julia variant, Julia-enabled
        // Formula: z(n+1) = z³ + c
        RegisterTemplate("Multibrot3Julia", () => CreateJuliaTemplate("Multibrot3Julia"));

        // Multibrot4Julia: z⁴ + c Julia set (center 0,0; zoom 1.5)
        // Quartic power Julia variant, Julia-enabled
        // Formula: z(n+1) = z⁴ + c
        RegisterTemplate("Multibrot4Julia", () => CreateJuliaTemplate("Multibrot4Julia"));

        // ═══════════════════════════════════════════════════════════════════════════
        // PHASE 4 PRIORITY 12: ADVANCED TECHNIQUES & EXTENSIONS (19 fractals)
        // ═══════════════════════════════════════════════════════════════════════════
        // Final batch: Orbital modifications, Phoenix/Magnet/Lambda extensions

        // ─────────────────────────────────────────────────────────────────────────────
        // Orbital Modifications Family (10 fractals)
        // Source: OrbitalModificationsFamily.cpp
        // Advanced orbital techniques with traps and path modifications
        // ─────────────────────────────────────────────────────────────────────────────

        // CircularOrbitTrap: Mandelbrot with circular trap at origin (center 0,0; zoom 0.8)
        // Colors by minimum distance to trap circle, Julia-enabled
        // Formula: z(n+1) = z² + c, color by min|z - trap|
        RegisterTemplate("CircularOrbitTrap", () => CreateJuliaTemplate("CircularOrbitTrap"));

        // CrossOrbitTrap: Orbit trap on coordinate axes (center 0,0; zoom 0.8)
        // Creates cruciform patterns, Julia-enabled
        // Formula: z(n+1) = z² + c, trap on x=0 or y=0
        RegisterTemplate("CrossOrbitTrap", () => CreateJuliaTemplate("CrossOrbitTrap"));

        // StalksConditional: Conditional formula based on magnitude (center 0,0; zoom 0.8)
        // Stalk-like protrusions, Julia-enabled
        // Formula: if |z| < r: z² + c, else: z³ + c
        RegisterTemplate("StalksConditional", () => CreateJuliaTemplate("StalksConditional"));

        // SmoothedOrbit: Averaging of orbit trajectory (center 0,0; zoom 0.8)
        // Smooth orbital path visualization, Julia-enabled
        // Formula: z(n+1) = z² + c, averaged orbit
        RegisterTemplate("SmoothedOrbit", () => CreateJuliaTemplate("SmoothedOrbit"));

        // OrbitAngleAccum: Accumulated angle changes in orbit (center 0,0; zoom 0.8)
        // Reveals rotation patterns, Julia-enabled
        // Formula: z(n+1) = z² + c, track Σangle
        RegisterTemplate("OrbitAngleAccum", () => CreateJuliaTemplate("OrbitAngleAccum"));

        // TriangleOrbitTrap: Triangle-shaped orbit trap (center 0,0; zoom 0.8)
        // Geometric trap variant, Julia-enabled
        // Formula: z(n+1) = z² + c, trap on triangle
        RegisterTemplate("TriangleOrbitTrap", () => CreateJuliaTemplate("TriangleOrbitTrap"));

        // StripeAverage: Average distance from horizontal stripes (center 0,0; zoom 0.8)
        // Creates banded patterns, Julia-enabled
        // Formula: z(n+1) = z² + c, stripe trap
        RegisterTemplate("StripeAverage", () => CreateJuliaTemplate("StripeAverage"));

        // CurvatureTracking: Track orbit path curvature (center 0,0; zoom 0.8)
        // Reveals trajectory bending, Julia-enabled
        // Formula: z(n+1) = z² + c, track curvature
        RegisterTemplate("CurvatureTracking", () => CreateJuliaTemplate("CurvatureTracking"));

        // DeltaMagnitude: Track magnitude changes (center 0,0; zoom 0.8)
        // Reveals growth rate patterns, Julia-enabled
        // Formula: z(n+1) = z² + c, track Δ|z|
        RegisterTemplate("DeltaMagnitude", () => CreateJuliaTemplate("DeltaMagnitude"));

        // PointLineOrbitTrap: Distance to point and line traps (center 0,0; zoom 0.8)
        // Combined point/line trap geometry, Julia-enabled
        // Formula: z(n+1) = z² + c, dual trap
        RegisterTemplate("PointLineOrbitTrap", () => CreateJuliaTemplate("PointLineOrbitTrap"));

        // ─────────────────────────────────────────────────────────────────────────────
        // Phoenix Extended Family (3 fractals)
        // Source: PhoenixExtendedFamily.cpp
        // Phoenix fractals with advanced functions and parameters
        // ─────────────────────────────────────────────────────────────────────────────

        // PhoenixCosh: Phoenix with hyperbolic cosine (center 0,0; zoom 1.2)
        // cosh(z) + c + p·z_prev, Julia-enabled
        // Formula: z(n+1) = cosh(z) + c + p·z(n-1)
        RegisterTemplate("PhoenixCosh", () => CreateJuliaTemplate("PhoenixCosh"));

        // PhoenixComplex: Phoenix with complex feedback parameter (center 0,0; zoom 0.7)
        // z² + c + (0.5+0.2i)·z_prev, Julia-enabled
        // Formula: z(n+1) = z² + c + (0.5+0.2i)·z(n-1)
        RegisterTemplate("PhoenixComplex", () => CreateJuliaTemplate("PhoenixComplex"));

        // PhoenixLambda: Hybrid Phoenix-Lambda (center 0,0; zoom 1.0)
        // c·z·(1-z) + p·z_prev, Julia-enabled
        // Formula: z(n+1) = c·z(1-z) + p·z(n-1)
        RegisterTemplate("PhoenixLambda", () => CreateJuliaTemplate("PhoenixLambda"));

        // ─────────────────────────────────────────────────────────────────────────────
        // Magnet Extended Family (2 fractals)
        // Source: MagnetExtendedFamily.cpp
        // Magnet fractals with cubic power variants
        // ─────────────────────────────────────────────────────────────────────────────

        // Magnet1Power3: Magnet I with cubic power (center 0.5,0; zoom 0.7)
        // ((z²+c-1)/(2z+c-2))³, Julia-enabled
        // Formula: z(n+1) = [(z²+c-1)/(2z+c-2)]³
        RegisterTemplate("Magnet1Power3", () => CreateJuliaTemplate("Magnet1Power3"));

        // Magnet2Power3: Magnet II with cubic power (center 1.5,0; zoom 0.6)
        // ((z³+3(c-1)z+(c-1)(c-2))/(3z²+3(c-2)z+(c-1)(c-2)+1))³, Julia-enabled
        // Formula: z(n+1) = [complex rational]³
        RegisterTemplate("Magnet2Power3", () => CreateJuliaTemplate("Magnet2Power3"));

        // ─────────────────────────────────────────────────────────────────────────────
        // Lambda Extended Family (2 fractals)
        // Source: LambdaExtendedFamily.cpp
        // Lambda fractals with modifications and Phoenix hybrid
        // ─────────────────────────────────────────────────────────────────────────────

        // LambdaModified: Modified Lambda with feedback (center 0,0; zoom 1.0)
        // λ·z·(1-z) + z, Julia-enabled
        // Formula: z(n+1) = λ·z·(1-z) + z
        RegisterTemplate("LambdaModified", () => CreateJuliaTemplate("LambdaModified"));

        // LambdaPhoenix: Lambda with Phoenix-style memory (center 0,0; zoom 1.0)
        // λ·z·(1-z) + p·z_prev, Julia-enabled
        // Formula: z(n+1) = λ·z·(1-z) + p·z(n-1)
        RegisterTemplate("LambdaPhoenix", () => CreateJuliaTemplate("LambdaPhoenix"));

        // ─────────────────────────────────────────────────────────────────────────────
        // Rational Function Family (1 fractal)
        // Source: RationalFunctionFamily.cpp
        // Newton method for cubic roots
        // ─────────────────────────────────────────────────────────────────────────────

        // NewtonCubic: Newton method for z³-1=0 (center 0,0; zoom 0.6)
        // Classic Newton fractal with three convergence basins
        // Formula: z(n+1) = z - (z³-1)/(3z²)
        RegisterTemplate("NewtonCubic", () => CreateStandardTemplate("NewtonCubic"));

        // ─────────────────────────────────────────────────────────────────────────────
        // Special Exotic Family (1 fractal)
        // Source: SpecialExoticFamily.cpp
        // 2D Collatz sequence visualization
        // ─────────────────────────────────────────────────────────────────────────────

        // Hailstone2D: 2D Collatz conjecture visualization (center 27,0; zoom 1.0)
        // Visualizes Collatz sequence starting points
        // Formula: if even: n/2, if odd: 3n+1
        RegisterTemplate("Hailstone2D", () => CreateStandardTemplate("Hailstone2D"));

        // ═══════════════════════════════════════════════════════════════════════════
        // FINAL RECONCILIATION: Last 3 fractals (279/279 complete)
        // ═══════════════════════════════════════════════════════════════════════════

        // Multibrot-10: z¹⁰ + c decic polynomial (center 0,0; zoom 1.5)
        // Tenth-order Mandelbrot variant with ten-fold rotational symmetry
        // Formula: z(n+1) = z¹⁰ + c
        RegisterTemplate("Multibrot-10", () => CreateJuliaTemplate("Multibrot-10"));

        // JuliaSanMarco: Named Julia preset with fixed c (center 0,0; zoom 0.5)
        // Pre-set Julia constant, no Julia toggle
        // Formula: z² + c where c is fixed at classic value
        RegisterTemplate("JuliaSanMarco", () => CreateStandardTemplate("JuliaSanMarco"));

        // JuliaDouadyRabbit: Named Julia preset with fixed c (center 0,0; zoom 0.5)
        // Pre-set Julia constant (Douady's rabbit), no Julia toggle
        // Formula: z² + c where c is fixed at Douady rabbit value
        RegisterTemplate("JuliaDouadyRabbit", () => CreateStandardTemplate("JuliaDouadyRabbit"));

        // ═══════════════════════════════════════════════════════════════════════════
        // FALLBACK: Generic escape-time template for unknown fractals
        // ═══════════════════════════════════════════════════════════════════════════
        // Any fractal not explicitly registered will use this as a fallback
        // This ensures the app never crashes due to missing parameter definitions
    }

    /// <summary>
    /// Register a parameter template factory for a fractal type.
    /// </summary>
    private void RegisterTemplate(string fractalType, Func<FractalParameterSet> factory)
    {
        _parameterTemplates[fractalType] = factory;
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // PUBLIC API
    // ═══════════════════════════════════════════════════════════════════════════════

    public async Task<FractalParameterSet?> GetParametersAsync(string fractalType)
    {
        if (!_initialized)
            await InitializeAsync();

        if (string.IsNullOrWhiteSpace(fractalType))
            return null;

        // Priority 1: Try native parameter metadata (Task 8 integration)
        var nativeParams = LoadParametersFromNative(fractalType);
        if (nativeParams != null)
        {
            Debug.WriteLine($"[FractalParameterService] Loaded {nativeParams.Parameters.Count} parameters from native registry for '{fractalType}'");

            // Apply saved overrides
            var overrides = await LoadParameterOverridesAsync(fractalType);
            if (overrides.Count > 0)
            {
                Debug.WriteLine($"[FractalParameterService] Applying {overrides.Count} saved parameter overrides for '{fractalType}'");
                nativeParams.ImportValues(overrides);
            }

            return nativeParams;
        }

        // Priority 2: Try registered template (fallback)
        if (_parameterTemplates.TryGetValue(fractalType, out var factory))
        {
            Debug.WriteLine($"[FractalParameterService] Using registered C# template for '{fractalType}'");
            var paramSet = factory();

            // Apply saved overrides
            var overrides = await LoadParameterOverridesAsync(fractalType);
            if (overrides.Count > 0)
            {
                Debug.WriteLine($"[FractalParameterService] Applying {overrides.Count} saved parameter overrides for '{fractalType}'");
                paramSet.ImportValues(overrides);
            }

            return paramSet;
        }

        // Priority 3: Fallback - create generic escape-time parameter set for unknown fractals
        Debug.WriteLine($"[FractalParameterService] No native or C# template for '{fractalType}', using generic fallback");
        var fallbackSet = StandardParameterTemplates.CreateStandardEscapeTime(fractalType);

        return fallbackSet;
    }

    /// <summary>
    /// Load parameter metadata from native C++ registry (Task 8).
    /// Returns null if fractal not found or has no parameters.
    /// </summary>
    private FractalParameterSet? LoadParametersFromNative(string fractalType)
    {
        try
        {
            Debug.WriteLine($"[FractalParameterService] Attempting to load native parameters for '{fractalType}'...");

            // Call native registry to get parameters
            var nativeParams = ManpCore.Native.FractalRegistryWrapper.GetParameters(fractalType);

            Debug.WriteLine($"[FractalParameterService] Native returned {nativeParams?.Count ?? 0} parameters for '{fractalType}'");

            if (nativeParams == null || nativeParams.Count == 0)
            {
                Debug.WriteLine($"[FractalParameterService] No native parameters found for '{fractalType}'");
                return null;
            }

            var paramSet = new FractalParameterSet(fractalType);

            // Convert each native ParameterInfo to FractalParameterDescriptor
            foreach (var nativeParam in nativeParams)
            {
                Debug.WriteLine($"[FractalParameterService]   - {nativeParam.Name} ({nativeParam.DisplayName}): {nativeParam.DefaultValue}");

                // For Integer types, convert double bounds to int bounds
                object? minValue = nativeParam.MinValue;
                object? maxValue = nativeParam.MaxValue;
                if (nativeParam.Type == ManpCore.Native.ManagedParameterType.Integer)
                {
                    minValue = (int)nativeParam.MinValue;
                    maxValue = (int)nativeParam.MaxValue;
                }

                var descriptor = new FractalParameterDescriptor
                {
                    Key = nativeParam.Name,
                    Name = nativeParam.DisplayName,
                    Description = nativeParam.Description,
                    Type = MapNativeParameterType(nativeParam.Type),
                    Category = MapNativeParameterCategory(nativeParam.Category),
                    DefaultValue = ParseNativeDefaultValue(nativeParam.DefaultValue, nativeParam.Type),
                    MinValue = minValue,
                    MaxValue = maxValue,
                    StepSize = nativeParam.Step,
                    FormatString = nativeParam.FormatString,
                    Unit = nativeParam.Unit,
                    DisplayOrder = nativeParam.DisplayOrder
                };

                // Handle choice values if present
                if (nativeParam.ChoiceValues != null && nativeParam.ChoiceValues.Count > 0)
                {
                    // Choice type parameters would need special handling here
                    // For now, just add the descriptor
                }

                paramSet.AddParameter(descriptor);
            }

            Debug.WriteLine($"[FractalParameterService] Successfully created parameter set with {paramSet.Parameters.Count} parameters from native registry");
            return paramSet;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[FractalParameterService] ERROR loading native parameters for '{fractalType}': {ex.Message}");
            Debug.WriteLine($"[FractalParameterService] Stack trace: {ex.StackTrace}");
            return null;
        }
    }

    /// <summary>
    /// Map native ParameterType to C# ParameterType
    /// </summary>
    private ParameterType MapNativeParameterType(ManpCore.Native.ManagedParameterType nativeType)
    {
        return nativeType switch
        {
            ManpCore.Native.ManagedParameterType.Integer => ParameterType.Integer,
            ManpCore.Native.ManagedParameterType.Float => ParameterType.Double,
            ManpCore.Native.ManagedParameterType.Boolean => ParameterType.Boolean,
            ManpCore.Native.ManagedParameterType.Choice => ParameterType.Choice,
            ManpCore.Native.ManagedParameterType.Complex => ParameterType.Complex,
            _ => ParameterType.Double
        };
    }

    /// <summary>
    /// Map native ParameterCategory to C# ParameterCategory
    /// </summary>
    private ParameterCategory MapNativeParameterCategory(ManpCore.Native.ManagedParameterCategory nativeCategory)
    {
        return nativeCategory switch
        {
            ManpCore.Native.ManagedParameterCategory.General => ParameterCategory.FractalSpecific,
            ManpCore.Native.ManagedParameterCategory.Calculation => ParameterCategory.Algorithm,
            ManpCore.Native.ManagedParameterCategory.View => ParameterCategory.View,
            ManpCore.Native.ManagedParameterCategory.Color => ParameterCategory.Color,
            ManpCore.Native.ManagedParameterCategory.Advanced => ParameterCategory.Advanced,
            _ => ParameterCategory.FractalSpecific
        };
    }

    /// <summary>
    /// Parse native default value string to appropriate type
    /// </summary>
    private object ParseNativeDefaultValue(string defaultValue, ManpCore.Native.ManagedParameterType type)
    {
        if (string.IsNullOrEmpty(defaultValue))
            return 0.0;

        return type switch
        {
            ManpCore.Native.ManagedParameterType.Integer => int.TryParse(defaultValue, out var i) ? i : 0,
            ManpCore.Native.ManagedParameterType.Float => double.TryParse(defaultValue, out var d) ? d : 0.0,
            ManpCore.Native.ManagedParameterType.Boolean => bool.TryParse(defaultValue, out var b) && b,
            _ => defaultValue
        };
    }

    public bool UpdateParameter(FractalParameterSet paramSet, string key, object value)
    {
        if (paramSet == null)
            throw new ArgumentNullException(nameof(paramSet));

        return paramSet.SetValue(key, value);
    }

    public Task<Dictionary<string, object>> LoadParameterOverridesAsync(string fractalType)
    {
        try
        {
            var json = _settingsService.GetFractalParameters(fractalType);

            if (string.IsNullOrWhiteSpace(json))
                return Task.FromResult(new Dictionary<string, object>());

            var overrides = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            if (overrides == null)
                return Task.FromResult(new Dictionary<string, object>());

            // Convert JsonElement values to appropriate types
            var result = new Dictionary<string, object>();
            foreach (var kvp in overrides)
            {
                result[kvp.Key] = ConvertJsonElement(kvp.Value);
            }

            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[FractalParameterService] Failed to load parameter overrides for '{fractalType}': {ex.Message}");
            return Task.FromResult(new Dictionary<string, object>());
        }
    }

    public Task SaveParameterOverridesAsync(string fractalType, Dictionary<string, object> values)
    {
        try
        {
            if (values == null || values.Count == 0)
            {
                // Clear saved overrides
                _settingsService.SetFractalParameters(fractalType, string.Empty);
                Debug.WriteLine($"[FractalParameterService] Cleared parameter overrides for '{fractalType}'");
                return Task.CompletedTask;
            }

            // Serialize to JSON
            var json = JsonSerializer.Serialize(values, new JsonSerializerOptions
            {
                WriteIndented = false
            });

            _settingsService.SetFractalParameters(fractalType, json);

            Debug.WriteLine($"[FractalParameterService] Saved {values.Count} parameter overrides for '{fractalType}'");
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[FractalParameterService] Failed to save parameter overrides for '{fractalType}': {ex.Message}");
            return Task.CompletedTask;
        }
    }

    public bool ValidateParameterSet(FractalParameterSet paramSet)
    {
        if (paramSet == null)
            throw new ArgumentNullException(nameof(paramSet));

        return paramSet.Validate();
    }

    public bool HasCustomParameters(string fractalType)
    {
        return _parameterTemplates.ContainsKey(fractalType);
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // HELPER METHODS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// ONE-TIME FIX: Clear saved ExpSquare MaxIterations setting if it's 5000.
    /// This was accidentally saved too high and needs to be reset to 1000.
    /// Remove this method after all users have run the app once with this fix.
    /// </summary>
    private void ClearExpSquareSettingsIfNeeded()
    {
        try
        {
            // Use the settings service which works for both MSIX and portable modes
            var savedParams = _settingsService.GetFractalParameters("ExpSquare");

            if (!string.IsNullOrEmpty(savedParams))
            {
                Debug.WriteLine("[FractalParameterService] Found saved ExpSquare parameters - clearing to apply new 1000 default");
                _settingsService.SetFractalParameters("ExpSquare", null!); // Clear the setting
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[FractalParameterService] Error clearing ExpSquare settings: {ex.Message}");
        }
    }

    /// <summary>
    /// Convert JsonElement to appropriate .NET type.
    /// Used when deserializing saved parameter values.
    /// </summary>
    private static object ConvertJsonElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? string.Empty,
            JsonValueKind.Number => element.TryGetInt32(out var intVal) ? intVal : element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Array => element.EnumerateArray().Select(ConvertJsonElement).ToArray(),
            JsonValueKind.Object => element.EnumerateObject()
                .ToDictionary(prop => prop.Name, prop => ConvertJsonElement(prop.Value)),
            _ => element.ToString() ?? string.Empty
        };
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // FUTURE: NATIVE REGISTRY INTEGRATION
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Load parameter definitions from native registry (Task 9).
    /// This will be implemented after native FractalSpec.parameters is populated.
    /// </summary>
    private async Task LoadFromNativeRegistryAsync()
    {
        // TODO: Task 9 - Query ManpCore.Native.FractalRegistryWrapper for parameter metadata
        // For each registered fractal:
        //   1. Get FractalSpec from native
        //   2. If spec.parameters is not empty:
        //      a. Convert native ParameterSpec[] to FractalParameterDescriptor[]
        //      b. Create parameter set factory
        //      c. Register template (override built-in if exists)
        //
        // This makes the system fully data-driven - no C# code changes needed for new fractals

        await Task.CompletedTask; // Placeholder
    }

    /// <summary>
    /// Load custom parameters for a specific fractal from native registry.
    /// Used as fallback when no built-in template exists.
    /// </summary>
    private async Task LoadCustomParametersFromNativeAsync(string fractalType, FractalParameterSet paramSet)
    {
        // TODO: Task 9 - Query native registry for this specific fractal
        // If spec.parameters is populated, add those descriptors to paramSet

        await Task.CompletedTask; // Placeholder
    }
}
