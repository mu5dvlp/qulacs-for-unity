cmake_minimum_required(VERSION 3.20)
project(qulacs_unity CXX)

set(CMAKE_CXX_STANDARD 14)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# ---------------------------------------------------------------------------
# Qulacs location
# QULACS_ROOT: directory containing include/cppsim/, include/Eigen/, lib/
# The Makefile sets this to native~/extern/qulacs-install automatically.
# ---------------------------------------------------------------------------
if(NOT DEFINED QULACS_ROOT)
    message(FATAL_ERROR
        "QULACS_ROOT is not set.\n"
        "Use the Makefile at the package root: 'make' to build everything,\n"
        "or pass -DQULACS_ROOT=<path/to/qulacs-install> manually.")
endif()

# ---------------------------------------------------------------------------
# Boost location (header-only, required by Qulacs headers)
# BOOST_INCLUDE_DIR: directory containing the boost/ folder
# ---------------------------------------------------------------------------
if(NOT DEFINED BOOST_INCLUDE_DIR)
    # Default: a sibling of QULACS_ROOT, i.e. native~/extern/boost
    get_filename_component(_extern_dir "${QULACS_ROOT}" DIRECTORY)
    set(BOOST_INCLUDE_DIR "${_extern_dir}/boost")
endif()

if(NOT EXISTS "${BOOST_INCLUDE_DIR}/boost/version.hpp")
    message(FATAL_ERROR
        "Boost headers not found at BOOST_INCLUDE_DIR=${BOOST_INCLUDE_DIR}\n"
        "Pass -DBOOST_INCLUDE_DIR=<path/to/boost> where boost/version.hpp exists.")
endif()

# ---------------------------------------------------------------------------
# Find Qulacs headers and libs
# ---------------------------------------------------------------------------
find_path(QULACS_INCLUDE_DIR
    NAMES cppsim/state.hpp
    PATHS "${QULACS_ROOT}/include"
    NO_DEFAULT_PATH
    REQUIRED)

find_library(QULACS_CPPSIM_LIB
    NAMES cppsim_static cppsim_static.lib
    PATHS "${QULACS_ROOT}/lib"
    NO_DEFAULT_PATH
    REQUIRED)

find_library(QULACS_CSIM_LIB
    NAMES csim_static csim_static.lib
    PATHS "${QULACS_ROOT}/lib"
    NO_DEFAULT_PATH
    REQUIRED)

# ---------------------------------------------------------------------------
# Shared library target
# ---------------------------------------------------------------------------
# iOS and WebGL (Emscripten) require a static library — Unity links it into the
# player binary. Other platforms load a shared library at runtime.
if(CMAKE_SYSTEM_NAME STREQUAL "iOS" OR EMSCRIPTEN)
    add_library(qulacs_unity STATIC
        src/qulacs_unity.cpp)
else()
    add_library(qulacs_unity SHARED
        src/qulacs_unity.cpp)
endif()

target_include_directories(qulacs_unity PRIVATE
    "${QULACS_INCLUDE_DIR}"
    "${BOOST_INCLUDE_DIR}")

target_link_libraries(qulacs_unity PRIVATE
    "${QULACS_CPPSIM_LIB}"
    "${QULACS_CSIM_LIB}")

# OpenMP (Qulacs uses it internally for parallelism)
if(ANDROID)
    # Android: link OpenMP statically to avoid libomp.so runtime dependency.
    # NDK ships libomp.a alongside libomp.so; -static-openmp selects the former.
    target_compile_options(qulacs_unity PRIVATE -fopenmp)
    target_link_options(qulacs_unity PRIVATE -fopenmp -static-openmp)
elseif(EMSCRIPTEN)
    # WebGL is single-threaded; Qulacs is built with USE_OMP=No, so the wrapper
    # must not pull in OpenMP either. Nothing to link here.
elseif(CMAKE_SYSTEM_NAME STREQUAL "Linux")
    # Linux wraps a Qulacs built with USE_OMP=No (see build-linux.sh), so no
    # OpenMP here either — this keeps the .so free of a libgomp.so runtime
    # dependency, which may be absent in minimal distros / CI docker images.
else()
    find_package(OpenMP)
    if(OpenMP_CXX_FOUND)
        target_link_libraries(qulacs_unity PRIVATE OpenMP::OpenMP_CXX)
    endif()
endif()

# Linux: statically link libstdc++/libgcc so the .so does not depend on the
# host distro's C++ runtime version (must load in the Unity editor and players
# across distros, including CI docker images).
if(CMAKE_SYSTEM_NAME STREQUAL "Linux")
    target_link_options(qulacs_unity PRIVATE -static-libstdc++ -static-libgcc)
endif()

# Android 15+ requires 16KB-aligned shared libraries
if(ANDROID)
    target_link_options(qulacs_unity PRIVATE "-Wl,-z,max-page-size=16384")
endif()

# Android and Linux require the "lib" prefix (dlopen probes libqulacs_unity.so
# for DllImport("qulacs_unity")). Other platforms strip it so Unity finds
# "qulacs_unity.dll/.dylib".
if(NOT ANDROID AND NOT CMAKE_SYSTEM_NAME STREQUAL "Linux")
    set_target_properties(qulacs_unity PROPERTIES PREFIX "")
endif()
