namespace Adoption.API.Application.Exceptions;

public class AdoptionRequestNotFoundException : AppException
{
    public AdoptionRequestNotFoundException(){}
    public AdoptionRequestNotFoundException(string message) : base(message) {}
    public AdoptionRequestNotFoundException(string message, Exception innerException) : base(message, innerException) {}
}
