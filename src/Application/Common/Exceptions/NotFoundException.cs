namespace Application.Common.Exceptions;

/// <summary>
/// Thrown when a requested resource does not exist. Mapped to HTTP 404 at the API boundary.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}
