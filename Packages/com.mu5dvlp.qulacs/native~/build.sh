#!/usr/bin/env bash
# build.sh — Qulacs ラッパー DLL のフルビルドスクリプト
# native~/ ディレクトリで実行: bash build.sh

set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
EXTERN_DIR="$SCRIPT_DIR/extern"
QULACS_SRC="$EXTERN_DIR/qulacs"
QULACS_BUILD="$SCRIPT_DIR/build/qulacs"
QULACS_INSTALL="$EXTERN_DIR/qulacs-install"
BOOST_DIR="$EXTERN_DIR/boost"
BOOST_ZIP="$EXTERN_DIR/boost.zip"
WRAPPER_BUILD="$SCRIPT_DIR/build/wrapper"
PLUGIN_DIR="$SCRIPT_DIR/../Runtime/Plugins/Windows/x86_64"

CMAKE="C:/Program Files/Microsoft Visual Studio/2022/Community/Common7/IDE/CommonExtensions/Microsoft/CMake/CMake/bin/cmake.exe"

echo "======================================================"
echo " Qulacs Unity DLL Build Script"
echo "======================================================"

# --- [1] Boost ヘッダー展開 ---
if [ ! -d "$BOOST_DIR/boost" ]; then
  if [ -f "$BOOST_ZIP" ]; then
    echo "[1/5] Extracting Boost headers..."
    cd "$EXTERN_DIR"
    unzip -q boost.zip "boost_1_86_0/boost/*" -d boost_extracted
    mv boost_extracted/boost_1_86_0 "$BOOST_DIR"
    rm -rf boost_extracted
    echo "      Done: $BOOST_DIR"
  else
    echo "ERROR: $BOOST_ZIP not found."
    echo "       Run: curl -L -o native~/extern/boost.zip https://github.com/boostorg/boost/releases/download/boost-1.86.0/boost-1.86.0.zip"
    exit 1
  fi
else
  echo "[1/5] Boost already extracted, skipping."
fi

# --- [2] Qulacs clone ---
if [ ! -d "$QULACS_SRC/.git" ]; then
  echo "[2/5] Cloning Qulacs..."
  git clone --depth 1 https://github.com/qulacs/qulacs.git "$QULACS_SRC"
else
  echo "[2/5] Qulacs source already present, skipping."
fi

# --- [3] Qulacs ビルド & インストール ---
echo "[3/5] Building Qulacs..."
"$CMAKE" -B "$QULACS_BUILD" -S "$QULACS_SRC" \
  -DCMAKE_BUILD_TYPE=Release \
  -DCMAKE_INSTALL_PREFIX="$QULACS_INSTALL" \
  -DBOOST_ROOT="$BOOST_DIR" \
  -DBoost_NO_SYSTEM_PATHS=ON \
  -DUSE_GPU=No \
  -DUSE_MPI=No \
  -DUSE_PYTHON=No
"$CMAKE" --build "$QULACS_BUILD" --config Release
"$CMAKE" --install "$QULACS_BUILD" --config Release
echo "      Installed to: $QULACS_INSTALL"

# --- [4] ラッパー DLL ビルド ---
echo "[4/5] Building qulacs_unity.dll..."
"$CMAKE" -B "$WRAPPER_BUILD" -S "$SCRIPT_DIR" \
  -DCMAKE_BUILD_TYPE=Release \
  -DQULACS_ROOT="$QULACS_INSTALL"
"$CMAKE" --build "$WRAPPER_BUILD" --config Release

# --- [5] DLL をプラグインフォルダへ ---
echo "[5/5] Deploying DLL..."
mkdir -p "$PLUGIN_DIR"
if [ -f "$WRAPPER_BUILD/Release/qulacs_unity.dll" ]; then
  cp "$WRAPPER_BUILD/Release/qulacs_unity.dll" "$PLUGIN_DIR/qulacs_unity.dll"
elif [ -f "$WRAPPER_BUILD/qulacs_unity.dll" ]; then
  cp "$WRAPPER_BUILD/qulacs_unity.dll" "$PLUGIN_DIR/qulacs_unity.dll"
else
  echo "ERROR: qulacs_unity.dll not found after build."
  exit 1
fi

echo ""
echo "======================================================"
echo " Build complete!"
echo " DLL: $PLUGIN_DIR/qulacs_unity.dll"
echo "======================================================"
