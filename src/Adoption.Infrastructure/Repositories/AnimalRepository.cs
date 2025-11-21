using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Domain.SeedWork;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Adoption.Infrastructure.Repositories;

public class AnimalRepository(AdoptionDbContext ctx) : IAnimalRepository
{
    public IUnitOfWork UnitOfWork() => ctx;
    public Task<Animal?> GetAnimalByIdAsync(Guid id,  CancellationToken cancellationToken) =>
        ctx.Animals.SingleOrDefaultAsync(x => x.Id == new AnimalId(id), cancellationToken);
    public void Add(Animal animal) => ctx.Add(animal);
    public void Delete(Animal animal) =>  ctx.Remove(animal); 
}
