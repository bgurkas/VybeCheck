using System.ComponentModel.DataAnnotations;

namespace VybeCheck.ViewModels;

public class RegisterFormViewModel
{
    [Required(ErrorMessage = "Username is required.")]
    [MinLength(2, ErrorMessage = "Username must be at least 2 characters long.")]
    public string Username { get; set; } = String.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email.")]
    public string Email { get; set; } = String.Empty;

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string Password { get; set; } = String.Empty;
    
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Please confirm your password.")]
    [Compare("Password", ErrorMessage = "Passwords must match.")]
    public string ConfirmPassword { get; set; } = String.Empty;
    
}