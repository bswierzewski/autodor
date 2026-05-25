using FluentValidation;

namespace Autodor.Modules.Orders.Features.GetOrder;

public class GetOrderCommandValidator : AbstractValidator<GetOrderCommand>
{
    public GetOrderCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Pole id zamówienia jest wymagane");

        RuleFor(x => x.Date)
            .NotEqual(default(DateTime))
            .WithMessage("Pole date jest wymagane")
            .GreaterThan(DateTime.MinValue)
            .WithMessage("Pole date musi zawierać prawidłową datę");
    }
}