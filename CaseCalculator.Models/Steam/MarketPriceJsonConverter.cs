using System.Text.Json;
using System.Text.Json.Serialization;

namespace CaseCalculator.Models.Steam;

internal class MarketPriceJsonConverter : JsonConverter<double>
{
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        double.Parse(reader.GetString().Remove(0, 1));

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString("N2"));
}
