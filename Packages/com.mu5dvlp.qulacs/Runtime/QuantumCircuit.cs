using System;
using Mu5dvlp.Qulacs.Internal;

namespace Mu5dvlp.Qulacs
{
    /// <summary>
    /// Represents a quantum circuit backed by the Qulacs native library.
    /// Gates are appended in order and applied to a <see cref="QuantumState"/> via
    /// <see cref="UpdateQuantumState"/>.
    /// Must be disposed when no longer needed to free native resources.
    /// This class is NOT thread-safe. Do not share an instance across threads without external synchronization.
    /// </summary>
    public sealed class QuantumCircuit : IDisposable
    {
        private IntPtr _handle;
        private bool _disposed;

        /// <summary>Number of qubits in the circuit.</summary>
        public int QubitCount { get; }

        /// <summary>Number of gates currently in the circuit.</summary>
        public int GateCount
        {
            get
            {
                ThrowIfDisposed();
                return (int)NativeMethods.qulacs_circuit_get_gate_count(_handle);
            }
        }

        public QuantumCircuit(int qubitCount)
        {
            if (qubitCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(qubitCount), "qubitCount must be > 0.");

            _handle = NativeMethods.qulacs_circuit_create((uint)qubitCount);
            if (_handle == IntPtr.Zero)
                throw new InvalidOperationException("Failed to create QuantumCircuit in native library.");

            QubitCount = qubitCount;
        }

        /// <summary>Applies all gates in the circuit to the given state in order.</summary>
        public void UpdateQuantumState(QuantumState state)
        {
            ThrowIfDisposed();
            if (state == null) throw new ArgumentNullException(nameof(state));
            NativeMethods.qulacs_circuit_update_quantum_state(_handle, state.Handle);
        }

        // --- Single-qubit gates ---

        public QuantumCircuit H(int qubitIndex)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_H_gate(_handle, (uint)qubitIndex);
            return this;
        }

        public QuantumCircuit X(int qubitIndex)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_X_gate(_handle, (uint)qubitIndex);
            return this;
        }

        public QuantumCircuit Y(int qubitIndex)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_Y_gate(_handle, (uint)qubitIndex);
            return this;
        }

        public QuantumCircuit Z(int qubitIndex)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_Z_gate(_handle, (uint)qubitIndex);
            return this;
        }

        public QuantumCircuit S(int qubitIndex)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_S_gate(_handle, (uint)qubitIndex);
            return this;
        }

        public QuantumCircuit Sdag(int qubitIndex)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_Sdag_gate(_handle, (uint)qubitIndex);
            return this;
        }

        public QuantumCircuit T(int qubitIndex)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_T_gate(_handle, (uint)qubitIndex);
            return this;
        }

        public QuantumCircuit Tdag(int qubitIndex)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_Tdag_gate(_handle, (uint)qubitIndex);
            return this;
        }

        public QuantumCircuit Identity(int qubitIndex)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_Identity_gate(_handle, (uint)qubitIndex);
            return this;
        }

        public QuantumCircuit SqrtX(int qubitIndex)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_sqrtX_gate(_handle, (uint)qubitIndex);
            return this;
        }

        public QuantumCircuit SqrtXdag(int qubitIndex)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_sqrtXdag_gate(_handle, (uint)qubitIndex);
            return this;
        }

        public QuantumCircuit SqrtY(int qubitIndex)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_sqrtY_gate(_handle, (uint)qubitIndex);
            return this;
        }

        public QuantumCircuit SqrtYdag(int qubitIndex)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_sqrtYdag_gate(_handle, (uint)qubitIndex);
            return this;
        }

        public QuantumCircuit P0(int qubitIndex)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_P0_gate(_handle, (uint)qubitIndex);
            return this;
        }

        public QuantumCircuit P1(int qubitIndex)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_P1_gate(_handle, (uint)qubitIndex);
            return this;
        }

        // --- Rotation gates ---
        // Qulacs convention: R{X,Y,Z}(θ) = exp(+iθP/2), opposite sign from standard exp(−iθP/2).

        /// <summary>Rotation around X-axis. Qulacs convention: RX(θ) = exp(+iθX/2).</summary>
        public QuantumCircuit RX(int qubitIndex, double angle)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_RX_gate(_handle, (uint)qubitIndex, angle);
            return this;
        }

        /// <summary>Rotation around Y-axis. Qulacs convention: RY(θ) = exp(+iθY/2).</summary>
        public QuantumCircuit RY(int qubitIndex, double angle)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_RY_gate(_handle, (uint)qubitIndex, angle);
            return this;
        }

        /// <summary>Rotation around Z-axis. Qulacs convention: RZ(θ) = exp(+iθZ/2).</summary>
        public QuantumCircuit RZ(int qubitIndex, double angle)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_RZ_gate(_handle, (uint)qubitIndex, angle);
            return this;
        }

        public QuantumCircuit U1(int qubitIndex, double lambda)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_U1_gate(_handle, (uint)qubitIndex, lambda);
            return this;
        }

        public QuantumCircuit U2(int qubitIndex, double phi, double lambda)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_U2_gate(_handle, (uint)qubitIndex, phi, lambda);
            return this;
        }

        public QuantumCircuit U3(int qubitIndex, double theta, double phi, double lambda)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_U3_gate(_handle, (uint)qubitIndex, theta, phi, lambda);
            return this;
        }

        // --- Two-qubit gates ---

        public QuantumCircuit CNOT(int control, int target)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_CNOT_gate(_handle, (uint)control, (uint)target);
            return this;
        }

        public QuantumCircuit CZ(int control, int target)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_CZ_gate(_handle, (uint)control, (uint)target);
            return this;
        }

        public QuantumCircuit SWAP(int qubit0, int qubit1)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_SWAP_gate(_handle, (uint)qubit0, (uint)qubit1);
            return this;
        }

        // --- Measurement ---

        /// <summary>
        /// Adds a measurement gate. The result is stored in the state's classical register
        /// at <paramref name="registerAddress"/>.
        /// </summary>
        public QuantumCircuit Measure(int qubitIndex, int registerAddress = 0)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_add_measurement_gate(
                _handle, (uint)qubitIndex, (uint)registerAddress);
            return this;
        }

        // --- Circuit inspection ---

        /// <summary>Returns the depth (critical path length) of the circuit.</summary>
        public int CalculateDepth()
        {
            ThrowIfDisposed();
            return (int)NativeMethods.qulacs_circuit_calculate_depth(_handle);
        }

        /// <summary>Returns true if all gates in the circuit are Clifford gates.</summary>
        public bool IsClifford()
        {
            ThrowIfDisposed();
            return NativeMethods.qulacs_circuit_is_Clifford(_handle) != 0;
        }

        /// <summary>Returns true if all gates in the circuit are Gaussian gates.</summary>
        public bool IsGaussian()
        {
            ThrowIfDisposed();
            return NativeMethods.qulacs_circuit_is_Gaussian(_handle) != 0;
        }

        // --- Circuit mutation ---

        /// <summary>Removes the gate at the given index.</summary>
        public void RemoveGate(int index)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_remove_gate(_handle, (uint)index);
        }

        /// <summary>Moves the gate at <paramref name="fromIndex"/> to <paramref name="toIndex"/>.</summary>
        public void MoveGate(int fromIndex, int toIndex)
        {
            ThrowIfDisposed();
            NativeMethods.qulacs_circuit_move_gate(_handle, (uint)fromIndex, (uint)toIndex);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(QuantumCircuit));
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                NativeMethods.qulacs_circuit_destroy(_handle);
                _handle = IntPtr.Zero;
                _disposed = true;
            }
        }
    }
}
