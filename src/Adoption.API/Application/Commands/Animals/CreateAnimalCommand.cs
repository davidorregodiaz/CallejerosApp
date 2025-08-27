using Adoption.API.Abstractions;
using Shared.Dtos;

namespace Adoption.API.Application.Commands.Animals;

public sealed record class CreateAnimalCommand(
    string OwnerId,
    string Name,
    int Age,
    string Breed,
    string Type,
    string Description,
    List<string> ImagePaths) : ICommand<ResponseAnimalDto>;
