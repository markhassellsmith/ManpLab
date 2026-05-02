#nullable enable
using System;
using System.Linq;
using ManpWinUI.Models.Parameters;

namespace ManpWinUI.ViewModels.Properties
{
    /// <summary>
    /// ParameterEditorViewModel partial class - Flexible parameter system integration (Task 7).
    /// Contains methods for loading and syncing with the flexible parameter system.
    /// This is the NEW architecture that replaces hard-coded parameter loading.
    /// </summary>
    public partial class ParameterEditorViewModel
    {
        /// <summary>
        /// Load parameters from the flexible parameter system (Task 1-6 architecture).
        /// Converts FractalParameterDescriptor objects to ParameterItem objects for UI binding.
        /// This is the NEW way - replaces LoadParametersForFractal for parameter-system fractals.
        /// </summary>
        public void LoadFromParameterSet(FractalParameterSet parameterSet)
        {
            if (parameterSet == null)
            {
                Parameters.Clear();
                return;
            }

            System.Diagnostics.Debug.WriteLine($"[ParameterEditorViewModel] Loading from parameter set: {parameterSet.FractalType}");

            _currentFractalName = parameterSet.FractalType;
            Parameters.Clear();

            // Group parameters by category for organized display
            var categorizedParams = parameterSet.GetParametersByCategory();

            foreach (var categoryGroup in categorizedParams.OrderBy(kvp => kvp.Key))
            {
                // Add category header (if more than one category)
                if (categorizedParams.Count > 1)
                {
                    Parameters.Add(new ParameterItem
                    {
                        Name = $"— {categoryGroup.Key} —",
                        Value = string.Empty,
                        Type = ParameterType.String,
                        IsReadOnly = true,
                        Description = $"Parameters in {categoryGroup.Key} category"
                    });
                }

                // Add parameters in this category
                foreach (var descriptor in categoryGroup.Value)
                {
                    var paramItem = ConvertDescriptorToParameterItem(descriptor, parameterSet);
                    if (paramItem != null)
                    {
                        if (descriptor.IsEditable)
                        {
                            AddEditableParameter(paramItem);
                        }
                        else
                        {
                            Parameters.Add(paramItem);
                        }
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"[ParameterEditorViewModel] Loaded {Parameters.Count} parameter UI items from flexible system");
        }

        /// <summary>
        /// Convert FractalParameterDescriptor to ParameterItem for UI binding.
        /// Maps the flexible parameter model to the existing UI model.
        /// </summary>
        private ParameterItem? ConvertDescriptorToParameterItem(
            FractalParameterDescriptor descriptor,
            FractalParameterSet parameterSet)
        {
            var value = parameterSet.GetValue(descriptor.Key);
            if (value == null)
                return null;

            // Map flexible ParameterType to UI ParameterType
            var uiType = descriptor.Type switch
            {
                Models.Parameters.ParameterType.Double => ParameterType.Double,
                Models.Parameters.ParameterType.Integer => ParameterType.Integer,
                Models.Parameters.ParameterType.Complex => ParameterType.Complex,
                Models.Parameters.ParameterType.Boolean => ParameterType.Boolean,
                Models.Parameters.ParameterType.Choice => ParameterType.Enum,
                _ => ParameterType.String
            };

            // Format value as string
            string valueStr = descriptor.Type switch
            {
                Models.Parameters.ParameterType.Double => ((double)value).ToString("F6"),
                Models.Parameters.ParameterType.Integer => value.ToString() ?? "0",
                Models.Parameters.ParameterType.Boolean => value.ToString() ?? "False",
                Models.Parameters.ParameterType.Complex => FormatComplexValue(value),
                _ => value?.ToString() ?? string.Empty
            };

            // Format default value
            string defaultStr = descriptor.DefaultValue switch
            {
                double d => d.ToString("F6"),
                int i => i.ToString(),
                bool b => b.ToString(),
                _ => descriptor.DefaultValue?.ToString() ?? string.Empty
            };

            var paramItem = new ParameterItem
            {
                Name = descriptor.Name,
                Value = valueStr,
                DefaultValue = defaultStr,
                Type = uiType,
                Description = descriptor.Description ?? string.Empty,
                IsReadOnly = !descriptor.IsEditable,
                MinValue = descriptor.MinValue as double? ?? double.MinValue,
                MaxValue = descriptor.MaxValue as double? ?? double.MaxValue,
                // Store descriptor key for reverse mapping
                Tag = descriptor.Key
            };

            return paramItem;
        }

        /// <summary>
        /// Format complex number for display.
        /// Expected format: (real, imaginary) or complex object.
        /// </summary>
        private string FormatComplexValue(object value)
        {
            // If value is already a formatted string like "-0.7,0.27", use it
            if (value is string str)
                return str;

            // If value is a tuple or complex type, extract components
            // For now, default to string representation
            return value?.ToString() ?? "0.0,0.0";
        }

        /// <summary>
        /// Update the flexible parameter system when UI parameter changes.
        /// Called when user edits a parameter in the UI.
        /// </summary>
        public void SyncParameterToSystem(ParameterItem paramItem, FractalParameterSet? parameterSet)
        {
            if (parameterSet == null || paramItem.Tag == null)
                return;

            var key = paramItem.Tag.ToString();
            if (string.IsNullOrEmpty(key))
                return;

            var descriptor = parameterSet.GetDescriptor(key);
            if (descriptor == null)
                return;

            // Convert UI value string to typed value
            object? typedValue = descriptor.Type switch
            {
                Models.Parameters.ParameterType.Double => double.TryParse(paramItem.Value, out var d) ? d : null,
                Models.Parameters.ParameterType.Integer => int.TryParse(paramItem.Value, out var i) ? i : null,
                Models.Parameters.ParameterType.Boolean => bool.TryParse(paramItem.Value, out var b) ? b : null,
                Models.Parameters.ParameterType.Complex => ParseComplexValue(paramItem.Value),
                _ => paramItem.Value
            };

            if (typedValue != null)
            {
                parameterSet.SetValue(key, typedValue);
                System.Diagnostics.Debug.WriteLine($"[ParameterEditorViewModel] Synced '{key}' = {typedValue} to parameter system");
            }
        }

        /// <summary>
        /// Parse complex value from string format "real,imaginary".
        /// </summary>
        private object? ParseComplexValue(string value)
        {
            var parts = value.Split(',');
            if (parts.Length == 2 &&
                double.TryParse(parts[0].Trim(), out var real) &&
                double.TryParse(parts[1].Trim(), out var imag))
            {
                // Return as tuple for now - future: use proper Complex type
                return (real, imag);
            }
            return null;
        }
    }
}
