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
    /// Default: HD (1280×720) for testing - smaller memory footprint.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPixels))]
    [NotifyPropertyChangedFor(nameof(IsHDResolution))]
    [NotifyPropertyChangedFor(nameof(IsFullHDResolution))]
    [NotifyPropertyChangedFor(nameof(Is2KResolution))]
    [NotifyPropertyChangedFor(nameof(Is4KResolution))]
    [NotifyPropertyChangedFor(nameof(Is4KPlusResolution))]
    public partial int ImageWidth { get; set; } = 1280;

    /// <summary>
    /// Image height in pixels.
    /// Default: HD (1280×720) for testing - smaller memory footprint.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPixels))]
    [NotifyPropertyChangedFor(nameof(IsHDResolution))]
    [NotifyPropertyChangedFor(nameof(IsFullHDResolution))]
    [NotifyPropertyChangedFor(nameof(Is2KResolution))]
    [NotifyPropertyChangedFor(nameof(Is4KResolution))]
    [NotifyPropertyChangedFor(nameof(Is4KPlusResolution))]
    public partial int ImageHeight { get; set; } = 720;

    /// <summary>
    /// Computed property for total megapixels.
    /// </summary>
    public string TotalPixels => $"{(ImageWidth * ImageHeight / 1_000_000.0):F2}";

    /// <summary>
    /// Gets whether the current resolution is HD (1280×720).
    /// </summary>
    public bool IsHDResolution => ImageWidth == 1280 && ImageHeight == 720;

    /// <summary>
    /// Gets whether the current resolution is Full HD (1920×1080).
    /// </summary>
    public bool IsFullHDResolution => ImageWidth == 1920 && ImageHeight == 1080;

    /// <summary>
    /// Gets whether the current resolution is 2K (2560×1440).
    /// </summary>
    public bool Is2KResolution => ImageWidth == 2560 && ImageHeight == 1440;

    /// <summary>
    /// Gets whether the current resolution is 4K (3840×2160).
    /// </summary>
    public bool Is4KResolution => ImageWidth == 3840 && ImageHeight == 2160;

    /// <summary>
    /// Gets whether the current resolution is 4K+ (4096×2160).
    /// </summary>
    public bool Is4KPlusResolution => ImageWidth == 4096 && ImageHeight == 2160;

    // ═══════════════════════════════════════════════════════════════════════════════
    // RENDERING STATE
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Cancellation token source for stopping renders.
    /// </summary>
    private CancellationTokenSource? _renderCancellationSource;

    /// <summary>
    /// Indicates whether a render operation is currently in progress.
    /// Used for UI state (progress overlay, stop button).
    /// Controls Render button enablement - disabled while rendering.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RenderCommand))]
    [NotifyCanExecuteChangedFor(nameof(StopRenderCommand))]
    public partial bool IsRendering { get; set; }

    /// <summary>
    /// Called when IsRendering property changes.
    /// </summary>
    partial void OnIsRenderingChanged(bool value)
    {
        // Render button state is controlled by CanRender() method
        System.Diagnostics.Debug.WriteLine($"[ViewModel] IsRendering changed to: {value}");
    }

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
    // HELPER METHODS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Converts raw pixel data to a WriteableBitmap for display.
    /// Creates or reuses the FractalImage bitmap as needed.
    /// MUST be called on UI thread for WinRT interop.
    /// </summary>
    /// <param name="pixelData">BGRA pixel data array.</param>
    /// <param name="width">Image width in pixels.</param>
    /// <param name="height">Image height in pixels.</param>
    private void ConvertPixelDataToBitmap(byte[] pixelData, int width, int height)
    {
        try
        {
            // Validate input parameters
            var expectedSize = width * height * 4; // BGRA = 4 bytes per pixel
            if (pixelData == null)
            {
                throw new ArgumentNullException(nameof(pixelData), "Pixel data is null");
            }
            if (pixelData.Length != expectedSize)
            {
                throw new ArgumentException(
                    $"Pixel data size mismatch: expected {expectedSize} bytes for {width}×{height} image, got {pixelData.Length} bytes");
            }
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentException($"Invalid dimensions: {width}×{height}");
            }

            System.Diagnostics.Debug.WriteLine($"[ConvertPixelDataToBitmap] Creating bitmap: {width}×{height}, data size: {pixelData.Length} bytes");

            // Create or reuse WriteableBitmap
            if (FractalImage == null || FractalImage.PixelWidth != width || FractalImage.PixelHeight != height)
            {
                System.Diagnostics.Debug.WriteLine($"[ConvertPixelDataToBitmap] Creating new WriteableBitmap({width}, {height})");

                try
                {
                    FractalImage = new WriteableBitmap(width, height);
                }
                catch (Exception createEx)
                {
                    System.Diagnostics.Debug.WriteLine($"[ConvertPixelDataToBitmap] Failed to create WriteableBitmap: {createEx.Message}");
                    throw new InvalidOperationException($"Failed to create {width}×{height} bitmap. WinRT error: {createEx.Message}", createEx);
                }

                System.Diagnostics.Debug.WriteLine($"[ConvertPixelDataToBitmap] Bitmap created successfully");
                System.Diagnostics.Debug.WriteLine($"[ConvertPixelDataToBitmap] PixelBuffer.Length: {FractalImage.PixelBuffer.Length}");
                System.Diagnostics.Debug.WriteLine($"[ConvertPixelDataToBitmap] PixelBuffer.Capacity: {FractalImage.PixelBuffer.Capacity}");
            }

            // Validate buffer size before copying
            if (FractalImage.PixelBuffer.Capacity < (uint)pixelData.Length)
            {
                throw new InvalidOperationException(
                    $"PixelBuffer capacity ({FractalImage.PixelBuffer.Capacity}) is less than data size ({pixelData.Length})");
            }

            // Write pixel data to bitmap buffer using WindowsRuntimeBufferExtensions
            System.Diagnostics.Debug.WriteLine($"[ConvertPixelDataToBitmap] Copying {pixelData.Length} bytes to PixelBuffer (capacity: {FractalImage.PixelBuffer.Capacity})");

            try
            {
                // Use AsStream() for safer WinRT interop
                using (var stream = FractalImage.PixelBuffer.AsStream())
                {
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    stream.Write(pixelData, 0, pixelData.Length);
                    stream.Flush();
                }
            }
            catch (Exception copyEx)
            {
                System.Diagnostics.Debug.WriteLine($"[ConvertPixelDataToBitmap] Pixel copy failed: {copyEx.Message}");
                throw new InvalidOperationException($"Failed to copy pixel data to bitmap buffer. WinRT error: {copyEx.Message}", copyEx);
            }

            // Invalidate the bitmap to trigger redraw
            FractalImage.Invalidate();
            System.Diagnostics.Debug.WriteLine($"[ConvertPixelDataToBitmap] Bitmap updated successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ConvertPixelDataToBitmap] ERROR: {ex.GetType().Name}: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[ConvertPixelDataToBitmap] Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    // NOTE: Render command implementations (RenderMandelbrotAsync, RenderHailstoneAsync, etc.)
    // remain in MainViewModel.cs to access cross-cutting concerns (services, fractal parameters).
    // This partial focuses on rendering STATE and image OUTPUT management.
}
