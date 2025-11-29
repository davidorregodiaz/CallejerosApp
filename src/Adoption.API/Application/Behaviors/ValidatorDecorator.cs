using Adoption.API.Abstractions;
using Adoption.API.Application.Models;
using Adoption.API.Extensions;
using FluentValidation;
using Shared;

namespace Adoption.API.Application.Behaviors;

internal static class ValidatorDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<ICommandHandler<TCommand, TResponse>> logger,
        IEnumerable<IValidator<TCommand>> validators) : ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    {
        public async Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            var typeName = command.GetGenericTypeName();

            logger.LogInformation("Validating command {CommandType}", typeName);

            var failures = validators
                .Select(v => v.Validate(command))
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();

            if (failures.Any())
            {
                logger.LogWarning("Validation errors - {CommandType} - Request: {@Command} - Errors: {@ValidationErrors}", typeName, command, failures);
                throw new ValidationException(failures);
            }

            return await innerHandler.HandleAsync(command, cancellationToken);
        }
    }
}
