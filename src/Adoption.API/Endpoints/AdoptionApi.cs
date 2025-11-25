using Adoption.API.Abstractions;
using Adoption.API.Application.Commands.AdoptionRequests;
using Adoption.API.Application.Exceptions;
using Adoption.API.Application.Models;
using Adoption.API.Application.Queries;
using Adoption.API.Application.Queries.AdoptionRequests;
using Adoption.API.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Shared.Utilities;

namespace Adoption.API.Endpoints;

public static class AdoptionApi
{
    public static IEndpointRouteBuilder MapAdoptionEndpoints(this IEndpointRouteBuilder app)
    {
        var adoptionApi = app
            .MapGroup("/adoptions")
            .WithTags("Adoptions");

        adoptionApi.MapPost("/", CreateAdoptionRequestAsync)
            .Produces<CreatedAtRoute<AdoptionResponse>>(StatusCodes.Status201Created)
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequireAuthorization("RequesterPolicy");

        adoptionApi.MapGet("/{Id:guid}", GetAdoptionRequestByIdAsync)
            .WithName("GetAdoptionRequestById")
            .WithSummary("Gets an adoption request by id")
            .Produces<Ok<AdoptionResponse>>()
            .Produces<NotFound<string>>(StatusCodes.Status404NotFound)
            .AllowAnonymous();
        
        adoptionApi.MapGet("/", GetAllAdoptionsRequestAsync)
            .WithName("GetAllAdoptionRequests")
            .WithSummary("Lists all the adoption requests.")
            .Produces<NoContent>()
            .Produces<Ok<PaginatedResponse<AdoptionResponse>>>()
            .AllowAnonymous();

        adoptionApi.MapDelete("/{Id:guid}", DeleteAdoptionRequestAsync)
            .WithSummary("Deletes an adoption request")
            .Produces<NoContent>()
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequireAuthorization("RequesterPolicy", "OwnerPolicy");

        adoptionApi.MapPut("/{Id:guid}", UpdateAdoptionRequestAsync)
            .WithSummary("Updates an adoption request")
            .Produces<Ok<AdoptionResponse>>(statusCode: StatusCodes.Status200OK)
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization("OwnerPolicy");
        
        return app;
    }

    private static async Task<Results<Ok<AdoptionResponse>, ProblemHttpResult>> UpdateAdoptionRequestAsync(
        [FromRoute] Guid id,
        UpdateAdoptionRequestBody request,
        ICommandHandler<UpdateAdoptionRequestCommand, AdoptionResponse> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateAdoptionRequestCommand(
            Id: id,
            Status: request.Status);
        var response = await handler.HandleAsync(command, cancellationToken);
        return TypedResults.Ok(response);
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> DeleteAdoptionRequestAsync(
        [AsParameters] DeleteAdoptionRequestCommand command,
        ICommandHandler<DeleteAdoptionRequestCommand> handler,
        CancellationToken cancellationToken = default)
    {
        await handler.HandleAsync(command, cancellationToken);
        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<PaginatedResponse<AdoptionResponse>>,NoContent>> GetAllAdoptionsRequestAsync(
        [AsParameters] GetAllAdoptionsQuery query,
        IQueryHandler<GetAllAdoptionsQuery, PaginatedResponse<AdoptionResponse>> handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(query, cancellationToken);
        
        if (!result.IsSuccessful(out var response))
            return TypedResults.NoContent();
        
        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<AdoptionResponse>, NotFound<string>>> GetAdoptionRequestByIdAsync(
        [AsParameters] GetAdoptionRequestByIdQuery query,
        IQueryHandler<GetAdoptionRequestByIdQuery,  AdoptionResponse> handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(query, cancellationToken);

        if (!result.IsSuccessful(out var response))
            return TypedResults.NotFound(result.Message);
        
        return TypedResults.Ok(response);
    }

    private static async Task<Results<CreatedAtRoute<AdoptionResponse>, ProblemHttpResult>> CreateAdoptionRequestAsync(
        CreateAdoptionRequest request,
        ICommandHandler<CreateAdoptionRequestCommand, AdoptionResponse> handler,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        var userId = context.GetUserIdFromContext();
        
        var command = new CreateAdoptionRequestCommand(
            RequesterId: userId ?? Guid.Empty,
            Comments: request.Comments,
            AnimalId: request.AnimalId);
        
        var response = await handler.HandleAsync(command,cancellationToken);
        return TypedResults.CreatedAtRoute(response, routeName: "GetAdoptionRequestById",
            routeValues: new { id = response.AdoptionRequestId });
    }
}
