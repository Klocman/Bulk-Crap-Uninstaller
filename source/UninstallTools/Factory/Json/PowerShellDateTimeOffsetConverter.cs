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
                if (TryParseWindowsPowerShellDate(value, out var windowsPowerShellDate))
                {
                    return windowsPowerShellDate;
                }
                if (value != null && value.StartsWith("/", StringComparison.Ordinal))
                {
                    // Keep malformed /Date(...)/ values on a controlled JsonException path.
                    throw new JsonException();
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
                        if (TryParseWindowsPowerShellDate(value, out var windowsPowerShellDate))
                            return windowsPowerShellDate;

                        throw new JsonException();
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        private static bool TryParseWindowsPowerShellDate(string value, out DateTimeOffset date)
        {
            date = default;

            const string prefix = "/Date(";
            const string suffix = ")/";

            if (string.IsNullOrEmpty(value) ||
                !value.StartsWith(prefix, StringComparison.Ordinal) ||
                !value.EndsWith(suffix, StringComparison.Ordinal))
            {
                return false;
            }

            var timestampText = value.Substring(prefix.Length, value.Length - prefix.Length - suffix.Length);
            if (!long.TryParse(timestampText, out var timestamp))
                return false;

            try
            {
                date = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }
    }
}
