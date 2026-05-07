# Graphics Rendering Abstraction

## Overview

This directory contains an abstraction layer for 2D graphics rendering, allowing ManpLab to swap between different rendering backends without changing business logic.

## Current Architecture

```
IGraphicsRenderer (interface)
├── Win2DGraphicsRenderer (current implementation)
├── SkiaGraphicsRenderer (future implementation)
└── LegacyPixelBufferRenderer (fallback - to be implemented)
```

## Design Goals

1. **Platform Flexibility**: Easy transition between Windows-only (Win2D) and cross-platform (SkiaSharp)
2. **Performance Options**: Choose GPU-accelerated (Win2D) or CPU-based (SkiaSharp) rendering
3. **Testing**: Mock graphics operations for unit tests
4. **Future-Proofing**: Add new backends (WebGL, Metal, etc.) without refactoring

## Current Status (Phase 4)

- ✅ **Win2D Implementation**: Fully functional, GPU-accelerated
- ⏳ **SkiaSharp Implementation**: Stub created, to be implemented in future branch
- ⏳ **Legacy Wrapper**: Not yet wrapped (existing HailstoneRenderService uses byte[] directly)

## Switching Backends

### Option 1: Global Configuration (Recommended)
```csharp
// In App.xaml.cs or configuration file
GraphicsRendererFactory.PreferredBackend = GraphicsBackend.Win2D; // or SkiaSharp
```

### Option 2: Per-Instance Override
```csharp
// Force a specific backend for testing or special cases
using var renderer = GraphicsRendererFactory.Create(width, height, 
    forceBackend: GraphicsBackend.SkiaSharp);
```

### Option 3: Runtime Detection
```csharp
// Automatically choose based on available dependencies
var available = GraphicsRendererFactory.GetAvailableBackends();
var backend = available.Contains(GraphicsBackend.Win2D) 
    ? GraphicsBackend.Win2D 
    : GraphicsBackend.SkiaSharp;
```

## Implementation Roadmap

### Phase 4 (Current - feature/phase4-ui-polish)
- [x] Create abstraction interfaces
- [x] Implement Win2D backend
- [x] Create SkiaSharp stub
- [ ] Add Win2D NuGet package: `Microsoft.Graphics.Win2D`
- [ ] Update HailstoneRenderService to use abstraction
- [ ] Test Win2D rendering with all features

### Phase 5 (Future Branch - feature/skia-renderer)
- [ ] Add SkiaSharp NuGet packages: `SkiaSharp`, `SkiaSharp.Views.WinUI`
- [ ] Implement SkiaGraphicsRenderer class
- [ ] Add backend selection UI in settings
- [ ] Performance comparison testing
- [ ] Update documentation with benchmarks

### Phase 6 (Optional - Cross-Platform)
- [ ] Test on Linux/macOS with SkiaSharp backend
- [ ] Create platform-specific factory logic
- [ ] Add fallback chains (Win2D → SkiaSharp → Legacy)

## API Surface

### Core Drawing Operations
```csharp
void Clear(Color color)
void DrawLine(int x1, int y1, int x2, int y2, Color color, float thickness)
void DrawText(string text, float x, float y, Color color, float fontSize, ...)
void DrawCircle(float centerX, float centerY, float radius, Color color)
void DrawRectangle(float x, float y, float width, float height, Color color)
```

### Text Operations
```csharp
(float width, float height) MeasureText(string text, float fontSize, ...)
```

### Alpha/Transparency
```csharp
void SetAlpha(byte alpha)  // Affects subsequent draw calls
```

### Output
```csharp
WriteableBitmap ToWriteableBitmap()  // For WinUI display
Task SaveToFileAsync(string filePath)  // PNG export
```

## Benefits of This Approach

1. **No Code Duplication**: Business logic (coordinate transforms, sequence rendering) stays the same
2. **Easy Testing**: Mock `IGraphicsRenderer` for unit tests
3. **Future-Proof**: Add DirectWrite, Cairo, or other backends later
4. **Performance Flexibility**: Choose GPU vs CPU based on deployment scenario
5. **Clean Separation**: Graphics API details hidden from domain logic

## NuGet Dependencies

### Current (Win2D)
```xml
<PackageReference Include="Microsoft.Graphics.Win2D" Version="1.2.0" />
```

### Future (SkiaSharp)
```xml
<PackageReference Include="SkiaSharp" Version="2.88.0" />
<PackageReference Include="SkiaSharp.Views.WinUI" Version="2.88.0" />
```

## Migration Strategy

### For Existing Code
1. Keep current `HailstoneRenderService` as-is (byte[] pixel manipulation)
2. Create new `HailstoneRenderServiceV2` using `IGraphicsRenderer`
3. Add feature flag to switch between implementations
4. Test both side-by-side
5. Deprecate old service once new one is validated

### For New Features
- Always use `IGraphicsRenderer` abstraction
- Default to Win2D for Windows deployment
- Plan SkiaSharp implementation for cross-platform milestone

## Questions & Decisions

### Why Not Use Existing Abstractions?
- **System.Drawing**: Not available in WinUI 3, deprecated for modern .NET
- **ImageSharp**: Good for image processing, but limited real-time rendering API
- **MonoGame/XNA**: Too heavyweight, game-focused

### Why This Custom Interface?
- Tailored to ManpLab's specific needs (scientific visualization)
- Minimal surface area (easier to implement multiple backends)
- No unnecessary dependencies
- Full control over performance characteristics

## Contact

For questions about graphics rendering architecture:
- Review this README
- Check `HailstoneRenderServiceRefactored.cs` for usage examples
- See issue tracker for backend implementation status
