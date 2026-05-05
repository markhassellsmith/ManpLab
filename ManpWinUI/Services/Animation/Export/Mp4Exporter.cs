using ManpWinUI.Models.Animation;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace ManpWinUI.Services.Animation.Export;

/// <summary>
/// Exports animations to MP4 format using FFmpeg/H.264 codec.
/// High quality, universal playback compatibility.
/// </summary>
public class Mp4Exporter : IAnimationExporter
{
    private readonly ILogger<Mp4Exporter> _logger;

    public ExportFormat SupportedFormat => ExportFormat.MP4;

    public Mp4Exporter(ILogger<Mp4Exporter> logger)
    {
        _logger = logger;
    }

    public async Task<string> ExportAsync(
        List<WriteableBitmap> frames,
        AnimationSettings settings,
        IProgress<AnimationProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (frames == null || frames.Count == 0)
        {
            throw new ArgumentException("No frames to export", nameof(frames));
        }

        _logger.LogInformation(
            "Exporting {FrameCount} frames to MP4: {OutputPath} at {FrameRate} fps",
            frames.Count,
            settings.OutputPath,
            settings.FrameRate);

        // Create temporary directory for frame PNGs
        var tempDir = Path.Combine(Path.GetTempPath(), $"ManpWinUI_Animation_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        _logger.LogDebug("Created temporary directory: {TempDir}", tempDir);

        try
        {
            // Phase 1: Save frames as numbered PNGs
            _logger.LogInformation("Phase 1: Saving {FrameCount} frames as PNGs...", frames.Count);
            await SaveFramesAsPngAsync(frames, tempDir, progress, cancellationToken);

            // Phase 2: Ensure FFmpeg is available
            await EnsureFfmpegAsync(cancellationToken);

            // Phase 3: Use FFmpeg to create MP4
            _logger.LogInformation("Phase 2: Encoding MP4 with FFmpeg...");
            await EncodeToMp4Async(tempDir, settings, progress, cancellationToken);

            _logger.LogInformation("MP4 export complete: {OutputPath}", settings.OutputPath);
            return settings.OutputPath;
        }
        finally
        {
            // Cleanup temp files
            try
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, recursive: true);
                    _logger.LogDebug("Cleaned up temporary directory: {TempDir}", tempDir);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to cleanup temporary directory: {TempDir}", tempDir);
            }
        }
    }

    /// <summary>
    /// Save all frames as numbered PNG files.
    /// </summary>
    private async Task SaveFramesAsPngAsync(
        List<WriteableBitmap> frames,
        string outputDirectory,
        IProgress<AnimationProgress>? progress,
        CancellationToken cancellationToken)
    {
        for (int i = 0; i < frames.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var framePath = Path.Combine(outputDirectory, $"frame_{i:D6}.png");
            await SaveFrameAsPngAsync(frames[i], framePath);

            progress?.Report(new AnimationProgress
            {
                CurrentFrame = i + 1,
                TotalFrames = frames.Count,
                Phase = "Saving Frames",
                StatusMessage = $"Saved {i + 1}/{frames.Count} frames"
            });
        }

        _logger.LogDebug("Saved {FrameCount} PNG frames to {Directory}", frames.Count, outputDirectory);
    }

    /// <summary>
    /// Save a single WriteableBitmap to PNG file.
    /// </summary>
    private async Task SaveFrameAsPngAsync(WriteableBitmap bitmap, string filePath)
    {
        // Get pixel data from bitmap
        var pixelData = bitmap.PixelBuffer.ToArray();

        // WinUI WriteableBitmap is in BGRA format, need to convert to RGBA for PNG
        // Actually, WinRT encoder handles this, so we can write directly

        // Use WinRT BitmapEncoder
        var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(filePath);
        using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
        {
            var encoder = await Windows.Graphics.Imaging.BitmapEncoder.CreateAsync(
                Windows.Graphics.Imaging.BitmapEncoder.PngEncoderId,
                stream);

            encoder.SetPixelData(
                Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8,
                Windows.Graphics.Imaging.BitmapAlphaMode.Premultiplied,
                (uint)bitmap.PixelWidth,
                (uint)bitmap.PixelHeight,
                96.0, // DPI X
                96.0, // DPI Y
                pixelData);

            await encoder.FlushAsync();
        }
    }

    /// <summary>
    /// Ensure FFmpeg binaries are available.
    /// For now, requires FFmpeg to be installed on the system PATH.
    /// Future: Implement automatic download.
    /// </summary>
    private Task EnsureFfmpegAsync(CancellationToken cancellationToken)
    {
        // Try to find FFmpeg on PATH
        var ffmpegPath = FindFfmpegInPath();

        if (ffmpegPath == null)
        {
            _logger.LogError("FFmpeg not found on system PATH");
            throw new InvalidOperationException(
                "FFmpeg is required for MP4 export but was not found. " +
                "Please install FFmpeg and add it to your system PATH. " +
                "Download from: https://ffmpeg.org/download.html");
        }

        _logger.LogInformation("FFmpeg found: {FFmpegPath}", ffmpegPath);
        FFmpeg.SetExecutablesPath(Path.GetDirectoryName(ffmpegPath)!);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Find FFmpeg executable in system PATH.
    /// </summary>
    private string? FindFfmpegInPath()
    {
        var pathEnv = Environment.GetEnvironmentVariable("PATH");
        if (pathEnv == null)
            return null;

        var paths = pathEnv.Split(Path.PathSeparator);

        foreach (var path in paths)
        {
            try
            {
                var ffmpegPath = Path.Combine(path, "ffmpeg.exe");
                if (File.Exists(ffmpegPath))
                    return ffmpegPath;
            }
            catch
            {
                // Skip invalid paths
            }
        }

        return null;
    }

    /// <summary>
    /// Encode PNG frames to MP4 using FFmpeg.
    /// </summary>
    private async Task EncodeToMp4Async(
        string frameDirectory,
        AnimationSettings settings,
        IProgress<AnimationProgress>? progress,
        CancellationToken cancellationToken)
    {
        // Input pattern: frame_%06d.png (zero-padded 6-digit frame numbers)
        var inputPattern = Path.Combine(frameDirectory, "frame_%06d.png");

        _logger.LogDebug(
            "FFmpeg input pattern: {InputPattern}, output: {OutputPath}, framerate: {FrameRate}",
            inputPattern,
            settings.OutputPath,
            settings.FrameRate);

        // Build FFmpeg conversion
        var conversion = FFmpeg.Conversions.New()
            .AddParameter($"-framerate {settings.FrameRate}") // Input framerate
            .AddParameter($"-i \"{inputPattern}\"") // Input files
            .AddParameter("-c:v libx264") // H.264 codec
            .AddParameter("-preset medium") // Encoding speed/quality balance
            .AddParameter("-crf 18") // Quality (0-51, lower=better, 18=visually lossless)
            .AddParameter("-pix_fmt yuv420p") // Pixel format (compatible with most players)
            .AddParameter($"-r {settings.FrameRate}") // Output framerate
            .SetOutput(settings.OutputPath);

        _logger.LogDebug("FFmpeg command: {Command}", conversion.Build());

        // Set up progress monitoring
        conversion.OnProgress += (sender, args) =>
        {
            // FFmpeg reports progress as percentage
            progress?.Report(new AnimationProgress
            {
                CurrentFrame = (int)(args.Percent * settings.FrameCount / 100.0),
                TotalFrames = settings.FrameCount,
                Phase = "Encoding MP4",
                StatusMessage = $"Encoding: {args.Percent:F1}%"
            });
        };

        // Execute conversion
        await conversion.Start(cancellationToken);

        _logger.LogInformation("FFmpeg encoding complete: {OutputPath}", settings.OutputPath);
    }
}
