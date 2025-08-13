using Microsoft.AspNetCore.Components.Forms;

namespace Shared.Dtos;

public record CreateAnimalDto(
    string OwnerId,
    string Name,
    int Age,
    string Breed,
    string Type,
    string Description);
