/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
