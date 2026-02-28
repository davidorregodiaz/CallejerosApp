using Adoption.API.Abstractions;
using Adoption.API.Application.Models;
using Adoption.API.Utils.Common;

namespace Adoption.API.Application.Queries.Users;

public record GetUserAnimalsQuery(
    Guid UserId,
    int Page,
    int PageSize) : IQuery<PaginatedResponse<AnimalViewModel>>;

public record GetUserAnimalsRequest(
    int Page = 1,
    int PageSize = 10);
