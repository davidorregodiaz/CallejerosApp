namespace Adoption.API.Application.Commands.Appointments;

public sealed record CreateAppointmentRequest(
    Guid AdoptionRequestId,
    DateTime Date,
    string Notes,
    string Location);
