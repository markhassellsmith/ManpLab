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
        // Get ViewModel from DI
        ViewModel = App.Current.Services.GetRequiredService<AnimationViewModel>();

        this.InitializeComponent();
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

            // Set file type filters based on selected export format
            switch (ViewModel.SelectedExportFormat)
            {
                case Models.Animation.ExportFormat.MP4:
                    savePicker.FileTypeChoices.Add("MP4 Video", new[] { ".mp4" });
                    savePicker.SuggestedFileName = "animation.mp4";
                    break;
                case Models.Animation.ExportFormat.GIF:
                    savePicker.FileTypeChoices.Add("Animated GIF", new[] { ".gif" });
                    savePicker.SuggestedFileName = "animation.gif";
                    break;
                case Models.Animation.ExportFormat.PNGSequence:
                    savePicker.FileTypeChoices.Add("ZIP Archive", new[] { ".zip" });
                    savePicker.SuggestedFileName = "animation.zip";
                    break;
                case Models.Animation.ExportFormat.WebM:
                    savePicker.FileTypeChoices.Add("WebM Video", new[] { ".webm" });
                    savePicker.SuggestedFileName = "animation.webm";
                    break;
            }

            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // Show the picker
            var file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                ViewModel.OutputPath = file.Path;
            }
        }
        catch (Exception ex)
        {
            // Log error
            System.Diagnostics.Debug.WriteLine($"File picker error: {ex.Message}");

            // Fallback to default path
            await ViewModel.BrowseOutputPathAsync();
        }
    }
}
