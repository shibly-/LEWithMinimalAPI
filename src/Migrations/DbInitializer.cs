using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Migrations;

/// <summary>
/// Owns database bootstrapping for the solution: applies any pending EF Core
/// migrations (schema changes) and seeds default records. Invoked from the API
/// at startup so a fresh checkout produces a ready-to-use database.
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(
        ApplicationDbContext context,
        CancellationToken cancellationToken = default)
    {
        await context.Database.MigrateAsync(cancellationToken);
        await SeedAsync(context, cancellationToken);
    }

    private static async Task SeedAsync(
        ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        if (await context.Organizations.AnyAsync(cancellationToken))
        {
            return;
        }

        var seedData = new[]
        {
            Organization.Create("Acme Corporation", "Default seeded organization."),
            Organization.Create("Globex Corporation", "Second default seeded organization."),
        };

        await context.Organizations.AddRangeAsync(seedData, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
