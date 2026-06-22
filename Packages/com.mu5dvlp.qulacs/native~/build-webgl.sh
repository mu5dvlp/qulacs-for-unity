#!/usr/bin/env bash
# build-webgl.sh — cross-compile the Qulacs wrapper for Unity WebGL (WebAssembly).
#
# Uses the Emscripten toolchain bundled with Unity's WebGL Build Support so the
# produced static library is ABI-compatible with Unity's IL2CPP/wasm link.
#
# Output: ../Runtime/Plugins/WebGL/qulacs_unity.a — a single static archive that
# bundles the wrapper + cppsim_static + csim_static. Unity links it into the
# player wasm and C# resolves the symbols via [DllImport("__Internal")].
#
# Run from the native~/ directory:  bash build-webgl.sh
# Override the Unity install if needed:  UNITY_EDITOR="C:/.../6000.4.1f1" bash build-webgl.sh
#
# Notes:
#  - WebGL is single-threaded: Qulacs is built with USE_OMP=No and USE_SIMD=No,
#    and its hard-coded -pthread flag is patched out (see patch_pthread below) so
#    the objects link into a non-threaded Unity wasm build.
#  - The Emscripten .bat wrappers echo a couple of lines that CMake captures into
#    CMakeCache.txt; the resulting "Parse error in cache file" warnings are
#    cosmetic — the build completes and the archive is correct.

set -euo pipefail

# Resolve to a native Windows path (C:/...). The bundled emcc/llvm-ar/cmake are
# native Windows binaries and cannot open Git-Bash/MSYS paths like /c/Users/...
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
if command -v cygpath >/dev/null 2>&1; then
  SCRIPT_DIR="$(cygpath -m "$SCRIPT_DIR")"
fi

# --- Toolchains -------------------------------------------------------------
UNITY_EDITOR="${UNITY_EDITOR:-C:/Program Files/Unity/Hub/Editor/6000.4.1f1}"
WEBGL_SUPPORT="$UNITY_EDITOR/Editor/Data/PlaybackEngines/WebGLSupport/BuildTools/Emscripten"
EM="$WEBGL_SUPPORT/emscripten"
LLVM="$WEBGL_SUPPORT/llvm"
TOOLCHAIN="$EM/cmake/Modules/Platform/Emscripten.cmake"
EM_PYTHON="$WEBGL_SUPPORT/python/python.exe"
LLVM_AR="$LLVM/llvm-ar.exe"

CMAKE="${CMAKE:-C:/Program Files/Microsoft Visual Studio/2022/Community/Common7/IDE/CommonExtensions/Microsoft/CMake/CMake/bin/cmake.exe}"
NINJA="${NINJA:-C:/Program Files/Microsoft Visual Studio/2022/Community/Common7/IDE/CommonExtensions/Microsoft/CMake/Ninja/ninja.exe}"

# --- Paths ------------------------------------------------------------------
EXTERN_DIR="$SCRIPT_DIR/extern"
QULACS_SRC="$EXTERN_DIR/qulacs"
BOOST="$EXTERN_DIR/boost"
EM_WORK="$SCRIPT_DIR/build/webgl-toolchain"
EM_CONFIG_FILE="$EM_WORK/.emscripten"
QBUILD="$SCRIPT_DIR/build/qulacs-webgl"
QINSTALL="$EXTERN_DIR/qulacs-install-webgl"
BUILD_LIB="$SCRIPT_DIR/build/lib"          # Qulacs writes archives here (PROJECT_BINARY_DIR/../lib)
WRAPPER_BUILD="$SCRIPT_DIR/build/wrapper-webgl"
PLUGIN_DIR="$SCRIPT_DIR/../Runtime/Plugins/WebGL"
FINAL_A="$PLUGIN_DIR/qulacs_unity.a"

echo "======================================================"
echo " Qulacs Unity WebGL (Emscripten) Build"
echo " emscripten: $EM"
echo "======================================================"

if [ ! -f "$EM/emcc.bat" ]; then
  echo "ERROR: Emscripten not found at $EM"
  echo "       Install Unity 'WebGL Build Support' or set UNITY_EDITOR=<editor root>."
  exit 1
fi

# --- [1/6] Patch Qulacs: drop -pthread under Emscripten ---------------------
echo "[1/6] Patching Qulacs CMakeLists (skip -pthread on Emscripten)..."
if grep -q "NOT EMSCRIPTEN" "$QULACS_SRC/CMakeLists.txt"; then
  echo "      Already patched, skipping."
else
  "$EM_PYTHON" - "$QULACS_SRC/CMakeLists.txt" <<'PY'
import sys
p = sys.argv[1]
s = open(p, encoding="utf-8").read()
old = ('\t# Enable pthread\n'
       '\tset(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -pthread")\n'
       '\tset(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -pthread")')
new = ('\t# Enable pthread (skipped on Emscripten: WebGL is single-threaded and\n'
       '\t# -pthread objects are ABI-incompatible with a non-threaded Unity wasm link)\n'
       '\tif(NOT EMSCRIPTEN)\n'
       '\tset(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -pthread")\n'
       '\tset(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -pthread")\n'
       '\tendif()')
