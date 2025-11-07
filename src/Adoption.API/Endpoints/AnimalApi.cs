using Adoption.API.Abstractions;
using Adoption.API.Application.Commands.Animals;
using Adoption.API.Application.Models;
using Adoption.API.Application.Queries;
using Adoption.API.Application.Queries.Animals;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Adoption.API.Endpoints;

public static class AnimalApi
{
    public static IEndpointRouteBuilder MapAnimalEndpoints(this IEndpointRouteBuilder app)
    {
        var animalApi = app
            .MapGroup("/animals")
            .WithTags("Animal")
            .WithOpenApi();

        animalApi.MapPost("/", CreateAnimalAsync)
            .Accepts<CreateAnimalCommand>("multipart/form-data")
            .DisableAntiforgery()
            .WithSummary("Creates an animal")
            .Produces<CreatedAtRoute<AnimalResponse>>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        animalApi.MapGet("/{Id:guid}", GetAnimalByIdAsync)
            .WithName("GetAnimalById")
            .WithSummary("Gets an animal by an id.")
            .Produces<Ok<AnimalResponse>>(statusCode: StatusCodes.Status200OK)
            .Produces<NotFound<string>>();
        
        animalApi.MapGet("/", GetAllAnimalsAsync)
            .WithName("GetAllAnimals")
            .WithSummary("Lists all the animals.")
            .Produces<PaginatedResponse<AnimalResponse>>()
            .Produces(StatusCodes.Status204NoContent);
        
        animalApi.MapPut("/{Id:guid}", UpdateAnimalAsync)
            .Accepts<UpdateAnimalCommand>("multipart/form-data")
            .DisableAntiforgery()
            .WithName("UpdateAnimal")
            .WithSummary("Updates an animal.")
            .Produces<Ok<AnimalResponse>>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        
        animalApi.MapDelete("/", DeleteAnimalAsync)
            .WithName("DeleteAnimal")
            .WithSummary("Deletes an animal.")
            .Produces<NoContent>(StatusCodes.Status204NoContent)
            .Produces<ProblemHttpResult>(StatusCodes.Status404NotFound)
            .Produces<ProblemHttpResult>(StatusCodes.Status500InternalServerError);
        
        return app;
    }
    private static async Task<Results<CreatedAtRoute<AnimalResponse>, ProblemHttpResult>> CreateAnimalAsync(
        [FromForm] CreateAnimalCommand createAnimalCommand,
        ICommandHandler<CreateAnimalCommand, AnimalResponse> handler,
        CancellationToken cancellationToken = default)
    {
        var response = await handler.HandleAsync(createAnimalCommand, cancellationToken);
        return TypedResults.CreatedAtRoute(response, routeName: "GetAnimalById", routeValues: new { id = response.Id });
    }

    private static async Task<Results<Ok<AnimalResponse>, NotFound<string>>> GetAnimalByIdAsync(
        [AsParameters] GetAnimalByIdQuery query,
        IQueryHandler<GetAnimalByIdQuery, AnimalResponse> handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(query, cancellationToken);
        
        if(!result.IsSuccessful(out var response))
            return TypedResults.NotFound(result.Message);
        
        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<PaginatedResponse<AnimalResponse>>, NoContent>> GetAllAnimalsAsync(
            [AsParameters] GetAllAnimalsQuery query,
            IQueryHandler<GetAllAnimalsQuery, PaginatedResponse<AnimalResponse>> handler,
            CancellationToken ct = default)
    {
        var result = await handler.HandleAsync(query, ct);

        if (!result.IsSuccessful(out var response))
            return TypedResults.NoContent();

        if (result.IsSuccessful(out var animals))
            return  TypedResults.Ok(animals);
        
        
        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<AnimalResponse>, ProblemHttpResult>> UpdateAnimalAsync(
        [FromRoute] Guid id,
        [FromForm] UpdateAnimalRequest request,
        ICommandHandler<UpdateAnimalCommand, AnimalResponse> handler,
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

    private static async Task<Results<NoContent, ProblemHttpResult>> DeleteAnimalAsync([AsParameters] DeleteAnimalCommand command,
        ICommandHandler<DeleteAnimalCommand> handler,
        CancellationToken ct = default)
    {
        await handler.HandleAsync(command, ct);
        return TypedResults.NoContent();
    }

}
 
