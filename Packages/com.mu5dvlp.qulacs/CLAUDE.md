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
```

## Unity-side scripts (Assets/)

These MonoBehaviours consume this package:

| Script | Role |
|---|---|
| `QbitObject` | Holds a 1-qubit `QuantumState`; colors its `Renderer` via HSV (hue = β phase, saturation = P(\|1⟩)) |
| `QuantumGateObject` | Holds a `GateType` (X/Y/Z/H); colors its `Renderer` by gate type |
| `QuantumCircuitObject` | Holds `List<QbitObject>` and `List<GateEntry>` (gate + qubit index pairs) |

## API reference

See [`docs/api-reference.md`](docs/api-reference.md).

## Building the DLL

```bash
cd Packages/com.mu5dvlp.qulacs
make build          # fetch-qulacs → build-qulacs → build-dll → deploy-dll
make build-dll      # wrapper only (Qulacs already built)
make deploy-dll     # copy DLL only
```

cmake: `C:/Program Files/Microsoft Visual Studio/2022/Community/Common7/IDE/CommonExtensions/Microsoft/CMake/CMake/bin/cmake.exe`

- Source/deps: `native~/extern/` (gitignored)
- Built libs: `native~/build/lib/`
- Install prefix: `native~/extern/qulacs-install/`
