using System.Text.Json.Serialization;

namespace BitstampLib;
public class BitstampResponseMessage
{
    [JsonPropertyName("event")]
    public required string Event { get; set; }
    [JsonPropertyName("channel")]
    public string? Channel { get; set; }
    [JsonPropertyName("data")]
    public object? Data { get; set; }

}
