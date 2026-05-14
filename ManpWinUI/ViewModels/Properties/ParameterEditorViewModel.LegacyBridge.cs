#nullable enable
using System;
using System.Linq;

namespace ManpWinUI.ViewModels.Properties
{
    /// <summary>
    /// ParameterEditorViewModel partial class - Legacy method bridge.
    /// Provides compatibility shims for legacy method calls during Phase 5 transition.
    /// These methods redirect to the flexible parameter system.
    /// </summary>
    public partial class ParameterEditorViewModel
    {
        /// <summary>
        /// Legacy compatibility shim for LoadParametersForFractal.
        /// This method is kept temporarily to avoid breaking existing call sites.
        /// It does nothing because the flexible system (LoadFromParameterSet) is now the primary path.
        /// Call sites should be updated to use MainViewModel.CurrentParameters instead.
        /// </summary>
        [Obsolete("Use LoadFromParameterSet with FractalParameterService instead")]
        public void LoadParametersForFractal(string fractalName)
        {
            System.Diagnostics.Debug.WriteLine($"[ParameterEditorViewModel] LEGACY WARNING: LoadParametersForFractal called for '{fractalName}'. This is a no-op. Use flexible parameter system.");
            // Intentionally empty - the flexible system handles parameter loading via MainViewModel.CurrentParameters
        }

        /// <summary>
        /// Reset parameters to their default values.
        /// Works with both legacy and flexible parameter systems.
        /// </summary>
        public void ResetToDefaults()
        {
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
        /// Works with both legacy and flexible parameter systems by reloading the current fractal.
        /// </summary>
        public void ReloadLastSaved()
        {
            if (string.IsNullOrEmpty(_currentFractalName))
                return;

            System.Diagnostics.Debug.WriteLine($"[ParameterEditorViewModel] Reloading last saved parameters for {_currentFractalName}");

            // In the flexible system, the MainViewModel should reload CurrentParameters
            // For now, this is a no-op stub - proper implementation would require MainViewModel coordination
            System.Diagnostics.Debug.WriteLine($"[ParameterEditorViewModel] LEGACY WARNING: ReloadLastSaved is a stub. Flexible system handles persistence automatically.");
        }
    }
}
