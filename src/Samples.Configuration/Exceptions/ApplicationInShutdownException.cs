namespace Samples.Configuration.Exceptions;

public class ApplicationInShutdownException : Exception
{
    public ApplicationInShutdownException()
        : base("Application currently in progress of shutting down, try again later") { }
}
