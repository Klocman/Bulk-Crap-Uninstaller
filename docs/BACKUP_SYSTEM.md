# Backup and Recovery Subsystem

The Backup and Recovery subsystem in OpenUninstall Pro provides automatic pre-operation protection, cryptographic verification, and one-click restoration.

---

## 1. Backup Architecture

Each backup package is stored under `%LOCALAPPDATA%\OpenUninstallPro\Backups\<BackupId>\`:

```
Backups/
  └── <BackupId>/
        ├── manifest.json       # JSON metadata and SHA-256 hashes
        ├── manifest.sha256     # Cryptographic checksum of manifest
        ├── registry.reg        # Standard Windows Registry Editor v5.00 export
        └── files.zip           # Compressed archive of application files
```

---

## 2. Manifest Specifications

The `manifest.json` file contains:
* `BackupId`: Unique GUID identifier.
* `CreatedAt`: UTC timestamp.
* `ApplicationName`, `ApplicationVersion`, `ApplicationPublisher`.
* `OperationType`: `PipelineUninstall`, `ForcedRemoval`, `ManualBackup`.
* `SystemRestorePointName`, `SystemRestorePointSequenceNumber`.
* `RegistryEntries`: List of registry keys with their individual SHA-256 hashes.
* `FileEntries`: List of original file paths, relative archive paths, file sizes, timestamps, and SHA-256 checksums.
* `DeletionManifest`: Complete list of paths unlinked during the operation.

---

## 3. Cryptographic Verification

* The `VerifyBackup(backupId)` method recalculates the SHA-256 checksum of every file in `files.zip` and `registry.reg`.
* Verification employs constant-time comparison to ensure 100% data integrity before restore.

---

## 4. One-Click Restoration

Restoration can be performed via the GUI Backup Center or CLI:
* **Registry Restoration**: Imports `registry.reg` via silent `regedit.exe /s`.
* **File Restoration**: Extracts archived files from `files.zip` back to their exact original absolute paths on disk, creating missing parent directories as needed.
