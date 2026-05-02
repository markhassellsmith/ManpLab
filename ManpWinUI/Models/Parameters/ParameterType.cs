namespace ManpWinUI.Models.Parameters;

/// <summary>
/// Defines the data types supported for fractal parameters.
/// Each type maps to specific UI controls and validation rules.
/// </summary>
public enum ParameterType
{
    /// <summary>
    /// Floating-point number with decimal precision.
    /// UI: NumberBox with decimal mode
    /// Example: centerX = -0.5
    /// </summary>
    Double,

    /// <summary>
    /// Whole number (integer).
    /// UI: NumberBox with integer mode
    /// Example: maxIterations = 512
    /// </summary>
    Integer,

    /// <summary>
    /// Complex number with real and imaginary parts.
    /// UI: Two NumberBoxes side-by-side (Real, Imaginary)
    /// Example: juliaC = (-0.7, 0.27015)
    /// </summary>
    Complex,

    /// <summary>
    /// Boolean true/false value.
    /// UI: ToggleSwitch or CheckBox
    /// Example: juliaMode = true
    /// </summary>
    Boolean,

    /// <summary>
    /// Selection from predefined choices.
    /// UI: ComboBox with dropdown options
    /// Example: trigFunction = "SIN" | "COS" | "TAN"
    /// </summary>
    Choice,

    /// <summary>
    /// 2D coordinate pair (X, Y).
    /// UI: Two NumberBoxes side-by-side (X, Y)
    /// Example: initialPosition = (0.0, 0.0)
    /// </summary>
    Point2D,

    /// <summary>
    /// Color palette selection.
    /// UI: ColorPicker or palette dropdown
    /// Example: colorPalette = "Classic" | "Rainbow" | "Grayscale"
    /// </summary>
    ColorPalette,

    /// <summary>
    /// Array of values (coefficients, rules, etc.).
    /// UI: Dynamic list editor with add/remove
    /// Example: polynomialCoefficients = [1.0, 2.5, -0.3, 4.1]
    /// </summary>
    Array,

    /// <summary>
    /// User-defined text formula or expression.
    /// UI: TextBox with syntax highlighting
    /// Example: formula = "z^2 + c * sin(z)"
    /// </summary>
    Formula,

    /// <summary>
    /// Text string (for names, descriptions, etc.).
    /// UI: TextBox
    /// Example: axiom = "F+F--F+F"
    /// </summary>
    String
}
