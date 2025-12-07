using Adoption.API.Abstractions;
using Adoption.API.Application.Exceptions;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Adoption.API.Application.Commands.AdoptionRequests;

public class DeleteAdoptionRequestCommandHandler(IAdoptionRequestRepository adoptionRepository)
    : ICommandHandler<DeleteAdoptionRequestCommand>
{
    public async Task HandleAsync(DeleteAdoptionRequestCommand command, CancellationToken cancellationToken)
    {
        var adoptionRequest = await adoptionRepository
            .GetByIdAsync(command.Id,cancellationToken)
            ?? throw new AdoptionRequestNotFoundException($"Adoption request with id - {command.Id} not found");
        
        adoptionRepository.Delete(adoptionRequest);
        await adoptionRepository.UnitOfWork().SaveChangesAsync(cancellationToken);
    }
}
