# API Reference — com.mu5dvlp.qulacs

Namespace: `Mu5dvlp.Qulacs`

---

## QuantumState

`new QuantumState(int qubitCount)` — `IDisposable`

State vector of `2^qubitCount` complex amplitudes. Initialized to |0…0⟩.

| Method | Description |
|---|---|
| `SetZeroState()` | Reset to \|0…0⟩ |
| `SetComputationalBasis(ulong index)` | Set to a specific basis state |
| `SetHaarRandomState()` | Haar-random state |
| `SetHaarRandomState(uint seed)` | Haar-random with fixed seed |
| `SetStateVector(Complex[] v)` | Load arbitrary state vector (length must be `2^qubitCount`) |
| `GetStateVector()` → `Complex[]` | Read current state vector |
| `GetZeroProbability(int qubit)` → `double` | Probability of measuring \|0⟩ on a qubit |
| `GetEntropy()` → `double` | Von Neumann entropy of the state |
| `GetSquaredNorm()` → `double` | Squared norm (≈ 1 for normalized states) |
| `Sampling(int count)` → `ulong[]` | Sample measurement outcomes |
| `Sampling(int count, uint seed)` → `ulong[]` | Sample with fixed seed |

Properties: `QubitCount`, `Dimension` (= `2^QubitCount`)

---

## QuantumCircuit

`new QuantumCircuit(int qubitCount)` — `IDisposable`, fluent builder

All gate methods return `this` for chaining.

| Method | Description |
|---|---|
| `UpdateQuantumState(QuantumState state)` | Apply all gates to the given state in order |

### Single-qubit gates

`H(int qubit)`, `X(int qubit)`, `Y(int qubit)`, `Z(int qubit)`
`S(int qubit)`, `Sdag(int qubit)`, `T(int qubit)`, `Tdag(int qubit)`
`Identity(int qubit)`
`SqrtX(int qubit)`, `SqrtXdag(int qubit)`, `SqrtY(int qubit)`, `SqrtYdag(int qubit)`
`P0(int qubit)`, `P1(int qubit)` — projection onto \|0⟩ / \|1⟩

### Rotation gates

`RX(int qubit, double angle)`, `RY(int qubit, double angle)`, `RZ(int qubit, double angle)`

> **Convention**: `R{X,Y,Z}(θ) = exp(+iθP/2)` — opposite sign from the standard physics convention `exp(−iθP/2)`.

### General single-qubit unitaries

`U1(int qubit, double lambda)`
`U2(int qubit, double phi, double lambda)`
`U3(int qubit, double theta, double phi, double lambda)`

### Two-qubit gates

`CNOT(int control, int target)`, `CZ(int control, int target)`, `SWAP(int qubit0, int qubit1)`

### Measurement

`Measure(int qubit, int registerAddress = 0)`

### Circuit inspection

| Method | Description |
|---|---|
| `CalculateDepth()` → `int` | Critical-path length of the circuit |
| `IsClifford()` → `bool` | True if all gates are Clifford |
| `IsGaussian()` → `bool` | True if all gates are Gaussian |

### Circuit mutation

| Method | Description |
|---|---|
| `RemoveGate(int index)` | Remove the gate at the given index |
| `MoveGate(int fromIndex, int toIndex)` | Move a gate to a new position |

Properties: `QubitCount`, `GateCount`
