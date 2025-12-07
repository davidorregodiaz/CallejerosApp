namespace Adoption.API.Application.Exceptions;

public class AnimalNotFoundException : AppException
{
    public AnimalNotFoundException(){}
    public AnimalNotFoundException(string message) : base(message) {}
    public AnimalNotFoundException(string message, Exception innerException) : base(message, innerException) {}
}
