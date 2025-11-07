using Adoption.API.Abstractions;
using Adoption.API.Application.Models;

namespace Adoption.API.Application.Commands.Animals;

public record UpdateAnimalCommand(
    Guid AnimalId,
    string? Name,
    string? Species,
    string? Breed,
    int? Age,
    string? Description,
    IFormFile? PrincipalImage,
    List<IFormFile>? AdditionalImages) : ICommand<AnimalResponse>;

public record UpdateAnimalRequest()
{ 
    public string? Name { get; init; } 
    public int? Age { get; init; }
    public string? Breed { get; init; } 
    public string? Species { get; init; } 
    public string? Description { get; init; } 
    public IFormFile? PrincipalImage { get; init; } 
    public List<IFormFile>? AdditionalImages { get; init; }
}
