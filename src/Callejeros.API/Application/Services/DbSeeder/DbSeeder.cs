using Adoption.API.Application.Exceptions;
using Adoption.Domain.AggregatesModel.UserAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;

namespace Adoption.API.Application.Services.DbSeeder;

public class DbSeeder(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : IDbSeeder<AdoptionDbContext>
{
    public async Task SeedAsync(AdoptionDbContext ctx)
    {
        var roles = new List<IdentityRole>()
            {
                new IdentityRole(Roles.SUPER_ADMIN),
                new IdentityRole(Roles.ADMIN),
                new IdentityRole(Roles.REQUESTER),
                new IdentityRole(Roles.OWNER)
            };

        foreach(var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
                await roleManager.CreateAsync(role);
        }

        var email = "davidorrego004@gmail.com";
        var user = await userManager.FindByEmailAsync(email);

        if(user is null)
        {
            user = new ApplicationUser(email, "Superadmin")
            {
                EmailConfirmed = true
            };
  
            var result = await userManager.CreateAsync(user, "David.1234");

            if (!result.Succeeded)
            {
                throw new DatabaseSeedException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(user, Roles.SUPER_ADMIN))
            await userManager.AddToRoleAsync(user, Roles.SUPER_ADMIN);
    }
}
