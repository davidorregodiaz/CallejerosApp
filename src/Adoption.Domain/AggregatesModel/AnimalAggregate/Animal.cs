using Adoption.Domain.SeedWork;
using Adoption.Domain.Events.Animal;
using Adoption.Domain.Exceptions.Animal;

namespace Adoption.Domain.AggregatesModel.AnimalAggregate;

public sealed class Animal
    : Entity, IAggregateRoot
{
    private Animal() : base(Guid.Empty) //EF
    {
        _imagesPath = new List<string>();
    }

    
    private Animal(AnimalId id, string name, int age, string description, OwnerId ownerId, Breed breed, AnimalType type, List<string> imagesPath)
        : base(id.Value)
    {
        Id = id;
        Name = name;
        Age = age;
        Description = description;
        OwnerId = ownerId;
        _breed = breed;
        _type = type;
        _imagesPath = imagesPath ?? new List<string>();
    }

    public new AnimalId Id { get; private set; }
    public string Name { get; private set; }
    public int Age { get; private set; }
    private Breed _breed;
    public Breed Breed => _breed;
    private AnimalType _type;
    public AnimalType AnimalType => _type;
    public string Description { get; private set; }
    public OwnerId OwnerId { get; private set; }
    private List<string> _imagesPath;
    public IReadOnlyCollection<string> ImagesPath => _imagesPath.AsReadOnly<string>();
    

    public static Animal Create(string name, int age, string description, string ownerId, string? breed, string? type, List<string> imagesPath)
    {
        if (!Guid.TryParse(ownerId, out var ownerIdGuid))
            throw new AnimalDomainException($"Invalid {nameof(ownerId)} format.");

        if (age < 0)
            throw new AnimalDomainException($"Invalid {nameof(age)}.");

        if (string.IsNullOrWhiteSpace(description))
            throw new AnimalDomainException($"Invalid {nameof(description)}.");

        var animal = new Animal(
            new AnimalId(Guid.NewGuid()),
            name,
            age,
            description,
            new OwnerId(ownerIdGuid),
            Breed.Create(breed),
            AnimalType.Create(type),
            imagesPath ?? new List<string>()
        );

        animal.AddDomainEvent(new AnimalCreatedDomainEvent(animal.Id.Value));

        return animal;
    }

    public bool BelongsTo(string userId)
    {
        if (!Guid.TryParse(userId, out var userIdGuid))
            throw new AnimalDomainException($"Invalid {nameof(userId)} format.");
        return OwnerId.Value == userIdGuid;
    }
}


public record AnimalId(Guid Value);
public record OwnerId(Guid Value);
