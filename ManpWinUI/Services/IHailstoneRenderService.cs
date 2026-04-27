using ManpCore.Services.Models;
using ManpWinUI.Models;

namespace ManpWinUI.Services;

/// <summary>
/// Service interface for rendering 2D Hailstone sequences to bitmap images.
/// Supports multiple rendering backends (pixel-based, Win2D, etc.).
/// </summary>
public interface IHailstoneRenderService
{
    /// <summary>
    /// Renders a 2D Hailstone sequence to a bitmap with coordinate transform information.
    /// </summary>
    /// <param name="result">The calculated Hailstone sequence to render.</param>
    /// <param name="width">Image width in pixels.</param>
    /// <param name="height">Image height in pixels.</param>
    /// <param name="showAxes">Whether to draw coordinate axes and grid lines.</param>
    /// <param name="showPoints">Whether to draw dots at trajectory points.</param>
    /// <param name="showLabels">Whether to show point labels (may be ignored if labels are overlay-based).</param>
    /// <param name="useFixedViewport">If true, uses fixed viewport bounds; if false, auto-scales to data.</param>
    /// <param name="customViewportMinX">Custom viewport minimum X (overrides auto-scale/fixed).</param>
    /// <param name="customViewportMaxX">Custom viewport maximum X (overrides auto-scale/fixed).</param>
    /// <param name="customViewportMinY">Custom viewport minimum Y (overrides auto-scale/fixed).</param>
    /// <param name="customViewportMaxY">Custom viewport maximum Y (overrides auto-scale/fixed).</param>
    /// <returns>A HailstoneRenderResult containing the bitmap and transform parameters.</returns>
    Task<HailstoneRenderResult> RenderSequenceAsync(
        HailstoneResult result,
        int width,
        int height,
        bool showAxes,
        bool showPoints,
        bool showLabels,
        bool useFixedViewport = false,
        double? customViewportMinX = null,
        double? customViewportMaxX = null,
        double? customViewportMinY = null,
        double? customViewportMaxY = null);
}
