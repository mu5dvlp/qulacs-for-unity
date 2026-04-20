using System;
using System.Numerics;
using Mu5dvlp.Qulacs;
using NUnit.Framework;

namespace Mu5dvlp.Qulacs.Tests
{
    /// <summary>
    /// Verifies each gate's effect on the state vector against analytically known values.
    /// Reference: Qulacs test/cppsim/test_gate.cpp (GateTest::ApplySingleQubitGate)
    /// </summary>
    public class GateTests
    {
        private const double Eps = 1e-6;
        private const double Inv_Sqrt2 = 0.70710678118654752;

        // Helper: apply a single-gate circuit to |0> and return the state vector.
        private static Complex[] ApplyToZero(int qubits, System.Action<QuantumCircuit> addGate)
        {
            using var state = new QuantumState(qubits);
            using var circuit = new QuantumCircuit(qubits);
            addGate(circuit);
            circuit.UpdateQuantumState(state);
            return state.GetStateVector();
        }

        // --- Pauli X ---

        [Test]
        public void X_OnZero_FlipsToOne()
        {
            var v = ApplyToZero(1, c => c.X(0));
            // |0> -X-> |1>
            Assert.AreEqual(0.0, Complex.Abs(v[0]), Eps);
            Assert.AreEqual(1.0, Complex.Abs(v[1]), Eps);
        }

        [Test]
        public void X_Twice_RestoresZero()
        {
            using var state = new QuantumState(1);
            using var circuit = new QuantumCircuit(1);
            circuit.X(0).X(0);
            circuit.UpdateQuantumState(state);
            var v = state.GetStateVector();
            Assert.AreEqual(1.0, v[0].Real, Eps);
            Assert.AreEqual(0.0, Complex.Abs(v[1]), Eps);
        }

        // --- Hadamard ---

        [Test]
        public void H_OnZero_CreatesSuperposition()
        {
            var v = ApplyToZero(1, c => c.H(0));
            // |0> -H-> (|0>+|1>)/√2
            Assert.AreEqual(Inv_Sqrt2, v[0].Real, Eps);
            Assert.AreEqual(Inv_Sqrt2, v[1].Real, Eps);
        }

        [Test]
        public void H_Twice_RestoresZero()
        {
            using var state = new QuantumState(1);
            using var circuit = new QuantumCircuit(1);
            circuit.H(0).H(0);
            circuit.UpdateQuantumState(state);
            var v = state.GetStateVector();
            Assert.AreEqual(1.0, v[0].Real, Eps);
            Assert.AreEqual(0.0, Complex.Abs(v[1]), Eps);
        }

        // --- Pauli Z ---

        [Test]
        public void Z_OnPlus_CreatesMinusState()
        {
            // |+> = H|0>, then Z|+> = |->  => amplitudes: 1/√2, -1/√2
            using var state = new QuantumState(1);
            using var circuit = new QuantumCircuit(1);
            circuit.H(0).Z(0);
            circuit.UpdateQuantumState(state);
            var v = state.GetStateVector();
            Assert.AreEqual( Inv_Sqrt2, v[0].Real, Eps);
            Assert.AreEqual(-Inv_Sqrt2, v[1].Real, Eps);
        }

        // --- Pauli Y ---

        [Test]
        public void Y_OnZero_GivesITimesOne()
        {
            // Y|0> = i|1>
            var v = ApplyToZero(1, c => c.Y(0));
            Assert.AreEqual(0.0, Complex.Abs(v[0]), Eps);
            Assert.AreEqual(0.0, v[1].Real, Eps);
            Assert.AreEqual(1.0, v[1].Imaginary, Eps);
        }

        // --- S and T gates ---

        [Test]
        public void S_OnPlus_AppliesPhase()
        {
            // S|+> : |0> coeff stays 1/√2, |1> coeff gets factor i
            using var state = new QuantumState(1);
            using var circuit = new QuantumCircuit(1);
            circuit.H(0).S(0);
            circuit.UpdateQuantumState(state);
            var v = state.GetStateVector();
            Assert.AreEqual(Inv_Sqrt2, v[0].Real, Eps);
            Assert.AreEqual(0.0,       v[1].Real, Eps);
            Assert.AreEqual(Inv_Sqrt2, v[1].Imaginary, Eps);
        }

        [Test]
        public void Sdag_AfterS_RestoresState()
        {
            using var state = new QuantumState(1);
            state.SetHaarRandomState(seed: 7);
            var before = state.GetStateVector();

            using var circuit = new QuantumCircuit(1);
            circuit.S(0).Sdag(0);
            circuit.UpdateQuantumState(state);
            var after = state.GetStateVector();

            for (int i = 0; i < after.Length; i++)
                Assert.AreEqual(0.0, Complex.Abs(after[i] - before[i]), Eps);
        }

