namespace Adoption.API.Application.Commands.Animals;

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
