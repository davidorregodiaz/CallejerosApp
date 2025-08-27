using System;
using MediatR;

namespace Adoption.Domain.Events.Animal;

public class AnimalCreatedDomainEvent : INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid AnimalId { get; }

    public AnimalCreatedDomainEvent(Guid animalId)
    {
        AnimalId = animalId;
    }
}
