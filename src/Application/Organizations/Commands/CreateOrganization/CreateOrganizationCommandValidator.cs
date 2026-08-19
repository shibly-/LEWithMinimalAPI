using Domain.Entities;
using FluentValidation;

namespace Application.Organizations.Commands.CreateOrganization;

public class CreateOrganizationCommandValidator : AbstractValidator<CreateOrganizationCommand>
{
    public CreateOrganizationCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(Organization.NameMinLength)
                .WithMessage($"Name must be at least {Organization.NameMinLength} characters long.")
            .MaximumLength(Organization.NameMaxLength)
                .WithMessage($"Name must not exceed {Organization.NameMaxLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(Organization.DescriptionMaxLength)
                .WithMessage($"Description must not exceed {Organization.DescriptionMaxLength} characters.")
            .When(x => x.Description is not null);
    }
}
