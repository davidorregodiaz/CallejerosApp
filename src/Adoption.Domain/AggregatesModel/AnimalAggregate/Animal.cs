using Adoption.Domain.SeedWork;
using Adoption.Domain.Events.Animal;
using Adoption.Domain.Exceptions.Animal;

namespace Adoption.Domain.AggregatesModel.AnimalAggregate;

public sealed class Animal
    : Entity, IAggregateRoot
{
    private Animal() : base(Guid.Empty) {} //EF
    
    private Animal(AnimalId id, string name, int age, string description, OwnerId ownerId, string breed, string species, List<string>? imagesPath, string principalImage)
        : base(id.Value)
    {
        Id = id;
        Name = name;
        NormalizedName = name.Trim().ToLower();
        Age = age;
        Description = description;
        OwnerId = ownerId;
        Breed = breed;
        NormalizedBreed = breed.Trim().ToLower();
        NormalizedSpecies = species.Trim().ToLower();
        Species = species;
        _aditionalImages = imagesPath ?? new List<string>();
        PrincipalImage = principalImage;
    }

    public new AnimalId Id { get; private set; }
    public string Name { get; private set; }
    public string NormalizedName { get; private set; }
    public int Age { get; private set; }
    public string Breed { get; private set; }
    public string NormalizedBreed { get; private set; }
    public string Species { get; private set; }
    public string NormalizedSpecies { get; private set; }
    public string Description { get; private set; }
    public OwnerId OwnerId { get; private set; }
    private List<string>? _aditionalImages;
    public IReadOnlyCollection<string>? AdditionalImagesUrl => _aditionalImages?.AsReadOnly<string>();
    public string PrincipalImage { get; private set; } = null!;

    public static Animal Create(string name, int age, string description, Guid ownerId, string breed, string species, List<string>? aditionalImages, string principalImage)
    {
        if (age <= 0)
            throw new AnimalDomainException($"Invalid {nameof(age)}.");

        var animal = new Animal(
            id: new AnimalId(Guid.NewGuid()),
            name: name,
            age: age,
            description: description,
            ownerId: new OwnerId(ownerId),
            breed: breed,
            species: species,
            imagesPath: aditionalImages,
            principalImage: principalImage
        );

        animal.AddDomainEvent(new AnimalCreatedDomainEvent(animal.Id.Value));

        return animal;
    }
 
    public Animal Update(string? name, int? age, string? description, string? breed, string? species, List<string>? aditionalImages, string? principalImage)
    {
        if (age <= 0)
            throw new AnimalDomainException($"Invalid {nameof(age)}.");

        if (!string.IsNullOrEmpty(name))
        {
            Name = name;
            NormalizedName = name.Trim().ToLower();
        }

        if (age.HasValue && age.Value > 0)
            Age = age.Value;
        
        if(!string.IsNullOrEmpty(description))
            Description = description;
        
        if(!string.IsNullOrEmpty(breed))
            Breed = breed;
        
        if(!string.IsNullOrEmpty(species))
            Species = species;
        
        if(aditionalImages != null)
            _aditionalImages =  aditionalImages;
        
        if(!string.IsNullOrEmpty(principalImage))
            PrincipalImage = principalImage;
        
        return this;
    }
}


public record AnimalId(Guid Value);
public record OwnerId(Guid Value);
