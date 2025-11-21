using Adoption.API.Abstractions;
using Adoption.API.Application.Models.User;

namespace Adoption.API.Application.Commands.Users;

public record GrantsUserPermissionsCommand(
    Guid UserId
) : ICommand<UserResponse>;
