using FluentValidation;

namespace Autodor.Modules.Orders.Features.GetOrders;

public class GetOrdersCommandValidator : AbstractValidator<GetOrdersCommand>
{
    public GetOrdersCommandValidator()
    {
        RuleFor(x => x.From)
            .NotEqual(default(DateTime))
            .WithMessage("From date is required")
            .GreaterThan(DateTime.MinValue)
            .WithMessage("From date must be a valid date");

        RuleFor(x => x.To)
            .NotEqual(default(DateTime))
            .WithMessage("To date is required")
            .GreaterThan(DateTime.MinValue)
            .WithMessage("To date must be a valid date")
            .GreaterThanOrEqualTo(x => x.From)
            .WithMessage("To date must be greater than or equal to From date");
    }
}