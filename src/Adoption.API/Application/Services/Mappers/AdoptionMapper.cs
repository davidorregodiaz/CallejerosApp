using Adoption.API.Application.Exceptions;
using Adoption.API.Application.Models;
using Adoption.API.Application.Models.User;
using Adoption.API.Application.Services.Minio;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Domain.AggregatesModel.UserAggregate;
using Microsoft.AspNetCore.Identity;

namespace Adoption.API.Application.Services.Mappers;

public class AdoptionMapper(
    IAnimalRepository animalRepository,
    UserManager<ApplicationUser> userManager,
    IMinioService minioService) : IAdoptionMapper
{
    public async Task<AdoptionViewModel> MapToResponseAsync(AdoptionRequest adoptionRequest,
        CancellationToken cancellationToken)
    {
        var animal = await animalRepository.GetAnimalByIdAsync(adoptionRequest.AnimalId, cancellationToken);
        var user = await userManager.FindByIdAsync(adoptionRequest.RequesterId.ToString());

        if (animal is null)
            throw new AnimalNotFoundException($"Animal with id  {adoptionRequest.AnimalId} not found");

        if (user is null)
            throw new UserNotFoundException($"User with id  {adoptionRequest.RequesterId} not found");

        return new AdoptionViewModel(
            AdoptionRequestId: adoptionRequest.Id.Value,
            Requester: new UserViewModel(
                Id: Guid.Parse(user.Id),
                Username: user.UserName!,
                Email: user.Email!,
                JoinedAt: user.JoinedAt,
                ImageUrl: string.Empty,
                Roles: []),
            RequestDate: adoptionRequest.RequestDate,
            Status: adoptionRequest.Status,
            Comments: adoptionRequest.Comments,
            Animal: new AnimalViewModel(
                Id: animal.Id.Value,
                OwnerId: animal.OwnerId.Value,
                Name: animal.Name,
                Species: animal.Species,
                Breed: animal.Localization,
                Age: animal.Age,
                Description: animal.Description,
                MedicalRecord: new MedicalViewModel(
                    Vaccine: animal.MedicalRecord.Vaccine,
                    IsStirilized: animal.MedicalRecord.IsStirilized,
                    IsDewormed: animal.MedicalRecord.IsDewormed,
                    HealthState: animal.MedicalRecord.HealthState),
                AdoptionRequirements: animal.AdoptionRequirements,
                PrincipalImageUrl: await minioService.PresignedGetUrl(animal.PrincipalImage, cancellationToken),
                Size: animal.Size,
                Personality: animal.Personality,
                Compatibility: animal.Compatibility,
                Sex: animal.Sex,
                Status: animal.Status,
                ExtraImagesUrls: []), 
            Appointments: adoptionRequest.Appointments.Select(a => new AppointmentViewModel(
                AppointmentId: a.Id.Value,
                Status: a.Status,
                Date: a.Date,
                Location: a.Location,
                Notes: a.Notes,
                DateProposed: a.DateProposed,
                RescheduleMessage: a.RescheduleMessage)).ToList().AsReadOnly());
    }
}
