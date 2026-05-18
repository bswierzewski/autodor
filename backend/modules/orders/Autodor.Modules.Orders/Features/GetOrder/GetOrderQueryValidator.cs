using FluentValidation;

namespace Autodor.Modules.Orders.Features.GetOrder;

public class GetOrderQueryValidator : AbstractValidator<GetOrderQuery>
{
    public GetOrderQueryValidator()
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
