# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**qulacs-for-unity** is a Unity 6 (6000.4.1f1 LTS) project that provides `com.mu5dvlp.qulacs` — a Unity native plugin package wrapping **Qulacs** (C++ quantum circuit simulator).

For package-specific details, see `Packages/com.mu5dvlp.qulacs/CLAUDE.md`.

## Architecture

```
Unity C# (Mu5dvlp.Qulacs)
    └── P/Invoke
        └── qulacs_unity.dll  (extern "C" C++ wrapper)
            └── Qulacs C++ library
```

## Platform Support

Windows x86_64 (current). Planned: macOS, Android ARM64, iOS.

Plugin paths per platform:
| Platform | Path | Ext |
|---|---|---|
| Windows x86_64 | `Runtime/Plugins/Windows/x86_64/` | `.dll` |
| macOS | `Runtime/Plugins/macOS/` | `.dylib` |
| Android ARM64 | `Runtime/Plugins/Android/ARM64/` | `.so` |
| Android x86_64 | `Runtime/Plugins/Android/x86_64/` | `.so` |
| iOS | `Runtime/Plugins/iOS/` | `.a` |

## Unity Commands

```bash
# EditMode tests
"C:\Program Files\Unity\Hub\Editor\6000.4.1f1\Editor\Unity.exe" -projectPath . -runTests -testPlatform editmode -quit -batchmode -logFile test.log
```

## C# Scripts

Scripts live in `Assets/`. Unity auto-generates `.csproj` — do not edit manually.

## Qulacs Resources

Key files for coverage analysis and API work:

| File | Role |
|---|---|
| `Packages/com.mu5dvlp.qulacs/native~/src/qulacs_unity.h` | C API declarations (all `extern "C"` exports) |
| `Packages/com.mu5dvlp.qulacs/native~/src/qulacs_unity.cpp` | extern "C" wrapper implementation |
| `Packages/com.mu5dvlp.qulacs/Runtime/Internal/NativeMethods.cs` | P/Invoke declarations (C# side) |
| `Packages/com.mu5dvlp.qulacs/Runtime/Plugins/Windows/x86_64/qulacs_unity.dll` | Built DLL (Windows) |
| `Packages/com.mu5dvlp.qulacs/docs/api-reference.md` | Full API reference |
| `Packages/com.mu5dvlp.qulacs/native~/extern/` | Qulacs C++ source and deps (gitignored) |
| `Packages/com.mu5dvlp.qulacs/native~/build/lib/` | Built Qulacs libs |
