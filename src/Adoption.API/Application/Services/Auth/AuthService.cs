using System.Text;
using Adoption.API.Application.Models.User;
using Adoption.API.Application.Queues;
using Adoption.API.Application.Services.Email;
using Adoption.API.Application.Services.Minio;
using Adoption.Domain.AggregatesModel.UserAggregate;
using Adoption.Infrastructure.Context;
using Adoption.Infrastructure.Services;
using Identity.API.Models;
using Identity.API.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shared;
using TokenViewModel = Adoption.API.Application.Models.TokenViewModel;

namespace Adoption.API.Application.Services.Auth;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IMinioService minioService,
    IConfiguration configuration,
    TokenService tokenService,
    IEmailQueue emailQueue,
    ILogger<AuthService> logger,
    AdoptionDbContext ctx)
    : IAuthService
{
    public async Task<Result<TokenViewModel>> Login(LoginViewModel loginVm)
    {
        var user = await userManager.FindByEmailAsync(loginVm.Email);
        if (user is not null)
        {
            var passwordExists = await userManager.CheckPasswordAsync(user, loginVm.Password);
            if (passwordExists)
            {
                var tokenDto = await tokenService.GenerateTokenDto(user);
                var imageUrl =
                    await minioService.PresignedGetUrl(user.AvatarUrl ?? string.Empty, CancellationToken.None);
                var userRoles = await userManager.GetRolesAsync(user);
                tokenDto.User = new UserViewModel(
                    Id: Guid.TryParse(user.Id, out var userId) ? userId : Guid.Empty,
                    Username: user.UserName!,
                    Email: user.Email!,
                    JoinedAt: user.JoinedAt,
                    ImageUrl: imageUrl,
                    Roles: userRoles.ToList());
                return Result<TokenViewModel>.FromData(tokenDto);
            }
        }

        return Result<TokenViewModel>.FromFailure("Invalid email or password");
    }

    public async Task<Result<TokenViewModel>> Register(RegisterViewModel registerVm)
    {
        if (registerVm.Avatar.Length == 0)
            return Result<TokenViewModel>.FromFailure("Avatar image is required");
        
        string imagePath;
        
        try
        {
            imagePath = await minioService.UploadBlob(registerVm.Avatar, null, CancellationToken.None);
        }
        catch (Exception e)
        {
            logger.LogError("Error uploading avatar for user {Username}: {ErrorMessage}", registerVm.Username, e.Message);
            throw;
        }
        
        var appUser = new ApplicationUser(registerVm.Email, registerVm.Username);
        var result = await userManager.CreateAsync(appUser, registerVm.Password);

        if (result.Succeeded)
        {
            appUser.AvatarUrl = imagePath;

            await userManager.AddToRoleAsync(appUser, ChooseRole(registerVm.Role));

            await userManager.UpdateAsync(appUser);

            var tokenDto = await tokenService.GenerateTokenDto(appUser);

            var emailRequest = new EmailRequest(
                To: appUser.Email!,
                Subject: "Registration successfully",
                Data: new Dictionary<string, string>() { ["AppUser"] = appUser.UserName! },
                TemplateType: EmailTemplateType.Welcome);

            await emailQueue.EnqueueAsync(emailRequest);

            var imageUrl =
                await minioService.PresignedGetUrl(appUser.AvatarUrl ?? string.Empty, CancellationToken.None);
            var userRoles = await userManager.GetRolesAsync(appUser);
            tokenDto.User = new UserViewModel(
                Id: Guid.TryParse(appUser.Id, out var userId) ? userId : Guid.Empty,
                Username: appUser.UserName!,
                Email: appUser.Email!,
                JoinedAt: appUser.JoinedAt,
                ImageUrl: imageUrl,
                Roles: userRoles.ToList());

            return Result<TokenViewModel>.FromData(tokenDto);
        }

        await minioService.DeleteBlob(imagePath, CancellationToken.None);

        return Result<TokenViewModel>.FromFailure($"User {registerVm.Email} registration failed");
    }

    public async Task<Result<TokenViewModel>> RefreshToken(string refreshToken)
    {
        var user = await tokenService.FindUserByRefreshToken(refreshToken);
        if (user is null)
            return Result<TokenViewModel>.FromFailure("Invalid token");

        var result = await tokenService.ValidateRefreshToken(refreshToken, user);

        if (result.IsSuccessful(out _))
        {
            // Rotar el refresh token (nuevo token)
            var newRefreshToken = await tokenService.GenerateRefreshToken(user);

            var imageUrl = await minioService.PresignedGetUrl(user.AvatarUrl ?? string.Empty, CancellationToken.None);
            var userRoles = await userManager.GetRolesAsync(user);
            var userResponse = new UserViewModel(
                Id: Guid.TryParse(user.Id, out var userId) ? userId : Guid.Empty,
                Username: user.UserName!,
                Email: user.Email!,
                JoinedAt: user.JoinedAt,
                ImageUrl: imageUrl,
                Roles: userRoles.ToList());

            return Result<TokenViewModel>.FromData(new TokenViewModel
            {
                AccessToken = await tokenService.GenerateAccessToken(user),
                RefreshToken = newRefreshToken,
                User = userResponse,
                ExpiresIn = Convert.ToInt32(configuration["Jwt:ExpireMinutes"])
            });
        }

        return Result<TokenViewModel>.FromFailure("Token expired or invalid");
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

    private static string ChooseRole(string role)
    {
        return role.ToLower().Trim() switch
        {
            "admin" => Roles.ADMIN,
            "owner" => Roles.OWNER,
            "requester" => Roles.REQUESTER,
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }
}
