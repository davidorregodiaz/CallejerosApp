using Adoption.API.Abstractions;
using Adoption.API.Application.Models;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Adoption.API.Application.Queries.Animals;

public class GetAnimalByIdQueryHandler(AdoptionDbContext ctx)
    : IQueryHandler<GetAnimalByIdQuery, AnimalResponse>
{
    public async Task<Result<AnimalResponse>> HandleAsync(GetAnimalByIdQuery query, CancellationToken cancellationToken)
    {
        var animal = await ctx.Animals.SingleOrDefaultAsync(x => x.Id == new AnimalId(query.Id), cancellationToken);
        
        if(animal is null)
            return Result<AnimalResponse>.FromFailure($"Animal with id - {query.Id} not found.");

        var animalResponse = new AnimalResponse(
            Id: animal.Id.Value,
            OwnerId: animal.OwnerId.Value,
            Name: animal.Name,
            Species: animal.Species,
            Breed: animal.Breed,
            Age: animal.Age,
            Description: animal.Description,
            PrincipalImageUrl: animal.PrincipalImage,
            ExtraImagesUrls: animal.AdditionalImagesUrl?.ToList());
        
        return Result<AnimalResponse>.FromData(animalResponse);
    }
}
