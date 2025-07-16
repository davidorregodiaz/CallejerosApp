namespace Application.Dtos;

public record CreateAnimalDto(
    string OwnerId,
    string Name,
    int Age,
    string Breed,
    string Type,
    List<string> Requierements 
){}
