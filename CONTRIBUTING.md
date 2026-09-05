# Contributing to EBUninstaller Pro

Thank you for your interest in contributing to **EBUninstaller Pro**! We welcome contributions, bug reports, feature enhancements, documentation improvements, and translations.

---

## 🛠️ Development Workflow & Setup

### Prerequisites
* **Windows 10 / 11** (x64 or ARM64)
* **Visual Studio 2022** (v17.8 or higher) with the **.NET Desktop Development** workload, or **JetBrains Rider** / **VS Code** with C# Dev Kit.
* **.NET 8.0 SDK** (targeting `net8.0-windows10.0.18362.0`).
* **Inno Setup 6** (for compiling the setup installer).

### Building the Project
```powershell
# Clone the repository
git clone -b arena/01a06ec7-bulk-crap-uninstaller https://github.com/EhabYT/Bulk-Crap-Uninstaller.git
cd Bulk-Crap-Uninstaller

# Build via PowerShell script
.\scripts\build.ps1 -Configuration Release -Platform AnyCPU -BuildInstaller
```

### Running the Test Suite
```powershell
dotnet test source\BulkCrapUninstallerTests\BulkCrapUninstallerTests.csproj --configuration Release
```

---

## 📐 Architecture Guidelines

EBUninstaller Pro is organized into modular subsystems under `source\UninstallTools`:
* **`Core/`**: Security guard, least-privilege enforcement, cryptography (`CryptoHasher`), signature verification (`DigitalSignatureVerifier`), and structured logging.
* **`Detection/`**: Application discovery, game launcher detection, package managers, and confidence scoring.
* **`Uninstaller/`**: Unified 10-phase uninstallation pipeline.
* **`ForcedRemoval/`**: Deep scanner and multi-signal confidence engine for corrupted uninstaller removal.
* **`InstallationMonitor/`**: User-mode file system and registry monitoring with snapshot diffs.
* **`Backup/`**: Cryptographic SHA-256 backup creation, verification, and restoration engine.
* **`JunkCleaner/`**: Windows temp, cache, log, crash dump, and update leftovers cleanup.
* **`PrivacyCleaner/`**: Multi-browser history, cookie, and cache privacy purger.
* **`BrowserExtensions/`**: Extension management for Chrome, Edge, Firefox, Brave, and Opera.
* **`Localization/`**: Multi-language support with automatic RTL mirroring (English, German, Arabic).

---

## 🔒 Security Standards

1. **Safety First**: Never bypass `SecurityGuard.IsProtectedPath()` or `SecurityGuard.IsProtectedRegistryKey()`. Protected system directories (e.g. `System32`, `WinSxS`, Windows registry roots) must never be deleted.
2. **Offline & Privacy First**: No telemetry, analytics, or hidden cloud communication.
3. **No Proprietary Code**: Do not copy proprietary source code, algorithms, or assets from commercial uninstallers.

---

## 🤝 Pull Request Process

1. Fork the repository and create a descriptive branch.
2. Ensure all C# code is cleanly formatted and builds with zero warnings.
3. Run the static verification script: `python3 scripts/verify_repo.py`.
4. Ensure all unit and integration tests pass.
5. Open a Pull Request with a detailed description of changes and test results.
