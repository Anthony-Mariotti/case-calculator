using System.Text.Json.Serialization;

namespace CaseCalculator.Models.ExchangeRate;

public class Currency
{
    public int Id { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}
