using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using KnifeZ.Virgo.Core;

namespace KnifeZ.Virgo.Mvc.Json
{
    //TODO by KnifeZ
    public class DateRangeConvert : JsonConverter<DateRange>
    {
        public override DateRange Read (ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            try
            {
                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    string[] ds = new string[2];
                    reader.Read();
                    ds[0] = reader.GetString();
                    reader.Read();
                    ds[1] = reader.GetString();
                    reader.Read();
                    if (DateRange.TryParse(ds, out var dateRange))
                    {
                        return dateRange;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public override void Write (Utf8JsonWriter writer, DateRange value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value.Value);
            }
        }
    }
}
