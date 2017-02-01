/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Tools;
using Microsoft.Win32;
using UninstallTools.Factory.InfoAdders;

namespace UninstallTools.Factory
{
    public class RegistryFactory : IUninstallerFactory
    {
        private static readonly string RegUninstallersKeyDirect = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        private static readonly string RegUninstallersKeyWow =
            @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

        public IEnumerable<ApplicationUninstallerEntry> GetUninstallerEntries()
        {
            var applicationUninstallers = new List<ApplicationUninstallerEntry>();

            var keysToCheck = GetRegistryKeys();
            var uninstallersToCreate = new List<KeyValuePair<RegistryKey, bool>>();

            foreach (var kvp in keysToCheck.Where(kvp => kvp.Key != null))
            {
                uninstallersToCreate.AddRange(from subkeyName in kvp.Key.GetSubKeyNames()
                    select kvp.Key.OpenSubKey(subkeyName)
                    into subkey
                    where subkey != null
                    select new KeyValuePair<RegistryKey, bool>(subkey, kvp.Value));

                kvp.Key.Close();
            }

            //var itemId = 0;
            foreach (var uninstallerToCreate in uninstallersToCreate)
            {
                try
                {
                    var entry = TryCreateFromRegistry(uninstallerToCreate.Key,
                        uninstallerToCreate.Value);
                    if (entry != null)
                        applicationUninstallers.Add(entry);
                }
                catch
                {
                    //Uninstaller is invalid or there is no uninstaller in the first place. Skip it to avoid problems.
                }
                finally
                {
                    uninstallerToCreate.Key.Close();

                    //itemId++;
                    //var progress = new GetUninstallerListProgress(uninstallersToCreate.Count) { CurrentCount = itemId };
                    //callback(progress);
                }
            }

            return applicationUninstallers;
        }

        private static string GetAboutUrl(RegistryKey uninstallerKey)
        {
            return ApplicationUninstallerEntry.RegistryNamesOfUrlSources.Select(urlSource =>
                uninstallerKey.GetValue(urlSource) as string)
                .FirstOrDefault(tempSource => !string.IsNullOrEmpty(tempSource) && tempSource.Contains('.'));
        }

