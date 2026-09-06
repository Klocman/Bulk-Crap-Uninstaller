# Real-Time Installation Monitor & Snapshots

EBUninstaller Pro includes a safe, user-mode installation monitoring engine and point-in-time snapshot diff system.

---

## 1. Operating Modes

### 1.1 Live Installation Monitor
1. User selects an installer executable (`.exe` or `.msi`).
2. The engine takes a pre-installation snapshot of the system.
3. `FileSystemWatcher` instances hook target directories (`Program Files`, `ProgramData`, `AppData`, `Desktop`, `Start Menu`).
4. The installer process is executed.
5. Live file creation, modification, and deletion events are streamed in real-time.
6. Once the installer process exits, a post-installation snapshot is captured.
7. The engine produces a consolidated `InstallationTrace` (`.trace` / `.json`).

### 1.2 Point-in-Time Snapshot Diffing
* Captures and compares snapshots across:
  * Registry subtrees (Uninstall, Run, App Paths, Software keys)
  * File metadata and directory structures
  * Installed Windows Services
  * Scheduled Tasks
  * Startup entries
  * Environment variables
* Generates an exact diff report categorized by `Added`, `Modified`, and `Removed`.

---

## 2. Trace Replay & Rollback

Every saved installation trace can be replayed to achieve a clean uninstallation:
* Files and directories added during setup are unlinked safely to the Recycle Bin or shredded.
* Registry values and keys created during setup are removed.
* Any registered services created during setup are stopped and removed.
