namespace Adoption.API.Application.Commands.AdoptionRequests;

public record CreateAdoptionRequest(
    Guid AnimalId,
    string Comments);
