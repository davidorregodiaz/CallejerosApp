using Adoption.Domain.SeedWork;

namespace Adoption.Domain.AggregatesModel.AdoptionAggregate;

public class Appointment
    : Entity
{
    public Appointment() : base(Guid.NewGuid())
    {
    }
}
