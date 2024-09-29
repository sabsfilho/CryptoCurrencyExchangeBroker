using System.Text.Json.Serialization;

namespace BitstampLib;
public class BitstampRequestData
{
    [JsonPropertyName("channel")]
    public required string Channel { get; set; }

}
