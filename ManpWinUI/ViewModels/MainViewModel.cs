using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using ManpWinUI.Services;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace ManpWinUI.ViewModels;

/// <summary>
/// Main view model for the fractal explorer interface.
/// Manages fractal rendering parameters, UI state, and coordinates with the fractal engine.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly IFractalRenderService _renderService;

    // Fractal rendering parameters
    [ObservableProperty]
    private double _centerX = -0.5;

    [ObservableProperty]
    private double _centerY = 0.0;

    [ObservableProperty]
    private double _zoom = 1.0;

    [ObservableProperty]
    private int _maxIterations = 256;

    [ObservableProperty]
    private int _imageWidth = 800;

    [ObservableProperty]
    private int _imageHeight = 600;

    [ObservableProperty]
    private string _selectedPalette = "Classic";

    // UI state
    [ObservableProperty]
    private bool _isRendering;

    [ObservableProperty]
    private double _renderProgress;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private TimeSpan _lastRenderTime;

    // Fractal image
    [ObservableProperty]
    private WriteableBitmap? _fractalImage;

    public MainViewModel(IFractalRenderService renderService)
    {
        _renderService = renderService;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    }

    /// <summary>
    /// Renders the Mandelbrot set with current parameters.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRender))]
    private async Task RenderMandelbrotAsync()
    {
        IsRendering = true;
        RenderProgress = 0;
        StatusMessage = "Rendering Mandelbrot set...";

        try
        {
            var startTime = DateTime.Now;

            // Progress reporting callback
            var progress = new Progress<double>(percentage =>
            {
                _dispatcherQueue.TryEnqueue(() =>
                {
                    RenderProgress = percentage * 100.0;
                });
            });

            // Call FractalRenderService to render the fractal
            var pixelData = await _renderService.RenderMandelbrotAsync(
                CenterX,
                CenterY,
                Zoom,
                ImageWidth,
                ImageHeight,
                MaxIterations,
                SelectedPalette,
                progress);

            // Convert byte[] to WriteableBitmap on UI thread
            _dispatcherQueue.TryEnqueue(() =>
            {
                ConvertPixelDataToBitmap(pixelData, ImageWidth, ImageHeight);
            });

            LastRenderTime = DateTime.Now - startTime;
            StatusMessage = $"Rendered in {LastRenderTime.TotalMilliseconds:F0} ms";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsRendering = false;
        }
    }

    private bool CanRender() => !IsRendering;

    /// <summary>
    /// Resets view to default Mandelbrot parameters.
    /// </summary>
    [RelayCommand]
    private void ResetView()
    {
        CenterX = -0.5;
        CenterY = 0.0;
        Zoom = 1.0;
        MaxIterations = 256;
        StatusMessage = "View reset to default";
    }

    /// <summary>
    /// Zooms in on the current center point.
    /// </summary>
    [RelayCommand]
    private void ZoomIn()
    {
        Zoom *= 2.0;
        StatusMessage = $"Zoom: {Zoom:F2}x";
    }

    /// <summary>
    /// Zooms out from the current center point.
    /// </summary>
    [RelayCommand]
    private void ZoomOut()
    {
        Zoom /= 2.0;
        StatusMessage = $"Zoom: {Zoom:F2}x";
    }

    partial void OnMaxIterationsChanged(int value)
    {
        // Clamp max iterations to reasonable range
        if (value < 50) MaxIterations = 50;
        if (value > 10000) MaxIterations = 10000;
    }

    partial void OnZoomChanged(double value)
    {
        // Prevent zoom from going negative or too small
        if (value < 0.001) Zoom = 0.001;
    }

    /// <summary>
    /// Converts pixel data byte array to WriteableBitmap for display.
    /// PixelData format is BGRA (4 bytes per pixel).
    /// </summary>
    private void ConvertPixelDataToBitmap(byte[] pixelData, int width, int height)
    {
        // Create or reuse WriteableBitmap
        if (FractalImage == null || FractalImage.PixelWidth != width || FractalImage.PixelHeight != height)
        {
            FractalImage = new WriteableBitmap(width, height);
        }

        // Write pixel data to bitmap
        using (var stream = FractalImage.PixelBuffer.AsStream())
        {
            stream.Write(pixelData, 0, pixelData.Length);
        }

        // Invalidate the bitmap to trigger redraw
        FractalImage.Invalidate();
    }
}
