using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ManpWinUI.Models.Parameters;

/// <summary>
/// Represents a complete set of parameters for a specific fractal type.
/// Contains both the parameter descriptors (metadata) and their current values.
/// Observable - changes to parameter values trigger property notifications.
/// </summary>
public partial class FractalParameterSet : ObservableObject
{
    // ═══════════════════════════════════════════════════════════════════════════════
    // PROPERTIES
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Fractal type name this parameter set belongs to.
    /// Example: "Mandelbrot", "Lambda", "Nova"
    /// </summary>
    [ObservableProperty]
    private string _fractalType = string.Empty;

    /// <summary>
    /// Collection of parameter descriptors (metadata).
    /// Defines what parameters exist, their types, ranges, etc.
    /// Immutable after initialization.
    /// </summary>
    public ObservableCollection<FractalParameterDescriptor> Parameters { get; }

    /// <summary>
    /// Current values for all parameters.
    /// Key = parameter key, Value = current value (typed object).
    /// </summary>
    private readonly Dictionary<string, object?> _values;

    /// <summary>
    /// Validation errors for parameters.
    /// Key = parameter key, Value = error message (null if valid).
    /// </summary>
    private readonly Dictionary<string, string?> _validationErrors;

    /// <summary>
    /// Event raised when any parameter value changes.
    /// Consumers can subscribe to react to parameter updates.
    /// </summary>
    public event EventHandler<ParameterChangedEventArgs>? ParameterChanged;

    // ═══════════════════════════════════════════════════════════════════════════════
    // CONSTRUCTOR
    // ═══════════════════════════════════════════════════════════════════════════════

