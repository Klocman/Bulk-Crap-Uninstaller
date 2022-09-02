using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UninstallTools.Factory.Json
{
    /// <summary>
    /// Handle JSON string array entry that has one dimension less or more.
    /// </summary>
    internal class DynamicStringArrayConverter : JsonConverter<string[]>
    {
        public override string[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // 0-dimension
            if (reader.TokenType == JsonTokenType.String)
            {
                return new[] { reader.GetString() };
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                List<string> results = new();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    // nested
                    if (reader.TokenType == JsonTokenType.StartArray)
                    {
                        var clone = reader;
                        _ = clone.Read();
                        results.Add(clone.GetString()); // take first value of nested array only               
                        reader.Skip();
                    }
                    // normal
                    else if (reader.TokenType == JsonTokenType.String)
                    {
                        results.Add(reader.GetString());
                    }
                }
                return results.ToArray();
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, string[] value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
