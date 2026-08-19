using Application.Organizations.Commands.CreateOrganization;
using Domain.Entities;
using FluentValidation.TestHelper;
using Xunit;

namespace Application.UnitTests.Organizations;

public class CreateOrganizationCommandValidatorTests
{
    private readonly CreateOrganizationCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new CreateOrganizationCommand("Acme Corporation", "A description.");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    public void Validate_WithInvalidName_HasError(string name)
    {
        var command = new CreateOrganizationCommand(name, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Validate_WithDescriptionOverMaxLength_HasError()
    {
        var command = new CreateOrganizationCommand(
            "Acme Corporation",
            new string('a', Organization.DescriptionMaxLength + 1));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Description);
    }
}
