# Demo Scenes

Sample scenes and MonoBehaviours showing how to drive `com.mu5dvlp.qulacs` from the Unity Editor.

> Imported via Package Manager → *Qulacs* → *Samples* → *Demo Scenes* → **Import**.
> Files land in `Assets/Samples/Qulacs/<version>/Demo Scenes/`.

## Scenes

| Scene | Demonstrates |
|---|---|
| `Scenes/BellState.unity` | Bell state `H(0)` → `CNOT(0,1)` — prints state vector / sampling histogram / per-qubit P(\|0⟩) to the Console (and an optional `TMP_Text`) |
| `Scenes/BlochSphere.unity` | Single-qubit state visualised as a Bloch-sphere arrow (transparent shell tinted by phase × P(\|1⟩)) |
| `Scenes/QuantumCircuitObject.unity` | Inspector-built circuit: list of `(GateType, controlIndex, targetIndex)` runs once on Start, marginal probabilities pushed back to each `QubitObject` |
| `Scenes/QuantumStateColor.unity` | `QubitObject`s coloured by P(\|1⟩) (saturation) and β phase (hue) |

## Components

### `QubitObject`

Thin MonoBehaviour wrapper around a 1-qubit `QuantumState`. The state is created in `OnEnable` and disposed in `OnDisable`.

| Field | Default | Description |
|---|---|---|
| `useColor` | false | When true, the attached `Renderer` is tinted via HSV: hue = phase of β, saturation = P(\|1⟩), value = 1 |

To set the state from the inspector, attach a `QuantumStateSetter` and press its **Apply** button in Play mode.

### `QuantumStateSetter`

Writes a 1-qubit state into a `QubitObject`. Two input modes:

| Mode | Fields | Formula |
|---|---|---|
| `Bloch` | `thetaDeg ∈ [0, 180]`, `phiDeg ∈ [0, 360]` | α = cos(θ/2), β = sin(θ/2)·e^{iφ} |
| `Components` | `alpha`, `betaReal`, `betaImag` | α = alpha + 0i, β = betaReal + i·betaImag |

The custom inspector hides irrelevant fields and exposes an **Apply** button (active in Play mode).

### `QuantumCircuitObject`

Inspector-configurable circuit. Set `qubitCount` and add entries to `gates`. Each `GateEntry`:

| Field | Description |
|---|---|
| `gateType` | One of `X`, `Y`, `Z`, `H`, `CNOT` |
| `controlIndex` | Control qubit (shown only for `CNOT`) |
| `targetIndex` | Target qubit |

On `Start`, instantiates a `QubitObject` per qubit and a `QuantumGateObject` per gate, runs the circuit once, and copies each qubit's marginal P(\|0⟩) back to its `QubitObject`. Two-qubit gates render a vertical wire between the control and target via `QuantumGateObject.SetwireHeight`.

### `BlochSphereObject`

Visualises a referenced `QubitObject`. Each frame: θ = 2·acos(√P(\|0⟩)), φ = arg β, then points an `arrow` Transform along that direction; a transparent sphere shell is tinted by phase × P(\|1⟩).

| Field | Default | Description |
|---|---|---|
| `qubitObject` | — | Source `QubitObject` |
| `sphereRenderer` | — | Renderer of the transparent sphere shell |
| `arrow` | — | Transform whose `up` is set to the Bloch vector |
| `shaderType` | `URP` | URP vs. Built-in transparency setup |

Bloch → Unity axis mapping: Bloch X → Unity X, Bloch Y → Unity Z, Bloch Z → Unity Y.

### `BellStateDemo`

Builds the two-qubit Bell state `(|00⟩ + |11⟩) / √2` (`H(0)` → `CNOT(0,1)`) and prints to the Console:

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

| Field | Default | Description |
|---|---|---|
| `samplingCount` | 1024 | Number of measurement shots |
| `resultText` | *(none)* | Optional `TMP_Text` to mirror the output on screen |

### Editor helpers

| Script | Role |
|---|---|
| `Scripts/Editor/GateEntryDrawer.cs` | Property drawer for `GateEntry` — hides `controlIndex` for single-qubit gates |
| `Scripts/Editor/QuantumStateSetterEditor.cs` | Mode-aware inspector for `QuantumStateSetter` with an **Apply** button |
