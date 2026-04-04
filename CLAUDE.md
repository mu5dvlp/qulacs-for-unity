# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**quri-toolkit** is a Unity 6 (6000.4.1f1 LTS) project whose primary purpose is to provide a Unity native plugin package (`com.mu5dvlp.qulacs`) wrapping **Qulacs** — a C++ quantum circuit simulator by QunaSys.

### Architecture

Qulacs has no C API, so the integration stack is:

```
Unity C# (Mu5dvlp.Qulacs)
    └── P/Invoke
        └── qulacs_unity.dll  (thin extern "C" C++ wrapper)
            └── Qulacs C++ library (libqulacs)
```

The C++ wrapper (`native~/`) wraps Qulacs' C++ API with `extern "C"` functions, compiled into a platform-specific shared library. The C# layer in `Runtime/` calls it via P/Invoke and exposes an idiomatic C# API.

### Platform Strategy

**Current target: Windows x86_64 only.**
Planned: macOS, Android (ARM64), iOS (static lib).

Plugin placement per platform:
| Platform | Path | Extension |
|---|---|---|
| Windows x86_64 | `Runtime/Plugins/Windows/x86_64/` | `.dll` |
| macOS | `Runtime/Plugins/macOS/` | `.dylib` or `.bundle` |
| Android ARM64 | `Runtime/Plugins/Android/ARM64/` | `.so` |
| iOS | `Runtime/Plugins/iOS/` | `.a` (static) |

When adding a new platform, add the compiled library to the corresponding folder and configure its Unity import settings (platform constraints) in the Inspector.

## Package: com.mu5dvlp.qulacs

Located in `Packages/com.mu5dvlp.qulacs/`. Structure:

```
com.mu5dvlp.qulacs/
├── package.json
├── Runtime/
│   ├── Mu5dvlp.Qulacs.Runtime.asmdef
│   ├── Plugins/                        # Compiled native libraries
│   │   └── Windows/x86_64/
│   │       └── qulacs_unity.dll
│   └── *.cs                            # P/Invoke bindings + C# API
├── Editor/
│   └── Mu5dvlp.Qulacs.Editor.asmdef
└── native~/                            # C++ wrapper source (not imported by Unity)
    ├── CMakeLists.txt
    └── src/
        └── qulacs_unity.cpp            # extern "C" wrapper
```

Assembly definitions:
- `Mu5dvlp.Qulacs.Runtime` — namespace `Mu5dvlp.Qulacs`, `allowUnsafeCode: false`
- `Mu5dvlp.Qulacs.Editor` — namespace `Mu5dvlp.Qulacs.Editor`, Editor-only

`native~/` uses a trailing `~` so Unity ignores it during import.

## Building the Native Plugin (Windows)

Requirements: CMake, MSVC (Visual Studio), Qulacs built as a static or shared library.

```bash
cd Packages/com.mu5dvlp.qulacs/native~
cmake -B build -DCMAKE_BUILD_TYPE=Release
cmake --build build --config Release
# Copy output DLL to Runtime/Plugins/Windows/x86_64/
```

## Build & Run (Unity)

```bash
# Command-line build (Windows player)
"C:\Program Files\Unity\Hub\Editor\6000.4.1f1\Editor\Unity.exe" -projectPath . -buildWindowsPlayer Build/quri-toolkit.exe -quit -batchmode -logFile build.log

# Run EditMode tests
"C:\Program Files\Unity\Hub\Editor\6000.4.1f1\Editor\Unity.exe" -projectPath . -runTests -testPlatform editmode -quit -batchmode -logFile test.log
```

VS Code is configured with the **Visual Studio Tools for Unity (VSTUC)** extension. Attach to the Unity Editor via the "Unity Editor" launch config in `.vscode/launch.json`.

## C# Scripts

Scripts live in `Assets/`. Editor-only scripts must be in a folder named `Editor/` to avoid inclusion in player builds. Unity auto-generates `.csproj` files — do not edit them manually.
