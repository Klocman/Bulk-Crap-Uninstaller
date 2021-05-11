/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Klocman.Extensions
{
    public static class XmlExtensions
    {
        /// <summary>
        ///     Get element with supplied name. If the element doesn't exist crate it.
        /// </summary>
        public static XElement GetOrCreateElement(this XElement baseElement, XName name)
        {
            var element = baseElement.Element(name);
            if (element == null)
            {
                element = new XElement(name);
                baseElement.Add(element);
            }
            return element;
        }

        public static string Serialize(this XmlSerializer serializer, object value, bool indent = false)
        {
            if (value == null) return null;

            var settings = new XmlWriterSettings
            {
                // no BOM in a .NET string
                Encoding = new UnicodeEncoding(false, false),
                Indent = indent,
                OmitXmlDeclaration = true
            };

            using (var textWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, value);
                }
                return textWriter.ToString();
            }
        }

        public static object Deserialize(this XmlSerializer serializer, string xml)
        {
            if (string.IsNullOrEmpty(xml)) return null;

            var settings = new XmlReaderSettings();

            using (var textReader = new StringReader(xml))
            {
                using (var xmlReader = XmlReader.Create(textReader, settings))
                {
                    return serializer.Deserialize(xmlReader);
                }
            }
        }
    }
}