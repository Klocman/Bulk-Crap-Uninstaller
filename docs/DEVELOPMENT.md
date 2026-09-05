# Development Guide

This guide outlines coding standards, repository structure, and contribution conventions for EBUninstaller Pro.

---

## 1. Solution Structure

```
/source
  /BCU-console               # Command-Line Interface (CLI)
  /BulkCrapUninstaller       # Modern Windows Desktop Application (WinForms)
  /BulkCrapUninstallerTests  # MSTest Unit and Integration Tests Suite
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
  /KlocTools                 # Foundational Win32 and system utility helpers
```

---

## 2. Coding Conventions

* **Target Framework**: `.NET 8.0-windows10.0.18362.0`.
* **Language**: C# 12.
* **Architecture Rules**:
  * Never bypass `SecurityGuard` before deleting files or registry keys.
  * Always create pre-removal backups before destructive operations.
  * Always use asynchronous scanning (`async`/`await`) with `CancellationToken` support.
  * Never block the UI thread during disk or registry scans.
  * Maintain clean separation between UI controls and domain business logic.
