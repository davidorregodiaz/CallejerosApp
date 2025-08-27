
using Adoption.API.Abstractions;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Application.Mappers;
using Shared;
using Shared.Dtos;
using Adoption.Domain.SeedWork;

namespace Adoption.API.Application.Commands.Animals;

public class CreateAnimalCommandHandler : ICommandHandler<CreateAnimalCommand, ResponseAnimalDto>
{
    private readonly IAnimalRepository _animalRepository;
    private readonly ILogger<CreateAnimalCommandHandler> _logger;

    public CreateAnimalCommandHandler(IAnimalRepository animalRepository, ILogger<CreateAnimalCommandHandler> logger)
    {
        _animalRepository = animalRepository;
        _logger = logger;
    }

    public async Task<TaskResult<ResponseAnimalDto>> Handle(CreateAnimalCommand request, CancellationToken cancellationToken)
    {
        var animal = Animal.Create(
            request.Name,
            request.Age,
            request.Description,
            request.OwnerId,
            request.Breed,
            request.Type,
            request.ImagePaths
        );

        var animalResponse = _animalRepository.Add(animal).ToResponse();

        _logger.LogInformation("Creating Animal - Animal : {@Animal}", animal);

        await _animalRepository.UnitOfWork().SaveEntitiesAsync(cancellationToken);
        return TaskResult<ResponseAnimalDto>.FromData(animalResponse);
    }
}
