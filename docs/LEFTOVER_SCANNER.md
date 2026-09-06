# Advanced Leftover Detection Engine

The Leftover Detection Engine in EBUninstaller Pro identifies remnants, residual configuration keys, and orphaned data left behind by official software uninstallers.

---

## 1. Scanned Locations

### 1.1 Registry Subsystem
* `HKLM\SOFTWARE` and `HKCU\SOFTWARE` (including 32-bit `WOW6432Node` views)
* `App Paths` (`HKLM\Software\Microsoft\Windows\CurrentVersion\App Paths`)
* `Uninstall` keys (`HKLM` and `HKCU` uninstall keys)
* `AppCompatFlags` (Compatibility layers and application flags)
* `AudioPolicyConfig` (Windows per-app audio mixer entries)
* `COM / CLSID` registrations
* `Tracing` and `Debug` keys
* `Firewall Rules` (`System\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\FirewallRules`)
* `EventLog` source registrations
* `Installer Folders` registrations
* `UserAssist` execution history keys

### 1.2 File System Subsystem
* `Program Files` and `Program Files (x86)`
* `ProgramData` (`%ALLUSERSPROFILE%`)
* `AppData\Local`
* `AppData\Roaming`
* `AppData\LocalLow`
* `VirtualStore` (`%LOCALAPPDATA%\VirtualStore`)
* Windows Prefetch (`%WINDIR%\Prefetch`)
* Windows Error Reporting (`WER`) directories

### 1.3 Miscellaneous Registrations
* Desktop, Start Menu, and Quick Launch shortcuts (`.lnk`)
* Windows Services (`ServiceController`)
* Scheduled Tasks (`TaskScheduler` API)
* Startup run keys (`Run`, `RunOnce`)
* Environment variables

---

## 2. Confidence Scoring & Multi-Signal Rules

Leftover candidates are evaluated using multi-signal scoring:

| Confidence Level | Default Action | Criteria |
| :--- | :--- | :--- |
| **High** (VeryGood) | Auto-selected for cleanup | Exact folder/key match with verified application metadata, subfolders under publisher root, or explicit registry references. |
| **Medium** (Good) | User confirmation recommended | Name resemblance verified with parent folder structure or valid product identifiers. |
| **Low** (Questionable) | Deselected by default | Generic name match or shared directory structure; requires explicit manual review. |
