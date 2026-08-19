using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Organizations.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Organizations.Queries.GetOrganizationById;

public class GetOrganizationByIdQueryHandler
    : IRequestHandler<GetOrganizationByIdQuery, OrganizationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetOrganizationByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OrganizationDto> Handle(
        GetOrganizationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var organization = await _context.Organizations
            .AsNoTracking()
            .Where(o => o.Id == request.Id)
            .ProjectTo<OrganizationDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(cancellationToken);

        if (organization is null)
        {
            throw new NotFoundException(nameof(Organization), request.Id);
        }

        return organization;
    }
}
