using Adoption.Domain.SeedWork;

namespace Adoption.Domain.Events.Animal;

public class AnimalCreatedDomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid AnimalId { get; }

    public AnimalCreatedDomainEvent(Guid animalId)
    {
        AnimalId = animalId;
    }
}
