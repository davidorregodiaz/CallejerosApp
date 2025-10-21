using Adoption.API.Application.Queries;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Infrastructure.Context;
using Adoption.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Adoption.API;

public static class DependencyInjection
{
    public static void AddAppServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddRazorPages(); // Necesario para Blazor
        builder.Services.AddOpenApi();
        builder.Services.AddDbContext<AdoptionDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
        builder.Services.AddScoped<IAnimalQueries, AnimalQueries>();
    }
}
