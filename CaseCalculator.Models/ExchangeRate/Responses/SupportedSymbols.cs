using CaseCalculator.Models.ExchangeRate.Converters;
using System.Text.Json.Serialization;

namespace CaseCalculator.Models.ExchangeRate.Responses;
public class SupportedSymbols
{
    [JsonConverter(typeof(SymbolsConverter))]
    [JsonPropertyName("symbols")]
    public List<Currency> Symbols { get; set; }
}
