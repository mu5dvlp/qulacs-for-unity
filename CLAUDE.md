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

Windows x86_64, macOS x86_64, Linux x86_64, Android ARM64, Android x86_64, iOS ARM64, WebGL (WebAssembly).

Plugin paths per platform:
| Platform | Path | Ext |
|---|---|---|
| Windows x86_64 | `Runtime/Plugins/Windows/x86_64/` | `.dll` |
| macOS | `Runtime/Plugins/macOS/` | `.dylib` |
| Android ARM64 | `Runtime/Plugins/Android/ARM64/` | `.so` |
| Android x86_64 | `Runtime/Plugins/Android/x86_64/` | `.so` |
| iOS | `Runtime/Plugins/iOS/` | `.a` |
| WebGL | `Runtime/Plugins/WebGL/` | `.a` (wasm, static) |
| Linux x86_64 | `Runtime/Plugins/Linux/x86_64/` | `.so` |

iOS and WebGL statically link the native lib into the player, so their P/Invoke
uses `[DllImport("__Internal")]` (see `NativeMethods.cs`).

## Test Commands

```bash
# Run tests with dotnet CLI (requires .NET 8 SDK)
make test
# Or directly:
dotnet test Packages/com.mu5dvlp.qulacs/dotnet~/Mu5dvlp.Qulacs.Tests.csproj
```

## Branch Strategy

- `main` — stable releases only
- `dev` — active development (default branch for PRs)
- Work branches: `{type}/#{issue}_{short-description}` (e.g., `feat/#42_add-ry-gate`)
- Types: `feat`, `fix`, `chore`
- Flow: work branch → PR to `dev` → release merge to `main`
- **Release tags (`vX.Y.Z`) are created ONLY on `main`, and only after the release merge lands.** A tag must always point to a commit reachable from `main` — never tag `dev` or a work branch, and never tag before merging to `main`. Consumers pin tags (`...git#vX.Y.Z`), so an off-`main` tag points outside the released line. Merge `dev → main` with a merge commit (no rewrite) so the released commit stays reachable, then tag the `main` commit. See CONTRIBUTING.md → Release flow.

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
