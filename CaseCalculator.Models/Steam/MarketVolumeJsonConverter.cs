using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace CaseCalculator.Models.Steam;

internal class MarketVolumeJsonConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        int.Parse(Regex.Replace(reader.GetString(), @",+", ""));

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options) =>
        value.ToString();
}
