# BatchRenderer Guide

**Phase 1, Week 3 Deliverable** - Batch rendering and animation system for ManpLab

## Overview

The `BatchRenderer` enables:
- Queuing multiple fractal render jobs
- Creating smooth animations with keyframe interpolation
- Progress tracking for long-running batch operations
- Cancellation support
- Error handling per job

## Architecture

```
C# Application Layer
    ↓
BatchRenderer (C++/CLI) ← NEW in Week 3
    ↓
FractalEngineWrapper (C++/CLI)
    ↓
Native Fractal Calculators (C++)
```

## Basic Usage

### Single Job Queue

```csharp
using ManpCore.Native;

// Create batch renderer
var batch = new BatchRenderer();

// Subscribe to events
batch.ProgressChanged += (sender, e) => 
{
    Console.WriteLine($"Overall: {e.OverallProgress:F1}% - Job: {e.Job.Name}");
};

batch.JobCompleted += (sender, e) => 
{
    if (e.Job.Status == BatchJobStatus.Completed)
        Console.WriteLine($"✓ {e.Job.Name} completed");
    else
        Console.WriteLine($"✗ {e.Job.Name} failed: {e.Job.ErrorMessage}");
};

// Queue jobs
var params1 = new FractalParameters 
{
    FractalType = "Mandelbrot",
    Width = 1920,
    Height = 1080,
    CenterX = -0.5,
    CenterY = 0.0,
    ViewWidth = 3.0,
    MaxIterations = 1000,
    Palette = ColorPalette.Rainbow
};

batch.AddJob("Mandelbrot Full View", params1, null);

var params2 = new FractalParameters 
{
    FractalType = "BurningShip",
    Width = 1920,
    Height = 1080,
    CenterX = -0.4,
    CenterY = -0.6,
    ViewWidth = 0.5,
    MaxIterations = 1000,
    Palette = ColorPalette.Fire
};

batch.AddJob("Burning Ship Detail", params2, null);

// Process all jobs
batch.ProcessAll();

// Access results
foreach (var job in batch.Jobs)
{
    if (job.Status == BatchJobStatus.Completed)
    {
        // job.ImageData contains BGRA pixel data
        // Use ImageExportService to save to file if needed
    }
}
```

## Animation Creation

### Zoom Animation

```csharp
var batch = new BatchRenderer();

// Define keyframes
var keyframes = new List<AnimationKeyframe>();

// Start: Full Mandelbrot view
keyframes.Add(new AnimationKeyframe 
{
    Time = 0.0,
    Duration = 2.0, // 2 seconds to next keyframe
    Parameters = new FractalParameters 
    {
        FractalType = "Mandelbrot",
        Width = 1920,
        Height = 1080,
        CenterX = -0.5,
        CenterY = 0.0,
        ViewWidth = 3.0,
        MaxIterations = 256,
        Palette = ColorPalette.Classic
    }
});

// Middle: Zoom into interesting area
keyframes.Add(new AnimationKeyframe 
{
    Time = 0.5,
    Duration = 2.0,
    Parameters = new FractalParameters 
    {
        FractalType = "Mandelbrot",
        Width = 1920,
        Height = 1080,
        CenterX = -0.7463,
        CenterY = 0.1102,
        ViewWidth = 0.01,
        MaxIterations = 1000,
        Palette = ColorPalette.Classic
    }
});

// End: Deep zoom
keyframes.Add(new AnimationKeyframe 
{
    Time = 1.0,
    Duration = 0.0, // Last keyframe
    Parameters = new FractalParameters 
{
        FractalType = "Mandelbrot",
        Width = 1920,
        Height = 1080,
        CenterX = -0.7463,
        CenterY = 0.1102,
        ViewWidth = 0.0001,
        MaxIterations = 2000,
        Palette = ColorPalette.Classic
    }
});

// Create animation (30 FPS, exponential zoom)
string outputDir = @"C:\Fractals\Animations\ZoomTest";
int frameCount = batch.CreateAnimation(
    "MandelbrotZoom",
    keyframes,
    fps: 30,
    InterpolationMode.Exponential,
    outputDir
);

Console.WriteLine($"Created {frameCount} frames");

// Process all frames
batch.ProcessAll();
```

### Pan Animation

```csharp
var batch = new BatchRenderer();
var keyframes = new List<AnimationKeyframe>();

// Pan horizontally across the Mandelbrot set
for (int i = 0; i <= 10; i++)
{
    double progress = i / 10.0;
    keyframes.Add(new AnimationKeyframe 
    {
        Time = progress,
        Duration = 0.5, // 0.5 seconds between keyframes
        Parameters = new FractalParameters 
        {
            FractalType = "Mandelbrot",
            Width = 1920,
            Height = 1080,
            CenterX = -1.5 + progress * 1.5, // Pan from -1.5 to 0.0
            CenterY = 0.0,
            ViewWidth = 1.0,
            MaxIterations = 500,
            Palette = ColorPalette.Rainbow
        }
    });
}

// Use EaseInOut for smooth start/stop
batch.CreateAnimation("PanAnimation", keyframes, 60, InterpolationMode.EaseInOut, outputDir);
batch.ProcessAll();
```

