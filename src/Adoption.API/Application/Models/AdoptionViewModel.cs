using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Models;

public record AdoptionViewModel(
    Guid AdoptionRequestId,
    Guid RequesterId,
    string AnimalImage,
    string AnimalName,
    string RequesterName,
    DateTime RequestDate,
    AdoptionStatus Status,
    string Comments);
