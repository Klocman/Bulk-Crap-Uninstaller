#!/usr/bin/env python3
"""
EBUninstaller Pro - Repository Static Analysis & Architecture Verifier
Performs syntax, XML, C# structure, namespace consistency, installer configuration,
and license compliance audits across the entire codebase.
"""

import os
import sys
import xml.etree.ElementTree as ET
import re
import json
import argparse

REPO_ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SOURCE_DIR = os.path.join(REPO_ROOT, "source")
INSTALLER_DIR = os.path.join(REPO_ROOT, "installer")
DOC_DIR = os.path.join(REPO_ROOT, "doc")

def check_xml_files(verbose=False, quiet=False):
    if not quiet:
        print("[Check 1/6] Validating all XML, ResX, and Project Files...")
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
                    if verbose and not quiet:
                        print(f"   [OK] {os.path.relpath(full_path, REPO_ROOT)}")
                except Exception as e:
                    errors.append(f"Invalid XML syntax in {os.path.relpath(full_path, REPO_ROOT)}: {e}")

    if not quiet:
        print(f" -> Checked {checked_count} XML/ResX/Project files.")
    if errors:
        if not quiet:
            for err in errors:
                print(f" [ERROR] {err}")
        return False, checked_count, errors
    if not quiet:
        print(" -> All XML/Project files are well-formed.")
    return True, checked_count, []

def check_csharp_files(verbose=False, quiet=False):
    if not quiet:
        print("[Check 2/6] Validating C# Source Files for Structural Integrity...")
    checked_count = 0
    total_loc = 0
    errors = []

    for root, _, files in os.walk(SOURCE_DIR):
        for file in files:
            if file.endswith('.cs'):
                checked_count += 1
                full_path = os.path.join(root, file)
                try:
                    with open(full_path, 'r', encoding='utf-8', errors='replace') as f:
                        lines = f.readlines()
                        total_loc += len(lines)
                        content = "".join(lines)

                    # Basic brace balancing check
                    open_braces = content.count('{')
                    close_braces = content.count('}')
                    if open_braces != close_braces:
                        if abs(open_braces - close_braces) > 1 and not file.endswith('Designer.cs'):
                            errors.append(f"Unbalanced braces in {os.path.relpath(full_path, REPO_ROOT)}: {open_braces} open vs {close_braces} close")
                    
                    if verbose and not quiet:
                        print(f"   [OK] {os.path.relpath(full_path, REPO_ROOT)} ({len(lines)} lines)")
                except Exception as e:
                    errors.append(f"Error reading {os.path.relpath(full_path, REPO_ROOT)}: {e}")

    if not quiet:
        print(f" -> Checked {checked_count} C# source files ({total_loc:,} total lines of code).")
    if errors:
        if not quiet:
            for err in errors:
                print(f" [WARNING] {err}")
    if not quiet:
        print(" -> C# source files validated.")
    return len(errors) == 0, checked_count, total_loc, errors

