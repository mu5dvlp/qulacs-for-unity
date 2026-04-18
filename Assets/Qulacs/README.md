# Assets/Qulacs — Samples

This folder contains sample scenes and scripts that demonstrate how to use the `com.mu5dvlp.qulacs` package.

## Contents

| Path | Description |
|---|---|
| `Scenes/BellState.unity` | Demo scene for the Bell state example |
| `Scripts/Runtime/BellStateDemo.cs` | MonoBehaviour that creates and measures a Bell state |
| `Scenes/QbitObject.unity` | Demo scene for the single-qubit inspector |
| `Scripts/Runtime/QbitObject.cs` | MonoBehaviour for inspector-configurable single-qubit state |

## QbitObject

`QbitObject` is an inspector-configurable MonoBehaviour representing a single qubit state |ψ⟩ = α|0⟩ + β|1⟩. Magnitudes are coupled via a single angle θ so that |α|²+|β|²=1 is always satisfied:

- |α| = cos(θ), |β| = sin(θ), where θ ∈ [0°, 90°]
- Sign and phase (×i) can be toggled independently per amplitude

**Inspector fields:**

| Field | Default | Description |
|---|---|---|
| `Theta Deg` | 0 | θ in degrees — 0° = \|0⟩, 90° = \|1⟩, 45° = equal superposition |
| `Alpha Negative` | false | Multiply α by −1 |
| `Alpha Imaginary` | false | Multiply α by i |
| `Beta Negative` | false | Multiply β by −1 |
| `Beta Imaginary` | false | Multiply β by i |

**Read-only properties (updated after `Apply()`):** `Alpha`, `Beta`, `ZeroProbability`, `OneProbability`, `SquaredNorm`

Open `Scenes/QbitObject.unity` and press **Play** to use the demo.

## BellState Demo

`BellStateDemo` creates the two-qubit Bell state (|00⟩ + |11⟩) / √2 and prints measurement results to the Console.

**Circuit:** H(0) → CNOT(0, 1)

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
| `Sampling Count` | 1024 | Number of measurement shots |
| `Result Text` | *(none)* | Optional `TMP_Text` to display output on screen |
