using System.Collections.Generic;

namespace ManpWinUI.Models.Parameters;

/// <summary>
/// Factory for creating standard parameter sets used across multiple fractal types.
/// Implements the 80/20 rule: 80% of fractals use these standard parameter templates.
/// 
/// Split into partial classes for maintainability:
/// - StandardParameterTemplates.cs (this file): Core view & algorithm templates
/// - StandardParameterTemplates.EscapeTime.cs: Mandelbrot, Multibrot, Julia
/// - StandardParameterTemplates.Complex.cs: Lambda, Phoenix, complex-exponent
/// - StandardParameterTemplates.Newton.cs: Newton, Nova, relaxation methods
/// - StandardParameterTemplates.Special.cs: Attractor, bifurcation, color
/// </summary>
public static partial class StandardParameterTemplates
{
    // ═══════════════════════════════════════════════════════════════════════════════
    // STANDARD VIEW PARAMETERS (100% of fractals)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Standard 2D view parameters: centerX, centerY, zoom.
    /// Used by all 2D escape-time fractals.
    /// Accepts native defaults from FractalDescriptor.
    /// </summary>
    public static IEnumerable<FractalParameterDescriptor> StandardView2D(
        double defaultCenterX = 0.0,
        double defaultCenterY = 0.0,
        double defaultZoom = 1.0)
    {
        yield return new FractalParameterDescriptor
        {
            Key = "center_x",
            Name = "Center X",
            Type = ParameterType.Double,
            Category = ParameterCategory.View,
            DefaultValue = defaultCenterX,
            MinValue = -10.0,
            MaxValue = 10.0,
            StepSize = 0.01,
            FormatString = "F6",
            Description = "Real coordinate of the viewport center",
            DisplayOrder = 1
        };

        yield return new FractalParameterDescriptor
        {
            Key = "center_y",
            Name = "Center Y",
            Type = ParameterType.Double,
            Category = ParameterCategory.View,
            DefaultValue = defaultCenterY,
            MinValue = -10.0,
            MaxValue = 10.0,
            StepSize = 0.01,
            FormatString = "F6",
            Description = "Imaginary coordinate of the viewport center",
            DisplayOrder = 2
        };

        yield return new FractalParameterDescriptor
        {
            Key = "zoom",
            Name = "Zoom",
            Type = ParameterType.Double,
            Category = ParameterCategory.View,
            DefaultValue = defaultZoom,
            MinValue = 0.001,
            MaxValue = 1e15,
            StepSize = 0.1,
            FormatString = "E2",
            Description = "Magnification level (higher = more zoomed in)",
            DisplayOrder = 3,
            Unit = "x"
        };
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // ALGORITHM PARAMETERS (100% of fractals)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Standard algorithm parameters: maxIterations, bailout.
    /// Used by all escape-time fractals.
    /// </summary>
    public static IEnumerable<FractalParameterDescriptor> StandardAlgorithm()
    {
        yield return new FractalParameterDescriptor
        {
            Key = "max_iterations",
            Name = "Max Iterations",
            Type = ParameterType.Integer,
            Category = ParameterCategory.Algorithm,
            DefaultValue = 512,
            MinValue = 50,
            MaxValue = 50000,
            StepSize = 10,
            FormatString = "N0",
            Description = "Maximum number of iterations before considering a point 'in the set'",
            DisplayOrder = 1
        };

        yield return new FractalParameterDescriptor
        {
            Key = "auto_scale_iterations",
            Name = "Auto-scale with Zoom",
            Type = ParameterType.Boolean,
            Category = ParameterCategory.Algorithm,
            DefaultValue = true,
            Description = "Automatically increase iterations based on zoom level for better detail",
            DisplayOrder = 2
        };

        yield return new FractalParameterDescriptor
        {
            Key = "bailout",
            Name = "Bailout Radius",
            Type = ParameterType.Double,
            Category = ParameterCategory.Algorithm,
            DefaultValue = 256.0,
            MinValue = 2.0,
            MaxValue = 1e6,
            StepSize = 1.0,
            FormatString = "F1",
            Description = "Escape radius threshold - higher values may show more detail but slower render",
            DisplayOrder = 3,
            Unit = "radius"
        };

        yield return new FractalParameterDescriptor
        {
            Key = "escape_radius",
            Name = "Escape Radius",
            Type = ParameterType.Double,
            Category = ParameterCategory.Algorithm,
            DefaultValue = 2.0,
            MinValue = 1.1,
            MaxValue = 1000.0,
            StepSize = 0.1,
            FormatString = "F2",
            Description = "Radius at which a point is considered to have escaped",
            DisplayOrder = 4
        };
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // JULIA MODE PARAMETERS (~50% of fractals)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Julia mode toggle and constant parameters.
    /// Used by fractals that support Julia set variants.
    /// </summary>
    public static IEnumerable<FractalParameterDescriptor> JuliaMode()
    {
        yield return new FractalParameterDescriptor
        {
            Key = "julia_mode",
            Name = "Julia Mode",
            Type = ParameterType.Boolean,
            Category = ParameterCategory.Julia,
            DefaultValue = false,
            Description = "Enable Julia set mode (constant c, variable z0)",
            DisplayOrder = 1
        };

        yield return new FractalParameterDescriptor
        {
            Key = "julia_c_real",
            Name = "Julia C (Real)",
            Type = ParameterType.Double,
            Category = ParameterCategory.Julia,
            DefaultValue = -0.7,
            MinValue = -2.0,
            MaxValue = 2.0,
            StepSize = 0.001,
            FormatString = "F6",
            Description = "Real part of the Julia constant",
            DisplayOrder = 2,
            VisibilityCondition = "julia_mode == true"
        };

        yield return new FractalParameterDescriptor
        {
            Key = "julia_c_imag",
            Name = "Julia C (Imaginary)",
            Type = ParameterType.Double,
            Category = ParameterCategory.Julia,
            DefaultValue = 0.27015,
            MinValue = -2.0,
            MaxValue = 2.0,
            StepSize = 0.001,
            FormatString = "F6",
            Description = "Imaginary part of the Julia constant",
            DisplayOrder = 3,
            VisibilityCondition = "julia_mode == true"
        };
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // INTEGER EXPONENT (Multibrot variants)
    // ═══════════════════════════════════════════════════════════════════════════════

    public static FractalParameterDescriptor IntegerExponent(int defaultValue, int min = 2, int max = 20)
    {
        return new FractalParameterDescriptor
        {
            Key = "exponent",
            Name = "Exponent",
            Type = ParameterType.Integer,
            Category = ParameterCategory.FractalSpecific,
            DefaultValue = defaultValue,
            MinValue = min,
            MaxValue = max,
            StepSize = 1,
            Description = "Integer power in the iteration formula",
            DisplayOrder = 1
        };
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // COMPLEX EXPONENT
    // ═══════════════════════════════════════════════════════════════════════════════

    public static IEnumerable<FractalParameterDescriptor> ComplexExponent()
    {
        yield return new FractalParameterDescriptor
        {
            Key = "exp_real",
            Name = "Exponent (Real)",
            Type = ParameterType.Double,
            Category = ParameterCategory.FractalSpecific,
            DefaultValue = 2.0,
            MinValue = -10.0,
            MaxValue = 10.0,
            StepSize = 0.1,
            Description = "Real part of complex exponent",
            DisplayOrder = 1
        };

        yield return new FractalParameterDescriptor
        {
            Key = "exp_imag",
            Name = "Exponent (Imaginary)",
            Type = ParameterType.Double,
            Category = ParameterCategory.FractalSpecific,
            DefaultValue = 0.0,
            MinValue = -10.0,
            MaxValue = 10.0,
            StepSize = 0.1,
            Description = "Imaginary part of complex exponent",
            DisplayOrder = 2
        };
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // POLYNOMIAL COEFFICIENTS (Newton, Halley, convergence methods)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Generate polynomial coefficient parameters for Newton/Halley/convergence methods.
    /// For a polynomial of degree n: z^n + a*z^(n-1) + b*z^(n-2) + ... + c*z + d = 0
    /// This generates parameters for all lower-degree coefficients (a, b, c, d, ...).
    /// </summary>
    /// <param name="degree">Polynomial degree (e.g., 3 for cubic, 4 for quartic)</param>
    /// <returns>Array of coefficient parameters</returns>
    public static IEnumerable<FractalParameterDescriptor> PolynomialCoefficients(int degree)
    {
        // Coefficient names: a, b, c, d, e, f, g, h, i, j (supports up to degree 10)
        string[] coeffNames = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };

        // Generate parameters for each lower-degree term
        for (int i = 0; i < degree; i++)
        {
            int exponent = degree - 1 - i; // z^(n-1), z^(n-2), ..., z^1, z^0
            string coeffName = coeffNames[i];

            yield return new FractalParameterDescriptor
            {
                Key = $"poly_coeff_{coeffName}",
                Name = $"Coefficient {coeffName} (z^{exponent})",
                Type = ParameterType.Double,
                Category = ParameterCategory.FractalSpecific,
                DefaultValue = 0.0,
                MinValue = -10.0,
                MaxValue = 10.0,
                StepSize = 0.1,
                FormatString = "F2",
                Description = $"Coefficient for z^{exponent} term in polynomial",
                DisplayOrder = 10 + i
            };
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // CONVENIENCE BUILDERS
    // ═══════════════════════════════════════════════════════════════════════════════

    public static FractalParameterSet CreateStandardEscapeTime(
        string fractalType,
        double defaultCenterX = 0.0,
        double defaultCenterY = 0.0,
        double defaultZoom = 1.0)
    {
        var paramSet = new FractalParameterSet(fractalType);
        paramSet.AddParameters(StandardView2D(defaultCenterX, defaultCenterY, defaultZoom).ToArray());
        paramSet.AddParameters(StandardAlgorithm().ToArray());
        return paramSet;
    }

    public static FractalParameterSet CreateWithJulia(
        string fractalType,
        double defaultCenterX = 0.0,
        double defaultCenterY = 0.0,
        double defaultZoom = 1.0)
    {
        var paramSet = CreateStandardEscapeTime(fractalType, defaultCenterX, defaultCenterY, defaultZoom);
        paramSet.AddParameters(JuliaMode().ToArray());
        return paramSet;
    }

    public static FractalParameterSet CreateMultibrot(
        string fractalType,
        int defaultExponent,
        double defaultCenterX = 0.0,
        double defaultCenterY = 0.0,
        double defaultZoom = 1.0)
    {
        var paramSet = CreateWithJulia(fractalType, defaultCenterX, defaultCenterY, defaultZoom);
        paramSet.AddParameter(IntegerExponent(defaultExponent));
        return paramSet;
    }

    public static FractalParameterSet CreateNewton(
        string fractalType,
        double defaultCenterX = 0.0,
        double defaultCenterY = 0.0,
        double defaultZoom = 1.0)
    {
        var paramSet = CreateStandardEscapeTime(fractalType, defaultCenterX, defaultCenterY, defaultZoom);
        // Add Newton-specific parameters here if needed
        return paramSet;
    }

    /// <summary>
    /// Create a Newton polynomial fractal with all coefficient parameters.
    /// For example, degree 3 creates: z³ + az² + bz + c = 0
    /// </summary>
    public static FractalParameterSet CreateNewtonPolynomial(
        string fractalType,
        int degree,
        double defaultCenterX = 0.0,
        double defaultCenterY = 0.0,
        double defaultZoom = 1.0)
    {
        var paramSet = CreateStandardEscapeTime(fractalType, defaultCenterX, defaultCenterY, defaultZoom);
        paramSet.AddParameters(PolynomialCoefficients(degree).ToArray());
        return paramSet;
    }
}
