namespace Adoption.API.Endpoints;

public static class AnimalApi
{
    public static IEndpointRouteBuilder MapAnimalEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/animals");

        api.MapPost("/", CreateAnimalAsync);
        
        return app;
    }

    public static Task CreateAnimalAsync(HttpContext context)
    {
        throw new NotImplementedException();
    }
}
