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
    # デフォルト: QULACS_ROOT の兄弟ディレクトリ native~/extern/boost
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
add_library(qulacs_unity SHARED
    src/qulacs_unity.cpp)

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
else()
    find_package(OpenMP)
    if(OpenMP_CXX_FOUND)
        target_link_libraries(qulacs_unity PRIVATE OpenMP::OpenMP_CXX)
    endif()
endif()

# Android 15+ requires 16KB-aligned shared libraries
if(ANDROID)
    target_link_options(qulacs_unity PRIVATE "-Wl,-z,max-page-size=16384")
endif()

# Android requires the "lib" prefix (dlopen looks for libqulacs_unity.so).
# Other platforms strip it so Unity finds "qulacs_unity.dll/.dylib".
if(NOT ANDROID)
    set_target_properties(qulacs_unity PROPERTIES PREFIX "")
endif()
