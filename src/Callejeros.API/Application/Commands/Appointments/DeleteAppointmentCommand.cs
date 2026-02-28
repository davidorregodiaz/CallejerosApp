using Adoption.API.Abstractions;

namespace Adoption.API.Application.Commands.Appointments;

public record DeleteAppointmentCommand(
    Guid AppointmentId,
    Guid AdoptionRequestId) : ICommand;
