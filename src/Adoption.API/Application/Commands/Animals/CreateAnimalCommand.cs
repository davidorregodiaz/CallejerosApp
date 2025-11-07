using Adoption.API.Abstractions;
using Adoption.API.Application.Models;

namespace Adoption.API.Application.Commands.Animals;

public record  CreateAnimalCommand : ICommand<AnimalResponse>
{
    public Guid OwnerId { get; init; }
    public string Name { get; init; } = null!;
    public int Age { get; init; }
    public string Breed { get; init; } = null!;
    public string Species { get; init; } = null!;
    public string Description { get; init; } = null!;
    public IFormFile PrincipalImage { get; init; } = null!;
    public List<IFormFile>? AdditionalImages { get; init; }
}

