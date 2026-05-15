# Contributing to Qulacs for Unity

Thank you for your interest in contributing! This guide covers how to set up your environment, follow code conventions, and submit changes.

## Prerequisites

| Tool | Version | Purpose |
|---|---|---|
| Unity | 6000.4.1f1 LTS | Project editor |
| .NET SDK | 8.0+ | C# formatter (CSharpier) |
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
# Unity EditMode tests (Windows)
"C:\Program Files\Unity\Hub\Editor\6000.4.1f1\Editor\Unity.exe" \
  -projectPath . -runTests -testPlatform editmode -quit -batchmode -logFile test.log

# Check test.log for results
```

## Branch Strategy

| Branch | Purpose |
|---|---|
| `main` | Stable releases |
| `dev` | Active development |
| Feature branches | Branch from `dev`, merge back to `dev` |

## Submitting a Pull Request

1. Create a feature branch from `dev`:
   ```bash
   git checkout dev
   git pull origin dev
   git checkout -b feature/your-feature
   ```

2. Make your changes.

3. Format and verify:
   ```bash
   make format
   ```

4. Run tests to ensure nothing is broken.

5. Commit with a clear message:
   ```
   feat: add RY gate support
   ```
   Follow [Conventional Commits](https://www.conventionalcommits.org/) where possible (`feat:`, `fix:`, `docs:`, `chore:`, etc.).

6. Push and open a PR targeting `dev`.

7. CI will automatically check formatting and run tests.

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
│   ├── Tests/                      # EditMode tests
│   ├── Samples~/                   # Sample scenes & scripts
│   ├── native~/                    # C++ wrapper source & build
│   │   ├── src/                    #   qulacs_unity.h / .cpp
│   │   └── extern/                 #   Qulacs & Boost (gitignored)
│   └── Makefile                    # Native build targets
├── .github/workflows/              # CI (test, coverage, lint)
├── Makefile                        # Root targets (format, coverage, release)
└── CONTRIBUTING.md                 # ← You are here
```

## License

By contributing, you agree that your contributions will be licensed under the [MIT License](Packages/com.mu5dvlp.qulacs/LICENSE.md).
