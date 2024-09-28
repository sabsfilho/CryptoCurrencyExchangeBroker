using System.Text.Json.Serialization;

namespace CryptoCurrencyExchangeBrokerLib.provider;
public class BitstampData
{
    [JsonPropertyName("channel")]
    public required string Channel { get; set; }

}
