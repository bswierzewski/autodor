using FluentValidation;

namespace Autodor.Modules.Errors.Features.ValidationModel;

public class CreateValidationModelErrorCommandValidator : AbstractValidator<CreateValidationModelErrorCommand>
{
    public CreateValidationModelErrorCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Pole name jest wymagane.")
            .MinimumLength(3)
            .WithMessage("Pole name musi mieć co najmniej 3 znaki.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Pole email jest wymagane.")
            .EmailAddress()
            .WithMessage("Pole email musi zawierać prawidłowy adres e-mail.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Pole quantity musi być większe od zera.");
    }
}
