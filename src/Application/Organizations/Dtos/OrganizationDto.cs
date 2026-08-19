namespace Application.Organizations.Dtos;

/// <summary>
/// Read model returned to API clients for an organization.
/// </summary>
public class OrganizationDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; }
}
