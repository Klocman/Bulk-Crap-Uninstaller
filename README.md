# OpenUninstall Pro

[![license](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](Licence.txt)
[![platform](https://img.shields.io/badge/platform-Windows%2010%20%2F%2011%20%28x64%20%7C%20ARM64%29-brightgreen.svg)]()
[![dotnet](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)

**OpenUninstall Pro** is a modern, high-performance, professional-grade open-source Windows uninstaller, leftover cleaner, and system maintenance suite. Built on .NET 8 and native Windows APIs, it delivers a comprehensive, independent alternative to commercial uninstallers with advanced forced removal, real-time installation monitoring, cryptographic backups, junk and privacy cleaning, browser extension management, and startup optimization.

---

## Key Features

### 1. Advanced Application Discovery & Confidence Engine
* **Comprehensive Multi-Source Detection**: Scans 32-bit and 64-bit Windows Uninstall Registry keys (HKLM & HKCU), MSI databases, Windows Store / AppX / MSIX packages, Steam, Epic Games Store, GOG Galaxy, Ubisoft Connect, EA Origin, Battle.net, WinGet, Scoop, Chocolatey, and portable applications.
* **Confidence Scoring**: Evaluates installation paths, digital signatures (Authenticode & X509 certificates), executable integrity, and registration signals to score discovered applications (0-100).

### 2. Unified 10-Phase Uninstallation Pipeline
* **Safe, Automated Workflow**:
  `Discover -> Validate -> Backup -> Restore Point -> Execute Uninstaller -> Await -> Re-Scan -> Detect Leftovers -> Classify -> Removal Preview -> Verify Removal -> Audit Report`
* **Supports Silent, Loud, Batch & Unattended Modes**: Automates quiet uninstallation parameters across Inno Setup, NSIS, MSI, InstallShield, Store Apps, and game launchers.
* **Reboot Detection**: Tracks post-uninstall system reboot requirements (exit code 3010).

### 3. Deep Forced Removal System
* Removes stubborn, corrupted, partially deleted, or broken applications without relying on a functional built-in uninstaller.
* Scans target folders, registry hives, user AppData, ProgramData, LocalLow, shortcuts, Windows Services, and scheduled tasks.
* Categorizes leftovers by confidence level (High, Medium, Low) and creates a pre-removal backup manifest before deletion.

### 4. Real-Time Installation Monitor & Snapshot Diff Engine
* **Live Installation Tracing**: Hooks `FileSystemWatcher` and registry polling during setup executions to capture all created, modified, and unlinked files and registry keys in real-time.
* **Before / After Snapshots**: Compares point-in-time system snapshots to generate exact diffs (`Added`, `Modified`, `Removed`).
* **Trace Replay / Rollback**: Completely uninstalls software by replaying recorded installation traces.

### 5. Cryptographic Backup & Recovery Center
* Automatic pre-removal backups with structured manifests (`manifest.json`), registry exports (`.reg`), and compressed file archives (`files.zip`).
* **SHA-256 Verification**: Verifies cryptographic checksums using constant-time comparison to ensure archive integrity before restoration.
* **System Restore Points**: Integrates with Windows System Restore (WMI/Srp) to capture system restore points prior to uninstallation.

### 6. System Junk & Privacy Cleaners
* **Junk Cleaner**: Scans and cleans user temp (`%TEMP%`), Windows temp (`%WINDIR%\Temp`), Windows Update download caches (`SoftwareDistribution\Download`), crash dumps, minidumps, system logs, thumbnail caches, browser caches, and the Recycle Bin.
* **Privacy Cleaner**: Scans and clears browsing history, download records, cookies, and session data for Google Chrome, Microsoft Edge, Mozilla Firefox, Brave Browser, and Opera, as well as Windows Recent Items and Jump Lists.

### 7. Browser Extension Manager
* Scans, inspects, and manages browser extensions across Chrome, Edge, Firefox, Brave, Opera, and Vivaldi.
* Displays manifest details, permissions, version numbers, publisher information, and allows safe extension folder unlinking.

### 8. Windows Tools Hub & Startup Manager
* **Windows Tools Hub**: Direct, safe launcher for essential built-in Windows administration tools (Task Manager, Services, Device Manager, Event Viewer, Registry Editor, Disk Management, System Information, Optional Features, PowerShell, Terminal, etc.).
* **Startup Manager**: Discovers, inspects, and manages autostart entries across user and system startup folders, Registry `Run` / `RunOnce` keys, Task Scheduler tasks, and Windows Services.

### 9. Secure File & Folder Shredder
* Normal Recycle Bin deletion, permanent unlinking, Zero-Fill shredding (1-pass), and DoD 5220.22-M multi-pass overwriting (3-pass).
* Includes transparent technical disclosures regarding SSD / TRIM / Copy-on-Write storage behavior.

### 10. Powerful Command-Line Interface (CLI)
* Full automation via `BCU-console.exe`: `list`, `uninstall`, `forced-uninstall`, `scan`, `leftovers`, `backup`, `restore`, `monitor`, `rollback-trace`, `clean-junk`, `clean-privacy`, `startup`, `extensions`, `tools`, `export`, and `history`.
* Supports machine-readable `--json` output, quiet `/Q` mode, unattended `/U` mode, and standard automation exit codes.

---

## Architecture Overview

```
/source
  /BCU-console               # Professional Command-Line Interface (CLI)
  /BulkCrapUninstaller       # Modern Windows Desktop Application (WinForms)
  /BulkCrapUninstallerTests  # MSTest Unit, Integration, and Lifecycle Test Suite
  /UninstallTools            # Core uninstaller engine and domain libraries
    /Core                    # Security, logging, hashing, digital signatures
    /Detection               # Discovery factories (Registry, Store, Game Launchers, WinGet)
    /Uninstaller             # Unified 10-phase uninstallation pipeline
    /ForcedRemoval           # Deep forced removal manager and planner
    /Leftovers               # Advanced residual file and registry scanner
    /RegistryEngine          # Safe registry engine (.reg export, 32/64-bit views)
    /FileSystemEngine        # Safe file engine with secure shredder
    /Backup                  # Backup manager with cryptographic SHA-256 verification
    /InstallationMonitor     # Snapshot engine and real-time live monitoring
    /HunterMode              # Window crosshair target mode
    /JunkCleaner             # System temporary files and browser cache cleaner
    /PrivacyCleaner          # Browser and Windows privacy history cleaner
    /BrowserExtensions       # Chromium & Firefox extension manager
    /SystemTools             # Windows administrative tools hub
    /Exclusions              # Whitelist and rule-based exclusion engine
    /History                 # Operation audit history manager
```

---

## Documentation Index

Detailed engineering documentation is available in the `/docs` directory:

* [Architecture Overview](docs/ARCHITECTURE.md)
* [Security Model & Denylists](docs/SECURITY.md)
* [Build Instructions](docs/BUILD.md)
* [Development Guide](docs/DEVELOPMENT.md)
* [Testing & Lifecycle Test Suite](docs/TESTING.md)
* [Unified Uninstallation Pipeline](docs/UNINSTALL_ENGINE.md)
* [Leftover Detection Engine](docs/LEFTOVER_SCANNER.md)
* [Installation Monitor & Snapshots](docs/INSTALLATION_MONITOR.md)
* [Backup & Recovery System](docs/BACKUP_SYSTEM.md)
* [Command-Line Interface (CLI) Reference](docs/CLI.md)
* [Contributing Guidelines](docs/CONTRIBUTING.md)
* [Third-Party Software Licenses](docs/THIRD_PARTY_LICENSES.md)

---

## Building from Source

### Prerequisites
* Windows 10 / 11 (x64 or ARM64)
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
* Visual Studio 2022 (v17.8+) or Visual Studio Code

### Build Commands
```bash
# Compile solution in Release mode
dotnet build source/BulkCrapUninstaller.sln -c Release /p:Platform=AnyCPU

# Run test suite
dotnet test source/BulkCrapUninstallerTests/BulkCrapUninstallerTests.csproj

# Run automated build script
powershell -File scripts/build.ps1 -Configuration Release -Platform AnyCPU
```

---

## License

OpenUninstall Pro is licensed under the [Apache License, Version 2.0](Licence.txt). Original Bulk Crap Uninstaller copyright notices and third-party open-source attributions are preserved in accordance with [NOTICE](NOTICE) and [THIRD_PARTY_LICENSES.md](docs/THIRD_PARTY_LICENSES.md).
