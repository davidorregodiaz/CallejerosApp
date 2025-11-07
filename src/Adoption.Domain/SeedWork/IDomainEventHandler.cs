using Shared;

namespace Adoption.Domain.SeedWork;

public interface IDomainEventHandler<T> where T : IDomainEvent
{
    Task<Result<T>> Handle(T domainEvent, CancellationToken cancellationToken);
}
