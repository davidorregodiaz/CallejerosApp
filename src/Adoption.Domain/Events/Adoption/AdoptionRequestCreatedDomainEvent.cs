

using Adoption.Domain.SeedWork;

namespace Adoption.Domain.Events.Adoption;

public class AdoptionRequestCreatedDomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid RequestId { get; }

    public AdoptionRequestCreatedDomainEvent(Guid requestId)
    {
        RequestId = requestId;
    }
}
