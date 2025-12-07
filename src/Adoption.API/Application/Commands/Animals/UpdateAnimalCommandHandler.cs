using Adoption.API.Abstractions;
using Adoption.API.Application.Exceptions;
using Adoption.API.Application.Models;
using Adoption.API.Application.Services.Mappers;
using Adoption.API.Application.Services.Minio;
using Adoption.Domain.AggregatesModel.AnimalAggregate;

namespace Adoption.API.Application.Commands.Animals;

public class UpdateAnimalCommandHandler(
    IAnimalRepository animalRepository, 
    IMinioService minioService,
    IAnimalMapper animalMapper,
    ILogger<UpdateAnimalCommandHandler> logger) : ICommandHandler<UpdateAnimalCommand,  AnimalViewModel>
{
    public async Task<AnimalViewModel> HandleAsync(UpdateAnimalCommand command,
        CancellationToken cancellationToken)
    {
        var animal = await animalRepository
            .GetAnimalByIdAsync(command.AnimalId, cancellationToken) 
                     ?? throw new AnimalNotFoundException($"No animal with id - {command.AnimalId} found");

        var uploadedImages = new List<string>();
        
        try
        {
            string principalImagePath = string.Empty;
            if (command.PrincipalImage != null)
            {
                principalImagePath = await minioService.UploadBlob(command.PrincipalImage, null, cancellationToken);
                uploadedImages.Add(principalImagePath);
            }

            var additionalImagesPaths = new List<string>();

            if (command.AdditionalImages != null)
            {
                foreach (var additionalImage in command.AdditionalImages)
                {
                    var additionalImagePath = await minioService.UploadBlob(additionalImage, null, cancellationToken);
                    uploadedImages.Add(additionalImagePath);
                }
            }
        
            animal.Update(
                name: command.Name ?? animal.Name,
                age: command.Age ?? animal.Age,
                description: command.Description ?? animal.Description,
                breed: command.Breed ?? animal.Breed,
                species: command.Species ?? animal.Species,
                principalImage: principalImagePath ?? animal.PrincipalImage,
                aditionalImages: additionalImagesPaths.Any() ? additionalImagesPaths : animal.AdditionalImagesUrl?.ToList()
            );

            await animalRepository.UnitOfWork().SaveChangesAsync(cancellationToken);
            return await animalMapper.MapToResponse(animal, cancellationToken);
            
        }
        catch (Exception e)
        {
            try
            {
                if (uploadedImages.Any())
                {
                    foreach (var uploadedImage in uploadedImages)
                    {
                        await minioService.DeleteBlob(uploadedImage, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning("Error while deleting images from minio service @{Exception}", ex);
                throw;
            }
            throw;
        }
    }
}
