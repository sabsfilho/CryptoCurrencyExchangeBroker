using System.Text.Json.Serialization;

namespace BitstampLib;
public class BitstampResponseData
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    [JsonPropertyName("message")]
    public string? Message { get; set; }

}
