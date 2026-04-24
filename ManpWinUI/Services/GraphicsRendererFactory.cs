namespace ManpWinUI.Services
{
    /// <summary>
    /// Graphics rendering backend options.
    /// </summary>
    public enum GraphicsBackend
    {
        /// <summary>
        /// Win2D (DirectX-based, GPU accelerated, Windows-only).
        /// Default for current implementation.
        /// </summary>
        Win2D,

        /// <summary>
        /// SkiaSharp (cross-platform, CPU-based).
        /// To be implemented in future branch.
        /// </summary>
        SkiaSharp,

        /// <summary>
        /// Legacy manual pixel manipulation (current byte[] approach).
        /// Kept for fallback compatibility.
        /// </summary>
        LegacyPixelBuffer
    }

    /// <summary>
    /// Factory for creating graphics renderer instances.
    /// Centralizes renderer selection logic.
    /// </summary>
    public static class GraphicsRendererFactory
    {
        /// <summary>
        /// Gets or sets the preferred graphics backend.
        /// Can be configured via settings or environment variables.
        /// </summary>
        public static GraphicsBackend PreferredBackend { get; set; } = GraphicsBackend.Win2D;

        /// <summary>
        /// Creates a graphics renderer instance based on the preferred backend.
        /// </summary>
        /// <param name="width">Width of rendering surface in pixels.</param>
        /// <param name="height">Height of rendering surface in pixels.</param>
        /// <param name="forceBackend">Optional: Force a specific backend regardless of preference.</param>
        /// <returns>A graphics renderer instance.</returns>
        /// <exception cref="NotSupportedException">Thrown if the requested backend is not available.</exception>
        public static IGraphicsRenderer Create(int width, int height, GraphicsBackend? forceBackend = null)
        {
            var backend = forceBackend ?? PreferredBackend;

            return backend switch
            {
                GraphicsBackend.Win2D => new Win2DGraphicsRenderer(width, height),

                GraphicsBackend.SkiaSharp => new SkiaGraphicsRenderer(width, height),

                GraphicsBackend.LegacyPixelBuffer => throw new NotImplementedException(
                    "Legacy pixel buffer renderer not yet wrapped in IGraphicsRenderer interface. " +
                    "This would wrap the existing HailstoneRenderService byte[] approach."),

                _ => throw new NotSupportedException($"Graphics backend {backend} is not supported.")
            };
        }

        /// <summary>
        /// Checks if a specific graphics backend is available (dependencies installed).
        /// </summary>
        public static bool IsBackendAvailable(GraphicsBackend backend)
        {
            return backend switch
            {
                GraphicsBackend.Win2D => true, // Currently implemented
                GraphicsBackend.SkiaSharp => false, // Not yet implemented
                GraphicsBackend.LegacyPixelBuffer => true, // Always available (existing code)
                _ => false
            };
        }

        /// <summary>
        /// Gets a list of all available backends for the current platform.
        /// </summary>
        public static IEnumerable<GraphicsBackend> GetAvailableBackends()
        {
            return Enum.GetValues<GraphicsBackend>()
                .Where(IsBackendAvailable);
        }
    }
}
