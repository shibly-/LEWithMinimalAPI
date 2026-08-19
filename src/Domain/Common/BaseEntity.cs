namespace Domain.Common;

/// <summary>
/// Base type for all persistable domain entities.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; protected set; }
}
