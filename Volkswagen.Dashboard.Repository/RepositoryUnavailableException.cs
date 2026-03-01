namespace Volkswagen.Dashboard.Repository;

public sealed class RepositoryUnavailableException : Exception
{
    public RepositoryUnavailableException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
