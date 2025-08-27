using Identity.API.Models;
using Identity.API.Models.ViewModels;
using Shared;

namespace Identity.API.Services;

public interface IAuthService
{
    public Task<TaskResult<Token>> Login(LoginViewModel userLoginDto);
    public Task<TaskResult<Token>> Register(RegisterViewModel registerUserDto);
    public Task<TaskResult<Token>> RefreshToken(string refreshToken);
}
