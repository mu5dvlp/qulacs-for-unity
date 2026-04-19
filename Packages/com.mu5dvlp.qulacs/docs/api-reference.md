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

### Rotation gates

`RX(int qubit, double angle)`, `RY(int qubit, double angle)`, `RZ(int qubit, double angle)`

> **Convention**: `R{X,Y,Z}(θ) = exp(+iθP/2)` — opposite sign from the standard physics convention `exp(−iθP/2)`.

### Two-qubit gates

`CNOT(int control, int target)`, `CZ(int control, int target)`, `SWAP(int qubit0, int qubit1)`

### Measurement

`Measure(int qubit, int registerAddress = 0)`

Properties: `QubitCount`, `GateCount`
