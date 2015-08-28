using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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

        public static IEnumerable<ApplicationUninstallerEntry> TryCreateFromDirectory(DirectoryInfo directory,
            bool is64Bit)
        {
            if (directory == null)
                throw new ArgumentNullException(nameof(directory));

            var results = new List<ApplicationUninstallerEntry>();

            CreateFromDirectoryHelper(results, directory, 0);

            foreach (var tempEntry in results)
            {
                tempEntry.Is64Bit = is64Bit;

                tempEntry.UninstallerKind = UninstallerType.Unknown;
                tempEntry.IsRegistered = false;
            }

            return results;
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

            tempEntry.Is64Bit = is64Bit;
            tempEntry.IsUpdate = GetIsUpdate(uninstallerKey);

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
            FillInMissingInfo(tempEntry, tempEntry.UninstallerKind, tempEntry.BundleProviderKey);

            // Finish up setting file/folder paths
            if (tempEntry.InstallLocation != null)
                tempEntry.InstallLocation = CleanupPath(tempEntry.InstallLocation);
            if (tempEntry.InstallSource != null)
                tempEntry.InstallSource = CleanupPath(tempEntry.InstallSource);

            // Use quiet uninstall string as normal uninstall string if the normal string is missing.
            if (!tempEntry.UninstallPossible && tempEntry.QuietUninstallPossible)
                tempEntry.UninstallString = tempEntry.QuietUninstallString;

            tempEntry.UninstallerFullFilename = GetUninstallerFilename(tempEntry.UninstallString,
                tempEntry.UninstallerKind, tempEntry.BundleProviderKey);

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

            string iconPath;
            tempEntry.IconBitmap = TryGetIcon(tempEntry, out iconPath);
            tempEntry.DisplayIcon = iconPath;

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
                    if (!string.IsNullOrEmpty(entry.InstallLocation))
                        result = TryExtractCertificateHelper(entry.GetMainExecutableCandidates());

                    // If no certs were found check the MSI store
                    if (result == null)
                        result = MsiTools.GetCertificate(entry.BundleProviderKey);
                }
                else
                {
                    // If no certs were found check the uninstaller
                    result = TryExtractCertificateHelper(entry.GetMainExecutableCandidates()) ??
                             new X509Certificate2(entry.UninstallerFullFilename);
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
                    var icon = Icon.ExtractAssociatedIcon(resultFilename);
                    if (icon != null)
                    {
                        path = resultFilename;
                        return icon;
                    }
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
            if (!string.IsNullOrEmpty(entry.InstallLocation))
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
                var icon = Icon.ExtractAssociatedIcon(entry.UninstallerFullFilename);
                if (icon != null)
                {
                    path = entry.UninstallerFullFilename;
                    return icon;
                }
            }

            // Finally try finding other executables in the uninstaller's dir. 
            // Check the InstallLocation for null again to prevent TryGetIconHelper from running twice
            if (string.IsNullOrEmpty(entry.InstallLocation))
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
                if (i > 0 && path.Substring(i).Contains('.'))
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

        private static void CreateFromDirectoryHelper(List<ApplicationUninstallerEntry> results, DirectoryInfo directory,
            int level)
        {
            if (level >= 2)
                return;

            // Get executables from this directory
            var files = new List<string>(Directory.GetFiles(directory.FullName, "*.exe", SearchOption.TopDirectoryOnly));
            var dirs = directory.GetDirectories();

            // Check for the bin directory and add files from it to the scan
            var binDirs =
                dirs.Where(x => x.Name.StartsWithAny(BinaryDirectoryNames, StringComparison.OrdinalIgnoreCase)).ToList();

            // Potentially dangerous to process this directory.
            if (files.Count > 40
                // Subdirs with names 3 and less are probably program's files.
                || (level > 0 && directory.Name.Length < 4)
                // This matches ISO language codes, much faster than a more specific compare
                || (directory.Name.Length == 5 && directory.Name[2].Equals('-')))
                return;

            if (files.Count == 0 && !binDirs.Any())
            {
                //if ()
                {
                    foreach (var dir in dirs)
                    {
                        CreateFromDirectoryHelper(results, dir, level + 1);
                    }
                }
                //else return;
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
                    var attribs =
                        compareBestMatchFile.GetExtendedAttributes().Where(x => !string.IsNullOrEmpty(x.Value)).ToList();
                    entry.Publisher = GetAttribMatch(attribs, new[] {"Company", "Publisher"}) ?? entry.Publisher;
                    entry.RawDisplayName =
                        GetAttribMatch(attribs, new[] {"Product name", "Friendly name", "Program Name"}) ??
                        entry.RawDisplayName;
                    entry.DisplayVersion = GetAttribMatch(attribs, new[] {"Product version", "File version"});
                }
                catch
                {
                    // Something was not supported by the OS, oh well
                    // TODO: Either change the method of getting attribs or indicate that it is a problem
                }

                // Attempt to find an uninstaller application
                var uninstallerFilters = new[] {"unins0", "uninstall", "uninst", "uninstaller"};
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
        ///     Properties type and guid are REQUIRED. They don't have to be set on the entry, but should be.
        ///     IMPORTANT: Run at the very end of the object creation!
        /// </summary>
        private static void FillInMissingInfo(ApplicationUninstallerEntry entry, UninstallerType type, Guid guid)
        {
            if (type == UninstallerType.Msiexec)
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

        private static string GetAttribMatch(IEnumerable<KeyValuePair<string, string>> attribs, string[] keywords)
        {
            var attribList = attribs as IList<KeyValuePair<string, string>> ?? attribs.ToList();

            return (from filter in keywords
                select attribList.FirstOrDefault(x => x.Key.Equals(filter, StringComparison.OrdinalIgnoreCase))
                into match
                where !match.IsDefault()
                select match.Value).FirstOrDefault();
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
                    (int) uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameSystemComponent, 0) != 0,
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

            if (uninstallerKind == UninstallerType.Msiexec)
            {
                return ApplicationUninstallerManager.WindowsInstallerValidGuids.Contains(bundleProviderKey);
            }

            return false;
        }

        private static bool GetProtectedFlag(RegistryKey uninstallerKey)
        {
            return ((int) uninstallerKey.GetValue("NoRemove", 0) != 0);
        }

        private static string GetUninstallerFilename(string uninstallString, UninstallerType type, Guid bundleKey)
        {
            if (!string.IsNullOrEmpty(uninstallString))
            {
                if (!PathPointsToMsiExec(uninstallString))
                {
                    try
                    {
                        if (File.Exists(uninstallString))
                            return uninstallString;
                    }
                    catch
                    {
                        /* Ignore io exceptions, access to the file is not necessary */
                    }

                    try
                    {
                        return ProcessTools.SeparateArgsFromCommand(uninstallString).FileName;
                    }
                    catch (ArgumentException)
                    {
                    }
                    catch (FormatException)
                    {
                    }
                }
            }

            if (type == UninstallerType.Msiexec)
            {
                return MsiTools.MsiGetProductInfo(bundleKey, MsiWrapper.INSTALLPROPERTY.LOCALPACKAGE);
            }

            return string.Empty;
        }

        private static UninstallerType GetUninstallerType(RegistryKey uninstallerKey)
        {
            // Detect MSI installer based on registry entry (the proper way)
            if ((int) uninstallerKey.GetValue(ApplicationUninstallerEntry.RegistryNameWindowsInstaller, 0) != 0)
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

            if (uninstallString.IsNotEmpty())
            {
                // Detect MSI installer based on the uninstall string
                //"C:\ProgramData\Package Cache\{33d1fd90-4274-48a1-9bc1-97e33d9c2d6f}\vcredist_x86.exe"  /uninstall
                if (PathPointsToMsiExec(uninstallString) || uninstallString.ContainsAll(
                    new[] {@"\Package Cache\{", @"}\", ".exe"}, StringComparison.OrdinalIgnoreCase))
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
                if (ProcessStartCommand.TryParse(uninstallString, out ps) &&
                    (Path.IsPathRooted(ps.FileName) && File.Exists(ps.FileName)))
                {
                    try
                    {
                        var result = File.ReadAllText(ps.FileName, Encoding.ASCII);

                        // Detect NSIS Nullsoft.NSIS
                        if (result.Contains("Nullsoft"))
                        {
                            return UninstallerType.Nsis;
                        }

                        // Detect InstallShield InstallShield.Setup
                        if (result.Contains("InstallShield"))
                        {
                            return UninstallerType.InstallShield;
                        }
                        /*
                        // Detect Adobe installer
                        if(result.Contains(@"<description>Adobe Systems Incorporated Setup</description>"))
                        {
                            return UninstallerType.AdobeSetup;
                        }*/
                    }
                    catch (IOException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                    catch (SecurityException)
                    {
                    }
                }
            }

            // Unknown type
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
            if (path.ContainsAny(new[] {"msiexec ", "msiexec.exe"}, StringComparison.OrdinalIgnoreCase))
                return true;

            return path.EndsWith(".msi", StringComparison.OrdinalIgnoreCase);
        }

        private static Icon TryGetIconHelper(ApplicationUninstallerEntry entry, out string path)
        {
            try
            {
                // Look for icons with known names in InstallLocation and UninstallerLocation
                var query = from targetDir in new[] {entry.InstallLocation, entry.UninstallerLocation}
                    where !string.IsNullOrEmpty(targetDir) && Directory.Exists(targetDir)
                    from iconName in IconNames
                    let combinedIconPath = Path.Combine(targetDir, iconName)
                    where File.Exists(combinedIconPath)
                    select combinedIconPath;

                var iconPath = query.FirstOrDefault();
                if (iconPath != null)
                {
                    path = iconPath;
                    return Icon.ExtractAssociatedIcon(iconPath);
                }
            }
            catch (SecurityException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }

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
            catch (SecurityException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }

            path = null;
            return null;
        }
    }
}