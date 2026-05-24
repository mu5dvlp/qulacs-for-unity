# =============================================================================
# Makefile for com.mu5dvlp.qulacs
#
# Prerequisites (Windows):
#   - Git
#   - CMake 3.20+
#   - Visual Studio 2022 (MSVC) with "Desktop development with C++" workload
#   - make (bundled with Git Bash, or `choco install make`)
#   - Android: NDK bundled with Unity (install via "Android Build Support")
#
# Prerequisites (macOS):
#   - Git
#   - CMake 3.20+
#   - Xcode (Command Line Tools)
#   - OpenMP: brew install libomp
#
# Commands:
#   make build                # Windows x86_64 full build
#   make build-android        # Android ARM64 full build
#   make build-android-x86_64 # Android x86_64 full build (emulator)
#   make build-android-all    # Android ARM64 + x86_64 full build
#   make build-macos          # macOS full build (host architecture)
#   make build-ios            # iOS ARM64 full build (cross-compile)
#   make fetch-qulacs         # git clone the Qulacs source
#   make fetch-boost          # download Boost headers
#   make clean-build          # remove build artifacts (native~/build/)
#   make clean-all            # remove build artifacts + Qulacs source & install dir
# =============================================================================

# ---------------------------------------------------------------------------
# CMake detection (cross-platform)
# ---------------------------------------------------------------------------
ifeq ($(OS),Windows_NT)
CMAKE := $(shell where cmake 2>/dev/null | head -1)
ifeq ($(CMAKE),)
CMAKE := C:/Program Files/Microsoft Visual Studio/2022/Community/Common7/IDE/CommonExtensions/Microsoft/CMake/CMake/bin/cmake.exe
endif
else
CMAKE := $(shell which cmake 2>/dev/null)
endif

QULACS_REPO    := https://github.com/qulacs/qulacs.git
QULACS_SRC     := native~/extern/qulacs
QULACS_BUILD   := native~/build/qulacs
QULACS_INSTALL := native~/extern/qulacs-install
BOOST_ROOT     := native~/extern/boost
WRAPPER_SRC    := native~
WRAPPER_BUILD  := native~/build/wrapper
PLUGIN_DIR     := Runtime/Plugins/Windows/x86_64
DLL            := qulacs_unity.dll

# ---------------------------------------------------------------------------
# Android common settings
# ---------------------------------------------------------------------------
ANDROID_NDK            := C:/Program Files/Unity/Hub/Editor/6000.4.1f1/Editor/Data/PlaybackEngines/AndroidPlayer/NDK
ANDROID_TOOLCHAIN      := $(ANDROID_NDK)/build/cmake/android.toolchain.cmake
NINJA                  := C:/Program Files/Microsoft Visual Studio/2022/Community/Common7/IDE/CommonExtensions/Microsoft/CMake/Ninja/ninja.exe
ANDROID_PLATFORM       := android-24
SO                     := libqulacs_unity.so

# ---------------------------------------------------------------------------
# Android ARM64 settings
# ---------------------------------------------------------------------------
ANDROID_ABI            := arm64-v8a
QULACS_BUILD_ANDROID   := native~/build/qulacs-android
QULACS_INSTALL_ANDROID := native~/extern/qulacs-install-android
WRAPPER_BUILD_ANDROID  := native~/build/wrapper-android
PLUGIN_DIR_ANDROID     := Runtime/Plugins/Android/ARM64

# ---------------------------------------------------------------------------
# Android x86_64 settings (for emulator)
# ---------------------------------------------------------------------------
ANDROID_ABI_X86_64            := x86_64
QULACS_BUILD_ANDROID_X86_64   := native~/build/qulacs-android-x86_64
QULACS_INSTALL_ANDROID_X86_64 := native~/extern/qulacs-install-android-x86_64
WRAPPER_BUILD_ANDROID_X86_64  := native~/build/wrapper-android-x86_64
PLUGIN_DIR_ANDROID_X86_64     := Runtime/Plugins/Android/x86_64

