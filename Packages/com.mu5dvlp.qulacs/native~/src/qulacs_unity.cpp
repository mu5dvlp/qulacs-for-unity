#include "qulacs_unity.h"

#include <cppsim/state.hpp>
#include <cppsim/circuit.hpp>
#include <cppsim/gate_factory.hpp>

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
    return static_cast<uint32_t>(as_state(state)->qubit_count);
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
    uint64_t dim = 1ULL << s->qubit_count;
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

void qulacs_state_set_vector(void* state, const double* real_in, const double* imag_in, uint64_t length) {
    QuantumState* s = as_state(state);
    uint64_t dim = 1ULL << s->qubit_count;
    uint64_t count = (length < dim) ? length : dim;
    std::vector<CPPCTYPE> vec(dim, CPPCTYPE(0.0, 0.0));
    for (uint64_t i = 0; i < count; ++i) {
        vec[i] = CPPCTYPE(real_in[i], imag_in[i]);
    }
    s->load(vec);
}

void qulacs_state_sampling(void* state, uint32_t count, uint64_t* results_out) {
    auto results = as_state(state)->sampling(static_cast<UINT>(count));
    for (size_t i = 0; i < results.size() && i < count; ++i) {
        results_out[i] = static_cast<uint64_t>(results[i]);
    }
}

void qulacs_state_sampling_seed(void* state, uint32_t count, uint32_t seed, uint64_t* results_out) {
    auto results = as_state(state)->sampling(static_cast<UINT>(count), static_cast<UINT>(seed));
    for (size_t i = 0; i < results.size() && i < count; ++i) {
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
    return static_cast<uint32_t>(as_circuit(circuit)->qubit_count);
}

uint32_t qulacs_circuit_get_gate_count(void* circuit) {
    return static_cast<uint32_t>(as_circuit(circuit)->gate_list.size());
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

// Additional single-qubit gates
void qulacs_circuit_add_sqrtX_gate(void* circuit, uint32_t qubit_index) {
    as_circuit(circuit)->add_gate(gate::sqrtX(qubit_index));
}

void qulacs_circuit_add_sqrtXdag_gate(void* circuit, uint32_t qubit_index) {
    as_circuit(circuit)->add_gate(gate::sqrtXdag(qubit_index));
}

void qulacs_circuit_add_sqrtY_gate(void* circuit, uint32_t qubit_index) {
    as_circuit(circuit)->add_gate(gate::sqrtY(qubit_index));
}

void qulacs_circuit_add_sqrtYdag_gate(void* circuit, uint32_t qubit_index) {
    as_circuit(circuit)->add_gate(gate::sqrtYdag(qubit_index));
}

void qulacs_circuit_add_P0_gate(void* circuit, uint32_t qubit_index) {
    as_circuit(circuit)->add_gate(gate::P0(qubit_index));
}

void qulacs_circuit_add_P1_gate(void* circuit, uint32_t qubit_index) {
    as_circuit(circuit)->add_gate(gate::P1(qubit_index));
}

// IBM-convention rotation gates
void qulacs_circuit_add_U1_gate(void* circuit, uint32_t qubit_index, double lambda) {
    as_circuit(circuit)->add_gate(gate::U1(qubit_index, lambda));
}

void qulacs_circuit_add_U2_gate(void* circuit, uint32_t qubit_index, double phi, double lambda) {
    as_circuit(circuit)->add_gate(gate::U2(qubit_index, phi, lambda));
}

void qulacs_circuit_add_U3_gate(void* circuit, uint32_t qubit_index, double theta, double phi, double lambda) {
    as_circuit(circuit)->add_gate(gate::U3(qubit_index, theta, phi, lambda));
}

// Circuit inspection
uint32_t qulacs_circuit_calculate_depth(void* circuit) {
    return static_cast<uint32_t>(as_circuit(circuit)->calculate_depth());
}

uint32_t qulacs_circuit_is_Clifford(void* circuit) {
    return as_circuit(circuit)->is_Clifford() ? 1u : 0u;
}

uint32_t qulacs_circuit_is_Gaussian(void* circuit) {
    return as_circuit(circuit)->is_Gaussian() ? 1u : 0u;
}

// Circuit mutation
void qulacs_circuit_remove_gate(void* circuit, uint32_t index) {
    as_circuit(circuit)->remove_gate(static_cast<UINT>(index));
}

void qulacs_circuit_move_gate(void* circuit, uint32_t from_index, uint32_t to_index) {
    as_circuit(circuit)->move_gate(static_cast<UINT>(from_index), static_cast<UINT>(to_index));
}

// QuantumState (additional)
double qulacs_state_get_entropy(void* state) {
    return as_state(state)->get_entropy();
}

void qulacs_state_add_state(void* state, const void* other) {
    as_state(state)->add_state(static_cast<const QuantumState*>(other));
}

void qulacs_state_multiply_coef(void* state, double coef_real, double coef_imag) {
    as_state(state)->multiply_coef(CPPCTYPE(coef_real, coef_imag));
}

double qulacs_state_get_marginal_probability(void* state, const uint32_t* measured_values, uint32_t length) {
    std::vector<UINT> mv(measured_values, measured_values + length);
    return as_state(state)->get_marginal_probability(mv);
}

// QuantumCircuit (additional)
void* qulacs_circuit_copy(void* circuit) {
    try {
        return as_circuit(circuit)->copy();
    } catch (...) {
        return nullptr;
    }
}

void* qulacs_circuit_get_inverse(void* circuit) {
    try {
        return as_circuit(circuit)->get_inverse();
    } catch (...) {
        return nullptr;
    }
}

uint32_t qulacs_circuit_to_string(void* circuit, char* buf, uint32_t buf_size) {
    std::string s = as_circuit(circuit)->to_string();
    uint32_t required = static_cast<uint32_t>(s.size() + 1);
    if (buf != nullptr && buf_size > 0) {
        uint32_t copy_len = (required < buf_size) ? required : buf_size;
        std::memcpy(buf, s.c_str(), copy_len - 1);
        buf[copy_len - 1] = '\0';
    }
    return required;
}

} // extern "C"
