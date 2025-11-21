using Adoption.Domain.SeedWork;

namespace Adoption.Domain.Events.Adoption;

public record AdoptionRequestCreatedDomainEvent(
    Guid RequesterId,
    Guid AnimalId,
    DateTime OcurredOn) : IDomainEvent;
