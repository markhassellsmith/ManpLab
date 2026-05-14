using CommunityToolkit.Mvvm.ComponentModel;
using ManpWinUI.Models.Parameters;
using System.Diagnostics;

namespace ManpWinUI.ViewModels;

/// <summary>
/// MainViewModel partial class - Flexible parameter system integration (Task 5).
/// Provides parameter-based access to fractal settings while maintaining backwards compatibility
/// with existing hard-coded properties during the migration period.
/// </summary>
public partial class MainViewModel
{
    // ═══════════════════════════════════════════════════════════════════════════════
    // FLEXIBLE PARAMETER SYSTEM (Task 1 Integration)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Current parameter set for the active fractal.
    /// Null if no fractal is loaded or parameters haven't been initialized.
    /// This is the NEW way of accessing fractal parameters.
    /// </summary>
    [ObservableProperty]
    private FractalParameterSet? _currentParameters;

    /// <summary>
    /// Indicates whether the parameter system is active.
    /// During migration, both old properties and new parameters coexist.
    /// Once migration is complete, this will always be true.
    /// </summary>
    public bool UseParameterSystem { get; private set; } = true;

    // ═══════════════════════════════════════════════════════════════════════════════
    // PARAMETER LIFECYCLE
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Initialize parameters for a specific fractal type.
    /// Called when fractal type changes.
    /// Automatically loads saved parameter values if they exist.
    /// </summary>
    private async void InitializeParametersForFractal(string fractalType)
    {
        try
        {
            if (_fractalParameterService == null)
            {
                Debug.WriteLine("[MainViewModel.Parameters] Parameter service not available");
                return;
            }

            // Ensure parameter service is initialized before use
            await _fractalParameterService.InitializeAsync();

            Debug.WriteLine($"[MainViewModel.Parameters] Initializing parameters for '{fractalType}'");

            // Load parameter set for this fractal
            var paramSet = await _fractalParameterService.GetParametersAsync(fractalType);
            if (paramSet == null)
            {
                Debug.WriteLine($"[MainViewModel.Parameters] Warning: No parameters found for '{fractalType}'");
                return;
            }

            // Subscribe to parameter changes
            if (CurrentParameters != null)
            {
                CurrentParameters.ParameterChanged -= OnParameterValueChanged;
            }

            CurrentParameters = paramSet;
            CurrentParameters.ParameterChanged += OnParameterValueChanged;

            Debug.WriteLine($"[MainViewModel.Parameters] Loaded {paramSet.Parameters.Count} parameters for '{fractalType}'");

            // TASK 6: Try to restore saved parameter values from LocalSettings
            var restored = CurrentParameters.LoadFromSettings();
            if (restored)
            {
                Debug.WriteLine($"[MainViewModel.Parameters] Restored saved parameter values for '{fractalType}'");

                // PHASE 5: SyncParametersToProperties() removed - legacy properties no longer needed
                Debug.WriteLine("[PHASE 5] Legacy property sync removed - UI binds directly to CurrentParameters");
            }
            else
            {
                Debug.WriteLine($"[MainViewModel.Parameters] No saved parameters found, using defaults for '{fractalType}'");

                // PHASE 5: Legacy sync removed - flexible parameter system is now the single source of truth
                Debug.WriteLine("[PHASE 5] SyncPropertiesToParameters() call removed - using parameter defaults");
            }
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine($"[MainViewModel.Parameters] Error initializing parameters: {ex.Message}");
            Debug.WriteLine($"[MainViewModel.Parameters] Stack trace: {ex.StackTrace}");
        }
    }

