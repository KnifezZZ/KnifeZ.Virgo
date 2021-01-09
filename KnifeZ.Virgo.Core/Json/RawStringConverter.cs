using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using NPOI.SS.Formula.Functions;
using KnifeZ.Virgo.Core.Extensions;

namespace KnifeZ.Virgo.Core.Json
{
    public class RawStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            try
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    string rv = reader.GetString();
                    return rv;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {                
                var txt = JsonEncodedText.Encode($"_raw_{value}_raw_", JavaScriptEncoder.UnsafeRelaxedJsonEscaping);
                writer.WriteStringValue(txt);
            }
        }
    }

}
