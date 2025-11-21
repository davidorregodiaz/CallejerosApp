using Adoption.API.Abstractions;

namespace Adoption.API.Application.Commands.AdoptionRequests;

public record DeleteAdoptionRequestCommand(
    Guid Id) : ICommand;
