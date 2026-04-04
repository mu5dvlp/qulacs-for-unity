# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**quri-toolkit** is a Unity 6 (6000.4.1f1 LTS) game project using the Universal Render Pipeline (URP). Target platform is Windows 64-bit Standalone.

## Build & Run Commands

Unity projects are built through the Unity Editor GUI or via CLI:

```bash
# Open project in Unity Editor (typical workflow)
# Launch Unity Hub and open the project at this directory

# Command-line build (Windows player)
"C:\Program Files\Unity\Hub\Editor\6000.4.1f1\Editor\Unity.exe" -projectPath . -buildWindowsPlayer Build/quri-toolkit.exe -quit -batchmode -logFile build.log

# Run EditMode tests (requires Unity Test Framework)
"C:\Program Files\Unity\Hub\Editor\6000.4.1f1\Editor\Unity.exe" -projectPath . -runTests -testPlatform editmode -quit -batchmode -logFile test.log

# Run PlayMode tests
"C:\Program Files\Unity\Hub\Editor\6000.4.1f1\Editor\Unity.exe" -projectPath . -runTests -testPlatform playmode -quit -batchmode -logFile test.log
```

VS Code is configured with the **Visual Studio Tools for Unity (VSTUC)** extension. To debug, attach to the Unity Editor via the "Unity Editor" launch config in `.vscode/launch.json`.

## Architecture

### Rendering Pipeline
Two URP renderer configurations exist in `Assets/Settings/`:
- **PC_Renderer.asset** — high-quality renderer for desktop
- **Mobile_Renderer.asset** — optimized renderer for mobile targets

Volume profiles (`DefaultVolumeProfile.asset`, `SampleSceneProfile.asset`) control post-processing per-scene.

### Input System
Uses the **new Input System** (v1.19.0), not the legacy InputManager. All input bindings are defined in `Assets/InputSystem_Actions.inputactions`. When adding new actions, edit this file in the Unity Editor's Input Actions window.

### AI Navigation
**AI Navigation 2.0.11** (NavMesh) is integrated. NavMesh areas are configured in `ProjectSettings/NavMeshAreas.asset`.

### Key Packages (from `Packages/manifest.json`)
| Package | Version | Purpose |
|---|---|---|
| com.unity.render-pipelines.universal | 17.4.0 | URP rendering |
| com.unity.inputsystem | 1.19.0 | Input handling |
| com.unity.ai.navigation | 2.0.11 | NavMesh pathfinding |
| com.unity.burst | 1.8.28 | SIMD-optimized C# compilation |
| com.unity.mathematics | 1.3.3 | Math utilities for Burst jobs |
| com.unity.collections | 6.4.0 | High-performance collections |
| com.unity.test-framework | 1.6.0 | NUnit-based unit testing |
| com.unity.timeline | 1.8.11 | Cinematic sequencing |
| com.unity.ugui | 2.0.0 | UI system |

## C# Scripts

Scripts live in `Assets/`. Unity compiles:
- `Assembly-CSharp.csproj` — runtime scripts
- `Assembly-CSharp-Editor.csproj` — Editor-only scripts (must be in `Editor/` folders)

Editor scripts (custom inspectors, tools, wizard windows) must reside in a folder named `Editor/` to avoid being included in player builds.

## VS Code Setup

The `.vscode/settings.json` excludes most Unity-generated binary/meta files from the file explorer to reduce noise. The `.slnx` solution file (`quri-toolkit.slnx`) is the workspace solution — use this with the VSTUC extension for IntelliSense.
