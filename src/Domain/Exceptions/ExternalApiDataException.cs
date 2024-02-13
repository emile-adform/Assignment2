
namespace Domain.Exceptions;

public class ExternalApiDataException : Exception
{
    public ExternalApiDataException(string message) : base(message) { }
}
