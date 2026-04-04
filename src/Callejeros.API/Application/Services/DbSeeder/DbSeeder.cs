using Adoption.API.Application.Exceptions;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Domain.AggregatesModel.UserAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
                await roleManager.CreateAsync(role);
        }

        var email = "davidorrego004@gmail.com";
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
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


        if (!ctx.Animals.Any())
        {
            var animals = new List<Animal>
            {
                Animal.Create(
                        name: "Coco",
                        age: 3,
                        description: "Juguetona y amigable",
                        ownerId: Guid.Parse(user.Id),
                        localization: "Matanzas",
                        species: "Labrador",
                        animalSex: Sex.Female,
                        aditionalImages: [],
                        principalImage: "TestImage",
                        vaccine: "Todas las vacunas",
                        isStirilized: false,
                        isDewormed: false,
                        healthState: "Sin caso conocido",
                        adoptionRequirements: new List<string>{
                            "Patio grande",
                            "Salir dos veces al dia"
                        },
                        size: Size.Big,
                        compatibility: new List<Compatibility>{
                            Compatibility.Childs
                        },
                        personality: new List<Personality>{
                            Personality.Energetic
                        }),

                Animal.Create(
                        name: "Champ",
                        age: 2,
                        description: "Bueno y noble",
                        ownerId: Guid.Parse(user.Id),
                        localization: "La Habana",
                        species: "Pastor",
                        animalSex: Sex.Male,
                        aditionalImages: [],
                        principalImage: "TestImage",
                        vaccine: "Todas las vacunas",
                        isStirilized: true,
                        isDewormed: false,
                        healthState: "Sin caso conocido",
                        adoptionRequirements: new List<string>{
                            "Correr por 10 minutos",
                            "Salir 1 vez al dia"
                        },
                        size: Size.Big,
                        compatibility: new List<Compatibility>{
                            Compatibility.Cats
                        },
                        personality: new List<Personality>{
                            Personality.Calm
                        })
                    };
            ctx.Animals.AddRange(animals);
            await ctx.SaveChangesAsync();
        }
    }
}
