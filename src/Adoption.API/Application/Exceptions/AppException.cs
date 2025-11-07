namespace Adoption.API.Application.Exceptions;

public class AppException : ApplicationException
{
    protected AppException() { }
    protected AppException(string message) : base(message) { }
    protected AppException(string message, Exception innerException) : base(message, innerException) { }
}
