using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ManpWinUI.ViewModels;

/// <summary>
/// MainViewModel partial class - Rendering state and coordination.
/// Handles image resolution, rendering progress, fractal image output, and render commands.
/// </summary>
public partial class MainViewModel
{
    // ═══════════════════════════════════════════════════════════════════════════════
    // IMAGE RESOLUTION
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Image width in pixels.
    /// </summary>
    [ObservableProperty]
    public partial int ImageWidth { get; set; } = 1200;

    /// <summary>
    /// Image height in pixels.
    /// </summary>
    [ObservableProperty]
    public partial int ImageHeight { get; set; } = 900;

    /// <summary>
    /// Computed property for total megapixels.
    /// </summary>
    public string TotalPixels => $"{(ImageWidth * ImageHeight / 1_000_000.0):F2}";

    // ═══════════════════════════════════════════════════════════════════════════════
    // RENDERING STATE
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Indicates whether a render operation is currently in progress.
    /// </summary>
    [ObservableProperty]
    public partial bool IsRendering { get; set; }

    /// <summary>
    /// Current render progress (0-100).
    /// </summary>
    [ObservableProperty]
    public partial double RenderProgress { get; set; }

    /// <summary>
    /// Time taken for the last render operation.
    /// </summary>
    [ObservableProperty]
    public partial TimeSpan LastRenderTime { get; set; }

    // ═══════════════════════════════════════════════════════════════════════════════
    // FRACTAL IMAGE OUTPUT
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// The rendered fractal image displayed in the UI.
    /// </summary>
    [ObservableProperty]
    public partial WriteableBitmap? FractalImage { get; set; }

    // ═══════════════════════════════════════════════════════════════════════════════
    // RENDER COMMANDS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Determines whether rendering can be executed (not already rendering).
    /// </summary>
    private bool CanRender() => !IsRendering;

    // ═══════════════════════════════════════════════════════════════════════════════
    // HELPER METHODS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Converts raw pixel data to a WriteableBitmap for display.
    /// Creates or reuses the FractalImage bitmap as needed.
    /// </summary>
    /// <param name="pixelData">BGRA pixel data array.</param>
    /// <param name="width">Image width in pixels.</param>
    /// <param name="height">Image height in pixels.</param>
    private void ConvertPixelDataToBitmap(byte[] pixelData, int width, int height)
    {
        // Create or reuse WriteableBitmap
        if (FractalImage == null || FractalImage.PixelWidth != width || FractalImage.PixelHeight != height)
        {
            FractalImage = new WriteableBitmap(width, height);
        }

        // Write pixel data to bitmap buffer using WindowsRuntimeBufferExtensions
        WindowsRuntimeBufferExtensions.CopyTo(pixelData, 0, FractalImage.PixelBuffer, 0, pixelData.Length);

        // Invalidate the bitmap to trigger redraw
        FractalImage.Invalidate();
    }

    // NOTE: Render command implementations (RenderMandelbrotAsync, RenderHailstoneAsync, etc.)
    // remain in MainViewModel.cs to access cross-cutting concerns (services, fractal parameters).
    // This partial focuses on rendering STATE and image OUTPUT management.
}
