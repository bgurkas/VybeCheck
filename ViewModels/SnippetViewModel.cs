using System.ComponentModel.DataAnnotations;

public class SnippetViewModel
{
    public int? AlbumId { get; set; }

    [Required(ErrorMessage = "Please make a valid selection.")]
    public int TrackId { get; set; }

    public string SnippetUrl { get; set; } = "";

    public List<TrackData> Tracklist { get; set; } = [];
}