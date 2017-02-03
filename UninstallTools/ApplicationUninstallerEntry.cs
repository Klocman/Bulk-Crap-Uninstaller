/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Localising;
using Klocman.Tools;
using Microsoft.Win32;
using UninstallTools.Factory.InfoAdders;
using UninstallTools.Lists;
using UninstallTools.Properties;
using UninstallTools.Startup;
using UninstallTools.Uninstaller;

namespace UninstallTools
{
    public class ApplicationUninstallerEntry
    {
        public static readonly IEnumerable<string> CompanyNameEndTrimmers =
            new[] {"corp", "corporation", "limited", "inc", "incorporated"};

        public static readonly string RegistryNameBundleProviderKey = "BundleProviderKey";
        public static readonly string RegistryNameComment = "Comment";
        public static readonly string RegistryNameDisplayIcon = "DisplayIcon";
        public static readonly string RegistryNameDisplayName = "DisplayName";
        public static readonly string RegistryNameDisplayVersion = "DisplayVersion";
        public static readonly string RegistryNameEstimatedSize = "EstimatedSize";
        public static readonly string RegistryNameInstallDate = "InstallDate";
        public static readonly string RegistryNameInstallLocation = "InstallLocation";
        public static readonly string RegistryNameInstallSource = "InstallSource";
        public static readonly string RegistryNameModifyPath = "ModifyPath";
        public static readonly string RegistryNameParentKeyName = "ParentKeyName";
        public static readonly string RegistryNamePublisher = "Publisher";
        public static readonly string RegistryNameQuietUninstallString = "QuietUninstallString";

        public static readonly IEnumerable<string> RegistryNamesOfUrlSources = new[]
        {"URLInfoAbout", "URLUpdateInfo", "HelpLink"};

        public static readonly string RegistryNameSystemComponent = "SystemComponent";
        public static readonly string RegistryNameUninstallString = "UninstallString";
        public static readonly string RegistryNameWindowsInstaller = "WindowsInstaller";
        private X509Certificate2 _certificate;
        private bool _certificateGotten;
        private bool? _certificateValid;
        private string[] _mainExecutableCandidates;
        private string _quietUninstallString;
        private string _ratingId;
        private string _uninstallerLocation;
        private string _uninstallString;
        internal Icon IconBitmap = null;
        private string _installLocation;
        private string _installSource;
        private string _modifyPath;

        internal ApplicationUninstallerEntry()
        {
        }
        
        [ComparisonTarget, LocalisedName(typeof (Localisation), "DisplayName")]
        public string DisplayName
        {
            get { return string.IsNullOrEmpty(RawDisplayName) ? RegistryKeyName : RawDisplayName; }
            set { RawDisplayName = value; }
        }

        [XmlIgnore]
        [LocalisedName(typeof (Localisation), "DisplayNameTrimmed")]
        public string DisplayNameTrimmed => StringTools.StripStringFromVersionNumber(DisplayName);

        [XmlIgnore]
        [LocalisedName(typeof (Localisation), "PublisherTrimmed")]
        public string PublisherTrimmed => string.IsNullOrEmpty(Publisher)
            ? string.Empty
            : Publisher.Replace("(R)", string.Empty)
                .ExtendedTrimEndAny(CompanyNameEndTrimmers, StringComparison.CurrentCultureIgnoreCase);

        [XmlIgnore]
        [LocalisedName(typeof (Localisation), "QuietUninstallPossible")]
        public bool QuietUninstallPossible => !string.IsNullOrEmpty(QuietUninstallString) ||
                                              (UninstallerKind == UninstallerType.Msiexec &&
                                               BundleProviderKey != Guid.Empty);

        [XmlIgnore]
        [LocalisedName(typeof (Localisation), "UninstallPossible")]
        public bool UninstallPossible => !string.IsNullOrEmpty(UninstallString);

        [ComparisonTarget, LocalisedName(typeof (Localisation), "AboutUrl")]
        public string AboutUrl { get; set; }

        /// <summary>
        ///     Product code used by msiexec. If it wasn't found, returns Guid.Empty.
        /// </summary>
        [ComparisonTarget, LocalisedName(typeof (Localisation), "BundleProviderKey")]
        public Guid BundleProviderKey { get; set; }

