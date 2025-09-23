public class AlbumDetailsViewModel
{
    public int AlbumId { get; set; }

    public string CurrentUser { get; set; } = "";

    public string Title { get; set; } = "";

    public string Artist { get; set; } = "";

    public string Description { get; set; } = "";

    public string UploadedBy { get; set; } = "";

    public DateTime LastUpdated { get; set; }

    public List<Comment> Comments { get; set; } = [];
}