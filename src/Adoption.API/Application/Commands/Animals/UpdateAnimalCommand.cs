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
    List<IFormFile>? AdditionalImages) : ICommand<AnimalViewModel>;
