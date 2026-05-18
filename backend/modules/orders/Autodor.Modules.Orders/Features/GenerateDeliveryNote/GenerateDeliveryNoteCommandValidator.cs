using FluentValidation;

namespace Autodor.Modules.Orders.Features.GenerateDeliveryNote;

public class GenerateDeliveryNoteCommandValidator : AbstractValidator<GenerateDeliveryNoteCommand>
{
    public GenerateDeliveryNoteCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.Date)
            .NotEqual(default(DateTime))
            .WithMessage("Date is required")
            .GreaterThan(DateTime.MinValue)
            .WithMessage("Date must be a valid date");
    }
}
