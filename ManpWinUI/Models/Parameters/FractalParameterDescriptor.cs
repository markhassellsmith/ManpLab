using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace ManpWinUI.Models.Parameters;

/// <summary>
/// Describes a single parameter for a fractal type.
/// Contains metadata about the parameter: name, type, range, default value, etc.
/// Immutable after creation - acts as a template for parameter instances.
/// </summary>
public partial class FractalParameterDescriptor : ObservableObject
{
    /// <summary>
    /// Internal key used for programmatic access.
    /// Example: "center_x", "max_iterations", "julia_c_real"
    /// Must be unique within a parameter set.
    /// </summary>
    public string Key { get; init; } = string.Empty;

    /// <summary>
    /// Display name shown in the UI.
    /// Example: "Center X", "Max Iterations", "Julia C (Real)"
    /// Should be human-readable and localized if needed.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Data type of this parameter.
    /// Determines validation rules and UI control selection.
    /// </summary>
    public ParameterType Type { get; init; }

    /// <summary>
    /// Logical category for UI grouping.
    /// Parameters in the same category are displayed together.
    /// </summary>
    public ParameterCategory Category { get; init; } = ParameterCategory.FractalSpecific;

    /// <summary>
    /// Default value for this parameter.
    /// Type must match ParameterType:
    /// - Double → double
    /// - Integer → int
    /// - Boolean → bool
    /// - Complex → (double Real, double Imaginary)
    /// - Choice → string (one of ChoiceValues)
    /// - etc.
    /// </summary>
    public object DefaultValue { get; init; } = 0.0;

    /// <summary>
    /// Minimum allowed value (for numeric types only).
    /// Null = no minimum constraint.
    /// </summary>
    public object? MinValue { get; init; }

    /// <summary>
    /// Maximum allowed value (for numeric types only).
    /// Null = no maximum constraint.
    /// </summary>
    public object? MaxValue { get; init; }

    /// <summary>
    /// Increment/decrement step size for spinners.
    /// Example: 0.01 for coordinates, 10 for iterations.
    /// Null = use default step for type.
    /// </summary>
    public double? StepSize { get; init; }

    /// <summary>
    /// Number format string for display.
    /// Example: "F6" for 6 decimal places, "N0" for integer with grouping.
    /// Null = use default format for type.
    /// </summary>
    public string? FormatString { get; init; }

    /// <summary>
    /// Available choices for Choice type parameters.
    /// Example: ["SIN", "COS", "TAN", "SINH", "COSH", "TANH"]
    /// Ignored for non-Choice types.
    /// </summary>
    public IReadOnlyList<string>? ChoiceValues { get; init; }

    /// <summary>
    /// Descriptive text explaining what this parameter does.
    /// Displayed as tooltip or help text in the UI.
    /// Example: "Real part of the Julia set constant. Controls horizontal offset."
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Whether the user can edit this parameter in the UI.
    /// False = read-only, computed, or internal parameter.
    /// Example: "escape_percentage" is computed from render results.
    /// </summary>
    public bool IsEditable { get; init; } = true;

    /// <summary>
    /// Whether this parameter is required (cannot be null/empty).
    /// Example: centerX, centerY, zoom are always required.
    /// Optional parameters can be null (e.g., juliaC when not in Julia mode).
    /// </summary>
    public bool IsRequired { get; init; } = true;

    /// <summary>
    /// Optional validation rule expression.
    /// Can be a regex pattern (for strings) or a C# expression (for logic).
    /// Example: "value >= 0 && value <= 360" for angle parameters.
    /// Null = use type-based validation only.
    /// </summary>
    public string? ValidationRule { get; init; }

    /// <summary>
    /// UI hint for rendering order.
    /// Lower values displayed first within the same category.
    /// Default: 0 (parameters appear in definition order).
    /// </summary>
    public int DisplayOrder { get; init; } = 0;

    /// <summary>
    /// Optional unit label displayed next to the value.
    /// Example: "px" for dimensions, "°" for angles, "ms" for time.
    /// Null = no unit display.
    /// </summary>
    public string? Unit { get; init; }

