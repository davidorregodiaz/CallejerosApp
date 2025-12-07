using Adoption.Domain.SeedWork;
using Microsoft.AspNetCore.Identity;

namespace Adoption.Domain.AggregatesModel.UserAggregate;

public sealed class ApplicationUser 
    : IdentityUser,IAggregateRoot
{
    public ApplicationUser(string email, string userName)
    {
        Email = email;
        UserName = userName;
        JoinedAt = DateTime.UtcNow;
    }
    public DateTime JoinedAt { get; set; }
    public string? AvatarUrl { get; set; }
    
}
