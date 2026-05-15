using System;
using System.Numerics;
using Mu5dvlp.Qulacs;
using NUnit.Framework;

namespace Mu5dvlp.Qulacs.Tests
{
    public class QuantumStateTests
    {
        private const double Eps = 1e-6;
        private const double Inv_Sqrt2 = 0.70710678118654752;

        // --- Construction ---

        [Test]
        public void Constructor_SetsQubitCount()
        {
            using var state = new QuantumState(3);
            Assert.AreEqual(3, state.QubitCount);
        }

        [Test]
        public void Constructor_DimensionIs2PowerN()
        {
            using var state = new QuantumState(4);
            Assert.AreEqual(16, state.Dimension);
        }

        [Test]
        public void Constructor_InvalidQubitCount_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new QuantumState(0));
        }

        // --- Initial state ---

        [Test]
        public void InitialState_IsZeroState()
        {
            using var state = new QuantumState(2);
            var vec = state.GetStateVector();
            // |00> has amplitude 1 at index 0, 0 elsewhere
            Assert.AreEqual(1.0, vec[0].Real, Eps);
            Assert.AreEqual(0.0, vec[0].Imaginary, Eps);
            for (int i = 1; i < vec.Length; i++)
                Assert.AreEqual(0.0, Complex.Abs(vec[i]), Eps);
        }

        // --- SetZeroState ---

        [Test]
        public void SetZeroState_ResetsToComputationalZero()
        {
            using var state = new QuantumState(2);
            state.SetHaarRandomState();
            state.SetZeroState();
            var vec = state.GetStateVector();
            Assert.AreEqual(1.0, vec[0].Real, Eps);
            for (int i = 1; i < vec.Length; i++)
                Assert.AreEqual(0.0, Complex.Abs(vec[i]), Eps);
        }

        // --- SetComputationalBasis ---

        [Test]
        public void SetComputationalBasis_SetsCorrectBasis()
        {
            using var state = new QuantumState(3);
            state.SetComputationalBasis(5); // |101>
            var vec = state.GetStateVector();
            Assert.AreEqual(1.0, vec[5].Real, Eps);
            for (int i = 0; i < vec.Length; i++)
                if (i != 5) Assert.AreEqual(0.0, Complex.Abs(vec[i]), Eps);
        }

        // --- GetSquaredNorm ---

        [Test]
        public void GetSquaredNorm_ZeroState_IsOne()
        {
            using var state = new QuantumState(3);
            Assert.AreEqual(1.0, state.GetSquaredNorm(), Eps);
        }

        [Test]
        public void GetSquaredNorm_HaarRandomState_IsOne()
        {
            using var state = new QuantumState(4);
            state.SetHaarRandomState(seed: 42);
            Assert.AreEqual(1.0, state.GetSquaredNorm(), Eps);
        }

        // --- GetZeroProbability ---

        [Test]
        public void GetZeroProbability_ZeroState_AllQubitsAreZero()
        {
            using var state = new QuantumState(3);
            for (int q = 0; q < 3; q++)
                Assert.AreEqual(1.0, state.GetZeroProbability(q), Eps);
        }

        [Test]
        public void GetZeroProbability_BasisState5_CorrectProbabilities()
        {
            // |101> : qubit0=1, qubit1=0, qubit2=1
            using var state = new QuantumState(3);
            state.SetComputationalBasis(5);
            Assert.AreEqual(0.0, state.GetZeroProbability(0), Eps); // qubit0 = 1
            Assert.AreEqual(1.0, state.GetZeroProbability(1), Eps); // qubit1 = 0
            Assert.AreEqual(0.0, state.GetZeroProbability(2), Eps); // qubit2 = 1
        }

        // --- Sampling ---

        [Test]
        public void Sampling_ZeroState_AlwaysReturnsZero()
        {
            using var state = new QuantumState(3);
            var results = state.Sampling(200, seed: 0);
            foreach (var r in results)
                Assert.AreEqual(0UL, r);
        }

        [Test]
        public void Sampling_BasisState_AlwaysReturnsThatBasis()
        {
            using var state = new QuantumState(3);
            state.SetComputationalBasis(6);
            var results = state.Sampling(100, seed: 0);
            foreach (var r in results)
                Assert.AreEqual(6UL, r);
        }

        [Test]
        public void Sampling_ReproducibleWithSameSeed()
        {
            using var state = new QuantumState(2);
            state.SetHaarRandomState(seed: 123);
            var a = state.Sampling(50, seed: 7);
            var b = state.Sampling(50, seed: 7);
            for (int i = 0; i < a.Length; i++)
                Assert.AreEqual(a[i], b[i]);
        }

        // --- SetStateVector ---

        [Test]
        public void SetStateVector_RoundTrip_PreservesAmplitudes()
        {
            using var state = new QuantumState(1);
            var input = new Complex[] { new Complex(Inv_Sqrt2, 0), new Complex(0, Inv_Sqrt2) };
            state.SetStateVector(input);
            var output = state.GetStateVector();
            for (int i = 0; i < input.Length; i++)
            {
                Assert.AreEqual(input[i].Real,      output[i].Real,      Eps);
                Assert.AreEqual(input[i].Imaginary, output[i].Imaginary, Eps);
            }
        }

        // --- GetEntropy ---

        [Test]
        public void GetEntropy_ZeroState_IsZero()
        {
            using var state = new QuantumState(2);
            Assert.AreEqual(0.0, state.GetEntropy(), Eps);
        }

        [Test]
        public void GetEntropy_EqualSuperposition_IsPositive()
        {
            using var state = new QuantumState(1);
            using var circuit = new QuantumCircuit(1);
            circuit.H(0);
            circuit.UpdateQuantumState(state);
            Assert.Greater(state.GetEntropy(), 0.0);
        }

        // --- Sampling (no seed) ---

        [Test]
        public void Sampling_NoSeed_ReturnsCorrectCountAndRange()
        {
            using var state = new QuantumState(3);
            state.SetHaarRandomState(seed: 0);
            var results = state.Sampling(50);
            Assert.AreEqual(50, results.Length);
            ulong maxIndex = (ulong)state.Dimension;
            foreach (var r in results)
                Assert.Less(r, maxIndex);
        }

        // --- AddState ---

        [Test]
        public void AddState_AddsVectorsElementWise()
        {
            using var a = new QuantumState(1);
            using var b = new QuantumState(1);
            // a = |0>, b = |1>
            b.SetComputationalBasis(1);
            a.AddState(b);
            var v = a.GetStateVector();
            Assert.AreEqual(1.0, v[0].Real, Eps);
            Assert.AreEqual(1.0, v[1].Real, Eps);
        }

        [Test]
        public void AddState_NullThrows()
        {
            using var state = new QuantumState(1);
            Assert.Throws<ArgumentNullException>(() => state.AddState(null));
        }

        // --- MultiplyCoef ---

        [Test]
        public void MultiplyCoef_ScalesVector()
        {
            using var state = new QuantumState(1);
            // |0> → multiply by 0.5 → amplitude[0] = 0.5
            state.MultiplyCoef(new Complex(0.5, 0.0));
            var v = state.GetStateVector();
            Assert.AreEqual(0.5, v[0].Real, Eps);
            Assert.AreEqual(0.0, v[0].Imaginary, Eps);
        }

        [Test]
        public void MultiplyCoef_ImaginaryCoef()
        {
            using var state = new QuantumState(1);
            state.MultiplyCoef(new Complex(0.0, 1.0));
            var v = state.GetStateVector();
            Assert.AreEqual(0.0, v[0].Real, Eps);
            Assert.AreEqual(1.0, v[0].Imaginary, Eps);
        }

        // --- GetMarginalProbability ---

        [Test]
        public void GetMarginalProbability_ZeroState_AllZero()
        {
            using var state = new QuantumState(2);
            // Both qubits measured as 0
            double p = state.GetMarginalProbability(new[] { 0, 0 });
            Assert.AreEqual(1.0, p, Eps);
        }

        [Test]
        public void GetMarginalProbability_BellState_MarginalIsHalf()
        {
            using var state = new QuantumState(2);
            using var circuit = new QuantumCircuit(2);
            circuit.H(0).CNOT(0, 1);
            circuit.UpdateQuantumState(state);
            // Measure qubit 0 as 0, qubit 1 not measured (2)
            double p = state.GetMarginalProbability(new[] { 0, 2 });
            Assert.AreEqual(0.5, p, Eps);
        }

        [Test]
        public void GetMarginalProbability_WrongLength_Throws()
        {
            using var state = new QuantumState(2);
            Assert.Throws<ArgumentException>(() =>
                state.GetMarginalProbability(new[] { 0, 0, 0 }));
        }

        // --- Dispose ---

        [Test]
        public void Dispose_CalledTwice_DoesNotThrow()
        {
            var state = new QuantumState(2);
            state.Dispose();
            Assert.DoesNotThrow(() => state.Dispose());
        }

        [Test]
        public void AfterDispose_MethodsThrowObjectDisposedException()
        {
            var state = new QuantumState(2);
            state.Dispose();
            Assert.Throws<ObjectDisposedException>(() => state.SetZeroState());
            Assert.Throws<ObjectDisposedException>(() => state.GetStateVector());
            Assert.Throws<ObjectDisposedException>(() => state.GetSquaredNorm());
        }
    }
}
