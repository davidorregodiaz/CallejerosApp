using Adoption.API.Abstractions;
using Adoption.API.Extensions;
using Shared;

namespace Adoption.API.Application.Behaviors;

internal static class LoggingDecorator
{
    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ILogger<IQueryHandler<TQuery, TResponse>> logger) : IQueryHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling query {QueryName} ({@Query})", query.GetGenericTypeName(), query);
            var result = await innerHandler.HandleAsync(query, cancellationToken);

            if (result.IsSuccessful(out var response))
            {
                logger.LogInformation("Query {QueryName} handled - with response: {@Response}", query.GetGenericTypeName(), response);
                return Result<TResponse>.FromData(response);
            }
            
            logger.LogWarning(result.Message);

            return Result<TResponse>.FromFailure(result.Message);
        }
    }
    
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<ICommandHandler<TCommand, TResponse>> logger) : ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    {
        public async Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling command {CommandName} ({@Command})", command.GetGenericTypeName(), command);
            var response = await innerHandler.HandleAsync(command, cancellationToken);
            logger.LogInformation("Command {CommandName} handled - with response: {@Response}", command.GetGenericTypeName(), response);
                
            return response;
        }
    }
}
