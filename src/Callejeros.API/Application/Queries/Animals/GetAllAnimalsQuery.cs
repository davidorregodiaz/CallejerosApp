using Adoption.API.Abstractions;
using Adoption.API.Application.Models;

namespace Adoption.API.Application.Queries.Animals;

public record GetAllAnimalsQuery(
    string? Name,
    string? Species,
    string? Localization,
    int? Age,
    Guid? OwnerId,
    string SortBy = "AnimalId",
    int Page = 1,
    int PageSize = 10,
    bool IsDescending = false) : IQuery<PaginatedResponse<AnimalViewModel>>;
