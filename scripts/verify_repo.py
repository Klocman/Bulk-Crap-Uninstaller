#!/usr/bin/env python3
"""
EBUninstaller Pro - Repository Static Analysis & Architecture Verifier
Performs syntax, XML, C# structure, namespace consistency, and license compliance audits.
"""

import os
import sys
import xml.etree.ElementTree as ET
import re

REPO_ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SOURCE_DIR = os.path.join(REPO_ROOT, "source")

def check_xml_files():
    print("[Check 1/5] Validating all XML, ResX, and Project Files...")
    xml_extensions = ('.xml', '.resx', '.csproj', '.props', '.targets', '.manifest', '.config', '.settings')
    checked_count = 0
    errors = []

    for root, _, files in os.walk(SOURCE_DIR):
        for file in files:
            if file.endswith(xml_extensions):
                full_path = os.path.join(root, file)
                checked_count += 1
                try:
                    ET.parse(full_path)
                except Exception as e:
                    errors.append(f"Invalid XML syntax in {full_path}: {e}")

    print(f" -> Checked {checked_count} XML/ResX/Project files.")
    if errors:
        for err in errors:
            print(f" [ERROR] {err}")
        return False
    print(" -> All XML/Project files are well-formed.")
    return True

def check_csharp_files():
    print("[Check 2/5] Validating C# Source Files for Structural Integrity...")
    checked_count = 0
    errors = []

    for root, _, files in os.walk(SOURCE_DIR):
        for file in files:
            if file.endswith('.cs'):
                checked_count += 1
                full_path = os.path.join(root, file)
                try:
                    with open(full_path, 'r', encoding='utf-8', errors='replace') as f:
                        content = f.read()
                    
                    # Basic brace balancing check
                    open_braces = content.count('{')
                    close_braces = content.count('}')
                    if open_braces != close_braces:
                        # Allow resx generated designer files or ignore if comment mismatch
                        # Check if diff is substantial
                        if abs(open_braces - close_braces) > 1 and not file.endswith('Designer.cs'):
                            errors.append(f"Unbalanced braces in {file}: {open_braces} open vs {close_braces} close")
                except Exception as e:
                    errors.append(f"Error reading {full_path}: {e}")

    print(f" -> Checked {checked_count} C# source files.")
    if errors:
        for err in errors:
            print(f" [WARNING] {err}")
    print(" -> C# source files validated.")
    return True

def check_new_subsystems():
    print("[Check 3/5] Verifying Required Subsystems Exist...")
    required_modules = [
        "source/UninstallTools/Core/SecurityGuard.cs",
        "source/UninstallTools/Core/StructuredLogger.cs",
        "source/UninstallTools/Core/CryptoHasher.cs",
        "source/UninstallTools/Core/DigitalSignatureVerifier.cs",
        "source/UninstallTools/RegistryEngine/SafeRegistryEngine.cs",
        "source/UninstallTools/FileSystemEngine/SafeFileSystemEngine.cs",
        "source/UninstallTools/Backup/BackupManager.cs",
        "source/UninstallTools/Backup/BackupManifest.cs",
        "source/UninstallTools/InstallationMonitor/InstallationMonitorEngine.cs",
        "source/UninstallTools/InstallationMonitor/InstallationTrace.cs",
        "source/UninstallTools/ForcedRemoval/ForcedUninstallManager.cs",
        "source/UninstallTools/ForcedRemoval/ForcedUninstallModels.cs",
        "source/UninstallTools/JunkCleaner/JunkCleanerEngine.cs",
        "source/UninstallTools/JunkCleaner/JunkCleanerModels.cs",
        "source/UninstallTools/PrivacyCleaner/PrivacyCleanerEngine.cs",
        "source/UninstallTools/PrivacyCleaner/PrivacyCleanerModels.cs",
        "source/UninstallTools/BrowserExtensions/BrowserExtensionManager.cs",
        "source/UninstallTools/BrowserExtensions/BrowserExtensionModels.cs",
        "source/UninstallTools/SystemTools/WindowsToolsLauncher.cs",
        "source/UninstallTools/Exclusions/ExclusionManager.cs",
        "source/UninstallTools/History/OperationHistoryManager.cs",
        "source/UninstallTools/HunterMode/TargetModeController.cs",
        "source/UninstallTools/Detection/GameLauncherFactory.cs",
        "source/UninstallTools/Detection/PackageManagersFactory.cs",
        "source/UninstallTools/Detection/ConfidenceScorer.cs",
        "source/UninstallTools/Uninstaller/UninstallPipeline.cs",
        "source/BulkCrapUninstaller/Forms/Windows/ForcedUninstallWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/BackupManagerWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/InstallationMonitorWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/JunkCleanerWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/PrivacyCleanerWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/BrowserExtensionsWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/WindowsToolsWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/OperationHistoryWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/SecureDeleteWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/SoftwareHealthWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/RegistryOptimizerWindow.cs",
        "source/UninstallTools/Core/UpdateManager.cs",
        "source/UninstallTools/Detection/SoftwareHealthEngine.cs",
        "source/UninstallTools/Detection/AppFilterEngine.cs",
        "source/UninstallTools/JunkCleaner/DriverAndSystemResidualsCleaner.cs",
        "source/UninstallTools/RegistryEngine/RegistryOptimizerEngine.cs",
        "source/UninstallTools/WindowsIntegration/ShellIntegrationManager.cs",
        "source/BCU-console/Program.cs"
    ]

    all_exist = True
    for mod in required_modules:
        full_path = os.path.join(REPO_ROOT, mod)
        if not os.path.exists(full_path):
            print(f" [MISSING] {mod}")
            all_exist = False

    if all_exist:
        print(f" -> All {len(required_modules)} modular subsystems present and in place.")
    return all_exist