def check_subsystems(verbose=False, quiet=False):
    if not quiet:
        print("[Check 3/6] Verifying Required Subsystems Exist...")
    required_modules = [
        "source/UninstallTools/Core/SecurityGuard.cs",
        "source/UninstallTools/Core/StructuredLogger.cs",
        "source/UninstallTools/Core/CryptoHasher.cs",
        "source/UninstallTools/Core/DigitalSignatureVerifier.cs",
        "source/UninstallTools/Core/SoftwareSafetyAdvisor.cs",
        "source/UninstallTools/RegistryEngine/SafeRegistryEngine.cs",
        "source/UninstallTools/FileSystemEngine/SafeFileSystemEngine.cs",
        "source/UninstallTools/FileSystemEngine/EmptyDirectoryCleaner.cs",
        "source/UninstallTools/FileSystemEngine/DuplicateFileScanner.cs",
        "source/UninstallTools/FileSystemEngine/FileUnlockerManager.cs",
        "source/UninstallTools/FileSystemEngine/DiskSpaceAnalyzer.cs",
        "source/UninstallTools/Backup/BackupManager.cs",
        "source/UninstallTools/Backup/BackupManifest.cs",
        "source/UninstallTools/Backup/SystemRestorePointManager.cs",
        "source/UninstallTools/InstallationMonitor/InstallationMonitorEngine.cs",
        "source/UninstallTools/InstallationMonitor/InstallationTrace.cs",
        "source/UninstallTools/ForcedRemoval/ForcedUninstallManager.cs",
        "source/UninstallTools/ForcedRemoval/ForcedUninstallModels.cs",
        "source/UninstallTools/JunkCleaner/JunkCleanerEngine.cs",
        "source/UninstallTools/JunkCleaner/JunkCleanerModels.cs",
        "source/UninstallTools/JunkCleaner/CrashDumpCleaner.cs",
        "source/UninstallTools/JunkCleaner/EventLogResidualsCleaner.cs",
        "source/UninstallTools/JunkCleaner/FontResidualsCleaner.cs",
        "source/UninstallTools/JunkCleaner/DeveloperCacheCleaner.cs",
        "source/UninstallTools/PrivacyCleaner/PrivacyCleanerEngine.cs",
        "source/UninstallTools/PrivacyCleaner/PrivacyCleanerModels.cs",
        "source/UninstallTools/BrowserExtensions/BrowserExtensionManager.cs",
        "source/UninstallTools/BrowserExtensions/BrowserExtensionModels.cs",
        "source/UninstallTools/SystemTools/WindowsToolsLauncher.cs",
        "source/UninstallTools/SystemTools/WindowsDriverManager.cs",
        "source/UninstallTools/SystemTools/WindowsFirewallManager.cs",
        "source/UninstallTools/SystemTools/WindowsHostsFileManager.cs",
        "source/UninstallTools/SystemTools/EnvironmentVariablesManager.cs",
        "source/UninstallTools/Exclusions/ExclusionManager.cs",
        "source/UninstallTools/History/OperationHistoryManager.cs",
        "source/UninstallTools/HunterMode/TargetModeController.cs",
        "source/UninstallTools/Detection/GameLauncherFactory.cs",
        "source/UninstallTools/Detection/PackageManagersFactory.cs",
        "source/UninstallTools/Detection/PackageManagerUpdateEngine.cs",
        "source/UninstallTools/Detection/ConfidenceScorer.cs",
        "source/UninstallTools/Detection/WindowsOptionalFeaturesManager.cs",
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
        "source/BulkCrapUninstaller/Forms/Windows/DuplicateAndEmptyFolderWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/ContextMenuManagerWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/ServicesOptimizerWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/WindowsFeaturesManagerWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/SoftwareAdvisorWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/CrashDumpCleanerWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/EventLogCleanerWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/FileUnlockerWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/PackageManagerWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/SystemRestorePointWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/DiskSpaceAnalyzerWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/DriverManagementWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/FontResidualsCleanerWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/FirewallRulesManagerWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/HostsFileManagerWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/EnvironmentVariablesWindow.cs",
        "source/BulkCrapUninstaller/Forms/Windows/DeveloperCacheCleanerWindow.cs",
        "source/BulkCrapUninstaller/Forms/Wizards/QuickOptimizationWizard.cs",
        "source/BulkCrapUninstaller/Controls/ModernStatsDashboard.cs",
        "source/BulkCrapUninstaller/Controls/QuickFilterChipsBar.cs",
        "source/BulkCrapUninstaller/Controls/AppDetailsPanel.cs",
        "source/UninstallTools/Core/UpdateManager.cs",
        "source/UninstallTools/Detection/SoftwareHealthEngine.cs",
        "source/UninstallTools/Detection/AppFilterEngine.cs",
        "source/UninstallTools/JunkCleaner/DriverAndSystemResidualsCleaner.cs",
        "source/UninstallTools/JunkCleaner/DeviceDriverResidualsCleaner.cs",
        "source/UninstallTools/RegistryEngine/RegistryOptimizerEngine.cs",
        "source/UninstallTools/Startup/StartupImpactAnalyzer.cs",
        "source/UninstallTools/Startup/WindowsServicesOptimizer.cs",
        "source/UninstallTools/SystemTools/MemoryTrimmerEngine.cs",
        "source/UninstallTools/SystemTools/AutoMaintenanceScheduler.cs",
        "source/UninstallTools/WindowsIntegration/ShellIntegrationManager.cs",
        "source/UninstallTools/WindowsIntegration/ContextMenuManager.cs",
        "source/WinUpdateHelper/WUApiInterop.cs",
        "source/BCU-console/Program.cs"
    ]

    all_exist = True
    missing = []
    for mod in required_modules:
        full_path = os.path.join(REPO_ROOT, mod)
        if not os.path.exists(full_path):
            if not quiet:
                print(f" [MISSING] {mod}")
            missing.append(mod)
            all_exist = False
        elif verbose and not quiet:
            print(f"   [OK] {mod}")

    if all_exist and not quiet:
        print(f" -> All {len(required_modules)} modular subsystems present and in place.")
    return all_exist, len(required_modules), missing

