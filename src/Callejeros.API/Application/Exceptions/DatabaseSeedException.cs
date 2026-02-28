
namespace Adoption.API.Application.Exceptions;

public class DatabaseSeedException: AppException
{
    public DatabaseSeedException() { }
    public DatabaseSeedException(string message) : base(message) { }
    public DatabaseSeedException(string message, Exception innerException) : base(message, innerException) { }
}
