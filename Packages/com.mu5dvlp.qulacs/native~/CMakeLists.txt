cmake_minimum_required(VERSION 3.20)
project(qulacs_unity CXX)

set(CMAKE_CXX_STANDARD 14)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# ---------------------------------------------------------------------------
# Qulacs location
# QULACS_ROOT should point to the CMake install prefix used when building
# Qulacs (i.e. the directory containing include/cppsim/ and lib/).
# The Makefile sets this to native~/extern/qulacs-install automatically.
# ---------------------------------------------------------------------------
if(NOT DEFINED QULACS_ROOT)
    message(FATAL_ERROR
        "QULACS_ROOT is not set.\n"
        "Use the Makefile at the package root: 'make' to build everything,\n"
        "or pass -DQULACS_ROOT=<path/to/qulacs-install> manually.")
endif()

find_path(QULACS_INCLUDE_DIR
    NAMES cppsim/state.hpp
    PATHS
        "${QULACS_ROOT}/include"
        "${QULACS_ROOT}/src"         # fallback: in-source build
    NO_DEFAULT_PATH
    REQUIRED)

find_library(QULACS_CPPSIM_LIB
    NAMES cppsim_static cppsim cppsim_static.lib
    PATHS
        "${QULACS_ROOT}/lib"
        "${QULACS_ROOT}/lib/Release"
    NO_DEFAULT_PATH
    REQUIRED)

find_library(QULACS_CSIM_LIB
    NAMES csim_static csim csim_static.lib
    PATHS
        "${QULACS_ROOT}/lib"
        "${QULACS_ROOT}/lib/Release"
    NO_DEFAULT_PATH
    REQUIRED)

# ---------------------------------------------------------------------------
# Shared library target
# ---------------------------------------------------------------------------
add_library(qulacs_unity SHARED
    src/qulacs_unity.cpp)

target_include_directories(qulacs_unity PRIVATE
    "${QULACS_INCLUDE_DIR}")

target_link_libraries(qulacs_unity PRIVATE
    "${QULACS_CPPSIM_LIB}"
    "${QULACS_CSIM_LIB}")

# OpenMP (Qulacs uses it internally for parallelism)
find_package(OpenMP)
if(OpenMP_CXX_FOUND)
    target_link_libraries(qulacs_unity PRIVATE OpenMP::OpenMP_CXX)
endif()

# Strip "lib" prefix on all platforms so Unity finds "qulacs_unity.dll/.so/.dylib"
set_target_properties(qulacs_unity PROPERTIES PREFIX "")
