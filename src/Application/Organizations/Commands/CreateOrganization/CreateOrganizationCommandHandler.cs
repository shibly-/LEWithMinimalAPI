using Application.Common.Interfaces;
using Application.Organizations.Dtos;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Organizations.Commands.CreateOrganization;

public class CreateOrganizationCommandHandler
    : IRequestHandler<CreateOrganizationCommand, OrganizationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateOrganizationCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OrganizationDto> Handle(
        CreateOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        // The domain factory enforces invariants; FluentValidation has already
        // rejected malformed input before we reach this point.
        var organization = Organization.Create(request.Name, request.Description);

        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<OrganizationDto>(organization);
    }
}
