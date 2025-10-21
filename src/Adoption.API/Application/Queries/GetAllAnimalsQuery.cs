using Adoption.API.Abstractions;
using Shared;

namespace Adoption.API.Application.Queries;

public record GetAllAnimalsQuery(
    string Name,
    int Page,
    int PageSize) : IQuery<List<AnimalResponse>>;

public class AnimalResponse
{
    
}
