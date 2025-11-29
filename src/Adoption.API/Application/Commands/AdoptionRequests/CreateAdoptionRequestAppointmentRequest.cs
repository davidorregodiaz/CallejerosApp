namespace Adoption.API.Application.Commands.AdoptionRequests;

public record CreateAdoptionRequestAppointmentRequest(
    Guid AdoptionRequestId,
    DateTime Date,
    string Notes,
    string Location);