# ---------------------------------------------------------------------------
# macOS settings
# ---------------------------------------------------------------------------
QULACS_BUILD_MACOS   := native~/build/qulacs-macos
QULACS_INSTALL_MACOS := native~/extern/qulacs-install-macos
WRAPPER_BUILD_MACOS  := native~/build/wrapper-macos
PLUGIN_DIR_MACOS     := Runtime/Plugins/macOS
DYLIB                := qulacs_unity.dylib

# Homebrew libomp (Apple Clang does not bundle OpenMP)
LIBOMP_PREFIX := $(shell brew --prefix libomp 2>/dev/null)

# ---------------------------------------------------------------------------
# iOS settings
# ---------------------------------------------------------------------------
QULACS_BUILD_IOS   := native~/build/qulacs-ios
QULACS_INSTALL_IOS := native~/extern/qulacs-install-ios
WRAPPER_BUILD_IOS  := native~/build/wrapper-ios
PLUGIN_DIR_IOS     := Runtime/Plugins/iOS
IOS_STATIC_LIB     := qulacs_unity.a
IOS_DEPLOYMENT_TARGET := 13.0

.PHONY: build fetch-qulacs fetch-boost build-qulacs build-dll deploy-dll \
        build-android build-qulacs-android build-dll-android deploy-dll-android \
        build-android-x86_64 build-qulacs-android-x86_64 build-dll-android-x86_64 deploy-dll-android-x86_64 \
        build-android-all \
        build-macos build-qulacs-macos build-dll-macos deploy-dll-macos \
        build-ios build-qulacs-ios build-dll-ios deploy-dll-ios \
        clean-build clean-all

build: fetch-qulacs build-qulacs build-dll deploy-dll

# -----------------------------------------------------------------------------
# Fetch the Qulacs source
# -----------------------------------------------------------------------------
fetch-qulacs:
	@echo ">>> [1/4] Fetching Qulacs source..."
	@if [ ! -d "$(QULACS_SRC)/.git" ]; then \
		git clone --depth 1 $(QULACS_REPO) $(QULACS_SRC); \
	else \
		echo "    Qulacs source already present, skipping clone."; \
	fi

# -----------------------------------------------------------------------------
# Download and extract Boost headers
# -----------------------------------------------------------------------------
fetch-boost:
	@echo ">>> Fetching Boost headers..."
	@if [ ! -d "$(BOOST_ROOT)/boost" ]; then \
		mkdir -p native~/extern && \
		curl -L -o native~/extern/boost.zip \
			https://archives.boost.io/release/1.86.0/source/boost_1_86_0.zip && \
		cd native~/extern && \
		unzip -q boost.zip "boost_1_86_0/boost/*" -d boost_extracted && \
		mv boost_extracted/boost_1_86_0 boost && \
		rm -rf boost_extracted boost.zip; \
	else \
		echo "    Boost headers already present, skipping."; \
	fi

# -----------------------------------------------------------------------------
# Build Qulacs and install include/ and lib/
# -----------------------------------------------------------------------------
build-qulacs:
	@echo ">>> [2/4] Building Qulacs..."
	"$(CMAKE)" -B $(QULACS_BUILD) -S $(QULACS_SRC) \
		-DCMAKE_BUILD_TYPE=Release \
		-DCMAKE_INSTALL_PREFIX=$(QULACS_INSTALL) \
		-DUSE_GPU=No \
		-DUSE_MPI=No \
		-DUSE_PYTHON=No \
		-DBOOST_ROOT=$(BOOST_ROOT) \
		-DBoost_NO_SYSTEM_PATHS=ON
	"$(CMAKE)" --build $(QULACS_BUILD) --config Release
	"$(CMAKE)" --build $(QULACS_BUILD) --config Release
	@echo ">>> Staging Qulacs headers and libs to $(QULACS_INSTALL)..."
	@mkdir -p $(QULACS_INSTALL)/include $(QULACS_INSTALL)/lib
	@cp -r $(QULACS_SRC)/src/cppsim $(QULACS_INSTALL)/include/
	@cp -r $(QULACS_SRC)/src/csim   $(QULACS_INSTALL)/include/
	@cp -r $(QULACS_BUILD)/eigen/src/eigen/Eigen $(QULACS_INSTALL)/include/
	@cp native~/build/lib/cppsim_static.lib $(QULACS_INSTALL)/lib/
	@cp native~/build/lib/csim_static.lib   $(QULACS_INSTALL)/lib/

