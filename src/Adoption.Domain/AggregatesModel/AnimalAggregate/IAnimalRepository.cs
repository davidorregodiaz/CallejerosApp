using System;
using Adoption.Domain.SeedWork;

namespace Adoption.Domain.AggregatesModel.AnimalAggregate;

public interface IAnimalRepository : IRepository<Animal>
{
    IUnitOfWork UnitOfWork();
    Task<Animal?> GetAnimalByIdAsync(Guid id, CancellationToken cancellationToken);
    void Add(Animal animal);
    void Delete(Animal animal);
}
