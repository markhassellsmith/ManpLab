using ManpWinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace ManpWinUI.Views.Animation;

/// <summary>
/// Animation control panel for creating and exporting fractal animations.
/// </summary>
public sealed partial class AnimationControlPanel : UserControl
{
    public AnimationViewModel ViewModel { get; }

    public AnimationControlPanel()
    {
        // Get singleton ViewModel from DI
        ViewModel = App.Current.Services.GetRequiredService<AnimationViewModel>();

        this.InitializeComponent();
    }

    /// <summary>
    /// Called when the control is loaded.
    /// MainViewModel reference is now set by MainPage during initialization.
    /// </summary>
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // MainViewModel is already injected by MainPage
        // No need to fetch from DI here
    }

    /// <summary>
    /// Sync animation start position from current fractal view.
    /// </summary>
    private void SyncFromCurrentViewButton_Click(object sender, RoutedEventArgs e)
    {
        // MainViewModel is accessed through ViewModel.GetMainViewModel() or similar
        // For now, get it from DI
        var mainViewModel = App.Current.Services.GetService<MainViewModel>();

        if (mainViewModel == null)
        {
            ShowError("MainViewModel not available", "Cannot sync from current view.");
            return;
        }

        // Update start positions based on animation type
        switch (ViewModel.SelectedAnimationType)
        {
            case Models.Animation.AnimationType.Zoom:
                ViewModel.StartZoom = mainViewModel.Zoom;
                ViewModel.StartCenterX = mainViewModel.CenterX;
                ViewModel.StartCenterY = mainViewModel.CenterY;
                ShowSuccess($"✓ Start position synced: zoom {mainViewModel.Zoom:F2}× at ({mainViewModel.CenterX:F4}, {mainViewModel.CenterY:F4})");
                break;

            case Models.Animation.AnimationType.Pan:
                ViewModel.StartCenterX = mainViewModel.CenterX;
                ViewModel.StartCenterY = mainViewModel.CenterY;
                ShowSuccess($"✓ Start position synced: ({mainViewModel.CenterX:F4}, {mainViewModel.CenterY:F4})");
                break;

            default:
                ShowSuccess("✓ Current fractal settings will be used");
                break;
        }
    }

    private void ShowSuccess(string message)
    {
        // Simple status update - could be enhanced with InfoBar
        System.Diagnostics.Debug.WriteLine($"[AnimationControlPanel] {message}");
    }

    private async void ShowError(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot
        };
        await dialog.ShowAsync();
    }

    private async void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Create a file save picker
            var savePicker = new FileSavePicker();

            // Get the window handle for the picker
            var window = App.Current.MainWindow;
            if (window == null) return;

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

            // Generate timestamped filename suggestion
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var animationType = ViewModel.SelectedAnimationType.ToString().ToLowerInvariant();

            // Set file type filters and suggested filename based on selected export format
            string suggestedFileName;
            switch (ViewModel.SelectedExportFormat)
            {
                case Models.Animation.ExportFormat.MP4:
                    savePicker.FileTypeChoices.Add("MP4 Video", new[] { ".mp4" });
                    suggestedFileName = $"ManpWinUI_{animationType}_{timestamp}.mp4";
                    break;
                case Models.Animation.ExportFormat.GIF:
                    savePicker.FileTypeChoices.Add("Animated GIF", new[] { ".gif" });
                    suggestedFileName = $"ManpWinUI_{animationType}_{timestamp}.gif";
                    break;
                case Models.Animation.ExportFormat.PNGSequence:
                    savePicker.FileTypeChoices.Add("ZIP Archive", new[] { ".zip" });
                    suggestedFileName = $"ManpWinUI_{animationType}_{timestamp}.zip";
                    break;
                case Models.Animation.ExportFormat.WebM:
                    savePicker.FileTypeChoices.Add("WebM Video", new[] { ".webm" });
                    suggestedFileName = $"ManpWinUI_{animationType}_{timestamp}.webm";
                    break;
                default:
                    suggestedFileName = $"ManpWinUI_{animationType}_{timestamp}.mp4";
                    break;
            }

            savePicker.SuggestedFileName = suggestedFileName;
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // Show the picker (selecting existing file = overwrite, new name = create new)
            var file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                ViewModel.OutputPath = file.Path;

                // Show confirmation if user selected an existing file
                if (System.IO.File.Exists(file.Path))
                {
                    System.Diagnostics.Debug.WriteLine($"File will be overwritten: {file.Path}");
                    // The save picker already showed overwrite confirmation, so just proceed
                }
            }
        }
        catch (Exception ex)
        {
            // Log error
            System.Diagnostics.Debug.WriteLine($"File picker error: {ex.Message}");

            // Show error to user
            var dialog = new ContentDialog
            {
                Title = "Error Selecting File",
                Content = $"Failed to open file picker: {ex.Message}\n\nUsing default path instead.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();

            // Fallback to default path
            await ViewModel.BrowseOutputPathAsync();
        }
    }
}
