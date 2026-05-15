# Contributing to Qulacs for Unity

Thank you for your interest in contributing! This guide covers how to set up your environment, follow code conventions, and submit changes.

## Prerequisites

| Tool | Version | Purpose |
|---|---|---|
| Unity | 6000.4.1f1 LTS | Project editor |
| .NET SDK | 8.0+ | C# formatter (CSharpier), test runner |
| Git | any | Version control |
| clang-format | 14+ | C++ formatter |
| make | any | Build & format commands |

For native library builds, see [Makefile prerequisites](Packages/com.mu5dvlp.qulacs/CLAUDE.md).

## Getting Started

```bash
# 1. Fork and clone
git clone https://github.com/<your-fork>/qulacs-for-unity.git
cd qulacs-for-unity

# 2. Install formatting tools
dotnet tool restore

# 3. Open in Unity
#    Open the project folder with Unity Hub
```

## Code Style

This project enforces formatting with automated tools. **Run the formatter before every commit.**

### Quick Commands

```bash
make format        # auto-format all files
make format-check  # check only (CI mode, no changes)
```

### C# — CSharpier

[CSharpier](https://csharpier.com/) is an opinionated C# formatter, configured via `.csharpierrc.yaml` and `.editorconfig`.

```bash
dotnet csharpier .           # format
dotnet csharpier --check .   # check
```

Key conventions:
- 4 spaces, Allman braces
- Private fields: `_camelCase`
- Public members: `PascalCase`
- `using` directives outside `namespace`
- Max line width: 120

### C++ — clang-format

Configured via `.clang-format` at the project root.

```bash
clang-format -i Packages/com.mu5dvlp.qulacs/native~/src/*.h \
                Packages/com.mu5dvlp.qulacs/native~/src/*.cpp
```

Key conventions:
- 4 spaces, attached braces
- `snake_case` for functions
- Max line width: 120

### Editor Integration

The `.editorconfig` file is recognized by most editors (VS Code, Rider, Visual Studio). Install the EditorConfig plugin if your editor doesn't support it natively.

## Running Tests

```bash
make test
# Or directly:
dotnet test Packages/com.mu5dvlp.qulacs/dotnet~/Mu5dvlp.Qulacs.Tests.csproj
```

Tests use NUnit and run via `dotnet test`. The native DLL (`qulacs_unity.dll`) is automatically copied to the output directory at build time.

## Branch Strategy

```
feat/#123_add-ry-gate  ─┐
fix/#456_null-check     ─┼──▶ dev ──▶ (release) ──▶ main
chore/#789_update-ci   ─┘
```

| Branch | Purpose |
|---|---|
| `main` | Stable releases only |
| `dev` | Active development — all feature/fix branches merge here |
| `{type}/#{issue}_{description}` | Short-lived work branches, created from `dev` |

### Branch Naming Convention

```
{type}/#{issue_number}_{short-description}
```

- **type**: `feat`, `fix`, `chore` (matches Conventional Commits)
- **issue_number**: GitHub Issue number
- **short-description**: kebab-case summary

Examples:
- `feat/#42_add-ry-gate`
- `fix/#108_null-check-sampling`
- `chore/#15_update-ci-workflow`

## Submitting a Pull Request

1. Create a branch from `dev` following the naming convention:
   ```bash
   git checkout dev
   git pull origin dev
   git checkout -b feat/#42_add-ry-gate
   ```

2. Make your changes.

3. Format and verify:
   ```bash
   make format
   ```

4. Run tests to ensure nothing is broken.

5. Commit with a clear message:
   ```
   feat: add RY gate support (#42)
   ```
   Follow [Conventional Commits](https://www.conventionalcommits.org/) (`feat:`, `fix:`, `docs:`, `chore:`, etc.). Append `(#issue)` when applicable.

6. Push and open a PR targeting `dev`.

7. CI will automatically check formatting and run tests.

> **Release flow**: When ready to release, `dev` is merged into `main` and tagged (e.g., `v0.3.0`).

## Reporting Issues

Use the [issue templates](https://github.com/mu5dvlp/qulacs-for-unity/issues/new/choose):

- **Bug Report** — something isn't working
- **Feature Request** — suggest an enhancement
- **Question** — ask about usage or architecture
- **Chore** — maintenance or housekeeping tasks

## Project Structure

```
.
├── Packages/com.mu5dvlp.qulacs/   # The UPM package
│   ├── Runtime/                    # C# public API + P/Invoke
│   ├── Tests/                      # NUnit tests (run via dotnet test)
│   ├── dotnet~/                    # .csproj / .sln for dotnet test
│   ├── Samples~/                   # Sample scenes & scripts
│   ├── native~/                    # C++ wrapper source & build
│   │   ├── src/                    #   qulacs_unity.h / .cpp
│   │   └── extern/                 #   Qulacs & Boost (gitignored)
│   └── Makefile                    # Native build targets
├── .github/workflows/              # CI (test, coverage, lint)
├── Makefile                        # Root targets (format, test, coverage, release)
└── CONTRIBUTING.md                 # ← You are here
```

## License

By contributing, you agree that your contributions will be licensed under the [MIT License](Packages/com.mu5dvlp.qulacs/LICENSE.md).
