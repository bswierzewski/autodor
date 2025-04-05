namespace Application.Orders.Commands.ExcludeOrder;

public class ExcludeOrderCommandValidator : AbstractValidator<ExcludeOrderCommand>
{
    public ExcludeOrderCommandValidator()
    {
        RuleFor(o => o.OrderId).NotEmpty();
    }
}