VENV_PYTHON := $(shell if [ -f ".venv/Scripts/python" ]; then echo ".venv/Scripts/python"; else echo ".venv/bin/python"; fi)

.PHONY: coverage

coverage:
	@echo ">>> Checking Qulacs wrapper coverage..."
	@if [ ! -f "$(VENV_PYTHON)" ]; then \
		echo "ERROR: .venv not found. Run: python -m venv .venv"; exit 1; \
	fi
	"$(VENV_PYTHON)" tools/check_coverage.py
