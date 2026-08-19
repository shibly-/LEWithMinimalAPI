using Application.Organizations.Dtos;
using MediatR;

namespace Application.Organizations.Queries.GetOrganizationById;

/// <summary>
/// Query to fetch a single organization by its identifier.
/// </summary>
public record GetOrganizationByIdQuery(int Id) : IRequest<OrganizationDto>;
