
using MediatR;
using Shared;

namespace Adoption.API.Abstractions;

internal interface ICommandHandler<TCommand> : IRequestHandler<TCommand, TaskResult>
    where TCommand : ICommand
{

}

interface ICommandHandler <TCommand,TResponse> : IRequestHandler<TCommand, TaskResult<TResponse>>
    where TCommand : ICommand<TResponse>
{
    
}