    /// <summary>
    /// Handle parameter value changes from the flexible parameter system.
    /// This is called when parameters are modified via the new API.
    /// TASK 6: Auto-saves parameters to LocalSettings on change (with debounce to avoid excessive saves).
    /// </summary>
    private void OnParameterValueChanged(object? sender, ParameterChangedEventArgs e)
    {
        Debug.WriteLine($"[MainViewModel.Parameters] Parameter '{e.ParameterKey}' changed: {e.OldValue} → {e.NewValue}");

        // PHASE 5: Legacy property sync removed - UI components bind directly to CurrentParameters
        Debug.WriteLine("[PHASE 5] SyncParametersToProperties() call removed - parameter changes propagate via INotifyPropertyChanged");

        // TASK 6: Auto-save parameters to LocalSettings
        // Only save if not currently syncing (to avoid save loops during initialization)
        if (CurrentParameters != null && UseParameterSystem)
        {
            CurrentParameters.SaveToSettings();
        }

        // Future: Trigger auto-render on parameter change
        // For now, keep existing behavior (user clicks Render button)
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // PHASE 5: BACKWARDS COMPATIBILITY BRIDGE REMOVED
    // ═══════════════════════════════════════════════════════════════════════════════
    //
    // REMOVED: SyncPropertiesToParameters() - Step 5.2 ✅
    //   - No longer needed; flexible parameter system is single source of truth
    //   - Parameter defaults come from FractalParameterService templates
    //
    // REMOVED: SyncParametersToProperties() - Step 5.3 ✅
    //   - No longer needed; UI binds directly to CurrentParameters
    //   - Legacy hard-coded properties will be removed in Step 5.4
    //   - Parameter changes propagate via INotifyPropertyChanged on FractalParameterSet
    //
    // Migration complete: All parameter access goes through CurrentParameters property.
    // ═══════════════════════════════════════════════════════════════════════════════

    // ═══════════════════════════════════════════════════════════════════════════════
    // MIGRATION HELPERS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Get a parameter value with type safety and fallback.
    /// Convenience method for accessing parameters during migration.
    /// </summary>
    public T? GetParameter<T>(string key, T? fallback = default)
    {
        if (CurrentParameters == null)
            return fallback;

        var value = CurrentParameters.GetValue<T>(key);
        return value ?? fallback;
    }

    /// <summary>
    /// Set a parameter value with validation.
    /// Returns true if successful, false if validation failed.
    /// </summary>
    public bool SetParameter(string key, object value)
    {
        if (CurrentParameters == null)
            return false;

        return CurrentParameters.SetValue(key, value);
    }

    /// <summary>
    /// Validate current parameter set.
    /// Returns true if all parameters are valid.
    /// </summary>
    public bool ValidateCurrentParameters()
    {
        if (CurrentParameters == null)
            return true; // No parameters = nothing to validate

        var isValid = CurrentParameters.Validate();
        if (!isValid)
        {
            var errors = CurrentParameters.GetAllValidationErrors();
            foreach (var error in errors)
            {
                Debug.WriteLine($"[MainViewModel.Parameters] Validation error: {error.Key} = {error.Value}");
            }
        }

        return isValid;
    }

    /// <summary>
    /// TASK 6: Reset current parameters to their default values and clear saved settings.
    /// Useful when user wants to start fresh with a fractal.
    /// </summary>
    public void ResetParametersToDefaults()
    {
        if (CurrentParameters == null)
            return;

        Debug.WriteLine($"[MainViewModel.Parameters] Resetting parameters to defaults for '{CurrentParameters.FractalType}'");

        // Clear saved settings
        CurrentParameters.ClearSavedSettings();

        // Reset each parameter to its default value
        foreach (var descriptor in CurrentParameters.Parameters)
        {
            if (descriptor.DefaultValue != null)
            {
                CurrentParameters.SetValue(descriptor.Key, descriptor.DefaultValue);
            }
        }

        // PHASE 5: Legacy property sync removed - parameter changes propagate automatically
        Debug.WriteLine("[PHASE 5] SyncParametersToProperties() call removed - defaults applied directly to CurrentParameters");

        Debug.WriteLine($"[MainViewModel.Parameters] Parameters reset to defaults");
    }

    /// <summary>
    /// Dump current parameters to debug output.
    /// Useful for troubleshooting parameter sync issues.
    /// </summary>
    public void DumpCurrentParameters()
    {
        if (CurrentParameters == null)
        {
            Debug.WriteLine("[MainViewModel.Parameters] No parameters loaded");
            return;
        }

        CurrentParameters.DumpToDebug();
    }
}
