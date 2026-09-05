# Unified Uninstallation Engine

EBUninstaller Pro implements a unified 10-phase pipeline for safely and completely uninstalling software on Windows.

---

## 1. The 10-Phase Pipeline

```
Phase 1: Discover & Validate
  │  - Validates application properties, install paths, and protection status
  ▼
Phase 2: Pre-Operation Backup
  │  - Exports associated registry keys to .reg format
  │  - Archives application data files to compressed ZIP
  │  - Computes cryptographic SHA-256 checksums
  ▼
Phase 3: System Restore Point
  │  - Creates an official Windows System Restore Point via WMI/Srp
  ▼
Phase 4: Run Official Uninstaller
  │  - Runs quiet/silent uninstaller (MSI, Inno, NSIS, InstallShield, AppX, Store, WinGet, Scoop)
  ▼
Phase 5: Wait for Completion
  │  - Monitors process exit code; detects reboot requirements (ExitCode 3010)
  ▼
Phase 6: Re-Scan System
  │  - Deep leftover scan across Registry hives, AppData, ProgramData, and Shortcuts
  ▼
Phase 7: Classify & Score
  │  - Evaluates leftover confidence (High, Medium, Low) and risk levels
  ▼
Phase 8: Preview & User Approval
  │  - Presents findings to the user for interactive approval
  ▼
Phase 9: Remove Approved Items
  │  - Deletes approved residual files, folders, and registry keys safely
  ▼
Phase 10: Verify & Report
     - Generates audit report and records operation in persistent History database
```

---

## 2. Supported Uninstaller Types

* **MSI (Windows Installer)**: Uninstallation via `msiexec.exe /x {GUID} /qn`.
* **Inno Setup**: Silent uninstallation via `/VERYSILENT /SUPPRESSMSGBOXES /NORESTART`.
* **NSIS**: Silent uninstallation via `/S`.
* **InstallShield**: Uninstallation with response files.
* **Store Apps (AppX / MSIX)**: PowerShell package removal via `Remove-AppxPackage`.
* **Game Launchers**: Steam, Epic Games Store, GOG Galaxy, Ubisoft Connect, EA Origin.
* **Package Managers**: WinGet (`winget uninstall --id <ID>`), Scoop, Chocolatey.
* **Broken / Missing Uninstallers**: Automated fallback to **Forced Removal Engine**.
