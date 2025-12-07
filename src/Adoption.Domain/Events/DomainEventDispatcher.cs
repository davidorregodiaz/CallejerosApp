using Adoption.Domain.SeedWork;

namespace Adoption.Domain.Events;

public class DomainEventDispatcher(IServiceProvider serviceProvider)
    : IDomainEventDispatcher
{
    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct)
    {
        foreach (var domainEvent in events)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handler = serviceProvider.GetService(handlerType);

            if (handler == null)
                return;
            
            var method = handlerType.GetMethod("HandleAsync");
            
            await (Task)method.Invoke(handler, new object[] { domainEvent, ct});
        }
    }
}
