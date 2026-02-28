
using Adoption.API.Abstractions;
using Adoption.API.Application.Commands.Users;
using Adoption.API.Application.Models;
using Adoption.API.Application.Models.User;
using Adoption.API.Application.Queries;
using Adoption.API.Application.Queries.Users;
using Adoption.API.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Adoption.API.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var usersApi = app
            .MapGroup("/user")
            .WithTags("User")
            .RequireAuthorization("UsersManagementPolicy")
            .WithOpenApi();

        usersApi.MapGet("/", GetAllUsersAsync)
            .WithSummary("Lists all the users in the system")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status200OK, responseType: typeof(PaginatedResponse<UserViewModel>));

        usersApi.MapPost("/grant_admin_permissions", GrantsUserAdminPermissions)
            .WithSummary("Provide the specified user with admin privileges")
            .Produces<Ok<UserViewModel>>()
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization("SuperAdminOnlyPolicy");

        usersApi.MapGet("/adoptions", GetUserAdoptions)
            .WithSummary("Gets the adoptions made or requested by an user")
            .Produces<Ok<PaginatedResponse<AdoptionViewModel>>>()
            .Produces<NoContent>();

        usersApi.MapGet("/animals", GetUserAnimals)
            .WithSummary("Gets the animals associated to an user")
            .Produces<Ok<PaginatedResponse<AnimalViewModel>>>()
            .Produces<NoContent>()
            .AllowAnonymous();
            
        return usersApi;
    }

    private static async Task<Results<Ok<PaginatedResponse<AdoptionViewModel>>,NoContent, ForbidHttpResult>> GetUserAdoptions(
        [AsParameters] GetUserAdoptionsRequest request,
        HttpContext context,
        IQueryHandler<GetUserAdoptionsQuery,PaginatedResponse<AdoptionViewModel>> handler,
        CancellationToken ct = default)
    {
        var userIdFromContext = context.GetUserIdFromContext();

        if (userIdFromContext is null)
        {
            return TypedResults.Forbid();
        }
        
        var query = new GetUserAdoptionsQuery(
            (Guid)userIdFromContext,
            request.Page,
            request.PageSize);
        
        var result = await handler.HandleAsync(query, ct);

        if (!result.IsSuccessful(out var response))
                return TypedResults.NoContent();
        
        return TypedResults.Ok(response);
    }
    
    private static async Task<Results<Ok<PaginatedResponse<AnimalViewModel>>,NoContent>> GetUserAnimals(
        [AsParameters] GetUserAnimalsRequest request,
        HttpContext context,
        IQueryHandler<GetUserAnimalsQuery,PaginatedResponse<AnimalViewModel>> handler,
        CancellationToken ct = default)
    {
        var userIdFromContext = context.GetUserIdFromContext();
        
        var query = new GetUserAnimalsQuery(
            userIdFromContext ?? Guid.Empty,
            request.Page,
            request.PageSize);
        
        var result = await handler.HandleAsync(query, ct);

        if (!result.IsSuccessful(out var response))
            return TypedResults.NoContent();
        
        return TypedResults.Ok(response);
    }


    private static async Task<Results<Ok<PaginatedResponse<UserViewModel>>, NoContent>> GetAllUsersAsync(
        [AsParameters] GetAllUsersQuery query,
        IQueryHandler<GetAllUsersQuery, PaginatedResponse<UserViewModel>> handler,
        CancellationToken ct = default)
    {
        var result = await handler.HandleAsync(query, ct);

        if (!result.IsSuccessful(out var response))
        {
            return TypedResults.NoContent();
        }

        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<UserViewModel>,ProblemHttpResult>> GrantsUserAdminPermissions(
        [AsParameters] GrantsUserPermissionsCommand command,
        ICommandHandler<GrantsUserPermissionsCommand, UserViewModel> handler,
        CancellationToken ct = default
    )
    {
        var response = await handler.HandleAsync(command, ct);
        return TypedResults.Ok(response);
    }
}
