namespace Adoption.API.Application.Exceptions;

public class UserIsAlreadyInAnAdoptionProcess
    : AppException
{
    public UserIsAlreadyInAnAdoptionProcess(){}
    public UserIsAlreadyInAnAdoptionProcess(string message) : base(message) {}
    public UserIsAlreadyInAnAdoptionProcess(string message, Exception innerException) : base(message, innerException) {}
}
