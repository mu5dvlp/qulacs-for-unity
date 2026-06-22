# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this package adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.1] - 2026-06-22

### Added
- **WebGL (WebAssembly) platform support**: prebuilt `qulacs_unity.a` (wasm static library) under `Runtime/Plugins/WebGL/`, cross-compiled with the Emscripten toolchain bundled with Unity's WebGL Build Support. The wrapper archive is merged with `libcppsim_static.a` / `libcsim_static.a` via `llvm-ar` so Unity links Qulacs symbols from a single self-contained plugin.
- Makefile target `make build-webgl` (delegates to `native~/build-webgl.sh`).

### Changed
- P/Invoke now binds to `"__Internal"` on WebGL as well as iOS (`(UNITY_IOS || UNITY_WEBGL) && !UNITY_EDITOR`), since both statically link the native library into the player.
- WebGL builds Qulacs with `USE_OMP=No` / `USE_SIMD=No` (WebGL is single-threaded) and patches out Qulacs' hard-coded `-pthread` flag for Emscripten ABI compatibility. Large circuits are therefore slower on WebGL than on native platforms.
- Platform support tables updated across all READMEs to list WebGL.

## [1.0.0] - 2026-05-24

First stable release.

### Added
- **macOS platform support**: prebuilt `qulacs_unity.dylib` (Apple Silicon).
- **iOS platform support**: prebuilt `qulacs_unity.a` (ARM64 static library). The wrapper archive is merged with `libcppsim_static.a` / `libcsim_static.a` via `libtool -static` so Unity picks up Qulacs symbols from a single plugin file.
- Makefile targets for macOS (`make build-macos`) and iOS (`make build-ios`).
- CI: **native symbol-sync workflow** (`.github/workflows/native-symbols.yml` + `.github/scripts/check-symbols.py`) that verifies every committed plugin binary exports every `QULACS_UNITY_API` function in `qulacs_unity.h`, and that `NativeMethods.cs` `[DllImport]` declarations stay in lockstep with the header.

### Fixed
- iOS `DllNotFoundException`: P/Invoke now binds to `"__Internal"` on iOS (`UNITY_IOS && !UNITY_EDITOR`) as Unity requires for statically linked plugins.
- iOS `Undefined symbol` link errors at Xcode build time: bundled Qulacs static archives into the iOS plugin (see *iOS platform support* above).

### Changed
- Platform support table updated across all READMEs: macOS and iOS ARM64 are now listed as supported (in addition to Windows / Android).
- Build scripts and CLAUDE.md comments normalised to English (Makefile, CMakeLists.txt, build.sh).

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
