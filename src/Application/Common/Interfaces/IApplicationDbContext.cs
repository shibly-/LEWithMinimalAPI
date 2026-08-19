using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

/// <summary>
/// Abstraction over the persistence context exposed to the application layer.
/// The spec forgoes the repository pattern, so handlers use the DbContext directly,
/// but through this interface so the application layer stays free of an
/// Infrastructure/EF-provider dependency.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Organization> Organizations { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
