
using Adoption.API.Abstractions;
using Adoption.API.Application.Mappers;
using Adoption.API.Application.Models;
using Adoption.API.Application.Services;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Shared;

namespace Adoption.API.Application.Commands.Animals;

public class CreateAnimalCommandHandler(IAnimalRepository animalRepository, ILogger<CreateAnimalCommandHandler> logger, IMinioService minioService)
    : ICommandHandler<CreateAnimalCommand, AnimalResponse>
{
    public async Task<AnimalResponse> HandleAsync(CreateAnimalCommand command, CancellationToken cancellationToken)
    {
        var principalImagePresignedUrl = await minioService.UploadBlob(command.PrincipalImage,null, cancellationToken);

        var aditionalImagesPresignedUrls = new List<string>();

        if (command.AdditionalImages != null)
        {
            foreach (var additionalImage in command.AdditionalImages)
            {
                aditionalImagesPresignedUrls.Add(await minioService.UploadBlob(additionalImage, null, cancellationToken));
            }
        }

        var animal = Animal.Create(
            name: command.Name,
            age: command.Age,
            description: command.Description,
            ownerId: command.OwnerId,
            breed: command.Breed,
            species: command.Species,
            aditionalImages:aditionalImagesPresignedUrls,
            principalImage: principalImagePresignedUrl
        );

        var animalResponse = animalRepository.Add(animal).MapToResponse();

        logger.LogInformation("Creating Animal - Animal : {@Animal}", animal);

        await animalRepository.UnitOfWork().SaveChangesAsync(cancellationToken);
        return animalResponse;
    }
}