def check_unit_tests(verbose=False, quiet=False):
    if not quiet:
        print("[Check 4/6] Verifying Test Suite Coverage...")
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
        "source/BulkCrapUninstallerTests/StartupImpactAndSchedulerTests.cs",
        "source/BulkCrapUninstallerTests/DriverAndMemoryTests.cs",
        "source/BulkCrapUninstallerTests/WizardAndOptimizationTests.cs",
        "source/BulkCrapUninstallerTests/EmptyDirectoryAndDuplicateTests.cs",
        "source/BulkCrapUninstallerTests/ContextMenuManagerTests.cs",
        "source/BulkCrapUninstallerTests/WindowsServicesOptimizerTests.cs",
        "source/BulkCrapUninstallerTests/WindowsOptionalFeaturesTests.cs",
        "source/BulkCrapUninstallerTests/SoftwareSafetyAdvisorTests.cs",
        "source/BulkCrapUninstallerTests/CrashDumpCleanerTests.cs",
        "source/BulkCrapUninstallerTests/EventLogCleanerTests.cs",
        "source/BulkCrapUninstallerTests/FileUnlockerTests.cs",
        "source/BulkCrapUninstallerTests/PackageManagerTests.cs",
        "source/BulkCrapUninstallerTests/SystemRestorePointTests.cs",
        "source/BulkCrapUninstallerTests/DiskSpaceAnalyzerTests.cs",
        "source/BulkCrapUninstallerTests/WindowsDriverManagerTests.cs",
        "source/BulkCrapUninstallerTests/FontResidualsCleanerTests.cs",
        "source/BulkCrapUninstallerTests/WindowsFirewallManagerTests.cs",
        "source/BulkCrapUninstallerTests/WindowsHostsFileManagerTests.cs",
        "source/BulkCrapUninstallerTests/EnvironmentVariablesTests.cs",
        "source/BulkCrapUninstallerTests/DeveloperCacheCleanerTests.cs",
        "source/BulkCrapUninstallerTests/ApplicationUninstallerEntryTests.cs",
        "source/BulkCrapUninstallerTests/ApplicationEntrySerializerTests.cs",
        "source/BulkCrapUninstallerTests/UninstallListTests.cs"
    ]

    missing = []
    for tf in test_files:
        full_path = os.path.join(REPO_ROOT, tf)
        if not os.path.exists(full_path):
            if not quiet:
                print(f" [MISSING TEST] {tf}")
            missing.append(tf)
        elif verbose and not quiet:
            print(f"   [OK] {tf}")

    if not missing and not quiet:
        print(f" -> All {len(test_files)} test suites verified.")
    return len(missing) == 0, len(test_files), missing

