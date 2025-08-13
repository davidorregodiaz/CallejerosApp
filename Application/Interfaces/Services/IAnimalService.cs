using Shared.Dtos;
using Shared;

namespace Application.Interfaces.Services;

public interface IAnimalService
{
    Task<TaskResult<ResponseAnimalDto>> CreateAnimal(CreateAnimalDto animalDto,List<string> picturesPaths);
    Task<TaskResult<ResponseAnimalDto>> GetAnimalById(Guid id);
    Task<TaskResult<IEnumerable<ResponseAnimalDto>>> GetAnimals();
    Task<TaskResult> DeleteAnimal(Guid id, string ownerId);
    Task<TaskResult<ResponseAnimalDto>> UpdateAnimal(CreateAnimalDto animalDto, Guid id, string ownerId);
    Task<TaskResult<IEnumerable<ResponseAnimalDto>>> FilterAnimalsByBreed(string breed);
    Task<TaskResult<IEnumerable<ResponseAnimalDto>>> FilterAnimalsByType(string type);
    Task<TaskResult<IEnumerable<ResponseAnimalDto>>> FilterAnimalsByOwnerId(string ownerId);
    Task<TaskResult<IEnumerable<ResponseAnimalDto>>> FilterAnimalsByTypeAndBreed(string type, string breed);

}
