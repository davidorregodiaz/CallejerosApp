namespace Adoption.API.Application.Commands.Appointments;

public sealed record RescheduleAppointmentRequest(
    DateTime DateProposed,
    string? RescheduleMessage);
