# =============================================================================
# Makefile for com.mu5dvlp.qulacs
#
# Prerequisites (Windows):
#   - Git
#   - CMake 3.20+
#   - Visual Studio 2022 (MSVC) with "Desktop development with C++" workload
#   - make (Git Bash 付属、または choco install make)
#   - Android: Unity 付属の NDK (Android Build Support をインストール)
#
# Commands:
#   make build                # Windows x86_64 フルビルド
#   make build-android        # Android ARM64 フルビルド
#   make build-android-x86_64 # Android x86_64 フルビルド (エミュレータ用)
#   make build-android-all    # Android ARM64 + x86_64 フルビルド
#   make fetch-qulacs         # Qulacs ソースを git clone する
#   make clean-build          # ビルド成果物を削除する (native~/build/)
#   make clean-all            # ビルド成果物 + Qulacs ソース & インストール先を全削除する
# =============================================================================

# VS 付属の cmake を使う（PATH に cmake がない場合のフォールバック）
CMAKE := $(shell where cmake 2>/dev/null | head -1)
ifeq ($(CMAKE),)
CMAKE := C:/Program Files/Microsoft Visual Studio/2022/Community/Common7/IDE/CommonExtensions/Microsoft/CMake/CMake/bin/cmake.exe
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
SO                     := qulacs_unity.so

# ---------------------------------------------------------------------------
# Android ARM64 settings
# ---------------------------------------------------------------------------
ANDROID_ABI            := arm64-v8a
QULACS_BUILD_ANDROID   := native~/build/qulacs-android
QULACS_INSTALL_ANDROID := native~/extern/qulacs-install-android
WRAPPER_BUILD_ANDROID  := native~/build/wrapper-android
PLUGIN_DIR_ANDROID     := Runtime/Plugins/Android/ARM64

# ---------------------------------------------------------------------------
# Android x86_64 settings (エミュレータ用)
# ---------------------------------------------------------------------------
ANDROID_ABI_X86_64            := x86_64
QULACS_BUILD_ANDROID_X86_64   := native~/build/qulacs-android-x86_64
QULACS_INSTALL_ANDROID_X86_64 := native~/extern/qulacs-install-android-x86_64
WRAPPER_BUILD_ANDROID_X86_64  := native~/build/wrapper-android-x86_64
PLUGIN_DIR_ANDROID_X86_64     := Runtime/Plugins/Android/x86_64

.PHONY: build fetch-qulacs build-qulacs build-dll deploy-dll \
        build-android build-qulacs-android build-dll-android deploy-dll-android \
        build-android-x86_64 build-qulacs-android-x86_64 build-dll-android-x86_64 deploy-dll-android-x86_64 \
        build-android-all \
        clean-build clean-all

build: fetch-qulacs build-qulacs build-dll deploy-dll

# -----------------------------------------------------------------------------
# Qulacs ソースを取得する
# -----------------------------------------------------------------------------
fetch-qulacs:
	@echo ">>> [1/4] Fetching Qulacs source..."
	@if [ ! -d "$(QULACS_SRC)/.git" ]; then \
		git clone --depth 1 $(QULACS_REPO) $(QULACS_SRC); \
	else \
		echo "    Qulacs source already present, skipping clone."; \
	fi

# -----------------------------------------------------------------------------
# Qulacs をビルドして include/ と lib/ をインストールする
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
# extern "C" ラッパーをビルドして qulacs_unity.dll を生成する
# -----------------------------------------------------------------------------
build-dll:
	@echo ">>> [3/4] Building qulacs_unity.dll..."
	"$(CMAKE)" -B $(WRAPPER_BUILD) -S $(WRAPPER_SRC) \
		-DCMAKE_BUILD_TYPE=Release \
		-DQULACS_ROOT=$(QULACS_INSTALL)
	"$(CMAKE)" --build $(WRAPPER_BUILD) --config Release

# -----------------------------------------------------------------------------
# 生成した DLL を Unity が読み込むフォルダにコピーする
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
# Qulacs を Android ARM64 向けにクロスビルドする
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
# ラッパーを Android ARM64 向けにクロスビルドする
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
		-DQULACS_ROOT=$(QULACS_INSTALL_ANDROID) \
		-DCMAKE_FIND_ROOT_PATH_MODE_INCLUDE=BOTH \
		-DCMAKE_FIND_ROOT_PATH_MODE_LIBRARY=BOTH
	"$(CMAKE)" --build $(WRAPPER_BUILD_ANDROID) --config Release

# -----------------------------------------------------------------------------
# .so を Unity プラグインフォルダにコピーする
# -----------------------------------------------------------------------------
deploy-dll-android:
	@echo ">>> [4/4] Deploying $(SO) to Unity plugin folder..."
	@mkdir -p $(PLUGIN_DIR_ANDROID)
	@find $(WRAPPER_BUILD_ANDROID) -name "$(SO)" | head -1 | xargs -I{} cp {} $(PLUGIN_DIR_ANDROID)/$(SO)
	@echo ">>> Done. $(PLUGIN_DIR_ANDROID)/$(SO)"

# =============================================================================
# Android x86_64 (エミュレータ用)
# =============================================================================

build-android-x86_64: fetch-qulacs build-qulacs-android-x86_64 build-dll-android-x86_64 deploy-dll-android-x86_64

# -----------------------------------------------------------------------------
# Qulacs を Android x86_64 向けにクロスビルドする
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
# ラッパーを Android x86_64 向けにクロスビルドする
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
		-DQULACS_ROOT=$(QULACS_INSTALL_ANDROID_X86_64) \
		-DCMAKE_FIND_ROOT_PATH_MODE_INCLUDE=BOTH \
		-DCMAKE_FIND_ROOT_PATH_MODE_LIBRARY=BOTH
	"$(CMAKE)" --build $(WRAPPER_BUILD_ANDROID_X86_64) --config Release

# -----------------------------------------------------------------------------
# x86_64 の .so を Unity プラグインフォルダにコピーする
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
# Clean
# =============================================================================

# -----------------------------------------------------------------------------
# ビルド成果物を削除する
# -----------------------------------------------------------------------------
clean-build:
	@echo ">>> Removing build artifacts (native~/build/)..."
	rm -rf native~/build

clean-all: clean-build
	@echo ">>> Removing Qulacs install and source..."
	rm -rf $(QULACS_INSTALL) $(QULACS_INSTALL_ANDROID) $(QULACS_INSTALL_ANDROID_X86_64) $(QULACS_SRC)
