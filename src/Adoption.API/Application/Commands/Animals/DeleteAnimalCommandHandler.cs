using Adoption.API.Abstractions;
using Adoption.API.Application.Exceptions;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Adoption.API.Application.Commands.Animals;

public class DeleteAnimalCommandHandler(AdoptionDbContext ctx)
    : ICommandHandler<DeleteAnimalCommand>
{
    public async Task HandleAsync(DeleteAnimalCommand command, CancellationToken cancellationToken)
    {
        var animal = await ctx.Animals.SingleOrDefaultAsync(x => x.Id == new AnimalId(command.Id), cancellationToken);

        if (animal is null)
            throw new AnimalNotFoundException($"Animal with id - {command.Id} was not found");

        ctx.Animals.Remove(animal);
        await ctx.SaveChangesAsync(cancellationToken);
    }
}
