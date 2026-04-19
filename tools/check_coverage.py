#!/usr/bin/env python3
"""
Check qulacs_unity C wrapper coverage against the Qulacs C++ public API.

Parses three Qulacs headers (gate_factory.hpp, state.hpp, circuit.hpp) and
compares their public symbols against qulacs_unity.h.

Usage (local – requires 'make fetch-qulacs' first):
  python tools/check_coverage.py

Usage (CI – supply paths explicitly):
  python tools/check_coverage.py --qulacs-src /path/to/qulacs/src/cppsim
"""
import re
import sys
import argparse
from pathlib import Path
from dataclasses import dataclass
from typing import Optional

REPO_ROOT = Path(__file__).resolve().parent.parent
DEFAULT_QULACS_SRC = (
    REPO_ROOT / "Packages/com.mu5dvlp.qulacs/native~/extern/qulacs/src/cppsim"
)
DEFAULT_WRAPPER_HEADER = (
    REPO_ROOT / "Packages/com.mu5dvlp.qulacs/native~/src/qulacs_unity.h"
)

# Explicit overrides for auto-derived wrapper names.
#   None => intentionally out of scope (not counted)
#   str  => use this wrapper name instead of the auto-derived one
OVERRIDES: dict[str, Optional[str]] = {
    # --- gate_factory: out of scope ---
    "gate::create_quantum_gate_from_string": None,  # string-parse API
    "gate::RotInvX": None,   # alias for RX (opposite sign)
    "gate::RotInvY": None,
    "gate::RotInvZ": None,
    "gate::RotX": None,      # alias for RX (same sign)
    "gate::RotY": None,
    "gate::RotZ": None,
    "gate::FusedSWAP": None,         # MPI-specific
    "gate::RandomUnitary": None,
    "gate::ReversibleBoolean": None,
    "gate::StateReflection": None,
    "gate::LinearCombination": None,
    "gate::BitFlipNoise": None,
    "gate::DephasingNoise": None,
    "gate::IndependentXZNoise": None,
    "gate::DepolarizingNoise": None,
    "gate::TwoQubitDepolarizingNoise": None,
    "gate::AmplitudeDampingNoise": None,
    "gate::NoisyEvolution": None,
    "gate::NoisyEvolution_auto": None,
    "gate::NoisyEvolution_fast": None,
    "gate::from_ptree": None,            # serialization
    "gate::MultiQubitPauliMeasurement": None,
    # --- gate_factory: non-obvious name mapping ---
    "gate::Measurement": "qulacs_circuit_add_measurement_gate",
    # --- QuantumState: out of scope (internals / pointer return types) ---
    "QuantumState::set_zero_norm_state": None,
    "QuantumState::get_squared_norm_single_thread": None,
    "QuantumState::normalize": None,
    "QuantumState::normalize_single_thread": None,
    "QuantumState::allocate_buffer": None,
    "QuantumState::copy": None,
    "QuantumState::load": None,
    "QuantumState::data": None,
    "QuantumState::data_cpp": None,
    "QuantumState::data_c": None,
    "QuantumState::duplicate_data_cpp": None,
    "QuantumState::duplicate_data_c": None,
    "QuantumState::add_state_with_coef": None,
    "QuantumState::add_state_with_coef_single_thread": None,
    "QuantumState::multiply_elementwise_function": None,  # function-pointer arg
    "QuantumState::to_ptree": None,
    # --- QuantumState: case mismatch with our wrapper ---
    "QuantumState::set_Haar_random_state": "qulacs_state_set_haar_random_state",
    # --- QuantumCircuit: out of scope (raw pointer API) ---
    "QuantumCircuit::add_gate": None,
    "QuantumCircuit::add_gate_copy": None,
    "QuantumCircuit::add_noise_gate": None,
    "QuantumCircuit::add_noise_gate_copy": None,
    "QuantumCircuit::add_multi_Pauli_gate": None,
    "QuantumCircuit::add_multi_Pauli_rotation_gate": None,
    "QuantumCircuit::add_diagonal_observable_rotation_gate": None,
    "QuantumCircuit::add_observable_rotation_gate": None,
    "QuantumCircuit::add_dense_matrix_gate": None,
    "QuantumCircuit::merge_circuit": None,
    "QuantumCircuit::to_ptree": None,
    # --- QuantumCircuit: rotation aliases ---
    "QuantumCircuit::add_RotInvX_gate": None,
    "QuantumCircuit::add_RotInvY_gate": None,
    "QuantumCircuit::add_RotInvZ_gate": None,
    "QuantumCircuit::add_RotX_gate": None,
    "QuantumCircuit::add_RotY_gate": None,
    "QuantumCircuit::add_RotZ_gate": None,
    "QuantumCircuit::add_FusedSWAP_gate": None,     # MPI-specific
    "QuantumCircuit::add_random_unitary_gate": None, # raw pointer arg
}


@dataclass
class ApiEntry:
    symbol: str                    # e.g. "gate::H", "QuantumState::set_zero_state"
    expected_wrapper: Optional[str]  # None = out of scope
    covered: bool = False


def _resolve(symbol: str, derived: str) -> Optional[str]:
    return OVERRIDES[symbol] if symbol in OVERRIDES else derived


def parse_wrapped_functions(header: Path) -> set[str]:
    text = header.read_text(encoding="utf-8")
    return set(re.findall(r"QULACS_UNITY_API\s+\S+\s+(\w+)\s*\(", text))


