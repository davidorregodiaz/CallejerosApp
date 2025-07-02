
using Shared;

namespace Core.ValueObjects;

public sealed class Breed : IEquatable<Breed>
{
    public string Value { get; }

    private Breed(string value)
    {
        Value = value;
    }


    public static TaskResult<Breed> Create(string value)
    {
        if (string.IsNullOrEmpty(value))
            return TaskResult<Breed>.FromFailure("Breed can not be empty");

        if (value.Length > 30)
            return TaskResult<Breed>.FromFailure("Breed doesn't exist");

        if (value.Length < 3)
            return TaskResult<Breed>.FromFailure("Breed doesn't exist");

        return TaskResult<Breed>.FromData(new Breed(value));
    }

    public bool Equals(Breed? other) => other is not null && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    public override bool Equals(object? obj) => obj is Breed other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();

}