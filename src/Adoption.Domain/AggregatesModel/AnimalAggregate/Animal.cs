using System.Text.Json.Serialization;
using Adoption.Domain.SeedWork;
using Adoption.Domain.Events.Animal;
using Adoption.Domain.Exceptions.Animal;

namespace Adoption.Domain.AggregatesModel.AnimalAggregate;

public sealed class Animal
    : Entity, IAggregateRoot
{
    private Animal() : base(Guid.Empty) { } //EF

    private Animal(AnimalId id, string name, int age, string description, OwnerId ownerId, string localization, string species,
        MedicalRecord medicalRecord, List<string>? imagesPath, string principalImage, List<string> adoptionRequirements,
        Sex sex, Size size, List<Compatibility> compatibility, List<Personality> personality)
        : base(id.Value)
    {
        Id = id;
        Name = name;
        NormalizedName = name.Trim().ToLower();
        Age = age;
        Description = description;
        OwnerId = ownerId;
        Localization = localization;
        NormalizedLocalization = localization.Trim().ToLower();
        NormalizedSpecies = species.Trim().ToLower();
        Species = species;
        _aditionalImages = imagesPath ?? new List<string>();
        PrincipalImage = principalImage;
        MedicalRecord = medicalRecord;
        Sex = sex;
        Size = size;
        _compatibility = compatibility;
        _personality =  personality;
        Status = AnimalStatus.Adoption;
        SetAdoptionRequirements(adoptionRequirements);
    }

    public new AnimalId Id { get; private set; }
    public string Name { get; private set; }
    public Sex Sex { get; private set; }
    public string NormalizedName { get; private set; }
    public MedicalRecord MedicalRecord { get; private set; }
    public int Age { get; private set; }
    public AnimalStatus Status { get; private set; }
    public string Localization { get; private set; }
    public string NormalizedLocalization { get; private set; }
    public string Species { get; private set; }
    public string NormalizedSpecies { get; private set; }
    public string Description { get; private set; }
    public OwnerId OwnerId { get; private set; }
    private List<string>? _aditionalImages;
    public IReadOnlyCollection<string>? AdditionalImagesUrl => _aditionalImages?.AsReadOnly<string>();
    public string PrincipalImage { get; private set; } = null!;
    private readonly List<string> _adoptionRequirements = new List<string>();
    public IReadOnlyCollection<string> AdoptionRequirements => _adoptionRequirements.AsReadOnly();
    public Size Size { get; private set; }
    private List<Compatibility> _compatibility = new List<Compatibility>();
    public IReadOnlyCollection<Compatibility> Compatibility => _compatibility.AsReadOnly();
    private List<Personality> _personality = new List<Personality>();
    public IReadOnlyCollection<Personality> Personality => _personality.AsReadOnly();

    public static Animal Create(string name,
        int age,
        string description,
        Guid ownerId,
        string localization,
        string species,
        Sex animalSex,
        List<string>? aditionalImages,
        string principalImage,
        string vaccine,
        bool isStirilized,
        bool isDewormed,
        string healthState,
        List<string> adoptionRequirements,
        Size size,
        List<Compatibility> compatibility,
        List<Personality> personality)
    {
        if (age <= 0)
            throw new AnimalDomainException($"Invalid {nameof(age)}.");

        var animal = new Animal(
            id: new AnimalId(Guid.NewGuid()),
            name: name,
            age: age,
            description: description,
            ownerId: new OwnerId(ownerId),
            localization: localization,
            species: species,
            imagesPath: aditionalImages,
            principalImage: principalImage,
            sex: animalSex,
            medicalRecord: MedicalRecord.Create(vaccine, isStirilized, isDewormed, healthState),
            adoptionRequirements: adoptionRequirements,
            size: size,
            compatibility: compatibility,
            personality: personality
        );

        animal.AddDomainEvent(new AnimalCreatedDomainEvent(animal.Id.Value));

        return animal;
    }

    public Animal Update(string? name, int? age, string? description, string? breed, string? species,
        List<string>? aditionalImages, string? principalImage)
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

        if (!string.IsNullOrEmpty(description))
            Description = description;

        if (!string.IsNullOrEmpty(breed))
            Localization = breed;

        if (!string.IsNullOrEmpty(species))
            Species = species;

        if (aditionalImages != null)
            _aditionalImages = aditionalImages;

        if (!string.IsNullOrEmpty(principalImage))
            PrincipalImage = principalImage;

        return this;
    }

    private void SetAdoptionRequirements(List<string> requirements)
    {
        foreach (var req in requirements)
        {
            if (!string.IsNullOrWhiteSpace(req))
                _adoptionRequirements.Add(req.Trim());
        }
    }

    public void MarkAnimalAsAdopted()
    {
        Status = AnimalStatus.Adopted;
    }

    public void MarkAnimalAsInAdoptionProcess()
    {
        Status = AnimalStatus.InProcess;
    }

    public void MarkAnimalAsInAdoption()
    {
        Status = AnimalStatus.Adoption;
    }

    public void HideAnimal()
    {
        Status = AnimalStatus.Hide;
    }

}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Sex
{
    Female,
    Male
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AnimalStatus
{
    Adoption,
    Adopted,
    InProcess,
    Hide
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Size
{
    Small,
    Medium,
    Big
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Compatibility
{
    Childs,
    Dogs,
    Cats
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Personality
{
    Playful,
    Calm,
    Shy,
    Energetic,
    Warm
}

public record AnimalId(Guid Value);

public record OwnerId(Guid Value);
