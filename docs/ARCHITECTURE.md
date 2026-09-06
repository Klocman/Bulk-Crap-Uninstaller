# EBUninstaller Pro Architecture

EBUninstaller Pro is an independent, high-performance, modular Windows uninstaller and system maintenance application built on .NET 8, C#, and native Windows APIs.

---

## 1. High-Level Architectural Diagram

```
+-------------------------------------------------------------------------+
|                              User Interface                             |
|  (Applications View | Batch Pipeline | Hunter Target Mode | Dark Theme)  |
|  (Forced Removal | Monitor | Backup Center | Junk & Privacy | Extensions)|
+------------------------------------+------------------------------------+
                                     |
+------------------------------------+------------------------------------+
|                       Command Line Interface (CLI)                      |
|       (list | uninstall | forced-uninstall | monitor | clean | backup)     |
+------------------------------------+------------------------------------+
                                     |
+------------------------------------+------------------------------------+
|                   Core Application & Domain Engine                      |
+------------------+-----------------+-------------------+----------------+
|  Discovery Engine| Uninstall Engine| Leftover Scanner  | Forced Removal |
|  - Registry Views| - 10-Phase Pipe | - Multi-Hive Scan | - Deep Scanner |
|  - MSI / StoreApp| - Silent Daemon | - Signal Scoring  | - Risk Planner |
|  - Games / WinGet| - Automation    | - Denylist Guard  | - Verified Exec|
+------------------+-----------------+-------------------+----------------+
| Install Monitor  | Backup & Restore| Cleaners & Tools  | Extensions & SM|
| - Snapshots Diff | - Pre-Manifest  | - Junk Cleaner    | - Chrome/Edge  |
| - FSW Live Trace | - Cryptographic | - Privacy Tracks  | - Firefox/Opera|
| - Rollback Replay| - Restore Points| - Secure Shredder | - Startup Mgr  |
+------------------+-----------------+-------------------+----------------+
                                     |
+------------------------------------+------------------------------------+
|               Safe Registry & File System Infrastructure                |
|  - 32/64-bit Registry Views   - .reg Pre-Export   - Permission Handler  |
|  - Reparse Point / Symlink Guard - TOCTOU Mitigation - SSD / TRIM Guard |
+------------------------------------+------------------------------------+
                                     |
+------------------------------------+------------------------------------+
|                      Native Windows Integration                         |
|  - WinTrust Authenticode  - Shell32 / CSIDL   - ServiceControl Manager  |
|  - Task Scheduler COM     - System Restore WMI - Win32 Window Hover     |
+-------------------------------------------------------------------------+
```

---

## 2. Subsystem Descriptions

### 2.1 Core Subsystem (`UninstallTools.Core`)
* **`SecurityGuard`**: Enforces strict denylists against deleting critical system paths (`%WINDIR%`, `System32`, `SysWOW64`, `WinSxS`, driver directories, root drives) and critical registry hives (`HKLM\SAM`, `HKLM\SECURITY`, `HKLM\SYSTEM\CurrentControlSet`). Provides path canonicalization and command injection sanitization.
* **`StructuredLogger`**: High-performance, thread-safe memory ring-buffer and file logger with level classification (Trace, Debug, Info, Warning, Error, Critical) and automatic redaction of sensitive credentials.
* **`CryptoHasher`**: Cryptographic file hashing using SHA-256 for backup manifests, file integrity checks, and constant-time verification.
* **`DigitalSignatureVerifier`**: WinTrust / WinVerifyTrust and X509 certificate parsing for PE binaries.

### 2.2 Application Discovery (`UninstallTools.Detection`)
Discovers software across multiple sources:
* **Registry**: 32-bit and 64-bit views of HKLM and HKCU uninstall keys.
* **MSI**: Windows Installer COM APIs and product GUIDs.
* **Store Apps**: AppX and MSIX package registrations.
* **Game Launchers**: Steam, Epic Games Store, GOG Galaxy, Ubisoft Connect, EA Desktop / Origin, and Battle.net.
* **Package Managers**: WinGet, Scoop, and Chocolatey.
* **Portable Applications**: Discovered from custom folders and system drives.
* **Confidence Scoring**: Multi-factor scoring (0-100) evaluating digital signatures, install locations, executable presence, and registration status.

### 2.3 Uninstallation Pipeline (`UninstallTools.Uninstaller`)
Unified 10-phase pipeline:
1. **Discover & Validate**: Parameter sanity checks and protection status verification.
2. **Create Backup**: Manifest creation, `.reg` export, file archiving.
3. **Create Restore Point**: Optional Windows System Restore Point via WMI.
4. **Execute Official Uninstaller**: Silent or loud uninstaller execution.
5. **Wait for Completion**: Process monitoring with timeout and reboot detection (exit code 3010).
6. **Re-Scan System**: Deep scan for remaining files, folders, and registry keys.
7. **Classify & Score**: Confidence scoring and risk level assignment.
8. **Preview & User Approval**: Review of detected items before deletion.
9. **Remove Approved Items**: Safe deletion of residual items.
10. **Verify & Report**: Final verification and audit logging.

### 2.4 Forced Removal Subsystem (`UninstallTools.ForcedRemoval`)
* Multi-location scanner targeting broken, corrupted, or missing uninstallers.
* Analyzes application names, publisher names, and folder paths across AppData, ProgramData, LocalLow, Registry, Services, and Shortcuts.
* Assigns confidence levels (High, Medium, Low) and generates a structured removal plan with mandatory pre-removal backup.

### 2.5 Real-Time Installation Monitor (`UninstallTools.InstallationMonitor`)
* **Before / After Snapshots**: Captures registry keys, file metadata, services, tasks, startup items, and environment variables.
* **Live Monitoring**: Hooks `FileSystemWatcher` during installer process execution.
* **Diff Engine**: Computes exact diffs (`Added`, `Modified`, `Removed`).
* **Trace Management**: Saves traces in versioned format and supports one-click trace rollback / clean removal.

### 2.6 Backup and Recovery (`UninstallTools.Backup`)
* Structured backup manifests recording operations, timestamps, file sizes, and cryptographic SHA-256 checksums.
* Registry export to `.reg` format and file archiving to compressed ZIP files.
* Backup verification and full one-click rollback/restore.

### 2.7 System Cleaners (`UninstallTools.JunkCleaner` & `PrivacyCleaner`)
* **Junk Cleaner**: Scans and cleans Windows Temp, User Temp, Windows Update caches, crash dumps, logs, thumbnails, browser caches, and recycle bin.
* **Privacy Cleaner**: Scans and clears browsing history, cookies, download history, and session data for Chrome, Edge, Firefox, Brave, and Opera, plus Windows Recent Items and Jump Lists.

### 2.8 Browser Extension Manager (`UninstallTools.BrowserExtensions`)
* Detects extensions for Google Chrome, Microsoft Edge, Mozilla Firefox, Brave, Opera, and Vivaldi.
* Parses `manifest.json`, resolves localized names, inspects permissions, and allows safe removal.

### 2.9 Windows Tools Hub (`UninstallTools.SystemTools`)
* Direct, secure launcher for essential Windows administration tools (Task Manager, Services, Device Manager, Event Viewer, Regedit, Disk Management, System Information, PowerShell, Terminal, etc.).
