using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Models;

public record AdoptionResponse(
    Guid AdoptionRequestId,
    Guid RequesterId,
    DateTime RequestDate,
    AdoptionStatus Status,
    string Comments);
