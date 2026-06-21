using System.Collections.Generic;

namespace ManpWinUI.Models.LSystem;

/// <summary>
/// Complete definition of an L-System fractal including grammar rules and rendering parameters.
/// Based on Lindenmayer system grammar rewriting with turtle graphics interpretation.
/// </summary>
public class LSystemDefinition
{
    /// <summary>
    /// Display name for the L-System (e.g., "Koch Snowflake", "Dragon Curve").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Starting string (axiom) before any rule applications.
    /// Example: "F++F++F" for Koch Snowflake
    /// </summary>
    public string Axiom { get; set; } = string.Empty;

    /// <summary>
    /// Production rules for string rewriting.
    /// Applied simultaneously on each generation.
    /// </summary>
    public List<LSystemRule> Rules { get; set; } = new();

    /// <summary>
    /// Angle in degrees for '+' and '-' turtle turn commands.
    /// Example: 60° for Koch Snowflake, 90° for Dragon Curve
    /// </summary>
    public double TurnAngle { get; set; } = 90.0;

    /// <summary>
    /// Default number of generations to apply rules.
    /// Each generation exponentially increases string length.
    /// </summary>
    public int DefaultGenerations { get; set; } = 4;

    /// <summary>
    /// Maximum safe generations before string explosion.
    /// Prevents UI lockup from excessive memory usage.
    /// </summary>
    public int MaxGenerations { get; set; } = 10;

    /// <summary>
    /// Starting direction in degrees (0 = right, 90 = up).
    /// Default: 0.0 (pointing right)
    /// </summary>
    public double InitialHeading { get; set; } = 0.0;

    /// <summary>
    /// Optional description or historical context.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Category for UI grouping (e.g., "Classic", "Plant", "Space-Filling").
    /// </summary>
    public string Category { get; set; } = "General";

    /// <summary>
    /// Rendering hint: whether to center and scale to fit viewport.
    /// Default: true (auto-fit)
    /// </summary>
    public bool AutoFit { get; set; } = true;
}
