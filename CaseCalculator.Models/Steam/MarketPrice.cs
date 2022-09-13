using System.Text.Json.Serialization;

namespace CaseCalculator.Models.Steam;

public class MarketPrice
{
    [JsonConverter(typeof(MarketPriceJsonConverter))]
    [JsonPropertyName("lowest_price")]
    public double LowestPrice { get; set; }

    [JsonConverter(typeof(MarketPriceJsonConverter))]
    [JsonPropertyName("median_price")]
    public double MedianPrice { get; set; }

    [JsonConverter(typeof(MarketVolumeJsonConverter))]
    [JsonPropertyName("volume")]
    public int Volume { get; set; }
}
