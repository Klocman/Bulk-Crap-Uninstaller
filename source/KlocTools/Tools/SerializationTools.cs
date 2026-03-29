/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Klocman.Tools
{
    public static class SerializationTools
    {
        public static string SanitizeInvalidXmlCharacters(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            StringBuilder output = null;
            var consumedUtf16Chars = 0;

            foreach (var rune in input.EnumerateRunes())
            {
                if (IsValidXmlCharacter(rune))
                {
                    output?.Append(rune.ToString());
                }
                else
                {
                    output ??= new StringBuilder(input.Length);
                    if (consumedUtf16Chars > 0 && output.Length == 0)
                        output.Append(input, 0, consumedUtf16Chars);
                }

                consumedUtf16Chars += rune.Utf16SequenceLength;
            }

            return output?.ToString() ?? input;
        }

        public static T DeserializeFromXml<T>(string fullFilename)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StreamReader(fullFilename))
            {
                var deserialized = serializer.Deserialize(reader.BaseStream);
                return (T)deserialized;
            }
        }

        public static void SerializeToXml<T>(string fullFilename, T input, XmlAttributeOverrides attributeOverrides = null)
        {
            var serializer = new XmlSerializer(typeof(T), attributeOverrides);
            using (var writer = new StreamWriter(fullFilename))
            {
                serializer.Serialize(writer.BaseStream, input);
            }
        }
        
        public static IDictionary<TKey, TValue> DeserializeDictionary<TKey, TValue>(string filename)
        {
            var l = DeserializeFromXml<List<DictSerializeData<TKey, TValue>>>(filename);

            return l.ToDictionary(x => x.Key, x => x.Data);
        }

        public static void SerializeDictionary<TKey, TValue>(IDictionary<TKey, TValue> source, string filename)
        {
            SerializeToXml(filename, source.Select(x => new DictSerializeData<TKey, TValue>(x.Key, x.Value)).ToList());
        }
        
        [Serializable]
        public class DictSerializeData<TKey, TValue>
        {
            public DictSerializeData()
            {
            }

            public DictSerializeData(TKey key, TValue data)
            {
                Key = key;
                Data = data;
            }

            public TValue Data { get; set; }
            public TKey Key { get; set; }
        }

        private static bool IsValidXmlCharacter(Rune value)
        {
            return value.Value == 0x9 ||
                   value.Value == 0xA ||
                   value.Value == 0xD ||
                   (value.Value >= 0x20 && value.Value <= 0xD7FF) ||
                   (value.Value >= 0xE000 && value.Value <= 0xFFFD) ||
                   (value.Value >= 0x10000 && value.Value <= 0x10FFFF);
        }
    }
}
