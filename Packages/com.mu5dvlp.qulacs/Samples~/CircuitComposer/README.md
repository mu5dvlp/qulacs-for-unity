# Circuit Composer

Interactive 2D quantum circuit composer built with Unity UI (uGUI).

## Setup

1. Create a new Scene
2. Add an empty GameObject
3. Attach the `CircuitComposer` component
4. Press Play

All UI is generated at runtime — no prefabs or additional setup required.

## Usage

- **Toolbar**: Select a gate (H, X, Y, Z, S, T, CNOT, SWAP, M) or the eraser tool
- **Grid**: Click a cell to place the selected gate on the qubit wire
- **Two-qubit gates (CNOT/SWAP)**: Click once for control, click another qubit in the same step for target
- **+Q / -Q**: Add or remove qubits (max 8)
- **+S / -S**: Add or remove time steps (max 16)
- **CLR**: Clear the entire circuit

Results update in real-time, showing basis-state probabilities and per-qubit P(|1>) values.
