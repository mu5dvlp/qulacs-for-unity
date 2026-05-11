# com.mu5dvlp.qulacs

[![CI](https://github.com/mu5dvlp/qulacs-for-unity/actions/workflows/test.yml/badge.svg)](https://github.com/mu5dvlp/qulacs-for-unity/actions/workflows/test.yml)
[![Release](https://img.shields.io/github/v/release/mu5dvlp/qulacs-for-unity)](https://github.com/mu5dvlp/qulacs-for-unity/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/mu5dvlp/qulacs-for-unity/blob/main/Packages/com.mu5dvlp.qulacs/LICENSE.md)

Unity native plugin package that wraps **Qulacs** — a high-performance C++ quantum circuit simulator.

## Requirements

- **Unity 6000.0 or later** (developed and tested on Unity 6000.4.1f1 LTS)
  - Unity 2022.3 LTS: confirmed working via UPM Git URL install
  - Unity 2021.x and earlier: untested, may work but not officially supported
- Windows x86_64, Android ARM64, Android x86_64 (macOS / iOS planned)

## Installation

### Via Unity Package Manager (Git URL)

In Unity Editor, open **Window → Package Manager → + → Add package from git URL…** and paste:

```
https://github.com/mu5dvlp/qulacs-for-unity.git?path=/Packages/com.mu5dvlp.qulacs#v0.1.0
```

Or edit `Packages/manifest.json` directly:

```json
{
  "dependencies": {
    "com.mu5dvlp.qulacs": "https://github.com/mu5dvlp/qulacs-for-unity.git?path=/Packages/com.mu5dvlp.qulacs#v0.1.0"
  }
}
```

Replace `v0.1.0` with the desired tag, or omit `#v0.1.0` to track `main`.

> Requires `git` to be available on the PATH — Unity invokes it internally to clone the repository.

## Quick Start

```csharp
using Mu5dvlp.Qulacs;

// Create a 2-qubit Bell state
using var state = new QuantumState(2);
using var circuit = new QuantumCircuit(2);

circuit.H(0).CNOT(0, 1);
circuit.UpdateQuantumState(state);

// Inspect the state vector
var vector = state.GetStateVector();   // Complex[4]

// Sample 1024 times
var samples = state.Sampling(1024);    // ulong[1024]
```

## API Reference

### QuantumState

```csharp
new QuantumState(int qubitCount)   // IDisposable
```

| Member | Description |
|---|---|
| `QubitCount` | Number of qubits |
| `Dimension` | 2^QubitCount |
| `SetZeroState()` | Reset to \|0…0⟩ |
| `SetComputationalBasis(ulong)` | Set to a specific basis state |
| `SetHaarRandomState()` | Set to a Haar-random state |
| `SetHaarRandomState(uint seed)` | Reproducible Haar-random state |
| `SetStateVector(Complex[])` | Load an arbitrary state vector (length must equal Dimension) |
| `GetStateVector()` → `Complex[]` | Full state vector (length = Dimension) |
| `GetZeroProbability(int qubit)` | P(qubit = \|0⟩) |
| `GetSquaredNorm()` | Squared norm (≈ 1 for normalised states) |
| `GetEntropy()` | Von Neumann entropy of the state |
| `Sampling(int count)` → `ulong[]` | Measure `count` times |
| `Sampling(int count, uint seed)` → `ulong[]` | Reproducible sampling |

### QuantumCircuit

```csharp
new QuantumCircuit(int qubitCount)  // IDisposable, fluent builder
```

All gate methods return `this` to allow method chaining.

| Member | Description |
|---|---|
| `QubitCount` | Number of qubits |
| `GateCount` | Number of gates currently in the circuit |
| `UpdateQuantumState(QuantumState)` | Apply the circuit to a state |

**Single-qubit gates:** `H`, `X`, `Y`, `Z`, `S`, `Sdag`, `T`, `Tdag`, `Identity`, `SqrtX`, `SqrtXdag`, `SqrtY`, `SqrtYdag`, `P0`, `P1`

**Rotation gates:** `RX(qubit, angle)`, `RY(qubit, angle)`, `RZ(qubit, angle)`

> **Convention:** Qulacs defines R{X,Y,Z}(θ) = exp(+iθP/2), which is the **opposite sign** from the standard physics convention exp(−iθP/2).

**General single-qubit unitaries:** `U1(qubit, lambda)`, `U2(qubit, phi, lambda)`, `U3(qubit, theta, phi, lambda)`

**Two-qubit gates:** `CNOT(control, target)`, `CZ(control, target)`, `SWAP(qubit0, qubit1)`

**Measurement:** `Measure(qubit, registerAddress = 0)`

**Circuit inspection:** `CalculateDepth()`, `IsClifford()`, `IsGaussian()`

**Circuit mutation:** `RemoveGate(index)`, `MoveGate(fromIndex, toIndex)`

See [`docs/api-reference.md`](docs/api-reference.md) for the full reference.

## Samples

A **Demo Scenes** sample is bundled with the package — Bell state console output, Bloch-sphere visualiser, inspector-driven quantum circuit, and qubit colouring. Import via **Window → Package Manager → Qulacs → Samples → Demo Scenes → Import**. Files land in `Assets/Samples/Qulacs/<version>/Demo Scenes/`.

See [`Samples~/DemoScenes/README.md`](Samples~/DemoScenes/README.md) for what each scene and MonoBehaviour does.

## Building the Native DLL

Prebuilt binaries are included for Windows x86_64 (`.dll`) and Android ARM64/x86_64 (`.so`). To rebuild from source:

```bash
cd Packages/com.mu5dvlp.qulacs

make build                # Windows x86_64 full build: fetch-qulacs → build-qulacs → build-dll → deploy-dll
make build-dll            # wrapper only (Qulacs already built)
make deploy-dll           # copy DLL to Runtime/Plugins only
make build-android        # Android ARM64 full build
make build-android-x86_64 # Android x86_64 full build (emulator)
make build-android-all    # Android ARM64 + x86_64
```

Requires CMake (bundled with Visual Studio 2022) and a C++17 compiler.

## Architecture

```
Unity C# (Mu5dvlp.Qulacs)
    └── P/Invoke
        └── qulacs_unity.dll  (extern "C" C++ wrapper)
            └── Qulacs C++ library
```

## Troubleshooting

### DllNotFoundException (all platforms)

The native plugin binary is missing or not recognized by Unity.

- Verify the `.meta` file for the plugin has the correct platformData settings (CPU, OS).
- Ensure the plugin file is in the correct `Runtime/Plugins/<Platform>/` folder.
- After renaming or moving plugin files, delete the old `.meta` and let Unity reimport, then verify the platform settings in the Inspector.

### DllNotFoundException on Android (`dlopen failed`)

- **`library "qulacs_unity" not found`**: Android requires the `lib` prefix. The `.so` must be named `libqulacs_unity.so`, not `qulacs_unity.so`.
- **`libomp.so` not found**: The plugin was built with dynamic OpenMP linking. Rebuild with `-static-openmp` (see `native~/CMakeLists.txt`). The prebuilt binaries in this package already include this fix.
- Use `readelf -d libqulacs_unity.so` to check runtime dependencies. Only `libm.so`, `libdl.so`, and `libc.so` should appear.

### Build failures (native plugin)

- **CMake not found**: Install CMake 3.20+ or use the one bundled with Visual Studio 2022.
- **Qulacs source missing**: Run `make fetch-qulacs` first, or `make build` which includes the fetch step.
- **Android `find_path` fails**: The Makefile uses `$(CURDIR)` for absolute paths. If building CMake manually, pass an absolute path for `-DQULACS_ROOT`.

## Contact

- Personal: mu5dvlp@gmail.com
- Work: dvlpwork@gmail.com
- X: [@Yugo_dvlp](https://x.com/Yugo_dvlp)
- Qiita: [@mu5dvlp](https://qiita.com/mu5dvlp)
- Zenn: [@mu5dvlp](https://zenn.dev/mu5dvlp)

## License

See `LICENSE` for details. Qulacs is licensed under the MIT License.
