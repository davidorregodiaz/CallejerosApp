
using Adoption.API.Abstractions;
using Adoption.API.Application.Models;
using Adoption.API.Application.Services.Mappers;
using Adoption.API.Application.Services.Minio;
using Adoption.API.Extensions;
using Adoption.Domain.AggregatesModel.AnimalAggregate;

namespace Adoption.API.Application.Commands.Animals;

public class CreateAnimalCommandHandler(
    IAnimalRepository animalRepository, 
    ILogger<CreateAnimalCommandHandler> logger, 
    IMinioService minioService, 
    IAnimalMapper animalMapper) : ICommandHandler<CreateAnimalCommand, AnimalViewModel>
{
    public async Task<AnimalViewModel> HandleAsync(
        CreateAnimalCommand command,
        CancellationToken cancellationToken)
    {
        var uploadedImages = new List<string>();
        try
        {
            string principalImagePath = string.Empty;
            var additionalImagesPaths = new List<string>();
            
            
            if (command.PrincipalImage.Length > 0)
            {
                principalImagePath = await minioService.UploadBlob(command.PrincipalImage,null, cancellationToken);
                uploadedImages.Add(principalImagePath);                
            }
            
            if (command.AdditionalImages != null)
            {
                foreach (var additionalImage in command.AdditionalImages)
                {
                    var additionalImagePath = await minioService.UploadBlob(additionalImage, null, cancellationToken);
                    uploadedImages.Add(additionalImagePath);
                    additionalImagesPaths.Add(additionalImagePath);
                }
            }
            
            var animal = Animal.Create(
                name: command.Name,
                age: command.Age,
                description: command.Description,
                ownerId: command.OwnerId,
                localization: command.Localization,
                species: command.Species,
                aditionalImages:additionalImagesPaths,
                principalImage: principalImagePath,
                isDewormed: command.IsDewormed,
                isStirilized: command.IsSterilized,
                healthState: command.HealthState,
                vaccine: command.Vaccine.NormalizeVaccines(),
                animalSex: command.Sex,
                adoptionRequirements:  (command.Requirements ?? new List<string>())
                .SelectMany(r => r.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(r => r.CapitalizeFirstWord())
                .ToList(),
                compatibility:  command.Compatibility,
                personality:  command.Personality,
                size: command.Size
            );
            
            logger.LogInformation("Creating Animal - Animal : {@Animal}", animal);

            animalRepository.Add(animal);
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
            catch(Exception ex)
            {
                logger.LogWarning("Error while deleting images from minio service @{Exception}", ex);
            }
            
            throw;
        }
    }
}
