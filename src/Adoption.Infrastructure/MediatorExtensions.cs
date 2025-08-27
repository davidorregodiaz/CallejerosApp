using System;
using Adoption.Domain.SeedWork;
using Adoption.Infrastructure.Context;
using MediatR;

namespace Adoption.Infrastructure;

internal static class MediatorExtensions
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, AdoptionDbContext ctx)
    {
        var domainEntities = ctx.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}
