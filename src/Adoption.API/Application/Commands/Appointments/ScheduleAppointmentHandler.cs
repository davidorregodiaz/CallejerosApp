using Adoption.API.Abstractions;
using Adoption.API.Application.Exceptions;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Commands.Appointments;

public class ScheduleAppointmentHandler(IAdoptionRequestRepository adoptionRequestRepository)
    : ICommandHandler<ScheduleAppointmentCommand>
{
    public async Task HandleAsync(ScheduleAppointmentCommand command, CancellationToken cancellationToken)
    {
        var adoptionRequest =
            await adoptionRequestRepository.GetByIdAsync(command.AdoptionRequestId, cancellationToken);

        if (adoptionRequest is null)
            throw new AdoptionRequestNotFoundException(
                $"Adoption request with id - {command.AdoptionRequestId} not found");

        adoptionRequest.ScheduleAppointment(command.AppointmentId);

        await adoptionRequestRepository.UnitOfWork().SaveChangesAsync(cancellationToken);
    }
}
