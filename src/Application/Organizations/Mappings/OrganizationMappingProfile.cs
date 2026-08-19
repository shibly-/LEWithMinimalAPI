using Application.Organizations.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Organizations.Mappings;

/// <summary>
/// AutoMapper profile for translating between the <see cref="Organization"/> entity
/// and its data transfer objects.
/// </summary>
public class OrganizationMappingProfile : Profile
{
    public OrganizationMappingProfile()
    {
        CreateMap<Organization, OrganizationDto>();
    }
}
