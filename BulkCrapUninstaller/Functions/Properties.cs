using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Localising;
using UninstallTools.Uninstaller;

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

    internal static class Properties
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

                if (kvp.Value is Guid && ((Guid)kvp.Value).IsEmpty())
                    continue;

                if (kvp.Value is DateTime && ((DateTime)kvp.Value).IsDefault())
                    continue;

                string result;

                if (kvp.Value is bool)
                    result = ((bool)kvp.Value).ToYesNo();
                else if (kvp.Value is Enum)
                    result = ((Enum)kvp.Value).GetLocalisedName();
                else if (kvp.Value is ICollection)
                    result = string.Join(" | ",
                        ((ICollection)kvp.Value).Cast<object>().Select(x => x.ToString()).ToArray());
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

            // Extract required data
            var lq = from property in typeof(X509Certificate2).GetProperties()
                     select new SingleProperty(property.Name, property.GetValue(cert, new object[] { }));
            var list = lq.ToList();

            // Convert the obtained data to a more human readable form
            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i].Value;

                var issName = item as X500DistinguishedName;
                if (issName != null)
                {
                    list[i] = new SingleProperty(list[i].Key, cert.IssuerName.Format(false));
                    continue;
                }
                var oid = item as Oid;
                if (oid != null)
                {
                    list[i] = new SingleProperty(list[i].Key, oid.FriendlyName);
                    continue;
                }
                var exts = item as X509ExtensionCollection;
                if (exts != null)
                {
                    var result = string.Join(", ", exts.Cast<X509Extension>().Select(x => x.Oid.FriendlyName).ToArray());
                    list[i] = new SingleProperty(list[i].Key, result);
                    continue;
                }
                var key = item as PublicKey;
                if (key != null)
                {
                    list[i] = new SingleProperty(list[i].Key, key.Key.SignatureAlgorithm);
                    continue;
                }
                var arr = item as byte[];
                if (arr != null)
                {
                    list[i] = new SingleProperty(list[i].Key, arr.ToHexString());
                }
            }

            // Create and return the table
            var dt = GetCleanDataTable();
            ConvertPropertiesIntoDataTable(list, dt);
            return dt;
        }

        private static DataTable ExtractFileInfo(ApplicationUninstallerEntry tag)
        {
            if (string.IsNullOrEmpty(tag.UninstallerFullFilename))
                throw new InvalidOperationException(Localisable.PropertiesWindow_Table_ErrorMissingUninstaller);

            var fi = new FileInfo(tag.UninstallerFullFilename);

            if (!fi.Exists)
            {
                if (tag.UninstallerKind == UninstallerType.Msiexec)
                    throw new NotSupportedException(Localisable.PropertiesWindow_Table_ErrorMsi);
                throw new IOException(Localisable.PropertiesWindow_Table_ErrorDoesntExist);
            }

            // Basic filesystem information
            var lq = fi.GetAttributes().Where(a=>!a.Key.Equals(nameof(FileInfo.Directory))).Select(x =>
            {
                if (x.Key.Equals(nameof(FileInfo.Length)))
                    return new SingleProperty(x.Key, FileSize.FromBytes((long)x.Value).ToString(true));
                return x;
            });

            // Extra information from resources
            var verInfo = FileVersionInfo.GetVersionInfo(fi.FullName);
            lq = lq.Concat(typeof(FileVersionInfo).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => !p.Name.EndsWith("Part") && !p.Name.Equals(nameof(FileVersionInfo.FileName)))
                .Select(p => new SingleProperty(p.Name, p.GetValue(verInfo, null))));

            // Create and return the table
            var dt = GetCleanDataTable();
            ConvertPropertiesIntoDataTable(lq, dt);
            return dt;
        }

        private static DataTable ExtractOverview(ApplicationUninstallerEntry tag)
        {
            var lq = from property in typeof(ApplicationUninstallerEntry).GetProperties()
                     select new SingleProperty(property.GetLocalisedName(), property.GetValue(tag, new object[] { }));

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
            var dt = new DataTable();
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