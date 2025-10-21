using Identity.API.Models;
using Identity.API.Models.ViewModels;
using Shared;

namespace Identity.API.Services;

public interface IAuthService
{
    public Task<Result<Token>> Login(LoginViewModel userLoginDto);
    public Task<Result<Token>> Register(RegisterViewModel registerUserDto);
    public Task<Result<Token>> RefreshToken(string refreshToken);
}
