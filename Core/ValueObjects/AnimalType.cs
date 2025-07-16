using System;
using Shared;

namespace Core.ValueObjects;

public class AnimalType : IEquatable<AnimalType>
{
    public string Value { get; }

    private AnimalType(string value)
    {
        Value = value;
    }

    public static TaskResult<AnimalType> Create(string value)
    {
        if (string.IsNullOrEmpty(value))
            return TaskResult<AnimalType>.FromFailure("Type must not be empty");

        if (value.Length > 20)
            return TaskResult<AnimalType>.FromFailure("No such Animal Type");

        if (value.Length < 3)
            return TaskResult<AnimalType>.FromFailure("No such Animal Type");

        return TaskResult<AnimalType>.FromData(new AnimalType(value));
    }

    public bool Equals(AnimalType? other) => other is not null && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    public override bool Equals(object? obj) => obj is AnimalType animal && Equals(animal);
    public override int GetHashCode() => Value.GetHashCode();
}
