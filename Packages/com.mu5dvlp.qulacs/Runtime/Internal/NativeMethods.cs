using System;
using System.Runtime.InteropServices;

namespace Mu5dvlp.Qulacs.Internal
{
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
            ulong length);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_state_set_vector(
            IntPtr state,
            [In] double[] realIn,
            [In] double[] imagIn,
            ulong length);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double qulacs_state_get_zero_probability(IntPtr state, uint qubitIndex);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double qulacs_state_get_squared_norm(IntPtr state);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_state_sampling(
            IntPtr state, uint count, [Out] ulong[] resultsOut);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void qulacs_state_sampling_seed(
            IntPtr state, uint count, uint seed, [Out] ulong[] resultsOut);

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
            IntPtr circuit, uint qubitIndex, uint registerAddress);
    }
}
