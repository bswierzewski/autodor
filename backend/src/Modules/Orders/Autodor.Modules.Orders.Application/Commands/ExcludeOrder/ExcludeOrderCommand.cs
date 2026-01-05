using ErrorOr;
using FluentValidation;
using MediatR;

namespace Autodor.Modules.Orders.Application.Commands.ExcludeOrder;

public record ExcludeOrderCommand(string OrderId) : IRequest<ErrorOr<bool>>;

public class ExcludeOrderCommandValidator : AbstractValidator<ExcludeOrderCommand>
{
    public ExcludeOrderCommandValidator()
    {
        RuleFor(o => o.OrderId).NotEmpty()
            .WithMessage("OrderId is required for order exclusion operations");
    }
}