# -----------------------------------------------------------------------------
# Build the extern "C" wrapper to produce qulacs_unity.dll
# -----------------------------------------------------------------------------
build-dll:
	@echo ">>> [3/4] Building qulacs_unity.dll..."
	"$(CMAKE)" -B $(WRAPPER_BUILD) -S $(WRAPPER_SRC) \
		-DCMAKE_BUILD_TYPE=Release \
		-DQULACS_ROOT=$(QULACS_INSTALL)
	"$(CMAKE)" --build $(WRAPPER_BUILD) --config Release

# -----------------------------------------------------------------------------
# Copy the built DLL into the folder Unity loads from
# -----------------------------------------------------------------------------
deploy-dll:
	@echo ">>> [4/4] Deploying $(DLL) to Unity plugin folder..."
	@mkdir -p $(PLUGIN_DIR)
	@if [ -f "$(WRAPPER_BUILD)/Release/$(DLL)" ]; then \
		cp $(WRAPPER_BUILD)/Release/$(DLL) $(PLUGIN_DIR)/$(DLL); \
	elif [ -f "$(WRAPPER_BUILD)/$(DLL)" ]; then \
		cp $(WRAPPER_BUILD)/$(DLL) $(PLUGIN_DIR)/$(DLL); \
	else \
		echo "ERROR: $(DLL) not found. Build may have failed."; exit 1; \
	fi
	@echo ">>> Done. $(PLUGIN_DIR)/$(DLL)"

# =============================================================================
# Android ARM64
# =============================================================================

build-android: fetch-qulacs build-qulacs-android build-dll-android deploy-dll-android

# -----------------------------------------------------------------------------
# Cross-build Qulacs for Android ARM64
# -----------------------------------------------------------------------------
build-qulacs-android:
	@echo ">>> [2/4] Building Qulacs for Android ARM64..."
	"$(CMAKE)" -G Ninja -Wno-dev \
		-DCMAKE_MAKE_PROGRAM="$(NINJA)" \
		-B $(QULACS_BUILD_ANDROID) -S $(QULACS_SRC) \
		-DCMAKE_TOOLCHAIN_FILE="$(ANDROID_TOOLCHAIN)" \
		-DANDROID_ABI=$(ANDROID_ABI) \
		-DANDROID_PLATFORM=$(ANDROID_PLATFORM) \
		-DCMAKE_BUILD_TYPE=Release \
		-DCMAKE_INSTALL_PREFIX=$(QULACS_INSTALL_ANDROID) \
		-DCMAKE_POLICY_DEFAULT_CMP0144=NEW \
		-DUSE_GPU=No \
		-DUSE_MPI=No \
		-DUSE_PYTHON=No \
		-DBOOST_ROOT=$(BOOST_ROOT) \
		-DBoost_ROOT=$(BOOST_ROOT) \
		-DBoost_INCLUDE_DIR=$(BOOST_ROOT) \
		-DBoost_NO_SYSTEM_PATHS=ON
	"$(CMAKE)" --build $(QULACS_BUILD_ANDROID) --config Release
	@echo ">>> Staging Qulacs headers and libs for Android..."
	@mkdir -p $(QULACS_INSTALL_ANDROID)/include $(QULACS_INSTALL_ANDROID)/lib
	@cp -r $(QULACS_SRC)/src/cppsim $(QULACS_INSTALL_ANDROID)/include/
	@cp -r $(QULACS_SRC)/src/csim   $(QULACS_INSTALL_ANDROID)/include/
	@cp -r $(QULACS_BUILD_ANDROID)/eigen/src/eigen/Eigen $(QULACS_INSTALL_ANDROID)/include/
	@find $(QULACS_BUILD_ANDROID) -name "libcppsim_static.a" | head -1 | xargs -I{} cp {} $(QULACS_INSTALL_ANDROID)/lib/
	@find $(QULACS_BUILD_ANDROID) -name "libcsim_static.a"   | head -1 | xargs -I{} cp {} $(QULACS_INSTALL_ANDROID)/lib/

