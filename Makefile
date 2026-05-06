ifeq ($(OS),Windows_NT)
    VENV_PYTHON := .venv/Scripts/python.exe
else
    VENV_PYTHON := .venv/bin/python
endif

# -----------------------------------------------------------------------------
# GitHub release
# -----------------------------------------------------------------------------
PACKAGE_DIR := Packages/com.mu5dvlp.qulacs
GH_REPO     := mu5dvlp/qulacs-for-unity
VERSION     := $(shell awk -F'"' '/"version"/ {print $$4; exit}' $(PACKAGE_DIR)/package.json)
TAG         := v$(VERSION)
CHANGELOG   := $(PACKAGE_DIR)/CHANGELOG.md
DRAFT       ?= 1

.PHONY: coverage release release-notes

coverage:
	@echo ">>> Checking Qulacs wrapper coverage..."
	"$(VENV_PYTHON)" tools/check_coverage.py

# Print release notes that would be sent to gh, without creating anything.
#   make release-notes
release-notes:
	@printf '## Installation (UPM Git URL)\n\n'
	@printf '```\n'
	@printf 'https://github.com/$(GH_REPO).git?path=/$(PACKAGE_DIR)#$(TAG)\n'
	@printf '```\n\n'
	@awk '/^## \[$(VERSION)\]/{flag=1; next} /^## \[/{flag=0} flag' $(CHANGELOG)

# Create a GitHub release for $(TAG) using CHANGELOG section as notes.
# DRAFT=1 (default) creates a draft; DRAFT=0 publishes immediately.
# The matching git tag must already be pushed to origin.
#   make release            # draft
#   make release DRAFT=0    # publish
release:
	@echo ">>> Creating GitHub release for $(TAG) (DRAFT=$(DRAFT))..."
	@notes=$$(mktemp); \
	$(MAKE) --no-print-directory release-notes > $$notes; \
	if [ "$(DRAFT)" = "1" ]; then \
		gh release create $(TAG) --repo $(GH_REPO) --title "$(TAG)" --notes-file "$$notes" --draft; \
	else \
		gh release create $(TAG) --repo $(GH_REPO) --title "$(TAG)" --notes-file "$$notes"; \
	fi; \
	rc=$$?; rm -f "$$notes"; exit $$rc
