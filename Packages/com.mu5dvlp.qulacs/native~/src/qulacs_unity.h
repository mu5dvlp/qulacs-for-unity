#pragma once

#include <stdint.h>

#if defined(_WIN32)
#  define QULACS_UNITY_API __declspec(dllexport)
#else
#  define QULACS_UNITY_API __attribute__((visibility("default")))
#endif

#ifdef __cplusplus
extern "C" {
#endif

/* -------------------------------------------------------------------------
 * QuantumState
 * Opaque handle: void* points to a heap-allocated QuantumState.
 * -------------------------------------------------------------------------*/

QULACS_UNITY_API void*        qulacs_state_create(uint32_t qubit_count);
QULACS_UNITY_API void         qulacs_state_destroy(void* state);

QULACS_UNITY_API uint32_t     qulacs_state_get_qubit_count(void* state);

QULACS_UNITY_API void         qulacs_state_set_zero_state(void* state);
QULACS_UNITY_API void         qulacs_state_set_computational_basis(void* state, uint64_t comp_basis);
QULACS_UNITY_API void         qulacs_state_set_haar_random_state(void* state);
QULACS_UNITY_API void         qulacs_state_set_haar_random_state_seed(void* state, uint32_t seed);

/* Copies the state vector into caller-allocated arrays of length (1 << qubit_count).
 * real_out[i] = Re(amplitude[i]), imag_out[i] = Im(amplitude[i]) */
QULACS_UNITY_API void         qulacs_state_get_vector(void* state, double* real_out, double* imag_out, uint64_t length);

/* Loads an arbitrary state vector from caller-supplied arrays of length (1 << qubit_count).
 * The vector need not be normalised beforehand. */
QULACS_UNITY_API void         qulacs_state_set_vector(void* state, const double* real_in, const double* imag_in, uint64_t length);

QULACS_UNITY_API double       qulacs_state_get_zero_probability(void* state, uint32_t qubit_index);
QULACS_UNITY_API double       qulacs_state_get_squared_norm(void* state);

/* Samples the state and writes `count` measurement outcomes into results_out. */
QULACS_UNITY_API void         qulacs_state_sampling(void* state, uint32_t count, uint64_t* results_out);
QULACS_UNITY_API void         qulacs_state_sampling_seed(void* state, uint32_t count, uint32_t seed, uint64_t* results_out);

/* -------------------------------------------------------------------------
 * QuantumCircuit
 * -------------------------------------------------------------------------*/

QULACS_UNITY_API void*        qulacs_circuit_create(uint32_t qubit_count);
QULACS_UNITY_API void         qulacs_circuit_destroy(void* circuit);

QULACS_UNITY_API uint32_t     qulacs_circuit_get_qubit_count(void* circuit);
QULACS_UNITY_API uint32_t     qulacs_circuit_get_gate_count(void* circuit);

QULACS_UNITY_API void         qulacs_circuit_update_quantum_state(void* circuit, void* state);

/* Single-qubit gates */
QULACS_UNITY_API void         qulacs_circuit_add_H_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void         qulacs_circuit_add_X_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void         qulacs_circuit_add_Y_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void         qulacs_circuit_add_Z_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void         qulacs_circuit_add_S_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void         qulacs_circuit_add_Sdag_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void         qulacs_circuit_add_T_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void         qulacs_circuit_add_Tdag_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void         qulacs_circuit_add_Identity_gate(void* circuit, uint32_t qubit_index);

/* Rotation gates */
QULACS_UNITY_API void         qulacs_circuit_add_RX_gate(void* circuit, uint32_t qubit_index, double angle);
QULACS_UNITY_API void         qulacs_circuit_add_RY_gate(void* circuit, uint32_t qubit_index, double angle);
QULACS_UNITY_API void         qulacs_circuit_add_RZ_gate(void* circuit, uint32_t qubit_index, double angle);

/* Two-qubit gates */
QULACS_UNITY_API void         qulacs_circuit_add_CNOT_gate(void* circuit, uint32_t control, uint32_t target);
QULACS_UNITY_API void         qulacs_circuit_add_CZ_gate(void* circuit, uint32_t control, uint32_t target);
QULACS_UNITY_API void         qulacs_circuit_add_SWAP_gate(void* circuit, uint32_t qubit0, uint32_t qubit1);

/* Measurement gate — result written to classical register at `register_address` */
QULACS_UNITY_API void         qulacs_circuit_add_measurement_gate(void* circuit, uint32_t qubit_index, uint32_t register_address);

#ifdef __cplusplus
}
#endif
