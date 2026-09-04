# Security Model & Guidelines

OpenUninstall Pro is designed with a defense-in-depth security model to ensure safe, reliable system cleanup and prevent accidental data loss or privilege escalation vulnerabilities.

---

## 1. System Protection & Denylists

The `SecurityGuard` subsystem enforces protected path and registry hive restrictions before any file or registry modification:

### 1.1 Protected Directories
* Windows Root (`%WINDIR%`, `C:\Windows`)
* `System32`, `SysWOW64`, `WinSxS`, `SystemApps`, `Boot`, `Fonts`, `assembly`, `Microsoft.NET`, `servicing`, `security`, `schemas`, `diagnostics`, `Drivers`
* Root drive directories (e.g., `C:\`, `D:\`)
* Core system profile paths

### 1.2 Protected Registry Keys
* `HKLM\SAM`
* `HKLM\SECURITY`
* `HKLM\SYSTEM\CurrentControlSet\Control`
* `HKLM\SYSTEM\CurrentControlSet\Services` (system core services)
* `HKLM\BCD00000000`
* `HKLM\HARDWARE`
* Top-level and 2nd-level root hives (`HKLM\SOFTWARE`, `HKLM\SYSTEM`, `HKCU\Software`, `HKCR`)

---

## 2. Command Injection Prevention

* All uninstaller and tool invocations sanitize command-line arguments.
* Dangerous shell metacharacters (`&`, `|`, `;`, `>`, `<`, `` ` ``, `$`) are rejected or escaped safely.
* Process paths are strictly validated to ensure they exist and point to legitimate binaries.

---

## 3. Reparse Points & Symlink Protections

* Recursive directory traversals explicitly check for `FileAttributes.ReparsePoint`.
* Deletion operations never traverse into NTFS junctions or symlink targets. Reparse points themselves are unlinked without affecting the target directory.

---

## 4. Time-of-Check to Time-of-Use (TOCTOU) Mitigation

* File and directory handles are checked immediately prior to operations.
* File attributes (ReadOnly, Hidden) are normalized only immediately before unlinking.

---

## 5. Digital Signature & Authenticode Verification

* PE binaries are verified using the native Windows `WinVerifyTrust` API (`WINTRUST_ACTION_GENERIC_VERIFY_V2`).
* Embedded X509 certificates are inspected for signer name, issuer name, serial number, thumbprint, and validity periods.

---

## 6. Cryptographic Integrity

* All backups compute and verify SHA-256 hashes for every backed-up file and exported registry file.
* Constant-time comparison (`FixedTimeEquals`) is used during hash verification to prevent side-channel timing attacks.

---

## 7. Privacy & Data Handling

* **Zero Telemetry**: No telemetry, analytics, user profiling, or background tracking requests exist by default.
* **Offline-First**: All core discovery, uninstallation, scanning, backup, monitoring, and cleaning engines operate 100% offline.
* **Credential Redaction**: The structured logging system automatically redacts tokens, passwords, and API keys.