        [Test]
        public void Tdag_AfterT_RestoresState()
        {
            using var state = new QuantumState(1);
            state.SetHaarRandomState(seed: 99);
            var before = state.GetStateVector();

            using var circuit = new QuantumCircuit(1);
            circuit.T(0).Tdag(0);
            circuit.UpdateQuantumState(state);
            var after = state.GetStateVector();

            for (int i = 0; i < after.Length; i++)
                Assert.AreEqual(0.0, Complex.Abs(after[i] - before[i]), Eps);
        }

        // --- Rotation gates ---

        [Test]
        public void RX_PiRotation_FlipsZeroToOne()
        {
            // NOTE: Qulacs defines RX(θ) = exp(+iθX/2), the opposite sign from the
            // standard physics convention exp(-iθX/2).
            // RX(π)|0> = +i|1>  (Qulacs convention)
            var v = ApplyToZero(1, c => c.RX(0, Math.PI));
            Assert.AreEqual(0.0, Complex.Abs(v[0]), Eps);
            Assert.AreEqual(0.0, v[1].Real, Eps);
            Assert.AreEqual(1.0, v[1].Imaginary, Eps);
        }

        [Test]
        public void RY_PiRotation_FlipsZeroToOne()
        {
            // NOTE: Qulacs defines RY(θ) = exp(+iθY/2), the opposite sign from the
            // standard physics convention exp(-iθY/2).
            // RY(π)|0> = -|1>  (Qulacs convention)
            var v = ApplyToZero(1, c => c.RY(0, Math.PI));
            Assert.AreEqual(0.0,  Complex.Abs(v[0]), Eps);
            Assert.AreEqual(-1.0, v[1].Real, Eps);
        }

        [Test]
        public void RZ_DoesNotChangeZeroStateProbabilities()
        {
            // RZ(θ)|0> only changes phase, P(|0>) stays 1
            using var state = new QuantumState(1);
            using var circuit = new QuantumCircuit(1);
            circuit.RZ(0, 1.234);
            circuit.UpdateQuantumState(state);
            Assert.AreEqual(1.0, state.GetZeroProbability(0), Eps);
        }

        // --- CNOT ---

        [Test]
        public void CNOT_ControlZero_DoesNotFlipTarget()
        {
            // |00> -CNOT(0,1)-> |00>
            var v = ApplyToZero(2, c => c.CNOT(0, 1));
            Assert.AreEqual(1.0, v[0].Real, Eps);
            for (int i = 1; i < v.Length; i++)
                Assert.AreEqual(0.0, Complex.Abs(v[i]), Eps);
        }

        [Test]
        public void CNOT_ControlOne_FlipsTarget()
        {
            // X(0) → |10>, then CNOT(0,1) → |11>
            var v = ApplyToZero(2, c => c.X(0).CNOT(0, 1));
            // index 3 = |11> in little-endian (qubit0=1, qubit1=1)
            Assert.AreEqual(1.0, v[3].Real, Eps);
            for (int i = 0; i < v.Length; i++)
                if (i != 3) Assert.AreEqual(0.0, Complex.Abs(v[i]), Eps);
        }

        // --- Identity ---

        [Test]
        public void Identity_OnZero_PreservesState()
        {
            var v = ApplyToZero(1, c => c.Identity(0));
            Assert.AreEqual(1.0, v[0].Real, Eps);
            Assert.AreEqual(0.0, Complex.Abs(v[1]), Eps);
        }

        // --- SqrtX / SqrtXdag ---

        [Test]
        public void SqrtX_AppliedTwice_EqualsX()
        {
            // (SqrtX)^2 = X, so two applications on |0> give |1>
            var v = ApplyToZero(1, c => c.SqrtX(0).SqrtX(0));
            Assert.AreEqual(0.0, Complex.Abs(v[0]), Eps);
            Assert.AreEqual(1.0, Complex.Abs(v[1]), Eps);
        }

        [Test]
        public void SqrtXdag_AfterSqrtX_RestoresState()
        {
            using var state = new QuantumState(1);
            state.SetHaarRandomState(seed: 42);
            var before = state.GetStateVector();

            using var circuit = new QuantumCircuit(1);
            circuit.SqrtX(0).SqrtXdag(0);
            circuit.UpdateQuantumState(state);
            var after = state.GetStateVector();

            for (int i = 0; i < after.Length; i++)
                Assert.AreEqual(0.0, Complex.Abs(after[i] - before[i]), Eps);
        }

        // --- SqrtY / SqrtYdag ---

        [Test]
        public void SqrtY_AppliedTwice_EqualsY()
        {
            // (SqrtY)^2 = Y, so two applications on |0> give Y|0> = i|1>
            var v = ApplyToZero(1, c => c.SqrtY(0).SqrtY(0));
            Assert.AreEqual(0.0, Complex.Abs(v[0]), Eps);
            Assert.AreEqual(0.0, v[1].Real,      Eps);
            Assert.AreEqual(1.0, v[1].Imaginary, Eps);
        }

