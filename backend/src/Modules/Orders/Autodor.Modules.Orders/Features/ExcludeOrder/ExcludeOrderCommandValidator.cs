using FluentValidation;

namespace Autodor.Modules.Orders.Features.ExcludeOrder;

public class ExcludeOrderCommandValidator : AbstractValidator<ExcludeOrderCommand>
{
    public ExcludeOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");
    }
}
