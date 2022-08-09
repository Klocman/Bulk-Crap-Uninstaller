/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Klocman.Extensions;
using Klocman.Properties;
using Microsoft.Win32;

namespace Klocman.Tools
{
    public static class RegistryTools
    {
        private const string HklmShortRootName = "HKLM";
        private const string HklmRootName = "HKEY_LOCAL_MACHINE";
        private const string HkcrRootName = "HKEY_CLASSES_ROOT";
        private const string HkcrShortRootName = "HKCR";
        private const string HkcuRootName = "HKEY_CURRENT_USER";
        private const string HkcuShortRootName = "HKCU";
        private const string HkuRootName = "HKEY_USERS";
        private const string HkuShortRootName = "HKUS";
        private const string HkuShortRootName2 = "HKU";
        private const string HkccRootName = "HKEY_CURRENT_CONFIG";
        private const string HkccShortRootName = "HKCC";

        /// <summary>
        ///     Rename subkey.
        /// </summary>
        public static void RenameSubKey(this RegistryKey parentKey,
            string subKeyName, string newSubKeyName)
        {
            CopySubKey(parentKey, subKeyName, newSubKeyName);
            parentKey.DeleteSubKeyTree(subKeyName);
        }

        /// <summary>
        ///     Move subkey under a new parent.
        /// </summary>
        public static void MoveSubKey(this RegistryKey parentKey,
            string subKeyName, RegistryKey newParentKey, string newSubKeyName)
        {
            CopySubKey(parentKey, subKeyName, newParentKey, newSubKeyName);
            parentKey.DeleteSubKeyTree(subKeyName);
        }

        /// <summary>
        ///     Copy subkey.
        /// </summary>
        public static void CopySubKey(this RegistryKey parentKey,
            string subKeyName, string newSubKeyName)
        {
            CopySubKey(parentKey, subKeyName, parentKey, newSubKeyName);
        }

        /// <summary>
        ///     Copy subkey to a different parent key.
        /// </summary>
        public static void CopySubKey(this RegistryKey parentKey,
            string subKeyName, RegistryKey newParentKey, string newSubKeyName)
        {
            using (var destinationKey = newParentKey.CreateSubKey(newSubKeyName))
            using (var sourceKey = parentKey.OpenSubKey(subKeyName, true))
                RecurseCopyKey(sourceKey, destinationKey);
        }

        private static void RecurseCopyKey(RegistryKey sourceKey, RegistryKey destinationKey)
        {
            foreach (var valueName in sourceKey.GetValueNames())
            {
                var valueData = sourceKey.GetValue(valueName);
                var valueKind = sourceKey.GetValueKind(valueName);
                destinationKey.SetValue(valueName, valueData!, valueKind);
            }

            foreach (var sourceSubKeyName in sourceKey.GetSubKeyNames())
            {
                using (var destSubKey = destinationKey.CreateSubKey(sourceSubKeyName))
                using (var sourceSubKey = sourceKey.OpenSubKey(sourceSubKeyName, true))
                    RecurseCopyKey(sourceSubKey, destSubKey);
            }
        }

