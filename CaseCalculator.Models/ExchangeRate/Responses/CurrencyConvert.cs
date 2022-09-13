using System.Text.Json.Serialization;

namespace CaseCalculator.Models.ExchangeRate.Responses;
public class CurrencyConvert
{
    [JsonPropertyName("result")]
    public double Result { get; set; }
}
