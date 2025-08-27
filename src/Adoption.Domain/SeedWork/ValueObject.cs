using System;

namespace Core.SeedWork;

public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object> GetEqualityComponents();
    public bool Equals(ValueObject? other)
    {
        return other is not null && ValuesAreEquals(other);
    }
    
    public override bool Equals(object? other)
    {
        return other is ValueObject otherValueObject && ValuesAreEquals(otherValueObject);
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => HashCode.Combine(x, y));
    }

    private bool ValuesAreEquals(ValueObject other)
    {
        return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    
}
