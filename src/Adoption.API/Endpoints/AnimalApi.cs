using Adoption.API.Abstractions;
using Adoption.API.Application.Commands.Animals;
using Adoption.API.Application.Models;
using Adoption.API.Application.Queries;
using Adoption.API.Application.Queries.Animals;
using Adoption.API.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Utilities;

namespace Adoption.API.Endpoints;

public static class AnimalApi
{
    public static IEndpointRouteBuilder MapAnimalEndpoints(this IEndpointRouteBuilder app)
    {
        var animalApi = app
            .MapGroup("/animals")
            .WithTags("Animals")
            .RequireAuthorization("AnimalsManagementPolicy");

        animalApi.MapPost("/", CreateAnimalAsync)
            .Accepts<CreateAnimalCommand>("multipart/form-data")
            .DisableAntiforgery()
            .WithSummary("Creates an animal")
            .Produces<CreatedAtRoute<AnimalViewModel>>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        animalApi.MapGet("/{Id:guid}", GetAnimalByIdAsync)
            .WithName("GetAnimalById")
            .WithSummary("Gets an animal by an id.")
            .Produces<Ok<AnimalViewModel>>(statusCode: StatusCodes.Status200OK)
            .Produces<NotFound<string>>()
            .AllowAnonymous();

        animalApi.MapGet("/", GetAllAnimalsAsync)
            .WithName("GetAllAnimals")
            .WithSummary("Lists all the animals.")
            .Produces<PaginatedResponse<AnimalViewModel>>()
            .Produces(StatusCodes.Status204NoContent)
            .AllowAnonymous();

        animalApi.MapPut("/{Id:guid}", UpdateAnimalAsync)
            .Accepts<UpdateAnimalCommand>("multipart/form-data")
            .DisableAntiforgery()
            .WithName("UpdateAnimal")
            .WithSummary("Updates an animal.")
            .Produces<Ok<AnimalViewModel>>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        
        animalApi.MapDelete("/{Id:guid}", DeleteAnimalAsync)
            .WithName("DeleteAnimal")
            .WithSummary("Deletes an animal.")
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        
        return app;
    }
    private static async Task<Results<CreatedAtRoute<AnimalViewModel>, ProblemHttpResult>> CreateAnimalAsync(
        [FromForm] CreateAnimalRequest request,
        HttpContext context,
        ICommandHandler<CreateAnimalCommand, AnimalViewModel> handler,
        CancellationToken cancellationToken = default)
    {
        var ownerId = context.GetUserIdFromContext();
        var createAnimalCommand = new CreateAnimalCommand
        {
            Name = request.Name,
            Age = request.Age,
            Breed = request.Breed,
            Species = request.Species,
            Description = request.Description,
            HealthState = request.HealthState,
            Vaccine = request.Vaccine,
            IsSterilized = request.IsStirilized,
            IsDewormed = request.IsDewormed,
            Size = request.Size,
            Compatibility = request.Compatibility,
            Personality = request.Personality,
            PrincipalImage = request.PrincipalImage,
            AdditionalImages = request.AdditionalImages,
            OwnerId = ownerId ?? Guid.Empty,
            Requirements = request.Requirements
        };
        
        var response = await handler.HandleAsync(createAnimalCommand, cancellationToken);
        return TypedResults.CreatedAtRoute(response, routeName: "GetAnimalById", routeValues: new { id = response.Id });
    }

    private static async Task<Results<Ok<AnimalViewModel>, NotFound<string>>> GetAnimalByIdAsync(
        [AsParameters] GetAnimalByIdQuery query,
        IQueryHandler<GetAnimalByIdQuery, AnimalViewModel> handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(query, cancellationToken);
        
        if(!result.IsSuccessful(out var response))
            return TypedResults.NotFound(result.Message);
        
        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<PaginatedResponse<AnimalViewModel>>, NoContent>> GetAllAnimalsAsync(
            [AsParameters] GetAllAnimalsQuery query,
            IQueryHandler<GetAllAnimalsQuery, PaginatedResponse<AnimalViewModel>> handler,
            CancellationToken ct = default)
    {
        var result = await handler.HandleAsync(query, ct);

        if (!result.IsSuccessful(out var response))
            return TypedResults.NoContent();

        if (result.IsSuccessful(out var animals))
            return  TypedResults.Ok(animals);
        
        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<AnimalViewModel>, ProblemHttpResult>> UpdateAnimalAsync(
        [FromRoute] Guid id,
        [FromForm] UpdateAnimalRequest request,
        ICommandHandler<UpdateAnimalCommand, AnimalViewModel> handler,
        CancellationToken ct = default)
    {
        var command = new UpdateAnimalCommand(
            id,
            request.Name,
            request.Species,
            request.Breed,
            request.Age,
            request.Description,
            request.PrincipalImage,
            request.AdditionalImages
        );
        
        var response = await handler.HandleAsync(command, ct);
        return TypedResults.Ok(response);
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> DeleteAnimalAsync([AsParameters] DeleteAnimalRequest request,
        ICommandHandler<DeleteAnimalCommand> handler,
        CancellationToken ct = default)
    {
        var command = new DeleteAnimalCommand(request.Id);
        
        await handler.HandleAsync(command, ct);
        return TypedResults.NoContent();
    }

}
 
