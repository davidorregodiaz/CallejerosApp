using Adoption.API.Abstractions;
using Adoption.API.Application.Exceptions;
using Adoption.API.Application.Models;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Commands.Appointments;

public class RescheduleAppointmentCommandHandler(IAdoptionRequestRepository adoptionRequestRepository) : ICommandHandler<RescheduleAppointmentCommand>
{
    public async Task HandleAsync(RescheduleAppointmentCommand command, CancellationToken cancellationToken)
    {
        var adoptionRequest =
            await adoptionRequestRepository.GetByIdAsync(command.AdoptionRequestId, cancellationToken);

        if (adoptionRequest is null)
            throw new AdoptionRequestNotFoundException(
                $"Adoption request with id - {command.AdoptionRequestId} not found");
        
        adoptionRequest.RescheduleAppointment(command.AppointmentId, command.DateProposed, command.RescheduleMessage);

        await adoptionRequestRepository.UnitOfWork().SaveChangesAsync(cancellationToken);
    }
}
