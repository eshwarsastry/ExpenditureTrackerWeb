using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace ExpenditureTrackerWeb.AutoGen.Services
{
    public class JsonDeserializerConverter : JsonConverter<DateTime>
    {
        private readonly string _format = "dd/MM/yyyy";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateString = reader.GetString()!;
            var formats = new[] { "dd/MM/yyyy", "yy.MM.dd", "yyyy-MM-dd", "dd.MM.yy" }; 
            return DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
                ? date
                : throw new FormatException($"Invalid date format: {dateString}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_format));
        }
    }

    public class StringToDoubleConverter : JsonConverter<double>
    {
        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Handle both string and number values in JSON
            return reader.TokenType switch
            {
                JsonTokenType.String => double.Parse(reader.GetString()!, CultureInfo.InvariantCulture),
                JsonTokenType.Number => reader.GetDouble(),
                _ => throw new JsonException("Unexpected token type for BillAmount")
            };
        }

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}

