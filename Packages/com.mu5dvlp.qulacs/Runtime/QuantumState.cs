using System;
using System.Numerics;
using Mu5dvlp.Qulacs.Internal;

namespace Mu5dvlp.Qulacs
{
    /// <summary>
    /// Represents a quantum state vector backed by the Qulacs native library.
    /// Must be disposed when no longer needed to free native resources.
    /// This class is NOT thread-safe. Do not share an instance across threads without external synchronization.
    /// </summary>
    public sealed class QuantumState : IDisposable
    {
        private IntPtr _handle;
        private bool _disposed;

        /// <summary>Number of qubits.</summary>
        public int QubitCount { get; }

        /// <summary>Dimension of the state vector (2^QubitCount).</summary>
        public int Dimension => 1 << QubitCount;

        public QuantumState(int qubitCount)
        {
            if (qubitCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(qubitCount), "qubitCount must be > 0.");

            _handle = NativeMethods.qulacs_state_create((uint)qubitCount);
            if (_handle == IntPtr.Zero)
                throw new InvalidOperationException("Failed to create QuantumState in native library.");

            QubitCount = qubitCount;
        }

        /// <summary>Resets to |0...0⟩.</summary>
        public void SetZeroState()
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_state_set_zero_state(_handle);
        }

        /// <summary>Sets the state to the given computational basis index.</summary>
        public void SetComputationalBasis(ulong basisIndex)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_state_set_computational_basis(_handle, basisIndex);
        }

        /// <summary>Sets the state to a random Haar-measure state.</summary>
        public void SetHaarRandomState()
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_state_set_haar_random_state(_handle);
        }

        /// <summary>Sets the state to a random Haar-measure state with a fixed seed.</summary>
        public void SetHaarRandomState(uint seed)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_state_set_haar_random_state_seed(_handle, seed);
        }

        /// <summary>
        /// Loads an arbitrary state vector. The array must have length 2^QubitCount.
        /// The vector need not be normalised beforehand.
        /// </summary>
        public void SetStateVector(Complex[] vector)
        {
            ThrowIfDisposed();
            if (vector == null)
                throw new ArgumentNullException(nameof(vector));
            if (vector.Length != Dimension)
                throw new ArgumentException($"vector.Length must be {Dimension}.", nameof(vector));

            var real = new double[Dimension];
            var imag = new double[Dimension];
            for (int i = 0; i < Dimension; i++)
            {
                real[i] = vector[i].Real;
                imag[i] = vector[i].Imaginary;
            }
            NativeMethods.qulacs_state_set_vector(_handle, real, imag, (ulong)Dimension);
        }

        /// <summary>
        /// Returns the full state vector as an array of complex amplitudes.
        /// Length is 2^QubitCount.
        /// </summary>
        public Complex[] GetStateVector()
        {
            ThrowIfDisposed();
            int dim = Dimension;
            var real = new double[dim];
            var imag = new double[dim];
            NativeMethods.qulacs_state_get_vector(_handle, real, imag, (ulong)dim);

            var result = new Complex[dim];
            for (int i = 0; i < dim; i++)
                result[i] = new Complex(real[i], imag[i]);
            return result;
        }

        /// <summary>
        /// Returns the probability of measuring |0⟩ on the given qubit.
        /// </summary>
        public double GetZeroProbability(int qubitIndex)
        {
            ThrowIfDisposed();
            return NativeMethods.qulacs_state_get_zero_probability(_handle, (uint)qubitIndex);
        }

        /// <summary>Returns the von Neumann entropy of the state.</summary>
        public double GetEntropy()
        {
            ThrowIfDisposed();
            return NativeMethods.qulacs_state_get_entropy(_handle);
        }

        /// <summary>Returns the squared norm of the state vector (should be ~1 for normalized states).</summary>
        public double GetSquaredNorm()
        {
            ThrowIfDisposed();
            return NativeMethods.qulacs_state_get_squared_norm(_handle);
        }

        /// <summary>
        /// Samples the state and returns an array of measurement outcomes (basis indices).
        /// </summary>
        public ulong[] Sampling(int count)
        {
            ThrowIfDisposed();
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            var results = new ulong[count];
            NativeMethods.qulacs_state_sampling(_handle, (uint)count, results);
            return results;
        }

        /// <summary>Samples with a fixed random seed for reproducibility.</summary>
        public ulong[] Sampling(int count, uint seed)
        {
            ThrowIfDisposed();
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            var results = new ulong[count];
            NativeMethods.qulacs_state_sampling_seed(_handle, (uint)count, seed, results);
            return results;
        }

        /// <summary>Adds another state's vector element-wise: |this⟩ += |other⟩.</summary>
        public void AddState(QuantumState other)
        {
            ThrowIfDisposed();
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            NativeMethods.qulacs_state_add_state(_handle, other.Handle);
        }

        /// <summary>Multiplies the entire state vector by a complex scalar.</summary>
        public void MultiplyCoef(Complex coef)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_state_multiply_coef(_handle, coef.Real, coef.Imaginary);
        }

        /// <summary>
        /// Returns the marginal probability for the given measurement pattern.
        /// Each element is 0 (measure |0⟩), 1 (measure |1⟩), or 2 (not measured).
        /// Length must equal QubitCount.
        /// </summary>
        public double GetMarginalProbability(int[] measuredValues)
        {
            ThrowIfDisposed();
            if (measuredValues == null)
                throw new ArgumentNullException(nameof(measuredValues));
            if (measuredValues.Length != QubitCount)
                throw new ArgumentException($"Length must be {QubitCount}.", nameof(measuredValues));

            var mv = new uint[measuredValues.Length];
            for (int i = 0; i < measuredValues.Length; i++)
                mv[i] = (uint)measuredValues[i];
            return NativeMethods.qulacs_state_get_marginal_probability(_handle, mv, (uint)mv.Length);
        }

        internal IntPtr Handle
        {
            get
            {
                ThrowIfDisposed();
                return _handle;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(QuantumState));
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                NativeMethods.qulacs_state_destroy(_handle);
                _handle = IntPtr.Zero;
                _disposed = true;
            }
        }
    }
}
