using ManpWinUI.Models;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace ManpWinUI.Services;

/// <summary>
/// Service for exporting fractal images with embedded metadata.
/// Supports PNG (with tEXt chunks), JPEG (with EXIF), and future SVG support.
/// </summary>
public class ImageExportService : IImageExportService
{
    /// <summary>
    /// Saves a fractal image to a file with embedded metadata.
    /// </summary>
    /// <param name="bitmap">The WriteableBitmap to save</param>
    /// <param name="metadata">Fractal metadata to embed</param>
    /// <param name="format">Image format (PNG or JPEG)</param>
    /// <param name="hwnd">Window handle for FileSavePicker</param>
    /// <returns>True if saved successfully, false if cancelled</returns>
    public async Task<bool> SaveImageAsync(
        WriteableBitmap bitmap,
        FractalMetadata metadata,
        ImageFormat format,
        IntPtr hwnd)
    {
        // Create file picker
        var savePicker = new FileSavePicker();

        // Initialize with window handle (required for WinUI 3)
        WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

        // Set picker properties based on format
        savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        savePicker.SuggestedFileName = GenerateFileName(metadata);

        switch (format)
        {
            case ImageFormat.PNG:
                savePicker.FileTypeChoices.Add("PNG Image", new[] { ".png" });
                break;
            case ImageFormat.JPEG:
                savePicker.FileTypeChoices.Add("JPEG Image", new[] { ".jpg", ".jpeg" });
                break;
        }

        // Show picker
        var file = await savePicker.PickSaveFileAsync();
        if (file == null)
            return false; // User cancelled

        // Save to file
        await SaveBitmapToFileAsync(bitmap, metadata, file, format);
        return true;
    }

