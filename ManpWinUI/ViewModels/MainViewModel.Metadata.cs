namespace ManpWinUI.ViewModels;

/// <summary>
/// MainViewModel partial class - Metadata creation.
/// Handles creation of fractal metadata for image export and persistence.
/// </summary>
public partial class MainViewModel
{
    // ═══════════════════════════════════════════════════════════════════════════════
    // METADATA CREATION
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Creates metadata object from current fractal state.
    /// </summary>
    public Models.FractalMetadata CreateMetadata()
    {
        Models.HailstoneParameters? hailstoneParams = null;

        // Include Hailstone parameters if in Hailstone mode
        if (IsHailstoneMode && CurrentHailstoneResult != null)
        {
            hailstoneParams = new Models.HailstoneParameters
            {
                StartX = HailstoneStartX,
                StartY = HailstoneStartY,
                MaxIterations = HailstoneMaxIterations,
                TotalPoints = CurrentHailstoneResult.Sequence.Count,
                HasCycle = CurrentHailstoneResult.HasCycle,
                CycleStartIndex = CurrentHailstoneResult.HasCycle ? CurrentHailstoneResult.CycleStartIndex : null,
                CycleLength = CurrentHailstoneResult.HasCycle ? CurrentHailstoneResult.CycleLength : null,
                BoundsMinX = CurrentHailstoneResult.MinX,
                BoundsMaxX = CurrentHailstoneResult.MaxX,
                BoundsMinY = CurrentHailstoneResult.MinY,
                BoundsMaxY = CurrentHailstoneResult.MaxY,
                UseFixedViewport = UseFixedHailstoneViewport
            };
        }

        return Models.FractalMetadata.FromViewModel(
            fractalType: SelectedFractalType,
            iterationMode: SelectedIterationMode,
            centerX: CenterX,
            centerY: CenterY,
            zoom: Zoom,
            maxIterations: MaxIterations,
            colorPalette: SelectedPalette,
            imageWidth: ImageWidth,
            imageHeight: ImageHeight,
            autoScaleIterations: AutoScaleIterations,
            juliaCX: IsJuliaMode ? JuliaCX : null,
            juliaCY: IsJuliaMode ? JuliaCY : null,
            renderTime: LastRenderTime,
            hailstone: hailstoneParams
        );
    }
}
