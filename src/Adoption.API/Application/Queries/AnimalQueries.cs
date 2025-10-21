using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Adoption.API.Application.Queries;

public class AnimalQueries : IAnimalQueries
{
    private readonly AdoptionDbContext _ctx;

    public AnimalQueries(AdoptionDbContext context)
    {
        _ctx = context;
    }

    public async Task<Result<IEnumerable<Animal>>> FindAllAnimals()
    {
        var animals = await _ctx.Animals
                        .Select(a => new Animal(
                            a.Id.Value,
                            a.OwnerId.Value,
                            a.Name,
                            a.Age,
                            a.Breed.Value,
                            a.AnimalType.Value,
                            a.ImagesPath,
                            a.Description
                        ))
                        .ToListAsync();
        if (animals is not null)
            return Result<IEnumerable<Animal>>.FromData(animals);
        
        return Result<IEnumerable<Animal>>.FromFailure("No animals available");
    }

    public async Task<Result<Animal>> FindAnimalById(Guid id)
    {
        
        var animal = await _ctx.Animals
                        .Where(a => a.Id == new AnimalId(id))
                        .SingleOrDefaultAsync();

        if (animal is not null)
        {
            var animalFound = new Animal(
                animal.Id.Value,
                animal.OwnerId.Value,
                animal.Name,
                animal.Age,
                animal.Breed.Value,
                animal.AnimalType.Value,
                animal.ImagesPath,
                animal.Description
            );
            return Result<Animal>.FromData(animalFound);
        }
        
        return Result<Animal>.FromFailure("Animal not found");
    }
}
