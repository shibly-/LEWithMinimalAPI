using System;
using Domain.Entities;
using Xunit;

namespace Domain.UnitTests;

public class OrganizationTests
{
    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var before = DateTime.UtcNow;

        var organization = Organization.Create("Acme Corporation", "A valid description.");

        Assert.Equal("Acme Corporation", organization.Name);
        Assert.Equal("A valid description.", organization.Description);
        Assert.True(organization.CreatedDate >= before && organization.CreatedDate <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_TrimsWhitespaceFromNameAndDescription()
    {
        var organization = Organization.Create("  Acme  ", "  spaced  ");

        Assert.Equal("Acme", organization.Name);
        Assert.Equal("spaced", organization.Description);
    }

    [Fact]
    public void Create_WithNullOrWhitespaceDescription_StoresNull()
    {
        var organization = Organization.Create("Acme Corporation", "   ");

        Assert.Null(organization.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithMissingName_Throws(string? name)
    {
        Assert.Throws<ArgumentException>(() => Organization.Create(name!));
    }

    [Fact]
    public void Create_WithNameShorterThanMinimum_Throws()
    {
        Assert.Throws<ArgumentException>(() => Organization.Create("ab"));
    }

    [Fact]
    public void Create_WithNameLongerThanMaximum_Throws()
    {
        var tooLong = new string('a', Organization.NameMaxLength + 1);

        Assert.Throws<ArgumentException>(() => Organization.Create(tooLong));
    }

    [Fact]
    public void Create_WithDescriptionLongerThanMaximum_Throws()
    {
        var tooLong = new string('a', Organization.DescriptionMaxLength + 1);

        Assert.Throws<ArgumentException>(() => Organization.Create("Valid Name", tooLong));
    }

    [Fact]
    public void Rename_WithValidName_UpdatesName()
    {
        var organization = Organization.Create("Acme Corporation");

        organization.Rename("Globex Corporation");

        Assert.Equal("Globex Corporation", organization.Name);
    }

    [Fact]
    public void Rename_WithInvalidName_Throws()
    {
        var organization = Organization.Create("Acme Corporation");

        Assert.Throws<ArgumentException>(() => organization.Rename("x"));
    }

    [Fact]
    public void UpdateDescription_WithTooLongValue_Throws()
    {
        var organization = Organization.Create("Acme Corporation");
        var tooLong = new string('a', Organization.DescriptionMaxLength + 1);

        Assert.Throws<ArgumentException>(() => organization.UpdateDescription(tooLong));
    }
}
