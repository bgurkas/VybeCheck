using System.ComponentModel.DataAnnotations;

public class CommentViewModel
{
    public int AlbumId { get; set; }

    [Required(ErrorMessage = "Blank comments are not allowed.")]
    public string CommentContent { get; set; } = "";
}