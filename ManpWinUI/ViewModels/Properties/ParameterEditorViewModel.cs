#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using ManpWinUI.Services;

namespace ManpWinUI.ViewModels.Properties
{
    /// <summary>
    /// Defines the type of a parameter for rendering type-specific controls.
    /// Week 6 Task 3: Parameter metadata for dynamic control generation.
    /// </summary>
    public enum ParameterType
    {
        String,      // General text (read-only display)
        Double,      // Numeric (editable with NumberBox)
        Integer,     // Whole number (editable with NumberBox)
        Complex,     // Complex number (custom control)
        Boolean,     // Checkbox
        Enum         // ComboBox with predefined values
    }

    /// <summary>
    /// Represents a single parameter with metadata for dynamic UI generation.
    /// Week 6 Task 3: Enhanced with type, constraints, and descriptions.
    /// Week 6 Task 4: Added typed property accessors for NumberBox/CheckBox binding.
    /// Week 6 Task 5: Added default value storage and change notifications.
    /// </summary>
    public class ParameterItem : INotifyPropertyChanged
    {
        private string _value = string.Empty;

        /// <summary>
        /// Display name of the parameter.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Current value of the parameter as a string.
        /// </summary>
        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueAsDouble)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueAsBoolean)));

                    // Week 6 Task 5: Notify parent that parameter changed
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Default value for reset functionality.
        /// Week 6 Task 5: Store original values for reset.
        /// </summary>
        public string DefaultValue { get; set; } = string.Empty;

        /// <summary>
        /// Get/set value as double for NumberBox binding.
        /// Week 6 Task 4: Typed accessor for numeric controls.
        /// </summary>
        public double ValueAsDouble
        {
            get => double.TryParse(_value, out var result) ? result : 0.0;
            set
            {
                var newValue = Type == ParameterType.Integer 
                    ? Math.Round(value).ToString() 
                    : value.ToString("F6");
                if (_value != newValue)
                {
                    _value = newValue;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueAsDouble)));
                }
            }
        }

        /// <summary>
        /// Get/set value as boolean for CheckBox binding.
        /// Week 6 Task 4: Typed accessor for boolean controls.
        /// </summary>
        public bool ValueAsBoolean
        {
            get => bool.TryParse(_value, out var result) && result;
            set
            {
                var newValue = value.ToString();
                if (_value != newValue)
                {
                    _value = newValue;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueAsBoolean)));
                }
            }
        }

        /// <summary>
        /// Real part of complex number (for Complex type).
        /// Week 6 Task 4: For Julia C complex parameter input.
        /// </summary>
        public double ComplexReal
        {
            get
            {
                // Parse "real,imaginary" format
                var parts = _value.Split(',');
                return parts.Length > 0 && double.TryParse(parts[0].Trim(), out var result) ? result : 0.0;
            }
            set
            {
                var imaginary = ComplexImaginary;
                var newValue = $"{value:F6},{imaginary:F6}";
                if (_value != newValue)
                {
                    _value = newValue;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ComplexReal)));
                }
            }
        }

        /// <summary>
        /// Imaginary part of complex number (for Complex type).
        /// Week 6 Task 4: For Julia C complex parameter input.
        /// </summary>
        public double ComplexImaginary
        {
            get
            {
                // Parse "real,imaginary" format
                var parts = _value.Split(',');
                return parts.Length > 1 && double.TryParse(parts[1].Trim(), out var result) ? result : 0.0;
            }
            set
            {
                var real = ComplexReal;
                var newValue = $"{real:F6},{value:F6}";
                if (_value != newValue)
                {
                    _value = newValue;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ComplexImaginary)));
                }
            }
        }

        /// <summary>
        /// Type of the parameter (determines control type).
        /// Week 6 Task 3: Used for DataTemplateSelector in Task 4.
        /// </summary>
        public ParameterType Type { get; set; } = ParameterType.String;

        /// <summary>
        /// Description/tooltip for the parameter.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Minimum value for numeric parameters (Double/Integer).
        /// </summary>
        public double? MinValue { get; set; }

        /// <summary>
        /// Maximum value for numeric parameters (Double/Integer).
        /// </summary>
        public double? MaxValue { get; set; }

        /// <summary>
        /// Whether this parameter is read-only (metadata display only).
        /// </summary>
        public bool IsReadOnly { get; set; } = false;

        /// <summary>
        /// Whether this parameter can be edited by the user.
        /// </summary>
        public bool IsEditable => !IsReadOnly;

        /// <summary>
        /// Event raised when the parameter value changes.
        /// Week 6 Task 5: For triggering re-renders on parameter updates.
        /// </summary>
        public event EventHandler? ValueChanged;

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class ParameterEditorViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ParameterItem> Parameters { get; } = new();

        private string _currentFractalName = string.Empty;
        private ManpCore.Native.FractalInfo? _currentFractalInfo;
        private readonly IAppSettingsService? _settingsService;

        /// <summary>
        /// Event raised when a parameter changes and requires re-render.
        /// Week 6 Task 5: MainPage subscribes to trigger automatic re-rendering.
        /// </summary>
        public event EventHandler? ParameterChanged;

        public ParameterEditorViewModel()
        {
            // Start with empty parameters - will be loaded when fractal is selected
            LoadPlaceholderParameters();
        }

        /// <summary>
        /// Constructor with settings service for parameter persistence.
        /// Week 6 Task 6: Enable parameter save/restore.
        /// </summary>
        public ParameterEditorViewModel(IAppSettingsService settingsService) : this()
        {
            _settingsService = settingsService;
        }

        /// <summary>
        /// Load parameters for the selected fractal.
        /// Week 6 Task 2: Dynamic parameter loading based on fractal type.
        /// Week 6 Task 3: Populate parameter metadata (type, constraints, descriptions).
        /// Week 6 Task 5: Store defaults and subscribe to changes for re-rendering.
        /// </summary>
        public void LoadParametersForFractal(string fractalName)
        {
            if (string.IsNullOrEmpty(fractalName))
            {
                Parameters.Clear();
                _currentFractalInfo = null;
                return;
            }

            _currentFractalName = fractalName;
            Parameters.Clear();

            // Get fractal info from registry
            var fractalInfo = ManpCore.Native.FractalRegistryWrapper.GetFractalInfo(fractalName);
            if (fractalInfo == null)
            {
                Parameters.Add(new ParameterItem
                {
                    Name = "Error",
                    Value = $"Fractal '{fractalName}' not found",
                    Type = ParameterType.String,
                    IsReadOnly = true
                });
                _currentFractalInfo = null;
                return;
            }

            // Store for reset functionality
            _currentFractalInfo = fractalInfo;

            // Load common parameters (metadata - read-only)
            Parameters.Add(new ParameterItem
            {
                Name = "Fractal",
                Value = fractalInfo.DisplayName,
                Type = ParameterType.String,
                Description = "Name of the selected fractal",
                IsReadOnly = true
            });

            Parameters.Add(new ParameterItem
            {
                Name = "Category",
                Value = fractalInfo.Category,
                Type = ParameterType.String,
                Description = "Fractal category classification",
                IsReadOnly = true
            });

            // Load editable parameters (view coordinates)
            AddEditableParameter(new ParameterItem
            {
                Name = "Center X",
                Value = fractalInfo.DefaultCenterX.ToString("F6"),
                DefaultValue = fractalInfo.DefaultCenterX.ToString("F6"),
                Type = ParameterType.Double,
                Description = "Horizontal center coordinate of the view",
                MinValue = -10.0,
                MaxValue = 10.0,
                IsReadOnly = false
            });

            AddEditableParameter(new ParameterItem
            {
                Name = "Center Y",
                Value = fractalInfo.DefaultCenterY.ToString("F6"),
                DefaultValue = fractalInfo.DefaultCenterY.ToString("F6"),
                Type = ParameterType.Double,
                Description = "Vertical center coordinate of the view",
                MinValue = -10.0,
                MaxValue = 10.0,
                IsReadOnly = false
            });

            AddEditableParameter(new ParameterItem
            {
                Name = "Zoom",
                Value = fractalInfo.DefaultZoom.ToString("F2"),
                DefaultValue = fractalInfo.DefaultZoom.ToString("F2"),
                Type = ParameterType.Double,
                Description = "Magnification level (higher = more zoomed in)",
                MinValue = 0.001,
                MaxValue = 1000000.0,
                IsReadOnly = false
            });

            // Load fractal-specific parameters based on type
            LoadTypeSpecificParameters(fractalName, fractalInfo);

            // Max iterations (universal editable parameter)
            AddEditableParameter(new ParameterItem
            {
                Name = "Max Iterations",
                Value = "1000",
                DefaultValue = "1000",
                Type = ParameterType.Integer,
                Description = "Maximum calculation steps per pixel (higher = more detail)",
                MinValue = 50,
                MaxValue = 50000,
                IsReadOnly = false
            });

            System.Diagnostics.Debug.WriteLine($"[ParameterEditorViewModel] Loaded {Parameters.Count} parameters for {fractalName}");

            // Week 6 Task 6: Restore saved parameter values (overrides defaults if saved)
            RestoreParameters();
        }

        /// <summary>
        /// Load type-specific parameters based on fractal characteristics.
        /// Week 6 Task 3: Enhanced with parameter metadata.
        /// </summary>
        private void LoadTypeSpecificParameters(string fractalName, ManpCore.Native.FractalInfo fractalInfo)
        {
            // Julia support
            if (fractalInfo.SupportsJulia)
            {
                Parameters.Add(new ParameterItem
                {
                    Name = "Julia Mode",
                    Value = "Supported",
                    Type = ParameterType.String,
                    Description = "This fractal supports Julia set rendering",
                    IsReadOnly = true
                });

                AddEditableParameter(new ParameterItem
                {
                    Name = "Julia C (Real)",
                    Value = "-0.7",
                    DefaultValue = "-0.7",
                    Type = ParameterType.Double,
                    Description = "Real part of the Julia constant",
                    MinValue = -2.0,
                    MaxValue = 2.0,
                    IsReadOnly = false
                });

                AddEditableParameter(new ParameterItem
                {
                    Name = "Julia C (Imag)",
                    Value = "0.27",
                    DefaultValue = "0.27",
                    Type = ParameterType.Double,
                    Description = "Imaginary part of the Julia constant",
                    MinValue = -2.0,
                    MaxValue = 2.0,
                    IsReadOnly = false
                });
            }

            // Power parameter for Multibrot fractals
            if (fractalName.Contains("Multibrot") || fractalName.Contains("multibrot"))
            {
                var powerStr = fractalName.Replace("Multibrot", "").Replace("multibrot", "");
                if (int.TryParse(powerStr, out int power))
                {
                    AddEditableParameter(new ParameterItem
                    {
                        Name = "Power",
                        Value = power.ToString(),
                        DefaultValue = power.ToString(),
                        Type = ParameterType.Integer,
                        Description = $"Exponent in the iteration formula (z^{power})",
                        MinValue = 2,
                        MaxValue = 10,
                        IsReadOnly = false
                    });
                }
            }
        }

        /// <summary>
        /// Load placeholder parameters for initial display.
        /// </summary>
        private void LoadPlaceholderParameters()
        {
            Parameters.Clear();
            Parameters.Add(new ParameterItem
            {
                Name = "Status",
                Value = "Select a fractal from the browser",
                Type = ParameterType.String,
                Description = "No fractal currently selected",
                IsReadOnly = true
            });
        }

        /// <summary>
        /// Add an editable parameter and subscribe to its change event.
        /// Week 6 Task 5: Centralized method to wire up change notifications.
        /// </summary>
        private void AddEditableParameter(ParameterItem parameter)
        {
            parameter.ValueChanged += OnParameterValueChanged;
            Parameters.Add(parameter);
        }

        /// <summary>
        /// Handle parameter value changes and notify for re-render.
        /// Week 6 Task 5: Debounced to avoid excessive renders during rapid changes.
        /// Week 6 Task 6: Save parameters to persistent storage.
        /// </summary>
        private void OnParameterValueChanged(object? sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[ParameterEditorViewModel] Parameter changed, notifying for re-render");

            // Week 6 Task 6: Save parameters when they change
            SaveParameters();

            // Notify for re-render
            ParameterChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Reset all editable parameters to their default values.
        /// Week 6 Task 5: Restore original fractal defaults.
        /// </summary>
        public void ResetToDefaults()
        {
            if (_currentFractalInfo == null)
                return;

            System.Diagnostics.Debug.WriteLine($"[ParameterEditorViewModel] Resetting parameters to defaults for {_currentFractalName}");

            // Reset each parameter to its default value
            foreach (var parameter in Parameters)
            {
                if (!parameter.IsReadOnly && !string.IsNullOrEmpty(parameter.DefaultValue))
                {
                    parameter.Value = parameter.DefaultValue;
                }
            }
        }

        /// <summary>
        /// Reload last saved parameter values from persistent storage.
        /// Week 6 Bonus: Allow reverting to previously saved state.
        /// </summary>
        public void ReloadLastSaved()
        {
            if (string.IsNullOrEmpty(_currentFractalName))
                return;

            System.Diagnostics.Debug.WriteLine($"[ParameterEditorViewModel] Reloading last saved parameters for {_currentFractalName}");

            // Reload parameters from scratch - this will call RestoreParameters()
            // which loads from LocalSettings, effectively reverting to last saved state
            LoadParametersForFractal(_currentFractalName);
        }

        /// <summary>
        /// Get parameter value by name (for MainViewModel synchronization).
        /// Week 6 Task 5: Allow MainPage to read current parameter values.
        /// </summary>
        public string? GetParameterValue(string parameterName)
        {
            var parameter = Parameters.FirstOrDefault(p => p.Name == parameterName);
            return parameter?.Value;
        }

        /// <summary>
        /// Update parameter value by name (for MainViewModel synchronization).
        /// Week 6 Task 5: Allow MainPage to update parameters from navigation/zoom.
        /// </summary>
        public void UpdateParameterValue(string parameterName, string value)
        {
            var parameter = Parameters.FirstOrDefault(p => p.Name == parameterName);
            if (parameter != null && !parameter.IsReadOnly)
            {
                // Temporarily unsubscribe to avoid circular updates
                parameter.ValueChanged -= OnParameterValueChanged;
                parameter.Value = value;
                parameter.ValueChanged += OnParameterValueChanged;
            }
        }

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

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}