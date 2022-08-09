/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using Klocman.Native;
using Klocman.Tools;
using Microsoft.Win32;
using UninstallTools.Properties;

namespace UninstallTools.Startup.Normal
{
    internal sealed class OldStartupDisable : IStartupDisable
    {
        /// <summary>
        ///     Extension used for link backup of the disabled entry
        ///     (it is appended to the end of the filename, after the original extension).
        /// </summary>
        private static readonly string BackupExtension = ".Startup";

        /// <summary>
        ///     Path used to store link backups of disabled entries
        /// </summary>
        private static readonly string DriveDisableBackupPath =
            Path.Combine(WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_WINDOWS), "pss");

        /// <summary>
        ///     Registry key used to store information about disabled links
        /// </summary>
        private static readonly StartupPointData DriveDisabledKey = new(
            true, false, false, false, string.Empty,
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Shared Tools\MSConfig\startupfolder");

        /// <summary>
        ///     Registry key used to store information about disabled registry values
        /// </summary>
        private static readonly StartupPointData RegistryDisabledKey = new(
            true, true, false, false, string.Empty,
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Shared Tools\MSConfig\startupreg");

        /// <summary>
        ///     Look for entries in the disabled startup backup store.
        /// </summary>
        public IEnumerable<StartupEntry> AddDisableInfo(IList<StartupEntry> existingEntries)
        {
            foreach (var entry in existingEntries)
                yield return entry;

            using (var regDisKey = RegistryTools.CreateSubKeyRecursively(RegistryDisabledKey.Path))
            {
                var badLocations = new List<string>();
                foreach (var subKeyName in regDisKey.GetSubKeyNames())
                {
                    using (var subKey = regDisKey.OpenSubKey(subKeyName))
                    {
                        if (subKey == null) continue;

                        var key = subKey.GetStringSafe("key");
                        var hkey = subKey.GetStringSafe("hkey");
                        var item = subKey.GetStringSafe("item");
                        var command = subKey.GetStringSafe("command");
                        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(hkey)
                            || string.IsNullOrEmpty(item) || string.IsNullOrEmpty(command))
                            continue;

                        string location;
                        try
                        {
                            location = Path.Combine(RegistryTools.GetKeyRoot(hkey, false), key);
                        }
                        catch (ArgumentException)
                        {
                            continue;
                        }

                        if (string.IsNullOrEmpty(location.Trim()))
                            continue;

                        var runLocation = StartupEntryFactory.RunLocations
                            .FirstOrDefault(x => PathTools.PathsEqual(location, x.Path));

                        if (runLocation != null)
                        {
                            StartupEntry startupEntry;
                            try
                            {
                                startupEntry = new StartupEntry(runLocation, item, command) { DisabledStore = true };
                            }
                            catch
                            {
                                badLocations.Add(location);
                                continue;
                            }
                            yield return startupEntry;
                        }
                        else
                        {
                            badLocations.Add(location);
                        }
                    }
                }

                if (badLocations.Any())
                {
                    var errorString = Localisation.Error_InvalidRegKeys + "\n" +
                        string.Join("\n", badLocations.Distinct().OrderBy(x => x).ToArray());
#if DEBUG
                    Debug.Fail(errorString);
#else
                    Trace.WriteLine(errorString);
#endif
                }
            }

            using (var hddDisKey = RegistryTools.CreateSubKeyRecursively(DriveDisabledKey.Path))
            {
                foreach (var subKeyName in hddDisKey.GetSubKeyNames())
                {
                    using (var subKey = hddDisKey.OpenSubKey(subKeyName))
                    {
                        if (subKey == null) continue;

                        var path = subKey.GetStringSafe("path");
                        var location = subKey.GetStringSafe("location");
                        var backup = subKey.GetStringSafe("backup");
                        var command = subKey.GetStringSafe("command");
                        if (backup == null || location == null || path == null || command == null)
                            continue;

                        var runLocation = StartupEntryFactory.RunLocations
                            .FirstOrDefault(x => PathTools.PathsEqual(x.Path, location));
                        if (runLocation == null) continue;

                        yield return new StartupEntry(runLocation, Path.GetFileName(path), command)
                        {
                            BackupPath = backup,
                            DisabledStore = true
                        };
                    }
                }
            }
        }

        public void Disable(StartupEntry startupEntry)
        {
            if (startupEntry.DisabledStore)
                return;

            var newPath = CreateDisabledEntryPath(startupEntry);

            // Remove the startup entry / move the file to the backup folder
            if (startupEntry.IsRegKey)
            {
                try
                {
                    startupEntry.Delete();
                }
                catch
                {
                    // Key doesn't exist
                }
            }
            else
            {
                if (File.Exists(startupEntry.FullLongName))
                {
                    File.Delete(newPath);
                    Directory.CreateDirectory(DriveDisableBackupPath);
                    File.Move(startupEntry.FullLongName, newPath);
                    startupEntry.BackupPath = newPath;
                }
                else
                    throw new InvalidOperationException(Localisation.StartupManager_FailedEnable_FileNotFound
                                                        + "\n\n" + startupEntry.FullLongName);
            }

            CreateDisabledEntry(startupEntry, newPath);

            startupEntry.DisabledStore = true;
        }

        public void Enable(StartupEntry startupEntry)
        {
            if (!startupEntry.DisabledStore)
                return;

            // Reconstruct the startup entry
            if (!startupEntry.IsRegKey)
            {
                // Move the backup file back to its original location
                var oldPath = GetDisabledEntryPath(startupEntry);

                if (File.Exists(oldPath))
                {
                    File.Delete(startupEntry.FullLongName);
                    File.Move(oldPath, startupEntry.FullLongName);
                }
                else
                    throw new InvalidOperationException(Localisation.StartupManager_FailedEnable_FileNotFound
                                                        + "\n\n" + oldPath);
            }
            else
            {
                // Recreate the registry key
                StartupEntryManager.CreateRegValue(startupEntry);
            }

            Delete(startupEntry);

            startupEntry.BackupPath = string.Empty;
            // Entry is no longer disabled
            startupEntry.DisabledStore = false;
        }

        /// <summary>
        ///     Create backup store path for the link. The backup extension is appended as well.
        ///     Works only for links, returns garbage for registry values.
        /// </summary>
        public string GetDisabledEntryPath(StartupEntry startupEntry)
        {
            return string.IsNullOrEmpty(startupEntry.BackupPath)
                ? CreateDisabledEntryPath(startupEntry)
                : startupEntry.BackupPath;
        }

        public bool StillExists(StartupEntry startupEntry)
        {
            try
            {
                using (var key = RegistryTools.OpenRegistryKey(
                    startupEntry.IsRegKey ? RegistryDisabledKey.Path : DriveDisabledKey.Path))
                {
                    var disabledSubKeyName = startupEntry.IsRegKey
                        ? startupEntry.EntryLongName
                        : startupEntry.FullLongName.Replace('\\', '^');
                    return key.GetSubKeyNames()
                        .Any(x => disabledSubKeyName.Equals(x, StringComparison.InvariantCultureIgnoreCase));
                }
            }
            catch
            {
                return false;
            }
        }

        public void Delete(StartupEntry startupEntry)
        {
            RemoveDisabledRegEntry(startupEntry);

            // Remove the backup file
            if (!startupEntry.IsRegKey)
                File.Delete(GetDisabledEntryPath(startupEntry));
        }

        /// <summary>
        ///     Create a new record in the appropriate disabled entry store. If the entry already exists it is overwritten.
        /// </summary>
        /// <param name="startupEntry">Startup entry to create the record for</param>
        /// <param name="newEntryPath">Full path to the new backup file</param>
        private static void CreateDisabledEntry(StartupEntry startupEntry, string newEntryPath)
        {
            using (var disabledStartupEntryStore = RegistryTools.CreateSubKeyRecursively(
                startupEntry.IsRegKey ? RegistryDisabledKey.Path : DriveDisabledKey.Path))
            {
                var disabledSubKeyName = startupEntry.IsRegKey
                    ? startupEntry.EntryLongName
                    : startupEntry.FullLongName.Replace('\\', '^');
                var disabledSubkeyKey =
                    disabledStartupEntryStore.GetSubKeyNames()
                        .FirstOrDefault(x => disabledSubKeyName.Equals(x, StringComparison.InvariantCultureIgnoreCase));

                // Clean up old disabled entry if any
                if (!string.IsNullOrEmpty(disabledSubkeyKey))
                {
                    disabledStartupEntryStore.DeleteSubKey(disabledSubkeyKey);
                }

                using (
                    var storeSubkey = disabledStartupEntryStore.CreateSubKey(disabledSubKeyName,
                        RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    if (storeSubkey == null)
                        return;

                    if (startupEntry.IsRegKey)
                    {
                        storeSubkey.SetValue("key", RegistryTools.StripKeyRoot(startupEntry.ParentLongName),
                            RegistryValueKind.String);
                        storeSubkey.SetValue("item", startupEntry.EntryLongName, RegistryValueKind.String);
                        storeSubkey.SetValue("hkey", RegistryTools.GetKeyRoot(startupEntry.ParentLongName, true),
                            RegistryValueKind.String);
                        storeSubkey.SetValue("inimapping", 0, RegistryValueKind.String);
                    }
                    else
                    {
                        storeSubkey.SetValue("item",
                            Path.GetFileNameWithoutExtension(startupEntry.EntryLongName) ?? string.Empty,
                            RegistryValueKind.String);
                        storeSubkey.SetValue("path", startupEntry.FullLongName, RegistryValueKind.String);
                        storeSubkey.SetValue("location", startupEntry.ParentLongName, RegistryValueKind.String);
                        storeSubkey.SetValue("backup", newEntryPath, RegistryValueKind.String);
                        storeSubkey.SetValue("backupExtension", BackupExtension, RegistryValueKind.String);
                    }

                    // Command stays the same for both
                    storeSubkey.SetValue("command", startupEntry.Command, RegistryValueKind.String);

                    // Set the disable date
                    var now = DateTime.Now;
                    storeSubkey.SetValue("YEAR", now.Year, RegistryValueKind.DWord);
                    storeSubkey.SetValue("MONTH", now.Month, RegistryValueKind.DWord);
                    storeSubkey.SetValue("DAY", now.Day, RegistryValueKind.DWord);
                    storeSubkey.SetValue("HOUR", now.Hour, RegistryValueKind.DWord);
                    storeSubkey.SetValue("MINUTE", now.Minute, RegistryValueKind.DWord);
                    storeSubkey.SetValue("SECOND", now.Second, RegistryValueKind.DWord);
                }
            }
        }

        private static string CreateDisabledEntryPath(StartupEntryBase startupEntry)
        {
            return Path.Combine(DriveDisableBackupPath, startupEntry.EntryLongName + BackupExtension);
        }

        /// <summary>
        ///     Remove registry key of a disabled startup entry. Link file is not touched if it exists.
        /// </summary>
        private static void RemoveDisabledRegEntry(StartupEntry startupEntry)
        {
            using (var disabledStartupEntryStore = RegistryTools.OpenRegistryKey(
                startupEntry.IsRegKey ? RegistryDisabledKey.Path : DriveDisabledKey.Path, true))
            {
                var disabledSubKeyName = startupEntry.IsRegKey
                    ? startupEntry.EntryLongName
                    : startupEntry.FullLongName.Replace('\\', '^');
                var disabledSubkeyKey =
                    disabledStartupEntryStore.GetSubKeyNames()
                        .FirstOrDefault(x => disabledSubKeyName.Equals(x, StringComparison.InvariantCultureIgnoreCase));

                if (!string.IsNullOrEmpty(disabledSubkeyKey))
                {
                    disabledStartupEntryStore.DeleteSubKey(disabledSubkeyKey);
                }
            }
        }
    }
}