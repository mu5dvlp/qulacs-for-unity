# qulacs-for-unity

[![CI](https://github.com/mu5dvlp/qulacs-for-unity/actions/workflows/test.yml/badge.svg)](https://github.com/mu5dvlp/qulacs-for-unity/actions/workflows/test.yml)
[![Release](https://img.shields.io/github/v/release/mu5dvlp/qulacs-for-unity)](https://github.com/mu5dvlp/qulacs-for-unity/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/mu5dvlp/qulacs-for-unity/blob/main/Packages/com.mu5dvlp.qulacs/LICENSE.md)

[Qulacs](https://github.com/qulacs/qulacs) — 高性能な C++ 量子回路シミュレータ — を Unity プロジェクトで利用するための Unity パッケージです。

## 概要

Qulacs は C++ で実装されており、C# のネイティブバインディングは存在しません。このパッケージは以下を提供します。

1. **薄い `extern "C"` C++ ラッパー** (`native~/`) — Qulacs の C++ API をラップし、プラットフォーム固有のネイティブライブラリとしてコンパイルします。
2. **C# API** (`Runtime/`) — ラッパーを P/Invoke 経由で呼び出し、Unity に馴染みやすいインターフェースを公開します。

```
Unity C# (Mu5dvlp.Qulacs)
    └── P/Invoke
        └── qulacs_unity.dll  (extern "C" C++ ラッパー)
            └── Qulacs C++ ライブラリ
```

## プラットフォームサポート

| プラットフォーム | 状態 |
|---|---|
| Windows x86_64 | 対応済み |
| Android ARM64 | 対応済み |
| Android x86_64 | 対応済み (エミュレータ) |
| macOS | 予定 |
| iOS | 予定 |

## パッケージ

**パッケージ ID:** `com.mu5dvlp.qulacs`
**Unity:** 6000.0+ (6000.4.1f1 LTS で開発; 2022.3 LTS でも動作確認済み)

### インストール

Unity Package Manager の git URL でインストールできます。

```
https://github.com/mu5dvlp/qulacs-for-unity.git?path=/Packages/com.mu5dvlp.qulacs#v0.1.0
```

または、プロジェクトの `Packages/manifest.json` に追記:

```json
{
  "dependencies": {
    "com.mu5dvlp.qulacs": "https://github.com/mu5dvlp/qulacs-for-unity.git?path=/Packages/com.mu5dvlp.qulacs#v0.1.0"
  }
}
```

`v0.1.0` を任意のリリースタグに置き換えてください。`#v0.1.0` を省略すると `main` を追跡します。
`git` コマンドが PATH に通っている必要があります (Unity が内部的に呼び出します)。

あるいはこのリポジトリをクローンして Unity プロジェクトを直接開くことも可能です。パッケージは `Packages/com.mu5dvlp.qulacs/` に埋め込まれています。

## ネイティブプラグインのビルド

### 要件

- CMake 3.20 以上
- MSVC (Visual Studio 2022) — Windows の場合
- ソースからビルドした Qulacs ([Qulacs ビルド手順](https://github.com/qulacs/qulacs) を参照)

### ビルド (Windows)

```bash
cd Packages/com.mu5dvlp.qulacs/native~
cmake -B build -DCMAKE_BUILD_TYPE=Release
cmake --build build --config Release
```

生成された `qulacs_unity.dll` を `Runtime/Plugins/Windows/x86_64/` にコピーしてください。

## 連絡先

- 個人用: mu5dvlp@gmail.com
- 仕事用: dvlpwork@gmail.com
- X: [@Yugo_dvlp](https://x.com/Yugo_dvlp)
- Qiita: [@mu5dvlp](https://qiita.com/mu5dvlp)
- Zenn: [@mu5dvlp](https://zenn.dev/mu5dvlp)

## ライセンス

このリポジトリのラッパーコードは MIT ライセンスです。Qulacs 本体も MIT ライセンスです。詳細は [Qulacs ライセンス](https://github.com/qulacs/qulacs/blob/main/LICENSE) を参照してください。
