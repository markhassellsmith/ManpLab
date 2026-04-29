using CommunityToolkit.Mvvm.Input;

namespace ManpWinUI.ViewModels;

/// <summary>
/// MainViewModel partial class - Render control commands.
/// Handles stopping and pausing/resuming render operations.
/// </summary>
public partial class MainViewModel
{
    // ═══════════════════════════════════════════════════════════════════════════════
    // RENDER CONTROL COMMANDS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Stops the current rendering operation.
    /// Clears intermediate computations and returns to pre-render state.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanStopRender))]
    private void StopRender()
    {
        System.Diagnostics.Debug.WriteLine("[StopRender] Called - Cancelling render");

        // Cancel the render operation
        _renderCancellationSource?.Cancel();

        // Reset all rendering state on UI thread
        _dispatcherQueue.TryEnqueue(() =>
        {
            IsRendering = false;
            IsPaused = false;
            RenderProgress = 0;
            StatusMessage = "Rendering stopped by user";
            System.Diagnostics.Debug.WriteLine("[StopRender] State reset complete");
        });
    }

    /// <summary>
    /// Toggles between paused and resumed rendering state.
    /// When paused, the entire UI is locked except for Resume/Stop buttons.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanPauseResumeRender))]
    private void PauseResumeRender()
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            IsPaused = !IsPaused;

            if (IsPaused)
            {
                // TODO: Signal the native rendering engine to pause
                StatusMessage = "Rendering paused - Click Resume to continue or Stop to cancel";
            }
            else
            {
                // TODO: Signal the native rendering engine to resume
                var fractalName = IsJuliaMode ? $"{SelectedFractalType} Julia" : SelectedFractalType;
                StatusMessage = IsJuliaMode 
                    ? $"Rendering {fractalName} set (c = {JuliaCX:F4}, {JuliaCY:F4})..." 
                    : $"Rendering {fractalName} set...";
            }
        });
    }

    /// <summary>
    /// Determines whether stop can be executed (rendering is active).
    /// </summary>
    private bool CanStopRender() => IsRendering;

    /// <summary>
    /// Determines whether pause/resume can be executed (rendering is active).
    /// </summary>
    private bool CanPauseResumeRender() => IsRendering;

    /// <summary>
    /// Gets the text to display on the pause/resume button.
    /// </summary>
    public string PauseResumeButtonText => IsPaused ? "Resume" : "Pause";
}
