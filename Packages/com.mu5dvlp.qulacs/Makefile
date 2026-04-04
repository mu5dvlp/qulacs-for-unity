# =============================================================================
# Makefile for com.mu5dvlp.qulacs
#
# Prerequisites (Windows):
#   - Git
#   - CMake 3.20+
#   - Visual Studio 2022 (MSVC) with "Desktop development with C++" workload
#   - make (e.g. via Git Bash / MinGW / Chocolatey: choco install make)
#
# Usage:
#   make            # full build: clone → build qulacs → build wrapper → install
#   make qulacs     # build Qulacs only
#   make wrapper    # build the C++ wrapper only (requires qulacs target done)
#   make install    # copy DLL to Unity plugin folder
#   make clean      # remove all build artifacts
#   make clean-all  # also remove the cloned Qulacs source
# =============================================================================

QULACS_REPO    := https://github.com/qulacs/qulacs.git
QULACS_SRC     := native~/extern/qulacs
QULACS_BUILD   := native~/build/qulacs
QULACS_INSTALL := native~/extern/qulacs-install
WRAPPER_SRC    := native~
WRAPPER_BUILD  := native~/build/wrapper
PLUGIN_DIR     := Runtime/Plugins/Windows/x86_64
DLL            := qulacs_unity.dll

.PHONY: all qulacs wrapper install clean clean-all

all: qulacs wrapper install

# -----------------------------------------------------------------------------
# [1-3] Clone and build Qulacs, then install to a known prefix
# -----------------------------------------------------------------------------
qulacs:
	@echo ">>> [1/3] Fetching Qulacs source..."
	@if [ ! -d "$(QULACS_SRC)/.git" ]; then \
		git clone --depth 1 $(QULACS_REPO) $(QULACS_SRC); \
	else \
		echo "    Qulacs source already present, skipping clone."; \
	fi
	@echo ">>> [2/3] Configuring Qulacs..."
	cmake -B $(QULACS_BUILD) -S $(QULACS_SRC) \
		-DCMAKE_BUILD_TYPE=Release \
		-DCMAKE_INSTALL_PREFIX=$(QULACS_INSTALL) \
		-DUSE_GPU=No \
		-DUSE_MPI=No
	@echo ">>> [3/3] Building and installing Qulacs..."
	cmake --build $(QULACS_BUILD) --config Release
	cmake --install $(QULACS_BUILD) --config Release

# -----------------------------------------------------------------------------
# [4-5] Build the extern "C" wrapper DLL
# -----------------------------------------------------------------------------
wrapper:
	@echo ">>> [4/5] Configuring wrapper..."
	cmake -B $(WRAPPER_BUILD) -S $(WRAPPER_SRC) \
		-DCMAKE_BUILD_TYPE=Release \
		-DQULACS_ROOT=$(QULACS_INSTALL)
	@echo ">>> [5/5] Building wrapper..."
	cmake --build $(WRAPPER_BUILD) --config Release

# -----------------------------------------------------------------------------
# [6] Copy DLL to Unity plugin folder
# -----------------------------------------------------------------------------
install:
	@echo ">>> Copying $(DLL) to $(PLUGIN_DIR)..."
	@mkdir -p $(PLUGIN_DIR)
	@if [ -f "$(WRAPPER_BUILD)/Release/$(DLL)" ]; then \
		cp $(WRAPPER_BUILD)/Release/$(DLL) $(PLUGIN_DIR)/$(DLL); \
	elif [ -f "$(WRAPPER_BUILD)/$(DLL)" ]; then \
		cp $(WRAPPER_BUILD)/$(DLL) $(PLUGIN_DIR)/$(DLL); \
	else \
		echo "ERROR: $(DLL) not found. Build may have failed."; exit 1; \
	fi
	@echo ">>> Done. DLL installed to $(PLUGIN_DIR)/$(DLL)"

# -----------------------------------------------------------------------------
# Cleanup
# -----------------------------------------------------------------------------
clean:
	@echo ">>> Removing build artifacts..."
	rm -rf native~/build

clean-all: clean
	@echo ">>> Removing Qulacs install and source..."
	rm -rf $(QULACS_INSTALL) $(QULACS_SRC)
