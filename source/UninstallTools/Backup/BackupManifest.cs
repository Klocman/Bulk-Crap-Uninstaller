/*
    EBUninstaller Pro - Backup Manifest Model
*/

using System;
using System.Collections.Generic;

namespace UninstallTools.Backup
{
    public sealed class BackupManifest
    {
        public string BackupId { get; set; } = Guid.NewGuid().ToString("N");
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
        public string Publisher { get; set; }
        public DateTime CreatedTimestampUtc { get; set; } = DateTime.UtcNow;
        public string MachineName { get; set; } = Environment.MachineName;
        public string OperatingSystem { get; set; } = Environment.OSVersion.ToString();
        public List<string> BackedUpRegistryKeys { get; set; } = new();
        public List<string> BackedUpDirectories { get; set; } = new();
        public Dictionary<string, string> FileSha256Checksums { get; set; } = new();
        public bool SystemRestorePointCreated { get; set; }
        public string RegistryExportRelativePath { get; set; } = "registry.reg";
        public string FileArchiveRelativePath { get; set; } = "files.zip";
        public string ManifestSha256Digest { get; set; }
    }
}
