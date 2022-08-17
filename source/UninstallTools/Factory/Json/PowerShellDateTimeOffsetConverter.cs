using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UninstallTools.Factory.Json
{
    internal class PowerShellDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var value = reader.GetString();
                // Windows PowerShell: /Date(1640995200000)/
                if (value.StartsWith("/", StringComparison.Ordinal))
                {
                    var timestamp = long.Parse(value.Substring(6, value.Length - 8));
                    return DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
                }
                // PowerShell Core: 2022-01-01T00:00:00.0000000+00:00
                else
                {
                    return DateTimeOffset.Parse(value);
                }
            }
            // Windows PowerShell: nested { value: /Date(1640995200000)/ }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                var clone = reader;
                reader.Skip();
                while (clone.Read() && clone.TokenType != JsonTokenType.EndObject)
                {
                    if (clone.TokenType == JsonTokenType.PropertyName && clone.GetString() == "value")
                    {
                        _ = clone.Read();
                        var value = clone.GetString();
                        var timestamp = long.Parse(value.Substring(6, value.Length - 8));
                        return DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
