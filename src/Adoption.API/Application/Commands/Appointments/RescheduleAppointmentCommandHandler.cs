using Adoption.API.Abstractions;
using Adoption.API.Application.Models;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Commands.Appointments;

public class RescheduleAppointmentCommandHandler(IAdoptionRequestRepository adoptionRequestRepository) : ICommandHandler<RescheduleAppointmentCommand, AppointmentViewModel>
{
    public async Task<AppointmentViewModel> HandleAsync(RescheduleAppointmentCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