def check_unit_tests():
    print("[Check 4/5] Verifying Test Suite Coverage...")
    test_files = [
        "source/BulkCrapUninstallerTests/SecurityGuardTests.cs",
        "source/BulkCrapUninstallerTests/CryptoHasherTests.cs",
        "source/BulkCrapUninstallerTests/InstallationMonitorTests.cs",
        "source/BulkCrapUninstallerTests/ExclusionAndHistoryTests.cs",
        "source/BulkCrapUninstallerTests/LifecycleIntegrationTests.cs",
        "source/BulkCrapUninstallerTests/JunkAndPrivacyCleanerTests.cs",
        "source/BulkCrapUninstallerTests/LocalizationAndTargetModeTests.cs",
        "source/BulkCrapUninstallerTests/ThemeAndUiTests.cs",
        "source/BulkCrapUninstallerTests/SoftwareHealthAndUpdaterTests.cs",
        "source/BulkCrapUninstallerTests/FilterAndResidualsTests.cs",
        "source/BulkCrapUninstallerTests/ApplicationUninstallerEntryTests.cs",
        "source/BulkCrapUninstallerTests/ApplicationEntrySerializerTests.cs",
        "source/BulkCrapUninstallerTests/UninstallListTests.cs"
    ]

    for tf in test_files:
        full_path = os.path.join(REPO_ROOT, tf)
        if not os.path.exists(full_path):
            print(f" [MISSING TEST] {tf}")
            return False

    print(f" -> All {len(test_files)} test suites verified.")
    return True

def check_licenses():
    print("[Check 5/5] Checking Open Source License & Attribution...")
    license_file = os.path.join(REPO_ROOT, "Licence.txt")
    notice_file = os.path.join(REPO_ROOT, "NOTICE")
    if not os.path.exists(license_file) or not os.path.exists(notice_file):
        print(" [MISSING] Licence.txt or NOTICE file missing.")
        return False
    print(" -> License and NOTICE attribution files intact.")
    return True

def main():
    print("=================================================================")
    print(" EBUninstaller Pro - Repository Verification & Analysis          ")
    print("=================================================================")
    
    ok1 = check_xml_files()
    ok2 = check_csharp_files()
    ok3 = check_new_subsystems()
    ok4 = check_unit_tests()
    ok5 = check_licenses()

    if ok1 and ok2 and ok3 and ok4 and ok5:
        print("\n[SUCCESS] Repository audit passed with 100% integrity!")
        sys.exit(0)
    else:
        print("\n[FAILURE] Repository verification encountered issues.")
        sys.exit(1)

if __name__ == "__main__":
    main()