        public static void AddRegToRegistry(string fullFilename, bool silent)
        {
            if (fullFilename == null)
                throw new ArgumentNullException(nameof(fullFilename));
            if (!File.Exists(fullFilename) || !fullFilename.EndsWith(".reg", StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException(Localisation.RegistryTools_AddRegToRegistry_FileNotExist,
                    nameof(fullFilename));

            RunRegeditCommand($"{(silent ? "/s " : string.Empty)}\"{fullFilename}\"");
        }

        /// <summary>
        ///     Export all of the supplied keys to a .reg file using Regedit
        /// </summary>
        /// <param name="outputFileName"></param>
        /// <param name="registryPaths"></param>
        /// <returns>False if nothing was written, else true</returns>
        public static bool ExportRegistry(string outputFileName, IEnumerable<string> registryPaths)
        {
            var result = new List<string>();
            var firstPass = true;

            foreach (var regPath in registryPaths)
            {
                var output = ExportRegistryHelper(regPath);
                if (output == null || output.Length < 2)
                    continue;

                if (!firstPass)
                {
                    output = output.SubArray(1, output.Length - 1);
                }

                firstPass = false;

                result.AddRange(output);
            }

            if (result.Count < 2)
                return false;

            File.WriteAllLines(outputFileName, result.ToArray(), Encoding.Unicode);
            return true;
        }

        /// <summary>
        /// Write specified unescaped values to a .reg file
        /// </summary>
        /// <param name="outputFileName">Filename with extension to save as</param>
        /// <param name="containingKeyPath">Full, rooted registry path of the key containing the values</param>
        /// <param name="values">Value names and their string values</param>
        public static void ExportRegistryStringValues(string outputFileName, string containingKeyPath,
            params KeyValuePair<string, string>[] values)
        {
            var builder = new StringBuilder();
            builder.AppendLine(@"Windows Registry Editor Version 5.00");
            foreach (var value in values)
            {
                builder.AppendLine();
                builder.AppendFormat(@"[{0}]", containingKeyPath);
                builder.AppendLine();
                builder.AppendFormat("\"{0}\"=\"{1}\"", value.Key, 
                    (value.Value ?? string.Empty).Replace(@"\", @"\\").Replace("\"", "\\\""));
                builder.AppendLine();
            }
            File.WriteAllText(outputFileName, builder.ToString());
        }

        /// <exception cref="IOException">Registry export failed because of filesystem or permission error. </exception>
        public static void ExportRegistry(string outputFileName, string registryPath)
        {
            RunRegeditCommand($"/e \"{outputFileName}\" \"{registryPath}\"");
        }

        /// <summary>
        ///     Open registry key using its fully qualified path. The key is opened read-only.
        ///     Root key can be named by either its long or short name. (long: "HKEY_LOCAL_MACHINE", short: "HKLM")
        /// </summary>
        /// <param name="fullPath">Full path of the requested registry key</param>
        public static RegistryKey OpenRegistryKey(string fullPath)
        {
            return OpenRegistryKey(fullPath, false);
        }

        /// <summary>
        ///     Open registry key using its fully qualified path.
        ///     Root key can be named by either its long or short name. (long: "HKEY_LOCAL_MACHINE", short: "HKLM")
        /// </summary>
        /// <param name="fullPath">Full path of the requested registry key</param>
        /// <param name="writable">If false, key is opened read-only</param>
        /// <param name="ignoreAccessExceptions">If true, return null instead of throwin an exception if the key is inaccessible</param>
        public static RegistryKey OpenRegistryKey(string fullPath, bool writable, bool ignoreAccessExceptions)
        {
            if(!ignoreAccessExceptions)
                return OpenRegistryKey(fullPath, writable);

            try
            {
                return OpenRegistryKey(fullPath, writable);
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException || ex is System.Security.SecurityException || ex is IOException)
                {
                    Debug.WriteLine(ex);
                    return null;
                }

                throw;
            }
        }

        /// <summary>
        ///     Open registry key using its fully qualified path.
        ///     Root key can be named by either its long or short name. (long: "HKEY_LOCAL_MACHINE", short: "HKLM")
        /// </summary>
        /// <param name="fullPath">Full path of the requested registry key</param>
        /// <param name="writable">If false, key is opened read-only</param>
        public static RegistryKey OpenRegistryKey(string fullPath, bool writable)
        {
            if (fullPath == null)
                throw new ArgumentNullException(nameof(fullPath));

            if (fullPath.Length < 4)
                throw new ArgumentException("Path is too short/invalid");

            var rootKey = GetRootHive(fullPath);

            var result = rootKey.OpenSubKey(StripKeyRoot(fullPath), writable);
            //if (result == null)
            //    throw new ArgumentException("Invalid subpath");
            return result;
        }

        [return: NotNull]
        private static RegistryKey GetRootHive(string fullPath)
        {
            RegistryKey rootKey;
            switch (GetKeyRoot(fullPath, true))
            {
                case HklmShortRootName:
                    rootKey = Registry.LocalMachine;
                    break;
                case HkcrShortRootName:
                    rootKey = Registry.ClassesRoot;
                    break;
                case HkcuShortRootName:
                    rootKey = Registry.CurrentUser;
                    break;
                case HkuShortRootName:
                case HkuShortRootName2:
                    rootKey = Registry.Users;
                    break;
                case HkccShortRootName:
                    rootKey = Registry.CurrentConfig;
                    break;

                default:
                    throw new ArgumentException("Path root is invalid or missing");
            }

            return rootKey;
        }

        /// <summary>
        ///     Return registry key at supplied path. If the key or its parents don't exist, create them before returning.
        ///     The returned RegistryKey is writable.
        /// </summary>
        /// <param name="fullPath">Path of the key to open or create. Not case-sensitive.</param>
        public static RegistryKey CreateSubKeyRecursively(string fullPath)
        {
            if (fullPath == null)
                throw new ArgumentNullException(nameof(fullPath));

            if (fullPath.Length < 4)
                throw new ArgumentException("Path is too short/invalid");

            var previousKey = GetRootHive(fullPath);

            var parts = StripKeyRoot(fullPath).Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < parts.Length; i++)
            {
                var newKey = previousKey!.CreateSubKey(parts[i]);
                // Don't try to close the root key
                if (i > 0)
                    previousKey.Close();
                previousKey = newKey;
            }

            return previousKey;
        }