# -----------------------------------------------------------------------------
# Cross-build the wrapper for Android ARM64
# -----------------------------------------------------------------------------
build-dll-android:
	@echo ">>> [3/4] Building $(SO) for Android ARM64..."
	"$(CMAKE)" -G Ninja -Wno-dev \
		-DCMAKE_MAKE_PROGRAM="$(NINJA)" \
		-B $(WRAPPER_BUILD_ANDROID) -S $(WRAPPER_SRC) \
		-DCMAKE_TOOLCHAIN_FILE="$(ANDROID_TOOLCHAIN)" \
		-DANDROID_ABI=$(ANDROID_ABI) \
		-DANDROID_PLATFORM=$(ANDROID_PLATFORM) \
		-DCMAKE_BUILD_TYPE=Release \
		-DQULACS_ROOT=$(CURDIR)/$(QULACS_INSTALL_ANDROID) \
		-DCMAKE_FIND_ROOT_PATH_MODE_INCLUDE=BOTH \
		-DCMAKE_FIND_ROOT_PATH_MODE_LIBRARY=BOTH
	"$(CMAKE)" --build $(WRAPPER_BUILD_ANDROID) --config Release

# -----------------------------------------------------------------------------
# Copy the .so into the Unity plugin folder
# -----------------------------------------------------------------------------
deploy-dll-android:
	@echo ">>> [4/4] Deploying $(SO) to Unity plugin folder..."
	@mkdir -p $(PLUGIN_DIR_ANDROID)
	@find $(WRAPPER_BUILD_ANDROID) -name "$(SO)" | head -1 | xargs -I{} cp {} $(PLUGIN_DIR_ANDROID)/$(SO)
	@echo ">>> Done. $(PLUGIN_DIR_ANDROID)/$(SO)"

# =============================================================================
# Android x86_64 (for emulator)
# =============================================================================

build-android-x86_64: fetch-qulacs build-qulacs-android-x86_64 build-dll-android-x86_64 deploy-dll-android-x86_64

# -----------------------------------------------------------------------------
# Cross-build Qulacs for Android x86_64
# -----------------------------------------------------------------------------
build-qulacs-android-x86_64:
	@echo ">>> [2/4] Building Qulacs for Android x86_64..."
	"$(CMAKE)" -G Ninja -Wno-dev \
		-DCMAKE_MAKE_PROGRAM="$(NINJA)" \
		-B $(QULACS_BUILD_ANDROID_X86_64) -S $(QULACS_SRC) \
		-DCMAKE_TOOLCHAIN_FILE="$(ANDROID_TOOLCHAIN)" \
		-DANDROID_ABI=$(ANDROID_ABI_X86_64) \
		-DANDROID_PLATFORM=$(ANDROID_PLATFORM) \
		-DCMAKE_BUILD_TYPE=Release \
		-DCMAKE_INSTALL_PREFIX=$(QULACS_INSTALL_ANDROID_X86_64) \
		-DCMAKE_POLICY_DEFAULT_CMP0144=NEW \
		-DUSE_GPU=No \
		-DUSE_MPI=No \
		-DUSE_PYTHON=No \
		-DBOOST_ROOT=$(BOOST_ROOT) \
		-DBoost_ROOT=$(BOOST_ROOT) \
		-DBoost_INCLUDE_DIR=$(BOOST_ROOT) \
		-DBoost_NO_SYSTEM_PATHS=ON
	"$(CMAKE)" --build $(QULACS_BUILD_ANDROID_X86_64) --config Release
	@echo ">>> Staging Qulacs headers and libs for Android x86_64..."
	@mkdir -p $(QULACS_INSTALL_ANDROID_X86_64)/include $(QULACS_INSTALL_ANDROID_X86_64)/lib
	@cp -r $(QULACS_SRC)/src/cppsim $(QULACS_INSTALL_ANDROID_X86_64)/include/
	@cp -r $(QULACS_SRC)/src/csim   $(QULACS_INSTALL_ANDROID_X86_64)/include/
	@cp -r $(QULACS_BUILD_ANDROID_X86_64)/eigen/src/eigen/Eigen $(QULACS_INSTALL_ANDROID_X86_64)/include/
	@find $(QULACS_BUILD_ANDROID_X86_64) -name "libcppsim_static.a" | head -1 | xargs -I{} cp {} $(QULACS_INSTALL_ANDROID_X86_64)/lib/
	@find $(QULACS_BUILD_ANDROID_X86_64) -name "libcsim_static.a"   | head -1 | xargs -I{} cp {} $(QULACS_INSTALL_ANDROID_X86_64)/lib/

