namespace Application.Dtos;

public record ResponseAnimalDto(
    string Id,
    string OwnerId,
    string Name,
    int Age,
    string Breed,
    string Type,
    List<string> Requierements 
){}
