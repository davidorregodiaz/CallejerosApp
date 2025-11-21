using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Adoption.Domain.SeedWork;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Adoption.Infrastructure.Repositories;

public class AdoptionRequestsRepository(AdoptionDbContext ctx) : IAdoptionRequestRepository
{
    public IUnitOfWork UnitOfWork() => ctx;
    public Task<AdoptionRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        ctx.AdoptionRequests.SingleOrDefaultAsync(x => x.Id == new AdoptionRequestId(id), cancellationToken);

    public Task<IEnumerable<AdoptionRequest>> GetByRequesterIdAsync(Guid requesterId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AdoptionRequest>> GetByAnimalIdAsync(Guid animalId)
    {
        throw new NotImplementedException();
    }

    public void Add(AdoptionRequest adoptionRequest) => ctx.Add(adoptionRequest);

    public void Delete(AdoptionRequest adoptionRequest) => ctx.AdoptionRequests.Remove(adoptionRequest);

    public async Task<bool> HaveAssociatedRequestsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await ctx.AdoptionRequests
            .AnyAsync(x => x.RequesterId == userId && 
                           x.Status != AdoptionStatus.Completed, cancellationToken);
    }
}
