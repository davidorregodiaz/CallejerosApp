using Adoption.API.Abstractions;
using Adoption.API.Application.Models.User;

namespace Adoption.API.Application.Queries.Users;

public record class GetAllUsersQuery(
    string SortBy = "Id",
    int Page = 1,
    int PageSize = 10,
    bool IsDescending = false
) : IQuery<PaginatedResponse<UserViewModel>>;
