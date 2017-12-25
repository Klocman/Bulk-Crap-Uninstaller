/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using System.Linq;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Resources;
using Klocman.Tools;
using UninstallTools;

namespace BulkCrapUninstaller.Functions.ApplicationList
{
    internal static class ListViewDelegates
    {
        internal static string AspectToStringConverter(object x)
        {
            return x is long ? new FileSize((long)x).ToString() : string.Empty;
        }

        internal static string BoolToYesNoAspectConverter(object rowObject)
        {
            var result = rowObject as bool?;
            return result.ToYesNo();
        }

        internal static object ColumnGuidAspectGetter(object rowObj)
        {
            var entry = rowObj as ApplicationUninstallerEntry;
            if (entry != null)
            {
                var result = entry.BundleProviderKey;
                if (!result.IsEmpty())
                    return $"{result:B}".ToUpperInvariant();
            }
            return string.Empty;
        }

        internal static object ColumnGuidGroupKeyGetter(object rowObj)
        {
            var entry = rowObj as ApplicationUninstallerEntry;
            if (entry != null)
            {
                var result = entry.BundleProviderKey;
                if (result.Equals(Guid.Empty))
                    return Localisable.GuidFound;
            }
            return Localisable.GuidMissing;
        }

        internal static object ColumnInstallLocationGroupKeyGetter(object rowObj)
        {
            var entry = rowObj as ApplicationUninstallerEntry;
            return GetFuzzyDirectory(entry?.InstallLocation);
        }

        internal static object ColumnInstallSourceGroupKeyGetter(object rowObj)
        {
            var entry = rowObj as ApplicationUninstallerEntry;
            return GetFuzzyDirectory(entry?.InstallSource);
        }

        internal static object ColumnPublisherGroupKeyGetter(object rowObj)
        {
            var entry = rowObj as ApplicationUninstallerEntry;
            return string.IsNullOrEmpty(entry?.PublisherTrimmed) ? CommonStrings.Unknown : entry.PublisherTrimmed;
        }

        internal static object ColumnQuietUninstallStringGroupKeyGetter(object rowObj)
        {
            var entry = rowObj as ApplicationUninstallerEntry;
            return GetFuzzyDirectory(entry?.QuietUninstallString);
        }

        internal static object ColumnSizeAspectGetter(object x)
        {
            var applicationUninstallerEntry = x as ApplicationUninstallerEntry;
            if (applicationUninstallerEntry != null)
                return applicationUninstallerEntry.EstimatedSize.GetRawSize();
            return (long)0;
        }

        internal static object ColumnUninstallStringGroupKeyGetter(object rowObj)
        {
            var entry = rowObj as ApplicationUninstallerEntry;
            return GetFuzzyDirectory(entry?.UninstallString);
        }

        /// <exception cref="InvalidOperationException">The source sequence is empty.</exception>
        internal static object GetFirstCharGroupKeyGetter(object rowobject)
        {
            var entry = rowobject as ApplicationUninstallerEntry;
            if (entry?.DisplayName == null)
                return Localisable.Empty;

            var character = entry.DisplayName.StripAccents().FirstOrDefault(x => !char.IsWhiteSpace(x));

            return character.IsDefault() ? Localisable.Empty : char.ToUpperInvariant(character).ToString();
        }

        internal static object DisplayVersionGroupKeyGetter(object rowObject)
        {
            var entry = rowObject as ApplicationUninstallerEntry;
            if (string.IsNullOrEmpty(entry?.DisplayVersion))
                return CommonStrings.Unknown;

            var dotIndex = entry.DisplayVersion.IndexOf('.');
            return dotIndex > 0 ? entry.DisplayVersion.Substring(0, dotIndex) + ".x" : entry.DisplayVersion;
        }

        internal static object ColumnSizeGroupKeyGetter(object rowObject)
        {
            var entry = rowObject as ApplicationUninstallerEntry;
            return entry == null || entry.EstimatedSize == FileSize.Empty
                ? CommonStrings.Unknown
                : "x " + entry.EstimatedSize.GetUnitName();
        }

        /// <summary>
        ///  Convert path to a directory string usable for grouping
        /// </summary>
        private static string GetFuzzyDirectory(string fullCommand)
        {
            if (string.IsNullOrEmpty(fullCommand)) return Localisable.Empty;

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
            return Localisable.Empty;
        }
    }
}