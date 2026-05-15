using System;
using System.Runtime.InteropServices;

namespace Mu5dvlp.Qulacs.Internal
{
    /// <summary>
    /// P/Invoke declarations for the qulacs_unity native library.
    ///
    /// Buffer safety: Array parameters (realOut, imagOut, resultsOut, etc.) must be
    /// pre-allocated by the caller with the correct length before passing to native code.
    /// The native side performs no bounds checking — undersized buffers cause undefined behavior.
    /// Required sizes: state vectors = 2^qubitCount, sampling results = count.
    ///
    /// Memory ownership: Handles returned by _create() must be freed with the matching _destroy().
    /// </summary>
    internal static class NativeMethods
    {
        private const string Lib = "qulacs_unity";

        // --- QuantumState ---

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr qulacs_state_create(uint qubitCount);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_state_destroy(IntPtr state);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint qulacs_state_get_qubit_count(IntPtr state);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_state_set_zero_state(IntPtr state);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_state_set_computational_basis(IntPtr state, ulong compBasis);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_state_set_haar_random_state(IntPtr state);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_state_set_haar_random_state_seed(IntPtr state, uint seed);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_state_get_vector(
            IntPtr state,
            [Out] double[] realOut,
            [Out] double[] imagOut,
            ulong length
        );

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_state_set_vector(
            IntPtr state,
            [In] double[] realIn,
            [In] double[] imagIn,
            ulong length
        );

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double qulacs_state_get_zero_probability(IntPtr state, uint qubitIndex);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double qulacs_state_get_squared_norm(IntPtr state);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_state_sampling(IntPtr state, uint count, [Out] ulong[] resultsOut);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_state_sampling_seed(
            IntPtr state,
            uint count,
            uint seed,
            [Out] ulong[] resultsOut
        );

        // --- QuantumCircuit ---

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr qulacs_circuit_create(uint qubitCount);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_destroy(IntPtr circuit);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint qulacs_circuit_get_qubit_count(IntPtr circuit);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint qulacs_circuit_get_gate_count(IntPtr circuit);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_update_quantum_state(IntPtr circuit, IntPtr state);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_H_gate(IntPtr circuit, uint qubitIndex);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_X_gate(IntPtr circuit, uint qubitIndex);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_Y_gate(IntPtr circuit, uint qubitIndex);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_Z_gate(IntPtr circuit, uint qubitIndex);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_S_gate(IntPtr circuit, uint qubitIndex);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_Sdag_gate(IntPtr circuit, uint qubitIndex);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_T_gate(IntPtr circuit, uint qubitIndex);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_Tdag_gate(IntPtr circuit, uint qubitIndex);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_Identity_gate(IntPtr circuit, uint qubitIndex);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_RX_gate(IntPtr circuit, uint qubitIndex, double angle);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_RY_gate(IntPtr circuit, uint qubitIndex, double angle);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_RZ_gate(IntPtr circuit, uint qubitIndex, double angle);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_CNOT_gate(IntPtr circuit, uint control, uint target);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_CZ_gate(IntPtr circuit, uint control, uint target);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_SWAP_gate(IntPtr circuit, uint qubit0, uint qubit1);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_measurement_gate(
            IntPtr circuit,
            uint qubitIndex,
            uint registerAddress
        );

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_sqrtX_gate(IntPtr circuit, uint qubitIndex);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_sqrtXdag_gate(IntPtr circuit, uint qubitIndex);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_sqrtY_gate(IntPtr circuit, uint qubitIndex);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_sqrtYdag_gate(IntPtr circuit, uint qubitIndex);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_P0_gate(IntPtr circuit, uint qubitIndex);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_P1_gate(IntPtr circuit, uint qubitIndex);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_U1_gate(IntPtr circuit, uint qubitIndex, double lambda);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_U2_gate(
            IntPtr circuit,
            uint qubitIndex,
            double phi,
            double lambda
        );

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_add_U3_gate(
            IntPtr circuit,
            uint qubitIndex,
            double theta,
            double phi,
            double lambda
        );

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint qulacs_circuit_calculate_depth(IntPtr circuit);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint qulacs_circuit_is_Clifford(IntPtr circuit);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint qulacs_circuit_is_Gaussian(IntPtr circuit);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_remove_gate(IntPtr circuit, uint index);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_circuit_move_gate(IntPtr circuit, uint fromIndex, uint toIndex);

        // --- QuantumState (additional) ---

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double qulacs_state_get_entropy(IntPtr state);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_state_add_state(IntPtr state, IntPtr other);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_state_multiply_coef(IntPtr state, double coefReal, double coefImag);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double qulacs_state_get_marginal_probability(
            IntPtr state,
            [In] uint[] measuredValues,
            uint length
        );

        // --- QuantumCircuit (additional) ---

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr qulacs_circuit_copy(IntPtr circuit);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr qulacs_circuit_get_inverse(IntPtr circuit);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint qulacs_circuit_to_string(IntPtr circuit, byte[] buf, uint bufSize);
    }
}