# -----------------------------------------------------------------------------
# Cross-build the wrapper for Android x86_64
# -----------------------------------------------------------------------------
build-dll-android-x86_64:
	@echo ">>> [3/4] Building $(SO) for Android x86_64..."
	"$(CMAKE)" -G Ninja -Wno-dev \
		-DCMAKE_MAKE_PROGRAM="$(NINJA)" \
		-B $(WRAPPER_BUILD_ANDROID_X86_64) -S $(WRAPPER_SRC) \
		-DCMAKE_TOOLCHAIN_FILE="$(ANDROID_TOOLCHAIN)" \
		-DANDROID_ABI=$(ANDROID_ABI_X86_64) \
		-DANDROID_PLATFORM=$(ANDROID_PLATFORM) \
		-DCMAKE_BUILD_TYPE=Release \
		-DQULACS_ROOT=$(CURDIR)/$(QULACS_INSTALL_ANDROID_X86_64) \
		-DCMAKE_FIND_ROOT_PATH_MODE_INCLUDE=BOTH \
		-DCMAKE_FIND_ROOT_PATH_MODE_LIBRARY=BOTH
	"$(CMAKE)" --build $(WRAPPER_BUILD_ANDROID_X86_64) --config Release

# -----------------------------------------------------------------------------
# Copy the x86_64 .so into the Unity plugin folder
# -----------------------------------------------------------------------------
deploy-dll-android-x86_64:
	@echo ">>> [4/4] Deploying $(SO) to Unity plugin folder (x86_64)..."
	@mkdir -p $(PLUGIN_DIR_ANDROID_X86_64)
	@find $(WRAPPER_BUILD_ANDROID_X86_64) -name "$(SO)" | head -1 | xargs -I{} cp {} $(PLUGIN_DIR_ANDROID_X86_64)/$(SO)
	@echo ">>> Done. $(PLUGIN_DIR_ANDROID_X86_64)/$(SO)"

# =============================================================================
# Android ALL (ARM64 + x86_64)
# =============================================================================

build-android-all: build-android build-android-x86_64

# =============================================================================
# macOS
# =============================================================================

build-macos: fetch-qulacs fetch-boost build-qulacs-macos build-dll-macos deploy-dll-macos

# -----------------------------------------------------------------------------
# Build Qulacs for macOS
# -----------------------------------------------------------------------------
build-qulacs-macos:
	@echo ">>> [2/4] Building Qulacs for macOS..."
	"$(CMAKE)" -B $(QULACS_BUILD_MACOS) -S $(QULACS_SRC) \
		-DCMAKE_BUILD_TYPE=Release \
		-DCMAKE_INSTALL_PREFIX=$(QULACS_INSTALL_MACOS) \
		-DCMAKE_POLICY_DEFAULT_CMP0144=NEW \
		-DUSE_GPU=No \
		-DUSE_MPI=No \
		-DUSE_PYTHON=No \
		-DBOOST_ROOT=$(CURDIR)/$(BOOST_ROOT) \
		-DBoost_ROOT=$(CURDIR)/$(BOOST_ROOT) \
		-DBoost_INCLUDE_DIR=$(CURDIR)/$(BOOST_ROOT) \
		-DBoost_NO_SYSTEM_PATHS=ON
	"$(CMAKE)" --build $(QULACS_BUILD_MACOS) --config Release
	@echo ">>> Staging Qulacs headers and libs for macOS..."
	@mkdir -p $(QULACS_INSTALL_MACOS)/include $(QULACS_INSTALL_MACOS)/lib
	@cp -r $(QULACS_SRC)/src/cppsim $(QULACS_INSTALL_MACOS)/include/
	@cp -r $(QULACS_SRC)/src/csim   $(QULACS_INSTALL_MACOS)/include/
	@cp -r $(QULACS_BUILD_MACOS)/eigen/src/eigen/Eigen $(QULACS_INSTALL_MACOS)/include/
	@cp native~/build/lib/libcppsim_static.a $(QULACS_INSTALL_MACOS)/lib/
	@cp native~/build/lib/libcsim_static.a   $(QULACS_INSTALL_MACOS)/lib/

