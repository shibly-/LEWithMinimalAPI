using System;
using Application.Organizations.Mappings;
using AutoMapper;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Application.UnitTests.Common;

/// <summary>
/// Test helpers that build an isolated in-memory <see cref="ApplicationDbContext"/>
/// and a real AutoMapper instance configured with the application's profiles.
/// </summary>
public static class TestContextFactory
{
    public static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"OrganizationsTests_{Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    public static IMapper CreateMapper()
    {
        var provider = new ServiceCollection()
            .AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance)
            .AddAutoMapper(cfg => cfg.AddProfile<OrganizationMappingProfile>())
            .BuildServiceProvider();

        return provider.GetRequiredService<IMapper>();
    }
}
