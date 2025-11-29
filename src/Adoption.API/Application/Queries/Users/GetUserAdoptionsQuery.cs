using Adoption.API.Abstractions;
using Adoption.API.Application.Models;
using Shared.Utilities;

namespace Adoption.API.Application.Queries.Users;

public record GetUserAdoptionsQuery(
    Guid UserId,
    int Page,
    int PageSize) : IQuery<PaginatedResponse<AdoptionViewModel>>;

public record GetUserAdoptionsRequest(
    int Page = 1,
    int PageSize = 10);
