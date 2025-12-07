using Adoption.API.Abstractions;
using Adoption.API.Application.Models;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Commands.AdoptionRequests;

public sealed record UpdateAdoptionRequestCommand(
    Guid Id,
    AdoptionStatus Status) : ICommand<AdoptionViewModel>;
