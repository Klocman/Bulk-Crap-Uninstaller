# Welcome to the EBUninstaller wiki!

<p align="center">
  <img src="../doc/ebuninstaller_logo.png" alt="EBUninstaller Pro Logo" width="180" />
</p>

<p align="center">
  <strong>EBUninstaller Pro — The Ultimate Open-Source Professional Windows Uninstaller & System Cleanup Suite</strong><br>
  <em>Version 7.0.0 | .NET 8 / Windows 10 & 11 (x64 / ARM64) | Apache License 2.0</em>
</p>

---

## 🌟 Introduction & Overview

Welcome to the official **EBUninstaller Pro** documentation wiki! Whether you are a system administrator looking to automate mass software rollbacks, a power user cleaning persistent bloatware, or a developer exploring our modular C# / .NET 8 architecture, this wiki contains in-depth documentation, guides, and architectural references.

EBUninstaller Pro is an independent, professional-grade open-source Windows uninstaller and system maintenance suite designed to remove software cleanly, safely, and completely without leaving orphaned registry keys, services, or filesystem remnants.

---

## 📚 Table of Contents

### 1. Getting Started
* [Installation & Deployment](#1-installation--deployment)
* [System Requirements](#2-system-requirements)
* [Portable Mode vs. Standard Installation](#3-portable-vs-installed-mode)

### 2. Core Features & Usage
* [Application Discovery Engine & Confidence Scoring](#4-application-discovery-engine)
* [Batch Multi-Uninstall Pipeline](#5-batch-multi-uninstall-pipeline)
* [Forced Uninstall & Stubborn Software Removal](#6-forced-uninstall-engine)
* [Real-Time Installation Monitor & Snapshot Diffs](#7-real-time-installation-monitor)
* [Cryptographic Backup & System Restore Points](#8-backup--system-restore-points)

### 3. System Cleanup & Maintenance Tools
* [Junk & Privacy Cleaner](#9-junk--privacy-cleaner)
* [Empty Folder & Duplicate File Scanner](#10-empty-folder--duplicate-file-scanner)
* [Windows Services Optimizer](#11-windows-services-optimizer)
* [Explorer Context Menu Manager](#12-explorer-context-menu-manager)
* [Windows Optional Features & Capabilities](#13-windows-optional-features--capabilities)
* [File & Process Unlocker (Restart Manager)](#14-file--process-unlocker)
* [Crash Dump & Event Log Cleaners](#15-crash-dump--event-log-cleaners)
* [Software Safety & Bloatware Advisor](#16-software-safety--bloatware-advisor)

### 4. Modern Interface & UX
* [Modern Stats Dashboard & Hygiene Indicator](#17-modern-stats-dashboard)
* [Quick Filter Chips Bar](#18-quick-filter-chips-bar)
* [Theme Customization (Dark/Light & Windows 11 Fluent)](#19-themes--fluent-ui)
* [Multi-Language & RTL Layout (English, German, Arabic)](#20-multilingual--rtl-support)

### 5. Automation & Command Line (CLI)
* [CLI Syntax & Commands](#21-command-line-interface-cli)
* [JSON Automation Pipeline](#22-json-output--automation)

### 6. Development & Build Pipelines
* [Building from Source (.NET 8 SDK)](#23-building-from-source)
* [Creating Inno Setup Installers (ISCC)](#24-installer-compilation)
* [Test Suite & Continuous Integration](#25-test-suite--ci)

---

## 🚀 1. Installation & Deployment

EBUninstaller Pro is distributed in three official packaging formats:

1. **Standalone Windows Installer (`EBUninstallSetup.exe`)**:
   - Modern Inno Setup 6 wizard with administrative privilege elevation (`PrivilegesRequired=admin`).
   - Native 64-bit (`x64compatible` & `ARM64`) architecture support.
   - Built-in multi-language installer with automatic Right-to-Left (RTL) Arabic layout.
2. **Portable Release (`EBUninstaller_Pro_Portable.zip`)**:
   - Zero-installation package. Extract to any USB flash drive or local folder and launch `BCUninstaller.exe`.
   - Stores settings and caches locally (`PortableSettingsProvider`).
3. **Windows Package Managers**:
   ```cmd
   # WinGet
   winget install EhabYT.EBUninstallerPro

   # Chocolatey
   choco install ebuninstaller-pro

   # Scoop
   scoop install ebuninstaller-pro
   ```

---

## 💻 2. System Requirements

* **Operating System**: Windows 11, Windows 10 (Build 18362+ / Version 1903 or later), Windows Server 2022/2019.
* **Architecture**: x64, ARM64, or x86.
* **Runtime**: .NET 8.0 Desktop Runtime (x64/ARM64).
* **Permissions**: Standard user for discovery and monitoring; Administrator (UAC) for registry removal, driver cleanup, and DISM feature management.

---

## 🔍 4. Application Discovery Engine

The discovery engine collects applications across 7 distinct system vectors:
1. **Windows Registry**: 32-bit (`Wow6432Node`) and 64-bit `Uninstall` keys.
2. **Windows Installer (MSI)**: Native `MsiEnumProductsEx` Win32 API.
3. **Universal Windows Platform (UWP / MSIX / Store Apps)**: Windows AppX package manager.
4. **Game Launchers**: Steam, Epic Games, GOG Galaxy, Ubisoft Connect, Origin/EA Desktop, Battle.net.
5. **Windows Package Managers**: WinGet, Chocolatey, Scoop.
6. **Windows Updates**: DISM and Windows Update Agent (WUApi).
7. **Portable Applications**: Smart filesystem sniffer.

### Confidence Scoring Algorithm (0–100%)
Every detected application is assigned a confidence rating based on digital signatures, valid installation directories, registry completeness, and executable headers.

---

## ⚡ 6. Forced Uninstall Engine

When software is damaged, partially removed, or missing its uninstaller executable:
1. Select the target directory or enter the software name.
2. EBUninstaller Pro performs a multi-signal scan matching:
   - Product GUIDs, App Paths, and ProgIDs.
   - Services, scheduled tasks, and autostart keys.
   - AppData, ProgramData, and registry subtrees.
3. Review the topological dependency tree and execute safe removal with SHA-256 backup creation.

---

## 🛡️ 8. Backup & System Restore Points

Before any destructive deletion or registry modification, EBUninstaller Pro provides automated safety nets:
* **Tamper-Proof Manifests**: Every backup archive includes a cryptographic `manifest.json` with SHA-256 digests.
* **Windows System Restore Points**: Automatically triggers VSS system restore points via `root\default:SystemRestore`.
* **Rollback Engine**: 1-click restore for registry trees (`.reg`) and quarantined filesystem files.

---

## 🧹 9–16. System Maintenance & Diagnostic Cleaners

* **Junk File Cleaner**: Cleans Windows Temp, prefetch, thumbnail caches, and setup leftovers.
* **Crash Dump Cleaner**: Identifies and purges kernel memory dumps (`MEMORY.DMP`), minidumps, and Windows Error Reporting (WER) archives.
* **Event Log Cleaner**: Safely clears bloated application, setup, and ETW diagnostic event logs while protecting security audits.
* **Windows Services Optimizer**: Audits 3rd-party services, configures startup modes, and removes orphaned service registrations.
* **Explorer Context Menu Manager**: Disables or purges orphaned shell extension handlers across files, directories, and drive menus.
* **File & Process Unlocker**: Resolves locked file handles using the Windows Restart Manager (`rstrtmgr.dll`).
* **Software Safety Advisor**: AI-heuristics identifying OEM bloatware, PUPs, and trialware with 1-click batch selection.

---

## ⌨️ 21. Command Line Interface (CLI)

EBUninstaller Pro includes a full-featured console engine (`BCU-console.exe` / `EBUninstaller.exe`):

```bash
# List all detected software in JSON
EBUninstaller.exe list --json

# Quiet automated uninstall with leftover cleaning
EBUninstaller.exe uninstall "Application Name" /Q /U /J=VeryGood

# Force remove damaged directory
EBUninstaller.exe forced-uninstall "C:\Program Files\BrokenApp" /U

# Run background junk & privacy cleanup
EBUninstaller.exe clean-junk --clean
EBUninstaller.exe clean-privacy --clean

# List and restore backups
EBUninstaller.exe backup --list
EBUninstaller.exe restore <BackupId> --verify
```

---

## 🛠️ 23. Building from Source

```powershell
# Windows 1-Click Publisher
publish.bat

# Automated PowerShell Release & Test Runner
powershell -ExecutionPolicy Bypass -File .\scripts\build.ps1 -Configuration Release -BuildInstaller

# Linux / macOS Cross-Platform Quality Verification
./scripts/build.sh --verbose
```

---

## 📄 License & Attribution

EBUninstaller Pro is developed and maintained by **EhabYT** (Copyright © 2026 EhabYT. All rights reserved). Licensed under the [Apache License, Version 2.0](../Licence.txt). Original Bulk Crap Uninstaller copyright notices and third-party open-source attributions are preserved in accordance with [NOTICE](../NOTICE).