## Interpolation Modes

### Linear
Constant speed throughout animation:
```csharp
InterpolationMode.Linear
```

### EaseIn
Slow start, accelerating:
```csharp
InterpolationMode.EaseIn  // Uses t²
```

### EaseOut
Fast start, decelerating:
```csharp
InterpolationMode.EaseOut  // Uses 1-(1-t)²
```

### EaseInOut
Smooth start and stop:
```csharp
InterpolationMode.EaseInOut  // Uses cubic ease
```

### Exponential
Logarithmic zoom (constant perceived zoom speed):
```csharp
InterpolationMode.Exponential  // Best for zoom animations
```

## Progress Tracking

```csharp
batch.ProgressChanged += (sender, e) => 
{
    // e.OverallProgress: 0-100 for entire batch
    // e.CurrentJobIndex: Which job is running
    // e.TotalJobs: Total jobs in batch
    // e.Job.Progress: 0-100 for current job

    UpdateProgressBar(e.OverallProgress);
    UpdateStatusText($"Rendering {e.Job.Name} ({e.CurrentJobIndex}/{e.TotalJobs})");
};
```

## Cancellation

```csharp
// From UI thread
private BatchRenderer _currentBatch;

void StartBatch()
{
    _currentBatch = new BatchRenderer();
    // ... add jobs ...

    Task.Run(() => _currentBatch.ProcessAll());
}

void CancelButton_Click(object sender, RoutedEventArgs e)
{
    _currentBatch?.CancelAll();
}
```

## Job Management

```csharp
var batch = new BatchRenderer();

// Add jobs
batch.AddJob("Job1", params1, null);
batch.AddJob("Job2", params2, null);
batch.AddJob("Job3", params3, null);

// Check status before processing
Console.WriteLine($"Pending: {batch.PendingJobCount}");

// Process
batch.ProcessAll();

// Check results
Console.WriteLine($"Completed: {batch.CompletedJobCount}");
Console.WriteLine($"Failed: {batch.FailedJobCount}");

// Clear completed jobs, keep pending
batch.ClearJobs(includePending: false);

// Add more jobs
batch.AddJob("Job4", params4, null);

// Process new jobs
batch.ProcessAll();
```

## Saving Images

The BatchRenderer produces raw BGRA pixel data. Use C#'s ImageExportService to save:

```csharp
using ManpWinUI.Services;

var imageExport = new ImageExportService();

batch.JobCompleted += async (sender, e) => 
{
    if (e.Job.Status == BatchJobStatus.Completed && !string.IsNullOrEmpty(e.Job.OutputPath))
    {
        // Convert BGRA array to WriteableBitmap
        var bitmap = new WriteableBitmap(e.Job.Parameters.Width, e.Job.Parameters.Height);
        using (var stream = bitmap.PixelBuffer.AsStream())
        {
            await stream.WriteAsync(e.Job.ImageData, 0, e.Job.ImageData.Length);
        }

        // Save to file
        await imageExport.SaveImageAsync(bitmap, e.Job.OutputPath);
    }
};
```

## Performance Considerations

### Memory
- Each job holds a full BGRA image in memory
- 1920×1080 = ~8MB per frame
- 300 frames = ~2.4GB RAM
- Clear completed jobs periodically for long batches

### Speed
- Processing is sequential (one job at a time)
- Future: Add parallel rendering option
- Animation generation is fast (parameter interpolation)
- Actual rendering time dominates

### Threading
- `ProcessAll()` is blocking - use `Task.Run()` for async
- Progress events fire on worker thread - marshal to UI thread
- Safe to call `CancelAll()` from any thread

## Integration with UI (Phase 2)

### Future Animation Builder Dialog

```
┌─────────────────────────────────────┐
│  Animation Builder                  │
├─────────────────────────────────────┤
│  Keyframes:                         │
│  [0.0s] Mandelbrot Full View        │
│  [2.0s] Zoom intermediate           │
│  [4.0s] Deep zoom detail            │
│                                     │
│  [Add Keyframe] [Remove] [Edit]    │
│                                     │
│  FPS: [30▼]  Mode: [Exponential▼]  │
│  Duration: 4.0s  Frames: 120        │
│                                     │
│  Output: [C:\Fractals\Anims\...]    │
│                                     │
│  [Preview] [Render]  [Cancel]      │
└─────────────────────────────────────┘
```

## Examples

See `ManpWinUI.Tests/BatchRendererTests.cs` for unit tests and more examples.

---

**Status**: ✅ Implemented (Phase 1, Week 3)  
**Next**: Phase 2 UI integration  
**Branch**: feature/week3-batch-renderer
