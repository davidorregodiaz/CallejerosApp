

using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Adoption.Domain.SeedWork;

namespace Adoption.Domain.Events.Adoption;

public record AdoptionStatusChangeDomainEvent(
    AdoptionStatus Status,
    Guid RequesterId,
    Guid AdoptionRequestId) : IDomainEvent;
