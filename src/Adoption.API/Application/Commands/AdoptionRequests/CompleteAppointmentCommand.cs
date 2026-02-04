using Adoption.API.Abstractions;

namespace Adoption.API.Application.Commands.AdoptionRequests;

public record CompleteAppointmentCommand(
    Guid AdoptionRequestId,
    Guid AppointmentId) : ICommand;
