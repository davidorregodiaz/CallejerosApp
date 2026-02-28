using Adoption.API.Abstractions;

namespace Adoption.API.Application.Commands.Animals;

public record DeleteAnimalCommand(
    Guid Id) : ICommand;
