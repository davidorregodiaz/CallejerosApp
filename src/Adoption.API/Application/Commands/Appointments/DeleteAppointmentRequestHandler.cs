using Adoption.API.Abstractions;
using Adoption.API.Application.Exceptions;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Adoption.Infrastructure.Context;

namespace Adoption.API.Application.Commands.Appointments;

public class DeleteAppointmentRequestHandler(AdoptionDbContext ctx, IAdoptionRequestRepository adoptionRequestRepository) : ICommandHandler<DeleteAppointmentCommand>
{
    public async Task HandleAsync(DeleteAppointmentCommand command, CancellationToken cancellationToken)
    {
        var adoptionRequest = await adoptionRequestRepository.GetByIdAsync(command.AdoptionRequestId, cancellationToken);

        if (adoptionRequest is null)
            throw new AdoptionRequestNotFoundException($"Adoption request with id -  {command.AdoptionRequestId} not found");
        
        adoptionRequest.RemoveAppointment(command.AdoptionRequestId);
        
        await adoptionRequestRepository.UnitOfWork().SaveChangesAsync(cancellationToken);
    }
}
