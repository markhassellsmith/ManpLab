using ManpCore.Services.Models;
using ManpWinUI.Models;
using ManpWinUI.Services.Hailstone;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ManpWinUI.Services;

/// <summary>
/// Service for rendering 2D Hailstone sequences to bitmap images using pixel operations.
/// Coordinates sub-renderers using composition pattern for maintainability.
/// 
/// Refactored from monolithic 731-line file into focused components:
/// - HailstoneCoordinateTransform: Transform calculations (shared with Win2D)
/// - HailstonePixelGridRenderer: Grid/axes rendering using pixels
/// - HailstonePixelTrajectoryRenderer: Path/points rendering using pixels
/// - HailstonePixelRenderer: Low-level pixel operations
/// - HailstoneRenderHelpers: Utility functions (shared with Win2D)
/// </summary>
public class HailstoneRenderService : IHailstoneRenderService
{
    private readonly HailstoneCoordinateTransform _transform;
    private readonly HailstonePixelGridRenderer _gridRenderer;
    private readonly HailstonePixelTrajectoryRenderer _trajectoryRenderer;

    public HailstoneRenderService()
    {
        _transform = new HailstoneCoordinateTransform();
        _gridRenderer = new HailstonePixelGridRenderer(_transform);
        _trajectoryRenderer = new HailstonePixelTrajectoryRenderer(_transform);
    }

    /// <summary>
    /// Renders a Hailstone sequence to a WriteableBitmap using pixel operations.
    /// </summary>
    /// <param name="result">The calculated Hailstone sequence.</param>
    /// <param name="width">Image width in pixels.</param>
    /// <param name="height">Image height in pixels.</param>
    /// <param name="showAxes">Whether to draw coordinate axes and grid lines.</param>
    /// <param name="showPoints">Whether to draw dots at trajectory points.</param>
    /// <param name="showLabels">Ignored - labels handled by Canvas overlay.</param>
    /// <param name="useFixedViewport">If true, uses fixed viewport bounds; if false, auto-scales to data.</param>
    /// <param name="customViewportMinX">Custom viewport minimum X (overrides auto-scale/fixed).</param>
    /// <param name="customViewportMaxX">Custom viewport maximum X (overrides auto-scale/fixed).</param>
    /// <param name="customViewportMinY">Custom viewport minimum Y (overrides auto-scale/fixed).</param>
    /// <param name="customViewportMaxY">Custom viewport maximum Y (overrides auto-scale/fixed).</param>
    /// <returns>A HailstoneRenderResult containing the bitmap and transform parameters.</returns>
    public async Task<HailstoneRenderResult> RenderSequenceAsync(
        HailstoneResult result,
        int width,
        int height,
        bool showAxes,
        bool showPoints,
        bool showLabels,  // Ignored - Canvas overlay handles labels
        bool useFixedViewport = false,
        double? customViewportMinX = null,
        double? customViewportMaxX = null,
        double? customViewportMinY = null,
        double? customViewportMaxY = null)
    {
        var stopwatch = Stopwatch.StartNew();

        Debug.WriteLine($"=== Hailstone Pixel Render ===");
        Debug.WriteLine($"Sequence points: {result.Sequence.Count}");
        Debug.WriteLine($"Toggles: Axes={showAxes}, Points={showPoints}");

        // Determine viewport bounds
        var (minX, maxX, minY, maxY) = DetermineViewportBounds(
            result, useFixedViewport, customViewportMinX, customViewportMaxX,
            customViewportMinY, customViewportMaxY);

        // Calculate world-to-screen transform
        bool hasCustomViewport = HasCustomViewport(customViewportMinX, customViewportMaxX,
            customViewportMinY, customViewportMaxY);
        var (scaleX, scaleY, offsetX, offsetY) = _transform.CalculateTransform(
            minX, maxX, minY, maxY, width, height, useFixedViewport || hasCustomViewport);

        Debug.WriteLine($"Transform: scaleX={scaleX:F2}, scaleY={scaleY:F2}, offsetX={offsetX:F2}, offsetY={offsetY:F2}");

        // Render on background thread
        byte[] pixels = await Task.Run(() => RenderToPixels(
            result, width, height, showAxes, showPoints,
            minX, maxX, minY, maxY, scaleX, scaleY, offsetX, offsetY));

        stopwatch.Stop();
        Debug.WriteLine($"=== Hailstone Pixel Render Complete ({stopwatch.ElapsedMilliseconds}ms) ===");

        // Create WriteableBitmap on UI thread
        var bitmap = new WriteableBitmap(width, height);
        using (var stream = bitmap.PixelBuffer.AsStream())
        {
            stream.Write(pixels, 0, pixels.Length);
        }

        return new HailstoneRenderResult
        {
            Bitmap = bitmap,
            SequenceResult = result,
            ScaleX = scaleX,
            ScaleY = scaleY,
            OffsetX = offsetX,
            OffsetY = offsetY
        };
    }