        [LocalisedName(typeof (Localisation), "Comment")]
        public string Comment { get; set; }

        [LocalisedName(typeof (Localisation), "DisplayIcon")]
        public string DisplayIcon { get; set; }

        [ComparisonTarget, LocalisedName(typeof (Localisation), "DisplayVersion")]
        public string DisplayVersion { get; set; }

        [LocalisedName(typeof (Localisation), "EstimatedSize")]
        public FileSize EstimatedSize { get; set; }

        [LocalisedName(typeof (Localisation), "InstallDate")]
        public DateTime InstallDate { get; set; }

        [ComparisonTarget, LocalisedName(typeof (Localisation), "InstallLocation")]
        public string InstallLocation
        {
            get { return _installLocation; }
            set { _installLocation = CleanupPath(value); }
        }

        [ComparisonTarget, LocalisedName(typeof (Localisation), "InstallSource")]
        public string InstallSource
        {
            get { return _installSource; }
            set { _installSource = CleanupPath(value); }
        }

        [ComparisonTarget, LocalisedName(typeof (Localisation), "Is64Bit")]
        public MachineType Is64Bit { get; set; }

        /// <summary>
        ///     Protection from uninstalling.
        /// </summary>
        [ComparisonTarget, LocalisedName(typeof (Localisation), "IsProtected")]
        public bool IsProtected { get; set; }

        /// <summary>
        ///     The application's uniunstaller is mentioned in the registry (if it's not normal uninstallers will not see it)
        /// </summary>
        [LocalisedName(typeof (Localisation), "IsRegistered")]
        public bool IsRegistered { get; set; }

        /// <summary>
        ///     The application is present on the drive, but not in any of the application listings
        /// </summary>
        [ComparisonTarget, LocalisedName(typeof(Localisation), "IsOrphaned")]
        public bool IsOrphaned { get; set; }

        /// <summary>
        ///     True if this is an update for another product
        /// </summary>
        [ComparisonTarget, LocalisedName(typeof (Localisation), "IsUpdate")]
        public bool IsUpdate { get; set; }

        /// <summary>
        ///     True if the application can be uninstalled. False if the uninstaller is missing or is otherwise invalid.
        /// </summary>
        [ComparisonTarget, LocalisedName(typeof (Localisation), "IsValid")]
        public bool IsValid { get; set; }

        [ComparisonTarget, LocalisedName(typeof (Localisation), "ModifyPath")]
        public string ModifyPath
        {
            get { return _modifyPath; }
            set { _modifyPath = CleanupPath(value); }
        }

        [LocalisedName(typeof (Localisation), "ParentKeyName")]
        public string ParentKeyName { get; set; }

        [ComparisonTarget, LocalisedName(typeof (Localisation), "Publisher")]
        public string Publisher { get; set; }

        [ComparisonTarget, LocalisedName(typeof (Localisation), "QuietUninstallString")]
        public string QuietUninstallString
        {
            get
            {
                if (string.IsNullOrEmpty(_quietUninstallString) && UninstallerKind == UninstallerType.Msiexec)
                {
                    _quietUninstallString = UninstallManager.GetMsiString(BundleProviderKey,
                        MsiUninstallModes.QuietUninstall);
                }
                return _quietUninstallString;
            }
            set { _quietUninstallString = value; }
        }

        public string RatingId
        {
            get { return string.IsNullOrEmpty(_ratingId) ? RegistryKeyName : _ratingId; }
            set { _ratingId = value; }
        }

        [ComparisonTarget, LocalisedName(typeof (Localisation), "RegistryKeyName")]
        public string RegistryKeyName { get; set; }

        /// <summary>
        ///     Full registry path of this entry
        /// </summary>
        [LocalisedName(typeof (Localisation), "RegistryPath")]
        public string RegistryPath { get; set; }

        [XmlIgnore]
        [LocalisedName(typeof (Localisation), "StartupEntries")]
        public IEnumerable<StartupEntryBase> StartupEntries { get; set; }

        [ComparisonTarget, LocalisedName(typeof (Localisation), "SystemComponent")]
        public bool SystemComponent { get; set; }

