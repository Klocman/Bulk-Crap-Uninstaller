﻿/*
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

namespace UninstallTools
{
    public class ApplicationUninstallerEntry
    {
        public static readonly IEnumerable<string> CompanyNameEndTrimmers =
            new[] { "corp", "corporation", "limited", "inc", "incorporated" };

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
        private string _ratingId;
        private string _installLocation;
        private string _installSource;
        private string _modifyPath;
        private string _displayIcon;

        internal ApplicationUninstallerEntry()
        {
        }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), "DisplayName")]
        public string DisplayName
        {
            get { return (string.IsNullOrEmpty(RawDisplayName) ? RegistryKeyName : RawDisplayName) ?? string.Empty; }
            set { RawDisplayName = value; }
        }

        [XmlIgnore]
        [LocalisedName(typeof(Localisation), "DisplayNameTrimmed")]
        public string DisplayNameTrimmed => StringTools.StripStringFromVersionNumber(DisplayName);

        [XmlIgnore]
        [LocalisedName(typeof(Localisation), "PublisherTrimmed")]
        public string PublisherTrimmed => string.IsNullOrEmpty(Publisher)
            ? string.Empty
            : Publisher.Replace("(R)", string.Empty)
                .ExtendedTrimEndAny(CompanyNameEndTrimmers, StringComparison.CurrentCultureIgnoreCase);

        [XmlIgnore]
        [LocalisedName(typeof(Localisation), "QuietUninstallPossible")]
        public bool QuietUninstallPossible => !string.IsNullOrEmpty(QuietUninstallString) ||
                                              (UninstallerKind == UninstallerType.Msiexec &&
                                               BundleProviderKey != Guid.Empty);

        [XmlIgnore]
        [LocalisedName(typeof(Localisation), "UninstallPossible")]
        public bool UninstallPossible => !string.IsNullOrEmpty(UninstallString);

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), "AboutUrl")]
        public string AboutUrl { get; set; }

        /// <summary>
        ///     Product code used by msiexec. If it wasn't found, returns Guid.Empty.
        /// </summary>
        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), "BundleProviderKey")]
        public Guid BundleProviderKey { get; set; }

        [LocalisedName(typeof(Localisation), "Comment")]
        public string Comment { get; set; }

        [LocalisedName(typeof(Localisation), "DisplayIcon")]
        public string DisplayIcon
        {
            get { return _displayIcon; }
            set { _displayIcon = CleanupPath(value, true); }
        }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), "DisplayVersion")]
        public string DisplayVersion { get; set; }

        [LocalisedName(typeof(Localisation), "EstimatedSize")]
        public FileSize EstimatedSize { get; set; }

        [LocalisedName(typeof(Localisation), "InstallDate")]
        public DateTime InstallDate { get; set; }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), "InstallLocation")]
        public string InstallLocation
        {
            get { return _installLocation; }
            set { _installLocation = CleanupPath(value); }
        }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), "InstallSource")]
        public string InstallSource
        {
            get { return _installSource; }
            set { _installSource = CleanupPath(value); }
        }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), "Is64Bit")]
        public MachineType Is64Bit { get; set; }

        /// <summary>
        ///     Protection from uninstalling.
        /// </summary>
        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), "IsProtected")]
        public bool IsProtected { get; set; }

        /// <summary>
        ///     The application's uniunstaller is mentioned in the registry (if it's not normal uninstallers will not see it)
        /// </summary>
        [LocalisedName(typeof(Localisation), "IsRegistered")]
        public bool IsRegistered { get; set; }

        /// <summary>
        ///     The application is present on the drive, but not in any of the application listings
        /// </summary>
        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), "IsOrphaned")]
        public bool IsOrphaned { get; set; }

        /// <summary>
        ///     True if this is an update for another product
        /// </summary>
        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), "IsUpdate")]
        public bool IsUpdate { get; set; }

        /// <summary>
        ///     True if the application can be uninstalled. False if the uninstaller is missing or is otherwise invalid.
        /// </summary>
        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), "IsValid")]
        public bool IsValid { get; set; }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), "ModifyPath")]
        public string ModifyPath
        {
            get { return _modifyPath; }
            set { _modifyPath = CleanupPath(value); }
        }

        [LocalisedName(typeof(Localisation), "ParentKeyName")]
        public string ParentKeyName { get; set; }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), "Publisher")]
        public string Publisher { get; set; }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), "QuietUninstallString")]
        public string QuietUninstallString { get; set; }

        public string RatingId
        {
            get { return string.IsNullOrEmpty(_ratingId) ? RegistryKeyName : _ratingId; }
            set { _ratingId = value; }
        }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), "RegistryKeyName")]
        public string RegistryKeyName { get; set; }

        /// <summary>
        ///     Full registry path of this entry
        /// </summary>
        [LocalisedName(typeof(Localisation), "RegistryPath")]
        public string RegistryPath { get; set; }

        [XmlIgnore]
        [LocalisedName(typeof(Localisation), "StartupEntries")]
        public IEnumerable<StartupEntryBase> StartupEntries { get; set; }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), "SystemComponent")]
        public bool SystemComponent { get; set; }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), "UninstallerFullFilename")]
        public string UninstallerFullFilename { get; set; }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), "UninstallerKind")]
        public UninstallerType UninstallerKind { get; set; }

        //[LocalisedName(typeof(Localisation), "IsInstalled")]
        //public bool IsInstalled { get; internal set; }

        [LocalisedName(typeof(Localisation), "UninstallerLocation")]
        public string UninstallerLocation { get; set; }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), "UninstallString")]
        public string UninstallString { get; set; }

        internal string RawDisplayName { get; set; }

        internal Icon IconBitmap { get; set; }

        /// <summary>
        ///     Check if the install location is not empty and is not a system directory
        /// </summary>
        public bool IsInstallLocationValid()
        {
            if (string.IsNullOrEmpty(InstallLocation?.Trim()))
                return false;
            return !UninstallToolsGlobalConfig.GetAllProgramFiles().Any(x => PathTools.PathsEqual(x, InstallLocation));
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
        ///     Ordered collection of filenames that could be the main executable of the application.
        ///     The most likely files are first, the least likely are last.
        /// </summary>
        internal string[] SortedExecutables { get; set; }

        public IEnumerable<string> GetSortedExecutables()
        {
            if (SortedExecutables == null)
                return Enumerable.Empty<string>();
            var output = SortedExecutables.AsEnumerable();
            if (!string.IsNullOrEmpty(UninstallerFullFilename))
                output = output.OrderBy(x => x.Equals(UninstallerFullFilename, StringComparison.InvariantCultureIgnoreCase));
            return output;
        }

        public Uri GetAboutUri()
        {
            var temp = AboutUrl;
            if (!temp.IsNotEmpty()) return null;

            temp = temp.ToLowerInvariant().Replace("www.",
                temp.StartsWith("www.", StringComparison.InvariantCulture) ? @"http://www." : string.Empty);

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
            return RegistryPath != null ? RegistryTools.OpenRegistryKey(RegistryPath) : null;
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
                using (var key = OpenRegKey())
                    return key != null;
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

        private static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();
        private static string CleanupPath(string path, bool isFilename = false)
        {
            if (string.IsNullOrEmpty(path)) return null;

            if (!isFilename)
            {
                // Try the fast method first for directories
                var trimmed = path.Trim('"', ' ', '\'', '\\', '/');

                if (!trimmed.ContainsAny(InvalidPathChars))
                    return trimmed;
            }

            try
            {
                path = ProcessTools.SeparateArgsFromCommand(path).FileName;
                if (!isFilename && path.Contains('.') && !Directory.Exists(path))
                    return Path.GetDirectoryName(path);
            }
            catch
            {
                // If sanitization failed just leave it be, it will be handled afterwards
            }
            return path.TrimEnd('\\');
        }
    }
}