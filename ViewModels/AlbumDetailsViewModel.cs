public class AlbumDetailsViewModel
{
    public int AlbumId { get; set; }

    public string CurrentUser { get; set; } = "";

    public string Title { get; set; } = "";

    public string Artist { get; set; } = "";

    public string Description { get; set; } = "";

    public string Genre { get; set; } = "";

    public DateOnly ReleaseDate { get; set; }

    public int CollectionId { get; set; }

    public string ArtworkUrl100 { get; set; } = "";

    public string SnippetUrl { get; set; } = "";

    public string SnippetName { get; set; } = "";

    public string UploadedBy { get; set; } = "";

    public int UploaderId { get; set; }

    public DateTime LastUpdated { get; set; }

    public List<Comment> Comments { get; set; } = [];
}