using ManpWinUI.Models.LSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ManpWinUI.Services;

/// <summary>
/// L-System rendering service - handles grammar expansion and turtle graphics interpretation.
/// Implements string rewriting (Lindenmayer grammar) and turtle command execution.
/// </summary>
public class LSystemRenderService
{
    // ═══════════════════════════════════════════════════════════════════════════════
    // CORE L-SYSTEM OPERATIONS
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Expand L-System string by applying production rules for specified generations.
    /// </summary>
    /// <param name="definition">L-System definition with axiom and rules</param>
    /// <param name="generations">Number of rule applications (exponential growth!)</param>
    /// <returns>Expanded string ready for turtle interpretation</returns>
    /// <exception cref="InvalidOperationException">Thrown if string grows beyond safe limits</exception>
    public string ExpandLSystem(LSystemDefinition definition, int generations)
    {
        if (definition == null)
            throw new ArgumentNullException(nameof(definition));

        if (generations < 0)
            throw new ArgumentOutOfRangeException(nameof(generations), "Generations cannot be negative");

        if (generations > definition.MaxGenerations)
        {
            throw new ArgumentOutOfRangeException(nameof(generations),
                $"Generations ({generations}) exceeds maximum ({definition.MaxGenerations}) for {definition.Name}");
        }

        string current = definition.Axiom;
        Debug.WriteLine($"[LSystemRenderService] Expanding '{definition.Name}' for {generations} generations");
        Debug.WriteLine($"[LSystemRenderService] Gen 0 (Axiom): {current} (length: {current.Length})");

        // Apply rules iteratively
        for (int gen = 0; gen < generations; gen++)
        {
            var nextGeneration = new StringBuilder(current.Length * 2); // Preallocate for growth

            // Apply rules to each character simultaneously
            foreach (char symbol in current)
            {
                // Find matching rule for this symbol
                var rule = definition.Rules.FirstOrDefault(r => r.Predecessor == symbol);

                if (rule != null)
                {
                    // Apply production rule
                    nextGeneration.Append(rule.Successor);
                }
                else
                {
                    // No rule found - copy symbol unchanged (constants like '+', '-', '[', ']')
                    nextGeneration.Append(symbol);
                }
            }

            current = nextGeneration.ToString();

            // Safety check: prevent string explosion
            if (current.Length > 10_000_000) // 10 million characters
            {
                throw new InvalidOperationException(
                    $"L-System string exceeded safe limit at generation {gen + 1}. " +
                    $"Length: {current.Length:N0} characters. Reduce generations or simplify rules.");
            }

            Debug.WriteLine($"[LSystemRenderService] Gen {gen + 1}: length {current.Length:N0}");
        }

        Debug.WriteLine($"[LSystemRenderService] Final string length: {current.Length:N0} characters");
        return current;
    }

    /// <summary>
    /// Calculate bounding box of L-System path without drawing.
    /// Used for auto-fit scaling and centering.
    /// </summary>
    public (double minX, double maxX, double minY, double maxY) CalculateBounds(
        string lSystemString,
        LSystemDefinition definition,
        double initialStepSize)
    {
        var turtle = new TurtleState(initialStepSize)
        {
            Heading = definition.InitialHeading
        };

        var stateStack = new Stack<TurtleState>();

        double minX = 0, maxX = 0, minY = 0, maxY = 0;
        bool firstPoint = true;

        foreach (char command in lSystemString)
        {
            switch (command)
            {
                case 'F': // Draw forward
                case 'G': // Move forward (also updates bounds)
                    turtle.MoveForward();

                    if (firstPoint)
                    {
                        minX = maxX = turtle.X;
                        minY = maxY = turtle.Y;
                        firstPoint = false;
                    }
                    else
                    {
                        if (turtle.X < minX) minX = turtle.X;
                        if (turtle.X > maxX) maxX = turtle.X;
                        if (turtle.Y < minY) minY = turtle.Y;
                        if (turtle.Y > maxY) maxY = turtle.Y;
                    }
                    break;

                case '+': // Turn left
                    turtle.TurnLeft(definition.TurnAngle);
                    break;

                case '-': // Turn right
                    turtle.TurnRight(definition.TurnAngle);
                    break;

                case '[': // Push state
                    stateStack.Push(turtle.Clone());
                    break;

                case ']': // Pop state
                    if (stateStack.Count > 0)
                        turtle = stateStack.Pop();
                    break;

                case '|': // Reverse heading
                    turtle.Reverse();
                    break;

                // Ignore non-drawing symbols (X, Y, etc.)
                default:
                    break;
            }
        }

        Debug.WriteLine($"[LSystemRenderService] Calculated bounds: X[{minX:F2}, {maxX:F2}], Y[{minY:F2}, {maxY:F2}]");
        return (minX, maxX, minY, maxY);
    }

