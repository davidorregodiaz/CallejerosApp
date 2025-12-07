using Adoption.API.Abstractions;
using Adoption.API.Application.Models;
using Adoption.API.Extensions;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Adoption.API.Application.Behaviors;

internal static class TransactionDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<ICommandHandler<TCommand, TResponse>> logger,
        AdoptionDbContext ctx) : ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    {
        public async Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            var response = default(TResponse);
            var typeName = command.GetGenericTypeName();

            try
            {
                if (ctx.HasActiveTransaction)
                {
                    return await innerHandler.HandleAsync(command, cancellationToken);
                }

                var strategy = ctx.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {

                    await using var transaction = await ctx.StartTransactionAsync();
                    using (logger.BeginScope(new List<KeyValuePair<string, object>>
                           {
                               new("TransactionContext", transaction.TransactionId)
                           }))
                    {
                        logger.LogInformation("Begin transaction {TransactionId} for {CommandName} ({@Command})",
                            transaction.TransactionId, typeName, command);

                        response = await innerHandler.HandleAsync(command, cancellationToken);

                        logger.LogInformation("Commit transaction {TransactionId} for {CommandName}",
                            transaction.TransactionId, typeName);

                        await ctx.CommitTransactionAsync(transaction);
                    }
                });

                return response!;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Handling transaction for {CommandName} ({@Command})", typeName, command);

                throw;
            }
        }
    }

    internal sealed class QueryHandler<TQuery, TResponse>(
            IQueryHandler<TQuery, TResponse> innerHandler,
            ILogger<IQueryHandler<TQuery,TResponse>> logger,
            AdoptionDbContext ctx) : IQueryHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>
        {
            public async Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken)
            {
                var result = default(Result<TResponse>);
                var typeName = query.GetGenericTypeName();

                try
                {
                    if (ctx.HasActiveTransaction)
                    {
                        return await innerHandler.HandleAsync(query, cancellationToken);
                    }

                    var strategy = ctx.Database.CreateExecutionStrategy();

                    await strategy.ExecuteAsync(async () =>
                    {
                        await using var transaction = await ctx.StartTransactionAsync();
                        using (logger.BeginScope(new List<KeyValuePair<string, object>> { new("TransactionContext", transaction.TransactionId) }))
                        {
                            logger.LogInformation("Begin transaction {TransactionId} for {QueryName} ({@Query})", transaction.TransactionId, typeName, query);

                            result = await innerHandler.HandleAsync(query, cancellationToken);

                            if (result.IsSuccessful(out _))
                            {
                                logger.LogInformation("Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                                await ctx.CommitTransactionAsync(transaction);
                            }
                        }
                        // await _orderingIntegrationEventService.PublishEventsThroughEventBusAsync(transactionId);
                    });

                    return result;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error Handling transaction for {QueryName} ({@Query})", typeName, query);
                    throw;
                }
            }
        }
}
