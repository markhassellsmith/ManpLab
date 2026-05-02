#nullable enable
using System;

namespace ManpWinUI.ViewModels.Properties
{
    /// <summary>
    /// ParameterEditorViewModel partial class - Legacy parameter loading.
    /// Contains the old LoadParametersForFractal method that loads from native registry.
    /// This exists for backward compatibility and fallback when flexible system not available.
    /// </summary>
    public partial class ParameterEditorViewModel
    {
        /// <summary>
        /// Load parameters for the selected fractal (LEGACY METHOD).
        /// Week 6 Task 2: Dynamic parameter loading based on fractal type.
        /// Week 6 Task 3: Populate parameter metadata (type, constraints, descriptions).
        /// Week 6 Task 5: Store defaults and subscribe to changes for re-rendering.
        /// NOTE: This method is kept for backward compatibility. New code should use LoadFromParameterSet.
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
        /// Load type-specific parameters based on fractal characteristics (LEGACY METHOD).
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
        /// Reset parameters to their default values (LEGACY METHOD).
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
        /// Reload last saved parameter values from persistent storage (LEGACY METHOD).
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
    }
}
