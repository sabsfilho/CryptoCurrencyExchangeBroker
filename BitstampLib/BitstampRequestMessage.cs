using System.Text.Json.Serialization;

namespace BitstampLib;
public class BitstampRequestMessage
{
    [JsonPropertyName("event")]
    public required string Event { get; set; }
    [JsonPropertyName("data")]
    public required BitstampRequestData Data { get; set; }
}
