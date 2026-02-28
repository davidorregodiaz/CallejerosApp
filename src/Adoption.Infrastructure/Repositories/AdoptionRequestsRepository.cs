using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Adoption.Domain.SeedWork;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Adoption.Infrastructure.Repositories;

public class AdoptionRequestsRepository(AdoptionDbContext ctx) : IAdoptionRequestRepository
{
    public IUnitOfWork UnitOfWork() => ctx;
    public Task<AdoptionRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        ctx.AdoptionRequests
            .Include(x => x.Appointments)
            .SingleOrDefaultAsync(x => x.Id == new AdoptionRequestId(id), cancellationToken);

    public async Task<IEnumerable<AdoptionRequest>> GetAllByUserId(Guid userId, CancellationToken cancellationToken)
    {
        return await ctx.AdoptionRequests
            .Where(x => x.RequesterId == userId).ToListAsync(cancellationToken);
    }

    public IQueryable<AdoptionRequest> GetByAnimalIdAsync(Guid animalId, CancellationToken cancellationToken)
    {
        return ctx.AdoptionRequests.Where(x => x.AnimalId == animalId);
    }

    public void Add(AdoptionRequest adoptionRequest) => ctx.Add(adoptionRequest);

    public void Delete(AdoptionRequest adoptionRequest) => ctx.AdoptionRequests.Remove(adoptionRequest);

    public async Task<bool> HaveAssociatedRequestsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await ctx.AdoptionRequests
            .AnyAsync(x => x.RequesterId == userId && 
                           x.Status != AdoptionStatus.Completed && x.Status != AdoptionStatus.Rejected, cancellationToken);
    }
}
