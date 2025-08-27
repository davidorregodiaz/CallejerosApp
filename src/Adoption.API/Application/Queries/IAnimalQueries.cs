using Shared;

namespace Adoption.API.Application.Queries;

public interface IAnimalQueries
{
    Task<TaskResult<Animal>> FindAnimalById(Guid id);
}


