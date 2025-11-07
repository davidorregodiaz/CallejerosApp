
namespace Identity.API.Models.ViewModels;

public record TokenViewModel
{
    public string Token { get; init; }
    public int ExpirationMinutes { get; init; }
    public string RefreshToken { get; init; }
}