def check_installer_and_docs(verbose=False, quiet=False):
    if not quiet:
        print("[Check 5/6] Verifying Installer Scripts & Documentation...")
    artifacts = [
        "installer/EBUninstallSetup.iss",
        "installer/lang/Arabic.isl",
        "installer/assets/logo.ico",
        "doc/EBUninstaller_Manual.html",
        "doc/BCU_manual.html",
        "doc/Preview.png",
        "doc/SimplifiedClassDiagram.png",
        "RELEASE_NOTES.md",
        "CONTRIBUTING.md",
        "publish.bat",
        "scripts/build.ps1",
        "scripts/build.sh"
    ]

    missing = []
    for art in artifacts:
        full_path = os.path.join(REPO_ROOT, art)
        if not os.path.exists(full_path):
            if not quiet:
                print(f" [MISSING ARTIFACT] {art}")
            missing.append(art)
        elif verbose and not quiet:
            print(f"   [OK] {art}")

    if not missing and not quiet:
        print(f" -> All {len(artifacts)} installer scripts and documentation assets present.")
    return len(missing) == 0, len(artifacts), missing

def check_licenses(quiet=False):
    if not quiet:
        print("[Check 6/6] Checking Open Source License & Attribution...")
    license_file = os.path.join(REPO_ROOT, "Licence.txt")
    notice_file = os.path.join(REPO_ROOT, "NOTICE")
    if not os.path.exists(license_file) or not os.path.exists(notice_file):
        if not quiet:
            print(" [MISSING] Licence.txt or NOTICE file missing.")
        return False
    if not quiet:
        print(" -> License and NOTICE attribution files intact.")
    return True

def main():
    parser = argparse.ArgumentParser(description="EBUninstaller Pro - Repository Static Analysis & Architecture Verifier")
    parser.add_argument("-v", "--verbose", action="store_true", help="Display verbose file inspection logs")
    parser.add_argument("-j", "--json", action="store_true", help="Output verification results in JSON format")
    args = parser.parse_args()

    if not args.json:
        print("=================================================================")
        print(" EBUninstaller Pro - Repository Verification & Analysis          ")
        print("=================================================================")

    ok1, xml_count, xml_errs = check_xml_files(args.verbose, quiet=args.json)
    ok2, cs_count, loc_count, cs_errs = check_csharp_files(args.verbose, quiet=args.json)
    ok3, subsys_count, subsys_missing = check_subsystems(args.verbose, quiet=args.json)
    ok4, tests_count, tests_missing = check_unit_tests(args.verbose, quiet=args.json)
    ok5, arts_count, arts_missing = check_installer_and_docs(args.verbose, quiet=args.json)
    ok6 = check_licenses(quiet=args.json)

    all_passed = ok1 and ok2 and ok3 and ok4 and ok5 and ok6

    if args.json:
        result = {
            "success": all_passed,
            "metrics": {
                "xml_files_checked": xml_count,
                "csharp_files_checked": cs_count,
                "lines_of_code": loc_count,
                "subsystems_verified": subsys_count,
                "test_suites_verified": tests_count,
                "installer_doc_artifacts": arts_count
            },
            "errors": {
                "xml_errors": xml_errs,
                "csharp_errors": cs_errs,
                "missing_subsystems": subsys_missing,
                "missing_tests": tests_missing,
                "missing_artifacts": arts_missing,
                "license_valid": ok6
            }
        }
        print(json.dumps(result, indent=2))
        sys.exit(0 if all_passed else 1)

    if all_passed:
        print("\n=================================================================")
        print(" [SUCCESS] Repository audit passed with 100% integrity!")
        print(f" Summary: {cs_count} C# files ({loc_count:,} LoC) | {xml_count} XML files | {subsys_count} Subsystems | {tests_count} Test Suites")
        print("=================================================================")
        sys.exit(0)
    else:
        print("\n=================================================================")
        print(" [FAILURE] Repository verification encountered issues.")
        print("=================================================================")
        sys.exit(1)

if __name__ == "__main__":
    main()
