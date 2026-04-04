cmake_minimum_required(VERSION 3.20)
project(qulacs_unity CXX)

set(CMAKE_CXX_STANDARD 14)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# ---------------------------------------------------------------------------
# Qulacs location
# Set QULACS_ROOT to the directory that contains include/ and lib/ from
# a Qulacs build (e.g. cmake -DQULACS_ROOT=C:/qulacs/build ..)
# ---------------------------------------------------------------------------
if(NOT DEFINED QULACS_ROOT)
    message(FATAL_ERROR
        "QULACS_ROOT is not set.\n"
        "Run cmake with -DQULACS_ROOT=<path> pointing to the Qulacs build directory.")
endif()

find_path(QULACS_INCLUDE_DIR
    NAMES cppsim/state.hpp
    PATHS "${QULACS_ROOT}/include"
    NO_DEFAULT_PATH
    REQUIRED)

find_library(QULACS_CPPSIM_LIB
    NAMES cppsim_static cppsim
    PATHS "${QULACS_ROOT}/lib" "${QULACS_ROOT}/build/lib"
    NO_DEFAULT_PATH
    REQUIRED)

find_library(QULACS_CSIM_LIB
    NAMES csim_static csim
    PATHS "${QULACS_ROOT}/lib" "${QULACS_ROOT}/build/lib"
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

# OpenMP (optional — Qulacs uses it internally)
find_package(OpenMP)
if(OpenMP_CXX_FOUND)
    target_link_libraries(qulacs_unity PRIVATE OpenMP::OpenMP_CXX)
endif()

# Windows: strip "lib" prefix so Unity finds "qulacs_unity.dll"
set_target_properties(qulacs_unity PROPERTIES PREFIX "")

# ---------------------------------------------------------------------------
# Install — copy DLL to the Unity plugin folder
# ---------------------------------------------------------------------------
set(UNITY_PLUGIN_DIR
    "${CMAKE_CURRENT_SOURCE_DIR}/../Runtime/Plugins/Windows/x86_64"
    CACHE PATH "Destination for the compiled Unity plugin")

install(TARGETS qulacs_unity
    RUNTIME DESTINATION "${UNITY_PLUGIN_DIR}")
