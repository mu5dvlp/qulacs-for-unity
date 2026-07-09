# Qulacs for Unity

<p align="center">
  <img src="docs/images/hero.png" alt="qulacs-for-unity" width="300">
</p>

[![CI](https://github.com/mu5dvlp/qulacs-for-unity/actions/workflows/test.yml/badge.svg)](https://github.com/mu5dvlp/qulacs-for-unity/actions/workflows/test.yml)
[![API Coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/mu5dvlp/8a140c24ea7fedc7e83595cdd5a5ffee/raw/qulacs-for-unity-coverage.json)](https://github.com/mu5dvlp/qulacs-for-unity/actions/workflows/coverage.yml)
[![Release](https://img.shields.io/github/v/release/mu5dvlp/qulacs-for-unity?include_prereleases)](https://github.com/mu5dvlp/qulacs-for-unity/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/mu5dvlp/qulacs-for-unity/blob/main/Packages/com.mu5dvlp.qulacs/LICENSE.md)

A Unity package that brings [Qulacs](https://github.com/qulacs/qulacs) — a high-performance C++ quantum circuit simulator — into Unity projects.

## Overview

Qulacs is written in C++ and has no native C# bindings. This package provides:

1. **A thin `extern "C"` C++ wrapper** (`native~/`) around the Qulacs C++ API, compiled as a platform-specific native library.
2. **A C# API** (`Runtime/`) that calls the wrapper via P/Invoke and exposes an idiomatic Unity-friendly interface.

```
Unity C# (Mu5dvlp.Qulacs)
    └── P/Invoke
        └── qulacs_unity.dll  (extern "C" C++ wrapper)
            └── Qulacs C++ library
```

## Platform Support

| Platform | Status |
|---|---|
| Windows x86_64 | Supported |
| macOS x86_64 | Supported |
| Android ARM64 | Supported |
| Android x86_64 | Supported (emulator) |
| iOS ARM64 | Supported |
| WebGL (WebAssembly) | Supported |
| Linux x86_64 | Supported |

> WebGL runs single-threaded (no OpenMP/SIMD), so large circuits are slower than on native platforms.

## Package

**Package ID:** `com.mu5dvlp.qulacs`
**Unity:** 6000.0+ (developed on 6000.4.1f1 LTS; also confirmed on 2022.3 LTS)

### Installation

Install via Unity Package Manager using the git URL:

```
https://github.com/mu5dvlp/qulacs-for-unity.git?path=/Packages/com.mu5dvlp.qulacs#v1.0.0
```

Or add to your project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.mu5dvlp.qulacs": "https://github.com/mu5dvlp/qulacs-for-unity.git?path=/Packages/com.mu5dvlp.qulacs#v1.0.0"
  }
}
```

Replace `v1.0.0` with the desired release tag, or omit `#v1.0.0` to track `main`.
`git` must be on the PATH — Unity invokes it internally.

Alternatively, clone this repository and open the Unity project directly — the package is embedded under `Packages/com.mu5dvlp.qulacs/`.

## Building the Native Plugin

### Requirements

| Platform | Prerequisites |
|---|---|
| Windows | CMake 3.20+, Visual Studio 2022 (MSVC), `make` (via Git Bash or `choco install make`) |
| Android | Above + Unity-bundled NDK (install via Unity Hub → Android Build Support) |
| macOS | CMake 3.20+, Xcode, `brew install libomp` |
| iOS | Same as macOS (cross-compile) |
| WebGL | Windows host + Unity-bundled Emscripten (install via Unity Hub → WebGL Build Support) |
| Linux | Linux host or WSL + CMake 3.20+, g++, make |

### Build

```bash
cd Packages/com.mu5dvlp.qulacs

make build                # Windows x86_64
make build-android-all    # Android ARM64 + x86_64
make build-macos          # macOS (host architecture)
make build-ios            # iOS ARM64 (cross-compile from macOS)
make build-webgl          # WebGL/WebAssembly (Emscripten, Windows host)
make build-linux          # Linux x86_64 (Linux host or WSL)
```

See [`Packages/com.mu5dvlp.qulacs/CLAUDE.md`](Packages/com.mu5dvlp.qulacs/CLAUDE.md) for details.

## Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines on code style, branch naming, and the PR process.

**Branch strategy**: Feature/fix branches → `dev` → `main` (releases). See the [branch naming convention](CONTRIBUTING.md#branch-naming-convention) for details.

## Support

This is a community-maintained, best-effort open-source project — there is no guaranteed response time or SLA.

**GitHub Issues is the only officially tracked support channel.** Please open an issue from the [issue chooser](https://github.com/mu5dvlp/qulacs-for-unity/issues/new/choose):

- **Bug reports** — use the *Bug report* template
- **Feature requests** — use the *Feature request* template
- **Questions** — use the *Question* template

The contacts below are for general inquiries only and are not guaranteed to receive a response.

## Contact

- Personal: mu5dvlp@gmail.com
- Work: dvlpwork@gmail.com
- X: [@Yugo_dvlp](https://x.com/Yugo_dvlp)
- Qiita: [@mu5dvlp](https://qiita.com/mu5dvlp)
- Zenn: [@mu5dvlp](https://zenn.dev/mu5dvlp)

## License

The wrapper code in this repository is MIT licensed. Qulacs itself is licensed under MIT. See [Qulacs license](https://github.com/qulacs/qulacs/blob/main/LICENSE) for details.
