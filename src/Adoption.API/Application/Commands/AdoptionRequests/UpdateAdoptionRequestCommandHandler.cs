using Adoption.API.Abstractions;
using Adoption.API.Application.Exceptions;
using Adoption.API.Application.Mappers;
using Adoption.API.Application.Models;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Adoption.API.Application.Commands.AdoptionRequests;

public class UpdateAdoptionRequestCommandHandler(IAdoptionRequestRepository adoptionRepository)
    : ICommandHandler<UpdateAdoptionRequestCommand, AdoptionResponse>
{
    public async Task<AdoptionResponse> HandleAsync(UpdateAdoptionRequestCommand command, CancellationToken cancellationToken)
    {
        var adoptionRequest = await adoptionRepository
            .GetByIdAsync(command.Id, cancellationToken) 
                              ??  throw new AdoptionRequestNotFoundException($"Adoption request with id - {command.Id} not found");
        
        if(command.Status ==  AdoptionStatus.Approved)
            adoptionRequest.Approve();
        
        if(command.Status ==  AdoptionStatus.Completed)
            adoptionRequest.Complete();
        
        if(command.Status ==  AdoptionStatus.Rejected)
            adoptionRequest.Reject();
        
        await adoptionRepository.UnitOfWork().SaveEntitiesAsync(cancellationToken);
        
        return adoptionRequest.MapToResponse();
    }
}
