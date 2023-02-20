namespace Samples.Configuration.Exceptions;

public class InvalidRequestException : Exception
{
    public InvalidRequestException(string message = "Invalid request")
        : base(message ?? "Invalid request", null) { }
}
