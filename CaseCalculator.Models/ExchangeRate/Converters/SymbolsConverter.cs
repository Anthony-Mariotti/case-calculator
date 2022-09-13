using System.Text.Json;
using System.Text.Json.Serialization;

namespace CaseCalculator.Models.ExchangeRate.Converters;

public class SymbolsConverter : JsonConverter<List<Currency>>
{
    public override List<Currency> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = new List<Currency>();

        var symbols = JsonDocument.ParseValue(ref reader).RootElement.EnumerateObject();

        int i = 1;
        foreach (var symbol in symbols)
        {
            var currency = symbol.Value.Deserialize<Currency>();
            currency.Id = i;

            result.Add(currency);
            i++;
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, List<Currency> value, JsonSerializerOptions options)
    {
        
    }
}
