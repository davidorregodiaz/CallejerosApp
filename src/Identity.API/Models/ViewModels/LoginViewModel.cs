

using System.ComponentModel.DataAnnotations;

namespace Identity.API.Models.ViewModels;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public required string Email { get; init; }
    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; init; }
    public bool RememberMe { get; set; } = false;
}
