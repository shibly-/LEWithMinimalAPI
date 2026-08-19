using Application.Organizations.Commands.CreateOrganization;
using Application.Organizations.Dtos;
using Application.Organizations.Queries.GetOrganizationById;
using MediatR;

namespace Api.Endpoints;

/// <summary>
/// Minimal API endpoints for the Organizations resource. Each endpoint is a thin
/// adapter that forwards to a MediatR command/query and shapes the HTTP response.
/// </summary>
public static class OrganizationEndpoints
{
    public static IEndpointRouteBuilder MapOrganizationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/organizations")
            .WithTags("Organizations");

        group.MapGet("/{id:int}", GetOrganizationById)
            .WithName(nameof(GetOrganizationById))
            .WithSummary("Get an organization by its identifier.")
            .Produces<OrganizationDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateOrganization)
            .WithName(nameof(CreateOrganization))
            .WithSummary("Create a new organization.")
            .Produces<OrganizationDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> GetOrganizationById(
        int id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var organization = await sender.Send(new GetOrganizationByIdQuery(id), cancellationToken);
        return Results.Ok(organization);
    }

    private static async Task<IResult> CreateOrganization(
        CreateOrganizationCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var organization = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/organizations/{organization.Id}", organization);
    }
}
