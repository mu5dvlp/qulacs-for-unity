#!/usr/bin/env python3
"""Verify committed native plugin binaries are in sync with the C API.

The C API in native~/src/qulacs_unity.h is the source of truth. This script
bidirectionally compares it against:
  - [DllImport] declarations in Runtime/Internal/NativeMethods.cs
  - exported symbols of every binary under Runtime/Plugins/

Any divergence (missing or extra qulacs_* symbol on either side) is an error.

Requires `llvm-nm` and `llvm-readobj` (apt-get install -y llvm on Ubuntu;
`brew install llvm` on macOS).
"""
from __future__ import annotations

import re
import shutil
import subprocess
import sys
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[2]
PKG_ROOT = REPO_ROOT / "Packages/com.mu5dvlp.qulacs"
HEADER = PKG_ROOT / "native~/src/qulacs_unity.h"
CS_FILE = PKG_ROOT / "Runtime/Internal/NativeMethods.cs"
PLUGINS_DIR = PKG_ROOT / "Runtime/Plugins"

BIN_SUFFIXES = (".dll", ".dylib", ".so", ".a")

# Matches: QULACS_UNITY_API <return type ...> qulacs_xxx(
HEADER_RE = re.compile(
    r"QULACS_UNITY_API\s+[^;{]*?\b(qulacs_[A-Za-z0-9_]+)\s*\(",
    re.DOTALL,
)
# Matches: [DllImport(...)] ... extern <type> qulacs_xxx(
CS_RE = re.compile(r"\bextern\s+\S+\s+(qulacs_[A-Za-z0-9_]+)\s*\(")


def _strip_comments(text: str) -> str:
    text = re.sub(r"/\*.*?\*/", "", text, flags=re.DOTALL)
    text = re.sub(r"//.*", "", text)
    return text


def expected_from_header() -> set[str]:
    return set(HEADER_RE.findall(_strip_comments(HEADER.read_text())))


def declared_in_cs() -> set[str]:
    return set(CS_RE.findall(_strip_comments(CS_FILE.read_text())))


def _run(cmd: list[str]) -> str:
    return subprocess.run(cmd, capture_output=True, text=True, check=True).stdout


def exported_from_binary(path: Path) -> set[str]:
    """Return the set of qulacs_* symbols defined/exported by `path`."""
    syms: set[str] = set()

    if path.suffix == ".dll":
        # PE export table: llvm-readobj --coff-exports lists DLL exports by name.
        out = _run(["llvm-readobj", "--coff-exports", str(path)])
        # Lines look like: "    Name: qulacs_state_create"
        for m in re.finditer(r"Name:\s*(qulacs_[A-Za-z0-9_]+)", out):
            syms.add(m.group(1))
        return syms

    # ELF / Mach-O / static archive: defined externs from llvm-nm.
    # Archive listings interleave "<member.o>:" header lines; we want only
    # symbol lines, which always have a single-letter type column (T, D, B, ...).
    out = _run(["llvm-nm", "--defined-only", "--extern-only", str(path)])
    for line in out.splitlines():
        parts = line.split()
        if len(parts) < 2 or len(parts[-2]) != 1:
            continue
        name = parts[-1]
        # Mach-O prefixes C symbols with `_`. Strip it for comparison.
        if name.startswith("_qulacs_"):
            name = name[1:]
        if name.startswith("qulacs_"):
            syms.add(name)
    return syms


def _diff_report(label: str, expected: set[str], actual: set[str]) -> str | None:
    if actual == expected:
        return None
    only_exp = sorted(expected - actual)
    only_act = sorted(actual - expected)
    lines = [f"{label}: MISMATCH ({len(actual)} found, {len(expected)} expected)"]
    if only_exp:
        lines.append(f"  missing here:  {only_exp}")
    if only_act:
        lines.append(f"  extra here:    {only_act}")
    return "\n".join(lines)


def _require(tool: str) -> None:
    if shutil.which(tool) is None:
        print(f"ERROR: `{tool}` not found in PATH.", file=sys.stderr)
        print("       Install LLVM tools (apt-get install -y llvm).", file=sys.stderr)
        sys.exit(2)


def main() -> int:
    _require("llvm-nm")
    _require("llvm-readobj")

    expected = expected_from_header()
    if not expected:
        print(f"ERROR: no QULACS_UNITY_API functions found in {HEADER}", file=sys.stderr)
        return 2
    print(f"Header declares {len(expected)} functions in qulacs_unity.h.")

    failures: list[str] = []

    cs_diff = _diff_report("NativeMethods.cs", expected, declared_in_cs())
    if cs_diff:
        failures.append(cs_diff)
    else:
        print("OK   Runtime/Internal/NativeMethods.cs")

    binaries = sorted(
        p for p in PLUGINS_DIR.rglob("*")
        if p.is_file() and p.suffix in BIN_SUFFIXES
    )
    if not binaries:
        failures.append(f"No native binaries found under {PLUGINS_DIR}.")

    for b in binaries:
        rel = b.relative_to(REPO_ROOT)
        diff = _diff_report(str(rel), expected, exported_from_binary(b))
        if diff:
            failures.append(diff)
        else:
            print(f"OK   {rel}")

    if failures:
        print("\n=== Symbol-sync check FAILED ===", file=sys.stderr)
        for f in failures:
            print(f, file=sys.stderr)
        print(
            "\nRebuild the affected plugin(s) with the matching `make build-*` target "
            "in Packages/com.mu5dvlp.qulacs/.",
            file=sys.stderr,
        )
        return 1

    print("\nAll plugin binaries and NativeMethods.cs are in sync with qulacs_unity.h.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
