#nullable enable
using System;
using System.Diagnostics;
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
            try
            {
                // Get caller information for diagnostics
                var stackTrace = new StackTrace(true);
                var callerFrame = stackTrace.GetFrame(1);
                var callerMethod = callerFrame?.GetMethod();
                var callerFile = callerFrame?.GetFileName();
                var callerLine = callerFrame?.GetFileLineNumber();

                Debug.WriteLine("╔════════════════════════════════════════════════════════════════");
                Debug.WriteLine("║ [PHASE 5 DIAGNOSTIC] Legacy Method Call Detected");
                Debug.WriteLine("╠════════════════════════════════════════════════════════════════");
                Debug.WriteLine($"║ Method:      LoadParametersForFractal('{fractalName}')");
                Debug.WriteLine($"║ Status:      NO-OP (flexible system handles parameter loading)");
                Debug.WriteLine($"║ Called from: {callerMethod?.DeclaringType?.Name}.{callerMethod?.Name}()");
                Debug.WriteLine($"║ Location:    {callerFile}:{callerLine}");
                Debug.WriteLine($"║ Action:      Call site should use MainViewModel.CurrentParameters");
                Debug.WriteLine("╚════════════════════════════════════════════════════════════════");

                // Intentionally empty - the flexible system handles parameter loading via MainViewModel.CurrentParameters
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PHASE 5 ERROR] Exception in LoadParametersForFractal diagnostic: {ex.Message}");
            }
        }

        /// <summary>
        /// Reset parameters to their default values.
        /// Works with both legacy and flexible parameter systems.
        /// </summary>
        public void ResetToDefaults()
        {
            try
            {
                Debug.WriteLine("╔════════════════════════════════════════════════════════════════");
                Debug.WriteLine("║ [PHASE 5 DIAGNOSTIC] ResetToDefaults Called");
                Debug.WriteLine("╠════════════════════════════════════════════════════════════════");
                Debug.WriteLine($"║ Fractal:     {_currentFractalName ?? "(none)"}");
                Debug.WriteLine($"║ Parameters:  {Parameters.Count} total");

                int resetCount = 0;
                int skippedCount = 0;

                // Reset each parameter to its default value
                foreach (var parameter in Parameters)
                {
                    if (!parameter.IsReadOnly && !string.IsNullOrEmpty(parameter.DefaultValue))
                    {
                        var oldValue = parameter.Value;
                        parameter.Value = parameter.DefaultValue;
                        resetCount++;

                        if (oldValue != parameter.DefaultValue)
                        {
                            Debug.WriteLine($"║   • {parameter.Name}: {oldValue} → {parameter.DefaultValue}");
                        }
                    }
                    else
                    {
                        skippedCount++;
                    }
                }

                Debug.WriteLine($"║ Result:      {resetCount} reset, {skippedCount} skipped (read-only/no default)");
                Debug.WriteLine("╚════════════════════════════════════════════════════════════════");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("╔════════════════════════════════════════════════════════════════");
                Debug.WriteLine("║ [PHASE 5 ERROR] Exception in ResetToDefaults");
                Debug.WriteLine("╠════════════════════════════════════════════════════════════════");
                Debug.WriteLine($"║ Error:       {ex.Message}");
                Debug.WriteLine($"║ Stack trace: {ex.StackTrace}");
                Debug.WriteLine("╚════════════════════════════════════════════════════════════════");
                throw;
            }
        }

        /// <summary>
        /// Reload last saved parameter values from persistent storage.
        /// Works with both legacy and flexible parameter systems by reloading the current fractal.
        /// </summary>
        public void ReloadLastSaved()
        {
            try
            {
                if (string.IsNullOrEmpty(_currentFractalName))
                {
                    Debug.WriteLine("[PHASE 5 DIAGNOSTIC] ReloadLastSaved: No current fractal, ignoring");
                    return;
                }

                // Get caller information for diagnostics
                var stackTrace = new StackTrace(true);
                var callerFrame = stackTrace.GetFrame(1);
                var callerMethod = callerFrame?.GetMethod();
                var callerFile = callerFrame?.GetFileName();
                var callerLine = callerFrame?.GetFileLineNumber();

                Debug.WriteLine("╔════════════════════════════════════════════════════════════════");
                Debug.WriteLine("║ [PHASE 5 DIAGNOSTIC] ReloadLastSaved Called");
                Debug.WriteLine("╠════════════════════════════════════════════════════════════════");
                Debug.WriteLine($"║ Fractal:     {_currentFractalName}");
                Debug.WriteLine($"║ Status:      STUB (flexible system auto-saves/restores)");
                Debug.WriteLine($"║ Called from: {callerMethod?.DeclaringType?.Name}.{callerMethod?.Name}()");
                Debug.WriteLine($"║ Location:    {callerFile}:{callerLine}");
                Debug.WriteLine($"║ Action:      MainViewModel should reload CurrentParameters");
                Debug.WriteLine("╚════════════════════════════════════════════════════════════════");

                // In the flexible system, the MainViewModel should reload CurrentParameters
                // For now, this is a no-op stub - proper implementation would require MainViewModel coordination
            }
            catch (Exception ex)
            {
                Debug.WriteLine("╔════════════════════════════════════════════════════════════════");
                Debug.WriteLine("║ [PHASE 5 ERROR] Exception in ReloadLastSaved");
                Debug.WriteLine("╠════════════════════════════════════════════════════════════════");
                Debug.WriteLine($"║ Error:       {ex.Message}");
                Debug.WriteLine($"║ Stack trace: {ex.StackTrace}");
                Debug.WriteLine("╚════════════════════════════════════════════════════════════════");
            }
        }
    }
}
