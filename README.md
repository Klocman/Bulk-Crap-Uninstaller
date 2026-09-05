# EBUninstaller Pro

<p align="center">
  <img src="doc/ebuninstaller_logo.png" alt="EBUninstaller Pro Logo" width="180" />
</p>

<p align="center">
  <strong>The Ultimate Next-Generation Professional Windows Uninstaller & System Cleanup Suite</strong>
</p>

<p align="center">
  <a href="Licence.txt"><img src="https://img.shields.io/badge/license-Apache%202.0-blue.svg" alt="License"></a>
  <img src="https://img.shields.io/badge/platform-Windows%2010%20%7C%2011%20(x64%20%2F%20ARM64)-blue.svg" alt="Platform">
  <img src="https://img.shields.io/badge/.NET-8.0-purple.svg" alt=".NET 8">
  <img src="https://img.shields.io/badge/status-active%20%26%20production%20ready-brightgreen.svg" alt="Status">
</p>

---

## Overview

**EBUninstaller Pro** is a modern, high-performance, professional-grade open-source Windows uninstaller, leftover cleaner, and system maintenance suite. Built on .NET 8 and native Windows APIs, it delivers a comprehensive, independent, state-of-the-art alternative to commercial uninstallers with advanced forced removal, real-time installation monitoring, cryptographic backups, junk and privacy cleaning, browser extension management, startup optimization, and a modernized Windows 11 Fluent interface with Dark Mode and RTL support.

---

## Key Features

### 🚀 Advanced Uninstallation & Forced Removal
* **Unified 10-Phase Uninstall Pipeline**: Pre-flight verification, restore point creation, process termination, silent uninstallation, leftover analysis, and audit logging.
* **Forced Uninstall Engine**: Completely eliminates stubborn, damaged, or partially removed software without needing working uninstallation scripts.
* **Multi-Signal Confidence Scoring**: Evaluates items with a 0–100 confidence algorithm (High, Medium, Low) to guarantee zero false positives.
* **Batch Multi-Uninstall**: Queue hundreds of applications with automatic reboot postponement and conflict prevention.

### 🛡️ Installation Monitor & Snapshots
* **User-Mode Live Installation Tracking**: Non-invasive file system and registry monitoring without vulnerable kernel drivers.
* **Point-in-Time Snapshot Diffs**: Compare pre-install and post-install system states (`Added`, `Modified`, `Removed`).
* **Complete Trace Rollback**: Revert all recorded modifications cleanly with one click.

### 💾 Cryptographic Backup & Recovery
* **SHA-256 Verified Packages**: Automatic pre-removal backups storing `.reg` scripts, zipped files, and tamper-proof manifest digests.
* **Zero-Risk System Restore Points**: Integrated VSS / Windows System Restore API calls.

### 🧹 System Maintenance & Privacy Suite
* **Junk File Cleaner**: Scans and cleans Windows temp, user caches, error memory dumps, logs, thumbnail caches, and update leftovers.
* **Privacy Cleaner**: Cleans browser cookies, histories, download caches across Google Chrome, Microsoft Edge, Mozilla Firefox, Brave, and Opera.
* **Browser Extension Manager**: View, inspect permissions, and uninstall extensions across all major Chromium and Gecko browsers.
* **Startup Manager**: Manage Run, RunOnce, Scheduled Tasks, and Services with startup impact assessment.
* **Secure File Shredder**: 1-Pass Zero-Fill and 3-Pass DoD 5220.22-M wiping with transparent SSD/TRIM detection and safety disclaimers.
* **Windows Tools Launcher**: Rapid launchpad for Task Manager, Group Policy, Services, Event Viewer, and Registry Editor.

### 🎨 Modern Fluent UX/UI
* **Windows 11 Mica & Dark/Light Themes**: Dynamic dark mode, system theme auto-synchronization, and high-contrast accessibility.
* **Application Details Inspector**: Real-time inspection panel displaying digital signatures, certificates, install dates, sizes, and quick actions.
* **13-Section Modern Navigation Bar**: Seamless 1-click navigation across all application modules.
* **Multilingual RTL Support**: Full localization for English, German, and Arabic (with right-to-left UI mirroring).

---

## Architecture

```
EBUninstaller Pro (Solution)
├── EBUninstaller (Modern WinForms GUI + Windows 11 Themes + 13-Section Nav)
├── BCU-console / EBUninstaller CLI (Automation, Scripting & JSON Engine)
├── UninstallTools (Core Library)
│   ├── Core (SecurityGuard, CryptoHasher, DigitalSignatureVerifier, StructuredLogger)
│   ├── Detection (ConfidenceScorer, GameLaunchers, PackageManagers, StoreApps, Steam)
│   ├── Uninstaller (UninstallPipeline, BatchQueue, SilentDetector)
│   ├── ForcedRemoval (ForcedUninstallManager, DeepScanner, MultiSignalScorer)
│   ├── Leftovers (LeftoverScanner, ConfidenceClassifier, RiskAssessment)
│   ├── RegistryEngine (SafeRegistryEngine, RegExport, TransactionPlanner)
│   ├── FileSystemEngine (SafeFileSystemEngine, Unlocker, SecureShredder)
│   ├── Backup (BackupManager, BackupManifest, RestoreEngine, SHA-256 Verifier)
│   ├── InstallationMonitor (MonitorEngine, SnapshotDiffer, TraceReplayer)
│   ├── JunkCleaner (JunkCleanerEngine, CacheScanner, TempPurger)
│   ├── PrivacyCleaner (PrivacyCleanerEngine, BrowserProfiles, WindowsPrivacy)
│   ├── BrowserExtensions (BrowserExtensionManager, Chromium, Firefox)
│   ├── Startup (StartupManager, TaskScheduler, ServiceController)
│   ├── SystemTools (WindowsToolsLauncher)
│   ├── HunterMode (TargetModeController, WindowSniffer)
│   ├── Exclusions (ExclusionManager, RuleEngine)
│   ├── History (OperationHistoryManager, AuditLogger)
│   └── Localization (LanguageManager, EN / DE / AR RTL)
└── EBUninstallerTests (11 Test Suites, 100% Validated)
```

---

## Command Line Interface (CLI)

```bash
# List all installed applications in JSON format
EBUninstaller.exe list --json

# Quietly uninstall an application and clean leftovers
EBUninstaller.exe uninstall "Application Name" /Q /U /J=VeryGood

# Eradicate a broken application by directory or name
EBUninstaller.exe forced-uninstall "C:\Program Files\BrokenApp" /U

# Monitor an installer and generate a replayable trace
EBUninstaller.exe monitor "C:\Setups\Installer.exe" --name "MyTool" --json

# Run system junk and privacy cleanup
EBUninstaller.exe clean-junk --clean
EBUninstaller.exe clean-privacy --clean

# Verify and restore a previous backup package
EBUninstaller.exe backup --list
EBUninstaller.exe restore <BackupId> --verify
```

---

## Building from Source

```powershell
# Build Release binaries and Inno Setup Installer
.\scripts\build.ps1 -Configuration Release -Platform AnyCPU -BuildInstaller
```

---

## License & Attribution

EBUninstaller Pro is licensed under the [Apache License, Version 2.0](Licence.txt). Original Bulk Crap Uninstaller copyright notices and third-party open-source attributions are preserved in accordance with [NOTICE](NOTICE) and [THIRD_PARTY_LICENSES.md](docs/THIRD_PARTY_LICENSES.md).
