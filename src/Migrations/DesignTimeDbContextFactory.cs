using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Migrations;

/// <summary>
/// Lets the EF Core CLI (<c>dotnet ef</c>) construct the context at design time so
/// migrations can be created/applied against this dedicated migrations project
/// without needing to boot the API host.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var connectionString = args.Length > 0
            ? args[0]
            : "Server=localhost\\SQLEXPRESS;Database=LEMinimalAPIDB;User Id=sqluser;Password=password;TrustServerCertificate=True;";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(
            connectionString,
            sqlServer => sqlServer.MigrationsAssembly(typeof(DesignTimeDbContextFactory).Assembly.GetName().Name));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
