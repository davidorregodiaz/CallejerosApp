
namespace Adoption.API.Application.Services.DbSeeder;

public interface IDbSeeder<TContext>
{
    Task SeedAsync(TContext ctx);
}
