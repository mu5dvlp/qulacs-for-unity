Update tests to match the latest program specification.

## Steps

1. **Inspect the current public API**
   - `Packages/com.mu5dvlp.qulacs/Runtime/QuantumState.cs`
   - `Packages/com.mu5dvlp.qulacs/Runtime/QuantumCircuit.cs`
   - `Packages/com.mu5dvlp.qulacs/Runtime/Internal/NativeMethods.cs`
   - `Packages/com.mu5dvlp.qulacs/docs/api-reference.md` (if present)

2. **Identify gaps against existing tests**
   - `Packages/com.mu5dvlp.qulacs/Tests/QuantumStateTests.cs`
   - `Packages/com.mu5dvlp.qulacs/Tests/GateTests.cs`
   - `Packages/com.mu5dvlp.qulacs/Tests/QuantumCircuitTests.cs`
   - For each public method/property, check whether a corresponding test case exists.

3. **Add missing tests** (in priority order)

   ### To add to QuantumStateTests.cs
   - `SetStateVector` — set an arbitrary vector and verify round-trip via `GetStateVector`
   - `GetEntropy` — entropy of |0⟩ is 0; entropy of an equal-amplitude superposition is positive
   - `Sampling(int count)` (seedless variant) — only validate the result length and value range

   ### To add to GateTests.cs
   - `Identity` — applying it to |0⟩ leaves the state unchanged
   - `SqrtX / SqrtXdag` — applying `SqrtX` twice is equivalent to X
   - `SqrtY / SqrtYdag` — same as above (for Y)
   - `P0 / P1` — applying P0 to |0⟩ keeps the state; applying P1 zeroes the amplitude
   - `CZ` — verify phase kickback in the Bell basis
   - `U1 / U2 / U3` — verify equivalence to known gates (Z, H, X) at specific parameters
   - `Measure` — after measurement, the state collapses (norm becomes a basis state with 0 or 1)

   ### To add to QuantumCircuitTests.cs
   - `CalculateDepth` — depth of a serial gate sequence matches the expected value
   - `IsClifford` — H+CNOT circuit returns true; a circuit containing R(θ) returns false
   - `IsGaussian` — verify true/false for appropriate gate combinations
   - `RemoveGate` — `GateCount` decreases after removal, and the resulting circuit behaves correctly
   - `MoveGate` — gate-order changes are correctly reflected in the state-update result

4. **Test-writing rules**
   - Watch Qulacs's rotation-gate convention: `R{X,Y,Z}(θ) = exp(+iθP/2)` (opposite sign from the standard).
   - Use `const double Eps = 1e-6` as the tolerance.
   - Each test is an independent method annotated with `[Test]`; dispose `QuantumState` / `QuantumCircuit` reliably via `using`.
   - Name tests using the `Target_Condition_ExpectedResult` form.

5. **Post-change verification**
   - Confirm the added/changed tests do not duplicate existing tests.
   - Run the Unity command from CLAUDE.md and confirm all tests pass:
     ```
     "C:\Program Files\Unity\Hub\Editor\6000.4.1f1\Editor\Unity.exe" \
       -projectPath . -runTests -testPlatform editmode -quit -batchmode -logFile test.log
     ```
