using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Organization aggregate root. The domain layer stays pure: it owns its own
/// invariants and exposes behavior through factory/mutator methods rather than
/// public setters, so an <see cref="Organization"/> can never exist in an invalid state.
/// </summary>
public class Organization : BaseEntity
{
    public const int NameMinLength = 3;
    public const int NameMaxLength = 100;
    public const int DescriptionMaxLength = 250;

    // Required by EF Core materialization.
    private Organization()
    {
    }

    private Organization(string name, string? description, DateTime createdDate)
    {
        Name = name;
        Description = description;
        CreatedDate = createdDate;
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public DateTime CreatedDate { get; private set; }

    /// <summary>
    /// Creates a new, valid <see cref="Organization"/> or throws when invariants are violated.
    /// </summary>
    public static Organization Create(string name, string? description = null)
    {
        var normalizedName = Normalize(name);
        ValidateName(normalizedName);

        var normalizedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        ValidateDescription(normalizedDescription);

        return new Organization(normalizedName, normalizedDescription, DateTime.UtcNow);
    }

    /// <summary>
    /// Renames the organization, enforcing the same name invariants as creation.
    /// </summary>
    public void Rename(string name)
    {
        var normalizedName = Normalize(name);
        ValidateName(normalizedName);
        Name = normalizedName;
    }

    /// <summary>
    /// Updates the optional description, enforcing the maximum-length invariant.
    /// </summary>
    public void UpdateDescription(string? description)
    {
        var normalizedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        ValidateDescription(normalizedDescription);
        Description = normalizedDescription;
    }

    private static string Normalize(string value) => (value ?? string.Empty).Trim();

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Organization name is required.", nameof(name));
        }

        if (name.Length < NameMinLength)
        {
            throw new ArgumentException(
                $"Organization name must be at least {NameMinLength} characters long.", nameof(name));
        }

        if (name.Length > NameMaxLength)
        {
            throw new ArgumentException(
                $"Organization name must not exceed {NameMaxLength} characters.", nameof(name));
        }
    }

    private static void ValidateDescription(string? description)
    {
        if (description is { Length: > DescriptionMaxLength })
        {
            throw new ArgumentException(
                $"Description must not exceed {DescriptionMaxLength} characters.", nameof(description));
        }
    }
}
