# Testing Strategy & Test Suites

EBUninstaller Pro maintains extensive test coverage across unit, integration, and lifecycle test suites.

---

## 1. Test Architecture

The test suite is located under `source/BulkCrapUninstallerTests/` and utilizes MSTest framework.

### 1.1 Test Suites Overview

| Test Suite | Focus Area |
| :--- | :--- |
| `SecurityGuardTests.cs` | System path denylists, protected hives, argument sanitization, metacharacter guards |
| `CryptoHasherTests.cs` | SHA-256 byte and string hashing, constant-time verification |
| `ConfidenceScorerTests.cs` | Multi-factor discovery scoring, orphaned application confidence grading |
| `InstallationMonitorTests.cs` | Snapshot diffing (`Added`, `Modified`, `Removed`), trace serialization |
| `BackupManagerTests.cs` | Backup creation, manifest validation, SHA-256 verification |
| `ExclusionAndHistoryTests.cs` | Rule-based exclusions, wildcard patterns, operation history CSV/JSON export |
| `ForcedUninstallManagerTests.cs` | Forced removal planner, confidence assignment |
| `JunkAndPrivacyCleanerTests.cs` | Junk categories, temp scanning, privacy disclosures, cookie warning |
| `LifecycleIntegrationTests.cs` | End-to-end lifecycle test in disposable sandbox environment |

---

## 2. Disposable Test Application Suite (`LifecycleIntegrationTests.cs`)

The lifecycle integration test exercises the full uninstallation workflow in an isolated temporary sandbox:

```
[MOCK INSTALL] -> [MONITOR TRACE] -> [CREATE BACKUP] -> [FORCED REMOVAL] -> [VERIFY GONE] -> [RESTORE] -> [VERIFY RESTORED]
```

1. **Simulated Install**: Generates test executable, configuration files, and profile data in a sandbox root.
2. **Snapshot / Live Monitoring**: Records filesystem events and generates an `InstallationTrace`.
3. **Backup Creation**: Backs up sandbox folders into a compressed archive and records SHA-256 checksums.
4. **Forced Removal**: Executes the removal plan, safely wiping the application.
5. **Verification**: Verifies all files and folders are completely removed.
6. **Restoration**: Restores all files and registry data from the backup manifest.
7. **Post-Restore Verification**: Verifies restored files exist and match original byte content.
