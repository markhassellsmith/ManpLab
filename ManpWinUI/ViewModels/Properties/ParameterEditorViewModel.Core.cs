#nullable enable
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        /// Tag for storing additional metadata (e.g., parameter key for flexible system).
        /// Task 7: Stores FractalParameterDescriptor.Key for reverse mapping.
        /// </summary>
        public object? Tag { get; set; }

        /// <summary>
        /// Event raised when the parameter value changes.
        /// Week 6 Task 5: For triggering re-renders on parameter updates.
        /// </summary>
        public event EventHandler? ValueChanged;

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    /// <summary>
    /// ViewModel for the parameter editor panel.
    /// Manages fractal parameters and provides UI-ready parameter items.
    /// 
    /// Split into partial classes for maintainability:
    /// - ParameterEditorViewModel.cs (this file): Core structure, ParameterItem class
    /// - ParameterEditorViewModel.Legacy.cs: Old LoadParametersForFractal method
    /// - ParameterEditorViewModel.Flexible.cs: Task 7 flexible parameter system integration
    /// - ParameterEditorViewModel.Persistence.cs: Save/load/restore functionality
    /// </summary>
    public partial class ParameterEditorViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Observable collection of parameters for the selected fractal.
        /// </summary>
        public ObservableCollection<ParameterItem> Parameters { get; } = new();

        protected string _currentFractalName = string.Empty;
        protected ManpCore.Native.FractalInfo? _currentFractalInfo;
        protected readonly IAppSettingsService? _settingsService;

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
        /// Load placeholder parameters (empty state).
        /// </summary>
        private void LoadPlaceholderParameters()
        {
            Parameters.Clear();
            Parameters.Add(new ParameterItem
            {
                Name = "Select a Fractal",
                Value = "Choose a fractal from the browser to view parameters",
                Type = ParameterType.String,
                IsReadOnly = true
            });
        }

        /// <summary>
        /// Add an editable parameter and subscribe to its ValueChanged event.
        /// Week 6 Task 5: Track changes for re-render triggering.
        /// </summary>
        protected void AddEditableParameter(ParameterItem parameter)
        {
            parameter.ValueChanged += OnParameterValueChanged;
            Parameters.Add(parameter);
        }

        /// <summary>
        /// Handle individual parameter value changes.
        /// Week 6 Task 5: Notify MainPage that a parameter changed.
        /// </summary>
        protected void OnParameterValueChanged(object? sender, EventArgs e)
        {
            ParameterChanged?.Invoke(this, EventArgs.Empty);
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

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
