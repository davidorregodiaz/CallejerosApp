using System;
using Application.Dtos;
using Core.Models;
using Shared;

namespace Application.Mappers;

public static class AnimalMapper
{
    public static ResponseAnimalDto ToDto(this Animal animal)
    {
        return new ResponseAnimalDto(
            animal.Id.ToString(),
            animal.OwnerId,
            animal.Name,
            animal.Age,
            animal.Breed.Value,
            animal.AnimalType.Value,
            animal.Requirements.Select(a => a.Value).ToList()
        );
    }

    public static TaskResult<Animal> ToModel(this CreateAnimalDto createAnimalDto,Animal? animalDb)
    {
        if (animalDb is null)
        {
            animalDb = new Animal
            {
                Age = createAnimalDto.Age,
                Name = createAnimalDto.Name,
                OwnerId = createAnimalDto.OwnerId
            };
        }

        var validateAnimalResult = ValidateAnimal(createAnimalDto, animalDb);

        if (validateAnimalResult.IsSuccessful(out var animalValidated))
            return TaskResult<Animal>.FromData(animalValidated);

        return TaskResult<Animal>.FromFailure(validateAnimalResult.Message);

    }

    private static TaskResult<Animal> ValidateAnimal(CreateAnimalDto createAnimalDto,Animal animalDb)
    {

        var breedResult = animalDb.SetBreed(createAnimalDto.Breed.ToLowerInvariant().Trim());
        var typeResult = animalDb.SetType(createAnimalDto.Type.ToLowerInvariant().Trim());
        TaskResult requierementResult = new();

        if (!breedResult.Success)
            return TaskResult<Animal>.FromFailure(breedResult.Message);

        if (!typeResult.Success)
            return TaskResult<Animal>.FromFailure(typeResult.Message);

        animalDb.ClearRequierements();
        foreach (var requierement in createAnimalDto.Requierements)
        {
            requierementResult = animalDb.AddRequierement(requierement);
            if (!requierementResult.Success)
                return TaskResult<Animal>.FromFailure(requierementResult.Message);
        }
            
        return TaskResult<Animal>.FromData(animalDb);
    }
}