        private static ApplicationUninstallerEntry GetBasicInformation(RegistryKey uninstallerKey)
        {
            return new ApplicationUninstallerEntry
            {
                RegistryPath = uninstallerKey.Name,
                RegistryKeyName = uninstallerKey.GetKeyName(),
                Comment = uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameComment) as string,
                RawDisplayName = uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameDisplayName) as string,
                DisplayVersion = uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameDisplayVersion) as string,
                ParentKeyName = uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameParentKeyName) as string,
                Publisher = uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNamePublisher) as string,
                UninstallString = uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameUninstallString) as string,
                QuietUninstallString = uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameQuietUninstallString) as string,
                ModifyPath = uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameModifyPath) as string,
                InstallLocation = uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameInstallLocation) as string,
                InstallSource = uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameInstallSource) as string,
                SystemComponent = (int) uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameSystemComponent, 0) != 0,
                DisplayIcon = uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameDisplayIcon) as string
            };
        }

        private static FileSize GetEstimatedSize(RegistryKey uninstallerKey)
        {
            var tempSize = (int) uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameEstimatedSize, 0);
            return FileSize.FromKilobytes(tempSize);
        }

        private static Guid GetGuid(RegistryKey uninstallerKey)
        {
            // Look for a GUID registry entry
            var tempGuidString =
                uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameBundleProviderKey) as string;
            Guid resultGuid;

            if (GuidTools.GuidTryParse(tempGuidString, out resultGuid))
                return resultGuid;

            if (GuidTools.TryExtractGuid(uninstallerKey.GetKeyName(), out resultGuid))
                return resultGuid;

            var uninstallString =
                uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameUninstallString) as string;
            // Look for a valid GUID in the path
            return GuidTools.TryExtractGuid(uninstallString, out resultGuid) ? resultGuid : Guid.Empty;
        }

        private static DateTime GetInstallDate(RegistryKey uninstallerKey)
        {
            var dateString = uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameInstallDate) as string;
            if (dateString != null && dateString.Length == 8)
            {
                try
                {
                    return new DateTime(int.Parse(dateString.Substring(0, 4)),
                        int.Parse(dateString.Substring(4, 2)),
                        int.Parse(dateString.Substring(6, 2)));
                }
                catch (FormatException)
                {
                }
                catch (ArgumentException)
                {
                }
            }

            return DateTime.MinValue;
        }

        private static bool GetIsUpdate(RegistryKey uninstallerKey)
        {
            var parentKeyName = uninstallerKey.GetValue("ParentKeyName", string.Empty) as string;
            if (parentKeyName.IsNotEmpty())
                return true;

            var releaseType = uninstallerKey.GetValue("ReleaseType", string.Empty) as string;
            if (releaseType.IsNotEmpty() &&
                releaseType.ContainsAny(new[] {"Update", "Hotfix"}, StringComparison.OrdinalIgnoreCase))
                return true;

            var defaultValue = uninstallerKey.GetValue(null) as string;
            if (string.IsNullOrEmpty(defaultValue))
                return false;

            //Regex WindowsUpdateRegEx = new Regex(@"KB[0-9]{6}$"); //Doesnt work for all cases
            return defaultValue.Length > 6 && defaultValue.StartsWith("KB", StringComparison.Ordinal)
                   && char.IsNumber(defaultValue[2]) && char.IsNumber(defaultValue.Last());
        }

        private static bool GetProtectedFlag(RegistryKey uninstallerKey)
        {
            return (int) uninstallerKey.GetValue("NoRemove", 0) != 0;
        }

        private static List<KeyValuePair<RegistryKey, bool>> GetRegistryKeys()
        {
            var keysToCheck = new List<KeyValuePair<RegistryKey, bool>>();

            var hklm = Registry.LocalMachine;
            var hkcu = Registry.CurrentUser;

            if (ProcessTools.Is64BitProcess)
            {
                keysToCheck.Add(new KeyValuePair<RegistryKey, bool>(hklm.OpenSubKey(RegUninstallersKeyDirect), true));
                keysToCheck.Add(new KeyValuePair<RegistryKey, bool>(hkcu.OpenSubKey(RegUninstallersKeyDirect), true));

                keysToCheck.Add(new KeyValuePair<RegistryKey, bool>(hklm.OpenSubKey(RegUninstallersKeyWow), false));
                keysToCheck.Add(new KeyValuePair<RegistryKey, bool>(hkcu.OpenSubKey(RegUninstallersKeyWow), false));
            }
            else
            {
                keysToCheck.Add(new KeyValuePair<RegistryKey, bool>(hklm.OpenSubKey(RegUninstallersKeyDirect), false));
                keysToCheck.Add(new KeyValuePair<RegistryKey, bool>(hkcu.OpenSubKey(RegUninstallersKeyDirect), false));
            }
            return keysToCheck;
        }

        /// <summary>
        ///     Tries to create a new uninstaller entry. If the registry key doesn't contain valid uninstaller
        ///     information, null is returned. It will throw ArgumentNullException if passed uninstallerKey is null.
        ///     If there are any problems while reading the registry an exception will be thrown as well.
        /// </summary>
        /// <param name="uninstallerKey">Registry key which contains the uninstaller information.</param>
        /// <param name="is64Bit">Is the registry key pointing to a 64 bit subkey?</param>
        public static ApplicationUninstallerEntry TryCreateFromRegistry(RegistryKey uninstallerKey, bool is64Bit)
        {
            if (uninstallerKey == null)
                throw new ArgumentNullException(nameof(uninstallerKey));

            var tempEntry = GetBasicInformation(uninstallerKey);
            tempEntry.IsRegistered = true;

            // Check for invalid registry key
            if (tempEntry.RawDisplayName == null)
            {
                if (tempEntry.Publisher == null && !tempEntry.UninstallPossible && !tempEntry.QuietUninstallPossible)
                {
                    //throw new ArgumentException("Supplied key doesn't contain any useful information");
                    return null;
                }
                tempEntry.RawDisplayName = string.Empty;
            }

            // Get rest of the information
            tempEntry.IsProtected = GetProtectedFlag(uninstallerKey);
            tempEntry.InstallDate = GetInstallDate(uninstallerKey);
            tempEntry.EstimatedSize = GetEstimatedSize(uninstallerKey);
            tempEntry.AboutUrl = GetAboutUrl(uninstallerKey);

            tempEntry.Is64Bit = is64Bit ? MachineType.X64 : MachineType.X86;
            tempEntry.IsUpdate = GetIsUpdate(uninstallerKey);

            // Figure out what we are dealing with
            tempEntry.UninstallerKind = UninstallerTypeAdder.GetUninstallerType(uninstallerKey);
            tempEntry.BundleProviderKey = GetGuid(uninstallerKey);

            // Corner case with some microsoft application installations.
            // They will sometimes create a naked registry key (product code as reg name) with only the display name value.
            if (tempEntry.UninstallerKind != UninstallerType.Msiexec && tempEntry.BundleProviderKey != Guid.Empty
                && !tempEntry.UninstallPossible && !tempEntry.QuietUninstallPossible)
            {
                if (ApplicationUninstallerManager.WindowsInstallerValidGuids.Contains(tempEntry.BundleProviderKey))
                    tempEntry.UninstallerKind = UninstallerType.Msiexec;
            }

            // Fill in missing fields with information that can now be obtained
            if (tempEntry.UninstallerKind == UninstallerType.Msiexec)
                MsiInfoAdder.ApplyMsiInfo(tempEntry, tempEntry.BundleProviderKey);

            // Use quiet uninstall string as normal uninstall string if the normal string is missing.
            if (!tempEntry.UninstallPossible && tempEntry.QuietUninstallPossible)
                tempEntry.UninstallString = tempEntry.QuietUninstallString;

            // Finish up setting file/folder paths
            tempEntry.UninstallerFullFilename =
                ApplicationUninstallerFactory.GetUninstallerFilename(tempEntry.UninstallString,
                    tempEntry.UninstallerKind, tempEntry.BundleProviderKey);

            if (tempEntry.InstallLocation != null)
                tempEntry.InstallLocation = CleanupPath(tempEntry.InstallLocation);
            else if (tempEntry.UninstallerKind == UninstallerType.Nsis ||
                     tempEntry.UninstallerKind == UninstallerType.InnoSetup)
                tempEntry.InstallLocation = CleanupPath(tempEntry.UninstallerLocation);

            if (tempEntry.InstallSource != null)
                tempEntry.InstallSource = CleanupPath(tempEntry.InstallSource);

            // Fill in the install date if it's missing
            try
            {
                if (tempEntry.InstallDate.IsDefault() && !string.IsNullOrEmpty(tempEntry.UninstallerFullFilename))
                {
                    tempEntry.InstallDate = File.GetCreationTime(tempEntry.UninstallerFullFilename);
                }
            }
            catch
            {
                tempEntry.InstallDate = DateTime.MinValue;
            }

            // Misc
            tempEntry.IsValid = IsValidAdder.GetIsValid(tempEntry.UninstallString,
                tempEntry.UninstallerFullFilename,
                tempEntry.UninstallerKind, tempEntry.BundleProviderKey);

            return tempEntry;
        }

        static string CleanupPath(string path)
        {
            if (path == null) return null;

            path = path.Trim('"', ' ', '\'', '\\', '/'); // Get rid of the quotation marks
            try
            {
                var i = path.LastIndexOf('\\');
                if (i > 0 && path.Substring(i).Contains('.') && !Directory.Exists(path))
                {
                    path = Path.GetDirectoryName(ProcessTools.SeparateArgsFromCommand(path).FileName);
                }
            }
            catch
            {
                // If sanitization failed just leave it be, it will be handled afterwards
            }
            return path;
        }
    }
}