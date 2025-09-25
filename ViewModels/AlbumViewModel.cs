using System.ComponentModel.DataAnnotations;

namespace VybeCheck.ViewModels;

public class AlbumViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MinLength(2, ErrorMessage = "Title must have a minimum of 2 characters.")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Artist is required.")]
    [MinLength(2, ErrorMessage = "Artist must have a minimum of 2 characters.")]
    public string Artist { get; set; } = "";

    public string? Description { get; set; } = "";
}