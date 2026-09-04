# Command-Line Interface (CLI) Reference

`BCU-console.exe` provides a comprehensive command-line interface for automation, enterprise scripting, and headless maintenance tasks.

---

## 1. Syntax

```
BCU-console.exe <command> [arguments] [switches]
```

---

## 2. Command Reference

### `list`
List all installed applications.
```bash
# Standard console table output
BCU-console.exe list

# Filter by application name
BCU-console.exe list --filter "Python"

# Output as JSON
BCU-console.exe list --json
```

---

### `uninstall`
Execute the uninstallation pipeline for matched applications or a `.bcul` list file.
```bash
# Quiet uninstallation
BCU-console.exe uninstall "VLC media player" /Q

# Unattended uninstallation with automatic leftover cleanup
BCU-console.exe uninstall "AppName" /Q /U /J=VeryGood

# Uninstall from .bcul list file
BCU-console.exe uninstall "C:\Lists\cleanup.bcul" /Q /U
```

---

### `forced-uninstall`
Deep forced removal of corrupted or broken applications.
```bash
# Forced removal by application name
BCU-console.exe forced-uninstall "CorruptedApp" /U

# Forced removal by installation directory
BCU-console.exe forced-uninstall "C:\Program Files\CorruptedApp" /U
```

---

### `scan`
Scan and list leftovers for an application without deleting.
```bash
BCU-console.exe scan "AppName"
BCU-console.exe scan "AppName" --json
```

---

### `leftovers`
Scan and automatically remove residual leftovers.
```bash
BCU-console.exe leftovers "AppName" --junk=Good /U
```

---

### `backup`
Create a complete pre-removal backup of an application.
```bash
BCU-console.exe backup "AppName"
BCU-console.exe backup "AppName" --output "D:\Backups"
```

---

### `restore`
Restore an application from a backup package.
```bash
BCU-console.exe restore "<BackupId>"
```

---

### `monitor`
Live monitor an installer execution and record a trace log.
```bash
BCU-console.exe monitor "C:\Downloads\setup.exe" --name "MySoftware"
```

---

### `rollback-trace`
Rollback all system changes recorded in an installation trace.
```bash
BCU-console.exe rollback-trace "<TraceId>"
```

---

### `clean-junk`
Scan and clean temporary and junk files.
```bash
# Scan only
BCU-console.exe clean-junk

# Clean all junk files
BCU-console.exe clean-junk --clean
```

---

### `clean-privacy`
Scan and clean browser and Windows privacy tracks.
```bash
BCU-console.exe clean-privacy --clean
```

---

### `startup`
List and inspect Windows startup entries.
```bash
BCU-console.exe startup
BCU-console.exe startup --json
```

---

### `extensions`
List and inspect browser extensions.
```bash
BCU-console.exe extensions
BCU-console.exe extensions --json
```

---

### `tools`
List and launch Windows administrative tools.
```bash
# List tools
BCU-console.exe tools

# Launch Task Manager
BCU-console.exe tools --launch "Task Manager"
```

---

### `export`
Export the installed software catalog to XML or JSON.
```bash
BCU-console.exe export "C:\Reports\installed_apps.json"
BCU-console.exe export "C:\Reports\installed_apps.xml"
```

---

### `history`
View operation history and audit log.
```bash
BCU-console.exe history
BCU-console.exe history --json
```

---

## 3. Exit Codes

| Exit Code | Meaning |
| :--- | :--- |
| `0` | Operation completed successfully |
| `1` | General error or execution failure |
| `87` | Invalid command line syntax or arguments |
| `1223` | Operation canceled by user |
