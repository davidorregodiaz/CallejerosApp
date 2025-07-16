using Microsoft.AspNetCore.Identity;

namespace Core.Models;

public class AppUser : IdentityUser
{
    public AppUser(string email, string userName)
    {
        Email = email;
        UserName = userName;
    }
    public ICollection<Animal> Animals { get; set; } = new List<Animal>();
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}