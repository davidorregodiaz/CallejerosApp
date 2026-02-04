using Adoption.API.Abstractions;

namespace Adoption.API.Application.Commands.Appointments;

public record ScheduleAppointmentCommand(
    Guid AdoptionRequestId,
    Guid AppointmentId) : ICommand;
