using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Native;
using Klocman.Tools;
using Microsoft.Win32;

namespace UninstallTools.Uninstaller
{
    public static class ApplicationUninstallerFactory
    {
        private static readonly string[] BinaryDirectoryNames =
        {
            "bin", "program", "client", "app", "application" //"system"
        };

        private static readonly string[] IconNames =
        {
            "DisplayIcon.ico", "Icon.ico", "app.ico", "appicon.ico",
            "application.ico", "logo.ico"
        };

        private static string _assemblyLocation;
        private static string AssemblyLocation
        {
            get
            {
                if (_assemblyLocation == null)
                {
                    _assemblyLocation = Assembly.GetExecutingAssembly().Location;
                    if (_assemblyLocation.ContainsAny(new[] { ".dll", ".exe" }, StringComparison.OrdinalIgnoreCase))
                        _assemblyLocation = PathTools.GetDirectory(_assemblyLocation);
                }
                return _assemblyLocation;
            }
        }

        private static string StoreAppHelperPath => Path.Combine(AssemblyLocation, @"StoreAppHelper.exe");
        private static string SteamHelperPath => Path.Combine(AssemblyLocation, @"SteamHelper.exe");

        private static bool? _steamHelperIsAvailable;
        private static string _steamLocation;
        public static bool SteamHelperIsAvailable
        {
            get
            {
                if (!_steamHelperIsAvailable.HasValue)
                {
                    _steamHelperIsAvailable = false;
                    if (File.Exists(SteamHelperPath) && WindowsTools.CheckNetFramework4Installed(true))
                    {
                        var output = StartProcessAndReadOutput(SteamHelperPath, "steam");
                        if (!string.IsNullOrEmpty(output)
                            && !output.Contains("error", StringComparison.InvariantCultureIgnoreCase)
                            && Directory.Exists(output = output.Trim().TrimEnd('\\', '/')))
                        {
                            _steamHelperIsAvailable = true;
                            _steamLocation = output;
                        }
                    }
                }
                return _steamHelperIsAvailable.Value;
            }
        }

        /// <summary>
        /// Use our helper instead of the built-in Steam uninstaller
        /// </summary>
        public static void ChangeSteamAppUninstallStringToHelper(ApplicationUninstallerEntry entryToModify)
        {
            if (entryToModify.UninstallerKind != UninstallerType.Steam || !SteamHelperIsAvailable) return;

            var appId = entryToModify.RatingId.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
            entryToModify.UninstallString = $"\"{SteamHelperPath}\" uninstall {appId}";
            entryToModify.QuietUninstallString = $"\"{ SteamHelperPath}\" uninstall /silent {appId}";
        }

