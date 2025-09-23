using System.ComponentModel.DataAnnotations;

namespace VybeCheck.ViewModels;

public class LoginFormViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email.")]
    public string Email { get; set; } = "";

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = "";
    
    public string? Error { get; set; }
    
}