# CLAUDE.md — com.mu5dvlp.qulacs

Unity native plugin package wrapping Qulacs (C++ quantum circuit simulator).

## Key facts

- C# namespace: `Mu5dvlp.Qulacs`
- Native library name: `qulacs_unity` (loaded via P/Invoke)
- **Rotation gate convention**: `R{X,Y,Z}(θ) = exp(+iθP/2)` — opposite sign from the standard `exp(−iθP/2)`

## Structure

```
Makefile                      # build targets (run from this directory)
Runtime/
  Internal/NativeMethods.cs   # P/Invoke declarations (internal)
  QuantumState.cs             # public API
  QuantumCircuit.cs           # public API
  Plugins/Windows/x86_64/     # qulacs_unity.dll
docs/
  api-reference.md            # full API reference
native~/
  src/qulacs_unity.h          # C API
  src/qulacs_unity.cpp        # extern "C" wrapper
  CMakeLists.txt
  build.sh                    # invoked by Makefile
Tests/
  QuantumStateTests.cs
  GateTests.cs
  QuantumCircuitTests.cs
Samples~/
  DemoScenes/                 # bundled sample (Package Manager → Import)
    Scenes/                   # BellState, BlochSphere, QuantumCircuitObject, QuantumStateColor
    Scripts/Runtime/          # QubitObject, QuantumGateObject, QuantumCircuitObject, ...
    Scripts/Editor/           # GateEntryDrawer, QuantumStateSetterEditor
    README.md
  CircuitComposer/            # 2D interactive circuit composer (Package Manager → Import)
    Scripts/Runtime/          # CircuitComposer, CircuitComposerUI
    README.md
```

> `Samples~/` is hidden from Unity (the trailing `~`). To edit the sample, import it via Package Manager → Samples → Demo Scenes → Import; the working copy lands in `Assets/Samples/Qulacs/<version>/Demo Scenes/`. Sync any changes back to `Samples~/DemoScenes/` before committing.

## Sample MonoBehaviours (Samples~/DemoScenes/Scripts/)

These MonoBehaviours consume this package:

| Script | Role |
|---|---|
| `QubitObject` | Holds a 1-qubit `QuantumState`; optionally colors its `Renderer` via HSV (hue = β phase, saturation = P(\|1⟩)) |
| `QuantumGateObject` | Holds a `GateType` (X/Y/Z/H/CNOT); colors its `Renderer` by gate type |
| `QuantumCircuitObject` | Spawns `QubitObject`s and `QuantumGateObject`s from `List<GateEntry>` (gate + control/target indices), runs the circuit, and pushes per-qubit marginal probabilities back to each `QubitObject` |
| `BlochSphereObject` | Visualises a `QubitObject`'s state as a Bloch-sphere arrow (with transparent shell coloured by phase × P(\|1⟩)) |
| `QuantumStateSetter` | Inspector helper that writes a 1-qubit state into a `QubitObject` either from Bloch angles (θ, φ) or from raw α/β components |
| `BellStateDemo` | Builds H(0)·CNOT(0,1), prints the state vector / sampling histogram / per-qubit P(\|0⟩) to the Console (and optional `TMP_Text`) |

Editor helpers (`Samples~/DemoScenes/Scripts/Editor/`): `GateEntryDrawer` (custom drawer for `GateEntry`) and `QuantumStateSetterEditor` (mode-aware inspector with an "Apply" button).

## Circuit Composer (Samples~/CircuitComposer/Scripts/)

Interactive 2D quantum circuit composer built with uGUI. All UI is generated at runtime — attach `CircuitComposer` to an empty GameObject and press Play.

| Script | Role |
|---|---|
| `CircuitComposer` | Data model, gate placement logic, circuit execution via `QuantumCircuit`/`QuantumState`. Supports H/X/Y/Z/S/T/CNOT/SWAP/Measure |
| `CircuitComposerUI` | Builds Canvas UI programmatically: toolbar, scrollable grid, connection lines for 2-qubit gates, real-time probability bar chart |

## API reference

See [`docs/api-reference.md`](docs/api-reference.md).

## Building the native library

```bash
cd Packages/com.mu5dvlp.qulacs
make build                # Windows x86_64 full build: fetch-qulacs → build-qulacs → build-dll → deploy-dll
make build-dll            # wrapper only (Qulacs already built)
make deploy-dll           # copy DLL only
make build-android        # Android ARM64 full build
make build-android-x86_64 # Android x86_64 full build (emulator)
make build-android-all    # Android ARM64 + x86_64
```

cmake: `C:/Program Files/Microsoft Visual Studio/2022/Community/Common7/IDE/CommonExtensions/Microsoft/CMake/CMake/bin/cmake.exe`

- Source/deps: `native~/extern/` (gitignored)
- Built libs: `native~/build/lib/`
- Install prefix: `native~/extern/qulacs-install/`
