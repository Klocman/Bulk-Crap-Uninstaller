# EBUninstaller Pro v7.0.0 — Official Release

<p align="center">
  <img src="doc/ebuninstaller_logo.png" alt="EBUninstaller Pro Logo" width="160" />
</p>

## 🚀 Welcome to EBUninstaller Pro v7.0.0

**EBUninstaller Pro** is the next-generation, high-performance, professional-grade Windows uninstaller and comprehensive system cleanup suite. Built on .NET 8 and native Windows APIs, it delivers a state-of-the-art independent alternative to commercial uninstallers.

---

### ✨ Highlights & Key Features

#### 🛡️ Advanced Uninstallation & Forced Removal
* **Unified 10-Phase Pipeline**: Pre-flight checks, System Restore point creation, process termination, silent uninstallation, leftover analysis, and audit logging.
* **Forced Uninstall Engine**: Completely eliminates stubborn or corrupted software without working uninstaller binaries.
* **Multi-Signal Confidence Scoring**: Evaluates items with a 0–100 confidence algorithm (High, Medium, Low) to prevent accidental deletions.
* **Batch Multi-Uninstall**: Queue hundreds of applications with automatic reboot postponement and conflict handling.

#### ⏱️ Installation Monitor & Snapshot Diffs
* **User-Mode Live Installation Tracking**: Non-invasive file system and registry monitoring without vulnerable kernel drivers.
* **Point-in-Time Snapshot Diffs**: Compare pre-install and post-install system states (`Added`, `Modified`, `Removed`).
* **1-Click Trace Rollback**: Revert all recorded modifications cleanly.

#### 💾 Cryptographic Backup & Recovery Center
* **SHA-256 Verified Packages**: Automatic pre-removal backups storing `.reg` scripts, zipped files, and tamper-proof manifest digests under `%LOCALAPPDATA%\EBUninstallerPro\Backups\`.
* **Zero-Risk System Restore Points**: Integrated VSS / Windows System Restore API.

#### 🧹 System Maintenance & Privacy Suite
* **System Junk Cleaner**: Scans and cleans Windows temp, user caches, error dumps, logs, thumbnail caches, and update leftovers.
* **Privacy Cleaner**: Cleans browser cookies, histories, download caches across Google Chrome, Microsoft Edge, Mozilla Firefox, Brave, and Opera.
* **Browser Extension Manager**: Inspect permissions and remove extensions across all major Chromium and Gecko browsers.
* **Startup Manager**: Manage Run, RunOnce, Scheduled Tasks, and Services with startup impact assessment.
* **Software Health & Hygiene Advisor**: Calculates a System Hygiene Score (0-100%) and identifies duplicate runtimes (Visual C++, Java) and space hogs (> 5 GB).
* **Registry Optimizer & Repair**: Scans and repairs broken App Paths, dead SharedDLL references, and orphaned MUI caches.
* **Secure File Shredder**: 1-Pass Zero-Fill and 3-Pass DoD 5220.22-M wiping with transparent SSD/TRIM detection.
* **Windows Tools Launcher**: Rapid launchpad for Task Manager, Group Policy, Services, Event Viewer, and Registry Editor.

#### 🎨 Modern Fluent UX/UI
* **Windows 11 Mica & Dark/Light Themes**: Dynamic dark mode, system theme auto-synchronization, and immersive title bars.
* **Application Details Inspector**: Real-time inspection panel displaying digital signatures, certificates, install dates, sizes, and quick actions.
* **13-Section Modern Navigation Bar**: Seamless 1-click navigation across all application modules.
* **Multilingual RTL Support**: Full localization for English, German, and Arabic (with right-to-left UI mirroring).

---

### 📦 Release Assets & Downloads

| Asset | Format | Description |
| :--- | :--- | :--- |
| **`EBUninstaller_Pro_v7.0.0_Setup.exe`** | Inno Setup Installer | Standard installer with desktop shortcuts and shell integration |
| **`EBUninstaller_Pro_Portable.zip`** | ZIP Archive | Fully portable, zero-install edition for USB drives |
| **`SHA256SUMS.txt`** | Text file | Cryptographic SHA-256 checksums for binary verification |

---

### 🛠️ Building the Release Locally

To build both the Setup Installer and the Portable package on Windows:

```powershell
# Run the automated build and release script
.\scripts\build.ps1 -Configuration Release -Platform AnyCPU -BuildInstaller
```

The compiled binaries and installer will be located in the `build\` folder:
* `build\installer\EBUninstaller_Pro_v7.0.0_Setup.exe`
* `build\portable\EBUninstaller_Pro_Portable.zip`
