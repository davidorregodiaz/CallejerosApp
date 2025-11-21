
using Adoption.Domain.SeedWork;

namespace Adoption.Domain.AggregatesModel.AdoptionAggregate;

public interface IAdoptionRequestRepository : IRepository<AdoptionRequest>
{
    IUnitOfWork UnitOfWork();
    Task<AdoptionRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<AdoptionRequest>> GetByRequesterIdAsync(Guid requesterId);
    Task<IEnumerable<AdoptionRequest>> GetByAnimalIdAsync(Guid animalId);
    void Add(AdoptionRequest adoptionRequest);
    void Delete(AdoptionRequest adoptionRequest);
    Task<bool> HaveAssociatedRequestsAsync(Guid userId, CancellationToken cancellationToken);
}

