# Contributing to EBUninstaller Pro

Thank you for your interest in contributing to EBUninstaller Pro! We welcome community contributions, bug reports, and feature pull requests.

---

## 1. Development Workflow

1. Fork the repository on GitHub.
2. Clone your fork locally.
3. Ensure you have the .NET 8 SDK installed.
4. Create a feature branch: `git checkout -b feature/my-feature`.
5. Implement your changes following the architecture guidelines in `ARCHITECTURE.md`.
6. Add unit and integration tests under `source/BulkCrapUninstallerTests/`.
7. Verify your build passes and all tests succeed:
   ```bash
   dotnet build source/BulkCrapUninstaller.sln -c Release
   dotnet test source/BulkCrapUninstallerTests/BulkCrapUninstallerTests.csproj
   python3 scripts/verify_repo.py
   ```
8. Commit your changes with clear, descriptive commit messages.
9. Open a Pull Request on GitHub.

---

## 2. Safety Rules for Code Contributions

* **Always prioritize safety**: Never introduce unvalidated deletions or wildcards in file system / registry code.
* **Respect the Security Model**: Operations on files and registry keys must pass through `SecurityGuard` validations.
* **No Telemetry**: Do not add tracking, telemetry, or unnecessary external network dependencies.
* **Offline First**: All core uninstaller functionality must work without an internet connection.
