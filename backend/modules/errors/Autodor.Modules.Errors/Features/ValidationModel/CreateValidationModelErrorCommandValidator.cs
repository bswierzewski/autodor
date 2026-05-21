using FluentValidation;

namespace Autodor.Modules.Errors.Features.ValidationModel;

public class CreateValidationModelErrorCommandValidator : AbstractValidator<CreateValidationModelErrorCommand>
{
    public CreateValidationModelErrorCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MinimumLength(3)
            .WithMessage("Name must be at least 3 characters long.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");
    }
}