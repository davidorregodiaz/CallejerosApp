namespace Adoption.API.Abstractions;

internal interface ICommand : IBaseCommand
{
    
}


internal interface ICommand<TResponse> : IBaseCommand
{

}

public interface IBaseCommand;
