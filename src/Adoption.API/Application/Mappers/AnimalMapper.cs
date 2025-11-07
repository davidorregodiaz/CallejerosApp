using Adoption.API.Application.Models;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Shared;
using Shared.Dtos;

namespace Adoption.API.Application.Mappers;

public static class AnimalMapper
{
    public static AnimalResponse MapToResponse(this Animal animal)
    {
        return new AnimalResponse(
            Id: animal.Id.Value,
            OwnerId: animal.OwnerId.Value,
            Name: animal.Name,
            Species: animal.Species,
            Breed: animal.Breed,
            Age: animal.Age,
            Description: animal.Description,
            PrincipalImageUrl: animal.PrincipalImage,
            ExtraImagesUrls: animal.AdditionalImagesUrl?.ToList()
        );
    }
}
