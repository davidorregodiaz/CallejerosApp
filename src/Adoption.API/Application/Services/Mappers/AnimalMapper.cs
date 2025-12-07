using Adoption.API.Application.Models;
using Adoption.API.Application.Services.Minio;
using Adoption.Domain.AggregatesModel.AnimalAggregate;

namespace Adoption.API.Application.Services.Mappers;

public class AnimalMapper(IMinioService minioService) : IAnimalMapper
{
    public async Task<AnimalViewModel> MapToResponse(Animal animal, CancellationToken ct)
    {
        var additionalImagesUrl = new List<string>();

        if (animal.AdditionalImagesUrl != null)
        {
            foreach (var image in animal.AdditionalImagesUrl)
            {
                additionalImagesUrl.Add(await minioService.PresignedGetUrl(image, ct));
            }
        }

        return new AnimalViewModel(
            Id: animal.Id.Value,
            OwnerId: animal.OwnerId.Value,
            Name: animal.Name,
            Species: animal.Species,
            Breed: animal.Breed,
            Age: animal.Age,
            Description: animal.Description,
            MedicalRecord: new MedicalViewModel(
                Vaccine: animal.MedicalRecord.Vaccine,
                IsStirilized: animal.MedicalRecord.IsStirilized,
                IsDewormed: animal.MedicalRecord.IsDewormed,
                HealthState: animal.MedicalRecord.HealthState),
            AdoptionRequirements: animal.AdoptionRequirements,
            PrincipalImageUrl: await minioService.PresignedGetUrl(animal.PrincipalImage, ct),
            ExtraImagesUrls: additionalImagesUrl,
            Size: animal.Size,
            Personality: animal.Personality,
            Compatibility: animal.Compatibility,
            Sex: animal.Sex,
            Status: animal.Status);
    }
}
