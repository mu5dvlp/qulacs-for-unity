#include "qulacs_unity.h"

#include <cppsim/state.hpp>
#include <cppsim/circuit.hpp>
#include <cppsim/gate_factory.hpp>

#include <cstring>   // memcpy
#include <algorithm> // std::min

// ---------------------------------------------------------------------------
// Internal helpers
// ---------------------------------------------------------------------------

static QuantumState* as_state(void* p) {
    return static_cast<QuantumState*>(p);
}

static QuantumCircuit* as_circuit(void* p) {
    return static_cast<QuantumCircuit*>(p);
}

// ---------------------------------------------------------------------------
// QuantumState
// ---------------------------------------------------------------------------

extern "C" {

void* qulacs_state_create(uint32_t qubit_count) {
    try {
        return new QuantumState(static_cast<UINT>(qubit_count));
    } catch (...) {
        return nullptr;
    }
}

void qulacs_state_destroy(void* state) {
    delete as_state(state);
}

uint32_t qulacs_state_get_qubit_count(void* state) {
    return static_cast<uint32_t>(as_state(state)->get_qubit_count());
}

void qulacs_state_set_zero_state(void* state) {
    as_state(state)->set_zero_state();
}

void qulacs_state_set_computational_basis(void* state, uint64_t comp_basis) {
    as_state(state)->set_computational_basis(static_cast<ITYPE>(comp_basis));
}

void qulacs_state_set_haar_random_state(void* state) {
    as_state(state)->set_Haar_random_state();
}

void qulacs_state_set_haar_random_state_seed(void* state, uint32_t seed) {
    as_state(state)->set_Haar_random_state(static_cast<UINT>(seed));
}

void qulacs_state_get_vector(void* state, double* real_out, double* imag_out, uint64_t length) {
    QuantumState* s = as_state(state);
    const CPPCTYPE* vec = s->data_cpp();
    uint64_t dim = static_cast<uint64_t>(s->get_dim());
    uint64_t count = (length < dim) ? length : dim;
    for (uint64_t i = 0; i < count; ++i) {
        real_out[i] = vec[i].real();
        imag_out[i] = vec[i].imag();
    }
}

double qulacs_state_get_zero_probability(void* state, uint32_t qubit_index) {
    return as_state(state)->get_zero_probability(static_cast<UINT>(qubit_index));
}

double qulacs_state_get_squared_norm(void* state) {
    return as_state(state)->get_squared_norm();
}

void qulacs_state_sampling(void* state, uint32_t count, uint64_t* results_out) {
    auto results = as_state(state)->sampling(static_cast<UINT>(count));
    for (uint32_t i = 0; i < count && i < results.size(); ++i) {
        results_out[i] = static_cast<uint64_t>(results[i]);
    }
}

void qulacs_state_sampling_seed(void* state, uint32_t count, uint32_t seed, uint64_t* results_out) {
    auto results = as_state(state)->sampling(static_cast<UINT>(count), static_cast<UINT>(seed));
    for (uint32_t i = 0; i < count && i < results.size(); ++i) {
        results_out[i] = static_cast<uint64_t>(results[i]);
    }
}

// ---------------------------------------------------------------------------
// QuantumCircuit
// ---------------------------------------------------------------------------

void* qulacs_circuit_create(uint32_t qubit_count) {
    try {
        return new QuantumCircuit(static_cast<UINT>(qubit_count));
    } catch (...) {
        return nullptr;
    }
}

void qulacs_circuit_destroy(void* circuit) {
    delete as_circuit(circuit);
}

uint32_t qulacs_circuit_get_qubit_count(void* circuit) {
    return static_cast<uint32_t>(as_circuit(circuit)->get_qubit_count());
}

uint32_t qulacs_circuit_get_gate_count(void* circuit) {
    return static_cast<uint32_t>(as_circuit(circuit)->get_gate_count());
}

void qulacs_circuit_update_quantum_state(void* circuit, void* state) {
    as_circuit(circuit)->update_quantum_state(as_state(state));
}

// Single-qubit gates
void qulacs_circuit_add_H_gate(void* circuit, uint32_t qubit_index) {
    as_circuit(circuit)->add_gate(gate::H(qubit_index));
}

void qulacs_circuit_add_X_gate(void* circuit, uint32_t qubit_index) {
    as_circuit(circuit)->add_gate(gate::X(qubit_index));
}

void qulacs_circuit_add_Y_gate(void* circuit, uint32_t qubit_index) {
    as_circuit(circuit)->add_gate(gate::Y(qubit_index));
}

void qulacs_circuit_add_Z_gate(void* circuit, uint32_t qubit_index) {
    as_circuit(circuit)->add_gate(gate::Z(qubit_index));
}

void qulacs_circuit_add_S_gate(void* circuit, uint32_t qubit_index) {
    as_circuit(circuit)->add_gate(gate::S(qubit_index));
}

void qulacs_circuit_add_Sdag_gate(void* circuit, uint32_t qubit_index) {
    as_circuit(circuit)->add_gate(gate::Sdag(qubit_index));
}

void qulacs_circuit_add_T_gate(void* circuit, uint32_t qubit_index) {
    as_circuit(circuit)->add_gate(gate::T(qubit_index));
}

void qulacs_circuit_add_Tdag_gate(void* circuit, uint32_t qubit_index) {
    as_circuit(circuit)->add_gate(gate::Tdag(qubit_index));
}

void qulacs_circuit_add_Identity_gate(void* circuit, uint32_t qubit_index) {
    as_circuit(circuit)->add_gate(gate::Identity(qubit_index));
}

// Rotation gates
void qulacs_circuit_add_RX_gate(void* circuit, uint32_t qubit_index, double angle) {
    as_circuit(circuit)->add_gate(gate::RX(qubit_index, angle));
}

void qulacs_circuit_add_RY_gate(void* circuit, uint32_t qubit_index, double angle) {
    as_circuit(circuit)->add_gate(gate::RY(qubit_index, angle));
}

void qulacs_circuit_add_RZ_gate(void* circuit, uint32_t qubit_index, double angle) {
    as_circuit(circuit)->add_gate(gate::RZ(qubit_index, angle));
}

// Two-qubit gates
void qulacs_circuit_add_CNOT_gate(void* circuit, uint32_t control, uint32_t target) {
    as_circuit(circuit)->add_gate(gate::CNOT(control, target));
}

void qulacs_circuit_add_CZ_gate(void* circuit, uint32_t control, uint32_t target) {
    as_circuit(circuit)->add_gate(gate::CZ(control, target));
}

void qulacs_circuit_add_SWAP_gate(void* circuit, uint32_t qubit0, uint32_t qubit1) {
    as_circuit(circuit)->add_gate(gate::SWAP(qubit0, qubit1));
}

// Measurement
void qulacs_circuit_add_measurement_gate(void* circuit, uint32_t qubit_index, uint32_t register_address) {
    as_circuit(circuit)->add_gate(gate::Measurement(qubit_index, register_address));
}

} // extern "C"
