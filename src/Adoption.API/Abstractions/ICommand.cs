using MediatR;
using Shared;

namespace Adoption.API.Abstractions;

internal interface ICommand : IRequest<TaskResult>
{
    
}


internal interface ICommand <TResponse> : IRequest<TaskResult<TResponse>>
{

}
