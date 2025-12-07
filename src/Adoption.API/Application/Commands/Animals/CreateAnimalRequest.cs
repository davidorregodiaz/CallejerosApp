using Adoption.Domain.AggregatesModel.AnimalAggregate;

namespace Adoption.API.Application.Commands.Animals;

public record CreateAnimalRequest()
{
    public string Name { get; init; } = null!;
    public int Age { get; init; }
    public Sex Sex { get; init; }
    public string Breed { get; init; } = null!;
    public string Species { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string Vaccine { get; init; } = null!;
    public bool IsStirilized { get; init; }
    public bool IsDewormed { get; init; }
    public string HealthState { get; init; } = null!;
    public List<string> Requirements { get; init; } = null!;
    public IFormFile PrincipalImage { get; init; } = null!;
    public List<IFormFile>? AdditionalImages { get; init; }
    public List<Compatibility> Compatibility { get; init; }
    public List<Personality> Personality { get; init; }
    public Size Size { get; init; }
}


