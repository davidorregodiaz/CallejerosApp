using Adoption.API.Abstractions;
using Adoption.API.Application.Exceptions;
using Adoption.API.Application.Mappers;
using Adoption.API.Application.Models;
using Adoption.API.Application.Services.Minio;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Adoption.API.Application.Commands.Animals;

public class UpdateAnimalCommandHandler(IAnimalRepository animalRepository, IMinioService minioService) : ICommandHandler<UpdateAnimalCommand,  AnimalResponse>
{
    public async Task<AnimalResponse> HandleAsync(UpdateAnimalCommand command, CancellationToken cancellationToken)
    {
        var animal = await animalRepository
            .GetAnimalByIdAsync(command.AnimalId, cancellationToken) 
                     ?? throw new AnimalNotFoundException($"No animal with id - {command.AnimalId} found");

        var principalImagePresignedUrl = await minioService.UploadBlob(command.PrincipalImage, null, cancellationToken);

        var aditionalImagesPresignedUrls = new List<string>();

        if (command.AdditionalImages != null)
        {
            foreach (var additionalImage in command.AdditionalImages)
            {
                aditionalImagesPresignedUrls.Add(await minioService.UploadBlob(additionalImage, null, cancellationToken));
            }
        }
        
        animal.Update(
            name: command.Name ?? animal.Name,
            age: command.Age ?? animal.Age,
            description: command.Description ?? animal.Description,
            breed: command.Breed ?? animal.Breed,
            species: command.Species ?? animal.Species,
            principalImage: principalImagePresignedUrl ?? animal.PrincipalImage,
            aditionalImages: aditionalImagesPresignedUrls.Any() ? aditionalImagesPresignedUrls : animal.AdditionalImagesUrl?.ToList()
        );

        await animalRepository.UnitOfWork().SaveChangesAsync(cancellationToken);
        return animal.MapToResponse();
    }
}
