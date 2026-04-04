# CLAUDE.md — com.mu5dvlp.qulacs

Unity native plugin package wrapping Qulacs (C++ quantum circuit simulator).

## Key facts

- C# namespace: `Mu5dvlp.Qulacs`
- Native library name: `qulacs_unity` (loaded via P/Invoke)
- **Qulacs rotation gate convention**: `R{X,Y,Z}(θ) = exp(+iθP/2)` — opposite sign from the standard physics convention `exp(-iθP/2)`. Tests and docs must reflect Qulacs' convention, not the standard one.

## Structure

```
Runtime/
  Internal/NativeMethods.cs   # P/Invoke declarations (internal)
  QuantumState.cs             # public API
  QuantumCircuit.cs           # public API
  Plugins/Windows/x86_64/     # qulacs_unity.dll
native~/
  src/qulacs_unity.h          # C API
  src/qulacs_unity.cpp        # extern "C" wrapper
  CMakeLists.txt
  build.sh                    # full build script
Tests/
  QuantumStateTests.cs
  GateTests.cs
  QuantumCircuitTests.cs
```

## Building the DLL

```bash
cd Packages/com.mu5dvlp.qulacs
make build          # fetch-qulacs → build-qulacs → build-dll → deploy-dll
make build-dll      # wrapper only (Qulacs already built)
make deploy-dll     # copy DLL only
```

cmake is at `C:/Program Files/Microsoft Visual Studio/2022/Community/Common7/IDE/CommonExtensions/Microsoft/CMake/CMake/bin/cmake.exe`

Qulacs and Boost are staged under `native~/extern/` (gitignored).
Built libs are in `native~/build/lib/`.
Install prefix: `native~/extern/qulacs-install/`.