def parse_gate_factory(src: Path) -> list[ApiEntry]:
    text = (src / "gate_factory.hpp").read_text(encoding="utf-8")
    names = re.findall(r"^DllExport\s+\S+\s+(\w+)\s*\(", text, re.MULTILINE)
    seen: set[str] = set()
    entries = []
    for name in names:
        if name in seen:
            continue
        seen.add(name)
        symbol = f"gate::{name}"
        entries.append(
            ApiEntry(symbol=symbol, expected_wrapper=_resolve(symbol, f"qulacs_circuit_add_{name}_gate"))
        )
    return entries


def parse_state_api(src: Path) -> list[ApiEntry]:
    text = (src / "state.hpp").read_text(encoding="utf-8")
    pure_virtual = re.findall(
        r"virtual\s+\S+\s+(\w+)\s*\([^)]*\)\s*(?:const\s*)?=\s*0", text
    )
    sampling = re.findall(r"virtual\s+\S+\s+(sampling)\s*\(", text)
    seen: set[str] = set()
    entries = []
    for m in pure_virtual + sampling:
        if m in seen:
            continue
        seen.add(m)
        symbol = f"QuantumState::{m}"
        entries.append(
            ApiEntry(symbol=symbol, expected_wrapper=_resolve(symbol, f"qulacs_state_{m}"))
        )
    return entries


def parse_circuit_api(src: Path) -> list[ApiEntry]:
    text = (src / "circuit.hpp").read_text(encoding="utf-8")
    methods = re.findall(r"^\s+(?:virtual\s+)?\S+\s+(\w+)\s*\(", text, re.MULTILINE)
    skip = {"QuantumCircuit", "operator"}
    seen: set[str] = set()
    entries = []
    for m in methods:
        if m in seen or m in skip or m.startswith("_") or (m[0].isupper() and m not in ("is_Clifford", "is_Gaussian")):
            continue
        seen.add(m)
        symbol = f"QuantumCircuit::{m}"
        entries.append(
            ApiEntry(symbol=symbol, expected_wrapper=_resolve(symbol, f"qulacs_circuit_{m}"))
        )
    return entries


def check_coverage(entries: list[ApiEntry], wrapped: set[str]) -> None:
    for e in entries:
        if e.expected_wrapper and e.expected_wrapper in wrapped:
            e.covered = True


def print_report(entries: list[ApiEntry], out=sys.stdout) -> None:
    in_scope = [e for e in entries if e.expected_wrapper is not None]
    out_scope = [e for e in entries if e.expected_wrapper is None]
    covered = [e for e in in_scope if e.covered]
    uncovered = [e for e in in_scope if not e.covered]
    pct = len(covered) / len(in_scope) * 100 if in_scope else 0.0

    def w(line=""):
        print(line, file=out)

    w("=" * 64)
    w("  Qulacs → qulacs_unity wrapper coverage")
    w("=" * 64)
    w(f"  In-scope:      {len(in_scope):3d}")
    w(f"  Covered:       {len(covered):3d}  ({pct:.1f}%)")
    w(f"  Uncovered:     {len(uncovered):3d}  ← gaps to fill")
    w(f"  Out-of-scope:  {len(out_scope):3d}  (skipped)")
    w()

    if uncovered:
        w("--- Uncovered (gaps) ---")
        for e in sorted(uncovered, key=lambda x: x.symbol):
            w(f"  MISS  {e.symbol}")
        w()

    w("--- Covered ---")
    for e in sorted(covered, key=lambda x: x.symbol):
        w(f"  OK    {e.symbol}")

    w("=" * 64)


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--qulacs-src", default=str(DEFAULT_QULACS_SRC))
    parser.add_argument("--wrapper-header", default=str(DEFAULT_WRAPPER_HEADER))
    parser.add_argument("--report", metavar="FILE", help="Also write report to FILE")
    parser.add_argument(
        "--min-coverage",
        type=float,
        metavar="PCT",
        help="Exit 1 if coverage is below this percentage (0–100)",
    )
    args = parser.parse_args()

    src = Path(args.qulacs_src)
    wrapper_header = Path(args.wrapper_header)

    if not src.exists():
        print(f"ERROR: Qulacs src not found: {src}", file=sys.stderr)
        print(
            "  Run from Packages/com.mu5dvlp.qulacs/:  make fetch-qulacs",
            file=sys.stderr,
        )
        sys.exit(2)

    wrapped = parse_wrapped_functions(wrapper_header)
    entries: list[ApiEntry] = (
        parse_gate_factory(src) + parse_state_api(src) + parse_circuit_api(src)
    )
    check_coverage(entries, wrapped)

    print_report(entries)

    if args.report:
        with open(args.report, "w", encoding="utf-8") as f:
            print_report(entries, out=f)

    if args.min_coverage is not None:
        in_scope = [e for e in entries if e.expected_wrapper is not None]
        covered = [e for e in in_scope if e.covered]
        pct = len(covered) / len(in_scope) * 100 if in_scope else 0.0
        if pct < args.min_coverage:
            print(
                f"\nFAIL: coverage {pct:.1f}% is below required {args.min_coverage}%",
                file=sys.stderr,
            )
            sys.exit(1)


if __name__ == "__main__":
    main()
