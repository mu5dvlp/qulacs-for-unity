#!/usr/bin/env bash
# build-linux.sh — Linux x86_64 build for libqulacs_unity.so
#
# Run on a Linux host or WSL, from the native~/ directory:
#   bash build-linux.sh
#
# IMPORTANT — glibc floor:
#   Run this in an OLD-glibc environment (Ubuntu 20.04 / glibc 2.31). The .so
#   inherits the build host's glibc symbol versions; building on a modern host
#   (Ubuntu 24.04 / glibc 2.38 emits __isoc23_* @ GLIBC_2.38) produces a .so
#   that fails to load on the older glibc of the game-ci Unity editor docker
#   images -> DllNotFoundException at the first P/Invoke. The reproducible way
#   is `make build-linux-docker` (builds inside ubuntu:20.04). The committed
#   ceiling (GLIBC_2.31) is enforced by .github/scripts/check-symbols.py.
#
# Notes:
#  - Qulacs is built with USE_OMP=No so the resulting .so has no libgomp.so
#    runtime dependency (it must load inside arbitrary distros, including the
#    game-ci docker images used for CI test runs).
#  - libstdc++/libgcc are linked statically for the same reason (see the Linux
#    branch in CMakeLists.txt).
#  - Boost headers and the Qulacs source in native~/extern/ are shared with the
#    other platform builds; build/install directories are Linux-specific.

set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
EXTERN_DIR="$SCRIPT_DIR/extern"
QULACS_SRC="$EXTERN_DIR/qulacs"
QULACS_BUILD="$SCRIPT_DIR/build/qulacs-linux"
QULACS_INSTALL="$EXTERN_DIR/qulacs-install-linux"
BOOST_DIR="$EXTERN_DIR/boost"
BOOST_ZIP="$EXTERN_DIR/boost.zip"
WRAPPER_BUILD="$SCRIPT_DIR/build/wrapper-linux"
PLUGIN_DIR="$SCRIPT_DIR/../Runtime/Plugins/Linux/x86_64"
JOBS="$(nproc 2>/dev/null || echo 4)"

echo "======================================================"
echo " Qulacs Unity Linux x86_64 Build Script"
echo "======================================================"

# --- [1] Extract Boost headers ---
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

# --- [2] Clone Qulacs ---
if [ ! -d "$QULACS_SRC/.git" ]; then
  echo "[2/5] Cloning Qulacs..."
  git clone --depth 1 https://github.com/qulacs/qulacs.git "$QULACS_SRC"
else
  echo "[2/5] Qulacs source already present, skipping."
fi

# --- [3] Build Qulacs (static libs, PIC, no OpenMP) & stage headers/libs ---
# Qulacs' CMake does not produce the expected layout from its install target, so
# stage the headers/libs manually like the other platforms in the Makefile. The
# static libs are emitted to native~/build/lib/ by Qulacs' own configuration.
echo "[3/5] Building Qulacs..."
cmake -B "$QULACS_BUILD" -S "$QULACS_SRC" \
  -DCMAKE_BUILD_TYPE=Release \
  -DCMAKE_INSTALL_PREFIX="$QULACS_INSTALL" \
  -DCMAKE_POSITION_INDEPENDENT_CODE=ON \
  -DBOOST_ROOT="$BOOST_DIR" \
  -DBoost_NO_SYSTEM_PATHS=ON \
  -DUSE_GPU=No \
  -DUSE_MPI=No \
  -DUSE_PYTHON=No \
  -DUSE_OMP=No
cmake --build "$QULACS_BUILD" --config Release --parallel "$JOBS"
echo "      Staging headers and libs to $QULACS_INSTALL..."
mkdir -p "$QULACS_INSTALL/include" "$QULACS_INSTALL/lib"
cp -r "$QULACS_SRC/src/cppsim" "$QULACS_INSTALL/include/"
cp -r "$QULACS_SRC/src/csim" "$QULACS_INSTALL/include/"
cp -r "$QULACS_BUILD/eigen/src/eigen/Eigen" "$QULACS_INSTALL/include/"
cp "$SCRIPT_DIR/build/lib/libcppsim_static.a" "$QULACS_INSTALL/lib/"
cp "$SCRIPT_DIR/build/lib/libcsim_static.a" "$QULACS_INSTALL/lib/"

# --- [4] Build the wrapper shared library ---
echo "[4/5] Building libqulacs_unity.so..."
cmake -B "$WRAPPER_BUILD" -S "$SCRIPT_DIR" \
  -DCMAKE_BUILD_TYPE=Release \
  -DCMAKE_POSITION_INDEPENDENT_CODE=ON \
  -DQULACS_ROOT="$QULACS_INSTALL"
cmake --build "$WRAPPER_BUILD" --config Release --parallel "$JOBS"

# --- [5] Deploy ---
echo "[5/5] Deploying libqulacs_unity.so..."
mkdir -p "$PLUGIN_DIR"
if [ -f "$WRAPPER_BUILD/libqulacs_unity.so" ]; then
  cp "$WRAPPER_BUILD/libqulacs_unity.so" "$PLUGIN_DIR/libqulacs_unity.so"
else
  echo "ERROR: libqulacs_unity.so not found after build."
  exit 1
fi

echo ""
echo "======================================================"
echo " Build complete!"
echo " SO: $PLUGIN_DIR/libqulacs_unity.so"
echo "======================================================"
