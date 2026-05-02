using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManpWinUI.Services;

/// <summary>
/// Service for managing fractal parameter definitions and values.
/// Acts as the bridge between the parameter model and the native registry.
/// </summary>
public interface IFractalParameterService
{
    /// <summary>
    /// Initialize the service (load parameter templates, etc.).
    /// Should be called once at app startup.
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Get a complete parameter set for a specific fractal type.
    /// Returns null if fractal type doesn't exist.
    /// </summary>
    /// <param name="fractalType">Fractal type name (e.g., "Mandelbrot", "Nova")</param>
    /// <returns>Parameter set with all descriptors and default values</returns>
    Task<Models.Parameters.FractalParameterSet?> GetParametersAsync(string fractalType);

    /// <summary>
    /// Update a parameter value in an existing parameter set.
    /// Validates and notifies of changes.
    /// </summary>
    /// <param name="paramSet">Parameter set to update</param>
    /// <param name="key">Parameter key</param>
    /// <param name="value">New value</param>
    /// <returns>True if update was successful, false if validation failed</returns>
    bool UpdateParameter(Models.Parameters.FractalParameterSet paramSet, string key, object value);

    /// <summary>
    /// Load saved parameter overrides for a fractal type from settings.
    /// Returns empty dictionary if no overrides exist.
    /// </summary>
    /// <param name="fractalType">Fractal type name</param>
    /// <returns>Dictionary of parameter key → value overrides</returns>
    Task<Dictionary<string, object>> LoadParameterOverridesAsync(string fractalType);

    /// <summary>
    /// Save parameter value overrides for a fractal type to settings.
    /// Only saves non-default values.
    /// </summary>
    /// <param name="fractalType">Fractal type name</param>
    /// <param name="values">Parameter key → value dictionary</param>
    Task SaveParameterOverridesAsync(string fractalType, Dictionary<string, object> values);

    /// <summary>
    /// Validate a complete parameter set.
    /// Returns true if all parameters are valid.
    /// </summary>
    bool ValidateParameterSet(Models.Parameters.FractalParameterSet paramSet);

    /// <summary>
    /// Check if a fractal type has custom parameter metadata registered.
    /// </summary>
    bool HasCustomParameters(string fractalType);
}
