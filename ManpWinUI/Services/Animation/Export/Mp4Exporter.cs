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
using Xabe.FFmpeg.Downloader;

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

            // Allow file system to fully flush all writes before FFmpeg reads
            // This prevents "Cannot find the file" or corrupt frame issues
            await Task.Delay(500, cancellationToken);
            _logger.LogDebug("File system flush delay complete");

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
        var savedFiles = new List<string>();

        for (int i = 0; i < frames.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var framePath = Path.Combine(outputDirectory, $"frame_{i:D6}.png");
            await SaveFrameAsPngAsync(frames[i], framePath);

            // Verify file was created and has content
            if (!File.Exists(framePath))
            {
                throw new InvalidOperationException($"Failed to create frame file: {framePath}");
            }

            var fileInfo = new FileInfo(framePath);
            if (fileInfo.Length == 0)
            {
                throw new InvalidOperationException($"Frame file is empty: {framePath}");
            }

            savedFiles.Add(framePath);

            progress?.Report(new AnimationProgress
            {
                CurrentFrame = i + 1,
                TotalFrames = frames.Count,
                Phase = "Saving Frames",
                StatusMessage = $"Saved {i + 1}/{frames.Count} frames"
            });
        }

        _logger.LogDebug("Saved {FrameCount} PNG frames to {Directory}. Total size: {TotalSize} bytes", 
            frames.Count, 
            outputDirectory,
            savedFiles.Sum(f => new FileInfo(f).Length));
    }

    /// <summary>
    /// Save a single WriteableBitmap to PNG file.
    /// </summary>
    private async Task SaveFrameAsPngAsync(WriteableBitmap bitmap, string filePath)
    {
        // Get pixel data from bitmap
        var pixelData = bitmap.PixelBuffer.ToArray();

        // Create parent directory if it doesn't exist
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Create or overwrite the file using StorageFolder API
        var folder = await Windows.Storage.StorageFolder.GetFolderFromPathAsync(directory!);
        var fileName = Path.GetFileName(filePath);
        var file = await folder.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);

        using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
        {
            var encoder = await Windows.Graphics.Imaging.BitmapEncoder.CreateAsync(
                Windows.Graphics.Imaging.BitmapEncoder.PngEncoderId,
                stream);

            // Use Straight alpha mode (not Premultiplied) to avoid color corruption
            // BGRA8 with Straight alpha ensures proper color representation for video encoding
            encoder.SetPixelData(
                Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8,
                Windows.Graphics.Imaging.BitmapAlphaMode.Straight,
                (uint)bitmap.PixelWidth,
                (uint)bitmap.PixelHeight,
                96.0, // DPI X
                96.0, // DPI Y
                pixelData);

            await encoder.FlushAsync();

            // Ensure all data is written to disk before proceeding
            await stream.FlushAsync();
        }
    }

    /// <summary>
    /// Ensure FFmpeg binaries are available.
    /// First checks system PATH, then downloads to app's local folder if needed.
    /// </summary>
    private async Task EnsureFfmpegAsync(CancellationToken cancellationToken)
    {
        // Try to find FFmpeg on PATH first (for development/user installations)
        var ffmpegPath = FindFfmpegInPath();

        if (ffmpegPath != null)
        {
            _logger.LogInformation("FFmpeg found on system PATH: {FFmpegPath}", ffmpegPath);
            FFmpeg.SetExecutablesPath(Path.GetDirectoryName(ffmpegPath)!);
            return;
        }

        // Not on PATH - check if we've downloaded it to app local folder
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var ffmpegDir = Path.Combine(localAppData, "ManpWinUI", "ffmpeg");
        var localFfmpegPath = Path.Combine(ffmpegDir, "ffmpeg.exe");

        if (File.Exists(localFfmpegPath))
        {
            _logger.LogInformation("FFmpeg found in app folder: {FFmpegPath}", localFfmpegPath);
            FFmpeg.SetExecutablesPath(ffmpegDir);
            return;
        }

        // Download FFmpeg to app local folder
        _logger.LogInformation("FFmpeg not found. Downloading to: {FFmpegDir}", ffmpegDir);

        try
        {
            Directory.CreateDirectory(ffmpegDir);

            // Use Xabe.FFmpeg.Downloader's API
            await FFmpegDownloader.GetLatestVersion(
                FFmpegVersion.Official,
                ffmpegDir);

            FFmpeg.SetExecutablesPath(ffmpegDir);
            _logger.LogInformation("FFmpeg downloaded successfully to: {FFmpegDir}", ffmpegDir);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download FFmpeg");
            throw new InvalidOperationException(
                "FFmpeg is required for MP4 export but could not be downloaded. " +
                "Please install FFmpeg manually and add it to your system PATH. " +
                "Download from: https://ffmpeg.org/download.html", ex);
        }
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

        // Ensure output directory exists
        var outputDir = Path.GetDirectoryName(settings.OutputPath);
        if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
            _logger.LogDebug("Created output directory: {OutputDir}", outputDir);
        }

        // Validate output path is writable
        try
        {
            var testFile = Path.Combine(outputDir ?? ".", $"test_write_{Guid.NewGuid():N}.tmp");
            File.WriteAllText(testFile, "test");
            File.Delete(testFile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Output directory is not writable: {OutputDir}", outputDir);
            throw new InvalidOperationException($"Cannot write to output directory: {outputDir}", ex);
        }

        // Check if output file already exists and try to delete it to ensure it's writable
        if (File.Exists(settings.OutputPath))
        {
            _logger.LogWarning("Output file already exists, deleting before encoding: {OutputPath}", settings.OutputPath);
            try
            {
                File.Delete(settings.OutputPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot overwrite existing output file: {OutputPath}", settings.OutputPath);
                throw new InvalidOperationException($"Output file exists and cannot be overwritten: {settings.OutputPath}", ex);
            }
        }

        // Build FFmpeg conversion with correct H.264/MP4 encoding parameters
        // Reference: FFmpeg H.264 encoding guide and MP4 container best practices
        // Critical for compatibility: yuv420p, faststart flag, explicit sync
        var conversion = FFmpeg.Conversions.New()
            .AddParameter($"-framerate {settings.FrameRate}", ParameterPosition.PreInput) // Input framerate
            .AddParameter($"-i \"{inputPattern}\"", ParameterPosition.PreInput) // Input file pattern
            .AddParameter("-c:v libx264", ParameterPosition.PostInput) // H.264 video codec
            .AddParameter("-preset medium", ParameterPosition.PostInput) // Encoding speed/quality balance
            .AddParameter("-crf 18", ParameterPosition.PostInput) // Constant quality (18 = visually lossless)
            .AddParameter("-pix_fmt yuv420p", ParameterPosition.PostInput) // YUV 4:2:0 chroma (universal compatibility)
            .AddParameter("-vf scale=trunc(iw/2)*2:trunc(ih/2)*2", ParameterPosition.PostInput) // Ensure even dimensions
            .AddParameter("-fps_mode cfr", ParameterPosition.PostInput) // Constant frame rate (replaces deprecated -vsync)
            .AddParameter("-movflags +faststart", ParameterPosition.PostInput) // Move metadata to start (web/streaming)
            .SetOutput(settings.OutputPath) // Use SetOutput instead of AddParameter - handles -y automatically
            .SetOverwriteOutput(true); // Enable overwrite

        _logger.LogDebug("FFmpeg command: {Command}", conversion.Build());

        try
        {
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

            // Capture FFmpeg output for diagnostics
            var conversionResult = await conversion.Start(cancellationToken);
            _logger.LogDebug("FFmpeg output: {Output}", conversionResult);
            _logger.LogInformation("FFmpeg encoding complete: {OutputPath}", settings.OutputPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "FFmpeg encoding failed. Command: {Command}", conversion.Build());
            throw new InvalidOperationException($"FFmpeg encoding failed: {ex.Message}", ex);
        }

        // Verify output file was created
        if (!File.Exists(settings.OutputPath))
        {
            throw new InvalidOperationException(
                $"FFmpeg completed but output file was not created: {settings.OutputPath}");
        }

        var fileInfo = new FileInfo(settings.OutputPath);
        if (fileInfo.Length == 0)
        {
            throw new InvalidOperationException(
                $"FFmpeg created an empty output file: {settings.OutputPath}");
        }

        _logger.LogInformation("Output file verified: {Size} bytes", fileInfo.Length);
    }
}
