
using Adoption.API.Application.Models.User;

namespace Adoption.API.Application.Models;

public class TokenViewModel
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
    public UserViewModel User { get; set; }
}
