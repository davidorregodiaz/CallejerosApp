namespace Adoption.API.Abstractions;

internal interface ICommand : IBaseCommand
{
    
}

public interface ICommand<TResponse> : IBaseCommand
{

}

public interface IBaseCommand;
