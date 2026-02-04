namespace Adoption.API.Application.Commands.Appointments;

public sealed record CreateAppointmentRequest(
    DateTime Date,
    string Notes,
    string Location);
