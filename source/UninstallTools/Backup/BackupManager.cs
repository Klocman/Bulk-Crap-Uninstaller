/*
    EBUninstaller Pro - Cryptographic Backup and Restoration Engine
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using UninstallTools.Core;
using UninstallTools.RegistryEngine;

namespace UninstallTools.Backup
{
    public static class BackupManager
    {
        private static string _defaultBackupDirectory;

        public static string BackupDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_defaultBackupDirectory))
                {
                    try
                    {
                        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        _defaultBackupDirectory = Path.Combine(localAppData, "EBUninstallerPro", "Backups");
                    }
                    catch
                    {
                        _defaultBackupDirectory = Path.Combine(Path.GetTempPath(), "EBUninstallerPro", "Backups");
                    }
                }

                if (!Directory.Exists(_defaultBackupDirectory))
                {
                    Directory.CreateDirectory(_defaultBackupDirectory);
                }

                return _defaultBackupDirectory;
            }
            set => _defaultBackupDirectory = value;
        }

        public static BackupManifest CreateBackup(string applicationName, string version, string publisher, IEnumerable<string> registryKeys, IEnumerable<string> filePaths, bool createRestorePoint = true)
        {
            var manifest = new BackupManifest
            {
                ApplicationName = applicationName ?? "Unknown Application",
                ApplicationVersion = version ?? "Unknown",
                Publisher = publisher ?? "Unknown",
                SystemRestorePointCreated = false
            };

            var backupDir = Path.Combine(BackupDirectory, manifest.BackupId);
            Directory.CreateDirectory(backupDir);

            // 1. System Restore Point
            if (createRestorePoint)
            {
                manifest.SystemRestorePointCreated = WindowsIntegration.WindowsRestorePointManager.CreateRestorePoint($"EBUninstaller Pro Pre-Removal: {manifest.ApplicationName}");
            }

            // 2. Export Registry Keys
            if (registryKeys != null)
            {
                var regList = new List<string>(registryKeys);
                manifest.BackedUpRegistryKeys = regList;
                var regExportPath = Path.Combine(backupDir, manifest.RegistryExportRelativePath);
                SafeRegistryEngine.ExportKeysToRegFile(regList, regExportPath);
            }

            // 3. Zip Backup Directories and Compute SHA-256 Hashes
            if (filePaths != null)
            {
                var zipPath = Path.Combine(backupDir, manifest.FileArchiveRelativePath);
                using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    foreach (var path in filePaths)
                    {
                        if (Directory.Exists(path))
                        {
                            manifest.BackedUpDirectories.Add(path);
                            foreach (var file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
                            {
                                try
                                {
                                    var hash = CryptoHasher.ComputeFileSha256(file);
                                    manifest.FileSha256Checksums[file] = hash;
                                    var entryName = Path.GetRelativePath(path, file);
                                    archive.CreateEntryFromFile(file, Path.Combine(Path.GetFileName(path), entryName));
                                }
                                catch (Exception ex)
                                {
                                    StructuredLogger.Warning(LogCategory.Backup, $"Skipping file in backup: {file}", ex.Message);
                                }
                            }
                        }
                        else if (File.Exists(path))
                        {
                            try
                            {
                                var hash = CryptoHasher.ComputeFileSha256(path);
                                manifest.FileSha256Checksums[path] = hash;
                                archive.CreateEntryFromFile(path, Path.GetFileName(path));
                            }
                            catch { }
                        }
                    }
                }
            }

            // 4. Save and Hash Manifest
            var manifestJson = JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true });
            var manifestPath = Path.Combine(backupDir, "manifest.json");
            File.WriteAllText(manifestPath, manifestJson, Encoding.UTF8);

            manifest.ManifestSha256Digest = CryptoHasher.ComputeStringSha256(manifestJson);
            File.WriteAllText(Path.Combine(backupDir, "manifest.sha256"), manifest.ManifestSha256Digest);

            StructuredLogger.Info(LogCategory.Backup, $"Backup package created successfully: {manifest.BackupId} for {manifest.ApplicationName}");
            return manifest;
        }

        public static bool VerifyBackup(string backupId)
        {
            var backupDir = Path.Combine(BackupDirectory, backupId);
            if (!Directory.Exists(backupDir)) return false;

            var manifestPath = Path.Combine(backupDir, "manifest.json");
            var shaPath = Path.Combine(backupDir, "manifest.sha256");

            if (!File.Exists(manifestPath) || !File.Exists(shaPath)) return false;

            var manifestJson = File.ReadAllText(manifestPath, Encoding.UTF8);
            var expectedSha = File.ReadAllText(shaPath).Trim();
            var actualSha = CryptoHasher.ComputeStringSha256(manifestJson);

            return string.Equals(expectedSha, actualSha, StringComparison.OrdinalIgnoreCase);
        }

        public static bool RestoreBackup(string backupId)
        {
            if (!VerifyBackup(backupId))
            {
                StructuredLogger.Error(LogCategory.Backup, $"Cannot restore corrupted backup package: {backupId}");
                return false;
            }

            var backupDir = Path.Combine(BackupDirectory, backupId);
            var manifestJson = File.ReadAllText(Path.Combine(backupDir, "manifest.json"), Encoding.UTF8);
            var manifest = JsonSerializer.Deserialize<BackupManifest>(manifestJson);

            // 1. Restore Registry
            var regFile = Path.Combine(backupDir, manifest.RegistryExportRelativePath);
            if (File.Exists(regFile))
            {
                SafeRegistryEngine.ImportRegFile(regFile);
            }

            // 2. Restore Files
            var zipPath = Path.Combine(backupDir, manifest.FileArchiveRelativePath);
            if (File.Exists(zipPath) && manifest.BackedUpDirectories.Count > 0)
            {
                var targetRoot = Path.GetDirectoryName(manifest.BackedUpDirectories[0]);
                if (!string.IsNullOrEmpty(targetRoot))
                {
                    ZipFile.ExtractToDirectory(zipPath, targetRoot, true);
                }
            }

            StructuredLogger.Info(LogCategory.Backup, $"Backup {backupId} restored successfully.");
            return true;
        }

        public static IReadOnlyList<BackupManifest> GetAvailableBackups()
        {
            var list = new List<BackupManifest>();
            if (!Directory.Exists(BackupDirectory)) return list;

            foreach (var dir in Directory.GetDirectories(BackupDirectory))
            {
                var manifestFile = Path.Combine(dir, "manifest.json");
                if (File.Exists(manifestFile))
                {
                    try
                    {
                        var json = File.ReadAllText(manifestFile, Encoding.UTF8);
                        var manifest = JsonSerializer.Deserialize<BackupManifest>(json);
                        if (manifest != null) list.Add(manifest);
                    }
                    catch { }
                }
            }

            return list;
        }

        public static bool DeleteBackup(string backupId)
        {
            try
            {
                var backupDir = Path.Combine(BackupDirectory, backupId);
                if (Directory.Exists(backupDir))
                {
                    Directory.Delete(backupDir, true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Backup, $"Failed to delete backup: {backupId}", ex.Message);
            }
            return false;
        }
    }
}
