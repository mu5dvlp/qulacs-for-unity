#pragma once

#include <stdint.h>

#if defined(_WIN32)
#define QULACS_UNITY_API __declspec(dllexport)
#else
#define QULACS_UNITY_API __attribute__((visibility("default")))
#endif

#ifdef __cplusplus
extern "C" {
#endif

/* -------------------------------------------------------------------------
 * QuantumState
 * Opaque handle: void* points to a heap-allocated QuantumState.
 *
 * Ownership: qulacs_state_create() allocates; the caller owns the handle
 * and MUST call qulacs_state_destroy() exactly once to free it.
 * Passing NULL or a dangling handle to any function is undefined behavior.
 * -------------------------------------------------------------------------*/

QULACS_UNITY_API void* qulacs_state_create(uint32_t qubit_count);
QULACS_UNITY_API void qulacs_state_destroy(void* state);

QULACS_UNITY_API uint32_t qulacs_state_get_qubit_count(void* state);

QULACS_UNITY_API void qulacs_state_set_zero_state(void* state);
QULACS_UNITY_API void qulacs_state_set_computational_basis(void* state, uint64_t comp_basis);
QULACS_UNITY_API void qulacs_state_set_haar_random_state(void* state);
QULACS_UNITY_API void qulacs_state_set_haar_random_state_seed(void* state, uint32_t seed);

/* Copies the state vector into caller-allocated arrays.
 * Buffer safety: real_out and imag_out must each have at least
 * min(length, 1 << qubit_count) elements. No bounds checking is performed.
 * real_out[i] = Re(amplitude[i]), imag_out[i] = Im(amplitude[i]) */
QULACS_UNITY_API void qulacs_state_get_vector(void* state, double* real_out, double* imag_out, uint64_t length);

/* Loads an arbitrary state vector from caller-supplied arrays.
 * Buffer safety: real_in and imag_in must each have at least
 * min(length, 1 << qubit_count) elements. No bounds checking is performed. */
QULACS_UNITY_API void qulacs_state_set_vector(void* state, const double* real_in, const double* imag_in,
                                              uint64_t length);

QULACS_UNITY_API double qulacs_state_get_zero_probability(void* state, uint32_t qubit_index);
QULACS_UNITY_API double qulacs_state_get_squared_norm(void* state);

/* Samples the state and writes `count` measurement outcomes into results_out.
 * Buffer safety: results_out must have at least `count` elements. */
QULACS_UNITY_API void qulacs_state_sampling(void* state, uint32_t count, uint64_t* results_out);
QULACS_UNITY_API void qulacs_state_sampling_seed(void* state, uint32_t count, uint32_t seed, uint64_t* results_out);

/* -------------------------------------------------------------------------
 * QuantumCircuit
 * Opaque handle: void* points to a heap-allocated QuantumCircuit.
 *
 * Ownership: qulacs_circuit_create() allocates; the caller owns the handle
 * and MUST call qulacs_circuit_destroy() exactly once to free it.
 * -------------------------------------------------------------------------*/

QULACS_UNITY_API void* qulacs_circuit_create(uint32_t qubit_count);
QULACS_UNITY_API void qulacs_circuit_destroy(void* circuit);

QULACS_UNITY_API uint32_t qulacs_circuit_get_qubit_count(void* circuit);
QULACS_UNITY_API uint32_t qulacs_circuit_get_gate_count(void* circuit);

QULACS_UNITY_API void qulacs_circuit_update_quantum_state(void* circuit, void* state);

/* Single-qubit gates */
QULACS_UNITY_API void qulacs_circuit_add_H_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void qulacs_circuit_add_X_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void qulacs_circuit_add_Y_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void qulacs_circuit_add_Z_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void qulacs_circuit_add_S_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void qulacs_circuit_add_Sdag_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void qulacs_circuit_add_T_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void qulacs_circuit_add_Tdag_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void qulacs_circuit_add_Identity_gate(void* circuit, uint32_t qubit_index);

/* Rotation gates */
QULACS_UNITY_API void qulacs_circuit_add_RX_gate(void* circuit, uint32_t qubit_index, double angle);
QULACS_UNITY_API void qulacs_circuit_add_RY_gate(void* circuit, uint32_t qubit_index, double angle);
QULACS_UNITY_API void qulacs_circuit_add_RZ_gate(void* circuit, uint32_t qubit_index, double angle);

/* Two-qubit gates */
QULACS_UNITY_API void qulacs_circuit_add_CNOT_gate(void* circuit, uint32_t control, uint32_t target);
QULACS_UNITY_API void qulacs_circuit_add_CZ_gate(void* circuit, uint32_t control, uint32_t target);
QULACS_UNITY_API void qulacs_circuit_add_SWAP_gate(void* circuit, uint32_t qubit0, uint32_t qubit1);

/* Measurement gate — result written to classical register at `register_address` */
QULACS_UNITY_API void qulacs_circuit_add_measurement_gate(void* circuit, uint32_t qubit_index,
                                                          uint32_t register_address);

/* Additional single-qubit gates */
QULACS_UNITY_API void qulacs_circuit_add_sqrtX_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void qulacs_circuit_add_sqrtXdag_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void qulacs_circuit_add_sqrtY_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void qulacs_circuit_add_sqrtYdag_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void qulacs_circuit_add_P0_gate(void* circuit, uint32_t qubit_index);
QULACS_UNITY_API void qulacs_circuit_add_P1_gate(void* circuit, uint32_t qubit_index);

/* IBM-convention rotation gates */
QULACS_UNITY_API void qulacs_circuit_add_U1_gate(void* circuit, uint32_t qubit_index, double lambda);
QULACS_UNITY_API void qulacs_circuit_add_U2_gate(void* circuit, uint32_t qubit_index, double phi, double lambda);
QULACS_UNITY_API void qulacs_circuit_add_U3_gate(void* circuit, uint32_t qubit_index, double theta, double phi,
                                                 double lambda);

/* Circuit inspection */
QULACS_UNITY_API uint32_t qulacs_circuit_calculate_depth(void* circuit);
QULACS_UNITY_API uint32_t qulacs_circuit_is_Clifford(void* circuit);
QULACS_UNITY_API uint32_t qulacs_circuit_is_Gaussian(void* circuit);

/* Circuit mutation */
QULACS_UNITY_API void qulacs_circuit_remove_gate(void* circuit, uint32_t index);
QULACS_UNITY_API void qulacs_circuit_move_gate(void* circuit, uint32_t from_index, uint32_t to_index);

/* -------------------------------------------------------------------------
 * QuantumState (additional)
 * -------------------------------------------------------------------------*/
QULACS_UNITY_API double qulacs_state_get_entropy(void* state);

/* Adds another state's vector element-wise: |this⟩ += |other⟩ */
QULACS_UNITY_API void qulacs_state_add_state(void* state, const void* other);

/* Multiplies the entire state vector by a complex scalar. */
QULACS_UNITY_API void qulacs_state_multiply_coef(void* state, double coef_real, double coef_imag);

/* Returns marginal probability.
 * measured_values[i] ∈ {0, 1, 2}: 0/1 = observed value, 2 = not measured.
 * Length must equal qubit_count. */
QULACS_UNITY_API double qulacs_state_get_marginal_probability(void* state, const uint32_t* measured_values,
                                                              uint32_t length);

/* -------------------------------------------------------------------------
 * QuantumCircuit (additional)
 * -------------------------------------------------------------------------*/

/* Returns a deep copy of the circuit. Caller owns the returned handle. */
QULACS_UNITY_API void* qulacs_circuit_copy(void* circuit);

/* Returns the inverse (adjoint) circuit. Caller owns the returned handle. */
QULACS_UNITY_API void* qulacs_circuit_get_inverse(void* circuit);

/* Writes the string representation into buf (including NUL).
 * Returns the required buffer size (including NUL).
 * If buf is NULL or buf_size is 0, only the required size is returned. */
QULACS_UNITY_API uint32_t qulacs_circuit_to_string(void* circuit, char* buf, uint32_t buf_size);

#ifdef __cplusplus
}
#endif
