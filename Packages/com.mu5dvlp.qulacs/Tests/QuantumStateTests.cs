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
