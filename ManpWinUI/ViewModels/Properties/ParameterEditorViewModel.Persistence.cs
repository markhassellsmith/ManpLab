#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ManpWinUI.ViewModels.Properties
{
    /// <summary>
    /// ParameterEditorViewModel partial class - Persistence functionality.
    /// Contains methods for saving, loading, and restoring parameter values.
    /// </summary>
    public partial class ParameterEditorViewModel
    {
        /// <summary>
        /// Save current parameter values to persistent storage.
        /// Week 6 Task 6: Persist parameters per fractal type.
        /// </summary>
        public void SaveParameters()
        {
            if (_settingsService == null || string.IsNullOrEmpty(_currentFractalName))
                return;

            try
            {
                // Create dictionary of editable parameter values
                var parameterValues = new Dictionary<string, string>();
                foreach (var parameter in Parameters)
                {
                    if (!parameter.IsReadOnly)
                    {
                        parameterValues[parameter.Name] = parameter.Value;
                    }
                }

                // Serialize to JSON
                var json = JsonSerializer.Serialize(parameterValues);
                _settingsService.SetFractalParameters(_currentFractalName, json);

                System.Diagnostics.Debug.WriteLine($"[ParameterEditorViewModel] Saved {parameterValues.Count} parameters for {_currentFractalName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ParameterEditorViewModel] Error saving parameters: {ex.Message}");
            }
        }

        /// <summary>
        /// Restore saved parameter values from persistent storage.
        /// Week 6 Task 6: Load last-used parameters for this fractal.
        /// </summary>
        private void RestoreParameters()
        {
            if (_settingsService == null || string.IsNullOrEmpty(_currentFractalName))
                return;

            try
            {
                var json = _settingsService.GetFractalParameters(_currentFractalName);
                if (string.IsNullOrEmpty(json))
                {
                    System.Diagnostics.Debug.WriteLine($"[ParameterEditorViewModel] No saved parameters for {_currentFractalName}, using defaults");
                    return;
                }

                // Deserialize from JSON
                var parameterValues = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (parameterValues == null)
                    return;

                // Apply saved values to parameters
                foreach (var parameter in Parameters)
                {
                    if (!parameter.IsReadOnly && parameterValues.TryGetValue(parameter.Name, out var savedValue))
                    {
                        // Temporarily unsubscribe to avoid triggering re-render during load
                        parameter.ValueChanged -= OnParameterValueChanged;
                        parameter.Value = savedValue;
                        parameter.ValueChanged += OnParameterValueChanged;
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[ParameterEditorViewModel] Restored {parameterValues.Count} saved parameters for {_currentFractalName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ParameterEditorViewModel] Error restoring parameters: {ex.Message}");
            }
        }
    }
}
