
using Adoption.API.Abstractions;
using Adoption.API.Application.Exceptions;
using Adoption.API.Application.Models;
using Adoption.API.Application.Models.User;
using Adoption.API.Application.Services.Minio;
using Adoption.Domain.AggregatesModel.UserAggregate;
using Microsoft.AspNetCore.Identity;

namespace Adoption.API.Application.Commands.Users;

public class GrantsUserPermissionCommandHandler(UserManager<ApplicationUser> userManager, IMinioService minioService)
    : ICommandHandler<GrantsUserPermissionsCommand, UserViewModel>
{
    public async Task<UserViewModel> HandleAsync(GrantsUserPermissionsCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());

        if (user is null)
            throw new UserNotFoundException($"User with id - {command.UserId} not found");

        await userManager.AddToRolesAsync(user, new List<string> { Roles.ADMIN });

        var roles = await userManager.GetRolesAsync(user);
        var imageUrl = await minioService.PresignedGetUrl(user.AvatarUrl ?? string.Empty, cancellationToken);

        return new UserViewModel(
            Id: Guid.TryParse(user.Id, out var id) ? id : Guid.Empty,
            Email: user.Email!,
            Username: user.UserName!,
            JoinedAt: user.JoinedAt,
            ImageUrl: imageUrl,
            Roles: roles.ToList()
        );
    }
}
