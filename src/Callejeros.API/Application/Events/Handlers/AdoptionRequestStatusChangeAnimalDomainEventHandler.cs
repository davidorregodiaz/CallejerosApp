using Adoption.API.Application.Exceptions;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Domain.Events.Adoption;
using Adoption.Domain.SeedWork;

namespace Adoption.API.Application.Events.Handlers;

public sealed class AdoptionRequestStatusChangeAnimalHandler(
    IAnimalRepository animalRepository,
    IAdoptionRequestRepository adoptionRepository)
    : IDomainEventHandler<AdoptionStatusChangeDomainEvent>
{
    public async Task HandleAsync(
        AdoptionStatusChangeDomainEvent domainEvent,
        CancellationToken cancellationToken)
    {
        var adoptionRequest = await adoptionRepository
                                  .GetByIdAsync(domainEvent.AdoptionRequestId, cancellationToken)
                              ?? throw new AdoptionRequestNotFoundException();

        var animal = await animalRepository
            .GetAnimalByIdAsync(adoptionRequest.AnimalId, cancellationToken)
            ?? throw new AnimalNotFoundException();

        switch (domainEvent.Status)
        {
            case AdoptionStatus.Approved:
                animal.MarkAnimalAsInAdoptionProcess();
                break;

            case AdoptionStatus.Completed:
                animal.MarkAnimalAsAdopted();
                break;

            case AdoptionStatus.Rejected:
                animal.MarkAnimalAsInAdoption();
                break;
        }

        await animalRepository.UnitOfWork()
            .SaveChangesAsync(cancellationToken);
    }
}

