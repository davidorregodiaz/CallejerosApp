using System;
using MediatR;

namespace Adoption.Domain.Events.Adoption;

public class AdoptionRequestCreatedDomainEvent : INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid RequestId { get; }

    public AdoptionRequestCreatedDomainEvent(Guid requestId)
    {
        RequestId = requestId;
    }
}
