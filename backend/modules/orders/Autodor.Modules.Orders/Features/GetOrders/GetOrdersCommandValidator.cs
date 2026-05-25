using FluentValidation;

namespace Autodor.Modules.Orders.Features.GetOrders;

public class GetOrdersCommandValidator : AbstractValidator<GetOrdersCommand>
{
    public GetOrdersCommandValidator()
    {
        RuleFor(x => x.From)
            .NotEqual(default(DateTime))
            .WithMessage("Pole from jest wymagane")
            .GreaterThan(DateTime.MinValue)
            .WithMessage("Pole from musi zawierać prawidłową datę");

        RuleFor(x => x.To)
            .NotEqual(default(DateTime))
            .WithMessage("Pole to jest wymagane")
            .GreaterThan(DateTime.MinValue)
            .WithMessage("Pole to musi zawierać prawidłową datę")
            .GreaterThanOrEqualTo(x => x.From)
            .WithMessage("Pole to musi być większe lub równe polu from");
    }
}