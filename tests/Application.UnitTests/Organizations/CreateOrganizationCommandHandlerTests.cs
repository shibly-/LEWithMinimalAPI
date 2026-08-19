using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Organizations.Commands.CreateOrganization;
using Application.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.UnitTests.Organizations;

public class CreateOrganizationCommandHandlerTests
{
    [Fact]
    public async Task Handle_PersistsOrganization_AndReturnsDto()
    {
        await using var context = TestContextFactory.CreateContext();
        var mapper = TestContextFactory.CreateMapper();
        var handler = new CreateOrganizationCommandHandler(context, mapper);

        var command = new CreateOrganizationCommand("Acme Corporation", "A description.");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Id > 0);
        Assert.Equal("Acme Corporation", result.Name);
        Assert.Equal("A description.", result.Description);
        Assert.NotEqual(default, result.CreatedDate);

        var persisted = await context.Organizations.SingleAsync();
        Assert.Equal(result.Id, persisted.Id);
        Assert.Equal("Acme Corporation", persisted.Name);
    }

    [Fact]
    public async Task Handle_WithNullDescription_PersistsNullDescription()
    {
        await using var context = TestContextFactory.CreateContext();
        var mapper = TestContextFactory.CreateMapper();
        var handler = new CreateOrganizationCommandHandler(context, mapper);

        var command = new CreateOrganizationCommand("Acme Corporation", null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Null(result.Description);
        Assert.Single(context.Organizations);
    }
}