        [ComparisonTarget, LocalisedName(typeof (Localisation), "UninstallerFullFilename")]
        public string UninstallerFullFilename { get; set; }
        
        [ComparisonTarget, LocalisedName(typeof (Localisation), "UninstallerKind")]
        public UninstallerType UninstallerKind { get; set; }

        //[LocalisedName(typeof(Localisation), "IsInstalled")]
        //public bool IsInstalled { get; internal set; }

        [XmlIgnore]
        [LocalisedName(typeof (Localisation), "UninstallerLocation")]
        public string UninstallerLocation
        {
            get
            {
                if (_uninstallerLocation == null)
                {
                    //TODO move to infoadder?
                    _uninstallerLocation = string.Empty;
                    if (!string.IsNullOrEmpty(UninstallerFullFilename))
                    {
                        try
                        {
                            _uninstallerLocation =
                                Path.GetDirectoryName(UninstallerFullFilename);
                        }
                        catch (ArgumentException)
                        {
                        }
                        catch (PathTooLongException)
                        {
                        }
                    }
                }
                return _uninstallerLocation;
            }
        }

        [ComparisonTarget, LocalisedName(typeof (Localisation), "UninstallString")]
        public string UninstallString
        {
            get
            {
                if (string.IsNullOrEmpty(_uninstallString) && UninstallerKind == UninstallerType.Msiexec)
                {
                    //TODO move to infoadder?
                    _uninstallString = UninstallManager.GetMsiString(BundleProviderKey,
                        MsiUninstallModes.Uninstall);
                }
                return _uninstallString;
            }
            set { _uninstallString = value; }
        }

        internal string RawDisplayName { get; set; }

        /// <summary>
        ///     Check if the install location is not empty and is not a system directory
        /// </summary>
        public bool IsInstallLocationValid()
            => !string.IsNullOrEmpty(InstallLocation?.Trim()) &&
               !UninstallToolsGlobalConfig.AllProgramFiles.Any(x => PathTools.PathsEqual(x, InstallLocation));

        public static string GetFuzzyDirectory(string fullCommand)
        {
            if (string.IsNullOrEmpty(fullCommand)) return Localisation.Error_InvalidPath;

            if (fullCommand.StartsWith("msiexec", StringComparison.OrdinalIgnoreCase)
                || fullCommand.Contains("msiexec.exe", StringComparison.OrdinalIgnoreCase))
                return "MsiExec";

            try
            {
                if (fullCommand.Contains('\\'))
                {
                    string strOut;
                    try
                    {
                        strOut = ProcessTools.SeparateArgsFromCommand(fullCommand).FileName;
                    }
                    catch
                    {
                        strOut = fullCommand;
                    }

                    strOut = Path.GetDirectoryName(strOut);

                    strOut = PathTools.GetPathUpToLevel(strOut, 1, false);
                    if (strOut.IsNotEmpty())
                    {
                        return PathTools.PathToNormalCase(strOut); //Path.GetFullPath(strOut);
                    }
                }
            }
            catch
            {
                // Assume path is invalid
            }
            return Localisation.Error_InvalidPath;
        }

        /// <summary>
        ///     Get the certificate associated to the uninstaller or application.
        /// </summary>
        /// <param name="onlyStored">If true only return the stored value, otherwise generate it if needed.</param>
        public X509Certificate2 GetCertificate(bool onlyStored)
        {
            return onlyStored ? _certificate : GetCertificate();
        }

        /// <summary>
        ///     Get the certificate associated to the uninstaller or application.
        /// </summary>
        public X509Certificate2 GetCertificate()
        {
            if (!_certificateGotten)
            {
                _certificateGotten = true;
                _certificate = CertificateGetter.TryGetCertificate(this);

                if (_certificate != null)
                    _certificateValid = _certificate.Verify();
            }
            return _certificate;
        }

        public Icon GetIcon()
        {
            return IconBitmap;
        }

