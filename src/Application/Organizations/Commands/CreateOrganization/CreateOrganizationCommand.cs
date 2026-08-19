using Application.Organizations.Dtos;
using MediatR;

namespace Application.Organizations.Commands.CreateOrganization;

/// <summary>
/// Command to create a new organization.
/// </summary>
public record CreateOrganizationCommand(string Name, string? Description) : IRequest<OrganizationDto>;
