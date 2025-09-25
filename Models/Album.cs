using System.ComponentModel.DataAnnotations;

public class Album
{
    [Key]
    public int Id { get; set; }

    public string Title { get; set; } = String.Empty;

    public string Artist { get; set; } = String.Empty;

    public string? Description { get; set; }

    public string Genre { get; set; } = "";

    public DateTime ReleaseDate { get; set; }

    public int CollectionId { get; set; }

    public string ArtworkUrl60 { get; set; } = "";

    public string ArtworkUrl100 { get; set; } = "";

    public string SnippetUrl { get; set; } = "";

    public string SnippetName { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Many-side
    public int UserId { get; set; }

    public User? User { get; set; }

    public List<Comment> Comments { get; set; } = new List<Comment>();
}