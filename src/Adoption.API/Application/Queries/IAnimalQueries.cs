using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Shared;

namespace Adoption.API.Application.Queries;

public interface IAnimalQueries
{
    Task<Result<Animal>> FindAnimalById(Guid id);
    Task<Result<IEnumerable<Animal>>> FindAllAnimals();
}


