ifeq ($(OS),Windows_NT)
    VENV_PYTHON := .venv/Scripts/python.exe
else
    VENV_PYTHON := .venv/bin/python
endif

.PHONY: coverage

coverage:
	@echo ">>> Checking Qulacs wrapper coverage..."
	"$(VENV_PYTHON)" tools/check_coverage.py
