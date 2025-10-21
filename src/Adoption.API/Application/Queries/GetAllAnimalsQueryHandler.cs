using Adoption.API.Abstractions;
using Shared;

namespace Adoption.API.Application.Queries;

public class GetAllAnimalsQueryHandler : IQueryHandler<GetAllAnimalsQuery, List<AnimalResponse>>
{
    public Task<Result<List<AnimalResponse>>> HandleAsync(GetAllAnimalsQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
