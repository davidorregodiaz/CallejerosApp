using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Domain.SeedWork;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Adoption.Infrastructure.Repositories;

public class AnimalRepository : IAnimalRepository
{
    public AnimalRepository(AdoptionDbContext ctx)
    {
        _ctx = ctx;
    }
    private readonly AdoptionDbContext _ctx;
    public IUnitOfWork UnitOfWork() => _ctx;
    public Animal Add(Animal animal)
    {
        return _ctx.Animals
                .Add(animal)
                .Entity;
    }
    public async Task<Animal?> GetByIdAsync(Guid id)
    {
        var animal = await _ctx.Animals
                        .Where(a => a.Id.Value == id)
                        .SingleOrDefaultAsync();
        return animal;
    }

    public Animal Update(Animal animal)
    {
        return _ctx.Animals
                .Update(animal)
                .Entity;
    }


}
