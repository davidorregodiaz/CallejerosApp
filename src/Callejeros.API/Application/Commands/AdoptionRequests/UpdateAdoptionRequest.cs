using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Commands.AdoptionRequests;

public sealed record UpdateAdoptionRequest(
    AdoptionStatus Status
);