    /// <summary>
    /// Determines viewport bounds based on priority: custom > fixed > auto-scale.
    /// </summary>
    private (int minX, int maxX, int minY, int maxY) DetermineViewportBounds(
        HailstoneResult result,
        bool useFixedViewport,
        double? customViewportMinX,
        double? customViewportMaxX,
        double? customViewportMinY,
        double? customViewportMaxY)
    {
        // Check if custom viewport is provided (highest priority)
        if (HasCustomViewport(customViewportMinX, customViewportMaxX, customViewportMinY, customViewportMaxY))
        {
            int minX = (int)Math.Floor(customViewportMinX!.Value);
            int maxX = (int)Math.Ceiling(customViewportMaxX!.Value);
            int minY = (int)Math.Floor(customViewportMinY!.Value);
            int maxY = (int)Math.Ceiling(customViewportMaxY!.Value);
            Debug.WriteLine($"Using CUSTOM viewport: X=[{minX}, {maxX}], Y=[{minY}, {maxY}]");
            return (minX, maxX, minY, maxY);
        }

        // Use fixed viewport bounds
        if (useFixedViewport)
        {
            Debug.WriteLine($"Using FIXED viewport: X=[{HailstoneCoordinateTransform.DefaultMinX}, {HailstoneCoordinateTransform.DefaultMaxX}], Y=[{HailstoneCoordinateTransform.DefaultMinY}, {HailstoneCoordinateTransform.DefaultMaxY}]");
            return (HailstoneCoordinateTransform.DefaultMinX, HailstoneCoordinateTransform.DefaultMaxX,
                    HailstoneCoordinateTransform.DefaultMinY, HailstoneCoordinateTransform.DefaultMaxY);
        }

        // Auto-scale to data bounds
        Debug.WriteLine($"Using AUTO-SCALE: X=[{result.MinX}, {result.MaxX}], Y=[{result.MinY}, {result.MaxY}]");
        return (result.MinX, result.MaxX, result.MinY, result.MaxY);
    }

    /// <summary>
    /// Checks if a complete custom viewport is specified.
    /// </summary>
    private bool HasCustomViewport(double? minX, double? maxX, double? minY, double? maxY)
    {
        return minX.HasValue && maxX.HasValue && minY.HasValue && maxY.HasValue;
    }

    /// <summary>
    /// Performs the actual rendering to pixel buffer.
    /// </summary>
    private byte[] RenderToPixels(
        HailstoneResult result,
        int width,
        int height,
        bool showAxes,
        bool showPoints,
        int minX,
        int maxX,
        int minY,
        int maxY,
        double scaleX,
        double scaleY,
        double offsetX,
        double offsetY)
    {
        // Create pixel buffer (BGRA format)
        byte[] pixels = new byte[width * height * 4];

        // 1. Clear to black background
        Array.Fill<byte>(pixels, 0);

        // 2. Draw grid and axes (if enabled)
        if (showAxes)
        {
            _gridRenderer.DrawGrid(pixels, width, height, minX, maxX, minY, maxY,
                scaleX, scaleY, offsetX, offsetY);
        }

        // 3. Draw sequence trajectory path
        _trajectoryRenderer.DrawSequence(pixels, width, height, result.Sequence,
            scaleX, scaleY, offsetX, offsetY);

        // 4. Draw point markers (if enabled)
        if (showPoints)
        {
            _trajectoryRenderer.DrawPoints(pixels, width, height, result.Sequence,
                scaleX, scaleY, offsetX, offsetY);
        }

        // NOTE: Labels and info text are handled by Canvas overlay system
        // (MainPage.HailstoneLabels.cs and MainPage.HailstoneInfo.cs)

        return pixels;
    }
}
