using Adoption.API.Abstractions;
using Adoption.API.Application.Exceptions;
using Adoption.API.Application.Mappers;
using Adoption.API.Application.Models;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Domain.AggregatesModel.UserAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Adoption.API.Application.Commands.AdoptionRequests;

public class CreateAdoptionRequestCommandHandler(
    IAnimalRepository animalRepository,
    IAdoptionRequestRepository adoptionRepository,
    UserManager<ApplicationUser>  userManager)
    : ICommandHandler<CreateAdoptionRequestCommand, AdoptionResponse>
{
    public async Task<AdoptionResponse> HandleAsync(CreateAdoptionRequestCommand command, CancellationToken cancellationToken)
    {
        var animal = await animalRepository
            .GetAnimalByIdAsync(command.AnimalId, cancellationToken) 
                     ?? throw new AnimalNotFoundException($"Animal with id - {command.AnimalId} not found");

        var user = await userManager
            .FindByIdAsync(command.RequesterId.ToString()) 
                   ??  throw new UserNotFoundException($"User with id - {command.RequesterId} not found");
        
        var userHaveAssociatedRequests =
            await adoptionRepository.HaveAssociatedRequestsAsync(Guid.Parse(user.Id), cancellationToken);

        if (userHaveAssociatedRequests)
            throw new UserIsAlreadyInAnAdoptionProcess("The user is still in an adoption process");

        var adoptionRequest = AdoptionRequest.Create(
            animalId: command.AnimalId,
            requesterId: command.RequesterId,
            comments: command.Comments);
        
        adoptionRepository.Add(adoptionRequest);
        await adoptionRepository.UnitOfWork().SaveEntitiesAsync(cancellationToken);
        
        return adoptionRequest.MapToResponse();
    }
}