        public static string GetKeyRoot(string fullPath, bool shortStyle)
        {
            if (fullPath == null)
                throw new ArgumentNullException(nameof(fullPath));

            if (fullPath.Length < 3)
                throw new ArgumentException("Path is too short/invalid");

            var firstSplitter = fullPath.IndexOf('\\');
            if (firstSplitter < 0) firstSplitter = fullPath.Length;
            var rootName = fullPath.Substring(0, firstSplitter).ToUpperInvariant();

            switch (rootName)
            {
                case HklmRootName:
                case HklmShortRootName:
                    return shortStyle ? HklmShortRootName : HklmRootName;
                case HkcrRootName:
                case HkcrShortRootName:
                    return shortStyle ? HkcrShortRootName : HkcrRootName;
                case HkcuRootName:
                case HkcuShortRootName:
                    return shortStyle ? HkcuShortRootName : HkcuRootName;
                case HkuRootName:
                case HkuShortRootName:
                case HkuShortRootName2:
                    return shortStyle ? HkuShortRootName : HkuRootName;
                case HkccRootName:
                case HkccShortRootName:
                    return shortStyle ? HkccShortRootName : HkccRootName;

                default:
                    throw new ArgumentException("Path root is invalid or missing");
            }
        }

        public static string StripKeyRoot(string fullPath)
        {
            if (fullPath == null)
                throw new ArgumentNullException(nameof(fullPath));

            if (fullPath.Length < 4)
                throw new ArgumentException("Path is too short/invalid");

            var firstSplitter = fullPath.IndexOf('\\');
            if (firstSplitter < 0) firstSplitter = fullPath.Length;
            return firstSplitter >= fullPath.Length - 1
                ? string.Empty
                : fullPath.Substring(firstSplitter + 1);
        }

        /*/// <exception cref="ArgumentException">Path can't be empty or null</exception>
        public static string GetParentRegistryPath(string fullRegistryPath)
        {
            if (string.IsNullOrEmpty(fullRegistryPath))
                throw new ArgumentException("Path can't be empty or null", "fullRegistryPath");
            fullRegistryPath.TrimEnd('\\');
            var lastIndex = fullRegistryPath.LastIndexOf('\\');
            return fullRegistryPath.Substring(0, lastIndex);
        }*/

