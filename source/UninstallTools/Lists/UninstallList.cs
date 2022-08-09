/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace UninstallTools.Lists
{
    public class UninstallList : ITestEntry
    {
        public UninstallList()
        {
        }

        public UninstallList(IEnumerable<Filter> items)
        {
            AddItems(items);
        }

        public List<Filter> Filters { get; set; } = new();

        /// <summary>
        ///     Test if the input matches this filter. Returns null if it did not hit any filter.
        ///     If there are only exclude filters this assumes that everything is included.
        /// </summary>
        public bool? TestEntry(ApplicationUninstallerEntry input)
        {
            if (!Enabled || input == null || Filters.Count < 1) return null;

            var excludes = new List<Filter>();
            var includes = new List<Filter>();

            foreach (var uninstallListItem in Filters)
            {
                if (uninstallListItem.Exclude)
                    excludes.Add(uninstallListItem);
                else
                    includes.Add(uninstallListItem);
            }

            bool? excluded = null;

            foreach (var uninstallListItem in excludes)
            {
                if (uninstallListItem.TestEntry(input) == true)
                    return false;
                excluded = false;
            }

            bool? included = null;

            foreach (var uninstallListItem in includes)
            {
                if (uninstallListItem.TestEntry(input) == true)
                {
                    included = true;
                    break;
                }
                included = false;
            }

            if (!included.HasValue)
                return excluded.HasValue ? true : null;
            return included.Value;
        }

        public bool Enabled { get; set; } = true;

        public static UninstallList ReadFromFile(string fileName)
        {
            var serializer = new XmlSerializer(typeof (UninstallList));
            using (var reader = new XmlTextReader(fileName))
            {
                return serializer.Deserialize(reader) as UninstallList;
            }
        }

        public void AddItems(IEnumerable<string> items)
        {
            Filters.AddRange(items.Select(x => new Filter(null, x)));
        }

        public void Add(Filter item)
        {
            Filters.Add(item);
        }

        public void AddItems(IEnumerable<Filter> items)
        {
            Filters.AddRange(items);
        }

        public void SaveToFile(string fileName)
        {
            var serializer = new XmlSerializer(typeof (UninstallList));
            using (var writer = new XmlTextWriter(fileName, Encoding.Unicode))
            {
                writer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, this);
            }
        }

        internal void Remove(Filter item)
        {
            if (Filters.Contains(item))
                Filters.Remove(item);
        }
    }
}