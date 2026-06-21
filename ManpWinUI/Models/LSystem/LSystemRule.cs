namespace ManpWinUI.Models.LSystem;

/// <summary>
/// Represents a single production rule in an L-System grammar.
/// Format: predecessor → successor (e.g., "F" → "F+F--F+F")
/// </summary>
public class LSystemRule
{
    /// <summary>
    /// The symbol to be replaced (single character).
    /// Example: 'F', 'G', 'X', 'Y'
    /// </summary>
    public char Predecessor { get; set; }

    /// <summary>
    /// The replacement string (can contain multiple symbols).
    /// Example: "F+F--F+F", "F-G+F+G-F"
    /// </summary>
    public string Successor { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of what this rule does.
    /// </summary>
    public string? Description { get; set; }
}
