
namespace Adoption.API.Application.Models.User;

public record TokenViewModel
{
    public string Token { get; init; }
    public int ExpiresIn { get; init; }
    public string RefreshToken { get; init; }
    public UserViewModel User { get; init; }
}
