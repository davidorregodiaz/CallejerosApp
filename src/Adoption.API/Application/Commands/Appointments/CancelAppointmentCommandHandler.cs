using Adoption.API.Abstractions;
using Adoption.API.Application.Exceptions;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Commands.Appointments;

public class CancelAppointmentCommandHandler(IAdoptionRequestRepository adoptionRequestRepository) : ICommandHandler<CancelAppointmentCommand>
{
    public async Task HandleAsync(CancelAppointmentCommand command, CancellationToken cancellationToken)
    {
        var adoptionRequest =
            await adoptionRequestRepository.GetByIdAsync(command.AdoptionRequestId, cancellationToken);

        if (adoptionRequest is null)
            throw new AdoptionRequestNotFoundException(
                $"Adoption request with id - {command.AdoptionRequestId} not found");

        adoptionRequest.CancelAppointment(command.AppointmentId);

        await adoptionRequestRepository.UnitOfWork().SaveChangesAsync(cancellationToken);
    }
}
