using Microsoft.AspNetCore.Identity;

namespace Adoption.API.Application.Models.User;

public record UserResponse(
    Guid Id,
    string Username,
    string Email,
    DateTime JoinedAt,
    string? ImageUrl,
    List<string> Roles
);
