using FluentValidation;

namespace Autodor.Modules.Orders.Application.Commands.ExcludeOrder;

/// <summary>
/// Validates the ExcludeOrderCommand to ensure required data is provided.
/// </summary>
public class ExcludeOrderCommandValidator : AbstractValidator<ExcludeOrderCommand>
{
    /// <summary>
    /// Initializes a new instance of the ExcludeOrderCommandValidator with validation rules.
    /// </summary>
    public ExcludeOrderCommandValidator()
    {
        RuleFor(o => o.OrderId).NotEmpty()
            .WithMessage("OrderId is required for order exclusion operations");
    }
}