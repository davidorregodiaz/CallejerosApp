
using Adoption.API.Abstractions;
using Adoption.API.Application.Mappers;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Shared;
using Shared.Dtos;

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

    public async Task<Result<ResponseAnimalDto>> HandleAsync(CreateAnimalCommand request, CancellationToken cancellationToken)
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
        return Result<ResponseAnimalDto>.FromData(animalResponse);
    }
}
