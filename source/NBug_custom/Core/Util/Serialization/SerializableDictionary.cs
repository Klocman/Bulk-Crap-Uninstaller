// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializableDictionary.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NBug.Core.Util.Serialization
{
    [Serializable]
    [XmlRoot("dictionary")]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SerializableDictionary{TKey,TValue}" /> class.
        ///     This is the default constructor provided for XML serializer.
        /// </summary>
        public SerializableDictionary()
        {
        }

        public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException();
            }

            foreach (var pair in dictionary)
            {
                Add(pair.Key, pair.Value);
            }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            /*if (reader.IsEmptyElement)
            {
                return;
            }*/
            var inner = reader.ReadSubtree();

            var xElement = XElement.Load(inner);
            if (xElement.HasElements)
            {
                foreach (var element in xElement.Elements())
                {
                    Add((TKey) Convert.ChangeType(element.Name.ToString(), typeof (TKey)),
                        (TValue) Convert.ChangeType(element.Value, typeof (TValue)));
                }
            }

            inner.Close();

            if (!reader.IsEmptyElement)
                reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var key in Keys)
            {
                writer.WriteStartElement(key.ToString().Replace(" ", ""));
                // Check to see if we can actually serialize element
                if (this[key].GetType().IsSerializable)
                {
                    // if it's Serializable doesn't mean serialization will succeed (IE. GUID and SQLError types)
                    try
                    {
                        writer.WriteValue(this[key]);
                    }
                    catch (Exception)
                    {
                        // we're not Throwing anything here, otherwise evil thing will happen
                        writer.WriteValue(this[key].ToString());
                    }
                }
                else
                {
                    // If Type has custom implementation of ToString() we'll get something useful here
                    // Otherwise we'll get Type string. (Still better than crashing).
                    writer.WriteValue(this[key].ToString());
                }
                writer.WriteEndElement();
            }
        }
    }
}