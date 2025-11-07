using Adoption.Domain.Exceptions.Animal;
using Core.SeedWork;

namespace Adoption.Domain.AggregatesModel.AnimalAggregate;

public class AnimalType : ValueObject
{
    public string Value { get; }
    private const int MaxLength = 20;
    private const int MinLength = 3;

    private AnimalType(string value)
    {
        Value = value;
    }
    private AnimalType()
    {
        
    }
    public static AnimalType Create(string? value)
    {
        if (string.IsNullOrEmpty(value))
            throw new AnimalDomainException("Type must not be empty");

        if (value.Length > MaxLength)
            throw new AnimalDomainException("No such Animal Type");

        if (value.Length < MinLength)
            throw new AnimalDomainException("No such Animal Type");

        return new AnimalType(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
