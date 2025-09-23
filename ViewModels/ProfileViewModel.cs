namespace VybeCheck.ViewModels;

public class ProfileViewModel
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int AlbumsPostedCount { get; set; }
    public int CommentsPostedCount { get; set; }
}