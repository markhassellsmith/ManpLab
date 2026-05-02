namespace ManpWinUI.Models.Parameters;

/// <summary>
/// Logical grouping for parameters in the UI.
/// Used to organize parameters into collapsible sections.
/// </summary>
public enum ParameterCategory
{
    /// <summary>
    /// View/navigation parameters (center, zoom, viewport).
    /// Always displayed first in the UI.
    /// </summary>
    View,

    /// <summary>
    /// Algorithm parameters (iterations, bailout, tolerance).
    /// Controls the mathematical behavior of the fractal.
    /// </summary>
    Algorithm,

    /// <summary>
    /// Julia mode parameters (juliaC, Julia toggle).
    /// Only displayed for fractals that support Julia mode.
    /// </summary>
    Julia,

    /// <summary>
    /// Color and rendering parameters (palette, coloring mode, smooth coloring).
    /// Controls visual appearance but not mathematical structure.
    /// </summary>
    Color,

    /// <summary>
    /// Fractal-specific parameters (exponent, coefficients, etc.).
    /// Parameters unique to this fractal type.
    /// </summary>
    FractalSpecific,

    /// <summary>
    /// Advanced/expert parameters (relaxation, perturbation, derivatives).
    /// Typically hidden by default; shown in "Advanced" section.
    /// </summary>
    Advanced
}
