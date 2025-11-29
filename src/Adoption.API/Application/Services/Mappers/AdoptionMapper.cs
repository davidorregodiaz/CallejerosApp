using Adoption.API.Application.Exceptions;
using Adoption.API.Application.Models;
using Adoption.API.Application.Services.Minio;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Domain.AggregatesModel.UserAggregate;
using Microsoft.AspNetCore.Identity;

namespace Adoption.API.Application.Services.Mappers;

public class AdoptionMapper(IAnimalRepository animalRepository, UserManager<ApplicationUser> userManager, IMinioService minioService) : IAdoptionMapper
{
    public async Task<AdoptionViewModel> MapToResponseAsync(AdoptionRequest adoptionRequest, CancellationToken cancellationToken)
    {
        var animal = await animalRepository.GetAnimalByIdAsync(adoptionRequest.AnimalId, cancellationToken);
        var user = await userManager.FindByIdAsync(adoptionRequest.RequesterId.ToString());
        
        if(animal is null)
            throw new AnimalNotFoundException($"Animal with id  {adoptionRequest.AnimalId} not found");
        
        if(user is null)
            throw new UserNotFoundException($"User with id  {adoptionRequest.RequesterId} not found");

        return new AdoptionViewModel(
            AdoptionRequestId: adoptionRequest.Id.Value,
            RequesterId: adoptionRequest.RequesterId,
            RequestDate: adoptionRequest.RequestDate,
            Status: adoptionRequest.Status,
            Comments: adoptionRequest.Comments,
            AnimalName: animal.Name,
            AnimalImage: await minioService.PresignedGetUrl(animal.PrincipalImage, cancellationToken),
            RequesterName: user.UserName);
    }
}