if old not in s:
    sys.exit("ERROR: could not find the -pthread block to patch in " + p)
open(p, "w", encoding="utf-8").write(s.replace(old, new))
print("      Patched.")
PY
fi

# --- [2/6] Generate the Emscripten config -----------------------------------
echo "[2/6] Writing Emscripten config -> $EM_CONFIG_FILE"
mkdir -p "$EM_WORK/cache"
cat > "$EM_CONFIG_FILE" <<EOF
LLVM_ROOT = '$LLVM'
BINARYEN_ROOT = '$WEBGL_SUPPORT/binaryen'
NODE_JS = '$WEBGL_SUPPORT/node/node.exe'
CACHE = '$EM_WORK/cache'
EOF
export EM_CONFIG="$EM_CONFIG_FILE"
export PATH="$EM:$WEBGL_SUPPORT/node:$WEBGL_SUPPORT/python:$PATH"

# --- [3/6] Cross-build Qulacs (cppsim + csim) -------------------------------
echo "[3/6] Building Qulacs (cppsim_static + csim_static) for wasm..."
"$CMAKE" -G Ninja -Wno-dev \
  -DCMAKE_MAKE_PROGRAM="$NINJA" \
  -DCMAKE_TOOLCHAIN_FILE="$TOOLCHAIN" \
  -B "$QBUILD" -S "$QULACS_SRC" \
  -DCMAKE_BUILD_TYPE=Release \
  -DCMAKE_INSTALL_PREFIX="$QINSTALL" \
  -DCMAKE_POLICY_DEFAULT_CMP0144=NEW \
  -DUSE_GPU=No -DUSE_MPI=No -DUSE_PYTHON=No -DUSE_TEST=No \
  -DUSE_SIMD=No -DUSE_OMP=No \
  -DBOOST_ROOT="$BOOST" -DBoost_ROOT="$BOOST" \
  -DBoost_INCLUDE_DIR="$BOOST" -DBoost_NO_SYSTEM_PATHS=ON
"$CMAKE" --build "$QBUILD" --target cppsim_static csim_static

echo "      Staging headers + libs -> $QINSTALL"
mkdir -p "$QINSTALL/include" "$QINSTALL/lib"
cp -r "$QULACS_SRC/src/cppsim" "$QINSTALL/include/"
cp -r "$QULACS_SRC/src/csim"   "$QINSTALL/include/"
cp -r "$QBUILD/eigen/src/eigen/Eigen" "$QINSTALL/include/"
cp "$BUILD_LIB/libcppsim_static.a" "$QINSTALL/lib/"
cp "$BUILD_LIB/libcsim_static.a"   "$QINSTALL/lib/"

# --- [4/6] Cross-build the extern "C" wrapper (static) ----------------------
# The Emscripten .bat compiler wrappers echo a couple of lines that CMake folds
# into CMakeCache.txt. A *fresh* configure tolerates this, but re-reading such a
# cache aborts with "Parse error in cache file". Start clean every time so the
# build is reproducible across re-runs.
echo "[4/6] Building qulacs_unity wrapper (static) for wasm..."
rm -rf "$WRAPPER_BUILD"
"$CMAKE" -G Ninja -Wno-dev \
  -DCMAKE_MAKE_PROGRAM="$NINJA" \
  -DCMAKE_TOOLCHAIN_FILE="$TOOLCHAIN" \
  -B "$WRAPPER_BUILD" -S "$SCRIPT_DIR" \
  -DCMAKE_BUILD_TYPE=Release \
  -DQULACS_ROOT="$QINSTALL" \
  -DCMAKE_FIND_ROOT_PATH_MODE_INCLUDE=BOTH \
  -DCMAKE_FIND_ROOT_PATH_MODE_LIBRARY=BOTH
"$CMAKE" --build "$WRAPPER_BUILD" --config Release

WRAPPER_A="$(find "$WRAPPER_BUILD" \( -name "qulacs_unity.a" -o -name "libqulacs_unity.a" \) | head -1)"
if [ -z "$WRAPPER_A" ]; then echo "ERROR: wrapper archive not found"; exit 1; fi

# --- [5/6] Merge wrapper + cppsim + csim into one archive -------------------
echo "[5/6] Merging into a single qulacs_unity.a..."
mkdir -p "$PLUGIN_DIR"
rm -f "$FINAL_A"
"$LLVM_AR" -M <<EOF
create $FINAL_A
addlib $WRAPPER_A
addlib $QINSTALL/lib/libcppsim_static.a
addlib $QINSTALL/lib/libcsim_static.a
save
end
EOF

# --- [6/6] Done -------------------------------------------------------------
echo "[6/6] Verifying exported symbols..."
DEFINED="$("$LLVM/llvm-nm.exe" "$FINAL_A" 2>/dev/null | grep -cE ' T qulacs_' || true)"
echo "      Defined qulacs_* exports: $DEFINED"
echo ""
echo "======================================================"
echo " WebGL build complete!"
echo " Plugin: $FINAL_A"
echo "======================================================"
