using Adoption.API.Application.Exceptions;
using Adoption.API.Application.Queues;
using Adoption.API.Application.Services.Email;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Domain.AggregatesModel.UserAggregate;
using Adoption.Domain.Events.Adoption;
using Adoption.Domain.SeedWork;
using Adoption.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Adoption.API.Application.Events.Handlers;

public class AdoptionRequestCreatedDomainEventHandler(
    IEmailQueue emailQueue, 
    UserManager<ApplicationUser> userManager,
    AdoptionDbContext ctx)
    : IDomainEventHandler<AdoptionRequestCreatedDomainEvent>
{
    public async Task HandleAsync(AdoptionRequestCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var animal = await ctx.Animals
            .SingleOrDefaultAsync(x => x.Id ==  new AnimalId(domainEvent.AnimalId), cancellationToken);

        if (animal is null)
            throw new AnimalNotFoundException($"Animal with id - {domainEvent.AnimalId} not found");
        
        var user = await userManager.FindByIdAsync(animal.OwnerId.Value.ToString());
        
        if(user is null)
            throw new UserNotFoundException($"User with id - {domainEvent.RequesterId} not found");
        
        var emailRequest = new EmailRequest(
            To: user.Email!,
            Subject: "Adoption request created",
            Data: new Dictionary<string, string>()
            {
                ["RequesterName"] = user.UserName!, 
                ["AnimalBreed"] =  animal.Localization,
                ["AnimalName"] =  animal.Name,
                ["RequestDate"] = domainEvent.OcurredOn.ToString("yyyy-MM-dd HH:mm:ss"),
            },
            TemplateType: EmailTemplateType.AdoptionRequestCreated);
        
        await emailQueue.EnqueueAsync(emailRequest);
    }
}
