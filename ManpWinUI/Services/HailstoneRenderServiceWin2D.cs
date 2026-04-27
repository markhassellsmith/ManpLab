using ManpWinUI.Models;
using ManpWinUI.Services.Hailstone;
using Microsoft.UI;
using System.Diagnostics;

namespace ManpWinUI.Services;

/// <summary>
/// Win2D-based Hailstone sequence renderer using graphics abstraction layer.
/// Coordinates sub-renderers using composition pattern for maintainability.
/// 
/// Refactored from monolithic 700-line file into focused components:
/// - HailstoneCoordinateTransform: Transform calculations
/// - HailstoneGridRenderer: Grid/axes drawing
/// - HailstoneTrajectoryRenderer: Path/points drawing
/// - HailstoneRenderHelpers: Utility functions
/// </summary>
public class HailstoneRenderServiceWin2D
{
    private readonly HailstoneCoordinateTransform _transform;
    private readonly HailstoneGridRenderer _gridRenderer;
    private readonly HailstoneTrajectoryRenderer _trajectoryRenderer;

    public HailstoneRenderServiceWin2D()
    {
        _transform = new HailstoneCoordinateTransform();
        _gridRenderer = new HailstoneGridRenderer();
        _trajectoryRenderer = new HailstoneTrajectoryRenderer(_transform);
    }

    /// <summary>
    /// Renders a Hailstone sequence using Win2D graphics abstraction.
    /// All elements (grid, trajectory, points) rendered directly on bitmap.
    /// Labels and info text handled by Canvas overlay system.
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

        Debug.WriteLine($"=== Win2D Hailstone Render ===");
        Debug.WriteLine($"Sequence points: {result.Sequence.Count}");
        Debug.WriteLine($"Toggles: Axes={showAxes}, Points={showPoints}");

        // Determine viewport bounds
        var (minX, maxX, minY, maxY) = DetermineViewportBounds(
            result, useFixedViewport, customViewportMinX, customViewportMaxX,
            customViewportMinY, customViewportMaxY);

        // Calculate world-to-screen transform
        bool hasCustomViewport = HasCustomViewport(customViewportMinX, customViewportMaxX, customViewportMinY, customViewportMaxY);
        var (scaleX, scaleY, offsetX, offsetY) = _transform.CalculateTransform(
            minX, maxX, minY, maxY, width, height, useFixedViewport || hasCustomViewport);

        Debug.WriteLine($"Transform: scaleX={scaleX:F2}, scaleY={scaleY:F2}, offsetX={offsetX:F2}, offsetY={offsetY:F2}");

        // Render on background thread
        var pixelData = await Task.Run(() => RenderToPixels(
            result, width, height, showAxes, showPoints,
            minX, maxX, minY, maxY, scaleX, scaleY, offsetX, offsetY));

        stopwatch.Stop();
        Debug.WriteLine($"=== Win2D Hailstone Render Complete ({stopwatch.ElapsedMilliseconds}ms) ===");

        if (pixelData == null)
        {
            throw new InvalidOperationException("Win2D rendering failed");
        }

        return new HailstoneRenderResult
        {
            PixelData = pixelData,
            Width = width,
            Height = height,
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
    /// Performs the actual rendering to pixel data.
    /// </summary>
    private byte[]? RenderToPixels(
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
        try
        {
            using var renderer = GraphicsRendererFactory.Create(width, height, GraphicsBackend.Win2D);

            // 1. Clear background
            renderer.Clear(Colors.Black);

            // 2. Set up coordinate transform for world-space rendering
            _transform.SetupCoordinateTransform(renderer, width, height,
                minX, maxX, minY, maxY, scaleX, scaleY, offsetX, offsetY);

            // 3. Draw grid and axes (if enabled) in world coordinates
            if (showAxes)
            {
                _gridRenderer.DrawGridWithTransform(renderer, minX, maxX, minY, maxY, scaleX, scaleY);
            }

            // 4. Draw sequence trajectory path in world coordinates
            _trajectoryRenderer.DrawTrajectoryPath(renderer, result.Sequence, scaleX, scaleY);

            // 5. Draw point markers (if enabled) with fixed pixel sizes
            if (showPoints)
            {
                _trajectoryRenderer.DrawPointMarkers(renderer, result.Sequence, scaleX, scaleY, offsetX, offsetY);
            }

            // NOTE: Labels and info text are handled by Canvas overlay system
            // (MainPage.HailstoneLabels.cs and MainPage.HailstoneInfo.cs)

            return renderer.GetPixelData();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR in Win2D render: {ex.Message}");
            Debug.WriteLine($"Stack: {ex.StackTrace}");
            return null;
        }
    }
}
