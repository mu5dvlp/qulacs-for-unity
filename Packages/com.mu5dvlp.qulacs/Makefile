# =============================================================================
# Makefile for com.mu5dvlp.qulacs
#
# Prerequisites (Windows):
#   - Git
#   - CMake 3.20+
#   - Visual Studio 2022 (MSVC) with "Desktop development with C++" workload
#   - make (Git Bash 付属、または choco install make)
#
# Commands:
#   make build                # フルビルド (fetch → build-qulacs → build-dll → deploy-dll)
#   make fetch-qulacs         # Qulacs ソースを git clone する
#   make build-qulacs         # Qulacs を CMake でビルド & インストールする
#   make build-dll            # C++ ラッパーをビルドして qulacs_unity.dll を生成する
#   make deploy-dll           # 生成した DLL を Unity プラグインフォルダにコピーする
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

.PHONY: build fetch-qulacs build-qulacs build-dll deploy-dll clean-build clean-all

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

# -----------------------------------------------------------------------------
# ビルド成果物を削除する
# -----------------------------------------------------------------------------
clean-build:
	@echo ">>> Removing build artifacts (native~/build/)..."
	rm -rf native~/build

clean-all: clean-build
	@echo ">>> Removing Qulacs install and source..."
	rm -rf $(QULACS_INSTALL) $(QULACS_SRC)
