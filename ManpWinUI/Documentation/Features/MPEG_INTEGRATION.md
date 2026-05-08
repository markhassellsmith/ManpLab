# MPEG Video Export Integration

## Overview

The `MPEG/` folder contains a legacy MPEG-2 encoder from Paul DeLeeuw's original codebase. This encoder is used to export fractal animation sequences to video files.

## Current Architecture

```
Animation Workflow:
User Creates Animation → BatchRenderer (Week 3) → Frame Sequence → MPEGWrite.cpp → MPEG-2 Encoder → .mpg file
```

### Key Components

**Native C++ Layer:**
- `MPEG/mpeg2enc.cpp` - MPEG-2 encoder implementation
- `MPEG/mpeg.sln` - Standalone solution for MPEG encoder project
- `ManpWIN64/MPEGWrite.cpp` - Integration interface between ManpWIN64 and MPEG encoder
- `ManpWIN64/anim.h` - Animation frame structure definitions

**Existing Infrastructure:**
- `ManpCore.Native/BatchRenderer.*` (✅ Week 3 Complete)
  - Keyframe-based animation system
  - 5 interpolation modes (Linear, EaseIn, EaseOut, EaseInOut, Exponential)
  - Frame generation and progress tracking
  - Queue management for batch operations

## Integration Plan for Week 10

### Phase 1: MVP (Use Existing MPEG-2)

**Tasks:**
1. Create C++/CLI wrapper in `ManpCore.Native/`:
   ```cpp
   // VideoExporter.h
   public ref class VideoExporter {
   public:
       static bool ExportToMPEG(
           array<AnimationFrame^>^ frames,
           String^ outputPath,
           int width, int height,
           int frameRate);
   };
   ```

2. Bridge to legacy code:
   - Wrap `MPEGWrite.cpp` functionality
   - Convert managed animation frames to native format
   - Handle file I/O and error reporting

3. Add WinUI dialog:
   - `Views/Dialogs/VideoExportDialog.xaml`
   - Settings: output path, frame rate, quality
   - Progress bar with cancellation support

4. Wire to BatchRenderer:
   - After frame generation completes
   - Pass frame buffer to VideoExporter
   - Show export progress in UI

**Benefits:**
- ✅ Proven, working implementation
- ✅ No external dependencies
- ✅ Already integrated with legacy animation code
- ✅ Unblocks Week 10 deliverable

**Limitations:**
- ❌ MPEG-2 is outdated (1995 standard)
- ❌ Large file sizes compared to modern codecs
- ❌ Limited browser/device compatibility

### Phase 2: Modernization (Post-Release)

**Option A: Windows Media Foundation**
```cpp
// Use Windows.Media.MediaComposition
// Pros: Native to Windows, no dependencies
// Cons: Windows-only, may require Win11
```

**Option B: FFmpeg Integration**
```cpp
// Link FFmpeg libraries for H.264/H.265
// Pros: Industry standard, small files, cross-platform
// Cons: Additional 5-10MB dependency, licensing considerations
```

**Recommended Approach:**
1. Start with Windows Media Foundation (built-in to Windows 10+)
2. Fallback to FFmpeg if WMF unavailable or for advanced features
3. Keep MPEG-2 as final fallback for compatibility

**Migration Tasks:**
- Create `IVideoEncoder` interface
- Implement `MPEG2Encoder`, `WMFEncoder`, `FFmpegEncoder`
- Add codec selection in export dialog
- Update documentation

## File Format Support

### Current (Week 10 MVP)
- `.mpg` - MPEG-2 video

### Future Enhancements
- `.mp4` - H.264/H.265 (most compatible)
- `.webm` - VP9 (web-friendly)
- `.gif` - Animated GIF (already supported via `anim.h`)
- `.apng` - Animated PNG (lossless alternative)

## Build Configuration

**Important:** The `ManpCore.Native.vcxproj` was modified to fix a build error where a PostBuildEvent was conflicting with a custom MSBuild Target. The redundant PostBuildEvent was removed, and the `CopyToSolutionOutput` target handles DLL copying for both Debug and Release configurations.

**Verify after MPEG integration:**
```powershell
# Ensure MPEG encoder builds correctly
msbuild MPEG\mpeg.vcxproj /p:Configuration=Release
# Should output: MPEG\x64\Release\mpeg.lib
```

## Testing Checklist (Week 10)

- [ ] Generate 10-frame animation with BatchRenderer
- [ ] Export to MPEG-2 with 30fps
- [ ] Verify file playback in Windows Media Player
- [ ] Test cancellation during export
- [ ] Test error handling (disk full, invalid path)
- [ ] Verify memory cleanup after export
- [ ] Performance: 1920x1080 @ 30fps should export in <5 seconds

## References

- `docs/BatchRenderer-Guide.md` - Frame generation system
- `ManpWIN64/MPEGWrite.cpp` - Legacy integration code
- `MPEG/mpeg2enc.cpp` - Encoder implementation
- Paul DeLeeuw's original ManpWIN documentation (see HTMLHelp/)

## Decision Log

**2025-01-XX:** Investigated MPEG folder purpose after build error fix. Confirmed it's required for animation video export (Week 10 feature). Decision: Keep MPEG folder and solution until post-release modernization phase.

**Build Error Fix:** Removed redundant PostBuildEvent from `ManpCore.Native.vcxproj` that was conflicting with `CopyToSolutionOutput` MSBuild target. DLL copying now works correctly for both Debug and Release builds.
