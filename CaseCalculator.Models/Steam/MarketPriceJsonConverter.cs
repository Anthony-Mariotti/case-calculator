using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CaseCalculator.Models.Steam;

internal class MarketPriceJsonConverter : JsonConverter<double>
{
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        double.Parse(reader.GetString(), NumberStyles.Currency, CultureInfo.CreateSpecificCulture("en-US"));

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString("C", CultureInfo.CreateSpecificCulture("en-US")));
}
