using ManpWinUI.Models.Animation;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ManpWinUI.Services.Animation;

/// <summary>
/// High-level service for creating and exporting fractal animations.
/// Coordinates rendering, encoding, and export operations.
/// </summary>
public class AnimationService
{
    private readonly AnimationRenderer _renderer;
    private readonly ILogger<AnimationService> _logger;

    public AnimationService(
        AnimationRenderer renderer,
        ILogger<AnimationService> logger)
    {
        _renderer = renderer;
        _logger = logger;
    }

    /// <summary>
    /// Create a complete animation: render frames and export to file.
    /// </summary>
    /// <param name="settings">Animation configuration</param>
    /// <param name="progress">Progress reporting callback</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Path to exported animation file</returns>
    public async Task<string> CreateAnimationAsync(
        AnimationSettings settings,
        IProgress<AnimationProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Creating animation: {Type} with {FrameCount} frames, exporting to {Format}",
            settings.Type,
            settings.FrameCount,
            settings.ExportFormat);

        try
        {
            // Phase 1: Render frames
            _logger.LogInformation("Phase 1: Rendering {FrameCount} frames...", settings.FrameCount);

            var frames = await _renderer.RenderFramesAsync(
                settings,
                progress,
                cancellationToken);

            _logger.LogInformation("Rendering complete. {FrameCount} frames ready for export.", frames.Count);

            // Phase 2: Export (will be implemented in Task 1.3)
            progress?.Report(new AnimationProgress
            {
                CurrentFrame = frames.Count,
                TotalFrames = frames.Count,
                Phase = "Export Ready",
                StatusMessage = $"Rendered {frames.Count} frames. Export functionality coming in Task 1.3."
            });

            _logger.LogInformation(
                "Animation creation complete. Frames stored in memory (export pending Task 1.3).");

            // TODO: Task 1.3 will add export functionality here

            return settings.OutputPath; // Return intended output path
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Animation creation cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create animation");
            throw;
        }
    }

    /// <summary>
    /// Render frames only (no export). Useful for preview or timeline scrubbing.
    /// </summary>
    /// <param name="settings">Animation configuration</param>
    /// <param name="progress">Progress reporting callback</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of rendered frame bitmaps</returns>
    public async Task<List<WriteableBitmap>> RenderFramesOnlyAsync(
        AnimationSettings settings,
        IProgress<AnimationProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Rendering {FrameCount} frames (no export)", settings.FrameCount);

        return await _renderer.RenderFramesAsync(
            settings,
            progress,
            cancellationToken);
    }

    /// <summary>
    /// Estimate rendering time based on a test frame.
    /// Useful for showing users how long an animation will take.
    /// </summary>
    /// <param name="settings">Animation configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Estimated total rendering time</returns>
    public async Task<TimeSpan> EstimateRenderTimeAsync(
        AnimationSettings settings,
        CancellationToken cancellationToken = default)
    {
        if (settings.StartParameters == null)
        {
            throw new ArgumentException("Start parameters required for estimation", nameof(settings));
        }

        _logger.LogInformation("Estimating render time by rendering test frame...");

        var testSettings = new AnimationSettings
        {
            Type = settings.Type,
            FrameCount = 1,
            FrameRate = settings.FrameRate,
            Easing = settings.Easing,
            ExportFormat = settings.ExportFormat,
            OutputPath = settings.OutputPath,
            BaseParameters = settings.BaseParameters,
            StartParameters = settings.StartParameters,
            EndParameters = settings.StartParameters, // Same as start for single frame
            ZoomSettings = settings.ZoomSettings,
            PanSettings = settings.PanSettings,
            ParameterSettings = settings.ParameterSettings,
            ColorSettings = settings.ColorSettings
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var testFrames = await _renderer.RenderFramesAsync(
            testSettings,
            progress: null,
            cancellationToken);

        stopwatch.Stop();

        _renderer.DisposeFrames(testFrames);

        var estimatedTotal = TimeSpan.FromMilliseconds(
            stopwatch.Elapsed.TotalMilliseconds * settings.FrameCount);

        _logger.LogInformation(
            "Test frame rendered in {TestTime:F2}s. Estimated total: {EstimatedTime:F2}s for {FrameCount} frames",
            stopwatch.Elapsed.TotalSeconds,
            estimatedTotal.TotalSeconds,
            settings.FrameCount);

        return estimatedTotal;
    }
}
