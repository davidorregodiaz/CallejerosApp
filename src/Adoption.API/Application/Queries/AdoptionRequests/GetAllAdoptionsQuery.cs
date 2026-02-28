using Adoption.API.Abstractions;
using Adoption.API.Application.Models;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Queries.AdoptionRequests;

public record GetAllAdoptionsQuery(
    Guid? UserId,
    DateTime? Date,
    AdoptionStatus? Status,
    int Page = 1,
    int PageSize = 10,
    string SortBy = "AdoptionRequestId",
    bool IsDescending = false) : IQuery<PaginatedResponse<AdoptionViewModel>>;
