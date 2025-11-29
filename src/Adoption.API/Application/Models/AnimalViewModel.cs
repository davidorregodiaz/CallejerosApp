using Adoption.API.Application.Commands.Animals;
using Adoption.Domain.AggregatesModel.AnimalAggregate;

namespace Adoption.API.Application.Models;

public record AnimalViewModel(
    Guid Id,
    Guid OwnerId,
    string Name,
    string Species,
    string Breed,
    int Age,
    string Description,
    string PrincipalImageUrl,
    IReadOnlyCollection<string>? ExtraImagesUrls,
    MedicalViewModel MedicalRecord,
    IReadOnlyCollection<string> AdoptionRequirements,
    IReadOnlyCollection<Compatibility> Compatibility,
    IReadOnlyCollection<Personality> Personality,
    Sex Sex,
    Size Size,
    AnimalStatus Status);