    /// <summary>
    /// Optional dependency expression.
    /// Parameter is only visible/editable if the expression evaluates to true.
    /// Example: "juliaMode == true" → juliaC parameters only shown in Julia mode.
    /// Null = always visible.
    /// </summary>
    public string? VisibilityCondition { get; init; }

    /// <summary>
    /// Creates a deep copy of this descriptor with optional property overrides.
    /// Useful for creating parameter variations.
    /// </summary>
    public FractalParameterDescriptor WithOverrides(
        string? key = null,
        string? name = null,
        object? defaultValue = null,
        object? minValue = null,
        object? maxValue = null,
        string? description = null)
    {
        return new FractalParameterDescriptor
        {
            Key = key ?? this.Key,
            Name = name ?? this.Name,
            Type = this.Type,
            Category = this.Category,
            DefaultValue = defaultValue ?? this.DefaultValue,
            MinValue = minValue ?? this.MinValue,
            MaxValue = maxValue ?? this.MaxValue,
            StepSize = this.StepSize,
            FormatString = this.FormatString,
            ChoiceValues = this.ChoiceValues,
            Description = description ?? this.Description,
            IsEditable = this.IsEditable,
            IsRequired = this.IsRequired,
            ValidationRule = this.ValidationRule,
            DisplayOrder = this.DisplayOrder,
            Unit = this.Unit,
            VisibilityCondition = this.VisibilityCondition
        };
    }

    /// <summary>
    /// Validates a value against this parameter's constraints.
    /// Returns null if valid, or an error message if invalid.
    /// </summary>
    public string? ValidateValue(object? value)
    {
        // Required check
        if (IsRequired && value == null)
            return $"{Name} is required";

        if (value == null)
            return null; // Optional parameter, null is OK

        // Type checking
        switch (Type)
        {
            case ParameterType.Double:
                if (value is not double doubleValue)
                    return $"{Name} must be a decimal number";

                if (MinValue is double minDouble && doubleValue < minDouble)
                    return $"{Name} must be >= {minDouble}";

                if (MaxValue is double maxDouble && doubleValue > maxDouble)
                    return $"{Name} must be <= {maxDouble}";

                if (double.IsNaN(doubleValue) || double.IsInfinity(doubleValue))
                    return $"{Name} must be a finite number";
                break;

            case ParameterType.Integer:
                if (value is not int intValue)
                    return $"{Name} must be a whole number";

                if (MinValue is int minInt && intValue < minInt)
                    return $"{Name} must be >= {minInt}";

                if (MaxValue is int maxInt && intValue > maxInt)
                    return $"{Name} must be <= {maxInt}";
                break;

            case ParameterType.Boolean:
                if (value is not bool)
                    return $"{Name} must be true or false";
                break;

            case ParameterType.Complex:
                if (value is not ValueTuple<double, double> complexValue)
                    return $"{Name} must be a complex number (real, imaginary)";

                if (double.IsNaN(complexValue.Item1) || double.IsInfinity(complexValue.Item1) ||
                    double.IsNaN(complexValue.Item2) || double.IsInfinity(complexValue.Item2))
                    return $"{Name} must have finite real and imaginary parts";
                break;

            case ParameterType.Choice:
                if (value is not string choiceValue)
                    return $"{Name} must be a text value";

                if (ChoiceValues != null && !ChoiceValues.Contains(choiceValue))
                    return $"{Name} must be one of: {string.Join(", ", ChoiceValues)}";
                break;

            case ParameterType.String:
                if (value is not string stringValue)
                    return $"{Name} must be text";

                if (IsRequired && string.IsNullOrWhiteSpace(stringValue))
                    return $"{Name} cannot be empty";
                break;

            case ParameterType.Point2D:
                if (value is not ValueTuple<double, double> pointValue)
                    return $"{Name} must be a 2D point (x, y)";

                if (double.IsNaN(pointValue.Item1) || double.IsInfinity(pointValue.Item1) ||
                    double.IsNaN(pointValue.Item2) || double.IsInfinity(pointValue.Item2))
                    return $"{Name} must have finite x and y coordinates";
                break;
        }

        // Custom validation rule (if specified)
        // TODO: Implement expression evaluation for ValidationRule
        // For now, we skip custom validation

        return null; // Valid
    }

    public override string ToString()
    {
        return $"{Name} ({Type}): {DefaultValue}";
    }
}
