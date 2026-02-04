using Adoption.API.Abstractions;
using Adoption.API.Application.Exceptions;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Commands.AdoptionRequests;

public class CompleteAppointmentCommnadHandler(IAdoptionRequestRepository adoptionRequestRepository) : ICommandHandler<CompleteAppointmentCommand>
{
    public async Task HandleAsync(CompleteAppointmentCommand command, CancellationToken cancellationToken)
    {
        var adoptionRequest =
            await adoptionRequestRepository.GetByIdAsync(command.AdoptionRequestId, cancellationToken);

        if (adoptionRequest is null)
            throw new AdoptionRequestNotFoundException(
                $"Adoption request with id - {command.AdoptionRequestId} not found");

        adoptionRequest.CompleteAppointment(command.AppointmentId);

        await adoptionRequestRepository.UnitOfWork().SaveChangesAsync(cancellationToken);
    }
}