        /// <summary>
        ///     Get ordered collection of filenames that could be the main executable of the application.
        ///     The most likely files are first, the least likely are last.
        /// TODO merge with DirectoryFactory version
        /// </summary>
        public string[] GetMainExecutableCandidates()
        {
            if (_mainExecutableCandidates == null)
            {
                _mainExecutableCandidates = new string[] {};

                var trimmedDispName = DisplayNameTrimmed;
                if (string.IsNullOrEmpty(trimmedDispName))
                {
                    trimmedDispName = DisplayName;
                    if (string.IsNullOrEmpty(trimmedDispName))
                        // Impossible to search for the executable without knowing the app name
                        return _mainExecutableCandidates;
                }

                foreach (var targetDir in new[] {InstallLocation, UninstallerLocation}
                    .Where(x => !string.IsNullOrEmpty(x) && Directory.Exists(x)))
                {
                    var files = Directory.GetFiles(targetDir, "*.exe", SearchOption.TopDirectoryOnly);
                    if (files.Length < 40) // Not likely to hit the correct file and would take too long, skip
                    {
                        // Use string similarity algorithm to find out which executable is likely the main application exe
                        var query = from file in files
                            where !file.Equals(UninstallerFullFilename, StringComparison.InvariantCultureIgnoreCase)
                            orderby
                                StringTools.CompareSimilarity(Path.GetFileNameWithoutExtension(file), trimmedDispName)
                                    ascending
                            select file;

                        _mainExecutableCandidates = query.ToArray();
                        if (_mainExecutableCandidates.Length > 0)
                            break;
                    }
                }
            }
            return _mainExecutableCandidates;
        }

        public Uri GetUri()
        {
            var temp = AboutUrl;
            if (!temp.IsNotEmpty()) return null;

            temp = temp.Replace("www.", temp.StartsWith("www.") ? @"http://" : string.Empty);

            try
            {
                return new Uri(temp, UriKind.Absolute);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Check if certificate is valid. It returns null if the certificate is missing or GetCertificate has not
        ///     been ran yet and onlyStored is set to true.
        /// </summary>
        public bool? IsCertificateValid(bool onlyStored)
        {
            if (!onlyStored && !_certificateGotten)
                GetCertificate();

            return _certificateValid;
        }

        /// <summary>
        ///     Opens a new read-only instance of registry key used by this uninstaller. Remember to close it!
        /// </summary>
        public RegistryKey OpenRegKey()
        {
            return RegistryTools.OpenRegistryKey(RegistryPath);
        }

        /// <summary>
        ///     Opens a new instance of registry key used by this uninstaller. Remember to close it!
        /// </summary>
        /// <exception cref="System.Security.SecurityException">
        ///     The user does not have the permissions required to access the registry key in the
        ///     specified mode.
        /// </exception>
        public RegistryKey OpenRegKey(bool writable)
        {
            return RegistryTools.OpenRegistryKey(RegistryPath, writable);
        }

        /// <summary>
        ///     Check if entry has not been uninstalled already (check registry key)
        /// </summary>
        /// <returns></returns>
        public bool RegKeyStillExists()
        {
            if (string.IsNullOrEmpty(RegistryPath))
                return false;
            try
            {
                using (OpenRegKey())
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public string ToLongString()
        {
            var sb = new StringBuilder();
            sb.Append(DisplayName);
            sb.AppendFormat(" | {0}", Publisher);
            sb.AppendFormat(" | {0}", DisplayVersion);
            sb.AppendFormat(" | {0}", DateTime.MinValue.Equals(InstallDate) ? "" : InstallDate.ToShortDateString());
            sb.AppendFormat(" | {0}", EstimatedSize);
            sb.AppendFormat(" | {0}", RegistryPath);
            sb.AppendFormat(" | {0}", UninstallerKind);
            sb.AppendFormat(" | {0}", UninstallString);
            sb.AppendFormat(" | {0}", QuietUninstallString);
            sb.AppendFormat(" | {0}", Comment);

            return sb.ToString();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(DisplayName);
            sb.AppendFormat(" | {0}", Publisher);
            sb.AppendFormat(" | {0}", DisplayVersion);
            sb.AppendFormat(" | {0}", UninstallString);
            sb.AppendFormat(" | {0}", Comment);

            return sb.ToString();
        }

        private static string CleanupPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            path = path.Trim('"', ' ', '\'', '\\', '/'); // Get rid of the quotation marks
            try
            {
                var i = path.LastIndexOf('\\');
                // TODO unnecessary?
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