using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(Organization.NameMaxLength);

        builder.Property(o => o.Description)
            .HasMaxLength(Organization.DescriptionMaxLength);

        builder.Property(o => o.CreatedDate)
            .IsRequired();

        builder.HasIndex(o => o.Name);
    }
}
