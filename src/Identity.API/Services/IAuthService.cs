using Identity.API.Models.ViewModels;
using Shared;
using Shared.Dtos;

namespace Identity.API.Services;

public interface IAuthService
{
    public Task<TaskResult<TokenModel>> Login(LoginViewModel userLoginDto);
    public Task<TaskResult<TokenModel>> Register(RegisterViewModel registerUserDto);
    public Task<TaskResult<TokenModel>> RefreshToken(string refreshToken);
}