        public static void RemoveRegistryKey(string fullRegistryPath)
        {
            if (string.IsNullOrEmpty(fullRegistryPath))
                throw new ArgumentException(Localisation.RegistryTools_RemoveRegistryKey_PathEmptyNull,
                    nameof(fullRegistryPath));

            if (fullRegistryPath.Count(x => x.Equals('\\')) < 2)
                throw new ArgumentException(Localisation.RegistryTools_RemoveRegistryKey_PointsAtRoot,
                    nameof(fullRegistryPath));

            using (var key = OpenRegistryKey(Path.GetDirectoryName(fullRegistryPath), true))
            {
                if (key != null)
                {
                    var subkeyName = Path.GetFileName(fullRegistryPath);
                    // Check if key exists before attempting to remove to avoid an exception
                    if (key.GetSubKeyNames().Contains(subkeyName, StringComparison.OrdinalIgnoreCase))
                        key.DeleteSubKeyTree(subkeyName);
                }
            }
        }

        public static void RemoveRegistryValue(string fullRegistryPath, string valueName)
        {
            if (string.IsNullOrEmpty(fullRegistryPath))
                throw new ArgumentException(Localisation.RegistryTools_RemoveRegistryKey_PathEmptyNull,
                    nameof(fullRegistryPath));

            if (string.IsNullOrEmpty(valueName))
                throw new ArgumentException(Localisation.RegistryTools_RemoveRegistryKey_RemoveDefault,
                    nameof(valueName));

            if (fullRegistryPath.Count(x => x.Equals('\\')) < 2)
                throw new ArgumentException(Localisation.RegistryTools_RemoveRegistryKey_PointsAtRoot,
                    nameof(fullRegistryPath));

            using (var key = OpenRegistryKey(fullRegistryPath, true))
            {
                key?.DeleteValue(valueName);
            }
        }

        private static void RunRegeditCommand(string command)
        {
            foreach (var process in Process.GetProcessesByName("regedit"))
            {
                try { process.Kill(); }
                catch (InvalidOperationException) { }
                catch (Win32Exception) { }
            }

            var startInfo = new ProcessStartInfo("regedit.exe", command)
            { UseShellExecute = false };
            var pr = startInfo.Start();

            if (!pr.WaitForExit(50000))
            {
                pr.Kill();
                throw new IOException("regedit.exe failed to execute properly");
            }
            if (pr.ExitCode != 0)
                throw new IOException("regedit.exe returned error code: " + pr.ExitCode);
        }

        private static string[] ExportRegistryHelper(string registryPath)
        {
            // Check if the key exists
            try
            {
                OpenRegistryKey(registryPath).Close();
            }
            catch
            {
                return null;
            }

            var temp = Path.GetTempPath();
            var tempDirectory = Directory.CreateDirectory(Path.Combine(temp, "BCU"));
            var tempFile = Path.Combine(tempDirectory.FullName, "tempBackup.reg");

            File.Delete(tempFile);

            try
            {
                RunRegeditCommand($"/e \"{tempFile}\" \"{registryPath}\"");
                return File.ReadAllLines(tempFile, Encoding.Unicode);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        /// <exception cref="IOException">Could not complete this operation. </exception>
        public static void OpenRegKeyInRegedit(string registryPath)
        {
            try
            {
                if (!ProcessTools.SafeKillProcess("regedit"))
                    throw new IOException("Could not close running instance of Regedit");

                var regeditSettings =
                    Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Applets\Regedit",
                        RegistryKeyPermissionCheck.ReadWriteSubTree);

                if (regeditSettings == null)
                    throw new IOException("Failed to set the last user registry key");

                regeditSettings.SetValue("LastKey", registryPath, RegistryValueKind.String);
                Process.Start("regedit.exe");
            }
            catch (Exception ex)
            {
                throw new IOException("Failed to open regedit.exe", ex);
            }
        }
    }
}