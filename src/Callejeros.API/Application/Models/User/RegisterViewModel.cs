
using System.ComponentModel.DataAnnotations;

namespace Adoption.API.Application.Models.User;

public record RegisterViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; init; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; init; }
    public string Username { get; init; }
    public IFormFile Avatar { get; init; }
    public string Role { get; init; }
}
