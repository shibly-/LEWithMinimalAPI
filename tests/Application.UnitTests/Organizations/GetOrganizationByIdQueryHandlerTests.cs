using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Organizations.Queries.GetOrganizationById;
using Application.UnitTests.Common;
using Domain.Entities;
using Xunit;

namespace Application.UnitTests.Organizations;

public class GetOrganizationByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenOrganizationExists_ReturnsDto()
    {
        await using var context = TestContextFactory.CreateContext();
        var mapper = TestContextFactory.CreateMapper();

        var organization = Organization.Create("Acme Corporation", "A description.");
        context.Organizations.Add(organization);
        await context.SaveChangesAsync();

        var handler = new GetOrganizationByIdQueryHandler(context, mapper);

        var result = await handler.Handle(new GetOrganizationByIdQuery(organization.Id), CancellationToken.None);

        Assert.Equal(organization.Id, result.Id);
        Assert.Equal("Acme Corporation", result.Name);
        Assert.Equal("A description.", result.Description);
    }

    [Fact]
    public async Task Handle_WhenOrganizationMissing_ThrowsNotFound()
    {
        await using var context = TestContextFactory.CreateContext();
        var mapper = TestContextFactory.CreateMapper();
        var handler = new GetOrganizationByIdQueryHandler(context, mapper);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetOrganizationByIdQuery(999), CancellationToken.None));
    }
}
