
using Adoption.API.Abstractions;
using Adoption.API.Application.Commands.Users;
using Adoption.API.Application.Models.User;
using Adoption.API.Application.Queries;
using Adoption.API.Application.Queries.Users;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Adoption.API.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var usersApi = app
            .MapGroup("/users")
            .WithTags("Users")
            .WithOpenApi();

        usersApi.MapGet("/", GetAllUsersAsync)
            .WithSummary("Lists all the users in the system")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status200OK, responseType: typeof(PaginatedResponse<UserResponse>));

        usersApi.MapPost("/grant_admin_permissions", GrantsUserAdminPermissions)
            .WithSummary("Provide the specified user with admin privileges")
            .Produces<Ok<UserResponse>>()
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization("SuperAdminOnlyPolicy");
            
        return usersApi;
    }


    private static async Task<Results<Ok<PaginatedResponse<UserResponse>>, NoContent>> GetAllUsersAsync(
        [AsParameters] GetAllUsersQuery query,
        IQueryHandler<GetAllUsersQuery, PaginatedResponse<UserResponse>> handler,
        CancellationToken ct = default)
    {
        var result = await handler.HandleAsync(query, ct);

        if (!result.IsSuccessful(out var response))
        {
            return TypedResults.NoContent();
        }

        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<UserResponse>,ProblemHttpResult>> GrantsUserAdminPermissions(
        [AsParameters] GrantsUserPermissionsCommand command,
        ICommandHandler<GrantsUserPermissionsCommand, UserResponse> handler,
        CancellationToken ct = default
    )
    {
        var response = await handler.HandleAsync(command, ct);
        return TypedResults.Ok(response);
    }
}
