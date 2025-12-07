using Adoption.API.Abstractions;
using Adoption.API.Application.Exceptions;
using Adoption.API.Application.Services.Minio;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Adoption.API.Application.Commands.Animals;

public class DeleteAnimalCommandHandler(IAnimalRepository animalRepository, IMinioService minioService)
    : ICommandHandler<DeleteAnimalCommand>
{
    public async Task HandleAsync(DeleteAnimalCommand command, CancellationToken cancellationToken)
    {
        var animal = await animalRepository
                         .GetAnimalByIdAsync(command.Id, cancellationToken)
                     ?? throw new AnimalNotFoundException($"Animal with id - {command.Id} was not found");

        animalRepository.Delete(animal);
        
        await minioService.DeleteBlob(animal.PrincipalImage, cancellationToken);

        if (animal.AdditionalImagesUrl != null)
        {
            foreach (var additionalImage in animal.AdditionalImagesUrl)
            {
                await minioService.DeleteBlob(additionalImage, cancellationToken);
            }
        }
        
        await animalRepository.UnitOfWork().SaveChangesAsync(cancellationToken);
    }
}
