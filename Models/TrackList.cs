using System.Text.Json.Serialization;

public class TrackData
{
    [JsonPropertyName("wrapperType")]
    public string WrapperType { get; set; } = "";

    [JsonPropertyName("trackName")]
    public string TrackName { get; set; } = "";

    [JsonPropertyName("trackId")]
    public int TrackId { get; set; }

    [JsonPropertyName("previewUrl")]
    public string SnippetUrl { get; set; } = "";
}

public class TrackWrapper
{
    [JsonPropertyName("results")]
    public List<TrackData> Results { get; set; } = [];
}