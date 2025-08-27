
using Microsoft.AspNetCore.Identity;

namespace Identity.API.Models;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser(string email, string userName)
    {
        Email = email;
        UserName = userName;
    }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
