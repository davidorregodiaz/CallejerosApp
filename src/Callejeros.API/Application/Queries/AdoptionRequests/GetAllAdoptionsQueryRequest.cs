using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Queries.AdoptionRequests;

public sealed record GetAllAdoptionsQueryRequest(
    DateTime? Date,
    AdoptionStatus? Status,
    int Page = 1,
    int PageSize = 10,
    string SortBy = "AdoptionRequestId",
    bool IsDescending = false);
