using FluentValidation;

namespace Autodor.Modules.Orders.Features.ExcludeOrderItem;

public class ExcludeOrderItemCommandValidator : AbstractValidator<ExcludeOrderItemCommand>
{
    public ExcludeOrderItemCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.ItemNumber)
            .NotEmpty()
            .WithMessage("Item number is required");
    }
}
