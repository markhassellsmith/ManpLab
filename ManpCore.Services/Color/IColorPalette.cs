namespace ManpCore.Services.Color;

/// <summary>
/// Interface for color palette generation.
/// Platform-agnostic color generation with no UI dependencies.
/// </summary>
public interface IColorPalette
{
    /// <summary>
    /// Gets a color from the palette based on an angle/degree value.
    /// </summary>
    /// <param name="degrees">Hue angle in degrees (0-359).</param>
    /// <returns>RGB color as (R, G, B) byte tuple.</returns>
    (byte R, byte G, byte B) GetColor(int degrees);
}
