using Adoption.API.Application.Exceptions;
using Adoption.API.Application.Queues;
using Adoption.API.Application.Services.Email;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Domain.AggregatesModel.UserAggregate;
using Adoption.Domain.Events.Adoption;
using Adoption.Domain.SeedWork;
using Adoption.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Adoption.API.Application.Events.Handlers;

public sealed class AdoptionRequestStatusChangeDomainEventHandler(
    IEmailQueue emailQueue,
    IAnimalRepository  animalRepository,
    IAdoptionRequestRepository adoptionRepository,
    UserManager<ApplicationUser> userManager)
    : IDomainEventHandler<AdoptionStatusChangeDomainEvent>
{
    public async Task HandleAsync(AdoptionStatusChangeDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var adoptionRequest = await adoptionRepository
            .GetByIdAsync(domainEvent.AdoptionRequestId, cancellationToken) 
                              ?? throw new AdoptionRequestNotFoundException($"Adoption request with id - {domainEvent.AdoptionRequestId} not found");

        var animal = await animalRepository
            .GetAnimalByIdAsync(adoptionRequest.AnimalId,  cancellationToken) 
                     ?? throw new AnimalNotFoundException($"Animal - {adoptionRequest.AnimalId} not found");
        
        var owner = await userManager.FindByIdAsync(animal.OwnerId.ToString()) 
                    ?? throw new UserNotFoundException($"User - {animal.OwnerId} not found");
        
        var requester = await userManager.FindByIdAsync(domainEvent.RequesterId.ToString()) 
                        ?? throw new UserNotFoundException($"User - {animal.OwnerId} not found");
        
        var emailRequest = new EmailRequest(
            To: requester.Email!,
            Subject: $"Adoption request {domainEvent.Status.ToString()}",
            Data: new Dictionary<string, string>()
            {
                ["Status"] = domainEvent.Status.ToString(),
                ["OwnerName"] = owner.UserName!,
                ["AnimalName"] = animal.Name,
                ["AnimalBreed"] = animal.Localization
            },
            TemplateType: EmailTemplateType.AdoptionRequestStatusChange);

        await emailQueue.EnqueueAsync(emailRequest);
    }
}
