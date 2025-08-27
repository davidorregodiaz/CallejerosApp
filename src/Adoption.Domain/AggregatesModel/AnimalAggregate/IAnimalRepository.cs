using System;
using Adoption.Domain.SeedWork;

namespace Adoption.Domain.AggregatesModel.AnimalAggregate;

public interface IAnimalRepository : IRepository<Animal>
{
    IUnitOfWork UnitOfWork();
    Task<Animal?> GetByIdAsync(Guid id);
    Animal Add(Animal animal);
    Animal Update(Animal animal);
}
