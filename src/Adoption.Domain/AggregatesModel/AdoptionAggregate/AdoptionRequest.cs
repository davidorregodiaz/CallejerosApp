using Adoption.Domain.SeedWork;
using Adoption.Domain.Events.Adoption;
using Adoption.Domain.Exceptions.Adoption;

namespace Adoption.Domain.AggregatesModel.AdoptionAggregate;

public sealed class AdoptionRequest
    : Entity, IAggregateRoot
{
    private AdoptionRequest(AdoptionRequestId id,Guid animalId, Guid requesterId, string comments) : base(id.Value)
    {
        Id = id;
        AnimalId = animalId;
        RequesterId = requesterId;
        Comments = comments;
        Status = AdoptionStatus.Pending;
        RequestDate = DateTime.UtcNow;
        
        AddDomainEvent(new AdoptionRequestCreatedDomainEvent(RequesterId, AnimalId, RequestDate));
    }
    public new AdoptionRequestId Id { get; private set; }
    public Guid AnimalId { get; private set; }
    public Guid RequesterId { get; private set; }
    public DateTime RequestDate { get; private set; }
    public AdoptionStatus Status { get; private set; }
    public string Comments { get; private set; }
    public static AdoptionRequest Create(Guid animalId, Guid requesterId, string comments) => 
        new AdoptionRequest(new AdoptionRequestId(Guid.NewGuid()), animalId, requesterId, comments);
    public void Approve()
    {
        Status = AdoptionStatus.Approved;
        AddDomainEvent(new AdoptionStatusChangeDomainEvent(Status, RequesterId, Id.Value));
    }
    public void Reject()
    {
        Status = AdoptionStatus.Rejected;
        AddDomainEvent(new AdoptionStatusChangeDomainEvent(Status, RequesterId, Id.Value));
    }
    public void Complete()
    {
        Status = AdoptionStatus.Completed;
        AddDomainEvent(new AdoptionStatusChangeDomainEvent(Status, RequesterId, Id.Value));
    }
}
public record AdoptionRequestId(Guid Value);
