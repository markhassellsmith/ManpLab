using System;

namespace ManpWinUI.Models.LSystem;

/// <summary>
/// Turtle graphics state for L-System rendering.
/// Tracks position, heading, and drawing pen state.
/// </summary>
public struct TurtleState
{
    /// <summary>
    /// Current X position in viewport coordinates.
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Current Y position in viewport coordinates.
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Current heading angle in degrees (0 = right, 90 = up).
    /// </summary>
    public double Heading { get; set; }

    /// <summary>
    /// Step size for forward movement commands ('F', 'G').
    /// Scaled based on generation depth and viewport size.
    /// </summary>
    public double StepSize { get; set; }

    /// <summary>
    /// Current drawing color index (for multi-color L-Systems).
    /// Default: 0 (primary color)
    /// </summary>
    public int ColorIndex { get; set; }

    /// <summary>
    /// Create a new turtle at origin facing right.
    /// </summary>
    public TurtleState(double stepSize)
    {
        X = 0;
        Y = 0;
        Heading = 0;
        StepSize = stepSize;
        ColorIndex = 0;
    }

    /// <summary>
    /// Create a copy of the current turtle state (for stack push '[').
    /// </summary>
    public TurtleState Clone()
    {
        return new TurtleState
        {
            X = this.X,
            Y = this.Y,
            Heading = this.Heading,
            StepSize = this.StepSize,
            ColorIndex = this.ColorIndex
        };
    }

    /// <summary>
    /// Move forward by StepSize in current heading direction.
    /// </summary>
    public void MoveForward()
    {
        double radians = Heading * Math.PI / 180.0;
        X += StepSize * Math.Cos(radians);
        Y += StepSize * Math.Sin(radians);
    }

    /// <summary>
    /// Turn left (counter-clockwise) by specified angle.
    /// </summary>
    public void TurnLeft(double angle)
    {
        Heading += angle;
        while (Heading >= 360.0) Heading -= 360.0;
    }

    /// <summary>
    /// Turn right (clockwise) by specified angle.
    /// </summary>
    public void TurnRight(double angle)
    {
        Heading -= angle;
        while (Heading < 0.0) Heading += 360.0;
    }

    /// <summary>
    /// Reverse heading by 180 degrees.
    /// </summary>
    public void Reverse()
    {
        Heading += 180.0;
        while (Heading >= 360.0) Heading -= 360.0;
    }
}
