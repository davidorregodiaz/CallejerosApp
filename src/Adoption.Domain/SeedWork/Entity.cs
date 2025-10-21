
namespace Adoption.Domain.SeedWork;

public abstract class Entity
{
    protected Entity(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; private init; }

    private List<IDomainEvent>? _domainEvents;
    public IReadOnlyCollection<IDomainEvent>? DomainEvents => _domainEvents?.AsReadOnly();
    public void AddDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents = _domainEvents ?? new List<IDomainEvent>();
        _domainEvents.Add(eventItem);
    }
    public void RemoveDomainevent(IDomainEvent eventItem)
    {
        _domainEvents?.Remove(eventItem);
    }
    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }

    public bool IsTransient()
    {
        return Id == default;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj is not Entity)
            return false;
        var other = (Entity)obj;
        if (IsTransient() || other.IsTransient())
            return false;
        if (GetType() != obj.GetType())
            return false;
        if (!ReferenceEquals(this, other))
            return false;

        return Id == other.Id;
    }
    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            return Id.GetHashCode() ^ 31; // Using a prime number to reduce hash collisions
        }
        else
        {
            return base.GetHashCode();
        }
    }

    public static bool operator ==(Entity left, Entity right)
    {
        return left is not null && right is not null && left.Equals(right);
    }

    public static bool operator !=(Entity left, Entity right)
    {
        return !(left == right);
    }

}
