using Adoption.API.Abstractions;
using Adoption.API.Application.Models;

namespace Adoption.API.Application.Commands.AdoptionRequests;

public record CreateAdoptionRequestCommand(
    Guid RequesterId,
    Guid AnimalId,
    string Comments) : ICommand<AdoptionResponse>;
