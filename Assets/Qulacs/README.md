# Assets/Qulacs — Samples

Sample scenes and scripts demonstrating how to use the `com.mu5dvlp.qulacs` package.

## Contents

| Path | Description |
|---|---|
| `Scenes/BellState.unity` | Bell-state demo (`H(0)` → `CNOT(0,1)`) — prints state vector / sampling / probabilities |
| `Scenes/BlochSphere.unity` | Single-qubit state visualised as a Bloch-sphere arrow |
| `Scenes/QuantumCircuitObject.unity` | Inspector-driven circuit built from a list of gates |
| `Scenes/QuauntumStateColor.unity` | `QubitObject`s coloured by P(\|1⟩) and phase |
| `Scripts/Runtime/BellStateDemo.cs` | MonoBehaviour for the Bell-state demo |
| `Scripts/Runtime/QubitObject.cs` | Holds a 1-qubit `QuantumState`; optionally colours its `Renderer` |
| `Scripts/Runtime/QuantumGateObject.cs` | Holds a `GateType` (X/Y/Z/H/CNOT); colours its `Renderer` accordingly |
| `Scripts/Runtime/QuantumCircuitObject.cs` | Spawns qubits/gates from a `List<GateEntry>` and runs the circuit |
| `Scripts/Runtime/BlochSphereObject.cs` | Renders a `QubitObject`'s state as a Bloch-sphere arrow |
| `Scripts/Runtime/QuantumStateSetter.cs` | Sets a `QubitObject`'s state from Bloch angles or raw α/β |
| `Scripts/Editor/GateEntryDrawer.cs` | Property drawer for `GateEntry` (hides `controlIndex` for single-qubit gates) |
| `Scripts/Editor/QuantumStateSetterEditor.cs` | Mode-aware inspector for `QuantumStateSetter` with an Apply button |

## QubitObject

`QubitObject` is a thin MonoBehaviour wrapper around a single-qubit `QuantumState`. The state is created in `OnEnable` and disposed in `OnDisable`.

| Field | Default | Description |
|---|---|---|
| `useColor` | false | When true, the attached `Renderer` is tinted via HSV: hue = phase of β, saturation = P(\|1⟩), value = 1 |

To assign a state from the inspector, attach a `QuantumStateSetter` (see below) and press its **Apply** button in Play mode.

## QuantumStateSetter

Writes a 1-qubit state vector into a `QubitObject`. Two input modes:

| Mode | Fields | Formula |
|---|---|---|
| `Bloch` | `thetaDeg ∈ [0, 180]`, `phiDeg ∈ [0, 360]` | α = cos(θ/2), β = sin(θ/2)·e^{iφ} |
| `Components` | `alpha`, `betaReal`, `betaImag` | α = alpha + 0i, β = betaReal + i·betaImag |

The custom inspector shows only the fields relevant to the active mode and exposes an **Apply** button (enabled in Play mode).

## QuantumCircuitObject

Inspector-configurable circuit. Set `qubitCount` and add entries to the `gates` list. Each `GateEntry` has:

| Field | Description |
|---|---|
| `gateType` | One of `X`, `Y`, `Z`, `H`, `CNOT` |
| `controlIndex` | Control qubit (shown only for `CNOT`) |
| `targetIndex` | Target qubit |

On `Start`, `QuantumCircuitObject` instantiates a `QubitObject` per qubit and a `QuantumGateObject` per gate, then runs the circuit once and copies each qubit's marginal P(\|0⟩) back to its `QubitObject`. Two-qubit gates render a vertical wire between the control and target via `QuantumGateObject.SetwireHeight`.

## BlochSphereObject

Visualises a referenced `QubitObject`. Each frame it converts the state to Bloch coordinates (θ = 2·acos(√P(\|0⟩)), φ = arg β) and points an `arrow` Transform along that direction; a transparent sphere shell is tinted by phase × P(\|1⟩).

| Field | Default | Description |
|---|---|---|
| `qubitObject` | — | Source `QubitObject` |
| `sphereRenderer` | — | Renderer of the transparent sphere shell |
| `arrow` | — | Transform whose `up` is set to the Bloch vector |
| `shaderType` | `URP` | Selects URP vs. Built-in transparency setup |

Bloch → Unity axis mapping: Bloch X → Unity X, Bloch Y → Unity Z, Bloch Z → Unity Y.

## BellState Demo

`BellStateDemo` creates the two-qubit Bell state (|00⟩ + |11⟩) / √2 and prints the result.

**Circuit:** `H(0)` → `CNOT(0, 1)`

### Running the demo

1. Open `Scenes/BellState.unity`.
2. Press **Play**.
3. Check the Console for output similar to:

```
=== Bell State ===
Qubit count : 2
Squared norm: 1.000000

State vector:
  |00> : (+0.707, +0.000i)
  |01> : (+0.000, +0.000i)
  |10> : (+0.000, +0.000i)
  |11> : (+0.707, +0.000i)

Sampling (1024 shots):
  |00> :   512 (50.0%)
  |11> :   512 (50.0%)

Zero probability:
  qubit[0] = 0.5000
  qubit[1] = 0.5000
```

### Inspector options

| Field | Default | Description |
|---|---|---|
| `samplingCount` | 1024 | Number of measurement shots |
| `resultText` | *(none)* | Optional `TMP_Text` to display output on screen |
