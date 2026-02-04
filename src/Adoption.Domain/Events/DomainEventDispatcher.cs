using Adoption.Domain.SeedWork;
using Microsoft.Extensions.DependencyInjection;

namespace Adoption.Domain.Events;

public sealed class DomainEventDispatcher(IServiceProvider serviceProvider)
    : IDomainEventDispatcher
{
    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct)
    {
        foreach (var domainEvent in events)
        {
            var handlerType = typeof(IDomainEventHandler<>)
                .MakeGenericType(domainEvent.GetType());

            var handlers = serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod("HandleAsync");

                await ((Task)method!.Invoke(
                    handler,
                    new object[] { domainEvent, ct }
                )!)!;
            }
        }
    }
}

