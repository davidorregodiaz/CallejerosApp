using System.Text;
using Identity.API.Models;
using Identity.API.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Shared;
using Shared.Dtos;

namespace Identity.API.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly TokenService _tokenService;
    
     
    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration, TokenService tokenService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _tokenService = tokenService;
    }

    public async Task<TaskResult<TokenModel>> Login(LoginViewModel loginVm)
    {
        var user = await _userManager.FindByEmailAsync(loginVm.Email);
        if (user is not null)
        {
            var passwordExists = await _userManager.CheckPasswordAsync(user, loginVm.Password);
            if (passwordExists)
            {
                var tokenDto = await _tokenService.GenerateTokenDto(user);
                return TaskResult<TokenModel>.FromData(tokenDto);
            }
        }
        return TaskResult<TokenModel>.FromFailure("Invalid email or password");
    }

    public async Task<TaskResult<TokenModel>> Register(RegisterViewModel registerVm)
    {
        var appUser = new ApplicationUser(registerVm.Email,registerVm.Username);
        var result = await _userManager.CreateAsync(appUser, registerVm.Password);

        if (result.Succeeded)
        {
            var tokenDto  = await _tokenService.GenerateTokenDto(appUser);
            return TaskResult<TokenModel>.FromData(tokenDto);
        }
        return TaskResult<TokenModel>.FromFailure($"User {registerVm.Email} registration failed with errors: {GetErrors(result.Errors)}");
    }

    public async Task<TaskResult<TokenModel>> RefreshToken(string refreshToken)
    {
        var user = await _tokenService.FindUserByRefreshToken(refreshToken);
        if (user is null)
            return TaskResult<TokenModel>.FromFailure("Invalid token");

        var result = await _tokenService.ValidateRefreshToken(refreshToken, user);

        if (result.IsSuccessful(out _))
        {
            // Generar nuevo access token
            var newAccessToken = _tokenService.GenerateAccessToken(user);

            // Rotar el refresh token (nuevo token)
            var newRefreshToken =  await _tokenService.GenerateRefreshToken(user);

            return TaskResult<TokenModel>.FromData(new TokenModel
            {
                AccessToken = _tokenService.GenerateAccessToken(user),
                RefreshToken = newRefreshToken,
                ExpiresIn = Convert.ToInt32(_configuration["Jwt:ExpireMinutes"])
            });
        }
        return TaskResult<TokenModel>.FromFailure("Token expired or invalid");
    }

    private static string GetErrors(IEnumerable<IdentityError> errors)
    {
        var stringBuilder = new StringBuilder();
        foreach (var error in errors)
        {
            stringBuilder.AppendLine(error.Description);
        }
        return stringBuilder.ToString();
    }
    
}
