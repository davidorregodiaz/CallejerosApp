using Adoption.API.Abstractions;
using Adoption.API.Application.Exceptions;
using Adoption.API.Application.Models;
using Adoption.API.Application.Services.Mappers;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Commands.AdoptionRequests;

public class UpdateAdoptionRequestCommandHandler(IAdoptionRequestRepository adoptionRepository, IAdoptionMapper adoptionMapper)
    : ICommandHandler<UpdateAdoptionRequestCommand, AdoptionViewModel>
{
    public async Task<AdoptionViewModel> HandleAsync(UpdateAdoptionRequestCommand command,
        CancellationToken cancellationToken)
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
        
        return await adoptionMapper.MapToResponseAsync(adoptionRequest, cancellationToken);
    }
}