        [Test]
        public void SqrtYdag_AfterSqrtY_RestoresState()
        {
            using var state = new QuantumState(1);
            state.SetHaarRandomState(seed: 42);
            var before = state.GetStateVector();

            using var circuit = new QuantumCircuit(1);
            circuit.SqrtY(0).SqrtYdag(0);
            circuit.UpdateQuantumState(state);
            var after = state.GetStateVector();

            for (int i = 0; i < after.Length; i++)
                Assert.AreEqual(0.0, Complex.Abs(after[i] - before[i]), Eps);
        }

        // --- P0 / P1 ---

        [Test]
        public void P0_OnZero_PreservesState()
        {
            // P0 = |0><0|: projecting |0> onto |0> subspace leaves the state intact
            using var state = new QuantumState(1);
            using var circuit = new QuantumCircuit(1);
            circuit.P0(0);
            circuit.UpdateQuantumState(state);
            var v = state.GetStateVector();
            Assert.AreEqual(1.0, v[0].Real, Eps);
            Assert.AreEqual(0.0, Complex.Abs(v[1]), Eps);
        }

        [Test]
        public void P1_OnZero_ZeroesAmplitude()
        {
            // P1 = |1><1|: projecting |0> onto |1> subspace yields zero vector
            using var state = new QuantumState(1);
            using var circuit = new QuantumCircuit(1);
            circuit.P1(0);
            circuit.UpdateQuantumState(state);
            Assert.AreEqual(0.0, state.GetSquaredNorm(), Eps);
        }

        // --- CZ ---

        [Test]
        public void CZ_OnOneOne_FlipsPhase()
        {
            // CZ|11> = -|11>  (phase kickback on |11>)
            using var state = new QuantumState(2);
            using var circuit = new QuantumCircuit(2);
            circuit.X(0).X(1).CZ(0, 1);
            circuit.UpdateQuantumState(state);
            var v = state.GetStateVector();
            for (int i = 0; i < 3; i++)
                Assert.AreEqual(0.0, Complex.Abs(v[i]), Eps);
            Assert.AreEqual(-1.0, v[3].Real,      Eps);
            Assert.AreEqual( 0.0, v[3].Imaginary, Eps);
        }

        // --- U1 / U2 / U3 ---

        [Test]
        public void U1_WithPiAngle_EquivToZ()
        {
            // U1(π) = diag(1, e^{iπ}) = diag(1, -1) = Z
            // Z(H|0>) = Z|+> = |->: amplitudes [1/√2, -1/√2]
            var v = ApplyToZero(1, c => c.H(0).U1(0, Math.PI));
            Assert.AreEqual( Inv_Sqrt2, v[0].Real, Eps);
            Assert.AreEqual(-Inv_Sqrt2, v[1].Real, Eps);
        }

        [Test]
        public void U2_WithZeroPiParams_EquivToH()
        {
            // U2(0, π) = (1/√2)[[1,1],[1,-1]] = H
            var v = ApplyToZero(1, c => c.U2(0, 0, Math.PI));
            Assert.AreEqual(Inv_Sqrt2, v[0].Real, Eps);
            Assert.AreEqual(Inv_Sqrt2, v[1].Real, Eps);
        }

        [Test]
        public void U3_WithPiZeroPiParams_EquivToX()
        {
            // U3(π, 0, π) = [[0,1],[1,0]] = X
            var v = ApplyToZero(1, c => c.U3(0, Math.PI, 0, Math.PI));
            Assert.AreEqual(0.0, Complex.Abs(v[0]), Eps);
            Assert.AreEqual(1.0, Complex.Abs(v[1]), Eps);
        }

        // --- Measure ---

        [Test]
        public void Measure_AfterSuperposition_CollapsesState()
        {
            // H creates |+>, Measure collapses to |0> or |1>
            using var state = new QuantumState(1);
            using var circuit = new QuantumCircuit(1);
            circuit.H(0).Measure(0, 0);
            circuit.UpdateQuantumState(state);
            double p0 = state.GetZeroProbability(0);
            Assert.IsTrue(p0 < Eps || p0 > 1.0 - Eps,
                $"Expected P(|0>) ≈ 0 or 1 after measurement, got {p0}");
            Assert.AreEqual(1.0, state.GetSquaredNorm(), Eps);
        }

        // --- SWAP ---

        [Test]
        public void SWAP_ExchangesQubits()
        {
            // X(0) → |10>, SWAP(0,1) → |01>
            var v = ApplyToZero(2, c => c.X(0).SWAP(0, 1));
            // index 2 = |10> in little-endian qubit1=1, qubit0=0
            Assert.AreEqual(1.0, v[2].Real, Eps);
            for (int i = 0; i < v.Length; i++)
                if (i != 2) Assert.AreEqual(0.0, Complex.Abs(v[i]), Eps);
        }
    }
}
