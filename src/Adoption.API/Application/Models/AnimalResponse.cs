namespace Adoption.API.Application.Models;

public record AnimalResponse(
    Guid Id,
    Guid OwnerId,
    string Name,
    string Species,
    string Breed,
    int Age,
    string Description,
    string PrincipalImageUrl,
    List<string>? ExtraImagesUrls);
