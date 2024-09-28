using System.Text.Json.Serialization;

namespace CryptoCurrencyExchangeBrokerLib.provider;
public class BitstampMessage
{
    [JsonPropertyName("event")]
    public required string Event { get; set; }
    [JsonPropertyName("data")]
    public required BitstampData Data { get; set; }

}
