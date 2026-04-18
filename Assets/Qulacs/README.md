# Assets/Qulacs — Samples

This folder contains sample scenes and scripts that demonstrate how to use the `com.mu5dvlp.qulacs` package.

## Contents

| Path | Description |
|---|---|
| `Scenes/BellState.unity` | Demo scene for the Bell state example |
| `Scripts/BellStateDemo.cs` | MonoBehaviour that creates and measures a Bell state |

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