    /// <summary>
    /// Render L-System to pixel buffer using turtle graphics interpreter.
    /// </summary>
    /// <param name="lSystemString">Expanded L-System command string</param>
    /// <param name="definition">L-System definition for parameters</param>
    /// <param name="pixels">Output BGRA pixel buffer (width * height * 4 bytes)</param>
    /// <param name="width">Image width in pixels</param>
    /// <param name="height">Image height in pixels</param>
    /// <param name="lineColor">RGB color for drawing (default: white)</param>
    public void RenderToPixelBuffer(
        string lSystemString,
        LSystemDefinition definition,
        byte[] pixels,
        int width,
        int height,
        uint lineColor = 0xFFFFFFFF) // ARGB white
    {
        if (pixels == null)
            throw new ArgumentNullException(nameof(pixels));

        if (pixels.Length != width * height * 4)
            throw new ArgumentException("Pixel buffer size mismatch", nameof(pixels));

        Debug.WriteLine($"[LSystemRenderService] Rendering to {width}x{height} buffer");

        // Clear buffer to black
        Array.Fill<byte>(pixels, 0);

        // Calculate bounds for auto-fit
        var initialStepSize = Math.Min(width, height) / 10.0; // Rough initial guess
        var (minX, maxX, minY, maxY) = CalculateBounds(lSystemString, definition, initialStepSize);

        Debug.WriteLine($"[LSystemRenderService] Bounds in turtle coords: X[{minX:F2}, {maxX:F2}], Y[{minY:F2}, {maxY:F2}]");

        // Calculate scale and offset to fit and center in viewport
        double boundsWidth = maxX - minX;
        double boundsHeight = maxY - minY;

        Debug.WriteLine($"[LSystemRenderService] Bounds size: {boundsWidth:F2} x {boundsHeight:F2}");

        double scaleX = (boundsWidth > 0) ? (width * 0.9) / boundsWidth : 1.0;
        double scaleY = (boundsHeight > 0) ? (height * 0.9) / boundsHeight : 1.0;
        double scale = Math.Min(scaleX, scaleY); // Uniform scale

        double offsetX = (width - boundsWidth * scale) / 2.0 - minX * scale;
        double offsetY = (height - boundsHeight * scale) / 2.0 - minY * scale;

        Debug.WriteLine($"[LSystemRenderService] Scale: {scale:F2}, Offset: ({offsetX:F2}, {offsetY:F2})");

        // Initialize turtle with SAME step size used for bounds calculation
        var turtle = new TurtleState(initialStepSize)
        {
            Heading = definition.InitialHeading,
            X = 0,
            Y = 0
        };

        var stateStack = new Stack<TurtleState>();

        // Extract RGB components from ARGB color
        byte r = (byte)((lineColor >> 16) & 0xFF);
        byte g = (byte)((lineColor >> 8) & 0xFF);
        byte b = (byte)(lineColor & 0xFF);

        int drawnLines = 0;

        // Execute turtle commands
        foreach (char command in lSystemString)
        {
            switch (command)
            {
                case 'F': // Draw forward
                case 'G': // Draw forward (some L-Systems use 'G' as second drawing symbol)
                    {
                        double startX = turtle.X;
                        double startY = turtle.Y;
                        turtle.MoveForward();
                        double endX = turtle.X;
                        double endY = turtle.Y;

                        // Transform to screen coordinates (flip Y-axis: screen Y increases downward)
                        int x0 = (int)(startX * scale + offsetX);
                        int y0 = (int)(height - (startY * scale + offsetY)); // Flip Y
                        int x1 = (int)(endX * scale + offsetX);
                        int y1 = (int)(height - (endY * scale + offsetY)); // Flip Y

                        // Draw line using Bresenham algorithm
                        DrawLine(pixels, width, height, x0, y0, x1, y1, r, g, b);
                        drawnLines++;
                    }
                    break;

                case '+': // Turn left
                    turtle.TurnLeft(definition.TurnAngle);
                    break;

                case '-': // Turn right
                    turtle.TurnRight(definition.TurnAngle);
                    break;

                case '[': // Push state (branch)
                    stateStack.Push(turtle.Clone());
                    break;

                case ']': // Pop state (return from branch)
                    if (stateStack.Count > 0)
                        turtle = stateStack.Pop();
                    break;

                case '|': // Reverse heading (180° turn)
                    turtle.Reverse();
                    break;

                // Non-drawing symbols (X, Y, etc.) are ignored
                default:
                    break;
            }
        }

        Debug.WriteLine($"[LSystemRenderService] Rendered {drawnLines:N0} line segments");
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // BRESENHAM LINE RASTERIZER
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Draw line using Bresenham's algorithm (fast integer-only rasterization).
    /// </summary>
    private void DrawLine(byte[] pixels, int width, int height, int x0, int y0, int x1, int y1, byte r, byte g, byte b)
    {
        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            // Plot pixel if in bounds
            if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < height)
            {
                int pixelIndex = (y0 * width + x0) * 4;
                pixels[pixelIndex + 0] = b;  // Blue
                pixels[pixelIndex + 1] = g;  // Green
                pixels[pixelIndex + 2] = r;  // Red
                pixels[pixelIndex + 3] = 255; // Alpha
            }

            if (x0 == x1 && y0 == y1)
                break;

            int e2 = 2 * err;

            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
}
