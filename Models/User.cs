using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int Id { get; set; }


    public string Username { get; set; } = String.Empty;

    public string Email { get; set; } = String.Empty;

    public string PasswordHash { get; set; } = String.Empty;

    // One-to-many relationship
    public List<Album> Albums { get; set; } = new List<Album>();
    
    // Many-to-many (comments)
    public List<Comment> Comments { get; set; } = new List<Comment>();
}