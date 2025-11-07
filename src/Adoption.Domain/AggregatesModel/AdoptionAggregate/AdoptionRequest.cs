
using Adoption.Domain.SeedWork;
using Adoption.Domain.Events.Adoption;
using Adoption.Domain.Exceptions.Adoption;

namespace Adoption.Domain.AggregatesModel.AdoptionAggregate;

public sealed class AdoptionRequest
    : Entity, IAggregateRoot
{
    private AdoptionRequest(AdoptionRequestId id,Guid animalId, Guid requesterId, string? comments) : base(id.Value)
    {
        Id = id;
        AnimalId = animalId;
        RequesterId = requesterId;
        Comments = comments;
        Status = AdoptionStatus.Pending;
        RequestDate = DateTime.UtcNow;
    }
    public new AdoptionRequestId Id { get; private set; }
    public Guid AnimalId { get; private set; }
    public Guid RequesterId { get; private set; }
    public DateTime RequestDate { get; private set; }
    public AdoptionStatus Status { get; private set; }
    public string? Comments { get; private set; }

    public static AdoptionRequest Create(string animalId, string requesterId, string? comments)
    {
        if (!Guid.TryParse(animalId, out var animalIdGuid))
            throw new AdoptionDomainException($"Invalid {nameof(animalId)} format.");

        if (!Guid.TryParse(requesterId, out var requesterIdGuid))
            throw new AdoptionDomainException($"Invalid {nameof(requesterId)} format.");

        var adoptionRequest = new AdoptionRequest(new AdoptionRequestId(Guid.NewGuid()), animalIdGuid, requesterIdGuid, comments);

        adoptionRequest.AddDomainEvent(new AdoptionRequestCreatedDomainEvent(adoptionRequest.Id.Value));

        return adoptionRequest;
    }

    public void Approve()
    {
        AddDomainEvent(new AdoptionApprovedDomainEvent());
        Status = AdoptionStatus.Approved;
    }

    public void Reject()
    {
        AddDomainEvent(new AdoptionRejectedDomainEvent());
        Status = AdoptionStatus.Rejected;
    }

    public void Complete()
    {
        AddDomainEvent(new AdoptionCompletedDomainEvent());
        Status = AdoptionStatus.Completed;
    }
}
public record AdoptionRequestId(Guid Value);
