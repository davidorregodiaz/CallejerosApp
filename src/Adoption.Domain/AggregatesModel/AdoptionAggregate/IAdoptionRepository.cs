
using Adoption.Domain.SeedWork;

namespace Adoption.Domain.AggregatesModel.AdoptionAggregate;

public interface IAdoptionRepository : IRepository<AdoptionRequest>     
{
    Task<AdoptionRequest?> GetByIdAsync(Guid id);
    Task<IEnumerable<AdoptionRequest>> GetByRequesterIdAsync(Guid requesterId);
    Task<IEnumerable<AdoptionRequest>> GetByAnimalIdAsync(Guid animalId);
    Task AddAsync(AdoptionRequest adoptionRequest);
    Task UpdateAsync(AdoptionRequest adoptionRequest);
    Task DeleteAsync(Guid id);
}