# -----------------------------------------------------------------------------
# Build the wrapper for macOS to produce qulacs_unity.dylib
# -----------------------------------------------------------------------------
build-dll-macos:
	@echo ">>> [3/4] Building $(DYLIB) for macOS..."
	"$(CMAKE)" -B $(WRAPPER_BUILD_MACOS) -S $(WRAPPER_SRC) \
		-DCMAKE_BUILD_TYPE=Release \
		-DQULACS_ROOT=$(CURDIR)/$(QULACS_INSTALL_MACOS) \
		-DOpenMP_C_FLAGS="-Xpreprocessor -fopenmp -I$(LIBOMP_PREFIX)/include" \
		-DOpenMP_CXX_FLAGS="-Xpreprocessor -fopenmp -I$(LIBOMP_PREFIX)/include" \
		-DOpenMP_C_LIB_NAMES=omp \
		-DOpenMP_CXX_LIB_NAMES=omp \
		-DOpenMP_omp_LIBRARY=$(LIBOMP_PREFIX)/lib/libomp.dylib
	"$(CMAKE)" --build $(WRAPPER_BUILD_MACOS) --config Release

# -----------------------------------------------------------------------------
# Copy the .dylib into the Unity plugin folder
# -----------------------------------------------------------------------------
deploy-dll-macos:
	@echo ">>> [4/4] Deploying $(DYLIB) to Unity plugin folder..."
	@mkdir -p $(PLUGIN_DIR_MACOS)
	@if [ -f "$(WRAPPER_BUILD_MACOS)/$(DYLIB)" ]; then \
		cp $(WRAPPER_BUILD_MACOS)/$(DYLIB) $(PLUGIN_DIR_MACOS)/$(DYLIB); \
	else \
		echo "ERROR: $(DYLIB) not found. Build may have failed."; exit 1; \
	fi
	@echo ">>> Done. $(PLUGIN_DIR_MACOS)/$(DYLIB)"

# =============================================================================
# iOS (cross-compile from macOS)
# =============================================================================

build-ios: fetch-qulacs fetch-boost build-qulacs-ios build-dll-ios deploy-dll-ios

# -----------------------------------------------------------------------------
# Cross-build Qulacs for iOS ARM64
# -----------------------------------------------------------------------------
build-qulacs-ios:
	@echo ">>> [2/4] Building Qulacs for iOS..."
	"$(CMAKE)" -B $(QULACS_BUILD_IOS) -S $(QULACS_SRC) \
		-DCMAKE_SYSTEM_NAME=iOS \
		-DCMAKE_OSX_DEPLOYMENT_TARGET=$(IOS_DEPLOYMENT_TARGET) \
		-DCMAKE_OSX_ARCHITECTURES=arm64 \
		-DCMAKE_BUILD_TYPE=Release \
		-DCMAKE_INSTALL_PREFIX=$(QULACS_INSTALL_IOS) \
		-DCMAKE_POLICY_DEFAULT_CMP0144=NEW \
		-DUSE_GPU=No \
		-DUSE_MPI=No \
		-DUSE_PYTHON=No \
		-DBOOST_ROOT=$(CURDIR)/$(BOOST_ROOT) \
		-DBoost_ROOT=$(CURDIR)/$(BOOST_ROOT) \
		-DBoost_INCLUDE_DIR=$(CURDIR)/$(BOOST_ROOT) \
		-DBoost_NO_SYSTEM_PATHS=ON
	"$(CMAKE)" --build $(QULACS_BUILD_IOS) --config Release
	@echo ">>> Staging Qulacs headers and libs for iOS..."
	@mkdir -p $(QULACS_INSTALL_IOS)/include $(QULACS_INSTALL_IOS)/lib
	@cp -r $(QULACS_SRC)/src/cppsim $(QULACS_INSTALL_IOS)/include/
	@cp -r $(QULACS_SRC)/src/csim   $(QULACS_INSTALL_IOS)/include/
	@cp -r $(QULACS_BUILD_IOS)/eigen/src/eigen/Eigen $(QULACS_INSTALL_IOS)/include/
	@cp native~/build/lib/libcppsim_static.a $(QULACS_INSTALL_IOS)/lib/
	@cp native~/build/lib/libcsim_static.a   $(QULACS_INSTALL_IOS)/lib/

