# Qulacs for Unity

[![CI](https://github.com/mu5dvlp/qulacs-for-unity/actions/workflows/test.yml/badge.svg)](https://github.com/mu5dvlp/qulacs-for-unity/actions/workflows/test.yml)
[![Release](https://img.shields.io/github/v/release/mu5dvlp/qulacs-for-unity)](https://github.com/mu5dvlp/qulacs-for-unity/releases)
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
| Android ARM64 | Supported |
| Android x86_64 | Supported (emulator) |
| macOS | Planned |
| iOS | Planned |

## Package

**Package ID:** `com.mu5dvlp.qulacs`
**Unity:** 6000.0+ (developed on 6000.4.1f1 LTS; also confirmed on 2022.3 LTS)

### Installation

Install via Unity Package Manager using the git URL:

```
https://github.com/mu5dvlp/qulacs-for-unity.git?path=/Packages/com.mu5dvlp.qulacs#v0.1.0
```

Or add to your project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.mu5dvlp.qulacs": "https://github.com/mu5dvlp/qulacs-for-unity.git?path=/Packages/com.mu5dvlp.qulacs#v0.1.0"
  }
}
```

Replace `v0.1.0` with the desired release tag, or omit `#v0.1.0` to track `main`.
`git` must be on the PATH — Unity invokes it internally.

Alternatively, clone this repository and open the Unity project directly — the package is embedded under `Packages/com.mu5dvlp.qulacs/`.

## Building the Native Plugin

### Requirements

- CMake 3.20+
- MSVC (Visual Studio 2022) — Windows
- Qulacs built from source (see [Qulacs build instructions](https://github.com/qulacs/qulacs))

### Build (Windows)

```bash
cd Packages/com.mu5dvlp.qulacs/native~
cmake -B build -DCMAKE_BUILD_TYPE=Release
cmake --build build --config Release
```

The output `qulacs_unity.dll` should be copied to `Runtime/Plugins/Windows/x86_64/`.

## Contact

- Personal: mu5dvlp@gmail.com
- Work: dvlpwork@gmail.com
- X: [@Yugo_dvlp](https://x.com/Yugo_dvlp)
- Qiita: [@mu5dvlp](https://qiita.com/mu5dvlp)
- Zenn: [@mu5dvlp](https://zenn.dev/mu5dvlp)

## License

The wrapper code in this repository is MIT licensed. Qulacs itself is licensed under MIT. See [Qulacs license](https://github.com/qulacs/qulacs/blob/main/LICENSE) for details.
