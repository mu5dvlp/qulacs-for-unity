# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this package adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0] - 2026-05-06

### Added
- Initial release as a Unity Package Manager package.
- `QuantumState` — state vector container with zero/computational-basis/Haar-random initialization, state vector I/O, squared norm, zero-probability, and sampling.
- `QuantumCircuit` — fluent circuit builder with single-qubit gates (`H`, `X`, `Y`, `Z`, `S`, `Sdag`, `T`, `Tdag`, `Identity`), rotation gates (`RX`, `RY`, `RZ`), two-qubit gates (`CNOT`, `CZ`, `SWAP`), and `Measure`.
- Windows x86_64 prebuilt native plugin (`qulacs_unity.dll`).
- Documentation: API reference under `docs/api-reference.md`.

### Notes
- Rotation gates follow the Qulacs convention `R{X,Y,Z}(θ) = exp(+iθP/2)`, opposite sign from the standard physics convention.
- macOS, Android ARM64, and iOS binaries are planned but not yet shipped.
