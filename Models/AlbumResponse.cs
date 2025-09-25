using System.Text.Json.Serialization;

public class AlbumData
{
    [JsonPropertyName("artistId")]
    public int ArtistId { get; set; }

    [JsonPropertyName("collectionId")]
    public int CollectionId { get; set; }

    [JsonPropertyName("artworkUrl60")]
    public string ArtworkUrl60 { get; set; } = "";

    [JsonPropertyName("artworkUrl100")]
    public string ArtworkUrl100 { get; set; } = "";

    [JsonPropertyName("releaseDate")]
    public DateTime ReleaseDate { get; set; }

    [JsonPropertyName("primaryGenreName")]
    public string Genre { get; set; } = "";
}

public class ResponseWrapper
{
    [JsonPropertyName("results")]
    public List<AlbumData> Results { get; set; } = [];
}