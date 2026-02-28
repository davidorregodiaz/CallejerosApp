using Adoption.API.Application.Models.User;
using Identity.API.Models;
using Identity.API.Models.ViewModels;
using Shared;
using TokenViewModel = Adoption.API.Application.Models.TokenViewModel;

namespace Adoption.API.Application.Services.Auth;

public interface IAuthService
{
    public Task<Result<TokenViewModel>> Login(LoginViewModel userLoginDto);
    public Task<Result<TokenViewModel>> Register(RegisterViewModel registerUserDto);
    public Task<Result<TokenViewModel>> RefreshToken(string refreshToken);
    public Task<Result> ChangePassword(ChangePasswordViewModel changePasswordViewModel, string? userId);
}
