/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using Klocman.Tools;

namespace UninstallTools
{
    public sealed class ApplicationEntrySerializer
    {
        private static readonly PropertyInfo[] SanitizableProperties = typeof(ApplicationUninstallerEntry)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => x.CanRead && x.CanWrite && x.GetIndexParameters().Length == 0 &&
                        (x.PropertyType == typeof(string) || x.PropertyType == typeof(string[])))
            .ToArray();
        private static readonly MethodInfo MemberwiseCloneMethod = typeof(object)
            .GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo[] SanitizableFields = typeof(ApplicationUninstallerEntry)
            .GetFields(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => x.FieldType == typeof(string) || x.FieldType == typeof(string[]))
            .ToArray();

        public static void SerializeApplicationEntries(string filename, IEnumerable<ApplicationUninstallerEntry> items)
        {
            var sanitizedItems = items?
                .Select(CloneAndSanitizeForXmlExport)
                .ToList() ?? new List<ApplicationUninstallerEntry>();

            SerializationTools.SerializeToXml(filename, new ApplicationEntrySerializer(sanitizedItems));
        }

        public static string SerializeApplicationEntriesToXml(IEnumerable<ApplicationUninstallerEntry> items)
        {
            var sanitizedItems = items?
                .Select(CloneAndSanitizeForXmlExport)
                .ToList() ?? new List<ApplicationUninstallerEntry>();

            var serializer = new XmlSerializer(typeof(ApplicationEntrySerializer));
            using (var writer = new Utf8StringWriter())
            {
                serializer.Serialize(writer, new ApplicationEntrySerializer(sanitizedItems));
                return writer.ToString();
            }
        }

        private sealed class Utf8StringWriter : StringWriter
        {
            public Utf8StringWriter() : base(CultureInfo.InvariantCulture)
            {
            }

            public override Encoding Encoding => new UTF8Encoding(false);
        }

        public static void SerializeApplicationEntriesToJson(string filename, IEnumerable<ApplicationUninstallerEntry> items)
        {
            File.WriteAllText(filename, SerializeApplicationEntriesToJson(items), new UTF8Encoding(false));
        }

        public static string SerializeApplicationEntriesToJson(IEnumerable<ApplicationUninstallerEntry> items)
        {
            var entries = (items ?? Enumerable.Empty<ApplicationUninstallerEntry>()).Select(x => new
            {
                x.DisplayName,
                x.DisplayVersion,
                x.Publisher,
                x.Comment,
                x.AboutUrl,
                x.InstallLocation,
                x.InstallSource,
                InstallDate = x.InstallDate == default ? null : x.InstallDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                EstimatedSizeKb = x.EstimatedSize.GetKbSize(),
                x.UninstallString,
                x.QuietUninstallString,
                UninstallerKind = x.UninstallerKind.ToString(),
                x.UninstallerLocation,
                Is64Bit = x.Is64Bit.ToString(),
                x.IsProtected,
                x.IsRegistered,
                x.IsOrphaned,
                x.IsUpdate,
                x.IsValid,
                x.IsWebBrowser,
                x.SystemComponent,
                x.RegistryKeyName,
                x.RegistryPath,
                x.ParentKeyName,
                BundleProviderKey = x.BundleProviderKey == Guid.Empty ? null : x.BundleProviderKey.ToString(),
                x.QuietUninstallPossible,
                x.UninstallPossible,
            });
            return JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
        }

        public ApplicationEntrySerializer(IEnumerable<ApplicationUninstallerEntry> items)
        {
            Items = items.ToList();
        }

        // Needed for serialization
        public ApplicationEntrySerializer()
        {
        }

        public List<ApplicationUninstallerEntry> Items { get; set; }

        private static ApplicationUninstallerEntry CloneAndSanitizeForXmlExport(ApplicationUninstallerEntry item)
        {
            if (item == null)
                return null;

            var clone = (ApplicationUninstallerEntry)MemberwiseCloneMethod.Invoke(item, null);
            SanitizeForXmlExport(clone);
            return clone;
        }

        private static void SanitizeForXmlExport(ApplicationUninstallerEntry item)
        {
            foreach (var property in SanitizableProperties)
            {
                if (property.PropertyType == typeof(string))
                {
                    var currentValue = property.GetValue(item) as string;
                    var sanitizedValue = SerializationTools.SanitizeInvalidXmlCharacters(currentValue);
                    if (!string.Equals(currentValue, sanitizedValue, System.StringComparison.Ordinal))
                        property.SetValue(item, sanitizedValue);
                }
                else if (property.PropertyType == typeof(string[]))
                {
                    var currentValue = property.GetValue(item) as string[];
                    if (currentValue == null)
                        continue;

                    var sanitizedValue = currentValue
                        .Select(SerializationTools.SanitizeInvalidXmlCharacters)
                        .ToArray();

                    if (!currentValue.SequenceEqual(sanitizedValue, System.StringComparer.Ordinal))
                        property.SetValue(item, sanitizedValue);
                }
            }

            foreach (var field in SanitizableFields)
            {
                if (field.FieldType == typeof(string))
                {
                    var currentValue = field.GetValue(item) as string;
                    var sanitizedValue = SerializationTools.SanitizeInvalidXmlCharacters(currentValue);
                    if (!string.Equals(currentValue, sanitizedValue, System.StringComparison.Ordinal))
                        field.SetValue(item, sanitizedValue);
                }
                else if (field.FieldType == typeof(string[]))
                {
                    var currentValue = field.GetValue(item) as string[];
                    if (currentValue == null)
                        continue;

                    var sanitizedValue = currentValue
                        .Select(SerializationTools.SanitizeInvalidXmlCharacters)
                        .ToArray();

                    if (!currentValue.SequenceEqual(sanitizedValue, System.StringComparer.Ordinal))
                        field.SetValue(item, sanitizedValue);
                }
            }
        }
    }
}
