using System.Collections.Generic;

namespace ManpWinUI.Models.Parameters;

/// <summary>
/// Factory for creating standard parameter sets used across multiple fractal types.
/// Implements the 80/20 rule: 80% of fractals use these standard parameter templates.
/// Use composition to build complete parameter sets.
/// </summary>
public static class StandardParameterTemplates
{
    // ═══════════════════════════════════════════════════════════════════════════════
    // STANDARD VIEW PARAMETERS (100% of fractals)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Standard 2D view parameters: centerX, centerY, zoom.
    /// Used by all 2D escape-time fractals.
    /// </summary>
    public static IEnumerable<FractalParameterDescriptor> StandardView2D()
    {
        yield return new FractalParameterDescriptor
        {
            Key = "center_x",
            Name = "Center X",
            Type = ParameterType.Double,
            Category = ParameterCategory.View,
            DefaultValue = 0.0,
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
            DefaultValue = 0.0,
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
            DefaultValue = 1.0,
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
            Key = "bailout",
            Name = "Bailout Radius",
            Type = ParameterType.Double,
            Category = ParameterCategory.Algorithm,
            DefaultValue = 256.0,
            MinValue = 2.0,
            MaxValue = 10000.0,
            StepSize = 1.0,
            FormatString = "F1",
            Description = "Escape radius threshold (|z| > bailout means point escapes)",
            DisplayOrder = 2
        };

        yield return new FractalParameterDescriptor
        {
            Key = "auto_scale_iterations",
            Name = "Auto-Scale Iterations",
            Type = ParameterType.Boolean,
            Category = ParameterCategory.Algorithm,
            DefaultValue = true,
            Description = "Automatically increase iterations at high zoom levels for better detail",
            DisplayOrder = 3
        };
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // JULIA MODE PARAMETERS (60% of fractals)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Julia set parameters: juliaMode toggle and complex constant c.
    /// Used by fractals that support Julia mode (Mandelbrot, Multibrot, etc.).
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
            Description = "Enable Julia set rendering (fixed c, varying start point z₀)",
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
            Description = "Real part of the Julia set constant c",
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
            Description = "Imaginary part of the Julia set constant c",
            DisplayOrder = 3,
            VisibilityCondition = "julia_mode == true"
        };
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // EXPONENT PARAMETERS (20% of fractals)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Integer exponent parameter for Multibrot family (z^n + c).
    /// </summary>
    public static FractalParameterDescriptor IntegerExponent(int defaultValue = 2, int min = 2, int max = 10)
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
            Description = $"Power in the iteration formula z^n + c",
            DisplayOrder = 1
        };
    }

    /// <summary>
    /// Complex exponent parameters (real + imaginary) for generalized power fractals.
    /// Used by MarksMandel and similar variants.
    /// </summary>
    public static IEnumerable<FractalParameterDescriptor> ComplexExponent()
    {
        yield return new FractalParameterDescriptor
        {
            Key = "exponent_real",
            Name = "Exponent (Real)",
            Type = ParameterType.Double,
            Category = ParameterCategory.FractalSpecific,
            DefaultValue = 2.0,
            MinValue = -5.0,
            MaxValue = 5.0,
            StepSize = 0.01,
            FormatString = "F3",
            Description = "Real part of the complex exponent",
            DisplayOrder = 1
        };

        yield return new FractalParameterDescriptor
        {
            Key = "exponent_imag",
            Name = "Exponent (Imaginary)",
            Type = ParameterType.Double,
            Category = ParameterCategory.FractalSpecific,
            DefaultValue = 0.0,
            MinValue = -5.0,
            MaxValue = 5.0,
            StepSize = 0.01,
            FormatString = "F3",
            Description = "Imaginary part of the complex exponent",
            DisplayOrder = 2
        };
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // NEWTON METHOD PARAMETERS (10% of fractals)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Newton method parameters: polynomial degree, relaxation, stripes.
    /// Used by Newton, Nova, and root-finding fractals.
    /// </summary>
    public static IEnumerable<FractalParameterDescriptor> NewtonMethod()
    {
        yield return new FractalParameterDescriptor
        {
            Key = "degree",
            Name = "Polynomial Degree",
            Type = ParameterType.Integer,
            Category = ParameterCategory.FractalSpecific,
            DefaultValue = 3,
            MinValue = 2,
            MaxValue = 20,
            StepSize = 1,
            Description = "Degree of the polynomial (e.g., 3 for z³-1)",
            DisplayOrder = 1
        };

        yield return new FractalParameterDescriptor
        {
            Key = "relaxation",
            Name = "Relaxation Factor",
            Type = ParameterType.Double,
            Category = ParameterCategory.Advanced,
            DefaultValue = 1.0,
            MinValue = 0.1,
            MaxValue = 2.0,
            StepSize = 0.01,
            FormatString = "F3",
            Description = "Controls convergence speed (1.0 = standard Newton, <1 = slower, >1 = faster)",
            DisplayOrder = 1
        };

        yield return new FractalParameterDescriptor
        {
            Key = "stripes",
            Name = "Stripe Coloring",
            Type = ParameterType.Boolean,
            Category = ParameterCategory.Color,
            DefaultValue = false,
            Description = "Enable stripe pattern coloring in Newton basins",
            DisplayOrder = 10
        };
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // ATTRACTOR PARAMETERS (10% of fractals)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Attractor system parameters: a, b, c, d coefficients and timestep.
    /// Used by Lorenz, Rossler, Henon, and similar strange attractors.
    /// </summary>
    public static IEnumerable<FractalParameterDescriptor> AttractorSystem(
        double defaultA = 0.0, 
        double defaultB = 0.0, 
        double defaultC = 0.0, 
        double defaultD = 0.0,
        double defaultTimestep = 0.01)
    {
        yield return new FractalParameterDescriptor
        {
            Key = "a",
            Name = "Parameter a",
            Type = ParameterType.Double,
            Category = ParameterCategory.FractalSpecific,
            DefaultValue = defaultA,
            MinValue = -10.0,
            MaxValue = 10.0,
            StepSize = 0.01,
            FormatString = "F3",
            Description = "First system parameter",
            DisplayOrder = 1
        };

        yield return new FractalParameterDescriptor
        {
            Key = "b",
            Name = "Parameter b",
            Type = ParameterType.Double,
            Category = ParameterCategory.FractalSpecific,
            DefaultValue = defaultB,
            MinValue = -10.0,
            MaxValue = 10.0,
            StepSize = 0.01,
            FormatString = "F3",
            Description = "Second system parameter",
            DisplayOrder = 2
        };

        yield return new FractalParameterDescriptor
        {
            Key = "c",
            Name = "Parameter c",
            Type = ParameterType.Double,
            Category = ParameterCategory.FractalSpecific,
            DefaultValue = defaultC,
            MinValue = -10.0,
            MaxValue = 10.0,
            StepSize = 0.01,
            FormatString = "F3",
            Description = "Third system parameter",
            DisplayOrder = 3
        };

        yield return new FractalParameterDescriptor
        {
            Key = "d",
            Name = "Parameter d",
            Type = ParameterType.Double,
            Category = ParameterCategory.FractalSpecific,
            DefaultValue = defaultD,
            MinValue = -10.0,
            MaxValue = 10.0,
            StepSize = 0.01,
            FormatString = "F3",
            Description = "Fourth system parameter",
            DisplayOrder = 4
        };

        yield return new FractalParameterDescriptor
        {
            Key = "timestep",
            Name = "Time Step",
            Type = ParameterType.Double,
            Category = ParameterCategory.Algorithm,
            DefaultValue = defaultTimestep,
            MinValue = 0.001,
            MaxValue = 0.1,
            StepSize = 0.001,
            FormatString = "F4",
            Description = "Integration time step (smaller = more accurate, slower)",
            DisplayOrder = 10,
            Unit = "Δt"
        };

        yield return new FractalParameterDescriptor
        {
            Key = "points_per_orbit",
            Name = "Points Per Orbit",
            Type = ParameterType.Integer,
            Category = ParameterCategory.Algorithm,
            DefaultValue = 1000,
            MinValue = 100,
            MaxValue = 10000,
            StepSize = 100,
            FormatString = "N0",
            Description = "Number of points to plot for each trajectory",
            DisplayOrder = 11
        };
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // BIFURCATION PARAMETERS (5% of fractals)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Bifurcation diagram parameters: seed, filter cycles.
    /// Used by bifurcation fractals and population dynamics models.
    /// </summary>
    public static IEnumerable<FractalParameterDescriptor> Bifurcation()
    {
        yield return new FractalParameterDescriptor
        {
            Key = "seed_population",
            Name = "Seed Population",
            Type = ParameterType.Double,
            Category = ParameterCategory.Algorithm,
            DefaultValue = 0.5,
            MinValue = 0.0,
            MaxValue = 1.0,
            StepSize = 0.01,
            FormatString = "F3",
            Description = "Initial population value (0 = extinct, 1 = maximum)",
            DisplayOrder = 1
        };

        yield return new FractalParameterDescriptor
        {
            Key = "filter_cycles",
            Name = "Filter Cycles",
            Type = ParameterType.Integer,
            Category = ParameterCategory.Algorithm,
            DefaultValue = 100,
            MinValue = 0,
            MaxValue = 10000,
            StepSize = 10,
            FormatString = "N0",
            Description = "Number of initial iterations to skip (removes transient behavior)",
            DisplayOrder = 2
        };
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // COLOR PARAMETERS (Optional - used for advanced coloring)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Advanced color parameters: palette, cycle speed, offset, smooth coloring.
    /// Optional - basic fractals use defaults from MainViewModel.
    /// </summary>
    public static IEnumerable<FractalParameterDescriptor> AdvancedColor()
    {
        yield return new FractalParameterDescriptor
        {
            Key = "color_cycle_speed",
            Name = "Color Cycle Speed",
            Type = ParameterType.Integer,
            Category = ParameterCategory.Color,
            DefaultValue = 50,
            MinValue = 0,
            MaxValue = 100,
            StepSize = 1,
            Description = "Speed of color palette rotation animation",
            DisplayOrder = 1
        };

        yield return new FractalParameterDescriptor
        {
            Key = "color_offset",
            Name = "Color Offset",
            Type = ParameterType.Integer,
            Category = ParameterCategory.Color,
            DefaultValue = 0,
            MinValue = 0,
            MaxValue = 360,
            StepSize = 1,
            Description = "Rotate color palette by this many degrees",
            DisplayOrder = 2,
            Unit = "°"
        };

        yield return new FractalParameterDescriptor
        {
            Key = "smooth_coloring",
            Name = "Smooth Coloring",
            Type = ParameterType.Boolean,
            Category = ParameterCategory.Color,
            DefaultValue = true,
            Description = "Use smooth gradient coloring instead of discrete bands",
            DisplayOrder = 3
        };
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // CONVENIENCE BUILDERS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Creates a complete standard escape-time fractal parameter set.
    /// Includes: View + Algorithm parameters.
    /// Used by 80% of fractals as the base.
    /// </summary>
    public static FractalParameterSet CreateStandardEscapeTime(string fractalType)
    {
        var paramSet = new FractalParameterSet(fractalType);
        paramSet.AddParameters(StandardView2D().ToArray());
        paramSet.AddParameters(StandardAlgorithm().ToArray());
        return paramSet;
    }

    /// <summary>
    /// Creates a standard escape-time set with Julia mode support.
    /// Includes: View + Algorithm + Julia parameters.
    /// Used by 60% of fractals (Mandelbrot family).
    /// </summary>
    public static FractalParameterSet CreateWithJulia(string fractalType)
    {
        var paramSet = CreateStandardEscapeTime(fractalType);
        paramSet.AddParameters(JuliaMode().ToArray());
        return paramSet;
    }

    /// <summary>
    /// Creates a parameter set with Julia + integer exponent.
    /// Used by Multibrot family (z^n + c).
    /// </summary>
    public static FractalParameterSet CreateMultibrot(string fractalType, int defaultExponent)
    {
        var paramSet = CreateWithJulia(fractalType);
        paramSet.AddParameter(IntegerExponent(defaultExponent));
        return paramSet;
    }

    /// <summary>
    /// Creates a parameter set for Newton method fractals.
    /// Includes: View + Algorithm + Newton parameters.
    /// </summary>
    public static FractalParameterSet CreateNewton(string fractalType)
    {
        var paramSet = CreateStandardEscapeTime(fractalType);
        paramSet.AddParameters(NewtonMethod().ToArray());
        return paramSet;
    }

    /// <summary>
    /// Creates a parameter set for attractor systems.
    /// Includes: View + Attractor parameters (a,b,c,d,timestep).
    /// </summary>
    public static FractalParameterSet CreateAttractor(
        string fractalType,
        double a = 0.0,
        double b = 0.0,
        double c = 0.0,
        double d = 0.0,
        double timestep = 0.01)
    {
        var paramSet = new FractalParameterSet(fractalType);
        // Attractors don't use standard view (different coordinate system)
        paramSet.AddParameters(AttractorSystem(a, b, c, d, timestep).ToArray());
        return paramSet;
    }
}
