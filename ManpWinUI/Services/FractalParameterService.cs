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
