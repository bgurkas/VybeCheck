namespace VybeCheck.ViewModels;

public class IndexItemViewModel
{
    public int AlbumId { get; set; }

    public string Title { get; set; } = "";

    public string Artist { get; set; } = "";

    public string ArtworkUrl60 { get; set; } = "";

    public int CommentsCount { get; set; }

    public string Username { get; set; } = "";

    public int UserId { get; set; }
    
    public DateTime LastUpdated { get; set; }
    
}