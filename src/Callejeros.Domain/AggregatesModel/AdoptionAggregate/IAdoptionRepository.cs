
using Adoption.Domain.SeedWork;

namespace Adoption.Domain.AggregatesModel.AdoptionAggregate;

public interface IAdoptionRequestRepository : IRepository<AdoptionRequest>
{
    IUnitOfWork UnitOfWork();
    Task<AdoptionRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<AdoptionRequest>> GetAllByUserId(Guid userId, CancellationToken cancellationToken);
    IQueryable<AdoptionRequest> GetByAnimalIdAsync(Guid animalId, CancellationToken cancellationToken);
    void Add(AdoptionRequest adoptionRequest);
    void Delete(AdoptionRequest adoptionRequest);
    Task<bool> HaveAssociatedRequestsAsync(Guid userId, CancellationToken cancellationToken);
}