    public FractalParameterSet(string fractalType)
    {
        _fractalType = fractalType;
        Parameters = new ObservableCollection<FractalParameterDescriptor>();
        _values = new Dictionary<string, object?>();
        _validationErrors = new Dictionary<string, string?>();
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // PARAMETER MANAGEMENT
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Adds a parameter descriptor to this set.
    /// Initializes the value to DefaultValue.
    /// </summary>
    public void AddParameter(FractalParameterDescriptor descriptor)
    {
        if (string.IsNullOrEmpty(descriptor.Key))
            throw new ArgumentException("Parameter key cannot be empty", nameof(descriptor));

        if (_values.ContainsKey(descriptor.Key))
            throw new InvalidOperationException($"Parameter '{descriptor.Key}' already exists");

        Parameters.Add(descriptor);
        _values[descriptor.Key] = descriptor.DefaultValue;
        _validationErrors[descriptor.Key] = null;
    }

    /// <summary>
    /// Adds multiple parameter descriptors at once.
    /// Convenience method for bulk initialization.
    /// </summary>
    public void AddParameters(params FractalParameterDescriptor[] descriptors)
    {
        foreach (var descriptor in descriptors)
        {
            AddParameter(descriptor);
        }
    }

    /// <summary>
    /// Gets a parameter descriptor by key.
    /// Returns null if not found.
    /// </summary>
    public FractalParameterDescriptor? GetDescriptor(string key)
    {
        return Parameters.FirstOrDefault(p => p.Key == key);
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // VALUE ACCESS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Gets the current value of a parameter.
    /// Returns null if parameter doesn't exist.
    /// </summary>
    public object? GetValue(string key)
    {
        return _values.TryGetValue(key, out var value) ? value : null;
    }

    /// <summary>
    /// Gets a parameter value with type casting.
    /// Returns default(T) if parameter doesn't exist or type doesn't match.
    /// </summary>
    public T? GetValue<T>(string key)
    {
        if (_values.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        return default;
    }

    /// <summary>
    /// Sets the value of a parameter.
    /// Validates the value and triggers change notifications.
    /// Returns true if successful, false if validation failed.
    /// </summary>
    public bool SetValue(string key, object? value)
    {
        var descriptor = GetDescriptor(key);
        if (descriptor == null)
        {
            System.Diagnostics.Debug.WriteLine($"[FractalParameterSet] Warning: Parameter '{key}' not found");
            return false;
        }

        // Validate
        var validationError = descriptor.ValidateValue(value);
        _validationErrors[key] = validationError;

        if (validationError != null)
        {
            System.Diagnostics.Debug.WriteLine($"[FractalParameterSet] Validation error for '{key}': {validationError}");
            return false;
        }

        // Update value
        var oldValue = _values[key];
        if (Equals(oldValue, value))
            return true; // No change

        _values[key] = value;

        // Notify observers
        ParameterChanged?.Invoke(this, new ParameterChangedEventArgs(key, oldValue, value));
        OnPropertyChanged(nameof(Parameters)); // Trigger UI update

        System.Diagnostics.Debug.WriteLine($"[FractalParameterSet] Parameter '{key}' changed: {oldValue} → {value}");

        return true;
    }

    /// <summary>
    /// Resets a parameter to its default value.
    /// </summary>
    public void ResetParameter(string key)
    {
        var descriptor = GetDescriptor(key);
        if (descriptor != null)
        {
            SetValue(key, descriptor.DefaultValue);
        }
    }

    /// <summary>
    /// Resets all parameters to their default values.
    /// </summary>
    public void ResetAll()
    {
        foreach (var descriptor in Parameters)
        {
            _values[descriptor.Key] = descriptor.DefaultValue;
            _validationErrors[descriptor.Key] = null;
        }

        OnPropertyChanged(nameof(Parameters));
        ParameterChanged?.Invoke(this, new ParameterChangedEventArgs(string.Empty, null, null));
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // VALIDATION
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Validates all parameters in this set.
    /// Returns true if all parameters are valid, false otherwise.
    /// </summary>
    public bool Validate()
    {
        var isValid = true;

        foreach (var descriptor in Parameters)
        {
            var value = GetValue(descriptor.Key);
            var error = descriptor.ValidateValue(value);
            _validationErrors[descriptor.Key] = error;

            if (error != null)
            {
                isValid = false;
                System.Diagnostics.Debug.WriteLine($"[FractalParameterSet] Validation error for '{descriptor.Key}': {error}");
            }
        }

        return isValid;
    }

    /// <summary>
    /// Gets validation error for a specific parameter.
    /// Returns null if parameter is valid or doesn't exist.
    /// </summary>
    public string? GetValidationError(string key)
    {
        return _validationErrors.TryGetValue(key, out var error) ? error : null;
    }

    /// <summary>
    /// Gets all validation errors.
    /// Returns empty dictionary if all parameters are valid.
    /// </summary>
    public IReadOnlyDictionary<string, string?> GetAllValidationErrors()
    {
        return _validationErrors.Where(kvp => kvp.Value != null)
                                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// Checks if the parameter set has any validation errors.
    /// </summary>
    public bool HasErrors => _validationErrors.Values.Any(e => e != null);

    // ═══════════════════════════════════════════════════════════════════════════════
    // CONVERSION TO RENDER PARAMETERS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Converts current parameter values to a dictionary suitable for rendering.
    /// Keys are parameter keys, values are typed objects.
    /// Used by render services to pass parameters to native layer.
    /// </summary>
    public Dictionary<string, object> ToRenderParameters()
    {
        var renderParams = new Dictionary<string, object>();

        foreach (var descriptor in Parameters)
        {
            var value = GetValue(descriptor.Key);
            if (value != null)
            {
                renderParams[descriptor.Key] = value;
            }
        }

        return renderParams;
    }

    /// <summary>
    /// Converts current parameter values to a structured RenderParameters object.
    /// Maps known parameters to strongly-typed fields, and places unknown parameters
    /// in ExtendedParameters dictionary.
    /// </summary>
    public RenderParameters ToStructuredRenderParameters(int imageWidth, int imageHeight)
    {
        var renderParams = new RenderParameters
        {
            FractalType = FractalType,
            Width = imageWidth,
            Height = imageHeight
        };

        // Map known parameters to strongly-typed fields
        renderParams.CenterX = GetValue<double>("center_x");
        renderParams.CenterY = GetValue<double>("center_y");
        renderParams.Zoom = GetValue<double>("zoom");
        renderParams.MaxIterations = GetValue<int>("max_iterations");
        renderParams.EscapeRadius = GetValue<double>("escape_radius");

        // Julia mode
        renderParams.IsJuliaMode = GetValue<bool>("julia_mode");
        renderParams.JuliaCReal = GetValue<double>("julia_c_real");
        renderParams.JuliaCImaginary = GetValue<double>("julia_c_imag");

        // Note: Color parameters are typically set at ViewModel level, not per-fractal
        // They're handled separately in the render command

        // Collect all other parameters as extended
        foreach (var descriptor in Parameters)
        {
            // Skip parameters we've already mapped
            var knownKeys = new[] 
            { 
                "center_x", "center_y", "zoom", "max_iterations", "escape_radius",
                "julia_mode", "julia_c_real", "julia_c_imag"
            };

            if (!knownKeys.Contains(descriptor.Key))
            {
                var value = GetValue(descriptor.Key);
                if (value != null)
                {
                    renderParams.ExtendedParameters[descriptor.Key] = value;
                }
            }
        }

        return renderParams;
    }

    /// <summary>
    /// Imports parameter values from a dictionary.
    /// Validates each value before setting.
    /// Returns number of successfully imported parameters.
    /// </summary>
    public int ImportValues(Dictionary<string, object> values)
    {
        var importCount = 0;

        foreach (var kvp in values)
        {
            if (SetValue(kvp.Key, kvp.Value))
            {
                importCount++;
            }
        }

        System.Diagnostics.Debug.WriteLine($"[FractalParameterSet] Imported {importCount}/{values.Count} parameters");
        return importCount;
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // SERIALIZATION / PERSISTENCE
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Exports parameter values to a simple key-value dictionary for saving.
    /// Only exports editable parameters with non-default values.
    /// </summary>
    public Dictionary<string, object> ExportForSave()
    {
        var export = new Dictionary<string, object>();

        foreach (var descriptor in Parameters.Where(p => p.IsEditable))
        {
            var value = GetValue(descriptor.Key);
            // Only save if value differs from default
            if (value != null && !Equals(value, descriptor.DefaultValue))
            {
                export[descriptor.Key] = value;
            }
        }

        return export;
    }

    /// <summary>
    /// Gets parameters grouped by category for UI rendering.
    /// Returns dictionary where key = category, value = list of descriptors in that category.
    /// </summary>
    public Dictionary<ParameterCategory, List<FractalParameterDescriptor>> GetParametersByCategory()
    {
        return Parameters.GroupBy(p => p.Category)
                        .OrderBy(g => g.Key)
                        .ToDictionary(g => g.Key, 
                                     g => g.OrderBy(p => p.DisplayOrder)
                                          .ThenBy(p => p.Name)
                                          .ToList());
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // DEBUGGING
    // ═══════════════════════════════════════════════════════════════════════════════

    public override string ToString()
    {
        return $"{FractalType} ({Parameters.Count} parameters, {(HasErrors ? "HAS ERRORS" : "valid")})";
    }

    /// <summary>
    /// Dumps all parameters and their current values to debug output.
    /// Useful for troubleshooting parameter issues.
    /// </summary>
    public void DumpToDebug()
    {
        System.Diagnostics.Debug.WriteLine($"╔═══════════════════════════════════════════════════════════");
        System.Diagnostics.Debug.WriteLine($"║ FractalParameterSet: {FractalType}");
        System.Diagnostics.Debug.WriteLine($"╠═══════════════════════════════════════════════════════════");

        foreach (var descriptor in Parameters.OrderBy(p => p.Category).ThenBy(p => p.DisplayOrder))
        {
            var value = GetValue(descriptor.Key);
            var error = GetValidationError(descriptor.Key);
            var status = error != null ? $" ❌ {error}" : " ✓";

            System.Diagnostics.Debug.WriteLine($"║ [{descriptor.Category}] {descriptor.Name}");
            System.Diagnostics.Debug.WriteLine($"║   Key: {descriptor.Key}");
            System.Diagnostics.Debug.WriteLine($"║   Type: {descriptor.Type}");
            System.Diagnostics.Debug.WriteLine($"║   Value: {value ?? "<null>"}{status}");
            System.Diagnostics.Debug.WriteLine($"║   Default: {descriptor.DefaultValue}");
            if (descriptor.MinValue != null || descriptor.MaxValue != null)
                System.Diagnostics.Debug.WriteLine($"║   Range: [{descriptor.MinValue ?? "∞"}..{descriptor.MaxValue ?? "∞"}]");
            System.Diagnostics.Debug.WriteLine($"║");
        }

        System.Diagnostics.Debug.WriteLine($"╚═══════════════════════════════════════════════════════════");
    }
}

/// <summary>
/// Event arguments for parameter value changes.
/// </summary>
public class ParameterChangedEventArgs : EventArgs
{
    public string ParameterKey { get; }
    public object? OldValue { get; }
    public object? NewValue { get; }

    public ParameterChangedEventArgs(string parameterKey, object? oldValue, object? newValue)
    {
        ParameterKey = parameterKey;
        OldValue = oldValue;
        NewValue = newValue;
    }
}
