/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Linq;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Resources;
using UninstallTools.Uninstaller;

namespace BulkCrapUninstaller.Functions
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
                    return $"{result:B}".ToUpper();
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
            return ApplicationUninstallerEntry.GetFuzzyDirectory(entry?.InstallLocation);
        }

        internal static object ColumnInstallSourceGroupKeyGetter(object rowObj)
        {
            var entry = rowObj as ApplicationUninstallerEntry;
            return ApplicationUninstallerEntry.GetFuzzyDirectory(entry?.InstallSource);
        }

        internal static object ColumnPublisherGroupKeyGetter(object rowObj)
        {
            var entry = rowObj as ApplicationUninstallerEntry;
            return string.IsNullOrEmpty(entry?.PublisherTrimmed) ? CommonStrings.Unknown : entry.PublisherTrimmed;
        }

        internal static object ColumnQuietUninstallStringGroupKeyGetter(object rowObj)
        {
            var entry = rowObj as ApplicationUninstallerEntry;
            return ApplicationUninstallerEntry.GetFuzzyDirectory(entry?.QuietUninstallString);
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
            return ApplicationUninstallerEntry.GetFuzzyDirectory(entry?.UninstallString);
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
    }
}