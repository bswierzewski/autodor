using FluentValidation;

namespace Autodor.Modules.Orders.Features.GenerateDeliveryNote;

public class GenerateDeliveryNoteCommandValidator : AbstractValidator<GenerateDeliveryNoteCommand>
{
    public GenerateDeliveryNoteCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Pole orderId jest wymagane");

        RuleFor(x => x.Date)
            .NotEqual(default(DateTime))
            .WithMessage("Pole date jest wymagane")
            .GreaterThan(DateTime.MinValue)
            .WithMessage("Pole date musi zawierać prawidłową datę");
    }
}
