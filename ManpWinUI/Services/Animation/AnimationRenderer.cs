using ManpWinUI.Models.Animation;
using ManpWinUI.Models.Parameters;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;

namespace ManpWinUI.Services.Animation;

/// <summary>
/// Renders individual animation frames by interpolating parameters and calling the fractal renderer.
/// </summary>
public class AnimationRenderer
{
    private readonly IFractalRenderService _fractalRenderer;
    private readonly FrameInterpolator _interpolator;
    private readonly ILogger<AnimationRenderer> _logger;

    public AnimationRenderer(
        IFractalRenderService fractalRenderer,
        FrameInterpolator interpolator,
        ILogger<AnimationRenderer> logger)
    {
        _fractalRenderer = fractalRenderer;
        _interpolator = interpolator;
        _logger = logger;
    }

    /// <summary>
    /// Render all frames for an animation sequence.
    /// </summary>
    /// <param name="settings">Animation configuration</param>
    /// <param name="progress">Progress reporting callback</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of rendered frame bitmaps</returns>
    public async Task<List<WriteableBitmap>> RenderFramesAsync(
        AnimationSettings settings,
        IProgress<AnimationProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var validation = settings.Validate();
        if (!validation.IsValid)
        {
            throw new ArgumentException(validation.ErrorMessage, nameof(settings));
        }

        if (settings.StartParameters == null || settings.EndParameters == null)
        {
            throw new ArgumentException("Start and end parameters are required.", nameof(settings));
        }

        _logger.LogInformation(
            "Starting animation render: {FrameCount} frames at {FrameRate} fps ({Duration:F2}s), type: {Type}",
            settings.FrameCount,
            settings.FrameRate,
            settings.DurationSeconds,
            settings.Type);

        var frames = new List<WriteableBitmap>(settings.FrameCount);
        var stopwatch = Stopwatch.StartNew();
        var frameTimes = new List<TimeSpan>();

        try
        {
            for (int i = 0; i < settings.FrameCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var frameStopwatch = Stopwatch.StartNew();

                // Calculate normalized time for this frame
                double t = _interpolator.GetNormalizedTime(i, settings.FrameCount);

                // Interpolate parameters for this frame
                var frameParams = _interpolator.InterpolateFrame(
                    settings.StartParameters,
                    settings.EndParameters,
                    t,
                    settings.Easing);

                _logger.LogDebug(
                    "Rendering frame {Frame}/{Total} (t={T:F4}): center=({CenterX:F6}, {CenterY:F6}), zoom={Zoom:F2}",
                    i + 1,
                    settings.FrameCount,
                    t,
                    frameParams.CenterX,
                    frameParams.CenterY,
                    frameParams.Zoom);

                // Render the frame
                var renderResult = await _fractalRenderer.RenderFractalAsync(
                    frameParams,
                    progress: null, // Don't report sub-progress for individual frames
                    cancellationToken);

                // Convert pixel data to WriteableBitmap
                var bitmap = await ConvertToWriteableBitmapAsync(renderResult);
                frames.Add(bitmap);

                frameStopwatch.Stop();
                frameTimes.Add(frameStopwatch.Elapsed);

                // Report progress
                if (progress != null)
                {
                    var avgFrameTime = TimeSpan.FromMilliseconds(
                        frameTimes.Average(ts => ts.TotalMilliseconds));

                    var remainingFrames = settings.FrameCount - (i + 1);
                    var estimatedRemaining = TimeSpan.FromMilliseconds(
                        avgFrameTime.TotalMilliseconds * remainingFrames);

                    progress.Report(new AnimationProgress
                    {
                        CurrentFrame = i + 1,
                        TotalFrames = settings.FrameCount,
                        Phase = "Rendering",
                        StatusMessage = $"Frame {i + 1}/{settings.FrameCount}",
                        AverageFrameTime = avgFrameTime,
                        EstimatedTimeRemaining = estimatedRemaining
                    });
                }
            }

            stopwatch.Stop();

            _logger.LogInformation(
                "Animation rendering complete: {FrameCount} frames in {TotalTime:F2}s (avg {AvgTime:F2}s/frame)",
                settings.FrameCount,
                stopwatch.Elapsed.TotalSeconds,
                stopwatch.Elapsed.TotalSeconds / settings.FrameCount);

            return frames;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Animation rendering cancelled after {FrameCount} frames", frames.Count);

            // Dispose partial frames
            foreach (var frame in frames)
            {
                // WriteableBitmap doesn't implement IDisposable in WinUI 3
                // Memory will be managed by GC
            }

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during animation rendering at frame {FrameIndex}", frames.Count + 1);

            // Dispose partial frames
            foreach (var frame in frames)
            {
                // WriteableBitmap doesn't implement IDisposable in WinUI 3
            }

            throw;
        }
    }

    /// <summary>
    /// Convert render result pixel data to WriteableBitmap.
    /// Must be called on UI thread or with dispatcher access.
    /// </summary>
    private Task<WriteableBitmap> ConvertToWriteableBitmapAsync(FractalRenderResult renderResult)
    {
        // Create bitmap on UI thread
        var bitmap = new WriteableBitmap(renderResult.Width, renderResult.Height);

        // Copy pixel data directly to buffer
        using var stream = bitmap.PixelBuffer.AsStream();
        stream.Write(renderResult.PixelData, 0, renderResult.PixelData.Length);

        // Invalidate to trigger redraw
        bitmap.Invalidate();

        return Task.FromResult(bitmap);
    }

    /// <summary>
    /// Dispose a list of bitmaps (for cleanup).
    /// </summary>
    public void DisposeFrames(List<WriteableBitmap> frames)
    {
        // WriteableBitmap in WinUI 3 doesn't implement IDisposable
        // Clear the list to allow GC to reclaim memory
        frames.Clear();

        _logger.LogDebug("Cleared {FrameCount} frames for garbage collection", frames.Count);
    }
}
