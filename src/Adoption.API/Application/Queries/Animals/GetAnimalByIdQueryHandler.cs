using Adoption.API.Abstractions;
using Adoption.API.Application.Commands.Animals;
using Adoption.API.Application.Models;
using Adoption.API.Application.Services.Mappers;
using Adoption.API.Application.Services.Minio;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Adoption.API.Application.Queries.Animals;

public class GetAnimalByIdQueryHandler(AdoptionDbContext ctx, IAnimalMapper animalMapper)
    : IQueryHandler<GetAnimalByIdQuery, AnimalViewModel>
{
    public async Task<Result<AnimalViewModel>> HandleAsync(GetAnimalByIdQuery query, CancellationToken cancellationToken)
    {
        var animal = await ctx.Animals.SingleOrDefaultAsync(x => x.Id == new AnimalId(query.Id), cancellationToken);
        
        if(animal is null)
            return Result<AnimalViewModel>.FromFailure($"Animal with id - {query.Id} not found.");
        
        var animalResponse = await animalMapper.MapToResponse(animal, cancellationToken);
        
        return Result<AnimalViewModel>.FromData(animalResponse);
    }
}
