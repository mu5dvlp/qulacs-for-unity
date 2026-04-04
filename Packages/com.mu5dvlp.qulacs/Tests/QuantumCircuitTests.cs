using System;
using System.Collections.Generic;
using System.Numerics;
using Mu5dvlp.Qulacs;
using NUnit.Framework;

namespace Mu5dvlp.Qulacs.Tests
{
    /// <summary>
    /// Tests for QuantumCircuit construction, gate ordering, and statistical properties.
    /// Reference: Qulacs test/cppsim/test_circuit.cpp (CircuitTest::CircuitBasic)
    /// </summary>
    public class QuantumCircuitTests
    {
        private const double Eps = 1e-6;
        private const double Inv_Sqrt2 = 0.70710678118654752;

        // --- Construction ---

        [Test]
        public void Constructor_SetsQubitCount()
        {
            using var circuit = new QuantumCircuit(3);
            Assert.AreEqual(3, circuit.QubitCount);
        }

        [Test]
        public void Constructor_GateCountIsZero()
        {
            using var circuit = new QuantumCircuit(2);
            Assert.AreEqual(0, circuit.GateCount);
        }

        [Test]
        public void Constructor_InvalidQubitCount_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new QuantumCircuit(0));
        }

        // --- Gate count ---

        [Test]
        public void GateCount_IncreasesAsGatesAreAdded()
        {
            using var circuit = new QuantumCircuit(2);
            circuit.H(0);
            Assert.AreEqual(1, circuit.GateCount);
            circuit.CNOT(0, 1);
            Assert.AreEqual(2, circuit.GateCount);
            circuit.RZ(1, 0.5);
            Assert.AreEqual(3, circuit.GateCount);
        }

        // --- Fluent chaining ---

        [Test]
        public void FluentChaining_ReturnsTheSameCircuit()
        {
            using var circuit = new QuantumCircuit(2);
            var returned = circuit.H(0).CNOT(0, 1).Z(1);
            Assert.AreSame(circuit, returned);
        }

        // --- Bell state ---

        [Test]
        public void BellState_AmplitudesAreCorrect()
        {
            // H(0), CNOT(0,1) → (|00> + |11>) / √2
            using var state = new QuantumState(2);
            using var circuit = new QuantumCircuit(2);
            circuit.H(0).CNOT(0, 1);
            circuit.UpdateQuantumState(state);

            var v = state.GetStateVector();
            Assert.AreEqual(Inv_Sqrt2, v[0].Real, Eps); // |00>
            Assert.AreEqual(0.0, Complex.Abs(v[1]), Eps); // |01>
            Assert.AreEqual(0.0, Complex.Abs(v[2]), Eps); // |10>
            Assert.AreEqual(Inv_Sqrt2, v[3].Real, Eps); // |11>
        }

        [Test]
        public void BellState_SquaredNormIsOne()
        {
            using var state = new QuantumState(2);
            using var circuit = new QuantumCircuit(2);
            circuit.H(0).CNOT(0, 1);
            circuit.UpdateQuantumState(state);
            Assert.AreEqual(1.0, state.GetSquaredNorm(), Eps);
        }

        [Test]
        public void BellState_EachQubitHalfProbabilityOfZero()
        {
            using var state = new QuantumState(2);
            using var circuit = new QuantumCircuit(2);
            circuit.H(0).CNOT(0, 1);
            circuit.UpdateQuantumState(state);
            Assert.AreEqual(0.5, state.GetZeroProbability(0), Eps);
            Assert.AreEqual(0.5, state.GetZeroProbability(1), Eps);
        }

        [Test]
        public void BellState_SamplingShowsOnlyCorrelatedOutcomes()
        {
            // Bell state must produce only |00> or |11>, never |01> or |10>
            using var state = new QuantumState(2);
            using var circuit = new QuantumCircuit(2);
            circuit.H(0).CNOT(0, 1);
            circuit.UpdateQuantumState(state);

            var samples = state.Sampling(500, seed: 42);
            foreach (var s in samples)
                Assert.IsTrue(s == 0UL || s == 3UL,
                    $"Expected |00>(0) or |11>(3), got {s}");
        }

        [Test]
        public void BellState_SamplingIsApproximately50_50()
        {
            using var state = new QuantumState(2);
            using var circuit = new QuantumCircuit(2);
            circuit.H(0).CNOT(0, 1);
            circuit.UpdateQuantumState(state);

            int shots = 2000;
            var samples = state.Sampling(shots, seed: 0);
            int count00 = 0;
            foreach (var s in samples)
                if (s == 0UL) count00++;

            double ratio = (double)count00 / shots;
            Assert.AreEqual(0.5, ratio, 0.05); // within 5%
        }

        // --- Circuit reuse ---

        [Test]
        public void Circuit_AppliedTwice_AccumulatesEffect()
        {
            // Applying X twice on the same state restores it
            using var state = new QuantumState(1);
            using var circuit = new QuantumCircuit(1);
            circuit.X(0);
            circuit.UpdateQuantumState(state);
            circuit.UpdateQuantumState(state);

            var v = state.GetStateVector();
            Assert.AreEqual(1.0, v[0].Real, Eps);
            Assert.AreEqual(0.0, Complex.Abs(v[1]), Eps);
        }

        // --- GHZ state (3-qubit entanglement) ---

        [Test]
        public void GHZState_OnlyCorrelatedOutcomes()
        {
            // H(0), CNOT(0,1), CNOT(0,2) → (|000> + |111>) / √2
            using var state = new QuantumState(3);
            using var circuit = new QuantumCircuit(3);
            circuit.H(0).CNOT(0, 1).CNOT(0, 2);
            circuit.UpdateQuantumState(state);

            var v = state.GetStateVector();
            Assert.AreEqual(Inv_Sqrt2, v[0].Real, Eps); // |000>
            Assert.AreEqual(Inv_Sqrt2, v[7].Real, Eps); // |111>
            for (int i = 1; i <= 6; i++)
                Assert.AreEqual(0.0, Complex.Abs(v[i]), Eps);
        }

        // --- Dispose ---

        [Test]
        public void Dispose_CalledTwice_DoesNotThrow()
        {
            var circuit = new QuantumCircuit(2);
            circuit.Dispose();
            Assert.DoesNotThrow(() => circuit.Dispose());
        }

        [Test]
        public void AfterDispose_MethodsThrowObjectDisposedException()
        {
            var circuit = new QuantumCircuit(2);
            circuit.Dispose();
            Assert.Throws<ObjectDisposedException>(() => circuit.H(0));
            Assert.Throws<ObjectDisposedException>(() => { var _ = circuit.GateCount; });
        }
    }
}
