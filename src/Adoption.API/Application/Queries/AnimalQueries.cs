using System;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Dtos;

namespace Adoption.API.Application.Queries;

public class AnimalQueries : IAnimalQueries
{
    private readonly AdoptionDbContext _ctx;

    public AnimalQueries(AdoptionDbContext context)
    {
        _ctx = context;
    }
    public async Task<TaskResult<Animal>> FindAnimalById(Guid id)
    {
        var animal = await _ctx.Animals
                        .Where(a => a.Id.Value == id)
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
            return TaskResult<Animal>.FromData(animalFound);
        }
        
        return TaskResult<Animal>.FromFailure("Animal not found");
    }
}
