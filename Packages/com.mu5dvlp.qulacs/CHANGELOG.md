# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this package adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.2.0] - 2026-05-12

### Added
- **Android platform support**: ARM64 and x86_64 prebuilt native plugins (`libqulacs_unity.so`).
- **Demo Scenes sample**: BellState, Bloch sphere visualiser, inspector-driven quantum circuit, qubit colouring — importable via Package Manager.
- **Circuit Composer sample**: interactive 2D quantum circuit composer with drag-and-place gates and real-time probability display.
- Makefile targets for Android cross-builds (`make build-android`, `make build-android-x86_64`, `make build-android-all`).
- Root `LICENSE` file for repository-level license visibility.
- Troubleshooting section in package README.

### Fixed
- Android `DllNotFoundException`: OpenMP is now statically linked (`-static-openmp`) to eliminate `libomp.so` runtime dependency.
- Android `dlopen` failure: `.so` files renamed to `libqulacs_unity.so` (Android requires the `lib` prefix).
- CI test runner reporting 0/0 tests due to unavailable Unity module packages (`adaptiveperformance`, `vectorgraphics`) in Docker image.

### Changed
- Platform support table updated: Android ARM64 and x86_64 are now listed as supported across all READMEs.
- Unity version requirements clarified: 6000.0+ (developed on 6000.4.1f1 LTS; also confirmed on 2022.3 LTS).
- Native API headers: added memory ownership and buffer safety documentation.

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
