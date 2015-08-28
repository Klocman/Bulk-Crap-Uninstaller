using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Klocman.Extensions;

namespace UninstallTools.Lists
{
    /*<summary>
    /*    Structure of the UninstallList file:
    /*    <Filters>
    /*        <Filter FilterText="Test sdsd" ComparisonMethod= Literal />
    /*    </Filters>
    /*</summary>*/

    public class UninstallList
    {
        private static readonly string XmlComparisonMethodName = "ComparisonMethod";
        private static readonly string XmlElementName = "Filter";
        private static readonly string XmlFilterTextName = "FilterText";
        private static readonly string XmlRootName = "Filters";
        private List<UninstallListItem> _items = new List<UninstallListItem>();
        //public static readonly string ListExtension = "txt";
        public UninstallList()
        {
        }

        public UninstallList(IEnumerable<UninstallListItem> items)
        {
            AddItems(items);
            CleanUp();
        }

        public IEnumerable<UninstallListItem> Items => _items;

        public static UninstallList FromFiles(params string[] files)
        {
            var result = new UninstallList();

            foreach (var file in files.Where(File.Exists))
            {
                result._items.AddRange(Path.GetExtension(file)
                    .Equals(".xml", StringComparison.InvariantCultureIgnoreCase)
                    ? ParseXml(file)
                    : File.ReadAllLines(file).Select(x => new UninstallListItem(x)));
            }

            result.CleanUp();
            return result;
        }

        public void AddItems(IEnumerable<string> items)
        {
            _items.AddRange(items.Select(x => new UninstallListItem(x)));
            CleanUp();
        }

        public void AddItems(IEnumerable<UninstallListItem> items)
        {
            _items.AddRange(items);
            CleanUp();
        }

        public void SaveToFile(string fileName)
        {
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
            }
        }

        internal void Remove(UninstallListItem item)
        {
            if (_items.Contains(item))
                _items.Remove(item);
        }

        private static IEnumerable<UninstallListItem> ParseXml(string file)
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
        }

        private void CleanUp()
        {
            _items = _items.DistinctBy(x => x.FilterText).OrderBy(x => x.FilterText).ToList();
        }
    }
}