    /// <summary>
    /// Copies the fractal image to clipboard with metadata.
    /// </summary>
    public async Task CopyToClipboardAsync(WriteableBitmap bitmap, FractalMetadata metadata)
    {
        var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();

        // Create a temporary in-memory stream
        using var stream = new InMemoryRandomAccessStream();

        // Encode as PNG with metadata
        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

        // Get pixel data from WriteableBitmap
        var pixelBuffer = bitmap.PixelBuffer;
        var pixels = pixelBuffer.ToArray();

        encoder.SetPixelData(
            BitmapPixelFormat.Bgra8,
            BitmapAlphaMode.Premultiplied,
            (uint)bitmap.PixelWidth,
            (uint)bitmap.PixelHeight,
            96.0,
            96.0,
            pixels);

        // Add metadata as PNG tEXt chunk
        await AddPngMetadataAsync(encoder, metadata);

        await encoder.FlushAsync();

        // Copy to clipboard
        stream.Seek(0);
        dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));

        // Also add metadata as text
        dataPackage.SetText($"Fractal: {metadata.FractalType} at ({metadata.CenterX:F8}, {metadata.CenterY:F8}), Zoom: {metadata.Zoom:F2}x");

        Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
    }

    /// <summary>
    /// Saves bitmap to a file with embedded metadata.
    /// </summary>
    private async Task SaveBitmapToFileAsync(
        WriteableBitmap bitmap,
        FractalMetadata metadata,
        StorageFile file,
        ImageFormat format)
    {
        using var stream = await file.OpenAsync(FileAccessMode.ReadWrite);

        // Create encoder based on format
        BitmapEncoder encoder = format switch
        {
            ImageFormat.PNG => await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream),
            ImageFormat.JPEG => await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream),
            _ => throw new ArgumentException($"Unsupported format: {format}")
        };

        // Get pixel data from WriteableBitmap
        var pixelBuffer = bitmap.PixelBuffer;
        var pixels = pixelBuffer.ToArray();

        // Set pixel data
        encoder.SetPixelData(
            BitmapPixelFormat.Bgra8,
            BitmapAlphaMode.Premultiplied,
            (uint)bitmap.PixelWidth,
            (uint)bitmap.PixelHeight,
            96.0, // DPI X
            96.0, // DPI Y
            pixels);

        // Add metadata based on format
        if (format == ImageFormat.PNG)
        {
            await AddPngMetadataAsync(encoder, metadata);
        }
        else if (format == ImageFormat.JPEG)
        {
            await AddJpegMetadataAsync(encoder, metadata);
        }

        // Save
        await encoder.FlushAsync();
    }

    /// <summary>
    /// Adds metadata to PNG as tEXt chunks.
    /// </summary>
    private async Task AddPngMetadataAsync(BitmapEncoder encoder, FractalMetadata metadata)
    {
        var properties = encoder.BitmapProperties;

        // Serialize metadata to JSON
        var jsonMetadata = JsonSerializer.Serialize(metadata, new JsonSerializerOptions 
        { 
            WriteIndented = false 
        });

        // Add metadata as PNG tEXt chunks
        var metadataProperties = new[]
        {
            // Individual fields for easy reading in image viewers
            new BitmapPropertySet
            {
                { "/tEXt/Software", new BitmapTypedValue($"{metadata.Software} {metadata.Version}", Windows.Foundation.PropertyType.String) }
            },
            new BitmapPropertySet
            {
                { "/tEXt/FractalType", new BitmapTypedValue(metadata.FractalType, Windows.Foundation.PropertyType.String) }
            },
            new BitmapPropertySet
            {
                { "/tEXt/Center", new BitmapTypedValue($"{metadata.CenterX:R},{metadata.CenterY:R}", Windows.Foundation.PropertyType.String) }
            },
            new BitmapPropertySet
            {
                { "/tEXt/Zoom", new BitmapTypedValue(metadata.Zoom.ToString("R"), Windows.Foundation.PropertyType.String) }
            },
            new BitmapPropertySet
            {
                { "/tEXt/MaxIterations", new BitmapTypedValue(metadata.MaxIterations.ToString(), Windows.Foundation.PropertyType.String) }
            },
            new BitmapPropertySet
            {
                { "/tEXt/ColorPalette", new BitmapTypedValue(metadata.ColorPalette, Windows.Foundation.PropertyType.String) }
            },
            // Complete metadata as JSON for easy parsing
            new BitmapPropertySet
            {
                { "/tEXt/ManpLabMetadata", new BitmapTypedValue(jsonMetadata, Windows.Foundation.PropertyType.String) }
            }
        };

        foreach (var propSet in metadataProperties)
        {
            try
            {
                await properties.SetPropertiesAsync(propSet);
            }
            catch
            {
                // Some PNG properties might not be supported, continue anyway
            }
        }
    }

    /// <summary>
    /// Adds metadata to JPEG as EXIF.
    /// </summary>
    private async Task AddJpegMetadataAsync(BitmapEncoder encoder, FractalMetadata metadata)
    {
        var properties = encoder.BitmapProperties;

        // Serialize metadata to JSON for EXIF UserComment
        var jsonMetadata = JsonSerializer.Serialize(metadata, new JsonSerializerOptions 
        { 
            WriteIndented = false 
        });

        var metadataProperties = new BitmapPropertySet
        {
            // Standard EXIF fields
            { "/app1/ifd/exif/{ushort=37510}", new BitmapTypedValue(jsonMetadata, Windows.Foundation.PropertyType.String) }, // UserComment
            { "/app1/ifd/{ushort=305}", new BitmapTypedValue($"{metadata.Software} {metadata.Version}", Windows.Foundation.PropertyType.String) }, // Software
            { "/app1/ifd/{ushort=270}", new BitmapTypedValue($"{metadata.FractalType} fractal", Windows.Foundation.PropertyType.String) } // ImageDescription
        };

        try
        {
            await properties.SetPropertiesAsync(metadataProperties);
        }
        catch
        {
            // EXIF properties might fail, that's okay
        }
    }

    /// <summary>
    /// Generates a filename based on fractal metadata.
    /// </summary>
    private string GenerateFileName(FractalMetadata metadata)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var fractalName = metadata.FractalType.Replace(" ", "");
        var mode = metadata.IterationMode == "Julia" ? "_Julia" : "";

        return $"{fractalName}{mode}_{timestamp}";
    }
}

/// <summary>
/// Supported image export formats.
/// </summary>
public enum ImageFormat
{
    PNG,
    JPEG,
    SVG // Future: for 2D Hailstone
}
