using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UninstallTools.Uninstaller;

namespace UninstallTools.Lists
{
    /*<summary>
    /*    Structure of the UninstallList file:
    /*    <Filters>
    /*        <Filter FilterText="Test sdsd" ComparisonMethod= Literal />
    /*    </Filters>
    /*</summary>*/

    public class UninstallList : ITestEntry
    {
/*
        private static readonly string XmlComparisonMethodName = "ComparisonMethod";
        private static readonly string XmlElementName = "Filter";
        private static readonly string XmlFilterTextName = "FilterText";
        private static readonly string XmlRootName = "Filters";*/
        //public static readonly string ListExtension = "txt";

        public UninstallList()
        {
        }

        public UninstallList(IEnumerable<Filter> items)
        {
            AddItems(items);
        }

        public List<Filter> Filters { get; set; } = new List<Filter>();

        public static UninstallList ReadFromFile(string fileName)
        {
            var serializer = new XmlSerializer(typeof(UninstallList));
            using (var reader = new XmlTextReader(fileName))
            {
                return serializer.Deserialize(reader) as UninstallList;
            }
        }

        /// <summary>
        ///     Test if the input matches this filter. Returns null if it did not hit any filter.
        ///     If there are only exclude filters this assumes that everything is included.
        /// </summary>
        public bool? TestEntry(ApplicationUninstallerEntry input)
        {
            if (input == null || Filters.Count < 1) return null;

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
                return excluded.HasValue ? (bool?) true : null;
            return included.Value;
        }

        public void AddItems(IEnumerable<string> items)
        {
            Filters.AddRange(items.Select(x => new Filter(x)));
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
            /*
            var rootElement = new XElement(XmlRootName);
            var document = new XDocument(new XDeclaration("1.0", "UTF-16", "yes"), rootElement);

            foreach (var item in _items)
            {
                rootElement.Add(new XElement(XmlElementName,
                    new XAttribute(XmlFilterTextName, item.FilterText),
                    new XAttribute(XmlComparisonMethodName, item.ComparisonMethod.ToString())));
            }

            using (var writer = new XmlTextWriter(fileName, Encoding.Unicode))
            {
                writer.Formatting = Formatting.Indented;
                document.Save(writer);
            }*/
        }

        internal void Remove(Filter item)
        {
            if (Filters.Contains(item))
                Filters.Remove(item);
        }

        /*
    private static IEnumerable<Filter> ParseXml(string file)
    {

        var document = XDocument.Load(file);
        var items = new List<UninstallListItem>();

        if (document.Root == null) return items;

        foreach (var filter in document.Root.Elements())
        {
            var filterText = filter.Attribute(XmlFilterTextName);
            if (string.IsNullOrEmpty(filterText?.Value))
                continue;

            var item = new UninstallListItem(filterText.Value);

            var filterMethod = filter.Attribute(XmlComparisonMethodName);
            if (filterMethod != null)
            {
                item.ComparisonMethod = (FilterComparisonMethod) Enum.Parse(
                    typeof (FilterComparisonMethod), filterMethod.Value);
            }
            else
            {
                item.ComparisonMethod = FilterComparisonMethod.Equals;
            }

            items.Add(item);
        }

        return items;
    }*/
    }
}