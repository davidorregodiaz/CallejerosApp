using Microsoft.AspNetCore.Identity;

namespace Adoption.API.Application.Models.User;

public record UserViewModel(
    Guid Id,
    string Username,
    string Email,
    DateTime JoinedAt,
    string? ImageUrl,
    List<string> Roles
);
