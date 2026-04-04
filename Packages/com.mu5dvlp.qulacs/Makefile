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

QULACS_REPO    := https://github.com/qulacs/qulacs.git
QULACS_SRC     := native~/extern/qulacs
QULACS_BUILD   := native~/build/qulacs
QULACS_INSTALL := native~/extern/qulacs-install
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
	cmake -B $(QULACS_BUILD) -S $(QULACS_SRC) \
		-DCMAKE_BUILD_TYPE=Release \
		-DCMAKE_INSTALL_PREFIX=$(QULACS_INSTALL) \
		-DUSE_GPU=No \
		-DUSE_MPI=No
	cmake --build $(QULACS_BUILD) --config Release
	cmake --install $(QULACS_BUILD) --config Release

# -----------------------------------------------------------------------------
# extern "C" ラッパーをビルドして qulacs_unity.dll を生成する
# -----------------------------------------------------------------------------
build-dll:
	@echo ">>> [3/4] Building qulacs_unity.dll..."
	cmake -B $(WRAPPER_BUILD) -S $(WRAPPER_SRC) \
		-DCMAKE_BUILD_TYPE=Release \
		-DQULACS_ROOT=$(QULACS_INSTALL)
	cmake --build $(WRAPPER_BUILD) --config Release

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