        public static IEnumerable<ApplicationUninstallerEntry> GetSteamApps()
        {
            if (!SteamHelperIsAvailable)
                yield break;

            var output = StartProcessAndReadOutput(SteamHelperPath, "list");
            if (string.IsNullOrEmpty(output) || output.Contains("error", StringComparison.InvariantCultureIgnoreCase))
                yield break;

            foreach (var idString in output.SplitNewlines(StringSplitOptions.RemoveEmptyEntries))
            {
                int appId;
                if (!int.TryParse(idString, out appId)) continue;

                output = StartProcessAndReadOutput(SteamHelperPath, "info " + appId.ToString("G"));
                if (string.IsNullOrEmpty(output) || output.Contains("error", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var lines = output.SplitNewlines(StringSplitOptions.RemoveEmptyEntries).Select(x =>
                {
                    var o = x.Split(new[] { " - " }, StringSplitOptions.None);
                    return new KeyValuePair<string, string>(o[0], o[1]);
                }).ToList();

                var entry = new ApplicationUninstallerEntry
                {
                    DisplayName = lines.Single(x => x.Key.Equals("Name", StringComparison.InvariantCultureIgnoreCase)).Value,
                    UninstallString = lines.Single(x => x.Key.Equals("UninstallString", StringComparison.InvariantCultureIgnoreCase)).Value,
                    InstallLocation = lines.Single(x => x.Key.Equals("InstallDirectory", StringComparison.InvariantCultureIgnoreCase)).Value,
                    UninstallerKind = UninstallerType.Steam,
                    IsValid = true,
                    IsOrphaned = true,
                    RatingId = "Steam App " + appId.ToString("G")
                };

                long bytes;
                if (long.TryParse(lines.Single(x => x.Key.Equals("SizeOnDisk", StringComparison.InvariantCultureIgnoreCase)).Value, out bytes))
                    entry.EstimatedSize = FileSize.FromBytes(bytes);

                yield return entry;
            }
        }

        private static string StartProcessAndReadOutput(string filename, string args)
        {
            using (var process = Process.Start(new ProcessStartInfo(filename, args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.Unicode
            })) return process?.StandardOutput.ReadToEnd();
        }

        public static IEnumerable<ApplicationUninstallerEntry> GetStoreApps()
        {
            if (!WindowsTools.CheckNetFramework4Installed(true) || !File.Exists(StoreAppHelperPath))
                yield break;

            var output = StartProcessAndReadOutput(StoreAppHelperPath, "/query");
            if (string.IsNullOrEmpty(output))
                yield break;

            var windowsPath = WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_WINDOWS);

            var parts = output.SplitNewlines(StringSplitOptions.None);
            var current = parts.Take(5).ToList();
            while (current.Count == 5)
            {
                /*
               @"FullName: "
               @"DisplayName: "
               @"PublisherDisplayName: "
               @"Logo: "
               @"InstalledLocation: "
               */

                //Trim the labels
                for (var i = 0; i < current.Count; i++)
                    current[i] = current[i].Substring(current[i].IndexOf(" ", StringComparison.Ordinal)).Trim();

                if (Directory.Exists(current[4]))
                {
                    var uninstallStr = $"\"{StoreAppHelperPath}\" /uninstall \"{current[0]}\"";
                    var result = new ApplicationUninstallerEntry
                    {
                        RatingId = current[0],
                        UninstallString = uninstallStr,
                        QuietUninstallString = uninstallStr,
                        RawDisplayName = string.IsNullOrEmpty(current[1]) ? current[0] : current[1],
                        Publisher = current[2],
                        IsValid = true,
                        UninstallerKind = UninstallerType.StoreApp,
                        InstallLocation = current[4],
                        InstallDate = Directory.GetCreationTime(current[4])
                    };

                    if (File.Exists(current[3]))
                    {
                        try
                        {
                            result.DisplayIcon = current[3];
                            result.IconBitmap = DrawingTools.IconFromImage(new Bitmap(current[3]));
                        }
                        catch
                        {
                            result.DisplayIcon = null;
                            result.IconBitmap = null;
                        }
                    }

                    if (result.InstallLocation.StartsWith(windowsPath, StringComparison.InvariantCultureIgnoreCase))
                    {
                        result.SystemComponent = true;
                        //result.IsProtected = true;
                    }

                    yield return result;
                }

                parts = parts.Skip(5).ToArray();
                current = parts.Take(5).ToList();
            }
        }

        public static IEnumerable<ApplicationUninstallerEntry> TryCreateFromDirectory(DirectoryInfo directory,
            bool? is64Bit)
        {
            if (directory == null)
                throw new ArgumentNullException(nameof(directory));

            var results = new List<ApplicationUninstallerEntry>();

            CreateFromDirectoryHelper(results, directory, 0);

            foreach (var tempEntry in results)
            {
                if (is64Bit.HasValue && tempEntry.Is64Bit == MachineType.Unknown)
                    tempEntry.Is64Bit = is64Bit.Value ? MachineType.X64 : MachineType.X86;

                tempEntry.IsRegistered = false;
                tempEntry.IsOrphaned = true;

                tempEntry.UninstallerKind = tempEntry.UninstallPossible ? GetUninstallerType(tempEntry.UninstallString) : UninstallerType.SimpleDelete;

                switch (tempEntry.UninstallerKind)
                {
                    case UninstallerType.InnoSetup:
                        tempEntry.QuietUninstallString = $"\"{tempEntry.UninstallString}\" /SILENT";
                        break;

                    case UninstallerType.Msiexec:
                        Guid resultGuid;
                        if (GuidTools.TryExtractGuid(tempEntry.UninstallString, out resultGuid))
                        {
                            tempEntry.BundleProviderKey = resultGuid;
                            ApplyMsiInfo(tempEntry, resultGuid);
                        }
                        break;

                    default:
                        // Generate uninstall commands if no uninstaller has been found
                        if (string.IsNullOrEmpty(tempEntry.UninstallString))
                        {
                            tempEntry.UninstallString = $"cmd.exe /C del /S \"{tempEntry.InstallLocation}\\\" && pause";
                            tempEntry.QuietUninstallString = $"cmd.exe /C del /F /S /Q \"{tempEntry.InstallLocation}\\\"";
                        }
                        break;
                }

                tempEntry.IsValid = true;
            }

            return results;
        }

        private static ApplicationUninstallerEntry GetOneDrive()
        {
            var result = new ApplicationUninstallerEntry();

            // Check if installed
            try
            {
                using (var key = RegistryTools.OpenRegistryKey(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\OneDrive", false))
                {
                    result.RegistryPath = key.Name;
                    result.RegistryKeyName = key.GetKeyName();

                    result.InstallLocation = key.GetValue("CurrentVersionPath") as string;
                    if (result.InstallLocation == null || !Directory.Exists(result.InstallLocation))
                        return null;

                    result.DisplayIcon = key.GetValue("OneDriveTrigger") as string;
                    result.DisplayVersion = key.GetValue("Version") as string;
                }
            }
            catch
            {
                return null;
            }

            // Check if the uninstaller is available
            var systemRoot = WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_WINDOWS);
            var uninstallPath = Path.Combine(systemRoot, @"System32\OneDriveSetup.exe");
            if (!File.Exists(uninstallPath))
            {
                uninstallPath = Path.Combine(systemRoot, @"SysWOW64\OneDriveSetup.exe");
                if (!File.Exists(uninstallPath))
                    uninstallPath = null;
            }

            if (uninstallPath != null)
            {
                result.IsValid = true;
                result.UninstallString = $"\"{uninstallPath}\" /uninstall";
                result.QuietUninstallString = result.UninstallString;
                if (!File.Exists(result.DisplayIcon))
                    result.DisplayIcon = uninstallPath;
            }

            result.AboutUrl = @"https://onedrive.live.com/";
            result.RawDisplayName = "OneDrive";
            result.Publisher = "Microsoft Corporation";
            result.EstimatedSize = FileSize.FromKilobytes(1024 * 90);
            result.Is64Bit = MachineType.X86;
            result.IsRegistered = true;

            result.UninstallerKind = UninstallerType.Unknown;

            result.InstallDate = Directory.GetCreationTime(result.InstallLocation);

            if (!string.IsNullOrEmpty(result.DisplayIcon))
                result.IconBitmap = Icon.ExtractAssociatedIcon(result.DisplayIcon);

            /*
             TODO Delete these keys to remove OneDrive from explorer. Add to junk remove, or as an extra step to the uninstallation.
             "HKEY_CLASSES_ROOT\CLSID\{018D5C66-4533-4307-9B53-224DE2ED1FE6}" 
             "HKEY_CLASSES_ROOT\Wow6432Node\CLSID\{018D5C66-4533-4307-9B53-224DE2ED1FE6}"
            */

            return result;
        }

        /// <summary>
        /// Get uninstallers that were pre-defined in BCU.
        /// </summary>
        public static IEnumerable<ApplicationUninstallerEntry> GetSpecialUninstallers(IEnumerable<ApplicationUninstallerEntry> entriesToSkip)
        {
            var toSkip = entriesToSkip as IList<ApplicationUninstallerEntry> ?? entriesToSkip.ToList();
            var items = new List<ApplicationUninstallerEntry>();

            if (toSkip.All(x => !x.DisplayName.Equals("OneDrive", StringComparison.InvariantCultureIgnoreCase)))
            {
                var i = GetOneDrive();
                if (i != null)
                    items.Add(i);
            }

            if (SteamHelperIsAvailable && toSkip.All(x => !_steamLocation.Equals(x.InstallLocation, StringComparison.InvariantCultureIgnoreCase)))
            {
                items.Add(new ApplicationUninstallerEntry
                {
                    AboutUrl = @"http://store.steampowered.com/about/",
                    InstallLocation = _steamLocation,
                    DisplayIcon = Path.Combine(_steamLocation, "Steam.exe"),
                    DisplayName = "Steam",
                    UninstallerKind = UninstallerType.Nsis,
                    UninstallString = Path.Combine(_steamLocation, "uninstall.exe"),
                    IsOrphaned = true,
                    IsValid = File.Exists(Path.Combine(_steamLocation, "uninstall.exe")),
                    InstallDate = Directory.GetCreationTime(_steamLocation),
                    Publisher = "Valve Software"
                });
            }

            return items;
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
            tempEntry.UninstallerKind = GetUninstallerType(uninstallerKey);
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
                ApplyMsiInfo(tempEntry, tempEntry.BundleProviderKey);
            
            // Use quiet uninstall string as normal uninstall string if the normal string is missing.
            if (!tempEntry.UninstallPossible && tempEntry.QuietUninstallPossible)
                tempEntry.UninstallString = tempEntry.QuietUninstallString;

            // Finish up setting file/folder paths
            tempEntry.UninstallerFullFilename = GetUninstallerFilename(tempEntry.UninstallString,
                tempEntry.UninstallerKind, tempEntry.BundleProviderKey);

            if (tempEntry.InstallLocation != null)
                tempEntry.InstallLocation = CleanupPath(tempEntry.InstallLocation);
            else if (tempEntry.UninstallerKind == UninstallerType.Nsis || tempEntry.UninstallerKind == UninstallerType.InnoSetup)
                tempEntry.InstallLocation = CleanupPath(tempEntry.UninstallerLocation);

            if (tempEntry.InstallSource != null)
                tempEntry.InstallSource = CleanupPath(tempEntry.InstallSource);

            // Fill in the install date if it's missing
            try
            {
                if (tempEntry.InstallDate.IsDefault() && File.Exists(tempEntry.UninstallerFullFilename))
                {
                    tempEntry.InstallDate = File.GetCreationTime(tempEntry.UninstallerFullFilename);
                }
            }
            catch
            {
                tempEntry.InstallDate = DateTime.MinValue;
            }

            // Misc
            tempEntry.IsValid = GetIsValid(tempEntry.UninstallString, tempEntry.UninstallerFullFilename,
                tempEntry.UninstallerKind, tempEntry.BundleProviderKey);

            return tempEntry;
        }

        internal static X509Certificate2 TryGetCertificate(ApplicationUninstallerEntry entry)
        {
            // Don't even try if the entry is invalid, it will be marked as bad anyways
            if (!entry.IsValid)
                return null;

            X509Certificate2 result = null;
            try
            {
                if (entry.UninstallerKind == UninstallerType.Msiexec)
                {
                    if (entry.IsInstallLocationValid())
                        result = TryExtractCertificateHelper(entry.GetMainExecutableCandidates());

                    // If no certs were found check the MSI store
                    if (result == null)
                        result = MsiTools.GetCertificate(entry.BundleProviderKey);
                }
                else
                {
                    // If no certs were found check the uninstaller
                    result = TryExtractCertificateHelper(entry.GetMainExecutableCandidates());
                    if (result == null && !string.IsNullOrEmpty(entry.UninstallerFullFilename))
                        result = new X509Certificate2(entry.UninstallerFullFilename);
                }
            }
            catch
            {
                // Default to no certificate
                return null;
            }
            return result;
        }

        /// <summary>
        ///     Check first few files from the install directory for certificates
        /// </summary>
        private static X509Certificate2 TryExtractCertificateHelper(string[] fileNames)
        {
            if (fileNames != null)
            {
                foreach (var candidate in fileNames.Take(3))
                {
                    try
                    {
                        return new X509Certificate2(candidate);
                    }
                    catch
                    {
                        // No cert was found
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Run after DisplayIcon, DisplayName, UninstallerKind, InstallLocation, UninstallString have been initialized.
        /// </summary>
        internal static Icon TryGetIcon(ApplicationUninstallerEntry entry, out string path)
        {
            // Check for any specified icons
            if (!string.IsNullOrEmpty(entry.DisplayIcon) && !PathPointsToMsiExec(entry.DisplayIcon))
            {
                string resultFilename = null;

                if (File.Exists(entry.DisplayIcon))
                    resultFilename = entry.DisplayIcon;

                if (resultFilename == null)
                {
                    try
                    {
                        var fName = ProcessTools.SeparateArgsFromCommand(entry.DisplayIcon).FileName;
                        if (fName != null && File.Exists(fName))
                        {
                            resultFilename = fName;
                        }
                    }
                    catch
                    {
                        // Ignore error and try another method
                    }
                }

                if (resultFilename != null)
                {
                    try
                    {
                        var icon = Icon.ExtractAssociatedIcon(resultFilename);
                        if (icon != null)
                        {
                            path = resultFilename;
                            return icon;
                        }
                    }
                    catch (ArgumentException) { }
                }
            }

            // SdbInst uninstallers do not have any executables to check
            if (entry.UninstallerKind == UninstallerType.SdbInst)
            {
                path = null;
                return null;
            }

            string resultPath;
            // Check the install location first, it is most likely to have the program executables
            if (entry.IsInstallLocationValid())
            {
                var result = TryGetIconHelper(entry, out resultPath);
                if (result != null)
                {
                    path = resultPath;
                    return result;
                }
            }

            // If install dir is not provided try extracting icon from the uninstaller
            if (entry.UninstallerFullFilename.IsNotEmpty() && !PathPointsToMsiExec(entry.UninstallerFullFilename)
                && File.Exists(entry.UninstallerFullFilename))
            {
                try
                {
                    var icon = Icon.ExtractAssociatedIcon(entry.UninstallerFullFilename);
                    if (icon != null)
                    {
                        path = entry.UninstallerFullFilename;
                        return icon;
                    }
                }
                catch (ArgumentException) { }
            }

            // Finally try finding other executables in the uninstaller's dir. 
            // Check the InstallLocation again to prevent TryGetIconHelper from running twice
            if (!entry.IsInstallLocationValid())
            {
                var result = TryGetIconHelper(entry, out resultPath);
                if (result != null)
                {
                    path = resultPath;
                    return result;
                }
            }

            // Nothing was found
            path = null;
            return null;
        }

        private static string CleanupPath(string path)
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

        private static void CreateFromDirectoryHelper(ICollection<ApplicationUninstallerEntry> results, DirectoryInfo directory,
            int level)
        {
            if (level >= 2)
                return;

            // Get contents of this directory
            List<string> files = null;
            DirectoryInfo[] dirs = null;
            var binDirs = new List<DirectoryInfo>();

            try
            {
                files = new List<string>(Directory.GetFiles(directory.FullName, "*.exe", SearchOption.TopDirectoryOnly));

                var rawDirs = directory.GetDirectories();
                dirs = rawDirs
                    // Directories with very short names likely contain program files
                    .Where(x => x.Name.Length > 3)
                    // This matches ISO language codes, much faster than a more specific compare
                    .Where(x => x.Name.Length != 5 || !x.Name[2].Equals('-'))
                    .ToArray();

                // Check for the bin directory and add files from it to the scan
                binDirs.AddRange(rawDirs.Where(x => x.Name.StartsWithAny(BinaryDirectoryNames,
                    StringComparison.OrdinalIgnoreCase)));
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }

            // Check if it is impossible or potentially dangerous to process this directory.
            if (files == null || dirs == null || files.Count > 40)
                return;

            if (files.Count == 0 && !binDirs.Any())
            {
                foreach (var dir in dirs)
                {
                    CreateFromDirectoryHelper(results, dir, level + 1);
                }
            }
            else
            {
                var entry = new ApplicationUninstallerEntry();

                // Parse directories into useful information
                if (level > 0 && directory.Name.StartsWithAny(BinaryDirectoryNames, StringComparison.OrdinalIgnoreCase))
                {
                    entry.InstallLocation = directory.Parent?.FullName;
                    entry.RawDisplayName = directory.Parent?.Name;
                }
                else
                {
                    entry.InstallLocation = directory.FullName;
                    entry.RawDisplayName = directory.Name;

                    if (level > 0)
                        entry.Publisher = directory.Parent?.Name;
                }

                // Add files from bin directories
                files.AddRange(binDirs.Aggregate(Enumerable.Empty<string>(),
                    (x, y) => x.Concat(Directory.GetFiles(y.FullName, "*.exe", SearchOption.TopDirectoryOnly))));
                if (files.Count == 0)
                    return;

                // Use string similarity algorithm to find out which executable is likely the main application
                var compareResults =
                    files.OrderBy(
                        x =>
                            StringTools.CompareSimilarity(Path.GetFileNameWithoutExtension(x), entry.DisplayNameTrimmed));

                // Extract info from file metadata
                var compareBestMatchFile = new FileInfo(compareResults.First());
                entry.InstallDate = compareBestMatchFile.CreationTime;
                entry.DisplayIcon = compareBestMatchFile.FullName;
                entry.IconBitmap = Icon.ExtractAssociatedIcon(compareBestMatchFile.FullName);

                try
                {
                    entry.Is64Bit = FilesystemTools.CheckExecutableMachineType(compareBestMatchFile.FullName);
                }
                catch
                {
                    entry.Is64Bit = MachineType.Unknown;
                }

                try
                {
                    FillInformationFromFileAttribs(entry, compareBestMatchFile.FullName, false);
                }
                catch
                {
                    // Not critical
                }

                // Attempt to find an uninstaller application
                var uninstallerFilters = new[] { "unins0", "uninstall", "uninst", "uninstaller" };
                var uninstaller = files.Concat(Directory.GetFiles(directory.FullName, "*.bat",
                    SearchOption.TopDirectoryOnly))
                    .FirstOrDefault(file =>
                    {
                        var name = Path.GetFileNameWithoutExtension(file);
                        if (string.IsNullOrEmpty(name)) return false;
                        return uninstallerFilters.Any(filter =>
                            name.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase) ||
                            name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase));
                    });

                if (uninstaller != null)
                {
                    entry.UninstallString = uninstaller;
                    entry.UninstallerFullFilename = GetUninstallerFilename(entry.UninstallString,
                        UninstallerType.Unknown, Guid.Empty);
                    entry.IsValid = true;
                }
                else
                {
                    entry.IsValid = false;
                }

                results.Add(entry);
            }
        }

        /// <summary>
        /// Add information from FileVersionInfo of specified file to the targetEntry
        /// </summary>
        /// <param name="targetEntry">Entry to update</param>
        /// <param name="infoSourceFilename">Binary file to get the information from</param>
        /// <param name="onlyUnpopulated">Only update unpopulated fields of the targetEntry</param>
        private static void FillInformationFromFileAttribs(ApplicationUninstallerEntry targetEntry, string infoSourceFilename, bool onlyUnpopulated)
        {
            var verInfo = FileVersionInfo.GetVersionInfo(infoSourceFilename);

            if (!(onlyUnpopulated && !string.IsNullOrEmpty(targetEntry.Publisher?.Trim()))
                && !string.IsNullOrEmpty(verInfo.CompanyName?.Trim()))
                targetEntry.Publisher = verInfo.CompanyName;

            if (!(onlyUnpopulated && !string.IsNullOrEmpty(targetEntry.RawDisplayName?.Trim()))
                && !string.IsNullOrEmpty(verInfo.ProductName?.Trim()))
                targetEntry.RawDisplayName = verInfo.ProductName;

            if (!(onlyUnpopulated && !string.IsNullOrEmpty(targetEntry.Comment?.Trim()))
                && !string.IsNullOrEmpty(verInfo.Comments?.Trim()))
                targetEntry.Comment = verInfo.Comments;

            if (!(onlyUnpopulated && !string.IsNullOrEmpty(targetEntry.DisplayVersion?.Trim())))
            {
                if (!string.IsNullOrEmpty(verInfo.ProductVersion?.Trim()))
                    targetEntry.DisplayVersion = verInfo.ProductVersion;
                else if (!string.IsNullOrEmpty(verInfo.FileVersion?.Trim()))
                    targetEntry.DisplayVersion = verInfo.FileVersion;
            }
        }

        /// <summary>
        ///     A valid guid is REQUIRED. It doesn't have to be set on the entry, but should be.
        ///     IMPORTANT: Run at the very end of the object creation!
        /// </summary>
        private static void ApplyMsiInfo(ApplicationUninstallerEntry entry, Guid guid)
        {
            //IMPORTANT: If MsiGetProductInfo returns null it means that the guid is invalid or app is not installed
            if (MsiTools.MsiGetProductInfo(guid, MsiWrapper.INSTALLPROPERTY.PRODUCTNAME) == null)
                return;

            FillInMissingInfoMsiHelper(() => entry.RawDisplayName, x => entry.RawDisplayName = x, guid,
                MsiWrapper.INSTALLPROPERTY.INSTALLEDPRODUCTNAME, MsiWrapper.INSTALLPROPERTY.PRODUCTNAME);
            FillInMissingInfoMsiHelper(() => entry.DisplayVersion, x => entry.DisplayVersion = x, guid,
                MsiWrapper.INSTALLPROPERTY.VERSIONSTRING, MsiWrapper.INSTALLPROPERTY.VERSION);
            FillInMissingInfoMsiHelper(() => entry.Publisher, x => entry.Publisher = x, guid,
                MsiWrapper.INSTALLPROPERTY.PUBLISHER);
            FillInMissingInfoMsiHelper(() => entry.InstallLocation, x => entry.InstallLocation = x, guid,
                MsiWrapper.INSTALLPROPERTY.INSTALLLOCATION);
            FillInMissingInfoMsiHelper(() => entry.InstallSource, x => entry.InstallSource = x, guid,
                MsiWrapper.INSTALLPROPERTY.INSTALLSOURCE);
            FillInMissingInfoMsiHelper(() => entry.DisplayIcon, x => entry.DisplayIcon = x, guid,
                MsiWrapper.INSTALLPROPERTY.PRODUCTICON);
            FillInMissingInfoMsiHelper(() => entry.AboutUrl, x => entry.AboutUrl = x, guid,
                MsiWrapper.INSTALLPROPERTY.HELPLINK, MsiWrapper.INSTALLPROPERTY.URLUPDATEINFO,
                MsiWrapper.INSTALLPROPERTY.URLINFOABOUT);

            if (!entry.InstallDate.IsDefault()) return;
            var temp = MsiTools.MsiGetProductInfo(guid, MsiWrapper.INSTALLPROPERTY.INSTALLDATE);
            if (!temp.IsNotEmpty()) return;
            try
            {
                entry.InstallDate = new DateTime(int.Parse(temp.Substring(0, 4)),
                    int.Parse(temp.Substring(4, 2)),
                    int.Parse(temp.Substring(6, 2)));
            }
            catch
            {
                // Date had invalid format, default to nothing
            }
        }

        private static void FillInMissingInfoMsiHelper(Func<string> get, Action<string> set, Guid guid,
            params MsiWrapper.INSTALLPROPERTY[] properties)
        {
            if (string.IsNullOrEmpty(get()))
            {
                foreach (var item in properties)
                {
                    var temp = MsiTools.MsiGetProductInfo(guid, item);

                    //IMPORTANT: Do not assign empty strings, they will mess up automatic property creation later on.
                    if (temp.IsNotEmpty())
                        set(temp);
                }
            }
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
                DisplayVersion =
                    uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameDisplayVersion) as string,
                ParentKeyName = uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameParentKeyName) as string,
                Publisher = uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNamePublisher) as string,
                UninstallString =
                    uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameUninstallString) as string,
                QuietUninstallString =
                    uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameQuietUninstallString) as string,
                ModifyPath = uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameModifyPath) as string,
                InstallLocation =
                    uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameInstallLocation) as string,
                InstallSource = uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameInstallSource) as string,
                SystemComponent =
                    (int)uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameSystemComponent, 0) != 0,
                DisplayIcon = uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameDisplayIcon) as string
            };
        }

        private static FileSize GetEstimatedSize(RegistryKey uninstallerKey)
        {
            var tempSize = (int)uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameEstimatedSize, 0);
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
                catch (FormatException) { }
                catch (ArgumentException) { }
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
                releaseType.ContainsAny(new[] { "Update", "Hotfix" }, StringComparison.OrdinalIgnoreCase))
                return true;

            var defaultValue = uninstallerKey.GetValue(null) as string;
            if (string.IsNullOrEmpty(defaultValue))
                return false;

            //Regex WindowsUpdateRegEx = new Regex(@"KB[0-9]{6}$"); //Doesnt work for all cases
            return defaultValue.Length > 6 && defaultValue.StartsWith("KB")
                   && char.IsNumber(defaultValue[2]) && char.IsNumber(defaultValue.Last());
        }

        private static bool GetIsValid(string uninstallString, string uninstallerFullFilename,
            UninstallerType uninstallerKind, Guid bundleProviderKey)
        {
            if (uninstallString.IsNotEmpty() && !PathPointsToMsiExec(uninstallString))
            {
                if (!Path.IsPathRooted(uninstallerFullFilename) || File.Exists(uninstallerFullFilename))
                    return true;
            }

            return uninstallerKind == UninstallerType.Msiexec && ApplicationUninstallerManager.WindowsInstallerValidGuids.Contains(bundleProviderKey);
        }

        private static bool GetProtectedFlag(RegistryKey uninstallerKey)
        {
            return (int)uninstallerKey.GetValue("NoRemove", 0) != 0;
        }

        private static string GetUninstallerFilename(string uninstallString, UninstallerType type, Guid bundleKey)
        {
            if (!string.IsNullOrEmpty(uninstallString))
            {
                var trimmedUninstallString = uninstallString.Trim('"', ' ');
                if (!PathPointsToMsiExec(trimmedUninstallString))
                {
                    try
                    {
                        if (File.Exists(trimmedUninstallString))
                            return trimmedUninstallString;
                    }
                    catch
                    {
                        /* Ignore io exceptions, access to the file is not necessary */
                    }

                    try
                    {
                        var fileName = ProcessTools.SeparateArgsFromCommand(uninstallString).FileName;

                        Debug.Assert(!fileName.Contains(' ') || File.Exists(fileName));

                        return fileName;
                    }
                    catch (ArgumentException) { }
                    catch (FormatException) { }
                }
            }

            return type == UninstallerType.Msiexec ? MsiTools.MsiGetProductInfo(bundleKey, MsiWrapper.INSTALLPROPERTY.LOCALPACKAGE) : string.Empty;
        }

        private static UninstallerType GetUninstallerType(RegistryKey uninstallerKey)
        {
            // Detect MSI installer based on registry entry (the proper way)
            if ((int)uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameWindowsInstaller, 0) != 0)
            {
                return UninstallerType.Msiexec;
            }

            // Detect InnoSetup
            if (uninstallerKey.GetValueNames().Any(x => x.Contains("Inno Setup:")))
            {
                return UninstallerType.InnoSetup;
            }

            // Detect Steam
            if (uninstallerKey.GetKeyName().StartsWith("Steam App "))
            {
                return UninstallerType.Steam;
            }

            var uninstallString =
                uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameUninstallString) as string;

            return string.IsNullOrEmpty(uninstallString) ? UninstallerType.Unknown : GetUninstallerType(uninstallString);
        }

        private static UninstallerType GetUninstallerType(string uninstallString)
        {
            // Detect MSI installer based on the uninstall string
            //"C:\ProgramData\Package Cache\{33d1fd90-4274-48a1-9bc1-97e33d9c2d6f}\vcredist_x86.exe"  /uninstall
            if (PathPointsToMsiExec(uninstallString) || uninstallString.ContainsAll(
                new[] { @"\Package Cache\{", @"}\", ".exe" }, StringComparison.OrdinalIgnoreCase))
            {
                return UninstallerType.Msiexec;
            }

            // Detect Sdbinst
            if (uninstallString.Contains("sdbinst", StringComparison.OrdinalIgnoreCase)
                && uninstallString.Contains(".sdb", StringComparison.OrdinalIgnoreCase))
            {
                return UninstallerType.SdbInst;
            }

            ProcessStartCommand ps;
            if (ProcessStartCommand.TryParse(uninstallString, out ps) && Path.IsPathRooted(ps.FileName) &&
                File.Exists(ps.FileName))
            {
                try
                {
                    var result = File.ReadAllText(ps.FileName, Encoding.ASCII);

                    // Detect NSIS Nullsoft.NSIS (the most common)
                    if (result.Contains("Nullsoft"))
                        return UninstallerType.Nsis;

                    // Detect InstallShield InstallShield.Setup
                    if (result.Contains("InstallShield"))
                        return UninstallerType.InstallShield;

                    /*// Try to lessen the amount of data that needs to be tested for remaining items (does not work well for InstallShield)
                    var infoStart = result.IndexOf(@"<?xml", StringComparison.OrdinalIgnoreCase);
                    if (infoStart > 0)
                        result = result.Substring(infoStart + 6);*/

                    if (result.Contains("Inno.Setup") || result.Contains("Inno Setup"))
                        return UninstallerType.InnoSetup;

                    /* // Detect Adobe installer
                    if(result.Contains(@"<description>Adobe Systems Incorporated Setup</description>"))
                    {
                        return UninstallerType.AdobeSetup;
                    }*/
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
                catch (SecurityException) { }
            }
            return UninstallerType.Unknown;
        }

        /// <summary>
        ///     Check if path points to the windows installer program or to a .msi package
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool PathPointsToMsiExec(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            return path.ContainsAny(new[] { "msiexec ", "msiexec.exe" }, StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".msi", StringComparison.OrdinalIgnoreCase);
        }

        private static Icon TryGetIconHelper(ApplicationUninstallerEntry entry, out string path)
        {
            try
            {
                // Look for icons with known names in InstallLocation and UninstallerLocation
                var query = from targetDir in new[] { entry.InstallLocation, entry.UninstallerLocation }
                            where !string.IsNullOrEmpty(targetDir) && Directory.Exists(targetDir)
                            from iconName in IconNames
                            let combinedIconPath = Path.Combine(targetDir, iconName)
                            where File.Exists(combinedIconPath)
                            select combinedIconPath;

                foreach (var iconPath in query)
                {
                    try
                    {
                        path = iconPath;
                        return Icon.ExtractAssociatedIcon(iconPath);
                    }
                    catch (ArgumentException) { }
                }
            }
            catch (SecurityException) { }
            catch (UnauthorizedAccessException) { }

            try
            {
                // Try getting icon from the app's executables
                foreach (var executablePath in entry.GetMainExecutableCandidates())
                {
                    var icon = Icon.ExtractAssociatedIcon(executablePath);
                    if (icon == null) continue;
                    path = executablePath;
                    return icon;
                }
            }
            catch (ArgumentException) { }
            catch (SecurityException) { }
            catch (UnauthorizedAccessException) { }

            path = null;
            return null;
        }
    }
}