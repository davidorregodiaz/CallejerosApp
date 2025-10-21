using Adoption.API.Abstractions;
using Adoption.API.Extensions;
using Adoption.Domain.Exceptions.Adoption;
using Adoption.Infrastructure.Context;
using FluentValidation;
using Shared;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace Adoption.API.Application.Behaviors;

internal static class ValidatorDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<ICommandHandler<TCommand, TResponse>> logger,
        IEnumerable<IValidator<TCommand>> validators) : ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken)
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

                var errors = failures
                    .GroupBy(f => f.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray());

                string errorString = string.Join(" | ",
                    errors.Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value)}"));
            
                return Result<TResponse>.FromFailure("Validation failed", 400, errorString);
            }

            return await innerHandler.HandleAsync(command, cancellationToken);
        }
    }
}
