using System;
using Adoption.Domain.SeedWork;

namespace Adoption.Domain.AggregatesModel.AnimalAggregate;

public interface IAnimalRepository : IRepository<Animal>
{
    IUnitOfWork UnitOfWork();
    Animal Add(Animal animal);
}
