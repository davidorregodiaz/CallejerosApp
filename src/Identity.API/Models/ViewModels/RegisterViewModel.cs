
using System.ComponentModel.DataAnnotations;

namespace Identity.API.Models.ViewModels;

public record RegisterViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; init; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; init; }
    public string Username { get; init; }
}
