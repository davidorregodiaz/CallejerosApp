using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Shared;
using Shared.Dtos;

namespace Adoption.API.Application.Mappers;

public static class AnimalMapper
{
    public static ResponseAnimalDto ToResponse(this Animal animal)
    {
        return new ResponseAnimalDto(
            animal.Id.ToString(),
            animal.OwnerId.ToString(),
            animal.Name,
            animal.Age,
            animal.Breed.Value,
            animal.AnimalType.Value,
            animal.ImagesPath,
            animal.Description
        );
    }

    public static Result<Animal> ToModel(this CreateAnimalDto createAnimalDto, Animal? animalDb, List<string>? picturesPaths)
    {
        // if (animalDb is null)
        // {
        //     animalDb = new Animal
        //     {
        //         Age = createAnimalDto.Age,
        //         Name = createAnimalDto.Name,
        //         OwnerId = createAnimalDto.OwnerId,
        //         ImagesPath = picturesPaths,
        //         Description = createAnimalDto.Description
        //     };
        // }

        var validateAnimalResult = ValidateAnimal(createAnimalDto, animalDb);

        if (validateAnimalResult.IsSuccessful(out var animalValidated))
            return Result<Animal>.FromData(animalValidated);

        return Result<Animal>.FromFailure(validateAnimalResult.Message);

    }

    private static Result<Animal> ValidateAnimal(CreateAnimalDto createAnimalDto, Animal animalDb)
    {

        // var breedResult = animalDb.SetBreed(createAnimalDto.Breed.ToLowerInvariant().Trim());
        // var typeResult = animalDb.SetType(createAnimalDto.Type.ToLowerInvariant().Trim());
        Result requierementResult = new();

        // if (!breedResult.Success)
        //     return Result<Animal>.FromFailure(breedResult.Message);

        // if (!typeResult.Success)
        //     return Result<Animal>.FromFailure(typeResult.Message);



        return Result<Animal>.FromData(animalDb);
    }
}
