/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Collections;
using System.IO;

namespace Klocman.Extensions
{
    public static class DictionaryExtensions
    {

        public static void Serialize(this IDictionary dictionary, TextWriter writer)
        {
            var entries = new List<Entry>(from object key in dictionary.Keys select new Entry(key, dictionary[key]));

            var serializer = new XmlSerializer(typeof(List<Entry>));
            serializer.Serialize(writer, entries);
        }

        public static void Deserialize(this IDictionary dictionary, TextReader reader)
        {
            dictionary.Clear();
            var serializer = new XmlSerializer(typeof(List<Entry>));
            var list = (List<Entry>)serializer.Deserialize(reader);
            if (list == null) throw new ArgumentException(@"Reader didn't contain valid data", nameof(reader));

            foreach (var entry in list)
            {
                dictionary[entry.Key] = entry.Value;
            }
        }

        private class Entry
        {
            public object Key;
            public object Value;

            public Entry()
            {
            }

            public Entry(object key, object value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}
