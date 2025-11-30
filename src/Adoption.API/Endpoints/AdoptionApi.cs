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
            .Produces<CreatedAtRoute<AdoptionViewModel>>(StatusCodes.Status201Created)
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequireAuthorization("RequesterPolicy");
        
        adoptionApi.MapPost("/appointment", CreateAdoptionRequestAppointmentAsync)
            .Produces<Ok<AdoptionViewModel>>()
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequireAuthorization("OwnerPolicy");

        adoptionApi.MapGet("/{Id:guid}", GetAdoptionRequestByIdAsync)
            .WithName("GetAdoptionRequestById")
            .WithSummary("Gets an adoption request by id")
            .Produces<Ok<AdoptionViewModel>>()
            .Produces<NotFound<string>>(StatusCodes.Status404NotFound)
            .AllowAnonymous();
        
        adoptionApi.MapGet("/", GetAllAdoptionsRequestAsync)
            .WithName("GetAllAdoptionRequests")
            .WithSummary("Lists all the adoption requests.")
            .Produces<NoContent>()
            .Produces<Ok<PaginatedResponse<AdoptionViewModel>>>()
            .AllowAnonymous();

        adoptionApi.MapDelete("/{Id:guid}", DeleteAdoptionRequestAsync)
            .WithSummary("Deletes an adoption request")
            .Produces<NoContent>()
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequireAuthorization("RequesterPolicy", "OwnerPolicy");

        adoptionApi.MapPut("/{Id:guid}", UpdateAdoptionRequestAsync)
            .WithSummary("Updates an adoption request")
            .Produces<Ok<AdoptionViewModel>>(statusCode: StatusCodes.Status200OK)
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization("OwnerPolicy");
        
        return app;
    }

    private static async Task<Results<Ok<AdoptionViewModel>, ProblemHttpResult>> CreateAdoptionRequestAppointmentAsync(
        CreateAdoptionRequestAppointmentRequest request,
        ICommandHandler<CreateAdoptionRequestAppointmentCommand, AdoptionViewModel> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateAdoptionRequestAppointmentCommand(
            AdoptionRequestid: request.AdoptionRequestId,
            Date: request.Date,
            Notes: request.Notes,
            Location: request.Location);
        
        var response = await handler.HandleAsync(command,cancellationToken);
        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<AdoptionViewModel>, ProblemHttpResult>> UpdateAdoptionRequestAsync(
        [FromRoute] Guid id,
        UpdateAdoptionRequest request,
        ICommandHandler<UpdateAdoptionRequestCommand, AdoptionViewModel> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateAdoptionRequestCommand(
            Id: id,
            Status: request.Status);
        var response = await handler.HandleAsync(command, cancellationToken);
        return TypedResults.Ok(response);
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> DeleteAdoptionRequestAsync(
        [AsParameters] DeleteAdoptionRequest request,
        ICommandHandler<DeleteAdoptionRequestCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteAdoptionRequestCommand(request.Id);
        
        await handler.HandleAsync(command, cancellationToken);
        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<PaginatedResponse<AdoptionViewModel>>,NoContent>> GetAllAdoptionsRequestAsync(
        [AsParameters] GetAllAdoptionsQueryRequest request,
        HttpContext httpContext,
        IQueryHandler<GetAllAdoptionsQuery, PaginatedResponse<AdoptionViewModel>> handler,
        CancellationToken cancellationToken = default)
    {

        var userId = httpContext.GetUserIdFromContext();
        
        var query = new GetAllAdoptionsQuery(
            UserId: userId ?? Guid.Empty,
            Status: request.Status,
            Date: request.Date,
            Page:  request.Page,
            PageSize: request.PageSize,
            SortBy: request.SortBy,
            IsDescending: request.IsDescending);
        
        var result = await handler.HandleAsync(query, cancellationToken);
        
        if (!result.IsSuccessful(out var response))
            return TypedResults.NoContent();
        
        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<AdoptionViewModel>, NotFound<string>>> GetAdoptionRequestByIdAsync(
        [AsParameters] GetAdoptionRequestByIdQuery query,
        IQueryHandler<GetAdoptionRequestByIdQuery,  AdoptionViewModel> handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(query, cancellationToken);

        if (!result.IsSuccessful(out var response))
            return TypedResults.NotFound(result.Message);
        
        return TypedResults.Ok(response);
    }

    private static async Task<Results<CreatedAtRoute<AdoptionViewModel>, ProblemHttpResult>> CreateAdoptionRequestAsync(
        CreateAdoptionRequest request,
        ICommandHandler<CreateAdoptionRequestCommand, AdoptionViewModel> handler,
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
