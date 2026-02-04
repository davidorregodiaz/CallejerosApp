namespace Adoption.API.Application.Commands.Appointments;

public record DeleteAppointmentReques(
    Guid AppointmentId,
    Guid AdoptionRequestId);