# -----------------------------------------------------------------------------
# Build the wrapper for iOS to produce a static library
# -----------------------------------------------------------------------------
build-dll-ios:
	@echo ">>> [3/4] Building $(IOS_STATIC_LIB) for iOS..."
	"$(CMAKE)" -B $(WRAPPER_BUILD_IOS) -S $(WRAPPER_SRC) \
		-DCMAKE_SYSTEM_NAME=iOS \
		-DCMAKE_OSX_DEPLOYMENT_TARGET=$(IOS_DEPLOYMENT_TARGET) \
		-DCMAKE_OSX_ARCHITECTURES=arm64 \
		-DCMAKE_BUILD_TYPE=Release \
		-DQULACS_ROOT=$(CURDIR)/$(QULACS_INSTALL_IOS) \
		-DCMAKE_FIND_ROOT_PATH_MODE_INCLUDE=BOTH \
		-DCMAKE_FIND_ROOT_PATH_MODE_LIBRARY=BOTH
	"$(CMAKE)" --build $(WRAPPER_BUILD_IOS) --config Release

# -----------------------------------------------------------------------------
# Merge the wrapper .a with the Qulacs (cppsim/csim) .a archives into a single .a.
# Unity iOS only picks up one plugin .a, so CMake's transitive link dependency
# alone leaves unresolved symbols (state_multiply, marginal_prob, etc.).
# Use `libtool -static` to physically combine the archives.
# -----------------------------------------------------------------------------
deploy-dll-ios:
	@echo ">>> [4/4] Deploying $(IOS_STATIC_LIB) to Unity plugin folder..."
	@mkdir -p $(PLUGIN_DIR_IOS)
	@WRAPPER_A=$$(find $(WRAPPER_BUILD_IOS) -name "$(IOS_STATIC_LIB)" | head -1); \
	if [ -z "$$WRAPPER_A" ]; then \
		echo "ERROR: $(IOS_STATIC_LIB) not found under $(WRAPPER_BUILD_IOS)"; exit 1; \
	fi; \
	echo "    Merging $$WRAPPER_A + libcppsim_static.a + libcsim_static.a"; \
	libtool -static -o $(PLUGIN_DIR_IOS)/$(IOS_STATIC_LIB) \
		"$$WRAPPER_A" \
		$(QULACS_INSTALL_IOS)/lib/libcppsim_static.a \
		$(QULACS_INSTALL_IOS)/lib/libcsim_static.a
	@echo ">>> Done. $(PLUGIN_DIR_IOS)/$(IOS_STATIC_LIB)"

# =============================================================================
# Clean
# =============================================================================

# -----------------------------------------------------------------------------
# Remove build artifacts
# -----------------------------------------------------------------------------
clean-build:
	@echo ">>> Removing build artifacts (native~/build/)..."
	rm -rf native~/build

clean-all: clean-build
	@echo ">>> Removing Qulacs install and source..."
	rm -rf $(QULACS_INSTALL) $(QULACS_INSTALL_ANDROID) $(QULACS_INSTALL_ANDROID_X86_64) \
		$(QULACS_INSTALL_MACOS) $(QULACS_INSTALL_IOS) $(QULACS_SRC) $(BOOST_ROOT)
