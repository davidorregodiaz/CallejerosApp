namespace Shared.Dtos;

public record ResponseAnimalDto(
    string Id,
    string OwnerId,
    string Name,
    int Age,
    string Breed,
    string Type,
    ICollection<string> Images,
    List<string> Requierements);