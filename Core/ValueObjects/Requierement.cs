using Shared;

public sealed class Requierement : IEquatable<Requierement>
{
    public string Value { get; }

    private Requierement(string value)
    {
        Value = value;
    }

    public static TaskResult<Requierement> Create(string value)
    {
        if (string.IsNullOrEmpty(value))
            return TaskResult<Requierement>.FromFailure("Requierement can not be empty");

        if (value.Length > 50)
            return TaskResult<Requierement>.FromFailure("Requierement too long, maybe you can try split it");

        if (value.Length < 10)
            return TaskResult<Requierement>.FromFailure("Requierement need to be more descriptive");

        return TaskResult<Requierement>.FromData(new Requierement(value.Trim().ToLowerInvariant()));
    }

    public bool Equals(Requierement? other) => other is not null && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    public override bool Equals(object? obj) => obj is Requierement other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
}