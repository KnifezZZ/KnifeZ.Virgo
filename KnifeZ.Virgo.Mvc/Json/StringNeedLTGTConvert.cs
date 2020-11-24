using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KnifeZ.Virgo.Mvc.Json
{
    /// <summary>
    /// StringNeedLTGTConvert
    /// </summary>
    public class StringNeedLTGTConvert : JsonConverter<string>
    {
        public override string Read (ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            try
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    return reader.GetString();
                }
            }
            catch (Exception)
            {
                throw new Exception($"Error converting value {reader.GetString()} to type '{typeToConvert}'");
            }
            throw new Exception($"Unexpected token {reader.TokenType} when parsing string");
        }

        public override void Write (Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value);
            }
        }
    }
}
