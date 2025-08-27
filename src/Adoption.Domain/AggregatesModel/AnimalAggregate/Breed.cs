using Adoption.Domain.Exceptions.Animal;

namespace Adoption.Domain.AggregatesModel.AnimalAggregate;

public sealed class Breed : IEquatable<Breed>
{
    public string Value { get; }

    private const int MaxLength = 30;
    private const int MinLength = 3;

    private Breed(string value)
    {
        Value = value;
    }

    private Breed()
    {
        
    }

    public static Breed Create(string? value)
    {
        if (string.IsNullOrEmpty(value))
            throw new AnimalDomainException("Breed can not be empty");

        if (value.Length > MaxLength)
            throw new AnimalDomainException("Breed is too long");

        if (value.Length < MinLength)
            throw new AnimalDomainException("Breed doesn't exist");

        return new Breed(value);
    }

    public bool Equals(Breed? other) => other is not null && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    public override bool Equals(object? obj) => obj is Breed other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();

}
