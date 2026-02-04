using Adoption.API.Abstractions;

namespace Adoption.API.Application.Commands.Appointments;

public record CancelAppointmentCommand(
    Guid AdoptionRequestId,
    Guid AppointmentId) : ICommand;
