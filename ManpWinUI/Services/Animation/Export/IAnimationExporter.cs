using ManpWinUI.Models.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ManpWinUI.Services.Animation.Export;

/// <summary>
/// Interface for animation exporters (MP4, GIF, PNG sequence, etc.).
/// </summary>
public interface IAnimationExporter
{
    /// <summary>
    /// Export format supported by this exporter.
    /// </summary>
    ExportFormat SupportedFormat { get; }

    /// <summary>
    /// Export rendered frames to a file.
    /// </summary>
    /// <param name="frames">Rendered animation frames</param>
    /// <param name="settings">Animation configuration (includes output path and frame rate)</param>
    /// <param name="progress">Progress reporting callback</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Path to exported file</returns>
    Task<string> ExportAsync(
        List<WriteableBitmap> frames,
        AnimationSettings settings,
        IProgress<AnimationProgress>? progress = null,
        CancellationToken cancellationToken = default);
}
