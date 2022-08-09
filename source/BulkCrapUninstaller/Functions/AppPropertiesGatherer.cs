/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using BulkCrapUninstaller.Functions.Tools;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Localising;
using UninstallTools;

namespace BulkCrapUninstaller.Functions
{
    using SingleProperty = KeyValuePair<string, object>;

    internal enum InfoType
    {
        Invalid = 0,
        Overview,
        FileInfo,
        Certificate,
        Registry
    }

    internal static class AppPropertiesGatherer
    {
        public static DataTable GetInfo(ApplicationUninstallerEntry entry, InfoType infoType)
        {
            try
            {
                switch (infoType)
                {
                    case InfoType.Overview:
                        return ExtractOverview(entry);

                    case InfoType.FileInfo:
                        return ExtractFileInfo(entry);

                    case InfoType.Registry:
                        return ExtractRegistryInfo(entry);

                    case InfoType.Certificate:
                        return ExtractCertificateInfo(entry);

                    default:
                        throw new InvalidOperationException("Selected tab is invalid or not supported.");
                }
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        private static void ConvertPropertiesIntoDataTable(IEnumerable<SingleProperty> lq, DataTable dt)
        {
            foreach (var kvp in lq.OrderBy(x => x.Key))
            {
                if (kvp.Value == null)
                    continue;

                if (kvp.Value is Guid guid && guid.IsEmpty())
                    continue;

                if (kvp.Value is DateTime time && time.IsDefault())
                    continue;
                
                if ((kvp.Value as Version)?.IsZeroOrNull() ?? false)
                    continue;

                string result;
                
                if (kvp.Value is bool b)
                    result = b.ToYesNo();
                else if (kvp.Value is Enum e)
                    result = e.GetLocalisedName();
                else if (kvp.Value is ICollection c)
                    result = string.Join(" | ", c.Cast<object>().Select(x => x.ToString()).ToArray());
                else
                    result = kvp.Value.ToString();

                if (!string.IsNullOrEmpty(result))
                    dt.Rows.Add(kvp.Key, result);
            }
        }

        private static DataTable ExtractCertificateInfo(ApplicationUninstallerEntry tag)
        {
            var cert = tag.GetCertificate();

            if (cert == null)
                return GetError(Localisable.PropertiesWindow_Table_ErrorNoCertificate);

            var localizedCert = new LocalizedX509Certificate2(cert);

            // Extract required data
            var lq = from property in typeof(LocalizedX509Certificate2).GetProperties()
                     select new SingleProperty(property.GetLocalisedName(), property.GetValue(localizedCert, new object[] { }));

            // Create and return the table
            var dt = GetCleanDataTable();
            ConvertPropertiesIntoDataTable(lq.ToList(), dt);
            return dt;
        }

        private static DataTable ExtractFileInfo(ApplicationUninstallerEntry tag)
        {
            if (string.IsNullOrEmpty(tag.UninstallerFullFilename))
                throw new InvalidOperationException(Localisable.PropertiesWindow_Table_ErrorMissingUninstaller);

            if (!File.Exists(tag.UninstallerFullFilename))
            {
                if (tag.UninstallerKind == UninstallerType.Msiexec)
                    throw new NotSupportedException(Localisable.PropertiesWindow_Table_ErrorMsi);
                throw new IOException(Localisable.PropertiesWindow_Table_ErrorDoesntExist);
            }
            
            var fi = AdvancedFileInfo.FromPath(tag.UninstallerFullFilename);
            
            var lq = from property in typeof(AdvancedFileInfo).GetProperties()
                     select new SingleProperty(property.GetLocalisedName(), property.GetValue(fi, new object[] { }));

            // Create and return the table
            var dt = GetCleanDataTable();
            ConvertPropertiesIntoDataTable(lq, dt);
            return dt;
        }

        private static DataTable ExtractOverview(ApplicationUninstallerEntry tag)
        {
            var lq = from property in typeof(ApplicationUninstallerEntry).GetProperties()
                     select new SingleProperty(property.GetLocalisedName(), property.GetValue(tag, new object[] { }));

            // Create and return the table
            var dt = GetCleanDataTable();
            ConvertPropertiesIntoDataTable(lq, dt);
            return dt;
        }

        private static DataTable ExtractRegistryInfo(ApplicationUninstallerEntry tag)
        {
            if (!tag.IsRegistered)
                throw new InvalidOperationException(Localisable.PropertiesWindow_Table_ErrorMissingRegistry);

            var targetKey = tag.OpenRegKey();
            var dt = GetCleanDataTable();

            var valueNames = targetKey.GetValueNames();
            foreach (var valueName in valueNames)
            {
                dt.Rows.Add(valueName, targetKey.GetValue(valueName));
            }

            targetKey.Close();
            return dt;
        }

        private static DataTable GetCleanDataTable()
        {
            var dt = new DataTable {Locale = CultureInfo.InvariantCulture};
            dt.Columns.Add(Localisable.PropertiesWindow_Table_Name, typeof(string));
            dt.Columns.Add(Localisable.PropertiesWindow_Table_Value, typeof(string));
            return dt;
        }

        private static DataTable GetError(string message)
        {
            return GetMessage(Localisable.PropertiesWindow_Table_Error, message);
        }

        private static DataTable GetMessage(string name, string message)
        {
            var dt = GetCleanDataTable();
            dt.Rows.Add(name, message);
            return dt;
        }
    }
}